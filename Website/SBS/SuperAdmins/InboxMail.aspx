<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/SBS/SuperAdmins/SuperAdmin.master" CodeFile="InboxMail.aspx.vb" Inherits="SBSSuperAdmin.InboxMail" %>
<%@ Register Src="~/Inc/SuperAdmins/InboxMail.ascx" TagName="InboxMail" TagPrefix="uc" %>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="Server">
  <uc:InboxMail ID="ucInboxMail" runat="server" />
</asp:Content> 
