<%@ Page Language="VB" MasterPageFile="Agents.master" AutoEventWireup="false" CodeFile="History.aspx.vb"
    Inherits="SBSAgents.History" %>

<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajx" %>
<%@ Register Src="~/Inc/DateTime.ascx" TagName="DateTime" TagPrefix="uc" %>
<%@ Register Src="~/SBS/Inc/historyGrid.ascx" TagName="historyGrid" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <contenttemplate>
            <div class="panel panel-grey">
                <div class="panel-heading"></div>
                <div class="panel-body">
        
                        <div class="form-group">
                            <asp:Label runat="server" ID="lblSubAgent" Visible="true" CssClass="control-label col-md-2" Text="SubAgents" />
                            <div class="col-md-2">
                                <div id="divSubAgent" runat="server" visible="true">
                                    <wlb:CDropDownList ID="ddlSubAgent" runat="server" CssClass="form-control" hasOptionalItem="false"
                                         AutoPostBack="true" />
                                </div>
                            </div>
                            <label class="control-label col-md-2">Player</label>
                            <div class="col-md-2">
                                <wlb:CDropDownList ID="ddlPlayers" runat="server" CssClass="form-control" hasOptionalItem="false"
                                         Width="187px" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="control-label col-md-2">Date Range :</label>
                            <label class="control-label col-md-1">From</label>
                            <div class="col-md-2">
                                <uc:DateTime ID="ucDateFrom" runat="server" ShowTime="false" ShowCalendar="false"  />
                            </div>
                            <label class="control-label col-md-1">To</label>
                            <div class="col-md-2">
                                <uc:DateTime ID="ucDateTo" runat="server" ShowTime="false" ShowCalendar="false"  />
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="control-label col-md-2">Type Of Bet</label>
                            <div class="col-md-2">
                                <wlb:CDropDownList ID="ddlTypeOfBet" runat="server" CssClass="form-control" hasOptionalItem="true"
                                    OptionalText="All" OptionalValue="All" AutoPostBack="true">
                                    <asp:ListItem Text="All" Value="All"></asp:ListItem>
                                    <asp:ListItem Text="Internet" Value="Internet"></asp:ListItem>
                                    <asp:ListItem Text="Phone" Value="Phone"></asp:ListItem>
                                </wlb:CDropDownList>
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="col-md-6 col-lg-offset-2">
                                <asp:Button ID="btnSearch" Text="Search" runat="server" CssClass="btn btn-primary" />
                                <asp:Button ID="btnReset" Text="Reset" runat="server" CssClass="btn btn-primary" />
                            </div>
                        </div>
                </div>
            </div>
            <div class="panel panel-grey" style="overflow: visible">
                <div class="panel-heading"></div>
                <div class="panel-body">
                     <uc1:historyGrid ID="ucHistoryGrid" runat="server" ShowPlayerName="true" />
                </div>
            </div>
        </contenttemplate>
    </asp:UpdatePanel>
</asp:Content>
