<%@ Page Language="VB" AutoEventWireup="false" CodeFile="RiskControl.aspx.vb"
    Inherits="SBSAgents.SBS_Agents_Management_Settings_RiskControl"
    MasterPageFile="~/SBS/Agents/Agents.master" %>

<%@ Register Src="~/Inc/Agents/FixedSpreadMoney.ascx" TagName="FixedSpreadMoney" TagPrefix="uc1" %>
<%@ Register Src="~/Inc/GameCircledSettings.ascx" TagName="GameCircledSettings" TagPrefix="uc1" %>
<%@ Register Src="~/Inc/SuperAdmins/JuiceControl.ascx" TagName="JuiceControl" TagPrefix="uc2" %>
<%@ Register Src="~/Inc/Agents/MaxPerGame24h.ascx" TagName="MaxPerGame24h" TagPrefix="uc3" %>
<%@ Register Src="../../../Inc/PlayerJuiceControl.ascx" TagName="PlayerJuiceControl" TagPrefix="uc4" %>


<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" runat="Server">
    <div class="col-md-6">
        <uc1:FixedSpreadMoney Visible="true" ID="ucFixedSpreadMoney" runat="server"></uc1:FixedSpreadMoney>
        <uc3:MaxPerGame24h Visible="true" ID="ucMaxPerGame24h" runat="server"></uc3:MaxPerGame24h>
    </div>
    <div class="col-md-6">
        <uc1:GameCircledSettings ID="ucGameCircledSettings" runat="server"></uc1:GameCircledSettings>
        <br />
        <uc2:JuiceControl ID="ucJuiceControl" runat="server"></uc2:JuiceControl>
        <br />
        <uc4:PlayerJuiceControl ID="PlayerJuiceControl1" runat="server" />
    </div>
</asp:Content>
