<%@ Page Language="VB" MasterPageFile="~/SBS/Agents/Agents.master" AutoEventWireup="false"
    CodeFile="AgentSetting.aspx.vb" Inherits="SBSAgents.AgentSetting" %>

<%@ Register Src="~/Inc/TeaserAllow.ascx" TagName="TeaserAllow" TagPrefix="uc1" %>
<%@ Register Src="~/Inc/Agents/GamePartTimeDisplaySetup.ascx" TagName="GamePartTimeDisplaySetup" TagPrefix="uc1" %>
<%@ Register Src="~/Inc/Agents/TimeLineOff.ascx" TagName="TimeLineOff" TagPrefix="uc1" %>
<%@ Register Src="~/Inc/Agents/ParlayAndReverseRules.ascx" TagName="ParlayAndReverseRules" TagPrefix="uc1" %>
<%@ Register Src="~/Inc/Agents/ParlaySetup.ascx" TagName="ParlaySetup" TagPrefix="uc1" %>
<%@ Register Src="~/Inc/Agents/FixedSpreadMoney.ascx" TagName="FixedSpreadMoney" TagPrefix="uc1" %>
<%@ Register Src="~/Inc/GameCircledSettings.ascx" TagName="GameCircledSettings" TagPrefix="uc1" %>
<%@ Register Src="~/Inc/SportAllowance.ascx" TagName="SportAllowance" TagPrefix="uc1" %>
<%@ Register Src="~/Inc/SuperAdmins/JuiceControl.ascx" TagName="JuiceControl" TagPrefix="uc2" %>
<%@ Register Src="~/Inc/Agents/QuarterDisplaySetup.ascx" TagName="TeamTotalDisplaySetup" TagPrefix="uc3" %>
<%@ Register Src="~/Inc/Agents/MaxPerGame24h.ascx" TagName="MaxPerGame24h" TagPrefix="uc3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" runat="Server">
    <script>
        $(document).ready(function () {
            setTimeout(function () {
                $(".panel-title a[href='#collapse-" + '<%= tabContent1.ClientID %>' + "']").mousedown(function () {
                    __doPostBack('<%=lbnSportAllow.UniqueID%>', '');
                });
                $(".panel-title a[href='#collapse-" + '<%= tabContent2.ClientID%>' + "']").mousedown(function () {
                    __doPostBack('<%=lbnDisplay.UniqueID%>', '');
                });
                $(".panel-title a[href='#collapse-" + '<%= tabContent3.ClientID%>' + "']").mousedown(function () {
                    __doPostBack('<%=lbnRiskControl.UniqueID%>', '');
                });
                $(".panel-title a[href='#collapse-" + '<%= tabContent4.ClientID%>' + "']").mousedown(function () {
                    __doPostBack('<%=lbnParlaySetup.UniqueID%>', '');
                });
                $(".panel-title a[href='#collapse-" + '<%= tabContent5.ClientID%>' + "']").mousedown(function () {
                    __doPostBack('<%=lbnParlayAllow.UniqueID%>', '');
                });
                $(".panel-title a[href='#collapse-" + '<%= tabContent6.ClientID%>' + "']").mousedown(function () {
                    __doPostBack('<%=lbnParlayAllowBWgame.UniqueID%>', '');
                });
            }, 300);

            $("#<%=lbnSportAllow.ClientID%>").click(function () {
                __doPostBack('<%=lbnSportAllow.UniqueID%>', '');
            });
            $("#<%=lbnDisplay.ClientID%>").click(function () {
                __doPostBack('<%=lbnDisplay.UniqueID%>', '');
            });
            $("#<%=lbnRiskControl.ClientID%>").click(function () {
                __doPostBack('<%=lbnRiskControl.UniqueID%>', '');
            });
            $("#<%=lbnParlaySetup.ClientID%>").click(function () {
                __doPostBack('<%=lbnParlaySetup.UniqueID%>', '');
            });
            $("#<%=lbnParlayAllow.ClientID%>").click(function () {
                __doPostBack('<%=lbnParlayAllow.UniqueID%>', '');
            });
            $("#<%=lbnParlayAllowBWgame.ClientID%>").click(function () {
                __doPostBack('<%=lbnParlayAllowBWgame.UniqueID%>', '');
            });
        });
    </script>

    <div id="tab-general">
        <div class="row mbl">
            <div class="col-lg-12">
                <ul id="generalTab" class="nav nav-tabs responsive">
                    <li id="liSPORT_ALLOW" runat="server">
                        <asp:LinkButton runat="server" ID="lbnSportAllow" CommandArgument="SPORT_ALLOW" Text="Bookmaker Setup"
                            OnClick="lbtTab_Click" CausesValidation="false" />
                    </li>
                    <li id="liDISPLAY" runat="server">
                        <asp:LinkButton runat="server" ID="lbnDisplay" CommandArgument="DISPLAY" Text="Display Rule"
                            OnClick="lbtTab_Click" CausesValidation="false" />
                    </li>
                    <li id="liRISK_CONTROL" runat="server">
                        <asp:LinkButton runat="server" ID="lbnRiskControl" CommandArgument="RISK_CONTROL"
                            Text="Risk Control" OnClick="lbtTab_Click" CausesValidation="false" />
                    </li>
                    <li id="liPARLAY_SETUP" runat="server">
                        <asp:LinkButton runat="server" ID="lbnParlaySetup" CommandArgument="PARLAY_SETUP"
                            Text="Parplay Setup" OnClick="lbtTab_Click" CausesValidation="false" />
                    </li>
                    <li id="liPARLAY_ALLOW" runat="server">
                        <asp:LinkButton runat="server" ID="lbnParlayAllow" CommandArgument="PARLAY_ALLOW"
                            Text="Parplay Allowance(in Games)" OnClick="lbtTab_Click" CausesValidation="false" />
                    </li>
                    <li id="liPARLAY_BW_ALLOW" runat="server">
                        <asp:LinkButton runat="server" ID="lbnParlayAllowBWgame" CommandArgument="PARLAY_BW_ALLOW"
                            Text="Parplay Allowance(b/w Games)" OnClick="lbtTab_Click" CausesValidation="false" />
                    </li>
                </ul>
                <div id="generalTabContent" class="tab-content responsive" style="border: 0 none">
                    <div id="tabContent1" class="tab-pane fade in" runat="server">
                        <div class="panel panel-grey">
                            <div class="panel-heading">Bookmaker Setup</div>
                            <div class="panel-body">
                                <uc1:SportAllowance ID="ucSportAllowance" runat="server"></uc1:SportAllowance>
                            </div>
                        </div>
                    </div>
                    <div id="tabContent2" class="tab-pane fade in" runat="server">
                        <div class="col-md-6">
                            <div class="panel panel-grey">
                                <div class="panel-heading">1H , 2H & Teamtotal Display Setup</div>
                                <div class="panel-body">
                                    <uc1:GamePartTimeDisplaySetup ID="ucGamePartTimeDisplaySetup" runat="server" />
                                </div>
                            </div>
                            <div class="panel panel-grey">
                                <div class="panel-heading">Quarter Display Setup</div>
                                <div class="panel-body">
                                    <uc3:TeamTotalDisplaySetup ID="TeamTotalDisplaySetup1" runat="server" />
                                </div>
                            </div>
                            <div class="panel panel-grey">
                                <div class="panel-heading">Teaser Allow Display Setup</div>
                                <div class="panel-body">
                                    <uc1:TeaserAllow ID="ucTeaserAllow" runat="server" />
                                </div>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="panel panel-grey">
                                <div class="panel-heading">Full Game Display Setup</div>
                                <div class="panel-body">
                                    <uc1:TimeLineOff ID="ucTimeLineOff" runat="server" ContextDisPlay="Current" />
                                </div>
                            </div>
                            <div class="panel panel-grey">
                                <div class="panel-heading">1H Display Setup</div>
                                <div class="panel-body">
                                    <uc1:TimeLineOff ID="TimeLineOff1" runat="server" ContextDisPlay="FirstHalf" />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="tabContent3" class="tab-pane fade in" runat="server">
                        <div class="col-md-6">
                            <uc1:FixedSpreadMoney Visible="true" ID="ucFixedSpreadMoney" runat="server"></uc1:FixedSpreadMoney>
                            <uc3:MaxPerGame24h Visible="true" ID="ucMaxPerGame24h" runat="server"></uc3:MaxPerGame24h>
                        </div>
                        <div class="col-md-6">
                            <uc1:GameCircledSettings ID="ucGameCircledSettings" runat="server"></uc1:GameCircledSettings>
                            <br />
                            <uc2:JuiceControl ID="ucJuiceControl" runat="server"></uc2:JuiceControl>
                        </div>
                    </div>
                    <div id="tabContent4" class="tab-pane fade in" runat="server">
                        <div class="panel panel-grey">
                            <div class="panel-heading">Parlay Setup</div>
                            <div class="panel-body">
                                <uc1:ParlaySetup ID="ucParlaySetup" runat="server" />
                            </div>
                        </div>
                    </div>
                    <div id="tabContent5" class="tab-pane fade in" runat="server">
                        <div class="panel panel-grey">
                            <div class="panel-heading">Parlay Setup In Game</div>
                            <div class="panel-body">
                                <uc1:ParlayAndReverseRules ID="ucParlayAndReverseRules" BetweenGames="false" runat="server" />
                            </div>
                        </div>
                    </div>
                    <div id="tabContent6" class="tab-pane fade in" runat="server">
                        <div class="panel panel-grey">
                            <div class="panel-heading">Parlay Setup Between Game</div>
                            <div class="panel-body">
                                <uc1:ParlayAndReverseRules ID="ParlayAndReverseRules1" runat="server" BetweenGames="true" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
