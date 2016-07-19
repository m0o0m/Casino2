<%@ Page Language="VB" MasterPageFile="~/SBS/SuperAdmins/SuperAdmin.master"
    AutoEventWireup="false" CodeFile="ManualProposition.aspx.vb" Inherits="SBSSuperAdmin.ManualPropPosition" %>

<%@ Register Src="~/Inc/SuperAdmins/ManualProposition.ascx" TagName="ManualProposition"
    TagPrefix="uc" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
    <div class="row">
        <div class="col-lg-12">
            <uc:ManualProposition ID="ucManualProposition" runat="server" SiteType="SBS" />
        </div>
    </div>
</asp:Content>
