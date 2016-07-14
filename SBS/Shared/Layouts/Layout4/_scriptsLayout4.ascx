<%@ Control Language="VB" AutoEventWireup="false" CodeFile="_scriptsLayout4.ascx.vb" Inherits="SBS_Shared_Layouts_Layout4_scriptsLayout4" %>


<script runat="server">
    Public Function GetPath() As String
        Return "/Content/themes/agent/layout4/"
    End Function
</script>

<script src="<%=GetPath()%>script/jquery-ui.js"></script>
<script src="<%=GetPath()%>script/bootstrap.min.js"></script>
<script src="<%=GetPath()%>script/bootstrap-hover-dropdown.js"></script>
<script src="<%=GetPath()%>script/html5shiv.js"></script>
<script src="<%=GetPath()%>script/respond.min.js"></script>
<script src="<%=GetPath()%>script/jquery.metisMenu.js"></script>
<script src="<%=GetPath()%>script/jquery.slimscroll.js"></script>
<script src="<%=GetPath()%>script/jquery.cookie.js"></script>
<script src="<%=GetPath()%>script/icheck.min.js"></script>
<script src="<%=GetPath()%>script/custom.min.js"></script>
<script src="<%=GetPath()%>script/jquery.news-ticker.js"></script>
<script src="<%=GetPath()%>script/jquery.menu.js"></script>
<script src="<%=GetPath()%>script/pace.min.js"></script>
<script src="<%=GetPath()%>script/holder.js"></script>
<script src="<%=GetPath()%>script/responsive-tabs.js"></script>
<script src="<%=GetPath()%>script/jquery.flot.js"></script>
<script src="<%=GetPath()%>script/jquery.flot.categories.js"></script>
<script src="<%=GetPath()%>script/jquery.flot.pie.js"></script>
<script src="<%=GetPath()%>script/jquery.flot.tooltip.js"></script>
<script src="<%=GetPath()%>script/jquery.flot.fillbetween.js"></script>
<script src="<%=GetPath()%>script/jquery.flot.stack.js"></script>
<script src="<%=GetPath()%>script/jquery.flot.spline.js"></script>
<script src="<%=GetPath()%>script/zabuto_calendar.min.js"></script>

<!--CORE JAVASCRIPT-->
<script src="<%=GetPath()%>script/main.js?v=1.0"></script>
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
