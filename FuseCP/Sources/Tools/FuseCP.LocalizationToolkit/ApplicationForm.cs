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
using System.Diagnostics;
using System.Net;
using System.Resources;
using System.Configuration;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace FuseCP.LocalizationToolkit
{
	/// <summary>
	/// Main application form
	/// </summary>
	public partial class ApplicationForm : Form
	{
		private Resources dsResources;
		private ProgressManager progressManager;
		private string importedLocale;
		private string importedFileName;
		private Thread thread;
		FindForm findForm;

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the ApplicationForm class.
		/// </summary>
		public ApplicationForm()
		{
			InitializeComponent();
			this.grdResources.AutoGenerateColumns = false;
			if (DesignMode)
			{
				return;
			}
			this.progressManager = new ProgressManager(this, this.statusBarLabel);

			try
			{
				LoadLocales();
			}
			catch (Exception ex)
			{
				ShowError(ex);
			}
		}

		private string DataDirectory
		{
			get
			{
				return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
			}
		}

		private string TmpDirectory
		{
			get
			{
				return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tmp");
			}
		}


		private string LanguagePacksDirectory
		{
			get
			{
				return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LanguagePacks");
			}
		}

		private string SelectedLocale
		{
			get
			{
				DataRowView row = cbSupportedLocales.SelectedValue as DataRowView;
				if (row != null)
				{
					return (string)row["Name"];
				}
				else
				{
					return cbSupportedLocales.SelectedValue as string;
				}
			}
		}


		private void LoadLocales()
		{
			if (this.InvokeRequired)
			{
				VoidCallback callBack = new VoidCallback(LoadLocales);
				this.Invoke(callBack, null);
			}
			else
			{
				DataSet dsLocales = GetSupportedLocales();
				cbSupportedLocales.DataSource = dsLocales.Tables[0];
				cbSupportedLocales.DisplayMember = "EnglishName";
				cbSupportedLocales.ValueMember = "Name";

				bool enabled = (cbSupportedLocales.SelectedValue != null);
				btnDelete.Enabled = enabled;
				btnSave.Enabled = enabled;
				btnExport.Enabled = enabled;
			}
		}

		private DataSet GetSupportedLocales()
		{
			string baseDir = this.DataDirectory;
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
					if (cultureName == "en-US")
						continue;
					dt.Rows.Add(new object[] { ci.Name, ci.EnglishName });
				}
				catch (ArgumentException) { }
			}
			return dsLocales;
		}
		#endregion

		private void OnLocaleChanged(object sender, EventArgs e)
		{
			SaveChanges(true);
			BindResourcesGrid();
		}

		private void BindResourcesGrid()
		{
			try
			{
				if (string.IsNullOrEmpty(cbSupportedLocales.Text))
				{
					grdResources.DataMember = null;
					grdResources.DataSource = null;
					return;
				}
				string localeName = null;
				DataRowView row = cbSupportedLocales.SelectedValue as DataRowView;
				if (row != null)
				{
					localeName = (string)row["Name"];
				}
				else
				{
					localeName = cbSupportedLocales.SelectedValue as string;
				}

				if (!string.IsNullOrEmpty(localeName))
				{
					dsResources = new Resources();
					string fileName = Path.Combine(this.DataDirectory, localeName + "\\resources.xml");
					dsResources.ReadXml(fileName);
					dsResources.AcceptChanges();
					grdResources.DataSource = dsResources;
					grdResources.DataMember = this.dsResources.Tables[0].TableName;
				}
			}
			catch (Exception ex)
			{
				ShowError(ex);
			}
		}

		private void grdResources_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			/*if (e.ColumnIndex == this.grdResources.Columns["KeyColumn"].Index)
			{
				DataGridViewCell keyCell = this.grdResources.Rows[e.RowIndex].Cells[e.ColumnIndex];
				DataGridViewCell fileCell = this.grdResources.Rows[e.RowIndex].Cells[this.grdResources.Columns["FileColumn"].Index];
				if ( fileCell.Value != null )
					keyCell.ToolTipText = fileCell.Value.ToString();
			}
			else*/
			if (e.ColumnIndex == this.grdResources.Columns["ValueColumn"].Index)
			{
				if (e.Value == DBNull.Value)
				{
					e.CellStyle.BackColor = Color.Khaki;
				}
			}
		}

		private void OnAddClick(object sender, EventArgs e)
		{
			AddLocale();
		}

		private void AddLocale()
		{
			SaveChanges(true);
			try
			{
				SelectLocaleForm form = new SelectLocaleForm();
				if (form.ShowDialog(this) == DialogResult.OK)
				{
					string locale = form.SelectedLocale;
					string path = Path.Combine(this.DataDirectory, locale);
					if (!Directory.Exists(path))
					{
						Directory.CreateDirectory(path);
					}
					string sourceFile = Path.Combine(this.DataDirectory, @"en-US\Resources.xml");
					string destFile = Path.Combine(path, "Resources.xml");
					File.Copy(sourceFile, destFile, true);
					LoadLocales();
					SetCurrentLocale(locale);
				}
			}
			catch (Exception ex)
			{
				ShowError(ex);
			}
		}


		private void SaveChanges(bool showWarning)
		{
			try
			{
				Resources ds = grdResources.DataSource as Resources;
				if (ds != null && ds.HasChanges())
				{

					DialogResult res = DialogResult.Yes;
					if (showWarning)
					{
						res = MessageBox.Show("Save changes?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
					}
					if (res == DialogResult.Yes)
					{
						ds.AcceptChanges();
						string fileName = GetLocaleFile();
						ds.WriteXml(fileName);
					}
				}
			}
			catch (Exception ex)
			{
				ShowError(ex);
			}
		}

		private string GetLocaleFile()
		{
			string ret = null;
			if (string.IsNullOrEmpty(cbSupportedLocales.Text))
			{
				return ret;
			}
			string localeName = null;
			DataRowView row = cbSupportedLocales.SelectedValue as DataRowView;
			if (row != null)
			{
				localeName = (string)row["Name"];
			}
			else
			{
				localeName = cbSupportedLocales.SelectedValue as string;
			}
			if (!string.IsNullOrEmpty(localeName))
			{
				ret = Path.Combine(this.DataDirectory, localeName + @"\Resources.xml");
			}
			return ret;
		}

		private void OnDeleteClick(object sender, EventArgs e)
		{
			DeleteLocale();
		}

		private void DeleteLocale()
		{
			if (MessageBox.Show(this,
				string.Format("{0} locale will be deleted.", cbSupportedLocales.Text),
				this.Text, MessageBoxButtons.OKCancel,
				MessageBoxIcon.Warning) == DialogResult.OK)
			{
				try
				{
					string locale = (string)cbSupportedLocales.SelectedValue;
					string path = Path.Combine(this.DataDirectory, locale);
					Directory.Delete(path, true);
					LoadLocales();
					BindResourcesGrid();
				}
				catch (Exception ex)
				{
					ShowError(ex);
				}
			}
		}

		private void OnSaveClick(object sender, EventArgs e)
		{
			SaveChanges(false);
		}

		private void OnExportClick(object sender, EventArgs e)
		{
			Compile();
		}

		private void Compile()
		{
			SaveChanges(true);

			try
			{
				string locale = this.SelectedLocale;
				if (string.IsNullOrEmpty(locale))
				{
					return;
				}

				StartAsyncProgress("Compiling...", true);
				ThreadStart threadDelegate = new ThreadStart(CompileLanguagePack);
				thread = new Thread(threadDelegate);
				thread.Start();
			}
			catch (Exception ex)
			{
				FinishProgress();
				ShowError(ex);
			}
		}

		private void CompileLanguagePack()
		{
			try
			{
				string locale = this.SelectedLocale;
				Resources ds = grdResources.DataSource as Resources;

				DeleteTmpDir();
				//create folder structure
				Hashtable files = new Hashtable();
				foreach (Resources.ResourceRow row in ds.Resource.Rows)
				{
					if (!files.ContainsKey(row.File))
					{
						files.Add(
							row.File,
							Path.Combine(this.TmpDirectory, row.File)
						);
					}
				}

				Hashtable folders = new Hashtable();
				string fullFolderName;
				foreach (string file in files.Keys)
				{
					fullFolderName = Path.GetDirectoryName((string)files[file]);
					if (!folders.ContainsKey(fullFolderName))
					{
						folders.Add(fullFolderName, fullFolderName);
					}
				}

				foreach (string folder in folders.Keys)
				{
					if (!Directory.Exists(folder))
					{
						Directory.CreateDirectory(folder);
					}
				}

				//write resources to resx files
				string fullFileName;
				ResXResourceWriter writer = null;
				foreach (string file in files.Keys)
				{

					string filter = string.Format("File = '{0}'", file);

					DataRow[] rows = ds.Resource.Select(filter, "File");
					if (rows.Length > 0)
					{
						fullFileName = (string)files[file];
						fullFileName = string.Format("{0}{1}{2}.{3}{4}",
							Path.GetDirectoryName(fullFileName),
							Path.DirectorySeparatorChar,
							Path.GetFileNameWithoutExtension(fullFileName),
							locale,
							Path.GetExtension(fullFileName)
						);
						writer = new ResXResourceWriter(fullFileName);

						foreach (DataRow row in rows)
						{
							if (row["Value"] != DBNull.Value)
							{
								writer.AddResource((string)row["Key"], (string)row["Value"]);
							}
						}
						writer.Close();
					}
				}
				if (!Directory.Exists(LanguagePacksDirectory))
				{
					Directory.CreateDirectory(LanguagePacksDirectory);
				}
				//zip lang pack
				string zipFile = string.Format("FuseCP Language Pack {0} {1}.zip", locale, GetApplicationVersion());
				ZipUtils.Zip("Tmp", "LanguagePacks\\" + zipFile);
				DeleteTmpDir();
				FinishProgress();
				string message = string.Format("{0} language pack compiled successfully to\n\"{1}\"", cbSupportedLocales.Text, zipFile);
				MessageBox.Show(this, message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				if (IsThreadAbortException(ex))
					return;
			}
		}

		private void ClearTmpDir()
		{
			DeleteTmpDir();
			try
			{
				if (!Directory.Exists(this.TmpDirectory))
					Directory.CreateDirectory(this.TmpDirectory);
			}
			catch { }
		}

		private void DeleteTmpDir()
		{
			try
			{
				if ( Directory.Exists(this.TmpDirectory))
					Directory.Delete(this.TmpDirectory, true);
			}
			catch { }
		}

		private string GetApplicationVersion()
		{
			Version version = this.GetType().Assembly.GetName().Version;
			string strVersion;
			if (version.Build == 0)
				strVersion = version.ToString(2);
			else
				strVersion = version.ToString(3);
			return strVersion;
		}

		/// <summary>
		/// Shows default error message
		/// </summary>
		internal void ShowError(Exception ex)
		{
			string message = string.Format("An unexpected error has occurred. We apologize for this inconvenience.\n" +
				"Please contact Technical Support at support@FuseCP.net\n\n{0}", ex);
			MessageBox.Show(this, message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		/// <summary>
		/// Current version
		/// </summary>
		public string Version
		{
			get
			{
				return this.GetType().Assembly.GetName().Version.ToString();
			}
		}

		/// <summary>
		/// Disables application content
		/// </summary>
		internal void DisableContent()
		{
			pnlTop.Enabled = false;
			grdResources.Enabled = false;
		}

		/// <summary>
		/// Enables application content
		/// </summary>
		internal void EnableContent()
		{
			pnlTop.Enabled = true;
			grdResources.Enabled = true;
		}

		/// <summary>
		/// Shows info message
		/// </summary>
		/// <param name="message"></param>
		internal void ShowInfo(string message)
		{
			MessageBox.Show(this, message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		/// <summary>
		/// Shows info message
		/// </summary>
		/// <param name="message"></param>
		internal void ShowWarning(string message)
		{
			MessageBox.Show(this, message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}






		#region Progress indication

		/// <summary>
		/// Starts progress indication
		/// </summary>
		/// <param name="title">Title</param>
		internal void StartProgress(string title)
		{
			StartProgress(title, false);
		}

		/// <summary>
		/// Starts progress indication
		/// </summary>
		/// <param name="title">Title</param>
		/// <param name="disableContent">Disable content</param>
		internal void StartProgress(string title, bool disableContent)
		{
			if (disableContent)
			{
				DisableContent();
			}
			progressManager.StartProgress(title);
		}

		/// <summary>
		/// Starts async progress indication
		/// </summary>
		/// <param name="title">Title</param>
		/// <param name="disableContent">Disable content</param>
		internal void StartAsyncProgress(string title, bool disableContent)
		{
			if (disableContent)
			{
				DisableContent();
			}
			topLogoControl.ShowProgress();
			progressManager.StartProgress(title);
		}

		/// <summary>
		/// Finishes progress indication
		/// </summary>
		internal void FinishProgress()
		{
			if (this.InvokeRequired)
			{
				VoidCallback callBack = new VoidCallback(FinishProgress);
				this.Invoke(callBack, null);
			}
			else
			{
				topLogoControl.HideProgress();
				progressManager.FinishProgress();
				EnableContent();
			}
		}

		#endregion

		private void OnImportClick(object sender, EventArgs e)
		{
			Import();
		}

		private void Import()
		{
			SaveChanges(true);

			try
			{
				string message;
				OpenFileDialog dialog = new OpenFileDialog();
				dialog.Filter = "Zip files (*.zip)|*.zip" ;
				dialog.RestoreDirectory = true;
				dialog.InitialDirectory = this.LanguagePacksDirectory;
				if (dialog.ShowDialog() != DialogResult.OK)
					return;

				importedFileName = dialog.FileName;

				string file = Path.GetFileNameWithoutExtension(importedFileName);
                string filePrefix1 = "FuseCP Language Pack ";
                string filePrefix2 = "FuseCP_Language_Pack_";
                if (file.StartsWith(filePrefix1))
				{
                    file = file.Substring(filePrefix1.Length);
					importedLocale = file.Substring(0, file.IndexOf(" "));
				}
                else if (file.StartsWith(filePrefix2))
				{
                    file = file.Substring(filePrefix2.Length);
					importedLocale = file.Substring(0, file.IndexOf("_"));
				}
				else
				{
					ShowInfo("Please select FuseCP Language Pack file.");
					return;
				}

				bool localeExists = false;
				string localeName = string.Empty;
				DataSet dsLocales = GetSupportedLocales();
				foreach (DataRow row in dsLocales.Tables[0].Rows)
				{
					if (row["Name"].ToString() == importedLocale)
					{
						localeExists = true;
						localeName = row["EnglishName"].ToString();
						break;
 					}
 				}
				if ( localeExists )
				{
					message = string.Format("{0} locale already exists. Overwrite?", localeName);
					if (MessageBox.Show(this, message, this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
					{
						return;
					}
				}
				
				StartAsyncProgress("Importing...", true);
				ThreadStart threadDelegate = new ThreadStart(ImportLanguagePack);
				thread = new Thread(threadDelegate);
				thread.Start();
			}
			catch (Exception ex)
			{
				FinishProgress();
				ShowError(ex);
			}
		}

		private void ImportLanguagePack()
		{
			try
			{
				string path = Path.Combine(this.DataDirectory, importedLocale);
				string sourceFile = Path.Combine(this.DataDirectory, @"en-US\Resources.xml");
				string destFile = Path.Combine(path, "Resources.xml");
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
				
				if (!File.Exists(destFile))
				{
					File.Copy(sourceFile, destFile);
				}
					
				ClearTmpDir();
				SetStatusBarText("Extracting language pack...");
				ZipUtils.Unzip("Tmp", importedFileName);

				Resources ds = new Resources();
				ds.ReadXml(destFile);
				int count = ImportFiles(importedLocale, ds, this.TmpDirectory, this.TmpDirectory);
				ds.AcceptChanges();
				ds.WriteXml(destFile);
				DeleteTmpDir();
				LoadLocales();
				SetCurrentLocale(importedLocale);
				FinishProgress();

				string message = string.Format("{0} record(s) imported successfully.", count);
				MessageBox.Show(this, message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				if (IsThreadAbortException(ex))
					return;
				throw;
			}
		}

		private int ImportFiles(string locale, Resources ds, string sourceDir, string baseDir)
		{
			int count = 0;
			string[] dirs = Directory.GetDirectories(sourceDir);
			foreach (string dir in dirs)
			{
				count += ImportFiles(locale, ds, dir, baseDir);
				//break;
			}
			string[] files = Directory.GetFiles(sourceDir, "*.resx", SearchOption.TopDirectoryOnly);
			foreach (string file in files)
			{
				count += ImportFile(locale, ds, file, baseDir);
				//break;
			}
			return count;
		}

		private int ImportFile(string locale, Resources ds, string file, string baseDir)
		{
			int count = 0;
			string path = file.Substring(baseDir.Length);
			path = path.Replace(locale+".resx", "resx");
			if (path.StartsWith(Path.DirectorySeparatorChar.ToString()))
			{
				path = path.TrimStart(Path.DirectorySeparatorChar);
			}
			SetStatusBarText(string.Format("Importing {0}...", path));
			// Create a ResXResourceReader for the file.
			ResXResourceReader rsxr = new ResXResourceReader(file);

			// Create an IDictionaryEnumerator to iterate through the resources.
			IDictionaryEnumerator id = rsxr.GetEnumerator();

			// Iterate through the resources and display the contents to the console.
			foreach (DictionaryEntry d in rsxr)
			{
				string key = d.Key.ToString();
				if (d.Value != null)
				{
					string value = d.Value.ToString();
					DataRow[] rows = FindRows(path, key, ds);
					foreach (Resources.ResourceRow row in rows)
					{
						row.Value = value;
						count++;
					}
				}
			}
			//Close the reader.
			rsxr.Close();
			return count;
		}

		private DataRow[] FindRows(string file, string key, Resources ds)
		{
			string filter = string.Format("File = '{0}' AND Key = '{1}'", file, key);
			return ds.Resource.Select(filter);
		}

		private void SetStatusBarText(string text)
		{
			// InvokeRequired required compares the thread ID of the
			// calling thread to the thread ID of the creating thread.
			// If these threads are different, it returns true.
			if (this.InvokeRequired)
			{
				StringCallback callBack = new StringCallback(SetStatusBarText);
				this.Invoke(callBack, new object[] { text });
			}
			else
			{
				this.statusBarLabel.Text = text;
				this.Update();
			}
		}

		private void SetCurrentLocale(string locale)
		{
			if (this.InvokeRequired)
			{
				StringCallback callBack = new StringCallback(SetCurrentLocale);
				this.Invoke(callBack, new object[] { locale });
			}
			else
			{
				cbSupportedLocales.SelectedValue = locale;
			}
		}

		delegate void StringCallback(string str);
		delegate void VoidCallback();

		private void OnFormClosing(object sender, FormClosingEventArgs e)
		{
			SaveChanges(true);
			AbortThread();
		}

		private void AbortThread()
		{
			if (this.thread != null)
			{
				if (this.thread.IsAlive)
				{
					this.thread.Interrupt();
					this.thread.Join(500);
				}
				this.thread = null;
			}
		}

		private static bool IsThreadAbortException(Exception ex)
		{
			Exception innerException = ex;
			while (innerException != null)
			{
				if (innerException is ThreadInterruptedException)
					return true;
				innerException = innerException.InnerException;
			}

			string str = ex.ToString();
			return str.Contains("System.Threading.ThreadInterruptedException");
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control && e.KeyCode == Keys.F)
			{
				e.Handled = true;
				ShowFindForm();
			}
		}

		private void ShowFindForm()
		{
			if (findForm == null || findForm.IsDisposed)
			{
				findForm = new FindForm(this.grdResources);
				findForm.Owner = this;
			}
			if ( !findForm.Visible )
				findForm.Show();
		}

		private void OnFindClick(object sender, EventArgs e)
		{
			ShowFindForm();
		}
	}
}
