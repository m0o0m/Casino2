<%@ Page Language="VB" MasterPageFile="Agents.master" AutoEventWireup="false" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" CodeFile="BetAction.aspx.vb" Inherits="SBSAgents.BetAction" %>

<%@ Register Src="~/SBS/Inc/Game/BetActions.ascx" TagName="BetActions" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="Server">
   <div style="padding-bottom:10px;width:100%">
    <asp:Label ID="lblPlayer" runat="server" style="margin-left:30px;color:red" Text=""></asp:Label>
    <uc:BetActions ID="ucBetActions" BackLink="SelectGame.aspx" runat="server" />
    </div>
</asp:Content>
