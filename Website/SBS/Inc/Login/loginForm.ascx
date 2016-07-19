<%@ Control Language="VB" AutoEventWireup="false" CodeFile="loginForm.ascx.vb" Inherits="SBS_Inc_Login_loginForm" %>
<%@ Register Src="defaultLoginForm.ascx" TagName="defaultLoginForm" TagPrefix="uc1" %>
<%@ Register Src="layout1LoginForm.ascx" TagName="layout1LoginForm" TagPrefix="uc2" %>
<%@ Register Src="layout4LoginForm.ascx" TagName="layout4LoginForm" TagPrefix="uc3" %>
<%@ Register Src="layout5LoginForm.ascx" TagName="layout5LoginForm" TagPrefix="uc4" %>
<%@ Register Src="layout6LoginForm.ascx" TagName="layout6LoginForm" TagPrefix="uc6" %>

<% 
    Dim oSetting As New SBCBL.CacheUtils.CWhiteLabelSettings()
    oSetting = oSetting.LoadByUrl(Request.Url.Host)

    If oSetting IsNot Nothing AndAlso Not String.IsNullOrEmpty(oSetting.LoginTemplate) Then
        Dim themeName As String
        themeName = oSetting.LoginTemplate.ToLower().Trim()
        Select Case themeName
            Case "template1"
%>
<uc2:layout1LoginForm ID="layout1LoginForm1" runat="server" />
<%
Case "template2"
%>
<uc3:layout4LoginForm ID="layout4LoginForm1" runat="server" />
<%
Case "template3"
%>
<uc4:layout5LoginForm ID="layout5LoginForm1" runat="server" />
<%
Case "template4"
%>
<uc6:layout6LoginForm ID="layout6LoginForm1" runat="server" />
<%
Case Else
%>
<uc1:defaultLoginForm ID="defaultLoginForm1" runat="server" />
<%
End Select
Else
%>
<uc1:defaultLoginForm ID="defaultLoginForm2" runat="server" />
<%  
End If
%>

