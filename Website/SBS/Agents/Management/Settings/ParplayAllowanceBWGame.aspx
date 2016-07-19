<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ParplayAllowanceBWGame.aspx.vb" 
    MasterPageFile="~/SBS/Agents/Agents.master"
    Inherits="SBSAgents.SBS_Agents_Management_Settings_ParplayAllowanceBWGame" %>


<%@ Register Src="~/Inc/Agents/ParlayAndReverseRules.ascx" TagName="ParlayAndReverseRules" TagPrefix="uc1" %>


<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" runat="Server">
    
    <div class="panel panel-grey">
        <div class="panel-heading">Parlay Setup Between Game</div>
        <div class="panel-body">
            <uc1:ParlayAndReverseRules ID="ParlayAndReverseRules1" runat="server" BetweenGames="true" />
        </div>
    </div>

</asp:Content>

