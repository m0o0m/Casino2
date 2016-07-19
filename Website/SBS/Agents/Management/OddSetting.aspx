<%@ Page Language="VB" MasterPageFile="~/SBS/Agents/Agents.master" AutoEventWireup="false" CodeFile="OddSetting.aspx.vb" Inherits="SBSAgents.OddSetting" %>
<%@ Register Src="~/Inc/Agents/OddSetting.ascx" TagName="OddSetting" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" Runat="Server">
<uc1:OddSetting id="ucOddSetting" runat="server" />
</asp:Content>