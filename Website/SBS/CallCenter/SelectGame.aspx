<%@ Page Language="VB" MasterPageFile="~/SBS/CallCenter/CCAgents.master" AutoEventWireup="false" CodeFile="SelectGame.aspx.vb" Inherits="SBS_CallCenter_SelectGame" %>
<%@ Register Src="~/SBS/Inc/Game/SelectGame.ascx" TagName="SelectGame" TagPrefix="uc" %>
<%@ Register Src="~/Inc/Callcenter/playerProfile.ascx" TagName="playerProfile" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHeader" Runat="Server">
    <uc2:playerProfile ID="ucPlayerProfile" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphLefMenu" Runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cphBody" Runat="Server">
    <uc:SelectGame ID="ucSelectGame" runat="server" BetActionLink="BetAction.aspx" />
</asp:Content>

