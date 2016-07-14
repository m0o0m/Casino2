<%@ Page Language="VB" MasterPageFile="Player.master" MaintainScrollPositionOnPostback="true" AutoEventWireup="false" EnableEventValidation="false" CodeFile="BetAction.aspx.vb" Inherits="SBSAgents.BetAction"  %>

<%@ Register Src="~/SBS/Inc/Game/BetActions.ascx" TagName="BetActions" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="Server">
    <uc:BetActions ID="ucBetActions" runat="server" BackLink="Default.aspx" />
</asp:Content>
