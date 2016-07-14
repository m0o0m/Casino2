<%@ Page Language="VB" MasterPageFile="~/SBS/SuperAdmins/SuperAdmin.master" AutoEventWireup="false"
    CodeFile="ManualGames.aspx.vb" Inherits="SBSSuperAdmin.ManualGames" %>

<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>
<%@ Register Src="~/Inc/DateTime.ascx" TagName="DateTime" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphContent" runat="Server">

    <div class="row">
        <div class="col-lg-12">
            <asp:RadioButtonList ID="rdGameType" runat="server" AutoPostBack="true" RepeatDirection="Horizontal" style="display: inline-block; top: 9px; position: relative;">
                <asp:ListItem Style="margin-left: 15px;" Text="Game" Value="GAME" Selected="True"></asp:ListItem>
                <asp:ListItem Style="margin-left: 15px;" Text="First Half" Value="FIRSTHALF"></asp:ListItem>
                <asp:ListItem Style="margin-left: 15px;" Text="Final Games" Value="FINAL"></asp:ListItem>
            </asp:RadioButtonList>
            &nbsp;|&nbsp;
            <span>Game Type:</span>
            <wlb:CDropDownList ID="ddlGameType" runat="server" OptionalText="" OptionalValue=""
                hasOptionalItem="false" CssClass="form-control" Style="display: inline-block;" Width="230px">
            </wlb:CDropDownList>

        </div>
        <div class="clearfix"></div>
    </div>
    <div class="row">
        <div class="col-lg-12 pull-left">
            <span>Date Range:</span>
            &nbsp;
            <uc:DateTime ID="ucDateFrom" runat="server" ShowTime="false" ShowCalendar="false" style="display: inline-block; width: 130px" />
            &nbsp;<span>To</span>&nbsp;
            <uc:DateTime ID="ucDateTo" runat="server" ShowTime="false" ShowCalendar="false" style="display: inline-block; width: 130px;" />
            &nbsp;&nbsp;
            <asp:CheckBox ID="chkLock" runat="server" Text="Lock Games" AutoPostBack="true" />
            &nbsp;&nbsp;
            <asp:Button ID="btnSearch" Text="Search" runat="server" CssClass="btn btn-green" />
            <asp:Button ID="btnReset" Text="Reset" runat="server" CssClass="btn btn-default" />
        </div>
        <div class="clearfix"></div>
    </div>
    <div class="mbxl"></div>
    <div class="row">
        <div class="col-lg-12">
            <asp:Button ID="btnLock" runat="server" Text="Lock" ToolTip="Lock Games" CssClass="btn btn-default" />
            <%--  <asp:Button ID="btnUpdate" runat="server" Text="Update Games" ToolTip="Update Score(s)" CssClass="button" />--%>
        </div>
        <div class="clearfix"></div>
    </div>
    <div class="mbxl"></div>

    <div class="row">
        <div class="col-lg-12">
            <asp:DataGrid ID="dgGames" runat="server" AutoGenerateColumns="false" AllowPaging="true" 
                PagerStyle-Mode="NumericPages" PagerStyle-Position="Bottom" CssClass="table table-hover table-bordered">
                <HeaderStyle CssClass="tableheading" HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Left" />
                <AlternatingItemStyle HorizontalAlign="Left" />
                <Columns>
                    <asp:TemplateColumn ItemStyle-HorizontalAlign="Center" ItemStyle-Width="20">
                        <HeaderTemplate>
                            <input id="chkChoice" type="checkbox" onclick="CheckAllGames(this);" title="Select all"
                                runat="server" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox type="checkbox" ID="chkID" runat="server" />
                            <asp:HiddenField ID="hfGameID" runat="server" Value='<%# Container.DataItem("GameID") %>' />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Home Score - Away Score" ItemStyle-HorizontalAlign="Center"
                        ItemStyle-Width="190" ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <asp:TextBox ID="txtHomeScore" ToolTip='<%#Container.DataItem("HomeTeam")%>' runat="server"
                                MaxLength="10" Text='<%# iif( rdGameType.SelectedValue = "GAME" Or rdGameType.SelectedValue = "FIRSTHALF" ,Container.DataItem("HomeFirstHalfScore"), Container.DataItem("HomeScore"))%>'
                                CssClass="textInput" Width="40" onkeypress="javascript:return inputNumberOnly(event);"
                                Style="text-align: center;"></asp:TextBox>
                            -
                                <asp:TextBox ID="txtAwayScore" ToolTip='<%#Container.DataItem("AwayTeam")%>' runat="server"
                                    MaxLength="10" Text='<%# iif( rdGameType.SelectedValue = "GAME" Or rdGameType.SelectedValue = "FIRSTHALF" ,Container.DataItem("AwayFirstHalfScore"), Container.DataItem("AwayScore"))%>'
                                    CssClass="textInput" Width="40" onkeypress="javascript:return inputNumberOnly(event);"
                                    Style="text-align: center;"></asp:TextBox>
                            <%--<asp:RadioButton runat="server" ID="rdFinal" Text="Final" GroupName='<%#"rdGameType_" & Container.ItemIndex.ToString()%>'
                                    onclick="CheckFinalMark(this);" Visible='<%# not IsFinalGames and not IsFirstHalf %>'
                                    AutoPostBack="false" OnCheckedChanged="rdGameType_CheckedChanged" />
                                <asp:RadioButton runat="server" ID="rd1H" Text="First Half" GroupName='<%#"rdGameType_" & Container.ItemIndex.ToString()%>'
                                   AutoPostBack="true" onclick="CheckFinalMark(this);" Checked='<%#SBCBL.std.SafeString(Container.DataItem("IsFirstHalfFinished")) = "Y"%>'
                                    Visible='<%# not IsFinalGames and not IsFirstHalf%>' OnCheckedChanged="rdGameType_CheckedChanged" />
                            --%>                   <%--<asp:Button ID="btnChangeScore" runat="server" Text="Change Score" ToolTip="Change score this game"
                                    CssClass="button" Width="80" Height="18" OnClientClick="return confirm('Are you sure you want to change score this game? All bets for this game will be processed immediately!');"
                                    Visible='<%# IsFinalGames or IsFirstHalf %>' CommandName="CHANGE_SCORE_GAME_FINAL"
                                    CommandArgument='<%# Container.DataItem("GameID") %>' />--%>
                            <asp:HiddenField ID="HFHomeScore" runat="server" Value='<%#Container.DataItem("HomeScore")%>' />
                            <asp:HiddenField ID="HFAwayScore" runat="server" Value='<%#Container.DataItem("AwayScore")%>' />
                            <asp:HiddenField ID="HFHomeFirstHalf" runat="server" Value='<%#Container.DataItem("HomeFirstHalfScore")%>' />
                            <asp:HiddenField ID="HFAwayFirstHalf" runat="server" Value='<%#Container.DataItem("AwayFirstHalfScore")%>' />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Home Team" ItemStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                            <%#Container.DataItem("HomeTeam") & " (" & Container.DataItem("HomeRotationNumber") & ")"%>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Away Team" ItemStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                            <%#Container.DataItem("AwayTeam") & " (" & Container.DataItem("AwayRotationNumber") & ")"%>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Is Series" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <%#IIf(SBCBL.std.SafeString(Container.DataItem("PropDescription")) = "Series Price", "Y", "N")%>
                        </ItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn HeaderText="Status" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <wlb:CDropDownList ID="ddlGameStatus" runat="server" OptionalText="" OptionalValue=""
                                hasOptionalItem="true" CssClass="textInput">
                                <asp:ListItem>CANCELLED</asp:ListItem>
                                <asp:ListItem>Final</asp:ListItem>
                            </wlb:CDropDownList>
                        </ItemTemplate>
                    </asp:TemplateColumn>

                    <%-- <asp:BoundColumn DataField="GameStatus" HeaderText="Status" ItemStyle-Width="85"
                            ItemStyle-HorizontalAlign="Center" ItemStyle-Wrap="false" />--%>
                    <asp:BoundColumn DataField="GameType" HeaderText="Type" ItemStyle-Width="50" ItemStyle-HorizontalAlign="Center"
                        ItemStyle-Wrap="false" />
                    <asp:TemplateColumn HeaderText="Game Date" ItemStyle-Width="80" ItemStyle-HorizontalAlign="Center"
                        ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <%#SBCBL.std.SafeString(Container.DataItem("GameDate"))%>&nbsp;
                        </ItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn HeaderText="Update Game" ItemStyle-HorizontalAlign="Center"
                        ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <asp:Button ID="btnUpdateGame" runat="server" Text="Update Game" OnClientClick="return confirm('Are you sure you wish to mark this game status and game score? All open bets for this game will be processed immediately!')" CommandName="UPDATE_GAME" CssClass="button" />
                        </ItemTemplate>
                    </asp:TemplateColumn>


                </Columns>
            </asp:DataGrid>
        </div>
        <div class="clearfix"></div>
    </div>
    <div class="mbxl"></div>

    <script type="text/javascript" language="javascript">
        function CheckAllGames(checkAll) {
            var oGrid = document.getElementById('<%=dgGames.ClientID%>');
            var oInputs = oGrid.getElementsByTagName("INPUT");

            for (var index = 0; index < oInputs.length; index++) {
                if (oInputs[index].type.toUpperCase() == "CHECKBOX" && !oInputs[index].getAttribute('onclick')) {
                    oInputs[index].checked = checkAll.checked;
                }
            }

            if (checkAll.checked) {
                checkAll.title = "Unselect all"
            }
            else {
                checkAll.title = "Select all"
            }
        }

        function CheckFinalMark(t) {
            if (t.checked) {
                if (!confirm("Are you sure you wish to mark this game Final? All open bets for this game will be processed immediately!"))
                    t.checked = false;
            }
        }
    </script>
</asp:Content>
