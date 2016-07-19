Imports SBCBL.std

Namespace SBSSuperAdmin

    Partial Class UsersManager
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "User Settings"
            SideMenuTabName = "USERS_MANAGER"
            Me.MenuTabName = "SYSTEM MANAGEMENT"
            DisplaySubTitlle(Me.Master, "User Settings")
        End Sub

        Protected Sub lbtTab_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            'setTabActive(CType(sender, LinkButton).CommandArgument)
            Response.Redirect("UsersManager.aspx?tab=" & CType(sender, LinkButton).CommandArgument)
        End Sub

        Private Sub setTabActive(ByVal psTabKey As String)
            tAgent.Attributes("class") = ""
            tCCAgent.Attributes("class") = ""
            tSuper.Attributes("class") = ""
            tPlayer.Attributes("class") = ""
            ucAgentManager.Visible = False
            ucSuperManager.Visible = False
            ucCCAgentManager.Visible = False
            ucPlayerManager.Visible = False
            ucPartnerManager.Visible = False

            Select Case True
                Case UCase(psTabKey) = "SUPER" AndAlso UserSession.SuperAdminInfo.IsManager
                    tSuper.Attributes("class") = "active"
                    ucSuperManager.Visible = True

                Case UCase(psTabKey) = "PARTNER"
                    tPartner.Attributes("class") = "active"
                    ucPartnerManager.Visible = True

                Case UCase(psTabKey) = "CCAGENT"
                    tCCAgent.Attributes("class") = "active"
                    ucCCAgentManager.Visible = True

                Case UCase(psTabKey) = "PLAYER"
                    tPlayer.Attributes("class") = "active"
                    ucPlayerManager.Visible = True

                Case Else
                    tAgent.Attributes("class") = "active"
                    ucAgentManager.Visible = True
            End Select
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                setTabActive(SafeString(Request("tab"))) 'if request tab is empty, default Agent tab
            End If
        End Sub

    End Class
End Namespace