Imports SBCBL.std

Namespace SBSAgents
    Partial Class AgentMail
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Email"

            MenuTabName = "ACCOUNT_STATUS"
            SubMenuActive = "MAIL_INBOX"
            DisplaySubTitlle(Me.Master, "Email")
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                setTabActive("INBOX_EMAIL")
            End If
        End Sub
#Region "Tabs"

        Private Sub setTabActive(ByVal psTabKey As String)
            lbnComposeEmail.Attributes.Remove("href")
            lbnInboxEmail.Attributes.Remove("href")
            lbnReplyEmail.Attributes.Remove("href")

            lbnComposeEmail.Attributes.Add("href", "#" + tabContent1.ClientID)
            lbnInboxEmail.Attributes.Add("href", "#" + tabContent2.ClientID)
            lbnReplyEmail.Attributes.Add("href", "#" + tabContent3.ClientID)

            ucComposeEmail.Visible = False
            ucInboxEmail.Visible = False
            ucReplyEmail.Visible = False
            Select Case UCase(psTabKey)
                Case "COMPOSE_EMAIL"
                    liCOMPOSE_EMAIL.Attributes.Add("class", "active")
                    tabContent1.Attributes.Add("class", "active")
                    ucComposeEmail.Visible = True
                Case "INBOX_EMAIL"
                    liINBOX_EMAIL.Attributes.Add("class", "active")
                    tabContent2.Attributes.Add("class", "active")
                    ucInboxEmail.Visible = True
                Case "REPLY_EMAIL"
                    liREPLY_EMAIL.Attributes.Add("class", "active")
                    tabContent3.Attributes.Add("class", "active")
                    ucReplyEmail.Visible = True
            End Select
        End Sub

        Protected Sub lbnTab_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            liCOMPOSE_EMAIL.Attributes.Remove("class")
            liINBOX_EMAIL.Attributes.Remove("class")
            liREPLY_EMAIL.Attributes.Remove("class")

            tabContent1.Attributes.Remove("class")
            tabContent2.Attributes.Remove("class")
            tabContent3.Attributes.Remove("class")

            tabContent1.Attributes.Add("class", "tab-pane fade in")
            tabContent2.Attributes.Add("class", "tab-pane fade in")
            tabContent3.Attributes.Add("class", "tab-pane fade in")

            setTabActive(CType(sender, LinkButton).CommandArgument)
        End Sub

#End Region
    End Class
End Namespace

