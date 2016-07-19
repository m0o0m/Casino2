<%@ Control Language="VB" AutoEventWireup="false" CodeFile="AgentMenu.ascx.vb" Inherits="SBSAgents.Inc_Agents_AgentMenu" %>

<%@ Register Src="~/Inc/Players/MailAlert.ascx" TagName="MailAlert" TagPrefix="uc" %>

<div class="panel panel-grey">
    <div class="panel-heading"></div>
    <div class="panel-body">
        <uc:MailAlert ID="ucMailAlert" runat="server" />
        <br />
        <asp:Panel ID="pnUserManament" runat="server" Visible="false">
            <table class="tbl-menu">
                <tr id="trAgent" runat="server">
                    <td>
                        <span class="btnGame" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)">
                            <asp:LinkButton ID="LinkButton1" runat="server" CssClass="btn btn-primary" PostBackUrl="/SBS/Agents/Management/Agents.aspx">Agents</asp:LinkButton>
                        </span>
                    </td>
                    <td>
                        <span class="btnGame" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)">
                            <asp:LinkButton ID="LinkButton4" runat="server" CssClass="btn btn-primary" PostBackUrl="/SBS/Agents/Management/Players.aspx">Players</asp:LinkButton>
                        </span>
                    </td>

                </tr>
                <tr id="trSubAgent" runat="server">
                    <td colspan="2">
                        <span class="btnGame" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)">
                            <asp:LinkButton ID="LinkButton14" runat="server" CssClass="btn btn-primary" PostBackUrl="/SBS/Agents/Management/SubAgentManager.aspx">Players</asp:LinkButton>
                        </span>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="pnGameManament" runat="server" Visible="false">
            <table class="tbl-menu">
                <tr>
                    <td>
                        <span class="btnGame" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)">
                            <asp:LinkButton ID="LinkButton2" runat="server" CssClass="btn btn-primary" PostBackUrl="/SBS/Agents/Management/OddSetting.aspx">Odds</asp:LinkButton>
                        </span>
                    </td>
                    <td>
                        <span class="btnGame" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)">
                            <asp:LinkButton ID="LinkButton3" runat="server" CssClass="btn btn-primary" PostBackUrl="/SBS/Agents/Management/AgentSetting.aspx">Settings</asp:LinkButton>
                        </span>
                    </td>
                    <td>
                        <span class="btnGame" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)">
                            <asp:LinkButton ID="LinkButton5" runat="server" CssClass="btn btn-primary" PostBackUrl="/SBS/Agents/Management/GameManual.aspx">Quarter Lines Setup</asp:LinkButton>
                        </span>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="pnAccount" runat="server" Visible="false">
            <table class="tbl-menu">
                <tr>
                    <td>
                        <span class="btnGame" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)">
                            <asp:LinkButton ID="LinkButton6" runat="server" CssClass="btn btn-primary" PostBackUrl="/SBS/Agents/AgentAccount.aspx">Account Status</asp:LinkButton>
                        </span>
                    </td>
                    <td>
                        <span class="btnGame" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)">
                            <asp:LinkButton ID="LinkButton7" runat="server" CssClass="btn btn-primary" PostBackUrl="/SBS/Agents/ChangePassword.aspx">Change Password</asp:LinkButton>
                        </span>
                    </td>
                    <td>
                        <span class="btnGame" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)">
                            <asp:LinkButton ID="LinkButton8" runat="server" CssClass="btn btn-primary" PostBackUrl="/SBS/Agents/AgentMail.aspx">Inbox Mail</asp:LinkButton>
                        </span>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="pnSysMagement" runat="server" Visible="false">
            <table class="tbl-menu">
                <tr>
                    <td>
                        <span class="btnGame" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)">
                            <asp:LinkButton ID="lbnAgents" runat="server" CssClass="btn btn-primary" PostBackUrl="/SBS/Agents/Management/ConfigureLogo.aspx">Upload Image</asp:LinkButton>
                        </span>
                    </td>
                    <td>
                        <span class="btnGame" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)">
                            <asp:LinkButton ID="lbnAgentBalanceReports" runat="server" CssClass="btn btn-primary" PostBackUrl="/SBS/Agents/Management/SiteOption.aspx">Site Setup</asp:LinkButton>
                        </span>
                    </td>
                </tr>

            </table>
        </asp:Panel>
        <asp:Panel ID="pnReports" runat="server" Visible="false">
            <table class="tbl-menu">
                <tr>
                    <td>
                        <span class="btnGame" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)">
                            <asp:LinkButton ID="LinkButton9" runat="server" CssClass="btn btn-primary" PostBackUrl="/SBS/Agents/OpenBets.aspx">Pending</asp:LinkButton>
                        </span>
                    </td>
                    <td>
                        <span class="btnGame" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)">
                            <asp:LinkButton ID="LinkButton10" runat="server" CssClass="btn btn-primary" PostBackUrl="/SBS/Agents/History.aspx">History</asp:LinkButton>
                        </span>
                    </td>
                    <td>
                        <span class="btnGame" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)">
                            <asp:LinkButton ID="LinkButton11" runat="server" CssClass="btn btn-primary" PostBackUrl="/SBS/Agents/Transactions.aspx">Transactions</asp:LinkButton>
                        </span>
                    </td>

                    <td>
                        <span class="btnGame" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)">
                            <asp:LinkButton ID="lbnPlayers" runat="server" CssClass="btn btn-primary" PostBackUrl="/SBS/Agents/Management/PlayersReports.aspx">Player Balance</asp:LinkButton>
                        </span>
                    </td>
                    <td>
                        <span class="btnGame" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)">
                            <asp:LinkButton ID="lbnPlayerBalanceReports" CssClass="btn btn-primary" runat="server" PostBackUrl="/SBS/Agents/Management/SubAgentReport.aspx">Sub - Agent Balance</asp:LinkButton>
                        </span>
                    </td>
                    <td>
                        <span class="btnGame" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)">
                            <asp:LinkButton ID="lbnIPReports" runat="server" CssClass="btn btn-primary" PostBackUrl="/SBS/Agents/Management/IPReports.aspx">IP Reports</asp:LinkButton>
                        </span>
                    </td>

                    <td>
                        <span class="btnGame" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)">
                            <asp:LinkButton ID="LinkButton12" CssClass="btn btn-primary" runat="server" PostBackUrl="/SBS/Agents/Management/PLReport.aspx">PL Reports</asp:LinkButton>
                        </span>
                    </td>
                    <td>
                        <span class="btnGame" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)">
                            <asp:LinkButton ID="lbnAgentPositionReport" CssClass="btn btn-primary" runat="server" PostBackUrl="/SBS/Agents/Management/AgentPositionReport.aspx">Position Reports</asp:LinkButton>
                        </span>
                    </td>
                    <td>
                        <span class="btnGame" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)">
                            <asp:LinkButton ID="LinkButton15" CssClass="btn btn-primary" runat="server" PostBackUrl="/SBS/Agents/LiveTicker.aspx">Live Ticker Report</asp:LinkButton>
                        </span>
                    </td>
                    <td>
                        <span class="btnGame" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)">
                            <asp:LinkButton ID="LinkButton13" CssClass="btn btn-primary" runat="server" PostBackUrl="/SBS/Agents/headcount.aspx">Head Count Reports</asp:LinkButton>
                        </span>
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </div>
</div>
