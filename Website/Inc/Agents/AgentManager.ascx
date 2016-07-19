<%@ Control Language="VB" AutoEventWireup="false" CodeFile="AgentManager.ascx.vb"
    Inherits="SBSAgents.Inc_Agents_AgentManager" %>
<%@ Register Src="~/Inc/Agents/AgentEdit.ascx" TagName="AgentEdit" TagPrefix="uc" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>

<script type="text/javascript" language="javascript">
     function CheckAll(checkAll) {
            var oGrid = document.getElementById('<%=dgAgents.ClientID%>');
            var oInputs = oGrid.getElementsByTagName("INPUT");

            for (var index = 0; index < oInputs.length; index++) {
                if (oInputs[index].type.toUpperCase() == "CHECKBOX") {
                    oInputs[index].checked = checkAll.checked;
                }
            }

            if (checkAll.checked) {
                checkAll.title = "Unselect all"
            }
            else {
                checkAll.title = "Select all"
            }
        }
        function resetAllField() {
            var obj = document.getElementById('<%=txtNameOrLogin.ClientID%>');
            if (obj) {
                obj.value = '';
                obj.focus();
            }
        }
</script>

<table cellpadding="2" cellspacing="2" width="100%">
    <tr>
        <td colspan="2">
            <asp:Button ID="btnLock" runat="server" Text="Lock" ToolTip="Lock Agents" CssClass="textInput" />
            <asp:Button ID="btnBettingLock" runat="server" Text="Betting Lock" ToolTip="Betting lock Agents"
                CssClass="textInput" />
            <asp:Button ID="btnViewLock" runat="server" Text="View Lock" ToolTip="View lock Agents"
                CssClass="textInput" />
            <asp:Button ID="btnViewBettingLock" runat="server" Text="View Betting Lock" ToolTip="View lock Agents"
                CssClass="textInput" />
        </td>
    </tr>
    <tr>
        <td colspan="2">
            &nbsp;Name or Login
            <asp:TextBox ID="txtNameOrLogin" CssClass="textInput" MaxLength="50" runat="server" />
            &nbsp;<asp:Button ID="btnFilter" runat="server" Text="Filter" ToolTip="Filter" Width="60"
                CssClass="textInput" />
            <input type="button" value="Reset" class="textInput" onclick="resetAllField()" />
        </td>
    </tr>
    <tr>
        <td valign="top">
            <asp:DataGrid ID="dgAgents" runat="server" Width="100%" AutoGenerateColumns="false"
                CellPadding="2" CellSpacing="2" CssClass="gamebox">
                <HeaderStyle CssClass="tableheading" HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Left" />
                <AlternatingItemStyle HorizontalAlign="Left" />
                <SelectedItemStyle BackColor="YellowGreen" />
                <Columns>
                    <asp:TemplateColumn ItemStyle-HorizontalAlign="Center" ItemStyle-Width="20">
                        <HeaderTemplate>
                            <input id="chkChoice" type="checkbox" onclick="CheckAll(this);" title="Select all"
                                runat="server" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox type="checkbox" ID="chkID" runat="server" value='<%# SBCBL.std.SafeString(Container.DataItem("AgentID")) & "|" & SBCBL.std.SafeString(Container.DataItem("Login")) %>'
                                name='Chosen_<%#Container.DataItem("AgentID") %>' />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Name (Login)" ItemStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                            <asp:LinkButton ID="lbtEdit" runat="server" CommandArgument='<%# SBCBL.std.SafeString(Container.DataItem("AgentID")) & "|" & SBCBL.std.SafeString(Container.DataItem("Login")) %>'
                                CommandName="EditUser" Text='<%#Container.DataItem("AgentName") %>' Font-Underline="false" />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:BoundColumn DataField="IsBettingLocked" HeaderText="Betting Locked" />
                    <asp:BoundColumn DataField="IsLocked" HeaderText="Locked" />
                    <asp:TemplateColumn HeaderText="Last Login" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <%#IIf(SBCBL.std.SafeString(Container.DataItem("LastLoginDate")) <> "", UserSession.ConvertToEST(SBCBL.std.SafeString(Container.DataItem("LastLoginDate"))), "").ToString%>&nbsp;
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
            </asp:DataGrid>
        </td>
        <td id="tdAgentEdit" runat="server" valign="top">
            <uc:AgentEdit ID="ucAgentEdit" runat="server" />
        </td>
    </tr>
</table>
