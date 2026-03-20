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
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;

namespace FuseCP.Ecommerce.EnterpriseServer
{
	public class WhoisLookup
	{
		#region Failure messages

		public const string GENERIC_FAILURE_MESSAGE = "Could not check the domain availability";
		public const string EMPTY_RESPONSE_MESSAGE = "WHOIS server couldn't be reached or returned an empty response";

		#endregion

		public static WhoisResult Query(string domain, string tld)
		{
			WhoisResult response = new WhoisResult();

			try
			{
				// check whether tld contains sld parts
				// applicable for .*.uk names
				string whoisKey = (tld.IndexOf(".") > -1) ? tld.Substring(tld.LastIndexOf(".") + 1) : tld;
				// no whois server association for TLD found
				if (!WhoisSettings.WhoisServers.Contains(whoisKey))
					throw new Exception(GENERIC_FAILURE_MESSAGE);
				// get whois server
				string whoisServer = (string)WhoisSettings.WhoisServers[whoisKey];
				// query whois server for specified domain & tld
				StringReader reader = Whois(whoisServer, domain, tld);
				// check response is not null
				if (reader == null)
					throw new Exception(EMPTY_RESPONSE_MESSAGE);
				// parse whois response
				response = WhoisParser.Parse(domain + "." + tld, whoisServer, reader);
				// query succeed
				response.Success = true;
			}
			catch (WhoisException ex)
			{
				response.Success = false;
				response.ResultException = ex;
				response.ErrorMessage = ex.Message;
			}
			catch (Exception ex)
			{
				response.Success = false;
				response.ErrorMessage = ex.Message;
				response.ResultException = ex;
			}

			return response;
		}

		/// <summary>
		/// Get domain TLD
		/// </summary>
		/// <param name="domain">Domain name</param>
		/// <returns>Top level domain</returns>
		private static string GetDomainTLD(string domain)
		{
			if (domain == null)
				throw new ArgumentNullException("domain");

			int ccStart = domain.LastIndexOf(".");

			if (ccStart < 0 || ccStart == domain.Length)
				throw new ArgumentException("Domain name has invalid format specified: " + domain);

			return domain.Substring(ccStart + 1);
		}

		/// <summary>
		/// Get domain name without TLD
		/// </summary>
		/// <param name="domain">Domain name</param>
		/// <returns>Domain name</returns>
		private static string GetDomainName(string domain)
		{
			if (domain == null)
				throw new ArgumentNullException("domain");

			int ccStart = domain.LastIndexOf(".");

			if (ccStart < 0 || ccStart == domain.Length)
				throw new ArgumentException("Domain name has invalid format specified: " + domain);

			return domain.Substring(0, ccStart);
		}

		/// <summary>
		/// Perform whois query
		/// </summary>
		/// <param name="domain">Domain name (FQDN) to query</param>
		/// <param name="tld">Domain tld</param>
		/// <returns></returns>
		private static StringReader Whois(string whoisServer, string domain, string tld)
		{
			StringReader sr = null;
			TcpClient tcp = null;

			if (String.IsNullOrEmpty(whoisServer))
				throw new ArgumentNullException("whoisServer");

			if (String.IsNullOrEmpty(domain))
				throw new ArgumentNullException("domain");

			if (String.IsNullOrEmpty(tld))
				throw new ArgumentNullException("tld");

			try
			{
				// get whois format
				string whoisFormat = (string)WhoisSettings.Parsers[whoisServer];
				// build whois command
				string whoisCommand = String.Concat(domain, ".", tld);
				// INTERNIC use domain option to query domain exactly.
				// It also affects on the result messages format so be careful.
                switch (whoisFormat)
                {
                    case WhoisSettings.INTERNIC:
                        whoisCommand = "domain " + whoisCommand;
                        break;
                    // Forces AFNIC servers to use english language
                    case WhoisSettings.AFNIC:
                        whoisCommand = "-l EN " + whoisCommand;
                        break;
                }
				// initialize string buffer
				StringBuilder builder = new StringBuilder();
				// creates a tcp client instance
				tcp = new TcpClient();
				// trying to connect
				tcp.Connect(new IPEndPoint(Dns.GetHostEntry(whoisServer).AddressList[0], 43));
				// obtain networkstream reference
				NetworkStream netStream = tcp.GetStream();
				// create reader
				StreamReader reader = new StreamReader(netStream);
				// create writer
				StreamWriter writer = new StreamWriter(netStream);
				// write command to the whois stream
				writer.WriteLine(whoisCommand);
				// flush written command
				writer.Flush();
				// read stream to end
				while (!reader.EndOfStream)
					builder.AppendLine(reader.ReadLine());
				// fill string reader
				sr = new StringReader(builder.ToString());
			}
			catch (Exception)
			{
				throw;
			}
			finally
			{
				if (tcp != null && tcp.Connected)
					tcp.Close();
			}

			return sr;
		}
	}
}
