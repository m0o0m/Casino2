<%@ Control Language="VB" AutoEventWireup="false" CodeFile="TopMenu.ascx.vb" Inherits="SBCAgents.Agents_Inc_TopMenu" %>

<script type="text/javascript" language="javascript">
    function btnActive(me) {
        $(me).removeClass("btnGame").addClass("btnGameActive")
    }
    function btnNoActive(me) {
        $(me).removeClass("btnGameActive").addClass("btnGame")
    }
</script>

    <a href="/SBS/Agents/Management/PlayersReports.aspx?tab=ALL_PLAYERS" id="A1" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)" class="btnGame" style="color:White">Home</a>
    <a href="/SBS/Agents/Default.aspx?MenuType=user" id="pnUserManagement" runat="server" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)" class="btnGame" style="color:White"> User Management</a>
    <a href="/SBS/Agents/SelectPlayers.aspx" id="A2" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)" class="btnGame" style="color:White">Wager</a>
        <a href="/SBS/Agents/Default.aspx?MenuType=game" id="pnGameManagement" runat="server" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)" class="btnGame" style="color:White">Games Management</a>
    <a href="/SBS/Agents/Default.aspx?MenuType=report" id="A3" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)" class="btnGame" style="color:White">Reports</a>
    <a href="/SBS/Agents/Default.aspx?MenuType=account" id="A4" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)" class="btnGame" style="color:White">Account Info</a>
   
    <a href="/SBS/Agents/Default.aspx?MenuType=sys" id="A5" style="display:none" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)" class="btnGame" style="color:White">System Management</a>



        <!--

                    <span class="btn_agent"  onmouseover="subMenuActive(this)" onmouseout="subMenuNotActive(this)" ID="subPending" runat="server"><a href="/SBS/Agents/OpenBets.aspx" ID="lbnPending"  runat="server"  >Pending Bet</a></span>
                    <span class="btn_agent"  onmouseover="subMenuActive(this)" onmouseout="subMenuNotActive(this)" ID="subHistory" runat="server"><a ID="lbnHistory" href="/SBS/Agents/History.aspx" runat="server" > History Bet</a></span>
                    <span class="btn_agent"   onmouseover="subMenuActive(this)" onmouseout="subMenuNotActive(this)" id="subTransaction" runat="server" ><a ID="lbnTransaction" href="/SBS/Agents/Transactions.aspx" runat="server"  > Transaction</a></span>
  
  
      
                    <span class="btn_agent"  onmouseover="subMenuActive(this)" onmouseout="subMenuNotActive(this)"  ID="subPlayers" runat="server"><a href="/SBS/Agents/Management/Players.aspx" ID="lbnPlayers"  runat="server" >Player</a></span>
                    <span class="btn_agent"  onmouseover="subMenuActive(this)" onmouseout="subMenuNotActive(this)" ID="subAgents"  runat="server"><a ID="lbnAgents" href="/SBS/Agents/Management/Agents.aspx"  runat="server" > Sub - Agent</a></span>
   
    
                    <span class="btn_agent" onmouseover="subMenuActive(this)" onmouseout="subMenuNotActive(this)" ID="subGamesOdds" runat="server" ><a href="/SBS/Agents/Management/OddSetting.aspx" ID="lbnGamesOdds"  runat="server" >Odds</a></span>
                    <span class="btn_agent"  onmouseover="subMenuActive(this)" onmouseout="subMenuNotActive(this)" ID="subGamesSettings" runat="server"><a ID="lbnGamesSettings" href="/SBS/Agents/Management/AgentSetting.aspx"  runat="server"> Settings</a></span>
                     <span class="btn_agent"  onmouseover="subMenuActive(this)" onmouseout="subMenuNotActive(this)" ID="subGameManual" runat="server"><a ID="lbnGameManual" href="/SBS/Agents/Management/GameManual.aspx"  runat="server"> Quarter Line Setup</a></span>
 -->
        <asp:LinkButton ID="lbnWager" CssClass="btn_agent" style="display:none" Width="118" Height="23" CommandArgument="WAGER"
            runat="server" Text="WAGER" OnClick="lbnMenu_Click"></asp:LinkButton>

                 <%--   <span class="btn_agent"  onmouseover="subMenuActive(this)" onmouseout="subMenuNotActive(this)"><a href="/SBS/Agents/SelectGame.aspx" ID="lbtStraight"  onclick="try{lbtStraight_Click()}catch(e){}" >Straight Bet</a></span>
                    <span id="spParlay"  class="btn_agent"  onmouseover="subMenuActive(this)" onmouseout="subMenuNotActive(this)"><a ID="lbtParlay" href="/SBS/Agents/SelectGame.aspx" onclick="try{lbtParlay_Click()}catch(e){}" >Parlays</a></span>
                    <span id="spReverse" class="btn_agent"  onmouseover="subMenuActive(this)" onmouseout="subMenuNotActive(this)"><a ID="lbtReverse" href="/SBS/Agents/SelectGame.aspx" onclick="try{lbtReverse_Click()}catch(e){}" >Action Reverse</a></span>
                    <span id="spTeaser" class="btn_agent"  onmouseover="subMenuActive(this)" onmouseout="subMenuNotActive(this)"><a ID="lbtTeaser" href="/SBS/Agents/SelectGame.aspx" onclick="try{lbtTeaser_Click()}catch(e){}">Teasers</a></span>
  

  
                  <span class="btn_agent"  onmouseover="subMenuActive(this)" onmouseout="subMenuNotActive(this)" id="subPlayerReport" runat="server" ><a id="lbnPlayerReport" href="/SBS/Agents/Management/PlayersReports.aspx" runat="server" > Player Balance</a></span>
                  <span class="btn_agent"  onmouseover="subMenuActive(this)" onmouseout="subMenuNotActive(this)" id="subAgentReport" runat="server"><a id="lbnAgentReport" href="/SBS/Agents/Management/SubAgentReport.aspx" runat="server" >Sub - Agent Balance</a></span>
                   <span class="btn_agent"  onmouseover="subMenuActive(this)" onmouseout="subMenuNotActive(this)" id="subPositionReport"  runat="server" ><a id="lbnPositionReport" href="/SBS/Agents/Management/AgentPositionReport.aspx" runat="server" > Position</a></span>
                  <span class="btn_agent"  onmouseover="subMenuActive(this)" onmouseout="subMenuNotActive(this)" id="subPLReport" runat="server" ><a id="lbnPLReport" href="/SBS/Agents/Management/PLReport.aspx" runat="server" >Profit Report</a></span>
                  <span class="btn_agent"  onmouseover="subMenuActive(this)" onmouseout="subMenuNotActive(this)" id="subIPReport" runat="server" ><a id="lbnIPReport" href="/SBS/Agents/Management/IPReports.aspx" runat="server" >IP Report</a></span>
                  <span class="btn_agent"  onmouseover="subMenuActive(this)" onmouseout="subMenuNotActive(this)" id="subIPAlert"  runat="server" ><a id="lbnIPAlert" href="/SBS/Agents/Management/IPAlert.aspx" runat="server" >IP Alert</a></span>

  
     
        
                  <span class="btn_agent"  onmouseover="subMenuActive(this)" onmouseout="subMenuNotActive(this)" id="subPlayerAccount" runat="server"><a id="lbtPlayerAccount" href="/SBS/Agents/AgentAccount.aspx" runat="server" > Account Status</a></span>
                  <span class="btn_agent"  onmouseover="subMenuActive(this)" onmouseout="subMenuNotActive(this)" id="subChangePassword" runat="server" ><a id="lbtChangePassword" href="/SBS/Agents/ChangePassword.aspx" runat="server" >Change Password</a></span>
                  <span class="btn_agent"  onmouseover="subMenuActive(this)" onmouseout="subMenuNotActive(this)" id="subMailInbox" runat="server"><a id="lbnMailInbox" href="/SBS/Agents/AgentMail.aspx" runat="server" >Inbox Mail</a></span>

      
                    <span class="btn_agent"  onmouseover="subMenuActive(this)" onmouseout="subMenuNotActive(this)" id="subConfigLogo" runat="server"><a id="lbnConfigLogo" href="/SBS/Agents/Management/ConfigureLogo.aspx" runat="server" >Upload Image </a></span>
                      <span class="btn_agent"  onmouseover="subMenuActive(this)" onmouseout="subMenuNotActive(this)" id="subSiteOption" runat="server" ><a id="lbnSiteOption" href="/SBS/Agents/Management/SiteOption.aspx" runat="server" >Site Options</a></span>

 --%>
