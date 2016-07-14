<%@ Page Language="VB" MasterPageFile="~/SBS/Agents/Agents.master" AutoEventWireup="false" CodeFile="VolumnReport.aspx.vb" Inherits="SBS_Agents_VolumnReport" %>

<%@ Register src="../Inc/VolumnReport.ascx" tagname="VolumnReport" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" Runat="Server">
    <uc1:VolumnReport ID="VolumnReport1" runat="server" />
</asp:Content>

