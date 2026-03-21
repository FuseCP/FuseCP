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
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using FuseCP.Providers.OS;
using System.Linq;
using System.Text.RegularExpressions;

namespace FuseCP.Portal
{
    public partial class FileManager : FuseCPModuleBase
    {
		/// <summary>
		/// gvFilesID control.
		/// </summary>
		/// <remarks>
		/// Auto-generated field.
		/// To modify move field declaration from designer file to code-behind file.
		/// </remarks>
		protected global::System.Web.UI.WebControls.Literal gvFilesID;

		public static string ALLOWED_EDIT_EXTENSIONS = ".txt,.htm,.html,.cfc,.cfml,.cfm,.php,.pl,.sql,.cs,.vb,.ascx,.aspx,.inc,.asp,.config,.xml,.xsl,.xslt,.xsd,.master,.htaccess,.htpasswd,.cshtml,.vbhtml,.ini,.config";

        protected void Page_Load(object sender, EventArgs e)
        {
			// Localization for JS
			RegisterJsLocalizedMessages();
			//
            if (gvFilesID != null && gvFiles != null)
            {
                gvFilesID.Text = gvFiles.ClientID;
            }

            string downloadFile = Request["DownloadFile"];
            if (downloadFile != null)
            {
                // download file
                Response.Clear();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(downloadFile));
                Response.ContentType = "application/octet-stream";

                int FILE_BUFFER_LENGTH = 5000000;
                byte[] buffer = null;
                int offset = 0;
                do
                {
                    try
                    {
                        // read remote content
                        buffer = ES.Services.Files.GetFileBinaryChunk(PanelSecurity.PackageId, downloadFile, offset, FILE_BUFFER_LENGTH);
                    }
                    catch (Exception ex)
                    {
                        messageBox.ShowErrorMessage("FILES_READ_FILE", ex);
                        break;
                    }

                    // write to stream
                    Response.BinaryWrite(buffer);

                    offset += FILE_BUFFER_LENGTH;
                }
                while (buffer.Length == FILE_BUFFER_LENGTH);
                Response.End();
            }

            // set display preferences
            gvFiles.PageSize = UsersHelper.GetDisplayItemsPerPage();
            if (!IsPostBack)
            {
                BindPath();
                BindEncodings();
            }
            //Make sure text boxes in popup screens are focussed
            SetModalPopupFocus();

        }

        void BindEncodings()
        {
            ddlFileEncodings.DataSource = Encoding.GetEncodings();
            ddlFileEncodings.DataTextField = "DisplayName";
            ddlFileEncodings.DataValueField = "Name";
            ddlFileEncodings.DataBind();

            ddlFileEncodings.SelectedValue = Encoding.UTF8.WebName;
        }

        void SetModalPopupFocus()
        {
            string setFocusJS =
@"function modalPopupFocus()
{{
    var createFilePopup = $find('{0}');
    createFilePopup.add_shown(SetCreateFileFocus);
    var createFolderPopup = $find('{2}');
    createFolderPopup.add_shown(SetCreateFolderFocus);
    var createDbPopup = $find('{4}');
    createDbPopup.add_shown(SetCreateDbFocus);
    var createZipPopup = $find('{6}');
    createZipPopup.add_shown(SetCreateZipFocus);
}}

function SetCreateFileFocus()
{{
    $get('{1}').focus();
}}

function SetCreateFolderFocus()
{{
    $get('{3}').focus();
}}

function SetCreateDbFocus()
{{
    $get('{5}').focus();
}}

function SetCreateZipFocus()
{{
    $get('{7}').focus();
}}
";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "setFocus",
                String.Format(setFocusJS,
                    CreateFileModal.BehaviorID, CreateFilePanel.FindControl("txtFileName").FindControl("txtFileName").ClientID,
                    CreateFolderModal.BehaviorID, CreateFolderPanel.FindControl("txtFolderName").FindControl("txtFileName").ClientID,
                    CreateDatabaseModal.BehaviorID, CreateDatabasePanel.FindControl("txtDatabaseName").FindControl("txtFileName").ClientID,
                    ZipFilesModal.BehaviorID, ZipFilesPanel.FindControl("txtZipName").FindControl("txtFileName").ClientID
                    ),
                true);
        }

        public string GetDownloadLink(string fileName)
        {
            string path = GetFullRelativePath(fileName);
            return NavigateURL(PortalUtils.SPACE_ID_PARAM, PanelSecurity.PackageId.ToString(),
                    "DownloadFile=" + Server.UrlEncode(path));
        }

        public string GetFileIcon(object obj)
        {
            SystemFile item = (SystemFile)obj;
            string iconsPath = GetThemedImage("FileManager/");

            string imagePath = "";

            // folder icon
            if (item.IsDirectory)
                return iconsPath + "OpenFolder.gif";
            else
            {
                string ext = Path.GetExtension(item.Name);
                ext = (ext.Length > 0) ? ext.Substring(1) : ext;
                imagePath = iconsPath + ext + ".png";
            }

            // check icon existance
            if (!File.Exists(Server.MapPath(imagePath)))
                imagePath = iconsPath + "unknown.png";

            return imagePath;
        }

        public string GetFileSize(long size)
        {
            if (size >= 0x400 && size < 0x100000)
                // kilobytes
                return Convert.ToString((int)Math.Round(((float)size / 1024))) + "K";
            else if (size >= 0x100000 && size < 0x40000000)
                // megabytes
                return Convert.ToString((int)Math.Round(((float)size / 1024 / 1024))) + "M";
            else if (size >= 0x40000000 && size < 0x10000000000)
                // gigabytes
                return Convert.ToString((int)Math.Round(((float)size / 1024 / 1024 / 1024))) + "G";
            else
                return size.ToString();
        }

        public bool IsFolder(object obj)
        {
            return ((SystemFile)obj).IsDirectory;
        }

        public bool IsEditable(object obj)
        {
            SystemFile file = (SystemFile)obj;
            if (file.IsDirectory)
                return false;

            // Get the Editable Extensions from the System Settings
            // If it has not yet been set, we will use the original FuseCP allowed editable extensions
            EnterpriseServer.SystemSettings settings = ES.Services.Files.GetFileManagerSettings();
            if (!String.IsNullOrEmpty(settings["EditableExtensions"]))
            {
                ALLOWED_EDIT_EXTENSIONS = settings["EditableExtensions"];
            }

            string ext = Path.GetExtension(file.Name);
            return ALLOWED_EDIT_EXTENSIONS.Split(',').ToArray().Contains(ext);
        }

        #region Path methods
        private void BindPath()
        {
            List<SystemFile> pathList = new List<SystemFile>();

            // add "Home" link
            SystemFile item = new SystemFile(GetLocalizedString("Home.Text"), "\\", false, 0, DateTime.Now, DateTime.Now);
            pathList.Add(item);

            string currPath = Server.HtmlDecode(litPath.Text).Substring(1);

            if (currPath != "")
            {
                string[] pathParts = currPath.Split(new char[] { '\\', '/' });
                for (int i = 0; i < pathParts.Length; i++)
                {
                    string subPath = "\\" + String.Join("\\", pathParts, 0, i + 1);
                    item = new SystemFile(pathParts[i], subPath, false, 0, DateTime.Now, DateTime.Now);
                    pathList.Add(item);
                }
            }

            path.DataSource = pathList;
            path.DataBind();
        }

        protected void path_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "browse")
            {                
                litPath.Text =  PortalAntiXSS.Encode((string)e.CommandArgument);
                BindPath();
            }
        }
        #endregion

        #region Upload
        protected void btnUpload_Click(object sender, EventArgs e)
        {
            // upload file
            this.UploadFile(fileUpload1.FileName, fileUpload1.FileContent);
            this.UploadFile(fileUpload2.FileName, fileUpload2.FileContent);
            this.UploadFile(fileUpload3.FileName, fileUpload3.FileContent);
            this.UploadFile(fileUpload4.FileName, fileUpload4.FileContent);
            this.UploadFile(fileUpload5.FileName, fileUpload5.FileContent);
            // hide form
            gvFiles.DataBind();
        }

        private void UploadFile(string fileName, Stream stream)
        {
            // upload file
            if (!String.IsNullOrEmpty(fileName))
            {
                string path = GetFullRelativePath(Path.GetFileName(fileName));

                try
                {
                    int result = ES.Services.Files.CreateFile(PanelSecurity.PackageId, path);
                    if (result < 0)
                    {
                        messageBox.ShowResultMessage(result);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    messageBox.ShowErrorMessage("FILES_CREATE_FILE", ex);
                }

                int buffLength = 1000000;
                byte[] buffer = new byte[buffLength];

                int readLength = 0;
                while (true)
                {
                    readLength = stream.Read(buffer, 0, buffLength);

                    if (readLength < buffLength)
                        Array.Resize<byte>(ref buffer, readLength);

                    try
                    {
                        int result = ES.Services.Files.AppendFileBinaryChunk(PanelSecurity.PackageId, path, buffer);
                        if (result < 0)
                        {
                            messageBox.ShowResultMessage(result);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        messageBox.ShowErrorMessage("FILES_UPLOAD_FILE", ex);
                        break;
                    }

                    if (readLength < buffLength)
                        break;
                }
            }
        }

        #endregion

        #region Create File
        protected void btnCreateFile_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                CreateFileModal.Show();
                return;
            }

            // create file
            string path = GetFullRelativePath(txtFileName.Text.Trim());
            try
            {
                int result = ES.Services.Files.CreateFile(PanelSecurity.PackageId, path);
                if (result < 0)
                {
                    messageBox.ShowResultMessage(result);
                    return;
                }

                // update content
                if (txtFileContent.Text != "")
                {
                    byte[] content = Encoding.UTF8.GetBytes(txtFileContent.Text);
                    result = ES.Services.Files.UpdateFileBinaryContent(PanelSecurity.PackageId, path, content);
                    if (result < 0)
                    {
                        messageBox.ShowResultMessage(result);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {

                messageBox.ShowErrorMessage("FILES_CREATE_FILE", ex);
            }

            // hide form
            gvFiles.DataBind();
        }
        #endregion

        #region Create Folder
        protected void btnCreateFolder_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                CreateFolderModal.Show();
                return;
            }

            // create folder
            string path = GetFullRelativePath(txtFolderName.Text.Trim());
            try
            {
                int result = ES.Services.Files.CreateFolder(PanelSecurity.PackageId, path);
                if (result < 0)
                {
                    messageBox.ShowResultMessage(result);
                    return;
                }
            }
            catch (Exception ex)
            {
                messageBox.ShowErrorMessage("FILES_CREATE_FOLDER", ex);
            }

            // hide form
            gvFiles.DataBind();
        }
        #endregion

        #region Zip
        protected void btnZip_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                ZipFilesModal.Show();
                return;
            }

            // zip files
            string fileName = txtZipName.Text.Trim();
            if (!fileName.ToLower().EndsWith(".zip"))
                fileName = fileName + ".zip";

            string path = GetFullRelativePath(fileName);
            string[] files = GetSelectedFiles();
            if (files.Length > 0)
            {
                try
                {
                    int result = ES.Services.Files.ZipFiles(PanelSecurity.PackageId, files, path);
                    if (result < 0)
                    {
                        messageBox.ShowResultMessage(result);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    messageBox.ShowErrorMessage("FILES_ZIP_FILES", ex);
                }

                // hide form
                gvFiles.DataBind();
            }
        }
        #endregion

        #region Unzip
        protected void cmdUnzipFiles_Click(object sender, EventArgs e)
        {
            // unzip files
            string[] files = GetSelectedFiles();
            if (files.Length > 0)
            {
                try
                {
                    ES.Services.Files.UnzipFiles(PanelSecurity.PackageId, files);
                }
                catch (Exception ex)
                {
                    messageBox.ShowErrorMessage("FILES_UNZIP_FILES", ex);
                }

                // refresh list
                gvFiles.DataBind();
            }
        }
        #endregion

        #region Copy
        protected void btnCopy_Click(object sender, EventArgs e)
        {
            // copy files
            string path = copyDestination.SelectedFile;
            string[] files = GetSelectedFiles();
            if (files.Length > 0)
            {
                try
                {
                    int result = ES.Services.Files.CopyFiles(PanelSecurity.PackageId, files, path);
                    if (result < 0)
                    {
                        messageBox.ShowResultMessage(result);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    messageBox.ShowErrorMessage("FILES_COPY_FILES", ex);
                }

                // hide form
                gvFiles.DataBind();
            }
        }
        #endregion

        #region Move
        protected void btnMove_Click(object sender, EventArgs e)
        {
            // move files
            string path = moveDestination.SelectedFile;
            string[] files = GetSelectedFiles();
            if (files.Length > 0)
            {
                try
                {
                    int result = ES.Services.Files.MoveFiles(PanelSecurity.PackageId, files, path);
                    if (result < 0)
                    {
                        messageBox.ShowResultMessage(result);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    messageBox.ShowErrorMessage("FILES_MOVE_FILES", ex);
                }

                // hide form
                gvFiles.DataBind();
            }
        }
        #endregion

        #region Edit
        protected void btnSaveEditFile_Click(object sender, EventArgs e)
        {
            // create file path
            string path = (string)ViewState["EditFile"];
            try
            {
                //update file
                int result = PutFileContents(path, ddlFileEncodings.SelectedValue, txtEditFileContent.Text);
                if (result < 0)
                {
                    messageBox.ShowResultMessage(result);
                    return;
                }
            }
            catch (Exception ex)
            {
                messageBox.ShowErrorMessage("FILES_UPDATE_FILE", ex);
            }

            // hide form
            EditFileModal.Hide();
            gvFiles.DataBind();
        }
        #endregion

        #region Rename
        protected void btnRename_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                RenameFileModal.Show();
                return;
            }

            // move files
            string oldPath = GetFullRelativePath((string)ViewState["RenameFile"]);
            string newPath = GetFullRelativePath(txtRenameFile.Text);

            try
            {
                int result = ES.Services.Files.RenameFile(PanelSecurity.PackageId, oldPath, newPath);
                if (result < 0)
                {
                    messageBox.ShowResultMessage(result);
                    return;
                }
            }
            catch (Exception ex)
            {
                messageBox.ShowErrorMessage("FILES_RENAME_FILE", ex);
            }

            // hide form
            RenameFileModal.Hide();
            gvFiles.DataBind();
        }
        #endregion

        #region Access Databases
        protected void btnCreateDatabase_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                CreateDatabaseModal.Show();
                return;
            }

            // create database
            string fileName = txtDatabaseName.Text.Trim();
            if (!fileName.ToLower().EndsWith(".mdb"))
                fileName = fileName + ".mdb";

            string path = GetFullRelativePath(fileName);
            try
            {
                int result = ES.Services.Files.CreateAccessDatabase(PanelSecurity.PackageId, path);
                if (result < 0)
                {
                    messageBox.ShowResultMessage(result);
                    return;
                }
            }
            catch (Exception ex)
            {
                messageBox.ShowErrorMessage("FILES_CREATE_DB", ex);
            }

            // hide form
            gvFiles.DataBind();
        }
        #endregion

        #region Delete
        protected void btnDeleteFiles_Click(object sender, EventArgs e)
        {
            // delete files
            string[] files = GetSelectedFiles();
            if (files.Length > 0)
            {
                try
                {
                    int result = ES.Services.Files.DeleteFiles(PanelSecurity.PackageId, files);
                    if (result < 0)
                    {
                        messageBox.ShowResultMessage(result);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    messageBox.ShowErrorMessage("FILES_DELETE_FILES", ex);
                }

                // refresh list
                gvFiles.DataBind();
            }
        }
        #endregion

        #region Set Permissions
        protected void btnSetPermissions_Click(object sender, EventArgs e)
        {
            // create file
            string path = (string)ViewState["EditPermissions"];
            try
            {
                // update permissions
                List<UserPermission> users = new List<UserPermission>();
                foreach (GridViewRow row in gvFilePermissions.Rows)
                {
                    Literal litAccountName = (Literal)row.FindControl("litAccountName");
                    CheckBox chkRead = (CheckBox)row.FindControl("chkRead");
                    CheckBox chkWrite = (CheckBox)row.FindControl("chkWrite");



                    if (litAccountName != null)
                    {
                        UserPermission user = new UserPermission();
                        user.AccountName = litAccountName.Text;
                        user.Read = chkRead.Checked;
                        user.Write = chkWrite.Checked;
                        users.Add(user);
                    }
                }

                int result = ES.Services.Files.SetFilePermissions(PanelSecurity.PackageId, path,
                    users.ToArray(), chkReplaceChildPermissions.Checked);
                if (result < 0)
                {
                    messageBox.ShowResultMessage(result);
                    return;
                }
            }
            catch (Exception ex)
            {
                messageBox.ShowErrorMessage("FILES_UPDATE_PERMISSIONS", ex);
            }

            // hide form
            PermissionsFileModal.Hide();
        }
		protected void btnSetPermissionsUnix_Click(object sender, EventArgs e)
		{
			// create file
			string path = (string)ViewState["EditPermissions"];
			string owner = txtOwner.Text;
			string group = txtGroup.Text;
			try
			{
				Providers.OS.UnixFileMode mode = default;
				if (chkReadOwner.Checked) mode |= Providers.OS.UnixFileMode.UserRead;
				if (chkWriteOwner.Checked) mode |= Providers.OS.UnixFileMode.UserWrite;
				if (chkExecuteOwner.Checked) mode |= Providers.OS.UnixFileMode.UserExecute;
				if (chkReadGroup.Checked) mode |= Providers.OS.UnixFileMode.GroupRead;
				if (chkWriteGroup.Checked) mode |= Providers.OS.UnixFileMode.GroupWrite;
				if (chkExecuteGroup.Checked) mode |= Providers.OS.UnixFileMode.GroupExecute;
				if (chkReadOthers.Checked) mode |= Providers.OS.UnixFileMode.OtherRead;
				if (chkWriteOthers.Checked) mode |= Providers.OS.UnixFileMode.OtherWrite;
				if (chkExecuteOthers.Checked) mode |= Providers.OS.UnixFileMode.OtherExecute;

				// update permissions
				int result = ES.Services.Files.SetUnixFilePermissions(PanelSecurity.PackageId, path, owner, group, mode, chkReplaceChildPermissionsUnix.Checked);

				if (result < 0)
				{
					messageBox.ShowResultMessage(result);
					return;
				}
			}
			catch (Exception ex)
			{
				messageBox.ShowErrorMessage("FILES_UPDATE_PERMISSIONS", ex);
			}

			// hide form
			PermissionsFileModal.Hide();
		}
		#endregion

		private string[] GetSelectedFiles()
        {
            List<string> files = new List<string>();
            foreach (GridViewRow row in gvFiles.Rows)
            {
                CheckBox chkSelected = (CheckBox)row.FindControl("selected");
                if (chkSelected != null && chkSelected.Checked)
                {
                    string fileName = (string)gvFiles.DataKeys[row.RowIndex][0];
                    fileName = GetFullRelativePath(fileName);
                    files.Add(fileName);
                }
            }
            return files.ToArray();
        }

        private string GetFullRelativePath(string fileName)
        {
            return Server.HtmlDecode(litPath.Text) + ((Server.HtmlDecode(litPath.Text) != "\\") ? ("\\" + fileName) : fileName);
        }

        protected void gvFiles_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "browse")
            {
                string fileName = (string)e.CommandArgument;
                litPath.Text += PortalAntiXSS.Encode((litPath.Text != "\\") ? ("\\" + fileName) : fileName);
                BindPath();
            }
            else if (e.CommandName == "download")
            {
                string path = GetFullRelativePath((string)e.CommandArgument);

                Response.Redirect(NavigateURL(PortalUtils.SPACE_ID_PARAM, PanelSecurity.PackageId.ToString(),
                    "DownloadFile=" + Server.UrlEncode(path)));


            }
            else if (e.CommandName == "edit_file")
            {
                // read file content
                try
                {
                    string path = GetFullRelativePath((string)e.CommandArgument);
                    ViewState["EditFile"] = path;

                    txtEditFileContent.Text = GetFileContent(path, ddlFileEncodings.SelectedValue);

                    // show edit panel
                    EditFileModal.Show();
                }
                catch (Exception ex)
                {
                    messageBox.ShowErrorMessage("FILES_READ_FILE", ex);
                }
            }
            else if (e.CommandName == "edit_permissions")
            {
                // read file content
                try
                {
					string path = GetFullRelativePath((string)e.CommandArgument);
					ViewState["EditPermissions"] = path;

					var packageId = PanelSecurity.PackageId;
					var package = ES.Services.Packages.GetPackage(packageId);
					var server = ES.Services.Servers.GetServerById(package.ServerId);
					if (server.OSPlatform == OSPlatform.Windows)
					{
						UserPermission[] users = ES.Services.Files.GetFilePermissions(PanelSecurity.PackageId, path);

						gvFilePermissions.DataSource = users;
						gvFilePermissions.DataBind();

						PermissionsFileModal.PopupControlID = "PermissionsWindowsFilePanel";
					}
					else
					{
						var permissions = ES.Services.Files.GetUnixFilePermissions(PanelSecurity.PackageId, path);
						txtOwner.Text = lblOwnerText.Text = permissions.Owner;
						txtGroup.Text = lblGroupText.Text = permissions.Group;
						chkReadOwner.Checked = permissions.Permissions.HasFlag(Providers.OS.UnixFileMode.UserRead);
						chkWriteOwner.Checked = permissions.Permissions.HasFlag(Providers.OS.UnixFileMode.UserWrite);
						chkExecuteOwner.Checked = permissions.Permissions.HasFlag(Providers.OS.UnixFileMode.UserExecute);
						chkReadGroup.Checked = permissions.Permissions.HasFlag(Providers.OS.UnixFileMode.GroupRead);
						chkWriteGroup.Checked = permissions.Permissions.HasFlag(Providers.OS.UnixFileMode.GroupWrite);
						chkExecuteGroup.Checked = permissions.Permissions.HasFlag(Providers.OS.UnixFileMode.GroupExecute);
						chkReadOthers.Checked = permissions.Permissions.HasFlag(Providers.OS.UnixFileMode.OtherRead);
						chkWriteOthers.Checked = permissions.Permissions.HasFlag(Providers.OS.UnixFileMode.OtherWrite);
						chkExecuteOthers.Checked = permissions.Permissions.HasFlag(Providers.OS.UnixFileMode.OtherExecute);

						PermissionsFileModal.PopupControlID = "PermissionsUnixFilePanel";
					}

					// show permissions panel
					PermissionsFileModal.Show();
                }
                catch (Exception ex)
                {
                    messageBox.ShowErrorMessage("FILES_READ_PERMISSIONS", ex);
                }
            }
            else if (e.CommandName == "rename")
            {
                RenameFileModal.Show();

                ViewState["RenameFile"] = (string)e.CommandArgument;
                txtRenameFile.Text = (string)e.CommandArgument;
            }
        }

        /// <summary>
        /// Returns string representing file contents.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <param name="encoding">Encoding to use when reading file. Web Name of encoding must be specified. See <see cref="Encoding"/> for details.</param>
        /// <returns>String representing file contents.</returns>
        private string GetFileContent(string path, string encoding)
        {
            Encoding currentEncoding = Encoding.GetEncoding(encoding);

            byte[] content = ES.Services.Files.GetFileBinaryContentUsingEncoding(PanelSecurity.PackageId, path, currentEncoding.WebName);

            return currentEncoding.GetString(content);
        }

        /// <summary>
        /// Saves file.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <param name="encoding">Name of encoding that will be used to store file contents.</param>
        /// <param name="fileContents">String representing file contents.</param>
        /// <returns>Error code or 0 if succeeded.</returns>
        private int PutFileContents(string path, string encoding, string fileContents)
        {
            return ES.Services.Files.UpdateFileBinaryContentUsingEncoding(
                PanelSecurity.PackageId
                , path
                , Encoding.GetEncoding(encoding).GetBytes(fileContents).Clone() as byte[]
                , encoding
                );
        }

        protected void ddlFileEncodings_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string path = (string)ViewState["EditFile"];

                txtEditFileContent.Text = GetFileContent(path, ddlFileEncodings.SelectedValue);

                EditFileModal.Show();
            }
            catch (Exception ex)
            {
                messageBox.ShowErrorMessage("FILES_READ_FILE", ex);
            }
        }

        protected void odsFilesPaged_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                if (PanelSecurity.SelectedUser.Role == FuseCP.EnterpriseServer.UserRole.Administrator)
                    messageBox.ShowWarningMessage("FILES_GET_LIST");
                else
                    messageBox.ShowErrorMessage("FILES_GET_LIST", e.Exception);

                //mnuOperations.Enabled = false;
                btnRecalcDiskspace.Enabled = false;
                e.ExceptionHandled = true;
            }
        }

		protected void odsFilesPaged_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			if (e.InputParameters == null)
				return;
			// Decode path parameter to pass it to the server as is
			if (e.InputParameters.Contains("path") == true)
			{
				e.InputParameters["path"] = Server.HtmlDecode((String)e.InputParameters["path"]);
			}
		}

        protected void btnRecalcDiskspace_Click(object sender, EventArgs e)
        {
            int result = ES.Services.Files.CalculatePackageDiskspace(PanelSecurity.PackageId);
            if (result < 0)
            {
                messageBox.ShowResultMessage(result);
                return;
            }
        }

        protected void btnCancelRename_Click(object sender, EventArgs e)
        {
            RenameFileModal.Hide();
        }

        protected void btnCancelEditFile_Click(object sender, EventArgs e)
        {
            EditFileModal.Hide();
        }

        protected void btnCancelPermissions_Click(object sender, EventArgs e)
        {
            PermissionsFileModal.Hide();
        }
		protected void btnCancelPermissionsUnix_Click(object sender, EventArgs e)
		{
			PermissionsFileModal.Hide();
		}
		private void RegisterJsLocalizedMessages()
		{
			if (!Page.ClientScript.IsClientScriptBlockRegistered("FM_UNZIP_FILES_MESSAGE"))
			{
				Page.ClientScript.RegisterClientScriptBlock(GetType(),
					"FM_UNZIP_FILES_MESSAGE",
					String.Format("FM_UNZIP_FILES_MESSAGE = \"{0}\";", GetLocalResourceObject("FM_UNZIP_FILES_MESSAGE")),
					true);
			}
		}
    }
}
