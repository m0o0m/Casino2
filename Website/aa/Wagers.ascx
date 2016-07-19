<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Wagers.ascx.vb" Inherits="SBSWebsite.Wagers" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>
<div id="betPickPanel" style="width: 98%; margin: 0 auto;" runat="server" >
    <div id="picks" onkeypress="if(event.keyCode==13){debugger;document.getElementById('cphBody_ucBetActions_ucWagers_btnSubmit').click();return false;}">
        <div id="wagers" style="width: 100%;">
        <table  border="0" cellpadding="0"  class="picks"  >
        <tr>
            <td colspan="2" align="right" >
            <asp:Button ID="btnBackGame" runat="server" Text="Back To Game" class="button" style="margin:5px" OnClick="btnClearWagers_Click" />
            </td>
        </tr>
        <tr><td>
        <table  border="0" cellpadding="5"  width="100%">
        <tr id="trTicketType" runat="server" style="background:black"  >
           <td colspan="2">
               <span style="padding: 5px;color:White;text-align :center">Wager type :
                   <%=BetTypeActive.Replace("BetTheBoard", "Straight Bet(s)")%>
              </span>
         </td>
       </tr>
            <asp:Repeater ID="rptTickets" runat="server">
                <ItemTemplate>
                       
                        <tr >
                            <td>
                                <table border="1" cellspacing="0" cellpadding="7" width="100%" align="center" style="margin:0 auto;border-collapse:collapse;background:#fff" class="tbl_wager">
                                    <%--  <tr id="trTicket" runat="server">
                            <td colspan="2" align="left">
                                <asp:Label ID="lblTicketType" runat="server" CssClass="org_txt" Text='<%# Container.DataItem.TicketType%>'></asp:Label>
                                <asp:Label ID="lblIndex" runat="server" CssClass="blue_txt" Text='<%# " (Wager #" & SBCBL.std.SafeString(Container.ItemIndex + 1) & ")"%>'></asp:Label>
                            </td>
                        </tr>--%> 
                                    
                                    <asp:Repeater ID="rptTicketBets" runat="server" OnItemDataBound="rptTicketBets_ItemDataBound"
                                        OnItemCommand="rptTicketBets_ItemCommand">
                                        <HeaderTemplate>
                                           
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <tr>
                                                <td colspan="2" align="center" style="background:#F1F1F1">
                                                    <nobr style='<%# IIf((SBCBL.std.SafeString(Session("BetTypeActive")).Equals("Straight") OrElse SBCBL.std.SafeString(Session("BetTypeActive")).Equals("BetTheBoard") OrElse SBCBL.std.SafeString(Session("BetTypeActive")).Contains("If")),"display:none","") %>'>  Select #<%#Container.ItemIndex + 1%> </nobr>
                                                    <nobr style='<%# IIf((SBCBL.std.SafeString(Session("BetTypeActive")).Equals("Straight") OrElse SBCBL.std.SafeString(Session("BetTypeActive")).Equals("BetTheBoard") OrElse SBCBL.std.SafeString(Session("BetTypeActive")).Contains("If")),"","display:none") %>'>  Select #<%#CType(Container.Parent.Parent, RepeaterItem).ItemIndex + 1%> </nobr>
                                                
                                                    :
                                                    <%#Container.DataItem.GameType.Replace("NCAA Football", "College Football").Replace("CFL Football", "Canadian Football").Replace("AFL Football", "Arena Football")%>
                                                </td>
                                                <td id="tdAmount" visible="false" class="amount" runat="server" style="background:#F1F1F1">
                                                    Amount 
                                                </td>
                                            </tr>
                                            <tr id="trTicketBet" runat="server" >
                                                <td>
                                                   <b>
                                                        <asp:Literal ID="lblTeam" runat="server" Text='<%# SBCBL.std.SafeString(Container.DataItem.Team)%>  '></asp:Literal><span
                                                            style="margin-left: 10px"><%# Container.DataItem.GameDate%>
                                                            - (EST)</span></b>
                                                    <asp:Literal ID="lblContext" runat="server" Text='<%# SBCBL.std.SafeString(iif(LCase(SBCBL.std.SafeString(Container.DataItem.Context)) = "current","for Game",Container.DataItem.Context ))%>'></asp:Literal>
                                                </td>
                                                <td nowrap="nowrap" align="center">
                                                 <asp:Label ID="lblLine" runat="server"></asp:Label>
                                                 <asp:Literal ID="lblBuyPoint" runat="server"></asp:Literal>
                                                </td>
                                               <td id="tdAmount2" visible ="false" style="text-align:right"  runat="server" class="amount">
                                                   <nobr><asp:TextBox ID="txtAmount" Visible ="false"   CssClass="amount textInput" Width="70px" onkeypress="javascript:return inputNumber(this,event, false);"  runat="server"></asp:TextBox>
                                                    <asp:Button ID="btnDelTicketBet" runat="server" Text="Del" 
                                                        CommandArgument='<%# Container.DataItem.TicketID & "|" & Container.DataItem.TicketBetID%>'
                                                        CommandName="DEL_TICKETBET" />
                                                    </nobr>    
                                               </td>
                                            </tr>
                                            <tr style="display:none">
                                                <td>
                                                </td>
                                                <td nowrap="nowrap" style="text-align:right">
                                                    
                                                    
                                                    <%--<asp:Literal ID="lblBuyPoint" runat="server"></asp:Literal>--%>
                                                    <wlb:CDropDownList ID="ddlBuyPoint" runat="server"  hasOptionalItem="false" />
                                                    <asp:TextBox ID="txtRisk" runat="server" Wager='<%# Container.DataItem.TicketID%>'
                                                        Rate='<%# Container.DataItem.BetPoint%>' CssClass="textInput" MaxLength="10"
                                                        Style="text-align: right; padding-left: 2px; display: none;" Width="50" />
                                                </td>
                                                <td></td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                 
                                    <tr align="right" >
                                        <asp:Literal ID="lblRiskWin" runat="server" Visible="false" />
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                             <asp:Literal ID="lblSperateStraight" runat="server"></asp:Literal>  
                             <asp:Panel ID="pnOptionWager" runat="server" Font-Size="Large" >
                        <div style="line-height: 30px;color:black;">
                            <table border="0" >
                                <tr>
                                    <td> 
                                     <asp:Panel ID="noticeParlay" runat="server" Visible ="false" >
                                    <wlb:CDropDownList ID="ddlType" runat="server" CssClass="textInput" hasOptionalItem="false"   />
                                        <asp:TextBox ID="txtBetParlay" runat="server" Text='<%# IIf(SBCBL.std.SafeRound(Container.DataItem.RiskAmount)=0 , "",SBCBL.std.SafeRound(Container.DataItem.RiskAmount))%>'
                                    CssClass="textInput" MaxLength="10" Style="text-align: right; padding-left: 2px;" 
                                    Width="50" onkeypress="javascript:return inputNumber(this,event, false);" /> 
                                    
                                    </asp:Panel>
                                    </td>
                                    <td>
                                         <asp:Panel ID="noticeStraight" runat="server" Visible ="false" >
                                            <div style="text-align:right">
                                              <asp:RadioButton ID="rdRiskAmount" runat="server" GroupName="wager" Checked ="true" onclick='showRisk(true)' />
                                                <asp:Literal ID="lblRiskAmmount" runat="server" Text="Risk Amount"></asp:Literal>
                                               
                                             <%--   <asp:RadioButton ID="rdWinAmount" runat="server" GroupName="wager" onclick='showRisk(false)' />
                                              To Win Amount 
                                              --%>
                                               <asp:TextBox ID="txtBet" runat="server" Text='<%# IIf(SBCBL.std.SafeRound(Container.DataItem.RiskAmount)=0,"",SBCBL.std.SafeRound(Container.DataItem.RiskAmount))%>'
                                                CssClass="textInput" MaxLength="10" Style="text-align: right; padding-left: 2px;"
                                                Width="50" onkeypress="javascript:return inputNumber(this,event, false);" /> 
                                                <asp:TextBox ID="txtWin" runat="server" Text='<%# IIf(SBCBL.std.SafeRound(Container.DataItem.WinAmount)=0,"",SBCBL.std.SafeRound(Container.DataItem.WinAmount))%>'
                                                CssClass="textInput" MaxLength="10" Style="text-align: right; padding-left: 2px;display:none"
                                                Width="50" onkeypress="javascript:return inputNumber(this,event, false);" />
                                                <asp:Button ID="btnContinue" runat="server" Text="" CssClass ="continue" Style="margin-left: 10px;" 
                                                OnClick="btnPreview_Click" />
                                                <asp:Button ID="btnCancel" Visible="false" OnClick="btnClearWagers_Click" runat="server" Text="" Style="padding-left: 10px;" CssClass="btnCancel"
                                                 ToolTip="Cancel Your Wager" />
                                            </div>
                                        </asp:Panel>
                                    </td>
                                    <td>
                                     <asp:Label ID="lblRiskDsp" runat="server" Text="Bet: " Visible ="false"></asp:Label>
                                    <%--<asp:TextBox ID="txtBet" runat="server" Text='<%# SBCBL.std.SafeRound(Container.DataItem.RiskAmount)%>'
                                        CssClass="textInput" MaxLength="10" Style="text-align: right; padding-left: 2px;"
                                        Width="50" onkeypress="javascript:return inputNumber(this,event, false);" />--%>
                                    <asp:Label ID="lblWinDsp" runat="server" Text="Win: " Visible ="false"></asp:Label>
                                   <%-- <asp:TextBox ID="txtWin" runat="server" Text='<%# SBCBL.std.SafeRound(Container.DataItem.WinAmount)%>'
                                        CssClass="textInput" MaxLength="10" Style="text-align: right; padding-left: 2px;"
                                        Width="50" onkeypress="javascript:return inputNumber(this,event, false);" />--%>
                                    <asp:Label ID="lblResult" Style="display: none" runat="server" Text='<%# "Risk/Win: " & SBCBL.std.SafeRound(Container.DataItem.RiskAmount) & "/" & SBCBL.std.SafeRound(Container.DataItem.WinAmount) %>'></asp:Label>
                                    <asp:Button ID="btnNextWager" runat="server" Text="" class="continue" Style="margin-left: 10px;"  OnClick="btnNextWager_Click" />
                                    <asp:Button ID="btnPreview" runat="server" Text="" class="continue" Style="margin-left: 10px;"
                                        OnClick="btnPreview_Click" />
                                    <asp:Button ID="btnCancel2" OnClick="btnClearWagers_Click" runat="server" Text="" Style="padding-left: 10px;" CssClass="btnCancel"
                        ToolTip="Cancel Your Wager" />    
                                    </td>
                                </tr>
                                 
                            </table>
                        </div>
                    </asp:Panel>
                            </td>
                        </tr>
                    <asp:Literal ID="tableEnd" runat="server"></asp:Literal>
                     
                </ItemTemplate>
                <FooterTemplate>
                    <%#IIf((SBCBL.std.SafeString(Session("BetTypeActive")).Equals("Straight") OrElse SBCBL.std.SafeString(Session("BetTypeActive")).Equals("BetTheBoard") OrElse SBCBL.std.SafeString(Session("BetTypeActive")).Contains("If")), "</table> </td></tr></table>", "")%>
                </FooterTemplate>
            </asp:Repeater>
           
              <table id="tblBetTheboard"  runat="server" Visible="True" class="picks" cellpadding="5">
            <tr>
                <td>
                    <div style="text-align: right;">
                        <div style="text-align:center" id="pnSameAmount" visible="false"  runat="server">
                        <span style="position:relative;left:12px">
                            <asp:CheckBox ID="chkCheckAmount" onclick="if(this.checked){$('.amount').hide()}else{$('.amount').show();$('.amount').val('');}" runat="server" />Use same amount for All Bets 
                            <asp:TextBox ID="txtSameAmount" onkeypress="javascript:return inputNumber(this,event, false);" Width="100" runat="server"></asp:TextBox></span>
                            <asp:Button ID="btnPreviewGame" runat="server"  class="continue" Text="" Style="margin-left: 10px;" />
                        </div> 
                        <asp:Label ID="lblMessage" Style=" font-size: 14px;text-align:left;display:inline-block"
                            ForeColor="black" runat="server" Text="Please Enter Your Password To Confirm !"></asp:Label>
                        <asp:TextBox ID="txtPassword" runat="server" Visible="true" TextMode="Password"  
                            Width="100" MaxLength="50" Style="border: 1px solid #999999; height: 18px;"></asp:TextBox>
                        <asp:Button ID="btnSubmit" runat="server"  Visible="true" Text="Confirm Bet(s)" Style="padding-left: 10px;"
                             ToolTip="Confirm Bet(s)" />
                        <asp:Button ID="btnCancel" runat="server"  Text="" Style="padding-left: 10px;" CssClass="btnCancel"
                        ToolTip="Cancel Your Wager" />    
                    </div>
                </td>
            </tr>
        </table>
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
                    <asp:Button ID="btnBack" runat="server" Visible ="false" Text="Back" Style="padding-left: 10px;" ToolTip="Go Back To Your Wager Selection"  />
                    
                </div>
            </div>
            <%--</div>--%>
        </div>
    </div>
</div>
<div id="noticeCancel" runat="server" style="text-align:center" visible="false" >

<font color="red" face="" size="4" style="BACKGROUND-COLOR: #ffffff">Current wager cancelled....</font>
<br />
<span style="color:black;font-size:12pt">You may begin entering another wager by selecting a wager type above.</span>

</div>