<%@ Control Language="VB" AutoEventWireup="false" CodeFile="editPlayerTemplateLimits.ascx.vb"
    Inherits="SBCSuperAdmin.editPlayerTemplateLimits" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="cc1" %>
<asp:Repeater ID="rptWagerLimits" runat="server">
    <HeaderTemplate>
        <table cellpadding="2" cellspacing="2" class="gamebox" width="100%">
            <tr>
                <td>
                </td>
                <td>
                </td>
                <td colspan="3" align="center" class="phoneLimit">
                    BY PHONE LIMIT
                </td>
                <td colspan="3" align="center" class="internetLimit">
                    BY INTERNET LIMIT
                </td>
                <td style="width: 40px;" />
            </tr>
            <tr class="tableheading">
                <td align="center">
                    Wager Limit
                </td>
                <td align="center">
                    Period
                </td>
                <td align="center">
                    Point Spread
                </td>
                <td align="center">
                    Total Points
                </td>
                <td align="center">
                    Money Line
                </td>
                <td align="center">
                    Point Spread
                </td>
                <td align="center">
                    Total Points
                </td>
                <td align="center">
                    Money Line
                </td>
                <td style="width: 40px;" />
            </tr>
    </HeaderTemplate>
    <AlternatingItemTemplate>
        <tr>
            <td align="center">
                <cc1:CDropDownList ID="ddlGameTypes" runat="server" CssClass="textInput" />
            </td>
            <td align="center">
                <asp:TextBox ID="txtContext" runat="server" CssClass="textInput" MaxLength="100" />
            </td>
            <td align="center">
                <asp:TextBox ID="txtPointSpreadP" runat="server" CssClass="textInput" Style="text-align: right;
                    padding-left: 2px;" Width="60" onkeypress="javascript:return inputNumber(this,event, false);"
                    onblur="javascript:formatNumber(this,2);" />
            </td>
            <td align="center">
                <asp:TextBox ID="txtTotalPointsP" runat="server" CssClass="textInput" Style="text-align: right;
                    padding-left: 2px;" Width="60" onkeypress="javascript:return inputNumber(this,event, false);"
                    onblur="javascript:formatNumber(this,2);" />
            </td>
            <td align="center">
                <asp:TextBox ID="txtMoneyLineP" runat="server" CssClass="textInput" Style="text-align: right;
                    padding-left: 2px;" Width="60" onkeypress="javascript:return inputNumber(this,event, false);"
                    onblur="javascript:formatNumber(this,2);" />
            </td>
            <td align="center">
                <asp:TextBox ID="txtPointSpreadI" runat="server" CssClass="textInput" Style="text-align: right;
                    padding-left: 2px;" Width="60" onkeypress="javascript:return inputNumber(this,event, false);"
                    onblur="javascript:formatNumber(this,2);" />
            </td>
            <td align="center">
                <asp:TextBox ID="txtTotalPointsI" runat="server" CssClass="textInput" Style="text-align: right;
                    padding-left: 2px;" Width="60" onkeypress="javascript:return inputNumber(this,event, false);"
                    onblur="javascript:formatNumber(this,2);" />
            </td>
            <td align="center">
                <asp:TextBox ID="txtMoneyLineI" runat="server" CssClass="textInput" Style="text-align: right;
                    padding-left: 2px;" Width="60" onkeypress="javascript:return inputNumber(this,event, false);"
                    onblur="javascript:formatNumber(this,2);" />
            </td>
            <td align="center">
                <asp:Button ID="btnDelete" runat="server" CssClass="textInput" CausesValidation="false"
                    ToolTip="Delete this wager limit" Text="Del" Width="40" CommandName="DELETE_LIMIT"
                    OnClientClick="return confirm('Are you sure you want to delete this wager limit?');" />
                <asp:HiddenField ID="hfLimitID" runat="server" />
            </td>
        </tr>
    </AlternatingItemTemplate>
    <ItemTemplate>
        <tr>
            <td align="center">
                <cc1:CDropDownList ID="ddlGameTypes" runat="server" CssClass="textInput" />
            </td>
            <td align="center">
                <asp:TextBox ID="txtContext" runat="server" CssClass="textInput" MaxLength="100" />
            </td>
            <td align="center">
                <asp:TextBox ID="txtPointSpreadP" runat="server" CssClass="textInput" Style="text-align: right;
                    padding-left: 2px;" Width="60" onkeypress="javascript:return inputNumber(this,event, false);"
                    onblur="javascript:formatNumber(this,2);" />
            </td>
            <td align="center">
                <asp:TextBox ID="txtTotalPointsP" runat="server" CssClass="textInput" Style="text-align: right;
                    padding-left: 2px;" Width="60" onkeypress="javascript:return inputNumber(this,event, false);"
                    onblur="javascript:formatNumber(this,2);" />
            </td>
            <td align="center">
                <asp:TextBox ID="txtMoneyLineP" runat="server" CssClass="textInput" Style="text-align: right;
                    padding-left: 2px;" Width="60" onkeypress="javascript:return inputNumber(this,event, false);"
                    onblur="javascript:formatNumber(this,2);" />
            </td>
            <td align="center">
                <asp:TextBox ID="txtPointSpreadI" runat="server" CssClass="textInput" Style="text-align: right;
                    padding-left: 2px;" Width="60" onkeypress="javascript:return inputNumber(this,event, false);"
                    onblur="javascript:formatNumber(this,2);" />
            </td>
            <td align="center">
                <asp:TextBox ID="txtTotalPointsI" runat="server" CssClass="textInput" Style="text-align: right;
                    padding-left: 2px;" Width="60" onkeypress="javascript:return inputNumber(this,event, false);"
                    onblur="javascript:formatNumber(this,2);" />
            </td>
            <td align="center">
                <asp:TextBox ID="txtMoneyLineI" runat="server" CssClass="textInput" Style="text-align: right;
                    padding-left: 2px;" Width="60" onkeypress="javascript:return inputNumber(this,event, false);"
                    onblur="javascript:formatNumber(this,2);" />
            </td>
            <td align="center">
                <asp:Button ID="btnDelete" runat="server" CssClass="textInput" CausesValidation="false"
                    ToolTip="Delete this wager limit" Text="Del" Width="40" CommandName="DELETE_LIMIT"
                    OnClientClick="return confirm('Are you sure you want to delete this wager limit?');" />
                <asp:HiddenField ID="hfLimitID" runat="server" />
            </td>
        </tr>
    </ItemTemplate>
    <FooterTemplate>
        <tr class="tableheading">
            <td align="center">
                <cc1:CDropDownList ID="ddlGameTypes" runat="server" CssClass="textInput" />
            </td>
            <td align="center">
                <asp:TextBox ID="txtContext" runat="server" CssClass="textInput" MaxLength="100" />
            </td>
            <td align="center">
                <asp:TextBox ID="txtPointSpreadP" runat="server" CssClass="textInput" Style="text-align: right;
                    padding-left: 2px;" Width="60" onkeypress="javascript:return inputNumber(this,event, false);"
                    onblur="javascript:formatNumber(this,2);" />
            </td>
            <td align="center">
                <asp:TextBox ID="txtTotalPointsP" runat="server" CssClass="textInput" Style="text-align: right;
                    padding-left: 2px;" Width="60" onkeypress="javascript:return inputNumber(this,event, false);"
                    onblur="javascript:formatNumber(this,2);" />
            </td>
            <td align="center">
                <asp:TextBox ID="txtMoneyLineP" runat="server" CssClass="textInput" Style="text-align: right;
                    padding-left: 2px;" Width="60" onkeypress="javascript:return inputNumber(this,event, false);"
                    onblur="javascript:formatNumber(this,2);" />
            </td>
            <td align="center">
                <asp:TextBox ID="txtPointSpreadI" runat="server" CssClass="textInput" Style="text-align: right;
                    padding-left: 2px;" Width="60" onkeypress="javascript:return inputNumber(this,event, false);"
                    onblur="javascript:formatNumber(this,2);" />
            </td>
            <td align="center">
                <asp:TextBox ID="txtTotalPointsI" runat="server" CssClass="textInput" Style="text-align: right;
                    padding-left: 2px;" Width="60" onkeypress="javascript:return inputNumber(this,event, false);"
                    onblur="javascript:formatNumber(this,2);" />
            </td>
            <td align="center">
                <asp:TextBox ID="txtMoneyLineI" runat="server" CssClass="textInput" Style="text-align: right;
                    padding-left: 2px;" Width="60" onkeypress="javascript:return inputNumber(this,event, false);"
                    onblur="javascript:formatNumber(this,2);" />
            </td>
            <td align="center">
                <asp:Button ID="btnAdd" runat="server" CssClass="textInput" CausesValidation="false"
                    ToolTip="Create new wager limit" Text="Add" Width="40" CommandName="ADD_NEW_LIMIT" />
            </td>
        </tr>
        </table>
    </FooterTemplate>
</asp:Repeater>
