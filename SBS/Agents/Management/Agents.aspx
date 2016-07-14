<%@ Page Language="VB" MasterPageFile="../Agents.master" AutoEventWireup="false" CodeFile="Agents.aspx.vb" Inherits="SBSAgents.Agents" %>

<%--<%@ Register Src="~/Inc/Agents/AgentManager.ascx" TagName="AgentManager" TagPrefix="uc" %>--%>
<%@ Register Src="~/Inc/Agents/ManagerAgent.ascx" TagName="AgentManager" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="Server">
    <%--  <uc:AgentManager ID="ucAgentManager" runat="server" SiteType="SBS" />--%>
    <uc:AgentManager ID="ucAgentManager" runat="server" />
</asp:Content>

