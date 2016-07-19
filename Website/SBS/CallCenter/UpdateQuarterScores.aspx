<%@ Page Language="VB" MasterPageFile="~/SBS/CallCenter/CCAgents.master" AutoEventWireup="false"
    CodeFile="UpdateQuarterScores.aspx.vb" Inherits="SBSCallCenterAgents.UpdateQuarterScores" %>

<%@ Register Src="~/Inc/DateTime.ascx" TagName="DateTime" TagPrefix="uc" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="Server">

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

    <div style="background-color: White; width: 100%">
        <table cellpadding="2" cellspacing="2" width="100%">
            <tr>
                <td>
                    <table>
                        <tr>
                            <td nowrap="nowrap" valign="middle" style="padding-top: 1px;">
                                <asp:RadioButtonList ID="rdGameType" runat="server" AutoPostBack="true" RepeatDirection="Horizontal">
                                    <asp:ListItem Text="Game" Value="GAME" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Final Games" Value="FINAL"></asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                            <td nowrap="nowrap" valign="middle" style="padding-top: 1px;">
                                <asp:CheckBox ID="chkLock" runat="server" Text="Lock Games" AutoPostBack="true" />
                            </td>
                            <td valign="middle" style="padding-top: 3px;">
                                &nbsp;| Game Type:
                            </td>
                            <td nowrap="nowrap" valign="middle">
                                <asp:DropDownList ID="ddlGameType" runat="server" CssClass="textInput">
                                    <asp:ListItem Text="NBA Basketball" Value="NBA Basketball"></asp:ListItem>
                                    <asp:ListItem Text="NFL Football" Value="NFL Football"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td valign="middle">
                                Date Range:
                            </td>
                            <td valign="middle">
                                <uc:DateTime ID="ucDateFrom" runat="server" ShowTime="false" ShowCalendar="false" />
                            </td>
                            <td>
                                ~
                            </td>
                            <td>
                                <uc:DateTime ID="ucDateTo" runat="server" ShowTime="false" ShowCalendar="false" />
                            </td>
                            <td width="150px" align="right">
                                <asp:Button ID="btnSearch" Text="Search" runat="server" Width="60px" CssClass="button" />
                                <asp:Button ID="btnReset" Text="Reset" runat="server" Width="60px" CssClass="button" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="btnLock" runat="server" Text="Lock" ToolTip="Lock Games" CssClass="button"
                        Width="70" />
             <%--       <asp:Button ID="btnUpdate" runat="server" Text="Update Games" ToolTip="Update Score(s)"
                        CssClass="textInput" />--%>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:DataGrid ID="dgGames" runat="server" Width="100%" AutoGenerateColumns="false"
                        AllowPaging="true" PagerStyle-Mode="NumericPages" PagerStyle-Position="Bottom"
                        CellPadding="2" CellSpacing="2" CssClass="gamebox">
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
                            <asp:BoundColumn DataField="GameType" HeaderText="Type" ItemStyle-Width="50" ItemStyle-HorizontalAlign="Center"
                                ItemStyle-Wrap="false" />
                            <asp:TemplateColumn HeaderText="Game Date" ItemStyle-Width="80" ItemStyle-HorizontalAlign="Center"
                                ItemStyle-Wrap="false">
                                <ItemTemplate>
                                    <%#IIf(SBCBL.std.SafeString(Container.DataItem("GameDate")) <> "", SBCBL.std.SafeDate(Container.DataItem("GameDate")).ToString("MM/dd HH:mm"), "").ToString%>&nbsp;
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
                            <asp:BoundColumn DataField="GameStatus" HeaderText="Status" ItemStyle-Width="85"
                                ItemStyle-HorizontalAlign="Center" ItemStyle-Wrap="false" />
                            <asp:TemplateColumn HeaderText="Home Score" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="200"
                                ItemStyle-Wrap="false">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtHomeScore1Q" ToolTip='<%#Container.DataItem("HomeTeam")%>' runat="server"
                                        MaxLength="10" Text='<%#  Container.DataItem("HomeFirstQScore")%>' CssClass="textInput"
                                        Width="30" onkeypress="javascript:return inputNumberOnly(event);" Style="text-align: center;"></asp:TextBox>
                                    <asp:TextBox ID="txtHomeScore2Q" ToolTip='<%#Container.DataItem("HomeTeam")%>' runat="server"
                                        MaxLength="10" Text='<%#   Container.DataItem("HomeSecondQScore")%>' CssClass="textInput"
                                        Width="30" onkeypress="javascript:return inputNumberOnly(event);" Style="text-align: center;"></asp:TextBox>
                                    <asp:TextBox ID="txtHomeScore3Q" ToolTip='<%#Container.DataItem("HomeTeam")%>' runat="server"
                                        MaxLength="10" Text='<%#  Container.DataItem("HomeThirdQScore")%>' CssClass="textInput"
                                        Width="30" onkeypress="javascript:return inputNumberOnly(event);" Style="text-align: center;"></asp:TextBox>
                                    <asp:TextBox ID="txtHomeScore4Q" ToolTip='<%#Container.DataItem("HomeTeam")%>' runat="server"
                                        MaxLength="10" Text='<%#   Container.DataItem("HomeFourQScore")%>' CssClass="textInput"
                                        Width="30" onkeypress="javascript:return inputNumberOnly(event);" Style="text-align: center;"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Away Score" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="200"
                                ItemStyle-Wrap="false">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtAwayScore1Q" ToolTip='<%#Container.DataItem("AwayTeam")%>' runat="server"
                                        MaxLength="10" Text='<%# Container.DataItem("AwayFirstQScore")%>' CssClass="textInput"
                                        Width="30" onkeypress="javascript:return inputNumberOnly(event);" Style="text-align: center;"></asp:TextBox>
                                    <asp:TextBox ID="txtAwayScore2Q" ToolTip='<%#Container.DataItem("AwayTeam")%>' runat="server"
                                        MaxLength="10" Text='<%# Container.DataItem("AwaySecondQScore")%>' CssClass="textInput"
                                        Width="30" onkeypress="javascript:return inputNumberOnly(event);" Style="text-align: center;"></asp:TextBox>
                                    <asp:TextBox ID="txtAwayScore3Q" ToolTip='<%#Container.DataItem("AwayTeam")%>' runat="server"
                                        MaxLength="10" Text='<%#  Container.DataItem("AwayThirdQScore")%>' CssClass="textInput"
                                        Width="30" onkeypress="javascript:return inputNumberOnly(event);" Style="text-align: center;"></asp:TextBox>
                                    <asp:TextBox ID="txtAwayScore4Q" ToolTip='<%#Container.DataItem("AwayTeam")%>' runat="server"
                                        MaxLength="10" Text='<%#  Container.DataItem("AwayFourQScore")%>' CssClass="textInput"
                                        Width="30" onkeypress="javascript:return inputNumberOnly(event);" Style="text-align: center;"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn>
                                <ItemTemplate>
                                    <asp:Button ID="btnChangeScore" runat="server" Text="Change Score" ToolTip="Change score this game"
                                        CssClass="textInput" Width="80" Height="18" OnClientClick="return confirm('Are you sure you want to change score this game? All bets for this game will be processed immediately!');"
                                        CommandName="CHANGE_SCORE_GAME_FINAL" CommandArgument='<%# Container.DataItem("GameID") %>' />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                    </asp:DataGrid>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
