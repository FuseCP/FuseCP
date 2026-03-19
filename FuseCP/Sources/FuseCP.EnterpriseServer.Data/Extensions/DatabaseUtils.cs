// Copyright (C) 2025 FuseCP
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Text.RegularExpressions;
using System.Data;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using MySql.Data;
using MySql.Data.MySqlClient;
using FuseCP.Providers.Common;
using System.Text;
using Data = FuseCP.EnterpriseServer.Data;
#if NETFRAMEWORK
using System.Data.SQLite;
#else
using Microsoft.Data.Sqlite;
#endif

namespace FuseCP.EnterpriseServer.Data
{
	/// <summary>
	/// Sql utils class used by the FuseCP Installer.
	/// </summary>
	public sealed class DatabaseUtils
	{
		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		private DatabaseUtils() { }

		public static Stream InstallScriptStream(string scriptName)
		{
			scriptName = Regex.Replace(scriptName, @"(?<=^!.*\\\.)([0-9]+)(?=\\\.)", "_$1");
			var assembly = Assembly.GetExecutingAssembly();
			var resNames = assembly.GetManifestResourceNames();
			var names = resNames
				.OrderBy(name =>
				{
					var tokens = name.Split('.');
					var type = tokens.Length >= 2 ? tokens[tokens.Length - 2] : "";
					char[] typeid = new char[4];
					for (int i = 0; i < 4; i++)
					{
						if (i < type.Length) typeid[i] = type[i];
						else typeid[i] = '0';
					}
					var typecode = new string(typeid);
					var orderText = tokens.Length >= 3 ? tokens[tokens.Length - 3] : "";
					orderText = orderText.Trim('_');
					int order;
					if (!int.TryParse(orderText, out order)) order = short.MaxValue;

					// Put Views first, then UserDefinedFunctions and then StoredProcedures and then the rest
					string sortExpression;
					switch (type)
					{
						case "View": sortExpression = $"00000{order:x8}{name}"; break;
						case "StoredProcedure": sortExpression = $"30000{order:x8}{name}"; break;
						case "UserDefinedFunction": sortExpression = $"10000{order:x8}{name}"; break;
						case "Trigger": sortExpression = $"01000{order:x8}{name}"; break;
						default: sortExpression = $"2{typecode}.{order:x8}{string.Join(".", tokens.Take(tokens.Length - 2))}"; break;
					}

					return sortExpression;
				})
				.Where(name => {
					if (scriptName.StartsWith("!")) return Regex.IsMatch(name, scriptName.Substring(1));
					else return name.EndsWith(scriptName);
				})
				.ToList();

			//foreach (var name in names) Console.WriteLine($"Adding SQL script {name}");

			var streams = names
				.Select(name => assembly.GetManifestResourceStream(name))
				.ToList();

			if (streams.Count == 0) throw new FileNotFoundException("No SQL scripts found");
			else if (streams.Count == 1) return streams.FirstOrDefault();

			var length = streams
				.Sum(stream => stream.Length);
			var mem = new MemoryStream((int)length + 512);
			var writer = new StreamWriter(mem, Encoding.UTF8);
			foreach (var stream in streams)
			{
				var text = new StreamReader(stream).ReadToEnd();
				writer.WriteLine(text);
			}
			writer.Flush();
			mem.Position = 0;
			return mem;
		}
		public static string InstallScript(string scriptName)
		{
			using (var scriptStream = InstallScriptStream(scriptName))
			using (var reader = new StreamReader(scriptStream)) return reader.ReadToEnd();
		}

		public static string InstallScriptSqlServer() => InstallScript("install.sqlserver.sql");
		public static string InstallScriptMySql() => InstallScript("install.mysql.sql");
		public static string InstallScriptSqlite() => InstallScript("install.sqlite.sql");
		public static string InstallScriptPostgreSql() => InstallScript("install.postgresql.sql");
		public static string InstallScriptUpdateDb() => InstallScript("update_db.sql");
		public static Stream InstallScriptUpdateDbStream() => InstallScriptStream("update_db.sql");
		public static Stream InstallScriptStream(DbType type) => InstallScriptStream($"install.{type.ToString().ToLower()}.sql");
		public static string InstallScript(DbType type) => InstallScript($"install.{type.ToString().ToLower()}.sql");



		public static string BuildSqlServerMasterConnectionString(string dbServer, string dbLogin, string dbPassw)
		{
			return BuildSqlServerConnectionString(dbServer, "master", dbLogin, dbPassw);
		}

		public static string BuildMySqlMasterConnectionString(string dbServer, int port, string dbLogin, string dbPassw)
		{
			return BuildMySqlConnectionString(dbServer, port, dbLogin, dbPassw, "mysql");
		}

		public static string BuildSqlServerConnectionString(string dbServer, string dbName, string dbLogin, string dbPassw)
		{
			if (String.IsNullOrEmpty(dbLogin) && String.IsNullOrEmpty(dbPassw))
			{
				return String.Format("DbType=SqlServer;Server={0};Initial Catalog={1};Integrated Security=SSPI;TrustServerCertificate=true;", dbServer, dbName);
			}
			else
			{
				return String.Format("DbType=SqlServer;Server={0};Initial Catalog={1};User id={2};Password={3};TrustServerCertificate=true;", dbServer, dbName, dbLogin, dbPassw);
			}
		}

		public static string BuildMySqlConnectionString(string server, int port, string user, string password, string database)
		{
			return $"DbType=MySql;Server={server};Port={port};User={user};Password={password};Database={database}";
		}

		public static string BuildSqliteMasterConnectionString(string database, string installationFolder,
			string enterpriseServerPath = null, bool integrated = false)
		{
			if (!Path.IsPathRooted(database))
			{
				if (!database.Contains(Path.DirectorySeparatorChar.ToString()))
				{
					if (!database.Contains(".")) database = $"{database}.sqlite";
					if (integrated) database = Path.Combine(enterpriseServerPath, "App_Data", database);
					else database = Path.Combine("App_Data", database);
				}
				database = Path.GetFullPath(Path.Combine(installationFolder, database));
			}
			return $"DbType=Sqlite;Data Source={database}";
		}

		public static string BuildSqliteConnectionString(string database, string enterpriseServerPath = null, bool integrated = false)
		{
			if (!Path.IsPathRooted(database) && !database.Contains(Path.DirectorySeparatorChar.ToString()))
			{
				if (!database.Contains(".")) database = $"{database}.sqlite";
				//var integrated = vars.EmbedEnterpriseServer;
				if (integrated) database = Path.Combine(enterpriseServerPath, "App_Data", database);
				else database = Path.Combine("App_Data", database);
			}
			return $"DbType=Sqlite;Data Source={database}";
		}

		public static string BuildConnectionString(Data.DbType type, string server, int port, string database, string user, string password,
			string enterpriseServerPath = null, bool integrated = false)
		{
			switch (type)
			{
				case Data.DbType.SqlServer:
					return BuildSqlServerConnectionString(server, database, user, password);
				case Data.DbType.MariaDb:
				case Data.DbType.MySql:
					return BuildMySqlConnectionString(server, port, user, password, database);
				case Data.DbType.Sqlite:
				case Data.DbType.SqliteFX:
					return BuildSqliteConnectionString(database, enterpriseServerPath, integrated);
				default: throw new NotSupportedException($"DbType {type} not supported.");
			}
		}

		public static bool CheckSqlConnection(string connectionString)
		{
			Data.DbType dbtype;
			ParseConnectionString(connectionString, out dbtype, out connectionString);
			switch (dbtype)
			{
				case Data.DbType.SqlServer: return CheckSqlServerConnection(connectionString);
				case Data.DbType.MySql:
				case Data.DbType.MariaDb: return CheckMySqlConnection(connectionString);
				case Data.DbType.Sqlite:
				case Data.DbType.SqliteFX: return CheckSqliteConnection(connectionString);
				default: return false;
			}
		}

		/// <summary>
		/// Check sql connection.
		/// </summary>
		/// <param name="connectionString">Connection string.</param>
		/// <returns>True if connecion is valid, otherwise false.</returns>
		public static bool CheckSqlServerConnection(string connectionString)
		{
			SqlConnection conn = new SqlConnection(connectionString);
			try
			{
				conn.Open();
			}
			catch
			{
				return false;
			}

			conn.Close();
			return true;
		}

		public static bool CheckMySqlConnection(string connectionString)
		{
			var csb = new ConnectionStringBuilder(connectionString);
			csb["database"] = "mysql";
			connectionString = csb.ToString();
			MySqlConnection conn = new MySqlConnection(connectionString);
			try
			{
				conn.Open();
			}
			catch
			{
				return false;
			}

			conn.Close();
			return true;
		}

		public static bool CheckSqliteConnection(string connectionString)
		{
			/*var csb = new ConnectionStringBuilder(connectionString);
			var dbFile = (string)(csb["data source"] ?? "");
			var assemblyPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
			if (!Path.IsPathRooted(dbFile)) dbFile = Path.GetFullPath(Path.Combine(assemblyPath, "..", dbFile));
			var dir = new DirectoryInfo(Path.GetDirectoryName(dbFile)).Exists;*/
			return true;
		}
		/// <summary>
		/// Gets the version of SQL Server instance.
		/// </summary>
		/// <param name="connectionString">Connection string.</param>
		/// <returns>True if connecion is valid, otherwise false.</returns>
		public static string GetSqlServerVersion(string connectionString)
		{
			DbType dbtype;
			string nativeConnectionString;
			ParseConnectionString(connectionString, out dbtype, out nativeConnectionString);
			if (dbtype == DbType.SqlServer || dbtype == DbType.Unknown)
			{
				SqlConnection conn = new SqlConnection(nativeConnectionString);
				try
				{
					SqlCommand cmd = new SqlCommand("SELECT SERVERPROPERTY('productversion')", conn);
					conn.Open();
					SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
					string version = "unknown";
					if (reader.HasRows)
					{
						reader.Read();
						version = reader[0].ToString();
						reader.Close();
					}
					return version;
				}
				finally
				{
					if (conn != null && conn.State == ConnectionState.Open)
						conn.Close();
				}
			}
			else return string.Empty;
		}

		/// <summary>
		/// Gets the security mode of SQL Server.
		/// </summary>
		/// <param name="connectionString">Connection string.</param>
		/// <returns>1 - Windows Authentication mode, 0 - Mixed mode.</returns>
		public static int GetSqlServerSecurityMode(string connectionString)
		{
			DbType dbtype;
			string nativeConnectionString;
			ParseConnectionString(connectionString, out dbtype, out nativeConnectionString);
			if (dbtype == DbType.SqlServer || dbtype == DbType.Unknown)
			{

				SqlConnection conn = new SqlConnection(nativeConnectionString);
				int mode = 0;
				try
				{
					SqlCommand cmd = new SqlCommand("SELECT SERVERPROPERTY('IsIntegratedSecurityOnly')", conn);
					conn.Open();
					SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

					if (reader.HasRows)
					{
						reader.Read();
						mode = Convert.ToInt32(reader[0]);
						reader.Close();
					}
				}
				catch
				{
				}
				finally
				{
					if (conn != null && conn.State == ConnectionState.Open)
						conn.Close();
				}
				return mode;
			}

			throw new ArgumentException("Invalid connection string");
		}

		/// <summary>
		/// Get databases.
		/// </summary>
		/// <param name="connectionString">Connection string.</param>
		/// <returns>Databases.</returns>
		public static string[] GetDatabases(string connectionString)
		{
			DataSet ds = ExecuteQuery(connectionString, "SELECT Name FROM master..sysdatabases ORDER BY Name");
			string[] ret = new string[ds.Tables[0].Rows.Count];
			for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
			{
				ret[i] = (string)ds.Tables[0].Rows[i]["Name"];
			}
			return ret;
		}

		public static int ExecuteStoredProcedure(string connectionString, string name, params SqlParameter[] commandParameters)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = name;
				cmd.Connection = connection;
				cmd.CommandTimeout = 300;
				//attach parameters
				if (commandParameters != null)
				{
					foreach (SqlParameter p in commandParameters)
					{
						if (p != null)
						{
							// Check for derived output value with no value assigned
							if ((p.Direction == ParameterDirection.InputOutput ||
								p.Direction == ParameterDirection.Input) &&
								(p.Value == null))
							{
								p.Value = DBNull.Value;
							}
							cmd.Parameters.Add(p);
						}
					}
				}
				int ret = cmd.ExecuteNonQuery();
				cmd.Parameters.Clear();
				connection.Close();
				return ret;
			}
		}

		private static int ExecuteNonQuery(string connectionString, string commandText)
		{
			Data.DbType dbType;
			string nativeConnectionString;
			ParseConnectionString(connectionString, out dbType, out nativeConnectionString);
			switch (dbType)
			{
				case Data.DbType.SqlServer: return ExecuteNonQuerySqlServer(nativeConnectionString, commandText);
				case Data.DbType.MySql:
				case Data.DbType.MariaDb: return ExecuteNonQueryMySql(nativeConnectionString, commandText);
				case Data.DbType.Sqlite:
				case Data.DbType.SqliteFX: return ExecuteNonQuerySqlite(nativeConnectionString, commandText);
				default: return 0;
			}
		}
		private static int ExecuteNonQuerySqlServer(string connectionString, string commandText)
		{
			SqlConnection conn = null;
			try
			{
				conn = new SqlConnection(connectionString);
				SqlCommand cmd = new SqlCommand(commandText, conn);
				cmd.CommandTimeout = 300;
				conn.Open();
				int ret = cmd.ExecuteNonQuery();
				return ret;
			}
			finally
			{
				// close connection if required
				if (conn != null && conn.State == ConnectionState.Open)
					conn.Close();
			}
		}

		private static object ExecuteScalarSqlServer(string connectionString, string commandText)
		{
			SqlConnection conn = null;
			try
			{
				conn = new SqlConnection(connectionString);
				SqlCommand cmd = new SqlCommand(commandText, conn);
				cmd.CommandTimeout = 300;
				conn.Open();
				object ret = cmd.ExecuteScalar();
				return ret;
			}
			finally
			{
				// close connection if required
				if (conn != null && conn.State == ConnectionState.Open)
					conn.Close();
			}
		}

		private static int ExecuteNonQueryMySql(string connectionString, string commandText)
		{
			MySqlConnection conn = null;
			try
			{
				conn = new MySqlConnection(connectionString);
				MySqlCommand cmd = new MySqlCommand(commandText, conn);
				cmd.CommandTimeout = 300;
				conn.Open();
				int ret = cmd.ExecuteNonQuery();
				return ret;
			}
			finally
			{
				// close connection if required
				if (conn != null && conn.State == ConnectionState.Open)
					conn.Close();
			}
		}

		private static int ExecuteNonQuerySqlite(string connectionString, string commandText)
		{
			System.Data.Common.DbConnection conn = null;
			try
			{
				conn = SqliteConnection(connectionString);
				System.Data.Common.DbCommand cmd = SqliteCommand();
				cmd.CommandText = commandText;
				cmd.Connection = conn;
				cmd.CommandTimeout = 300;
				conn.Open();
				int ret = cmd.ExecuteNonQuery();
				return ret;
			}
			finally
			{
				// close connection if required
				if (conn != null && conn.State == ConnectionState.Open)
					conn.Close();
			}
		}

		public static void ParseConnectionString(string connectionString, out Data.DbType dbType, out string nativeConnectionString)
		{
			var csb = new ConnectionStringBuilder(connectionString);
			var dbTypeName = (string)(csb["dbtype"] ?? "");
			csb.Remove("dbtype");
			Enum.TryParse(dbTypeName, out dbType);
			nativeConnectionString = csb.ToString();
		}
		public static DataSet ExecuteQuery(string connectionString, string commandText)
		{
			Data.DbType dbType;
			string nativeConnectionString;
			ParseConnectionString(connectionString, out dbType, out nativeConnectionString);
			switch (dbType)
			{
				case Data.DbType.SqlServer:
					return ExecuteQuerySqlServer(nativeConnectionString, commandText);
				case Data.DbType.MySql:
				case Data.DbType.MariaDb:
					return ExecuteQueryMySql(nativeConnectionString, commandText);
				case Data.DbType.Sqlite:
				case Data.DbType.SqliteFX:
					return ExecuteQuerySqlite(nativeConnectionString, commandText);
				default: return null;
			}
		}
		public static DataSet ExecuteQuerySqlServer(string connectionString, string commandText)
		{
			SqlConnection conn = null;
			try
			{
				conn = new SqlConnection(connectionString);
				conn.Open();
				SqlDataAdapter adapter = new SqlDataAdapter(commandText, conn);
				DataSet ds = new DataSet();
				adapter.Fill(ds);
				return ds;
			}
			finally
			{
				// close connection if required
				if (conn != null && conn.State == ConnectionState.Open)
					conn.Close();
			}
		}

		public static DataSet ExecuteQueryMySql(string connectionString, string commandText)
		{
			MySqlConnection conn = null;
			try
			{
				conn = new MySqlConnection(connectionString);
				MySqlDataAdapter adapter = new MySqlDataAdapter(commandText, conn);
				DataSet ds = new DataSet();
				conn.Open();
				adapter.Fill(ds);
				return ds;
			}
			finally
			{
				// close connection if required
				if (conn != null && conn.State == ConnectionState.Open)
					conn.Close();
			}
		}

		public static DataSet ExecuteQuerySqlite(string connectionString, string commandText)
		{
			System.Data.Common.DbConnection conn = null;
			try
			{
				conn = SqliteConnection(connectionString);
				DataSet ds = new DataSet();
				var cmd = SqliteCommand();
				cmd.CommandText = commandText;
				cmd.Connection = conn;
				conn.Open();

				using (var dr = cmd.ExecuteReader())
				{
					do
					{
						var dt = new DataTable();
						dt.BeginLoadData();
						dt.Load(dr);
						dt.EndLoadData();
						ds.Tables.Add(dt);

					} while (!dr.IsClosed && dr.NextResult());
				}
				return ds;
			}
			finally
			{
				// close connection if required
				if (conn != null && conn.State == ConnectionState.Open)
					conn.Close();
			}
		}

		public static string ChangeDb(string connectionString, string database)
		{
			var csb = new ConnectionStringBuilder(connectionString);
			csb["database"] = database;
			return csb.ToString();
		}
		public static DataSet ExecuteSql(string Conn, string Query)
		{
			return ExecuteQuery(Conn, Query);
		}

		/// <summary>
		/// Checks if the database exists.
		/// </summary>
		/// <param name="databaseName">Database name.</param>
		/// <param name="connectionString">Connection string.</param>
		/// <returns>Returns True if the database exists.</returns>
		public static bool DatabaseExists(string connectionString, string databaseName, string installationFolder = null)
		{
			Data.DbType dbType;
			string nativeConnectionString;
			ParseConnectionString(connectionString, out dbType, out nativeConnectionString);
			switch (dbType)
			{
				case Data.DbType.SqlServer:
					return (ExecuteQuery(connectionString,
						String.Format("select name from master..sysdatabases where name = '{0}'", databaseName)).Tables[0].Rows.Count > 0);
				case Data.DbType.MySql:
				case Data.DbType.MariaDb:
					return (ExecuteQuery(connectionString,
						$"SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '{databaseName}'").Tables[0].Rows.Count > 0);
				case Data.DbType.Sqlite:
				case Data.DbType.SqliteFX:
					var csb = new ConnectionStringBuilder(connectionString);
					var dbFile = (string)(csb["data source"] ?? "");
					var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
					if (!Path.IsPathRooted(dbFile))
					{
						if (!string.IsNullOrEmpty(installationFolder)) dbFile = Path.Combine(installationFolder, dbFile);
						dbFile = Path.GetFullPath(dbFile);
					}
					return File.Exists(dbFile);
				default: return false;
			}
		}
		public static bool IsEmptyDatabase(string ConnStr, string DbName)
		{
			Data.DbType dbType;
			int Tmp;
			ParseConnectionString(ConnStr, out dbType, out ConnStr);
			switch (dbType)
			{
				case Data.DbType.SqlServer:
					Tmp = (int)ExecuteSql(ConnStr, string.Format("use [{0}]; select case when exists(select * from information_schema.tables) then 1 else 0 end;", DbName)).Tables[0].Rows[0][0];
					return Tmp == 0;
				case Data.DbType.MySql:
				case Data.DbType.MariaDb:
					Tmp = (int)ExecuteSql(ConnStr, $"SELECT COUNT(DISTINCT `TABLE_NAME`) AS anyAliasName FROM `INFORMATION_SCHEMA`. `COLUMNS` WHERE `table_schema` = '{DbName}';").Tables[0].Rows[0][0];
					return Tmp == 0;
				case Data.DbType.Sqlite:
				case Data.DbType.SqliteFX:
					var csb = new ConnectionStringBuilder(ConnStr);
					var dbFile = (string)(csb["data source"] ?? "");
					var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
					if (!Path.IsPathRooted(dbFile)) dbFile = Path.GetFullPath(Path.Combine(assemblyPath, "..", dbFile));
					csb["data source"] = dbFile;
					ConnStr = csb.ToString();
					Tmp = (int)ExecuteSql(ConnStr, $"SELECT COUNT(name) FROM sqlite_master").Tables[0].Rows[0][0];
					return Tmp == 0;
				default: return false;
			}
		}

		/// <summary>
		/// Creates database.
		/// </summary>
		/// <param name="connectionString">Connection string.</param>
		/// <param name="databaseName">Database name.</param>
		public static void CreateDatabase(string connectionString, string databaseName, string installationFolder = null)
		{
			Data.DbType dbType;
			string ConnStr, commandText;
			ParseConnectionString(connectionString, out dbType, out ConnStr);
			switch (dbType)
			{
				case Data.DbType.SqlServer:
					// create database in the default location
					commandText = string.Format("CREATE DATABASE [{0}] COLLATE Latin1_General_CI_AS;", databaseName);
					// create database
					ExecuteNonQuery(connectionString, commandText);
					// grant users access
					//UpdateDatabaseUsers(database.Name, database.Users);
					break;
				case Data.DbType.MySql:
				case Data.DbType.MariaDb:
					// create database in the default location
					commandText = $"CREATE DATABASE {databaseName};";
					// create database
					ExecuteNonQuery(connectionString, commandText);
					break;
				case Data.DbType.Sqlite:
				case Data.DbType.SqliteFX:
					var csb = new ConnectionStringBuilder(ConnStr);
					var dbFile = (string)(csb["data source"] ?? "");
					var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
					if (!Path.IsPathRooted(dbFile))
					{
						if (!string.IsNullOrEmpty(installationFolder)) dbFile = Path.Combine(installationFolder, dbFile);
						dbFile = Path.GetFullPath(dbFile);
					}
					CreateDatabaseSqlite(dbFile);
					break;
				default: break;
			}
		}

		public static void CreateDatabaseLocalDb(string connectionString, string database, string file)
		{
			Data.DbType dbType;
			string ConnStr;
			var guid = Guid.NewGuid();
			ParseConnectionString(connectionString, out dbType, out ConnStr);
			ExecuteNonQuerySqlServer(ConnStr, @$"CREATE DATABASE [{database}]
ON PRIMARY(
	NAME = '{guid}',
	FILENAME = '{file}'
)
LOG ON(
	NAME = '{guid}_log',
	FILENAME = '{Path.ChangeExtension(file, ".ldf")}'
)");
		}
		public static void CreateDatabaseSqlite(string dbFile)
		{
			var path = Path.GetDirectoryName(dbFile);
			if (!Directory.Exists(path)) Directory.CreateDirectory(path);
#if NETFRAMEWORK
			SQLiteConnection.CreateFile(dbFile);
#else
			//File.Create(dbFile);
#endif

		}
		/// <summary>
		/// Creates database user.
		/// </summary>
		/// <param name="connectionString">Connection string.</param>
		/// <param name="userName">User name</param>
		/// <param name="password">Password.</param>
		/// <param name="database">Default database.</param>
		public static bool CreateUser(string connectionString, string userName, string password, string database)
		{
			Data.DbType dbType;
			string ConnStr, server;
			var csb = new ConnectionStringBuilder(connectionString);
			server = csb["server"] as string;
			ParseConnectionString(connectionString, out dbType, out ConnStr);
			bool userCreated = false;
			switch (dbType)
			{
				case Data.DbType.SqlServer:
					if (!UserExists(connectionString, userName))
					{
						ExecuteNonQuery(connectionString,
							String.Format("CREATE LOGIN {0} WITH PASSWORD='{1}', DEFAULT_DATABASE={2}, CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF",
							userName, password, database));
						userCreated = true;
					}
					AddUserToDatabase(connectionString, database, userName);
					return userCreated;
				case Data.DbType.MySql:
				case Data.DbType.MariaDb:
					if (!UserExists(connectionString, userName))
					{
						// create user
						var host = server == "localhost" ? server : Environment.MachineName;
						ExecuteNonQuery(connectionString, $"CREATE USER '{userName}'@'{host}' IDENTIFIED BY '{password}';");
						ExecuteNonQuery(connectionString, $"CREATE USER '{userName}'@'%' IDENTIFIED BY '{password}';");
						userCreated = true;
					}
					AddUserToDatabase(connectionString, database, userName);
					return userCreated;
				case Data.DbType.Sqlite:
				case Data.DbType.SqliteFX: return true;
				default: return false;
			}
		}

		private static void AddUserToDatabase(string connectionString, string databaseName, string user)
		{
			Data.DbType dbType;
			string ConnStr, server;
			var csb = new ConnectionStringBuilder(connectionString);
			server = csb["server"] as string;
			ParseConnectionString(connectionString, out dbType, out ConnStr);
			switch (dbType)
			{
				case Data.DbType.SqlServer:
					if (user == "sa")
						return;

					// grant database access
					try
					{
						ExecuteNonQuery(connectionString,
							string.Format("USE {0};EXEC sp_grantdbaccess '{1}';", databaseName, user));
					}
					catch (SqlException ex)
					{
						if (ex.Number == 15023)
						{
							// the user already exists in the database
							// so, try to auto fix his login in the database
							ExecuteNonQuery(connectionString,
								string.Format("USE {0};EXEC sp_change_users_login 'Auto_Fix', '{1}';", databaseName, user));
						}
						else
						{
							throw new Exception("Can't add user to database", ex);
						}
					}

					// add database owner
					ExecuteNonQuery(connectionString,
						string.Format("USE {0};EXEC sp_addrolemember 'db_owner', '{1}';", databaseName, user));
					break;
				case Data.DbType.MySql:
				case Data.DbType.MariaDb:
					var host = server == "localhost" ? server : Environment.MachineName;
					ExecuteNonQuery(connectionString, $"GRANT ALL ON {databaseName}.* TO '{user}'@'{host}';");
					ExecuteNonQuery(connectionString, $"GRANT ALL ON {databaseName}.* TO '{user}'@'%';");
					ExecuteNonQuery(connectionString, "FLUSH PRIVILEGES;");
					break;
				case Data.DbType.Sqlite:
				case Data.DbType.SqliteFX:
					break;
				default: break;
			}
		}

		/// <summary>
		/// Checks whether specified user exists.
		/// </summary>
		/// <param name="connectionString">Connection string</param>
		/// <param name="username">User name.</param>
		/// <returns>True if specified user exists, otherwise false.</returns>
		public static bool UserExists(string connectionString, string username)
		{
			Data.DbType dbType;
			string ConnStr, server;
			var csb = new ConnectionStringBuilder(connectionString);
			server = csb["server"] as string;
			ParseConnectionString(connectionString, out dbType, out ConnStr);
			switch (dbType)
			{
				case Data.DbType.SqlServer:
					return (ExecuteQuery(connectionString,
						string.Format("select name from master..syslogins where name = '{0}'", username)).Tables[0].Rows.Count > 0);
				case Data.DbType.MySql:
				case Data.DbType.MariaDb:
					var host = server == "localhost" ? server : Environment.MachineName;
					return 1 == (long)ExecuteQuery(connectionString, $"SELECT EXISTS(SELECT 1 FROM mysql.user WHERE user = '{username}' AND host = '{host}')").Tables[0].Rows[0][0];
				case Data.DbType.Sqlite:
				case Data.DbType.SqliteFX: return true;
				default: return false;
			}
		}

		/// <summary>
		/// Checks whether specified login exists.
		/// </summary>
		/// <param name="connectionString">Connection string</param>
		/// <param name="loginName">Login name.</param>
		/// <returns>True if specified login exists, otherwise false.</returns>
		public static bool LoginExists(string connectionString, string loginName)
		{
			Data.DbType dbType;
			string ConnStr, server;
			var csb = new ConnectionStringBuilder(connectionString);
			server = csb["server"] as string;
			ParseConnectionString(connectionString, out dbType, out ConnStr);
			switch (dbType)
			{
				case Data.DbType.SqlServer:
					return (ExecuteQuery(connectionString,
						string.Format("SELECT * FROM sys.server_principals WHERE name = N'{0}'", loginName)).Tables[0].Rows.Count > 0);
				default: return UserExists(connectionString, loginName);
			}
		}

		/// <summary>
		/// Deletes login
		/// </summary>
		/// <param name="connectionString">Connection string</param>
		/// <param name="loginName">Login name</param>
		public static void DeleteLogin(string connectionString, string loginName)
		{
			Data.DbType dbType;
			string ConnStr, server;
			var csb = new ConnectionStringBuilder(connectionString);
			server = csb["server"] as string;
			ParseConnectionString(connectionString, out dbType, out ConnStr);
			switch (dbType)
			{
				case Data.DbType.SqlServer:
					// drop login
					if (LoginExists(connectionString, loginName))
						ExecuteNonQuery(connectionString, String.Format("DROP LOGIN [{0}]", loginName));
					break;
				default: DeleteUser(connectionString, loginName); break;
			}
		}

		/// <summary>
		/// Deletes database user
		/// </summary>
		/// <param name="connectionString">Connection string</param>
		/// <param name="username">Username</param>
		public static void DeleteUser(string connectionString, string username)
		{
			Data.DbType dbType;
			string ConnStr, server;
			var csb = new ConnectionStringBuilder(connectionString);
			server = csb["server"] as string;
			ParseConnectionString(connectionString, out dbType, out ConnStr);
			switch (dbType)
			{
				case Data.DbType.SqlServer:
					// remove user from databases
					string[] userDatabases = GetUserDatabases(connectionString, username);
					foreach (string database in userDatabases)
						RemoveUserFromDatabase(connectionString, database, username);

					// close all user connection
					CloseUserConnections(connectionString, username);

					// drop login
					ExecuteNonQuery(connectionString, String.Format("EXEC sp_droplogin '{0}'", username));
					break;
				case Data.DbType.MySql:
				case Data.DbType.MariaDb:
					var host = server == "localhost" ? server : Environment.MachineName;
					ExecuteNonQuery(connectionString, $"DROP USER '{username}'@'{host}';");
					ExecuteNonQuery(connectionString, $"DROP USER '{username}'@'%';");
					break;
				default: break;
			}
		}

		/// <summary>
		/// Deletes database
		/// </summary>
		/// <param name="connectionString">Connection string</param>
		/// <param name="databaseName">Database name</param>
		public static void DeleteDatabase(string connectionString, string databaseName, string installPath = null)
		{
			Data.DbType dbType;
			string ConnStr;
			ParseConnectionString(connectionString, out dbType, out ConnStr);
			var csb = new ConnectionStringBuilder(connectionString);
			switch (dbType)
			{
				case Data.DbType.SqlServer:
					// remove all users from database
					string[] users = GetDatabaseUsers(connectionString, databaseName);
					foreach (string user in users)
					{
						RemoveUserFromDatabase(connectionString, databaseName, user);
					}

					// close all connection
					CloseDatabaseConnections(connectionString, databaseName);

					// drop database
					ExecuteNonQuery(connectionString,
						@$"ALTER DATABASE [{databaseName}] SET OFFLINE;DROP DATABASE [{databaseName}]");

					var server = csb.ContainsKey("server") ? csb["server"] as string :
						csb.ContainsKey("data source") ? csb["data source"] as string : null;
					var localDb = server.ToLower().Contains("localdb");

					if (localDb)
					{
						var profile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
						var dbfile = Path.Combine(profile, $"{databaseName}.mdf");
						var logfile = Path.Combine(profile, $"{databaseName}_log.ldf");
						if (File.Exists(dbfile)) File.Delete(dbfile);
						if (File.Exists(logfile)) File.Delete(logfile);
					}
					break;
				case Data.DbType.MySql:
				case Data.DbType.MariaDb:
					// drop database
					ExecuteNonQuery(connectionString,
						String.Format("DROP DATABASE {0}", databaseName));
					break;
				case Data.DbType.Sqlite:
				case Data.DbType.SqliteFX:
#if NETFRAMEWORK
					SQLiteConnection.ClearAllPools();
#else
					Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
#endif
					var dbFile = (string)(csb["data source"] ?? "");
					if (!Path.IsPathRooted(dbFile))
					{
						if (installPath == null) installPath = AppDomain.CurrentDomain.BaseDirectory;
						dbFile = Path.GetFullPath(Path.Combine(installPath, dbFile));
					}
					File.Delete(dbFile);
					break;
				default: break;
			}
		}

		private static string[] GetDatabaseUsers(string connectionString, string databaseName)
		{
			Data.DbType dbType;
			string ConnStr;
			ParseConnectionString(connectionString, out dbType, out ConnStr);

			if (dbType == Data.DbType.SqlServer)
			{
				string cmdText = String.Format(@"
				select su.name FROM {0}..sysusers as su
				inner JOIN master..syslogins as sl on su.sid = sl.sid
				where su.hasdbaccess = 1 AND su.islogin = 1 AND su.issqluser = 1 AND su.name <> 'dbo'",
					databaseName);
				DataView dvUsers = ExecuteQuery(connectionString, cmdText).Tables[0].DefaultView;

				string[] users = new string[dvUsers.Count];
				for (int i = 0; i < dvUsers.Count; i++)
				{
					users[i] = (string)dvUsers[i]["Name"];
				}
				return users;
			}
			return new string[0];
		}

		private static string[] GetUserDatabaseObjects(string connectionString, string databaseName, string user)
		{
			Data.DbType dbType;
			string ConnStr;
			ParseConnectionString(connectionString, out dbType, out ConnStr);

			if (dbType == Data.DbType.SqlServer)
			{
				DataView dvObjects = ExecuteQuery(connectionString,
				String.Format("select so.name from {0}..sysobjects as so" +
				" inner join {1}..sysusers as su on so.uid = su.uid" +
				" where su.name = '{2}'", databaseName, databaseName, user)).Tables[0].DefaultView;
				string[] objects = new string[dvObjects.Count];
				for (int i = 0; i < dvObjects.Count; i++)
				{
					objects[i] = (string)dvObjects[i]["Name"];
				}
				return objects;
			}
			return new string[0];
		}

		private static string[] GetUserDatabases(string connectionString, string username)
		{
			Data.DbType dbType;
			string ConnStr;
			ParseConnectionString(connectionString, out dbType, out ConnStr);

			if (dbType == Data.DbType.SqlServer)
			{
				string cmdText = String.Format(@"
					DECLARE @Username varchar(100)
					SET @Username = '{0}'

					CREATE TABLE #UserDatabases
					(
						Name nvarchar(100) collate database_default
					)

					DECLARE @DbName nvarchar(100)
					DECLARE DatabasesCursor CURSOR FOR
					SELECT name FROM master..sysdatabases
					WHERE (status & 256) = 0 AND (status & 512) = 0

					OPEN DatabasesCursor

					WHILE (10 = 10)
					BEGIN    --LOOP 10: thru Databases
						FETCH NEXT FROM DatabasesCursor
						INTO @DbName

					--print @DbName

						IF (@@fetch_status <> 0)
						BEGIN
							DEALLOCATE DatabasesCursor
							BREAK
						END

						DECLARE @sql nvarchar(1000)
                SET @sql = 'if exists (select ''' + @DbName + ''' from [' + @DbName + ']..sysusers where name = ''' + @Username + ''') insert into #UserDatabases (Name) values (''' + @DbName + ''')'

						EXECUTE(@sql)

					END

					SELECT Name FROM #UserDatabases

					DROP TABLE #UserDatabases
					", username);
				DataView dvDatabases = ExecuteQuery(connectionString, cmdText).Tables[0].DefaultView;

				string[] databases = new string[dvDatabases.Count];
				for (int i = 0; i < dvDatabases.Count; i++)
					databases[i] = (string)dvDatabases[i]["Name"];
				return databases;
			}
			return new string[0];
		}

		private static void CloseDatabaseConnections(string connectionString, string databaseName)
		{
			Data.DbType dbType;
			string ConnStr;
			ParseConnectionString(connectionString, out dbType, out ConnStr);

			if (dbType == Data.DbType.SqlServer)
			{
				DataView dv = ExecuteQuery(connectionString,
					String.Format(@"SELECT spid FROM master..sysprocesses WHERE dbid = DB_ID('{0}') AND spid > 50 AND spid <> @@spid", databaseName)).Tables[0].DefaultView;

				// kill processes
				for (int i = 0; i < dv.Count; i++)
				{
					KillProcess(connectionString, (short)(dv[i]["spid"]));
				}
			}
		}

		private static void CloseUserConnections(string connectionString, string userName)
		{
			Data.DbType dbType;
			string ConnStr;
			ParseConnectionString(connectionString, out dbType, out ConnStr);

			if (dbType == Data.DbType.SqlServer)
			{
				DataView dv = ExecuteQuery(connectionString,
					String.Format(@"SELECT spid FROM master..sysprocesses WHERE loginame = '{0}'", userName)).Tables[0].DefaultView;

				// kill processes
				for (int i = 0; i < dv.Count; i++)
				{
					KillProcess(connectionString, (short)(dv[i]["spid"]));
				}
			}
		}

		private static void KillProcess(string connectionString, short spid)
		{
			Data.DbType dbType;
			string ConnStr;
			ParseConnectionString(connectionString, out dbType, out ConnStr);

			if (dbType == Data.DbType.SqlServer)
			{
				try
				{
					ExecuteNonQuery(connectionString,
						String.Format("KILL {0}", spid));
				}
				catch (SqlException)
				{
				}
			}
		}

		private static void RemoveUserFromDatabase(string connectionString, string databaseName, string user)
		{
			Data.DbType dbType;
			string ConnStr;
			ParseConnectionString(connectionString, out dbType, out ConnStr);

			if (dbType == Data.DbType.SqlServer)
			{
				// change ownership of user's objects
				string[] userObjects = GetUserDatabaseObjects(connectionString, databaseName, user);
				foreach (string userObject in userObjects)
				{
					try
					{
						ExecuteNonQuery(connectionString,
							String.Format("USE {0};EXEC sp_changeobjectowner '{1}.{2}', 'dbo'",
							databaseName, user, userObject));
					}
					catch (SqlException ex)
					{
						if (ex.Number == 15505)
						{
							// Cannot change owner of object 'user.ObjectName' or one of its child objects because
							// the new owner 'dbo' already has an object with the same name.

							// try to rename object before changing owner
							string renamedObject = user + DateTime.Now.Ticks + "_" + userObject;
							ExecuteNonQuery(connectionString,
								String.Format("USE {0};EXEC sp_rename '{1}.{2}', '{3}'",
								databaseName, user, userObject, renamedObject));

							// change owner
							ExecuteNonQuery(connectionString,
								String.Format("USE {0};EXEC sp_changeobjectowner '{1}.{2}', 'dbo'",
								databaseName, user, renamedObject));
						}
						else
						{
							throw new Exception("Can't change database object owner", ex);
						}
					}
				}

				// revoke db access
				ExecuteNonQuery(connectionString,
					String.Format("USE {0};EXEC sp_revokedbaccess '{1}';",
					databaseName, user));
			}
		}

		public static bool IsValidDatabaseName(string name)
		{
			if (name == null || name.Trim().Length == 0 || name.Length > 128)
				return false;

			return Regex.IsMatch(name, "^[0-9A-z_@#$]+$", RegexOptions.Singleline);
		}

		public static string BackupDatabase(string connectionString, string databaseName)
		{
			Data.DbType dbType;
			string ConnStr;
			ParseConnectionString(connectionString, out dbType, out ConnStr);
			if (dbType == Data.DbType.SqlServer)
			{
				string bakFile = databaseName + ".bak";
				// backup database
				ExecuteNonQuery(connectionString,
					String.Format(@"BACKUP DATABASE [{0}] TO DISK = N'{1}'", // WITH INIT, NAME = '{2}'
					databaseName, bakFile));
				return bakFile;
			}
			return null;
		}

		public static void BackupDatabase(string connectionString, string databaseName, out string bakFile, out string position)
		{
			Data.DbType dbType;
			string ConnStr;
			ParseConnectionString(connectionString, out dbType, out ConnStr);
			bakFile = databaseName + ".bak";
			position = "1";
			if (dbType == Data.DbType.SqlServer)
			{
				string backupName = "Backup " + DateTime.Now.ToString("yyyyMMddHHmmss");
				// backup database
				ExecuteNonQuery(connectionString,
					String.Format(@"BACKUP DATABASE [{0}] TO DISK = N'{1}' WITH NAME = '{2}'", // WITH INIT, NAME = '{2}'
					databaseName, bakFile, backupName));

				//define last position in backup set
				string query = string.Format("RESTORE HEADERONLY FROM DISK = N'{0}'", bakFile);
				DataSet ds = ExecuteQuery(connectionString, query);
				query = string.Format("BackupName = '{0}'", backupName);
				DataRow[] rows = ds.Tables[0].Select(query, "Position DESC");
				if (rows != null && rows.Length > 0)
					position = rows[0]["Position"].ToString();
			}
		}

		public static void RestoreDatabase(string connectionString, string databaseName, string bakFile)
		{
			Data.DbType dbType;
			string ConnStr;
			ParseConnectionString(connectionString, out dbType, out ConnStr);
			if (dbType == Data.DbType.SqlServer)
			{
				// close current database connections
				CloseDatabaseConnections(connectionString, databaseName);

				// restore database
				ExecuteNonQuery(connectionString,
					String.Format(@"RESTORE DATABASE [{0}] FROM DISK = '{1}' WITH REPLACE",
						databaseName, bakFile));
			}
		}

		public static void RestoreDatabase(string connectionString, string databaseName, string bakFile, string position)
		{
			Data.DbType dbType;
			string ConnStr;
			ParseConnectionString(connectionString, out dbType, out ConnStr);
			if (dbType == Data.DbType.SqlServer)
			{
				// close current database connections
				CloseDatabaseConnections(connectionString, databaseName);

				// restore database
				ExecuteNonQuery(connectionString,
					String.Format(@"RESTORE DATABASE [{0}] FROM DISK = '{1}' WITH FILE = {2}, REPLACE",
						databaseName, bakFile, position));
			}
		}
		public class Script : IDisposable
		{
			public bool IsMySql => DbType == Data.DbType.MySql || DbType == Data.DbType.MariaDb;
			public bool IsSqlite => DbType == Data.DbType.Sqlite || DbType == Data.DbType.SqliteFX;
			public bool IsSqlServer => DbType == Data.DbType.SqlServer;
			public Data.DbType DbType = Data.DbType.Unknown;
			public string Delimiter = null;
			public Stream Source;
			public StreamReader Reader;
			public string File = "";

			public Script(Stream stream, string connectionString)
			{
				Source = stream;
				Data.DbType dbtype;
				ParseConnectionString(connectionString, out dbtype, out connectionString);
				DbType = dbtype;
				Reset();
			}
			public Script(string fileName, string connectionString) : this(new FileStream(fileName, FileMode.Open, FileAccess.Read), connectionString) { }

			public void Dispose()
			{
				Source?.Dispose();
				Source = null;
				Reader?.Dispose();
				Reader = null;
			}
			public void Reset()
			{
				Source.Seek(0, SeekOrigin.Begin);
				Reader = new StreamReader(Source);
				if (IsMySql) Delimiter = ";";
			}

			public bool HasDelimiter(string line)
			{
				if (IsMySql)
				{
					var delimiter = Delimiter;
					var match = Regex.Match(line, @"^DELIMITER\s+(.*?)\s*$", RegexOptions.Singleline | RegexOptions.IgnoreCase);
					if (match.Success) Delimiter = match.Groups[1].Value;
					return line.EndsWith(delimiter);
				}
				else if (IsSqlite) return line.EndsWith(";");
				else
				{
					return Regex.IsMatch(line,
						@"^\s*GO\s*$",
						RegexOptions.Singleline | RegexOptions.IgnoreCase);
				}
			}
			public bool IsDelimiter(string cmd) => IsSqlServer && HasDelimiter(cmd);

			public string ReadNextStatement()
			{
				StringBuilder sb = new StringBuilder();
				string lineOfText;

				while (true)
				{
					lineOfText = Reader.ReadLine();
					if (lineOfText == null)
					{
						if (sb.Length > 0)
						{
							return sb.ToString();
						}
						else
						{
							return null;
						}
					}

					if (HasDelimiter(lineOfText))
					{
						if (!IsDelimiter(lineOfText)) sb.Append(lineOfText + Environment.NewLine);
						break;
					}

					sb.Append(lineOfText + Environment.NewLine);
				}

				return sb.ToString();
			}

			public int StatementCount
			{
				get
				{
					int count = 0;
					Reset();
					while (ReadNextStatement() != null) count++;
					Reset();
					return count;
				}
			}
		}
		public static System.Data.Common.DbConnection SqlServerConnection(string connectionString)
		{
			return new SqlConnection(connectionString);
		}
		public static System.Data.Common.DbConnection MySqlConnection(string connectionString)
		{
			return new MySqlConnection(connectionString);
		}
		private static System.Data.Common.DbConnection SqliteConnection(string connectionString)
		{
#if NETFRAMEWORK
			return new SQLiteConnection(connectionString);
#else
			return new Microsoft.Data.Sqlite.SqliteConnection(connectionString);
#endif
		}
		public static System.Data.Common.DbCommand SqlServerCommand() => new SqlCommand();
		public static System.Data.Common.DbCommand MySqlCommand() => new MySqlCommand();
		private static System.Data.Common.DbCommand SqliteCommand()
		{
#if NETFRAMEWORK
			return new SQLiteCommand();
#else
			return new Microsoft.Data.Sqlite.SqliteCommand();
#endif
		}

		public static System.Data.Common.DbConnection DbConnection(string connectionString)
		{
			Data.DbType dbtype;
			ParseConnectionString(connectionString, out dbtype, out connectionString);
			switch (dbtype)
			{
				case Data.DbType.SqlServer: return SqlServerConnection(connectionString);
				case Data.DbType.MySql:
				case Data.DbType.MariaDb: return MySqlConnection(connectionString);
				case Data.DbType.Sqlite:
				case Data.DbType.SqliteFX: return SqliteConnection(connectionString);
				default: throw new NotSupportedException();
			}
		}

		public static System.Data.Common.DbCommand DbCommand(string connectionString)
		{
			Data.DbType dbtype;
			ParseConnectionString(connectionString, out dbtype, out connectionString);
			switch (dbtype)
			{
				case Data.DbType.SqlServer: return SqlServerCommand();
				case Data.DbType.MySql:
				case Data.DbType.MariaDb: return MySqlCommand();
				case Data.DbType.Sqlite:
				case Data.DbType.SqliteFX: return SqliteCommand();
				default: throw new NotSupportedException();
			}
		}

		static void ProcessMySqlScript(System.Data.Common.DbConnection connection, string connectionString, Script script, int commandCount = 0,
			Action<float> OnProgressChange = null, Func<string, string> ProcessInstallVariables = null)
		{
			// iterate through delimited command text
			var command = new MySqlScript(connectionString);
			command.Connection = (MySqlConnection)connection;
			command.Delimiter = ";";
			int i = 0;
			float n = commandCount;
			var sql = script.Reader.ReadToEnd();
			command.Query = ProcessInstallVariables != null ? ProcessInstallVariables?.Invoke(sql) : sql;
			command.StatementExecuted += (sender, args) =>
				OnProgressChange?.Invoke((float)++i / n);
			command.Error += (sender, args) =>
				throw new Exception("Error executing SQL command: " + args.StatementText, args.Exception);
			command.Execute();
		}

		public static void SetServerAdminPassword(string connectionString, string database, string password)
		{
			const string SqlServerStatement = @"USE [{0}]; UPDATE [dbo].[Users] SET [Password] = '{1}' WHERE [UserID] = 1;";
			const string SqliteStatement = @"UPDATE Users SET Password = '{0}' WHERE UserID = 1;";
			const string MySqlStatement = @"USE {0}; UPDATE Users SET Password = '{1}' WHERE UserID = 1;";

			string cmd, nativeConnStr;

			Data.DbType dbtype;
			ParseConnectionString(connectionString, out dbtype, out nativeConnStr);

			switch (dbtype)
			{
				case Data.DbType.SqlServer: cmd = string.Format(SqlServerStatement, database, password); break;
				case Data.DbType.MySql:
				case Data.DbType.MariaDb: cmd = string.Format(MySqlStatement, database, password); break;
				case Data.DbType.Sqlite:
				case Data.DbType.SqliteFX: cmd = string.Format(SqliteStatement, password); break;
				default: throw new NotSupportedException("This database type is not supported.");
			}
			DatabaseUtils.ExecuteQuery(connectionString, cmd);
		}

		public Version GetDatabaseVersion(string connectionString, string database)
		{
			try
			{
				var set = ExecuteQuery(connectionString,
	@$"USE {database}
SELECT DatabaseVersion FROM Version");

				return set.Tables[0].Rows.OfType<DataRow>()
					.Select(row =>
					{
						var str = row["DatabaseVersion"] as string;
						Version version = default;
						Version.TryParse(str, out version);
						return version;
					})
					.Max() ?? default;
			}
			catch (Exception)
			{
				return default;
			}
		}
		public static void RunSqlScript(string connectionString, Script script, int commandCount = 0,
					Action<float> OnProgressChange = null, Func<string, string> ProcessInstallVariables = null,
					string database = null)
		{

			var connection = DbConnection(connectionString);
			connection.Open();
			string sql;
			int i = 0;
			float n = commandCount;

			try
			{
				// iterate through delimited command text
				var command = DbCommand(connectionString);
				command.Connection = connection;
				command.CommandType = System.Data.CommandType.Text;
				command.CommandTimeout = 600;

				if (!string.IsNullOrEmpty(database))
				{
					if (script.IsSqlServer)
					{
						command.CommandText = $"USE [{database}]";
						command.ExecuteNonQuery();
					}
					else if (script.IsMySql)
					{
						command.CommandText = $"USE {database}";
						command.ExecuteNonQuery();
					}
				}

				if (!script.IsMySql)
				{
					while (null != (sql = script.ReadNextStatement()))
					{
						if (!string.IsNullOrWhiteSpace(sql))
						{
							sql = ProcessInstallVariables?.Invoke(sql) ?? sql;
							command.CommandText = sql;
							try
							{
								command.ExecuteNonQuery();
							}
							catch (Exception ex)
							{
								throw new Exception("Error executing SQL command: " + sql, ex);
							}
						}

						i++;
						if (commandCount != 0)
						{
							OnProgressChange?.Invoke((float)i / n);
						}

					}
				}
				else
				{
					ProcessMySqlScript(connection, connectionString, script, commandCount, OnProgressChange, ProcessInstallVariables);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Can't run SQL script " + script.File, ex);
			}
			finally
			{
				connection.Close();
			}
		}

		public static void InstallFreshDatabase(string masterConnectionString, string databaseName,
			string user, string password, Action<float> OnProgressChange = null,
			Action<int> ReportCommandCount = null, Func<string, string> ProcessInstallVariables = null,
			string scriptFile = "", bool countOnly = false)
		{
			DbType dbType;
			string nativeConnectionString;
			var csb = new ConnectionStringBuilder(masterConnectionString);
			var server = csb.ContainsKey("server") ? csb["server"] as string :
				csb.ContainsKey("data source") ? csb["data source"] as string : null;
			var localDb = server.ToLower().Contains("localdb");
			var dbfile = csb.ContainsKey("AttachDbFilename") ? csb["AttachDbFilename"] as string : null;
			csb.Remove("AttachDbFilename");
			csb.Remove("Database");
			csb.Remove("Initial Catalog");
			masterConnectionString = csb.ToString(); 

			ParseConnectionString(masterConnectionString, out dbType, out nativeConnectionString);

			var assembly = Assembly.GetExecutingAssembly();
			var resourceNames = assembly.GetManifestResourceNames();
			var InstallSqlFile = $"install.{dbType.ToString().ToLowerInvariant()}.sql";
			var InstallSqlFileEndsWith = $".{InstallSqlFile}";

			var installSqlStream = resourceNames
				.Where(name => name.EndsWith(InstallSqlFileEndsWith))
				.Select(name => assembly.GetManifestResourceStream(name))
				.FirstOrDefault();
			if (installSqlStream != null)
			{
				using (var installSqlScript = new Script(installSqlStream, masterConnectionString))
				{
					if (countOnly)
					{
						ReportCommandCount(installSqlScript.StatementCount);
						return;
					}

					if (!DatabaseExists(masterConnectionString, databaseName))
					{
						CreateDatabase(masterConnectionString, databaseName);
					}
					else throw new InvalidOperationException($"Database {databaseName} already exists.");

					if (dbType != DbType.Sqlite && dbType != DbType.SqliteFX &&
						!localDb && !string.IsNullOrEmpty(user))
					{
						if (!UserExists(masterConnectionString, user)) CreateUser(masterConnectionString, user, password, databaseName);
						else throw new NotSupportedException($"Database user {user} already exists.");
					}

					RunSqlScript(masterConnectionString, installSqlStream, OnProgressChange, ReportCommandCount,
						ProcessInstallVariables, InstallSqlFile, databaseName);
				}
			}
		}

		public static void UpdateDatabase(string masterConnectionString, string databaseName,
			string user, string password, bool oldVersionUpdate = true, Action<float> OnProgressChange = null,
			Action<int> ReportCommandCount = null, Func<string, string> ProcessInstallVariables = null,
			string scriptFile = "", bool countOnly = false)
		{
			DbType dbType;
			string nativeConnectionString;
			ParseConnectionString(masterConnectionString, out dbType, out nativeConnectionString);
			//
			if (!countOnly)
			{
				if (!DatabaseExists(masterConnectionString, databaseName))
					throw new InvalidOperationException($"Database {databaseName} does not exist.");

				if (dbType != DbType.Sqlite && dbType != DbType.SqliteFX)
				{
					if (!UserExists(masterConnectionString, user)) CreateUser(masterConnectionString, user, password, databaseName);
					else AddUserToDatabase(masterConnectionString, databaseName, user);
				}
			}
			var updateSql = InstallScriptUpdateDbStream();
			var installSql = InstallScriptStream(dbType);
			using (var updateSqlScript = new Script(updateSql, masterConnectionString))
			using (var installSqlScript = new Script(installSql, masterConnectionString))
			{
				var updateCount = updateSqlScript.StatementCount;
				var installCount = installSqlScript.StatementCount;
				ReportCommandCount?.Invoke(updateCount + installCount);
				if (!countOnly)
				{
					if (dbType != DbType.Sqlite && dbType != DbType.SqliteFX)
					{
						if (dbType == DbType.SqlServer)
						{
							var efMigrationHistory = ExecuteScalarSqlServer(masterConnectionString, "SELECT OBJECT_ID(N'[dbo].[__EFMigrationsHistory]')");
							var versions = ExecuteScalarSqlServer(masterConnectionString, "SELECT OBJECT_ID(N'[dbo].[Versions]')");
							if (versions != null && versions != DBNull.Value && (efMigrationHistory == null || efMigrationHistory == DBNull.Value))
							{
								RunSqlScript(masterConnectionString, updateSqlScript, updateCount, OnProgressChange,
									ProcessInstallVariables, databaseName);
							}
						}
						RunSqlScript(masterConnectionString, installSqlScript, installCount, OnProgressChange,
						ProcessInstallVariables, databaseName);
					} else
					{
						if (Providers.OS.OSInfo.IsCore)
						{
							var contextType = Type.GetType("FuseCP.EnterpriseServer.Data.SqliteDbContext, FuseCP.EnterpriseServer.Data.NetCore");
							var sqliteConnectionString = DbSettings.GetNativeConnectionString(masterConnectionString);
							// Migrations are generated from the seeded model, so runtime SQLite updates
							// must use the same model shape to avoid false pending-model-changes errors.
							using (var context = Activator.CreateInstance(contextType, new object[] { sqliteConnectionString, true }) as IMigratableDbContext)
							{
								context.Migrate();
							}
						}
						else throw new NotSupportedException("Upgrade SQLIte database must run on .NET Core.");
					}
				}
			}
		}

		public static void RunSqlScript(string connectionString, Stream sql, Action<float> OnProgressChange = null,
			Action<int> ReportCommandCount = null, Func<string, string> ProcessInstallVariables = null, string scriptFile = "",
			string database = null)
		{
			using (var script = new Script(sql, connectionString) { File = scriptFile })
			{
				int commandCount = 0;
				try
				{
					while (null != script.ReadNextStatement())
					{
						commandCount++;
					}
				}
				catch (Exception ex)
				{
					throw new Exception("Can't read SQL script " + script.File, ex);
				}

				ReportCommandCount?.Invoke(commandCount);

				script.Reset();

				RunSqlScript(connectionString, script, commandCount, OnProgressChange, ProcessInstallVariables, database);
			}
		}
	}
}
