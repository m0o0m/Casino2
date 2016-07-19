<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ManualProposition.ascx.vb"
    Inherits="SBCSuperAdmin.ManualProposition" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>
<div id="dateRange" style="color: #000; margin-bottom: 4px; margin-top: 3px">
    <span style="margin-right: 10px; margin-left: 3px">Game Type : </span>
    <wlb:CDropDownList ID="ddlGameType" runat="server" CssClass="form-control" hasOptionalItem="false"
        AutoPostBack="true" Style="display: inline-block;" Width="230px" >
    </wlb:CDropDownList>
</div>
<div style="float: left; margin-top: 3px;">
    <asp:Repeater runat="server" ID="rptProps">
        <ItemTemplate>
            <div id="LineType" style="width: 700px">
                <a>
                    <%#Container.DataItem & " Proposition"%></a>
            </div>
            <div runat="server" id="divGame" visible='<%#Not CollapsedGames.Contains(Container.DataItem & "_Proposition" )%>'>
                <asp:Repeater runat="server" ID="rptPropLines" OnItemDataBound="rptPropLines_ItemDataBound">
                    <ItemTemplate>
                        <div runat="server" id="divProp" style="margin-left: 140px;">
                            <table width="100%" class="<%# IIF(Container.ItemType = ListItemType.Item,"gametable_odd","gametable_even") %> table table-hover table-bordered">
                                <colgroup>
                                    <col width="60px" />
                                    <col width="*" />
                                    <col width="350px" />
                                    <col width="90px" />
                                </colgroup>
                                <tbody>
                                    <tr class="tableheading" style="text-align: center;">
                                        <td>
                                            Game
                                        </td>
                                        <td colspan="2">
                                            <asp:Literal runat="server" ID="lblGameDate"></asp:Literal>
                                            -
                                            <asp:Literal runat="server" ID="lblPropDes"></asp:Literal>
                                        </td>
                                        <td>
                                            MLine
                                        </td>
                                    </tr>
                                    <asp:Repeater runat="server" ID="rptPropTeams" OnItemDataBound="rptPropTeams_ItemdataBound">
                                        <ItemTemplate>
                                            <tr>
                                                <td class="game_num" style="text-align: center;">
                                                    <asp:Literal ID="lblPropRotationNumber" runat="server" Text='<%#Container.DataItem("PropRotationNumber")%>'></asp:Literal>
                                                </td>
                                                <td style="padding-left: 10px;">
                                                    <asp:Literal ID="lbnPropParticipantName" runat="server" Text='<%#Container.DataItem("PropParticipantName")%>'></asp:Literal>
                                                </td>
                                                <td>
                                                    <div style="position: relative; bottom: 0; display: inline; text-align: left; padding-left: 10px;">
                                                        <asp:RadioButton ID="rdWin" GroupName="Prop" runat="server" /><span style="position: relative;
                                                            bottom: 2px; padding-right: 10px;">WIN</span> <span style="padding-right: 10px;">
                                                                <asp:RadioButton ID="rdLose" runat="server" GroupName="Prop" /><span style="position: relative;
                                                                    bottom: 2px;">LOSE</span></span>
                                                        <asp:RadioButton ID="rdCancel" runat="server" GroupName="Prop" /><span style="position: relative;
                                                            bottom: 2px; padding-left: 10px;">CANCELLED</span>
                                                        <asp:RadioButton ID="rdPending" runat="server" GroupName="Prop" /><span style="position: relative;
                                                            bottom: 2px; padding-left: 10px;">PENDING</span>
                                                    </div>
                                                    <asp:HiddenField ID="hfGameID" runat="server" Value='<%#Container.DataItem("GameID")%>'>
                                                    </asp:HiddenField>
                                                    <asp:HiddenField ID="hfGameLineID" runat="server" Value='<%#Container.DataItem("GameLineID")%>'>
                                                    </asp:HiddenField>
                                                    <asp:HiddenField ID="hfGameType" runat="server" Value='<%#Container.DataItem("GameType")%>'>
                                                    </asp:HiddenField>
                                                </td>
                                                <td id="tdMLine2" runat="server" align="right">
                                                    <asp:Label CssClass="betinput_red" Style="text-align: center; height: 20px; vertical-align: middle;
                                                        padding-top: 7px;" ID="lblPropMoneyLine" runat="server" Width="60" CommandName="PROP_LINE" />
                                                </td>
                                            </tr>
                                            <%-- <tr>
                                                <td colspan="5" style="border-bottom:1px solid #000;"></td>
                                            </tr>--%>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <tr>
                                        <td colspan="4" style="text-align: right">
                                            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Save" CssClass="button"
                                                Style="margin-right: 10px" />
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <br />
        </ItemTemplate>
    </asp:Repeater>
</div>
