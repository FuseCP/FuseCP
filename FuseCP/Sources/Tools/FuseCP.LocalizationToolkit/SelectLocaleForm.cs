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
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FuseCP.LocalizationToolkit
{
	public partial class SelectLocaleForm : Form
	{
		public SelectLocaleForm()
		{
			InitializeComponent();
			LoadLocales();
		}

		private string BaseDirectory
		{
			get
			{
				return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
			}
		}

		private void LoadLocales()
		{
			Hashtable existingLocales = new Hashtable();
			string baseDir = this.BaseDirectory;
			string[] dirs = Directory.GetDirectories(baseDir);

			DataSet dsLocales = new DataSet();
			DataTable dt = new DataTable("Locales");
			dsLocales.Tables.Add(dt);
			DataColumn col1 = new DataColumn("Name", typeof(string));
			DataColumn col2 = new DataColumn("EnglishName", typeof(string));
			dt.Columns.AddRange(new DataColumn[] { col1, col2 });
			foreach (string dir in dirs)
			{
				try
				{
					string cultureName = Path.GetFileName(dir);
					CultureInfo ci = new CultureInfo(Path.GetFileName(dir));
					if (!existingLocales.ContainsKey(ci.Name))
						existingLocales.Add(ci.Name, ci.Name);
				}
				catch (ArgumentException) { }
			}
			CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures | CultureTypes.NeutralCultures);
			foreach (CultureInfo ci in cultures)
			{
				if (!existingLocales.ContainsKey(ci.Name) && !ci.IsNeutralCulture)
				{
					dsLocales.Tables[0].Rows.Add(new object[] { ci.Name, ci.EnglishName });
				}
			}
			DataView dv = new DataView(dsLocales.Tables[0]);
			dv.Sort = "EnglishName";
			lstLocales.DataSource = dv;
			lstLocales.DisplayMember = "EnglishName";
			lstLocales.ValueMember = "Name";
			lstLocales.SelectedIndex = 0;
		}

		private string selectedLocale;

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string SelectedLocale
		{
			get { return selectedLocale; }
			set { selectedLocale = value; }
		}

		private void OnOKClick(object sender, EventArgs e)
		{
			CloseForm();
		}

		private void lstLocales_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			CloseForm();
		}

		private void CloseForm()
		{

			this.SelectedLocale = (string)lstLocales.SelectedValue;
			this.DialogResult = DialogResult.OK;
			this.Close();
		}
	}
}
