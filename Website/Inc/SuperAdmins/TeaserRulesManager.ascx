<%@ Control Language="VB" AutoEventWireup="false" CodeFile="TeaserRulesManager.ascx.vb"
    Inherits="SBCSuperAdmin.TeaserRulesManager" %>
<%@ Register Src="~/Inc/SuperAdmins/TeaserRuleEdit.ascx" TagName="TeaserRuleEdit"
    TagPrefix="uc" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>

<script type="text/javascript" language="javascript">
    function CheckAllTeaserRuleEdit(checkAll) {
        var oGrid = document.getElementById('<%=dgTeaserRules.ClientID%>');
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
</script>

<div class="row">
    <div class="col-lg-12">
        <asp:Button ID="btnLock" runat="server" Text="Lock" ToolTip="Lock TeaserRules" CssClass="btn btn-default" />
        <asp:Button ID="btnViewLock" runat="server" Text="View Lock" ToolTip="View Lock TeaserRules"
            CssClass="btn btn-default" />
    </div>
    <div class="clearfix"></div>
</div>
<div class="mbxl"></div>

<div class="row">
    <div class="col-lg-8">
        <asp:DataGrid ID="dgTeaserRules" runat="server" AutoGenerateColumns="false" CssClass="table table-hover table-bordered">
            <HeaderStyle CssClass="tableheading" HorizontalAlign="Center" />
            <ItemStyle HorizontalAlign="Left" />
            <AlternatingItemStyle HorizontalAlign="Left" />
            <SelectedItemStyle BackColor="YellowGreen" />
            <Columns>
                <asp:TemplateColumn ItemStyle-HorizontalAlign="Center" ItemStyle-Width="20">
                    <HeaderTemplate>
                        <input id="chkChoice" type="checkbox" onclick="CheckAllTeaserRuleEdit(this);" title="Select all"
                            runat="server" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:CheckBox type="checkbox" ID="chkID" runat="server" value='<%# SBCBL.std.SafeString(Container.DataItem("TeaserRuleID")) %>'
                            name='Chosen_<%#Container.DataItem("TeaserRuleID") %>' />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Index" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:TextBox ID="txtIndex" runat="server" Text='<%# Container.DataItem("TeaserRuleIndex") %>'
                            CssClass="textInput" Width="35" Style="text-align: center;" />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Name" ItemStyle-HorizontalAlign="Left">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtEdit" runat="server" CommandArgument='<%# SBCBL.std.SafeString(Container.DataItem("TeaserRuleID")) %>'
                            CommandName="EditTeaserRule" Text='<%#Container.DataItem("TeaserRuleName") %>' />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:BoundColumn DataField="MinTeam" HeaderText="From Teams" ItemStyle-Width="50"
                    DataFormatString="{0:#,#}" ItemStyle-HorizontalAlign="Center" />
                <asp:BoundColumn DataField="MaxTeam" HeaderText="To Teams" ItemStyle-Width="50" DataFormatString="{0:#,#}"
                    ItemStyle-HorizontalAlign="Center" />
                <asp:BoundColumn DataField="BasketballPoint" HeaderText="Basketball Point" DataFormatString="{0:#,#.0}"
                    ItemStyle-Width="50" ItemStyle-HorizontalAlign="Center" />
                <asp:BoundColumn DataField="FootballPoint" HeaderText="Football Point" DataFormatString="{0:#,#.0}"
                    ItemStyle-Width="50" ItemStyle-HorizontalAlign="Center" />
                <asp:TemplateColumn ItemStyle-Width="50" ItemStyle-HorizontalAlign="Center" HeaderText="Tie">
                    <ItemTemplate>
                        <%#IIf(Container.DataItem("IsTiesLose") = "Y", "Lose", "Push")%>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtDel" runat="server" CommandArgument='<%# SBCBL.std.SafeString(Container.DataItem("TeaserRuleID")) %>'
                            CommandName="DeleteTeaserRule" Text="Delete" />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
        </asp:DataGrid>
        <div class="mbxl"></div>
        <div class="row">
            <div class="col-lg-12">
                <asp:Button ID="btnChangedIndex" runat="server" Text="Save Changed Index" CssClass="btn btn-primary" />
            </div>
        </div>

    </div>
    <div class="col-lg-4" id="tdAgentEdit" runat="server">
        <uc:TeaserRuleEdit ID="ucTeaserRuleEdit" runat="server" />
    </div>
    <div class="clearfix"></div>
</div>
<div class="mbxl"></div>
