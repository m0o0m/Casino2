<%@ Page Language="VB" MasterPageFile="~/SBS/SuperAdmins/SuperAdmin.master" AutoEventWireup="false" CodeFile="Weeklyfigure.aspx.vb" Inherits="SBSSuperAdmin.Weeklyfigure" %>

<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>
<%@ Register Src="~/Inc/Reports/SummaryReport.ascx" TagName="SummaryReport" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphContent" runat="Server">

    <div class="row">
        <div class="col-lg-12">
            <span>Agents :</span>
            <wlb:CDropDownList ID="ddlAgents" runat="server" CssClass="form-control" Style="display: inline-block;" hasOptionalItem="false"
                AutoPostBack="true" OnSelectedIndexChanged="SelectedIndexChanged" Width="220px" />
        </div>
        <div class="clearfix"></div>
    </div>
    <div class="mbxl"></div>

    <uc1:SummaryReport ID="ucSummaryReport" runat="server" HistoryPage="/SBS/SuperAdmins/HistoryTickets.aspx"
        PendingPage="/SBS/SuperAdmins/PendingTickets.aspx" />
</asp:Content>

