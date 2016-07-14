Imports SBCBL.Managers
Imports SBCBL.std
Imports SBCBL.CacheUtils
Imports System.Data

Namespace SBCPlayer
    Partial Class ComposeEmail
        Inherits SBCBL.UI.CSBCUserControl


#Region "Property"

        Public ReadOnly Property ListPlayer() As List(Of String)
            Get
                Dim odtPlayer As DataTable
                Dim lstPlayerID As New List(Of String)
                odtPlayer = (New CPlayerManager()).GetAllPlayerByListAgentID(ListAgent)
                For Each drPlayerID As DataRow In odtPlayer.Rows
                    lstPlayerID.Add(SafeString(drPlayerID("PlayerID")))
                Next
                Return lstPlayerID
            End Get
        End Property

        Public ReadOnly Property ListAgent() As List(Of String)
            Get
                Dim oAgentsManager As CAgentManager = New CAgentManager()
                Dim odtAgent As DataTable
                Dim lstAgentID As New List(Of String)
                odtAgent = loadAgents(UserSession.UserID) ' oAgentsManager.GetAgentsBySuperID(UserSession.UserID, False)
                For Each drAgentID As DataRow In odtAgent.Rows
                    lstAgentID.Add(SafeString(drAgentID("AgentID")))
                Next
                Return lstAgentID
            End Get
        End Property

#End Region

#Region "Bind Data"

        Public Sub BindEmailSubject()
            Dim oCSubjectEmailManager As CSubjectEmailManager = New CSubjectEmailManager()
            ddlEmailSubject.DataTextField = "Subject"
            ddlEmailSubject.DataValueField = "Subject"
            ddlEmailSubject.DataSource = oCSubjectEmailManager.GetALLSubjectEmail()
            ddlEmailSubject.DataBind()
        End Sub

        Private Sub bindAgents(ByVal psSuperID As String)
            ddlAgent.DataSource = loadAgents(psSuperID)
            ddlAgent.DataTextField = "AgentName"
            ddlAgent.DataValueField = "AgentID"
            ddlAgent.DataBind()
        End Sub

        Private Sub bindPlayers(ByVal plstAgentID As List(Of String))
            ddlPlayer.DataSource = (New CPlayerManager()).GetAllPlayerByListAgentID(plstAgentID)
            ddlPlayer.DataTextField = "FullName"
            ddlPlayer.DataValueField = "PlayerID"
            ddlPlayer.DataBind()
        End Sub
#End Region

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                    trSelectUser.Visible = True
                Else
                    trSelectUser.Visible = False
                End If
                BindEmailSubject()
            End If
        End Sub

#Region "Event"

        Protected Sub SendEmail_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSenMail.Click
            Dim oEmailsManager As CEmailsManager = New CEmailsManager()
            Dim bInsertSuccessfull As Boolean = False
            If Not ValidEmail() Then
                Return
            End If
            If UserSession.UserType = SBCBL.EUserType.Player Then
                bInsertSuccessfull = oEmailsManager.InsertEmail(UserSession.UserID, UserSession.UserID, UserSession.PlayerUserInfo.SuperAdminID, GetSiteType, ddlEmailSubject.SelectedValue, fckMessage.Text, txtReplyToAddress.Text)

            ElseIf UserSession.UserType = SBCBL.EUserType.Agent Then
                bInsertSuccessfull = oEmailsManager.InsertEmail(UserSession.UserID, UserSession.UserID, UserSession.AgentUserInfo.SuperAdminID, GetSiteType, ddlEmailSubject.SelectedValue, fckMessage.Text, txtReplyToAddress.Text)
            Else

                If rblUser.SelectedValue.Equals("Agent", StringComparison.CurrentCultureIgnoreCase) Then
                    bInsertSuccessfull = SendMailAgent("")
                ElseIf rblUser.SelectedValue.Equals("Player", StringComparison.CurrentCultureIgnoreCase) Then
                    bInsertSuccessfull = SendMailPlayer("")
                Else
                    bInsertSuccessfull = SendMailAgent("ALL")
                    bInsertSuccessfull = SendMailPlayer("ALL")
                End If

                '  bInsertSuccessfull = oEmailsManager.InsertEmail(UserSession.UserID, UserSession.UserID, UserSession.AgentUserInfo.SuperAdminID, GetSiteType, ddlEmailSubject.SelectedValue, fckMessage.Text, txtReplyToAddress.Text)
            End If
            If bInsertSuccessfull Then
                txtReplyToAddress.Text = ""
                fckMessage.Text = ""
                lblMessageEmpty.Text = ""
            Else
                ClientAlert("Send Email Error", True)
                Return
            End If
        End Sub

        Protected Sub ddlAgent_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlAgent.SelectedIndexChanged
            Dim lstAgentID As New List(Of String)
            If ddlAgent.SelectedValue.Equals("ALL", StringComparison.CurrentCultureIgnoreCase) Then
                bindPlayers(ListAgent)
            Else
                lstAgentID.Add(SafeString(ddlAgent.SelectedValue))
                bindPlayers(lstAgentID)
            End If

        End Sub

        Protected Sub rblUser_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles rblUser.SelectedIndexChanged
            ddlAgent.Visible = False
            ddlPlayer.Visible = False
            If rblUser.SelectedValue.Equals("Agent", StringComparison.CurrentCultureIgnoreCase) Then
                bindAgents(UserSession.UserID)
                ddlAgent.Visible = True
                ddlAgent.AutoPostBack = False
            ElseIf rblUser.SelectedValue.Equals("Player", StringComparison.CurrentCultureIgnoreCase) Then
                bindAgents(UserSession.UserID)
                bindPlayers(ListAgent)
                ddlAgent.Visible = True
                ddlAgent.AutoPostBack = True
                ddlPlayer.Visible = True

            Else
                ddlAgent.Items.Clear()
                ddlPlayer.Items.Clear()
                ddlAgent.Visible = False
                ddlPlayer.Visible = False
            End If

        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            lblAgent.Visible = ddlAgent.Visible
            lblPlayer.Visible = ddlPlayer.Visible
        End Sub
#End Region

        Public Function ValidEmail() As Boolean
            lblMessageEmpty.Text = ""
            If String.IsNullOrEmpty(fckMessage.Text) Then
                lblMessageEmpty.Text = "Message is required"
                Return False
            End If
            If fckMessage.Text.Length > 5000 Then
                ClientAlert("Email Lenght Is Larger Than 5000 Character", True)
                Return False
            End If
            Return True
        End Function

        Public Function SendMailAgent(ByVal psSendAll As String) As Boolean
            Dim bInsertSuccessfull As Boolean = False
            Dim lstAgent As List(Of String)
            Dim oEmailsManager As CEmailsManager = New CEmailsManager()
            If ddlAgent.SelectedValue.Equals("ALL") OrElse psSendAll.Equals("ALL", StringComparison.CurrentCultureIgnoreCase) Then
                lstAgent = ListAgent
                For Each sAgentID As String In lstAgent
                    bInsertSuccessfull = oEmailsManager.InsertEmail(UserSession.UserID, UserSession.UserID, sAgentID, GetSiteType, ddlEmailSubject.SelectedValue, fckMessage.Text, txtReplyToAddress.Text, "Y")
                    If Not bInsertSuccessfull Then
                        Return False
                    End If
                Next
            Else
                bInsertSuccessfull = oEmailsManager.InsertEmail(UserSession.UserID, UserSession.UserID, ddlAgent.SelectedValue, GetSiteType, ddlEmailSubject.SelectedValue, fckMessage.Text, txtReplyToAddress.Text, "Y")
            End If
            Return bInsertSuccessfull
        End Function

        Public Function SendMailPlayer(ByVal psSendAll As String) As Boolean
            Dim lstPlayer As List(Of String)
            Dim bInsertSuccessfull As Boolean = False
            Dim oEmailsManager As CEmailsManager = New CEmailsManager()
            If psSendAll.Equals("ALL", StringComparison.CurrentCultureIgnoreCase) OrElse (ddlPlayer.SelectedValue.Equals("ALL", StringComparison.CurrentCultureIgnoreCase) AndAlso ddlAgent.SelectedValue.Equals("ALL", StringComparison.CurrentCultureIgnoreCase)) Then
                lstPlayer = ListPlayer
                For Each sPlayerID As String In lstPlayer
                    bInsertSuccessfull = oEmailsManager.InsertEmail(UserSession.UserID, UserSession.UserID, sPlayerID, GetSiteType, ddlEmailSubject.SelectedValue, fckMessage.Text, txtReplyToAddress.Text, "Y")
                    If Not bInsertSuccessfull Then
                        Return bInsertSuccessfull
                    End If
                Next
            ElseIf Not ddlAgent.SelectedValue.Equals("ALL", StringComparison.CurrentCultureIgnoreCase) AndAlso ddlPlayer.SelectedValue.Equals("ALL") Then
                For Each odr As DataRow In (New CPlayerManager()).GetPlayers(SafeString(ddlAgent.SelectedValue), False).Rows
                    bInsertSuccessfull = oEmailsManager.InsertEmail(UserSession.UserID, UserSession.UserID, SafeString(odr("PlayerID")), GetSiteType, ddlEmailSubject.SelectedValue, fckMessage.Text, txtReplyToAddress.Text, "Y")
                    If Not bInsertSuccessfull Then
                        Return bInsertSuccessfull
                    End If
                Next
            Else
                bInsertSuccessfull = oEmailsManager.InsertEmail(UserSession.UserID, UserSession.UserID, ddlPlayer.SelectedValue, GetSiteType, ddlEmailSubject.SelectedValue, fckMessage.Text, txtReplyToAddress.Text, "Y")
            End If
            Return bInsertSuccessfull
        End Function

        Private Function loadAgents(ByVal psSuperID As String) As DataTable
            Dim odtAgents As New DataTable
            odtAgents.Columns.Add("AgentID", GetType(Guid))
            odtAgents.Columns.Add("AgentName", GetType(String))
            odtAgents.Columns.Add("Login", GetType(String))
            odtAgents.Columns.Add("IsLocked", GetType(String))
            odtAgents.Columns.Add("IsBettingLocked", GetType(String))
            odtAgents.Columns.Add("LastLoginDate", GetType(DateTime))

            Dim oAgentManager As New CAgentManager
            Dim dtParents As DataTable = oAgentManager.GetAllAgentsBySuperAdminID(psSuperID, Nothing)

            Dim odrParents As DataRow() = dtParents.Select("ParentID IS NULL", "AgentName")

            If odrParents.Length = 0 Then
                odrParents = dtParents.Select("ParentID IS NOT NULL", "AgentName")
            End If

            For Each drParent As DataRow In odrParents
                Dim odrAgent As DataRow = odtAgents.NewRow
                odtAgents.Rows.Add(odrAgent)

                odrAgent("AgentID") = drParent("AgentID")
                odrAgent("AgentName") = loopString("----", SafeInteger(drParent("AgentLevel")) - 1) & SafeString(drParent("AgentName"))
                odrAgent("Login") = drParent("Login")
                odrAgent("IsLocked") = drParent("IsLocked")
                odrAgent("IsBettingLocked") = drParent("IsBettingLocked")
                odrAgent("LastLoginDate") = drParent("LastLoginDate")
                loadSubAgent(SafeString(drParent("AgentID")), dtParents, odtAgents)
            Next

            Return odtAgents
        End Function

        Public Sub loadSubAgent(ByVal psParentAgentID As String, ByVal podtParents As DataTable, ByRef odtAgents As DataTable)
            Dim odrSubAgents As DataRow() = podtParents.Select("ParentID=" & SQLString(psParentAgentID), "AgentName")
            For Each drChild As DataRow In odrSubAgents
                Dim odrAgent As DataRow = odtAgents.NewRow
                odtAgents.Rows.Add(odrAgent)

                odrAgent("AgentID") = drChild("AgentID")
                odrAgent("AgentName") = loopString("----", SafeInteger(drChild("AgentLevel")) - 1) & SafeString(drChild("AgentName"))
                odrAgent("Login") = drChild("Login")
                odrAgent("IsLocked") = drChild("IsLocked")
                odrAgent("IsBettingLocked") = drChild("IsBettingLocked")
                odrAgent("LastLoginDate") = drChild("LastLoginDate")

                loadSubAgent(SafeString(drChild("AgentID")), podtParents, odtAgents)
            Next
        End Sub

        Private Function loopString(ByVal psSource As String, ByVal pnLoop As Integer) As String
            Dim sLoop As String = ""
            For nIndex As Integer = 0 To pnLoop - 1
                sLoop &= psSource
            Next
            Return sLoop & " "
        End Function

    End Class
End Namespace

