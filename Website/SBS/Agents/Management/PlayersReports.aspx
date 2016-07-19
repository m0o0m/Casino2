<%@ Page Language="VB" MasterPageFile="../Agents.master"
    AutoEventWireup="false" CodeFile="PlayersReports.aspx.vb" Inherits="SBSAgents.PlayersReports" %>

<%@ Register Src="~/Inc/Reports/PlayerBalanceReport.ascx" TagName="PlayerReports" TagPrefix="uc" %>
<%@ Register Src="~/Inc/Reports/SummaryReport.ascx" TagName="SummaryReport" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="Server">
    <div id="tab-general">
        <div class="row mbl">
            <div class="col-lg-12">
                <ul id="generalTab" class="nav nav-tabs responsive">
                    <li id="liAllPlayers" runat="server">
                        <asp:LinkButton runat="server" ID="lbtAllPlayer" CommandArgument="ALL_PLAYERS" Visible="true"
                            Text="Weekly Figures" CausesValidation="false" OnClick="lbtTab_Click" data-toggle="tab" />
                    </li>
                    <li id="liPlayerReport" runat="server">
                        <asp:LinkButton runat="server" ID="lbtPlayers" CommandArgument="PLAYERS" Text="Players"
                            CausesValidation="false" OnClick="lbtTab_Click" data-toggle="tab" />
                    </li>
                    <li id="liSubAgents" runat="server">
                        <asp:LinkButton runat="server" ID="lbtSubAgents" CommandArgument="SUB_AGENTS" Visible="false"
                            Text="Sub Agents" CausesValidation="false" OnClick="lbtTab_Click" data-toggle="tab" />
                    </li>
                </ul>
                <div id="generalTabContent" class="tab-content responsive" style="border: 0 none">
                    <div id="tabContent1" class="tab-pane fade in" runat="server">
                        <uc:SummaryReport ID="ucSummaryReport" HistoryPage="../History.aspx" PendingPage="../OpenBets.aspx" runat="server" Visible="true" />
                    </div>
                    <div id="tabContent2" class="tab-pane fade in" runat="server">
                        <uc:PlayerReports Visible="false" ID="ucPlayerReports" ShowWeekList="true" ShowAgentList="false"
                            LabelSelectedUser="Agents:" HistoryPage="../History.aspx" PendingPage="../OpenBets.aspx" runat="server" />
                    </div>
                    <div id="tabContent3" class="tab-pane fade in" runat="server"></div>
                </div>
            </div>
        </div>
    </div>
    <script>
        $(document).ready(function () {
            setTimeout(function () {
                $(".panel-title a[href='#collapse-" + '<%= tabContent1.ClientID %>' + "']").mousedown(function () {
                    __doPostBack('<%=lbtAllPlayer.UniqueID%>', '');
                });

                $(".panel-title a[href='#collapse-" + '<%= tabContent2.ClientID%>' + "']").mousedown(function () {
                    __doPostBack('<%=lbtPlayers.UniqueID%>', '');
                });

                $(".panel-title a[href='#collapse-" + '<%= tabContent3.ClientID%>' + "']").mousedown(function () {
                    __doPostBack('<%=lbtSubAgents.UniqueID%>', '');
                });
            }, 300);

        });

        $("#<%=lbtAllPlayer.ClientID%>").click(function () {
            __doPostBack('<%=lbtAllPlayer.UniqueID%>', '');
        });

        $("#<%=lbtPlayers.ClientID%>").click(function () {
            __doPostBack('<%=lbtPlayers.UniqueID%>', '');
        });

        $("#<%=lbtSubAgents.ClientID%>").click(function () {
            __doPostBack('<%=lbtSubAgents.UniqueID%>', '');
        });

    </script>
</asp:Content>

