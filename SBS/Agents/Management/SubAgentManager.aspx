<%@ Page Language="VB" MasterPageFile="~/SBS/Agents/Agents.master" AutoEventWireup="false" CodeFile="SubAgentManager.aspx.vb" Inherits="SBSAgents.Management.SubAgentManager" %>
<%@ Register Src="~/Inc/Agents/SubplayerManager.ascx" TagName="SubplayerManager" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" Runat="Server">
<uc:SubplayerManager ID="ucSubplayerManager"  runat="server" PendingPage="../OpenBets.aspx" />
</asp:Content>

