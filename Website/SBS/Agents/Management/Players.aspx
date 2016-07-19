<%@ Page Language="VB" MasterPageFile="../Agents.master"  AutoEventWireup="false" CodeFile="Players.aspx.vb" Inherits="SBSAgents.Management.Players" %>
<%@ Register Src="~/Inc/Agents/PlayerManager.ascx" TagName="PlayerManager" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="Server">
    <uc:PlayerManager ID="ucPlayerManager"  runat="server" PendingPage="../OpenBets.aspx" />
</asp:Content>
