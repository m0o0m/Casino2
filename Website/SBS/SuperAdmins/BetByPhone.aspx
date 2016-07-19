<%@ Page Language="VB" MasterPageFile="~/SBS/SuperAdmins/SuperAdmin.master" AutoEventWireup="false"
    CodeFile="BetByPhone.aspx.vb" Inherits="SBSSuperAdmin.Betbyphone" Title="Untitled Page" %>

<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
    <div class="row mbl">
        <div class="col-lg-12">
            <span>Account Name</span>
            <cc1:CDropDownList ID="ddlAgents" runat="server" hasOptionalItem="true" OptionalText="All Account Name"
                OptionalValue="" AutoPostBack="true" CssClass="form-control" Style="display: inline-block;" Width="230px">
            </cc1:CDropDownList>
            &nbsp;
            <span>Player ID</span>
            <cc1:CDropDownList ID="ddlPlayers" runat="server" hasOptionalItem="true" OptionalText="All Player"
                OptionalValue="" AutoPostBack="true" CssClass="form-control" Style="display: inline-block;" Width="230px">
            </cc1:CDropDownList>
            &nbsp;
            <span>Week</span>
            <cc1:CDropDownList ID="ddlWeeks" runat="server" CssClass="form-control" Style="display: inline-block;" Width="250px"
                hasOptionalItem="false" AutoPostBack="true" />
        </div>

    </div>
    <asp:DataGrid runat="server" ID="dtgLogs" AutoGenerateColumns="false" HeaderStyle-HorizontalAlign="Center"
        PageSize="30" AllowPaging="true" CssClass="table table-hover table-bordered">
        <PagerStyle Mode="NumericPages" HorizontalAlign="Right" />
        <Columns>
            <asp:BoundColumn HeaderText="Account Name" DataField="FullName"></asp:BoundColumn>
            <asp:BoundColumn HeaderText="Player ID" DataField="PlayerName"></asp:BoundColumn>
            <asp:BoundColumn HeaderText="Ticket" DataField="TicketNumber"></asp:BoundColumn>
            <asp:TemplateColumn HeaderText="Note">
                <ItemTemplate>
                    <%#SBCBL.std.SafeString(Container.DataItem("Note")).Replace(vbLf, "<br/>")%>
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn HeaderText="Date">
                <ItemTemplate>
                    <%#UserSession.ConvertToEST(Container.DataItem("CreatedDate"))%>
                </ItemTemplate>
            </asp:TemplateColumn>
        </Columns>

        <HeaderStyle HorizontalAlign="Center" CssClass="tableheading"></HeaderStyle>
    </asp:DataGrid>
</asp:Content>
