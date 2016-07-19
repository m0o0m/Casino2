<%@ Control Language="VB" AutoEventWireup="false" CodeFile="SummaryReport.ascx.vb"
    Inherits="SBCWebsite.SummaryReport" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>

<div class="panel panel-grey">
    <div class="panel-heading"></div>
    <div class="panel-body">
        <div class="form-group">
            <label class="control-label col-md-1">Weeks</label>
            <div class="col-md-9">
                <wlb:CDropDownList ID="ddlWeeks" runat="server" AutoPostBack="true" CssClass="form-control" hasOptionalItem="false"
                    Width="220px" />
            </div>
        </div>
    </div>
</div>

<div class="panel panel-grey">
    <div class="panel-heading"></div>
    <div class="panel-body">
        <table border="0" width="100%">
            <tr>
                <td valign="top">
                    <asp:Repeater ID="rptMain" runat="server">
                        <ItemTemplate>
                            <h2>
                                <asp:Literal ID="lblAgentWeekly" runat="server" Text=""></asp:Literal></h2>
                            <asp:DataGrid ID="dgSubPlayers" runat="server" AutoGenerateColumns="false"
                                OnItemDataBound="dgSubPlayers_ItemDataBound" CssClass="table table-hover table-bordered">
                                <HeaderStyle CssClass="tableheading" HorizontalAlign="Center" />
                                <Columns>
                                    <asp:TemplateColumn HeaderText="Player" ItemStyle-Width="230">
                                        <ItemTemplate>
                                            <asp:Label ID="lblPlayer" runat="server" Text='<%# Container.DataItem("Player")%>'></asp:Label>
                                            <br />
                                            <asp:HyperLink ID="hlCreditBack" Style="font-size: 8pt; text-decoration: none" runat="server">Credit Back</asp:HyperLink>
                                            <asp:HyperLink ID="hlHistoricalAmount" Style="text-decoration: none" runat="server" ToolTip="Statistical Report">Statistics</asp:HyperLink>

                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText='Mon' ItemStyle-HorizontalAlign="Right" ItemStyle-Width="70px">
                                        <ItemTemplate>
                                            <span style='color: <%# IIf(SBCBL.std.SafeDouble(Container.DataItem("Mon"))<0,"red","blue") %>'>
                                                <asp:Label ID="lblMon" runat="server" Text='<%# FormatNumber(SBCBL.std.SafeDouble(Container.DataItem("Mon")), SBCBL.std.GetRoundMidPoint) %>'></asp:Label>
                                            </span>
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Tue" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="70px">
                                        <ItemTemplate>
                                            <span style='color: <%# IIf(SBCBL.std.SafeDouble(Container.DataItem("Tues"))<0,"red","blue") %>'>
                                                <asp:Label ID="lblTues" runat="server" Text='<%# FormatNumber(SBCBL.std.SafeDouble(Container.DataItem("Tues")), SBCBL.std.GetRoundMidPoint) %>'></asp:Label>
                                            </span>
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Wed" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="70px">
                                        <ItemTemplate>
                                            <span style='color: <%# IIf(SBCBL.std.SafeDouble(Container.DataItem("Wed"))<0,"red","blue") %>'>
                                                <asp:Label ID="lblWed" runat="server" Text='<%# FormatNumber(SBCBL.std.SafeDouble(Container.DataItem("Wed")), SBCBL.std.GetRoundMidPoint) %>'></asp:Label>
                                            </span>
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Thurs" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="70px">
                                        <ItemTemplate>
                                            <span style='color: <%# IIf(SBCBL.std.SafeDouble(Container.DataItem("Thurs"))<0,"red","blue") %>'>
                                                <asp:Label ID="lblThurs" runat="server" Text='<%# FormatNumber(SBCBL.std.SafeDouble(Container.DataItem("Thurs")), SBCBL.std.GetRoundMidPoint) %>'></asp:Label>
                                            </span>
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Fri" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="70px">
                                        <ItemTemplate>
                                            <span style='color: <%# IIf(SBCBL.std.SafeDouble(Container.DataItem("Fri"))<0,"red","blue") %>'>
                                                <asp:Label ID="lblFri" runat="server" Text='<%# FormatNumber(SBCBL.std.SafeDouble(Container.DataItem("Fri")), SBCBL.std.GetRoundMidPoint) %>'></asp:Label>
                                            </span>
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Sat" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="70px">
                                        <ItemTemplate>
                                            <span style='color: <%# IIf(SBCBL.std.SafeDouble(Container.DataItem("Sat"))<0,"red","blue") %>'>
                                                <asp:Label ID="lblSat" runat="server" Text='<%# FormatNumber(SBCBL.std.SafeDouble(Container.DataItem("Sat")), SBCBL.std.GetRoundMidPoint) %>'></asp:Label>
                                            </span>
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Sun" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="70px">
                                        <ItemTemplate>
                                            <span style='color: <%# IIf(SBCBL.std.SafeDouble(Container.DataItem("Sun"))<0,"red","blue") %>'>
                                                <asp:Label ID="lblSun" runat="server" Text='<%# FormatNumber(SBCBL.std.SafeDouble(Container.DataItem("Sun")), SBCBL.std.GetRoundMidPoint) %>'></asp:Label>
                                            </span>
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Gross" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="70px">
                                        <ItemTemplate>
                                            <span style='color: <%# IIf(SBCBL.std.SafeDouble(Container.DataItem("Gross"))<0,"red","blue") %>'>
                                                <asp:Label ID="lblGross" runat="server" Text='<%# FormatNumber(SBCBL.std.SafeDouble(Container.DataItem("Gross")), SBCBL.std.GetRoundMidPoint) %>'></asp:Label>
                                            </span>
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Net" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="70px">
                                        <ItemTemplate>
                                            <span style='color: <%# IIf(SBCBL.std.SafeDouble(Container.DataItem("Net"))<0,"red","blue") %>'>
                                                <asp:Label ID="lblNet" runat="server" Text='<%# FormatNumber(SBCBL.std.SafeDouble(Container.DataItem("Net")), SBCBL.std.GetRoundMidPoint) %>'></asp:Label>
                                            </span>
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="P/L" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="70px">
                                        <ItemTemplate>
                                            <span style='color: <%# IIf(SBCBL.std.SafeDouble(Container.DataItem("PL"))<0,"red","blue") %>'>
                                                <asp:Label ID="lblPL" runat="server" Text='<%# FormatNumber(SBCBL.std.SafeDouble(Container.DataItem("PL")), SBCBL.std.GetRoundMidPoint) %>'></asp:Label>
                                            </span>
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Pending" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="70px">
                                        <ItemTemplate>
                                            <asp:Label ID="lblPending" Text='<%#Container.DataItem("Pending") %>' runat="server"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateColumn>

                                    <asp:TemplateColumn Visible="false" HeaderText="This Week" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="70px">
                                        <ItemTemplate>
                                            <asp:Label ID="lblThisWeek" runat="server" Text='<%#FormatNumber((SBCBL.std.SafeDouble(Container.DataItem("Mon"))+SBCBL.std.SafeDouble(Container.DataItem("Tues"))+SBCBL.std.SafeDouble(Container.DataItem("Wed"))+SBCBL.std.SafeDouble(Container.DataItem("Thurs"))+SBCBL.std.SafeDouble(Container.DataItem("Fri"))+SBCBL.std.SafeDouble(Container.DataItem("Sat"))+SBCBL.std.SafeDouble(Container.DataItem("Sun"))), SBCBL.std.GetRoundMidPoint)%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Total Balance" ItemStyle-HorizontalAlign="center" Visible="false"
                                        ItemStyle-Width="70px">
                                        <ItemTemplate>
                                            <asp:Label ID="lblTotalBalance" runat="server" Text='<%#FormatNumber((SBCBL.std.SafeDouble(Container.DataItem("Mon"))+SBCBL.std.SafeDouble(Container.DataItem("Tues"))+SBCBL.std.SafeDouble(Container.DataItem("Wed"))+SBCBL.std.SafeDouble(Container.DataItem("Thurs"))+SBCBL.std.SafeDouble(Container.DataItem("Fri"))+SBCBL.std.SafeDouble(Container.DataItem("Sat"))+SBCBL.std.SafeDouble(Container.DataItem("Sun"))), SBCBL.std.GetRoundMidPoint)%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                </Columns>
                            </asp:DataGrid>
                        </ItemTemplate>
                    </asp:Repeater>
                </td>
                <td align="right" valign="top" style="width: 350px" id="trChart" runat="server">
                    <object classid="clsid:d27cdb6e-ae6d-11cf-96b8-444553540000" codebase="/FusionCharts_Enterprise/swflash.cab"
                        width="350" height="250" id="chart1">
                        <param name="movie" value="/FusionCharts_Enterprise/Charts/MSLine.swf" />
                        <%="<param name=""FlashVars"" value=""&dataXML="%><asp:Literal ID="lblXML1" runat="server"></asp:Literal>
                        <%="""/>"%>
                        <param name="wmode" value="opaque" />
                        <param name="quality" value="high" />
                        <embed src="/FusionCharts_Enterprise/Charts/MSLine.swf" flashvars="&dataXML=<%= lblXML1.Text%>"
                            quality="high" bgcolor="#ffffff" wmode="opaque" width="350" height="250" name="MSLine"
                            align="middle" type="application/x-shockwave-flash" pluginspage="http://www.macromedia.com/go/getflashplayer" />
                    </object>
                </td>
            </tr>
        </table>
    </div>
</div>



