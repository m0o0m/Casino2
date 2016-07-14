<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ParlayPayout.ascx.vb"
    Inherits="SBCSuperAdmins.ParlayPayout" %>
<table class="table table-hover table-bordered">
    <tr>
        <td align="center">
            <b>Teams</b>
        </td>
        <td align="center">
            <b>Current</b>
        </td>
        <td align="center">
            <b>WinSB</b>
        </td>
    </tr>
    <asp:Repeater ID="rptTeams" runat="server">
        <ItemTemplate>
            <tr style="border-bottom: solid 1px #CFCFCF;">
                <td align="right" style="border-right: solid 1px #CFCFCF; width: 85px;">
                    <asp:Label ID="lblTeam" runat="server" Text='<%#Container.DataItem%>' />
                    Teams
                </td>
                <td>
                    <asp:HiddenField ID="hfCurrentID" runat="server" />
                    <asp:TextBox ID="txtCurrent" runat="server" Width="170" Style="text-align: right;"
                        CssClass="textInput" ReadOnly="true" onkeypress="javascript:return inputNumber(this,event, false);"
                        onblur="javascript:formatNumber(this,3);" />
                </td>
                <td>
                    <asp:HiddenField ID="hfTigerSBID" runat="server" />
                    <asp:TextBox ID="txtTigerSB" runat="server" Width="170" Style="text-align: right;"
                        CssClass="textInput" onkeypress="javascript:return inputNumber(this,event, false);"
                        onblur="javascript:formatNumber(this,3);" />
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
    <tr>
        <td colspan="3" align="center" style="padding-top: 7px; padding-bottom: 5px;">
            <asp:Button ID="btnSave" runat="server" Text="Save Payouts" ToolTip="Save Payouts" CssClass="btn btn-primary"/>
                <br />(Only save WinSB payouts)
        </td>
    </tr>
</table>
