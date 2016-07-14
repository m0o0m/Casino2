<%@ Control Language="VB" AutoEventWireup="false" CodeFile="TopMenu.ascx.vb" Inherits="Players.TopMenu" %>
  <div class="Menu_Left">
 </div>
 <div class="Menu_Right">
 </div>
 <div class="menu_nav">
 <ul class="sf-menu">
   <li id="liWager" runat="server" class="border_left_menu" onmouseover="menuActive(this)" onmouseout="menuNotActive(this)">
        <asp:LinkButton ID="lbnWager" CssClass="nav_nonselected" CommandArgument="WAGER"
            runat="server" Text="WAGER" OnClick="lbnMenu_Click"></asp:LinkButton>
          <ul class="wager" style="display:none;margin-left:-30px">
            <li> 
                    <span class="firstSubmenu" onmouseover="subMenuActive(this)" onmouseout="subMenuNotActive(this)"><a href="default.aspx"  ID="lbtStraight" onclick="try{lbtStraight_Click()}catch(e){}" >Straight Bet</a></span>
                    <span id="spParlay" class="submenu" onmouseover="subMenuActive(this)" onmouseout="subMenuNotActive(this)"><a ID="lbtParlay" href="default.aspx" onclick="try{lbtParlay_Click()}catch(e){}" >Parlays</a></span>
                    <span id="spReverse" class="submenu" onmouseover="subMenuActive(this)" onmouseout="subMenuNotActive(this)"><a ID="lbtReverse" href="default.aspx" onclick="try{lbtReverse_Click()}catch(e){}" >Action Reverse</a></span>
                    <span id="spTeaser" class="submenu" onmouseover="subMenuActive(this)" onmouseout="subMenuNotActive(this)"><a ID="lbtTeaser" href="default.aspx" onclick="try{lbtTeaser_Click()}catch(e){}">Teasers</a></span>
              </li>
           </ul>    
     </li>
     <li id="liAccStatus" runat="server" class ="itemMenu"  onmouseover="menuActive(this)" onmouseout="menuNotActive(this)">
        <asp:LinkButton ID="lbnAccStatus" CssClass="nav_nonselected" CommandArgument="ACCSTATUS" 
            runat="server" Text="ACCOUNT INFO" OnClick="lbnMenu_Click"></asp:LinkButton>
          <ul class="accountInfo" style="display:none;margin-left:-100px">
            <li>
                  <span class="firstSubmenu" onmouseover="subMenuActive(this)" onmouseout="subMenuNotActive(this)" id="subPlayerAccount" runat="server"><a id="lbtPlayerAccount" href="/SBS/Players/PlayerAccount.aspx" runat="server" > Account Status</a></span>
                  <span class="submenu" onmouseover="subMenuActive(this)" onmouseout="subMenuNotActive(this)" id="subChangePassword"  runat="server" ><a id="lbtChangePassword" href="/SBS/Players/ChangePassword.aspx" runat="server" >Change Password</a></span>
                  <span class="submenu" onmouseover="subMenuActive(this)" onmouseout="subMenuNotActive(this)" id="subMailInbox" runat="server"><a id="lbnMailInbox" href="/SBS/Players/playerEmail.aspx" runat="server" >Inbox Mail</a></span>
             </li>
           </ul> 
     </li>
 <%--   <li>
        <asp:LinkButton ID="LinkButton1" CssClass="nav_nonselected" CommandArgument="TRANSACTIONS"
            runat="server" Text="TRANSACTIONS" OnClick="lbnMenu_Click"></asp:LinkButton></li>--%>
    <li id="liOpenBet" runat="server" class ="itemMenu"  onmouseover="menuActive(this)" onmouseout="menuNotActive(this)">
        <asp:LinkButton ID="lbnOpenBet" CssClass="nav_nonselected" CommandArgument="OPENBET" 
            runat="server" Text="PENDING"></asp:LinkButton>
    </li>
<%--    <li>
        <asp:LinkButton ID="lbnBetAction" CssClass="nav_nonselected" CommandArgument="BETACTION"
            runat="server" Text="WEEKLY FIGURES"></asp:LinkButton></li>--%>
    <li id="liBalance" runat="server"  class ="itemMenu" onmouseover="menuActive(this)" onmouseout="menuNotActive(this)">
        <asp:LinkButton ID="lblBalance" CssClass="nav_nonselected" CommandArgument="BALANCE" 
            runat="server" Text="WEEKLY FIGURES"></asp:LinkButton>
    </li>
    <li  id="liHistory" runat="server" class ="itemMenu" onmouseover="menuActive(this)" onmouseout="menuNotActive(this)">
        <asp:LinkButton ID="lbnHistory" CssClass="nav_nonselected" CommandArgument="HISTORY" 
            runat="server" Text="HISTORY"></asp:LinkButton>
    </li>
    <li  id="liCasino" runat="server" class ="itemMenu" onmouseover="menuActive(this)" onmouseout="menuNotActive(this)">
        <asp:LinkButton ID="lbnCasino" CssClass="nav_nonselected" CommandArgument="CASINO" 
            runat="server" Text="CASINO"></asp:LinkButton>
    </li>
</ul>
</div>