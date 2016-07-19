<%@ Control Language="VB" AutoEventWireup="false" CodeFile="SportAllowance.ascx.vb"
    Inherits="SBCAgents.Inc_SportAllowance" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>

<div class="row">
    <div class="col-md-7" runat="server" id="rowAgent">
        <div class="form-group">
            <label class="col-md-2 control-label">Agent</label>
            <div class="col-md-7">
                <wlb:CDropDownList runat="server" ID="ddlSuperAgents" CssClass="form-control" AutoPostBack="True"></wlb:CDropDownList>
            </div>
        </div>
    </div>
    <div class="col-md-5 pull-right">
        <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary pull-right" />
    </div>
    <div class="clearfix"></div>
</div>
<div class="mbxl"></div>

<div class="row">
    <asp:Repeater ID="rptParentNodes" runat="server">
        <ItemTemplate>
            <div class="col-md-4">
                <div class="panel panel-grey">
                    <div class="panel-heading">
                        <asp:Label runat="server" ID="lblTitle" Text='<%# Container.DataItem.Key %>'></asp:Label>
                    </div>
                    <div class="panel-body">
                        <asp:Repeater ID="rptSubNodes" runat="server" OnItemDataBound="rptSubNodes_ItemDataBound"
                            OnItemCommand="rptSubNodes_ItemCommand">
                            <HeaderTemplate>
                                <div style="height: 190px; overflow: auto; margin-bottom: 10px;">
                                    <table class="table table-hover">
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td nowrap="nowrap">
                                        <asp:Label ID="lblSubActive" runat="server" Style="vertical-align: middle;" />
                                        <wlb:CDropDownList ID="ddlChoiceBookmaker" runat="server" hasOptionalItem="false"
                                            Visible="false" OptionalText="" OptionalValue="" Style="vertical-align: middle;"
                                            CssClass="textInput">
                                        </wlb:CDropDownList>
                                        <asp:HiddenField ID="HFSysSetting" Value='<%# Container.DataItem.SysSettingID %>'
                                            runat="server" />
                                    </td>
                                    <td>
                                        <asp:Button ID="btnUpdate" runat="server" Text="Update" ToolTip="Update Bookmaker"
                                            Visible="false" CommandName="UPDATE" CssClass="button" Style="vertical-align: middle;" />
                                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" ToolTip="Cancel Update Bookmaker"
                                            Visible="false" CommandName="CANCEL" CssClass="button" Style="vertical-align: middle;" />
                                        <asp:Button ID="btnSubEdit" runat="server" Text="Edit" ToolTip="Edit Bookmaker" CommandName="EDIT"
                                            CommandArgument='<%# Container.DataItem.SysSettingID & "|" & Container.DataItem.Key %>'
                                            CssClass="button" Style="vertical-align: middle;" />
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chkDisbleGame" runat="server" Style="vertical-align: bottom; position: relative; bottom: -2px; margin-left: 30px" /><span style="font-size: 11px">Enable</span>
                                        <asp:HiddenField ID="HFGame" Value='<%# Container.DataItem.Key %>' runat="server" />
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                </table>
                                        </div>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>
