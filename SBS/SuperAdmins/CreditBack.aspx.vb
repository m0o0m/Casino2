Imports SBCBL.CacheUtils
Imports SBCBL.std

Namespace SBSSuperAdmin
    Partial Class CreditBack
        Inherits SBCBL.UI.CSBCPage

        Public ReadOnly Property PlayerID() As String
            Get
                Return SafeString(Request("playerid"))
            End Get
        End Property

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            lblPlayer.Text = SafeString(Request("player"))
        End Sub

        Protected Sub btnOK_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnOK.Click
            If SafeInteger(txtAmount.Text) = 0 Then
                ClientAlert("Please Input Amount.")
                Return
            End If

            If PlayerID = "" Then
                ClientAlert("Can't Credit Back Now")
                Return
            End If

            Try
                Application.Lock()
                Dim oPLayer As CPlayer = UserSession.Cache.GetPlayerInfo(PlayerID)

                If (New SBCBL.Managers.CPlayerManager()).CreditBack(oPLayer.UserID, oPLayer.AgentID, _
                        SafeDouble(txtAmount.Text), SafeString(txtDescription.Text), UserSession.UserID) Then
                    RunJS("close", "window.close()", False)
                Else
                    ClientAlert("Can't Credit Back Now.")
                End If
            Catch ex As Exception
                ClientAlert("Unsuccessfully Credit Back.")
            Finally
                Application.UnLock()
            End Try


        End Sub
    End Class
End Namespace