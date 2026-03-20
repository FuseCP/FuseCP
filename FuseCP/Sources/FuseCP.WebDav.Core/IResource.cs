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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Xml;

namespace FuseCP.WebDav.Core
{
    namespace Client
    {
        public interface IResource : IItemContent, IHierarchyItem, IConnectionSettings
        {
            bool CheckedOut { get; }
            bool VersionControlled { get; }
        }

        public class WebDavResource : IResource
        {
            // IResource
            private Uri _baseUri;
            private bool _checkedOut = false;
            private string _comment = "";
            private long _contentLength;
            private DateTime _creationDate = new DateTime(0);
            private string _creatorDisplayName = "";
            private ICredentials _credentials = new NetworkCredential();
            private Uri _href;
            private ItemType _itemType;
            private DateTime _lastModified = new DateTime(0);
            private Property[] _properties = {};
            private int _timeOut = 30000;
            private bool _versionControlled = false;

            private string BuildBasicAuthHeader(NetworkCredential credentials)
            {
                return "Basic " + Convert.ToBase64String(
                    Encoding.Default.GetBytes((credentials?.UserName ?? string.Empty) + ":" + (credentials?.Password ?? string.Empty)));
            }

            private HttpClient CreateHttpClient(NetworkCredential credentials, bool ignoreServerCertificateErrors = false)
            {
                var handler = new HttpClientHandler();
                if (credentials != null)
                {
                    handler.Credentials = credentials;
                }

                if (ignoreServerCertificateErrors)
                {
                    handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
                }

                var client = new HttpClient(handler);
                if (TimeOut == Timeout.Infinite)
                {
                    client.Timeout = Timeout.InfiniteTimeSpan;
                }
                else
                {
                    client.Timeout = TimeSpan.FromMilliseconds(TimeOut);
                }

                return client;
            }

            public WebDavResource()
            {
                SendChunked = false;
                AllowWriteStreamBuffering = false;
            }

            public WebDavResource(ICredentials credentials, IHierarchyItem item)
            {
                SendChunked = false;
                AllowWriteStreamBuffering = false;

                IsRootItem = item.IsRootItem;
                SetCredentials(credentials);
                SetHierarchyItem(item);
            }

            public Uri BaseUri
            {
                get { return _baseUri; }
            }

            public bool CheckedOut
            {
                get { return _checkedOut; }
            }

            public bool VersionControlled
            {
                get { return _versionControlled; }
            }

            // IItemContent

            public long ContentLength
            {
                get { return _contentLength; }
                set { _contentLength = value; }
            }

            public string ContentType
            {
                get
                {
                    {
                        var property = _properties.FirstOrDefault(x => x.Name.Name == "getcontenttype");
                        return property == null ? MediaTypeNames.Application.Octet : property.StringValue;
                    }
                }
            }

            public string Summary { get; set; }

            /// <summary>
            ///     Downloads content of the resource to a file specified by filename
            /// </summary>
            /// <param name="filename">Full path of a file to be downloaded to</param>
            public void Download(string filename)
            {
                File.WriteAllBytes(filename, Download());
            }

            public byte[] Download()
            {
                var credentials = (NetworkCredential)_credentials;
                using (var client = CreateHttpClient(credentials))
                using (var request = new HttpRequestMessage(HttpMethod.Get, _href))
                {
                    request.Headers.TryAddWithoutValidation("Authorization", BuildBasicAuthHeader(credentials));
                    using (HttpResponseMessage response = client.Send(request))
                    {
                        response.EnsureSuccessStatusCode();
                        return response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
                    }
                }
            }

            /// <summary>
            ///     Uploads content of a file specified by filename to the server
            /// </summary>
            /// <param name="filename">Full path of a file to be uploaded from</param>
            public void Upload(string filename)
            {
                Upload(File.ReadAllBytes(filename));
            }

            /// <summary>
            ///     Uploads content of a file specified by filename to the server
            /// </summary>
            /// <param name="data">Posted file data to be uploaded</param>
            public void Upload(byte[] data)
            {
                var credentials = (NetworkCredential)_credentials;
                using (var client = CreateHttpClient(credentials, ignoreServerCertificateErrors: true))
                using (var request = new HttpRequestMessage(HttpMethod.Put, Href))
                {
                    request.Headers.TryAddWithoutValidation("Authorization", BuildBasicAuthHeader(credentials));
                    request.Content = new ByteArrayContent(data ?? Array.Empty<byte>());
                    using (HttpResponseMessage response = client.Send(request))
                    {
                        response.EnsureSuccessStatusCode();
                    }
                }
            }

            /// <summary>
            ///     Loads content of the resource from WebDAV server.
            /// </summary>
            /// <returns>Stream to read resource content.</returns>
            public Stream GetReadStream()
            {
                var credentials = (NetworkCredential) _credentials;
                using (var client = CreateHttpClient(credentials, ignoreServerCertificateErrors: true))
                using (var request = new HttpRequestMessage(HttpMethod.Get, _href))
                {
                    request.Headers.TryAddWithoutValidation("Authorization", BuildBasicAuthHeader(credentials));
                    using (HttpResponseMessage response = client.Send(request))
                    {
                        response.EnsureSuccessStatusCode();
                        byte[] bytes = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
                        return new MemoryStream(bytes, writable: false);
                    }
                }
            }

            /// <summary>
            ///     Saves resource's content to WebDAV server.
            /// </summary>
            /// <param name="contentLength">Length of data to be written.</param>
            /// <returns>Stream to write resource content.</returns>
            public Stream GetWriteStream(long contentLength)
            {
                return GetWriteStream("application/octet-stream", contentLength);
            }

            /// <summary>
            ///     Saves resource's content to WebDAV server.
            /// </summary>
            /// <param name="contentType">Media type of the resource.</param>
            /// <param name="contentLength">Length of data to be written.</param>
            /// <returns>Stream to write resource content.</returns>
            public Stream GetWriteStream(string contentType, long contentLength)
            {
                var tcpClient = new TcpClient(Href.Host, Href.Port);
                if (tcpClient.Connected)
                {
                    var credentials = (NetworkCredential) _credentials;
                    string auth = "Basic " +
                                  Convert.ToBase64String(
                                      Encoding.Default.GetBytes(credentials.UserName + ":" + credentials.Password));

                    try
                    {
                        if (TimeOut != Timeout.Infinite)
                        {
                            tcpClient.SendTimeout = TimeOut;
                            tcpClient.ReceiveTimeout = TimeOut;
                        }
                        else
                        {
                            tcpClient.SendTimeout = 0;
                            tcpClient.ReceiveTimeout = 0;
                        }
                    }
                    catch (SocketException)
                    {
                        tcpClient.SendTimeout = 0;
                        tcpClient.ReceiveTimeout = 0;
                    }
                    NetworkStream networkStream = tcpClient.GetStream();
                    if (networkStream.CanTimeout)
                    {
                        try
                        {
                            networkStream.WriteTimeout = TimeOut;
                            networkStream.ReadTimeout = TimeOut;
                        }
                        catch (Exception)
                        {
                        }
                    }
                    byte[] methodBuffer = Encoding.UTF8.GetBytes("PUT " + Href.AbsolutePath + " HTTP/1.1\r\n");
                    byte[] hostBuffer = Encoding.UTF8.GetBytes("Host: " + Href.Host + "\r\n");
                    byte[] contentLengthBuffer = Encoding.UTF8.GetBytes("Content-Length: " + contentLength + "\r\n");
                    byte[] authorizationBuffer = Encoding.UTF8.GetBytes("Authorization: " + auth + "\r\n");
                    byte[] connectionBuffer = Encoding.UTF8.GetBytes("Connection: Close\r\n\r\n");
                    networkStream.Write(methodBuffer, 0, methodBuffer.Length);
                    networkStream.Write(hostBuffer, 0, hostBuffer.Length);
                    networkStream.Write(contentLengthBuffer, 0, contentLengthBuffer.Length);
                    networkStream.Write(authorizationBuffer, 0, authorizationBuffer.Length);
                    networkStream.Write(connectionBuffer, 0, connectionBuffer.Length);

                    return networkStream;
                }

                throw new IOException("could not connect to server");
            }

            // IHierarchyItem

            public string Comment
            {
                get { return _comment; }
            }

            public DateTime CreationDate
            {
                get { return _creationDate; }
            }

            public string CreatorDisplayName
            {
                get { return _creatorDisplayName; }
            }

            public string DisplayName
            {
                get
                {
                    string displayName = _href.ToString().Trim('/').Replace(_baseUri.ToString().Trim('/'), "");
                    displayName = Regex.Replace(displayName, "\\/$", "");
                    Match displayNameMatch = Regex.Match(displayName, "([\\/]+)$");
                    if (displayNameMatch.Success)
                    {
                        displayName = displayNameMatch.Groups[1].Value;
                    }
                    return HttpUtility.UrlDecode(displayName.Trim('/'));
                }
            }

            public long AllocatedSpace { get; set; }
            public bool IsRootItem { get; set; }

            public Uri Href
            {
                get { return _href; }
                set { SetHref(value.ToString(), new Uri(value.Scheme + "://" + value.Host + value.Segments[0] + value.Segments[1])); }
            }

            public ItemType ItemType
            {
                get { return _itemType; }
                set { _itemType = value; }
            }

            public DateTime LastModified
            {
                get { return _lastModified; }
            }

            public Property[] Properties
            {
                get { return _properties; }
            }

            // IHierarchyItem Methods
            /// <summary>
            ///     Retrieves all custom properties exposed by the item.
            /// </summary>
            /// <returns>This method returns the array of custom properties exposed by the item.</returns>
            public Property[] GetAllProperties()
            {
                return _properties;
            }

            /// <summary>
            ///     Returns names of all custom properties exposed by this item.
            /// </summary>
            /// <returns></returns>
            public PropertyName[] GetPropertyNames()
            {
                return _properties.Select(p => p.Name).ToArray();
            }

            /// <summary>
            ///     Retrieves values of specific properties.
            /// </summary>
            /// <param name="names"></param>
            /// <returns>Array of requested properties with values.</returns>
            public Property[] GetPropertyValues(PropertyName[] names)
            {
                return (from p in _properties from pn in names where pn.Equals(p.Name) select p).ToArray();
            }

            /// <summary>
            ///     Deletes this item.
            /// </summary>
            public void Delete()
            {
                var credentials = (NetworkCredential) _credentials;
                using (var client = CreateHttpClient(credentials))
                using (var request = new HttpRequestMessage(HttpMethod.Delete, Href))
                {
                    request.Headers.TryAddWithoutValidation("Authorization", BuildBasicAuthHeader(credentials));
                    using (HttpResponseMessage response = client.Send(request))
                    {
                        response.EnsureSuccessStatusCode();
                    }
                }
            }


            /// <summary>
            ///     Lock this item.
            /// </summary>
            public string Lock()
            {
                var credentials = (NetworkCredential)_credentials;
                string lockToken = string.Empty;


                string lockXml =string.Format( "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" +
                                 "<D:lockinfo xmlns:D='DAV:'>" +
                                 "<D:lockscope><D:exclusive/></D:lockscope>" +
                                 "<D:locktype><D:write/></D:locktype>" +
                                 "<D:owner>{0}</D:owner>" +
                                 "</D:lockinfo>", ScpContext.User.Login);

                using (var client = CreateHttpClient(credentials))
                using (var request = new HttpRequestMessage(new HttpMethod("LOCK"), Href))
                {
                    request.Headers.TryAddWithoutValidation("Authorization", BuildBasicAuthHeader(credentials));
                    request.Headers.TryAddWithoutValidation("Prefer", "return=representation");
                    request.Content = new StringContent(lockXml, Encoding.UTF8, "application/xml");

                    using (HttpResponseMessage response = client.Send(request))
                    {
                        response.EnsureSuccessStatusCode();
                        if (response.Headers.TryGetValues("Lock-Token", out var values))
                        {
                            lockToken = values.FirstOrDefault() ?? string.Empty;
                        }
                    }
                }

                return lockToken;
            }

            /// <summary>
            ///     Lock this item.
            /// </summary>
            public void UnLock()
            {
                var credentials = (NetworkCredential)_credentials;
                using (var client = CreateHttpClient(credentials))
                using (var request = new HttpRequestMessage(new HttpMethod("UNLOCK"), Href))
                {
                    request.Headers.TryAddWithoutValidation("Authorization", BuildBasicAuthHeader(credentials));
                    request.Headers.TryAddWithoutValidation("Lock-Token", Properties.First(x => x.Name.Name == "locktoken").StringValue);
                    using (HttpResponseMessage response = client.Send(request))
                    {
                        response.EnsureSuccessStatusCode();
                    }
                }
            }

            public bool AllowWriteStreamBuffering { get; set; }
            public bool SendChunked { get; set; }

            public int TimeOut
            {
                get { return _timeOut; }
                set { _timeOut = value; }
            }

            /// <summary>
            ///     For internal use only.
            /// </summary>
            /// <param name="comment"></param>
            public void SetComment(string comment)
            {
                _comment = comment;
            }

            /// <summary>
            ///     For internal use only.
            /// </summary>
            /// <param name="comment"></param>
            public void SetCreationDate(string creationDate)
            {
                _creationDate = DateTime.Parse(creationDate);
            }

            /// <summary>
            ///     For internal use only.
            /// </summary>
            /// <param name="comment"></param>
            public void SetCreationDate(DateTime creationDate)
            {
                _creationDate = creationDate;
            }

            /// <summary>
            ///     For internal use only.
            /// </summary>
            /// <param name="comment"></param>
            public void SetCreatorDisplayName(string creatorDisplayName)
            {
                _creatorDisplayName = creatorDisplayName;
            }

            /// <summary>
            ///     For internal use only.
            /// </summary>
            /// <param name="comment"></param>
            public void SetHref(string href, Uri baseUri)
            {
                _href = new Uri(href);
                _baseUri = baseUri;
            }

            /// <summary>
            ///     For internal use only.
            /// </summary>
            /// <param name="comment"></param>
            public void SetHref(Uri href)
            {
                _href = href;

                var baseUrl = href.ToString().Remove(href.ToString().Length - href.ToString().Trim('/').Split('/').Last().Length);

                _baseUri = new Uri(baseUrl);
            }

            /// <summary>
            ///     For internal use only.
            /// </summary>
            /// <param name="comment"></param>
            public void SetLastModified(string lastModified)
            {
                _lastModified = DateTime.Parse(lastModified);
            }

            /// <summary>
            ///     For internal use only.
            /// </summary>
            /// <param name="comment"></param>
            public void SetLastModified(DateTime lastModified)
            {
                _lastModified = lastModified;
            }

            /// <summary>
            ///     For internal use only.
            /// </summary>
            /// <param name="comment"></param>
            public void SetItemType(ItemType type)
            {
                _itemType = type;
            }

            /// <summary>
            ///     For internal use only.
            /// </summary>
            /// <param name="comment"></param>
            public void SetProperty(Property property)
            {
                if (property.Name.Name == "resourcetype" && property.StringValue != String.Empty)
                {
                    var XmlDoc = new XmlDocument();
                    try
                    {
                        XmlDoc.LoadXml(property.StringValue);
                        property.StringValue = XmlDoc.DocumentElement.LocalName;
                        switch (property.StringValue)
                        {
                            case "collection":
                                _itemType = ItemType.Folder;
                                break;

                            default:
                                break;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                bool propertyFound = false;
                foreach (Property prop in _properties)
                {
                    if (prop.Name.Equals(property.Name))
                    {
                        prop.StringValue = property.StringValue;
                        propertyFound = true;
                    }
                }

                if (!propertyFound)
                {
                    var newProperties = new Property[_properties.Length + 1];
                    for (int i = 0; i < _properties.Length; i++)
                    {
                        newProperties[i] = _properties[i];
                    }
                    if (property.Name.Name == "getcontentlength")
                    {
                        try
                        {
                            _contentLength = Convert.ToInt64(property.StringValue);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    newProperties[_properties.Length] = property;
                    _properties = newProperties;
                }
            }

            /// <summary>
            ///     For internal use only.
            /// </summary>
            /// <param name="comment"></param>
            public void SetProperty(PropertyName propertyName, string value)
            {
                SetProperty(new Property(propertyName, value));
            }

            /// <summary>
            ///     For internal use only.
            /// </summary>
            /// <param name="comment"></param>
            public void SetProperty(string name, string nameSpace, string value)
            {
                SetProperty(new Property(name, nameSpace, value));
            }

            /// <summary>
            ///     For internal use only.
            /// </summary>
            /// <param name="comment"></param>
            public void SetProperties(Property[] properties)
            {
                foreach (Property property in properties)
                {
                    SetProperty(property);
                }
            }

            /// <summary>
            ///     For internal use only.
            /// </summary>
            /// <param name="comment"></param>
            public void SetCredentials(ICredentials credentials)
            {
                _credentials = credentials;
            }

            // IConnectionSettings

            /// <summary>
            ///     For internal use only.
            /// </summary>
            /// <param name="comment"></param>
            public void SetHierarchyItem(IHierarchyItem item)
            {
                SetComment(item.Comment);
                SetCreationDate(item.CreationDate);
                SetCreatorDisplayName(item.CreatorDisplayName);
                SetHref(item.Href);
                SetLastModified(item.LastModified);
                SetProperties(item.Properties);
                SetItemType(item.ItemType);
            }
        }
    }
}
