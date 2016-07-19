<%@ Page Language="VB" MasterPageFile="~/SBS/Agents/Agents.master" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="SBSAgent.SBS_Agents_Default" %>
<%@ Register Src="~/Inc/Agents/AgentMenu.ascx" TagName="TopMenu" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" Runat="Server">
 <uc1:TopMenu ID="ucMainMenu" runat="server" />
</asp:Content>

