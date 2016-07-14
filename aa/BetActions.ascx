<%@ Control Language="VB" AutoEventWireup="false" CodeFile="BetActions.ascx.vb" Inherits="SBS_Inc_Game_BetActions" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="~/SBS/Inc/Game/Wagers.ascx" TagName="Wagers" TagPrefix="uc" %>
    <script src="/Inc/Scripts/jQuery.js" language="javascript" type="text/javascript"></script>
  <script src="/Inc/Scripts/BetAction.js" language="javascript" type="text/javascript"></script>

  <script language="javascript">
      var objParlays = <%= ParlayJson %>
      var objTeasers = <%= TeaserJson %>
      var btnContinue="<%=btnContinue.ClientID %>";
        </script>
        <div style="position:relative;top:-30px;text-align:center;font-weight:900;color:white;">
            Wager type :<span id="Span1"> <%=IIf(UserSession.SelectedBetType(Me.SelectedPlayerID) <> "PROP", BetTypeActive.Replace("BetTheBoard", "Straight").Replace("Reverse", "Action Reverse").Replace("BetIfAll", "Bet The Board"), "Proposition/Future")%></span>
        </div>
<asp:UpdatePanel ID="UpdatePanel13" runat="server" UpdateMode="Always">
<ContentTemplate>
<asp:HiddenField ID="hfIsWagers" runat="server" Value="false" />
<asp:Panel ID="pnBetAction" runat="server" onkeypress="if(event.keyCode==13){$('#'+btnContinue).click();}">
<asp:HiddenField ID="hfBetTypeActive" runat="server" Value="Straight" />
<asp:HiddenField ID="hfColor" runat="server" />
<input type="hidden" id='htmlhfselectPlayer' value='<%=SelectedPlayerID%>' />
<input type="hidden" id='htmlhfBetTypeActive' value='<%=hfBetTypeActive.ClientID%>' /> 
<input type="hidden" id='hfIsWagersID' value='<%=hfIsWagers.ClientID%>' />
<div class="gameHeaderLoad" >
<asp:Panel ID="pnBuyPoint" runat="server" Visible ="false">
<asp:CheckBox  id="chkBuyPoint" runat="server" ></asp:CheckBox>Show the buy points options.<span style="font-weight:bold"> (Select this feature then click the "Refresh" button) <--BUY POINTS OPTION</span> 
</asp:Panel>
<br />
<asp:Label ID="lblLastUpdateBottom" runat="server" Text=""></asp:Label><asp:Button ID="btnUpdateLines" class="update" runat="server" style="margin-left:10px"></asp:Button>

<br /><br />
</div>
<nobr class="gameHeaderLoad" >

<div id="dvHeaderIfBet" runat="server">Please select one wager from the chart below for your <b><%=IfBetOrdinal%></b> if-bet selection.</div>
<div id="dvHeaderOtherBet" runat="server">
<%=getHeader()%>
    <asp:Button ID="btnContinue" runat="server" Text="" CssClass="continue" OnClick="btnContinue_Click" /> here or at the bottom of the page.</div></nobr>
<asp:Label ID="lblMessage" runat="server" Style="color: Red; margin-left: 20px; position: relative;
top: 15px"></asp:Label>
<asp:Panel ID="pnBetTheBoard" runat="server" style="margin-top:5px" Visible="false">
<asp:panel id="pnCheckSameMount" runat="server">
<input type="checkbox" id="chkShowCheckBox"   onclick="showCheckBox(this)" />Use same amount for all bets:<input type="text" id="txtMultiBet" style="width:60px" onkeypress="javascript:return inputNumber(this,event, false);" /><h2 style="display:inline;margin-left:10px">NEW!!!</h2>
</asp:panel> 
<br/>
<asp:CheckBox runat="server" id="chkTeamTotal" AutoPostBack="true" OnCheckedChanged="chkTeamTotal_click"  /><span style="color:blue"> Team Total Bettings</span> 
</asp:Panel>
<span id="BetTypeActive" style="display:none"> <%=IIf(UserSession.SelectedBetType(Me.SelectedPlayerID) <> "PROP", BetTypeActive.Replace("BetTheBoard", "Straight").Replace("Reverse", "Action Reverse"), "Proposition/Future")%></span>
<span id="BetTypeActive2" style="display:none"> <%=IIf(UserSession.SelectedBetType(Me.SelectedPlayerID) <> "PROP", BetTypeActive.Replace("Reverse", "Action Reverse"), "Proposition/Future")%></span>

<asp:Label ID="lblBettingMsg" runat="server" Text="There is no line available" Style="color: Red;
margin-left: 20px; position: relative; top: 10px" Visible="false"></asp:Label>
<div id="content" >
<table  border="0" align="left" cellpadding="3" cellspacing="0" class="table_lines">
<asp:Repeater runat="server" ID="rptMain">
<ItemTemplate>
<asp:Repeater runat="server" ID="rptBets" OnItemDataBound="rptBets_ItemDataBound">
<ItemTemplate>
<tr>
    <td colspan="6" class="GameType">
        <asp:Label ID="lblGameType" runat="server" Text=""></asp:Label>
        <div style="float:right;color:black"> <asp:Label ID="lblGameDate" runat="server" Text=""></asp:Label></div>              
   </td>
</tr>
<asp:Repeater runat="server" ID="rptGameLines" OnItemDataBound="rptGameLines_ItemDataBound">
<ItemTemplate>
<%--<tr context='<%#Container.DataItem("Context")%>'>
<th class="ThSport" id="thGameLineHeader" runat="server" colspan="8" align="left">
<asp:Literal ID="ltlTitle" runat="server"></asp:Literal>
</th>
</tr>--%>
<tr context='<%# Container.DataItem("Context")%>' class="GameHeader2">
<th id="tdDate" runat="server">
Date
</th>
<th id="tdNum" runat="server">
#
</th>
<th id="tdTeam" align="left" runat="server"  >
    Team
</th>
<th id="tdSpread" runat="server" align="left" >
<%#GetSpreadTitle(CType(Container.Parent.Parent.Parent.Parent, RepeaterItem).DataItem, "Spread")%>
</th>
<th align="left" id="tdTotal" runat="server">
<%#GetSpreadTitle(CType(Container.Parent.Parent.Parent.Parent, RepeaterItem).DataItem, "Total Points")%>
</th>
<th align="left"   id="tdMLine" runat="server" >
 Money Line
</th>
<th align="left" id="tdTeamTotal" class="tdTeamTotal" runat="server" colspan="2" >
 <span><%#IIf(SBCBL.std.SafeString((CType(Container.Parent.Parent.Parent.Parent, RepeaterItem).DataItem)).Contains("Baseball"), "Team Total Runs", "Team Total Points")%></span>
</th>
</tr>

<tr class="offering_pair_<%#SBCBL.std.SafeString(ViewState("OddEven")) %>" context='<%#Container.DataItem("Context")%>'>
<td width="100">
<%#SBCBL.std.SafeDate(Container.DataItem("GameDate")).ToString("f", System.Globalization.CultureInfo.CreateSpecificCulture("en-US")).Substring(0, 3)%>
&nbsp;&nbsp;
<%#SBCBL.std.SafeDate(Container.DataItem("GameDate")).ToString("MM/dd")%>
</td>
<td  width="30">
<asp:Literal ID="lblAwayNumber" runat="server" Text='<%#Container.DataItem("AwayRotationNumber")%>'></asp:Literal>
</td>
<td  >
<span style="font-weight:800" >
<asp:Literal ID="lblAwayTeam" runat="server" Text='<%#Container.DataItem("AwayTeam")%>'></asp:Literal>
</span>
<span class="red">
<asp:Literal ID="lblAwayPitcher" runat="server" />
</span>
<asp:Literal ID="lblAwayPitcherRightHand" runat="server"></asp:Literal>
</td>
<td  id="tdSpread2" runat="server" width="160" >
<asp:TextBox ID="txtMoneyAwaySpread"  
runat="server" size="3" MaxLength="5" onkeypress="javascript:return inputNumber(this,event, false);"
AutoPostBack="false" />
    <asp:RadioButton ID="rdSelectAwaySpread" runat="server" CssClass="chkBetActionSelect"  />
<asp:CheckBox ID="chkSelectAwaySpread" runat="server" CssClass="chkBetActionSelect"
Style="margin-top: 2px"  />
 <wlb:CDropDownList ID="ddlBuyPointAwaySpread" runat="server"  hasOptionalItem="false" Visible="false" />
<asp:Label ID="lblAwaySpread"  runat="server" class="labelodd" />
</td>
<td  id="tdTotal2" runat="server" width="190" >
<asp:TextBox ID="txtMoneyAwayTotal" runat="server" size="3" MaxLength="5" onkeypress="javascript:return inputNumber(this,event, false);"
AutoPostBack="false" />
<asp:RadioButton ID="rdSelectAwayTotal" runat="server" CssClass="chkBetActionSelect" />
<asp:CheckBox ID="chkSelectAwayTotal" CssClass="chkBetActionSelect" runat="server" />
 <wlb:CDropDownList ID="ddlBuyPointAwayTotal" runat="server"  hasOptionalItem="false" Visible="false" />
<asp:Label ID="lblAwayTotal" runat="server"  class="labelodd" />
</td>
<td  id="tdMLine2" runat="server" >
<nobr>
<asp:TextBox ID="txtMoneyAwayMLine" runat="server" size="3" MaxLength="5"  onkeypress="javascript:return inputNumber(this,event, false);"
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
<td  id="tdTeam_Total_AwayOver" runat="server" width="190" >
<asp:TextBox ID="txtMoneyAwayTeamTotalOver" runat="server"    size="3" MaxLength="5" onkeypress="javascript:return inputNumber(this,event, false);" AutoPostBack="false" />
 <asp:RadioButton ID="rdAwayTeamTotalOver" runat="server" CssClass="chkBetActionSelect"  />
 <asp:CheckBox ID="chkAwayTeamTotalOver" CssClass="chkBetActionSelect" runat="server" />
 <span class="TeamTotal"><%#SBCBL.std.safeVegass(SBCBL.std.SafeString(Container.DataItem("AwayTeamTotalPoints")))%></span>
  <asp:Literal ID="lblAwayTeamTotalPointsOverMoney" runat="server"></asp:Literal>  
</td>
<td  id="tdTeam_Total_AwayUnder" runat="server"  width="190">
<asp:TextBox ID="txtMoneyAwayTeamTotalUnder" runat="server"   size="3" MaxLength="5" onkeypress="javascript:return inputNumber(this,event, false);" AutoPostBack="false" />
  <asp:RadioButton ID="rdAwayTeamTotalUnder" runat="server" CssClass="chkBetActionSelect" />
  <asp:CheckBox ID="chkAwayTeamTotalUnder" CssClass="chkBetActionSelect" runat="server" />
 <span class="TeamTotal"><%#SBCBL.std.safeVegass(SBCBL.std.SafeString(Container.DataItem("AwayTeamTotalPoints")))%></span>
    <asp:Literal ID="lblAwayTeamTotalPointsUnderMoney" runat="server"></asp:Literal>
</td>
</tr>
<tr class="offering_pair_<%#SBCBL.std.SafeString(ViewState("OddEven")) %>" context='<%#Container.DataItem("Context")%>'>
<td>
<%#SBCBL.std.SafeDate(Container.DataItem("GameDate")).ToString("hh:mm tt")%>

</td>
<td>
<asp:Literal ID="lblHomeNumber" runat="server" Text='<%#Container.DataItem("HomeRotationNumber")%>'></asp:Literal>
</td>
<td  >
<span style="font-weight:800">
<asp:Literal ID="lblHomeTeam" runat="server" Text='<%#Container.DataItem("HomeTeam")%>'></asp:Literal>

</span> <span class="red">
<asp:Literal ID="lblHomePitcher" runat="server"></asp:Literal></span>
<asp:Literal ID="lblHomePitcherRightHand" runat="server"></asp:Literal>
</td>
<td  id="tdSpread3" runat="server" >
<asp:TextBox ID="txtMoneyHomeSpread" runat="server" size="3" MaxLength="5" onkeypress="javascript:return inputNumber(this,event, false);"
AutoPostBack="false" />
<asp:RadioButton ID="rdSelectHomeSpread" runat="server" CssClass="chkBetActionSelect" />
<asp:CheckBox ID="chkSelectHomeSpread" CssClass="chkBetActionSelect" runat="server"/>
 <wlb:CDropDownList ID="ddlBuyPointHomeSpread" runat="server"  hasOptionalItem="false" Visible="false" />
<asp:Label ID="lblHomeSpread"  runat="server" class="labelodd" />
</td>
<td  id="tdTotal3" runat="server">
<asp:TextBox ID="txtMoneyHomeTotal" runat="server" size="3"  MaxLength="5" onkeypress="javascript:return inputNumber(this,event, false);"
AutoPostBack="false" />
<asp:RadioButton ID="rdSelectHomeTotal" runat="server" CssClass="chkBetActionSelect"/>
<asp:Label ID="Label1" runat="server" class="labelodd" />
<asp:CheckBox ID="chkSelectHomeTotal" CssClass="chkBetActionSelect" runat="server" />
 <wlb:CDropDownList ID="ddlBuyPointHomeTotal" runat="server"  hasOptionalItem="false" Visible="false" />
<asp:Label ID="lblHomeTotal" runat="server" class="labelodd" />
</td>
<td  id="tdMLine3" runat="server">
<asp:TextBox ID="txtMoneyHomeMLine" runat="server" size="3" MaxLength="5" onkeypress="javascript:return inputNumber(this,event, false);"
AutoPostBack="false" />
<asp:RadioButton ID="rdSelectHomeMLine" runat="server" CssClass="chkBetActionSelect"  />
<asp:Label ID="lblHomeMoney" runat="server" class="labelodd" />
<asp:CheckBox ID="chkSelectHomeMLine" CssClass="chkBetActionSelect" runat="server"/>
<%#IIf(SBCBL.std.IsBaseball(Container.DataItem("GameType")), "<span class='mlaction'>Action</span>", "")%>
<br/>
<asp:RadioButton ID="rdSelectHomeMLine2" runat="server" CssClass="chkBetActionSelect"  />
<asp:TextBox ID="txtMoneyHomeMLine2"  runat="server"  size="3" MaxLength="5" onkeypress="javascript:return inputNumber(this,event, false);"
AutoPostBack="false" />
<asp:CheckBox ID="chkSelectHomeMLine2" CssClass="chkBetActionSelect" runat="server" />
<asp:Label ID="lblHomeMoney2" runat="server"  class="labelodd"  />
<%#IIf(SBCBL.std.IsBaseball(Container.DataItem("GameType")), "<span class='mlaction'>Listed </span>", "")%>

</td>
<td id="tdTeam_Total_HomeOver" runat="server"   >
<asp:TextBox ID="txtMoneyHomeTeamTotalOver" runat="server" size="3" MaxLength="5" onkeypress="javascript:return inputNumber(this,event, false);"
AutoPostBack="false" />  
<asp:RadioButton ID="rdHomeTeamTotalOver" runat="server" CssClass="chkBetActionSelect" />
<asp:CheckBox ID="chkHomeTeamTotalOver" CssClass="chkBetActionSelect" runat="server" />
<span class="TeamTotal"  ><%#SBCBL.std.safeVegass(SBCBL.std.SafeString(Container.DataItem("HomeTeamTotalPoints")))%></span>
<asp:Literal ID="lblHomeTeamTotalPointsOverMoney" runat="server"></asp:Literal>
</td>
<td id="tdTeam_Total_HomeUnder" runat="server" >
<asp:TextBox ID="txtMoneyHomeTeamTotalUnder" runat="server" size="3" MaxLength="5"  onkeypress="javascript:return inputNumber(this,event, false);" AutoPostBack="false" />  <span class="TeamTotal"  >
<asp:RadioButton ID="rdHomeTeamTotalUnder" runat="server" CssClass="chkBetActionSelect" />
<asp:CheckBox ID="chkHomeTeamTotalUnder" CssClass="chkBetActionSelect" runat="server" />
<span class="TeamTotal"  ><%#SBCBL.std.safeVegass(SBCBL.std.SafeString(Container.DataItem("HomeTeamTotalPoints")))%></span>
<asp:Literal ID="lblHomeTeamTotalPointsUnderMoney" runat="server"></asp:Literal></div>
</td>
</tr>
<tr id="trDraw" runat="server" visible="false"  class="<%#getCssClass() %>" context='<%#Container.DataItem("Context")%>'   >
<td >
</td>
<td>
<asp:Literal ID="Literal2" runat="server" Text='<%#Container.DataItem("DrawRotationNumber")%>'></asp:Literal>
</td>
<td>
Draw
</td>
<td>
</td>
<td>
</td>
<td>
<asp:TextBox ID="txtMoneyDraw" runat="server" size="3" MaxLength="5" onkeypress="javascript:return inputNumber(this,event, false);" 
AutoPostBack="false" />
<asp:RadioButton ID="rdSelectDraw" runat="server" CssClass="chkBetActionSelect"   />
<asp:CheckBox ID="chkSelectDraw" CssClass="chkBetActionSelect" runat="server"  />
<asp:Label ID="lblDrawMoney" runat="server" Width="60"  class="labelodd"/>
</td>

<td id="tdDrawLast" runat="server" colspan="3" ></td>

</tr>
<tr id="trCircle" runat="server" visible="false" context='<%#Container.DataItem("Context")%>'>
<%--<td class="game_num" style="color: Red;" align="center">
*
</td>
<td colspan="1" style="color: Red;">
Game Circled
</td>--%>
</tr>
<tr context='<%#Container.DataItem("Context")%>' id="trDescription" runat="server"   >
<td colspan="9" class="description" id="tdDescript1" runat="server">
<asp:Literal ID="lblDescription" runat="server"></asp:Literal>
</td>
</tr>
<tr context='<%#Container.DataItem("Context")%>' id="trDescription2" runat="server" class='offering_pair_<%#SBCBL.std.SafeString(ViewState("OddEven")) %>'>
<td colspan="9" class="description" id="tdDescript2" runat="server" >
<asp:Literal ID="Literal1" runat="server"></asp:Literal>
</td>
</tr>
<asp:HiddenField ID="hfInfo" runat="server" >
</asp:HiddenField>

</ItemTemplate>
</asp:Repeater>
</ItemTemplate>
</asp:Repeater>
</ItemTemplate>
</asp:Repeater>
</table>
<asp:Repeater runat="server" ID="rptProps">
<ItemTemplate>
<asp:Repeater runat="server" ID="rptPropLines" OnItemDataBound="rptPropLines_ItemDataBound">
<ItemTemplate>
<div style="clear:both"></div>
<table width="900" border="0" align="left"  cellpadding="0" cellspacing="0" style="margin:0">
<tr>
<th  colspan="4" align="left" >
<span >
<%#CType(Container.Parent.Parent, RepeaterItem).DataItem & " Proposition"%>
- Game -
<asp:Literal runat="server" ID="lblGameDate"></asp:Literal>
</th>
</tr>
<tr >
<td colspan="7" align="center" style="color:black;font-size:16px;font-weight:bolder">
<asp:Literal runat="server" ID="lblHourDate"></asp:Literal>
<asp:Literal runat="server" ID="lblPropDes"></asp:Literal>
</td>
</tr>
<asp:Repeater runat="server" ID="rptPropTeams" OnItemDataBound="rptPropTeams_ItemdataBound">
<ItemTemplate>
<tr class="offering_pair_odd">
<td colspan='3'  >
<b>
<asp:Literal ID="lblPropRotationNumber" runat="server" Text='<%#Container.DataItem("PropRotationNumber")%>'></asp:Literal></b>
<span >
<asp:Literal ID="lbnPropParticipantName" runat="server" Text='<%#Container.DataItem("PropParticipantName")%>'></asp:Literal>
</td>
<td  id="tdMLine2" runat="server" width="30%">
<asp:TextBox ID="txtMoneyPropMoneyLine" runat="server" size="3" MaxLength="5" onkeypress="javascript:return inputNumber(this,event, false);"
AutoPostBack="false" />

<asp:Label ID="lblPropMoneyLine" runat="server" CssClass="PropMoneyLine" />
<asp:HiddenField ID="hfInfoProp" runat="server" Value='<%#Container.DataItem("GameLineID") %>' >
</asp:HiddenField>

</td>
</tr>
<tr>
<td colspan="4">

</td>
</tr>
</ItemTemplate>
</asp:Repeater>
</table>
</ItemTemplate>
</asp:Repeater>
</ItemTemplate>
</asp:Repeater>


</div>
<div class="gameHeaderLoad" style="margin-top:20px;float:left;">
<div id="dvBottomIfBet" runat="server">Please select one wager from the chart below for your <b><%=IfBetOrdinal%></b> if-bet selection.</div>
<div id="dvBottomOtherBet" runat="server">
<%=getHeader().Replace("below", "above")%>
<asp:Button ID="btnContinue2" runat="server" Text="" OnClick="btnContinue_Click"  CssClass="continue" /> </div>
</div> 
<br />
</asp:Panel>
<table style="width: 100%; padding-top: 0px">
<tr>
<td>
<uc:Wagers ID="ucWagers" runat="server" Visible="false" />
</td>
</tr>
</table>
<asp:LinkButton ID="autoSubmit" runat="server" />
<input type="hidden" id="hfTeaser" value='<asp:Literal ID="lblDisableTeaser" runat="server"></asp:Literal>' />
<input type="hidden" id="hfQuarterOnly" value='<asp:Literal ID="lblQuarterOnly" runat="server"></asp:Literal>' />
</ContentTemplate>
</asp:UpdatePanel>
<%--
<input type="hidden" id='hfStraightID' value='<%=Straight.ClientID%>' />
<input type="hidden" id='hfParlayID' value='<%= Parlay.ClientID  %>' />
<input type="hidden" id='hfReverseID' value='<%= Reverse.ClientID %>' />
<input type="hidden" id='hfTeaserID' value='<%= Teaser.ClientID  %>' />
<input type="hidden" id='hfColorID' value='<%=hfColor.ClientID%>' />--%>


<script language="javascript">
    var bSubmit = false;
    var form;
    var btnSubmitID = "<%=autoSubmit.ClientID %>"
    var btnSubmit = document.getElementById(btnSubmitID);
    function SetSubmitEvent(aform) {
        debugger;
        bSubmit = true;
        form = aform;
    }
    function TextChange() {
        if (bSubmit) {
            bSubmit = false;
            // form.submit();
            debugger;
            btnSubmit.click();
            form = null;
            // document.getElementById("modal").style.display = "block";
        }
    }
</script>

<%--<script type="text/javascript" language="javascript">
//Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(StartRequestHandler);
//function StartRequestHandler(sender, args) { 
//}
//Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
//function EndRequestHandler(sender, args) {
////    try {
////        changeColor();
////        initMenuColor();
////    } catch (e) {}
//}

</script>--%>

 <script src="/Inc/Scripts/BetActionBottom.js" language="javascript" type="text/javascript"></script>




