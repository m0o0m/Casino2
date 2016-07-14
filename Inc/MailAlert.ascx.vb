Imports SBCBL.std
Imports SBCBL.Managers

Namespace SBCPlayer
    Partial Class Inc_MailAlert
        Inherits SBCBL.UI.CSBCUserControl
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim oEmailsManager As CEmailsManager = New CEmailsManager()
            If Not IsPostBack Then
                If UserSession.UserType = SBCBL.EUserType.Agent Then
                    linkMailBox.HRef = "/SBS/Agents/AgentMail.aspx"
                ElseIf UserSession.UserType = SBCBL.EUserType.Player Then
                    linkMailBox.HRef = "/SBS/Players/PlayereMail.aspx"
                End If


                If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                    If oEmailsManager.CheckSuperExistsEmailOpen(UserSession.UserID) Then
                        ' pnAlertMail.Visible = True
                        hfHaveMail.Value = "Y"
                    Else
                        'pnAlertMail.Visible = False
                        hfHaveMail.Value = "N"
                    End If

                Else
                    If oEmailsManager.CheckExistsEmailReply(UserSession.UserID) Then
                        'pnAlertMail.Visible = True
                        hfHaveMail.Value = "Y"
                    Else
                        ' pnAlertMail.Visible = False
                        hfHaveMail.Value = "N"
                    End If

                End If
            End If


        End Sub

        'Public Sub lbtMailAlert_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lbtMailAlert.Click
        '    If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
        '        Response.Redirect("InboxMail.aspx?mailinbox=true")
        '    Else
        '        Response.Redirect("AccountStatus.aspx?mailinbox=true")
        '    End If
        'End Sub
    End Class
End Namespace
