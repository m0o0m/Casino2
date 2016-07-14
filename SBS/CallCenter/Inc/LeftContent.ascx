<%@ Control Language="VB" AutoEventWireup="false" CodeFile="LeftContent.ascx.vb"
    Inherits="SBCCallCenterAgents.LeftContent" %>
<div id="leftcontent" style="width: 200px;">
    <asp:Repeater ID="rptMenus" runat="server">
        <HeaderTemplate>
            <ul>
        </HeaderTemplate>
        <ItemTemplate>
            <li>
                <asp:LinkButton ID="lbnMenu" runat="server" CommandArgument='<%# Container.DataItem("MenuValue") %>'
                    CommandName='<%# Container.DataItem("MenuUrl") %>' Text='<%# Container.DataItem("MenuText") %>'
                    ToolTip='<%# Container.DataItem("MenuToolTip") %>' /></li>
        </ItemTemplate>
        <FooterTemplate>
            </ul></FooterTemplate>
    </asp:Repeater>
</div>
