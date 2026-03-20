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
using System.Drawing;
using System.Net;
using System.Web;
using System.Web.Services;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Drawing.Imaging;
#if NET5_0_OR_GREATER
using System.Runtime.Versioning;
using System.Net.Http;
#endif

namespace FuseCP.Portal
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
#if NET5_0_OR_GREATER
    [SupportedOSPlatform("windows")]
#endif
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding]
    public class ResizeImage : IHttpHandler
    {

        public const string URL = "url";

        public const string WIDTH = "width";

        public const string HEIGHT = "height";

		public const int MaxDownloadAttempts = 5;

		public const int BitmapCacheDurationInSeconds = 900;


        private static bool Abort()
        {
            return false;
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Clear();
            context.Response.ContentType = "image/png";

            string imageUrl = context.Request.QueryString[URL];
            if (!string.IsNullOrEmpty(imageUrl))
            {
				// Create decoded version of the image url
				imageUrl = context.Server.UrlDecode(imageUrl);
				//
				Image img = null;

				try
				{
#if NET5_0_OR_GREATER
                    using (var client = new HttpClient())
                    {
                        var imageBytes = client.GetByteArrayAsync(imageUrl).GetAwaiter().GetResult();
                        using (var imageStream = new MemoryStream(imageBytes))
                        {
                            img = new Bitmap(imageStream);
                        }
                    }
#else
					WebRequest request = WebRequest.Create(imageUrl);
					WebResponse response = request.GetResponse();
					// Load image stream from the response
                    img = new Bitmap(response.GetResponseStream());
#endif
				}
				catch (Exception ex)
				{
					Trace.TraceError(ex.StackTrace);
				}

                int width = Utils.ParseInt(context.Request.QueryString[WIDTH], 20);
                int height = Utils.ParseInt(context.Request.QueryString[HEIGHT], 20);

                // calculate new size
                int h = (img != null) ? img.Height : height;
                int w = (img != null) ? img.Width : width;
                int b = Math.Max(h, w);
                double per = b > Math.Max(width, height) ? (Math.Max(width, height) * 1.0) / b : 1.0;

                h = (int)(h * per);
                w = (int)(w * per);

                Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                Graphics new_g = Graphics.FromImage(bitmap);
                new_g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                new_g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                // draw white background
                SolidBrush brush = new SolidBrush(Color.White);
                new_g.FillRectangle(brush, new Rectangle(0, 0, width, height));
                brush.Dispose();

                if (img != null)
                {
                    // draw image
                    new_g.DrawImage(img, 0, 0, w, h);
                    img.Dispose();
                }

                // emit it to the response stream
                bitmap.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);

                // clean-up
                bitmap.Dispose();
                new_g.Dispose();

				// set cache info if image was loaded
                if (img != null)
                {
                    context.Response.Cache.SetExpires(DateTime.Now.AddSeconds(BitmapCacheDurationInSeconds));
                    context.Response.Cache.SetCacheability(HttpCacheability.Public);
                    context.Response.Cache.SetValidUntilExpires(true);
                }

                // end response
				context.Response.End();                                               
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
