<%@ Page Language="VB" MasterPageFile="Agents.master" AutoEventWireup="false" CodeFile="TicketManagement.aspx.vb" Inherits="SBS_Agents_TicketManagement" %>

<%@ Register Src="../Inc/TicketManagements.ascx" TagName="TicketManagements" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="Server">

    <uc1:TicketManagements ID="TicketManagements1" runat="server" />

</asp:Content>
