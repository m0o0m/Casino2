Imports System.Web.UI
Imports AjaxControlToolkit

<Assembly: WebResource("WebsiteLibrary.CTabContainer.js", "application/x-javascript")> 

<ClientScriptResource("WebsiteLibrary.CTabContainer", "WebsiteLibrary.CTabContainer.js")> _
Public Class CTabContainer
    Inherits TabContainer

    Protected Overrides Sub RenderHeader(ByVal writer As System.Web.UI.HtmlTextWriter)
        writer.Write(String.Format("<div id='{0}'>", ClientID & "_headerCover"))
        writer.Write(String.Format("<span id='{0}'></span>", ClientID & "_headerHPad"))
        writer.Write(String.Format("<div id='{0}'>", ClientID & "_headerCenter"))
        writer.Write(String.Format("<div id='{0}'>", ClientID & "_headerScroll"))
        MyBase.RenderHeader(writer)
        writer.Write("</div></div>")
        writer.Write(String.Format("<div id='{0}'></div>", ClientID & "_headerLeft"))
        writer.Write(String.Format("<div id='{0}'></div>", ClientID & "_headerRight"))
        writer.Write("</div>")
    End Sub

End Class
