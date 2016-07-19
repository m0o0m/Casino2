<%@ Control Language="VB" AutoEventWireup="false" CodeFile="QuarterDisplaySetup.ascx.vb"
    Inherits="SBSAgents.Inc_Agents_QuarterDisplaySetup" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>

<div class="panel panel-grey">
    <div class="panel-heading">Quarter Display Setup</div>
    <div class="panel-body">
        <div class="form-group" runat="server" id="trAgents">
            <label class="control-label col-md-6">Agents</label>
            <div class="col-md-6">
                <asp:DropDownList runat="server" ID="ddlAgents" AutoPostBack="true" CssClass="form-control" />
            </div>
        </div>
        <div class="form-group">
            <label class="control-label col-md-6">Sport Type</label>
            <div class="col-md-6">
                <wlb:CDropDownList ID="ddlSportType" runat="server" OptionalText="" OptionalValue=""
                    hasOptionalItem="false" CssClass="form-control" AutoPostBack="true" SortItems="true">
                </wlb:CDropDownList>
            </div>
        </div>
        <div class="form-group">
            <label class="control-label col-md-6">Game Time Off Line (Minutes)</label>
            <div class="col-md-6">
                <asp:TextBox ID="txtOffMinutes" MaxLength="5" Width="110" CssClass="form-control" runat="server"></asp:TextBox>
            </div>
        </div>
        <div class="form-group">
            <label class="control-label col-md-6">Game Time Display (Hours)</label>
            <div class="col-md-6">
                <asp:TextBox ID="txtDisplayHours" MaxLength="5" Width="110" CssClass="form-control" runat="server"></asp:TextBox>
            </div>
        </div>
        <div class="form-group">
            <label class="control-label col-md-6"></label>
            <div class="col-md-6">
                <asp:Button ID="btnSave" runat="server" CssClass="btn btn-primary" Text="Save" OnClientClick="return check1HInfo();" />
            </div>
        </div>


        <div>
            <asp:HiddenField runat="server" ID="hfIsSuperAdmin" />
        </div>
    </div>
</div>

<script type="text/javascript">

    function checkAgentInfo() {
        var ohfIsSuperAdmin = $get('<%=hfIsSuperAdmin.ClientId %>');

        if (ohfIsSuperAdmin.value == 'True') {
            var oddlAgents = $get('<%=ddlAgents.ClientId %>');
            if (oddlAgents.value == '') {
                alert('Please select Agent');
                return false;
            }
        }

        return true;
    }

    function check1HInfo() {
        if (checkAgentInfo()) {
            var oddlSportType = $get('<%=ddlSportType.ClientId %>');
            if (oddlSportType.value == '') {
                alert('Please choose Sport type');
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

