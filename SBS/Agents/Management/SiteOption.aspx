<%@ Page Language="VB" MasterPageFile="~/SBS/Agents/Agents.master" AutoEventWireup="false" CodeFile="SiteOption.aspx.vb" Inherits="SBSAgents.SiteOption" %>
<%@ Register Src="~/Inc/Agents/SiteOption.ascx" TagName="SiteOption" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" Runat="Server">
    <uc:SiteOption ID="ucSiteOption" runat="server" />
</asp:Content>

