<%@ Page Language="VB" MasterPageFile="Player.master" AutoEventWireup="false" CodeFile="OpenBet.aspx.vb"
    Inherits="SBSPlayer.OpenBet" %>

<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="cc1" %>
<%@ Register Src="Inc/ticketBetsGrid.ascx" TagName="ticketBetsGrid" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="Server">

    <div class="row">
        <div class="col-lg-12">
            <div class="page-title-breadcrumb">
                <div class="page-header pull-left">
                    <div class="page-title pull-left mrm">
                        Section
                    </div>
                    <span class="label label-grey pull-left"  style="font-size: large">Open Bets</span>
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
                <label class="col-md-1 control-label">Context</label>
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
        <div class="panel-heading">Result</div>
        <div class="panel-body">
            <asp:UpdatePanel ID="up1" runat="server">
                <ContentTemplate>
                    <uc1:ticketBetsGrid ID="ucTicketBetsGrid" runat="server" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

</asp:Content>
