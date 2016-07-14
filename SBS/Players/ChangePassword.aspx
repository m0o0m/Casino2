<%@ Page Language="VB" MasterPageFile="~/SBS/Players/Player.master" AutoEventWireup="false" CodeFile="ChangePassword.aspx.vb" Inherits="SBSPlayer.ChangePassword" %>
<%@ Register Src="~/Inc/changePassword.ascx" TagName="changePassword" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" Runat="Server">
 <div style="padding-top:20px;float:left">
  <uc1:changePassword ID="ucChangePassword" runat="server" />
 </div>
      
</asp:Content>

