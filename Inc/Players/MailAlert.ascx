<%@ Control Language="VB" AutoEventWireup="false" CodeFile="MailAlert.ascx.vb" Inherits="SBCPlayer.MailAlert" %>
<script language="javascript">
    var bShowEmail = true;
    window.onload = function () {
        var imgMail = document.getElementById("imgMail")
        if (imgMail) {
            setInterval(function () {

                if (bShowEmail) {
                    bShowEmail = false;
                }
                else
                    bShowEmail = true;
                imgMail.style.display = bShowEmail ? "" : "None"
            }

        , 500);
        }
    }
</script>
<asp:Panel ID="pnAlertMail" runat="server">
    <table class="tbl-menu">
        <tr>
            <td width="20">
                <img alt="Email" src="/images/MailAlert.png" id="imgMail" />
            </td>
            <td>
                <span id="MailAlert">
                    <asp:LinkButton ID="lbtMailAlert" Font-Underline="false" runat="server"><b>You Have New Mail</b></asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Panel>
