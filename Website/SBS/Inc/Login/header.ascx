<%@ Control Language="VB" AutoEventWireup="false" CodeFile="header.ascx.vb" Inherits="SBS_Inc_Login_header1" %>

<%@ Register Src="defaultHeader.ascx" TagName="defaultHeader" TagPrefix="uc1" %>
<%@ Register Src="layout1Header.ascx" TagName="layout1Header" TagPrefix="uc3" %>
<%@ Register Src="layout4Header.ascx" TagName="layout4Header" TagPrefix="uc5" %>
<%@ Register Src="layout5Header.ascx" TagName="layout5Header" TagPrefix="uc7" %>

<% 
    Dim oSetting As New SBCBL.CacheUtils.CWhiteLabelSettings()
    oSetting = oSetting.LoadByUrl(Request.Url.Host)

    If oSetting IsNot Nothing AndAlso Not String.IsNullOrEmpty(oSetting.LoginTemplate) Then
        Dim themeName As String
        themeName = oSetting.LoginTemplate.ToLower().Trim()
        Select Case themeName
            Case "template1"
%>
<uc3:layout1Header ID="layout1Header" runat="server" />
<%
Case "template2"
%>
<uc5:layout4Header ID="layout4Header" runat="server" />
<%
Case "template3"
%>
<uc7:layout5Header ID="layout5Header" runat="server" />
<%
Case Else
%>
<uc1:defaultHeader ID="defaultHeader1" runat="server" />
<%
End Select
Else
%>
<uc1:defaultHeader ID="defaultHeader2" runat="server" />
<%  
End If
%>
