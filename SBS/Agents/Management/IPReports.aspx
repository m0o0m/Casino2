<%@ Page Language="VB" MasterPageFile="../Agents.master" AutoEventWireup="false"
    CodeFile="IPReports.aspx.vb" Inherits="SBSAgents.IPReports" %>

<%@ Register Src="~/Inc/Reports/IPReports.ascx" TagName="IPReports" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="Server">
   <uc:IPReports ID="ucIPReports" runat="server" />
</asp:Content>
