<%@ Page Language="VB" MasterPageFile="~/SBS/Agents/Agents.master" AutoEventWireup="false" CodeFile="test.aspx.vb" Inherits="SBS_Agents_test" %>



<%@ Register src="../../Inc/Reports/SummaryReport.ascx" tagname="SummaryReport" tagprefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" Runat="Server">
    <uc2:SummaryReport ID="ucSummaryReport" runat="server" />
 
</asp:Content>

