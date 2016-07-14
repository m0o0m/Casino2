<%@ Control Language="VB" AutoEventWireup="false" CodeFile="_menuViewMode.ascx.vb" Inherits="SBS_Shared_Layouts_Common_menuViewMode" %>
<li id="liMode" class="dropdown topbar-user pull-right" style="display: none">
    <a data-hover="dropdown" href="#" class="dropdown-toggle">
        <i class="fa fa-desktop"></i>
        <span class="hidden-xs">View Mode</span>&nbsp;<span class="caret"></span>
    </a>
    <ul class="dropdown-menu dropdown-user pull-right">
        <li><a onclick="desktopMode();"><i class="fa fa-desktop"></i>Desktop Mode</a></li>
        <li><a onclick="responsiveMode();"><i class="fa fa-mobile-phone"></i>Responsive Mode</a></li>
    </ul>
</li>
