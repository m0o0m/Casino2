Imports System.Data
Imports SBCBL.Security
Imports SBCBL.CEnums
Imports SBCBL.std
Imports SBCBL.Utils
Imports WebsiteLibrary.DBUtils
Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports SBCBL

Namespace SBSAgents
    Partial Class PlayerManager
        Inherits SBCBL.UI.CSBCUserControl
        Dim log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

#Region "Properties"

        'Public WriteOnly Property AgentID() As String
        '    Set(ByVal value As String)
        '        ddlAgents.Value = value
        '    End Set
        'End Property

        Public Property AgentID() As String
            Get
                Return ddlAgents.SelectedValue
            End Get
            Set(ByVal value As String)
                ddlAgents.Value = value
            End Set
        End Property

        Property PlayerTemplateEditID() As String
            Get
                If ViewState("PlayerTemplateEditID") Is Nothing Then
                    Return ""
                Else
                    Return SafeString(ViewState("PlayerTemplateEditID"))
                End If
            End Get
            Set(ByVal value As String)
                ViewState("PlayerTemplateEditID") = value
            End Set
        End Property

        Property PlayerID() As String
            Get
                If ViewState("PlayerID") Is Nothing Then
                    Return ""
                Else
                    Return SafeString(ViewState("PlayerID"))
                End If
            End Get
            Set(ByVal value As String)
                ViewState("PlayerID") = value
            End Set
        End Property

        Public Property PendingPage() As String
            Get
                Return SafeString(ViewState("_PENDING_PAGE"))
            End Get
            Set(ByVal value As String)
                ViewState("_PENDING_PAGE") = value
            End Set
        End Property
#End Region

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                bindAgents()
                ddlAgents.SelectedIndex = 1
                rdAllAcct.Checked = True
                bindPlayers()

                pnPlayerManager.Visible = True
                ucPlayerEdit.Visible = False

                If UserSession.UserType = EUserType.Agent Then
                    rdLockAcct.Visible = SafeBoolean(UserSession.AgentUserInfo.IsEnableBlockPlayer)
                    rdLockBetAcct.Visible = SafeBoolean(UserSession.AgentUserInfo.IsEnableBlockPlayer)
                    lblLockBetAcct.Visible = SafeBoolean(UserSession.AgentUserInfo.IsEnableBlockPlayer)
                    lblLockAcct.Visible = SafeBoolean(UserSession.AgentUserInfo.IsEnableBlockPlayer)
                End If
                bindTemplates()
            End If

        End Sub

#Region "Bind data"

        Private Sub bindTemplates()
            ddlplayerTemplate.Items.Clear()
            ddlplayerTemplate.DataSource = (New CPlayerTemplateManager).GetPlayerTemplates(SBCBL.std.GetSiteType)
            ddlplayerTemplate.DataTextField = "TemplateName"
            ddlplayerTemplate.DataValueField = "PlayerTemplateID"
            ddlplayerTemplate.DataBind()
        End Sub

        Private Sub bindPlayers()
            Dim oMng As New CPlayerManager
            If rdAllAcct.Checked Then
                dgPlayers.DataSource = oMng.GetPlayersByAgentID(ddlAgents.Value)
            Else
                dgPlayers.DataSource = oMng.GetPlayers(ddlAgents.Value, rdLockBetAcct.Checked, rdLockAcct.Checked, txtNameOrLogin.Text)
            End If

            dgPlayers.DataBind()
        End Sub

        Private Sub bindAgents()
            ddlAgents.DataSource = loadAgents()
            ddlAgents.DataTextField = "AgentName"
            ddlAgents.DataValueField = "AgentID"
            ddlAgents.DataBind()
        End Sub

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

        Private Function loadAgents() As DataTable
            Dim odtAgents As New DataTable
            odtAgents.Columns.Add("AgentID", GetType(Guid))
            odtAgents.Columns.Add("AgentName", GetType(String))
            odtAgents.Columns.Add("Login", GetType(String))
            odtAgents.Columns.Add("IsLocked", GetType(String))
            odtAgents.Columns.Add("IsBettingLocked", GetType(String))
            odtAgents.Columns.Add("LastLoginDate", GetType(DateTime))
            ''parent
            Dim sParentName As String = String.Format("{0} ({1})", UserSession.AgentUserInfo.Login, UserSession.AgentUserInfo.Name)
            odtAgents.Rows.Add(New Object() {UserSession.UserID, sParentName})
            Dim oAgentManager As New CAgentManager
            Dim dtParents As DataTable = oAgentManager.GetAllAgentsByAgent(UserSession.UserID, Nothing)
            Dim nMinAgentLevel As Integer = SafeInteger(dtParents.Compute("MIN(AgentLevel)", ""))
            Dim odrParents As DataRow() = dtParents.Select("AgentLevel=" & SafeString(nMinAgentLevel), "AgentName")
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

        Private Function loopString(ByVal psSource As String, ByVal pnLoop As Integer) As String
            Dim sLoop As String = ""
            For nIndex As Integer = 0 To pnLoop - 1
                sLoop &= psSource
            Next
            Return sLoop & " "
        End Function
#End Region
        Protected Sub btnFilter_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFilter.Click
            Dim oPlayersManager As New CPlayerManager()
            If txtNameOrLogin.Text = "" Then
                'ClientAlert("Please input name or log in to filter", True)
                'txtNameOrLogin.Focus()
                Dim odtPlayersSource = oPlayersManager.GetPlayersByAgentID(SafeString(ddlAgents.SelectedItem.Value))
                dgPlayers.DataSource = odtPlayersSource
                dgPlayers.DataBind()
            Else

                Dim odtPlayersSource = oPlayersManager.GetPlayerByKeywords(SafeString(txtNameOrLogin.Text), ddlAgents.SelectedValue)
                If odtPlayersSource.Rows.Count > 0 Then
                    dgPlayers.Visible = True
                    lblmsg.Visible = False
                    dgPlayers.DataSource = odtPlayersSource
                    dgPlayers.DataBind()
                Else
                    dgPlayers.Visible = False
                    lblmsg.Visible = True
                    txtNameOrLogin.Focus()
                End If


            End If
        End Sub

        Protected Sub chbLockAcct_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rdAllAcct.CheckedChanged, rdLockAcct.CheckedChanged, rdLockBetAcct.CheckedChanged
            bindPlayers()
        End Sub

        Protected Sub dgPlayers_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgPlayers.ItemCommand
           
            Dim oPlayerIDAndLogins As New List(Of String)
            Dim oPlayerManager As New CPlayerManager
            Select Case UCase(e.CommandName)
                Case "EDITUSER"
                   
                    If Not ucPlayerEdit.LoadPlayerInfo(SafeString(e.CommandArgument)) Then
                        ClientAlert("Can't Load Player's Information", True)
                        ucPlayerEdit.ResetPlayerInfor()
                        Return
                    End If

                    pnPlayerManager.Visible = False
                    PlayerEdit.Visible = True
                    ucPlayerEdit.Visible = True

                    dgPlayers.SelectedIndex = e.Item.ItemIndex
                    ''Edit template User
                    If UserSession.AgentUserInfo.IsEnableBettingProfile Then
                        PlayerID = e.CommandArgument.Split("|")(0)
                        Dim hfPlayerTemplateID = CType(e.Item.FindControl("hfPlayerTemplateID"), HiddenField)
                        dgPlayers.SelectedIndex = e.Item.ItemIndex
                        ''load template player
                        Dim arrTemplateID As String() = hfPlayerTemplateID.Value.Split("|")
                        If String.IsNullOrEmpty(arrTemplateID(0)) Then
                            PlayerTemplateEditID = arrTemplateID(1)
                        Else
                            PlayerTemplateEditID = arrTemplateID(0)
                        End If
                    End If
                Case "LOCK"
                    oPlayerIDAndLogins.Add(e.CommandArgument)
                    Dim hfLock As HiddenField = CType(e.Item.FindControl("hfLock"), HiddenField)
                    oPlayerManager.SetLockPlayers(oPlayerIDAndLogins, IIf(hfLock.Value.Equals("Y", StringComparison.CurrentCultureIgnoreCase), False, True))
                    bindPlayers()
                Case "BETTING LOCK"
                    oPlayerIDAndLogins.Add(e.CommandArgument)
                    Dim hfBettingLock As HiddenField = CType(e.Item.FindControl("hfBettingLock"), HiddenField)
                    oPlayerManager.SetBettingLockPlayers(oPlayerIDAndLogins, IIf(hfBettingLock.Value.Equals("Y", StringComparison.CurrentCultureIgnoreCase), False, True))
                    bindPlayers()
            End Select
        End Sub

        'Protected Sub btnAddNewPlayer_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddNewPlayer.Click
        Private Sub CreateNewAcct()
            pnPlayerManager.Visible = False
            PlayerEdit.Visible = True
            ucPlayerEdit.Visible = True
            ucPlayerEdit.DisnableTemplate()
            ucPlayerEdit.ClearLimits()
            ucPlayerEdit.bindNextPlayer()
            '  ucPlayerEdit.ShowLimitsInfo()
        End Sub
       
        'End Sub

        Protected Sub dgPlayers_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgPlayers.ItemDataBound
            If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
                Dim oCacheManager As CCacheManager = New CCacheManager()
                Dim sUserID As String
                Dim sLink As String = ""

                Dim oData As DataRowView = CType(e.Item.DataItem, DataRowView)

                Dim lbnLock, lbnBettingLock As LinkButton
                lbnLock = CType(e.Item.FindControl("lbnLock"), LinkButton)
                lbnBettingLock = CType(e.Item.FindControl("lbnBettingLock"), LinkButton)
                Dim lblLimit As Label = CType(e.Item.FindControl("lblLimit"), Label)
                Dim lblBalance As Label = CType(e.Item.FindControl("lblBalance"), Label)
                Dim lblPending As Label = CType(e.Item.FindControl("lblPending"), Label)
                sUserID = SafeString(CType(e.Item.DataItem, DataRowView)("PlayerID"))
                lblLimit.Text = FormatNumber(oCacheManager.GetPlayerInfo(sUserID, False).Template.CreditMaxAmount, SBCBL.std.GetRoundMidPoint())
                lblBalance.Text = FormatNumber(oCacheManager.GetPlayerInfo(sUserID, False).BalanceAmount, SBCBL.std.GetRoundMidPoint())
                lblPending.Text = FormatNumber(oCacheManager.GetPlayerInfo(sUserID, False).PendingAmount, SBCBL.std.GetRoundMidPoint())
                If SafeDouble(lblPending.Text) < 0 Then
                    lblPending.ForeColor = Drawing.Color.Red
                Else
                    lblPending.ForeColor = Drawing.Color.Blue
                End If

                'set link for pending
                If PendingPage <> "" Then
                    sLink = String.Format("window.location='{0}?AgentID={1}&PlayerID={2}'", PendingPage, SafeString(AgentID), SafeString(oData("PlayerID")))

                    If SafeDouble(lblPending.Text) <> 0 Then
                        lblPending.CssClass = "hyperlink"
                        lblPending.Attributes.Add("onClick", sLink)

                    End If
                End If

                lbnBettingLock.Visible = SafeBoolean(UserSession.AgentUserInfo.IsEnableBlockPlayer)
                lbnLock.Visible = SafeBoolean(UserSession.AgentUserInfo.IsEnableBlockPlayer)
            End If
        End Sub

        'format date
        Public Function formatDate(ByVal obj As Object) As String

            Dim dDate As Date?
            If Not String.IsNullOrEmpty(SafeString(obj)) Then
                dDate = SafeDate(obj)
            End If

            If dDate.HasValue Then
                Return dDate.Value.ToString("MM/dd/yyyy")
            End If
            Return String.Empty
        End Function


#Region "PlayerTemplate"
        'Private Function saveTemplate(ByVal poTemplate As CPlayerTemplate) As Boolean
        '    Dim oTemplateMng As New CPlayerTemplateManager
        '    ''set player template id
        '    If PlayerTemplateEditID <> "" Then
        '        poTemplate.PlayerTemplateID = PlayerTemplateEditID
        '    End If
        '    ''save template
        '    Dim bSuccess As Boolean = False
        '    Dim oPlayer = (New CPlayerManager()).GetPlayer(PlayerID)
        '    If Not String.IsNullOrEmpty(oPlayer.PlayerTemplateID) Then
        '        bSuccess = oTemplateMng.UpdatePlayerTemplate(poTemplate, "N")
        '    Else
        '        poTemplate.PlayerTemplateID = newGUID()
        '        bSuccess = oTemplateMng.InsertPlayerTemplate(poTemplate, SBCBL.std.GetSiteType, "N")
        '        If bSuccess Then
        '            bSuccess = (New CPlayerManager).UpdatePlayerTemplateID(PlayerID, poTemplate.PlayerTemplateID, oPlayer.Login, UserSession.UserID)
        '        End If
        '        If bSuccess Then
        '            PlayerTemplateEditID = poTemplate.PlayerTemplateID
        '        End If
        '        bindPlayers()
        '    End If
        '    If Not bSuccess Then
        '        ClientAlert("Can't Save", True)
        '        Return False
        '    End If
        '    ClientAlert("Successfully Saved", True)
        '    Return True
        'End Function
#End Region

        Protected Sub btnreset_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnreset.Click
            txtNameOrLogin.Text = ""
            lblmsg.Visible = False
            txtNameOrLogin.Focus()
        End Sub

        Protected Sub ucPlayerEdit_ButtonClick(ByVal sButtonType As String) Handles ucPlayerEdit.ButtonClick
            dgPlayers.SelectedIndex = -1
            ucPlayerEdit.ResetPlayerInfor()
            Select Case UCase(sButtonType)
                Case "SAVE UNSUCCESSFUL"
                    ClientAlert("Failed to Save Setting", True)
                Case "SAVE SUCCESSFUL"
                    bindPlayers()
            End Select
            pnPlayerManager.Visible = True
            ucPlayerEdit.Visible = False
            PlayerEdit.Visible = False
        End Sub

       
        Protected Sub dgPlayers_PageIndexChanged(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles dgPlayers.PageIndexChanged
            dgPlayers.CurrentPageIndex = e.NewPageIndex
            bindPlayers()
        End Sub


        Private Function checkCreateNewGroupPlayer() As Boolean
            If ddlplayerTemplate.SelectedValue = "" Then
                ClientAlert("Please select preset player template.", True)
                Return False
            End If
            Return True
        End Function

        Protected Sub btnCreate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreate.Click

            'If rdlNumAcc.SelectedValue = "1" Then
            '    CreateNewAcct()
            '    Return
            'End If

            If checkCreateNewGroupPlayer() Then
                Dim sPresetAgentID As String = SafeString(ddlAgents.SelectedValue)
                If String.IsNullOrEmpty(sPresetAgentID) Then
                    ClientAlert("Please, select Agent", True)
                    Return
                End If
                Dim nCurrentPlayer As Integer = UserSession.Cache.GetAgentInfo(sPresetAgentID).CurrentPlayerNumber
                Dim oPlayerManager As New CPlayerManager()
                Dim bPresetSuccess = oPlayerManager.CreatePresetPlayers(SafeString(sPresetAgentID), SafeInteger(rdlNumAcc.SelectedValue), UserSession.Cache.GetAgentInfo(sPresetAgentID).SpecialKey, _
                                nCurrentPlayer + 1, 1, _
                                ddlplayerTemplate.SelectedValue, UserSession.UserID, std.GetSiteType.ToString(), True)
                If (bPresetSuccess) Then
                    Dim oAgent As New CAgentManager
                    oAgent.IncreaseCurrentPlayerNumber(sPresetAgentID, nCurrentPlayer + SafeInteger(rdlNumAcc.SelectedValue))
                    UserSession.Cache.ClearAgentInfo(sPresetAgentID, SafeString(UserSession.Cache.GetAgentInfo(sPresetAgentID).Login))
                    ddlplayerTemplate.SelectedIndex = 0
                    ClientAlert("Create successful", True)
                End If
            End If
            bindPlayers()
        End Sub



    End Class
End Namespace

