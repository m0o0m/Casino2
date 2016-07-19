Imports System.Data

Imports SBCBL.Managers
Imports SBCBL.std

Namespace SBSPlayer

    Partial Class AccountStatus
        Inherits SBCBL.UI.CSBCPage

#Region "Page events"

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Account Status"
            MenuTabName = "ACCOUNT_STATUS"
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not Me.IsPostBack Then
                Select Case True
                    Case SafeBoolean(Request.QueryString("mailinbox"))
                        setTabActive("INBOX_EMAIL")

                    Case SafeBoolean(Request.QueryString("RequireChangePass"))
                        setTabActive("CHANGE_PASSWORD")

                    Case Else
                        setTabActive("ACCOUNT_STATUS")

                End Select
            End If
        End Sub

#End Region

#Region "Tabs"

        Private Sub setTabActive(ByVal psTabKey As String)
            lbnAccountStatus.Attributes.Remove("href")
            lbnChangePass.Attributes.Remove("href")
            lbnComposeEmail.Attributes.Remove("href")
            lbnInboxEmail.Attributes.Remove("href")
            lbnReplyEmail.Attributes.Remove("href")

            lbnAccountStatus.Attributes.Add("href", "#" + tabContent1.ClientID)
            lbnChangePass.Attributes.Add("href", "#" + tabContent2.ClientID)
            lbnComposeEmail.Attributes.Add("href", "#" + tabContent3.ClientID)
            lbnInboxEmail.Attributes.Add("href", "#" + tabContent4.ClientID)
            lbnReplyEmail.Attributes.Add("href", "#" + tabContent5.ClientID)

            ucAccountStatus.Visible = False
            ucChangePassword.Visible = False
            ucComposeEmail.Visible = False
            ucInboxEmail.Visible = False
            ucReplyEmail.Visible = False
            Select Case UCase(psTabKey)
                Case "ACCOUNT_STATUS"
                    liACCOUNT_STATUS.Attributes.Add("class", "active")
                    tabContent1.Attributes.Add("class", "active")
                    ucAccountStatus.Visible = True
                Case "CHANGE_PASSWORD"
                    liCHANGE_PASSWORD.Attributes.Add("class", "active")
                    tabContent2.Attributes.Add("class", "active")
                    ucChangePassword.Visible = True
                Case "COMPOSE_EMAIL"
                    liCOMPOSE_EMAIL.Attributes.Add("class", "active")
                    tabContent3.Attributes.Add("class", "active")
                    ucComposeEmail.Visible = True
                Case "INBOX_EMAIL"
                    liINBOX_EMAIL.Attributes.Add("class", "active")
                    tabContent4.Attributes.Add("class", "active")
                    ucInboxEmail.Visible = True
                Case "REPLY_EMAIL"
                    liREPLY_EMAIL.Attributes.Add("class", "active")
                    tabContent5.Attributes.Add("class", "active")
                    ucReplyEmail.Visible = True
            End Select
        End Sub

        Protected Sub lbnTab_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            liACCOUNT_STATUS.Attributes.Remove("class")
            liCHANGE_PASSWORD.Attributes.Remove("class")
            liCOMPOSE_EMAIL.Attributes.Remove("class")
            liINBOX_EMAIL.Attributes.Remove("class")
            liREPLY_EMAIL.Attributes.Remove("class")

            tabContent1.Attributes.Remove("class")
            tabContent2.Attributes.Remove("class")
            tabContent3.Attributes.Remove("class")
            tabContent4.Attributes.Remove("class")
            tabContent5.Attributes.Remove("class")

            tabContent1.Attributes.Add("class", "tab-pane fade in")
            tabContent2.Attributes.Add("class", "tab-pane fade in")
            tabContent3.Attributes.Add("class", "tab-pane fade in")
            tabContent4.Attributes.Add("class", "tab-pane fade in")
            tabContent5.Attributes.Add("class", "tab-pane fade in")

            setTabActive(CType(sender, LinkButton).CommandArgument)
        End Sub

#End Region

    End Class

End Namespace

