<%@ Control Language="VB" AutoEventWireup="false" CodeFile="GameTypeManager.ascx.vb"
    Inherits="SBCSuperAdmin.GameTypeManager" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>

<script type="text/javascript">
    function UpdateGameType(HFSysSettingID, HFKey, chkValue) {
        var sSysSettingID = document.getElementById(HFSysSettingID).value;
        var sKey = document.getElementById(HFKey).value;
        var sValue = "Yes";
        if (!chkValue.checked)
            sValue = "No";
        PageMethods.UpdateGameType(sSysSettingID, sKey, sValue);
    }
</script>
<div class="row">
    <div class="col-lg-12">
        <h4>Add New GameType</h4>
        <asp:CheckBox ID="chkActive" runat="server" Style="vertical-align: middle;" Text="Is Active: &nbsp;&nbsp;" TextAlign="Left" />
        &nbsp;&nbsp;
        <span>GameType:</span>
        &nbsp;&nbsp;
        <asp:TextBox ID="txtNode" runat="server" MaxLength="50" CssClass="form-control" Style="display: inline-block;" Width="230px"></asp:TextBox>
        &nbsp;&nbsp;
        <asp:Button ID="btnAddNode" runat="server" Text="Add Node" ToolTip="Add Parent Node" CssClass="btn btn-primary" Style="" />
    </div>
    <div class="clearfix"></div>
</div>
<div class="mbxl"></div>

<div class="row">
    <div class="col-lg-12">
        <asp:CheckBox ID="chkTigerSBBookmaker" Text="Only use Win1t Bookmaker." ToolTip="Win1t Bookmaker is used for manual game lines."
            ForeColor="Red" AutoPostBack="true" runat="server"></asp:CheckBox>
        &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:CheckBox ID="chkAssignBookMaker" Text="Only display lines of book maker assigned to game type."
            ForeColor="Red" AutoPostBack="true" runat="server"></asp:CheckBox>
    </div>
    <div class="clearfix"></div>
</div>
<div class="mbxl"></div>
<div class="row">
    <div class="col-lg-12">
        <asp:Repeater ID="rptParentNodes" runat="server">
            <ItemTemplate>

                <div class="col-lg-4 block-space">
                    <div class="portlet box mbl">
                        <div class="portlet-header" style="background-color: #faf2cc;">
                            <div class="caption">
                                <asp:CheckBox ID="chkParentActive" runat="server" Text='<%# Container.DataItem.Key %>'
                                    Style="vertical-align: middle;" OnCheckedChanged="chkSubActive_CheckedChanged" AutoPostBack="true" />
                                <asp:HiddenField ID="HFSysSetting" Value='<%# Container.DataItem.SysSettingID %>' runat="server" />
                            </div>
                            <div class="tools">
                                <asp:LinkButton ID="btnDelete" runat="server" Text="Delete" ToolTip="Delete Parent Node"
                                    CommandName="DELETE" CommandArgument='<%# Container.DataItem.SysSettingID %>'>
                                    <i class="fa fa-times"></i>
                                </asp:LinkButton>
                                <wlb:CButtonConfirmer ID="cfrmDelete" runat="server" AttachTo="btnDelete" ConfirmExpression="confirm('Are You Sure Want To Delete This Type?')">
                                </wlb:CButtonConfirmer>
                                <asp:LinkButton ID="lbtnMoveLeft" runat="server" Text="<<" ToolTip="Move Left" CommandName="LEFT"
                                    CommandArgument='<%# Container.ItemIndex %>'><i class="fa fa-chevron-left"></i></asp:LinkButton>
                                <asp:LinkButton ID="lbtnMoveRight" runat="server" Text=">>" ToolTip="Move Right"
                                    CommandName="RIGHT" CommandArgument='<%# Container.ItemIndex %>'><i class="fa fa-chevron-right"></i></asp:LinkButton>

                            </div>
                        </div>
                        <div class="portlet-body">
                            <asp:Repeater ID="rptSubNodes" runat="server" OnItemDataBound="rptSubNodes_ItemDataBound"
                                OnItemCommand="rptSubNodes_ItemCommand">
                                <HeaderTemplate>
                                    <div style="height: 190px; overflow: auto; margin-bottom: 10px;">
                                        <table class="table table-hover">
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td nowrap="nowrap">
                                            <asp:CheckBox ID="chkSubActive" runat="server" Style="vertical-align: middle;" />
                                            <wlb:CDropDownList ID="ddlChoiceBookmaker" runat="server" hasOptionalItem="false"
                                                Visible="false" OptionalText="" OptionalValue="" Style="vertical-align: middle;"
                                                CssClass="textInput">
                                            </wlb:CDropDownList>
                                            <asp:HiddenField ID="HFSysSetting" Value='<%# Container.DataItem.SysSettingID %>'
                                                runat="server" />
                                            <asp:HiddenField ID="HFKey" Value='<%# Container.DataItem.Key %>' runat="server" />
                                        </td>
                                        <td>
                                            <asp:Button ID="btnUpdate" runat="server" Text="Update" ToolTip="Update Bookmaker"
                                                Visible="false" CommandName="UPDATE" CssClass="button" />
                                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" ToolTip="Cancel Update Bookmaker"
                                                Visible="false" CommandName="CANCEL" CssClass="button" />
                                            <asp:Button ID="btnSubDelete" runat="server" Text="Delete" ToolTip="Delete Sub Node"
                                                CommandName="DELETE" CommandArgument='<%# Container.DataItem.SysSettingID & "|" & Container.DataItem.Key %>'
                                                CssClass="button" />
                                            <wlb:CButtonConfirmer ID="cfrmSubDelete" runat="server" AttachTo="btnSubDelete" ConfirmExpression="confirm('Are You Sure Want To Delete This Type?')">
                                            </wlb:CButtonConfirmer>
                                            <asp:Button ID="btnSubEdit" runat="server" Text="Edit" ToolTip="Edit Bookmaker" CommandName="EDIT"
                                                CommandArgument='<%# Container.DataItem.SysSettingID & "|" & Container.DataItem.Key %>'
                                                CssClass="button" />
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </table>
                                    </div>
                                </FooterTemplate>
                            </asp:Repeater>
                            <div class="row">
                                <div class="col-lg-12">
                                    <asp:CheckBox ID="chkSubActive" runat="server" />
                                    <wlb:CDropDownList ID="ddlGameType" runat="server" hasOptionalItem="true" OptionalText="Game Type"
                                        OptionalValue="" CssClass="textInput">
                                    </wlb:CDropDownList>
                                    <wlb:CDropDownList ID="ddlBookmaker" runat="server" hasOptionalItem="true" OptionalText="Book Maker"
                                        OptionalValue=""  CssClass="textInput">
                                    </wlb:CDropDownList>
                                    <asp:Button ID="btnAddNode" runat="server" Text="Add" ToolTip="Add Sub Node"
                                        CommandName="ADD" CommandArgument='<%# Container.DataItem.SysSettingID %>' CssClass="button" />
                                </div>
                                <div class="clearfix"></div>
                            </div>

                        </div>
                    </div>
                </div>

            </ItemTemplate>
        </asp:Repeater>
    </div>
    <div class="clearfix"></div>
</div>
<div class="mbxl"></div>


