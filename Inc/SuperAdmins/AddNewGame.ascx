<%@ Control Language="VB" AutoEventWireup="false" CodeFile="AddNewGame.ascx.vb" Inherits="SBCSuperAdmin.AddNewGame" %>
<%@ Register Src="~/Inc/DateTime.ascx" TagName="DateTime" TagPrefix="uc" %>

<div class="row">
    <div class="col-lg-12">
        <label class="control-label">Game Type:</label>
        <asp:DropDownList ID="ddlGameType" runat="server" AutoPostBack="true" CssClass="form-control" Width="230px" Style="display: inline-block;">
        </asp:DropDownList>
    </div>
    <div class="clearfix"></div>
</div>
<div class="mbxl"></div>

<div class="row">
    <div class="col-lg-12">
        <table class="table">
            <tr>
                <td rowspan="3" style="width: 390px;">
                    <span style="display: block;">GDate:</span>
                    <uc:DateTime ID="txtGameDate" runat="server" ShowTime="true" ShowCalendar="false" />
                    <br />
                    <span style="display: block;">SecondHalfTime:</span>
                    <uc:DateTime ID="txtSecondHalfTime" runat="server" ShowTime="true" ShowCalendar="false" />
                </td>
                <td rowspan="3" style="width: 25px; vertical-align: middle;">RN
                </td>
                <td style="width: 90px;">
                    <asp:TextBox CssClass="form-control" ID="txtAwayRotationNumber"
                        runat="server" onkeypress="javascript:return inputNumber(this,event, true);" MaxLength="7" />
                </td>
                <td rowspan="2" style="width: 50px; vertical-align: middle;">Team
                </td>
                <td style="width: 150px;">
                    <asp:TextBox CssClass="form-control" ID="txtAwayTeam" runat="server" />
                </td>
                <td rowspan="2" style="width: 50px; vertical-align: middle;">Spread
                </td>
                <td align="center" style="width: 210px;">
                    <asp:TextBox CssClass="form-control" ID="txtAwaySpread" Text="" Style="display: inline-block;"
                        runat="server" onkeypress="javascript:return inputNumber(this,event, true);" Width="65px" />
                    <span>-</span>
                    <asp:TextBox CssClass="form-control" ID="txtAwaySpreadMoney" Text="" Style="display: inline-block;" Width="65px"
                        runat="server" onkeypress="javascript:return inputNumber(this,event, true);" MaxLength="7" />
                </td>
                <td align="center" style="width: 230px;">
                    <span>O</span>
                    <asp:TextBox CssClass="form-control" ID="txtAwayTotal" Style="display: inline-block;" Width="65px"
                        runat="server" onkeypress="javascript:return inputNumber(this,event, true);" MaxLength="7" />
                    <span>-</span>
                    <asp:TextBox CssClass="form-control" ID="txtTotalPointsOverMoney" Style="display: inline-block;" Width="65px"
                        runat="server" onkeypress="javascript:return inputNumber(this,event, true);" MaxLength="7" />
                </td>
                <td rowspan="2" style="width: 30px; vertical-align: middle;">ML
                </td>
                <td align="center" style="width: 100px;">
                    <asp:TextBox CssClass="form-control" ID="txtAwayMoneyLine"
                        runat="server" onkeypress="javascript:return inputNumber(this,event, true);" MaxLength="7" />
                </td>
                <td rowspan="2">
                    <asp:DropDownList ID="ddlContext" runat="server">
                        <asp:ListItem Value="Current">Current</asp:ListItem>
                        <asp:ListItem Value="1H">1H</asp:ListItem>
                        <asp:ListItem Value="2H">2H</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="">
                    <asp:TextBox CssClass="form-control" ID="txtHomeRotationNumber"
                        runat="server" onkeypress="javascript:return inputNumber(this,event, true);" MaxLength="7" />
                </td>
                <td>
                    <asp:TextBox CssClass="form-control" ID="txtHomeTeam" runat="server" />
                </td>
                <td align="center">
                    <asp:TextBox CssClass="form-control" ID="txtHomeSpread" runat="server" Enabled="false"
                        onkeypress="javascript:return inputNumber(this,event, true);" Style="display: inline-block;" Width="65px"
                        MaxLength="7" />
                    <span>-</span>
                    <asp:TextBox CssClass="form-control" ID="txtHomeSpreadMoney" runat="server" Style="display: inline-block;" Width="65px"
                        onkeypress="javascript:return inputNumber(this,event, true);" MaxLength="7" />
                </td>
                <td align="center">
                    <span>U</span>
                    <asp:TextBox CssClass="form-control" ID="txtTotalPoints2" Enabled="false" Style="display: inline-block;" Width="65px"
                        runat="server" onkeypress="javascript:return inputNumber(this,event, true);"
                        MaxLength="7" />
                    <span>-</span>
                    <asp:TextBox CssClass="form-control" ID="txtTotalPointsUnderMoney" Style="display: inline-block;" Width="65px"
                        runat="server" onkeypress="javascript:return inputNumber(this,event, true);" MaxLength="7" />
                </td>
                <td align="center">
                    <asp:TextBox CssClass="form-control" ID="txtHomeMoney" runat="server"
                        onkeypress="javascript:return inputNumber(this,event, true);" MaxLength="7" />
                </td>
            </tr>
            <tr>
                <td class="" align="right">
                    <asp:TextBox CssClass="form-control" ID="txtDrawnRotationNumber"
                        runat="server" onkeypress="javascript:return inputNumber(this,event, true);" MaxLength="7" />
                </td>
                <td colspan="7" align="right"><span>Draw</span>
                    <asp:TextBox CssClass="form-control" ID="txtDrawMoney" runat="server" Style="display: inline-block;"
                        onkeypress="javascript:return inputNumber(this,event, true);" MaxLength="7" Width="75px" />
                </td>
                <td></td>
            </tr>
        </table>
    </div>
    <div class="col-lg-12" style="text-align: center;">
        <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary" />
    </div>
    <div class="clearfix"></div>
</div>
<div class="mbxl"></div>

<div class="row">
    <div class="col-lg-12">
        <asp:Repeater ID="rptGameAdd" runat="server">
            <HeaderTemplate>
                <table class="table">
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td rowspan="3" style="width: 390px;">
                        <asp:HiddenField ID="hfGameID" runat="server" Value='<%#Container.DataItem("GameID")%>' />
                        <span style="display: block;">GDate:</span>
                        <uc:DateTime ID="txtGameDate" runat="server" Value='<%#Container.DataItem("GameDate")%>' ShowTime="true" ShowCalendar="false" />
                        <br />
                        <span style="display: block;">SecondHalfTime:</span>
                        <uc:DateTime ID="txtSecondHalfTime" runat="server" Value='<%#Container.DataItem("GameDate")%>' ShowTime="true" ShowCalendar="false" />
                    </td>
                    <td rowspan="3" style="width: 25px; vertical-align: middle;">RN
                    </td>
                    <td style="width: 90px;">
                        <asp:TextBox CssClass="form-control" ID="txtAwayRotationNumber" Text='<%#Container.DataItem("AwayRotationNumber")%>'
                            runat="server" onkeypress="javascript:return inputNumber(this,event, true);" MaxLength="7" />
                    </td>
                    <td rowspan="2" style="width: 30px; vertical-align: middle;">Team
                    </td>
                    <td style="width: 150px;">
                        <asp:TextBox CssClass="form-control" ID="txtAwayTeam" runat="server" Text='<%#Container.DataItem("AwayTeam")%>' />
                    </td>
                    <td rowspan="2" style="width: 50px; vertical-align: middle;">Spread
                    </td>
                    <td align="center" style="width: 201px;">
                        <asp:TextBox CssClass="form-control" ID="txtAwaySpread" Text='<%#FormatValue(Container.DataItem("AwaySpread"))%>'
                            runat="server" onkeypress="javascript:return inputNumber(this,event, true);" Style="display: inline-block;" Width="65px" />
                        <span>-</span>
                        <asp:TextBox CssClass="form-control" ID="txtAwaySpreadMoney" Text='<%#FormatValue(Container.DataItem("AwaySpreadMoney"))%>'
                            runat="server" onkeypress="javascript:return inputNumber(this,event, true);" MaxLength="7" Style="display: inline-block;" Width="65px" />
                    </td>
                    <td align="center" style="width: 220px;">
                        <span>O</span>
                        <asp:TextBox CssClass="form-control" ID="txtAwayTotal" Text='<%#FormatValue(Container.DataItem("TotalPoints"))%>'
                            runat="server" onkeypress="javascript:return inputNumber(this,event, true);" MaxLength="7" Style="display: inline-block;" Width="65px" />
                        <span>-</span>
                        <asp:TextBox CssClass="form-control" ID="txtTotalPointsOverMoney" Text='<%#FormatValue(Container.DataItem("TotalPointsOverMoney"))%>'
                            runat="server" onkeypress="javascript:return inputNumber(this,event, true);" MaxLength="7" Style="display: inline-block;" Width="65px" />
                    </td>
                    <td rowspan="2" style="width: 30px; vertical-align: middle;">ML
                    </td>
                    <td align="center" style="width: 90px;">
                        <asp:TextBox CssClass="form-control" ID="txtAwayMoneyLine" Text='<%#FormatValue(Container.DataItem("AwayMoneyLine"))%>'
                            runat="server" onkeypress="javascript:return inputNumber(this,event, true);" MaxLength="7" />
                    </td>
                    <td rowspan="2" style="width: 95px;">
                        <asp:DropDownList ID="ddlContext" runat="server">
                            <asp:ListItem Value="Current">Current</asp:ListItem>
                            <asp:ListItem Value="1H">1H</asp:ListItem>
                            <asp:ListItem Value="2H">2H</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td rowspan="2" style="width: 30px; vertical-align: middle;">Score
                    </td>
                    <td style="width: 75px;">
                        <asp:TextBox CssClass="form-control" ID="txtAwayScore" Text='<%#FormatValue(Container.DataItem("AwayScore"))%>'
                            runat="server" onkeypress="javascript:return inputNumber(this,event, true);" MaxLength="3" />
                    </td>
                    <td rowspan="3">
                        <asp:Button ID="btnSave" runat="server" Text="Update" CssClass="btn btn-sm btn-primary " CommandName="UPDATE" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:TextBox CssClass="form-control" ID="txtHomeRotationNumber"
                            runat="server" onkeypress="javascript:return inputNumber(this,event, true);" MaxLength="7" Text='<%#Container.DataItem("HomeRotationNumber")%>' />
                    </td>
                    <td>
                        <asp:TextBox CssClass="form-control" ID="txtHomeTeam" runat="server" Text='<%#Container.DataItem("HomeTeam")%>' />
                    </td>
                    <td align="center">
                        <asp:TextBox CssClass="form-control" ID="txtHomeSpread" runat="server" Enabled="false" Text='<%#FormatValue(Container.DataItem("HomeSpread"))%>'
                            onkeypress="javascript:return inputNumber(this,event, true);"
                            MaxLength="7" Style="display: inline-block;" Width="65px" />
                        <span>-</span>
                        <asp:TextBox CssClass="form-control" ID="txtHomeSpreadMoney" runat="server" Text='<%#FormatValue(Container.DataItem("HomeSpreadMoney"))%>'
                            onkeypress="javascript:return inputNumber(this,event, true);" MaxLength="7" Style="display: inline-block;" Width="65px" />
                    </td>
                    <td align="center">
                        <span>U</span>
                        <asp:TextBox CssClass="form-control" ID="txtTotalPoints2" Enabled="false" Text='<%#FormatValue(Container.DataItem("TotalPoints"))%>'
                            runat="server" onkeypress="javascript:return inputNumber(this,event, true);"
                            MaxLength="7" Style="display: inline-block;" Width="65px" />
                        <span>-</span>
                        <asp:TextBox CssClass="form-control" ID="txtTotalPointsUnderMoney" Text='<%#FormatValue(Container.DataItem("TotalPointsUnderMoney"))%>'
                            runat="server" onkeypress="javascript:return inputNumber(this,event, true);" MaxLength="7" Style="display: inline-block;" Width="65px" />
                    </td>
                    <td align="center">
                        <asp:TextBox CssClass="form-control" ID="txtHomeMoney" runat="server" Text='<%#FormatValue(Container.DataItem("HomeMoneyLine"))%>'
                            onkeypress="javascript:return inputNumber(this,event, true);" MaxLength="7" />
                    </td>
                    <td>
                        <asp:TextBox CssClass="form-control" ID="txtHomeScore" Text='<%#FormatValue(Container.DataItem("HomeScore"))%>'
                            runat="server" onkeypress="javascript:return inputNumber(this,event, true);" MaxLength="3" />
                    </td>
                </tr>
                <tr>

                    <td class="" align="right">
                        <asp:TextBox CssClass="form-control" ID="txtDrawnRotationNumber"
                            runat="server" onkeypress="javascript:return inputNumber(this,event, true);" Text='<%#FormatValue(Container.DataItem("DrawRotationNumber"))%>' MaxLength="7" />
                    </td>
                    <td colspan="7" align="right">
                        <span>Draw</span>
                        <asp:TextBox CssClass="form-control" ID="txtDrawMoney" runat="server" Text='<%#FormatValue(Container.DataItem("DrawMoneyLine"))%>'
                            onkeypress="javascript:return inputNumber(this,event, true);" MaxLength="7" Width="65px" Style="display: inline-block;" />
                    </td>
                    <td></td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td colspan="14">
                        <hr />
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>
    <div class="clearfix"></div>
</div>
<div class="mbxl"></div>

