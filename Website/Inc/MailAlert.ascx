<%@ Control Language="VB" AutoEventWireup="false" CodeFile="MailAlert.ascx.vb" Inherits="SBCPlayer.Inc_MailAlert" %>

<input type ="hidden" id="hfHaveMail" value="Y" runat="server" />
<asp:Panel ID="pnAlertMail" runat="server" style="position:relative;height:20px;width:22px;float:left">
            
            
             <a style="position:absolute;" href="/SBS/Agents/AgentMail.aspx" id="linkMailBox" runat="server" > <img style="border:none"  alt="Email" width="22" height="14" src="/images/MailIcon.png" id="imgMail" /></a>
            
            </asp:Panel>
      <script language="javascript">
                var bShowEmail = true;
              function FlashEmail() {
                    
                    var imgMail = document.getElementById("imgMail");
                    var hfHaveMail = document.getElementById("<%=hfHaveMail.ClientID %>").value;
                    var bAlert = (hfHaveMail == "Y") ? true : false;
                    if (!bAlert) {
                        imgMail.style.display = "none";
                        imgMail=false;//   return;
                    }
                    if (imgMail) {
                        setInterval(function() {

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