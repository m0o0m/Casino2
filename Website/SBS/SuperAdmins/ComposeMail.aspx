<%@ Page Language="VB" MasterPageFile="~/SBS/SuperAdmins/SuperAdmin.master"
    AutoEventWireup="false" CodeFile="ComposeMail.aspx.vb" Inherits="SBSSuperAdmin.ComposeMail" %>

<%@ Register Src="~/Inc/Players/ComposeEmail.ascx" TagName="ComposeEmail" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphContent" runat="Server">
    <uc1:ComposeEmail ID="ucComposeEmail" runat="server" />
</asp:Content>
