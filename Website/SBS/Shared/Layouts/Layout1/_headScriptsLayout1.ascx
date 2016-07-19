<%@ Control Language="VB" AutoEventWireup="false" CodeFile="_headScriptsLayout1.ascx.vb" Inherits="SBS_Shared_Layouts_Layout1_headScriptsLayout1" %>

<script runat="server">
    Public Function GetPath() As String
        Return "/Content/themes/agent/layout1/"
    End Function
</script>

<script src="<%=GetPath()%>script/jquery-1.10.2.min.js"></script>
<script src="<%=GetPath()%>script/jquery-migrate-1.2.1.min.js"></script>
