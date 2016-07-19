<%@ Page Language="VB" AutoEventWireup="false" CodeFile="BookmakerSetup.aspx.vb" MasterPageFile="~/SBS/Agents/Agents.master"
     Inherits="SBSAgents.SBS_Agents_Management_Settings_BookmakerSetup" %>

<%@ Register Src="~/Inc/SportAllowance.ascx" TagName="SportAllowance" TagPrefix="uc1" %>

<asp:content id="Content2" contentplaceholderid="cphBody" runat="Server">
    <div class="panel panel-grey">
        <div class="panel-heading">Bookmaker Setup</div>
        <div class="panel-body">
            <uc1:SportAllowance ID="ucSportAllowance" runat="server"></uc1:SportAllowance>
        </div>
    </div>

</asp:content>
