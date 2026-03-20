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
using System.Data;
using System.Web;
using System.Collections;
using FuseCP.Web.Services;
using System.ComponentModel;
using FuseCP.Providers;
using FuseCP.Server.Utils;

namespace FuseCP.Server
{
    /// <summary>
    /// Summary description for ServiceProvider
    /// </summary>
    [WebService(Namespace = "http://smbsaas/fusecp/server/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [Policy("ServerPolicy")]
    [ToolboxItem(false)]
    public class ServiceProvider : HostingServiceProviderWebService
    {
        [WebMethod, SoapHeader("settings")]
        public string[] Install()
        {
            try
            {
                Log.WriteStart("'{0}' Install", ProviderSettings.ProviderName);
                return Provider.Install();
            }
            catch (Exception ex)
            {
                Log.WriteError(String.Format("Can't Install '{0}' provider", ProviderSettings.ProviderName), ex);
                throw;
            }
            finally
            {
                Log.WriteEnd("'{0}' Install", ProviderSettings.ProviderName);
            }
        }

        [WebMethod, SoapHeader("settings")]
        public SettingPair[] GetProviderDefaultSettings()
        {
            try
            {
                Log.WriteStart("'{0}' GetProviderDefaultSettings", ProviderSettings.ProviderName);
                return Provider.GetProviderDefaultSettings();
            }
            catch (Exception ex)
            {
				Log.WriteError(String.Format("Can't GetProviderDefaultSettings '{0}' provider", ProviderSettings.ProviderName), ex);
                throw;
            }
            finally
            {
				Log.WriteEnd("'{0}' GetProviderDefaultSettings", ProviderSettings.ProviderName);
            }
        }

        [WebMethod, SoapHeader("settings")]
        public void Uninstall()
        {
            try
            {
                Log.WriteStart("'{0}' Uninstall", ProviderSettings.ProviderName);
                Provider.Uninstall();
                Log.WriteEnd("'{0}' Uninstall", ProviderSettings.ProviderName);
            }
            catch (Exception ex)
            {
                Log.WriteError(String.Format("Can't Uninstall '{0}' provider", ProviderSettings.ProviderName), ex);
                throw;
            }
        }

        [WebMethod, SoapHeader("settings")]
        public bool IsInstalled()
        {
            try
            {
                Log.WriteStart("'{0}' IsInstalled", ProviderSettings.ProviderName);
                return Provider.IsInstalled();
            }
            catch (Exception ex)
            {
                Log.WriteError(String.Format("Can't check '{0}' provider IsInstalled", ProviderSettings.ProviderName), ex);
                throw;
            }
            finally
            {
                Log.WriteEnd("'{0}' IsInstalled", ProviderSettings.ProviderName);
            }
        }

        [WebMethod, SoapHeader("settings")]
        public void ChangeServiceItemsState(SoapServiceProviderItem[] items, bool enabled)
        {
            try
            {
                Log.WriteStart("'{0}' ChangeServiceItemsState", ProviderSettings.ProviderName);
                Provider.ChangeServiceItemsState(UnwrapServiceProviderItems(items), enabled);
                Log.WriteEnd("'{0}' ChangeServiceItemsState", ProviderSettings.ProviderName);
            }
            catch (Exception ex)
            {
                Log.WriteError(String.Format("Error on ChangeServiceItemsState() in '{0}' provider", ProviderSettings.ProviderName), ex);
                throw;
            }
        }

        [WebMethod, SoapHeader("settings")]
        public void DeleteServiceItems(SoapServiceProviderItem[] items)
        {
            try
            {
                Log.WriteStart("'{0}' DeleteServiceItems", ProviderSettings.ProviderName);
                Provider.DeleteServiceItems(UnwrapServiceProviderItems(items));
                Log.WriteEnd("'{0}' DeleteServiceItems", ProviderSettings.ProviderName);
            }
            catch (Exception ex)
            {
                Log.WriteError(String.Format("Error on DeleteServiceItems() in '{0}' provider", ProviderSettings.ProviderName), ex);
                throw;
            }
        }

        [WebMethod, SoapHeader("settings")]
        public ServiceProviderItemDiskSpace[] GetServiceItemsDiskSpace(SoapServiceProviderItem[] items)
        {
            try
            {
                Log.WriteStart("'{0}' GetServiceItemsDiskSpace", ProviderSettings.ProviderName);

                if (items.Length == 0) return new ServiceProviderItemDiskSpace[] {};
                return Provider.GetServiceItemsDiskSpace(UnwrapServiceProviderItems(items));
            }
            catch (Exception ex)
            {
                Log.WriteError(String.Format("Error on GetServiceItemsDiskSpace() in '{0}' provider", ProviderSettings.ProviderName), ex);
                throw;
            }
            finally
            {
                Log.WriteEnd("'{0}' GetServiceItemsDiskSpace", ProviderSettings.ProviderName);
            }
        }

        [WebMethod, SoapHeader("settings")]
        public ServiceProviderItemBandwidth[] GetServiceItemsBandwidth(SoapServiceProviderItem[] items, DateTime since)
        {
            try
            {
                Log.WriteStart("'{0}' GetServiceItemsBandwidth", ProviderSettings.ProviderName);
                return Provider.GetServiceItemsBandwidth(UnwrapServiceProviderItems(items), since);
            }
            catch (Exception ex)
            {
                Log.WriteError(String.Format("Error on GetServiceItemsBandwidth() in '{0}' provider", ProviderSettings.ProviderName), ex);
                throw;
            }
            finally
            {
                Log.WriteEnd("'{0}' GetServiceItemsBandwidth", ProviderSettings.ProviderName);
            }
        }

        [WebMethod]
        public string GetCryptoKey() => Settings.CryptoKey;

        [WebMethod]
        public ServerAuthenticationInfo HardenServerAuthentication(string newSharedSecret)
        {
            var manager = new ServerAuthenticationSettingsManager();
            return manager.HardenServerAuthentication(newSharedSecret);
        }

        private ServiceProviderItem[] UnwrapServiceProviderItems(SoapServiceProviderItem[] soapItems)
        {
            if (soapItems == null)
                return null;
            ServiceProviderItem[] items = new ServiceProviderItem[soapItems.Length];
            for (int i = 0; i < items.Length; i++)
                items[i] = SoapServiceProviderItem.Unwrap(soapItems[i]);

            return items;
        }
    }
}
