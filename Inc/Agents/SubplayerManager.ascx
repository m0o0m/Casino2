<%@ Control Language="VB" AutoEventWireup="false" CodeFile="SubplayerManager.ascx.vb" Inherits="SBSAgents.SubplayerManager" %>
<%@ Import Namespace="System.ComponentModel" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/Inc/Agents/PlayerSubAgentEdit.ascx" TagName="PlayerSubAgentEdit" TagPrefix="uc1" %>

<asp:Panel ID="pnPlayerManager" runat="server">
Agent:
 <cc1:CDropDownList ID="ddlAgents" runat="server" CssClass="textInput" Width="200px" />
 &nbsp;&nbsp;&nbsp;<asp:Button ID="btnFilter" runat="server" Text="Filter" CssClass="button"
 Style="margin-right: 10px; width: 80px;" />
<asp:DataGrid ID="dgPlayers" runat="server" Width="100%" AutoGenerateColumns="false"
    CellPadding="2" CellSpacing="8" CssClass="gamebox" Style="padding: 5px;" AllowPaging="True"
    PageSize="30">
    <HeaderStyle CssClass="tableheading" HorizontalAlign="Center" />
    <ItemStyle HorizontalAlign="Left" />
    <PagerStyle Font-Names="tahoma" HorizontalAlign="Right" Mode="NumericPages" />
    <AlternatingItemStyle HorizontalAlign="Left" />
    <SelectedItemStyle BackColor="YellowGreen" />
    <Columns>
        <asp:TemplateColumn ItemStyle-HorizontalAlign="left" HeaderText="Name (Login)">
            <ItemTemplate>
                <nobr>
                <asp:HiddenField ID="hfPlayerTemplateID" runat="server" Value='<%# SBCBL.std.SafeString(Container.DataItem("PlayerTemplateID")) & "|" & SBCBL.std.SafeString(Container.DataItem("DefaultPlayerTemplateID")) %>' />
                <asp:LinkButton ID="lbtEdit" CssClass="itemplayer" runat="server" CommandArgument='<%#Container.DataItem("PlayerID") %>' CommandName="EditUser" Text='<%#Container.DataItem("Login") & " (" & Container.DataItem("Name")  & ")" %>'></asp:LinkButton> </nobr>
                <asp:HiddenField ID="hfLock" runat="server" Value='<%# SBCBL.std.SafeString(Container.DataItem("IsLocked")) %>'>
                </asp:HiddenField>
                <asp:HiddenField ID="hfBettingLock" runat="server" Value='<%# SBCBL.std.SafeString(Container.DataItem("IsBettingLocked")) %>'>
                </asp:HiddenField>
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Left" />
        </asp:TemplateColumn>
        <asp:TemplateColumn ItemStyle-HorizontalAlign="Center" HeaderText="Block">
            <ItemTemplate>
                <nobr>
                        <asp:LinkButton  CssClass="itemplayer" runat="server" style="text-decoration:none"  id="lbnLock" Text='<%#IIf(SBCBL.std.SafeString(Container.DataItem("IsLocked")).Equals("Y"),"Y","N")   %>'  CommandName="LOCK" CommandArgument='<%#  sbcbl.std.safestring(Container.DataItem("PlayerID"))  & "|" & Container.DataItem("Login")  %>' ></asp:LinkButton>
                        
                        </nobr>
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateColumn>
        <asp:TemplateColumn ItemStyle-HorizontalAlign="Center" HeaderText="Block Bet">
            <ItemTemplate>
                <nobr>
                        <asp:LinkButton  CssClass="itemplayer" style="text-decoration:none"  runat="server" id="lbnBettingLock" Text='<%#IIf(SBCBL.std.SafeString(Container.DataItem("IsBettingLocked")).Equals("Y"),"Y","N")   %>'  CommandName="Betting Lock" CommandArgument='<%#  sbcbl.std.safestring(Container.DataItem("PlayerID"))  & "|" & Container.DataItem("Login")  %>' ></asp:LinkButton>

                        </nobr>
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateColumn>
        <asp:BoundColumn DataField="PassWord" HeaderText="PassWord" ItemStyle-HorizontalAlign="Center">
            <ItemStyle HorizontalAlign="Center" />
        </asp:BoundColumn>
        <asp:TemplateColumn ItemStyle-HorizontalAlign="Center" ItemStyle-Width="100">
            <HeaderTemplate>
                Account Status
            </HeaderTemplate>
            <ItemTemplate>
                <%#IIf(SBCBL.std.SafeString(Container.DataItem("IsLocked")).Equals("Y"), "Locked", "Active")%>
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" Width="100px" />
        </asp:TemplateColumn>
        <asp:TemplateColumn ItemStyle-HorizontalAlign="Center" ItemStyle-Width="100">
            <HeaderTemplate>
                Last Log in
            </HeaderTemplate>
            <ItemTemplate>
                <%#formatDate(Eval("LastLoginDate"))%>
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" Width="100px" />
        </asp:TemplateColumn>
        <asp:TemplateColumn ItemStyle-HorizontalAlign="Center">
            <HeaderTemplate>
                Casino
            </HeaderTemplate>
            <ItemTemplate>
                <%#IIf(SBCBL.std.SafeString(Container.DataItem("HasCasino")).Equals("Y"), "Yes", "No")%>
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateColumn>
        <asp:TemplateColumn ItemStyle-HorizontalAlign="Center" ItemStyle-Width="100">
            <HeaderTemplate>
                Limit
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="lblLimit" runat="server" Text=""></asp:Label>
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" Width="100px" />
        </asp:TemplateColumn>
        <asp:TemplateColumn ItemStyle-HorizontalAlign="Center" ItemStyle-Width="100">
            <HeaderTemplate>
                Balance
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="lblBalance" runat="server" Text=""></asp:Label>
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" Width="100px" />
        </asp:TemplateColumn>
        <asp:TemplateColumn ItemStyle-HorizontalAlign="Center" ItemStyle-Width="100">
            <HeaderTemplate>
                Pending
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="lblPending" runat="server" Text=""></asp:Label>
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" Width="100px" />
        </asp:TemplateColumn>
    </Columns>
</asp:DataGrid>
</asp:Panel>

 

 <uc1:PlayerSubAgentEdit ID="ucPlayerSubAgentEdit" Visible ="false"  runat="server" />