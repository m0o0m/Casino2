<%@ Page Language="VB" MasterPageFile="Agents.master" AutoEventWireup="false" CodeFile="OpenBets.aspx.vb"
    Inherits="SBSAgents.OpenBets" %>

<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="~/SBS/Inc/ticketBetsGridAgent.ascx" TagName="ticketBetsGridAgent"
    TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="Server">
    <div class="panel panel-grey">
        <div class="panel-heading"></div>
        <div class="panel-body">
            <div class="form-group">
                <span class="control-label col-sm-2">Sub Agents</span>
                <div class="col-sm-2">
                    <div id="divSubAgent" runat="server" visible="false">
                        <cc1:CDropDownList ID="ddlSubAgent" runat="server" CssClass="form-control"
                            AutoPostBack="true" />
                    </div>
                </div>
                <span class="control-label col-sm-2">Player</span>
                <div class="col-sm-2">
                    <div id="dateRange" style="float: left; color: black;">
                        <cc1:CDropDownList ID="ddlPlayers" runat="server" CssClass="form-control" AutoPostBack="true" hasOptionalItem="false" />
                    </div>
                </div>
            </div>
            <div class="form-group">
                <span class="control-label col-sm-2">Type Of Bet</span>
                <div class="col-sm-2">
                    <cc1:CDropDownList ID="ddlTypeOfBet" runat="server" CssClass="form-control" AutoPostBack="true">
                        <asp:ListItem Text="All" Value="All"></asp:ListItem>
                        <asp:ListItem Text="Internet" Value="Internet"></asp:ListItem>
                        <asp:ListItem Text="Phone" Value="Phone"></asp:ListItem>
                    </cc1:CDropDownList>
                </div>
                <span class="control-label col-sm-2">Context</span>
                <div class="col-sm-2">
                    <cc1:CDropDownList ID="ddlContext" runat="server" CssClass="form-control" AutoPostBack="true">
                        <asp:ListItem Text="All" Value=""></asp:ListItem>
                        <asp:ListItem Text="Current" Value="Current"></asp:ListItem>
                        <asp:ListItem Text="1H" Value="1H"></asp:ListItem>
                        <asp:ListItem Text="2H" Value="2H"></asp:ListItem>
                        <asp:ListItem Text="Quarters" Value="Q"></asp:ListItem>
                        <asp:ListItem Text="Proposition" Value="Proposition"></asp:ListItem>
                    </cc1:CDropDownList>
                </div>
            </div>
        </div>
    </div>
    <div class="panel panel-grey" style="overflow: visible">
        <div class="panel-heading"></div>
        <div class="panel-body">
            <asp:UpdatePanel ID="up1" runat="server">
                <triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlPlayers" EventName="SelectedIndexChanged" />
                    </triggers>
                <contenttemplate>
                    <asp:Timer ID="tmrRefresh" runat="server" Interval="1500" Enabled="false" />
                    <uc1:ticketBetsGridAgent ID="ucTicketBetsGridAgent" runat="server" />
                </contenttemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>
