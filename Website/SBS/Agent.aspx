<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Agent.aspx.vb"
    Inherits="SBSWebsite.SBS_Default" %>

<%@ Register Src="../Login/LoginControl.ascx" TagName="LoginControl" TagPrefix="uc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Vegas Sportsbook</title>
    <link rel="stylesheet" type="text/css" href="/SBS/Inc/Styles/styles.css" />
</head>
<body style="background: url(/images/bg_vegas1.jpg) no-repeat center top black">
    <form runat="server" id="PageForm">
    <%--<asp:placeholder id="phContent" runat="server">
--%>
    <table cellpadding="0" cellspacing="0" style="margin:0 auto;margin-top:20px; " border="0" width="800">
        <tr>
            <td align="right" >
                <table cellpadding="0" border="0" style="margin-right:-30px">
                    <tr align="right">
                        <td> 
                    <img src="/images/Vietmvplogo.png" height="50px" style="margin: 0 auto; display: inline; float:right;" />
                        
                        </td>
                        <td width="194">
                        <div style="margin-right:38px;height:87px">
                             <uc2:LoginControl ID="ucLogin" runat="server" />
                        </div>
                        </td>
                    </tr>
                </table>
            </td>
           
        </tr>
        <tr>
            <td align="right" style="padding-right:20px;padding-top:7px">
                
            </td>
        </tr>
        <tr>
            <td>
                <div style="text-align:right;margin-top:50px;margin-right:6px;">
                    <asp:ImageButton ID="btnAgent" ImageUrl="/images/btn_agent2.jpg" runat="server"></asp:ImageButton>
                </div>
            </td>
        </tr>
    </table>
    <%--
</asp:placeholder>--%>
        
    </form>
</body>
</html>