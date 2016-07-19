Namespace SBSCallCenterAgents

    Partial Class AccountStatus
        Inherits SBCBL.UI.CSBCPage

#Region "Page events"

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Account Status"
            MenuTabName = "ACCOUNT_STATUS"
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not Me.IsPostBack Then
                setTabActive("ACCOUNT_STATUS")
            End If
        End Sub

#End Region

#Region "Tabs"

        Private Sub setTabActive(ByVal psTabKey As String)
            lbnChangePass.CssClass = ""
            ucChangePassword.Visible = False

            Select Case UCase(psTabKey)
                Case "CHANGE_PASSWORD"
                    lbnChangePass.CssClass = "selected"
                    ucChangePassword.Visible = True

                Case Else
                    lbnChangePass.CssClass = "selected"
                    ucChangePassword.Visible = True
            End Select
        End Sub

        Protected Sub lbnTab_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            setTabActive(CType(sender, LinkButton).CommandArgument)
        End Sub

#End Region

    End Class

End Namespace

