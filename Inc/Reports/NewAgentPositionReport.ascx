<%@ Control Language="VB" AutoEventWireup="false" CodeFile="NewAgentPositionReport.ascx.vb" Inherits="SBCWebsite.NewAgentPositionReport" %>
<style type="text/css">
    table tr td {
        font-size: 12px;
    }

    .offering_pair_odd td, .offering_pair_even td {
        font-size: 12px;
    }
</style>

<div class="panel panel-grey">
    <div class="panel-heading">Filters</div>
    <div class="panel-body">
        <div id="divAgents" runat="server" class="form-group">
            <label class="col-md-1 control-label">Agents</label>
            <div class="col-md-3">
                <asp:DropDownList ID="ddlAgents" runat="server" AutoPostBack="true" CssClass="form-control">
                </asp:DropDownList>
            </div>
        </div>
        <div class="form-group">
            <label class="col-md-1 control-label">Amount</label>
            <div class="col-md-1 w10">
                <asp:RadioButton ID="rdRisk" GroupName="a" Checked="true" runat="server" />
            </div>
            <div class="col-md-1">
                <label class="control-label pt2">Risk</label>
            </div>
            <div class="col-md-1 w10">
                <asp:RadioButton ID="rdWin" GroupName="a" runat="server" />
            </div>
            <div class="col-md-1">
                <label class="control-label pt2">Win</label>
            </div>
            <div class="col-md-1 w10">
                <asp:CheckBox ID="chkAllOpen" runat="server" />
            </div>
            <div class="col-md-1">
                <label class="control-label pt2">All Open</label>
            </div>
        </div>
        <div class="form-group">
            <label class="col-md-1 control-label">BetType</label>
            <div class="col-md-1 w10">
                <asp:RadioButton ID="rdStraight" Checked="true" runat="server" />
            </div>
            <div class="col-md-1">
                <label class="control-label pt2">Straight</label>
            </div>
        </div>
        <div class="form-group">
            <div class="col-md-offset-1 col-md-2">
                <asp:Button ID="btnViewPosition" class="btn btn-info" Text="View Position" runat="server" />
            </div>

        </div>
    </div>
</div>

<asp:Repeater ID="rptPortType" runat="server">
    <ItemTemplate>
        <div style="text-align: center">
            <h2>
                <asp:Label ID="lblGameType" runat="server" Text=""></asp:Label></h2>
        </div>
        <asp:Repeater ID="rptOpenBet" runat="server" OnItemDataBound="rptOpenBet_ItemDataBound">
            <HeaderTemplate>
                <table class="table table-hover table-bordered">
                    <tr class="tableheading">
                        <td>Date</td>
                        <td>Game</td>
                        <td>Event</td>
                        <td colspan="2">Spread</td>
                        <td colspan="2">Money Line</td>
                        <td colspan="2">Total Points</td>
                        <td colspan="2" id="td1HSpread" runat="server">1H Spread</td>
                        <td colspan="2" id="td1HML" runat="server">1H Money Line</td>
                        <td colspan="2" id="td1HTotal" runat="server">1H Total Points</td>
                        <td colspan="2" id="td2HSpread" runat="server">2H Spread</td>
                        <td colspan="2" id="td2HML" runat="server">2H Money Line</td>
                        <td colspan="2" id="td2HTotal" runat="server">2H Total Points</td>
                    </tr>
                    <tr class="tableheading">
                        <td></td>
                        <td></td>
                        <td></td>
                        <td>Bet</td>
                        <td>Amount
                                    <asp:Label ID="lblRiskSpread" runat="server"></asp:Label></td>

                        <td>Bet</td>
                        <td>Amount
                                    <asp:Label ID="lblRiskML" runat="server"></asp:Label></td>
                        <td>Bet</td>
                        <td>Amount
                                    <asp:Label ID="lblRiskTotal" runat="server"></asp:Label></td>

                        <td id="td1HSpreadBet" runat="server">Bet</td>
                        <td id="td1HSpreadRisk" runat="server">Amount (Risk)
                                    <asp:Label ID="lblRiskSpread1H" runat="server"></asp:Label></td>
                        <td id="td1HMLBet" runat="server">Bet</td>
                        <td id="td1HMLRisk" runat="server">Amount
                                    <asp:Label ID="lblRiskML1H" runat="server"></asp:Label></td>
                        <td id="td1HTotalBet" runat="server">Bet</td>
                        <td id="td1HTotalRisk" runat="server">Amount
                                    <asp:Label ID="lblRiskTotal1H" runat="server"></asp:Label></td>

                        <td id="td2HSpreadBet" runat="server">Bet</td>
                        <td id="td2HSpreadRisk" runat="server">Amount 
                                    <asp:Label ID="lblRiskSpread2H" runat="server"></asp:Label></td>
                        <td id="td2HMLBet" runat="server">Bet</td>
                        <td id="td2HMLRisk" runat="server">Amount
                                    <asp:Label ID="lblRiskML2H" runat="server"></asp:Label></td>
                        <td id="td2HTotalBet" runat="server">Bet</td>
                        <td id="td2HTotalRisk" runat="server">Amount
                                    <asp:Label ID="lblRiskTotal2H" runat="server"></asp:Label></td>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="offering_pair_<%#SBCBL.std.SafeString(ViewState("OddEven")) %>" style="text-align: center">
                    <td rowspan="2" style="text-align: left">
                        <asp:Label ID="lblTransactionDate" runat="server"></asp:Label>
                    </td>
                    <td style="text-align: left">
                        <asp:Label ID="lblAwayRotationNumber" runat="server"></asp:Label>
                    </td>
                    <td style="text-align: left">
                        <asp:Label ID="lblAwayTeam" runat="server"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblCurrentBetAwaySpread" runat="server"></asp:Label></td>
                    <td>
                        <asp:Label ID="lblRiskAmountCurrentAwaySpread" runat="server"></asp:Label></td>
                    <td>
                        <asp:Label ID="lblCurrentBetAwayML" runat="server"></asp:Label></td>
                    <td>
                        <asp:Label ID="lblRiskAmountCurrentAwayML" runat="server"></asp:Label></td>
                    <td>
                        <asp:Label ID="lblCurrentBetAwayTotalPoint" runat="server"></asp:Label></td>
                    <td>
                        <asp:Label ID="lblRiskAmountCurrentTotalPointOver" runat="server"></asp:Label></td>


                    <td id="td1HSpreadBetAway" runat="server">
                        <asp:Label ID="lbl1HBetAwaySpread" runat="server"></asp:Label></td>
                    <td id="td1HSpreadRiskAway" runat="server">
                        <asp:Label ID="lblRiskAmount1HAwaySpread" runat="server"></asp:Label></td>
                    <td id="td1HMLBetAway" runat="server">
                        <asp:Label ID="lbl1HBetAwayML" runat="server"></asp:Label></td>
                    <td id="td1HMLRiskAway" runat="server">
                        <asp:Label ID="lblRiskAmount1HAwayML" runat="server"></asp:Label></td>
                    <td id="td1HTotalBetAway" runat="server">
                        <asp:Label ID="lbl1HBetAwayTotalPoint" runat="server"></asp:Label></td>
                    <td id="td1HTotalRiskAway" runat="server">
                        <asp:Label ID="lblRiskAmount1HTotalPointOver" runat="server"></asp:Label></td>

                    <td id="td2HSpreadBetAway" runat="server">
                        <asp:Label ID="lbl2HBetAwaySpread" runat="server"></asp:Label></td>
                    <td id="td2HSpreadRiskAway" runat="server">
                        <asp:Label ID="lblRiskAmount2HAwaySpread" runat="server"></asp:Label></td>
                    <td id="td2HMLBetAway" runat="server">
                        <asp:Label ID="lbl2HBetAwayML" runat="server"></asp:Label></td>
                    <td id="td2HMLRiskAway" runat="server">
                        <asp:Label ID="lblRiskAmount2HAwayML" runat="server"></asp:Label></td>
                    <td id="td2HTotalBetAway" runat="server">
                        <asp:Label ID="lbl2HBetAwayTotalPoint" runat="server"></asp:Label></td>
                    <td id="td2HTotalRiskAway" runat="server">
                        <asp:Label ID="lblRiskAmount2HTotalPointOver" runat="server"></asp:Label></td>
                </tr>
                <tr class="offering_pair_<%#SBCBL.std.SafeString(ViewState("OddEven")) %>" style="text-align: center">

                    <td style="text-align: left">
                        <asp:Label ID="lblHomeRotationNumber" runat="server"></asp:Label></td>
                    <td style="text-align: left">
                        <asp:Label ID="lblHomeTeam" runat="server"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblCurrentBetHomeSpread" runat="server"></asp:Label></td>
                    <td>
                        <asp:Label ID="lblRiskAmountCurrentHomeSpread" runat="server"></asp:Label></td>
                    <td>
                        <asp:Label ID="lblCurrentBetHomeML" runat="server"></asp:Label></td>
                    <td>
                        <asp:Label ID="lblRiskAmountCurrentHomeML" runat="server"></asp:Label></td>
                    <td>
                        <asp:Label ID="lblCurrentBetHomeTotalPoint" runat="server"></asp:Label></td>
                    <td>
                        <asp:Label ID="lblRiskAmountCurrentTotalPointUnder" runat="server"></asp:Label></td>

                    <td id="td1HSpreadBetHome" runat="server">
                        <asp:Label ID="lbl1HBetHomeSpread" runat="server"></asp:Label></td>
                    <td id="td1HSpreadRiskHome" runat="server">
                        <asp:Label ID="lblRiskAmount1HHomeSpread" runat="server"></asp:Label></td>
                    <td id="td1HMLBetHome" runat="server">
                        <asp:Label ID="lbl1HBetHomeML" runat="server"></asp:Label></td>
                    <td id="td1HMLRiskHome" runat="server">
                        <asp:Label ID="lblRiskAmount1HHomeML" runat="server"></asp:Label></td>
                    <td id="td1HTotalBetHome" runat="server">
                        <asp:Label ID="lbl1HBetHomeTotalPoint" runat="server"></asp:Label></td>
                    <td id="td1HTotalRiskHome" runat="server">
                        <asp:Label ID="lblRiskAmount1HTotalPointUnder" runat="server"></asp:Label></td>

                    <td id="td2HSpreadBetHome" runat="server">
                        <asp:Label ID="lbl2HBetHomeSpread" runat="server"></asp:Label></td>
                    <td id="td2HSpreadRiskHome" runat="server">
                        <asp:Label ID="lblRiskAmount2HHomeSpread" runat="server"></asp:Label></td>
                    <td id="td2HMLBetHome" runat="server">
                        <asp:Label ID="lbl2HBetHomeML" runat="server"></asp:Label></td>
                    <td id="td2HMLRiskHome" runat="server">
                        <asp:Label ID="lblRiskAmount2HHomeML" runat="server"></asp:Label></td>
                    <td id="td2HTotalBetHome" runat="server">
                        <asp:Label ID="lbl2HBetHomeTotalPoint" runat="server"></asp:Label></td>
                    <td id="td2HTotalRiskHome" runat="server">
                        <asp:Label ID="lblRiskAmount2HTotalPointUnder" runat="server"></asp:Label></td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </ItemTemplate>
</asp:Repeater>

<div class="panel panel-grey">
    <div class="panel-heading"></div>
    <div class="panel-body">
        <table class="table table-hover table-bordered">
            <thead>
                <tr class="tableheading">
                    <th>Total</th>
                    <th>Amount</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td>Bet</td>
                    <td>
                        <asp:Label ID="lblBet" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblAmountRiskWin" Text="Amount (risk)" runat="server"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblAmount" runat="server"></asp:Label>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</div>
