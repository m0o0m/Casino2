<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ManagerAgent.ascx.vb" Inherits="SBSAgents.ManagerAgent" %>
<%@ Register Src="~/Inc/Agents/AgentEdit.ascx" TagName="AgentEdit" TagPrefix="uc" %>

<script language="javascript">
    function resetAllField() {
        var obj = document.getElementById('<%=txtNameOrLogin.ClientID%>');
        if (obj) {
            obj.value = '';
            obj.focus();
        }
    }
</script>


<asp:Panel ID="pnAgentManager" runat="server">
    <div class="panel panel-grey">
        <div class="panel-heading">Manage Sub Agent</div>
        <div class="panel-body">
            <div class="form-group">
                <div class="col-md-1 w10">
                    <asp:RadioButton ID="rdAllAcct" runat="server" GroupName="Acct" AutoPostBack="true" />
                </div>
                <div class="col-md-2">
                    <label class="control-label pt2">All Acct</label>
                </div>
                <div class="col-md-1 w10">
                    <asp:RadioButton ID="rdLockAcct" runat="server" GroupName="Acct" AutoPostBack="true" />
                </div>
                <div class="col-md-2">
                    <label class="control-label pt1">Locked Acct</label>
                </div>
                <div class="col-md-1 w10">
                    <asp:RadioButton ID="rdLockBetAcct" runat="server" GroupName="Acct" AutoPostBack="true" />
                </div>
                <div class="col-md-2">
                    <label class="control-label pt1">Betting Locked Acct</label>
                </div>
                <div class="col-md-1">
                    <asp:Button ID="btnAddNew" runat="server" Text="ADD NEW SUB AGENT" CssClass="btn btn-success" />
                </div>
            </div>
            <div class="form-group">
                <div class="col-md-2" style="width: 120px">
                    <label class="control-label">Name or Login</label>
                </div>
                <div class="col-md-2">
                    <asp:TextBox ID="txtNameOrLogin" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-md-1">
                    <asp:Button ID="btnFilter" runat="server" Text="Filter" CssClass="btn btn-primary" />
                </div>
                <div class="col-md-2">
                    <input type="button" value="Reset" class="btn btn-defult" onclick="resetAllField()" />
                </div>
            </div>
            <asp:DataGrid ID="dgAgents" runat="server" Width="100%" AutoGenerateColumns="false" Font-Size="12px"
                CssClass="table table-hover table-bordered" AllowPaging="True"
                PageSize="30">
                <HeaderStyle CssClass="tableheading" HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Left" />
                <PagerStyle Font-Names="tahoma" Font-Size="10pt" HorizontalAlign="Right"
                    Mode="NumericPages" />
                <AlternatingItemStyle HorizontalAlign="Left" />
                <SelectedItemStyle BackColor="YellowGreen" />
                <Columns>
                    <asp:TemplateColumn ItemStyle-HorizontalAlign="left" HeaderText="Login (Name)">
                        <ItemTemplate>
                            <nobr>
                        <asp:LinkButton ID="lbtEdit" style="color:Blue" runat="server" CommandArgument='<%# SBCBL.std.SafeString(Container.DataItem("AgentID")) & "|" & SBCBL.std.SafeString(Container.DataItem("Login")) %>'
                                CommandName="EditUser" Text='<%#Container.DataItem("AgentName") %>' Font-Underline="false"  />
                        </nobr>
                            <asp:HiddenField ID="hfLock" runat="server" Value='<%# SBCBL.std.SafeString(Container.DataItem("IsLocked")) %>'></asp:HiddenField>
                            <asp:HiddenField ID="hfBettingLock" runat="server" Value='<%# SBCBL.std.SafeString(Container.DataItem("IsBettingLocked")) %>'></asp:HiddenField>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Left" />
                    </asp:TemplateColumn>
                    <asp:TemplateColumn ItemStyle-HorizontalAlign="Center" HeaderText="Block">
                        <ItemTemplate>
                            <asp:LinkButton ID="lbnLock" runat="server" Style="text-decoration: none" Text='<%#IIf(SBCBL.std.SafeString(Container.DataItem("IsLocked")).Equals("Y"),"Y","N")   %>' CommandName="Lock" CommandArgument='<%# SBCBL.std.SafeString(Container.DataItem("AgentID")) & "|" & SBCBL.std.SafeString(Container.DataItem("Login")) %>' />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateColumn>
                    <asp:TemplateColumn ItemStyle-HorizontalAlign="Center" HeaderText="Betting Block">
                        <ItemTemplate>
                            <asp:LinkButton ID="lbnBettingLock" runat="server" Style="text-decoration: none" Text='<%#IIf(SBCBL.std.SafeString(Container.DataItem("IsBettingLocked")).Equals("Y"),"Y","N")   %>' CommandName="Betting Lock" CommandArgument='<%# SBCBL.std.SafeString(Container.DataItem("AgentID")) & "|" & SBCBL.std.SafeString(Container.DataItem("Login")) %>' />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateColumn>
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
                            Last Login
                        </HeaderTemplate>
                        <ItemTemplate>
                            <%#SBCBL.std.SafeDate(Container.DataItem("LastLoginDate")).ToString("MM/dd/yyyy")%>
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
                    <asp:BoundColumn DataField="NumOfAgent" HeaderText="# of Agent"
                        ItemStyle-HorizontalAlign="Center">
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundColumn>
                    <asp:BoundColumn DataField="NumOfPlayer" HeaderText="# of Players"
                        ItemStyle-HorizontalAlign="Center">
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundColumn>
                </Columns>
            </asp:DataGrid>
        </div>
    </div>
</asp:Panel>

<uc:AgentEdit ID="ucAgentEdit" runat="server" SiteType="SBS" />
