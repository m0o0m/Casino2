<%@ Control Language="VB" AutoEventWireup="false" CodeFile="YearlyTotalDetail.ascx.vb"
    Inherits="SBCSuperAdmin.YearlyTotalDetail" %>
    <%@ Register Src="~/Inc/DateTime.ascx" TagName="DateTime" TagPrefix="uc" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>
<table style="float: left;">
    <tr id="trTop" runat="server">
        <td>
            Weeks:
            <wlb:CDropDownList ID="ddlWeeks" runat="server" CssClass="textInput" hasOptionalItem="false"
                AutoPostBack="true" />
        </td>
    </tr>
     <tr>
        <td>
            Straight
        </td>
    </tr>
    <tr>
        <td>
            <asp:DataGrid ID="dgStraight" runat="server" Width="100%" AutoGenerateColumns="false"
                CellPadding="2" CellSpacing="2" CssClass="gamebox">
                <HeaderStyle CssClass="tableheading" HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Left" Wrap="false" />
                <AlternatingItemStyle HorizontalAlign="Left" Wrap="false" />
                <Columns>
                    <asp:TemplateColumn ItemStyle-HorizontalAlign="Center" HeaderText="Total Of Bet"
                        ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:Label ID="lblTotalOfBet" runat="server" Text='<%# Eval("TotalOfBet") %>' />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn ItemStyle-HorizontalAlign="Center" HeaderText="Total Bet Amount"
                        ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:Label ID="lblTotalBetAmount" runat="server" Text='<%# Eval("TotalBetAmount") %>' />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn ItemStyle-HorizontalAlign="Center" HeaderText="Total Win/Lose"
                        ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:Label ID="lblTotalWinLose" runat="server" Text='<%# Eval("TotalWinLose") %>' />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn ItemStyle-HorizontalAlign="Center" HeaderText="P/L Percentage"
                        ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:Label ID="lblPlPercentage" runat="server" Text='<%# Eval("PLPercentage") %>'/>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
            </asp:DataGrid>
        </td>
    </tr>
    <tr>
        <td>
            Parlay + Reverse + Teaser
        </td>
    </tr>
    <tr>
        <td>
            <asp:DataGrid ID="dgOther" runat="server" Width="100%" AutoGenerateColumns="false"
                CellPadding="2" CellSpacing="2" CssClass="gamebox">
                <HeaderStyle CssClass="tableheading" HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Left" Wrap="false" />
                <AlternatingItemStyle HorizontalAlign="Left" Wrap="false" />
                <Columns>
                    <asp:TemplateColumn ItemStyle-HorizontalAlign="Center" HeaderText="Total Of Bet"
                        ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:Label ID="lblTotalOfBet" runat="server" Text='<%# Eval("TotalOfBet") %>' />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn ItemStyle-HorizontalAlign="Center" HeaderText="Total Bet Amount"
                        ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:Label ID="lblTotalBetAmount" runat="server" Text='<%# Eval("TotalBetAmount") %>' />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn ItemStyle-HorizontalAlign="Center" HeaderText="Total Win/Lose"
                        ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:Label ID="lblTotalWinLose" runat="server" Text='<%# Eval("TotalWinLose") %>' />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn ItemStyle-HorizontalAlign="Center" HeaderText="P/L Percentage"
                        ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:Label ID="lblPlPercentage" runat="server" Text='<%# Eval("PLPercentage") %>' />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
            </asp:DataGrid>
        </td>
    </tr>
    
</table>