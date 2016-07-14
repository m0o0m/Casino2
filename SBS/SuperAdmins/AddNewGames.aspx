<%@ Page Language="VB" MasterPageFile="~/SBS/SuperAdmins/SuperAdmin.master" AutoEventWireup="false" CodeFile="AddNewGames.aspx.vb" Inherits="SBSSuperAdmin.AddNewGames" title="Untitled Page" %>
<%@ Register Src="~/Inc/SuperAdmins/AddNewGame.ascx"  TagName="AddNewGame" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphContent" Runat="Server">
    <uc:AddNewGame ID="ucAddNewGame" runat="server" /> 
</asp:Content>

