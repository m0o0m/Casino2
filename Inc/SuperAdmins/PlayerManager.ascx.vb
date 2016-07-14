Imports SBCBL.std
Imports SBCBL.Managers
Imports System.Data
Imports SBCBL
Imports SBCBL.CacheUtils

Namespace SBCSuperAdmin
    Partial Class PlayerManager
        Inherits SBCBL.UI.CSBCUserControl
        Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

#Region "Properties"
        Public Property ViewLock() As Boolean
            Get
                If ViewState("VIEW_LOCK") Is Nothing Then
                    ViewState("VIEW_LOCK") = False
                End If
                Return CBool(ViewState("VIEW_LOCK"))
            End Get
            Set(ByVal value As Boolean)
                ViewState("VIEW_LOCK") = value
                Dim sMessage As String = SafeString(IIf(value, "UnLock", "Lock"))
                btnLock.Text = sMessage
                btnViewLock.Text = "View " & sMessage
            End Set
        End Property

        Public Property ViewBettingLock() As Boolean
            Get
                If ViewState("VIEW_BETTING_LOCK") Is Nothing Then
                    ViewState("VIEW_BETTING_LOCK") = False
                End If

                Return CBool(ViewState("VIEW_BETTING_LOCK"))
            End Get
            Set(ByVal value As Boolean)
                ViewState("VIEW_BETTING_LOCK") = value

                Dim sMessage As String = SafeString(IIf(value, "Unlock", "Lock"))
                btnBettingLock.Text = "Betting " & sMessage
                btnViewBettingLock.Text = "View Betting " & sMessage
            End Set
        End Property

        Property SiteType() As CEnums.ESiteType
            Get
                Return SafeString(ViewState("SiteType"))
            End Get
            Set(ByVal value As CEnums.ESiteType)
                ViewState("SiteType") = value
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

#End Region

#Region "Page Event"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                bindAgents()
                bindTemplates()
                Dim sAgentID As String = SafeString(Request("AgentID"))
                ddlAgents.Value = sAgentID
                PlayerEdit1.AgentID = sAgentID
                bindPlayers()
                EnabledButton(True)
            End If
        End Sub
#End Region

#Region "Bind data"


        Private Sub bindAgents()
            Dim dtParents As DataTable = (New CAgentManager).GetAllAgentsBySuperAdminID(UserSession.UserID, Nothing)

            Dim odrParents As DataRow() = dtParents.Select("ParentID IS NULL", "AgentName")

            For Each drParent As DataRow In odrParents
                Dim sAgentID As String = SafeString(drParent("AgentID"))
                ddlAgents.Items.Add(New ListItem(SafeString(drParent("AgentName")), sAgentID))

                loadSubAgent(sAgentID, dtParents)
            Next

        End Sub

        Public Sub loadSubAgent(ByVal psParentAgentID As String, ByVal podtAgents As DataTable)
            Dim odrSubAgents As DataRow() = podtAgents.Select("ParentID=" & SQLString(psParentAgentID), "AgentName")

            For Each drChild As DataRow In odrSubAgents
                Dim sText As String = loopString("----", SafeInteger(drChild("AgentLevel")) - 1) & SafeString(drChild("AgentName"))
                Dim sAgentID As String = SafeString(drChild("AgentID"))

                ddlAgents.Items.Add(New ListItem(sText, sAgentID))

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

        Private Sub bindTemplates()
            ddlTemplates.DataSource = (New CPlayerTemplateManager).GetPlayerTemplates(SBCBL.std.GetSiteType)
            ddlTemplates.DataTextField = "TemplateName"
            ddlTemplates.DataValueField = "PlayerTemplateID"
            ddlTemplates.DataBind()

            ddlplayerTemplate.Items.Clear()
            ddlplayerTemplate.DataSource = (New CPlayerTemplateManager).GetPlayerTemplates(SBCBL.std.GetSiteType)
            ddlplayerTemplate.DataTextField = "TemplateName"
            ddlplayerTemplate.DataValueField = "PlayerTemplateID"
            ddlplayerTemplate.DataBind()
        End Sub

        Private Sub bindPlayers()
            Dim oPlayerManager As New CPlayerManager
            If Not String.IsNullOrEmpty(ddlAgents.Value) Then
                dgPlayers.DataSource = oPlayerManager.GetPlayers(ddlAgents.Value, Me.ViewBettingLock, Me.ViewLock, txtNameOrLogin.Text, ddlTemplates.Value)
                dgPlayers.DataBind()
            End If
            
        End Sub

#End Region

#Region "Tabs"

        'Private Sub setTabActive(ByVal psTabKey As String)
        '    lbtPlayerInfo.CssClass = ""
        '    lbnBettingProfile.CssClass = ""
        '    PlayerEdit1.Visible = False
        '    ucEditTemplate.Visible = False
        '    Select Case UCase(psTabKey)
        '        Case "PLAYER_INFO"
        '            lbtPlayerInfo.CssClass = "selected"
        '            PlayerEdit1.Visible = True
        '        Case "BETTING_PROFILE"
        '            lbnBettingProfile.CssClass = "selected"
        '            ucEditTemplate.Visible = True
        '            ''load template player
        '            If Not String.IsNullOrEmpty(PlayerTemplateEditID) Then
        '                Dim oTemplate As CPlayerTemplate = (New CPlayerTemplateManager).GetPlayerTemplate(SafeString(PlayerTemplateEditID))
        '                ucEditTemplate.LoadPlayerTemplate(oTemplate)
        '            Else
        '                ucEditTemplate.ResetPlayerTemplate()
        '            End If


        '    End Select
        'End Sub

        'Protected Sub lbtTab_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        '    setTabActive(CType(sender, LinkButton).CommandArgument)
        'End Sub

#End Region
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

        Protected Sub dgPlayers_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgPlayers.ItemCommand
            Select Case UCase(e.CommandName)
                Case "EDITPLAYER"
                    pnPlayerManager.Visible = False
                    divPlayerEdit.Visible = True
                    PlayerEdit1.Visible = True
                    If Not PlayerEdit1.LoadPlayerInfo(SafeString(e.CommandArgument).Split("|")(0)) Then
                        ClientAlert("Can't Load Player Info", True)
                        PlayerEdit1.ResetPlayerInfor()
                        Return
                    End If
                    EnabledButton(False)
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
                    divPlayerEdit.Visible = True
                    pnPlayerManager.Visible = False
                Case "DELETEPLAYER"
                    Dim oArgs As String() = SafeString(e.CommandArgument).Split("|"c)
                    If (New CPlayerManager).DeletePlayer(oArgs(0), oArgs(1), UserSession.UserID) Then
                        bindPlayers()
                        PlayerEdit1.ResetPlayerInfor()
                    End If
            End Select
        End Sub

        Protected Sub btnViewLock_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnViewLock.Click
            Me.ViewLock = Not Me.ViewLock

            dgPlayers.SelectedIndex = -1
            PlayerEdit1.ResetPlayerInfor()

            bindPlayers()
        End Sub

        Protected Sub btnViewBettingLock_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnViewBettingLock.Click
            Me.ViewBettingLock = Not Me.ViewBettingLock

            dgPlayers.SelectedIndex = -1
            PlayerEdit1.ResetPlayerInfor()

            bindPlayers()
        End Sub

        Private Function getPlayerIDAndLogins() As List(Of String)
            Dim oPlayerIDAndLogins As New List(Of String)

            For Each oItem As DataGridItem In dgPlayers.Items
                Dim chkID As CheckBox = CType(oItem.FindControl("chkID"), CheckBox)
                If chkID.Checked Then
                    oPlayerIDAndLogins.Add(SafeString(chkID.Attributes("value")))
                End If
            Next

            Return oPlayerIDAndLogins
        End Function

        Protected Sub btnLock_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLock.Click
            Dim oPlayerIDAndLogins As List(Of String) = getPlayerIDAndLogins()

            If oPlayerIDAndLogins.Count = 0 Then
                ClientAlert("Select Players To " & LCase(btnLock.Text), True)
                Return
            End If

            If (New CPlayerManager).SetLockPlayers(oPlayerIDAndLogins, Not Me.ViewLock) Then
                dgPlayers.SelectedIndex = -1
                PlayerEdit1.ResetPlayerInfor()
                bindPlayers()
            End If
        End Sub

        Protected Sub btnBettingLock_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnBettingLock.Click
            Dim oPlayerIDAndLogins As List(Of String) = getPlayerIDAndLogins()

            If oPlayerIDAndLogins.Count = 0 Then
                ClientAlert("Select Players To " & LCase(btnBettingLock.Text), True)
                Return
            End If
            log.Debug("List for betting: " & oPlayerIDAndLogins.Item(0))
            If (New CPlayerManager).SetBettingLockPlayers(oPlayerIDAndLogins, Not Me.ViewBettingLock) Then
                dgPlayers.SelectedIndex = -1
                PlayerEdit1.ResetPlayerInfor()

                bindPlayers()
            End If
        End Sub

        Protected Sub btnFilter_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFilter.Click
            '  If txtNameOrLogin.Text <> "" Then
            dgPlayers.SelectedIndex = -1
            PlayerEdit1.ResetPlayerInfor()
            bindPlayers()
            pnPlayerManager.Visible = True
            divPlayerEdit.Visible = False
            ' Else
            ' ClientAlert("Please input Name or Login to Filter", True)
            ' txtNameOrLogin.Focus()
            ' pnPlayerManager.Visible = False
            '  divPlayerEdit.Visible = False
            '  End If
        End Sub

        Protected Sub ddlAgents_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlAgents.SelectedIndexChanged
            bindPlayers()
            PlayerEdit1.AgentID = ddlAgents.Value
        End Sub

        Protected Sub ddlTemplates_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlTemplates.SelectedIndexChanged
            bindPlayers()
            PlayerEdit1.TemplateID = ddlTemplates.Value
        End Sub


#Region "PlayerTemplate"

        Private Function checkTemplate(ByVal poTemplate As CPlayerTemplate) As Boolean
            'If poTemplate.TemplateName = "" Then
            '    ucEditTemplate.SetTemplateNameFocus()
            '    setTabActive("BETTING_PROFILE")
            '    ClientAlert("Template name is required", True)
            '    Return False
            'End If

            ''If (New CPlayerTemplateManager).IsExistedTemplateName(poTemplate.TemplateName, SBCBL.std.GetSiteType, PlayerTemplateEditID) Then
            ''    ClientAlert("Template name '" & poTemplate.TemplateName & "' has already existed", True)
            ''    ucEditTemplate.SetTemplateNameFocus()
            ''    Return False
            ''End If

            'Return True
        End Function

        Private Function saveTemplate(ByVal poTemplate As CPlayerTemplate) As Boolean
            Dim oTemplateMng As New CPlayerTemplateManager

            ''set player template id
            If PlayerTemplateEditID <> "" Then
                poTemplate.PlayerTemplateID = PlayerTemplateEditID
            End If

            ''save template
            Dim bSuccess As Boolean = False
            Dim oPlayer = (New CPlayerManager()).GetPlayer(PlayerID)
            If Not String.IsNullOrEmpty(oPlayer.PlayerTemplateID) Then
                bSuccess = oTemplateMng.UpdatePlayerTemplate(poTemplate, "N")
            Else
                poTemplate.PlayerTemplateID = newGUID()
                bSuccess = oTemplateMng.InsertPlayerTemplate(poTemplate, SBCBL.std.GetSiteType, "N")
                If bSuccess Then
                    bSuccess = (New CPlayerManager).UpdatePlayerTemplateID(PlayerID, poTemplate.PlayerTemplateID, oPlayer.Login, UserSession.UserID)
                End If
                If bSuccess Then
                    PlayerTemplateEditID = poTemplate.PlayerTemplateID
                End If
                bindPlayers()
            End If

            If Not bSuccess Then
                ClientAlert("Failed to Save Setting", True)
                Return False
            End If
            ClientAlert("Successfully saved", True)
            Return True
        End Function


        'Private Sub resetTemplate()
        '    setTabActive("Template")
        '    PlayerTemplateEditID = ""
        '    ucEditTemplate.ResetPlayerTemplate()
        '    ucEditTemplate.SetTemplateNameFocus()

        'End Sub
#End Region

        Protected Sub btnAddnewplayer_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddnewplayer.Click
            pnPlayerManager.Visible = False
            divPlayerEdit.Visible = True
            PlayerEdit1.Visible = True
            PlayerEdit1.DisnableTemplate()
            PlayerEdit1.ClearLimits()
            PlayerEdit1.ResetPlayerInfor()
            'disable other buttons
            EnabledButton(False)
            PlayerEdit1.bindNextPlayer()
            PlayerEdit1.SetNewPlayerForm(True)
        End Sub

        Private Sub EnabledButton(ByVal isEnabled As Boolean)
            btnBettingLock.Enabled = isEnabled
            btnLock.Enabled = isEnabled
            btnViewLock.Enabled = isEnabled
            btnViewBettingLock.Enabled = isEnabled
        End Sub

        Protected Sub PlayerEdit1_ButtonClick(ByVal sButtonType As String) Handles PlayerEdit1.ButtonClick
            dgPlayers.SelectedIndex = -1
            PlayerEdit1.ResetPlayerInfor()
            Select Case UCase(sButtonType)
                Case "SAVE UNSUCCESSFUL"
                    ClientAlert("Failed to Save Setting", True)
                Case "SAVE SUCCESSFUL"
                    bindPlayers()
            End Select
            pnPlayerManager.Visible = True
            PlayerEdit1.Visible = False
            divPlayerEdit.Visible = False
            EnabledButton(True)
        End Sub

        Private Function checkCreateNewGroupPlayer() As Boolean
            If ddlplayerTemplate.SelectedValue = "" Then
                ClientAlert("Please select preset player template.", True)
                Return False
            End If
            Return True
        End Function

        Protected Sub btnCreate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreate.Click
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
