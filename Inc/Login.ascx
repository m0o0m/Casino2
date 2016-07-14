<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Login.ascx.vb" Inherits="SBCWebsite.Inc_Login" %>
   <table border="0" cellspacing="0" cellpadding="0">  
    <tr valign="top">
        <td>
            Username
        </td>
        <td valign="top">
         <asp:TextBox ID="txtLogin"  Font-Size="11px"  runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtLogin"
                EnableClientScript="False" ErrorMessage="*"></asp:RequiredFieldValidator>
        </td>
        </tr>
        <tr>
        <td valign="top">
            Password
        </td>
        <td valign="top">
          <asp:TextBox ID="txtPassword"  runat="server" Font-Size="11px"  TextMode="Password"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtPassword"
                EnableClientScript="False" ErrorMessage="*"></asp:RequiredFieldValidator>
        </td>
        </tr>
        <tr>
        <td>
        </td>
        <td>
            <asp:Button ID="btnLogin"  runat="server" Text="LOGIN" />
        </td>
    </tr>
   </table>
