// Copyright (C) 2026 FuseCP
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using FuseCP.EnterpriseServer.Data;
using FuseCP.EnterpriseServer.Data.Entities;
using FuseCP.Providers;
using Microsoft.Win32;

namespace FuseCP.Tools.ServerCredentialRecovery.Cli
{
	internal static class Program
	{
		private const string EnterpriseServerRegistryPath = "SOFTWARE\\FuseCP\\EnterpriseServer";

		private static int Main(string[] args)
		{
			try
			{
				var options = Options.Parse(args);
				if (options.ShowHelp)
				{
					PrintUsage();
					return 0;
				}

				options.Validate();

				var config = EnterpriseConfiguration.Load(options.ConfigPath);
				var dbType = DbSettings.GetDbType(config.ConnectionString);
				using var context = new FuseCP.EnterpriseServer.Data.DbContext(config.ConnectionString, dbType);

				var server = ResolveServer(context, options);
				if (server == null)
				{
					Console.Error.WriteLine("The requested server could not be found.");
					return 1;
				}

				var targetMode = ResolveTargetMode(server, options.Mode);
				var encryptedPassword = new Cryptor(config.CryptoKey, config.EncryptionEnabled).Encrypt(options.Password!);

				Console.WriteLine($"Config: {config.ConfigPath}");
				Console.WriteLine($"Server: {server.ServerId} ({server.ServerName})");
				Console.WriteLine($"URL: {server.ServerUrl}");
				Console.WriteLine($"Current mode: {(server.PasswordIsSHA256 ? "sha256" : "sha1")}");
				Console.WriteLine($"Target mode: {targetMode}");
				Console.WriteLine(options.DryRun ? "Dry run: yes" : "Dry run: no");

				if (options.DryRun)
				{
					Console.WriteLine("No changes were written.");
					return 0;
				}

				server.Password = encryptedPassword;
				server.PasswordIsSHA256 = string.Equals(targetMode, "sha256", StringComparison.OrdinalIgnoreCase);
				context.SaveChanges();

				Console.WriteLine("Enterprise-side server credential was updated successfully.");
				Console.WriteLine("Next step: validate the server from Portal and confirm the Password Lifecycle dashboard reports the expected auth posture.");
				return 0;
			}
			catch (ArgumentException ex)
			{
				Console.Error.WriteLine(ex.Message);
				Console.Error.WriteLine();
				PrintUsage();
				return 2;
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex);
				return 1;
			}
		}

		private static Server? ResolveServer(FuseCP.EnterpriseServer.Data.DbContext context, Options options)
		{
			if (options.ServerId.HasValue)
				return context.Servers.FirstOrDefault(server => server.ServerId == options.ServerId.Value);

			var serverName = options.ServerName!.Trim();
			var matches = context.Servers
				.Where(server => server.ServerName.ToUpper() == serverName.ToUpper())
				.Take(2)
				.ToList();

			if (matches.Count > 1)
				throw new ArgumentException("Multiple servers matched the supplied name. Use --server-id instead.");

			return matches.SingleOrDefault();
		}

		private static string ResolveTargetMode(Server server, string mode)
		{
			if (string.Equals(mode, "keep", StringComparison.OrdinalIgnoreCase))
				return server.PasswordIsSHA256 ? "sha256" : "sha1";

			return mode;
		}

		private static void PrintUsage()
		{
			Console.WriteLine("FuseCP Server Credential Recovery CLI");
			Console.WriteLine();
			Console.WriteLine("Usage:");
			Console.WriteLine("  FuseCP.ServerCredentialRecovery.Cli --server-id <id> --password <secret> [--mode keep|sha256|sha1] [--config <path>] [--dry-run]");
			Console.WriteLine("  FuseCP.ServerCredentialRecovery.Cli --server-name <name> --password <secret> [--mode keep|sha256|sha1] [--config <path>] [--dry-run]");
			Console.WriteLine();
			Console.WriteLine("Notes:");
			Console.WriteLine("  --mode keep preserves the current Enterprise-side PasswordIsSHA256 flag.");
			Console.WriteLine("  Use --mode sha256 after resetting a modern server with the installer/configuration tool.");
			Console.WriteLine("  The CLI updates only Enterprise-side credential storage. Reset the server-host password first.");
		}

		private sealed class Options
		{
			public bool ShowHelp { get; private set; }
			public string? ConfigPath { get; private set; }
			public int? ServerId { get; private set; }
			public string? ServerName { get; private set; }
			public string? Password { get; private set; }
			public string Mode { get; private set; } = "keep";
			public bool DryRun { get; private set; }

			public static Options Parse(IReadOnlyList<string> args)
			{
				var options = new Options();

				for (var index = 0; index < args.Count; index++)
				{
					var argument = args[index];
					switch (argument)
					{
						case "--help":
						case "-h":
						case "/?":
							options.ShowHelp = true;
							break;
						case "--config":
							options.ConfigPath = ReadValue(args, ref index, argument);
							break;
						case "--server-id":
							options.ServerId = int.Parse(ReadValue(args, ref index, argument));
							break;
						case "--server-name":
							options.ServerName = ReadValue(args, ref index, argument);
							break;
						case "--password":
							options.Password = ReadValue(args, ref index, argument);
							break;
						case "--mode":
							options.Mode = ReadValue(args, ref index, argument).Trim().ToLowerInvariant();
							break;
						case "--dry-run":
							options.DryRun = true;
							break;
						default:
							throw new ArgumentException($"Unknown argument: {argument}");
					}
				}

				return options;
			}

			public void Validate()
			{
				if (ShowHelp)
					return;

				if (ServerId.HasValue == !string.IsNullOrWhiteSpace(ServerName))
					throw new ArgumentException("Specify exactly one of --server-id or --server-name.");

				if (string.IsNullOrWhiteSpace(Password))
					throw new ArgumentException("--password is required.");

				if (Mode != "keep" && Mode != "sha256" && Mode != "sha1")
					throw new ArgumentException("--mode must be one of: keep, sha256, sha1.");
			}

			private static string ReadValue(IReadOnlyList<string> args, ref int index, string argument)
			{
				if (index + 1 >= args.Count)
					throw new ArgumentException($"Missing value for {argument}.");

				index++;
				return args[index];
			}
		}

		private sealed class EnterpriseConfiguration
		{
			public string ConfigPath { get; private set; } = string.Empty;
			public string ConnectionString { get; private set; } = string.Empty;
			public string CryptoKey { get; private set; } = string.Empty;
			public bool EncryptionEnabled { get; private set; } = true;

			public static EnterpriseConfiguration Load(string? inputPath)
			{
				var configPath = ResolveConfigPath(inputPath);
				var document = XDocument.Load(configPath, LoadOptions.PreserveWhitespace);
				var root = document.Root ?? throw new ArgumentException("The EnterpriseServer configuration file is empty.");

				var connectionString = ResolveConnectionString(root);
				var cryptoKey = ResolveCryptoKey(root);
				var encryptionEnabled = ResolveEncryptionEnabled(root);

				if (string.IsNullOrWhiteSpace(connectionString))
					throw new ArgumentException("The EnterpriseServer connection string could not be resolved from configuration.");

				if (string.IsNullOrWhiteSpace(cryptoKey))
					throw new ArgumentException("The EnterpriseServer crypto key could not be resolved from configuration.");

				return new EnterpriseConfiguration
				{
					ConfigPath = configPath,
					ConnectionString = NormalizeConnectionStringPath(connectionString, configPath),
					CryptoKey = cryptoKey,
					EncryptionEnabled = encryptionEnabled
				};
			}

			private static string ResolveConfigPath(string? inputPath)
			{
				if (!string.IsNullOrWhiteSpace(inputPath))
				{
					var explicitPath = Path.GetFullPath(inputPath);
					if (!File.Exists(explicitPath))
						throw new ArgumentException($"Config file was not found: {explicitPath}");
					return explicitPath;
				}

				var candidates = new[]
				{
					Path.Combine(Environment.CurrentDirectory, "Web.config"),
					Path.Combine(Environment.CurrentDirectory, "FuseCP.EnterpriseServer", "Web.config"),
					Path.Combine(Environment.CurrentDirectory, "FuseCP", "Sources", "FuseCP.EnterpriseServer", "Web.config")
				};

				var resolvedPath = candidates.FirstOrDefault(File.Exists);
				if (resolvedPath == null)
					throw new ArgumentException("No EnterpriseServer Web.config file was found. Supply it with --config.");

				return resolvedPath;
			}

			private static string ResolveConnectionString(XElement root)
			{
				var altConnectionKey = FindAppSetting(root, "FuseCP.AltConnectionString");
				var connectionString = FindConnectionString(root, "EnterpriseServer");
				var registryValue = ReadRegistryValue(altConnectionKey);

				return !string.IsNullOrWhiteSpace(registryValue) ? registryValue : connectionString;
			}

			private static string ResolveCryptoKey(XElement root)
			{
				var altCryptoKey = FindAppSetting(root, "FuseCP.AltCryptoKey");
				var cryptoKey = FindAppSetting(root, "FuseCP.CryptoKey");
				var registryValue = ReadRegistryValue(altCryptoKey);

				return !string.IsNullOrWhiteSpace(registryValue) ? registryValue : cryptoKey;
			}

			private static bool ResolveEncryptionEnabled(XElement root)
			{
				var rawValue = FindAppSetting(root, "FuseCP.EncryptionEnabled");
				return string.IsNullOrWhiteSpace(rawValue) || bool.Parse(rawValue);
			}

			private static string NormalizeConnectionStringPath(string connectionString, string configPath)
			{
				var dbType = DbSettings.GetDbType(connectionString);
				if (dbType != DbType.Sqlite && dbType != DbType.SqliteFX)
					return connectionString;

				var builder = new DbConnectionStringBuilder
				{
					ConnectionString = connectionString
				};

				if (!TryGetValue(builder, "Data Source", out var dataSource) && !TryGetValue(builder, "data source", out dataSource))
					return connectionString;

				var sqlitePath = Convert.ToString(dataSource);
				if (string.IsNullOrWhiteSpace(sqlitePath) || Path.IsPathRooted(sqlitePath))
					return connectionString;

				builder["Data Source"] = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(configPath) ?? Environment.CurrentDirectory, sqlitePath));
				return builder.ConnectionString;
			}

			private static string FindConnectionString(XElement root, string name)
			{
				return root.Element("connectionStrings")?
					.Elements("add")
					.FirstOrDefault(element => string.Equals((string?)element.Attribute("name"), name, StringComparison.OrdinalIgnoreCase))?
					.Attribute("connectionString")?
					.Value ?? string.Empty;
			}

			private static string FindAppSetting(XElement root, string key)
			{
				return root.Element("appSettings")?
					.Elements("add")
					.FirstOrDefault(element => string.Equals((string?)element.Attribute("key"), key, StringComparison.OrdinalIgnoreCase))?
					.Attribute("value")?
					.Value ?? string.Empty;
			}

			private static string ReadRegistryValue(string valueName)
			{
				if (string.IsNullOrWhiteSpace(valueName) || !OperatingSystem.IsWindows())
					return string.Empty;

				using var key = Registry.LocalMachine.OpenSubKey(EnterpriseServerRegistryPath);
				return Convert.ToString(key?.GetValue(valueName)) ?? string.Empty;
			}

			private static bool TryGetValue(DbConnectionStringBuilder builder, string key, out object? value)
			{
				if (builder.TryGetValue(key, out value))
					return true;

				value = null;
				return false;
			}
		}
	}
}