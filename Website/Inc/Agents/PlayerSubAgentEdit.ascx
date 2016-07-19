<%@ Control Language="VB" AutoEventWireup="false" CodeFile="PlayerSubAgentEdit.ascx.vb" Inherits="SBSAgents.PlayerSubAgentEdit" %>

<fieldset>
    <legend style="margin-left: 20px"><h3>Update Player Info</h3> </legend>
   
<table>
    <tr>
        <td>
            Name
        </td>
        <td>
            <asp:TextBox ID="txtName" runat="server"></asp:TextBox>
        </td>
    </tr>
<tr>
        <td>
            Login
        </td>
        <td>
         <asp:TextBox ID="txtLogin" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            PassWord
        </td>
        <td>
         <asp:TextBox ID="txtPass" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
        
        </td>
        <td>
            <asp:Button ID="btnSave" runat="server" Text="Save" class="button" />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" class="button" />
        </td>
    </tr>
</table>
<asp:HiddenField ID="hfPlayerID" runat="server" />
 </fieldset> 