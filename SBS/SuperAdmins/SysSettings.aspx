<%@ Page Language="VB" MasterPageFile="~/SBS/SuperAdmins/SuperAdmin.master"
    AutoEventWireup="false" CodeFile="SysSettings.aspx.vb" Inherits="SBSSuperAdmin.SysSettings" %>

<%@ Register Src="~/Inc/Players/MailAlert.ascx" TagName="MailAlert" TagPrefix="uc" %>
<%@ Register Src="~/Inc/SuperAdmins/SysSettingManager.ascx" TagName="SettingManager" TagPrefix="uc" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
    <uc:MailAlert ID="ucMailAlert" runat="server" />
    <uc:SettingManager ID="ucSettingManager" runat="server" />
</asp:Content>
