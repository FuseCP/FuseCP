<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ServerServicesControl.ascx.cs" Inherits="FuseCP.Portal.ServerServicesControl" %>
<asp:Panel ID="pnlLoadServices" runat="server" CssClass="alert alert-secondary d-flex flex-wrap justify-content-between align-items-center gap-2 mb-3">
	<asp:Label ID="lblLoadServicesHint" runat="server" Text="Installed services are loaded on demand to keep large server pages responsive."></asp:Label>
	<asp:LinkButton ID="btnLoadServices" runat="server" CssClass="btn btn-outline-primary" OnClick="btnLoadServices_Click" CausesValidation="False">
		<i class="bi bi-arrow-repeat">&nbsp;</i>&nbsp;Load Existing Services
	</asp:LinkButton>
</asp:Panel>
<asp:Repeater id="dlServiceGroups" Runat="server" >
	<ItemTemplate>
        <div class="card border-info">
            <div class="card-header">
                <h3 class="card-title"><%# GetSharedLocalizedString("ResourceGroup." + (string)Eval("GroupName")) %>
                    <asp:hyperlink id="lnkAddService" runat="server"
				        NavigateUrl='<%# EditServiceUrl("GroupID", Eval("GroupID").ToString(), "add_service") %>'
				        CssClass="btn btn-secondary btn-sm float-end">
                        <i class="bi bi-plus-lg">&nbsp;</i>&nbsp;<asp:Localize runat="server"  meta:resourcekey="lnkAddService"/>
					</asp:hyperlink>
                </h3>
            </div>

            <ul class="list-group">
                <asp:Repeater ID="dlServices" Runat="server" DataSource='<%# GetGroupServices((int)Eval("GroupID")) %>'>
						<ItemTemplate>
							<li class="list-group-item">
								<asp:hyperlink id="lnkEditService" runat="server" NavigateUrl='<%# EditServiceUrl("ServiceID", Eval("ServiceID").ToString(), "edit_service") %>' Width=100% Height=100% ToolTip='<%# Eval("Comments") %>'>
									<%# Eval("ServiceName") %>
								</asp:hyperlink>
							 </li>
						</ItemTemplate>
					</asp:Repeater>
            </ul>
        </div>
	</ItemTemplate>
</asp:Repeater>
