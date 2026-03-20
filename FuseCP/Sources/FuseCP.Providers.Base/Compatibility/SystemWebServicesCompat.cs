using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace System.Web.Services
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class WebServiceBindingAttribute : Attribute
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public Description.WsiProfiles ConformsTo { get; set; }
        public bool EmitConformanceClaims { get; set; }
    }

    public class WebService
    {
    }
}

namespace System.Web.Services.Description
{
    public enum WsiProfiles
    {
        None = 0,
        BasicProfile1_1 = 1
    }

    public enum SoapBindingUse
    {
        Default,
        Encoded,
        Literal
    }
}

namespace System.Web.Services.Protocols
{
    public enum SoapParameterStyle
    {
        Default,
        Bare,
        Wrapped
    }

    public enum SoapHeaderDirection
    {
        In,
        Out,
        InOut,
        Fault
    }

    public enum SoapProtocolVersion
    {
        Default,
        Soap11,
        Soap12
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class SoapDocumentMethodAttribute : Attribute
    {
        public SoapDocumentMethodAttribute(string action)
        {
            Action = action;
        }

        public string Action { get; }
        public string RequestNamespace { get; set; }
        public string ResponseNamespace { get; set; }
        public Description.SoapBindingUse Use { get; set; }
        public SoapParameterStyle ParameterStyle { get; set; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class SoapRpcMethodAttribute : Attribute
    {
        public SoapRpcMethodAttribute(string action)
        {
            Action = action;
        }

        public string Action { get; }
        public string RequestNamespace { get; set; }
        public string ResponseNamespace { get; set; }
        public Description.SoapBindingUse Use { get; set; }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class SoapHeaderAttribute : Attribute
    {
        public SoapHeaderAttribute(string memberName)
        {
            MemberName = memberName;
        }

        public string MemberName { get; }
        public SoapHeaderDirection Direction { get; set; }
    }

    public class SoapHeader
    {
        public bool DidUnderstand { get; set; }
    }

    public class InvokeCompletedEventArgs : AsyncCompletedEventArgs
    {
        private readonly object[] _results;

        public InvokeCompletedEventArgs(object[] results, Exception error, bool cancelled, object userState)
            : base(error, cancelled, userState)
        {
            _results = results ?? Array.Empty<object>();
        }

        public object[] Results
        {
            get
            {
                RaiseExceptionIfNecessary();
                return _results;
            }
        }
    }

    public class SoapHttpClientProtocol
    {
        private static readonly HttpClient Client = new HttpClient();

        public string Url { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public ICredentials Credentials { get; set; }
        public SoapProtocolVersion SoapVersion { get; set; } = SoapProtocolVersion.Soap11;

        public virtual void CancelAsync(object userState)
        {
            // Generated legacy proxies call this to cancel pending async operations.
            // The shim executes async calls fire-and-forget, so cancellation is best-effort no-op.
        }

        protected object[] Invoke(string methodName, object[] parameters)
        {
            var method = GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance)
                ?? throw new MissingMethodException(GetType().FullName, methodName);

            var returnType = method.ReturnType;
            var request = parameters != null && parameters.Length > 0 ? parameters[0] : null;
            var action = ResolveSoapAction(methodName, method);
            var requestNs = ResolveRequestNamespace(method, request);

            var bodyPayload = request == null
                ? $"<{methodName} xmlns=\"{requestNs}\"/>"
                : Serialize(request, requestNs);

            var envelopeNs = SoapVersion == SoapProtocolVersion.Soap12
                ? "http://www.w3.org/2003/05/soap-envelope"
                : "http://schemas.xmlsoap.org/soap/envelope/";

            var envelope = $"<s:Envelope xmlns:s=\"{envelopeNs}\"><s:Body>{bodyPayload}</s:Body></s:Envelope>";

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, Url)
            {
                Content = new StringContent(envelope, Encoding.UTF8,
                    SoapVersion == SoapProtocolVersion.Soap12 ? "application/soap+xml" : "text/xml")
            };

            if (!string.IsNullOrEmpty(action))
            {
                if (SoapVersion == SoapProtocolVersion.Soap12)
                {
                    requestMessage.Content.Headers.ContentType?.Parameters.Add(
                        new System.Net.Http.Headers.NameValueHeaderValue("action", $"\"{action}\""));
                }
                else
                {
                    requestMessage.Headers.TryAddWithoutValidation("SOAPAction", $"\"{action}\"");
                }
            }

            if (UseDefaultCredentials)
                requestMessage.SetDefaultCredentials();

            var response = Client.Send(requestMessage);
            response.EnsureSuccessStatusCode();

            var soapXml = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = DeserializeSoapResult(returnType, soapXml, envelopeNs);
            return new[] { result };
        }

        protected IAsyncResult BeginInvoke(string methodName, object[] parameters, AsyncCallback callback, object asyncState)
        {
            var task = Task<object[]>.Factory.StartNew(_ => Invoke(methodName, parameters), asyncState);
            if (callback != null)
                task.ContinueWith(t => callback(t), TaskScheduler.Default);
            return task;
        }

        protected object[] EndInvoke(IAsyncResult asyncResult)
        {
            return ((Task<object[]>)asyncResult).GetAwaiter().GetResult();
        }

        protected void InvokeAsync(string methodName, object[] parameters, SendOrPostCallback callback, object userState)
        {
            Task.Run(() =>
            {
                try
                {
                    var results = Invoke(methodName, parameters);
                    callback?.Invoke(new InvokeCompletedEventArgs(results, null, false, userState));
                }
                catch (Exception ex)
                {
                    callback?.Invoke(new InvokeCompletedEventArgs(Array.Empty<object>(), ex, false, userState));
                }
            });
        }

        private static string ResolveSoapAction(string methodName, MethodInfo method)
        {
            var doc = method.GetCustomAttribute<SoapDocumentMethodAttribute>();
            if (!string.IsNullOrWhiteSpace(doc?.Action))
                return doc.Action;

            var rpc = method.GetCustomAttribute<SoapRpcMethodAttribute>();
            if (!string.IsNullOrWhiteSpace(rpc?.Action))
                return rpc.Action;

            var binding = method.DeclaringType?.GetCustomAttribute<System.Web.Services.WebServiceBindingAttribute>();
            if (!string.IsNullOrWhiteSpace(binding?.Namespace))
                return binding.Namespace.TrimEnd('/') + "/" + methodName;

            return methodName;
        }

        private static string ResolveRequestNamespace(MethodInfo method, object request)
        {
            var doc = method.GetCustomAttribute<SoapDocumentMethodAttribute>();
            if (!string.IsNullOrWhiteSpace(doc?.RequestNamespace))
                return doc.RequestNamespace;

            var rpc = method.GetCustomAttribute<SoapRpcMethodAttribute>();
            if (!string.IsNullOrWhiteSpace(rpc?.RequestNamespace))
                return rpc.RequestNamespace;

            var root = request?.GetType().GetCustomAttribute<XmlRootAttribute>();
            if (!string.IsNullOrWhiteSpace(root?.Namespace))
                return root.Namespace;

            var binding = method.DeclaringType?.GetCustomAttribute<System.Web.Services.WebServiceBindingAttribute>();
            if (!string.IsNullOrWhiteSpace(binding?.Namespace))
                return binding.Namespace;

            return string.Empty;
        }

        private static string Serialize(object value, string xmlNamespace)
        {
            var serializer = new XmlSerializer(value.GetType());
            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, xmlNamespace ?? string.Empty);
            using var writer = new StringWriter();
            serializer.Serialize(writer, value, ns);
            return writer.ToString();
        }

        private static object DeserializeSoapResult(Type returnType, string soapXml, string envelopeNs)
        {
            var doc = new XmlDocument();
            doc.LoadXml(soapXml);
            var ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("s", envelopeNs);
            var body = doc.SelectSingleNode("/s:Envelope/s:Body", ns)
                ?? throw new InvalidOperationException("SOAP envelope body was not found.");

            foreach (XmlNode node in body.ChildNodes)
            {
                if (node.NodeType != XmlNodeType.Element)
                    continue;

                var serializer = new XmlSerializer(returnType);
                using var reader = new StringReader(node.OuterXml);
                return serializer.Deserialize(reader);
            }

            throw new InvalidOperationException("SOAP response body did not include an element payload.");
        }
    }

    internal static class HttpRequestMessageExtensions
    {
        public static void SetDefaultCredentials(this HttpRequestMessage request)
        {
            // Intentionally no-op: HttpClient does not support per-request default credentials.
            // This shim preserves compatibility with generated proxy callsites.
        }
    }
}
