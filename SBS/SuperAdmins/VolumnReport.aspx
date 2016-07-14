<%@ Page Title="Volumn Report" Language="VB" MasterPageFile="~/SBS/SuperAdmins/SuperAdmin.master" AutoEventWireup="false" CodeFile="VolumnReport.aspx.vb" Inherits="SBS_SuperAdmins_VolumnReport" %>

<%@ Register src="../Inc/VolumnReport.ascx" tagname="VolumnReport" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphContent" Runat="Server">
    
    <uc1:VolumnReport ID="VolumnReport1" runat="server" />
    
</asp:Content>

