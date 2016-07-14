Imports SBCBL.std
Imports FileDB

Namespace SBSSuperAdmin

    Partial Class ContentManager
        Inherits SBCBL.UI.CSBCPage

#Region "Page events"

        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Rules-Odds-Payout Display"
            SideMenuTabName = "HTML_CONTENTS_MANAGER"
            Me.MenuTabName = "SYSTEM MANAGEMENT"
            DisplaySubTitlle(Me.Master, "Rules-Odds-Payout Display")
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not Me.IsPostBack Then
                ucHTMLContent.loadFileDBContent(ddlTypes.Value)
            End If
        End Sub

#End Region

        Protected Sub ddlTypes_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlTypes.SelectedIndexChanged
            ucHTMLContent.loadFileDBContent(ddlTypes.Value)
        End Sub

    End Class

End Namespace

