<%@ Control Language="VB" AutoEventWireup="false" CodeFile="SysSettingManager.ascx.vb" Inherits="SBCSuperAdmins.SysSettingManager" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="cc1" %>
<table class="table table-hover">
    <tr class="tableheading">
        <th>
            CATEGORY
        </th>
        <th>
            SUB CATEGORY
        </th>
        <th>
            NAME - VALUE
        </th>
    </tr>
    <asp:Repeater ID="rptCategories" runat="server">
        <SeparatorTemplate>
            <tr>
                <td colspan="3">
                    &nbsp;
                </td>
            </tr>
        </SeparatorTemplate>
        <ItemTemplate>
            <tr>
                <td valign="top" nowrap>
                    <asp:Label runat="server" ID="lblCategory" Font-Bold="true" />
                    <div style="padding-top: 5px;">
                        <asp:ImageButton runat="server" ID="ibtEditCategory" CommandName="EDIT_CATEGORY"
                            ImageUrl="~/images/icn_modify.gif" ToolTip="Edit Category" CausesValidation="false" />
                        <asp:ImageButton runat="server" ID="ibtDeleteCategory" CommandName="DELETE_CATEGORY"
                            ImageUrl="~/images/icn_delete.gif" ToolTip="Delete Category" CausesValidation="false" />
                        <cc1:CButtonConfirmer ID="Cbuttonconfirmer3" runat="server" AttachTo="ibtDeleteCategory"
                            ConfirmExpression="confirm('Are You Sure Want To Delete This Category?')">
                        </cc1:CButtonConfirmer>
                    </div>
                </td>
                <td valign="top" nowrap>
                    <cc1:CDropDownList ID="ddlSubCategory" AutoPostBack="true" OnSelectedIndexChanged="ddlSubCategory_SelectedChanged"
                        CausesValidation="false" runat="server" CssClass="textInput" OptionalText="--- None ---"
                        OptionalValue="" />
                    <asp:ImageButton runat="server" ID="ibtEditSubCategory" CommandName="EDIT_SUB_CATEGORY"
                        ImageUrl="~/images/icn_modify.gif" ToolTip="Edit Sub Category" CausesValidation="false"
                        Visible="false" />
                    <asp:ImageButton runat="server" ID="ibtDeleteSubCategory" CommandName="DELETE_SUB_CATEGORY"
                        ImageUrl="~/images/icn_delete.gif" ToolTip="Delete Sub Category" CausesValidation="false"
                        Visible="false" />
                    <cc1:CButtonConfirmer ID="Cbuttonconfirmer2" runat="server" AttachTo="ibtDeleteSubCategory"
                        ConfirmExpression="confirm('Are You Sure Want To Delete This Sub Category?')" />
                    <table id="tblEditSubCategory" runat="server" visible="false" class="table table-hover-color">
                        <tr class="tableheading">
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div style="padding: 5px;">
                                    <asp:TextBox CssClass="textInput" ID="txtSubCategory" runat="server" MaxLength="20" />
                                    <div style="text-align: right; padding-top: 5px;">
                                        <asp:Button ID="btnSaveSubCategory" runat="server" Text="Save" OnClick="SaveSubCategory_Click"
                                            CausesValidation="false" CssClass="button" ToolTip="Save sub category" />
                                        <asp:Button ID="btnCancelSub" runat="server" Text="Cancel" OnClick="CloseFormEditSubCategory_Click"
                                            CausesValidation="false" CssClass="button" ToolTip="Cancel" />
                                    </div>
                                </div>
                            </td>
                        </tr>
                    </table>
                </td>
                <td valign="top">
                    <table cellpadding="3" cellspacing="3">
                        <asp:Repeater runat="server" ID="rptKeyValues" OnItemDataBound="rptKeyValues_ItemDataBound">
                            <ItemTemplate>
                                <tr>
                                    <td nowrap>
                                        <asp:Label runat="server" ID="lblKey" Text='<%#Container.DataItem("Key") %>' />
                                    </td>
                                    <td>
                                        :
                                    </td>
                                    <td>
                                        <asp:Label runat="server" ID="lblValue" Text='<%#Container.DataItem("Value") %>'
                                            Visible="true" />
                                    </td>
                                    <td>
                                        <asp:ImageButton runat="server" ID="ibtEditKeyValue" ImageUrl="~/images/icn_modify.gif"
                                            ToolTip="Edit Sub Category" CausesValidation="false" OnClick="OpenFormEditKeyValue_Click"
                                            CommandArgument='<%#Container.DataItem("SysSettingID") %>' />
                                        <asp:ImageButton runat="server" ID="ibtDeleteKeyValue" ImageUrl="~/images/icn_delete.gif"
                                            ToolTip="Delete Sub Category" CausesValidation="false" OnClick="DeleteKeyValue_Click"
                                            CommandArgument='<%#Container.DataItem("SysSettingID") %>' />
                                        <cc1:CButtonConfirmer ID="Cbuttonconfirmer2" runat="server" AttachTo="ibtDeleteKeyValue"
                                            ConfirmExpression="confirm('Are You Sure Want To Remove This Value?')">
                                        </cc1:CButtonConfirmer>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                        <tr>
                            <td colspan="4">
                                <asp:LinkButton CommandName="ADD_KEY_VALUE" runat="server" ID="lbnAddKeyValue" CausesValidation="false"
                                    ToolTip="Add new Name & Value" style="text-decoration:none;">
                                    <asp:Image ID="img" runat="server" ImageUrl="~/images/add2.gif" ImageAlign="AbsMiddle" />
                                    &nbsp;<b>Add New</b>
                                </asp:LinkButton>
                            </td>
                        </tr>
                    </table>
                    <table id="tblEditKeyValue" visible="false" runat="server" class="table table-hover-color">
                        <tr>
                            <td>
                                Name
                            </td>
                            <td>
                                <asp:TextBox ID="txtNewKey" runat="server" CssClass="textInput" MaxLength="50" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Value
                            </td>
                            <td>
                                <asp:TextBox ID="txtNewValue" runat="server" CssClass="textInput" MaxLength="50" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td>
                                <asp:Button ID="btnSaveKeyValue" runat="server" Text="Save" OnClick="SaveKeyValue_Click"
                                    CausesValidation="false" CssClass="button" ToolTip="Save new Name and Value"
                                    Width="50" />
                                <asp:Button ID="btnKeyValueCancel" runat="server" Text="Cancel" OnClick="CloseFormEditKeyValue_Click"
                                    CausesValidation="false" CssClass="button" ToolTip="Cancel" Width="70" />
                                <asp:HiddenField ID="hfSettingID" runat="server" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
    <tr>
        <td>
            <b>Rounding Option</b>
        </td>
        <td colspan="2">
            <asp:RadioButtonList ID="rblRoundingOption" runat="server" AutoPostBack="true" RepeatDirection="Horizontal">
                <asp:ListItem Text="None" Value="None" />
                <asp:ListItem Text="Up" Value="Up" />
                <asp:ListItem Text="Down" Value="Down" />
                <asp:ListItem Text="Nearest" Value="Nearest" Selected="True" />
            </asp:RadioButtonList>
            <asp:HiddenField ID="hfSettingID" runat="server" />
        </td>
    </tr>
</table>
<table id="tblEditCategory" runat="server" visible="false" class="table table-hover">
    <tr>
        <th colspan="2">
            Add/Edit Setting
        </th>
    </tr>
    <tr>
        <td>
            Category Name:
        </td>
        <td>
            <asp:TextBox CssClass="textInput" ID="txtCategory" runat="server" MaxLength="20"
                Width="200" />
        </td>
    </tr>
    <tr>
        <td>
        </td>
        <td>
            <asp:Button ID="btnSaveCategory" runat="server" Text="Save" OnClick="SaveCategory_Click"
                CausesValidation="false" CssClass="button" Width="60" ToolTip="Save" />
            <asp:Button ID="btnCancelSetting" runat="server" Text="Cancel" OnClick="CloseFormEditCategory_Click"
                CausesValidation="false" CssClass="button" Width="60" ToolTip="Cancel" />
        </td>
    </tr>
</table>
<asp:Button ID="btnAddCategory" runat="server" Text="Create New Setting" ToolTip="Create New Setting"
    CssClass="button" CausesValidation="false" OnClick="OpenFormEditCategory_Click" />
