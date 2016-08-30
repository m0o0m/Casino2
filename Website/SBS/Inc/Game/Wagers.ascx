<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Wagers.ascx.vb" Inherits="SBSWebsite.Wagers" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>
<div id="betPickPanel" runat="server" class="row">
    <div id="picks" class="col-lg-12" onkeypress="if(event.keyCode==13){debugger;document.getElementById('cphBody_ucBetActions_ucWagers_btnSubmit').click();return false;}">
        <div id="wagers">
            <%--<h2 id="trTicketType" runat="server">Wager type : <%=BetTypeActive.Replace("BetTheBoard", "Straight Bet(s)").Replace("Reverse", "Action Reverse").Replace("BetIfAll", "Bet The Board")%></h2>--%>
            <div id="wager-print">
            <table class="table table-condensed table-style-6 full wager-print">
                <tr class="caption">
                    <td class="pdT10 pdL25" colspan="2"><%=BetTypeActive.Replace("BetTheBoard", "Straight Bet(s)").Replace("Reverse", "Action Reverse").Replace("BetIfAll", "Bet The Board").Replace("If Win", "If Bet (If Win Only)").Replace("If Win or Push", "If Bet (If Win, Push or Cancel)")%>
                        <asp:Literal ID="ltrDateBet" runat="server"></asp:Literal>
                        - ET</td>
                    <td><asp:Literal ID="ltrHeadRisk" runat="server">Risk</asp:Literal></td>
                    <td><asp:Literal ID="ltrHeadWin" runat="server">Win</asp:Literal></td>
                    <td><asp:Literal ID="ltrHeadTicketNumber" Visible="False" runat="server">Ticket #</asp:Literal></td>
                </tr>
                <asp:Repeater ID="rptTickets" runat="server">
                    <ItemTemplate>
                        <asp:Repeater ID="rptTicketBets" runat="server" OnItemDataBound="rptTicketBets_ItemDataBound"
                                OnItemCommand="rptTicketBets_ItemCommand">
                                <ItemTemplate>
                                    <tr id="trTicketContent" runat="server" class="content">
                                        <td class="pdL25-i">
                                            <span class="fz12" style='<%# IIf((SBCBL.std.SafeString(Session("BetTypeActive")).Equals("Straight") OrElse SBCBL.std.SafeString(Session("BetTypeActive")).Equals("BetTheBoard") OrElse SBCBL.std.SafeString(Session("BetTypeActive")).Contains("If")),"display:none","") %>'>Selection <%#Container.ItemIndex + 1%>: </span>
                                            <span style='<%# IIf((SBCBL.std.SafeString(Session("BetTypeActive")).Equals("Straight") OrElse SBCBL.std.SafeString(Session("BetTypeActive")).Equals("BetTheBoard") OrElse SBCBL.std.SafeString(Session("BetTypeActive")).Contains("If")),"","display:none") %>'>Selection <%#CType(Container.Parent.Parent, RepeaterItem).ItemIndex + 1%>: </span>
                                        </td>
                                        <td>
                                            <%# Container.DataItem.DescriptionHtml %>
                                            <asp:HiddenField ID="hdfTicketBetId" Value="<%# Container.DataItem.TicketBetID%>" runat="server" />
                                            <asp:HiddenField ID="hdfContext" Value="<%# Container.DataItem.Context%>" runat="server" />
                                        </td>
                                        <td style="width: 81px" id="tdRisk" runat="server"><%# FormatNumber(CType(Container.Parent.Parent, RepeaterItem).DataItem.RiskAmount, 2)%></td>
                                        <td style="width: 81px" id="tdWin" runat="server"><%# FormatNumber(CType(Container.Parent.Parent, RepeaterItem).DataItem.WinAmount, 2)%></td>
                                        <td style="width: 81px" id="tdActions" runat="server">
                                            <asp:Label ID="lblTicketNumber" Text="<%# CType(Container.Parent.Parent, RepeaterItem).DataItem.TicketNumber%>" CssClass="ticket-number" Visible="False" runat="server"></asp:Label>
                                            <asp:LinkButton ID="lbtDeleteTicket" runat="server" Text="Remove" CssClass="link-2 clr-red" CommandName="DEL_TICKETBET" CommandArgument='<%# Container.DataItem.TicketID & "|" & Container.DataItem.TicketBetID%>'></asp:LinkButton>
                                            <asp:HiddenField ID="hfTicketID" Value="<%# Container.DataItem.TicketID%>" runat="server" />
                                        </td>
                                    </tr>
                                    <tr style="background: #f9f9f9; display: none">
                                        <td colspan="2" align="center">
                                            <h5>
                                                <span style='<%# IIf((SBCBL.std.SafeString(Session("BetTypeActive")).Equals("Straight") OrElse SBCBL.std.SafeString(Session("BetTypeActive")).Equals("BetTheBoard") OrElse SBCBL.std.SafeString(Session("BetTypeActive")).Contains("If")),"display:none","") %>'>Select #<%#Container.ItemIndex + 1%> </span>
                                                <%--<span style='<%# IIf((SBCBL.std.SafeString(Session("BetTypeActive")).Equals("Straight") OrElse SBCBL.std.SafeString(Session("BetTypeActive")).Equals("BetTheBoard") OrElse SBCBL.std.SafeString(Session("BetTypeActive")).Contains("If")),"","display:none") %>'>Select #<%#CType(Container.Parent.Parent, RepeaterItem).ItemIndex + 1%> </span>--%>
                                                        :
                                                   
                                                    <%#Container.DataItem.GameType.Replace("NCAA Football", "College Football").Replace("CFL Football", "Canadian Football").Replace("AFL Football", "Arena Football")%>
                                                    </h5>
                                        </td>
                                        <td id="tdAmount" visible="false" class="amount" runat="server">
                                            <h5>Amount</h5>
                                        </td>
                                    </tr>
                                    <tr id="trTicketBet" runat="server" style="display: none">
                                        <td>
                                            <b>
                                                <asp:Literal ID="lblTeam" runat="server" Text='<%# SBCBL.std.SafeString(Container.DataItem.Team)%>  '></asp:Literal>
                                                <span style="margin-left: 10px"><%# Container.DataItem.GameDate%>
                                                            - (EST)</span></b>
                                            <asp:Literal ID="lblContext" runat="server" Text='<%# SBCBL.std.SafeString(iif(LCase(SBCBL.std.SafeString(Container.DataItem.Context)) = "current","for Game",Container.DataItem.Context ))%>'></asp:Literal>
                                        </td>
                                        <td nowrap="nowrap" align="center">
                                            <asp:Label ID="lblLine" runat="server"></asp:Label>
                                            <asp:Literal ID="lblBuyPoint" runat="server"></asp:Literal>
                                        </td>
                                        <td id="tdAmount2" visible="false" style="text-align: right" runat="server" class="amount">
                                            <nobr><asp:TextBox ID="txtAmount" Visible ="false"   CssClass="amount textInput" Width="70px" onkeypress="javascript:return inputNumber(this,event, false);"  runat="server"></asp:TextBox>
                                                    <asp:Button ID="btnDelTicketBet" runat="server" Text="Del" 
                                                        CommandArgument='<%# Container.DataItem.TicketID & "|" & Container.DataItem.TicketBetID%>'
                                                        CommandName="DEL_TICKETBET" />
                                                    </nobr>
                                        </td>
                                    </tr>
                                    <tr style="display: none">
                                        <td></td>
                                        <td nowrap="nowrap" style="text-align: right">


                                            <%--<asp:Literal ID="lblBuyPoint" runat="server"></asp:Literal>--%>
                                            <wlb:CDropDownList ID="ddlBuyPoint" runat="server" hasOptionalItem="false" />
                                            <asp:TextBox ID="txtRisk" runat="server" Wager='<%# Container.DataItem.TicketID%>'
                                                Rate='<%# Container.DataItem.BetPoint%>' CssClass="textInput" MaxLength="10"
                                                Style="text-align: right; padding-left: 2px; display: none;" Width="50" />
                                        </td>
                                        <td></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                       

                        <table class="table table-condensed" runat="server" Visible="False">

                            <tr align="right" style="display: none">
                                <td>
                                <asp:Literal ID="lblRiskWin" runat="Server" Visible="False" />
                                    </td>
                            </tr>
                            <tr style="text-align: left; display: none">
                                <td colspan="3">
                                    <asp:Label ID="lblBetLimits" runat="server" Font-Bold="True" CssClass="label label-danger" />
                                </td>
                            </tr>
                        </table>


                        <tr>
                            <td>
                                <asp:Literal ID="lblSperateStraight" runat="server"></asp:Literal>
                                <asp:Panel ID="pnOptionWager" runat="server" CssClass="row" Style="color: black;">
                                    <asp:Panel ID="noticeParlay" runat="server" Visible="false" CssClass="col-lg-4">
                                        <div class="row">
                                            <div class="col-md-6">
                                                <wlb:CDropDownList ID="ddlType" runat="server" CssClass="form-control" hasOptionalItem="false" />
                                            </div>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="txtBetParlay" runat="server" Text='<%# IIf(SBCBL.std.SafeRound(Container.DataItem.RiskAmount)=0 , "",SBCBL.std.SafeRound(Container.DataItem.RiskAmount))%>'
                                                    CssClass="form-control" MaxLength="10" onkeypress="javascript:return inputNumber(this,event, false);" />
                                            </div>
                                        </div>
                                    </asp:Panel>

                                    <asp:Panel ID="noticeStraight" runat="server" Visible="false" CssClass="col-lg-4">
                                        <div class="row">
                                            <div class="col-md-12">
                                                <asp:RadioButton ID="rdRiskAmount" runat="server" GroupName="wager" Checked="true" onclick='showRisk(true)' />
                                                <asp:Label ID="lblRiskAmmount" runat="server" Text="Risk Amount"></asp:Label>
                                                <%--<asp:RadioButton ID="rdWinAmount" runat="server" GroupName="wager" onclick='showRisk(false)' /> To Win Amount --%>
                                                <asp:TextBox ID="txtBet" runat="server" Text='<%# IIf(SBCBL.std.SafeRound(Container.DataItem.RiskAmount)=0,"",SBCBL.std.SafeRound(Container.DataItem.RiskAmount))%>'
                                                    CssClass="form-control" Style="display: inline-block;" MaxLength="10" Width="90" onkeypress="javascript:return inputNumber(this,event, false);" />
                                                <asp:TextBox ID="txtWin" runat="server" Text='<%# IIf(SBCBL.std.SafeRound(Container.DataItem.WinAmount)=0,"",SBCBL.std.SafeRound(Container.DataItem.WinAmount))%>'
                                                    CssClass="form-control" Style="display: none;" MaxLength="10" Width="90" onkeypress="javascript:return inputNumber(this,event, false);" />
                                                <asp:Button ID="btnContinue" runat="server" Text="" CssClass="btn btn-dark" Style="margin-left: 10px;"
                                                    OnClick="btnPreview_Click" />
                                                <asp:Button ID="btnCancel" Visible="false" OnClick="btnClearWagers_Click" runat="server" Text="" Style="margin-left: 10px;" CssClass="btn btn-red"
                                                    ToolTip="Cancel Your Wager" />
                                            </div>
                                        </div>
                                    </asp:Panel>

                                    <div class="col-md-4">
                                        <div class="form-group">
                                            <asp:Label ID="lblRiskDsp" runat="server" Text="Bet: " Visible="false"></asp:Label>
                                            <%--<asp:TextBox ID="txtBet" runat="server" Text='<%# SBCBL.std.SafeRound(Container.DataItem.RiskAmount)%>'
                                        CssClass="textInput" MaxLength="10" Style="text-align: right; padding-left: 2px;"
                                        Width="50" onkeypress="javascript:return inputNumber(this,event, false);" />--%>
                                            <asp:Label ID="lblWinDsp" runat="server" Text="Win: " Visible="false"></asp:Label>
                                            <%-- <asp:TextBox ID="txtWin" runat="server" Text='<%# SBCBL.std.SafeRound(Container.DataItem.WinAmount)%>'
                                        CssClass="textInput" MaxLength="10" Style="text-align: right; padding-left: 2px;"
                                        Width="50" onkeypress="javascript:return inputNumber(this,event, false);" />--%>
                                            <asp:Label ID="lblResult" Style="display: none" runat="server" Text='<%# "Risk/Win: " & SBCBL.std.SafeRound(Container.DataItem.RiskAmount) & "/" & SBCBL.std.SafeRound(Container.DataItem.WinAmount) %>'></asp:Label>
                                            <asp:Button ID="btnNextWager" runat="server" Text="Continue" class="btn btn-dark" Style="margin-left: 10px;" OnClick="btnNextWager_Click" />
                                            <asp:Button ID="btnPreview" runat="server" Text="Continue" class="btn btn-dark" Style="margin-left: 10px;"
                                                OnClick="btnPreview_Click" />
                                            <asp:Button ID="btnCancel2" OnClick="btnClearWagers_Click" runat="server" Text="Cancel Wager" Style="margin-left: 10px;" CssClass="btn btn-red" ToolTip="Cancel Your Wager" />
                                        </div>
                                    </div>
                                </asp:Panel>
                            </td>
                        </tr>
                        <asp:Panel runat="server" Visible="False">
                            <asp:Literal ID="tableEnd" runat="server"></asp:Literal>
                        </asp:Panel>
                    </ItemTemplate>
                   <%-- <FooterTemplate>
                        <%#IIf((SBCBL.std.SafeString(Session("BetTypeActive")).Equals("Straight") OrElse SBCBL.std.SafeString(Session("BetTypeActive")).Equals("BetTheBoard") OrElse SBCBL.std.SafeString(Session("BetTypeActive")).Contains("If")), "</table> </td></tr></table>", "")%>
                    </FooterTemplate>--%>
                </asp:Repeater>
            </table>
            </div><!-- /#wager-print-->
            <asp:Label ID="lblTicketSummary" CssClass="ticket-summary" runat="server"></asp:Label>

            <asp:Panel ID="pnWagerConfirmed" runat="server" Visible="False">
                <div class="confirmed-message">
                    <img src="/Content/themes/agent/layout6/images/icons/confirmed-wager.png"/>
                    <asp:Label ID="lblCountWagerConfirmed" runat="server"></asp:Label>
                </div>
                <div class="confirmed-actions">
                    <asp:Button ID="btnMainMenu" class="button-style-3 w110px h24px" runat="server" Text="Main Menu"></asp:Button>
                    <a href="/SBS/Players/OpenBet.aspx">Pending Bets</a>
                    <button class="button-style-3 w110px h24px" onclick="printContent('wager-print');">Print</button>
                </div>
            </asp:Panel>

            <div id="pnWarningMessage" runat="server" class="pdTB10 pdLR25">
                <span class="fz12 bold clr-black">Please Review Wagers Carefully! Enter Password and click 'Confirm Bet(s)' to confirm and receive Ticket Numbers.</span>
            </div>

            <div id="tblBetTheboard" runat="server" visible="True" class="row pdTB10 pdL25 pdR50 text-center">
                <span id="pnSameAmount" visible="false" runat="server" style="text-align: right; display: inline-block;">
                    <asp:CheckBox ID="chkCheckAmount" onclick="if(this.checked){$('.amount').hide()}else{$('.amount').show();$('.amount').val('');}" runat="server" />Use same amount for All Bets 
                                       
                    <asp:TextBox ID="txtSameAmount" onkeypress="javascript:return inputNumber(this,event, false);" Width="100" runat="server" CssClass="form-control" Style="display: inline-block;"></asp:TextBox>
                    <asp:Button ID="btnPreviewGame" runat="server" CssClass="btn btn-dark" Text="Preview Bet(s)" Style="margin-left: 10px;" />
                </span>
                <asp:Label ID="lblMessage" CssClass="fz14 bold mgR10"
                    ForeColor="black" runat="server" Text="Password:"></asp:Label>
                <asp:TextBox ID="txtPassword" runat="server" Visible="true" TextMode="Password"
                    Width="180" MaxLength="50" CssClass="form-control input-field-2 h30px" Style="display: inline-block;"></asp:TextBox>
                <asp:Button ID="btnSubmit" runat="server" Visible="true" Text="Submit" Style="margin-left: 20px;"
                    ToolTip="Confirm Bet(s)" CssClass="btn btn-dark button-style-2 w100px h24px" />
                <asp:Button ID="btnCancel" runat="server" Text="Cancel Your Wager" Style="display: none; margin-left: 10px; margin-right: 25px;" CssClass="btn btn-red button-style-2 red w175px h24px"
                    ToolTip="Cancel Your Wager" />
                <%--<div class="clearfix"></div>--%>
            </div>
        </div>

        <div>
            <%--<div style=" margin: 0px 7px 0px 0px;">--%>

            <div style="float: right">
                <div id="dvBtnWager" runat="server">
                    <asp:Button ID="btnNewWager" runat="server" Text="Add New Wager" CommandName="NEW_WAGER" />
                    <%--  <asp:Button ID="btnBackWager" runat="server" Visible="false" Text="Back To Wager"
                        Style="padding-left: 10px;" ToolTip="Back To Wager" />--%>
                </div>
                <div id="dvBtnReview" runat="server" style="text-align: right;">
                    <asp:Button ID="btnBack" runat="server" Visible="false" Text="Back" Style="padding-left: 10px;" ToolTip="Go Back To Your Wager Selection" />

                </div>
            </div>
            <%--</div>--%>
        </div>
    </div>
    <div class="clearfix"></div>
</div>
<div id="noticeCancel" runat="server" style="text-align: center" visible="false">

    <font color="red" face="" size="4" style="background-color: #ffffff">Current wager cancelled....</font>
    <br />
    <span style="color: black; font-size: 12pt">You may begin entering another wager by selecting a wager type above.</span>

</div>
