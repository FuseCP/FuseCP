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
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using FuseCP.Providers;
#if !NETFRAMEWORK
using CoreWCF;
using CoreWCF.Web;
using CoreWCF.Description;
using CoreWCF.Channels;
using CoreWCF.Dispatcher;
#else
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
#endif


namespace FuseCP.Web.Services
{
	public class SoapHeaderMessageInspector : IDispatchMessageInspector, IEndpointBehavior
	{

		public const string Namespace = "http://fusecp.com/headers/";

		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{
		}

		bool HasApi(string adr, string type, string api) {
			var match = Regex.Match(adr, $"/(?<api>[a-z]+)/{type}(?:\\?|$|/)");
			return match.Success && match.Groups["api"].Value == api;
		}

		public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
		{
			var instance = instanceContext.GetServiceInstance();
			var instanceType = instance.GetType();
			var adr = OperationContext.Current.EndpointDispatcher.EndpointAddress.Uri.AbsolutePath;
			var isRest = HasApi(adr, instanceType.Name, "api");
			string operationName;
			if (!isRest) {
				var action = request.Headers.Action;
				operationName = action.Substring(action.LastIndexOf("/") + 1);
			} else {
				operationName = request.Properties["HttpOperationName"] as string;
			}
			var contract = instanceType.GetInterfaces()
				.FirstOrDefault(intf => intf.GetCustomAttribute<ServiceContractAttribute>() != null);
			var method = contract.GetMethod(operationName);
			var soapattr = method.GetCustomAttribute<SoapHeaderAttribute>();
			if (soapattr != null)
			{
				var p = instanceType.GetProperty(soapattr.Field, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				if (p != null)
				{
					if (!isRest)
					{
						int hpos = request.Headers.FindHeader(p.PropertyType.Name, $"{Namespace}{p.PropertyType.Name}");
						var getHeaderMethod = request.Headers.GetType().GetMethod("GetHeader", new Type[] { typeof(string), typeof(string) });
						getHeaderMethod = getHeaderMethod.MakeGenericMethod(p.PropertyType);
						var header = getHeaderMethod.Invoke(request.Headers, new object[] { p.PropertyType.Name, $"{Namespace}{p.PropertyType.Name}" });
						p.SetValue(instance, header);
					} else
					{
						var headerText = WebOperationContext.Current.IncomingRequest.Headers[$"Header-{soapattr.Field}"];
						if (string.IsNullOrEmpty(headerText)) throw new NotSupportedException($"Header Header-{soapattr.Field} not set.");

						headerText = headerText.Trim();
						if (!headerText.StartsWith("<"))
						{
							var header = JsonConvert.DeserializeObject(headerText, p.PropertyType, new VersionConverter(), new StringEnumConverter());
							p.SetValue(instance, header);
						} else
						{
							var s = new DataContractSerializer(p.PropertyType);
							var reader = XmlDictionaryReader.CreateTextReader(Encoding.Unicode.GetBytes(headerText), new XmlDictionaryReaderQuotas());
							var header = s.ReadObject(reader);
							p.SetValue(instance, header);
						}
					}
				} else {
					var f = instanceType.GetField(soapattr.Field, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
					if (f != null)
					{
						if (!isRest)
						{
							int hpos = request.Headers.FindHeader(f.FieldType.Name, $"{Namespace}{f.FieldType.Name}");
							var getHeaderMethod = request.Headers.GetType().GetMethod("GetHeader", new Type[] { typeof(string), typeof(string) });
							getHeaderMethod = getHeaderMethod.MakeGenericMethod(f.FieldType);
							var header = getHeaderMethod.Invoke(request.Headers, new object[] { f.FieldType.Name, $"{Namespace}{f.FieldType.Name}" });
							f.SetValue(instance, header);
						} else
						{
							var headerText = WebOperationContext.Current.IncomingRequest.Headers[$"Header-{soapattr.Field}"];
							if (string.IsNullOrEmpty(headerText)) throw new NotSupportedException($"Header Header-{soapattr.Field} not set.");

							headerText = headerText.Trim();
							if (!headerText.StartsWith("<"))
							{
								var header = JsonConvert.DeserializeObject(headerText, f.FieldType, new VersionConverter(), new StringEnumConverter());
								f.SetValue(instance, header);
							}
							else
							{
								var s = new DataContractSerializer(f.FieldType);
								var reader = XmlDictionaryReader.CreateTextReader(Encoding.Unicode.GetBytes(headerText), new XmlDictionaryReaderQuotas());
								var header = s.ReadObject(reader);
								f.SetValue(instance, header);
							}

						}
					}
				}
			}
			var policy = contract.GetCustomAttribute<PolicyAttribute>();
			if (!isRest && policy != null && policy.Policy == PolicyAttribute.ServerAuthenticated &&
				UserNamePasswordValidator.ValidateServerRequestAuthentication != null)
			{
				var requestAuthentication = TryGetServerRequestAuthentication(request);
				if (requestAuthentication != null)
				{
					requestAuthentication.Action = request.Headers.Action;
					requestAuthentication.Resource = request.Headers.To?.PathAndQuery ??
						OperationContext.Current.EndpointDispatcher.EndpointAddress.Uri.AbsolutePath;

					if (!UserNamePasswordValidator.ValidateServerRequestAuthentication(requestAuthentication))
						throw new FaultException("Invalid server request authentication");

					return null;
				}
			}
			if (!isRest && policy != null && policy.Policy != PolicyAttribute.Encrypted)
			{
                int hpos = request.Headers.FindHeader(nameof(Credentials), $"{Namespace}{nameof(Credentials)}");
				if (hpos < 0) throw new FaultException("No anonymous access allowed.");
				var header = request.Headers.GetHeader<Credentials>(hpos);
				var validator = new UserNamePasswordValidator() { Policy = policy };
				validator.ValidateAsync(header.Username, header.Password).AsTask().Wait();
			}
            return null;
		}

		private static ServerRequestAuthenticationData TryGetServerRequestAuthentication(Message request)
		{
			if (!request.Properties.TryGetValue(HttpRequestMessageProperty.Name, out object property) ||
				!(property is HttpRequestMessageProperty httpRequest))
			{
				return null;
			}

			string version = httpRequest.Headers[ServerRequestAuthentication.VersionHeaderName];
			if (string.IsNullOrEmpty(version))
				return null;

			return new ServerRequestAuthenticationData
			{
				Version = version,
				Timestamp = httpRequest.Headers[ServerRequestAuthentication.TimestampHeaderName],
				Nonce = httpRequest.Headers[ServerRequestAuthentication.NonceHeaderName],
				KeyId = httpRequest.Headers[ServerRequestAuthentication.KeyIdHeaderName],
				ClusterId = httpRequest.Headers[ServerRequestAuthentication.ClusterIdHeaderName],
				Signature = httpRequest.Headers[ServerRequestAuthentication.SignatureHeaderName]
			};
		}

		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
		}

		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
			var channelDispatcher = endpointDispatcher.ChannelDispatcher;
			if (channelDispatcher == null) return;
			var inspector = new SoapHeaderMessageInspector();
			foreach (var ed in channelDispatcher.Endpoints)
			{
				ed.DispatchRuntime.MessageInspectors.Add(inspector);
			}
		}

		public void BeforeSendReply(ref Message reply, object correlationState)
		{
		}

		public void Validate(ServiceEndpoint endpoint)
		{
		}
	}
}
