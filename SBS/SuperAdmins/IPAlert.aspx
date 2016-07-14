<%@ Page Language="VB" MasterPageFile="~/SBS/SuperAdmins/SuperAdmin.master"
    AutoEventWireup="false" CodeFile="IPAlert.aspx.vb" Inherits="SBSSuperAdmin.IPAlert" %>

<%@ Register Src="~/Inc/Reports/IPAlert.ascx" TagName="IPAlert" TagPrefix="uc" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">

    <div id="ipReportPanel" class="row mbl">
        <div class="col-lg-12">
            <uc:IPAlert ID="ucIPAlert" runat="server" />
        </div>
    </div>
</asp:Content>
