Imports SBCBL.CacheUtils
Imports SBCBL.CEnums
Imports SBCBL.Managers
Imports SBCBL.std
Imports System.Data

Namespace SBCWebsite

    Partial Class Inc_Transactions
        Inherits SBCBL.UI.CSBCUserControl

        Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

        Private _nRowCount As Integer = 0
        Private _nTotalAmountOwed As Double = 0
        Private _nTotalAmountPaid As Double = 0

        Public ReadOnly Property UserID() As String
            Get
                If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                    If UserSession.SuperAdminInfo.IsPartner Then
                        ' Return main SuperAdminID
                        Return UserSession.SuperAdminInfo.PartnerOf
                    End If
                End If

                Return UserSession.UserID
            End Get
        End Property

#Region "Properties"
        Public Property UserType() As ETransactionUserType
            Get
                If SBCBL.std.SafeString(ViewState("_USER_TYPE")) = "" Then
                    Return ETransactionUserType.Agent
                End If

                Return CType(ViewState("_USER_TYPE"), ETransactionUserType)
            End Get
            Set(ByVal value As ETransactionUserType)
                ViewState("_USER_TYPE") = value
            End Set
        End Property

        Public Property ViewPlayerTransaction() As Boolean
            Get
                Return SBCBL.std.SafeBoolean(ViewState("_PLAYER_TRANSACTIONS"))
            End Get
            Set(ByVal value As Boolean)
                ViewState("_PLAYER_TRANSACTIONS") = value
            End Set
        End Property
#End Region

#Region "Page's events"
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                bindUsers()
                bindUserAmounts(UserID, UserType)
            End If
        End Sub

        Protected Sub ddlUsers_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlUsers.SelectedIndexChanged
            If ddlUsers.Value <> "" Then
                If ViewPlayerTransaction Then
                    Dim oPlayerManger As New SBCBL.Managers.CPlayerManager()
                    ddlPlayer.DataSource = oPlayerManger.GetPlayers(ddlUsers.Value, Nothing)
                    ddlPlayer.DataTextField = "FullName"
                    ddlPlayer.DataValueField = "PlayerID"
                    ddlPlayer.DataBind()

                    bindUserAmounts(ddlUsers.Value, ETransactionUserType.Agent)
                Else
                    If UserType = ETransactionUserType.Agent Then
                        loadUserInfo(ddlUsers.Value)
                    End If
                    bindTransactions(UserID, ddlUsers.Value)
                End If

            Else
                lblCurrentBalance.Text = ""
                ddlPlayer.Items.Clear()
                ddlPlayer.DataSource = Nothing
                ddlPlayer.DataBind()
                bindUserAmounts(UserID, UserType)
            End If

            clearUserInfo()
        End Sub

        Protected Sub ddlPlayer_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlPlayer.SelectedIndexChanged
            lblCurrentBalance.Text = ""
            If ddlPlayer.Value <> "" Then
                loadUserInfo(ddlPlayer.Value)
                If ddlUsers.Visible Then
                    bindTransactions(ddlUsers.Value, ddlPlayer.Value)
                Else
                    bindTransactions(UserID, ddlPlayer.Value)
                End If
            Else
                clearUserInfo()
                bindUserAmounts(UserID, UserType)
            End If

        End Sub

        Protected Sub grdTransaction_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles grdTransaction.ItemDataBound
            If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
                Dim oDr As Data.DataRowView = CType(e.Item.DataItem, Data.DataRowView)


                Dim nAmountOwed, nAmountPaid As Double
                nAmountOwed = SBCBL.std.SafeRound(oDr("AmountOwed"))
                nAmountPaid = SBCBL.std.SafeRound(oDr("TransactionAmount"))

                Dim lblAmountOwed, lblAmountPaid As Literal
                lblAmountOwed = CType(e.Item.FindControl("lblAmountOwed"), Literal)
                lblAmountPaid = CType(e.Item.FindControl("lblAmountPaid"), Literal)

                If e.Item.ItemIndex <> _nRowCount - 1 Then
                    If nAmountOwed < 0 Then
                        lblAmountOwed.Text = String.Format("<label style=""color: Red;"">{0}</label>", FormatNumber(nAmountOwed, SBCBL.std.GetRoundMidPoint))
                    Else
                        lblAmountOwed.Text = FormatNumber(nAmountOwed, SBCBL.std.GetRoundMidPoint)
                    End If

                    If nAmountPaid < 0 Then
                        lblAmountPaid.Text = String.Format("<label style=""color: Red;"">{0}</label>", FormatNumber(nAmountPaid, SBCBL.std.GetRoundMidPoint))
                    Else
                        lblAmountPaid.Text = FormatNumber(nAmountPaid, SBCBL.std.GetRoundMidPoint)
                    End If

                Else
                    If _nTotalAmountOwed < 0 Then
                        lblAmountOwed.Text = String.Format("<label style=""color: Red;"">{0}</label>", FormatNumber(_nTotalAmountOwed, SBCBL.std.GetRoundMidPoint))
                    Else
                        lblAmountOwed.Text = FormatNumber(_nTotalAmountOwed, SBCBL.std.GetRoundMidPoint)
                    End If

                    If _nTotalAmountPaid < 0 Then
                        lblAmountPaid.Text = String.Format("<label style=""color: Red;"">{0}</label>", FormatNumber(_nTotalAmountPaid, SBCBL.std.GetRoundMidPoint))
                    Else
                        lblAmountPaid.Text = FormatNumber(_nTotalAmountPaid, SBCBL.std.GetRoundMidPoint)
                    End If

                    e.Item.Cells(0).ColumnSpan = 2
                    e.Item.Cells(0).Font.Bold = True
                    e.Item.Cells(1).Visible = False
                    e.Item.Cells(2).Font.Bold = True
                    e.Item.Cells(3).Font.Bold = True
                    e.Item.Cells(4).Font.Bold = True
                End If

                _nTotalAmountOwed += nAmountOwed
                _nTotalAmountPaid += nAmountPaid
            End If
        End Sub

        Protected Sub dgTransaction_PageIndexChanged(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles grdTransaction.PageIndexChanged
            grdTransaction.CurrentPageIndex = e.NewPageIndex

            If ddlUsers.Value = "" Then
                bindUserAmounts(UserID, UserType)
            Else
                If ddlPlayer.Value = "" Then
                    bindUserAmounts(ddlUsers.Value, ETransactionUserType.Agent)
                Else
                    bindTransactions(ddlUsers.Value, ddlUsers.Value)
                End If

            End If
        End Sub

        Protected Sub btnReset_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnReset.Click
            Application.Lock()
            Try
                If validate() Then
                    Dim oPlayer As CPlayer = UserSession.Cache.GetPlayerInfo(ddlPlayer.Value)

                    If oPlayer IsNot Nothing Then
                        Dim oMng As New SBCBL.Managers.CPlayerManager()
                        If oMng.ResetOriginalAmount(ddlPlayer.Value, oPlayer.Template.AccountBalance, UserSession.UserID) Then
                            SBCBL.std.ClientAlert("Successfully Reset", True)
                            UserSession.Cache.ClearPlayerInfo(ddlPlayer.Value)
                            loadUserInfo(ddlPlayer.Value)
                        End If
                    End If
                End If
            Finally
                Application.UnLock()
            End Try

        End Sub

        Protected Sub btnResetZero_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnResetZero.Click
            Application.Lock()
            Try
                If validate() Then
                    Dim oPlayer As CPlayer = UserSession.Cache.GetPlayerInfo(ddlPlayer.Value)

                    If oPlayer IsNot Nothing Then
                        Dim oMng As New SBCBL.Managers.CPlayerManager()
                        If oMng.ResetOriginalAmount(ddlPlayer.Value, 0, UserSession.UserID) Then
                            SBCBL.std.ClientAlert("Successfully Reset", True)
                            UserSession.Cache.ClearPlayerInfo(ddlPlayer.Value)
                            loadUserInfo(ddlPlayer.Value)
                        End If
                    End If
                End If
            Finally
                Application.UnLock()
            End Try
        End Sub
#End Region

#Region "Bind Data"
        Private Sub bindPAgents()
            Dim dtParents As DataTable = Nothing

            If UserType = ETransactionUserType.SuperAdmin Then
                ddlUsers.Items.Clear()
                If Not UserSession.SuperAdminInfo.IsPartner Then
                    dtParents = (New CAgentManager).GetAllAgentsBySuperAdminID(UserID, Nothing)
                Else
                    dtParents = (New CAgentManager).GetAllAgentsBySuperAdminID(UserSession.SuperAdminInfo.PartnerOf, Nothing)
                End If

                Dim odrParents As DataRow() = dtParents.Select("ParentID IS NULL", "AgentName")

                For Each drParent As DataRow In odrParents
                    Dim sAgentID As String = SafeString(drParent("AgentID"))
                    ddlUsers.Items.Add(New ListItem(SafeString(drParent("AgentName")), sAgentID))

                    loadSubAgent(sAgentID, dtParents)
                Next

                ddlUsers.Items.Insert(0, New ListItem("All", ""))
            End If
        End Sub

        Public Sub loadSubAgent(ByVal psParentAgentID As String, ByVal podtAgents As DataTable)
            Dim odrSubAgents As DataRow() = podtAgents.Select("ParentID=" & SQLString(psParentAgentID), "AgentName")

            For Each drChild As DataRow In odrSubAgents
                Dim sText As String = loopString("----", SafeInteger(drChild("AgentLevel")) - 1) & SafeString(drChild("AgentName"))
                Dim sAgentID As String = SafeString(drChild("AgentID"))

                ddlUsers.Items.Add(New ListItem(sText, sAgentID))

                loadSubAgent(sAgentID, podtAgents)
            Next
        End Sub

        Private Function loopString(ByVal psSource As String, ByVal pnLoop As Integer) As String
            Dim sLoop As String = ""

            For nIndex As Integer = 0 To pnLoop - 1
                sLoop &= psSource
            Next

            Return sLoop
        End Function

        Private Sub bindUsers()
            Select Case UserType
                Case ETransactionUserType.Agent '' Get list of Players
                    Dim oMng As New SBCBL.Managers.CPlayerManager()
                    ddlUsers.DataSource = oMng.GetPlayers(UserID, Nothing)
                    ddlUsers.DataValueField = "PlayerID"
                    lblUser.Text = "Player: "
                    lblUserInfo.Text = "Player's Info"
                    lblDispCurrentBalance.Text = "Current Balance: "

                    ddlPlayer.DataSource = oMng.GetPlayers(UserID, Nothing)
                    ddlPlayer.DataValueField = "PlayerID"
                    ddlPlayer.DataTextField = "FullName"
                    ddlPlayer.DataBind()

                    tdUserInfoDisp.Visible = True
                    tdUserInfo.Visible = True
                    lblUser.Visible = False
                    ddlUsers.Visible = False

                    grdTransaction.Columns(2).HeaderText = "Amount Owed"
                    grdTransaction.Columns(3).HeaderText = "Amount Paid"

                Case Else
                    lblUser.Text = "Agent: "

                    If ViewPlayerTransaction Then '' Get list of all Agents, subagents
                        bindPAgents()
                    Else '' Get list of Agent only
                        Dim oMng As New SBCBL.Managers.CAgentManager()
                        If UserType = ETransactionUserType.SuperAgent Then
                            ddlUsers.DataSource = oMng.GetAgentsByAgentID(UserID, Nothing)
                        ElseIf UserType = ETransactionUserType.SuperAdmin AndAlso Not UserSession.SuperAdminInfo.IsPartner Then
                            ddlUsers.DataSource = oMng.GetAgentsBySuperID(UserID, Nothing)
                        Else
                            ddlUsers.DataSource = oMng.GetAgentsBySuperID(UserSession.SuperAdminInfo.PartnerOf, Nothing)
                        End If
                        ddlUsers.DataValueField = "AgentID"
                    End If

                    lblUserInfo.Text = ""

                    If ViewPlayerTransaction Then
                        lblUserInfo.Text = "Player's Info"
                        lblDispCurrentBalance.Text = "Current Balance: "
                        grdTransaction.Columns(2).HeaderText = "Amount Owed"
                        grdTransaction.Columns(3).HeaderText = "Amount Paid"
                        ddlPlayer.DataSource = Nothing
                        ddlPlayer.DataBind()
                    Else
                        grdTransaction.Columns(2).HeaderText = "Total P/L"
                        grdTransaction.Columns(3).HeaderText = "Total Paid"

                    End If

                    tdUserInfoDisp.Visible = ViewPlayerTransaction
                    tdUserInfo.Visible = ViewPlayerTransaction
                    lblDispCurrentBalance.Visible = ViewPlayerTransaction
                    lblCurrentBalance.Visible = ViewPlayerTransaction

            End Select

            If Not ViewPlayerTransaction Then
                ddlUsers.DataTextField = "FullName"
                ddlUsers.DataBind()
            End If

        End Sub

        Private Sub bindTransactions(ByVal psPaymentID As String, ByVal psWithdrawID As String)
            Dim oMng As New SBCBL.Managers.CTransactionManager()
            grdTransaction.DataSource = oMng.GetTransactions(psPaymentID, psWithdrawID)
            grdTransaction.Columns(0).Visible = False
            grdTransaction.DataBind()
        End Sub

        Private Sub bindUserAmounts(ByVal psUserID As String, ByVal psUserType As ETransactionUserType)
            Dim oMng As New SBCBL.Managers.CTransactionManager()

            Dim oTbl As Data.DataTable = oMng.GetUsersAmount(psUserID, psUserType)
            If oTbl IsNot Nothing AndAlso oTbl.Rows.Count > 0 Then
                Dim odrTotal As Data.DataRow = oTbl.NewRow
                odrTotal("FullName") = "Total"
                oTbl.Rows.Add(odrTotal)

                _nRowCount = oTbl.Rows.Count
            End If

            grdTransaction.DataSource = oTbl
            grdTransaction.Columns(0).Visible = True
            grdTransaction.DataBind()

        End Sub
#End Region

#Region "Methods"
        Private Sub loadUserInfo(ByVal psPlayerID As String)
            Dim oPlayer As CPlayer = UserSession.Cache.GetPlayerInfo(psPlayerID)

            If oPlayer IsNot Nothing Then
                '' Transaction's Info
                lblCurrentBalance.Text = FormatNumber(SBCBL.std.SafeRound(oPlayer.BalanceAmount), SBCBL.std.GetRoundMidPoint)

                btnReset.Text = "Reset To Original Balance: " & FormatNumber(SBCBL.std.SafeRound(oPlayer.Template.AccountBalance), SBCBL.std.GetRoundMidPoint)

                btnReset.Enabled = oPlayer.OriginalAmount = 0
                btnResetZero.Enabled = oPlayer.OriginalAmount > 0
            End If

            btnReset.Visible = oPlayer IsNot Nothing
            btnResetZero.Visible = oPlayer IsNot Nothing
        End Sub

        Private Sub clearUserInfo()
            '' Player's Info
            btnReset.Text = "Reset To Original Balance: "
            btnReset.Visible = False
            btnResetZero.Visible = False
        End Sub

        Private Function validate(Optional ByVal pbProcess As Boolean = False) As Boolean
            If ddlUsers.Value = "" Then
                SBCBL.std.ClientAlert("Please Choose User", True)
                ddlUsers.Focus()
                Return False
            End If

            Return True
        End Function
#End Region

    End Class
End Namespace