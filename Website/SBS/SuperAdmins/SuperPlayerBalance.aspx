<%@ Page Language="VB" MasterPageFile="~/SBS/SuperAdmins/SuperAdmin.master" AutoEventWireup="false"
    CodeFile="SuperPlayerBalance.aspx.vb" Inherits="SBSSuperAdmin.SuperPlayerBalance"
    Title="Untitled Page" %>

<%@ Register Src="~/Inc/Reports/PlayerBalanceReport.ascx" TagName="PlayerBalanceReport"
    TagPrefix="uc" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc:PlayerBalanceReport ID="ucPlayerBalanceReport" runat="server" ShowAgentList="true"
            LabelSelectedUser="Agents:" ShowWeekList="true" HistoryPage="/SBS/SuperAdmins/HistoryTickets.aspx" PendingPage="/SBS/SuperAdmins/PendingTickets.aspx" />
            <uc:PlayerBalanceReport ID="ucSubPlayerBalanceReport" runat="server" ShowAgentList="true"
            LabelSelectedUser="SubAgents:" ShowWeekList="false" HistoryPage="/SBS/SuperAdmins/HistoryTickets.aspx" PendingPage="/SBS/SuperAdmins/PendingTickets.aspx" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
