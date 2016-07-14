<%@ Page Language="VB" MasterPageFile="~/SBS/SuperAdmins/SuperAdmin.master" AutoEventWireup="false"
    CodeFile="ContentManager.aspx.vb" Inherits="SBSSuperAdmin.ContentManager" ValidateRequest="false" %>

<%@ Register Src="~/Inc/SuperAdmins/HTMLContent.ascx" TagName="HTMLContent" TagPrefix="uc1" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="cc1" %>
<%@ Register Assembly="FredCK.FCKeditorV2" Namespace="FredCK.FCKeditorV2" TagPrefix="fck" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
    <div class="row">
        <div class="col-lg-12">
            <b>Content Type:</b>
            <cc1:CDropDownList ID="ddlTypes" runat="server" CssClass="textInput" AutoPostBack="true">
                <asp:ListItem Text="Rules" Value="RULES" />
                <asp:ListItem Text="Odds" Value="ODDS" />
                <asp:ListItem Text="Letter Template Call Center Agent" Value="CCAGENT_LETTER_TEAMPLATE" />
            </cc1:CDropDownList>
        </div>
        <div class="clearfix"></div>
    </div>
    <div class="mbxl"></div>

    <div class="row">
        <div class="col-lg-12">
            <uc1:HTMLContent ID="ucHTMLContent" runat="server" />
        </div>
        <div class="clearfix"></div>
    </div>
    <div class="mbxl"></div>

</asp:Content>
