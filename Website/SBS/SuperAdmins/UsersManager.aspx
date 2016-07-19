<%@ Page Language="VB" MasterPageFile="~/SBS/SuperAdmins/SuperAdmin.master" AutoEventWireup="false"
    CodeFile="UsersManager.aspx.vb" Inherits="SBSSuperAdmin.UsersManager" %>

<%@ Register Src="~/Inc/SuperAdmins/AgentManager.ascx" TagName="AgentManager" TagPrefix="uc" %>
<%@ Register Src="~/Inc/SuperAdmins/CallCenterAgentManager.ascx" TagName="CallCenterAgentManager"
    TagPrefix="uc" %>
<%@ Register Src="~/Inc/SuperAdmins/SuperManager.ascx" TagName="SuperManager" TagPrefix="uc2" %>
<%@ Register Src="~/Inc/SuperAdmins/PlayerManager.ascx" TagName="PlayerManager" TagPrefix="uc3" %>
<%@ Register Src="../../Inc/SuperAdmins/PartnerManager.ascx" TagName="PartnerManager"
    TagPrefix="uc1" %>
<asp:Content ID="Content4" ContentPlaceHolderID="cphContent" runat="Server">
    <div class="row mbl">
        <div class="col-lg-12">
            <ul id="generalTab" class="nav nav-tabs">
                <li id="tSuper" runat="server">
                    <asp:LinkButton runat="server" ID="lbtTabSuper" CommandArgument="SUPER" Text="Super Admins"
                        CausesValidation="false" OnClick="lbtTab_Click" />
                </li>
                <li id="tPartner" runat="server">
                    <asp:LinkButton runat="server" ID="lbnTabPartner" CommandArgument="PARTNER" Text="Partners"
                        CausesValidation="false" OnClick="lbtTab_Click" />
                </li>
                <li id="tAgent" runat="server">
                    <asp:LinkButton runat="server" ID="lbtTabAgent" CommandArgument="AGENT" CssClass="selected"
                        Text="Agents" CausesValidation="false" OnClick="lbtTab_Click" />
                </li>
                <li id="tCCAgent" runat="server">
                    <asp:LinkButton runat="server" ID="lbtTabCCAgent" CommandArgument="CCAGENT" CssClass="selected"
                        Text="Call Center Agents" CausesValidation="false" OnClick="lbtTab_Click" />
                </li>
                <li id="tPlayer" runat="server">
                    <asp:LinkButton runat="server" ID="lbtTabPlayer" CommandArgument="PLAYER" Text="Players"
                        CausesValidation="false" OnClick="lbtTab_Click" />
                </li>
            </ul>
            <div class="tab-content responsive" style="border: none;">
                <div class="tab-pane fade active in">
                    <uc:AgentManager ID="ucAgentManager" runat="server" SiteType="SBS" />
                    <uc1:PartnerManager ID="ucPartnerManager" runat="server" Visible="false" SiteType="SBS" />
                    <uc:CallCenterAgentManager ID="ucCCAgentManager" runat="server" SiteType="SBS" />
                    <uc2:SuperManager ID="ucSuperManager" runat="server" Visible="false" SiteType="SBS" />
                    <uc3:PlayerManager ID="ucPlayerManager" runat="server" Visible="false" SiteType="SBS" />
            </div>
            </div>
        </div>
    </div>
</asp:Content>
