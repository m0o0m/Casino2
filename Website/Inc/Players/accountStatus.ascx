<%@ Control Language="VB" AutoEventWireup="false" CodeFile="accountStatus.ascx.vb"
    Inherits="SBCPlayer.accountStatus" %>

<link href="../../Content/themes/agent/layout6/styles/_toan-linhtinh.css" rel="stylesheet" />

<div>
    <h2 class="title-style-1 pdB10">Account Balance</h2>
    <table class="table-style-3 ly-w-1:2 center">
        <tr>
            <td class="caption ly-w-1:2">
                <div class="baseline">
                    <img src="../../Content/themes/agent/layout6/images/HmCrrnt.png" />
                    <span>Current</span>
                </div>
            </td>
            <td class="ly-w-1:2">
                <a href="/SBS/Players/WeekBalance.aspx">$<asp:Label ID="lblCurrentBalance" runat="server" /></a>
            </td>
        </tr>
        <tr>
            <td class="caption ly-w-1:2">
                <div class="baseline">
                    <img src="../../Content/themes/agent/layout6/images/HmAvbl.png" />
                    <span>Available</span>
                </div>
            </td>
            <td class="ly-w-1:2">$<asp:Label ID="lblAvailableBalance" runat="server" /></td>
        </tr>
        <tr>
            <td class="caption ly-w-1:2">
                <div class="baseline">
                    <img src="../../Content/themes/agent/layout6/images/HmPnd.png" />
                    <span>Pending</span>
                </div>
            </td>
            <td class="ly-w-1:2">
                <a href="/SBS/Players/OpenBet.aspx">$<asp:Label ID="lblPendingAmount" runat="server" /></a>
            </td>
        </tr>
    </table>
</div>


<div class="row" style="display: none">
    <div class="col-lg-12">
        <div class="page-title-breadcrumb">
            <div class="page-header pull-left">
                <div class="page-title pull-left mrm">
                    Section
                </div>
                <span class="label label-grey pull-left" style="font-size: large">Figures</span>
            </div>
            <ol class="breadcrumb page-breadcrumb pull-right">
                <li style="font-size: 17px;">Acount :
                    <asp:Literal ID="lblAcct" runat="server"></asp:Literal></li>
            </ol>
            <div class="clearfix">
            </div>
        </div>
    </div>
</div>

<div class="row mt20" style="display: none">
    <div class="col-lg-12">
        <div class="panel panel-grey">
            <div class="panel-heading">Figures</div>
            <div class="panel-body">
                <table class="table table-hover table-bordered table-striped">
                    <thead>
                        <tr>
                            <th></th>
                            <th>Amount</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>Original Balance
                            </td>
                            <td>
                                <%--<asp:Label ID="lblCurrentBalance" runat="server" ForeColor="Blue" />--%>
                            </td>
                        </tr>
                        <tr>
                            <td>Amount at Risk
                            </td>
                            <td>
                                <%--<asp:Label ID="lblPendingAmount" runat="server" ForeColor="Blue" />--%>
                            </td>
                        </tr>
                        <tr>
                            <td>Available Balance
                            </td>
                            <td>
                                <%--<asp:Label ID="lblAvailableBalance" runat="server" ForeColor="Blue" />--%>
                            </td>
                        </tr>
                        <tr>
                            <td>This Week`s Win/Loss Figure:
                            </td>
                            <td>
                                <asp:Label ID="lblThisWeek" runat="server" ForeColor="Blue" />
                            </td>
                        </tr>
                        <tr>
                            <td>Last Week`s Win/Loss Figure:
                            </td>
                            <td>
                                <asp:Label ID="lblLastWeek" runat="server" ForeColor="Blue" />
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>

    </div>
</div>
