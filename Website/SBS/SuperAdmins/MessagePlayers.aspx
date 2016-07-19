<%@ Page Language="VB" MasterPageFile="~/SBS/SuperAdmins/SuperAdmin.master" AutoEventWireup="false"
    CodeFile="MessagePlayers.aspx.vb" Inherits="SBSSuperAdmin.MessagePlayers" Title="Untitled Page" %>

<%@ Register Src="~/Inc/SuperAdmins/MessagePlayers.ascx" TagName="MessagePlayers" TagPrefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
    <uc1:MessagePlayers ID="ucMessagePlayers" runat="server" />
</asp:Content>
