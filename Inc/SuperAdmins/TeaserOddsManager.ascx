<%@ Control Language="VB" AutoEventWireup="false" CodeFile="TeaserOddsManager.ascx.vb"
    Inherits="SBCSuperAdmins.TeaserOddsManager" %>
<h5>For every 100 bet</h5><br />
<table rules="rows" class="table table-hover table-bordered">
    <tr>
        <td rowspan="2" align="center" >
            <b>Teams</b>
        </td>
        <td align="center" >
            <b>Teaser Rules</b>
        </td>
    </tr>
    <tr>
        <td class="success" valign="top">
            <asp:DataList ID="dltRuleTitles" runat="server" RepeatDirection="Horizontal" Width="100%"
                CellPadding="2" CellSpacing="2">
                <ItemStyle Width="90" HorizontalAlign="Center" />
                <ItemTemplate>
                    <asp:Label ID="lblRuleName" runat="server" />
                </ItemTemplate>
            </asp:DataList>
        </td>
    </tr>
    <asp:Repeater ID="rptTeams" runat="server">
        <ItemTemplate>
            <tr style="border-bottom: solid 1px #CFCFCF;">
                <td align="center" style="border-right: solid 1px #CFCFCF; width: 70px;">
                    <asp:Label ID="lblTeam" runat="server" Text='<%#Container.DataItem%>' />
                    Teams
                </td>
                <td>
                    <asp:DataList ID="dltRules" runat="server" RepeatDirection="Horizontal" Width="100%"
                        CellPadding="2" CellSpacing="2" OnItemDataBound="dltRules_ItemDataBound" OnItemCommand="dltRules_ItemCommand">
                        <ItemStyle Width="90" HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:TextBox ID="txtOdds" runat="server" Width="50" Style="text-align: center;" CssClass="textInput" />
                            <asp:HiddenField ID="hfRuleID" runat="server" Value='<%#Container.DataItem("TeaserRuleID")%>' />
                            <asp:HiddenField ID="hfSysSettingID" runat="server" />
                            <asp:ImageButton ID="ibtClearOdds" runat="server" ImageAlign="AbsMiddle" ToolTip="Clear this odds"
                                CommandName="CLEAR_ODDS" ImageUrl="/images/icn_delete.gif" Visible="false" />
                        </ItemTemplate>
                    </asp:DataList>
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
    <tr>
        <td colspan="2" align="center" style="padding-top: 5px; padding-bottom: 5px;">
            <asp:Button ID="btnSave" runat="server" Text="Save Odds" ToolTip="Save Odds" CssClass="btn btn-primary" />
        </td>
    </tr>
</table>
