<%@ Page Language="VB" MasterPageFile="~/SBS/Agents/Agents.master" AutoEventWireup="false" CodeFile="AgentAccount.aspx.vb" Inherits="SBSAgents.AgentAccount" %>
<%@ Register Src="~/Inc/Agents/accountStatus.ascx" TagName="accountStatus" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" Runat="Server">
<div id="historypanel" class="roundcorner" style="padding: 5px 5px 5px 5px; width: 98%;">
 <uc1:accountStatus ID="ucAccountStatus" runat="server" />
 </div>
</asp:Content>

