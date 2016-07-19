<%@ Page Language="VB" MasterPageFile="~/SBS/SuperAdmins/SuperAdmin.master" AutoEventWireup="false"
    CodeFile="ManageTickets.aspx.vb" Inherits="SBSSuperAdmin.ManageTickets" Title="Untitled Page" %>

<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>
<%@ Register Src="~/Inc/DateTime.ascx" TagName="DateTime" TagPrefix="uc" %>
<%@ Register Src="../Inc/TicketManagements.ascx" TagName="TicketManagements" TagPrefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">

    <uc1:TicketManagements ID="TicketManagements1" runat="server" />

</asp:Content>
