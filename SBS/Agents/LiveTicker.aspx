<%@ Page Language="VB" MasterPageFile="~/SBS/Agents/Agents.master" AutoEventWireup="false" CodeFile="LiveTicker.aspx.vb" Inherits="SBSAgents.LiveTicker" %>

<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="~/SBS/Inc/ticketBetsGridAgent.ascx" TagName="ticketBetsGridAgent"
    TagPrefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" runat="Server">

    <div class="panel panel-grey">
        <div class="panel-heading"></div>
        <div class="panel-body">

            <div class="form-group">
                <label class="control-label col-md-2">Enter the filter threshold</label>
                <div class="col-md-2">
                    <asp:TextBox ID="txtThreshold" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-md-2">
                    <asp:Button
                        ID="btnSearch" CssClass="btn btn-primary" runat="server" Text="Filter" />
                </div>
            </div>
            <div class="form-group">
                <div id="divSubAgent" runat="server" class="col-md-4">
                    <div class="col-md-6 p0">
                        <label class="control-label">SubAgents</label>
                    </div>
                    <div class="col-md-6 pr0">
                        <cc1:CDropDownList ID="ddlSubAgent" runat="server" CssClass="form-control"
                            AutoPostBack="true" />
                    </div>
                </div>
                <div id="dateRange" class="col-md-4">
                    <div class="col-md-6 p0">
                        <label class="control-label">Player</label>
                    </div>
                    <div class="col-md-6 pr0">
                        <cc1:CDropDownList ID="ddlPlayers" runat="server" CssClass="form-control" AutoPostBack="true" hasOptionalItem="false" />
                    </div>
                </div>
            </div>
            <div class="form-group">
                <div class="col-md-2">
                    <label class="control-label">Type Of Bet</label>
                </div>
                <div class="col-md-2">
                    <cc1:CDropDownList ID="ddlTypeOfBet" runat="server" CssClass="form-control" AutoPostBack="true">
                        <asp:ListItem Text="All" Value="All"></asp:ListItem>
                        <asp:ListItem Text="Internet" Value="Internet"></asp:ListItem>
                        <asp:ListItem Text="Phone" Value="Phone"></asp:ListItem>
                    </cc1:CDropDownList>
                </div>
                <div class="col-md-2">
                    <label class="control-label">Context</label>
                </div>
                <div class="col-md-2">
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
    <div class="panel panel-grey">
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

