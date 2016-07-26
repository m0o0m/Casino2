<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" EnableViewStateMac="false"
    Inherits="SBSWebsite.SBS_Default" %>

<%@ Register Src="../Login/LoginControl.ascx" TagName="LoginControl" TagPrefix="uc2" %>
<%@ Register Src="Inc/Login/header.ascx" TagName="header" TagPrefix="uc1" %>
<%@ Register Src="Inc/Login/loginForm.ascx" TagName="loginForm" TagPrefix="uc3" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head2" runat="server">
    <title>Sports Spicks System</title>
    <uc1:header ID="header1" runat="server" />
</head>
<body class="home">
    <uc3:loginForm ID="loginForm1" runat="server" />
</body>
</html>

