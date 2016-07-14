<%@ Control Language="VB" AutoEventWireup="false" CodeFile="AgentTopMenu.ascx.vb"
    Inherits="SBCCallCenterAgents.AgentTopMenu" %>
<div class="Menu_Left">
 </div>
 <div class="Menu_Right">
 </div>
 <div class="menu_nav">
           
    <ul class="sf-menu">
        <li id="liHome" runat="server" class="border_left_menu" onmouseover="menuActive(this)" onmouseout="menuNotActive(this)">
            <asp:LinkButton ID="lbnHomepage" CssClass="nav_nonselected" CommandArgument="HOME"
                runat="server" Text="HOME" OnClick="lbnMenu_Click" />
        </li>
        <li id="liGameManager" runat="server"  class="itemMenu" onmouseover="menuActive(this)" onmouseout="menuNotActive(this)">
            <asp:LinkButton ID="lbnGameManager" CssClass="nav_nonselected" CommandArgument="GAME_MANAGER"
                runat="server" Text="GAME MANAGER" OnClick="lbnMenu_Click" />
            <ul class="gameManager" style="margin-left: -60px">
                <li><span class="firstSubmenu" id="subLinesMonitor" runat="server"  onmouseover="subMenuActive(this)" onmouseout="subMenuNotActive(this)"><a href="/SBS/CallCenter/LinesMonitor.aspx" id="lbnLinesMonitor"
                    runat="server">Lines Monitor</a></span> 
                    <span class="submenu"  id="subUpdateGameScores" runat="server"  onmouseover="subMenuActive(this)" onmouseout="subMenuNotActive(this)">
                    <a id="lbnUpdateGameScores" href="/SBS/CallCenter/UpdateGameScores.aspx" runat="server">Update Game Scores</a></span>
                    <span class="submenu" id="subSetupQuarter" runat="server"  onmouseover="subMenuActive(this)" onmouseout="subMenuNotActive(this)"><a id="lbnSetupQuarter" href="/SBS/CallCenter/SetupQuarter.aspx"
                        runat="server">Setup Quarter Lines</a></span>
                     <span class="submenu" id="subUpdateQuarterScores" runat="server"  onmouseover="subMenuActive(this)" onmouseout="subMenuNotActive(this)"><a id="lbnUpdateQuarterScores"
                            href="/SBS/CallCenter/UpdateQuarterScores.aspx" runat="server">Update Quarter Scores</a></span>
                </li>
            </ul>
        </li>
        <li id="liPending" runat="server"  class="itemMenu" onmouseover="menuActive(this)" onmouseout="menuNotActive(this)">
            <asp:LinkButton ID="lbnOpenBet" CssClass="nav_nonselected" CommandArgument="OPEN_BET"
                runat="server" Text="PENDING" OnClick="lbnMenu_Click" />
        </li>
        <li  id="liHistory" runat="server"  class="itemMenu" onmouseover="menuActive(this)" onmouseout="menuNotActive(this)">
            <asp:LinkButton ID="lbnHistory" CssClass="nav_nonselected" CommandArgument="HISTORY"
                runat="server" Text="HISTORY" OnClick="lbnMenu_Click" />
        </li>
        <li id="liAccountInfo" runat="server"  class="itemMenu" onmouseover="menuActive(this)" onmouseout="menuNotActive(this)">
            <asp:LinkButton ID="lbnAccountStatus" CssClass="nav_nonselected" CommandArgument="ACCOUNT_STATUS"
                runat="server" Text="ACCOUNT INFO" OnClick="lbnMenu_Click" />
        </li>
    </ul>
</div>
