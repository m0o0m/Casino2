<%@ Page Language="VB" MasterPageFile="~/SBS/Agents/Agents.master" AutoEventWireup="false" CodeFile="ChangePassword.aspx.vb" Inherits="SBSAgents.ChangePassword" %>
<%@ Register Src="~/Inc/changePassword.ascx" TagName="changePassword" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" Runat="Server">
 <div id="historypanel" class="roundcorner" style="padding: 5px 5px 5px 5px; width: 98%;">
 <uc1:changePassword ID="ucChangePassword" runat="server" />
 </div> 
</asp:Content>

