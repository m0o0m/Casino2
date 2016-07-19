<%@ Page Language="VB" MasterPageFile="~/SBS/SuperAdmins/SuperAdmin.master" AutoEventWireup="false"
    MaintainScrollPositionOnPostback="true" CodeFile="GameType.aspx.vb" Inherits="SBSSuperAdmin.GameType" %>

<%@ Register Src="~/Inc/SuperAdmins/GameTypeManager.ascx" TagName="GameType" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphContent" runat="Server">
    <uc:GameType ID="ucGameType" runat="server"></uc:GameType>
</asp:Content>
