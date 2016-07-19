<%@ Control Language="VB" AutoEventWireup="false" CodeFile="WeeklyAllAgent.ascx.vb"
    Inherits="SBCWebsite.WeeklyAllAgent" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>
<table cellpadding="0" cellspacing="5" width="100%" style="color: white">
    <tr id="trAgent" runat="server">
        <td colspan="2" style="font-weight: bold;">
            <div style="color: white; text-align: center">
                <h2>
                    Weekly Balance for Agent
                    <asp:Literal ID="lblAgent" runat="server"></asp:Literal>
                </h2>
                <h2 style="display: inline">
                    Weekly Ending</h2>
                <wlb:CDropDownList ID="ddlWeeks" runat="server" CssClass="textInput" hasOptionalItem="false"
                    AutoPostBack="false" Width="220px" />
                <asp:ImageButton ID="btnSeach" runat="server" ImageUrl="~/images/Agentweekly.gif"
                    Style="position: relative; top: 6px;" />
                    <br />
                <asp:Button ID="btnPrint" runat="server" Text="Print Friendly" />
                    
            </div>
            <br />
        </td>
    </tr>
    <tr>
        <td valign="top">
            <asp:DataGrid ID="dgSubPlayers" runat="server" Width="100%" AutoGenerateColumns="false"
                Font-Size="12px" CellPadding="2" CellSpacing="2" CssClass="gamebox" Style="min-width: 600px;">
                <HeaderStyle CssClass="tableheading" HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Left" Wrap="false" CssClass="offering_pair_even" />
                <AlternatingItemStyle HorizontalAlign="Left" Wrap="false" CssClass="offering_pair_odd" />
                <Columns>
                    <asp:TemplateColumn HeaderText="Agent" ItemStyle-HorizontalAlign="left" HeaderStyle-BackColor="#a50502" FooterText="aaa">
                        <ItemTemplate>
                            <asp:Label ID="lblAgent" runat="server" Text='<%# Container.DataItem("Player").split("|")(0)%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText='Mon' ItemStyle-HorizontalAlign="Right" ItemStyle-Width="70px"
                        HeaderStyle-BackColor="#a50502">
                        <ItemTemplate>
                            <span style='color: <%# IIf(SBCBL.std.SafeDouble(Container.DataItem("Mon"))<0,"red","blue") %>'>
                                <asp:LinkButton  ID="lbtMon" OnClick ="changeAgent_Click"  runat="server" ForeColor="White" Text='<%# FormatNumber(SBCBL.std.SafeDouble(Container.DataItem("Mon")), SBCBL.std.GetRoundMidPoint) %>' CommandArgument='<%# Container.DataItem("Player").split("|"c)(1)%>'></asp:LinkButton>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Tue" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="70px"
                        HeaderStyle-BackColor="#a50502">
                        <ItemTemplate>
                            <span style='color: <%# IIf(SBCBL.std.SafeDouble(Container.DataItem("Tues"))<0,"red","blue") %>'>
                                <asp:LinkButton ID="lbtTues" OnClick ="changeAgent_Click" runat="server" ForeColor="White" Text='<%# FormatNumber(SBCBL.std.SafeDouble(Container.DataItem("Tues")), SBCBL.std.GetRoundMidPoint) %>' CommandArgument='<%# Container.DataItem("Player").split("|"c)(1)%>'></asp:LinkButton>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Wed" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="70px"
                        HeaderStyle-BackColor="#a50502">
                        <ItemTemplate>
                            <span style='color: <%# IIf(SBCBL.std.SafeDouble(Container.DataItem("Wed"))<0,"red","blue") %>'>
                                <asp:LinkButton ID="lbtWed" OnClick ="changeAgent_Click" runat="server" ForeColor="White" Text='<%# FormatNumber(SBCBL.std.SafeDouble(Container.DataItem("Wed")), SBCBL.std.GetRoundMidPoint) %>' CommandArgument='<%# Container.DataItem("Player").split("|"c)(1)%>'></asp:LinkButton>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Thurs" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="70px"
                        HeaderStyle-BackColor="#a50502">
                        <ItemTemplate>
                            <span style='color: <%# IIf(SBCBL.std.SafeDouble(Container.DataItem("Thurs"))<0,"red","blue") %>'>
                                <asp:LinkButton ID="lbtThurs" OnClick ="changeAgent_Click" runat="server" ForeColor="White" Text='<%# FormatNumber(SBCBL.std.SafeDouble(Container.DataItem("Thurs")), SBCBL.std.GetRoundMidPoint) %>' CommandArgument='<%# Container.DataItem("Player").split("|"c)(1)%>'></asp:LinkButton>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Fri" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="70px"
                        HeaderStyle-BackColor="#a50502">
                        <ItemTemplate>
                            <span style='color: <%# IIf(SBCBL.std.SafeDouble(Container.DataItem("Fri"))<0,"red","blue") %>'>
                                <asp:LinkButton ID="lbtFri" OnClick ="changeAgent_Click" runat="server" ForeColor="White" Text='<%# FormatNumber(SBCBL.std.SafeDouble(Container.DataItem("Fri")), SBCBL.std.GetRoundMidPoint) %>' CommandArgument='<%# Container.DataItem("Player").split("|"c)(1)%>'></asp:LinkButton>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Sat" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="70px"
                        HeaderStyle-BackColor="#a50502">
                        <ItemTemplate>
                            <span style='color: <%# IIf(SBCBL.std.SafeDouble(Container.DataItem("Sat"))<0,"red","blue") %>'>
                                <asp:LinkButton ID="lbtSat" OnClick ="changeAgent_Click" runat="server" ForeColor="White" Text='<%# FormatNumber(SBCBL.std.SafeDouble(Container.DataItem("Sat")), SBCBL.std.GetRoundMidPoint) %>' CommandArgument='<%# Container.DataItem("Player").split("|"c)(1)%>'></asp:LinkButton>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Sun" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="70px"
                        HeaderStyle-BackColor="#a50502">
                        <ItemTemplate>
                            <span style='color: <%# IIf(SBCBL.std.SafeDouble(Container.DataItem("Sun"))<0,"red","blue") %>'>
                                <asp:LinkButton ID="lbtSun" OnClick ="changeAgent_Click" runat="server" ForeColor="White" Text='<%# FormatNumber(SBCBL.std.SafeDouble(Container.DataItem("Sun")), SBCBL.std.GetRoundMidPoint) %>' CommandArgument='<%# Container.DataItem("Player").split("|"c)(1)%>'></asp:LinkButton>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    
                    <%--<asp:TemplateColumn HeaderText="Net" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="70px"
                        Visible="false" HeaderStyle-BackColor="#a50502">
                        <ItemTemplate>
                            <span style='color: <%# IIf(SBCBL.std.SafeDouble(Container.DataItem("Net"))<0,"red","blue") %>'>
                                <asp:Label ID="lblNet" ForeColor="White" runat="server" Text='<%# FormatNumber(SBCBL.std.SafeDouble(Container.DataItem("Net")), SBCBL.std.GetRoundMidPoint) %>'></asp:Label>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="P/L" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="70px"
                        HeaderStyle-BackColor="#a50502">
                        <ItemTemplate>
                            <span style='color: <%# IIf(SBCBL.std.SafeDouble(Container.DataItem("PL"))<0,"red","blue") %>'>
                                <asp:Label ID="lblPL" ForeColor="White" runat="server" Text='<%# FormatNumber(SBCBL.std.SafeDouble(Container.DataItem("PL")), SBCBL.std.GetRoundMidPoint) %>'></asp:Label>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateColumn>--%>
                    
                    <asp:TemplateColumn HeaderText="Pending" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="70px"
                        HeaderStyle-BackColor="#a50502">
                        <ItemTemplate>
                            <asp:Label ID="lblPending" ForeColor="White" runat="server" Text='<%# FormatNumber(SBCBL.std.SafeDouble(Container.DataItem("Pending")), SBCBL.std.GetRoundMidPoint) %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    
                    <asp:TemplateColumn HeaderText="This Week" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="70px"
                        HeaderStyle-BackColor="#a50502">
                        <ItemTemplate>
                            <asp:Label ID="lblThisWeek" ForeColor="black" runat="server" Text='<%#FormatNumber((SBCBL.std.SafeDouble(Container.DataItem("Mon"))+SBCBL.std.SafeDouble(Container.DataItem("Tues"))+SBCBL.std.SafeDouble(Container.DataItem("Wed"))+SBCBL.std.SafeDouble(Container.DataItem("Thurs"))+SBCBL.std.SafeDouble(Container.DataItem("Fri"))+SBCBL.std.SafeDouble(Container.DataItem("Sat"))+SBCBL.std.SafeDouble(Container.DataItem("Sun"))), SBCBL.std.GetRoundMidPoint)%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText=" Non-posted Casino" ItemStyle-HorizontalAlign="center"
                        ItemStyle-Width="70px" HeaderStyle-BackColor="#a50502">
                        <ItemTemplate>
                            <span style="color: black">
                                <asp:Label ID="lblCasino" ForeColor="White" runat="server" Text='0.00'></asp:Label>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Total Balance" ItemStyle-HorizontalAlign="center"
                        ItemStyle-Width="70px" HeaderStyle-BackColor="#a50502">
                        <ItemTemplate>
                            <asp:Label ID="lblTotalBalance" ForeColor="White" runat="server" Text='<%#FormatNumber((SBCBL.std.SafeDouble(Container.DataItem("Mon"))+SBCBL.std.SafeDouble(Container.DataItem("Tues"))+SBCBL.std.SafeDouble(Container.DataItem("Wed"))+SBCBL.std.SafeDouble(Container.DataItem("Thurs"))+SBCBL.std.SafeDouble(Container.DataItem("Fri"))+SBCBL.std.SafeDouble(Container.DataItem("Sat"))+SBCBL.std.SafeDouble(Container.DataItem("Sun"))), SBCBL.std.GetRoundMidPoint)%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Head Count" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="70px"
                        HeaderStyle-BackColor="#a50502">
                        <ItemTemplate>
                            <span>
                                <asp:Label ID="lblCountHead" ForeColor="Black"  runat="server" ></asp:Label>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
            </asp:DataGrid>
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
