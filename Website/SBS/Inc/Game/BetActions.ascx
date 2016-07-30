<%@ Control Language="VB" AutoEventWireup="false" CodeFile="BetActions.ascx.vb" Inherits="SBS_Inc_Game_BetActions" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>
<%@ Register Src="~/SBS/Inc/Game/Wagers.ascx" TagName="Wagers" TagPrefix="uc" %>


<div class="row">
    <div class="col-lg-12">
        <!--
        <div class="page-title-breadcrumb">
            <div class="col-md-1 pull-left">
                <span class="page-title">WagerType</span>
            </div>
            <div class="col-md-1 pull-left">
                <span class="label label-dark pull-left mtm" style="margin-top: -1px !important; font-size: larger;">
                    <%=IIf(UserSession.SelectedBetType(Me.SelectedPlayerID) <> "PROP", BetTypeActive.Replace("BetTheBoard", "Straight").Replace("Reverse", "Action Reverse").Replace("BetIfAll", "Bet The Board"), "Proposition/Future")%>
                </span>
            </div>
            <div class="clearfix"></div>
        </div>-->
        <div class="clear">
            <div class="left pdL30">
                <div class="ly-fixed w-auto-i baseline">
                    <div class="w130px clr-black">
                        <b>Wager Amount:</b>
                    </div>
                    <div class="w75px clr-black">
                        <input type="radio" name="rdC" value="" />
                        Rish
                    </div>
                    <div class="w75px clr-black">
                        <input type="radio" name="rdC" value="" />
                        Win
                    </div>
                    <div class="clr-black">
                        <input type="radio" name="rdC" value="" />
                        Base Amount
                    </div>
                </div>
            </div>
            <div class="right pdR20">
                <button class="button-style-3 w110px h24px">Main Menu</button>
                <asp:Button ID="btnUpdateLines" class="update button-style-3 w110px h24px" runat="server" Text="Refresh"></asp:Button>
            </div>
        </div>
    </div>
</div>
<div class="mbl"></div>
<div class="panel gamebetdetail">
    <div class="panel-body">

        <asp:UpdatePanel ID="UpdatePanel13" runat="server" UpdateMode="Always">
            <ContentTemplate>
                <asp:HiddenField ID="hfIsWagers" runat="server" Value="false" />
                <asp:Panel ID="pnBetAction" runat="server" onkeypress="if(event.keyCode==13){$('#'+btnContinue).click();}">
                    <asp:HiddenField ID="hfBetTypeActive" runat="server" Value="Straight" />
                    <asp:HiddenField ID="hfColor" runat="server" />
                    <input type="hidden" id='htmlhfselectPlayer' value='<%=SelectedPlayerID%>' />
                    <input type="hidden" id='htmlhfBetTypeActive' value='<%=hfBetTypeActive.ClientID%>' />
                    <input type="hidden" id='hfIsWagersID' value='<%=hfIsWagers.ClientID%>' />
                    <div class="gameHeaderLoad none">
                        <asp:Panel ID="pnBuyPoint" runat="server" Visible="false" CssClass="pull-left">
                            <asp:CheckBox ID="chkBuyPoint" runat="server" Checked="true"></asp:CheckBox>
                            Show the buy points options. 
                        </asp:Panel>
                        <div class="col-md-2 pull-right">
                            <button type="button" class="btn btn-dark pull-right" onclick='continueBet(this);'>
                                Continue  
                                <i class="fa fa-forward"></i>
                            </button>
                        </div>
                        <div class="clearfix"></div>
                        <asp:Label ID="lblLastUpdateBottom" runat="server" Text=""></asp:Label>
                        <%--<asp:Button ID="btnUpdateLines" class="update" runat="server" Style="margin-left: 10px" Text="Refresh"></asp:Button>--%>
                    </div>
                    <nobr class="gameHeaderLoad none">

<div id="dvHeaderIfBet" class="none" runat="server">Please select one wager from the chart below for your <b><%=IfBetOrdinal%></b> if-bet selection.</div>
<div id="dvHeaderOtherBet" runat="server">
            <%=getHeader()%>
            <asp:Button ID="btnContinue" runat="server" Text="" CssClass="continue" OnClick="btnContinue_Click" style="display: none;" /></div></nobr>
                    <asp:Label ID="lblMessage" runat="server" Style="color: Red; margin-left: 20px; position: relative; top: 15px"></asp:Label>
                    <asp:Panel ID="pnBetTheBoard" runat="server" Style="margin-top: 5px" Visible="false" class="none">
                        <asp:Panel ID="pnCheckSameMount" runat="server">
                            <input type="checkbox" id="chkShowCheckBox" onclick="showCheckBox(this)" />Use same amount for all bets:<input type="text" id="txtMultiBet" style="width: 60px" onkeypress="javascript:return inputNumber(this,event, false);" /><h2 style="display: inline; margin-left: 10px">NEW!!!</h2>
                        </asp:Panel>
                        <br />
                        <asp:CheckBox runat="server" ID="chkTeamTotal" AutoPostBack="true" OnCheckedChanged="chkTeamTotal_click" /><span style="color: blue"> Team Total Bettings</span>
                    </asp:Panel>
                    <span id="BetTypeActive" style="display: none"><%=IIf(UserSession.SelectedBetType(Me.SelectedPlayerID) <> "PROP", BetTypeActive.Replace("BetTheBoard", "Straight").Replace("Reverse", "Action Reverse"), "Proposition/Future")%></span>
                    <span id="BetTypeActive2" style="display: none"><%=IIf(UserSession.SelectedBetType(Me.SelectedPlayerID) <> "PROP", BetTypeActive.Replace("Reverse", "Action Reverse"), "Proposition/Future")%></span>

                    <asp:Label ID="lblBettingMsg" runat="server" Text="There is no line available" Style="color: Red; margin-left: 20px; position: relative; top: 10px"
                        Visible="false"></asp:Label>
                    <div id="content" class="mgT15">
                        <table class="table table-bordered full-w lst-game-bet">
                            <tbody>
                                <asp:Repeater runat="server" ID="rptMain">
                                    <ItemTemplate>
                                        <asp:Repeater runat="server" ID="rptBets" OnItemDataBound="rptBets_ItemDataBound">
                                            <ItemTemplate>
                                                <tr class="gameheader none">
                                                    <td colspan="6" class="GameType pdTB10 pdLR15">
                                                        <div class="ly-fixed">
                                                            <div>
                                                                <%--<asp:Label ID="lblGameType" runat="server" Text="" class="lh25"></asp:Label>
                                                                <asp:Label ID="lblGameContext" runat="server" Text=""></asp:Label>--%>
                                                            </div>
                                                            <div class="w150px text-right">
                                                                <%--<asp:Label ID="lblGameDate" runat="server" Text=""></asp:Label>--%>
                                                                <button class="button-style-2 w110px h24px">Continue</button>
                                                            </div>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr class="none">
                                                    <td colspan="6" style="text-align: center">
                                                        <a href="#">
                                                            <asp:Label ID="lblLimitWager" runat="server" Font-Bold="True" CssClass="label label-danger"></asp:Label>
                                                        </a>
                                                    </td>
                                                </tr>
                                                <asp:Repeater runat="server" ID="rptGameLines" OnItemDataBound="rptGameLines_ItemDataBound">
                                                    <ItemTemplate >
                                                        <%--<tr context='<%#Container.DataItem("Context")%>'>
                                                        <th class="ThSport" id="thGameLineHeader" runat="server" colspan="8" align="left">
                                                        <asp:Literal ID="ltlTitle" runat="server"></asp:Literal>
                                                        </th>
                                                        </tr>--%>
                                                        <tr>
                                                            <td colspan="7" class="itm-game-bet">
                                                                <div class="caption ly-fixed">
                                                                    <div class="pdL10 fz14 lh-2">
                                                                        <asp:Label ID="lblGameTypeHeader" runat="server" Text="" class="lh25"></asp:Label>
                                                                    </div>
                                                                    <div class="w200px clear pdR10 v-top-i ">
                                                                        <button class="button-style-2 w110px right">Continue</button>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                        </tr>

                                                        <tr context='<%# Container.DataItem("Context")%>' class='<%# "GameHeader2 " + If(Container.ItemIndex Mod 2 = 0, "", "success") & If(SBCBL.std.SafeDate(Container.DataItem("GameDate")).DayOfWeek = DayOfWeek.Monday, " mondaygame", "")%>'>
                                                            <th colspan="3">
                                                                <span id="tdDate" runat="server">
                                                                    <!--Date-->
                                                                </span>
                                                                <span id="tdNum" runat="server">
                                                                    <!--#-->
                                                                </span>
                                                                <span id="tdTeam" runat="server">
                                                                    <asp:Label ID="lblGameContext" runat="server"  Text=""></asp:Label>
                                                                </span>
                                                            </th>
                                                            <th id="tdSpread" runat="server" align="left">
                                                                <%#GetSpreadTitle(CType(Container.Parent.Parent.Parent.Parent, RepeaterItem).DataItem, "Spread")%>
                                                            </th>

                                                            <th align="left" id="tdMLine" runat="server">Money Line</th>
                                                            <th align="left" id="tdTotal" runat="server">
                                                                <%#GetSpreadTitle(CType(Container.Parent.Parent.Parent.Parent, RepeaterItem).DataItem, "Total Points")%>
                                                            </th>

                                                            <th align="left" id="tdTeamTotal" class="tdTeamTotal" runat="server" colspan="2">
                                                                <span><%#IIf(SBCBL.std.SafeString((CType(Container.Parent.Parent.Parent.Parent, RepeaterItem).DataItem)).Contains("Baseball"), "Team Total Runs", "Team Total Points")%></span>
                                                            </th>
                                                        </tr>

                                                        <tr class='gamecontent<%# " offering_pair_" + SBCBL.std.SafeString(ViewState("OddEven")) & If(Container.ItemIndex Mod 2 = 0, "", " success") & If(SBCBL.std.SafeDate(Container.DataItem("GameDate")).DayOfWeek = DayOfWeek.Monday, " mondaygame", "")%>' context='<%#Container.DataItem("Context")%>'>
                                                            <td class="game-date nobdr" width="100">
                                                                <%--<%#SBCBL.std.SafeDate(Container.DataItem("GameDate")).ToString("f", System.Globalization.CultureInfo.CreateSpecificCulture("en-US")).Substring(0, 3)%>--%>
                                                                <%--&nbsp;--%>
                                                                <%#SBCBL.std.SafeDate(Container.DataItem("GameDate")).ToString("MM/dd")%>
                                                            </td>
                                                            <td class="game-number nobdr" width="30">
                                                                <asp:Literal ID="lblAwayNumber" runat="server" Text='<%#Container.DataItem("AwayRotationNumber")%>'></asp:Literal>
                                                            </td>
                                                            <td class="game-team">
                                                                <span style="font-weight: 800">
                                                                    <asp:Literal ID="lblAwayTeam" runat="server" Text='<%#Container.DataItem("AwayTeam")%>'></asp:Literal>
                                                                </span>
                                                                <span class="red">
                                                                    <asp:Literal ID="lblAwayPitcher" runat="server" />
                                                                </span>
                                                                <asp:Literal ID="lblAwayPitcherRightHand" runat="server"></asp:Literal>
                                                            </td>
                                                            <td id="tdSpread2" runat="server" width="190">
                                                                <asp:TextBox ID="txtMoneyAwaySpread" class="input-field-1 w43px h21px" runat="server" size="3" MaxLength="5" onkeypress="javascript:return inputNumber(this,event, false);"
                                                                    AutoPostBack="false" />
                                                                <asp:RadioButton ID="rdSelectAwaySpread" runat="server" CssClass="chkBetActionSelect" />
                                                                <asp:CheckBox ID="chkSelectAwaySpread" runat="server" CssClass="chkBetActionSelect" Style="margin-top: 2px" />
                                                                <wlb:CDropDownList ID="ddlBuyPointAwaySpread" runat="server" hasOptionalItem="false" Visible="false" />
                                                                <asp:Label ID="lblAwaySpread" runat="server" class="labelodd" />
                                                            </td>

                                                            <td id="tdMLine2" runat="server">
                                                                <nobr>
                                                                <asp:TextBox ID="txtMoneyAwayMLine" class="input-field-1 w43px h21px" runat="server" size="3" MaxLength="5"  onkeypress="javascript:return inputNumber(this,event, false);"
                                                                AutoPostBack="false" />
                                                                <asp:RadioButton ID="rdSelectAwayMLine" runat="server" CssClass="chkBetActionSelect" />
                                                                <asp:CheckBox ID="chkSelectAwayMLine" CssClass="chkBetActionSelect" runat="server" Style="margin-top: 2px"  />
                                                                <asp:Label ID="lblAwayMoneyLine" runat="server"  class="labelodd" />
                                                                <%#IIf(SBCBL.std.IsBaseball(Container.DataItem("GameType")), "<span class='mlaction'>Action</span>", "")%>
                                                                <br/>
                                                                <asp:RadioButton ID="rdSelectAwayMLine2" runat="server" CssClass="chkBetActionSelect" />
                                                                <asp:TextBox ID="txtMoneyAwayMLine2" runat="server" size="3"  MaxLength="5" onkeypress="javascript:return inputNumber(this,event, false);"
                                                                AutoPostBack="false" />
                                                                <asp:CheckBox ID="chkSelectAwayMLine2" CssClass="chkBetActionSelect" runat="server" />
                                                                <asp:Label ID="lblAwayMoneyLine2" runat="server"  class="labelodd" />
                                                                <%#IIf(SBCBL.std.IsBaseball(Container.DataItem("GameType")), "<span class='mlaction'>Listed </span>", "")%>
                                                                </nobr>
                                                            </td>
                                                            <td id="tdTotal2" runat="server" width="190">
                                                                <asp:TextBox ID="txtMoneyAwayTotal" class="input-field-1 w43px h21px" runat="server" size="3" MaxLength="5" onkeypress="javascript:return inputNumber(this,event, false);"
                                                                    AutoPostBack="false" />
                                                                <asp:RadioButton ID="rdSelectAwayTotal" runat="server" CssClass="chkBetActionSelect" />
                                                                <asp:CheckBox ID="chkSelectAwayTotal" CssClass="chkBetActionSelect" runat="server" />
                                                                <wlb:CDropDownList ID="ddlBuyPointAwayTotal" runat="server" hasOptionalItem="false" Visible="false" />
                                                                <asp:Label ID="lblAwayTotal" runat="server" class="labelodd" />
                                                            </td>

                                                            <td id="tdTeam_Total_AwayOver" runat="server" width="190">
                                                                <asp:TextBox ID="txtMoneyAwayTeamTotalOver" class="input-field-1 w43px h21px" runat="server" size="3" MaxLength="5" onkeypress="javascript:return inputNumber(this,event, false);" AutoPostBack="false" />
                                                                <asp:RadioButton ID="rdAwayTeamTotalOver" runat="server" CssClass="chkBetActionSelect" />
                                                                <asp:CheckBox ID="chkAwayTeamTotalOver" CssClass="chkBetActionSelect" runat="server" />
                                                                <span class="TeamTotal"><%#SBCBL.std.safeVegass(SBCBL.std.SafeString(Container.DataItem("AwayTeamTotalPoints")))%></span>
                                                                <asp:Literal ID="lblAwayTeamTotalPointsOverMoney" runat="server"></asp:Literal>
                                                            </td>
                                                            <td id="tdTeam_Total_AwayUnder" runat="server" width="190">
                                                                <asp:TextBox ID="txtMoneyAwayTeamTotalUnder" class="input-field-1 w43px h21px" runat="server" size="3" MaxLength="5" onkeypress="javascript:return inputNumber(this,event, false);" AutoPostBack="false" />
                                                                <asp:RadioButton ID="rdAwayTeamTotalUnder" runat="server" CssClass="chkBetActionSelect" />
                                                                <asp:CheckBox ID="chkAwayTeamTotalUnder" CssClass="chkBetActionSelect" runat="server" />
                                                                <span class="TeamTotal"><%#SBCBL.std.safeVegass(SBCBL.std.SafeString(Container.DataItem("AwayTeamTotalPoints")))%></span>
                                                                <asp:Literal ID="lblAwayTeamTotalPointsUnderMoney" runat="server"></asp:Literal>
                                                            </td>
                                                        </tr>
                                                      
                                                          <tr class='gamecontent <%# " offering_pair_" + SBCBL.std.SafeString(ViewState("OddEven")) & If(Container.ItemIndex Mod 2 = 0, "", " success") & If(SBCBL.std.SafeDate(Container.DataItem("GameDate")).DayOfWeek = DayOfWeek.Monday, " mondaygame", "")%>' context='<%#Container.DataItem("Context")%>'>
                                                            <td class="game-date nobdr">
                                                                <%#SBCBL.std.SafeDate(Container.DataItem("GameDate")).ToString("hh:mm tt")%>
                                                            </td>
                                                            <td class="game-number nobdr">
                                                                <asp:Literal ID="lblHomeNumber" runat="server" Text='<%#Container.DataItem("HomeRotationNumber")%>'></asp:Literal>
                                                            </td>
                                                            <td class="game-team">
                                                                <span style="font-weight: 800">
                                                                    <asp:Literal ID="lblHomeTeam" runat="server" Text='<%#Container.DataItem("HomeTeam")%>'></asp:Literal>

                                                                </span><span class="red">
                                                                    <asp:Literal ID="lblHomePitcher" runat="server"></asp:Literal></span>
                                                                <asp:Literal ID="lblHomePitcherRightHand" runat="server"></asp:Literal>
                                                            </td>
                                                            <td id="tdSpread3" runat="server" width="165">
                                                                <asp:TextBox ID="txtMoneyHomeSpread" class="input-field-1 w43px h21px" runat="server" size="3" MaxLength="5" onkeypress="javascript:return inputNumber(this,event, false);"
                                                                    AutoPostBack="false" />
                                                                <asp:RadioButton ID="rdSelectHomeSpread" runat="server" CssClass="chkBetActionSelect" />
                                                                <asp:CheckBox ID="chkSelectHomeSpread" CssClass="chkBetActionSelect" runat="server" Style="margin-top: 2px" />
                                                                <wlb:CDropDownList ID="ddlBuyPointHomeSpread" runat="server" hasOptionalItem="false" Visible="false" />
                                                                <asp:Label ID="lblHomeSpread" runat="server" class="labelodd" />
                                                            </td>
                                                              
                                                              <td id="tdMLine3" runat="server">
                                                                <asp:TextBox ID="txtMoneyHomeMLine" class="input-field-1 w43px h21px" runat="server" size="3" MaxLength="5" onkeypress="javascript:return inputNumber(this,event, false);"
                                                                    AutoPostBack="false" />
                                                                <asp:RadioButton ID="rdSelectHomeMLine" runat="server" CssClass="chkBetActionSelect" />
                                                                <asp:CheckBox ID="chkSelectHomeMLine" CssClass="chkBetActionSelect" runat="server" />
                                                                <asp:Label ID="lblHomeMoney" runat="server" class="labelodd" />
                                                                <%#IIf(SBCBL.std.IsBaseball(Container.DataItem("GameType")), "<span class='mlaction'>Action</span>", "")%>
                                                                <br />
                                                                <asp:RadioButton ID="rdSelectHomeMLine2" runat="server" CssClass="chkBetActionSelect" />
                                                                <asp:TextBox ID="txtMoneyHomeMLine2" class="input-field-1 w43px h21px" runat="server" size="3" MaxLength="5" onkeypress="javascript:return inputNumber(this,event, false);"
                                                                    AutoPostBack="false" />
                                                                <asp:CheckBox ID="chkSelectHomeMLine2" CssClass="chkBetActionSelect" runat="server" />
                                                                <asp:Label ID="lblHomeMoney2" runat="server" class="labelodd" />
                                                                <%#IIf(SBCBL.std.IsBaseball(Container.DataItem("GameType")), "<span class='mlaction'>Listed </span>", "")%>

                                                            </td>
                                                            <td id="tdTotal3" runat="server">
                                                                <asp:TextBox ID="txtMoneyHomeTotal" class="input-field-1 w43px h21px" runat="server" size="3" MaxLength="5" onkeypress="javascript:return inputNumber(this,event, false);"
                                                                    AutoPostBack="false" />
                                                                <asp:RadioButton ID="rdSelectHomeTotal" runat="server" CssClass="chkBetActionSelect" />
                                                                <asp:CheckBox ID="chkSelectHomeTotal" CssClass="chkBetActionSelect" runat="server" />
                                                                <asp:Label ID="Label1" runat="server" class="labelodd" />
                                                                <wlb:CDropDownList ID="ddlBuyPointHomeTotal" runat="server" hasOptionalItem="false" Visible="false" />
                                                                <asp:Label ID="lblHomeTotal" runat="server" class="labelodd" />
                                                            </td>

                                                            <td id="tdTeam_Total_HomeOver" runat="server">
                                                                <asp:TextBox ID="txtMoneyHomeTeamTotalOver" class="input-field-1 w43px h21px" runat="server" size="3" MaxLength="5" onkeypress="javascript:return inputNumber(this,event, false);"
                                                                    AutoPostBack="false" />
                                                                <asp:RadioButton ID="rdHomeTeamTotalOver" runat="server" CssClass="chkBetActionSelect" />
                                                                <asp:CheckBox ID="chkHomeTeamTotalOver" CssClass="chkBetActionSelect" runat="server" />
                                                                <span class="TeamTotal"><%#SBCBL.std.safeVegass(SBCBL.std.SafeString(Container.DataItem("HomeTeamTotalPoints")))%></span>
                                                                <asp:Literal ID="lblHomeTeamTotalPointsOverMoney" runat="server"></asp:Literal>
                                                            </td>
                                                            <td id="tdTeam_Total_HomeUnder" runat="server">
                                                                <asp:TextBox ID="txtMoneyHomeTeamTotalUnder" class="input-field-1 w43px h21px" runat="server" size="3" MaxLength="5" onkeypress="javascript:return inputNumber(this,event, false);" AutoPostBack="false" />
                                                                <span class="TeamTotal">
                                                                    <asp:RadioButton ID="rdHomeTeamTotalUnder" runat="server" CssClass="chkBetActionSelect" />
                                                                    <asp:CheckBox ID="chkHomeTeamTotalUnder" CssClass="chkBetActionSelect" runat="server" />
                                                                    <span class="TeamTotal"><%#SBCBL.std.safeVegass(SBCBL.std.SafeString(Container.DataItem("HomeTeamTotalPoints")))%></span>
                                                                    <asp:Literal ID="lblHomeTeamTotalPointsUnderMoney" runat="server"></asp:Literal></div>
                                                            </td>
                                                        </tr>
                                                     
                                                           <tr id="trDraw" runat="server" visible="false" class='gamecontent <%#getCssClass() & If(Container.ItemIndex Mod 2 = 0, "", " success") & If(SBCBL.std.SafeDate(Container.DataItem("GameDate")).DayOfWeek = DayOfWeek.Monday, " mondaygame", "")%>' context='<%#Container.DataItem("Context")%>'>
                                                            <td class="game-date nobdr"></td>
                                                            <td class="game-number nobdr">
                                                                <asp:Literal ID="Literal2" runat="server" Text='<%#Container.DataItem("DrawRotationNumber")%>'></asp:Literal>
                                                            </td>
                                                            <td class="game-team">Draw</td>
                                                            <td></td>
                                                            
                                                            <td id="tdDrawLast" runat="server"></td>
                                                            <td colspan="3">
                                                                <asp:TextBox ID="txtMoneyDraw" class="input-field-1 w43px h21px" runat="server" size="3" MaxLength="5" onkeypress="javascript:return inputNumber(this,event, false);"
                                                                    AutoPostBack="false" />
                                                                <asp:RadioButton ID="rdSelectDraw" runat="server" CssClass="chkBetActionSelect" />
                                                                <asp:CheckBox ID="chkSelectDraw" CssClass="chkBetActionSelect" runat="server" />
                                                                <asp:Label ID="lblDrawMoney" runat="server" Width="60" class="labelodd" />
                                                            </td>

                                                        </tr>

                                                        <tr id="trCircle" runat="server" visible="false" class='<%#If(Container.ItemIndex Mod 2 = 0, "", "success") & If(SBCBL.std.SafeDate(Container.DataItem("GameDate")).DayOfWeek = DayOfWeek.Monday, " mondaygame", "")%>' context='<%#Container.DataItem("Context")%>'>
                                                            <%--<td class="game_num" style="color: Red;" align="center">
                                                            *
                                                            </td>
                                                            <td colspan="1" style="color: Red;">
                                                            Game Circled
                                                            </td>--%>
                                                        </tr>

                                                        <tr context='<%#Container.DataItem("Context")%>' id="trDescription" runat="server" class='<%#If(Container.ItemIndex Mod 2 = 0, "", "success") & If(SBCBL.std.SafeDate(Container.DataItem("GameDate")).DayOfWeek = DayOfWeek.Monday, " mondaygame", "")%>'>
                                                            <td colspan="9" class="description" id="tdDescript1" runat="server">
                                                                <asp:Literal ID="lblDescription" runat="server"></asp:Literal>
                                                            </td>
                                                        </tr>

                                                        <tr context='<%#Container.DataItem("Context")%>' id="trDescription2" runat="server" class='<%# "offering_pair_" + SBCBL.std.SafeString(ViewState("OddEven")) & If(Container.ItemIndex Mod 2 = 0, "", " success") & If(SBCBL.std.SafeDate(Container.DataItem("GameDate")).DayOfWeek = DayOfWeek.Monday, " mondaygame", "")%>'>
                                                            <td colspan="9" class="description" id="tdDescript2" runat="server">
                                                                <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                                                            </td>
                                                        </tr>

                                                        <asp:HiddenField ID="hfInfo" runat="server"></asp:HiddenField>

                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                        <asp:Repeater runat="server" ID="rptProps">
                            <ItemTemplate>
                                <asp:Repeater runat="server" ID="rptPropLines" OnItemDataBound="rptPropLines_ItemDataBound">
                                    <ItemTemplate>
                                        <div style="clear: both"></div>
                                        <table class="table table-bordered">
                                            <tr class="gameheader">
                                                <td colspan="7" align="left">
                                                    <span>
                                                        <%#CType(Container.Parent.Parent, RepeaterItem).DataItem & " Proposition"%>
- Game -
                                                       
                                                        <asp:Literal runat="server" ID="lblGameDate"></asp:Literal>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="7" align="center" style="color: black; font-size: 16px; font-weight: bolder">
                                                    <asp:Literal runat="server" ID="lblHourDate"></asp:Literal>
                                                    <asp:Literal runat="server" ID="lblPropDes"></asp:Literal>
                                                </td>
                                            </tr>
                                            <asp:Repeater runat="server" ID="rptPropTeams" OnItemDataBound="rptPropTeams_ItemdataBound">
                                                <ItemTemplate>
                                                    <tr class='<%# "offering_pair_odd" & If(Container.ItemIndex Mod 2 = 0, "", " success") & If(SBCBL.std.SafeDate(Container.DataItem("GameDate")).DayOfWeek = DayOfWeek.Monday, " mondaygame", "")%>'>
                                                        <td colspan='3'>
                                                            <b>
                                                                <asp:Literal ID="lblPropRotationNumber" runat="server" Text='<%#Container.DataItem("PropRotationNumber")%>'></asp:Literal></b>
                                                            <span>
                                                                <asp:Literal ID="lbnPropParticipantName" runat="server" Text='<%#Container.DataItem("PropParticipantName")%>'></asp:Literal>
                                                        </td>
                                                        <td id="tdMLine2" runat="server" width="30%">
                                                            <asp:TextBox ID="txtMoneyPropMoneyLine" runat="server" size="3" MaxLength="5" onkeypress="javascript:return inputNumber(this,event, false);"
                                                                AutoPostBack="false" />

                                                            <asp:Label ID="lblPropMoneyLine" runat="server" CssClass="PropMoneyLine" />
                                                            <asp:HiddenField ID="hfInfoProp" runat="server" Value='<%#Container.DataItem("GameLineID") %>'></asp:HiddenField>

                                                        </td>
                                                    </tr>
                                                    <tr class='<%#If(Container.ItemIndex Mod 2 = 0, "", " success") & If(SBCBL.std.SafeDate(Container.DataItem("GameDate")).DayOfWeek = DayOfWeek.Monday, " mondaygame", "")%>'>
                                                        <td colspan="4"></td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </table>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </ItemTemplate>
                        </asp:Repeater>


                    </div>
                    <div class="gameHeaderLoad" style="margin-top: 20px; float: left;">
                        <div id="dvBottomIfBet" runat="server"><%--Please select one wager from the chart below for your <b><%=IfBetOrdinal%></b> if-bet selection.--%></div>
                        <div id="dvBottomOtherBet" runat="server">
                            <%=getHeader().Replace("below", "above")%>
                            <asp:Button ID="btnContinue2" runat="server" Text="" OnClick="btnContinue_Click" CssClass="continue" Visible="False" />
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-md-2 pull-right" style="margin-right: 20px;">
                            <button type="button" class="btn btn-dark pull-right" onclick='continueBet(this);'>
                                Continue
                                   
                                <i class="fa fa-forward"></i>
                            </button>
                        </div>
                        <div class="clearfix"></div>
                    </div>
                </asp:Panel>
                <uc:Wagers ID="ucWagers" runat="server" Visible="false" />
                <asp:LinkButton ID="autoSubmit" runat="server" />
                <input type="hidden" id="hfTeaser" value='<asp:Literal ID="lblDisableTeaser" runat="server"></asp:Literal>' />
                <input type="hidden" id="hfQuarterOnly" value='<asp:Literal ID="lblQuarterOnly" runat="server"></asp:Literal>' />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>

<script src="/Inc/Scripts/jQuery.js" type="text/javascript"></script>
<script src="/Inc/Scripts/BetAction.js?v=1.0" type="text/javascript"></script>

<script type="text/javascript" language="javascript">
    var objParlays = <%= ParlayJson %>
    var objTeasers = <%= TeaserJson %>
    var btnContinue="<%=btnContinue.ClientID %>";


    function continueBet(sender) {
        sender.style.display  = "none";
        
        var btnCont = document.getElementById(btnContinue);
        btnCont.click();
    }
</script>


<script type="text/javascript" language="javascript">
    var bSubmit = false;
    var form;
    var btnSubmitID = "<%=autoSubmit.ClientID %>"
    var btnSubmit = document.getElementById(btnSubmitID);
    function SetSubmitEvent(aform) {
        //debugger;
        bSubmit = true;
        form = aform;
    }
    function TextChange() {
        if (bSubmit) {
            bSubmit = false;
            // debugger;
            btnSubmit.click();
            form = null;
        }
    }
</script>



<script src="/Inc/Scripts/BetActionBottom.js?v=1.0" language="javascript" type="text/javascript"></script>




