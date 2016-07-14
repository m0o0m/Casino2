<%@ Control Language="VB" AutoEventWireup="false" CodeFile="_headerTopBarAgent.ascx.vb" Inherits="SBS_Agents_Layout_headerTopBarAgent" %>
<%@ Register Src="~/SBS/Shared/Layouts/Common/_menuViewMode.ascx" TagPrefix="uc1" TagName="_menuViewMode" %>

<div id="header-topbar-option-demo" class="page-header-topbar">
    <nav id="topbar" role="navigation" style="margin-bottom: 0;" data-step="3" class="navbar navbar-default navbar-static-top">
        <div class="navbar-header">
            <button type="button" data-toggle="collapse" data-target=".sidebar-collapse" class="navbar-toggle"><span class="sr-only">Toggle navigation</span><span class="icon-bar"></span><span class="icon-bar"></span><span class="icon-bar"></span></button>
            <a id="logo" href="/SBS/Agents/Management/PlayersReports.aspx?tab=ALL_PLAYERS" class="navbar-brand"><span class="fa fa-rocket"></span><span class="logo-text logo-cap" style="display: none">Agent System</span><span class="logo-text-icon logo-cap">Agent System</span></a>
        </div>
        <div class="topbar-main">
            <a id="menu-toggle" href="#" class="hidden-xs"><i class="fa fa-bars"></i></a>
            <ul class="nav navbar navbar-top-links navbar-right mbn">
                <li class="dropdown topbar-user pull-right">
                    <a data-hover="dropdown" href="#" class="dropdown-toggle">
                        <i class="fa fa-user"></i>
                        <span class="hidden-xs">
                            <asp:Label ID="lblLoginId" runat="server"></asp:Label>
                        </span>
                        &nbsp;<span class="caret"></span>
                    </a>
                    <ul class="dropdown-menu dropdown-user pull-right">
                        <li>
                            <asp:LinkButton ID="lbnLogout" runat="server" CssClass="LogOut2" ToolTip="LOG OUT" OnClick="lbnLogout_Click">
                                <i class="fa fa-key"></i>Log Out
                            </asp:LinkButton>
                        </li>
                    </ul>
                </li>
                <% If UserSession.AgentUserInfo.HasGameManagement Then%>
                <li class="dropdown topbar-user pull-right">
                    <a data-hover="dropdown" href="#" class="dropdown-toggle">
                        <i class="fa fa-gear"></i>
                        <span class="hidden-xs">Settings</span>
                        &nbsp;<span class="caret">
                        </span>
                    </a>
                    <ul class="dropdown-menu dropdown-user pull-right">
                        <% If UserSession.AgentUserInfo.IsEnableChangeBookmaker Then%>
                        <li>
                            <a href="/SBS/Agents/Management/Settings/BookmakerSetup.aspx">Bookmaker Setup</a>
                        </li>
                        <% End If%>
                        <li>
                            <a href="/SBS/Agents/Management/Settings/DisplayRule.aspx">Display Rule</a>
                        </li>
                        <li>
                            <a href="/SBS/Agents/Management/Settings/RiskControl.aspx">Risk Control</a>
                        </li>
                        <li>
                            <a href="/SBS/Agents/Management/Settings/ParplaySetup.aspx">Parplay Setup</a>
                        </li>
                        <li>
                            <a href="/SBS/Agents/Management/Settings/ParplayAllowanceInGames.aspx">Parplay Allowance(in Games)</a>
                        </li>
                        <li>
                            <a href="/SBS/Agents/Management/Settings/ParplayAllowanceBWGame.aspx">Parplay Allowance(b/w Games)</a>
                        </li>
                    </ul>
                </li>
                <% End If%>
                <%--<uc1:_menuViewMode runat="server" ID="_menuViewMode" />--%>
            </ul>
        </div>
    </nav>
</div>
