<%@ Page Language="VB" AutoEventWireup="false" CodeFile="closeSelf.aspx.vb" Inherits="SBCWebsite.CloseSelf" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
    <title>Close Me!</title>

    <script>
	function init()
	{
		if (!window.top.self.opener || window.top.self.opener.closed)
		{			
			window.top.self.close();
			return;
		}	
			
		//todo: how to gracefully handled changed domain in parent:-T
	<%If Request("CBClient") = "Y" Then %>
		if(window.top.self.opener.cb_ChildClosed)
		{
			var cb = window.top.self.opener.cb_ChildClosed;
			cb(document.frm.CBData.value);
		}
	<%ElseIf Request("CB") = "Y" Then %>		
		var theform = window.top.self.opener.document.forms[0];
		
		if (theform && theform.__EVENTTARGET)
		{
			//this is an asp.net form 
			theform.__EVENTTARGET.value = "__POPUP_CB";
			theform.__EVENTARGUMENT.value = document.frm.CBData.value;
			//theform.submit();
			
			var callBackButton = window.top.self.opener.document.getElementById('<%= Request("ButtonClientID") %>');
			
		    if (callBackButton)
		        callBackButton.click(); //in the parent page auto call button click (this button instead of Request("ButtonClientID")) 
		    else
		        theform.submit();
		}
		else
		{
			//old schoo ASP or some non asp.net
			self.opener.location.reload(true) ;
		}
	<%ElseIf Request("resubmit_opener") <> "" Then %>		
		//obsolete b/c not as flexible as the CB version
		var theform = window.top.self.opener.document.forms[0];
		theform.submit();
	<%ElseIf Request.QueryString("url") <> "" Then %>
		window.top.self.opener.location.href = '<%=Request("url")%>';			
	<%End If%>
	
		window.top.self.close();
	}
    </script>

</head>
<body onload="init();">
    <form id="frm" method="post" runat="server">
        <input type="hidden" name="CBData" value="<%=Server.HTMLEncode(Request("CBData"))%>" />
    </form>
</body>
</html>
