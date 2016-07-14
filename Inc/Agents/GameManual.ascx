<%@ Control Language="VB" AutoEventWireup="false" CodeFile="GameManual.ascx.vb" Inherits="SBSAgents.GameManual" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/Inc/DateTime.ascx" TagName="DateTime" TagPrefix="uc" %>

<script type="text/javascript">
    function updatespread(obj) {
        var id = obj.getAttribute("spreadinput");
        var txt = document.getElementById(id);
        txt.value = -obj.value;
    }
</script>

<div class="row">
    <div class="col-lg-12">
        <div style="display: inline-block;" id="tdAgents" runat="server" visible="false">
            <span>Agents :</span>
            <cc1:CDropDownList runat="server" ID="ddlAgent" AutoPostBack="true" hasOptionalItem="false" CssClass="form-control" Style="display: inline-block;" Width="290px">
            </cc1:CDropDownList>
        </div>
        &nbsp;&nbsp;
        <span>Game Types:</span>
        <cc1:CDropDownList runat="server" ID="ddlGameType" AutoPostBack="true" CssClass="form-control" Style="display: inline-block;" Width="230px">
            <asp:ListItem Value="NBA Basketball">NBA Basketball	</asp:ListItem>
            <asp:ListItem Value="NCAA Basketball">NCAA Basketball</asp:ListItem>
            <asp:ListItem Value="WNBA Basketball">WNBA Basketball</asp:ListItem>
            <asp:ListItem Value="WNCAA Basketball">WNCAA Basketball</asp:ListItem>
            <asp:ListItem Value="AFL Football ">AFL Football</asp:ListItem>
            <asp:ListItem Value="CFL Football">CFL Football</asp:ListItem>
            <asp:ListItem Value="NCAA Football">NCAA Football</asp:ListItem>
            <asp:ListItem Value="NFL Football">NFL Football</asp:ListItem>
            <asp:ListItem Value="NFL Preseason">NFL Preseason</asp:ListItem>
            <asp:ListItem Value="UFL Football">UFL Football</asp:ListItem>
        </cc1:CDropDownList>
        &nbsp;&nbsp;
        <span>Quarter:</span>
        &nbsp;&nbsp;
        <div style="display: inline-block;" id="tdFilterQuarter" runat="server" visible="false">
            <asp:CheckBoxList ID="cblContext" runat="server" AutoPostBack="true" RepeatDirection="Horizontal" Style="position: relative; top: 10px;">
                <asp:ListItem Style="margin-left: 10px;" Text="1Q" Selected="True" Value="1Q"></asp:ListItem>
                <asp:ListItem Style="margin-left: 10px;" Text="2Q" Selected="True" Value="2Q"></asp:ListItem>
                <asp:ListItem Style="margin-left: 10px;" Text="3Q" Selected="True" Value="3Q"></asp:ListItem>
                <asp:ListItem Style="margin-left: 10px;" Text="4Q" Selected="True" Value="4Q"></asp:ListItem>
            </asp:CheckBoxList>
        </div>
        <div style="display: inline-block;" id="tdOffAllQuarter" runat="server" visible="false">
            <asp:RadioButtonList ID="rdQuarter" runat="server" RepeatDirection="Horizontal" Style="display: inline-block; position: relative; top: 10px;">
                <asp:ListItem Style="margin-left: 10px;" Value="QuarterOn">On</asp:ListItem>
                <asp:ListItem Style="margin-left: 10px;" Value="QuarterOff">Off</asp:ListItem>
            </asp:RadioButtonList>
            <asp:Button ID="btnSaveAll" runat="server" Text="Save" CssClass="btn btn-primary" Style="margin-left: 10px;" />
        </div>
    </div>
    <div class="clearfix"></div>
</div>
<div class="mbxl"></div>


<div class="row" id="tblGame" runat="server">
    <div class="col-lg-12" style="text-align: right;">
        <asp:CheckBoxList ID="cblQuarterOff" runat="server" AutoPostBack="true" RepeatDirection="Horizontal" Style="display: inline-block; position: relative; top: 10px;">
            <asp:ListItem Style="margin-left: 10px;" Text="1Q Off" Selected="True" Value="1QOff"></asp:ListItem>
            <asp:ListItem Style="margin-left: 10px;" Text="2Q Off" Selected="True" Value="2QOff"></asp:ListItem>
            <asp:ListItem Style="margin-left: 10px;" Text="3Q Off" Selected="True" Value="3QOff"></asp:ListItem>
            <asp:ListItem Style="margin-left: 10px;" Text="4Q Off" Selected="True" Value="4QOff"></asp:ListItem>
        </asp:CheckBoxList>
        &nbsp;&nbsp;
        <asp:Button ID="btnReload1" runat="server" Text="Reload All" CssClass="btn btn-primary" />
        &nbsp;
        <asp:Button ID="btnSave1" runat="server" Text="Save All" CssClass="btn btn-primary" />
    </div>
    <div class="clearfix"></div>
    <div class="col-lg-12">
        <asp:Repeater runat="server" ID="rptMain">
            <ItemTemplate>
                <asp:Repeater runat="server" ID="rptBets" OnItemDataBound="rptBets_ItemDataBound">
                    <ItemTemplate>
                        <div style="background-position: #1E1E22; background: #1E1E22; color: #009CFF; padding: 5px 3px 3px 5px; font-weight: bold;">
                            <%#CType(Container.Parent.Parent, RepeaterItem).DataItem%>&nbsp;<%#Container.DataItem.Key%>
                                Lines
                        </div>
                        <asp:Label ID="lblNoGameLine" runat="server" Style="margin-left: 20px;">There is no <%#CType(Container.Parent.Parent, RepeaterItem).DataItem%> game.</asp:Label>
                        <div id="rptgameLine">
                            <asp:Repeater runat="server" ID="rptGameLines" OnItemCommand="rptGameLines_ItemCommand">
                                <HeaderTemplate>
                                    <table class="<%# IIF(Container.ItemType = ListItemType.Item,"gametable_odd","gametable_even") %> table table-hover table-borderred">

                                        <tr class="tableheading" style="text-align: center; height: 23px;">
                                            <td width="50%" colspan="3">Game
                                            </td>
                                            <td width="20%" id="tdSpread" runat="server">Spread
                                            </td>
                                            <td width="20%" id="tdTotal" runat="server">Total
                                            </td>
                                            <td></td>
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
                                        <td align="center">O
                                                <asp:TextBox CssClass="bet_input textInput" ID="txtAwayTotal" runat="server" Text='<%#FormatValue(Container.DataItem("TotalPoints"))%>' />
                                            -
                                                <asp:TextBox CssClass="bet_input textInput" ID="txtTotalPointsOverMoney" runat="server"
                                                    Text='<%#FormatValue(Container.DataItem("TotalPointsOverMoney"))%>' />
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
                                        <td align="center">U
                                                <asp:TextBox CssClass="bet_input textInput" ID="txtTotalPoints2" Enabled="false"
                                                    Style="background-color: #CFCCC5; color: white" runat="server" Text='<%# FormatValue(Container.DataItem("TotalPoints"))%>' />
                                            -
                                                <asp:TextBox CssClass="bet_input textInput" ID="txtTotalPointsUnderMoney" runat="server"
                                                    Text='<%#FormatValue(Container.DataItem("TotalPointsUnderMoney"))%>' />
                                        </td>
                                        <td valign="top">
                                            <asp:Button CssClass="btn btn-primary" ID="btnSave" CommandName="SAVE" runat="server" Text="Save Line" />
                                            <asp:Button CssClass="btn btn-red" ID="btnClear" CommandName="DELETE" runat="server" Text="Clear Line" />
                                            <cc1:CButtonConfirmer ID="CButtonConfirmer1" AttachTo="btnClear" ConfirmExpression="confirm('Are you sure to clear this line?')"
                                                runat="server">
                                            </cc1:CButtonConfirmer>
                                            <asp:HiddenField ID="hfBookmaker" runat="server" Value='<%#Container.DataItem("BookMaker")%>'></asp:HiddenField>
                                            <asp:HiddenField ID="hfGameID" runat="server" Value='<%#Container.DataItem("GameID")%>'></asp:HiddenField>
                                            <asp:HiddenField ID="hfGameLineID" runat="server" Value='<%#Container.DataItem("GameLineID")%>'></asp:HiddenField>
                                            <asp:HiddenField ID="hfGameType" runat="server" Value='<%#Container.DataItem("GameType")%>'></asp:HiddenField>
                                            <asp:HiddenField ID="hfContext" runat="server" Value='<%#Container.DataItem("Context")%>'></asp:HiddenField>
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
                                    </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>
                        <br />
                    </ItemTemplate>
                </asp:Repeater>
            </ItemTemplate>
        </asp:Repeater>
    </div>
    <div class="clearfix"></div>
</div>
<div class="mbxl"></div>

<script language="javascript">

    function HiddenNoGame(lblNogame, hidden) {
        if (hidden)
            // document.parentNode
            var rptgameLine = lblNogame.parentNode.getElementsByTagName("table")
        rptgameLine.style.display = "none";
    }

</script>

