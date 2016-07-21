<%@ Control Language="VB" AutoEventWireup="false" CodeFile="_mainMenuLayout6.ascx.vb" Inherits="SBS_Players_Layout_Layout6_mainMenuLayout6" %>


<script runat="server">
    Public Function MenuActive(ByVal pageNames As String()) As String
        if pageNames.Any(Function(pageName) Request.Url.AbsoluteUri.ToLower().EndsWith(pageName.ToLower())) Then
            Return "active"
        End If
        Return String.Empty
    End Function


    Public Function SubMenuActive(ByVal pageName As String) As String
        If Request.Url.AbsoluteUri.ToLower().EndsWith(pageName.ToLower()) Then
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
            </li>
            <li><a href="#">Casino</a></li>
            <li><a href="#">Account</a></li>
            <li><a href="#">Pending</a></li>
            <li><a href="#">Scores</a></li>
            <li><a href="#">Rules</a></li>
        </ul>
    </div>
    <div class="sub-menu-main-search-area pdB8 clear">
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
        <div class="w260px right pdT8">
            <div class="frm-search-1">
                <input type="text" name="" value="" placeholder="Search..." />
                <button>
                    <img class="h13px" src="../images/icons/Search-icon.png" />
                </button>
            </div>
        </div>
    </div>
</div>
