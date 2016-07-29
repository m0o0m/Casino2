<%@ Control Language="VB" AutoEventWireup="false" CodeFile="_headScriptsLayout6.ascx.vb" Inherits="SBS_Shared_Layouts_Layout6_headScriptsLayout6" %>


<script runat="server">
    Public Function GetPath() As String
        Return "/Content/themes/agent/layout6/"
    End Function
</script>

<script src="<%=GetPath()%>script/wowslider/wowslider.js"></script>
<script src="<%=GetPath()%>script/wowslider/wowslider.script.js"></script>
<script src="<%=GetPath()%>script/jquery-1.10.2.min.js"></script>
<script src="<%=GetPath()%>script/jquery-migrate-1.2.1.min.js"></script>
