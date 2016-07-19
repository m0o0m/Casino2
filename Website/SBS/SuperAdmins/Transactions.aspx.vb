Imports SBCBL.std

Namespace SBSSuperAdmin
    Partial Class Transactions
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Me.PageTitle = "Transactions"
            Me.SideMenuTabName = "TRANSACTIONS"
            Me.MenuTabName = "REPORTS MANAGEMENT"
            DisplaySubTitlle(Me.Master, "Transactions")

            If Not IsPostBack Then
                setTabActive(SafeString(Request("tab"))) 'if request tab is empty, default Agent tab
            End If
        End Sub

        Protected Sub lbtTab_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            'setTabActive(CType(sender, LinkButton).CommandArgument)
            Response.Redirect("Transactions.aspx?tab=" & CType(sender, LinkButton).CommandArgument)
        End Sub

        Private Sub setTabActive(ByVal psTabKey As String)
            lbtnTransactions.CssClass = ""
            lbtnMaintenanceCharge.CssClass = ""
            ucTransactions.Visible = False
            ucMaintenanceCharge.Visible = False

            Select Case True
                Case UCase(psTabKey) = "MAINTENANCE_CHARGE"
                    tMainternaceTrans.Attributes("class") = "active"
                    ucMaintenanceCharge.Visible = True

                Case UCase(psTabKey) = "PLAYER_TRANSACTIONS"
                    tPlayerTrans.Attributes("class") = "active"
                    ucTransactions.ViewPlayerTransaction = True
                    ucTransactions.Visible = True

                Case Else
                    tTrans.Attributes("class") = "active"
                    ucTransactions.ViewPlayerTransaction = False
                    ucTransactions.Visible = True

            End Select
        End Sub
    End Class
End Namespace