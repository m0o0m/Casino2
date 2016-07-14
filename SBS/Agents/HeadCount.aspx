<%@ Page Language="VB" MasterPageFile="~/SBS/Agents/Agents.master" AutoEventWireup="false" CodeFile="HeadCount.aspx.vb" Inherits="SBSAgents.HeadCount" title="Untitled Page" %>
<%@ Register Src="~/Inc/Reports/HeadCount.ascx" TagName="HeadCount" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" Runat="Server">
<uc:HeadCount ID="ucHeadCount" runat="server" />
</asp:Content>

