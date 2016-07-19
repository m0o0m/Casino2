<%@ Control Language="VB" AutoEventWireup="false" CodeFile="HeadCount.ascx.vb" Inherits="SBCWebsite.HeadCount" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>

<div class="panel panel-grey">
    <div class="panel-heading">Head Count Report</div>
    <div class="panel-body">

        <div class="form-group">
            <label class="col-md-2 w140 control-label">Weekly Ending</label>
            <div class="col-md-3">
                <wlb:CDropDownList ID="ddlWeeks" runat="server" CssClass="form-control" hasOptionalItem="false"
                    AutoPostBack="false" Width="220px" />
            </div>
            <div class="col-md-2">
                <asp:Button ID="btnSeach" runat="server" Text="View head count" class="btn btn-warning" />
            </div>
        </div>
    </div>
</div>

<div class="panel panel-grey">
    <div class="panel-heading"></div>
    <div class="panel-body">
        <table class="table table-hover table-bordered table-striped">
            <thead>
                <tr>
                    <th>Agent</th>
                    <th>Internet HC</th>
                    <th>Price</th>
                    <th>Total</th>
                    <th>Phone HC</th>
                    <th>Price</th>
                    <th>Total</th>
                    <th>Casino Profit</th>
                    <th>Total Amount</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td>
                        <asp:Label ID="lblAgent" runat="server" Text=""></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblNumAccInternet" runat="server" Text=""></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblInternetPrice" runat="server" Text=""></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblTotalIn" runat="server" Text=""></asp:Label></td>
                    <td>
                        <asp:Label ID="lblNumAccPhone" runat="server" Text=""></asp:Label></td>
                    <td>
                        <asp:Label ID="lblPhonePrice" runat="server" Text=""></asp:Label></td>
                    <td>
                        <asp:Label ID="lblTotalPhone" runat="server" Text=""></asp:Label></td>
                    <td>
                        <asp:Label ID="lblCasinoProfit" runat="server" Text="0"></asp:Label></td>

                    <td>
                        <asp:Label ID="lblTotalAmount" runat="server" Text=""></asp:Label></td>
                </tr>
                <tr>
                    <td>Total</td>
                    <td>
                        <asp:Label ID="lblNumAccInternet2" runat="server" Text=""></asp:Label>
                    </td>
                    <td></td>
                    <td>
                        <asp:Label ID="lblTotalIn2" runat="server" Text=""></asp:Label></td>
                    <td>
                        <asp:Label ID="lblNumAccPhone2" runat="server" Text=""></asp:Label></td>
                    <td></td>
                    <td>
                        <asp:Label ID="lblTotalPhone2" runat="server" Text=""></asp:Label></td>
                    <td>
                        <asp:Label ID="lblCasinoProfit2" runat="server" Text=""></asp:Label></td>
                    <td>
                        <asp:Label ID="lblTotalAmount2" runat="server" Text=""></asp:Label></td>
                </tr>
            </tbody>
        </table>
    </div>
</div>


