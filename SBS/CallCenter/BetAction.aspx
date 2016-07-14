<%@ Page Language="VB" EnableEventValidation="false" MasterPageFile="~/SBS/CallCenter/CCAgents.master"
    MaintainScrollPositionOnPostback="true" AutoEventWireup="false" CodeFile="BetAction.aspx.vb"
    Inherits="CallCenter_BetAction" %>

<%@ Register Src="~/SBS/Inc/Game/BetActions.ascx" TagName="BetActions" TagPrefix="uc" %>
<%@ Register Src="~/Inc/CallCenter/playerProfile.ascx" TagName="playerProfile" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHeader" runat="Server">
    <uc1:playerProfile ID="ucPlayerProfile" runat="server" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" runat="Server">
    <asp:Panel ID="pnlBetAction" runat="server">
        <uc:BetActions ID="ucBetActions" BackLink="SelectGame.aspx" runat="server" />
    </asp:Panel>
</asp:Content>
