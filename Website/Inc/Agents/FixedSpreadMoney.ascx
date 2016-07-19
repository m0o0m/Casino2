<%@ Control Language="VB" AutoEventWireup="false" CodeFile="FixedSpreadMoney.ascx.vb"
    Inherits="SBSAgents.Inc_Agent_FixedSpreadMoney" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wbl" %>
<div class="panel panel-grey">
    <div class="panel-heading">Flat Juice Setup</div>
    <div class="panel-body">

        <div runat="server" id="trAgents" class="form-group">
            <label class="control-label col-md-6">Agents</label>
            <div class="col-md-6">
                <wbl:CDropDownList runat="server" ID="ddlAgents" AutoPostBack="true" CssClass="form-control" />
            </div>
        </div>
        <div class="form-group">
            <label class="control-label col-md-6">Fixed Juice for Half Time</label>
            <div class="col-md-6">
                <asp:TextBox ID="txtHalfSpreadMoney" Width="70" runat="server"
                    onkeypress="javascript:return inputNumber(this,event, true);" CssClass="form-control"></asp:TextBox>
            </div>
        </div>
        <div class="form-group">
            <label class="control-label col-md-6">Fixed Juice for Team Total</label>
            <div class="col-md-6">
                <asp:TextBox ID="txtSpreadMoneyGT" runat="server" CssClass="form-control"
                    Width="70px" onkeypress="javascript:return inputNumber(this,event, true);" Style="text-align: right"></asp:TextBox>
            </div>
        </div>
        <div class="form-group">
            <label class="control-label col-md-6">Flat Juice for Full Game </label>
            <div class="col-md-6">
                <asp:TextBox ID="txtFlatSpreadMoney" Width="70" CssClass="form-control" runat="server" MaxLength="7"
                    onkeypress="javascript:return inputNumber(this,event, true);" Style="text-align: right"></asp:TextBox>
            </div>
        </div>
        <div class="form-group">
            <label class="control-label col-md-6"></label>
            <div class="col-md-6">
                <asp:RadioButtonList ID="rdbtnSpreadMoney" runat="server" CssClass="rdo-group" RepeatDirection="Horizontal">
                    <asp:ListItem Value="Yes">Flat Juice</asp:ListItem>
                    <asp:ListItem Value="No">Default</asp:ListItem>
                </asp:RadioButtonList>
            </div>
        </div>
        <div class="form-group">
            <label class="control-label col-md-6">Apply for Football and Basketball only</label>
        </div>
        <div class="form-group">
            <label class="control-label col-md-6"></label>
            <div class="col-md-6">
                <asp:Button ID="btnSave" runat="server" CssClass="btn btn-primary"
                    Text="Save" />
            </div>
        </div>
        <div>
            <asp:HiddenField runat="server" ID="hfIsSuperAdmin" />
        </div>
    </div>
</div>

<script type="text/javascript">

    function checkInfo() {
        var ohfIsSuperAdmin = $get('<%=hfIsSuperAdmin.ClientId %>');
        if (ohfIsSuperAdmin.value == 'True') {
            var oddlAgents = $get('<%=ddlAgents.ClientId %>');
            if (oddlAgents.value == '') {
                alert('Please select agent');
                return false;
            }
        }

        var otxtFlatSpreadMoney = $get('<%=txtFlatSpreadMoney.ClientId %>');
        if (otxtFlatSpreadMoney) {
            if (otxtFlatSpreadMoney.value == '') {
                alert('Flat spread money is required');
                return false;
            }
        }

        return true;
    }

</script>
