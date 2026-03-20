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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace FuseCP.Server.WPIService
{
    class Server
    {
        static Mutex mutex = null;
        static void Main(string[] args)
        {
            bool onlyInstance = false;
            mutex = new Mutex(false, "Global\\{5DE133EC-49AE-4AE4-99BE-0F0A0BB5719E}", out onlyInstance);
            if (!mutex.WaitOne(0, false))  //if (!onlyInstance)
            {
                Console.WriteLine("The service is already running.");
                return;
            }
            WPIService wpiService = new WPIService();

            var listener = new HttpListener();
            listener.Prefixes.Add($"http://127.0.0.1:{WPIServiceContract.PORT}/");
            listener.Start();

            var worker = new Thread(() => ProcessRequests(listener, wpiService));
            worker.IsBackground = true;
            worker.Start();

            Console.WriteLine("The service is running.");
                        
            while (!wpiService.IsFinished)
            {
                Thread.Sleep(2000);
            }

            listener.Stop();

            Console.WriteLine("The service is finished.");
        }

        private static void ProcessRequests(HttpListener listener, WPIService service)
        {
            while (listener.IsListening)
            {
                try
                {
                    var context = listener.GetContext();
                    string path = context.Request.Url.AbsolutePath.Trim('/').ToLowerInvariant();
                    string response = "";

                    switch (path)
                    {
                        case "ping":
                            response = service.Ping();
                            break;
                        case "initialize":
                            service.Initialize(GetValues(context.Request, "feed"));
                            response = "OK";
                            break;
                        case "begininstallation":
                            service.BeginInstallation(GetValues(context.Request, "product"));
                            response = "OK";
                            break;
                        case "status":
                            response = service.GetStatus();
                            break;
                        case "logfiledirectory":
                            response = service.GetLogFileDirectory() ?? "";
                            break;
                        default:
                            context.Response.StatusCode = 404;
                            response = "Not Found";
                            break;
                    }

                    WriteResponse(context.Response, response);
                }
                catch (HttpListenerException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
        }

        private static string[] GetValues(HttpListenerRequest request, string key)
        {
            string value = request.QueryString[key] ?? "";
            if (string.IsNullOrWhiteSpace(value))
            {
                return Array.Empty<string>();
            }

            return value.Split(',', StringSplitOptions.RemoveEmptyEntries);
        }

        private static void WriteResponse(HttpListenerResponse response, string text)
        {
            byte[] payload = Encoding.UTF8.GetBytes(text ?? "");
            response.ContentType = "text/plain; charset=utf-8";
            response.ContentLength64 = payload.Length;
            using var stream = response.OutputStream;
            stream.Write(payload, 0, payload.Length);
        }
    }
}
