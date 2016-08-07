<%@ Page Language="VB" MasterPageFile="Player.master" AutoEventWireup="false" CodeFile="HistoryDetail.aspx.vb"
    Inherits="SBSPlayer.HistoryDetail" Title="Untitled Page" %>

<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="../Inc/historyGridDetail.ascx" TagName="historyGridDetail" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            
            <div class="verticalline mgB10">
                <asp:Label ID="lblActivityDate" CssClass="bold" runat="server" Text=""></asp:Label>
                <input type="button" class="button-style-2 w110px mgL25" onclick="javascript:history.back();" value="Back"/>
            </div>

            <div class="panel panel-grey">
                <%--div class="panel-heading">Results</div>--%>
                <div class="panel-body">
                    <uc1:historyGridDetail ID="ucHistoryGridDetail" runat="server" />
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
