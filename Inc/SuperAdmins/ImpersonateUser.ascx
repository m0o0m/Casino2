<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ImpersonateUser.ascx.vb"
    Inherits="SBCSuperAdmins.ImpersonateUser" %>
<div class="input-icon right text-white" style="top: 7px;">
    <%--<asp:Label ID="lblImpersonate" runat="server" CssClass="control-label" Text="Impersonate:"></asp:Label>--%>
    <asp:TextBox ID="txtImpersonateUser" runat="server" CssClass="form-control text-dark" placeholder="Impersonate" MaxLength="30" Width="150px"></asp:TextBox>
</div>

