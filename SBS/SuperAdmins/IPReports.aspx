<%@ Page Language="VB" MasterPageFile="~/SBS/SuperAdmins/SuperAdmin.master"
    AutoEventWireup="false" CodeFile="IPReports.aspx.vb" Inherits="SBSSuperAdmin.IPReports" %>

<%@ Register Src="~/Inc/Reports/IPReports.ascx" TagName="IPReport" TagPrefix="uc" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
    <div id="ipReportPanel" class="row mbl">
        <div class="col-lg-12">
            <uc:IPReport ID="ucIPReports" runat="server" />
        </div>
    </div>
</asp:Content>
