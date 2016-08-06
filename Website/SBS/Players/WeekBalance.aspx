<%@ Page Language="VB" MasterPageFile="Player.master" AutoEventWireup="false"
    CodeFile="WeekBalance.aspx.vb" Inherits="SBSPlayer.WeekBalance" Title="Untitled Page" %>

<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="Server">
    
    <%--<div class="row" style="display: none">
        <div class="col-lg-12">
            <div class="page-title-breadcrumb">
                <div class="page-header pull-left">
                    <div class="page-title pull-left mrm">
                        Section
                    </div>
                    <span class="label label-grey pull-left"  style="font-size: large">Weekly Balance</span>
                </div>
                <div class="clearfix">
                </div>
            </div>
        </div>
    </div>

    <div class="mbxl"></div>

    <div class="panel panel-grey box-title-1">
        <div class="panel-heading" style="display: none">Filter:</div>
        <div class="panel-body">
            <div class="form-group clear">
                <label class="col-md-2 control-label left mgT5 mgR10 fz12 clr-white">Date Range: </label>
                <div class="col-md-4 left">
                    <wlb:CDropDownList ID="ddlWeeks" runat="server" CssClass="form-control select-field-2 black h26px" hasOptionalItem="false"
                        AutoPostBack="true" OnSelectedIndexChanged="SelectedIndexChanged" />
                </div>
            </div>
        </div>
    </div>--%>

    <div class="panel panel-grey">
        <div class="panel-heading" style="display: none">Result</div>
        <div class="panel-body">
            <asp:DataGrid ID="dgPlayers" runat="server" AutoGenerateColumns="false"
                CssClass="table table-hover table-bordered table-style-2 full-w">
                <HeaderStyle CssClass="tableheading" HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
                <AlternatingItemStyle HorizontalAlign="Center" />
                <Columns>
                    <asp:TemplateColumn HeaderText="Week Start" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="11%">
                        <ItemTemplate>
                            <asp:Literal ID="ltrWeekOf" runat="server"></asp:Literal>
                            <%--<asp:LinkButton ID="lbnWeekOf" runat="server" Text=""/>--%>
                            <asp:HiddenField ID="HFMonday" Value='<%# Container.DataItem("ThisMonday") %>' runat="server" />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Previous <br> Balance" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="110px">
                        <ItemTemplate>
                            0
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Mon" ItemStyle-Width="65px">
                        <ItemTemplate>
                            <asp:LinkButton ID="lbnMon" runat="server" CommandName="VIEW_HISTORY" 
                                CommandArgument='<%# Container.DataItem("ThisMonday")%>' Text='<%#Container.DataItem("Mon")%>' ForeColor='<%# If(Container.DataItem("Mon") < 0, Drawing.ColorTranslator.FromHtml("#b30000"), Drawing.Color.Black)%>' />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Tue" ItemStyle-Width="65px">
                        <ItemTemplate>
                            <asp:LinkButton ID="lbnTues" runat="server" CommandName="VIEW_HISTORY"
                                CommandArgument='<%# SBCBL.std.SafeDate( Container.DataItem("ThisMonday")).AddDays(1) %>'
                                Text='<%#Container.DataItem("Tues")%>'  ForeColor='<%# If(Container.DataItem("Tues") < 0, Drawing.ColorTranslator.FromHtml("#b30000"), Drawing.Color.Black)%>' />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Wed" ItemStyle-Width="65px">
                        <ItemTemplate>
                            <asp:LinkButton ID="lbnWed" runat="server" CommandName="VIEW_HISTORY"
                                CommandArgument='<%# SBCBL.std.SafeDate( Container.DataItem("ThisMonday")).AddDays(2) %>'
                                Text='<%#Container.DataItem("Wed") %>' ForeColor='<%# If(Container.DataItem("Wed") < 0, Drawing.ColorTranslator.FromHtml("#b30000"), Drawing.Color.Black)%>' />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Thu" ItemStyle-Width="65px">
                        <ItemTemplate>
                            <asp:LinkButton ID="lbnThurs" runat="server" CommandName="VIEW_HISTORY" 
                                CommandArgument='<%# SBCBL.std.SafeDate( Container.DataItem("ThisMonday")).AddDays(3) %>'
                                Text='<%#Container.DataItem("Thurs")%>' ForeColor='<%# If(Container.DataItem("Thurs") < 0, Drawing.ColorTranslator.FromHtml("#b30000"), Drawing.Color.Black)%>' />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Fri" ItemStyle-Width="65px">
                        <ItemTemplate>
                            <asp:LinkButton ID="lbnFri" runat="server" CommandName="VIEW_HISTORY" 
                                CommandArgument='<%# SBCBL.std.SafeDate( Container.DataItem("ThisMonday")).AddDays(4) %>'
                                Text='<%#Container.DataItem("Fri")%>' ForeColor='<%# If(Container.DataItem("Fri") < 0, Drawing.ColorTranslator.FromHtml("#b30000"), Drawing.Color.Black)%>' />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Sat" ItemStyle-Width="65px">
                        <ItemTemplate>
                            <asp:LinkButton ID="lbnSat" runat="server" CommandName="VIEW_HISTORY" 
                                CommandArgument='<%# SBCBL.std.SafeDate( Container.DataItem("ThisMonday")).AddDays(5) %>'
                                Text='<%#Container.DataItem("Sat")%>' ForeColor='<%# If(Container.DataItem("Sat") < 0, Drawing.ColorTranslator.FromHtml("#b30000"), Drawing.Color.Black)%>' />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Sun" ItemStyle-Width="65px">
                        <ItemTemplate>
                            <asp:LinkButton ID="lbnSun" runat="server" CommandName="VIEW_HISTORY" 
                                CommandArgument='<%# SBCBL.std.SafeDate( Container.DataItem("ThisMonday")).AddDays(6) %>'
                                Text='<%#Container.DataItem("Sun")%>' ForeColor='<%# If(Container.DataItem("Sun") < 0, Drawing.ColorTranslator.FromHtml("#b30000"), Drawing.Color.Black)%>' />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Weekly <br> Balance" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="90px">
                        <ItemTemplate>
                            0
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Deposits and <br> Withdrawals" ItemStyle-Width="130px">
                        <ItemTemplate>
                            <asp:Label ID="lblTotal" runat="server" Text='<%#SBCBL.std.SafeInteger(Container.DataItem("Net"))%>'
                                 ForeColor='<%# If(Container.DataItem("Net") < 0, Drawing.ColorTranslator.FromHtml("#b30000"), Drawing.Color.Black)%>' ></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Pending" ItemStyle-Width="95px">
                        <ItemTemplate>
                            <%--<asp:LinkButton ID="lbnPending" runat="server"
                                ForeColor='<%# If(Container.DataItem("Pending") < 0, Drawing.ColorTranslator.FromHtml("#b30000"), Drawing.Color.Black)%>' />--%>

                            <asp:Label ID="lblPending" runat="server"
                                 ForeColor='<%# If(Container.DataItem("Pending") < 0, Drawing.ColorTranslator.FromHtml("#b30000"), Drawing.Color.Black)%>' ></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Balance" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="95px">
                        <ItemTemplate>
                            0
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
            </asp:DataGrid>
        </div>
    </div>

    <div id="trchart" runat="server" class="panel panel-grey">
        <div class="panel-heading"></div>
        <div class="panel-body">
            <object classid="clsid:d27cdb6e-ae6d-11cf-96b8-444553540000" codebase="/FusionCharts_Enterprise/swflash.cab"
                width="490" height="350" id="chart1">
                <param name="movie" value="/FusionCharts_Enterprise/Charts/MSLine.swf" />
                <%="<param name=""FlashVars"" value=""&dataXML="%><asp:Literal ID="lblXML1" runat="server"></asp:Literal>
                <%="""/>"%>
                <param name="wmode" value="opaque" />
                <param name="quality" value="high" />
                <embed src="/FusionCharts_Enterprise/Charts/MSLine.swf" flashvars="&dataXML=<%= lblXML1.Text%>"
                    quality="high" bgcolor="#ffffff" wmode="opaque" width="500" height="350" name="MSLine"
                    align="middle" type="application/x-shockwave-flash" pluginspage="http://www.macromedia.com/go/getflashplayer" />
            </object>
        </div>
    </div>

</asp:Content>
