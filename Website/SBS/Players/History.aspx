<%@ Page Language="VB" MasterPageFile="Player.master" AutoEventWireup="false" CodeFile="History.aspx.vb"
    Inherits="SBSPlayer.History" Title="Untitled Page" %>

<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="../Inc/historyGrid.ascx" TagName="historyGrid" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="pdB25" style="padding-left: 150px">
                <div class="">
                    <table class="full-w">
                        <tr>
                            <td align="" class="v-top w40">
                                <asp:CheckBox CssClass="mgR20" ID="cbWin" Text=" Win" Checked="True" AutoPostBack="False" runat="server" />
                                <asp:CheckBox CssClass="mgR20" ID="cbLose" Text=" Lose" Checked="True" AutoPostBack="False" runat="server" />
                                <asp:CheckBox CssClass="mgR20" ID="cbCanceled" Text=" Canceled" Checked="True" AutoPostBack="False" runat="server" />
                            </td>
                            <td align="" class="v-top w35">
                                <cc1:CDropDownList ID="ddlWeekly" runat="server" CssClass="select-field-3" AutoPostBack="true">
                                </cc1:CDropDownList>
                            </td>
                            <td align="" class="v-top">
                                <asp:Button ID="btnRefreshHistory" CssClass="btn-refresh-history" runat="server" Text="" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <div class="panel panel-grey">
                <%--div class="panel-heading">Results</div>--%><div class="panel-body">
                    <uc1:historyGrid ID="ucHistoryGrid" runat="server" />
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
