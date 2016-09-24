<%@ Control Language="VB" AutoEventWireup="false" CodeFile="_leftMenuAgent.ascx.vb" Inherits="SBS_Agents_Layout_leftMenuAgent" %>
<script runat="server">
    Public Function MenuActive(ByVal pageName As String) As String
        If Request.Url.AbsoluteUri.ToLower().EndsWith(pageName.ToLower()) Then
            Return "active"
        End If
        Return String.Empty
    End Function
</script>

<nav id="sidebar" role="navigation" data-step="2" data-intro="Template has &lt;b&gt;many navigation styles&lt;/b&gt;"
    data-position="right" class="navbar-default navbar-static-side">
    <div class="sidebar-collapse menu-scroll">
        <ul id="side-menu" class="nav">
            <div class="clearfix"></div>
            <li class="<%= MenuActive("PlayersReports.aspx?tab=ALL_PLAYERS")%>">
                <a href="/SBS/Agents/Management/PlayersReports.aspx?tab=ALL_PLAYERS">
                    <i class="fa fa-tachometer fa-fw">
                        <div class="icon-bg bg-orange"></div>
                    </i>
                    <span class="menu-title">Day / Week Balances</span>
                </a>
            </li>
            <li class="<%= MenuActive("OpenBets.aspx")%>">
                <a href="/SBS/Agents/OpenBets.aspx">
                    <i class="fa fa-cube fa-fw">
                        <div class="icon-bg bg-orange"></div>
                    </i>
                    <span class="menu-title">Pending Bets</span>
                </a>
            </li>
            <li class="<%= MenuActive("Default.aspx?MenuType=use")%>">
                <a href="/SBS/Agents/Default.aspx?MenuType=user">
                    <i class="fa fa-child fa-fw">
                        <div class="icon-bg bg-orange"></div>
                    </i>
                    <span class="menu-title">User Management</span>
                </a>
            </li>
            
            <% If UserSession.AgentUserInfo.ShowBetTicker Then%>
            <li class="<%= MenuActive("LiveTicker.aspx")%>">
                <a href="/SBS/Agents/LiveTicker.aspx">
                    <i class="fa fa-openid fa-fw">
                        <div class="icon-bg bg-orange"></div>
                    </i>
                    <span class="menu-title">Bet Ticker</span>
                </a>
            </li>
            <% End If%>
            
            <% If UserSession.AgentUserInfo.ShowGameScheduleScores Then%>
            <li>
                <a href="javascript:window.open('http://scores.sportsoptions.com/scores/archives.html','Game Schedule / Scores','width=900','height=600');">
                    <i class="fa fa-flag-checkered fa-fw">
                        <div class="icon-bg bg-orange"></div>
                    </i>
                    <span class="menu-title">Game Schedule / Scores</span>
                </a>
            </li>
            <% End If%>
            <% If UserSession.AgentUserInfo.ShowOddMonitor Then%>
            <li class="<%= MenuActive("OddSetting.aspx")%>">
                <a href="/SBS/Agents/Management/OddSetting.aspx">
                    <i class="fa fa-steam fa-fw">
                        <div class="icon-bg bg-orange"></div>
                    </i>
                    <span class="menu-title">Odd Monitor</span>
                </a>
            </li>
            <% End If%>
            <li class="<%= MenuActive("PlayersReports.aspx")%>">
                <a href="/SBS/Agents/Management/PlayersReports.aspx">
                    <i class="fa fa-money fa-fw">
                        <div class="icon-bg bg-orange"></div>
                    </i>
                    <span class="menu-title">Player Balance</span>
                </a>
            </li>
            <li class="<%= MenuActive("SubAgentReport.aspx")%>">
                <a href="/SBS/Agents/Management/SubAgentReport.aspx">
                    <i class="fa fa-qrcode fa-fw">
                        <div class="icon-bg bg-orange"></div>
                    </i>
                    <span class="menu-title">Sub - Agent Balance</span>
                </a>
            </li>
            <li class="<%= MenuActive("History.aspx")%>">
                <a href="/SBS/Agents/History.aspx">
                    <i class="fa fa-calendar fa-fw">
                        <div class="icon-bg bg-orange"></div>
                    </i>
                    <span class="menu-title">Agent History</span>
                </a>
            </li>
            <li class="<%= MenuActive("Transactions.aspx")%>">
                <a href="/SBS/Agents/Transactions.aspx">
                    <i class="fa fa-stumbleupon fa-fw">
                        <div class="icon-bg bg-orange"></div>
                    </i>
                    <span class="menu-title">Acct Transaction History</span>
                </a>
            </li>
            <li class="<%= MenuActive("TicketManagement.aspx")%>">
                <a href="/SBS/Agents/TicketManagement.aspx">
                    <i class="fa fa-openid fa-fw">
                        <div class="icon-bg bg-orange"></div>
                    </i>
                    <span class="menu-title">Ticket Management</span>
                </a>
            </li>
            <li class="<%= MenuActive("SelectPlayers.aspx")%>">
                <a href="/SBS/Agents/SelectPlayers.aspx">
                    <i class="fa fa-ge fa-fw">
                        <div class="icon-bg bg-orange"></div>
                    </i>
                    <span class="menu-title">Wagering for Player</span>
                </a>
            </li>
            <li class="<%= MenuActive("IPReports.aspx")%>">
                <a href="/SBS/Agents/Management/IPReports.aspx">
                    <i class="fa fa-ticket fa-fw">
                        <div class="icon-bg bg-orange"></div>
                    </i>
                    <span class="menu-title">IP Reports</span>
                </a>
            </li>
            <li class="<%= MenuActive("PLReport.aspx")%>">
                <a href="/SBS/Agents/Management/PLReport.aspx">
                    <i class="fa fa-tint fa-fw">
                        <div class="icon-bg bg-orange"></div>
                    </i>
                    <span class="menu-title">PL Reports</span>
                </a>
            </li>
            <li class="<%= MenuActive("AgentPositionReport.aspx")%>">
                <a href="/SBS/Agents/Management/AgentPositionReport.aspx">
                    <i class="fa fa-map-marker fa-fw">
                        <div class="icon-bg bg-orange"></div>
                    </i>
                    <span class="menu-title">Position Reports</span>
                </a>
            </li>
            <li class="<%= MenuActive("headcount.aspx")%>">
                <a href="/SBS/Agents/headcount.aspx">
                    <i class="fa fa-lightbulb-o fa-fw">
                        <div class="icon-bg bg-orange"></div>
                    </i>
                    <span class="menu-title">Head Count Reports</span>
                </a>
            </li>
            <li class="<%= MenuActive("VolumnReport.aspx")%>">
                <a href="/SBS/Agents/VolumnReport.aspx">
                    <i class="fa fa-lightbulb-o fa-fw">
                        <div class="icon-bg bg-orange"></div>
                    </i>
                    <span class="menu-title">Volumn Report</span>
                </a>
            </li>
            <li class="<%= MenuActive("AgentAccount.aspx")%>">
                <a href="/SBS/Agents/AgentAccount.aspx">
                    <i class="fa fa-heart-o fa-fw">
                        <div class="icon-bg bg-orange"></div>
                    </i>
                    <span class="menu-title">Acount Status</span>
                </a>
            </li>
            <li class="<%= MenuActive("ChangePassword.aspx")%>">
                <a href="/SBS/Agents/ChangePassword.aspx">
                    <i class="fa fa-key fa-fw">
                        <div class="icon-bg bg-orange"></div>
                    </i>
                    <span class="menu-title">Change Password</span>
                </a>
            </li>
            <li class="<%= MenuActive("AgentMail.aspx")%>">
                <a href="/SBS/Agents/AgentMail.aspx">
                    <i class="fa fa-inbox fa-fw">
                        <div class="icon-bg bg-orange"></div>
                    </i>
                    <span class="menu-title">Inbox Mail</span>
                </a>
            </li>

            
            
            
        </ul>
    </div>
</nav>
