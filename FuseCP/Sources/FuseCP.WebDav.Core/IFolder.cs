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
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using FuseCP.WebDav.Core.Config;
using FuseCP.WebDav.Core.Exceptions;
using System.Reflection;

namespace FuseCP.WebDav.Core
{
    namespace Client
    {
        public interface IFolder : IHierarchyItem
        {
            IResource CreateResource(string name);
            IFolder CreateFolder(string name);
            IHierarchyItem[] GetChildren();
            IResource GetResource(string name);
            Uri Path { get; }
        }

        public class WebDavFolder : WebDavHierarchyItem, IFolder
        {
            private IHierarchyItem[] _children = new IHierarchyItem[0];
            private Uri _path;

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

            public Uri Path { get { return _path; } }

            /// <summary>
            ///     The constructor
            /// </summary>
            public WebDavFolder()
            {
            }

            /// <summary>
            ///     The constructor
            /// </summary>
            /// <param name="path">Path to the folder.</param>
            public WebDavFolder(string path)
            {
                _path = new Uri(path);
            }

            /// <summary>
            ///     The constructor
            /// </summary>
            /// <param name="path">Path to the folder.</param>
            public WebDavFolder(Uri path)
            {
                _path = path;
            }

            /// <summary>
            ///     Creates a resource with a specified name.
            /// </summary>
            /// <param name="name">Name of the new resource.</param>
            /// <returns>Newly created resource.</returns>
            public IResource CreateResource(string name)
            {
                var resource = new WebDavResource();
                try
                {
                    resource.SetHref(new Uri(Href.AbsoluteUri + name));
                    var credentials = (NetworkCredential) _credentials;
                    using (var client = CreateHttpClient(credentials))
                    using (var request = new HttpRequestMessage(HttpMethod.Put, resource.Href))
                    {
                        request.Headers.TryAddWithoutValidation("Authorization", BuildBasicAuthHeader(credentials));
                        request.Headers.TryAddWithoutValidation("translate", "f");
                        request.Headers.TryAddWithoutValidation("Accept", "text/xml");
                        request.Content = new ByteArrayContent(Array.Empty<byte>());
                        request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/xml");

                        using (var response = client.Send(request))
                        {
                            if (response.StatusCode == HttpStatusCode.Created ||
                                response.StatusCode == HttpStatusCode.NoContent)
                            {
                                Open(Href);
                                resource = (WebDavResource)GetResource(name);
                                resource.SetCredentials(_credentials);
                            }
                        }
                    }
                }
                catch (AmbiguousMatchException)
                {
                }

                return resource;
            }

            /// <summary>
            ///     Creates new folder with specified name as child of this one.
            /// </summary>
            /// <param name="name">Name of the new folder.</param>
            /// <returns>IFolder</returns>
            public IFolder CreateFolder(string name)
            {
                var folder = new WebDavFolder();
                try
                {
                    var credentials = (NetworkCredential) _credentials;
                    using (var client = CreateHttpClient(credentials))
                    using (var request = new HttpRequestMessage(new HttpMethod("MKCOL"), Href.AbsoluteUri + name))
                    {
                        request.Headers.TryAddWithoutValidation("Authorization", BuildBasicAuthHeader(credentials));

                        using (var response = client.Send(request))
                        {
                            if (response.StatusCode == HttpStatusCode.Created ||
                                response.StatusCode == HttpStatusCode.NoContent)
                            {
                                folder.SetCredentials(_credentials);
                                folder.Open(Href.AbsoluteUri + name + "/");
                            }
                        }
                    }
                }
                catch (AmbiguousMatchException)
                {
                }

                return folder;
            }

            /// <summary>
            ///     Returns children of this folder.
            /// </summary>
            /// <returns>Array that include child folders and resources.</returns>
            public IHierarchyItem[] GetChildren()
            {
                return _children;
            }

            /// <summary>
            ///     Gets the specified resource from server.
            /// </summary>
            /// <param name="name">Name of the resource.</param>
            /// <returns>Resource corresponding to requested name.</returns>
            public IResource GetResource(string name)
            {
                try
                {
                    IHierarchyItem item =
                        _children.Single(i => i.DisplayName.ToLowerInvariant().Trim('/') == name.ToLowerInvariant().Trim('/'));
                    var resource = new WebDavResource();
                    resource.SetCredentials(_credentials);
                    resource.SetHierarchyItem(item);
                    return resource;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            /// <summary>
            ///     Opens the folder.
            /// </summary>
            public void Open()
            {
                var credentials = (NetworkCredential)_credentials;
                using (var client = CreateHttpClient(credentials, ignoreServerCertificateErrors: true))
                using (var request = new HttpRequestMessage(new HttpMethod("PROPFIND"), _path))
                {
                    request.Headers.TryAddWithoutValidation("Depth", "1");
                    if (credentials != null && credentials.UserName != null)
                    {
                        request.Headers.TryAddWithoutValidation("Authorization", BuildBasicAuthHeader(credentials));
                    }
                    request.Content = new ByteArrayContent(Array.Empty<byte>());
                    request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/xml");

                    using (var response = client.Send(request))
                    {
                        if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                        {
                            throw new UnauthorizedException();
                        }

                        response.EnsureSuccessStatusCode();
                        string responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        ProcessResponse(responseString);
                    }
                }
                
                try
                {
                }
                catch (WebException e)
                {
                    if (e.Status == WebExceptionStatus.ProtocolError)
                    {
                        throw new UnauthorizedException();
                    }
                    throw;
                }
            }

            /// <summary>
            ///     Opens the folder
            /// </summary>
            /// <param name="path">Path of the folder to open.</param>
            public void Open(string path)
            {
                _path = new Uri(path);
                Open();
            }

            /// <summary>
            ///     Opens the folder
            /// </summary>
            /// <param name="path">Path of the folder to open.</param>
            public void Open(Uri path)
            {
                _path = path;
                Open();
            }

            public void OpenPaged(string path)
            {
                _path = new Uri(path);
                OpenPaged();
            }

            public void OpenPaged()
            {
                var credentials = (NetworkCredential)_credentials;
                using (var client = CreateHttpClient(credentials, ignoreServerCertificateErrors: true))
                using (var request = new HttpRequestMessage(new HttpMethod("SEARCH"), _path))
                {
                    if (credentials != null && credentials.UserName != null)
                    {
                        request.Headers.TryAddWithoutValidation("Authorization", BuildBasicAuthHeader(credentials));
                    }

                    var strQuery = "<?xml version=\"1.0\"?><D:searchrequest xmlns:D = \"DAV:\" >"
                            + "<D:sql>SELECT \"DAV:displayname\" FROM \"" + _path + "\""
                            + "WHERE \"DAV:ishidden\" = false"
                            + "</D:sql></D:searchrequest>";

                    request.Content = new StringContent(strQuery, Encoding.UTF8, "text/xml");

                    using (var response = client.Send(request))
                    {
                        if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                        {
                            throw new UnauthorizedException();
                        }

                        response.EnsureSuccessStatusCode();
                        string responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        ProcessResponse(responseString);
                    }
                }
            }

            /// <summary>
            ///     Processes the response from the server.
            /// </summary>
            /// <param name="response">The raw response from the server.</param>
            private void ProcessResponse(string response)
            {
                try
                {
                    var XmlDoc = new XmlDocument();
                    XmlDoc.LoadXml(response);
                    XmlNodeList XmlResponseList = XmlDoc.GetElementsByTagName("D:response");
                    if (XmlResponseList.Count == 0)
                    {
                        XmlResponseList = XmlDoc.GetElementsByTagName("d:response");
                    }
                    var children = new WebDavResource[XmlResponseList.Count];
                    int counter = 0;
                    foreach (XmlNode XmlCurrentResponse in XmlResponseList)
                    {
                        var item = new WebDavResource();
                        item.SetCredentials(_credentials);

                        foreach (XmlNode XmlCurrentNode in XmlCurrentResponse.ChildNodes)
                        {
                            switch (XmlCurrentNode.LocalName)
                            {
                                case "href":
                                    string href = XmlCurrentNode.InnerText;
                                    if (!Regex.Match(href, "^https?:\\/\\/").Success)
                                    {
                                        href = _path.Scheme + "://" + _path.Host + href;
                                    }
                                    item.SetHref(href, _path);
                                    break;

                                case "propstat":
                                    foreach (XmlNode XmlCurrentPropStatNode in XmlCurrentNode)
                                    {
                                        switch (XmlCurrentPropStatNode.LocalName)
                                        {
                                            case "prop":
                                                foreach (XmlNode XmlCurrentPropNode in XmlCurrentPropStatNode)
                                                {
                                                    switch (XmlCurrentPropNode.LocalName)
                                                    {
                                                        case "creationdate":
                                                            item.SetCreationDate(XmlCurrentPropNode.InnerText);
                                                            break;

                                                        case "getcontentlanguage":
                                                            item.SetProperty(
                                                                new Property(
                                                                    new PropertyName("getcontentlanguage",
                                                                        XmlCurrentPropNode.NamespaceURI),
                                                                    XmlCurrentPropNode.InnerText));
                                                            break;

                                                        case "getcontentlength":
                                                            item.SetProperty(
                                                                new Property(
                                                                    new PropertyName("getcontentlength",
                                                                        XmlCurrentPropNode.NamespaceURI),
                                                                    XmlCurrentPropNode.InnerText));
                                                            break;
                                                        case "getcontenttype":
                                                            item.SetProperty(
                                                                new Property(
                                                                    new PropertyName("getcontenttype",
                                                                        XmlCurrentPropNode.NamespaceURI),
                                                                    XmlCurrentPropNode.InnerText));
                                                            break;

                                                        case "getlastmodified":
                                                            item.SetLastModified(XmlCurrentPropNode.InnerText);
                                                            break;

                                                        case "resourcetype":
                                                            item.SetProperty(
                                                                new Property(
                                                                    new PropertyName("resourcetype",
                                                                        XmlCurrentPropNode.NamespaceURI),
                                                                    XmlCurrentPropNode.InnerXml));
                                                            break;
                                                        //case "lockdiscovery":
                                                        //{
                                                        //    if (XmlCurrentPropNode.HasChildNodes == false)
                                                        //    {
                                                        //        break;
                                                        //    }

                                                        //    foreach (XmlNode activeLockNode in XmlCurrentPropNode.FirstChild)
                                                        //    {
                                                        //        switch (activeLockNode.LocalName)
                                                        //        {
                                                        //            case "owner":
                                                        //                item.SetProperty(
                                                        //                    new Property(
                                                        //                        new PropertyName("owner",
                                                        //                            activeLockNode.NamespaceURI),
                                                        //                        activeLockNode.InnerXml));
                                                        //                break;
                                                        //            case "locktoken":
                                                        //                var lockTokenNode = activeLockNode.FirstChild;
                                                        //                item.SetProperty(
                                                        //                    new Property(
                                                        //                        new PropertyName("locktoken",
                                                        //                            lockTokenNode.NamespaceURI),
                                                        //                        lockTokenNode.InnerXml));
                                                        //                break;
                                                        //        }
                                                        //    }
                                                        //    break;
                                                        //}
                                                    }
                                                }
                                                break;

                                            case "status":
                                                break;
                                        }
                                    }
                                    break;
                            }
                        }

                        if (item.DisplayName != String.Empty)
                        {
                            children[counter] = item;
                            counter++;
                        }
                        else
                        {
                            SetItemType(ItemType.Folder);
                            SetHref(item.Href.AbsoluteUri, item.Href);
                            SetCreationDate(item.CreationDate);
                            SetComment(item.Comment);
                            SetCreatorDisplayName(item.CreatorDisplayName);
                            SetLastModified(item.LastModified);

                            foreach (Property property in item.Properties)
                            {
                                SetProperty(property);
                            }
                        }
                    }

                    int childrenCount = 0;
                    foreach (IHierarchyItem item in children)
                    {
                        if (item != null)
                        {
                            childrenCount++;
                        }
                    }
                    _children = new IHierarchyItem[childrenCount];

                    counter = 0;
                    foreach (IHierarchyItem item in children)
                    {
                        if (item != null)
                        {
                            _children[counter] = item;
                            counter++;
                        }
                    }
                }
                catch (AmbiguousMatchException)
                {
                }
            }
        }
    }
}
