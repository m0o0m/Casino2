Imports SBCBL.Managers
Imports SBCBL.std
Imports SBCBL.CacheUtils
Imports System.Data

Namespace SBCPlayer
    Partial Class InboxEmail
        Inherits SBCBL.UI.CSBCUserControl

        Public Enum EReply
            Reply
            Inbox
        End Enum

#Region "Property"

        Public Property ReplyEmail() As EReply
            Get
                Return SafeString(ViewState("Reply"))
            End Get
            Set(ByVal value As EReply)
                ViewState("Reply") = value
            End Set
        End Property

#End Region

#Region "Bind Data"
        Public Sub BindEmailInbox()
            Dim oEmailsManager As CEmailsManager = New CEmailsManager()
            Dim odtMailMessage As DataTable
            If ReplyEmail = EReply.Reply Then
                odtMailMessage = oEmailsManager.GetReplyEmailsByUserID(UserSession.UserID)
                dgEmailInbox.Columns(4).Visible = False
            Else
                odtMailMessage = oEmailsManager.GetEmailsByUserID(UserSession.UserID, GetSiteType)
            End If
            dgEmailInbox.DataSource = odtMailMessage
            dgEmailInbox.DataBind()
            If odtMailMessage Is Nothing OrElse odtMailMessage.Rows.Count = 0 Then
                lblNoMessage.Text = IIf(ReplyEmail.ToString.Equals("Reply"), "No Reply Email Message", "No Email Message")
            Else
                lblNoMessage.Text = ""
            End If
        End Sub
#End Region

#Region "Page Event"
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            BindEmailInbox()
            fdsEmailContent.Visible = False
            dgEmailInbox.Visible = True
            dgEmailInbox.PageSize = 10
            dgEmailInbox.AllowPaging = True
            lblEmailSubject.Text = ""
        End Sub
#End Region

#Region "Event"

        Protected Sub dgEmailInbox_DataBound(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgEmailInbox.ItemDataBound
            If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
                If ReplyEmail = EReply.Reply Then
                    Dim lbnSubject, lbnMessages, lbnSentDates As LinkButton

                    lbnSubject = CType(e.Item.FindControl("lbnSubject"), LinkButton)
                    lbnSentDates = CType(e.Item.FindControl("lbnSentDates"), LinkButton)
                    lbnMessages = CType(e.Item.FindControl("lbnMessages"), LinkButton)
                    lbnSubject.CommandArgument = CType(e.Item.DataItem, DataRowView)("ReplyID").ToString
                    lbnSentDates.CommandArgument = CType(e.Item.DataItem, DataRowView)("ReplyID").ToString
                    lbnMessages.CommandArgument = CType(e.Item.DataItem, DataRowView)("ReplyID").ToString
                End If
            End If
        End Sub

        Protected Sub dgEmailInbox_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgEmailInbox.ItemCommand

            Select Case UCase(e.CommandName)
                Case "SHOW"
                    If ReplyEmail = EReply.Reply Then

                        DisplayReplyEmail(e.CommandArgument)
                    Else

                        DisplayMail(e.CommandArgument, False)
                    End If

                Case "REPLY"
                    DisplayMail(e.CommandArgument, True)

            End Select

        End Sub

        Protected Sub dgEmailInbox_PageIndexChanged(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles dgEmailInbox.PageIndexChanged
            dgEmailInbox.CurrentPageIndex = e.NewPageIndex
            BindEmailInbox()
        End Sub


#End Region

        Public Sub DisplayMail(ByVal psEmailID As String, ByVal bReply As Boolean)
            Dim oEmailsManager As CEmailsManager = New CEmailsManager()
            Dim oCSuperUserManager As CSuperUserManager = New CSuperUserManager()
            Dim odtEmail As DataTable
            If bReply Then
                odtEmail = oEmailsManager.GetEmailsByReplyID(psEmailID)
                lblFrom.Text = oCSuperUserManager.GetSuperByID(SafeString(odtEmail.Rows(0)("FromID")))(0)("Login") ' & " (" & SafeString(odtEmail.Rows(0)("ReplyToAddress")) & ")"
                If UserSession.UserType = SBCBL.EUserType.Agent Then
                    lblTo.Text = UserSession.AgentUserInfo.Login
                Else
                    lblTo.Text = UserSession.PlayerUserInfo.Login
                End If
                lblMessages.Text = SafeString(odtEmail.Rows(0)("EmailID"))
                oEmailsManager.UpdateReplyEmailOpenByID(psEmailID, UserSession.UserID)
                oEmailsManager.UpdateEmailOpenByID(psEmailID, UserSession.UserID)
            Else
                odtEmail = oEmailsManager.GetEmailsByEmailID(psEmailID)
                If oCSuperUserManager.GetSuperByID(SafeString(odtEmail.Rows(0)("ToID"))).Rows.Count > 0 Then ''sent mail from player or agent
                    lblTo.Text = oCSuperUserManager.GetSuperByID(SafeString(odtEmail.Rows(0)("ToID")))(0)("Login")
                    If UserSession.UserType = SBCBL.EUserType.Agent Then
                        lblFrom.Text = UserSession.AgentUserInfo.Login & " (" & SafeString(odtEmail.Rows(0)("ReplyToAddress")) & ")"
                    Else
                        lblFrom.Text = UserSession.PlayerUserInfo.Login & " (" & SafeString(odtEmail.Rows(0)("ReplyToAddress")) & ")"
                    End If

                Else
                    lblFrom.Text = oCSuperUserManager.GetSuperByID(SafeString(odtEmail.Rows(0)("FromID")))(0)("Login") ' & " (" & SafeString(odtEmail.Rows(0)("ReplyToAddress")) & ")"
                    If UserSession.UserType = SBCBL.EUserType.Agent Then
                        lblTo.Text = UserSession.AgentUserInfo.Login
                    Else
                        lblTo.Text = UserSession.PlayerUserInfo.Login
                    End If
                    oEmailsManager.UpdateEmailOpenByID(psEmailID, UserSession.UserID) ''open mail from super
                End If

                lblMessages.Text = psEmailID
            End If
            fdsEmailContent.Visible = True
            dgEmailInbox.Visible = False
            trReplyEmail.Visible = False
            trEmailMessages.Visible = True
            lblEmailSubject.Text = SafeString(odtEmail.Rows(0)("Subject"))

        End Sub

        Public Sub DisplayReplyEmail(ByVal psEmailID As String)
            Dim oEmailsManager As CEmailsManager = New CEmailsManager()
            Dim oCSuperUserManager As CSuperUserManager = New CSuperUserManager()
            Dim odtEmail As DataTable
            Dim odtEmailReply As DataTable
            odtEmail = oEmailsManager.GetEmailsByEmailID(psEmailID)
            If UserSession.UserType = SBCBL.EUserType.Agent Then
                lblTo.Text = UserSession.AgentUserInfo.Login
            Else
                lblTo.Text = UserSession.PlayerUserInfo.Login
            End If
            lblFrom.Text = oCSuperUserManager.GetSuperByID(SafeString(odtEmail.Rows(0)("ToID")))(0)("Login")
            lblEmailMessages.Text = SafeString(odtEmail.Rows(0)("EmailID"))
            odtEmail = oEmailsManager.GetEmailsByEmailID(psEmailID)
            If odtEmail.Rows(0)("IsAnswer").Equals("Y") Then
                odtEmailReply = oEmailsManager.GetEmailsByReplyID(psEmailID)
                If odtEmailReply IsNot Nothing AndAlso odtEmailReply.Rows.Count > 0 Then
                    lblMessagesReply.Text = SafeString(odtEmailReply.Rows(0)("EmailID"))
                End If
            End If
            trEmailMessages.Visible = False
            trReplyEmail.Visible = True
            fdsEmailContent.Visible = True
            dgEmailInbox.Visible = False
            lblEmailSubject.Text = SafeString(odtEmail.Rows(0)("Subject"))
            oEmailsManager.UpdateReplyEmailOpenByID(psEmailID, UserSession.UserID)
            oEmailsManager.UpdateEmailOpenByID(psEmailID, UserSession.UserID)
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

    End Class
End Namespace
