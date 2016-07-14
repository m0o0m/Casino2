Imports SBCBL.Managers
Imports SBCBL.std
Imports SBCBL.CacheUtils
Imports System.Data

Namespace SBCSuperAdmins
    Partial Class InboxMail
        Inherits SBCBL.UI.CSBCUserControl

        Private _emailAdress As String = ""

#Region "property"

        Property EmailAdressUser() As String
            Get
                Return SafeString(ViewState("EmailAdressUser"))
            End Get
            Set(ByVal value As String)
                ViewState("EmailAdressUser") = SafeString(value)
            End Set
        End Property

#End Region

#Region "Bind Data"
        Public Sub BindEmailSubject()
            Dim oEmailsManager As CEmailsManager = New CEmailsManager()
            ddlEmailSubject.DataTextField = "Subject"
            ddlEmailSubject.DataValueField = "Subject"
            ddlEmailSubject.DataSource = oEmailsManager.GetSubjectEmails(UserSession.UserID, GetSiteType)
            ddlEmailSubject.DataBind()
        End Sub

        Public Sub BindEmailInbox()
            Dim oEmailsManager As CEmailsManager = New CEmailsManager()
            Dim odtMailMessage As DataTable = oEmailsManager.GetEmailsBySuperAdminID(UserSession.UserID, ddlEmailSubject.SelectedValue, GetSiteType)
            dgEmailInbox.DataSource = odtMailMessage
            dgEmailInbox.DataBind()
            If odtMailMessage Is Nothing OrElse odtMailMessage.Rows.Count = 0 Then
                lblNoMessage.Text = "No Email Message"
            Else
                lblNoMessage.Text = ""
            End If
        End Sub
#End Region

#Region "Page Event"
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                dgEmailInbox.PageSize = 10
                dgEmailInbox.AllowPaging = True
                BindEmailSubject()
                BindEmailInbox()
            End If
            btnReply.Visible = True
            fdsEmailContent.Visible = False
            dgEmailInbox.Visible = True

        End Sub
#End Region

#Region "Event"
        Protected Sub dgEmailInbox_ItemDataBound(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgEmailInbox.ItemDataBound
            If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
                Dim lblSender As Label
                Dim lblUserType As Label

                lblSender = CType(e.Item.FindControl("lblSender"), Label)
                lblUserType = CType(e.Item.FindControl("lblUserType"), Label)
                GetUser(CType(e.Item.DataItem, DataRowView)("FromID").ToString, lblSender.Text, lblUserType.Text)


            End If
        End Sub

        Protected Sub dgEmailInbox_PageIndexChanged(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles dgEmailInbox.PageIndexChanged
            dgEmailInbox.CurrentPageIndex = e.NewPageIndex
            BindEmailInbox()
        End Sub


        Protected Sub dgEmailInbox_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgEmailInbox.ItemCommand

            Select Case UCase(e.CommandName)
                Case "SHOW"
                    DisplayMail(e.CommandArgument)
                    trReply.Visible = False
                    btnReply.Visible = False
                Case "REPLY"
                    DisplayMail(e.CommandArgument)
                    trReply.Visible = True
                    btnReply.Visible = True
            End Select

        End Sub

        Protected Sub ddlEmailSubject_Changed(ByVal source As Object, ByVal e As System.EventArgs) Handles ddlEmailSubject.SelectedIndexChanged
            BindEmailInbox()
        End Sub

        Protected Sub btnRepLy_Click(ByVal source As Object, ByVal e As System.EventArgs) Handles btnReply.Click
            Dim bInsertSuccessfull As Boolean
            Dim oEmailsManager As CEmailsManager = New CEmailsManager()
            If String.IsNullOrEmpty(fckMessage.Text) Then
                ClientAlert("Can't Reply, Message Is Required")
                btnReply.Visible = True
                fdsEmailContent.Visible = True
                dgEmailInbox.Visible = False
                Return
            End If
            bInsertSuccessfull = oEmailsManager.InsertReplyEmail(UserSession.UserID, UserSession.UserID, hfToID.Value, GetSiteType, lblEmailSubject.Text, fckMessage.Text, "", hfEmailID.Value)
            If Not bInsertSuccessfull Then
                ClientAlert("Can't Reply, Email Length Is Too Long To Send")
                btnReply.Visible = True
                fdsEmailContent.Visible = True
                dgEmailInbox.Visible = False
                Return
            End If
            bInsertSuccessfull = oEmailsManager.UpdateEmailAnswerByID(hfEmailID.Value, UserSession.UserID)
            If Not String.IsNullOrEmpty(EmailAdressUser) Then
                SentMail(EmailAdressUser, lblEmailSubject.Text, fckMessage.Text)
            End If
            fckMessage.Text = ""
            BindEmailInbox()


        End Sub

        Protected Sub btnCancel_Click(ByVal source As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
            BindEmailInbox()
        End Sub

#End Region

        Public Sub DisplayMail(ByVal psEmailID As String)
            Dim oEmailsManager As CEmailsManager = New CEmailsManager()
            Dim oCSuperUserManager As CSuperUserManager = New CSuperUserManager()
            Dim odtEmail As DataTable
            Dim odtEmailReply As DataTable
            odtEmail = oEmailsManager.GetEmailsByEmailID(psEmailID)
            lblTo.Text = UserSession.SuperAdminInfo.Login
            _emailAdress = SafeString(odtEmail.Rows(0)("ReplyToAddress"))
            EmailAdressUser = _emailAdress
            If Not String.IsNullOrEmpty(_emailAdress) Then
                lblFrom.Text = GetUserLogin(odtEmail.Rows(0)("FromID").ToString) & " (" & _emailAdress & ")"
            Else
                lblFrom.Text = GetUserLogin(odtEmail.Rows(0)("FromID").ToString)
            End If

            lblMessages.Text = SafeString(odtEmail.Rows(0)("EmailID"))

            fdsEmailContent.Visible = True
            dgEmailInbox.Visible = False
            lblEmailSubject.Text = SafeString(odtEmail.Rows(0)("Subject"))
            If odtEmail.Rows(0)("IsAnswer").Equals("Y") Then
                btnReply.Visible = False
                odtEmailReply = oEmailsManager.GetEmailsByReplyID(psEmailID)
                If odtEmailReply IsNot Nothing AndAlso odtEmailReply.Rows.Count > 0 Then
                    lblMessagesReply.Text = SafeString(odtEmailReply.Rows(0)("EmailID"))
                    dvContentReply.Visible = True
                Else
                    dvContentReply.Visible = False


                End If
            Else
                dvContentReply.Visible = False
            End If
            oEmailsManager.UpdateSuperEmailOpenByID(psEmailID, UserSession.UserID)
            hfEmailID.Value = psEmailID
            hfToID.Value = odtEmail.Rows(0)("FromID").ToString
        End Sub

        Public Sub GetUser(ByVal UserID As String, ByRef psLogin As String, ByRef psUserType As String)

            Dim odtUser As DataTable
            Dim oAgentManager As CAgentManager = New CAgentManager()
            Dim oPlayerManager As CPlayerManager = New CPlayerManager()
            odtUser = oAgentManager.GetByID(UserID)
            If odtUser Is Nothing OrElse odtUser.Rows.Count = 0 Then
                'ClientAlert(UserID, True)
                Return
                psLogin = oPlayerManager.GetPlayer(UserID).Login
                psUserType = "Player"
            Else
                psUserType = "Agent"
                psLogin = odtUser.Rows(0)("Login")
            End If

        End Sub

        Public Function GetUserLogin(ByVal UserID As String) As String
            Dim sLogin As String = ""
            Dim odtUser As DataTable
            Dim oAgentManager As CAgentManager = New CAgentManager()
            Dim oPlayerManager As CPlayerManager = New CPlayerManager()
            odtUser = oAgentManager.GetByID(UserID)
            If odtUser Is Nothing OrElse odtUser.Rows.Count = 0 Then
                sLogin = oPlayerManager.GetPlayer(UserID).Login
            Else
                sLogin = odtUser.Rows(0)("Login")
            End If
            Return sLogin
        End Function

        Protected Sub Page_Rerender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            ddlEmailSubject.Visible = dgEmailInbox.Visible
            lblEmailSubject.Visible = Not dgEmailInbox.Visible
        End Sub

        Protected Sub Page_Prerender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            If GetSiteType.ToString.Equals("SBC") Then
                Dim oEmailsManager As CEmailsManager = New CEmailsManager()
                Dim ucMailAlert As SBCBL.UI.CSBCUserControl = CType(Me.Page.Master.FindControl("ucMailAlert"), SBCBL.UI.CSBCUserControl)
                Dim pnAlertMail As Panel = DirectCast(ucMailAlert.FindControl("pnAlertMail"), Panel)
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

        Public Sub SentMail(ByVal psToEmailAdress As String, ByVal psSubject As String, ByVal psContent As String)
            Dim oMessage As New System.Net.Mail.MailMessage(SMTP_FROM, psToEmailAdress, psSubject, psContent)
            SendEmail(oMessage)
        End Sub



    End Class
End Namespace

