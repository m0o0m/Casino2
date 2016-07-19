<%@ Control Language="VB" AutoEventWireup="false" CodeFile="AgentPositionReport.ascx.vb"
    Inherits="SBCWebsite.AgentPositionReport" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>
<style type="text/css">
    #sportnav {
        border: #777 solid 1px;
        padding: 5px;
    }

    .gameType li {
        display: inline;
        list-style-type: none;
    }

    .subGameType {
        margin: 6px;
        margin-left: 25px;
    }

    .menu {
        font-weight: bolder;
        text-decoration: none;
    }

    .subMenu {
        text-decoration: none;
    }
</style>

<div class="panel panel-grey">
    <div class="panel-heading">Click on the Deference Amount to see details</div>
    <div class="panel-body">
        <div id="agentDrop" runat="server" visible="false" class="form-group">
            <label class="col-md-2 control-label">Agents</label>
            <div class="col-md-2">
                <wlb:CDropDownList ID="ddlAgents" runat="server" CssClass="form-control"
                    hasOptionalItem="true" OptionalText="All" OptionalValue="" AutoPostBack="true" />
            </div>
        </div>
        <div class="form-group">
            <label class="col-md-1 control-label w120 pt2">Game Type</label>
            <div class="col-md-6">
                <asp:RadioButtonList ID="rblContext" RepeatDirection="Horizontal" AutoPostBack="true" runat="server" RepeatLayout="Flow">
                    <asp:ListItem Selected="True" Value="Current"> 
                       <span style="position:relative;top:-3px; margin-left: 4px; margin-right: 20px"> Current</span> 
                    </asp:ListItem>
                    <asp:ListItem Value="1H"> 
                      <span style="position:relative;top:-3px; margin-left: 4px; margin-right: 20px"> 1H</span> 
                    </asp:ListItem>
                    <asp:ListItem Value="2H"> 
                      <span style="position:relative;top:-3px; margin-left: 4px; margin-right: 20px"> 2H</span> 
                    </asp:ListItem>
                </asp:RadioButtonList>
            </div>
        </div>
    </div>
</div>

<div class="form-group">
    <div class="col-lg-4">
        <div class="panel panel-grey">
            <div class="panel-heading"></div>
            <div class="panel-body">
                <ul class="gameType">
                    <asp:Repeater runat="server" ID="rptGameType">
                        <ItemTemplate>
                            <li id="liParent" runat="server">
                                <span class="menu">
                                    <%#Container.DataItem.Key%></span>
                                <asp:Panel ID="pnSubMenu" runat="server" ToolTip='<%#Container.DataItem.Key%>'>
                                    <div class="subGameType">
                                        <asp:Repeater ID="rptSubGameType" runat="server">
                                            <HeaderTemplate>
                                                <table cellspacing="1">
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <tr>
                                                    <td id="tdSub" runat="server">
                                                        <asp:LinkButton ID="lbtGameType" runat="server" CssClass="subMenu" OnClick="lbtGameType_Click"
                                                            Text='<%#Container.DataItem.Key%>'></asp:LinkButton>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                </table>
                                            </FooterTemplate>
                                        </asp:Repeater>
                                    </div>
                                </asp:Panel>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
            </div>
        </div>
    </div>
    <div class="col-lg-8">
        <div class="panel panel-grey">
            <div id="LineType" class="panel-heading"><%= SelectedGameType()%>&nbsp;<%=rblContext.SelectedValue%>&nbsp; Lines</div>
            <div id="games" class="panel-body">
                <div runat="server" id="divGame">
                    <asp:Label ID="lblNoGameLine" runat="server" Style="margin-left: 20px;">There is no <%= SelectedGameType()%> game.</asp:Label>
                    <asp:Repeater runat="server" ID="rptGameLines" OnItemDataBound="rptGameLines_ItemDataBound">
                        <ItemTemplate>
                            <table class="<%# IIF(Container.ItemType = ListItemType.Item,"table-striped","") %> table table-hover table-bordered">
                                <thead>
                                    <tr class="tableheading" style="text-align: center;">
                                        <th width="5%">Game
                                        </th>
                                        <th width="30%">
                                            <%#CDate(Container.DataItem("GameDate"))%>
                                        </th>
                                        <th width="20%" id="tdSpread" runat="server">
                                            <%#GetSpreadTitle("Spread")%>
                                        </th>
                                        <th width="20%" id="tdTotal" runat="server">
                                            <%#GetSpreadTitle("Total")%>
                                        </th>
                                        <th width="10%" id="tdMLine" runat="server">MLine
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td class="game_num">
                                            <asp:Literal ID="lblAwayNumber" runat="server" Text='<%#Container.DataItem("AwayRotationNumber")%>'></asp:Literal>
                                        </td>
                                        <td>
                                            <asp:Literal ID="lblAwayTeam" runat="server" Text='<%#Container.DataItem("AwayTeam")%>'></asp:Literal>
                                            <span style="color: Red;">
                                                <asp:Literal ID="lblAwayPitcher" runat="server"></asp:Literal>
                                            </span>
                                            <asp:Literal ID="lblAwayPitcherRightHand" runat="server"></asp:Literal>
                                        </td>
                                        <td id="tdSpread2" runat="server">
                                            <asp:Label CssClass="betinput_red" Style="display: block; height: 18px; text-align: center; padding-top: 2px"
                                                ID="lblAwaySpread" runat="server" />
                                        </td>
                                        <td id="tdTotal2" runat="server">
                                            <asp:Label CssClass="betinput_red" Style="display: block; height: 18px; text-align: center; padding-top: 2px"
                                                ID="lblAwayTotal" runat="server" />
                                        </td>
                                        <td id="tdMLine2" runat="server">
                                            <asp:Label CssClass="betinput_red" Style="display: block; height: 18px; text-align: center; padding-top: 2px"
                                                ID="lblAwayMoneyLine" runat="server" Width="60" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="game_num">
                                            <asp:Literal ID="lblHomeNumber" runat="server" Text='<%#Container.DataItem("HomeRotationNumber")%>'></asp:Literal>
                                        </td>
                                        <td>
                                            <asp:Literal ID="lblHomeTeam" runat="server" Text='<%#Container.DataItem("HomeTeam")%>'></asp:Literal>
                                            <span style="color: Red">
                                                <asp:Literal ID="lblHomePitcher" runat="server"></asp:Literal></span>
                                            <asp:Literal ID="lblHomePitcherRightHand" runat="server"></asp:Literal>
                                        </td>
                                        <td id="tdSpread3" runat="server">
                                            <asp:Label CssClass="betinput_red" Style="display: block; height: 18px; text-align: center; padding-top: 2px"
                                                ID="lblHomeSpread" runat="server" />
                                        </td>
                                        <td id="tdTotal3" runat="server">
                                            <asp:Label CssClass="betinput_red" Style="display: block; height: 18px; text-align: center; padding-top: 2px"
                                                ID="lblHomeTotal" runat="server" />
                                        </td>
                                        <td id="tdMLine3" runat="server">
                                            <asp:Label CssClass="betinput_red" Style="display: block; height: 18px; text-align: center; padding-top: 2px"
                                                ID="lblHomeMoney" runat="server" Width="60" />
                                        </td>
                                    </tr>

                                    <tr>
                                        <td class="game_num"></td>
                                        <td>Difference Amount 
                                        </td>
                                        <td>
                                            <asp:Label CssClass="betinput_red" Style="color: Blue; display: block; height: 18px; text-align: center; padding-top: 2px"
                                                ID="lblDiffSpread" runat="server" />
                                        </td>
                                        <td>
                                            <asp:Label CssClass="betinput_red" Style="color: Blue; display: block; height: 18px; text-align: center; padding-top: 2px"
                                                ID="lblDiffTotal" runat="server" />
                                        </td>
                                        <td>
                                            <asp:Label CssClass="betinput_red" Style="color: Blue; display: block; height: 18px; text-align: center; padding-top: 2px"
                                                ID="lblDiffMLine" runat="server" Width="60" />
                                        </td>
                                    </tr>
                                    <tr id="trDraw" runat="server">
                                        <td class="game_num"></td>
                                        <td>Draw
                                        </td>
                                        <td></td>
                                        <td></td>
                                        <td>
                                            <asp:Label CssClass="betinput_red" Style="color: Blue; display: block; height: 18px; text-align: center; padding-top: 2px"
                                                ID="lblDrawMoney" runat="server" Width="60" />
                                        </td>
                                    </tr>

                                    <tr id="rowTotal" runat="server" visible="false" class="tableheading">
                                        <td style="text-indent: 5px" colspan="2">Total Amount
                                        </td>
                                        <td>
                                            <asp:Label CssClass="betinput_red" Style="display: block; height: 18px; text-align: center; padding-top: 2px; color: Black"
                                                ID="lblTotalSpread" runat="server" />
                                        </td>
                                        <td>
                                            <asp:Label CssClass="betinput_red" Style="display: block; height: 18px; text-align: center; padding-top: 2px; color: Black"
                                                ID="lblTotalTotal" runat="server" />
                                        </td>
                                        <td>
                                            <asp:Label CssClass="betinput_red" Style="display: block; height: 18px; text-align: center; padding-top: 2px; color: Black"
                                                ID="lblTotalMline" runat="server" Width="60" />
                                        </td>
                                    </tr>

                                </tbody>
                            </table>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>
    </div>
</div>

