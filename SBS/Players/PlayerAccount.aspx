<%@ Page Language="VB" MasterPageFile="~/SBS/Players/Player.master" AutoEventWireup="false" CodeFile="PlayerAccount.aspx.vb" Inherits="SBSPlayer.PlayerAccount" %>
<%@ Register Src="~/Inc/Players/accountStatus.ascx" TagName="accountStatus" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
 </asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" Runat="Server">
<uc1:accountStatus ID="ucAccountStatus" runat="server" />
</asp:Content>

