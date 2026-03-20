using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace FuseCP.Providers.FTP.CerberusFTP6Proxy
{
    public sealed class CerberusFTPService
    {
        private const string SoapEnvelopeNs = "http://www.w3.org/2003/05/soap-envelope";
        private const string ServiceNs = "http://cerberusllc.com/service/cerberusftpservice";

        public bool RequireMtom { get; set; }
        public bool EnableDecompression { get; set; }
        public string Url { get; set; } = "http://localhost:10001/service/cerberusftpservice";

        public GetUserListResponse GetUserList(GetUserListRequest request)
        {
            return Invoke<GetUserListRequest, GetUserListResponse>("GetUserList", request);
        }

        public GetUserInformationResponse GetUserInformation(GetUserInformationRequest request)
        {
            return Invoke<GetUserInformationRequest, GetUserInformationResponse>("GetUserInformation", request);
        }

        public AddUserResponse AddUser(AddUserRequest request)
        {
            return Invoke<AddUserRequest, AddUserResponse>("AddUser", request);
        }

        public DeleteUserResponse DeleteUser(DeleteUserRequest request)
        {
            return Invoke<DeleteUserRequest, DeleteUserResponse>("DeleteUser", request);
        }

        private TResponse Invoke<TRequest, TResponse>(string operation, TRequest request)
        {
            if (string.IsNullOrWhiteSpace(Url))
                throw new InvalidOperationException("Cerberus SOAP endpoint URL is not configured.");

            var bodyXml = Serialize(request);
            var envelope = $"<s:Envelope xmlns:s=\"{SoapEnvelopeNs}\"><s:Body>{bodyXml}</s:Body></s:Envelope>";
            using var handler = new HttpClientHandler();
            if (EnableDecompression)
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli;

            using var client = new HttpClient(handler, disposeHandler: true);
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, Url)
            {
                Content = new StringContent(envelope, Encoding.UTF8, "application/soap+xml")
            };

            httpRequest.Content.Headers.ContentType!.Parameters.Add(new System.Net.Http.Headers.NameValueHeaderValue("action", $"\"{ServiceNs}/{operation}\""));

            using var response = client.Send(httpRequest);
            response.EnsureSuccessStatusCode();

            var payload = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            return DeserializeSoapBody<TResponse>(payload);
        }

        private static string Serialize<T>(T instance)
        {
            var serializer = new XmlSerializer(typeof(T));
            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, ServiceNs);
            using var sw = new StringWriter();
            serializer.Serialize(sw, instance, ns);
            return sw.ToString();
        }

        private static T DeserializeSoapBody<T>(string soapXml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(soapXml);

            var ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("s", SoapEnvelopeNs);
            var body = doc.SelectSingleNode("/s:Envelope/s:Body", ns)
                ?? throw new InvalidOperationException("SOAP response does not contain a body.");

            XmlNode payloadNode = null;
            foreach (XmlNode child in body.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Element)
                {
                    payloadNode = child;
                    break;
                }
            }

            if (payloadNode == null)
                throw new InvalidOperationException("SOAP body does not contain a response payload.");

            var serializer = new XmlSerializer(typeof(T));
            using var sr = new StringReader(payloadNode.OuterXml);
            return (T)serializer.Deserialize(sr);
        }
    }

    [XmlType(Namespace = "http://cerberusllc.com/common")]
    public class AuthenticatedRequest
    {
        public Credentials credentials;
    }

    [XmlType(Namespace = "http://cerberusllc.com/common")]
    public class Credentials
    {
        public string user;
        public string password;
    }

    [XmlType(Namespace = "http://cerberusllc.com/common")]
    public class VirtualDirectory
    {
        public string name;
        public string path;
        public DirectoryPermissions permissions;
    }

    [XmlType(Namespace = "http://cerberusllc.com/common")]
    public class DirectoryPermissions
    {
        public bool allowListFile;
        public bool allowListDir;
        public bool allowDownload;
        public bool allowUpload;
        public bool allowRename;
        public bool allowDelete;
        public bool allowDirectoryCreation;
        public bool allowDisplayHidden;
    }

    [XmlType(Namespace = "http://cerberusllc.com/common")]
    public class UserPropertyInt
    {
        [XmlAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public int value;

        [XmlIgnore]
        public bool valueSpecified;
    }

    [XmlType(Namespace = "http://cerberusllc.com/common")]
    public class UserPropertyBool
    {
        [XmlAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public bool value;

        [XmlIgnore]
        public bool valueSpecified;
    }

    [XmlType(Namespace = "http://cerberusllc.com/common")]
    public class Password
    {
        [XmlAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public string value;

        [XmlAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public PasswordType type;

        [XmlAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public bool noExpire;

        [XmlAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public DateTime lastChange;
    }

    [XmlType(Namespace = "http://cerberusllc.com/common")]
    public enum PasswordType
    {
        plain,
        sha1,
        sha256,
        sha512,
        md5
    }

    [XmlType(Namespace = "http://cerberusllc.com/common")]
    public class User
    {
        public Password password;
        public UserPropertyBool isAnonymous;
        public UserPropertyBool isSimpleDirectoryMode;
        public UserPropertyBool isDisabled;
        public UserPropertyInt maxLoginsAllowed;

        [XmlArrayItem("root", IsNullable = false)]
        public VirtualDirectory[] rootList;

        [XmlAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public string name;
    }

    [XmlRoot("AddUserRequest", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    [XmlType(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public class AddUserRequest : AuthenticatedRequest
    {
        public User User;
    }

    [XmlRoot("AddUserResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    [XmlType(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public class AddUserResponse
    {
        public bool result;
        public string message;
    }

    [XmlRoot("DeleteUserRequest", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    [XmlType(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public class DeleteUserRequest : AuthenticatedRequest
    {
        public string name;
    }

    [XmlRoot("DeleteUserResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    [XmlType(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public class DeleteUserResponse
    {
        public bool result;
        public string message;
    }

    [XmlRoot("GetUserListRequest", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    [XmlType(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public class GetUserListRequest : AuthenticatedRequest
    {
    }

    [XmlRoot("GetUserListResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    [XmlType(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public class GetUserListResponse
    {
        [XmlElement("UserList")]
        public string[] UserList;

        public bool result;
    }

    [XmlRoot("GetUserInformationRequest", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    [XmlType(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public class GetUserInformationRequest : AuthenticatedRequest
    {
        public string userName;
    }

    [XmlRoot("GetUserInformationResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    [XmlType(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public class GetUserInformationResponse
    {
        public User UserInformation;
        public bool result;
        public string message;
    }
}
