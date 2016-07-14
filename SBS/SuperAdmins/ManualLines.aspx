<%@ Page Language="VB" MasterPageFile="~/SBS/SuperAdmins/SuperAdmin.master" AutoEventWireup="false"
    CodeFile="ManualLines.aspx.vb" Inherits="SBSSuperAdmin.ManualLines" Title="Untitled Page" %>
<%@ Register Src="~/Inc/Agents/GameManual.ascx" TagName="GameManual"
    TagPrefix="uc" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <uc:GameManual ID="ucGameManual" runat="server" />
  <%--  <script type="text/javascript">
        function updatespread(obj) {
            var id = obj.getAttribute("spreadinput");
            var txt = document.getElementById(id);
            txt.value = -obj.value;
        }
    </script>

    <fieldset id="dddd" style="width: auto; float: left; height: 50px">
        <legend>Game Types</legend>
        <asp:CheckBoxList ID="cblGameTypes" runat="server" AutoPostBack="true" RepeatDirection="Horizontal">
            <asp:ListItem Text="NBA Basketball" Selected="True" Value="NBA Basketball"></asp:ListItem>
            <asp:ListItem Text="NFL Football" Selected="True" Value="NFL Football"></asp:ListItem>
        </asp:CheckBoxList>
    </fieldset>
    <fieldset style="width: auto; float: left; height: 50px">
        <legend>Contexts</legend>
        <asp:CheckBoxList ID="cblContext" runat="server" AutoPostBack="true" RepeatDirection="Horizontal">
            <asp:ListItem Text="1Q" Selected="True" Value="1Q"></asp:ListItem>
            <asp:ListItem Text="2Q" Selected="True" Value="2Q"></asp:ListItem>
            <asp:ListItem Text="3Q" Selected="True" Value="3Q"></asp:ListItem>
            <asp:ListItem Text="4Q" Selected="True" Value="4Q"></asp:ListItem>
        </asp:CheckBoxList>
    </fieldset>
    <fieldset style="width: auto; float: left; height: 50px; padding-right: 10px">
        <legend>Line Odds</legend>
        <asp:CheckBox ID="chkOdds" Text="Lines already updated" Checked="false" runat="server"
            AutoPostBack="true" />
    </fieldset>
    <table style="width: 80%">
        <tr>
            <td align="right">
                <asp:Button ID="btnReload1" runat="server" Text="Reload All" CssClass="textInput" />&nbsp;<asp:Button
                    ID="btnSave1" runat="server" Text="Save All" CssClass="textInput" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Repeater runat="server" ID="rptMain">
                    <ItemTemplate>
                        <asp:Repeater runat="server" ID="rptBets" OnItemDataBound="rptBets_ItemDataBound">
                            <ItemTemplate>
                                <div style="background-position: #1E1E22; background: #1E1E22; color: #009CFF; padding: 5px 3px 3px 5px;
                                    font-weight: bold;">
                                    <%#CType(Container.Parent.Parent, RepeaterItem).DataItem%>&nbsp;<%#Container.DataItem.Key%>
                                    Lines
                                </div>
                                <asp:Label ID="lblNoGameLine" runat="server" Style="margin-left: 20px;">There is no <%#CType(Container.Parent.Parent, RepeaterItem).DataItem%> game.</asp:Label>
                                <asp:Repeater runat="server" ID="rptGameLines" OnItemCommand="rptGameLines_ItemCommand">
                                    <HeaderTemplate>
                                        <table class="table_bet_input" width="100%" align="center" cellpadding="2" cellspacing="1"
                                            class="<%# IIF(Container.ItemType = ListItemType.Item,"gametable_odd","gametable_even") %> gamebox">
                                            <tr class="tableheading" style="text-align: center;">
                                                <td width="50%" colspan="3">
                                                    Game
                                                </td>
                                                <td width="20%" id="tdSpread" runat="server">
                                                    Spread
                                                </td>
                                                <td width="20%" id="tdTotal" runat="server">
                                                    Total
                                                </td>
                                                <td width="10%" id="tdMLine" runat="server">
                                                    MLine
                                                </td>
                                                <td>
                                                </td>
                                            </tr>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td rowspan="2">
                                                <%#CDate(Container.DataItem("GameDate"))%>
                                            </td>
                                            <td class="game_num">
                                                <asp:Literal ID="lblAwayNumber" runat="server" Text='<%# Container.DataItem("AwayRotationNumber")%>'></asp:Literal>
                                            </td>
                                            <td>
                                                <asp:Literal ID="lblAwayTeam" runat="server" Text='<%#Container.DataItem("AwayTeam")%>'></asp:Literal>
                                            </td>
                                            <td align="center">
                                                <asp:TextBox CssClass="bet_input textInput" ID="txtAwaySpread" runat="server" Text='<%#FormatValue(Container.DataItem("AwaySpread"))%>'
                                                    onchange="javascript:updatespread(this);" spreadinput='<%#Container.FindControl("txtHomeSpread").ClientID%> ' />
                                                -
                                                <asp:TextBox CssClass="bet_input textInput" ID="txtAwaySpreadMoney" runat="server"
                                                    Text='<%#  FormatValue(Container.DataItem("AwaySpreadMoney")) %>' />
                                            </td>
                                            <td align="center">
                                                O
                                                <asp:TextBox CssClass="bet_input textInput" ID="txtAwayTotal" runat="server" Text='<%#FormatValue(Container.DataItem("TotalPoints"))%>' />
                                                -
                                                <asp:TextBox CssClass="bet_input textInput" ID="txtTotalPointsOverMoney" runat="server"
                                                    Text='<%#FormatValue(Container.DataItem("TotalPointsOverMoney"))%>' />
                                            </td>
                                            <td align="center">
                                                <asp:TextBox CssClass="bet_input textInput" ID="txtAwayMoneyLine" runat="server"
                                                    Text='<%#FormatValue(Container.DataItem("AwayMoneyLine"))%>' />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="game_num">
                                                <asp:Literal ID="lblHomeNumber" runat="server" Text='<%#Container.DataItem("HomeRotationNumber")%>'></asp:Literal>
                                            </td>
                                            <td>
                                                <asp:Literal ID="lblHomeTeam" runat="server" Text='<%#Container.DataItem("HomeTeam")%>'></asp:Literal>
                                            </td>
                                            <td align="center">
                                                <asp:TextBox CssClass="bet_input textInput" ID="txtHomeSpread" runat="server" Text='<%# FormatValue(Container.DataItem("HomeSpread"))%>'
                                                    onchange="javascript:updatespread(this);" spreadinput='<%#Container.FindControl("txtAwaySpread").ClientID%>' />
                                                -
                                                <asp:TextBox CssClass="bet_input textInput" ID="txtHomeSpreadMoney" runat="server"
                                                    Text='<%# FormatValue(Container.DataItem("HomeSpreadMoney"))%>' />
                                            </td>
                                            <td align="center">
                                                U
                                                <asp:TextBox CssClass="bet_input textInput" ID="txtTotalPoints2" Enabled="false"
                                                    Style="background-color: #CFCCC5; color: white" runat="server" Text='<%# FormatValue(Container.DataItem("TotalPoints"))%>' />
                                                -
                                                <asp:TextBox CssClass="bet_input textInput" ID="txtTotalPointsUnderMoney" runat="server"
                                                    Text='<%#FormatValue(Container.DataItem("TotalPointsUnderMoney"))%>' />
                                            </td>
                                            <td align="center">
                                                <asp:TextBox CssClass="bet_input textInput" ID="txtHomeMoney" runat="server" Text='<%#FormatValue(Container.DataItem("HomeMoneyLine")) %>' />
                                            </td>
                                            <td valign="top" style="padding-top: -30px; white-space: nowrap">
                                                <asp:Button CssClass="textInput" ID="btnSave" CommandName="SAVE" runat="server" Text="Save Line" />
                                                <asp:Button CssClass="textInput" ID="btnClear" CommandName="DELETE" runat="server"
                                                    Text="Clear Line" />
                                                <cc1:CButtonConfirmer ID="CButtonConfirmer1" AttachTo="btnClear" ConfirmExpression="confirm('Are you sure to clear this line?')"
                                                    runat="server">
                                                </cc1:CButtonConfirmer>
                                                <asp:HiddenField ID="hfBookmaker" runat="server" Value='<%#Container.DataItem("BookMaker")%>'>
                                                </asp:HiddenField>
                                                <asp:HiddenField ID="hfGameID" runat="server" Value='<%#Container.DataItem("GameID")%>'>
                                                </asp:HiddenField>
                                                <asp:HiddenField ID="hfGameLineID" runat="server" Value='<%#Container.DataItem("GameLineID")%>'>
                                                </asp:HiddenField>
                                                <asp:HiddenField ID="hfGameType" runat="server" Value='<%#Container.DataItem("GameType")%>'>
                                                </asp:HiddenField>
                                                <asp:HiddenField ID="hfContext" runat="server" Value='<%#Container.DataItem("Context")%>'>
                                                </asp:HiddenField>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <SeparatorTemplate>
                                        <tr>
                                            <td colspan="7">
                                                <hr />
                                            </td>
                                        </tr>
                                    </SeparatorTemplate>
                                    <FooterTemplate>
                                        </table></FooterTemplate>
                                </asp:Repeater>
                                <br />
                            </ItemTemplate>
                        </asp:Repeater>
                    </ItemTemplate>
                </asp:Repeater>
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:Button ID="btnReload2" runat="server" Text="Reload All" CssClass="textInput" />&nbsp;<asp:Button
                    ID="btnSave2" runat="server" Text="Save All" CssClass="textInput" />
            </td>
        </tr>
    </table>--%>
</asp:Content>
