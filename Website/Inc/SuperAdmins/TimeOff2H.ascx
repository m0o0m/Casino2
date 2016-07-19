<%@ Control Language="VB" AutoEventWireup="false" CodeFile="TimeOff2H.ascx.vb" Inherits="SBCSuperAdmin.TimeOff2H" %>
<table class="table table-hover">
     <tr>
        <td>
           Off Before 2H Time (Minutes)<br/>
           (for Footbal, Basketball, Soccer)
        </td>
        <td>
            <asp:TextBox ID="txt2HOff" CssClass="form-control" Width="110" runat="server" onkeypress="javascript:return inputNumber(this,event, false);"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            
        </td>
        <td>
            <asp:Button ID="btnSaveTime" runat="server" Text="Save" OnClientClick ="return check2HDisplaySetup()" CssClass="btn btn-primary"/>
        </td>
    </tr>
</table>
<script language="javascript">

    function check2HDisplaySetup() {

        var otxt2H = $get('<%=txt2HOff.ClientID %>');
        if (otxt2H.value == '') {
            alert('Please, input 2H time off ');
            return false;
        }
        else {
            return true;
        }
    }
</script>