Imports System.Data

Imports SBCBL.Managers
Imports SBCBL.std

Namespace SBSPlayer

    Partial Class SBS_Players_PlayerInfo
        Inherits SBCBL.UI.CSBCPage

#Region "Page events"

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Account Status"
            MenuTabName = "ACCOUNT_STATUS"
            CurrentPageName = "Account_Setting"
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not Me.IsPostBack Then
                setTabActive("CHANGE_PASSWORD")
            End If
        End Sub

#End Region

#Region "Tabs"

        Private Sub setTabActive(ByVal psTabKey As String)
            tbReplyEm.Attributes("class") = ""
            tbChangePw.Attributes("class") = ""
            tbInboxEm.Attributes("class") = ""
            tbComposeEm.Attributes("class") = ""
            ucAccountStatus.Visible = False
            ucChangePassword.Visible = False
            ucComposeEmail.Visible = False
            ucInboxEmail.Visible = False
            ucReplyEmail.Visible = False
            Select Case UCase(psTabKey)

                Case "CHANGE_PASSWORD", ""
                    tbChangePw.Attributes("class") = "active"
                    ucChangePassword.Visible = True
                Case "COMPOSE_EMAIL"
                    tbComposeEm.Attributes("class") = "active"
                    ucComposeEmail.Visible = True
                Case "INBOX_EMAIL"
                    tbInboxEm.Attributes("class") = "active"
                    ucInboxEmail.Visible = True
                Case "REPLY_EMAIL"
                    tbReplyEm.Attributes("class") = "active"
                    ucReplyEmail.Visible = True
            End Select
        End Sub

        Protected Sub lbnTab_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            setTabActive(CType(sender, LinkButton).CommandArgument)
        End Sub

#End Region
    End Class
End Namespace
