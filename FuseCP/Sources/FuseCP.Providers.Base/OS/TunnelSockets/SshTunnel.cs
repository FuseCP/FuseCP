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

using Renci.SshNet;
using Renci.SshNet.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FuseCP.Providers.OS
{
    /// <summary>
    /// A class that implements ssh port forwarding of a local port to a remote port over ssh
    /// </summary>
    public class SshTunnel : IDisposable
    {
        public string AccessUrl => Uri.AccessUrl(Loopback, ForwardedPort.BoundPort);

        SshUri uri = new SshUri(null);
        [XmlIgnore, IgnoreDataMember]
        public SshUri Uri
        {
            get => uri;
            set => uri = value;
        }
        [XmlIgnore, IgnoreDataMember]
        public SshClient Client { get; set; }
        [XmlIgnore, IgnoreDataMember]
        public ForwardedPortLocal ForwardedPort { get; protected set; }
        public string Url
        {
            get => Uri.Url;
            set => Uri.Url = value;
        }
        [XmlIgnore, IgnoreDataMember]
        public Exception Exception { get; set; }
        [XmlIgnore, IgnoreDataMember]
        public Exception ConnectException { get; set; }
        public bool IsConnecting { get; protected set; }
        public TimeSpan ConnectTimeout { get; set; } = TimeSpan.FromSeconds(15);
        public IPAddress Loopback { get; protected set; }

        public SshTunnel(string url)
        {
            Url = url;
            Client = null;
            Exception = null;
            ConnectException = null;
            ForwardedPort = null;
            IsConnecting = true;
        }

        private async Task<SshUri> InitAsync()
        {
            var sshhost = Uri.DnsSafeHost;
            var remotehost = Uri.RemoteForwardHost;
            var sshhostip = await DnsService.GetFirstIPAddressAsync(Uri.DnsSafeHost);
            if (uri.RemoteForwardHost == null)
            {
                if (sshhostip.AddressFamily == AddressFamily.InterNetwork)
                {
                    remotehost = IPAddress.Loopback.ToString();
                    Loopback = IPAddress.Loopback;
                }
                else
                {
                    remotehost = IPAddress.IPv6Loopback.ToString();
                    Loopback = IPAddress.IPv6Loopback;
                }
            }
            sshhost = sshhostip.ToString();

            if (string.IsNullOrEmpty(Uri.Password))
            {
                Client = new SshClient(sshhost, Uri.Port != -1 ? Uri.Port : 22, Uri.Username, Uri.Keys);
            }
            else
            {
                Client = new SshClient(sshhost, Uri.Port != -1 ? Uri.Port : 22, Uri.Username, Uri.Password);
            }

            if (Uri.LocalForwardPort == 0) ForwardedPort = new ForwardedPortLocal(Loopback.ToString(), remotehost, Uri.RemoteForwardPort);
            else ForwardedPort = new ForwardedPortLocal(Loopback.ToString(), Uri.LocalForwardPort, remotehost, Uri.RemoteForwardPort);

            return uri;
        }

        private SshUri Init()
        {
            var sshhost = Uri.DnsSafeHost;
            var remotehost = Uri.RemoteForwardHost;
            var sshhostip = DnsService.GetFirstIPAddress(Uri.DnsSafeHost);
            if (Uri.RemoteForwardHost == null)
            {
                if (sshhostip.AddressFamily == AddressFamily.InterNetwork)
                {
                    remotehost = IPAddress.Loopback.ToString();
                    Loopback = IPAddress.Loopback;
                    sshhost = sshhostip.ToString();
                }
                else if (sshhostip.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    remotehost = IPAddress.IPv6Loopback.ToString();
                    Loopback = IPAddress.IPv6Loopback;
                    sshhost = sshhostip.ToString();
                }
                else if (sshhostip != default(IPAddress))
                {
                    throw new ArgumentException("No supported ip adress");
                }
            }

            if (string.IsNullOrEmpty(Uri.Password))
            {
                Client = new SshClient(sshhost, Uri.Port != -1 ? Uri.Port : 22, Uri.Username, Uri.Keys);
            }
            else
            {
                Client = new SshClient(sshhost, Uri.Port != -1 ? Uri.Port : 22, Uri.Username, Uri.Password);
            }

            if (Uri.LocalForwardPort == 0) ForwardedPort = new ForwardedPortLocal(Loopback.ToString(), remotehost, Uri.RemoteForwardPort);
            else ForwardedPort = new ForwardedPortLocal(Loopback.ToString(), Uri.LocalForwardPort, remotehost, Uri.RemoteForwardPort);

            return Uri;
        }

        public bool Create()
        {
            Init();

            try
            {
                IsConnecting = true;
                Client.Connect();
                Client.AddForwardedPort(ForwardedPort);
                ForwardedPort.Start();
                IsConnecting = false;
                Client.ErrorOccurred += Restart;
                ForwardedPort.Exception += Restart;

                Trace.TraceInformation($"SSH Tunnel on {SanitizeForLog(Url)} started.");
                return true;
            }
            catch (Exception ex)
            {
                ConnectException = ex;
                IsConnecting = false;
                Disconnect();

                Trace.TraceError($"Failed to connect SSH Tunnel to {SanitizeForLog(Url)}: {ex}");
                return false;
            }
        }

        public async Task CreateAsync()
        {
            await InitAsync();

            try
            {
                IsConnecting = true;
                await Client.ConnectAsync(new CancellationTokenSource(ConnectTimeout).Token);
                await Task.Run(() =>
                {
                    Client.AddForwardedPort(ForwardedPort);
                    ForwardedPort.Start();
                });
                IsConnecting = false;
                Client.ErrorOccurred += Restart;
                ForwardedPort.Exception += Restart;
                Trace.TraceInformation($"SSH Tunnel on {SanitizeForLog(Url)} started.");
            }
            catch (Exception ex)
            {
                ConnectException = ex;
                IsConnecting = false;
                Disconnect();

                Trace.TraceError($"Failed to connect SSH Tunnel to {SanitizeForLog(Url)}: {ex}");
            }
        }

        bool isRestarting = false;
        public void Restart(object sender, ExceptionEventArgs args)
        {
            lock (this)
            {
                if (isRestarting || Exception == args.Exception || ConnectException != null) return;
                Exception = args.Exception;
                isRestarting = true;
                IsConnecting = true;
                Client.ErrorOccurred -= Restart;
                ForwardedPort.Exception -= Restart;

                Trace.TraceError($"Exception on SSH Tunnel {SanitizeForLog(Url)}: {Exception}");

                Disconnect();

                ConnectException = null;
                try
                {
                    Client.Connect();
                    ForwardedPort.Start();
                }
                catch (Exception ex)
                {
                    ConnectException = ex;
                    isRestarting = false;
                    Disconnect();

                    Trace.TraceError($"Failed to reconnect SSH Tunnel to {SanitizeForLog(Url)}: {ex}");
                }
                isRestarting = IsConnecting = false;
                Client.ErrorOccurred += Restart;
                ForwardedPort.Exception += Restart;
            }
        }

        /// <summary>Strips control characters from a user-provided value before it appears in a log entry.</summary>
        private static string SanitizeForLog(string value) =>
            value == null ? string.Empty : value.Replace('\r', ' ').Replace('\n', ' ');

        bool isDisposed = false;
        public void Disconnect()
        {
            lock (this)
            {
                if (ForwardedPort.IsStarted)
                {
                    try
                    {
                        ForwardedPort.Stop();
                    }
                    catch { }
                }
                if (Client.IsConnected)
                {
                    try
                    {
                        Client.Disconnect();
                    }
                    catch { }
                }
            }
        }
        public void Dispose()
        {
            Disconnect();
            if (!isDisposed)
            {
                isDisposed = true;
                Client.Dispose();
            }
        }
    }
}
