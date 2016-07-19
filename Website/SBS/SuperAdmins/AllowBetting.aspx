<%@ Page Language="VB" MasterPageFile="~/SBS/SuperAdmins/SuperAdmin.master" AutoEventWireup="false"
    CodeFile="AllowBetting.aspx.vb" Inherits="SBSSuperAdmin.AllowBetting" %>
    <%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
    <table class="table table-hover">
        <tr>
            <td>
                <b>Daylight Saving Time</b>
            </td>
            <td colspan="2">
                <asp:CheckBox ID="chkDST" runat="server" Style="margin-left: 5px" AutoPostBack="true" /><span
                    style="font-weight: bold; position: relative; bottom: 2px">DST</span>
            </td>
        </tr>
        <tr>
            <td>
                <b>Enable Impersonate User</b>
            </td>
            <td colspan="2">
                <asp:CheckBox ID="chkImpersonate" runat="server" Style="margin-left: 5px" AutoPostBack="true" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:HiddenField ID="hfBettingID" runat="server" />
                Betting Enable:
            </td>
            <td>
                <asp:Label ID="lblBettingEnble" runat="server"></asp:Label>
            </td>
            <td>
                <asp:Button ID="btnChangeBetting" Text="Change" ToolTip="Change Betting" runat="server"
                    CssClass="button" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:HiddenField ID="hfOverrideBettingID" runat="server" />
                Override Betting Enable:
            </td>
            <td>
                <asp:Label ID="lblOverrideBetting" runat="server"></asp:Label>
            </td>
            <td>
                <asp:Button ID="btnOverrideBetting" Text="Change" ToolTip="Change Override Betting"
                    runat="server" CssClass="button" />
            </td>
        </tr>
         <tr>
            <td>
                Purging Data :
            </td>
            <td>
                <wlb:CDropDownList ID="ddlSportType" runat="server" AutoPostBack="true" hasOptionalItem="true"
                    CssClass="textInput">
                    <asp:ListItem Value="30" Text="30 Day"></asp:ListItem>
                    <asp:ListItem Value="60" Text="60 Day"></asp:ListItem>
                    <asp:ListItem Value="90" Text="90 Day"></asp:ListItem>
                </wlb:CDropDownList>
            </td>
            <td>
                <asp:Button ID="btnProcess" Text="Process" runat="server" OnClick="btnProcess_Click" CssClass="button"
                    OnClientClick="return confirm('Are you sure delete data ?')" />
            </td>
        </tr>
    </table>
    <table class="table table-hover">
        <tr>
            <td>
                <span>Num Game Row Will Delete </span>
            </td>
            <td>
                <asp:Label ID="lblGameDelete" runat="server" ForeColor="red" Text=""></asp:Label>
            </td>
            <td>
                <asp:LinkButton ID="lbtGameDetail" OnClick="ViewDetailDelete" CommandArgument="Game"
                    runat="server">View Detail</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td>
                <span>Num GameLine Row Will Delete </span>
            </td>
            <td>
                <asp:Label ID="lblGameLineDelete" runat="server" ForeColor="red" Text=""></asp:Label>
            </td>
            <td>
                <asp:LinkButton ID="lbtGameLineDetail" OnClick="ViewDetailDelete" CommandArgument="GameLine"
                    runat="server">View Detail</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td>
                <span>Num Ticket Row Will Delete </span>
            </td>
            <td>
                <asp:Label ID="lblTicketDelete" runat="server" ForeColor="red" Text=""></asp:Label>
            </td>
            <td>
                <asp:LinkButton ID="lbtTicketDetail" OnClick="ViewDetailDelete" CommandArgument="Ticket"
                    runat="server">View Detail</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td>
                <span>Num TicketBet Row Will Delete </span>
            </td>
            <td>
                <asp:Label ID="lblTicketBetDelete" runat="server" ForeColor="red" Text=""></asp:Label>
            </td>
            <td>
                <asp:LinkButton ID="lbtTicketBetDetail" OnClick="ViewDetailDelete" CommandArgument="TicketBet"
                    runat="server">View Detail</asp:LinkButton>
            </td>
        </tr>
    </table>
     <asp:DataGrid ID="dgRowDelete" runat="server" AutoGenerateColumns="true" AllowPaging="true"
        PageSize="50" CssClass="table table-hover table-bordered">
        <HeaderStyle CssClass="tableheading" ForeColor="Black" HorizontalAlign="Center" />
        <ItemStyle HorizontalAlign="Left" />
    </asp:DataGrid>
</asp:Content>
