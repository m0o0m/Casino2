<%@ Control Language="VB" AutoEventWireup="false" CodeFile="_headScriptsLayout4.ascx.vb" Inherits="SBS_Shared_Layouts_Layout4_headScriptsLayout4" %>


<script runat="server">
    Public Function GetPath() As String
        Return "/Content/themes/agent/layout4/"
    End Function
</script>


<script src="<%=GetPath()%>script/jquery-1.10.2.min.js"></script>
<script src="<%=GetPath()%>script/jquery-migrate-1.2.1.min.js"></script>
