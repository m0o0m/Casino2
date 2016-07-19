<%@ Page Language="VB" MasterPageFile="~/SBS/SuperAdmins/SuperAdmin.master" AutoEventWireup="false"
    CodeFile="TeaserOdds.aspx.vb" Inherits="SBSSuperAdmin.TeaserOdds" Title="Untitled Page" %>

<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/Inc/SuperAdmins/TeaserOddsManager.ascx" TagName="TeaserOddsManager"
    TagPrefix="uc" %>
<%@ Register Src="~/Inc/SuperAdmins/TeaserRulesManager.ascx" TagName="TeaserRulesManager"
    TagPrefix="uc" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
    <div class="row">
        <div class="col-lg-12">
            <h3>Teaser Rules:</h3>
            <uc:TeaserRulesManager ID="ucTeaserRulesManager" runat="server" />
        </div>
    </div>

    <div class="row">
        <div class="col-lg-12">
            <h3>Teaser Payout:</h3>
            <uc:TeaserOddsManager ID="ucTeaserOddsManager" runat="server" />
        </div>
        <div class="clearfix"></div>
    </div>
    <div class="mbxl"></div>
</asp:Content>
