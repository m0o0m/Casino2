<%@ Page Language="VB" MasterPageFile="~/SBS/CallCenter/CCAgents.master" AutoEventWireup="false"
    CodeFile="LinesMonitor.aspx.vb" Inherits="SBSCallCenterAgents.LinesMonitor" %>

<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <p>
                <style type="text/css">
                    .bet_input
                    {
                        text-align: right;
                        width: 30px;
                    }
                </style>
            </p>
            <table cellspacing="7">
                <tr>
                    <td>
                        Bookmaker
                        <cc1:CDropDownList ID="ddlBookmaker" runat="server" DataValueField="value" DataTextField="key"
                            Style="margin-left: 2px" hasOptionalItem="false" CssClass="textInput" AutoPostBack="true">
                        </cc1:CDropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        Game Type
                        <cc1:CDropDownList ID="ddlGameType" runat="server" OptionalText="" OptionalValue=""
                            hasOptionalItem="false" CssClass="textInput" AutoPostBack="true">
                        </cc1:CDropDownList>
                    </td>
                    <td>
                        Game Context
                        <asp:DropDownList ID="ddlContext" runat="server" CssClass="textInput" AutoPostBack="true">
                            <asp:ListItem Text="--All--" Value=""></asp:ListItem>
                            <asp:ListItem Text="Current" Value="Current"></asp:ListItem>
                            <asp:ListItem Text="1H" Value="1H"></asp:ListItem>
                            <asp:ListItem Text="2H" Value="2H"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                        <label style="float: left;">
                            Game circled
                            <asp:CheckBox ID="chkCircle" AutoPostBack="true" runat="server" Style="float: right;
                                margin-top: 0px;" /></label>
                    </td>
                     <td>
                        <label style="float: left;">
                            Added game
                            <asp:CheckBox ID="chkAdded" AutoPostBack="true" runat="server" Style="float: right;
                                margin-top: 0px;" /></label>
                    </td>
                    <td>
                        <label style="float: left;">
                            Warning
                            <asp:CheckBox ID="chkWarn" AutoPostBack="true" runat="server" Style="float: right;
                                margin-top: 0px;" /></label>
                    </td>
                    <td>
                     <asp:Button ID="btnReload" runat="server" Text="Reload" CssClass="button" ToolTip="Reload Game Lines" />
                        &nbsp;
                        <asp:Button ID="btnAutomatic" runat="server" Text="Automatic" CssClass="button"
                            ToolTip="Automatic Game Lines" />
                    </td>
                    <td>
                        <asp:Button ID="btnManual" runat="server" Text="Manual" CssClass="button" ToolTip="Manual Game Lines" />
                    </td>
                </tr>
            </table>
            <table style="width: 100%">
                <tr>
                    <td>
                        <asp:Repeater runat="server" ID="rptMain">
                            <ItemTemplate>
                                <table style="width: 100%">
                                    <tr>
                                        <td align="right">
                                            <asp:HiddenField ID="HFGameType" Value='<%#Container.DataItem%>' runat="server" />
                                            <asp:CheckBox ID="chk1H" Text="1H Off" OnCheckedChanged="chkOffline_CheckedChanged"
                                                AutoPostBack="true" runat="server" />
                                            <asp:CheckBox ID="chk2H" Text="2H Off" OnCheckedChanged="chkOffline_CheckedChanged"
                                                AutoPostBack="true" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                                <asp:Repeater runat="server" ID="rptBets" OnItemDataBound="rptBets_ItemDataBound">
                                    <ItemTemplate>
                                        <div style="background-position: #1E1E22; background: #1E1E22; color: #009CFF; padding: 5px 3px 3px 5px;
                                            font-weight: bold;">
                                            <%#CType(Container.Parent.Parent, RepeaterItem).DataItem%>&nbsp;<%#Container.DataItem.Key%>
                                            Lines
                                        </div>
                                        <asp:Label ID="lblNoGameLine" runat="server" Style="margin-left: 20px;">There is no <%#CType(Container.Parent.Parent, RepeaterItem).DataItem%> game.</asp:Label>
                                        <asp:Repeater runat="server" ID="rptGameLines" OnItemCommand="rptGameLines_ItemCommand"
                                            OnItemDataBound="rptGameLines_ItemDataBound">
                                            <HeaderTemplate>
                                                <table class="table_bet_input" width="100%" align="center" cellpadding="2" cellspacing="1"
                                                    class="<%# IIF(Container.ItemType = ListItemType.Item,"gametable_odd","gametable_even") %> gamebox">
                                                    <colgroup>
                                                        <col width="80" />
                                                        <col width="40" />
                                                        <col width="*" />
                                                        <col width="100" />
                                                        <col width="120" />
                                                        <col width="60" />
                                                        <col width="50" />
                                                        <col width="70" />
                                                        <col width="70" />
                                                    </colgroup>
                                                    <tr class="tableheading" style="text-align: center;">
                                                        <td colspan="3">
                                                            Game
                                                        </td>
                                                        <td id="tdSpread" runat="server">
                                                            Spread
                                                        </td>
                                                        <td id="tdTotal" runat="server">
                                                            Total
                                                        </td>
                                                        <td id="tdMLine" runat="server">
                                                            MLine
                                                        </td>
                                                        <td>
                                                            Offline
                                                            <asp:LinkButton ID="lbnLockAllGame" CommandArgument='<%#CType(Container.Parent.Parent, RepeaterItem).DataItem.Key & "|Yes"%>'
                                                            CommandName='LOCK_ALL_GAME' ToolTip="Click here to lock all games" runat="server">Y</asp:LinkButton>/
                                                            <asp:LinkButton ID="lbnUnLockAllGame" CommandArgument='<%#CType(Container.Parent.Parent, RepeaterItem).DataItem.Key & "|No"%>'
                                                            CommandName='LOCK_ALL_GAME' ToolTip="Click here to unlock all games" runat="server">N</asp:LinkButton>
                                                        </td>
                                                        <td>
                                                            MLine Off
                                                            <asp:LinkButton ID="lbnLockMLAllGame" CommandArgument='<%#CType(Container.Parent.Parent, RepeaterItem).DataItem.Key & "|Yes"%>'
                                                            CommandName='LOCK_ML_ALL_GAME' ToolTip="Click here to lock all games" runat="server">Y</asp:LinkButton>/
                                                            <asp:LinkButton ID="lbnUnLockMLAllGame" CommandArgument='<%#CType(Container.Parent.Parent, RepeaterItem).DataItem.Key & "|No"%>'
                                                            CommandName='LOCK_ML_ALL_GAME' ToolTip="Click here to unlock all games" runat="server">N</asp:LinkButton>
                                                        </td>
                                                        <td>
                                                            TPoint Off
                                                            <asp:LinkButton ID="lbnLockTPointAllGame" CommandArgument='<%#CType(Container.Parent.Parent, RepeaterItem).DataItem.Key & "|Yes"%>'
                                                            CommandName='LOCK_TPOINT_ALL_GAME' ToolTip="Click here to lock all games" runat="server">Y</asp:LinkButton>/
                                                            <asp:LinkButton ID="lbnUnLockTPointAllGame" CommandArgument='<%#CType(Container.Parent.Parent, RepeaterItem).DataItem.Key & "|No"%>'
                                                            CommandName='LOCK_TPOINT_ALL_GAME' ToolTip="Click here to unlock all games" runat="server">N</asp:LinkButton>
                                                        </td>
                                                    </tr>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <tr>
                                                    <td rowspan="2">
                                                        <%#CDate(Container.DataItem("GameDate"))%><br />
                                                        <%#IIf(SBCBL.std.SafeString(Container.DataItem("IsCircle")) = "Y", "<span style='color:red'>* Game Circled</span>", "")%>
                                                        <%#IIf(SBCBL.std.SafeString(Container.DataItem("IsAddedGame")) = "Y", "<span style='color:red'>* Added Game</span>", "")%>
                                                    <asp:Label runat="server" ID="lblWarn"><%#IIf(SBCBL.std.SafeString(Container.DataItem("IsWarn")) = "Y", "<span style='color:red'>** " & SBCBL.std.SafeString(Container.DataItem("WarnReason")) & "</span>", "")%>
                                                        </asp:Label>
                                                        <asp:LinkButton ID="lbnWarn" CommandName="REMOVE_WARN" CommandArgument='<%# Container.DataItem("GamelineID")%>'
                                                            ToolTip="Click to remove warning" runat="server" Visible='<%#SBCBL.std.SafeString(Container.DataItem("IsWarn")) = "Y"%>'>X</asp:LinkButton>
                                                    </td>
                                                    <td class="game_num">
                                                        <asp:Literal ID="lblAwayNumber" runat="server" Text='<%# Container.DataItem("AwayRotationNumber")%>'></asp:Literal>
                                                    </td>
                                                    <td>
                                                        <asp:Literal ID="lblAwayTeam" runat="server" Text='<%#Container.DataItem("AwayTeam")%>'></asp:Literal>
                                                    </td>
                                                    <td align="center">
                                                        <asp:TextBox Enabled='<%# IsManual %>' CssClass="bet_input textInput" ID="txtAwaySpread"
                                                            runat="server" Text='<%#FormatValue(Container.DataItem("AwaySpread"))%>' onkeypress="javascript:return inputNumber(this,event, true);"
                                                            MaxLength="7" />
                                                        -
                                                        <asp:TextBox Enabled='<%# IsManual %>' CssClass="bet_input textInput" ID="txtAwaySpreadMoney"
                                                            runat="server" Text='<%#  FormatValue(Container.DataItem("AwaySpreadMoney")) %>'
                                                            onkeypress="javascript:return inputNumber(this,event, true);" MaxLength="7" />
                                                    </td>
                                                    <td align="center">
                                                        O
                                                        <asp:TextBox Enabled='<%# IsManual %>' CssClass="bet_input textInput" ID="txtAwayTotal"
                                                            runat="server" Text='<%#FormatValue(Container.DataItem("TotalPoints"))%>' onkeypress="javascript:return inputNumber(this,event, true);"
                                                            MaxLength="7" />
                                                        -
                                                        <asp:TextBox Enabled='<%# IsManual %>' CssClass="bet_input textInput" ID="txtTotalPointsOverMoney"
                                                            runat="server" Text='<%#FormatValue(Container.DataItem("TotalPointsOverMoney"))%>'
                                                            onkeypress="javascript:return inputNumber(this,event, true);" MaxLength="7" />
                                                    </td>
                                                    <td align="center">
                                                        <asp:TextBox Enabled='<%# IsManual %>' CssClass="bet_input textInput" ID="txtAwayMoneyLine"
                                                            runat="server" Text='<%#FormatValue(Container.DataItem("AwayMoneyLine"))%>' onkeypress="javascript:return inputNumber(this,event, true);"
                                                            MaxLength="7" />
                                                    </td>
                                                    <td style="white-space: nowrap" valign="middle" rowspan="2">
                                                        <asp:Button ID="btnUpdate" runat="server" Visible='<%# IsManual %>' Text="Update"
                                                            ToolTip="Update Lines" CssClass="textInput" CommandArgument='<%#Container.DataItem("GameID")%>'
                                                            CommandName='UPDATE_LINE' />
                                                        <asp:LinkButton ID="lbnBetLock" Visible='<%# Not IsManual %>' CommandArgument='<%#Container.DataItem("GameID")%>'
                                                            CommandName='LOCK_BET' runat="server"></asp:LinkButton>
                                                        <asp:HiddenField ID="hfGameID" runat="server" Value='<%#Container.DataItem("GameID")%>'>
                                                        </asp:HiddenField>
                                                        <asp:HiddenField ID="HFContext" Value='<%#Container.DataItem("Context")%>' runat="server" />
                                                    </td>
                                                    <td style="white-space: nowrap" valign="middle" rowspan="2">
                                                        <asp:LinkButton ID="lbnMLLock" Visible='<%# Not IsManual %>' CommandArgument='<%#Container.DataItem("GameID")%>'
                                                            CommandName='LOCK_ML' runat="server"></asp:LinkButton>
                                                    </td>
                                                    <td style="white-space: nowrap" valign="middle" rowspan="2">
                                                        <asp:LinkButton ID="lbnTPointLock" Visible='<%# Not IsManual %>' CommandArgument='<%#Container.DataItem("GameID")%>'
                                                            CommandName='LOCK_TPOINT' runat="server"></asp:LinkButton>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="game_num">
                                                        <asp:Literal ID="lblHomeNumber" runat="server" Text='<%#Container.DataItem("HomeRotationNumber")%>'></asp:Literal>
                                                    </td>
                                                    <td>
                                                        <asp:Literal ID="lblHomeTeam" runat="server" Text='<%#Container.DataItem("HomeTeam")%>'></asp:Literal>
                                                    </td>
                                                    <td align="center">
                                                        <asp:TextBox Enabled="false" CssClass="bet_input textInput" ID="txtHomeSpread" runat="server"
                                                            Text='<%# FormatValue(Container.DataItem("HomeSpread"))%>' onkeypress="javascript:return inputNumber(this,event, true);"
                                                            MaxLength="7" />
                                                        -
                                                        <asp:TextBox Enabled='<%# IsManual %>' CssClass="bet_input textInput" ID="txtHomeSpreadMoney"
                                                            runat="server" Text='<%# FormatValue(Container.DataItem("HomeSpreadMoney"))%>'
                                                            onkeypress="javascript:return inputNumber(this,event, true);" MaxLength="7" />
                                                    </td>
                                                    <td align="center">
                                                        U
                                                        <asp:TextBox CssClass="bet_input textInput" ID="txtTotalPoints2" Enabled="false"
                                                            runat="server" Text='<%# FormatValue(Container.DataItem("TotalPoints"))%>' onkeypress="javascript:return inputNumber(this,event, true);"
                                                            MaxLength="7" />
                                                        -
                                                        <asp:TextBox Enabled='<%# IsManual %>' CssClass="bet_input textInput" ID="txtTotalPointsUnderMoney"
                                                            runat="server" Text='<%#FormatValue(Container.DataItem("TotalPointsUnderMoney"))%>'
                                                            onkeypress="javascript:return inputNumber(this,event, true);" MaxLength="7" />
                                                    </td>
                                                    <td align="center">
                                                        <asp:TextBox Enabled='<%# IsManual %>' CssClass="bet_input textInput" ID="txtHomeMoney"
                                                            runat="server" Text='<%#FormatValue(Container.DataItem("HomeMoneyLine")) %>'
                                                            onkeypress="javascript:return inputNumber(this,event, true);" MaxLength="7" />
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
                                                </table></FooterTemplate>
                                        </asp:Repeater>
                                        <br />
                                    </ItemTemplate>
                                </asp:Repeater>
                            </ItemTemplate>
                        </asp:Repeater>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
