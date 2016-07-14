<%@ Control Language="VB" AutoEventWireup="false" CodeFile="DisplayAllGame.ascx.vb"
    Inherits="SBCSuperAdmin.DisplayAllGame" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="cc1" %>

<div class="row">
    <div class="col-lg-12">
        <span>Game Type</span>
        &nbsp;&nbsp;
        <cc1:CDropDownList ID="ddlGameType" runat="server" OptionalText="" OptionalValue=""
            hasOptionalItem="false" CssClass="form-control" AutoPostBack="true" Style="display: inline-block;" Width="230px">
        </cc1:CDropDownList>
        &nbsp;&nbsp;
        <span>Game Context</span>
        &nbsp;&nbsp;
        <asp:DropDownList ID="ddlContext" runat="server" CssClass="form-control" AutoPostBack="true" Style="display: inline-block;" Width="170px">
            <asp:ListItem Text="--All--" Value=""></asp:ListItem>
            <asp:ListItem Text="Current" Value="Current"></asp:ListItem>
            <asp:ListItem Text="1H" Value="1H"></asp:ListItem>
            <asp:ListItem Text="2H" Value="2H"></asp:ListItem>
        </asp:DropDownList>
        &nbsp;&nbsp;
        <asp:Button ID="btnReload" runat="server" Text="Reload" CssClass="btn btn-green" ToolTip="Reload Game Lines" />
    </div>
    <div class="clearfix"></div>
</div>
<div class="mbxl"></div>

<asp:Repeater runat="server" ID="rptContext">
    <ItemTemplate>
        <div id="dvLine" runat="server" style="background-position: #1E1E22; color: #009CFF; padding: 5px 3px 3px 5px; font-weight: bold;">
            <%# Container.DataItem%>
            Lines
        </div>
        <asp:Repeater runat="server" ID="rptGameLines" OnItemDataBound="rptGameLines_DataBound">
            <HeaderTemplate>
                <table class="<%# IIF(Container.ItemType = ListItemType.Item,"gametable_odd","gametable_even") %> table table-hover table-bordered">
                    <tr class="tableheading" style="text-align: center;">
                        <td colspan="3">Game
                        </td>
                        <td colspan="2">Spread
                        </td>
                        <td colspan="2">Total
                        </td>
                        <td>Game Status
                        </td>
                        <td>Score
                        </td>
                    </tr>
                    <tr class="tableheading" style="text-align: center;">
                        <td colspan="3"></td>
                        <td>5Dimes
                        </td>
                        <td>Pinnacle
                        </td>
                        <td>5Dimes
                        </td>
                        <td>Pinnacle
                        </td>
                        <td colspan="2"></td>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td rowspan="2" width="180">
                        <asp:Literal ID="lblGameInfo" runat="server"></asp:Literal>
                    </td>
                    <td width="30" class="game_num">
                        <asp:Literal ID="lblAwayNumber" runat="server"></asp:Literal>
                    </td>
                    <td width="*">
                        <asp:Literal ID="lblAwayTeam" runat="server"></asp:Literal>
                    </td>
                    <td width="70" align="center" style="background-color: #87CEFA">
                        <asp:Literal ID="lblAwaySpread" runat="server"></asp:Literal>
                        <asp:Literal ID="lblAwaySpreadMoney" runat="server"></asp:Literal>
                    </td>
                    <td width="70" align="center">
                        <asp:Literal ID="lblAwaySpread2" runat="server"></asp:Literal>
                        <asp:Literal ID="lblAwaySpreadMoney2" runat="server"></asp:Literal>
                    </td>
                    <td width="90" align="center" style="background-color: #87CEFA">
                        <asp:Literal ID="lblAwayTotalPoints" runat="server"></asp:Literal>
                        <asp:Literal ID="lblTotalPointsOverMoney" runat="server"></asp:Literal>
                    </td>
                    <td width="90" align="center">
                        <asp:Literal ID="lblAwayTotalPoints2" runat="server"></asp:Literal>
                        <asp:Literal ID="lblTotalPointsOverMoney2" runat="server"></asp:Literal>
                    </td>
                    <td width="60" rowspan="2" align="center">
                        <asp:Literal ID="lblGameStatus" runat="server"></asp:Literal>
                    </td>
                    <td width="30" align="center">
                        <asp:Literal ID="lblAwayScore" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td class="game_num">
                        <asp:Literal ID="lblHomeNumber" runat="server"></asp:Literal>
                    </td>
                    <td>
                        <asp:Literal ID="lblHomeTeam" runat="server"></asp:Literal>
                    </td>
                    <td align="center" style="background-color: #87CEFA">
                        <asp:Literal ID="lblHomeSpread" runat="server"></asp:Literal>
                        <asp:Literal ID="lblHomeSpreadMoney" runat="server"></asp:Literal>
                    </td>
                    <td align="center">
                        <asp:Literal ID="lblHomeSpread2" runat="server"></asp:Literal>
                        <asp:Literal ID="lblHomeSpreadMoney2" runat="server"></asp:Literal>
                    </td>
                    <td align="center" style="background-color: #87CEFA">
                        <asp:Literal ID="lblHomeTotalPoints" runat="server"></asp:Literal>
                        <asp:Literal ID="lblTotalPointsUnderMoney" runat="server"></asp:Literal>
                    </td>
                    <td align="center">
                        <asp:Literal ID="lblHomeTotalPoints2" runat="server"></asp:Literal>
                        <asp:Literal ID="lblTotalPointsUnderMoney2" runat="server"></asp:Literal>
                    </td>
                    <td align="center">
                        <asp:Literal ID="lblHomeScore" runat="server"></asp:Literal>
                    </td>
                </tr>
            </ItemTemplate>
            <SeparatorTemplate>
                <tr>
                    <td colspan="9">
                        <hr />
                    </td>
                </tr>
            </SeparatorTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </ItemTemplate>
</asp:Repeater>
