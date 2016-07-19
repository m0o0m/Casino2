<%@ Page Language="VB" MasterPageFile="~/SBS/SuperAdmins/SuperAdmin.master"
    AutoEventWireup="false" CodeFile="BookMakerSetup.aspx.vb" Inherits="SBSSuperAdmin.BookMakerSetup" %>

<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>
<%@ Register Src="~/Inc/SportAllowance.ascx" TagName="SportAllowance" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphContent" runat="Server">
    <div class="row">
        <div class="col-lg-12">
            <h4>Bookmaker Settings</h4>
            <asp:LinkButton ID="lbtChangePage" Style="margin-left: 10px" PostBackUrl="UsersManager.aspx"
                Visible="false" runat="server">Please Set Up At Least One Agent Before You Can Use This Feature</asp:LinkButton>
        </div>
        <div class="clearfix"></div>
    </div>
    <div class="mbxl"></div>
    <div class="row" runat="server" id="trAgent">
        <div class="col-lg-12">
            <uc1:SportAllowance ID="SportAllowance1" runat="server" />
        </div>
        <div class="clearfix"></div>
    </div>
    <div class="mbxl"></div>
</asp:Content>
