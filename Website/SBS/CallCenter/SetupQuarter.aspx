<%@ Page Language="VB" MasterPageFile="~/SBS/CallCenter/CCAgents.master" AutoEventWireup="false"
    CodeFile="SetupQuarter.aspx.vb" Inherits="SBSCallCenterAgents.SetupQuarter" Title="Untitled Page" %>

<%@ Register Src="~/Inc/Agents/GameManual.ascx" TagName="GameManual"
    TagPrefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" runat="Server">
    <uc1:GameManual ID="GameManual1" runat="server" />
</asp:Content>
