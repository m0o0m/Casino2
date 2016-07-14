Imports SBCBL.std
Namespace SBCWebsite
    Partial Class _Default
        Inherits System.Web.UI.Page

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            If Not IsPostBack Then
                Dim oCache As New SBCBL.CacheUtils.CCacheManager()
                Dim eSiteType As SBCBL.CEnums.ESiteType = oCache.GetSiteType(Request.Url.Host)

                Response.Redirect(String.Format("/{0}/default.aspx", eSiteType.ToString()))
            End If
        End Sub

    End Class
End Namespace