Imports SBCBL.std
Imports SBCBL.Managers

Namespace SBCPlayer
    Partial Class MailAlert
        Inherits SBCBL.UI.CSBCUserControl
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim oEmailsManager As CEmailsManager = New CEmailsManager()
            If Not IsPostBack Then
                If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                    If oEmailsManager.CheckSuperExistsEmailOpen(UserSession.UserID) Then
                        pnAlertMail.Visible = True
                    Else
                        pnAlertMail.Visible = False
                    End If

                Else
                    If oEmailsManager.CheckExistsEmailReply(UserSession.UserID) Then
                        pnAlertMail.Visible = True
                    Else
                        pnAlertMail.Visible = False
                    End If

                End If
            End If


        End Sub

        Public Sub lbtMailAlert_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lbtMailAlert.Click
            If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                Response.Redirect("InboxMail.aspx?mailinbox=true")
            Else
                Response.Redirect("AccountStatus.aspx?mailinbox=true")
            End If
        End Sub
    End Class
End Namespace
