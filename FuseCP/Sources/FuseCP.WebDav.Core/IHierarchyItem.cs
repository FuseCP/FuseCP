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
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace FuseCP.WebDav.Core
{
    namespace Client
    {
        public interface IHierarchyItem : IConnectionSettings
        {
            string Comment { get; }
            DateTime CreationDate { get; }
            string CreatorDisplayName { get; }
            string DisplayName { get; }
            bool IsRootItem { get; set; }
            Uri Href { get; }
            ItemType ItemType { get;}
            DateTime LastModified { get; }
            Property[] Properties { get; }

            Property[] GetAllProperties();
            PropertyName[] GetPropertyNames();
            Property[] GetPropertyValues(PropertyName[] names);
            void Delete();
        }

        public class WebDavHierarchyItem : WebDavConnectionSettings, IHierarchyItem
        {
            private Uri _baseUri;
            private string _comment = "";
            private DateTime _creationDate = new DateTime(0);
            private string _creatorDisplayName = "";
            protected ICredentials _credentials = new NetworkCredential();
            private Uri _href;
            private ItemType _itemType;
            private DateTime _lastModified = new DateTime(0);
            private Property[] _properties = {};

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
                    var href = HttpUtility.UrlDecode(_href.AbsoluteUri);
                    var baseUri = HttpUtility.UrlDecode(_baseUri.AbsoluteUri);

                    string displayName = href.Replace(baseUri, "");
                    displayName = Regex.Replace(displayName, "\\/$", "");
                    Match displayNameMatch = Regex.Match(displayName, "([\\/]+)$");
                    if (displayNameMatch.Success)
                    {
                        displayName = displayNameMatch.Groups[1].Value;
                    }
                    return HttpUtility.UrlDecode(displayName);
                }
            }

            public bool IsRootItem { get; set; }

            public Uri Href
            {
                get { return _href; }
                set { SetHref(value.ToString(), new Uri(value.Scheme + "://" + value.Host + value.Segments[0] + value.Segments[1])); }
            }

            public ItemType ItemType
            {
                get { return _itemType; }
                set { SetItemType(value); }
            }

            public DateTime LastModified
            {
                get { return _lastModified; }
            }

            public Property[] Properties
            {
                get { return _properties; }
            }

            public Property[] GetAllProperties()
            {
                return _properties;
            }

            public PropertyName[] GetPropertyNames()
            {
                return _properties.Select(p => p.Name).ToArray();
            }

            public Property[] GetPropertyValues(PropertyName[] names)
            {
                return (from p in _properties from pn in names where pn.Equals(p.Name) select p).ToArray();
            }

            public void Delete()
            {
                var credentials = (NetworkCredential) _credentials;
                string auth = "Basic " +
                              Convert.ToBase64String(
                                  Encoding.Default.GetBytes((credentials?.UserName ?? string.Empty) + ":" + (credentials?.Password ?? string.Empty)));
                var handler = new HttpClientHandler();
                if (credentials != null)
                {
                    handler.Credentials = credentials;
                }

                using (var client = new HttpClient(handler))
                using (var request = new HttpRequestMessage(HttpMethod.Delete, Href))
                {
                    request.Headers.TryAddWithoutValidation("Authorization", auth);
                    using (var response = client.Send(request))
                    {
                        response.EnsureSuccessStatusCode();
                    }
                }
            }

            public void Delete(LockUriTokenPair[] lockTokens)
            {
            }

            public void Delete(string lockToken)
            {
            }

            public void SetComment(string comment)
            {
                _comment = comment;
            }

            public void SetCreationDate(string creationDate)
            {
                _creationDate = DateTime.Parse(creationDate);
            }
            
            public void SetCreationDate(DateTime creationDate)
            {
                _creationDate = creationDate;
            }

            public void SetCreatorDisplayName(string creatorDisplayName)
            {
                _creatorDisplayName = creatorDisplayName;
            }

            public void SetItemType(ItemType itemType)
            {
                _itemType = itemType;
            }

            public void SetHref(string href, Uri baseUri)
            {
                _href = new Uri(href);
                _baseUri = baseUri;
            }

            public void SetLastModified(string lastModified)
            {
                _lastModified = DateTime.Parse(lastModified);
            }

            public void SetLastModified(DateTime lastModified)
            {
                _lastModified = lastModified;
            }

            public void SetProperty(Property property)
            {
                if (property.Name.Name == "resourcetype" && property.StringValue != String.Empty)
                {
                    var XmlDoc = new XmlDocument();
                    try
                    {
                        if (property.StringValue == "collection")
                        {
                            _itemType = ItemType.Folder;
                        }
                        else
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
                    }
                    catch (AmbiguousMatchException)
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
                    newProperties[_properties.Length] = property;
                    _properties = newProperties;
                }
            }

            public void SetProperty(PropertyName propertyName, string value)
            {
                SetProperty(new Property(propertyName, value));
            }

            public void SetProperty(string name, string nameSpace, string value)
            {
                SetProperty(new Property(name, nameSpace, value));
            }

            public void SetCredentials(ICredentials credentials)
            {
                _credentials = credentials;
            }
        }
    }
}
