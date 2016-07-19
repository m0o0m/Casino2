<%@ Control Language="VB" AutoEventWireup="false" CodeFile="PLReport.ascx.vb" Inherits="SBCSuperAdmin.PLReport" %>
<%@ Register Src="~/Inc/DateTime.ascx" TagName="DateTime" TagPrefix="uc" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>


<div class="panel panel-grey">
    <div class="panel-heading"></div>
    <div class="panel-body">
        <div id="trTop" runat="server">
            <div class="form-group">
                <label class="col-md-1 control-label">Agents</label>
                <div class="col-md-4">
                    <wlb:CDropDownList ID="ddlAgents" runat="server" CssClass="form-control" hasOptionalItem="false"
                        AutoPostBack="true" />
                </div>
            </div>
            <div id="pnlWeeks" runat="server" class="form-group">
                <label class="col-md-1 control-label">Weeks</label>
                <div class="col-md-4">
                    <wlb:CDropDownList ID="ddlWeeks" runat="server" CssClass="form-control" hasOptionalItem="false"
                        AutoPostBack="true" />
                </div>
            </div>
        </div>
        <asp:DataGrid ID="dgDailyPLReport" runat="server" AutoGenerateColumns="false" 
                CssClass="table table-hover table-bordered">
                <HeaderStyle CssClass="tableheading" HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Left" Wrap="false" />
                <AlternatingItemStyle HorizontalAlign="Left" Wrap="false" />
                <Columns>
                    <asp:TemplateColumn ItemStyle-HorizontalAlign="Center" HeaderText="Day" ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:Label ID="lblDay" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn ItemStyle-HorizontalAlign="Center" HeaderText="Handle" ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:Label ID="lblHandle" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn ItemStyle-HorizontalAlign="Center" HeaderText="Net" ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:Label ID="lblNet" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn ItemStyle-HorizontalAlign="Center" HeaderText="Hold %" ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:Label ID="lblHold" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
            </asp:DataGrid>
    </div>
</div>

<div class="panel panel-grey">
    <div class="panel-heading">Straight</div>
    <div class="panel-body">
        <asp:DataGrid ID="dgStraight" runat="server" AutoGenerateColumns="false"
                CssClass="table table-hover table-bordered">
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
                            <span style='color: <%# IIf(SBCBL.std.SafeDouble(Container.DataItem("TotalWinLose"))<0,"red","black") %>'>
                                <asp:Literal ID="lblTotalWinLose" runat="server" Text='<%# Eval("TotalWinLose") %>' />
                            </span>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn ItemStyle-HorizontalAlign="Center" HeaderText="P/L Percentage"
                        ItemStyle-Width="150px">
                        <ItemTemplate>
                            <span style='color: <%# IIf(SBCBL.std.SafeDouble(Container.DataItem("PLPercentage").replace("%",""))<0,"red","black") %>'>
                                <asp:Literal ID="lblPlPercentage" runat="server" Text='<%# Eval("PLPercentage") %>' />
                            </span>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
            </asp:DataGrid>
    </div>
</div>

<div class="panel panel-grey">
    <div class="panel-heading">Parlay + Reverse + Teaser</div>
    <div class="panel-body">
        <asp:DataGrid ID="dgOther" runat="server"  AutoGenerateColumns="false"
                CssClass="table table-hover table-bordered">
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
                            <span style='color: <%# IIf(SBCBL.std.SafeDouble(Container.DataItem("TotalWinLose"))<0,"red","black") %>'>
                                <asp:Literal ID="lblTotalWinLose" runat="server" Text='<%# Eval("TotalWinLose") %>' />
                            </span>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn ItemStyle-HorizontalAlign="Center" HeaderText="P/L Percentage"
                        ItemStyle-Width="150px">
                        <ItemTemplate>
                            <span style='color: <%# IIf(SBCBL.std.SafeDouble(Container.DataItem("PLPercentage").replace("%",""))<0,"red","black") %>'>
                                <asp:Literal ID="lblPlPercentage" runat="server" Text='<%# Eval("PLPercentage") %>' />
                            </span>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
            </asp:DataGrid>
    </div>
</div>

