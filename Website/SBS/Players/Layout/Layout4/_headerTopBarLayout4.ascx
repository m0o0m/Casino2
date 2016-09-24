<%@ Control Language="VB" AutoEventWireup="false" CodeFile="_headerTopBarLayout4.ascx.vb" Inherits="SBS_Players_Layout_Layout4_headerTopBarLayout4" %>
<%@ Import Namespace="SBCBL.UI" %>
<%@ Import Namespace="SBCBL" %>
<%@ Register Src="~/SBS/Shared/Layouts/Common/_menuViewMode.ascx" TagPrefix="uc1" TagName="_menuViewMode" %>


<div id="header-topbar-option-demo" class="page-header-topbar">
    <nav id="topbar" role="navigation" style="margin-bottom: 0;" data-step="3" class="navbar navbar-default navbar-static-top">
        <div class="navbar-header">
            <button type="button" data-toggle="collapse" data-target=".sidebar-collapse" class="navbar-toggle">
                <span class="sr-only">Toggle navigation</span>
                <span class="icon-bar"></span>
                <span class="icon-bar"></span>
                <span class="icon-bar"></span>
            </button>
            <a id="logo" href="/SBS/Players/PlayerAccount.aspx" class="navbar-brand">
                <span class="fa fa-rocket"></span>
                <span class="logo-text" style="display: none">Player System</span>
                <span class="logo-text-icon player-logo">
                    <img id="imgLogo" runat="server" src="/images/Winsb.png" alt="Player System" class="img-responsive" />
                </span>
            </a>
        </div>
        <div class="topbar-main">
            <a id="menu-toggle" href="#" class="hidden-xs">
                <i class="fa fa-bars"></i>
            </a>
            <ul class="nav navbar navbar-top-links navbar-right mbn">
                <li id="liBackToAgent" runat="server" class="dropdown pull-right" visible="False">
                    <a id="btnBacktoAgent" data-hover="dropdown" cssclass="dropdown-toggle" runat="server" onserverclick="btnBacktoAgent_Click">
                        <span class="hidden-xs">Back to Agent</span>
                    </a>
                </li>
                <%  Dim userSession As New CSBCSession()
                    If (userSession.UserType = EUserType.Player) Then
                %>
                <li class="dropdown">
                    <a class="dropdown-toggle" data-hover="dropdown" href="/SBS/Players/OpenBet.aspx">Open Bets</a>
                </li>
                <li class="dropdown">
                    <a class="dropdown-toggle" data-hover="dropdown" href="/SBS/Players/History.aspx">History
                    </a>
                </li>
                <li class="dropdown">
                    <a class="dropdown-toggle" data-hover="dropdown" href="/SBS/Players/PlayerAccount.aspx">Figures
                    </a>
                </li>
                <li class="dropdown">
                    <a class="dropdown-toggle" data-hover="dropdown" href="/SBS/Players/WeekBalance.aspx">Weekly Balance
                    </a>
                </li>
                <%  End If%>
                <li class="dropdown topbar-user pull-right">
                    <a data-hover="dropdown" href="#" class="dropdown-toggle">
                        <i class="fa fa-user"></i>
                        <span class="hidden-xs"><span style="color: #fff;">Welcome:</span> <asp:Literal ID="lblUser" runat="server"></asp:Literal>
                        </span>
                        &nbsp;<span class="caret"></span>
                    </a>
                    <ul class="dropdown-menu dropdown-user pull-right">
                        <li>
                            <a href="/SBS/Players/OpenBet.aspx">Open Bets
                            </a>
                        </li>
                        <li>
                            <a href="/SBS/Players/History.aspx">History
                            </a>
                        </li>
                        <li>
                            <a href="/SBS/Players/PlayerAccount.aspx">Figures
                            </a>
                        </li>
                        <li>
                            <a href="/SBS/Players/WeekBalance.aspx">Weekly Balance
                            </a>
                        </li>
                        <li>
                            <a href="/SBS/Players/Rules.aspx">Rules</a>
                        </li>
                        <li class="divider"></li>
                        <li>
                            <a href="/SBS/Players/PlayerInfo.aspx">Account Status
                            </a>
                        </li>
                        <li>
                            <a id="lbnLogout" runat="server" onserverclick="lbnLogout_Click">Logout
                            </a>
                        </li>
                    </ul>
                </li>
                <%--<uc1:_menuViewMode runat="server" ID="_menuViewMode" />--%>
            </ul>
        </div>
    </nav>
</div>
