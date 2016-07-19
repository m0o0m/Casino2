<%@ Control Language="VB" AutoEventWireup="false" CodeFile="OddSetting.ascx.vb" Inherits="SBSAgents.Inc_Agents_OddSetting" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="cc1" %>

<div class="panel panel-grey">
    <div class="panel-heading">Manager Games</div>
    <div class="panel-body">

        <div class="form-group">
            <div class="col-md-2">
                <asp:LinkButton runat="server" ID="lbnFullGame" CommandArgument="Current" Text="FullGame" CssClass="label label-default"
                    OnClick="lbtTab_Click" CausesValidation="false">
                </asp:LinkButton>
            </div>
            <div class="col-md-2">
                <asp:LinkButton runat="server" ID="lbn1stHalf" CommandArgument="1H" Text="1st Half" CssClass="label label-default"
                    OnClick="lbtTab_Click" CausesValidation="false" />
            </div>
            <div class="col-md-2">
                <asp:LinkButton runat="server" ID="lbn2ndHalf" CommandArgument="2H" Text="2nd Half" CssClass="label label-default"
                    OnClick="lbtTab_Click" CausesValidation="false" />
            </div>
        </div>
        <div class="form-group">
            <div class="col-md-1 w120">
                <label class="control-label pt2">Select Game</label>
            </div>
            <div class="col-md-2">
                <cc1:CDropDownList ID="ddlGameType" runat="server" Style="margin-bottom: 8px" CssClass="form-control" hasOptionalItem="true" OptionalText="" OptionalValue=""
                    AutoPostBack="true">
                </cc1:CDropDownList>
            </div>
            <div class="col-md-2">
                <label class="control-label pt2">Select Bookmaker</label>
            </div>
            <div class="col-md-2">
                <cc1:CDropDownList ID="ddlBookMaker" runat="server" Style="margin-bottom: 8px;" CssClass="form-control" hasOptionalItem="false" AutoPostBack="true">
                </cc1:CDropDownList>
            </div>
        </div>
        <asp:Repeater runat="server" ID="rptMain">
            <ItemTemplate>
                <asp:Repeater runat="server" ID="rptBets" OnItemDataBound="rptBets_ItemDataBound">
                    <ItemTemplate>
                        <h3>
                            <%#CType(Container.Parent.Parent, RepeaterItem).DataItem%>&nbsp;<%#Container.DataItem.Key%>
                                                    Lines - <span style="margin-left: 5px;">Game
                                                        <%#SBCBL.std.SafeDate(CType(Container.DataItem.Value, System.Data.DataRow())(0)("Gamedate")).ToString("MM/dd/yyyy")%>
                                                    </span>
                        </h3>
                        <asp:Label ID="lblNoGameLine" runat="server" Style="margin-left: 20px;">There is no <%#CType(Container.Parent.Parent, RepeaterItem).DataItem%> game.</asp:Label>
                        <asp:Repeater runat="server" ID="rptGameLines" OnItemCommand="rptGameLines_ItemCommand"
                            OnItemDataBound="rptGameLines_ItemDataBound">
                            <HeaderTemplate>
                                <table class="table table-bordered" width="100%" align="center" cellpadding="2" cellspacing="1"
                                    class="<%# IIF(Container.ItemType = ListItemType.Item,"gametable_odd","gametable_even") %> gamebox"
                                    border="0">
                                    <tr class="tableheading" style="text-align: center;">
                                        <td colspan="3">Team
                                        </td>
                                        <td width="20%" id="tdSpread" runat="server">Spread
                                        </td>
                                        <td width="10%" id="tdMLine" runat="server">Money Line
                                        </td>
                                        <td width="20%" id="tdTotal" runat="server">Total Points
                                        </td>
                                        <td>Off Line
                                        </td>
                                        <td>Circle
                                        </td>
                                        <td>
                                            <nobr> Manual Setting </nobr>
                                        </td>
                                        <td></td>
                                    </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr align="center">
                                    <td rowspan="2">
                                        <%#IIf(SBCBL.std.SafeString(Container.DataItem("IsCircle")) = "Y", "<span style='color:red'>* Game Circled</span>", "")%>
                                        <%#IIf(SBCBL.std.SafeString(Container.DataItem("IsAddedGame")) = "Y", "<span style='color:red'>* Added Game</span>", "")%>
                                        <asp:Label runat="server" ID="lblWarn"><%#IIf(SBCBL.std.SafeString(Container.DataItem("IsWarn")) = "Y", "<span style='color:red'>** " & SBCBL.std.SafeString(Container.DataItem("WarnReason")) & "</span>", "")%>
                                        </asp:Label>
                                    </td>
                                    <td class="game_num" align="left">
                                        <asp:Literal ID="lblAwayNumber" runat="server" Text='<%# Container.DataItem("AwayRotationNumber")%>'></asp:Literal>
                                    </td>
                                    <td align="left">
                                        <asp:Literal ID="lblAwayTeam" runat="server" Text='<%#Container.DataItem("AwayTeam")%>'></asp:Literal>
                                    </td>
                                    <td align="center">
                                        <asp:TextBox awayspread="1" CssClass="form-control" ID="txtAwaySpread" runat="server"
                                            Text='<%#FormatValue(Container.DataItem("AwaySpread"))%>' onkeypress="javascript:return inputNumber(this,event, true);"
                                            MaxLength="7" Width="70px" Style="display: inline-block" />
                                        <span>-</span>
                                        <asp:TextBox awayspreadmoney="2" CssClass="form-control" ID="txtAwaySpreadMoney"
                                            runat="server" Text='<%#  FormatValue(Container.DataItem("AwaySpreadMoney")) %>'
                                            onkeypress="javascript:return inputNumber(this,event, true);" MaxLength="7" Width="70px" Style="display: inline-block" />
                                    </td>
                                    <td align="center">
                                        <asp:TextBox awaymoneyline="3" CssClass="form-control" ID="txtAwayMoneyLine"
                                            runat="server" Text='<%#FormatValue(Container.DataItem("AwayMoneyLine"))%>' onkeypress="javascript:return inputNumber(this,event, true);"
                                            MaxLength="7" />
                                    </td>
                                    <td align="center"><span>O</span>
                                        <asp:TextBox awaytotal="5" CssClass="form-control" ID="txtAwayTotal" runat="server"
                                            Text='<%#FormatValue(Container.DataItem("TotalPoints"))%>' onkeypress="javascript:return inputNumber(this,event, true);"
                                            MaxLength="7" Width="70px" Style="display: inline-block" />
                                        <span>-</span>
                                        <asp:TextBox TotalPointsOverMoney="totalmoney123" CssClass="form-control"
                                            ID="txtTotalPointsOverMoney" runat="server" Text='<%#FormatValue(Container.DataItem("TotalPointsOverMoney"))%>'
                                            onkeypress="javascript:return inputNumber(this,event, true);" MaxLength="7" Width="70px" Style="display: inline-block" />
                                    </td>
                                    <td rowspan="2">
                                        <asp:CheckBox name="chkLineOnOff" ID="chkLineOnOff" runat="server" Checked='<%#  SBCBL.std.SafeString(Container.DataItem("GameLineOff")).Equals("Y", StringComparison.CurrentCultureIgnoreCase) %>' />
                                    </td>
                                    <td rowspan="2">
                                        <asp:CheckBox circle="7" ID="chkCircle" runat="server" Checked='<%#  SBCBL.std.SafeString(Container.DataItem("IsCircle")).Equals("Y", StringComparison.CurrentCultureIgnoreCase) %>' />
                                    </td>
                                    <td rowspan="2">
                                        <asp:CheckBox manualsetting="8" ID="chkManualSetting" runat="server" Checked='<%#  SBCBL.std.SafeString(Container.DataItem("ManualSetting")).Equals("Y", StringComparison.CurrentCultureIgnoreCase) %>' />
                                    </td>
                                    <td rowspan="2">
                                        <asp:HiddenField ID="hfGameID" runat="server" Value='<%#Container.DataItem("GameID")%>' />
                                        <asp:HiddenField ID="hfGameLineID" runat="server" Value='<%#Container.DataItem("GameLineID")%>' />
                                        <asp:Button btnsave="9" ID="btnSave" runat="server" Text="Save" CommandName="SAVE"
                                            CommandArgument='<%#Container.DataItem("GameID")%>' CssClass="button" />
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
                                        <asp:TextBox Enabled="false" homespread="10" CssClass="form-control" ID="txtHomeSpread"
                                            runat="server" Text='<%# FormatValue(Container.DataItem("HomeSpread"))%>' onkeypress="javascript:return inputNumber(this,event, true);"
                                            MaxLength="7" Width="70px" Style="display: inline-block" />
                                        <span>-</span>
                                        <asp:TextBox homespreadmoney="11" CssClass="form-control" ID="txtHomeSpreadMoney"
                                            runat="server" Text='<%# FormatValue(Container.DataItem("HomeSpreadMoney"))%>'
                                            onkeypress="javascript:return inputNumber(this,event, true);" MaxLength="7" Width="70px" Style="display: inline-block" />
                                    </td>
                                    <td align="center">
                                        <asp:TextBox runat="server" ID="txtHomeMoney" Text='<%#FormatValue(Container.DataItem("HomeMoneyLine")) %>'
                                            onkeypress="javascript:return inputNumber(this,event, true);" MaxLength="7" CssClass="form-control"
                                            homemoney="123homeMoney"></asp:TextBox>
                                    </td>
                                    <td align="center">
                                        <span>U</span>
                                        <asp:TextBox totalpoint2="13" CssClass="form-control" ID="txtTotalPoints2"
                                            Enabled="false" runat="server" Text='<%# FormatValue(Container.DataItem("TotalPoints"))%>'
                                            onkeypress="javascript:return inputNumber(this,event, true);" MaxLength="7" Width="70px" Style="display: inline-block" />
                                        <span>-</span>
                                        <asp:TextBox TotalPointsUnderMoney="TotalPointsUnderMoney234" CssClass="form-control"
                                            ID="txtTotalPointsUnderMoney" runat="server" Text='<%#FormatValue(Container.DataItem("TotalPointsUnderMoney"))%>'
                                            onkeypress="javascript:return inputNumber(this,event, true);" MaxLength="7" Width="70px" Style="display: inline-block" />
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <SeparatorTemplate>
                                <tr>
                                    <td colspan="11">
                                        <hr />
                                    </td>
                                </tr>
                            </SeparatorTemplate>
                            <FooterTemplate>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                        <br />
                    </ItemTemplate>
                </asp:Repeater>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>



<script type="text/javascript" language="javascript">

    function ShowControls(theCheckbox) {

        var sAwayspread = $(theCheckbox).parent().parent().parent().find('input[awayspread]');
        var sAwayspreadmoney = $(theCheckbox).parent().parent().parent().find("input[awayspreadmoney]");
        var sawaymoneyline = $(theCheckbox).parent().parent().parent().find("input[awaymoneyline]");
        var sawaytotal = $(theCheckbox).parent().parent().parent().find("input[awaytotal]");
        var stotalpointovermoney = $(theCheckbox).parent().parent().parent().find("input[TotalPointsOverMoney]");
        var shomespreadmoney = $(theCheckbox).parent().parent().parent().next().find("input[homespreadmoney]");
        var stotalpointundermoney = $(theCheckbox).parent().parent().parent().next().find("input[TotalPointsUnderMoney]");
        var shomemoney = $(theCheckbox).parent().parent().parent().next().find("input[homemoney]");
        var spPrentLine = $(theCheckbox).parent().parent().parent().find("span[name=chkLineOnOff]");
        var chklineonoff = $(theCheckbox).parent().parent().parent().find("span[name=chkLineOnOff] input");
        //                var spParentCircle = $(theCheckbox).parent().parent().parent().find("span[circle]");
        //     var chkcircle = $(theCheckbox).parent().parent().parent().find("span[circle] input");

        if (theCheckbox.checked) {
            sAwayspread.attr('disabled', '');
            sAwayspreadmoney.attr('disabled', '');
            sawaymoneyline.attr('disabled', '');
            sawaytotal.attr('disabled', '');
            shomespreadmoney.attr('disabled', '');
            shomemoney.attr('disabled', '');
            stotalpointovermoney.attr('disabled', '');
            stotalpointundermoney.attr('disabled', '');
            spPrentLine.removeAttr('disabled');
            chklineonoff.removeAttr('disabled');
            //                    spParentCircle.removeAttr('disabled');
            //                    chkcircle.removeAttr('disabled');

        } else {
            sAwayspread.attr('disabled', 'disabled');
            sAwayspreadmoney.attr('disabled', 'disabled');
            sawaymoneyline.attr('disabled', 'disabled');
            sawaytotal.attr('disabled', 'disabled');
            shomespreadmoney.attr('disabled', 'disabled');
            shomemoney.attr('disabled', 'disabled');
            stotalpointovermoney.attr('disabled', 'disabled');
            stotalpointundermoney.attr('disabled', 'disabled');
            chklineonoff.attr('disabled', 'disabled');
            //                    chkcircle.attr('disabled', 'disabled');
        }
    }


</script>

