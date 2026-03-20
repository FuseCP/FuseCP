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
using System.Collections.Specialized;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace FuseCP.EnterpriseServer
{
    public interface IKeyValueBunch
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        [XmlIgnore, IgnoreDataMember]
        string this[string settingName] { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string[] GetAllKeys();
    }

    [Serializable]
    public class KeyValueBunch : IKeyValueBunch
    {
        private NameValueCollection _settings;
        private bool hasPendingChanges;

        public string[][] KeyValueArray;

        [XmlIgnore, IgnoreDataMember]
        public bool IsEmpty
        {
            get { return (KeyValueArray == null || KeyValueArray.Length == 0); }
        }

        [XmlIgnore, IgnoreDataMember]
        public bool HasPendingChanges
        {
            get { return hasPendingChanges; }
        }

        [XmlIgnore, IgnoreDataMember]
        NameValueCollection Settings
        {
            get
            {
                //
                SyncCollectionsState(true);
                //
                hasPendingChanges = false;
                //
                return _settings;
            }
        }

        public string this[string settingName]
        {
            get
            {
                return Settings[settingName];
            }
            set
            {
                // check whether changes are really made
                if (!String.Equals(Settings[settingName], value))
                    hasPendingChanges = true;
                // set setting
                Settings[settingName] = value;
                //
                SyncCollectionsState(false);
            }
        }

        private void SyncCollectionsState(bool inputSync)
        {
            if (inputSync)
            {
                if (_settings == null)
                {
                    // create new dictionary
                    _settings = new NameValueCollection();

                    // fill dictionary
                    if (KeyValueArray != null)
                    {
                        foreach (string[] pair in KeyValueArray)
                            _settings.Add(pair[0], pair[1]);
                    }
                }
            }
            else
            {
                // rebuild array
                KeyValueArray = new string[Settings.Count][];
                //
                for (int i = 0; i < Settings.Count; i++)
                {
                    KeyValueArray[i] = new string[] { Settings.Keys[i], Settings[Settings.Keys[i]] };
                }
            }
        }

        public string[] GetAllKeys()
        {
            if (Settings != null)
                return Settings.AllKeys;

            return null;
        }

        public void RemoveKey(string name)
        {
            Settings.Remove(name);
            //
            SyncCollectionsState(false);
        }
    }

    /*public class CommonSettings
    {
        public const string INTERACTIVE = "interactive";
        public const string LIVE_MODE = "live_mode";
        public const string USERNAME = "username";
        public const string PASSWORD = "password";
    }*/

}
