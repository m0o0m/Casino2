

Namespace SBSAgents
    Partial Class SBS_Agents_Management_Settings_RiskControl
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
            If Not IsPostBack Then
                ucFixedSpreadMoney.BindCurrentUserData()
            End If
        End Sub

    End Class
End Namespace