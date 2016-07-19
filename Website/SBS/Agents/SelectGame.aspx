<%@ Page Language="VB"  EnableEventValidation ="false"  MasterPageFile="~/SBS/Agents/Agents.master" AutoEventWireup="false" CodeFile="SelectGame.aspx.vb" Inherits="SBSAgents.SBC_Agents_SelectGame" %>


<%@ Register Src="~/SBS/Inc/Game/SelectGame.ascx" TagName="SelectGame" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="Server">
   <div style="padding-bottom:10px;width:100%">
    <uc:SelectGame ID="ucSelectGame" runat="server" BetActionLink="BetAction.aspx" />
    </div>
</asp:Content>

