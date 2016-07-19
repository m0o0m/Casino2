<%@ Page Language="VB" MasterPageFile="~/SBS/SuperAdmins/SuperAdmin.master" AutoEventWireup="false"
    CodeFile="SuperAgentBalance.aspx.vb" Inherits="SBSSuperAdmin.SuperAgentBalance"
    Title="Untitled Page" %>

<%@ Register Src="~/Inc/Reports/AgentBalanceReport.ascx" TagName="AgentBalanceReport"
    TagPrefix="uc" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc:AgentBalanceReport ID="ucAgentBalanceReport" runat="server" ShowAgentList="true"
                ShowWeekList="true" 
                HistoryPage="/SBS/SuperAdmins/HistoryTickets.aspx" 
                PendingPage="/SBS/SuperAdmins/PendingTickets.aspx" 
                ReportPage="/SBS/SuperAdmins/weeklyfigure.aspx" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
