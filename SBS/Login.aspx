<%@ Page Language="VB" MasterPageFile="~/SBS/SBS.master" AutoEventWireup="false" CodeFile="Login.aspx.vb" Inherits="SBCWebsite.Login" Title="Tiger Sportbook" %>

<%@ Register Src="~/Login/LoginControl.ascx" TagName="LoginControl" TagPrefix="uc2" %>

 
<asp:Content ID="cntBody" ContentPlaceHolderID="cphBody" runat="Server">
    <div id="historypanel" class="roundcorner">
        <div style="width: 96%; margin: auto; padding: 7px 5px;">
            <div id="playerID" style="float: left; margin-right: 20px; color: #fff;">
                Login</div>
            <br style="clear: both;" />
        </div>
        <table width="98%" border="0" align="center" cellpadding="3" cellspacing="1" class="gamebox">
            <tr>
                <td>
                    <uc2:LoginControl ID="LoginControl1" runat="server" />
                </td>
            </tr>
        </table>
        <div class="clear">
        </div>
    </div>
</asp:Content>
