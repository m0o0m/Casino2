<%@ Control Language="VB" AutoEventWireup="false" CodeFile="accountStatus.ascx.vb"
    Inherits="SBCPlayer.accountStatus" %>

<div class="row">
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

<div class="row mt20">
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
                                <asp:Label ID="lblCurrentBalance" runat="server" ForeColor="Blue" />
                            </td>
                        </tr>
                        <tr>
                            <td>Amount at Risk
                            </td>
                            <td>
                                <asp:Label ID="lblPendingAmount" runat="server" ForeColor="Blue" />
                            </td>
                        </tr>
                        <tr>
                            <td>Available Balance
                            </td>
                            <td>
                                <asp:Label ID="lblAvailableBalance" runat="server" ForeColor="Blue" />
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
                                <asp:Label ID="lblLastWeek"  runat="server" ForeColor="Blue" />
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>

    </div>
</div>
