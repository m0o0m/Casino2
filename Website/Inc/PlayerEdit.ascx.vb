Imports System.Data
Imports SBCBL
Imports SBCBL.std
Imports SBCBL.CacheUtils
Imports SBCBL.Managers

Namespace SBCAgents

    Partial Class Inc_PlayerEdit
        Inherits SBCBL.UI.CSBCUserControl

        Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())
        Public Event ButtonClick(ByVal sButtonType As String)
        Private psAgentID As String
#Region "Properties"

        Public Property CurrentPlayer() As Integer
            Get
                Return SafeInteger(Session("CurrentPlayer"))
            End Get
            Set(ByVal value As Integer)
                Session("CurrentPlayer") = value
            End Set
        End Property

        Public Property AgentID() As String
            Get
                Return ddlAgents.Value
            End Get
            Set(ByVal value As String)
                ddlAgents.Value = value
            End Set
        End Property

        Public Property TemplateID() As String
            Get
                Return SafeString(ViewState("TEMPLATE_ID"))
            End Get
            Set(ByVal value As String)
                ViewState("TEMPLATE_ID") = value
            End Set
        End Property

        Private ReadOnly Property IsCreateNew() As Boolean
            Get
                Return SafeString(hfPlayerID.Value) = ""
            End Get
        End Property

        Public ReadOnly Property SiteType() As CEnums.ESiteType
            Get
                Return CEnums.ESiteType.SBS
            End Get

        End Property
#End Region

#Region "Page events"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                bindTimeZones()
                bindTemplates()
                bindAgents()
                ResetPlayerInfor()
                DisnableTemplate()
                ucPlayerLimit.Visible = False
                bindNextPlayer()


                'If UserSession.UserType = EUserType.Agent Then
                '    psAgentID = UserSession.UserID
                'ElseIf UserSession.UserType = EUserType.SuperAdmin Then
                '    psAgentID = UserSession.AgentUserInfo.SuperAdminID
                'End If
            End If
        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            btnSave.Text = "Save"
            'log.Debug("P ID:" + hfPlayerID.Value)

            If ViewState("IsNewPlayerForm") = True Then
                btnSave.Visible = True
                btnSaveInfo.Visible = True

                btnSavePlayerInfo.Visible = False
                btnSavePlayerLimitInfo.Visible = False
            Else
                btnSave.Visible = False
                btnSaveInfo.Visible = False

                btnSavePlayerInfo.Visible = True
                btnSavePlayerLimitInfo.Visible = True
            End If

            ibtGenerateLogin.Visible = hfPlayerID.Value <> ""
        End Sub

        Protected Sub ucPlayerLimit_SaveTemplate(ByVal sender As Object, ByVal e As EventArgs) Handles ucPlayerLimit.SavePlayerTemplate
            Dim oTemplateManager As New CPlayerTemplateManager()
            If Not String.IsNullOrEmpty(hfPlayerTemplateID.Value) Then
                Dim oPlayerTemplate As CPlayerTemplate = oTemplateManager.GetPlayerTemplate(SafeString(hfPlayerTemplateID.Value))
                If ucPlayerLimit.SaveCopy(oPlayerTemplate.PlayerTemplateID) Then  'UpdateLimits() Then thuong
                    ClientAlert("Successfully Saved", True)
                Else
                    ClientAlert("Failed to Save Setting", True)
                End If
            End If

        End Sub

        Protected Sub ucPlayerLimit_UpdateLimitUseDefaultValues(ByVal sender As Object, ByVal e As EventArgs) Handles ucPlayerLimit.UpdateLimitUseDefaultValues
            SetDefaultValuesForLimitSetting()
        End Sub


#End Region

#Region "Public methods"

        Public Function SetDefaultValuesForLimitSetting() As Boolean
            Dim oDefaultValues As New Dictionary(Of String, String)
            oDefaultValues.Add("Max Single", txtCreditmaxSingle.Text)
            oDefaultValues.Add("Max Parlay", txtCreditMaxParlay.Text)
            oDefaultValues.Add("Max Reverse", txtCreditMaxReverseActionParlay.Text)
            oDefaultValues.Add("Max Teaser", txtCreditMaxTeaserParlay.Text)
            oDefaultValues.Add("1H", txt1stH.Text)
            oDefaultValues.Add("2H", txt2stH.Text)
            oDefaultValues.Add("1Q", txt1stQ.Text)
            oDefaultValues.Add("2Q", txt2ndQ.Text)
            oDefaultValues.Add("3Q", txt3rdQ.Text)
            oDefaultValues.Add("4Q", txt4thQ.Text)
            ucPlayerLimit.LoadDefaultLimitValues(oDefaultValues)
        End Function

        Public Function SetMaxGameValuesForLimitSetting() As Boolean
            Dim oDefaultValues As New Dictionary(Of String, String)
            oDefaultValues.Add("Max Single", txtCreditmaxSingle.Text)
            oDefaultValues.Add("Max Parlay", txtCreditmaxSingle.Text)
            oDefaultValues.Add("Max Reverse", txtCreditmaxSingle.Text)
            oDefaultValues.Add("Max Teaser", txtCreditmaxSingle.Text)
            oDefaultValues.Add("1H", txtCreditmaxSingle.Text)
            oDefaultValues.Add("2H", txtCreditmaxSingle.Text)
            oDefaultValues.Add("1Q", txtCreditmaxSingle.Text)
            oDefaultValues.Add("2Q", txtCreditmaxSingle.Text)
            oDefaultValues.Add("3Q", txtCreditmaxSingle.Text)
            oDefaultValues.Add("4Q", txtCreditmaxSingle.Text)
            ucPlayerLimit.LoadDefaultLimitValues(oDefaultValues)
        End Function

        Public Function LoadPlayerInfo(ByVal psPlayerID As String) As Boolean
            ViewState("PlayerID") = psPlayerID
            Dim odrPlayer As DataRow = (New CPlayerManager).GetPlayerDataRow(psPlayerID)
            If odrPlayer Is Nothing Then
                ResetPlayerInfor()
                Return False
            End If
            Session("PlayerID") = psPlayerID
            hfPlayerID.Value = psPlayerID
            btnSave.Text = "Save"
            btnSave.ToolTip = "Save"

            txtName.Text = SafeString(odrPlayer("Name"))
            txtLogin.Text = SafeString(odrPlayer("Login"))
            hfOldLogin.Value = txtLogin.Text
            hfBalanceAmount.Value = SafeDouble(odrPlayer("BalanceAmount"))
            psdPassword.Password = ""
            ddlTimeZone.Value = SafeString(odrPlayer("TimeZone"))
            txtLockreason.Text = SafeString(odrPlayer("LockReason"))
            LogDebug(log, "PlayerTemplateID to show Player Info: " & SafeString(odrPlayer("PlayerTemplateID")))

            If SafeString(odrPlayer("PlayerTemplateID")) <> "" Then
                ddlTemplates.Value = SafeString(odrPlayer("PlayerTemplateID"))
                hfPlayerTemplateID.Value = SafeString(odrPlayer("PlayerTemplateID"))
                ddlTemplates.Enabled = False
                btnUpdateLimits.Visible = False

                btnUpdateOrginalAmount.Visible = True
                btnTemporary.Visible = True
                btnUpdateMaxPerGame.Visible = True
            Else
                hfPlayerTemplateID.Value = ""
                ddlTemplates.Value = SafeString(odrPlayer("DefaultPlayerTemplateID"))
                'ddlTemplates.Enabled = True
                ddlTemplates.Enabled = GetPlayerTemplateSubAgent(AgentID)
                If SafeDouble(odrPlayer("BalanceAmount")) = SafeDouble(odrPlayer("OriginalAmount")) Then
                    'btnUpdateLimits.Visible = True
                    btnUpdateLimits.Visible = False
                End If

                btnUpdateOrginalAmount.Visible = False
                btnTemporary.Visible = False
                btnUpdateMaxPerGame.Visible = False
            End If
            chkManuaReset.Checked = SafeBoolean(odrPlayer("IsResetManual"))
            btnResetMunualy.Visible = SafeBoolean(odrPlayer("IsResetManual"))
            txtAlertMessage.Text = SafeString(odrPlayer("AlertMessage"))
            txtNumFailedAttempts.Text = FormatNumber(SafeInteger(odrPlayer("NumFailedAttempts")), 0)
            chkIsBettingLocked.Checked = SafeString(odrPlayer("IsBettingLocked")) = "Y"
            chkCasino.Checked = SafeString(odrPlayer("HasCasino")) = "Y"
            chkIsLocked.Checked = SafeString(odrPlayer("IsLocked")) = "Y"
            ddlAgents.Value = SafeString(odrPlayer("AgentID"))
            psdPassword.Password = odrPlayer("Password")

            txtAccountBalance.Text = SafeDouble(odrPlayer("BalanceAmount")) 'show on Account Balance textbox

            ShowLimitsInfo()
            Return True
        End Function

        Public Sub ResetPlayerInfor()
            ViewState("PlayerID") = ""
            hfPlayerID.Value = ""
            hfOldLogin.Value = ""
            btnSave.Text = "Add"
            btnSave.ToolTip = "Create new player"

            txtName.Text = ""
            txtLogin.Text = ""
            psdPassword.Password = ""
            ' ddlTemplates.Value = ""
            ddlTemplates.Value = ""
            txtAlertMessage.Text = ""
            txtNumFailedAttempts.Text = "5"
            chkIsBettingLocked.Checked = False
            chkCasino.Checked = False
            chkIsLocked.Checked = False
            txtLockreason.Text = ""
            txtName.Focus()
        End Sub

        Public Sub SetNewPlayerForm(ByVal IsNewPlayer As Boolean)
            ViewState("IsNewPlayerForm") = IsNewPlayer
        End Sub

        Public Sub ShowLimitsInfo(Optional ByVal IsGetDefault As Boolean = False)
            'show Limit INfo
            Dim oTemplateManager As New CPlayerTemplateManager()
            Dim poTemplate As CPlayerTemplate = Nothing
            If SafeString(hfPlayerTemplateID.Value) <> "" Then
                poTemplate = oTemplateManager.GetPlayerTemplate(SafeString(hfPlayerTemplateID.Value))
            Else
                poTemplate = oTemplateManager.GetPlayerTemplate(ddlTemplates.Value)
            End If
            hfPlayerID.Value = ViewState("PlayerID")
            If poTemplate IsNot Nothing Then
                Dim odrPlayer As DataRow = Nothing
                If Not String.IsNullOrEmpty(SafeString(hfPlayerID.Value)) Then
                    odrPlayer = (New CPlayerManager).GetPlayerDataRow(hfPlayerID.Value)
                Else
                    'ClientAlert(hfPlayerID.Value, True)
                End If

                With poTemplate
                    txtCreditMaxAmount.Text = .CreditMaxAmount
                    hfMaxCredit.Value = SafeDouble(.CreditMaxAmount)
                    ' txtAccountBalance.Text = .AccountBalance

                    'When create new Player, set default value of Account Balance is CreditMax Amount
                    If Me.IsCreateNew Or (odrPlayer IsNot Nothing AndAlso SafeString(odrPlayer("PlayerTemplateID")) = "") Then
                        txtAccountBalance.Text = .CreditMaxAmount
                    Else
                        If odrPlayer IsNot Nothing Then
                            txtAccountBalance.Text = SafeDouble(odrPlayer("BalanceAmount"))
                        End If

                    End If


                    txtCasinoMaxAmount.Text = .CasinoMaxAmount
                    txtCreditmaxSingle.Text = .MaxSingle
                    txtCreditMinBetPhone.Text = .CreditMinBetPhone
                    txtCreditMinBetInternet.Text = .CreditMinBetInternet
                    txt1stQ.Text = .Max1Q
                    txt2ndQ.Text = .Max2Q
                    txt3rdQ.Text = .Max3Q
                    txt4thQ.Text = .Max4Q
                    txt1stH.Text = .Max1H
                    txt2stH.Text = .Max2H
                    txtCreditMaxReverseActionParlay.Text = .CreditMaxReverseActionParlay
                    txtCreditMaxTeaserParlay.Text = .CreditMaxTeaserParlay
                    txtCreditMaxParlay.Text = .CreditMaxParlay

                    If ViewState("Straight") IsNot Nothing Then
                        ViewState("Straight") = txtCreditmaxSingle.Text
                        ViewState("Reverse") = txtCreditMaxReverseActionParlay.Text
                        ViewState("Parlay") = txtCreditMaxParlay.Text
                        ViewState("Teaser") = txtCreditMaxTeaserParlay.Text
                    End If
                End With
                'show limit setting
                ucPlayerLimit.Visible = True

                If SafeString(hfPlayerTemplateID.Value) <> "" Then
                    ucPlayerLimit.LoadPlayerSetting(SafeString(hfPlayerTemplateID.Value))
                Else
                    ucPlayerLimit.LoadPlayerSetting(SafeString(ddlTemplates.Value))
                End If

                If IsGetDefault = True Then
                    SetDefaultValuesForLimitSetting()
                End If
            End If
        End Sub

#End Region

#Region "Methods"

        Private Function UpdateLimits() As Boolean
            Dim bOldPlayer As Boolean = False
            Try
                bOldPlayer = SaveLimitTemplate()
                Dim oPlayerTemplateManager As New CPlayerTemplateManager
                Dim oPlayerTemplate As New CPlayerTemplate
                Dim oPlayerManager As New CPlayerManager()
                Dim dNewPlayerBalanceAmout As Double = 0
                With oPlayerTemplate
                    'add new value
                    If UserSession.UserType = EUserType.Agent Then
                        .SuperAdminID = UserSession.AgentUserInfo.SuperAdminID
                    ElseIf UserSession.UserType = EUserType.SuperAdmin Then
                        .SuperAdminID = UserSession.UserID
                    End If
                    .LastSaveBy = UserSession.UserID
                    .LastSavedDate = Date.UtcNow()

                    'check to save new Amount of Player
                    Dim dtmpCreditMaxAmount As Double = 0
                    If SafeDouble(txtCreditMaxAmount.Text) <> SafeDouble(hfMaxCredit.Value) Then
                        dtmpCreditMaxAmount = SafeDouble(txtCreditMaxAmount.Text) - SafeDouble(hfMaxCredit.Value)
                    Else
                        dtmpCreditMaxAmount = 0
                    End If
                    dNewPlayerBalanceAmout = SafeDouble(dtmpCreditMaxAmount) + SafeDouble(hfBalanceAmount.Value)
                    'log.Debug("lai update:" & SafeString(Session("Updatelimit")))
                    If (SafeString(Session("Updatelimit")) = "Y") Then
                        Session("Updatelimit") = Nothing
                    Else
                        oPlayerManager.UpdatePlayerBalanceAmount(SafeString(hfPlayerID.Value), dNewPlayerBalanceAmout, txtLogin.Text, SafeString(UserSession.UserID)) 'update player's balance
                        Session("Updatelimit") = Nothing
                        'end check
                    End If
                    .AccountBalance = SafeDouble(txtAccountBalance.Text)
                    .CreditMaxAmount = SafeDouble(txtCreditMaxAmount.Text)
                    .CasinoMaxAmount = SafeDouble(txtCasinoMaxAmount.Text)
                    .CreditMinBetPhone = SafeDouble(txtCreditMinBetPhone.Text)
                    .CreditMinBetInternet = SafeDouble(txtCreditMinBetInternet.Text)
                    .MaxSingle = SafeDouble(txtCreditmaxSingle.Text)
                    .CreditMaxParlay = SafeDouble(txtCreditMaxParlay.Text)
                    .Max1H = SafeDouble(txt1stH.Text)
                    .Max2H = SafeDouble(txt2stH.Text)
                    .Max1Q = SafeDouble(txt1stQ.Text)
                    .Max2Q = SafeDouble(txt2ndQ.Text)
                    .Max3Q = SafeDouble(txt3rdQ.Text)
                    .Max4Q = SafeDouble(txt4thQ.Text)
                    .CreditMaxReverseActionParlay = SafeDouble(txtCreditMaxReverseActionParlay.Text)
                    .CreditMaxTeaserParlay = SafeDouble(txtCreditMaxTeaserParlay.Text)

                End With
                Dim oplayers = UserSession.Cache.GetPlayerInfo(hfPlayerID.Value)

                If SafeString(oplayers.PlayerTemplateID) = "" Then
                    'set new templateID
                    oPlayerTemplate.PlayerTemplateID = newGUID()
                    If oPlayerTemplateManager.InsertPlayerTemplate(oPlayerTemplate, GetSiteType(), "N") Then
                        'update PlayerTemplateID in Players

                        oPlayerManager.UpdatePlayerTemplateID(SafeString(hfPlayerID.Value), SafeString(oPlayerTemplate.PlayerTemplateID), txtLogin.Text, SafeString(UserSession.UserID))

                        'ClientAlert("Created new template succeed.", True)
                        bindTemplates() 'refresh Template
                        ' Else
                        'ClientAlert("Created new template failed.", True)
                    End If
                Else
                    'update 
                    oPlayerTemplate.PlayerTemplateID = hfPlayerTemplateID.Value
                    LogDebug(log, "playertemplateID to update:" & oPlayerTemplate.PlayerTemplateID)
                    ' If oPlayerTemplateManager.UpdatePlayerTemplate(oPlayerTemplate, "N") Then
                    If oPlayerTemplateManager.UpdatePlayerTemplate2(oPlayerTemplate, "N") Then
                        'ClientAlert("Update succeed.", True)
                        bindTemplates() 'refresh Template
                    End If
                End If
                If Not bOldPlayer Then
                    ucPlayerLimit.SaveCopy(oPlayerTemplate.PlayerTemplateID)
                    hfPlayerTemplateID.Value = oPlayerTemplate.PlayerTemplateID
                    SaveLimitTemplate(True)
                End If

                'save Limit settings

                ' ucPlayerLimit.SaveCopy(oPlayerTemplate.PlayerTemplateID)
                'SaveLimitTemplate()
                LoadPlayerInfo(hfPlayerID.Value)
                'load template again

                'ddlTemplates.SelectedItem.Value = hfPlayerTemplateID.Value
                ddlTemplates.SelectedItem.Value = oplayers.PlayerTemplateID 'test
                If hfPlayerTemplateID.Value IsNot Nothing Then ' check to disabled ddlTemplates
                    ddlTemplates.Enabled = False
                    btnUpdateLimits.Visible = False
                End If

                ' ShowLimitsInfo()
                UserSession.Cache.ClearPlayerInfo(SafeString(hfPlayerID.Value)) 'clear cache

                Return True
            Catch ex As Exception
                LogError(log, "Can not save Limits settings now.", ex)
            End Try
            Return False
        End Function

        Private Function UpdateMaxPerGameLimits() As Boolean
            Dim bOldPlayer As Boolean = False
            Try
                bOldPlayer = SaveLimitTemplate()
                Dim oPlayerTemplateManager As New CPlayerTemplateManager
                Dim oPlayerTemplate As New CPlayerTemplate
                Dim oPlayerManager As New CPlayerManager()
                With oPlayerTemplate
                    'add new value
                    If UserSession.UserType = EUserType.Agent Then
                        .SuperAdminID = UserSession.AgentUserInfo.SuperAdminID
                    ElseIf UserSession.UserType = EUserType.SuperAdmin Then
                        .SuperAdminID = UserSession.UserID
                    End If
                    .LastSaveBy = UserSession.UserID
                    .LastSavedDate = Date.UtcNow()

                    .MaxSingle = SafeDouble(txtCreditmaxSingle.Text)

                End With

                Dim oplayers = UserSession.Cache.GetPlayerInfo(hfPlayerID.Value)

                If SafeString(oplayers.PlayerTemplateID) = "" Then
                    'set new templateID
                    oPlayerTemplate.PlayerTemplateID = newGUID()
                    If oPlayerTemplateManager.InsertPlayerTemplate(oPlayerTemplate, GetSiteType(), "N") Then
                        'update PlayerTemplateID in Players

                        oPlayerManager.UpdatePlayerTemplateID(SafeString(hfPlayerID.Value), SafeString(oPlayerTemplate.PlayerTemplateID), txtLogin.Text, SafeString(UserSession.UserID))

                        'ClientAlert("Created new template succeed.", True)
                        bindTemplates() 'refresh Template
                        ' Else
                        'ClientAlert("Created new template failed.", True)
                    End If
                Else
                    'update 
                    oPlayerTemplate.PlayerTemplateID = hfPlayerTemplateID.Value
                    LogDebug(log, "playertemplateID to update:" & oPlayerTemplate.PlayerTemplateID)
                    ' If oPlayerTemplateManager.UpdatePlayerTemplate(oPlayerTemplate, "N") Then
                    If oPlayerTemplateManager.UpdatePlayerMaxPerGameTemplate(oPlayerTemplate) Then
                        'ClientAlert("Update succeed.", True)
                        bindTemplates() 'refresh Template
                    End If
                End If
                If Not bOldPlayer Then
                    ucPlayerLimit.SaveCopy(oPlayerTemplate.PlayerTemplateID)
                    hfPlayerTemplateID.Value = oPlayerTemplate.PlayerTemplateID
                    SaveLimitTemplate(True)
                End If

                'save Limit settings

                ' ucPlayerLimit.SaveCopy(oPlayerTemplate.PlayerTemplateID)
                'SaveLimitTemplate()
                'LoadPlayerInfo(hfPlayerID.Value)
                'load template again

                'ddlTemplates.SelectedItem.Value = hfPlayerTemplateID.Value
                ddlTemplates.SelectedItem.Value = oplayers.PlayerTemplateID 'test
                If hfPlayerTemplateID.Value IsNot Nothing Then ' check to disabled ddlTemplates
                    ddlTemplates.Enabled = False
                    btnUpdateLimits.Visible = False
                End If

                ' ShowLimitsInfo()
                UserSession.Cache.ClearPlayerInfo(SafeString(hfPlayerID.Value)) 'clear cache

                Return True
            Catch ex As Exception
                LogError(log, "Can not save Limits settings now.", ex)
            End Try
            Return False
        End Function

        Private Function checkPlayerInfor() As Boolean
            If SafeString(txtName.Text) = "" Then
                ClientAlert("Player Name Is Required", True)
                txtName.Focus() : Return False
            End If
            If SafeString(txtLogin.Text) = "" Then
                ClientAlert("Login Name Is Required", True)
                txtName.Focus() : Return False
            End If

            Dim sError As String = ""
            Dim bExisted As Boolean = (New CPlayerManager).IsExistedLogin(txtLogin.Text, IIf(String.IsNullOrEmpty(hfPlayerID.Value), SafeString(ViewState("PlayerID")), hfPlayerID.Value), sError, std.GetSiteType)
            If String.IsNullOrEmpty(hfPlayerID.Value) AndAlso SafeString(ViewState("PlayerID")) <> "" Then
                hfPlayerID.Value = SafeString(ViewState("PlayerID"))
            End If
            If sError <> "" Then
                ClientAlert("Can't Check Login. Please Try Again.", True)
                txtLogin.Text = "" : txtLogin.Focus() : Return False
            End If
            If bExisted Then
                ClientAlert("Login Is Existed. Please Try Again.", True)
                txtLogin.Text = "" : txtLogin.Focus() : Return False
            End If
            If hfPlayerID.Value = "" Then
                If SafeString(psdPassword.Password) = "" Then
                    ClientAlert("Password Is Required", True)
                    psdPassword.Focus() : Return False
                End If
                If Not ValidPassword(psdPassword.Password) Then
                    ClientAlert("Password Is Invalid", True)
                    psdPassword.Focus() : Return False
                End If
            End If
            If Me.IsCreateNew AndAlso SafeString(ddlTemplates.SelectedItem.Text) = "" Then
                ClientAlert("Template Is Required", True)
                ddlTemplates.Focus() : Return False
            End If

            Return True
        End Function

        Private Function insertPlayer() As Boolean
            Dim oMng As New CPlayerManager
            hfPlayerID.Value = newGUID()
            '        Dim bResult As Boolean = oMng.InsertPlayer(hfPlayerID.Value, Me.AgentID, SafeString(txtName.Text), SafeString(txtLogin.Text), SafeString(psdPassword.Password), SafeInteger(ddlTimeZone.Value), ddlTemplates.Value, _
            'SafeDouble(txtCreditMaxAmount.Text), txtAlertMessage.Text, _
            'SafeInteger(txtNumFailedAttempts.Text), False, chkIsBettingLocked.Checked, chkCasino.Checked, _
            'chkIsLocked.Checked, txtLockreason.Text, SafeString(txtLogin.Text), SafeString(psdPassword.Password), UserSession.UserID, SiteType.ToString(), chkRequireChangePass.Checked, 0)

            'If bResult Then
            '    Dim oAgent As New CAgentManager
            '    oAgent.IncreaseCurrentPlayerNumber(ddlAgents.Value, CurrentPlayer)
            'End If

            ' Return bResult
        End Function

        Private Function updatePlayer() As Boolean
            Dim oMng As New CPlayerManager
            Dim sPlayerTemplateID As String = IIf(ddlTemplates.Value <> "", SafeString(ddlTemplates.Value), SafeString(hfPlayerTemplateID.Value))

            Return oMng.UpdatePlayer(SafeString(hfPlayerID.Value), ddlAgents.Value, SafeString(txtName.Text), SafeString(txtLogin.Text), hfOldLogin.Value, SafeString(psdPassword.Password) _
                                , SafeInteger(ddlTimeZone.Value), sPlayerTemplateID, txtAlertMessage.Text, SafeInteger(txtNumFailedAttempts.Text) _
                                 , chkIsBettingLocked.Checked, chkCasino.Checked, False, chkIsLocked.Checked, txtLockreason.Text, SafeString(txtLogin.Text), SafeString(psdPassword.Password), UserSession.UserID, 0, SafeDouble(txtCreditMaxAmount.Text))

        End Function

        Protected Sub AutoGenerateLogin(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlAgents.SelectedIndexChanged, txtName.TextChanged
            If hfPlayerID.Value <> "" Then
                Return
            End If
            bindNextPlayer()
            'ibtGenerateLogin_Click(Nothing, Nothing)
        End Sub

        Public Function findMinTemplate() As String
            Dim odtTemplate As DataTable = (New CPlayerTemplateManager).GetPlayerTemplates(SBCBL.std.GetSiteType)
            If odtTemplate.Rows.Count = 0 Then
                Return Nothing
            End If
            odtTemplate.DefaultView.Sort = "AccountBalance asc"
            Return (SafeString(odtTemplate.DefaultView.ToTable().Rows(0)("PlayerTemplateID")))

        End Function

        Public Sub DisnableTemplate()
            If findMinTemplate() Is Nothing Then
                Return
            End If
            If UserSession.UserType = EUserType.Agent AndAlso Not SafeBoolean(UserSession.AgentUserInfo.IsEnablePlayerTemplate) Then
                Try
                    ddlTemplates.Value = findMinTemplate()
                Catch ex As Exception
                    bindTemplates()
                    ddlTemplates.Value = findMinTemplate()
                End Try
                ddlTemplates.Enabled = SafeBoolean(UserSession.AgentUserInfo.IsEnablePlayerTemplate)
            End If
        End Sub

        Public Sub ClearLimits()
            txtAccountBalance.Text = ""
            txtCreditMaxAmount.Text = ""
            txtCasinoMaxAmount.Text = ""
            txtCreditmaxSingle.Text = ""
            txtCreditMinBetPhone.Text = ""
            txtCreditMinBetInternet.Text = ""
            txt1stQ.Text = ""
            txt2ndQ.Text = ""
            txt3rdQ.Text = ""
            txt4thQ.Text = ""
            txt1stH.Text = ""
            txt2stH.Text = ""
            txtCreditMaxReverseActionParlay.Text = ""
            txtCreditMaxTeaserParlay.Text = ""
            txtCreditMaxParlay.Text = ""
            ddlTemplates.Enabled = True
            hfPlayerTemplateID.Value = ""
        End Sub

        Private Function CheckLimitValue() As Boolean
            Dim sTemplateID As String = ddlTemplates.Value
            Dim oTemplateManager As New CPlayerTemplateManager()
            Dim oPlayerTemplate As CPlayerTemplate = oTemplateManager.GetPlayerTemplate(sTemplateID)
            If oPlayerTemplate IsNot Nothing Then
                'check value from textboxs and value in oPlayerTemplate
                With oPlayerTemplate
                    If txtAccountBalance.Text <> .AccountBalance OrElse txtCreditMaxAmount.Text <> .CreditMaxAmount _
                       OrElse txtCasinoMaxAmount.Text <> .CasinoMaxAmount OrElse _
                       txtCreditMinBetPhone.Text <> .CreditMinBetPhone OrElse _
                       txtCreditMinBetInternet.Text <> .CreditMinBetInternet OrElse _
                       txtCreditmaxSingle.Text <> .MaxSingle OrElse txtCreditMaxParlay.Text <> .CreditMaxParlay _
                       OrElse txt1stH.Text <> .Max1H OrElse txt2stH.Text <> .Max2H OrElse txt1stQ.Text <> .Max1Q _
                       OrElse txt2ndQ.Text <> .Max2Q OrElse txt3rdQ.Text <> .Max3Q OrElse txt4thQ.Text <> .Max4Q _
                       OrElse txtCreditMaxReverseActionParlay.Text <> .CreditMaxReverseActionParlay _
                       OrElse txtCreditMaxTeaserParlay.Text <> .CreditMaxTeaserParlay Then
                        Return True
                    End If
                End With
            End If
            Return False
        End Function

#End Region

        Protected Sub btnTemporary_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTemporary.Click
            Dim bSuccess = False
            If String.IsNullOrEmpty(txtTemporary.Text) Then
                ClientAlert("Please, Temporary credit cannot empty", True)
                Return
            Else
                Dim oPlayerManager As New CPlayerManager()
                bSuccess = oPlayerManager.UpdateAccountBalance(SafeString(hfPlayerID.Value), SafeInteger(txtAccountBalance.Text) + SafeInteger(txtTemporary.Text), UserSession.UserID)
            End If
            If bSuccess Then
                LoadPlayerInfo(SafeString(hfPlayerID.Value))
                ClientAlert("Update , Successful", True)
            Else
                ClientAlert("Update ,Failed")
            End If

        End Sub



        'Protected Sub ibtGenerateLogin_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ibtGenerateLogin.Click
        '    If ddlAgents.Value = "" Then
        '        ClientAlert("Agent Is Required To Auto Generate Login", True)
        '        ddlAgents.Focus() : Return
        '    End If

        '    Dim sLogin, sAgentKey As String
        '    Dim oAgentManager As New CAgentManager
        '    sAgentKey = UserSession.Cache.GetAgentInfo(ddlAgents.Value).SpecialKey
        '    sLogin = sAgentKey & SafeString(oAgentManager.GetCurrentPlayerNumber(ddlAgents.Value) + 1)

        '    While oAgentManager.IsExistedLogin(sLogin, "", std.GetSiteType)
        '        oAgentManager.IncreaseCurrentPlayerNumber(ddlAgents.Value)
        '        sLogin = sAgentKey & SafeString(oAgentManager.GetCurrentPlayerNumber(ddlAgents.Value) + 1)
        '    End While

        '    txtLogin.Text = sLogin
        '    ' txtPhoneLogin.Text = txtLogin.Text
        'End Sub

#Region "Bind data"

        Private Sub bindAgents()
            If UserSession.UserType = EUserType.Agent Then
                ddlAgents.DataSource = loadAgents()
                ddlAgents.DataTextField = "AgentName"
                ddlAgents.DataValueField = "AgentID"
                ddlAgents.DataBind()
            ElseIf UserSession.UserType = EUserType.SuperAdmin Then
                bindAgentSuperAdmin()
            End If

        End Sub

#Region "Bind Agents - SuperAdmin"
        Private Sub bindAgentSuperAdmin()
            Dim dtParents As DataTable = (New CAgentManager).GetAllAgentsBySuperAdminID(UserSession.UserID, Nothing)

            Dim odrParents As DataRow() = dtParents.Select("ParentID IS NULL", "AgentName")

            For Each drParent As DataRow In odrParents
                Dim sAgentID As String = SafeString(drParent("AgentID"))
                ddlAgents.Items.Add(New ListItem(SafeString(drParent("AgentName")), sAgentID))

                loadSubAgentSuperAdmin(sAgentID, dtParents)
            Next

            ddlAgents.Items.Insert(0, "")
            ddlAgents.SelectedValue = SafeString(Request("AgentID"))
        End Sub

        Public Sub loadSubAgentSuperAdmin(ByVal psParentAgentID As String, ByVal podtAgents As DataTable)
            Dim odrSubAgents As DataRow() = podtAgents.Select("ParentID=" & SQLString(psParentAgentID), "AgentName")

            For Each drChild As DataRow In odrSubAgents
                Dim sText As String = loopString("----", SafeInteger(drChild("AgentLevel")) - 1) & SafeString(drChild("AgentName"))
                Dim sAgentID As String = SafeString(drChild("AgentID"))

                ddlAgents.Items.Add(New ListItem(sText, sAgentID))

                loadSubAgentSuperAdmin(sAgentID, podtAgents)
            Next
        End Sub
#End Region

#Region "Bind Agents - Agents"

        Private Function loadAgents() As DataTable
            Dim odtAgents As New DataTable
            odtAgents.Columns.Add("AgentID", GetType(Guid))
            odtAgents.Columns.Add("AgentName", GetType(String))

            ''parent
            Dim sParentName As String = String.Format("{0} ({1})", UserSession.AgentUserInfo.Login, UserSession.AgentUserInfo.Name)
            odtAgents.Rows.Add(New Object() {UserSession.UserID, sParentName})

            Dim oAgentManager As New CAgentManager()
            Dim dtParents As DataTable = oAgentManager.GetAllAgentsByAgent(UserSession.UserID, Nothing)

            Dim nMinAgentLevel As Integer = SafeInteger(dtParents.Compute("MIN(AgentLevel)", ""))
            Dim odrParents As DataRow() = dtParents.Select("AgentLevel=" & SafeString(nMinAgentLevel), "AgentName")

            For Each drParent As DataRow In odrParents
                Dim odrAgent As DataRow = odtAgents.NewRow
                odtAgents.Rows.Add(odrAgent)

                odrAgent("AgentID") = drParent("AgentID")
                odrAgent("AgentName") = loopString("----", SafeInteger(drParent("AgentLevel")) - 1) & SafeString(drParent("AgentName"))
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
#End Region



        Private Sub bindTimeZones()
            ddlTimeZone.DataSource = UserSession.SysSettings("TimeZone")
            ddlTimeZone.DataTextField = "Key"
            ddlTimeZone.DataValueField = "Value"
            ddlTimeZone.DataBind()
        End Sub

        Private Sub bindTemplates()
            ddlTemplates.Items.Clear()
            ddlTemplates.DataSource = (New CPlayerTemplateManager).GetPlayerTemplates(SBCBL.std.GetSiteType)
            ddlTemplates.DataTextField = "TemplateName"
            ddlTemplates.DataValueField = "PlayerTemplateID"
            ddlTemplates.DataBind()
        End Sub

#End Region

        Protected Sub ddlTemplates_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlTemplates.SelectedIndexChanged
            If ddlTemplates.Value <> "" Then
                ShowLimitsInfo(True)
            Else
                ClearLimits()
            End If

        End Sub

        Protected Sub btnUpdateLimits_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateLimits.Click
            If hfPlayerID.Value = "" AndAlso ViewState("PlayerID") Is Nothing Then
                ClientAlert("Save Player's Information Before Update Limits Template, Please!", True)
                Return
            ElseIf hfPlayerID.Value = "" AndAlso ViewState("PlayerID") IsNot Nothing Then
                hfPlayerID.Value = ViewState("PlayerID")
            End If
            SetDefaultValuesForLimitSetting()
            Dim oPlayerManager As New CPlayerManager()
            oPlayerManager.UpdateLimitByUser(SafeString(hfPlayerID.Value), SafeDouble(txtCreditMaxAmount.Text))
            Session("Updatelimit") = "Y"
            If UpdateLimits() Then
                ClientAlert("Successfully Updated", True)
            Else
                ClientAlert("Unsuccessfully Updated", True)
            End If
        End Sub

        Protected Sub btnSavePlayerLimitInfo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSavePlayerLimitInfo.Click
            If hfPlayerID.Value = "" AndAlso ViewState("PlayerID") Is Nothing Then
                ClientAlert("Save Player's Information Before Update Limits Template, Please!", True)
                Return
            ElseIf hfPlayerID.Value = "" AndAlso ViewState("PlayerID") IsNot Nothing Then
                hfPlayerID.Value = ViewState("PlayerID")
            End If
            SetDefaultValuesForLimitSetting()
            Dim oPlayerManager As New CPlayerManager()
            oPlayerManager.UpdateLimitByUser(SafeString(hfPlayerID.Value), SafeDouble(txtCreditMaxAmount.Text))
            Session("Updatelimit") = "Y"
            If UpdateLimits() Then
                ClientAlert("Successfully Updated", True)
            Else
                ClientAlert("Unsuccessfully Updated", True)
            End If
        End Sub

        Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
            RaiseEvent ButtonClick("CANCEL")
            DisnableTemplate()
        End Sub

        Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click, btnSaveInfo.Click
            If Not checkPlayerInfor() Then
                Return
            End If
            If btnUpdateLimits.Visible Then
                ClientAlert("Please Click on the Update Limit Button before You Can Save", True)
                Return
            End If

            Dim bSuccess As Boolean = False
            Application.Lock()
            Try
                If Me.IsCreateNew Then
                    bSuccess = insertPlayer()
                    ClientAlert("Successfully Saved", True)
                Else
                    bSuccess = updatePlayer()
                    bSuccess = UpdateLimits() And bSuccess
                    ClientAlert("Successfully Saved", True)
                End If

            Finally
                Application.UnLock()
            End Try
            RaiseEvent ButtonClick(SafeString(IIf(bSuccess, "SAVE SUCCESSFUL", "SAVE UNSUCCESSFUL")))
        End Sub

        Protected Sub btnSavePlayerInfo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSavePlayerInfo.Click
            If Not checkPlayerInfor() Then
                Return
            End If
            'If btnUpdateLimits.Visible Then
            '    ClientAlert("Please Click on the Update Limit Button before You Can Save", True)
            '    Return
            'End If

            Dim bSuccess As Boolean = False
            Application.Lock()
            Try
                If Me.IsCreateNew Then
                    'bSuccess = insertPlayer()
                    'ClientAlert("Successfully Saved", True)
                Else
                    bSuccess = updatePlayer()
                    'bSuccess = UpdateLimits() And bSuccess
                    ClientAlert("Successfully Saved", True)
                End If

            Finally
                Application.UnLock()
            End Try
            RaiseEvent ButtonClick(SafeString(IIf(bSuccess, "SAVE SUCCESSFUL", "SAVE UNSUCCESSFUL")))
        End Sub

        Protected Sub btnCancelInfo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancelInfo.Click
            RaiseEvent ButtonClick("CANCEL")
            DisnableTemplate()
        End Sub

        Public Sub bindNextPlayer(Optional ByVal AgentID As String = "")
            Dim sPresetAgentID = IIf(String.IsNullOrEmpty(AgentID), ddlAgents.Value, AgentID)
            If String.IsNullOrEmpty(sPresetAgentID) Then
                Return
            End If
            Dim oPlayerManager = New CPlayerManager()
            CurrentPlayer = UserSession.Cache.GetAgentInfo(sPresetAgentID).CurrentPlayerNumber
            Dim sLoginname As String
            Dim bCheckExist As Boolean = True
            While (bCheckExist)
                sLoginname = UserSession.Cache.GetAgentInfo(sPresetAgentID).SpecialKey & CurrentPlayer
                If oPlayerManager.IsExistedLogin(sLoginname, "", "", std.GetSiteType) Then
                    CurrentPlayer += 1
                Else
                    txtLogin.Text = UserSession.Cache.GetAgentInfo(sPresetAgentID).SpecialKey & CurrentPlayer
                    txtName.Text = UserSession.Cache.GetAgentInfo(sPresetAgentID).SpecialKey & CurrentPlayer
                    bCheckExist = False
                    Return
                End If
            End While
        End Sub

        ''' <summary>
        ''' ''''''''start new''''''''''''''''
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Sub Txt_Change(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt1stH.TextChanged, txt1stQ.TextChanged, txt2ndQ.TextChanged, txt3rdQ.TextChanged, txt4thQ.TextChanged, txtCreditMaxParlay.TextChanged, txtCreditMaxReverseActionParlay.TextChanged, txtCreditmaxSingle.TextChanged, txt2stH.TextChanged
            Dim txt = CType(sender, TextBox)
            'If txt.ID = txt1stH.ID Then
            '    ViewState("1H") = True
            'End If
            'If txt.ID = txt2stH.ID Then
            '    ViewState("2H") = True
            'End If
            'If txt.ID = txt1stQ.ID Then
            '    ViewState("1Q") = True
            'End If
            'If txt.ID = txt2ndQ.ID Then
            '    ViewState("2Q") = True
            'End If
            'If txt.ID = txt3rdQ.ID Then
            '    ViewState("3Q") = True
            'End If
            'If txt.ID = txt4thQ.ID Then
            '    ViewState("4Q") = True
            'End If
            ' LogDebug(log, "adsa" + txt.ID)
            'If txt.ID = txtCreditmaxSingle.ID Then
            '    ViewState("Straight") = True
            'End If
            'If txt.ID = txtCreditMaxReverseActionParlay.ID Then
            '    ViewState("Reverse") = True
            'End If
            'If txt.ID = txtCreditMaxParlay.ID Then
            '    ViewState("Parlay") = True
            'End If
            'If txt.ID = txtCreditMaxTeaserParlay.ID Then
            '    ViewState("Teaser") = True
            'End If
        End Sub
        Public Function CreateSaveDataTable() As DataTable
            Dim odt As New DataTable
            odt.Columns.Add("PlayerTemplateLimitID")
            odt.Columns.Add("PlayerTemplateID")
            odt.Columns.Add("GameType")
            odt.Columns.Add("Context")
            odt.Columns.Add("MaxSingle")
            odt.Columns.Add("MaxParlay")
            odt.Columns.Add("MaxTeaser")
            odt.Columns.Add("MaxReverse")
            Return odt
        End Function

        Public Function SaveLimitTemplate(Optional ByVal bUpdate As Boolean = False) As Boolean
            Try
                Dim oPlayerLimitManager As New CPlayerLimitManager()
                Dim odtSaveTable As DataTable = CreateSaveDataTable()
                CreateSaveData(odtSaveTable, hfPlayerTemplateID.Value, txtCreditmaxSingle.Text, SafeInteger(txtCreditMaxParlay.Text), SafeInteger(txtCreditMaxReverseActionParlay.Text), SafeInteger(txtCreditMaxTeaserParlay.Text), bUpdate)
                If odtSaveTable.Rows.Count = 0 Then
                    Return False
                End If
                'For Each odr As DataRow In odtSaveTable.Rows

                '    log.Error(hfPlayerTemplateID.Value & "----" & SafeString(odr("PlayerTemplateLimitID")) & "--" & SafeString(odr("MaxSingle")))
                'Next
                LogDebug(log, "save strart" & Join(getAllGameType().ToArray, "','"))
                Return oPlayerLimitManager.UpdateData(odtSaveTable, hfPlayerTemplateID.Value, "'" & Join(getAllGameType().ToArray, "','"))
            Catch ex As Exception
                LogError(log, "Save Limit Template", ex)
            End Try
            Return False
        End Function

        Public Function CreateSaveData(ByRef odtLimitSettingData As DataTable, ByVal poPlayerTemplateID As String, ByVal nMaxSingle As Integer, ByVal nMaxParlay As Integer, ByVal nMaxReverse As Integer, ByVal nMaxTeaser As Integer, Optional ByVal bUpdate As Boolean = False) As Boolean
            Try
                Dim oTempalte As CPlayerTemplate
                Dim lstContext As New List(Of String)
                lstContext.Add("Current")
                lstContext.Add("1H")
                lstContext.Add("2H")
                lstContext.Add("1Q")
                lstContext.Add("2Q")
                lstContext.Add("3Q")
                lstContext.Add("4Q")
                log.Error(hfPlayerID.Value)
                If Not String.IsNullOrEmpty(SafeString(hfPlayerTemplateID.Value)) Then
                    Dim oTemplateManager As New CPlayerTemplateManager()
                    oTempalte = oTemplateManager.GetPlayerTemplate(SafeString(hfPlayerTemplateID.Value))
                Else
                    Return False
                End If
                Dim lstSportType = GetAllSportType()
                '  LogDebug(log, SafeInteger(txtCreditmaxSingle.Text) & "<>" & SafeInteger(oTempalte.MaxSingle))
                If SafeInteger(txtCreditmaxSingle.Text) <> SafeInteger(oTempalte.MaxSingle) OrElse bUpdate Then
                    For Each strSportType As String In lstSportType
                        For Each strGameType As String In getListGameType(strSportType)
                            Dim oRow As DataRow = odtLimitSettingData.NewRow
                            oRow("PlayerTemplateLimitID") = newGUID()
                            oRow("PlayerTemplateID") = poPlayerTemplateID
                            oRow("GameType") = strGameType
                            oRow("Context") = "Current"
                            'log.Error(txtCreditmaxSingle.Text + "-" + SafeString(oTempalte.MaxSingle))
                            oRow("MaxSingle") = nMaxSingle
                            odtLimitSettingData.Rows.Add(oRow)

                            Dim oRow2 As DataRow = odtLimitSettingData.NewRow
                            oRow2("PlayerTemplateLimitID") = newGUID()
                            oRow2("PlayerTemplateID") = poPlayerTemplateID
                            oRow2("GameType") = strGameType
                            oRow2("Context") = "1H"
                            oRow2("MaxSingle") = nMaxSingle
                            odtLimitSettingData.Rows.Add(oRow2)

                            Dim oRow3 As DataRow = odtLimitSettingData.NewRow
                            oRow3("PlayerTemplateLimitID") = newGUID()
                            oRow3("PlayerTemplateID") = poPlayerTemplateID
                            oRow3("GameType") = strGameType
                            oRow3("Context") = "2H"
                            oRow3("MaxSingle") = nMaxSingle
                            odtLimitSettingData.Rows.Add(oRow3)

                            Dim oRow4 As DataRow = odtLimitSettingData.NewRow
                            oRow4("PlayerTemplateLimitID") = newGUID()
                            oRow4("PlayerTemplateID") = poPlayerTemplateID
                            oRow4("GameType") = strGameType
                            oRow4("Context") = "1Q"
                            oRow4("MaxSingle") = nMaxSingle
                            odtLimitSettingData.Rows.Add(oRow4)

                            Dim oRow5 As DataRow = odtLimitSettingData.NewRow
                            oRow5("PlayerTemplateLimitID") = newGUID()
                            oRow5("PlayerTemplateID") = poPlayerTemplateID
                            oRow5("GameType") = strGameType
                            oRow5("Context") = "2Q"
                            oRow5("MaxSingle") = nMaxSingle
                            odtLimitSettingData.Rows.Add(oRow5)

                            Dim oRow6 As DataRow = odtLimitSettingData.NewRow
                            oRow6("PlayerTemplateLimitID") = newGUID()
                            oRow6("PlayerTemplateID") = poPlayerTemplateID
                            oRow6("GameType") = strGameType
                            oRow6("Context") = "3Q"
                            oRow6("MaxSingle") = nMaxSingle
                            odtLimitSettingData.Rows.Add(oRow6)

                            Dim oRow7 As DataRow = odtLimitSettingData.NewRow
                            oRow7("PlayerTemplateLimitID") = newGUID()
                            oRow7("PlayerTemplateID") = poPlayerTemplateID
                            oRow7("GameType") = strGameType
                            oRow7("Context") = "4Q"
                            oRow7("MaxSingle") = nMaxSingle
                            odtLimitSettingData.Rows.Add(oRow7)

                            Dim oRow8 As DataRow = odtLimitSettingData.NewRow
                            oRow8("PlayerTemplateLimitID") = newGUID()
                            oRow8("PlayerTemplateID") = poPlayerTemplateID
                            oRow8("GameType") = strGameType
                            oRow8("Context") = "TotalPoints"
                            oRow8("MaxSingle") = nMaxSingle
                            odtLimitSettingData.Rows.Add(oRow8)

                            Dim oRow9 As DataRow = odtLimitSettingData.NewRow
                            oRow9("PlayerTemplateLimitID") = newGUID()
                            oRow9("PlayerTemplateID") = poPlayerTemplateID
                            oRow9("GameType") = strGameType
                            oRow9("Context") = "MoneyLine"
                            oRow9("MaxSingle") = nMaxSingle
                            odtLimitSettingData.Rows.Add(oRow9)
                        Next
                    Next
                Else
                    Return False
                End If
            Catch ex As Exception
                LogError(log, "Save Data", ex)
                Return False
            End Try
            Return True
        End Function

        Protected Sub btnResetMunualy_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnResetMunualy.Click
            Dim oCResetAccountBalance As CResetAccountBalance = New CResetAccountBalance()
            Dim oPlayerManager As CPlayerManager = New CPlayerManager()
            Dim oAgentManager As CAgentManager = New CAgentManager()
            Dim odr As DataRow = oPlayerManager.GetPlayerDataRow(hfPlayerID.Value, False)
            oCResetAccountBalance.ResetAccountBalance(odr, oAgentManager.GetAgentByAgentID(SafeString(odr("AgentID"))))
            ClientAlert("Reset Succesfull", True)
        End Sub

        Protected Sub chkManuaReset_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkManuaReset.CheckedChanged
            Dim oPlayerManager As CPlayerManager = New CPlayerManager()
            If chkManuaReset.Checked Then
                btnResetMunualy.Visible = True
            Else
                btnResetMunualy.Visible = False
            End If
            oPlayerManager.UpdatePlayerPresetManual(hfPlayerID.Value, txtLogin.Text, chkManuaReset.Checked, UserSession.UserID)
        End Sub

        Protected Sub btnUpdateOrginalAmount_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateOrginalAmount.Click
            Dim oPlayerManager As CPlayerManager = New CPlayerManager()
            ClientAlert(oPlayerManager.UpdateOriginalAmount(hfPlayerID.Value, SafeDouble(txtCreditMaxAmount.Text)), True)
        End Sub

        Protected Sub btnUpdateMaxPerGame_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateMaxPerGame.Click

            If hfPlayerID.Value = "" AndAlso ViewState("PlayerID") Is Nothing Then
                ClientAlert("Save Player's Information Before Update Limits Template, Please!", True)
                Return
            End If
            SetMaxGameValuesForLimitSetting()
            'Session("Updatelimit") = "Y"
            If UpdateMaxPerGameLimits() Then
                ClientAlert("Successfully Updated", True)
            Else
                ClientAlert("Unsuccessfully Updated", True)
            End If

        End Sub

    End Class

End Namespace