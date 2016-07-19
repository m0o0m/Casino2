<%@ Control Language="VB" AutoEventWireup="false" CodeFile="_headScriptsLayout3.ascx.vb" Inherits="SBS_Shared_Layouts_Layout3_headScriptsLayout3" %>

<script runat="server">
    Public Function GetPath() As String
        Return "/Content/themes/agent/layout3/"
    End Function
</script>

<script src="<%=GetPath()%>script/jquery-1.10.2.min.js"></script>

