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
using System.Globalization;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using FuseCP.Providers.Virtualization;

namespace FuseCP.Portal.VPSForPC
{
    public partial class VpsMonitoring : FuseCPModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ClientScriptManager cs = Page.ClientScript;
            cs.RegisterClientScriptInclude("jquery", ResolveUrl("~/JavaScript/jquery-1.4.4.min.js"));
            cs.RegisterClientScriptInclude("jqueryui", ResolveUrl("~/JavaScript/jquery-ui-1.8.9.min.js"));

            if (!IsPostBack)
            {
                hItemId.Value = PanelRequest.ItemID.ToString();
            }

            DateTime StartP = DateTime.Now.AddDays(-1);
            DateTime EndP = DateTime.Now;

            EndP = (EndP.CompareTo(DateTime.Now.Date) == 0 ? DateTime.Now : EndP);

            LoadChartData(litProcessorChart, PerformanceType.Processor, StartP, EndP, "Processor", "#418cf0");
            LoadChartData(litNetworkChart, PerformanceType.Network, StartP, EndP, "Network", "#418cf0");
            LoadChartData(litMemoryChart, PerformanceType.Memory, StartP, EndP, "Memory", "#33cc33");
        }

        private void LoadChartData(Literal target, PerformanceType perfType, DateTime startPeriod, DateTime endPeriod, string title, string lineColor)
        {
            PerformanceDataValue[] perfValues = ES.Services.VPSPC.GetPerfomanceValue(PanelRequest.ItemID, perfType, startPeriod, endPeriod);

            target.Text = BuildSvgChart(title, perfValues, lineColor);
        }

        private static string BuildSvgChart(string title, PerformanceDataValue[] values, string lineColor)
        {
            const int width = 584;
            const int height = 296;
            const int padLeft = 40;
            const int padTop = 20;
            const int plotWidth = width - 55;
            const int plotHeight = height - 50;

            if (values == null || values.Length == 0)
            {
                return $"<div class=\"fcp-mb-3\"><div class=\"fw-bold mb-1\">{title}</div><div class=\"text-muted\">No data available for the selected period.</div></div>";
            }

            double min = double.MaxValue;
            double max = double.MinValue;
            for (int i = 0; i < values.Length; i++)
            {
                double sample = values[i].SampleValue ?? 0d;
                if (sample < min) min = sample;
                if (sample > max) max = sample;
            }
            if (Math.Abs(max - min) < 0.000001)
            {
                max = min + 1;
            }

            var points = new StringBuilder();
            for (int i = 0; i < values.Length; i++)
            {
                double sample = values[i].SampleValue ?? 0d;
                double xRatio = values.Length == 1 ? 0 : (double)i / (values.Length - 1);
                double yRatio = (sample - min) / (max - min);
                double x = padLeft + (xRatio * plotWidth);
                double y = padTop + ((1 - yRatio) * plotHeight);
                if (i > 0)
                {
                    points.Append(' ');
                }
                points.Append(x.ToString("0.##", CultureInfo.InvariantCulture));
                points.Append(',');
                points.Append(y.ToString("0.##", CultureInfo.InvariantCulture));
            }

                        return $@"<div class=""fcp-mb-3"">
<div class=""fw-bold mb-1"">{title}</div>
<svg width=""{width}"" height=""{height}"" viewBox=""0 0 {width} {height}"" xmlns=""http://www.w3.org/2000/svg"" role=""img"" aria-label=""{title} performance trend"">
    <rect x=""0"" y=""0"" width=""{width}"" height=""{height}"" fill=""#fffaf0"" stroke=""#b54001"" stroke-width=""1"" />
    <line x1=""{padLeft}"" y1=""{padTop + plotHeight}"" x2=""{padLeft + plotWidth}"" y2=""{padTop + plotHeight}"" stroke=""#666"" stroke-width=""1"" />
    <line x1=""{padLeft}"" y1=""{padTop}"" x2=""{padLeft}"" y2=""{padTop + plotHeight}"" stroke=""#666"" stroke-width=""1"" />
    <polyline fill=""none"" stroke=""{lineColor}"" stroke-width=""2"" points=""{points}"" />
    <text x=""{padLeft}"" y=""{height - 8}"" font-size=""11"" fill=""#444"">{values[0].TimeSampled:yyyy-MM-dd HH:mm}</text>
    <text x=""{padLeft + plotWidth - 170}"" y=""{height - 8}"" font-size=""11"" fill=""#444"">{values[values.Length - 1].TimeSampled:yyyy-MM-dd HH:mm}</text>
    <text x=""{padLeft + 4}"" y=""{padTop + 12}"" font-size=""11"" fill=""#444"">Max {max:0.##}</text>
    <text x=""{padLeft + 4}"" y=""{padTop + plotHeight - 4}"" font-size=""11"" fill=""#444"">Min {min:0.##}</text>
</svg>
</div>";
        }
    }
}
