<%@ Page Language="VB" MasterPageFile="~/SBS/CallCenter/CCAgents.master" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="SBSWebsite.SBS_CallCenter_Default" %>
<%@ Register Src="Inc/TopMenu.ascx" TagName="TopMenu" TagPrefix="uc1" %>
<%@ Register Src="~/Inc/contentFileDB.ascx" TagName="contentFileDB" TagPrefix="uc1" %>
<%@ Register Src="~/Inc/CallCenter/playerProfile.ascx" TagName="playerProfile" TagPrefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" Runat="Server">
   <uc1:contentFileDB ID="ucContentFileDB" runat="server" />
   <uc1:playerProfile ID="ucPlayerProfile" runat="server" />
      
</asp:Content>

