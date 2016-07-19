<%@ Control Language="VB" AutoEventWireup="false" CodeFile="_scriptsLayout6.ascx.vb" Inherits="SBS_Shared_Layouts_Layout6_scriptsLayout6" %>


<script runat="server">
    Public Function GetPath() As String
        Return "/Content/themes/agent/layout6/"
    End Function
</script>

<!--CORE JAVASCRIPT-->
<script src="<%=GetPath()%>script/ttl_scripts.js?v=1.0"></script>

<script>        (function (i, s, o, g, r, a, m) {
            i['GoogleAnalyticsObject'] = r;
            i[r] = i[r] || function () {
                (i[r].q = i[r].q || []).push(arguments)
            }, i[r].l = 1 * new Date();
            a = s.createElement(o),
            m = s.getElementsByTagName(o)[0];
            a.async = 1;
            a.src = g;
            m.parentNode.insertBefore(a, m)
        })
</script>
