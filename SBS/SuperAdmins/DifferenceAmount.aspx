<%@ Page Language="VB" MasterPageFile="~/SBS/SuperAdmins/SuperAdmin.master" AutoEventWireup="false" CodeFile="DifferenceAmount.aspx.vb"
    Inherits="SBSAgents.DifferenceAmount" %>

<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="~/SBS/Inc/ticketBetsGridAgent.ascx" TagName="ticketBetsGridAgent"
    TagPrefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
    <div style="width:100%;text-align:center">
   <input type="button" class="button" style="margin:0 auto" value="Back To Agent Position Report" onclick="window.location='/SBS/SuperAdmins/SuperPositionReport.aspx'" />
    </div>
    <div style="width: 100%; padding-bottom: 10px; padding-top: 20px">
        <asp:UpdatePanel ID="up1" runat="server">
            <Triggers>
            </Triggers>
            <ContentTemplate>
                <asp:Timer ID="tmrRefresh" runat="server" Interval="1500" Enabled="false" />
                <uc1:ticketBetsGridAgent ID="ucTicketBetsGridAgent" runat="server" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
