<%@ Control Language="VB" AutoEventWireup="false" CodeFile="FixSpreadMoney.ascx.vb" Inherits="SBCSuperAdmin.FixSpreadMoney" %>
<table id="tbFixedSpread" runat="server" class="b-bet-grid" style="margin-right: 10px; margin-bottom: 10px; vertical-align: top; margin-top: 0px; margin-left: 0px; width: 380px;
    display: inline-table;">
    <tr class="b-bet-grid__odd">
        <td style="white-space: nowrap" class="b-bet-grid__cell">
            Fixed Spread Money
        </td>
        <td style="text-align: right" class="b-bet-grid__cell">
            <asp:TextBox ID="txtSpreadMoney" Width="70" CssClass="textInput" runat="server" onkeypress="javascript:return inputNumber(this,event, true);"
                Style="margin-right: 50px; text-align: right"></asp:TextBox>
        </td>
    </tr>
    <tr class="b-bet-grid__odd">
        <td style="white-space: nowrap" class="b-bet-grid__cell">
            Fixed Spread Money (1H, 2H)
        </td>
        <td style="text-align: right" class="b-bet-grid__cell">
            <asp:TextBox ID="txtHalfSpreadMoney" Width="70" CssClass="textInput" runat="server"
                onkeypress="javascript:return inputNumber(this,event, true);" Style="margin-right: 50px;
                text-align: right"></asp:TextBox>
        </td>
    </tr>
    <tr class="b-bet-grid__odd">
        <td class="b-bet-grid__cell" colspan="2">
            <asp:Button ID="btnSave" runat="server" CssClass="button" Style="float: right"
                Text="Save" />
        </td>
    </tr>
</table>

