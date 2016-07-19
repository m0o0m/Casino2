<%@ Control Language="VB" AutoEventWireup="false" CodeFile="SiteOption.ascx.vb" Inherits="SBSAgents.SiteOption" %>
<fieldset style="padding: 10px">
    <legend style="margin-left: 20px">Your Site Option</legend>
    <table>
        <tr>
            <td>
                Color Scheme
            </td>
            <td>
                <asp:DropDownList CssClass="textInput" ID="ddlColor" runat="server">
                    <asp:ListItem Text="Red" Value="Red"></asp:ListItem>
                    <asp:ListItem Text="Green" Value="Green"></asp:ListItem>
                    <asp:ListItem Text="Blue" Value="Blue"></asp:ListItem>
                    <asp:ListItem Text="Sky" Value="Sky"></asp:ListItem>
                    <asp:ListItem Text="Black" Value="Black"></asp:ListItem>
                    <asp:ListItem Text="Brown" Value="Brown"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                Login Template
            </td>
            <td>
                <asp:DropDownList CssClass="textInput" ID="ddlLoginTemplate" runat="server">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                Phone 
            </td>
            <td>
                <asp:TextBox ID="txtSuperAgentPhone" runat="server" CssClass="textInput"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
             <asp:Button ID="bnSave" runat="server" CssClass="button" Text="Save" Width="60px" />
            </td>
        </tr> 
    </table>
   
</fieldset>
