<%@ Page Language="VB" MasterPageFile="Player.master" AutoEventWireup="false" CodeFile="History.aspx.vb"
    Inherits="SBSPlayer.History" Title="Untitled Page" %>

<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="../Inc/historyGrid.ascx" TagName="historyGrid" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <div class="row">
                <div class="col-lg-12">
                    <div class="page-title-breadcrumb">
                        <div class="page-header pull-left">
                            <div class="page-title pull-left mrm">
                                Section
                            </div>
                            <span class="label label-grey pull-left" style="font-size: large">History</span>
                        </div>
                        <div class="clearfix">
                        </div>
                    </div>
                </div>
            </div>

            <div class="mbxl"></div>

            <div class="panel panel-grey">
                <div class="panel-heading">Filter</div>
                <div class="panel-body">
                    <div class="form-group">
                        <label class="col-md-2 control-label">Type Of Bet</label>
                        <div class="col-md-2">
                            <cc1:CDropDownList ID="ddlTypeOfBet" runat="server" CssClass="form-control" hasOptionalItem="true"
                                OptionalText="All" OptionalValue="All" AutoPostBack="true">
                                <asp:ListItem Text="All" Value="All"></asp:ListItem>
                                <asp:ListItem Text="Internet" Value="Internet"></asp:ListItem>
                                <asp:ListItem Text="Phone" Value="Phone"></asp:ListItem>
                            </cc1:CDropDownList>
                        </div>
                        <label class="col-md-2 control-label">From Date</label>
                        <div class="col-md-2">
                            <asp:TextBox CssClass="form-control" ID="txtDateFrom" runat="server" AutoPostBack="true" />
                            <cc1:CalendarExtender ID="ce4" runat="server" TargetControlID="txtDateFrom" />
                        </div>
                        <label class="col-md-2 control-label">To Date</label>
                        <div class="col-md-2">
                            <asp:TextBox CssClass="form-control" ID="txtDateTo" runat="server" AutoPostBack="true" />
                            <cc1:CalendarExtender ID="ce5" runat="server" TargetControlID="txtDateTo" />
                        </div>
                    </div>
                </div>
            </div>

            <div class="panel panel-grey">
                <div class="panel-heading">Results</div>
                <div class="panel-body">
                    <uc1:historyGrid ID="ucHistoryGrid" runat="server" />
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
