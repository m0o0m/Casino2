<%@ Page Language="VB" MasterPageFile="~/SBS/SuperAdmins/SuperAdmin.master"
    AutoEventWireup="false" CodeFile="SuperPositionReport.aspx.vb" Inherits="SBSSuperAdmin.SuperPositionReport" %>

<%@ Register Src="~/Inc/Reports/NewAgentPositionReport.ascx" TagName="NewAgentPositionReport"
    TagPrefix="uc" %>
<%@ Register Src="~/Inc/Reports/AgentPositionReport.ascx" TagName="AgentPositionReport"
    TagPrefix="uc" %>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="Server">
    <style type="text/css">
        #LineType { color: #f7a30b; }
    </style>
    <div class="row">
        <div class="col-lg-12">
            <ul id="generalTab" class="nav nav-tabs responsive hidden-xs hidden-sm">
                <li id="tAll" runat="server">
                    <asp:LinkButton runat="server" ID="lbtAllPlayer" CommandArgument="ALL"
                        Text="All" CausesValidation="false" OnClick="lbtTab_Click" />
                </li>
                <li id="tDetail" runat="server">
                    <asp:LinkButton runat="server" ID="lbtPlayers" CommandArgument="DETAILS" Text="Details"
                        CausesValidation="false" OnClick="lbtTab_Click" />
                </li>
            </ul>

            <uc:AgentPositionReport ID="ucAgentPositionReport" Visible="false" runat="server" Title="AgentPositionReport" />
            <uc:NewAgentPositionReport ID="ucNewAgentPositionReport" runat="server" Title="AgentPositionReport" />
        </div>
        <div class="clearfix"></div>
    </div>
    <div class="mbxl"></div>

</asp:Content>
