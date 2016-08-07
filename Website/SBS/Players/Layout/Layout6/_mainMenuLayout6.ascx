<%@ Control Language="VB" AutoEventWireup="false" CodeFile="_mainMenuLayout6.ascx.vb" Inherits="SBS_Players_Layout_Layout6_mainMenuLayout6" %>


<script runat="server">
    Public Function MenuActive(ByVal pageName As String) As String
        If Me.Page.CurrentPageName.ToLower().Contains(pageName.ToLower()) Then
            Return "active"
        End If
        Return String.Empty
    End Function

    Public Function SubMenuActive(ByVal pageName As String) As String
        If Me.Page.CurrentPageName.ToLower().Equals(pageName.ToLower()) Then
            Return "current"
        End If
        Return String.Empty
    End Function
</script>


<div class="navigation-area">
    <div class="main-menu-area">
        <ul class="main-menu menu-style-1 clear pdL10">
            <li class="<%= MenuActive("Sport") %>">
                <a href="Default.aspx?bettype=BetIfAll">Sports</a>
                <ul class="sub-menu menu-style-2 clear pdL10 left pdT12">
                    <li class="<%= SubMenuActive("Sport_Straight")%>">
                        <a href="Default.aspx?bettype=BetIfAll">Straights</a>
                    </li>
                    <li class="<%= SubMenuActive("Sport_Parlay")%>">
                        <a href="Default.aspx?bettype=Parlay">Parlays</a>
                    </li>
                    <li class="<%= SubMenuActive("Sport_Teaser")%>">
                        <a href="Default.aspx?bettype=Teaser">Teasers</a>
                    </li>
                    <li class="<%= SubMenuActive("Sport_IfBet")%>">
                        <a href="#">If Bets / Reverses</a>
                    </li>
                    <li class="<%= SubMenuActive("Sport_Prop")%>">
                        <a href="Default.aspx?bettype=Prop">Future/ Props</a>
                    </li>
                </ul>
            </li>
            <li class="<%= MenuActive("Casino") %>"><a href="/SBS/Players/Casino.aspx?bettype=casino">Casino</a></li>
            <li class="<%= MenuActive("Account") %>">
                <a href="WeekBalance.aspx">Account</a>
                <ul class="sub-menu menu-style-2 clear pdL10 left pdT12">
                    <li class="<%= SubMenuActive("Account_Account")%>"><a href="PlayerAccount.aspx">Account</a></li>
                    <li class="<%= SubMenuActive("Account_DailyFigure")%>"><a href="WeekBalance.aspx">Daily Figure</a></li>
                    <li class=""><a href="#">Account History</a></li>
                    <li class="<%= SubMenuActive("Account_WagerHistory")%>"><a href="History.aspx">Wager History</a></li>
                    <li class=""><a href="#">Settings</a></li>
                </ul>
            </li>
            <li class="<%= MenuActive("Pending")%>"><a href="OpenBet.aspx">Pending</a></li>
            <li><a href="http://scores.sportsoptions.com/scores/today.html" target="_blank">Scores</a></li>
            <li class="<%= MenuActive("Rule")%>"><a href="/SBS/Players/Rules.aspx">Rules</a></li>
        </ul>
    </div>

    <div class="sub-menu-main-search-area pdB8 clear">
        <div class="w260px right pdT8">
            <div class="frm-search-1">
                <input type="text" name="" value="" placeholder="Search..." />
                <button>
                    <img class="h13px" src="/Content/themes/agent/layout6/images/icons/Search-icon.png" />
                </button>
            </div>
        </div>
    </div>
</div>
