<%@ Control Language="VB" AutoEventWireup="false" CodeFile="OddsRulesSetup.ascx.vb"
    Inherits="SBCAgents.Inc_OddsRulesSetup" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="cc1" %>

<div class="col-lg-12">
    <table class="table table-hover">
        <tr runat="server" id="rowAgent">
            <td style="white-space: nowrap">Agent
            </td>
            <td colspan="3">
                <cc1:CDropDownList runat="server" ID="ddlAgents" AutoPostBack="true" hasOptionalItem="true"
                    OptionalValue="">
                </cc1:CDropDownList>
            </td>
            <td style="padding-right: 10px">&nbsp;
            </td>
        </tr>
        <tr>
            <td style="white-space: nowrap; padding-top: 10px;">Total Betting Amount:
            </td>
            <td>&nbsp;
            </td>
            <td style="padding-right: 10px">&nbsp;
            </td>
            <td>&nbsp;
            </td>
            <td style="padding-right: 10px">&nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="5">
                <table class="table table-hover">
                    <tr>
                        <td>
                            <asp:Label ID="lblGreaterThan" runat="server" Text="In The Range From"></asp:Label>
                        </td>
                        <td style="padding-right: 10px">
                            <asp:TextBox ID="txtGreaterThan" Width="70px" CssClass="textInput" onkeypress="javascript:return inputNumber(this,event, true);"
                                runat="server"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="lblLowerThan" runat="server" Text="To"></asp:Label>
                        </td>
                        <td style="padding-right: 10px">
                            <asp:TextBox ID="txtLowerThan" Width="70px" CssClass="textInput" onkeypress="javascript:return inputNumber(this,event, true);"
                                runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr valign="bottom" height="22">
                        <td align="left">Increase Juice To
                        </td>
                        <td style="padding-right: 10px">
                            <asp:TextBox ID="txtIncrease" Width="70px" CssClass="textInput" onkeypress="javascript:return inputNumber(this,event, true);"
                                runat="server"></asp:TextBox>&nbsp;%
                        </td>
                        <td>Game Off
                        </td>
                        <td style="padding-right: 10px">
                            <asp:CheckBox ID="chkOddRuleLocked" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right; padding-right: 12px" colspan="4">
                            <div style="margin-top: 7px">
                                <asp:Button ID="btnAdd" runat="server" Text="Add" CssClass="btn btn-primary" CausesValidation="false" />&nbsp;
                                        <asp:Button ID="btnUpdate" runat="server" Text="Update" CssClass="btn btn-primary" Enabled="false"
                                            CausesValidation="false" />
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</div>
<div class="col-lg-12">
    <asp:DataGrid ID="dgOddsRules" runat="server" AutoGenerateColumns="false" CssClass="table table-hover table-bordered">
        <HeaderStyle CssClass="tableheading" HorizontalAlign="Center" Font-Size="11px" />
        <ItemStyle HorizontalAlign="Left" Wrap="false" />
        <AlternatingItemStyle HorizontalAlign="Left" Wrap="false" />
        <SelectedItemStyle BackColor="YellowGreen" />
        <Columns>
            <asp:BoundColumn DataField="GreaterThan" HeaderText="<nobr>From</nobr>" ItemStyle-Width="70"
                ItemStyle-HorizontalAlign="Center" />
            <asp:BoundColumn DataField="LessThan" HeaderText="To" ItemStyle-Width="70" ItemStyle-HorizontalAlign="Center" />
            <asp:BoundColumn DataField="Increase" HeaderText="% Increase" ItemStyle-Width="70"
                ItemStyle-HorizontalAlign="Center" />
            <asp:TemplateColumn HeaderText="Game Off">
                <ItemTemplate>
                    <div style="text-align: center">
                        <%#SBCBL.std.SafeString(IIf(SBCBL.std.SafeBoolean(DataBinder.Eval(Container.DataItem, "LockGame")), "Y", "N"))%>
                    </div>
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn>
                <ItemTemplate>
                    <asp:LinkButton ID="lbtEditOddsRules" runat="server" CommandArgument='<%# SBCBL.std.SafeString( DataBinder.Eval(Container.DataItem, "ID") ) %>'
                        CommandName="EditOddsRules" ToolTip="Edit OddsRules" Text="Edit" Font-Underline="false"
                        CausesValidation="false" CssClass="itemplayer" />
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn>
                <ItemTemplate>
                    <asp:LinkButton ID="lbtDeleteOddsRules" runat="server" OnClientClick=" return confirm('Are you sure delete this item?')"
                        CommandArgument='<%# SBCBL.std.SafeString(DataBinder.Eval(Container.DataItem, "ID")) %>'
                        CommandName="DeleteOddsRules" ToolTip="Delete OddsRules" Text="Delete" Font-Underline="false"
                        CausesValidation="false" CssClass="itemplayer" />
                </ItemTemplate>
            </asp:TemplateColumn>
        </Columns>
    </asp:DataGrid>
</div>

