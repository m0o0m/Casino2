<%@ Control Language="VB" AutoEventWireup="false" CodeFile="PartnerTopMenu.ascx.vb" Inherits="SBSSuperAdmins.PartnerTopMenu" %>
<asp:Repeater runat="server" ID="rptMenu">
    <HeaderTemplate>
    </HeaderTemplate>
    <ItemTemplate>
        <li id="liMenu" runat="server" menu='<%#Container.DataItem.Key%>' class="selected">
            <%#Container.DataItem.Key%>
                <asp:Repeater ID="rptSubMenu" runat="server" OnItemCommand="rptSubMenu_ItemCommand">
                    <HeaderTemplate>
                    <div>
                        <table cellspacing="0" cellpadding="0">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td id="tdSubMenu" runat="server" menu='<%#Container.DataItem("MenuValue")%>' class="selected">
                                <asp:LinkButton ID="lbtnSubMenu" runat="server" Text='<%#Container.DataItem("MenuText")%>' 
                                ToolTip='<%#Container.DataItem("MenuToolTip")%>' CommandArgument='<%#Container.DataItem("MenuUrl")%>' 
                                CommandName="CLICK"></asp:LinkButton>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table>
                    </div>
                    </FooterTemplate>
                </asp:Repeater>
            
        </li>
    </ItemTemplate>
    <FooterTemplate>
    </FooterTemplate>
</asp:Repeater>
