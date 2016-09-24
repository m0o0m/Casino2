<%@ Control Language="VB" AutoEventWireup="false" CodeFile="_headerTopBarLayout6.ascx.vb" Inherits="SBS_Players_Layout_Layout6_headerTopBarLayout6" %>
<%@ Import Namespace="SBCBL.UI" %>
<%@ Import Namespace="SBCBL" %>
<%@ Register Src="~/SBS/Shared/Layouts/Common/_menuViewMode.ascx" TagPrefix="uc1" TagName="_menuViewMode" %>


<div class="header">
    <div class="logo-account-info-area clear pdB20">
        <div class="logo-main h102px left mgT10">
            <a class="block full" href="#">
                <img id="imgLogo" runat="server" src="/images/Winsb.png" alt="Player System"/>
            </a>
        </div>
        <div class="w360px right clear mgT10">
            <div class="user-info-balance-area left">
                <div class="info-balance-name baseline mgB20">
                    <label>User ID:</label>
                    <div class="inlineb fz18 bold clr-gray-2">
                        <asp:Literal ID="lblUser" runat="server"></asp:Literal>
                    </div>
                </div>
                <div class="user-info-balance">
                    <div class="info-balance-itm baseline">
                        <label>Current Balance:</label>
                        <a href="/SBS/Players/WeekBalance.aspx" class="ft-secondary fz13 bold clr-white nonunderline">
                            <asp:Label ID="lblThisWeek" runat="server" />
                        </a>
                    </div>
                    <div class="info-balance-itm baseline mgT5">
                        <label>Available:</label>
                        <a href="/SBS/Players/PlayerAccount.aspx" class="ft-secondary fz13 bold clr-white nonunderline">
                            <asp:Label ID="lblAvailableBalance" runat="server" />
                        </a>
                    </div>
                    <div class="info-balance-itm baseline mgT5">
                        <label>Pending:</label>
                        <a href="/SBS/Players/OpenBet.aspx" class="ft-secondary fz13 bold clr-white nonunderline">
                            <asp:Label ID="lblPendingAmount" runat="server" />
                        </a>
                    </div>
                </div>
            </div>
            <div class="right">
                <button class="button-style-1 w100px" id="lbnLogout" runat="server" onserverclick="lbnLogout_Click">Log Off</button>
            </div>
        </div>
    </div>
</div>
