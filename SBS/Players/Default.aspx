<%@ Page Language="VB" MasterPageFile="~/SBS/Players/Player.master" AutoEventWireup="false"
    CodeFile="Default.aspx.vb" Inherits="SBS_Players_Default" %>

<%@ Register Src="~/SBS/Inc/Game/SelectGame.ascx" TagName="SelectGame" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="Server">
    <uc:SelectGame ID="ucSelectGame" runat="server" BetActionLink="BetAction.aspx" />
</asp:Content>
