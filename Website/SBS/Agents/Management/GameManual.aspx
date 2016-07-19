<%@ Page Language="VB" MasterPageFile="~/SBS/Agents/Agents.master" AutoEventWireup="false" CodeFile="GameManual.aspx.vb" Inherits="SBSAgents.GameManual" %>
<%@ Register Src="~/Inc/Agents/GameManual.ascx" TagName="GameManual" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" Runat="Server">
  <div style="margin-top:20px">
  <uc:GameManual ID="ucGameManual" runat="server" />
  </div>
</asp:Content>

