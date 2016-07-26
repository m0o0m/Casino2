<%@ Page Language="VB" MasterPageFile="Player.master" AutoEventWireup="false" CodeFile="History.aspx.vb"
    Inherits="SBSPlayer.History" Title="Untitled Page" %>

<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="../Inc/historyGrid.ascx" TagName="historyGrid" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="table-filter-style-1 pdTB10 pdLR10">
                <div class="ly-fixed">
                    <table class="full-w">
                        <tr>
                            <td align="right">
                                <asp:CheckBox ID="cbWin" Text="Win" Checked="True" AutoPostBack="False" runat="server" />
                                <asp:CheckBox ID="cbLose" Text="Lose" Checked="True" AutoPostBack="False" runat="server" />
                                <asp:CheckBox ID="cbCanceled" Text="Canceled" Checked="True" AutoPostBack="False" runat="server" />
                            </td>
                            <td align="center">
                                <cc1:CDropDownList ID="ddlWeekly" runat="server" CssClass="select-field-3" AutoPostBack="true">
                                </cc1:CDropDownList>
                            </td>
                            <td align="center">
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
