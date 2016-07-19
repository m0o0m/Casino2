<%@ Page Language="VB" MasterPageFile="~/SBS/SuperAdmins/SuperAdmin.master" AutoEventWireup="false" CodeFile="PLReports.aspx.vb" Inherits="SBSSuperAdmin.PLReports" Title="Untitled Page" %>

<%@ Register Src="~/Inc/SuperAdmins/PLReport.ascx" TagName="PLReport" TagPrefix="uc" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">

    <div class="row mbl">
        <div class="col-lg-12">
            <ul id="generalTab" class="nav nav-tabs responsive hidden-xs hidden-sm">
                <li id="tDaily" runat="server">
                    <asp:LinkButton runat="server" ID="lbtTabDaily" CommandArgument="DAILY" Text="Daily"
                        ToolTip="Daily P&L Report" CausesValidation="false" OnClick="lbtTab_Click" />
                </li>
                <li id="tWeekly" runat="server">
                    <asp:LinkButton runat="server" ID="lbtTabWeekly" CommandArgument="WEEKLY" Text="Weekly"
                        ToolTip="Weekly P&L report" CausesValidation="false" OnClick="lbtTab_Click" />
                </li>
                <li id="tYTD" runat="server">
                    <asp:LinkButton runat="server" ID="lbtYTD" CommandArgument="YTD" Text="Yearly Total Detail"
                        ToolTip="Year To Date" CausesValidation="false" OnClick="lbtTab_Click" />
                </li>
            </ul>
        </div>
    </div>

    <div class="row">
        <div class="col-lg-12">
            <uc:PLReport ID="ucPLReport" runat="server" />
        </div>
        <div class="clearfix"></div>
    </div>
    <div class="mbxl"></div>
</asp:Content>

