<%@ Page Language="VB" MasterPageFile="~/SBS/SuperAdmins/SuperAdmin.master" AutoEventWireup="false"
    CodeFile="PendingTickets.aspx.vb" Inherits="SBSSuperAdmin.PendingTickets" %>

<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>
<%@ Register Src="~/SBS/SuperAdmins/Inc/ticketBetsGrid.ascx" TagName="ticketBetsGrid" TagPrefix="uc1" %>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="Server">
    <div class="row">
        <div id="historypanel" runat="server" class="col-lg-12">
            <table class="table">
                <tr>
                    <td>
                        <span style="font-weight: bolder">Agents:</span>
                    </td>
                    <td>
                        <wlb:CDropDownList ID="ddlSubAgent" runat="server" CssClass="textInput" hasOptionalItem="false"
                            OptionalText="" OptionalValue="" AutoPostBack="true" Width="220px" />
                    </td>

                </tr>
                <tr>
                    <td>
                        <span style="margin-bottom: 5px; margin-top: 5px; font-weight: bolder">Players :</span>
                    </td>
                    <td>
                        <div style="margin-bottom: 5px; margin-top: 5px">
                            <asp:UpdatePanel ID="upPlayer" runat="server">
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="ddlSubAgent" EventName="SelectedIndexChanged" />
                                </Triggers>
                                <ContentTemplate>
                                    <wlb:CDropDownList ID="ddlPlayers" runat="server" CssClass="textInput" hasOptionalItem="false"
                                        AutoPostBack="true" Width="220px" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <span style="font-weight: bolder; margin-right: 20px">Type Of Bet</span>
                    </td>
                    <td>
                        <wlb:CDropDownList ID="ddlTypeOfBet" runat="server" CssClass="textInput" hasOptionalItem="true"
                            OptionalText="All" OptionalValue="All" AutoPostBack="true">
                            <asp:ListItem Text="All" Value="All"></asp:ListItem>
                            <asp:ListItem Text="Internet" Value="Internet"></asp:ListItem>
                            <asp:ListItem Text="Phone" Value="Phone"></asp:ListItem>
                        </wlb:CDropDownList>
                        <span style="position: relative; bottom: -2px; font-weight: bolder; margin-left: 10px">Context : </span>
                        <wlb:CDropDownList ID="ddlContext" runat="server" CssClass="textInput"
                            AutoPostBack="true" Width="150px">
                            <asp:ListItem Text="All" Value=""></asp:ListItem>
                            <asp:ListItem Text="Current" Value="Current"></asp:ListItem>
                            <asp:ListItem Text="1H" Value="1H"></asp:ListItem>
                            <asp:ListItem Text="2H" Value="2H"></asp:ListItem>
                            <asp:ListItem Text="Quarters" Value="Q"></asp:ListItem>
                            <asp:ListItem Text="Proposition" Value="Proposition"></asp:ListItem>
                        </wlb:CDropDownList>
                    </td>
                </tr>
            </table>
        </div>

        <asp:UpdatePanel ID="up1" runat="server">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="ddlSubAgent" EventName="SelectedIndexChanged" />
                <asp:AsyncPostBackTrigger ControlID="ddlPlayers" EventName="SelectedIndexChanged" />
                <asp:AsyncPostBackTrigger ControlID="ddlTypeOfBet" EventName="SelectedIndexChanged" />
            </Triggers>
            <Triggers>
            </Triggers>
            <ContentTemplate>
                <asp:Timer ID="tmrRefresh" runat="server" Interval="1500" Enabled="false" />
                <uc1:ticketBetsGrid ID="ucTicketBetsGrid" runat="server" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
