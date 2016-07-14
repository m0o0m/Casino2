<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ParplaySetup.aspx.vb" 
    MasterPageFile="~/SBS/Agents/Agents.master"
    Inherits="SBSAgents.SBS_Agents_Management_Settings_ParplaySetup" %>

<%@ Register Src="~/Inc/Agents/ParlaySetup.ascx" TagName="ParlaySetup" TagPrefix="uc1" %>


<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" runat="Server">
    
    <div class="panel panel-grey">
        <div class="panel-heading">Parlay Setup</div>
        <div class="panel-body">
            <uc1:ParlaySetup ID="ucParlaySetup" runat="server" />
        </div>
    </div>

</asp:Content>