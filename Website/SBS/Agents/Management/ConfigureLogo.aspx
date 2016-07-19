<%@ Page Language="VB" MasterPageFile="~/SBS/Agents/Agents.master" AutoEventWireup="false" CodeFile="ConfigureLogo.aspx.vb" Inherits="SBSAgents.ConfigureLogo" %>
<%@ Register Src="~/Inc/Agents/ConfigureLogo.ascx" TagName="ConfigureLogo" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" Runat="Server">
<uc:ConfigureLogo ID="ucConfigureLogo" runat="server" />
</asp:Content>

