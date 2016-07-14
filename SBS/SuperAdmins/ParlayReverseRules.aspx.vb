Imports SBCBL.std

Namespace SBSSuperAdmins
    Partial Class SBC_SuperAdmins_ParlayReverseRules
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Me.PageTitle = "Parlay Setup"
            Me.SideMenuTabName = "PARLAY_REVERSE_RULES"
            Me.MenuTabName = "GAME MANAGEMENT"
            DisplaySubTitlle(Me.Master, "Parlay Setup")
        End Sub
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                setTabActive(SafeString(Request("tab"))) 'if request tab is empty, default Agent tab
            End If
        End Sub

        Protected Sub lbtTab_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            'setTabActive(CType(sender, LinkButton).CommandArgument)
            Response.Redirect("ParlayReverseRules.aspx?tab=" & CType(sender, LinkButton).CommandArgument)
        End Sub

        Private Sub setTabActive(ByVal psTabKey As String)
            tParlaySetup.Attributes("class") = ""
            tParlayAllow.Attributes("class") = ""
            tBWParlayAllow.Attributes("class") = ""
            tParlayPayout.Attributes("class") = ""
            ucParlaysAllowance.Visible = False
            ucParlaySetup.Visible = False
            ucParlayPayout.Visible = False

            Select Case True
                Case UCase(psTabKey) = "PARLAY_ALLOWANCE"
                    tParlayAllow.Attributes("class") = "active"
                    ucParlaysAllowance.BetweenGames = False
                    ucParlaysAllowance.Visible = True

                Case UCase(psTabKey) = "BW_PARLAY_ALLOWANCE"
                    tBWParlayAllow.Attributes("class") = "active"
                    ucParlaysAllowance.BetweenGames = True
                    ucParlaysAllowance.Visible = True

                Case UCase(psTabKey) = "PARLAY_PAYOUT"
                    tParlayPayout.Attributes("class") = "active"
                    ucParlayPayout.Visible = True

                Case Else
                    tParlaySetup.Attributes("class") = "active"
                    ucParlaySetup.Visible = True
            End Select
        End Sub

    End Class
End Namespace
