<%@ Control Language="VB" AutoEventWireup="false" CodeFile="TimeLineOff.ascx.vb"
    Inherits="SBSAgents.Inc_Agents_TimeLineOff" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>

<div class="form-group" runat="server" id="trAgents">
    <label class="control-label col-md-6">Agents</label>
    <div class="col-md-6">
        <wlb:CDropDownList runat="server" ID="ddlAgents" hasOptionalItem="false" AutoPostBack="true" CssClass="form-control" />
    </div>
</div>
<div class="form-group">
    <label class="control-label col-md-6">Game Type</label>
    <div class="col-md-6">
        <wlb:CDropDownList ID="ddlGameType" runat="server" OptionalText="" OptionalValue=""
            hasOptionalItem="false" CssClass="form-control" AutoPostBack="true" SortItems="true">
        </wlb:CDropDownList>
    </div>
</div>
<div class="form-group">
    <label class="control-label col-md-6">Game Time Off Line (Minutes)</label>
    <div class="col-md-6">
        <asp:TextBox ID="txtOffMinutes" MaxLength="5" CssClass="form-control" runat="server"></asp:TextBox>
    </div>
</div>
<div class="form-group">
    <label class="control-label col-md-6">Game Time Display (Hours)</label>
    <div class="col-md-6">
        <asp:TextBox ID="txtDisplayHours" MaxLength="5" CssClass="form-control" runat="server"></asp:TextBox>
    </div>
</div>
<div class="form-group">
    <label class="control-label col-md-6">Time to Display Total Points (Hours)</label>
    <div class="col-md-6">
        <asp:TextBox ID="txtDisplayTotalpoints" MaxLength="5" CssClass="form-control" runat="server"></asp:TextBox>
    </div>
</div>
<div class="form-group">
    <label class="control-label col-md-6"></label>
    <div class="col-md-6">
        <asp:Button ID="btnSave" runat="server" CssClass="btn btn-primary" Text="Save" />
    </div>
</div>

<asp:DataGrid runat="server" ID="dtgTimeOff" AutoGenerateColumns="false" HeaderStyle-CssClass="tableheading"
    Width="100%" HeaderStyle-HorizontalAlign="Center" CssClass="table table-hover table-bordered">
    <Columns>
        <asp:BoundColumn HeaderText="Game Type" DataField="GameType" HeaderStyle-HorizontalAlign="Left"></asp:BoundColumn>
        <asp:BoundColumn HeaderText="Time Off" DataField="OffBefore" ItemStyle-HorizontalAlign="Center"
            HeaderStyle-Width="70"></asp:BoundColumn>
        <asp:BoundColumn HeaderText="Time Display" DataField="DisplayBefore" ItemStyle-HorizontalAlign="Center"
            HeaderStyle-Width="100"></asp:BoundColumn>
        <asp:BoundColumn HeaderText="<nobr>Time Total Points Display</nobr>" DataField="GameTotalPointsDisplay" ItemStyle-HorizontalAlign="Center"
            HeaderStyle-Width="100"></asp:BoundColumn>
        <asp:TemplateColumn HeaderText="Delete Setting">
            <ItemTemplate>
                <asp:LinkButton ID="lbndelete" runat="server" CommandName="deletesettings" CommandArgument='<%#Eval("ID")%>' Text="Delete" ForeColor="Black" Style="text-decoration: none;"></asp:LinkButton>
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateColumn>
    </Columns>
</asp:DataGrid>
<div>
    <asp:HiddenField runat="server" ID="hfIsSuperAdmin" />
</div>

<script type="text/javascript">

    function checkSuperAgentInfo() {
        var ohfIsSuperAdmin = $get('<%=hfIsSuperAdmin.ClientId %>');

        if (ohfIsSuperAdmin.value == 'True') {
            var oddlAgents = $get('<%=ddlAgents.ClientId %>');

            if (oddlAgents.value == '') {
                alert('Please select agent');
                return false;
            }
        }

        return true;
    }

    function checkAgentTimeOnOffInfo() {
        if (checkSuperAgentInfo()) {
            var oddlGameType = $get('<%=ddlGameType.ClientId %>');
            if (oddlGameType.value == '') {
                alert('Please choose game type');
                return false;
            }

            var otxtOffMinutes = $get('<%=txtOffMinutes.ClientId %>');
            if (otxtOffMinutes.value == '') {
                alert('Game time off line not valid');
                return false;
            }

            var otxtDisplayHours = $get('<%=txtDisplayHours.ClientId %>');
            if (otxtDisplayHours.value == '') {
                alert('Game time display not valid');
                return false;
            }

            return true;
        }

        return false;
    }

</script>

