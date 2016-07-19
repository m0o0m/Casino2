<%@ Control Language="VB" AutoEventWireup="false" CodeFile="SportAllowance.ascx.vb" Inherits="SBSAgents.SportAllowance" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>
<div style="text-align:right;padding-right:12px;padding-bottom :5px">
<asp:Button ID="btnSave" runat="server" Text="Save"   />
</div> 
<asp:Repeater ID="rptParentNodes" runat="server">
    <ItemTemplate>
         <table class="gamebox" border="1" cellpadding="0" cellspacing="0" style="float: left; margin-left: 10px;border-collapse:collapse;width: 419px;">
            <tr>
                <td class="tableheading" nowrap="nowrap" >
                   <asp:Label runat="server" ID="lblTitle" Text='<%# Container.DataItem.Key %>' style="padding-left:5px" ></asp:Label>
                </td>
            </tr>
            <asp:Repeater ID="rptSubNodes" runat="server" OnItemDataBound="rptSubNodes_ItemDataBound" OnItemCommand="rptSubNodes_ItemCommand">
                <HeaderTemplate>
                    <tr>
                        <td>
                            <div style="padding-top:10px;margin-left: 20px; height: 150px; overflow: auto;">
                                <table cellpadding="2" cellspacing="0">
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
                                Visible="false" CommandName="UPDATE" CssClass="textInput" Style="vertical-align: middle;" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" ToolTip="Cancel Update Bookmaker"
                                Visible="false" CommandName="CANCEL" CssClass="textInput" Style="vertical-align: middle;" />

                            <asp:Button ID="btnSubEdit" runat="server" Text="Edit" ToolTip="Edit Bookmaker" CommandName="EDIT"
                                CommandArgument='<%# Container.DataItem.SysSettingID & "|" & Container.DataItem.Key %>'
                                CssClass="textInput" Style="vertical-align: middle;" />
                        </td>
                        <td>
                         <asp:CheckBox ID="chkDisbleGame" runat="server"  Style="vertical-align: bottom;position:relative;bottom:-2px;margin-left:30px"  /><span style="font-size:11px" >Enable</span> 
                            <asp:HiddenField ID="HFGame" Value='<%# Container.DataItem.Key %>'
                                runat="server" />
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </table> </div></td> </tr>
                </FooterTemplate>
            </asp:Repeater>
        </table>
    </ItemTemplate>
</asp:Repeater>
