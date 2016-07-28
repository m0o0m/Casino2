<%@ Page Language="VB" MasterPageFile="Player.master" AutoEventWireup="false" CodeFile="OpenBet.aspx.vb"
    Inherits="SBSPlayer.OpenBet" %>

<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="cc1" %>
<%@ Register Src="Inc/ticketBetsGrid.ascx" TagName="ticketBetsGrid" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="Server">

   
    <div class="clear mgB5">
        <button class="button-style-2 w110px">Print</button>
    </div>
    <div class="table-filter-style-1 pdTB10 pdLR10">
        <div class="ly-fixed">
            <div class="text-left">
                <span class="fz12 bold clr-white mgR80">Filter:</span>
                <label class="col-md-2 control-label">Type Of Bet</label>
                    <cc1:CDropDownList ID="ddlTypeOfBet" runat="server" CssClass="select-field-3" hasOptionalItem="true"
                        OptionalText="All" OptionalValue="All" AutoPostBack="true">
                        <asp:ListItem Text="All" Value="All"></asp:ListItem>
                        <asp:ListItem Text="Internet" Value="Internet"></asp:ListItem>
                        <asp:ListItem Text="Phone" Value="Phone"></asp:ListItem>
                    </cc1:CDropDownList>
                <label class="col-md-1 control-label">Context</label>
                    <cc1:CDropDownList ID="ddlContext" runat="server" CssClass="select-field-3" AutoPostBack="true">
                        <asp:ListItem Text="All" Value=""></asp:ListItem>
                        <asp:ListItem Text="Current" Value="Current"></asp:ListItem>
                        <asp:ListItem Text="1H" Value="1H"></asp:ListItem>
                        <asp:ListItem Text="2H" Value="2H"></asp:ListItem>
                        <asp:ListItem Text="Quarters" Value="Q"></asp:ListItem>
                        <asp:ListItem Text="Proposition" Value="Proposition"></asp:ListItem>
                    </cc1:CDropDownList>
                <label class="col-md-1 control-label">Horses</label>
                    <cc1:CDropDownList ID="ddlHorse" runat="server" CssClass="select-field-3" AutoPostBack="true" Enabled="False">
                        <asp:ListItem Text="All" Value=""></asp:ListItem>
                    </cc1:CDropDownList>
                <label class="col-md-1 control-label">Games</label>
                    <cc1:CDropDownList ID="ddlGameType" runat="server" CssClass="select-field-3" AutoPostBack="true">
                        <asp:ListItem Text="All" Value=""></asp:ListItem>
                        <asp:ListItem Text="Football" Value="Football"></asp:ListItem>
                        <asp:ListItem Text="Baseball" Value="Baseball"></asp:ListItem>
                        <asp:ListItem Text="Basketball" Value="Basketball"></asp:ListItem>
                        <asp:ListItem Text="Hockey" Value="Hockey"></asp:ListItem>
                        <asp:ListItem Text="Soccer" Value="Soccer"></asp:ListItem>
                    </cc1:CDropDownList>
                <label class="col-md-1 control-label">Free Plays</label>
                    <cc1:CDropDownList ID="ddlFreePlay" runat="server" CssClass="select-field-3" AutoPostBack="true" Enabled="False">
                        <asp:ListItem Text="All" Value=""></asp:ListItem>
                    </cc1:CDropDownList>
            </div>
        </div>
    </div>
    <div class="panel panel-grey">
        <%--<div class="panel-heading">Result</div>--%>
        <div class="panel-body">
            <asp:UpdatePanel ID="up1" runat="server">
                <ContentTemplate>
                    <uc1:ticketBetsGrid ID="ucTicketBetsGrid" runat="server" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

</asp:Content>
