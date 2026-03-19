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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.Globalization;
using FuseCP.Providers;

namespace FuseCP.Web.Clients
{
	public class SoapHeaderClientMessageInspector : IClientMessageInspector
	{
		public const string Namespace = "http://fusecp.com/headers/";

		public ClientBase Client;
		public object SoapHeader => Client.SoapHeader;
		
		public void AfterReceiveReply(ref Message reply, object correlationState)
		{
		}

		public object BeforeSendRequest(ref Message request, IClientChannel channel)
		{
			var methodName = Regex.Match(request.Headers.Action, "(?<=/)[^/]*?$").Value;
			var hasSoapHeader = Client.CheckSoapHeader(methodName);
			var useServerRequestAuthentication = Client.IsAuthenticated &&
				(Client.IsHttp || Client.IsHttps) &&
				Client.Credentials != null &&
				!string.IsNullOrEmpty(Client.Credentials.Password) &&
				string.IsNullOrEmpty(Client.Credentials.UserName);

			if (hasSoapHeader || Client.Credentials != null && Client.Credentials.Password != null && 
				(Client.IsSecureProtocol || Client.IsLocal) || useServerRequestAuthentication)
			{
				// Prepare the request message copy to be modified
				MessageBuffer buffer = request.CreateBufferedCopy(Int32.MaxValue);
				request = buffer.CreateMessage();
			}

			if (hasSoapHeader)
			{
				var header = MessageHeader.CreateHeader(SoapHeader.GetType().Name, $"{Namespace}{SoapHeader.GetType().Name}", SoapHeader);
				request.Headers.Add(header);
			}
			if (Client.Credentials != null && Client.Credentials.Password != null && Client.IsAuthenticated && (Client.IsSecureProtocol || Client.IsLocal))
			{
				var cred = new Credentials { Username = Client.Credentials.UserName, Password = Client.Credentials.Password };
				var header = MessageHeader.CreateHeader(nameof(Credentials), $"{Namespace}{nameof(Credentials)}", cred);
				request.Headers.Add(header);
				// Client.Credentials.UserName = Client.Credentials.Password = null;
			}
			if (useServerRequestAuthentication)
			{
				var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);
				var nonce = ServerRequestAuthentication.CreateNonce();
				var keyId = ServerRequestAuthentication.BuildKeyId(Client.Credentials.Password);
				var resource = request.Headers.To != null
					? request.Headers.To.PathAndQuery
					: channel.RemoteAddress?.Uri?.PathAndQuery ?? string.Empty;
				var signature = ServerRequestAuthentication.ComputeSignature(
					Client.Credentials.Password,
					request.Headers.Action,
					resource,
					timestamp,
					nonce,
					keyId,
					string.Empty);

				HttpRequestMessageProperty httpRequest;
				if (request.Properties.ContainsKey(HttpRequestMessageProperty.Name) &&
					request.Properties[HttpRequestMessageProperty.Name] is HttpRequestMessageProperty existing)
				{
					httpRequest = existing;
				}
				else
				{
					httpRequest = new HttpRequestMessageProperty();
					request.Properties[HttpRequestMessageProperty.Name] = httpRequest;
				}

				httpRequest.Headers[ServerRequestAuthentication.VersionHeaderName] = ServerRequestAuthentication.CurrentVersion;
				httpRequest.Headers[ServerRequestAuthentication.TimestampHeaderName] = timestamp;
				httpRequest.Headers[ServerRequestAuthentication.NonceHeaderName] = nonce;
				httpRequest.Headers[ServerRequestAuthentication.KeyIdHeaderName] = keyId;
				httpRequest.Headers[ServerRequestAuthentication.ClusterIdHeaderName] = string.Empty;
				httpRequest.Headers[ServerRequestAuthentication.SignatureHeaderName] = signature;
			}
			return null;
		}
	}

	public class SoapHeaderClientBehavior : IEndpointBehavior
	{
		public ClientBase Client;

		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
			clientRuntime.ClientMessageInspectors.Add(new SoapHeaderClientMessageInspector() { Client = this.Client });
		}

		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
		}

		public void Validate(ServiceEndpoint endpoint)
		{
		}
	}
}
