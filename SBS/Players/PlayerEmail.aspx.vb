Imports SBCBL.std

Namespace SBSPlayers

    Partial Class PlayerEmail
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Email"
            DisplaySubTitlle(Me.Master, "Email")
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not Me.IsPostBack Then
                setTabActive("INBOX_EMAIL")
                MenuTabName = "ACCOUNT_STATUS"
                SubMenuActive = "MAIL_INBOX"
            End If
        End Sub

#Region "Tabs"

        Private Sub setTabActive(ByVal psTabKey As String)
            lbnComposeEmail.CssClass = ""
            lbnInboxEmail.CssClass = ""
            lbnReplyEmail.CssClass = ""
            ucComposeEmail.Visible = False
            ucInboxEmail.Visible = False
            ucReplyEmail.Visible = False
            Select Case UCase(psTabKey)
                Case "COMPOSE_EMAIL"
                    lbnComposeEmail.CssClass = "selected"
                    ucComposeEmail.Visible = True
                Case "INBOX_EMAIL"
                    lbnInboxEmail.CssClass = "selected"
                    ucInboxEmail.Visible = True
                Case "REPLY_EMAIL"
                    lbnReplyEmail.CssClass = "selected"
                    ucReplyEmail.Visible = True
            End Select
        End Sub

        Protected Sub lbnTab_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            setTabActive(CType(sender, LinkButton).CommandArgument)
        End Sub

#End Region

    End Class
End Namespace
