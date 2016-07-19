<%@ Language="VB" MasterPageFile="~/SBS/SBS.master" AutoEventWireup="false" CodeFile="Odds.aspx.vb" Inherits="SBSWebsite.SBS_Players_Odds" %>

<%@ Register Src="~/Inc/contentFileDB.ascx" TagName="contentFileDB" TagPrefix="uc1" %>
<%@ Register Src="~/Inc/Login.ascx" TagName="Login" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTopMenu" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" Runat="Server">
  <div id="historypanel" class="roundcorner">
        <div style="width: 98%; margin: auto; padding: 7px 5px;">
            <div id="playerID" style="float: left; margin-right: 20px; color: #fff; font-weight:bold; font-size:15px;">
                Odds & Payout</div>
            <br style="clear: both;" />
        </div>
        <table width="98%" border="0" align="center" cellpadding="3" cellspacing="1" class="gamebox">
            <tr>
                <td>
                    <uc1:contentFileDB ID="ucContentFileDB" runat="server" />
                </td>
            </tr>
        </table>
        <div class="clear">
        </div>
    </div>
</asp:Content>

