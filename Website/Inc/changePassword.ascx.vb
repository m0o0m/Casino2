Imports SBCBL.std
Imports SBCBL.Managers
Imports SBCBL.CEnums

Namespace SBCWebsite

    Partial Class Inc_changePassword
        Inherits SBCBL.UI.CSBCUserControl

        Protected Sub btnChangePassword_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnChangePassword.Click
            If SafeString(txtCurrentPassword.Text) = "" Then
                ClientAlert("Current Password Is Required", True)
                txtCurrentPassword.Focus()
                Return
            End If
            If SafeString(psdPassword.Password) = "" Then
                ClientAlert("New Password Is Required", True)
                psdPassword.Focus()
                Return
            End If

            Dim bSuccess As Boolean = False

            Select Case UserSession.UserType
                Case SBCBL.EUserType.Agent
                    bSuccess = (New CAgentManager).ChangePassword(UserSession.UserID, SafeString(txtCurrentPassword.Text) _
                                                                        , SafeString(psdPassword.Password), UserSession.UserID)
                    '' Clean Session
                    UserSession.Cache.ClearAgentInfo(UserSession.UserID, UserSession.AgentUserInfo.Login)
                Case SBCBL.EUserType.CallCenterAgent
                    bSuccess = (New CCallCenterAgentManager).ChangePassword(UserSession.UserID, SafeString(txtCurrentPassword.Text) _
                                                                        , SafeString(psdPassword.Password), UserSession.UserID)
                Case SBCBL.EUserType.Player
                    bSuccess = (New CPlayerManager).ChangePassword(UserSession.UserID, SafeString(txtCurrentPassword.Text) _
                                                                        , SafeString(psdPassword.Password), UserSession.UserID)
                    If bSuccess Then
                        bSuccess = (New CPlayerManager).ChangePhonePassword(UserSession.UserID, _
                                                                   SafeString(psdPassword.Password), UserSession.UserID)
                        '' Clean Session
                        UserSession.Cache.ClearPlayerInfo(UserSession.UserID, UserSession.PlayerUserInfo.Login)
                    End If
                Case SBCBL.EUserType.SuperAdmin
                    If UserSession.SuperAdminInfo.IsPartner Then
                        bSuccess = (New CSuperUserManager()).ChangePartnerPassword(UserSession.UserID, SafeString(txtCurrentPassword.Text) _
                                                                        , SafeString(psdPassword.Password), UserSession.UserID)
                    End If
            End Select

            If bSuccess Then
                ClientAlert("Successfully Changed.", True)
            Else
                ClientAlert("Cann't Change New Password. Re-check Current Password", True)
            End If
        End Sub

    End Class

End Namespace

