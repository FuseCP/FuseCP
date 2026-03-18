<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileManager.ascx.cs" Inherits="FuseCP.Portal.FileManager" %>
<%@ Register Src="UserControls/FileNameControl.ascx" TagName="FileNameControl" TagPrefix="uc2" %>
<%@ Register Src="UserControls/FileLookup.ascx" TagName="FileLookup" TagPrefix="uc1" %>
<%@ Register Src="UserControls/Quota.ascx" TagName="Quota" TagPrefix="uc4" %>
<%@ Register Src="UserControls/PopupHeader.ascx" TagName="PopupHeader" TagPrefix="fcp" %>
<%@ Register Src="UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport"
	TagPrefix="fcp" %>
	
<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server" />

<asp:UpdatePanel ID="MessageBoxUpdatePanel" runat="server" UpdateMode="Always">
<ContentTemplate>
<fcp:SimpleMessageBox id="messageBox" runat="server"></fcp:SimpleMessageBox>
</ContentTemplate>
</asp:UpdatePanel>

<script type="text/javascript" src="/DesktopModules/FuseCP/Scripts/file-manager.js"></script>

<div class="FormButtonsBar">
		<asp:LinkButton ID="cmdUpload" runat="server" CssClass="btn btn-secondary" CausesValidation="false">
			<asp:Image ID="Img1" runat="server" SkinID="FM_Upload" /><asp:Localize ID="locUpload" runat="server" meta:resourcekey="locUpload" Text="Upload"/>
		</asp:LinkButton>
		<asp:LinkButton ID="cmdCreateFile" runat="server" CssClass="btn btn-secondary" CausesValidation="false">
			<asp:Image ID="Img2" runat="server" SkinID="FM_CreateFile" /><asp:Localize ID="locCreateFile" runat="server" meta:resourcekey="locCreateFile" Text="Create File"/>
		</asp:LinkButton>
		<asp:LinkButton ID="cmdCreateFolder" runat="server" CssClass="btn btn-secondary" CausesValidation="false">
			<asp:Image ID="Img3" runat="server" SkinID="FM_CreateFolder" /><asp:Localize ID="locCreateFolder" runat="server" meta:resourcekey="locCreateFolder" Text="Create Folder"/>
		</asp:LinkButton>
	
	<asp:Image ID="Image8" runat="server" SkinID="FM_Separator" />
	
		<asp:LinkButton ID="cmdCreateAccessDB" runat="server" CssClass="btn btn-secondary" CausesValidation="false" Enabled="false" >
			<asp:Image ID="Image2" runat="server" SkinID="FM_CreateAccessDB" /><asp:Localize ID="locCreateAccessDB" runat="server" meta:resourcekey="locCreateAccessDB" Text="Create Access DB" />
		</asp:LinkButton>
	
	<asp:Image ID="Image9" runat="server" SkinID="FM_Separator" />
	
		<asp:LinkButton ID="cmdZipFiles" runat="server" CssClass="btn btn-secondary" CausesValidation="false">
			<asp:Image ID="Image3" runat="server" SkinID="FM_Zip" /><asp:Localize ID="locZip" runat="server" meta:resourcekey="locZip" Text="Zip"/>
		</asp:LinkButton>
		<asp:LinkButton ID="cmdUnzipFiles" runat="server" CssClass="btn btn-secondary" CausesValidation="false" OnClick="cmdUnzipFiles_Click" OnClientClick="ShowUnzipFilesDialog();">
			<asp:Image ID="Image4" runat="server" SkinID="FM_Unzip" /><asp:Localize ID="locUnzip" runat="server" meta:resourcekey="locUnzip" Text="Unzip"/>
		</asp:LinkButton>
	
	<asp:Image ID="Image10" runat="server" SkinID="FM_Separator" />
	
		<asp:LinkButton ID="cmdCopyFiles" runat="server" CssClass="btn btn-secondary" CausesValidation="false">
			<asp:Image ID="Image5" runat="server" SkinID="FM_Copy" /><asp:Localize ID="locCopy" runat="server" meta:resourcekey="locCopy" Text="Copy"/>
		</asp:LinkButton>
		<asp:LinkButton ID="cmdMoveFiles" runat="server" CssClass="btn btn-secondary" CausesValidation="false">
			<asp:Image ID="Image6" runat="server" SkinID="FM_Move" /><asp:Localize ID="locMove" runat="server" meta:resourcekey="locMove" Text="Move"/>
		</asp:LinkButton>
		<asp:LinkButton ID="cmdDeleteFiles" runat="server" CssClass="btn btn-secondary" CausesValidation="false">
			<asp:Image ID="Image7" runat="server" SkinID="FM_Delete" /><asp:Localize ID="locDelete" runat="server" meta:resourcekey="locDelete" Text="Delete"/>
		</asp:LinkButton>
</div>

<asp:Panel ID="UploadFilePanel" runat="server" CssClass="PopupContainer" style="display:none" DefaultButton="btnUpload">
    <div class="widget">
        <div class="widget-content Popup">
        <div class="d-flex justify-content-between align-items-center mb-3 pb-2 border-bottom">
            <h3 class="m-0 fs-6 text-secondary"><i class="bi bi-upload"></i> <fcp:PopupHeader runat="server" meta:resourcekey="lblUploadFile" Text="Upload File" /></h3>
            <button type="button" class="btn btn-sm btn-outline-secondary" onclick="$find(this).get_element().closest('.PopupContainer').style.display='none'; return false;">
                <i class="bi bi-x-lg" aria-hidden="true"></i>
                <span class="ms-1">Close</span>
            </button>
        </div>
			<div class="FormRow">
				<asp:FileUpload ID="fileUpload1" runat="server" Width="400px" />
				<asp:FileUpload ID="fileUpload2" runat="server" Width="400px" />
				<asp:FileUpload ID="fileUpload3" runat="server" Width="400px" />
				<asp:FileUpload ID="fileUpload4" runat="server" Width="400px" />
				<asp:FileUpload ID="fileUpload5" runat="server" Width="400px" />
			</div>
			</div>
					<div class="popup-buttons text-end">
            <asp:LinkButton id="btnCancelUpload" CssClass="btn btn-warning" runat="server" CausesValidation="False"> <i class="bi bi-x-lg">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnCancelText"/> </asp:LinkButton>&nbsp;
            <asp:LinkButton id="btnUpload" CssClass="btn btn-success" runat="server" ValidationGroup="UploadFile" OnClick="btnUpload_Click"> <i class="bi bi-upload">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnUploadText"/> </asp:LinkButton>
	</div>
  </div>
</asp:Panel>

<ajaxToolkit:ModalPopupExtender ID="UploadModal" runat="server"
    TargetControlID="cmdUpload" PopupControlID="UploadFilePanel"
    BackgroundCssClass="modalBackground" DropShadow="false" CancelControlID="btnCancelUpload" />

<asp:Panel ID="CopyFilesPanel" runat="server" CssClass="PopupContainer" style="display:none" DefaultButton="btnCopy">
     <div class="widget">
        <div class="widget-content Popup">
        <div class="d-flex justify-content-between align-items-center mb-3 pb-2 border-bottom">
            <h3 class="m-0 fs-6 text-secondary"><i class="bi bi-files"></i> <fcp:PopupHeader runat="server" meta:resourcekey="lblCopySelectedFiles" Text="Copy Selected Files" /></h3>
            <button type="button" class="btn btn-sm btn-outline-secondary" onclick="$find(this).get_element().closest('.PopupContainer').style.display='none'; return false;">
                <i class="bi bi-x-lg" aria-hidden="true"></i>
                <span class="ms-1">Close</span>
            </button>
        </div>
			<div class="FormRow">
				<asp:Label ID="lblDestinationFolder1" runat="server" meta:resourcekey="lblDestinationFolder" Text="Destination Folder:"></asp:Label>
				<uc1:FileLookup ID="copyDestination" runat="server" Width="400px" DropShadow="False" ValidationGroup="CopyFiles" />
			</div>
			</div>
					<div class="popup-buttons text-end">
            <asp:LinkButton id="btnCancelCopy" CssClass="btn btn-warning" runat="server" CausesValidation="False"> <i class="bi bi-x-lg">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnCancelText"/> </asp:LinkButton>&nbsp;
            <asp:LinkButton id="btnCopy" CssClass="btn btn-success" runat="server" OnClick="btnCopy_Click" ValidationGroup="CopyFiles"> <i class="bi bi-copy">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnCopyText"/> </asp:LinkButton>
		</div>
	</div>
</asp:Panel>

<ajaxToolkit:ModalPopupExtender ID="CopyFilesModal" runat="server"
    TargetControlID="cmdCopyFiles" PopupControlID="CopyFilesPanel"
    BackgroundCssClass="modalBackground" DropShadow="false" CancelControlID="btnCancelCopy" />

<asp:Panel ID="MoveFilesPanel" runat="server" CssClass="PopupContainer" style="display:none" DefaultButton="btnMove">
    <div class="widget">
        <div class="widget-content Popup">
        <div class="d-flex justify-content-between align-items-center mb-3 pb-2 border-bottom">
            <h3 class="m-0 fs-6 text-secondary"><i class="bi bi-clipboard"></i> <fcp:PopupHeader runat="server" meta:resourcekey="lblMoveSelectedFiles" Text="Move Selected Files" /></h3>
            <button type="button" class="btn btn-sm btn-outline-secondary" onclick="$find(this).get_element().closest('.PopupContainer').style.display='none'; return false;">
                <i class="bi bi-x-lg" aria-hidden="true"></i>
                <span class="ms-1">Close</span>
            </button>
        </div>
			<div class="FormRow">
				<asp:Label ID="lblDestinationFolder2" runat="server" meta:resourcekey="lblDestinationFolder" Text="Destination Folder:"></asp:Label>
				<uc1:FileLookup ID="moveDestination" runat="server" Width="400px" DropShadow="False" ValidationGroup="MoveFiles" />
			</div>
			</div>
					<div class="popup-buttons text-end">
			<asp:LinkButton id="btnCancelMove" CssClass="btn btn-warning" runat="server" CausesValidation="False"> <i class="bi bi-x-lg">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnCancelText"/> </asp:LinkButton>&nbsp;
            <asp:LinkButton id="btnMove" CssClass="btn btn-success" runat="server" OnClick="btnMove_Click" ValidationGroup="MoveFiles"> <i class="bi bi-check-lg">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnMoveText"/> </asp:LinkButton>
		</div>
     </div>
</asp:Panel>

<ajaxToolkit:ModalPopupExtender ID="MoveFilesModal" runat="server"
    TargetControlID="cmdMoveFiles" PopupControlID="MoveFilesPanel"
    BackgroundCssClass="modalBackground" DropShadow="false" CancelControlID="btnCancelMove" />


<asp:Panel ID="CreateFilePanel" runat="server" CssClass="PopupContainer mpeTarget" style="display:none" DefaultButton="btnCreateFile">
    <div class="widget">
        <div class="widget-content Popup">
        <div class="d-flex justify-content-between align-items-center mb-3 pb-2 border-bottom">
            <h3 class="m-0 fs-6 text-secondary"><i class="bi bi-file-earmark-code"></i> <fcp:PopupHeader runat="server" meta:resourcekey="lblCreateFile" Text="Create Text File" /></h3>
            <button type="button" class="btn btn-sm btn-outline-secondary" onclick="$find(this).get_element().closest('.PopupContainer').style.display='none'; return false;">
                <i class="bi bi-x-lg" aria-hidden="true"></i>
                <span class="ms-1">Close</span>
            </button>
        </div>
			<div class="FormRow">
				<asp:Label ID="lblFileName" runat="server" meta:resourcekey="lblFileName" Text="File Name:"></asp:Label>
				<uc2:FileNameControl ID="txtFileName" runat="server" ValidationGroup="NewFileName" Width="400px" />
			</div>
			<div class="FormRow">
				<asp:Label ID="lblFileContentOptional" runat="server" meta:resourcekey="lblFileContentOptional" Text="File Content (Optional):"></asp:Label>
				<asp:TextBox ID="txtFileContent" runat="server" Rows="10" TextMode="MultiLine" Wrap="False"></asp:TextBox>
			</div>
            </div>
					<div class="popup-buttons text-end">
            <asp:LinkButton id="btnCancelCreateFile" CssClass="btn btn-warning" runat="server" CausesValidation="False"> <i class="bi bi-x-lg">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnCancelText"/> </asp:LinkButton>&nbsp;
            <asp:LinkButton id="btnCreateFile" CssClass="btn btn-success" runat="server" OnClick="btnCreateFile_Click" ValidationGroup="NewFileName"> <i class="bi bi-check-lg">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnCreateText"/> </asp:LinkButton>
		</div>
	</div>
</asp:Panel>

<ajaxToolkit:ModalPopupExtender ID="CreateFileModal" runat="server"
    TargetControlID="cmdCreateFile" PopupControlID="CreateFilePanel"
    BackgroundCssClass="modalBackground" DropShadow="false" CancelControlID="btnCancelCreateFile" />
    
    
<asp:Panel ID="CreateFolderPanel" runat="server" CssClass="PopupContainer" style="display:none" DefaultButton="btnCreateFolder">
    <div class="widget">
        <div class="widget-content Popup">
        <div class="d-flex justify-content-between align-items-center mb-3 pb-2 border-bottom">
            <h3 class="m-0 fs-6 text-secondary"><i class="bi bi-folder"></i> <fcp:PopupHeader runat="server" meta:resourcekey="lblCreateFolder" Text="Create Folder" /></h3>
            <button type="button" class="btn btn-sm btn-outline-secondary" onclick="$find(this).get_element().closest('.PopupContainer').style.display='none'; return false;">
                <i class="bi bi-x-lg" aria-hidden="true"></i>
                <span class="ms-1">Close</span>
            </button>
        </div>
			<div class="FormRow">
				<asp:Label ID="lblFolderName" runat="server" meta:resourcekey="lblFolderName" Text="Folder Name:"></asp:Label>
				<uc2:FileNameControl ID="txtFolderName" runat="server" ValidationGroup="NewFolderName" Width="400px" />
			</div>
			</div>
					<div class="popup-buttons text-end">
            <asp:LinkButton id="btnCancelCreateFolder" CssClass="btn btn-warning" runat="server" CausesValidation="False"> <i class="bi bi-x-lg">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnCancelText"/> </asp:LinkButton>&nbsp;
            <asp:LinkButton id="btnCreateFolder" CssClass="btn btn-success" runat="server" OnClick="btnCreateFolder_Click" ValidationGroup="NewFolderName"> <i class="bi bi-check-lg">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnCreateText"/> </asp:LinkButton>
		</div>
	</div>
</asp:Panel>

<ajaxToolkit:ModalPopupExtender ID="CreateFolderModal" runat="server"
    TargetControlID="cmdCreateFolder" PopupControlID="CreateFolderPanel"
    BackgroundCssClass="modalBackground" DropShadow="false" CancelControlID="btnCancelCreateFolder" />

<asp:Panel ID="ZipFilesPanel" runat="server" CssClass="PopupContainer" style="display:none" DefaultButton="btnZip">
    <div class="widget">
        <div class="widget-content Popup">
        <div class="d-flex justify-content-between align-items-center mb-3 pb-2 border-bottom">
            <h3 class="m-0 fs-6 text-secondary"><i class="bi bi-file-earmark-zip"></i> <fcp:PopupHeader runat="server" meta:resourcekey="lblZipFiles" Text="Zip Selected Files" /></h3>
            <button type="button" class="btn btn-sm btn-outline-secondary" onclick="$find(this).get_element().closest('.PopupContainer').style.display='none'; return false;">
                <i class="bi bi-x-lg" aria-hidden="true"></i>
                <span class="ms-1">Close</span>
            </button>
        </div>
			<div class="FormRow">
				<asp:Label ID="lblZIPArchiveName" runat="server" meta:resourcekey="lblZIPArchiveName" Text="ZIP Archive Name:"></asp:Label>
				<uc2:FileNameControl ID="txtZipName" runat="server" ValidationGroup="ZipName" Width="400px" />
			</div>
			</div>
					<div class="popup-buttons text-end">
            <asp:LinkButton id="btnCancelZip" CssClass="btn btn-warning" runat="server" CausesValidation="False"> <i class="bi bi-x-lg">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnCancelText"/> </asp:LinkButton>&nbsp;
            <asp:LinkButton id="btnZip" CssClass="btn btn-success" runat="server" OnClick="btnZip_Click" ValidationGroup="ZipName"> <i class="bi bi-file-earmark-zip">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnZipText"/> </asp:LinkButton>
		</div>
	</div>
</asp:Panel>

<ajaxToolkit:ModalPopupExtender ID="ZipFilesModal" runat="server"
    TargetControlID="cmdZipFiles" PopupControlID="ZipFilesPanel"
    BackgroundCssClass="modalBackground" DropShadow="false" CancelControlID="btnCancelZip" />

<asp:Panel ID="CreateDatabasePanel" runat="server" CssClass="PopupContainer" style="display:none" DefaultButton="btnCreateDatabase">
    <div class="widget">
        <div class="widget-content Popup">
        <div class="d-flex justify-content-between align-items-center mb-3 pb-2 border-bottom">
            <h3 class="m-0 fs-6 text-secondary"><i class="bi bi-database"></i> <fcp:PopupHeader runat="server" meta:resourcekey="lblCreateAccessDatabase" Text="Create Access Database" /></h3>
            <button type="button" class="btn btn-sm btn-outline-secondary" onclick="$find(this).get_element().closest('.PopupContainer').style.display='none'; return false;">
                <i class="bi bi-x-lg" aria-hidden="true"></i>
                <span class="ms-1">Close</span>
            </button>
        </div>
			<div class="FormRow">
				<asp:Label ID="lblDatabaseName" runat="server" meta:resourcekey="lblDatabaseName" Text="Database Name:"></asp:Label>
				<uc2:FileNameControl ID="txtDatabaseName" runat="server" ValidationGroup="DatabaseName" Width="400px" />
			</div>
			</div>
					<div class="popup-buttons text-end">
            <asp:LinkButton id="btnCancelCreateDatabase" CssClass="btn btn-warning" runat="server" CausesValidation="False"> <i class="bi bi-x-lg">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnCancelText"/> </asp:LinkButton>&nbsp;
            <asp:LinkButton id="btnCreateDatabase" CssClass="btn btn-success" runat="server" OnClick="btnCreateDatabase_Click" ValidationGroup="DatabaseName"> <i class="bi bi-database">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnCreateText"/> </asp:LinkButton>
		</div>
	</div>
</asp:Panel>

<ajaxToolkit:ModalPopupExtender ID="CreateDatabaseModal" runat="server"
    TargetControlID="cmdCreateAccessDB" PopupControlID="CreateDatabasePanel"
    BackgroundCssClass="modalBackground" DropShadow="false" CancelControlID="btnCancelCreateDatabase" />


<asp:Panel ID="DeleteFilesPanel" runat="server" CssClass="PopupContainer" style="display:none">
    <div class="widget">
        <div class="widget-content Popup">
        <div class="d-flex justify-content-between align-items-center mb-3 pb-2 border-bottom">
            <h3 class="m-0 fs-6 text-secondary"><i class="bi bi-trash"></i> <fcp:PopupHeader runat="server" meta:resourcekey="lblDeleteFiles" Text="Delete Selected Files" /></h3>
            <button type="button" class="btn btn-sm btn-outline-secondary" onclick="$find(this).get_element().closest('.PopupContainer').style.display='none'; return false;">
                <i class="bi bi-x-lg" aria-hidden="true"></i>
                <span class="ms-1">Close</span>
            </button>
        </div>
			<div class="FormRow">
				<asp:Label ID="lblDeleteConfirmation" runat="server" meta:resourcekey="lblDeleteConfirmation" Text="Do you really want to delete selected files and folders?"></asp:Label>
			</div>
			</div>
					<div class="popup-buttons text-end">
            <asp:LinkButton id="btnDeleteFiles" CssClass="btn btn-danger" runat="server" OnClick="btnDeleteFiles_Click" CausesValidation="false"> <i class="bi bi-trash">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnDeleteFilesText"/> </asp:LinkButton>&nbsp;
            <asp:LinkButton id="btnCancelDeleteFiles" CssClass="btn btn-warning" runat="server"> <i class="bi bi-x-lg">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnCancelDeleteFilesText"/> </asp:LinkButton>
		</div>
	</div>
</asp:Panel>

<ajaxToolkit:ModalPopupExtender ID="DeleteFilesModal" runat="server"
    TargetControlID="cmdDeleteFiles" PopupControlID="DeleteFilesPanel"
    BackgroundCssClass="modalBackground" DropShadow="false" CancelControlID="btnCancelDeleteFiles" />
    

<asp:UpdatePanel ID="FilesUpdatePanel" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
    <ContentTemplate>
 <div class="FormButtonsBar">
	<table class="table table-borderless align-middle mb-0 w-100">
		<tr>
			<td class="Medium" >
				<asp:Repeater ID="path" Runat="server" OnItemCommand="path_ItemCommand">
					<ItemTemplate><asp:LinkButton ID="fileName" Runat="server" CssClass=CommandButton CommandName="browse" CommandArgument='<%# Eval("FullName")%>' Text='<%# Eval("Name")%>' CausesValidation="false">
					</asp:LinkButton></ItemTemplate>
					<SeparatorTemplate>
						&nbsp;/&nbsp;
					</SeparatorTemplate>
				</asp:Repeater>
			</td>
			<td>
			    <asp:UpdateProgress ID="filesProgress" runat="server"
			        AssociatedUpdatePanelID="FilesUpdatePanel" DynamicLayout="false">
			        <ProgressTemplate>
			            <asp:Image ID="imgSep" runat="server" SkinID="MediumAjaxIndicator" />
			        </ProgressTemplate>
			    </asp:UpdateProgress>
			</td>
		</tr>
	</table>
</div><div class="NormalGridView">
	<div class="AspNet-GridView">
		<table class="table table-borderless align-middle mb-0">
			<thead>
				<tr>
					<th ><asp:CheckBox ID="selectAll" Runat="server" onclick="checkAll(this);"></asp:CheckBox></th>
					<th><asp:Localize ID="locFileName" runat="server" meta:resourcekey="locFileName" /></th>
					<th ><asp:Localize ID="locSize" runat="server" meta:resourcekey="locSize" /></th>
					<th ><asp:Localize ID="locModified" runat="server" meta:resourcekey="locModified" /></th>
				</tr>
			</thead>
		</table>
	</div>
</div><div style="height:350px; overflow:auto">
	<asp:GridView ID="gvFiles" runat="server" AutoGenerateColumns="False"
		AllowSorting="True" CssSelectorClass="NormalGridView" ShowHeader="false"
		EmptyDataText="gvFiles" DataKeyNames="Name" OnRowCommand="gvFiles_RowCommand"
		DataSourceID="odsFilesPaged" PageSize="20" EnableViewState="true">
		<Columns>
			<asp:TemplateField>
				<ItemStyle Width="25px"></ItemStyle>
				<ItemTemplate>
					<asp:CheckBox ID="selected" Runat="server" onclick="javascript:HighlightRow(this);"></asp:CheckBox>
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="gvFilesFileName">
				<ItemTemplate>
				    <img src="<%# GetFileIcon(Container.DataItem) %>" class="align-middle me-1">
					<asp:HyperLink ID="lnkDownload" runat="server" Visible='<%# !(bool)Eval("IsDirectory") %>'
						NavigateUrl='<%# GetDownloadLink((string)Eval("Name")) %>'>
						<%# Eval("Name")%>
					</asp:HyperLink>
					<asp:LinkButton ID="fileName" Runat="server" CssClass=CommandButton CausesValidation="false"
							CommandName='<%# (bool)Eval("IsDirectory") ? "browse" : "download" %>' Visible='<%# (bool)Eval("IsDirectory") %>'
							CommandArgument='<%# Eval("Name") %>'>
						<%# Eval("Name")%>
					</asp:LinkButton>
					
					<asp:ImageButton ID="cmdRenameFile" runat="server" SkinID="RenameFile" AlternateText="Rename file/folder"
							CommandName='rename' meta:resourcekey="cmdRenameFile" CausesValidation="false"
							CommandArgument='<%# Eval("Name") %>'></asp:ImageButton>
					<asp:ImageButton ID="cmdEditFile" runat="server" SkinID="EditFile" AlternateText="Edit file"
						visible='<%# IsEditable(Container.DataItem) %>'
							CommandName='edit_file' meta:resourcekey="cmdEditFile" CausesValidation="false"
							CommandArgument='<%# Eval("Name") %>'></asp:ImageButton>
					<asp:ImageButton ID="cmdEditPermissions" runat="server" SkinID="EditPermissions" AlternateText="Edit Permissions"
							CommandName='edit_permissions' meta:resourcekey="cmdEditPermissions" CausesValidation="false"
							CommandArgument='<%# Eval("Name") %>'></asp:ImageButton>
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="gvFilesSize">
				<ItemStyle Wrap="False" Width="65px"></ItemStyle>
				<ItemTemplate>
					<span id="Span1" runat=server visible='<%# !IsFolder(Container.DataItem) %>' title='<%# Eval("Size") %>'>
						<%# GetFileSize((long)Eval("Size")) %>
					</span>
				</ItemTemplate>
			</asp:TemplateField>
			<asp:BoundField DataField="Changed" HeaderText="gvFilesModified">
				<ItemStyle Wrap="False" Width="135px" ></ItemStyle>
			</asp:BoundField>
		</Columns>
	</asp:GridView>
	<asp:Literal ID="litPath" runat="server" Visible="false" Text="\"></asp:Literal>
	<asp:ObjectDataSource ID="odsFilesPaged" runat="server"
		SelectMethod="GetFiles" TypeName="FuseCP.Portal.FilesHelper" MaximumRowsParameterName="" StartRowIndexParameterName="" OnSelected="odsFilesPaged_Selected" 
		OnSelecting="odsFilesPaged_Selecting">
		<SelectParameters>
			<asp:ControlParameter ControlID="litPath" Name="path" PropertyName="Text" />
		</SelectParameters>
	</asp:ObjectDataSource>
</div>

<asp:Panel ID="EditFilePanel" runat="server" CssClass="PopupContainer fileeditor" style="display:none" DefaultButton="btnSaveEditFile">
    <div class="widget">
        <div class="widget-content Popup">
        <div class="d-flex justify-content-between align-items-center mb-3 pb-2 border-bottom">
            <h3 class="m-0 fs-6 text-secondary"><i class="bi bi-pencil"></i> <fcp:PopupHeader runat="server" meta:resourcekey="lblEditFile" Text="Edit File" /></h3>
            <button type="button" class="btn btn-sm btn-outline-secondary" onclick="$find(this).get_element().closest('.PopupContainer').style.display='none'; return false;">
                <i class="bi bi-x-lg" aria-hidden="true"></i>
                <span class="ms-1">Close</span>
            </button>
        </div>
			<div class="FormRow">
				<asp:Label ID="lblFileEncoding" runat="server" meta:resourcekey="lblFileEncoding" Text="Encoding:"></asp:Label><br />
				<asp:DropDownList ID="ddlFileEncodings" runat="server" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlFileEncodings_SelectedIndexChanged"></asp:DropDownList>
			</div>
			<div class="FormRow">
				<asp:Label ID="lblFileContent" runat="server" meta:resourcekey="lblFileContent" Text="File Content:"></asp:Label>
				<asp:TextBox ID="txtEditFileContent" runat="server" Rows="10" TextMode="MultiLine" Wrap="False"></asp:TextBox>
			</div>
            </div>
					<div class="popup-buttons text-end">
            <asp:LinkButton id="btnCancelEditFile" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancelEditFile_Click"> <i class="bi bi-x-lg">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnCancelText"/> </asp:LinkButton>&nbsp;
            <asp:LinkButton id="btnSaveEditFile" CssClass="btn btn-success" runat="server" OnClick="btnSaveEditFile_Click" CausesValidation="false"> <i class="bi bi-floppy">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnSaveEditFileText"/> </asp:LinkButton> 
		</div>
	</div>
</asp:Panel>
<asp:Button ID="btnEditFile" runat="server" style="display:none" />
<ajaxToolkit:ModalPopupExtender ID="EditFileModal" runat="server"
    TargetControlID="btnEditFile" PopupControlID="EditFilePanel"
    BackgroundCssClass="modalBackground" DropShadow="false" />
    
    
<asp:Panel ID="RenameFilePanel" runat="server" CssClass="PopupContainer" style="display:none" DefaultButton="btnRename">
    <div class="widget">
        <div class="widget-content Popup">
        <div class="d-flex justify-content-between align-items-center mb-3 pb-2 border-bottom">
            <h3 class="m-0 fs-6 text-secondary"><i class="bi bi-pencil"></i> <fcp:PopupHeader runat="server" meta:resourcekey="lblRenameFileFolder" Text="Rename File" /></h3>
            <button type="button" class="btn btn-sm btn-outline-secondary" onclick="$find(this).get_element().closest('.PopupContainer').style.display='none'; return false;">
                <i class="bi bi-x-lg" aria-hidden="true"></i>
                <span class="ms-1">Close</span>
            </button>
        </div>
			<div class="FormRow">
				<asp:Label ID="lblNewName" runat="server" meta:resourcekey="lblNewName" Text="New Name:"></asp:Label>
				<uc2:FileNameControl ID="txtRenameFile" runat="server" ValidationGroup="RenameFile" Width="400px" />
			</div>
			</div>
					<div class="popup-buttons text-end">
            <asp:LinkButton id="btnCancelRename" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancelRename_Click"> <i class="bi bi-x-lg">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnCancelText"/> </asp:LinkButton>&nbsp;
            <asp:LinkButton id="btnRename" CssClass="btn btn-success" runat="server" OnClick="btnRename_Click" ValidationGroup="RenameFile"> <i class="bi bi-check-lg">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnRenameText"/> </asp:LinkButton>
		</div>
	</div>
</asp:Panel>
<asp:Button ID="btnRenameFile" runat="server" style="display:none" />
<ajaxToolkit:ModalPopupExtender ID="RenameFileModal" runat="server"
    TargetControlID="btnRenameFile" PopupControlID="RenameFilePanel"
    BackgroundCssClass="modalBackground" DropShadow="false" />
    

<asp:Panel ID="PermissionsWindowsFilePanel" runat="server" CssClass="PopupContainer" style="display:none">
    <div class="widget">
        <div class="widget-content Popup">
        <div class="d-flex justify-content-between align-items-center mb-3 pb-2 border-bottom">
            <h3 class="m-0 fs-6 text-secondary"><i class="bi bi-people"></i> <fcp:PopupHeader runat="server" meta:resourcekey="lblPermissions" Text="File/Folder Permissions" /></h3>
            <button type="button" class="btn btn-sm btn-outline-secondary" onclick="$find(this).get_element().closest('.PopupContainer').style.display='none'; return false;">
                <i class="bi bi-x-lg" aria-hidden="true"></i>
                <span class="ms-1">Close</span>
            </button>
        </div>
			<div class="fcp-log-scroll-top">
                <asp:GridView id="gvFilePermissions" runat="server" AutoGenerateColumns="False"
                        CssSelectorClass="NormalGridView" ShowHeader="false">
                    <Columns>
			            <asp:TemplateField>
				            <ItemTemplate>
					            <b><asp:Literal id="litDisplayName" runat="server" Text='<%# Eval("DisplayName") %>'></asp:Literal></b>
					            <asp:Literal id="litAccountName" runat="server" Text='<%# Eval("AccountName") %>' visible="false"></asp:Literal>
				            </ItemTemplate>
			            </asp:TemplateField>
			            <asp:TemplateField>
				            <ItemTemplate>
					            <asp:CheckBox ID="chkRead" Runat="server" Checked='<%# Eval("Read") %>' Text="Read" meta:resourcekey="chkRead"></asp:CheckBox>
				            </ItemTemplate>
			            </asp:TemplateField>
			            <asp:TemplateField>
				            <ItemTemplate>
					            <asp:CheckBox ID="chkWrite" Runat="server" Checked='<%# Eval("Write") %>' Text="Write" meta:resourcekey="chkWrite"></asp:CheckBox>
				            </ItemTemplate>
			            </asp:TemplateField>
			         </Columns>
                </asp:GridView>
            </div>
			<div class="FormRow">
				<asp:CheckBox ID="chkReplaceChildPermissions" Runat="server" Text="Replace permissions on all child objects" meta:resourcekey="chkReplaceChildPermissions"></asp:CheckBox>
			</div>
			</div>
					<div class="popup-buttons text-end">
            <asp:LinkButton id="btnCancelPermissions" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancelPermissions_Click"> <i class="bi bi-x-lg">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnCancelPermissionsText"/> </asp:LinkButton>&nbsp;
            <asp:LinkButton id="btnSetPermissions" CssClass="btn btn-success" runat="server" OnClick="btnSetPermissions_Click"> <i class="bi bi-check-lg">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnSetPermissionsText"/> </asp:LinkButton>
		</div>
	</div>
</asp:Panel>

<asp:Panel ID="PermissionsUnixFilePanel" runat="server" CssClass="PopupContainer" style="display:none">
	<fcp:PopupHeader runat="server" meta:resourcekey="lblPermissions" Text="File/Folder Permissions" />
	<div class="Content">
		<div class="Body">
			<br />
			<div class="fcp-log-scroll-top">
				<table class="table table-borderless align-middle mb-0 w-100">
					<tr>
						<td><asp:Literal ID="lblOwner" runat="server" Text="Owner:" meta:resourcekey="lblOwner" /></td>
						<td>
							<asp:Label ID="lblOwnerText" runat="server" Text="websitepanel" CssClass="OwnerText" />
							<asp:TextBox ID="txtOwner" runat="server" Text="websitepanel" Style="display:none;" CssClass="OwnerTextBox" />
							<asp:ImageButton ID="btnRenameOwner" runat="server" SkinID="RenameButton" AlternateText="Change Owner"
								meta:resourcekey="btnRenameOwner" OnClientClick="ShowTextBox('Owner'); return false;" />
						</td>
					    <td>
							<asp:CheckBox ID="chkReadOwner" runat="server" Text="Read" meta:resourcekey="chkRead" />
							<asp:CheckBox ID="chkWriteOwner" runat="server" Text="Write" meta:resourcekey="chkWrite" />
							<asp:CheckBox ID="chkExecuteOwner" runat="server" Text="Execute" meta:resourcekey="chkExecute" />
						</td>
					</tr>
					<tr>
						<td><asp:Literal ID="lblGroup" runat="server" Text="Group:" meta:resourcekey="lblGroup" /></td>
						<td>
							<asp:Label ID="lblGroupText" runat="server" Text="websitepanel" CssClass="GroupText" />
							<asp:TextBox ID="txtGroup" runat="server" Text="websitepanel" Style="display:none;" CssClass="GroupTextBox" />
							<asp:ImageButton ID="btnRenameGroup" runat="server" SkinID="RenameButton" AlternateText="Change Group"
								meta:resourcekey="btnRenameGroup" OnClientClick="ShowTextBox('Group'); return false;" />					</td>
						<td>
							<asp:CheckBox ID="chkReadGroup" runat="server" Text="Read" meta:resourcekey="chkRead" />
							<asp:CheckBox ID="chkWriteGroup" runat="server" Text="Write" meta:resourcekey="chkWrite" />
							<asp:CheckBox ID="chkExecuteGroup" runat="server" Text="Execute" meta:resourcekey="chkExecute" />
					    </td>
					</tr>
					<tr>
						<td><asp:Literal ID="lblOthers" runat="server" Text="Others:" meta:resourcekey="lblOthers" /></td>
						<td></td>
					    <td>
							<asp:CheckBox ID="chkReadOthers" runat="server" Text="Read" meta:resourcekey="chkRead" />
							<asp:CheckBox ID="chkWriteOthers" runat="server" Text="Write" meta:resourcekey="chkWrite" />
							<asp:CheckBox ID="chkExecuteOthers" runat="server" Text="Execute" meta:resourcekey="chkExecute" />
					    </td>
					</tr>
				</table>
            </div>
			<div class="FormRow">
				<asp:CheckBox ID="chkReplaceChildPermissionsUnix" Runat="server" Text="Replace permissions on all child objects" meta:resourcekey="chkReplaceChildPermissions"></asp:CheckBox>
			</div>
			<br />
		</div>
		<div class="FormFooter">
            <asp:Button ID="btnSetPermissionsUnix" runat="server" CssClass="btn btn-primary" meta:resourcekey="btnSetPermissions" Text="Set Permissions" OnClick="btnSetPermissionsUnix_Click" />
            <asp:Button ID="btnCancelPermissionsUnix" runat="server" CssClass="btn btn-primary" meta:resourcekey="btnCancelPermissions" Text="Cancel" CausesValidation="false" OnClick="btnCancelPermissionsUnix_Click" />
		</div>
	</div>
</asp:Panel>

<asp:LinkButton ID="btnSetPermissionsFile" runat="server" style="display:none" />
<ajaxToolkit:ModalPopupExtender ID="PermissionsFileModal" runat="server"
    TargetControlID="btnSetPermissionsFile" PopupControlID="PermissionsWindowsFilePanel"
    BackgroundCssClass="modalBackground" DropShadow="false" />

	</ContentTemplate>
</asp:UpdatePanel>

<div class="GridFooter">
	<div class="Left">
		<asp:Label ID="lblDiskSpace" runat="server" meta:resourcekey="lblDiskSpace" Text="Disk Space, MB:"></asp:Label>
		<uc4:Quota ID="Quota1" runat="server" QuotaName="OS.Diskspace" />
	</div>
	<div class="Right">
		<asp:LinkButton ID="btnRecalcDiskspace" runat="server" meta:resourcekey="btnRecalcDiskspace"
						CssClass="btn btn-primary"
						Text="Calculate Diskspace" OnClientClick="return confirm('Proceed?');" OnClick="btnRecalcDiskspace_Click" />
	</div>
</div>

