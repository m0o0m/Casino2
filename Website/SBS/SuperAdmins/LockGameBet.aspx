<%@ Page Language="VB" MasterPageFile="~/SBS/SuperAdmins/SuperAdmin.master" AutoEventWireup="false"
    CodeFile="LockGameBet.aspx.vb" Inherits="SBSSuperAdmin.LockGameBet" %>

<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/Inc/SuperAdmins/DisplayAllGame.ascx" TagName="DisPlayAllGame" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphContent" runat="Server">

    <div class="row">
        <div class="col-lg-12">
            <asp:CheckBox ID="chkAllGame" runat="server" Text="Today Games" AutoPostBack="true" />
            <uc:DisPlayAllGame ID="ucDisPlayAllGame" runat="server" Visible="false" />
        </div>
        <div class="clearfix"></div>
    </div>
    <div class="mbxl"></div>
    <div class="row">
        <asp:UpdatePanel runat="server" ID="aa">
            <ContentTemplate>
                <asp:Panel ID='pnlGameMonitor' runat="server" CssClass="col-lg-12">
                    <style type="text/css">
                        .bet_input { text-align: right; width: 30px; }
                    </style>

                    <div class="row">
                        <div class="col-lg-12">
                            <span>Bookmaker</span>
                            <cc1:CDropDownList ID="ddlBookmaker" runat="server" DataValueField="value" DataTextField="key"
                                Style="display: inline-block;" Width="270px" hasOptionalItem="false" CssClass="form-control" AutoPostBack="true">
                            </cc1:CDropDownList>

                        </div>
                    </div>
                    <div class="row">
                        <div class="col-lg-12">
                            <span>Game Type</span>
                            <cc1:CDropDownList ID="ddlGameType" runat="server" OptionalText="" OptionalValue="" Style="display: inline-block;" Width="230px"
                                hasOptionalItem="false" CssClass="form-control" AutoPostBack="true">
                            </cc1:CDropDownList>
                            &nbsp;&nbsp;
                            <span>Game Context</span>
                            <asp:DropDownList ID="ddlContext" runat="server" CssClass="form-control" AutoPostBack="true" Style="display: inline-block;" Width="110px">
                                <asp:ListItem Text="--All--" Value=""></asp:ListItem>
                                <asp:ListItem Text="Current" Value="Current"></asp:ListItem>
                                <asp:ListItem Text="1H" Value="1H"></asp:ListItem>
                                <asp:ListItem Text="2H" Value="2H"></asp:ListItem>
                            </asp:DropDownList>
                            &nbsp;&nbsp;
                            <asp:CheckBox ID="chkCircle" AutoPostBack="true" runat="server" Text="Game circled" TextAlign="Left" />
                            &nbsp;&nbsp;
                            <asp:CheckBox ID="chkAdded" AutoPostBack="true" runat="server" Text="Added game" TextAlign="Left" />
                            &nbsp;&nbsp;
                            <asp:CheckBox ID="chkWarn" AutoPostBack="true" runat="server" Text="Warning" TextAlign="Left" />
                            &nbsp;&nbsp;
                            <asp:Button ID="btnReload" runat="server" Text="Reload" CssClass="btn btn-primary" ToolTip="Reload Game Lines" />
                            &nbsp;
                            <asp:Button ID="btnAutomatic" runat="server" Text="Automatic" CssClass="btn btn-primary" ToolTip="Automatic Game Lines" />
                            &nbsp;
                            <asp:Button ID="btnManual" runat="server" Text="Manual" CssClass="btn btn-primary" ToolTip="Manual Game Lines" />
                        </div>
                        <div class="clearfix"></div>
                    </div>
                    <div class="mbxl"></div>

                    <asp:Repeater runat="server" ID="rptMain">
                        <ItemTemplate>
                            <div class="col-lg-12 pull-right">
                                <asp:HiddenField ID="HFGameType" Value='<%#Container.DataItem%>' runat="server" />
                                <asp:CheckBox ID="chk1H" Text="1H Off" OnCheckedChanged="chkOffline_CheckedChanged"
                                    AutoPostBack="true" runat="server" />
                                <asp:CheckBox ID="chk2H" Text="2H Off" OnCheckedChanged="chkOffline_CheckedChanged"
                                    AutoPostBack="true" runat="server" />
                            </div>
                            <div class="clearfix"></div>

                            <asp:Repeater runat="server" ID="rptBets" OnItemDataBound="rptBets_ItemDataBound">
                                <ItemTemplate>
                                    <div style="background-position: #1E1E22; background: #1E1E22; color: #009CFF; padding: 5px 3px 3px 5px; font-weight: bold;">
                                        <%#CType(Container.Parent.Parent, RepeaterItem).DataItem%>&nbsp;<%#Container.DataItem.Key%>Lines
                                    </div>
                                    <asp:Label ID="lblNoGameLine" runat="server" Style="margin-left: 20px;">There is no <%#CType(Container.Parent.Parent, RepeaterItem).DataItem%> game.</asp:Label>
                                    <asp:Repeater runat="server" ID="rptGameLines" OnItemCommand="rptGameLines_ItemCommand"
                                        OnItemDataBound="rptGameLines_ItemDataBound">
                                        <HeaderTemplate>
                                            <table class="<%# IIF(Container.ItemType = ListItemType.Item,"gametable_odd","gametable_even") %> table table-hover table-bordered">
                                                <colgroup>
                                                    <col width="110" />
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
                                                    <td colspan="3">Game
                                                    </td>
                                                    <td id="tdSpread" runat="server">Spread
                                                    </td>
                                                    <td id="tdTotal" runat="server">Total
                                                    </td>
                                                    <td id="tdMLine" runat="server">MLine
                                                    </td>
                                                    <td>Offline
                                                            <asp:LinkButton ForeColor="White" ID="lbnLockAllGame" CommandArgument='<%#CType(Container.Parent.Parent, RepeaterItem).DataItem.Key & "|Yes"%>'
                                                                CommandName='LOCK_ALL_GAME' ToolTip="Click here to lock all games" runat="server">Y</asp:LinkButton>/
                                                            <asp:LinkButton ForeColor="White" ID="lbnUnLockAllGame" CommandArgument='<%#CType(Container.Parent.Parent, RepeaterItem).DataItem.Key & "|No"%>'
                                                                CommandName='LOCK_ALL_GAME' ToolTip="Click here to unlock all games" runat="server">N</asp:LinkButton>
                                                    </td>
                                                    <td>MLine Off
                                                            <asp:LinkButton ForeColor="White" ID="lbnLockMLAllGame" CommandArgument='<%#CType(Container.Parent.Parent, RepeaterItem).DataItem.Key & "|Yes"%>'
                                                                CommandName='LOCK_ML_ALL_GAME' ToolTip="Click here to lock all games" runat="server">Y</asp:LinkButton>/
                                                            <asp:LinkButton ForeColor="White" ID="lbnUnLockMLAllGame" CommandArgument='<%#CType(Container.Parent.Parent, RepeaterItem).DataItem.Key & "|No"%>'
                                                                CommandName='LOCK_ML_ALL_GAME' ToolTip="Click here to unlock all games" runat="server">N</asp:LinkButton>
                                                    </td>
                                                    <td>TPoint Off
                                                            <asp:LinkButton ForeColor="White" ID="lbnLockTPointAllGame" CommandArgument='<%#CType(Container.Parent.Parent, RepeaterItem).DataItem.Key & "|Yes"%>'
                                                                CommandName='LOCK_TPOINT_ALL_GAME' ToolTip="Click here to lock all games" runat="server">Y</asp:LinkButton>/
                                                            <asp:LinkButton ForeColor="White" ID="lbnUnLockTPointAllGame" CommandArgument='<%#CType(Container.Parent.Parent, RepeaterItem).DataItem.Key & "|No"%>'
                                                                CommandName='LOCK_TPOINT_ALL_GAME' ToolTip="Click here to unlock all games" runat="server">N</asp:LinkButton>
                                                    </td>
                                                    <td>TeamTotalPoint Off
                                                                <br />
                                                        <asp:LinkButton ForeColor="White" ID="lbnLockTeamTotalPointAllGame" CommandArgument='<%#CType(Container.Parent.Parent, RepeaterItem).DataItem.Key & "|Yes"%>'
                                                            CommandName='LOCK_TEAMTOTALPOINT_ALL_GAME' ToolTip="Click here to lock all games" runat="server">Y</asp:LinkButton>/
                                                            <asp:LinkButton ForeColor="White" ID="lbnUnLockTeamTotalPointAllGame" CommandArgument='<%#CType(Container.Parent.Parent, RepeaterItem).DataItem.Key & "|No"%>'
                                                                CommandName='LOCK_TEAMTOTALPOINT_ALL_GAME' ToolTip="Click here to unlock all games" runat="server">N</asp:LinkButton>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lblDeleteGame" runat="server" Text="Delete Game"></asp:Label>
                                                    </td>
                                                </tr>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <tr <%#IIf(Container.DataItem("isNew").Equals("Y"), "class='isnew'", "")%>>
                                                <td rowspan="2">
                                                    <%#CDate(Container.DataItem("GameDate"))%><br />
                                                    <%#IIf(SBCBL.std.SafeString(Container.DataItem("IsCircle")) = "Y", "<span style='color:red'>* Game Circled</span>", "")%>
                                                    <%#IIf(SBCBL.std.SafeString(Container.DataItem("IsAddedGame")) = "Y", "<span style='color:red'>* Added Game</span>", "")%>
                                                    <asp:Label runat="server" ID="lblWarn"><%#IIf(SBCBL.std.SafeString(Container.DataItem("IsWarn")) = "Y", "<span style='color:red'>** " & SBCBL.std.SafeString(Container.DataItem("WarnReason")) & "</span>", "")%></asp:Label>
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
                                                <td align="center">O
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
                                                <td style="white-space: nowrap" valign="middle" rowspan="2" align="center">
                                                    <asp:Button ID="btnUpdate" runat="server" Visible='<%# IsManual %>' Text="Update"
                                                        ToolTip="Update Lines" CssClass="button" CommandArgument='<%#Container.DataItem("GameID")%>'
                                                        CommandName='UPDATE_LINE' />
                                                    <asp:LinkButton ID="lbnBetLock" Visible='<%# Not IsManual %>' CommandArgument='<%#Container.DataItem("GameID")%>'
                                                        CommandName='LOCK_BET' runat="server"></asp:LinkButton>
                                                    <asp:HiddenField ID="hfGameID" runat="server" Value='<%#Container.DataItem("GameID")%>'></asp:HiddenField>
                                                    <asp:HiddenField ID="HFContext" Value='<%#Container.DataItem("Context")%>' runat="server" />
                                                </td>
                                                <td style="white-space: nowrap" valign="middle" rowspan="2" align="center">
                                                    <asp:LinkButton ID="lbnMLLock" Visible='<%# Not IsManual %>' CommandArgument='<%#Container.DataItem("GameID")%>'
                                                        CommandName='LOCK_ML' runat="server"></asp:LinkButton>
                                                </td>
                                                <td style="white-space: nowrap" valign="middle" rowspan="2" align="center">
                                                    <asp:LinkButton ID="lbnTPointLock" Visible='<%# Not IsManual %>' CommandArgument='<%#Container.DataItem("GameID")%>'
                                                        CommandName='LOCK_TPOINT' runat="server"></asp:LinkButton>
                                                </td>
                                                <td style="white-space: nowrap" valign="middle" rowspan="2" align="center">
                                                    <asp:LinkButton ID="lbnTeamTotalPointLock" Visible='<%# Not IsManual %>' CommandArgument='<%#Container.DataItem("GameID")%>'
                                                        CommandName='LOCK_TEAMTOTALPOINT' runat="server"></asp:LinkButton>
                                                </td>
                                                <td rowspan="2" align="center">
                                                    <asp:Button ID="btnDelete" CommandArgument='<%#Container.DataItem("GameID")%>'
                                                        CommandName='DELETE_GAME' CssClass="button" runat="server" Text="Delete" />
                                                    <asp:Button ID="btnInsert" CommandArgument='<%#Container.DataItem("GameID")%>'
                                                        CommandName='INSERT_GAME' CssClass="button" runat="server" Text="Update" />
                                                </td>
                                            </tr>
                                            <tr <%#IIf(Container.DataItem("isNew").Equals("Y"), "class='isnew'", "")%>>
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
                                                <td align="center">U
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
                                                <td colspan="10">
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

                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <style type="text/css">
        .isnew { background-color: yellowgreen; }
    </style>
    <script language="javascript">
        var audioElement = document.createElement('audio');
        $(document).ready(function () {

            audioElement.setAttribute('src', '/2bet/beep.mp3');
            audioElement.setAttribute('autoplay', 'autoplay');
            //audioElement.load()

            $.get();

            audioElement.addEventListener("load", function () {
                // audioElement.play();
            }, true);


        });

        function beep() {
            //if ($(".isnew").length > 0)
            debugger;
            $("input").each(function (index, obj) {
                if ($(obj).css("background-color") == "rgb(255, 0, 0)")
                { audioElement.play(); return false; }
            });

        }

        setInterval(function () { document.getElementById("ctl00_cphContent_btnReload").click(); beep(); }, 30000);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
        function EndRequestHandler(sender, args) {
            try {
                beep();
            } catch (e) { }
        }
    </script>
</asp:Content>
