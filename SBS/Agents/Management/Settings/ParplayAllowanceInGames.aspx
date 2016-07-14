<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ParplayAllowanceInGames.aspx.vb" 
    MasterPageFile="~/SBS/Agents/Agents.master"
    Inherits="SBSAgents.SBS_Agents_Management_Settings_ParplayAllowanceInGames" %>

<%@ Register Src="~/Inc/Agents/ParlayAndReverseRules.ascx" TagName="ParlayAndReverseRules" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" runat="Server">
    <div class="panel panel-grey">
        <div class="panel-heading">Parlay Setup In Game</div>
        <div class="panel-body">
            <uc1:ParlayAndReverseRules ID="ucParlayAndReverseRules" BetweenGames="false" runat="server" />
        </div>
    </div>
</asp:Content>
