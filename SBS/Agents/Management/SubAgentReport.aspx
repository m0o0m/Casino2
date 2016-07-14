<%@ Page Language="VB" MasterPageFile="../Agents.master" 
AutoEventWireup="false" CodeFile="SubAgentReport.aspx.vb" Inherits="SBSAgents.SubAgentReport" %>
<%@ Register Src="~/Inc/Reports/AgentBalanceReport.ascx" TagName="AgentBalanceReport" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="Server">
    <uc:AgentBalanceReport ID="ucAgentBalanceReport" runat="server" 
        HistoryPage="../History.aspx" PendingPage="../OpenBets.aspx" ShowAgentList="true" 
        ReportPage="PlayersReports.aspx" ShowWeekList="true" />
</asp:Content>


