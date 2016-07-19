<%@ Control Language="VB" AutoEventWireup="false" CodeFile="_mainMenuLayout4.ascx.vb" Inherits="SBS_Players_Layout_Layout4_mainMenuLayout4" %>
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
            <li class="<%= MenuActive("Default.aspx?bettype=BetIfAll")%>">
                <a href="Default.aspx?bettype=BetIfAll">
                    <i class="fa fa-tachometer fa-fw">
                        <div class="icon-bg bg-orange"></div>
                    </i>
                    <span class="menu-title"></span>
                    <span class="menu-title">Bet The Board</span>
                </a>
            </li>
            <%--<li class="<%= MenuActive("Default.aspx?bettype=BetTheBoard")%>">
                <a href="Default.aspx?bettype=BetTheBoard">
                    <i class="fa fa-tachometer fa-fw">
                        <div class="icon-bg bg-orange"></div>
                    </i>
                    <span class="menu-title">Straight</span>
                </a>
            </li>--%>
            <li class="<%= MenuActive("Default.aspx?bettype=Parlay")%>">
                <a href="Default.aspx?bettype=Parlay">
                    <i class="fa fa-tachometer fa-fw">
                        <div class="icon-bg bg-orange"></div>
                    </i>
                    <span class="menu-title">Parlay</span>
                </a>
            </li>
            <li class="<%= MenuActive("Default.aspx?bettype=Teaser")%>">
                <a href="Default.aspx?bettype=Teaser">
                    <i class="fa fa-tachometer fa-fw">
                        <div class="icon-bg bg-orange"></div>
                    </i>
                    <span class="menu-title">Teaser</span>
                </a>
            </li>
            <li class="<%= MenuActive("Default.aspx?bettype=Reverse")%>">
                <a href="Default.aspx?bettype=Reverse">
                    <i class="fa fa-tachometer fa-fw">
                        <div class="icon-bg bg-orange"></div>
                    </i>
                    <span class="menu-title">Action Reverse</span>
                </a>
            </li>
            <li class="<%= MenuActive("Default.aspx?bettype=Prop")%>">
                <a href="Default.aspx?bettype=Prop">
                    <i class="fa fa-tachometer fa-fw">
                        <div class="icon-bg bg-orange"></div>
                    </i>
                    <span class="menu-title">Futures / Props</span>
                </a>
            </li>
            <%--<li class="<%= MenuActive("Casino.aspx?bettype=casino")%>">
                <a href="Casino.aspx?bettype=casino">
                    <i class="fa fa-tachometer fa-fw">
                        <div class="icon-bg bg-orange"></div>
                    </i>
                    <span class="menu-title">Casino</span>
                </a>
            </li>--%>
            <li class="<%= MenuActive(Server.HtmlEncode("Default.aspx?bettype=If%20Win%20or%20Push"))%>">
                <a href="Default.aspx?bettype=If Win or Push">
                    <i class="fa fa-tachometer fa-fw">
                        <div class="icon-bg bg-orange"></div>
                    </i>
                    <span class="menu-title">If Win or Push</span>
                </a>
            </li>
            <li class="<%= MenuActive(Server.HtmlEncode("Default.aspx?bettype=If%20Win"))%>">
                <a href="Default.aspx?bettype=If Win">
                    <i class="fa fa-tachometer fa-fw">
                        <div class="icon-bg bg-orange"></div>
                    </i>
                    <span class="menu-title">If Win Only</span>
                </a>
            </li>
        </ul>
    </div>
</nav>
