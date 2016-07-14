Imports System.Data
Imports SBCBL.Security
Imports SBCBL.CEnums
Imports SBCBL.std
Imports SBCBL.Utils
Imports WebsiteLibrary.DBUtils
Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports SBCBL


Namespace SBSAgents
    Partial Class PlayerSubAgentEdit
        Inherits SBCBL.UI.CSBCUserControl
        Dim log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())
        Public Event ButtonClick(ByVal sButtonType As String)

        Public Function LoadPlayerInfo(ByVal psPlayerID As String) As Boolean
            Dim odrPlayer As DataRow = (New CPlayerManager).GetPlayerDataRow(psPlayerID)
            txtName.Text = SafeString(odrPlayer("Name"))
            txtLogin.Text = SafeString(odrPlayer("Login"))
            txtPass.Text = SafeString(odrPlayer("Password"))
            hfPlayerID.Value = psPlayerID
            Return True
        End Function

        Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
            If String.IsNullOrEmpty(txtName.Text) Then
                ClientAlert("Please, input player name")
                Return
            End If

            If String.IsNullOrEmpty(txtLogin.Text) Then
                ClientAlert("Please, input player login")
                Return
            End If

            If String.IsNullOrEmpty(txtPass.Text) Then
                ClientAlert("Please, input password")
                Return
            End If

            Dim sError As String = ""
            Dim bExisted As Boolean = (New CPlayerManager).IsExistedLogin(txtLogin.Text, hfPlayerID.Value, sError, std.GetSiteType)
            If bExisted Then
                ClientAlert("Loginis existed")
                Return
            End If



            Dim oPlayerManager = New CPlayerManager()
            If oPlayerManager.UpdatePlayer(hfPlayerID.Value, txtName.Text, txtLogin.Text, txtPass.Text, UserSession.UserID) Then
                ClientAlert("Save successful")
                RaiseEvent ButtonClick(SafeString(IIf(True, "SAVE SUCCESSFUL", "SAVE UNSUCCESSFUL")))
            End If
        End Sub

        Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
            RaiseEvent ButtonClick(SafeString(IIf(True, "SAVE SUCCESSFUL", "SAVE UNSUCCESSFUL")))
        End Sub
    End Class
End Namespace
