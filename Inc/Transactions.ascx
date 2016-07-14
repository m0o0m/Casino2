<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Transactions.ascx.vb"
    Inherits="SBCWebsite.Inc_Transactions" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>
<div class="panel panel-grey">
    <div class="panel-heading"></div>
    <div class="panel-body">

        <table class="table table-hover table-bordered">
            <thead>
                <tr>
                    <th>Transaction's Info
                    </th>
                    <th id="tdUserInfoDisp" runat="server">
                        <asp:Label ID="lblUserInfo" runat="server"></asp:Label>
                    </th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td>
                        <div class="form-group">
                            <asp:Label ID="lblDispCurrentBalance" runat="server" CssClass="control-label col-md-4"></asp:Label>
                            <asp:Label ID="lblCurrentBalance" runat="server" CssClass="control-label col-md-6"></asp:Label>
                        </div>
                        <div class="form-group">
                            <asp:Label ID="lblUser" runat="server" Text="Player" CssClass="control-label col-md-6"></asp:Label>
                            <div class="col-md-6">
                                <wlb:CDropDownList ID="ddlUsers" runat="server" CssClass="form-control" hasOptionalItem="true"
                                    OptionalValue="" OptionalText="All" AutoPostBack="true" />
                            </div>
                        </div>
                    </td>
                    <td>
                        <div id="tdUserInfo" runat="server" class="form-group">
                            <asp:Label ID="lblPlayer" runat="server" Text="Player" CssClass="control-label col-md-2"></asp:Label>
                            <div class="col-md-6">
                                <wlb:CDropDownList ID="ddlPlayer" runat="server" CssClass="form-control" hasOptionalItem="true"
                                    OptionalValue="" OptionalText="All" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="col-md-6">
                                <asp:Button ID="btnReset" runat="server" Text="Reset Original Balance: " CssClass="btn btn-primary"
                                    ToolTip="Reset To Original Balance" Enabled="false" Visible="false" />
                            </div>
                            <div class="col-md-6">
                                <asp:Button ID="btnResetZero" runat="server" Text="Reset To Zero" CssClass="btn btn-primary" Visible="false"
                                    ToolTip="Don't allow player betting" />
                            </div>
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>


        <asp:DataGrid ID="grdTransaction" runat="server" AutoGenerateColumns="false"
            CssClass="table table-hover table-bordered"
            AllowPaging="True" PageSize="30">
            <HeaderStyle CssClass="tableheading" HorizontalAlign="Center" />
            <ItemStyle HorizontalAlign="Left" BorderColor="Black" BorderWidth="1px" />
            <PagerStyle Font-Bold="False"
                HorizontalAlign="Right" Mode="NumericPages"
                Font-Names="tahoma" Font-Size="10pt" />
            <AlternatingItemStyle HorizontalAlign="Left" BorderColor="Black" BorderWidth="1px" />
            <SelectedItemStyle BackColor="YellowGreen" />
            <PagerStyle CssClass="gridpager" />
            <Columns>
                <asp:TemplateColumn HeaderText="Login (Name)" ItemStyle-HorizontalAlign="Left">
                    <ItemTemplate>
                        <%#Container.DataItem("FullName")%>
                    </ItemTemplate>

                    <ItemStyle HorizontalAlign="Left"></ItemStyle>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Transaction Date" ItemStyle-HorizontalAlign="Left">
                    <ItemTemplate>
                        <%#IIf(SBCBL.std.SafeString(Container.DataItem("TransactionDate")) <> "", UserSession.ConvertToEST(SBCBL.std.SafeString(Container.DataItem("TransactionDate"))), "")%>
                    </ItemTemplate>

                    <ItemStyle HorizontalAlign="Left"></ItemStyle>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Amount Owed" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <asp:Literal ID="lblAmountOwed" runat="server"></asp:Literal>
                    </ItemTemplate>

                    <ItemStyle HorizontalAlign="Right"></ItemStyle>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Amount Paid" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <asp:Literal ID="lblAmountPaid" runat="server"></asp:Literal>
                    </ItemTemplate>

                    <ItemStyle HorizontalAlign="Right"></ItemStyle>
                </asp:TemplateColumn>
                <asp:BoundColumn DataField="TransactionType" HeaderText="Type"
                    ItemStyle-Width="85" ItemStyle-HorizontalAlign="Center">
                    <ItemStyle HorizontalAlign="Center" Width="85px"></ItemStyle>
                </asp:BoundColumn>
                <asp:BoundColumn DataField="Description" HeaderText="Notes"
                    ItemStyle-HorizontalAlign="Center">
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                </asp:BoundColumn>
            </Columns>
        </asp:DataGrid>
    </div>
</div>
