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

using FuseCP.Server.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace FuseCP.Providers.Filters
{
    public class SpamExperts : HostingServiceProviderBase, ISpamExperts
    {
        public SpamExperts()
        {
        }

        public override bool IsInstalled()
        {
            return true;
        }

        private string emptyPassword => Guid.NewGuid().ToString();

        private string Url => ProviderSettings["SpamExpertsUrl"];

        private string User => ProviderSettings["SpamExpertsUser"];

        private string Password => ProviderSettings["SpamExpertsPassword"];


        private SpamExpertsResult CheckSuccess(string result)
        {
            if (result == null) return SpamExpertsResult.None;

            if (result.Contains("SUCCESS")) return new SpamExpertsResult(SpamExpertsStatus.Success, result);
            if (result.StartsWith("[") && result.EndsWith("]")) return new SpamExpertsResult(SpamExpertsStatus.Success, result);
            if (result.StartsWith("{") && result.EndsWith("}")) return new SpamExpertsResult(SpamExpertsStatus.Success, result);

            if (result.Contains("already exists")) return new SpamExpertsResult(SpamExpertsStatus.AlreadyExists, result);
            if (result.Contains("Unable to find")) return new SpamExpertsResult(SpamExpertsStatus.NotFound, result);
            if (result.Contains("No such")) return new SpamExpertsResult(SpamExpertsStatus.NotFound, result);

            if (result.StartsWith("EXCEPTION:")) return new SpamExpertsResult(SpamExpertsStatus.Error, result);
            if (result.StartsWith("ERROR:")) return new SpamExpertsResult(SpamExpertsStatus.Error, result);

            return new SpamExpertsResult(SpamExpertsStatus.Success, result);
        }

        private SpamExpertsResult ExecCommand(string command, params string[] param)
        {
            Log.WriteStart("ExecCommand {0}", command);

            UriBuilder uri = new UriBuilder();
            uri.Scheme = "https";
            uri.Host = Url;

            string path = "/api/" + command + "/";
            int paramCount = param.Length / 2;
            for (int i = 0; i < paramCount; i++)
            {
                string name = param[i * 2];
                string val = param[i * 2 + 1];

                path += name + "/" + HttpUtility.UrlEncode(val) + "/";

                if (name != "password")
                    Log.WriteInfo("{0}={1}", name, val);

            }
            uri.Path = path;

            string result = string.Empty;
            try
            {
                var handler = new HttpClientHandler
                {
                    Credentials = new NetworkCredential(User, Password)
                };

                using (var client = new HttpClient(handler))
                {
                    result = client.GetStringAsync(uri.Uri).GetAwaiter().GetResult();
                }

                if (result == null) result = "";

                Log.WriteInfo("result = {0}", result);
            }
            catch (Exception exc)
            {
                result = "EXCEPTION:" + exc.ToString();
                Log.WriteWarning(result);
            }

            Log.WriteEnd("ExecCommand");
            return CheckSuccess(result);
        }

        public SpamExpertsResult AddDomainFilter(string domain, string password, string email, string[] destinations)
        {
            SpamExpertsResult result;

            Log.WriteStart("AddDomainFilter");

            if (String.IsNullOrEmpty(password))
                password = emptyPassword;

            if ((destinations == null) || (destinations.Length == 0))
            {
                result = ExecCommand("domain/add", "domain", domain);
            }
            else
            {
                string list = "[\"" + String.Join("\",\"", destinations) + "\"]";
                result = ExecCommand("domain/add", "domain", domain, "destinations", list);
            }
            if (result.Status == SpamExpertsStatus.Success)
            {
                result = ExecCommand("domainuser/add", "domain", domain, "password", password, "email", email);
            }

            Log.WriteEnd("AddDomainFilter");

            return result;
        }

        public SpamExpertsResult SetDomainFilterUser(string domain, string password, string email)
        {
            SpamExpertsResult result;

            Log.WriteStart("SetDomainFilterUser");

            if (String.IsNullOrEmpty(password))
                password = emptyPassword;

            result = ExecCommand("domainuser/add", "domain", domain, "password", password, "email", email);

            Log.WriteEnd("SetDomainFilterUser");

            return result;
        }



        public SpamExpertsResult DeleteDomainFilter(string domain)
        {
            Log.WriteStart("DeleteDomainFilter");

            var result = ExecCommand("domainuser/remove", "username", domain);
            result = ExecCommand("outgoingusers/remove", "domain", domain);
            result = ExecCommand("domain/remove", "domain", domain);

            Log.WriteEnd("DeleteDomainFilter");

            return result;
        }

        public SpamExpertsResult AddEmailFilter(string name, string domain, string password)
        {
            Log.WriteStart("AddEmailFilter");

            if (String.IsNullOrEmpty(password))
                password = emptyPassword;

            var result = ExecCommand("emailusers/add", "username", name, "password", password, "domain", domain);

            Log.WriteEnd("AddEmailFilter");

            return result;
        }

        public SpamExpertsResult DeleteEmailFilter(string email)
        {
            Log.WriteStart("DeleteEmailFilter");

            var result = ExecCommand("emailusers/remove", "username", email);

            Log.WriteEnd("DeleteEmailFilter");

            return result;
        }

        public SpamExpertsResult SetEmailFilterUserPassword(string email, string password)
        {
            Log.WriteStart("SetEmailFilterUserPassword");

            if (String.IsNullOrEmpty(password))
                password = emptyPassword;

            var result = ExecCommand("emailusers/setpassword", "username", email, "password", password);

            Log.WriteEnd("SetEmailFilterUserPassword");

            return result;
        }

        public SpamExpertsResult SetDomainFilterUserPassword(string name, string password)
        {
            Log.WriteStart("SetDomainFilterUserPassword");

            if (String.IsNullOrEmpty(password))
                password = emptyPassword;

            var result = ExecCommand("domainuser/setpassword", "username", name, "password", password);

            Log.WriteEnd("SetDomainFilterUserPassword");

            return result;
        }

        public SpamExpertsResult SetDomainFilterDestinations(string domain, string[] destinations)
        {
            Log.WriteStart("SetDomainFilterDestinations");
            if (destinations == null) destinations = new string[] { };

            string list = "[\"" + String.Join("\",\"", destinations) + "\"]";
            var result = ExecCommand("domain/edit", "domain", domain, "destinations", list);

            Log.WriteEnd("SetDomainFilterDestinations");

            return result;
        }

        public SpamExpertsResult AddDomainFilterAlias(string domain, string alias)
        {
            SpamExpertsResult res = null;

            Log.WriteStart("AddDomainFilterAlias");

            res = ExecCommand("domainalias/add", "domain", domain, "alias", alias);

            Log.WriteEnd("AddDomainFilterAlias");

            return res;
        }

        public SpamExpertsResult DeleteDomainFilterAlias(string domain, string alias)
        {
            SpamExpertsResult res = null;

            Log.WriteStart("DeleteDomainFilterAlias");

            res = ExecCommand("domainalias/remove", "domain", domain, "alias", alias);

            Log.WriteEnd("DeleteDomainFilterAlias");

            return res;
        }



    }
}
