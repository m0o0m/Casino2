<%@ Control Language="VB" AutoEventWireup="false" CodeFile="_headerTopBarLayout6.ascx.vb" Inherits="SBS_Players_Layout_Layout6_headerTopBarLayout6" %>
<%@ Import Namespace="SBCBL.UI" %>
<%@ Import Namespace="SBCBL" %>
<%@ Register Src="~/SBS/Shared/Layouts/Common/_menuViewMode.ascx" TagPrefix="uc1" TagName="_menuViewMode" %>


<div class="header">
    <div class="logo-account-info-area clear pdB20">
        <div class="logo-main h102px left mgT10">
            <a class="block full" href="#">
                <img id="imgLogo" runat="server" src="/images/Winsb.png" alt="Player System" class="full" />
            </a>
        </div>
        <div class="w360px right clear mgT10">
            <div class="user-info-balance-area left">
                <div class="info-balance-name baseline mgB20">
                    <label>User ID:</label>
                    <span><asp:Literal ID="lblUser" runat="server"></asp:Literal></span>
                </div>
                <div class="user-info-balance">
                    <div class="info-balance-itm baseline">
                        <label>Current Balance:</label>
                        <span><asp:Label ID="lblThisWeek" runat="server" /></span>
                    </div>
                    <div class="info-balance-itm baseline mgT5">
                        <label>Available:</label>
                        <span><asp:Label ID="lblAvailableBalance" runat="server" /></span>
                    </div>
                    <div class="info-balance-itm baseline mgT5">
                        <label>Pending:</label>
                        <span><asp:Label ID="lblPendingAmount" runat="server" /></span>
                    </div>
                </div>
            </div>
            <div class="right">
                <button class="button-style-1 w100px" id="lbnLogout" runat="server" onserverclick="lbnLogout_Click">Log Off</button>
            </div>
        </div>
    </div>
</div>
