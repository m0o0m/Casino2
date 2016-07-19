<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Sessionexpired.aspx.vb" Inherits="Sessionexpired" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>The session has expired</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h4 style="color:Red;">
                You do not have any currently open applications. This can be caused if your session has expired, or you tried to use the browser's Back button. Please reopen your application to continue working.</h4>
                <br />
                <asp:Button ID="btnGoToLogin" runat="server" Text="Go To Login Page" />
        </div>
    </form>
</body>
</html>
