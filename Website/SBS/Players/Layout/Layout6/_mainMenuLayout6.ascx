<%@ Control Language="VB" AutoEventWireup="false" CodeFile="_mainMenuLayout6.ascx.vb" Inherits="SBS_Players_Layout_Layout6_mainMenuLayout6" %>


<script runat="server">
    Public Function MenuActive(ByVal pageNames As String()) As String
        if pageNames.Any(Function(pageName) Request.Url.AbsoluteUri.ToLower().Contains(pageName.ToLower())) Then
            Return "active"
        End If
        Return String.Empty
    End Function


    Public Function SubMenuActive(ByVal pageName As String) As String
        If Request.Url.AbsoluteUri.ToLower().Contains(pageName.ToLower()) Then
            Return "current"
        End If
        Return String.Empty
    End Function
</script>


<div class="navigation-area">
    <div class="main-menu-area">
        <ul class="main-menu menu-style-1 clear pdL10">
            <li class="<%= MenuActive({"Default.aspx?bettype=BetIfAll", "Default.aspx?bettype=Parlay", "Default.aspx?bettype=Teaser", "Default.aspx?bettype=Prop"}) %>">
                <a href="#">Sports</a>
                <ul class="sub-menu menu-style-2 clear pdL10 left pdT12">
                    <li class="<%= SubMenuActive("Default.aspx?bettype=BetIfAll")%>">
                        <a href="Default.aspx?bettype=BetIfAll">Straights</a>
                    </li>
                    <li class="<%= SubMenuActive("Default.aspx?bettype=Parlay")%>">
                        <a href="Default.aspx?bettype=Parlay">Parlays</a>
                    </li>
                    <li class="<%= SubMenuActive("Default.aspx?bettype=Teaser")%>">
                        <a href="Default.aspx?bettype=Teaser">Teasers</a>
                    </li>
                    <li class="">
                        <a href="#">If Bets / Reverses</a>
                    </li>
                    <li class="<%= SubMenuActive("Default.aspx?bettype=Prop")%>">
                        <a href="Default.aspx?bettype=Prop">Future/ Props</a>
                    </li>
                </ul>
            </li>
            <li><a href="#">Casino</a></li>
            <li class="<%= MenuActive({"PlayerAccount.aspx", "WeekBalance.aspx", "History.aspx", "Setting.aspx"}) %>">
                <a href="WeekBalance.aspx">Account</a>
                <ul class="sub-menu menu-style-2 clear pdL10 left pdT12">
                    <li class="<%= SubMenuActive("PlayerAccount.aspx")%>"><a href="PlayerAccount.aspx">Account</a></li>
                    <li class="<%= SubMenuActive("WeekBalance.aspx")%>"><a href="WeekBalance.aspx">Daily Figure</a></li>
                    <li class=""><a href="#">Account History</a></li>
                    <li class="<%= SubMenuActive("History.aspx")%>"><a href="History.aspx">Wager History</a></li>
                    <li class=""><a href="#">Settings</a></li>
                </ul>
            </li>
            <li class="<%= MenuActive({"OpenBet.aspx"})%>"><a href="OpenBet.aspx">Pending</a></li>
            <li><a href="#">Scores</a></li>
            <li><a href="#">Rules</a></li>
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
