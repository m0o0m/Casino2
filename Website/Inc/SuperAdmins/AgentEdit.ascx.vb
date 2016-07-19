Imports SBCBL.std
Imports SBCBL.Managers
Imports System.Data
Imports SBCBL

Namespace SBCSuperAdmin

    Partial Class AgentEdit
        Inherits SBCBL.UI.CSBCUserControl
        Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())
        Public Event ButtonClick(ByVal sButtonType As String)
        Dim strOffBefore As String = "OffBefore"
        Dim strDisplayBefore As String = "DisplayBefore"
        Dim strGameTotalPointsDisplay As String = "GameTotalPointsDisplay"


#Region "Properties"
        Public Property AgentID() As String
            Get
                Return SafeString(SafeString(ViewState("_EDIT_AGENTID")).Split("|"c)(0))
            End Get
            Set(ByVal value As String)
                ViewState("_EDIT_AGENTID") = value
            End Set
        End Property
        Public Property SiteType() As String
            Get
                Return SafeString(ViewState("SiteType"))
            End Get
            Set(ByVal value As String)
                ViewState("SiteType") = value
            End Set
        End Property
#End Region

#Region "Page Events"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                bindPAgents()
                bindTimeZones()
                bindTemplates()
                chk10Acc.Attributes("onclick") = String.Format("javascript:CheckPreset(this, '{0}')", chk20Acc.ClientID)
                chk20Acc.Attributes("onclick") = String.Format("javascript:CheckPreset(this, '{0}')", chk10Acc.ClientID)
                If SiteType.Equals("SBC") Then
                    trCasino.Visible = False
                    trHasGameManagement.Visible = False
                    trHasSystemManagement.Visible = False
                End If

            End If
        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            If AgentID <> "" Then
                btnSave.Text = "Update"
                btnSave.ToolTip = "Update agent"
            Else
                btnSave.Text = "Add"
                btnSave.ToolTip = "Create new agent"
            End If

            ibtGenerateLogin.Visible = AgentID <> ""
            trRequireChangePass.Visible = AgentID = ""
        End Sub

#End Region

#Region "Bind Data"

        Private Sub bindPAgents()
            Dim dtParents As DataTable = (New CAgentManager).GetAllAgentsBySuperAdminID(UserSession.UserID, Nothing)

            Dim odrParents As DataRow() = dtParents.Select("ParentID IS NULL", "AgentName")

            For Each drParent As DataRow In odrParents
                Dim sAgentID As String = SafeString(drParent("AgentID"))
                ddlPAgents.Items.Add(New ListItem(SafeString(drParent("AgentName")), sAgentID))

                loadSubAgent(sAgentID, dtParents)
            Next

            ddlPAgents.Items.Insert(0, "")
        End Sub

        Public Sub loadSubAgent(ByVal psParentAgentID As String, ByVal podtAgents As DataTable)
            Dim odrSubAgents As DataRow() = podtAgents.Select("ParentID=" & SQLString(psParentAgentID), "AgentName")

            For Each drChild As DataRow In odrSubAgents
                Dim sText As String = loopString("----", SafeInteger(drChild("AgentLevel")) - 1) & SafeString(drChild("AgentName"))
                Dim sAgentID As String = SafeString(drChild("AgentID"))

                ddlPAgents.Items.Add(New ListItem(sText, sAgentID))

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

        Private Sub bindTimeZones()
            ddlTimeZone.DataSource = UserSession.SysSettings("TimeZone")
            ddlTimeZone.DataTextField = "Key"
            ddlTimeZone.DataValueField = "Value"
            ddlTimeZone.DataBind()
        End Sub

        Private Sub bindTemplates()
            ddlTemplates.DataSource = (New CPlayerTemplateManager).GetPlayerTemplates(SBCBL.std.GetSiteType)
            ddlTemplates.DataTextField = "TemplateName"
            ddlTemplates.DataValueField = "PlayerTemplateID"
            ddlTemplates.DataBind()
        End Sub
#End Region

#Region "Edit Agent"

        Private Function checkCondition() As Boolean
            If SafeString(txtName.Text) = "" Then
                ClientAlert("Agent Name Is Required", True)
                txtName.Focus()
                Return False
            End If
            If (New CAgentManager).IsExistedLogin(txtLogin.Text, AgentID, GetSiteType) Then
                ClientAlert("Login Name Is Existed. Please Try  A Different Name ", True)
                txtLogin.Focus()
                Return False
            End If
            If AgentID = "" Then
                If SafeString(psdPassword.Password) = "" Then
                    ClientAlert("Password Is Required", True)
                    psdPassword.Focus()
                    Return False
                End If

                If Not ValidPassword(psdPassword.Password) Then
                    ClientAlert("Password Is Not Valid", True)
                    psdPassword.Focus()
                    Return False
                End If
            End If
            If Not CAgentManager.CheckDuplicateLogin(SafeString(txtLogin.Text), AgentID, SBCBL.std.GetSiteType) Then
                ClientAlert("Login Name Is Existed", True)
                Return False
            End If

            If SafeString(txtSpecialKey.Text) = "" OrElse Not ValidLogin(txtSpecialKey.Text) Then
                ClientAlert("Assigned Group Letters Is Not Valid", True)
                txtSpecialKey.Focus()
                Return False
            End If

            If Me.AgentID <> "" Then
                If UCase(Me.AgentID) = UCase(ddlPAgents.Value) Then
                    ClientAlert("Please Pick Another Agent Parent", True)
                    ddlPAgents.Focus()
                    Return False
                End If
            End If

            If (New CAgentManager).IsExistedSpecialKey(SafeString(txtSpecialKey.Text), AgentID) Then
                ClientAlert("Assigned Group Letters Is Existed. Please Choose A Different One", True)
                txtSpecialKey.Focus()
                Return False
            End If

            If (chk10Acc.Checked Or chk20Acc.Checked) And ddlTemplates.SelectedValue = "" Then
                ClientAlert("Please Select Preset Player Template.")
                Return False
            End If

            Return True
        End Function

        Public Sub ClearAgentInfo()
            AgentID = ""
            txtLogin.Text = ""
            txtName.Text = ""
            psdPassword.Password = ""
            ddlTimeZone.SelectedIndex = -1
            chkIsBettingLocked.Checked = False
            chkIsLocked.Checked = False
            txtLockReason.Text = ""
            ddlPAgents.Value = ""
            hfPAgentID.Value = ""
            txtProfitPercentage.Text = ""
            txtGrossPercentage.Text = ""
            txtSpecialKey.Text = ""
            txtCurrentPlayerNumber.Text = ""
            txtSpecialKey.Enabled = True
            chk10Acc.Checked = False
            chk20Acc.Checked = False
            chk10Acc.Enabled = True
            chk20Acc.Enabled = True
            ddlTemplates.SelectedValue = ""
            ddlTemplates.Enabled = True
            chkEnablePlayerTemplate.Checked = False
            chkEnablePlayerBlock.Checked = False
            chkIsEnableBettingProfile.Checked = False
            chkCasino.Checked = False
            chkHasGameManagement.Checked = False
            chkHasSystemManagement.Checked = False
            chkIsEnableChangeBookmaker.Checked = False
            chkMaxCreditSetting.Checked = False
            chkSubAgentEnable.Checked = False
        End Sub

        Public Function LoadAgentInfo(ByVal psAgentID As String) As Boolean
            AgentID = psAgentID

            Dim oAgentManager As New CAgentManager()
            Dim oData As DataTable = oAgentManager.GetByID(AgentID)

            If oData Is Nothing OrElse oData.Rows.Count = 0 Then
                ClearAgentInfo()
                Return False
            End If

            With oData
                txtLogin.Text = SafeString(.Rows(0)("Login"))
                txtName.Text = SafeString(.Rows(0)("Name"))
                psdPassword.Password = SafeString(.Rows(0)("Password"))
                'psdPassword.ShowUserPassword(SafeString(.Rows(0)("Password")))
                ddlTimeZone.Value = SafeString(.Rows(0)("TimeZone"))
                chkIsBettingLocked.Checked = SafeString(.Rows(0)("IsBettingLocked")) = "Y"
                chkIsLocked.Checked = SafeString(.Rows(0)("IsLocked")) = "Y"
                txtLockReason.Text = SafeString(.Rows(0)("LockReason"))
                hfPAgentID.Value = SafeString(.Rows(0)("ParentID"))
                txtProfitPercentage.Text = SafeString(.Rows(0)("ProfitPercentage"))
                txtGrossPercentage.Text = SafeString(.Rows(0)("GrossPercentage"))
                txtCurrentPlayerNumber.Text = SafeInteger(.Rows(0)("CurrentPlayerNumber"))
                txtSpecialKey.Text = SafeString(.Rows(0)("SpecialKey"))
                ddlPAgents.Value = hfPAgentID.Value
                chkEnablePlayerTemplate.Checked = SafeBoolean(.Rows(0)("IsEnablePlayerTemplate"))
                chkEnablePlayerBlock.Checked = SafeBoolean(.Rows(0)("IsEnableBlockPlayer"))
                chkIsEnableBettingProfile.Checked = SafeBoolean(.Rows(0)("IsEnableBettingProfile"))
                chkCasino.Checked = SafeBoolean(.Rows(0)("HasCasino"))
                chkHasGameManagement.Checked = SafeBoolean(.Rows(0)("HasGameManagement"))
                chkHasSystemManagement.Checked = SafeBoolean(.Rows(0)("HasSystemManagement"))
                chkIsEnableChangeBookmaker.Checked = SafeBoolean(.Rows(0)("IsEnableChangeBookmaker"))
                chkMaxCreditSetting.Checked = SafeBoolean(.Rows(0)("HasCrediLimitSetting"))
                chkSubAgentEnable.Checked = SafeBoolean(.Rows(0)("AddNewSubAgent"))

            End With

            txtSpecialKey.Enabled = Not oAgentManager.NumOfSubPlayers(psAgentID) > 0

            '' dont allow to create preset player if the agent has players already
            If psAgentID <> "" AndAlso oAgentManager.HasPlayers(psAgentID) Then
                chk10Acc.Enabled = False
                chk20Acc.Enabled = False
                ddlTemplates.Enabled = False
            Else
                chk10Acc.Enabled = True
                chk20Acc.Enabled = True
                ddlTemplates.Enabled = True
            End If

            Return True
        End Function
#End Region

        Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
            If Not checkCondition() Then
                Return
            End If
            Dim nNumberAcc As Integer = 0
            Dim bSuccess As Boolean = False
            Dim oAgentManager As New CAgentManager()
            Dim bPresetSuccess As Boolean = False


            If chk10Acc.Checked Then
                nNumberAcc = 10
            ElseIf chk20Acc.Checked Then
                nNumberAcc = 20
            End If

            Dim sAgentID As String = AgentID
            If sAgentID <> "" Then 'Update
                bSuccess = oAgentManager.UpdateAgent(sAgentID, SafeString(txtName.Text), _
                    SafeString(txtLogin.Text), SafeString(psdPassword.Password), SafeInteger(ddlTimeZone.Value), _
                    chkIsBettingLocked.Checked, chkIsLocked.Checked, SafeString(txtLockReason.Text), ddlPAgents.Value, _
                    UserSession.UserID, SafeSingle(txtProfitPercentage.Text), SafeSingle(txtGrossPercentage.Text), _
                    SafeString(txtSpecialKey.Text), SafeInteger(txtCurrentPlayerNumber.Text), _
                     UserSession.UserID, SafeBoolean(chkEnablePlayerTemplate.Checked), SafeBoolean(chkEnablePlayerBlock.Checked), _
                     SafeBoolean(chkCasino.Checked), chkSubAgentEnable.Checked, SafeBoolean(chkMaxCreditSetting.Checked), IIf(chkIsEnableBettingProfile.Checked, "Y", "N"), SafeBoolean(chkHasGameManagement.Checked), _
                    SafeBoolean(chkHasSystemManagement.Checked), SafeBoolean(chkIsEnableChangeBookmaker.Checked))

                oAgentManager.UpdateSubAgentsPercent(sAgentID, SafeSingle(txtProfitPercentage.Text), _
                                                     SafeSingle(txtGrossPercentage.Text), UserSession.UserID)
                Dim oCPlayermanager As New CPlayerManager()
                oCPlayermanager.UpdatePlayerCasino(sAgentID, SafeBoolean(chkCasino.Checked), UserSession.UserID)
                UserSession.Cache.ClearAgentInfo(sAgentID, SafeString(txtLogin.Text))
                ClientAlert("Successfully Updated", True)
            Else 'Insert
                sAgentID = newGUID()
                bSuccess = oAgentManager.InsertAgent(sAgentID, SafeString(txtName.Text), SafeString(txtLogin.Text), _
                    SafeString(psdPassword.Password), SafeInteger(ddlTimeZone.Value), chkIsBettingLocked.Checked, _
                    chkIsLocked.Checked, SafeString(txtLockReason.Text), ddlPAgents.Value, UserSession.UserID, _
                    SafeSingle(txtProfitPercentage.Text), SafeSingle(txtGrossPercentage.Text), _
                    SafeString(txtSpecialKey.Text), SafeInteger(txtCurrentPlayerNumber.Text), _
                    UserSession.UserID, SiteType.ToString(), SafeBoolean(chkEnablePlayerTemplate.Checked), _
                    SafeBoolean(chkEnablePlayerBlock.Checked), chkRequireChangePass.Checked, SafeBoolean(chkCasino.Checked), chkSubAgentEnable.Checked, SafeBoolean(chkMaxCreditSetting.Checked), chkIsEnableBettingProfile.Checked, _
                    SafeBoolean(chkHasGameManagement.Checked), SafeBoolean(chkHasSystemManagement.Checked), SafeBoolean(chkIsEnableChangeBookmaker.Checked))
                Dim oCSysSettingManager As CSysSettingManager = New CSysSettingManager()
                oCSysSettingManager.CopyParplayReverser(SafeString(sAgentID))
                '  ClientAlert("Save user's info successfully", True)
                Dim oSysManager As New CSysSettingManager()
                Dim oGameTypes As Dictionary(Of String, String) = GetGameType()
                For Each kGameType As KeyValuePair(Of String, String) In oGameTypes
                    If IsSoccer(kGameType.Value) Then
                        bSuccess = oSysManager.AddSysSetting(sAgentID & "LineOffHour", strOffBefore, "0", "Current", 0, kGameType.Value)
                        bSuccess = oSysManager.AddSysSetting(sAgentID & "LineOffHour", strDisplayBefore, "24", "Current", 0, kGameType.Value)
                        bSuccess = oSysManager.AddSysSetting(sAgentID & "LineOffHour", strGameTotalPointsDisplay, "24", "Current", 0, kGameType.Value)
                    ElseIf IsOtherGameType(kGameType.Value) Then
                        bSuccess = oSysManager.AddSysSetting(sAgentID & "LineOffHour", strOffBefore, "0", "Current", 0, kGameType.Value)
                        bSuccess = oSysManager.AddSysSetting(sAgentID & "LineOffHour", strDisplayBefore, "24", "Current", 0, kGameType.Value)
                        bSuccess = oSysManager.AddSysSetting(sAgentID & "LineOffHour", strGameTotalPointsDisplay, "0", "Current", 0, kGameType.Value)

                    Else
                        bSuccess = oSysManager.AddSysSetting(sAgentID & "LineOffHour", strOffBefore, "0", "Current", 0, kGameType.Value)
                        bSuccess = oSysManager.AddSysSetting(sAgentID & "LineOffHour", strDisplayBefore, "24", "Current", 0, kGameType.Value)
                        bSuccess = oSysManager.AddSysSetting(sAgentID & "LineOffHour", strGameTotalPointsDisplay, "12", "Current", 0, kGameType.Value)
                    End If
                    bSuccess = oSysManager.AddSysSetting(sAgentID & "LineOffHour", strOffBefore, "0", "1H", 0, kGameType.Value)
                    bSuccess = oSysManager.AddSysSetting(sAgentID & "LineOffHour", strDisplayBefore, "12", "1H", 0, kGameType.Value)
                    bSuccess = oSysManager.AddSysSetting(sAgentID & "LineOffHour", strGameTotalPointsDisplay, "8", "1H", 0, kGameType.Value)
                Next
            End If
            Dim oPlayerManager As New CPlayerManager()
            bPresetSuccess = oPlayerManager.CreatePresetPlayers(sAgentID, nNumberAcc, SafeString(txtSpecialKey.Text), _
                            SafeInteger(txtCurrentPlayerNumber.Text), SafeInteger(ddlTimeZone.Value), _
                            ddlTemplates.SelectedValue, UserSession.UserID, SiteType.ToString(), chkRequireChangePass.Checked)
            If AgentID = "" Then
                If bSuccess And bPresetSuccess Then
                    ClientAlert("Successfully Saved", True)
                ElseIf bSuccess And Not bPresetSuccess Then
                    ClientAlert("Can't Create Preset Users.", True)
                ElseIf Not bSuccess Then
                    ClientAlert("Unsuccessfully Updated.", True)
                End If
            Else
                If bSuccess Then
                    ClientAlert("Successfully Saved", True)
                Else
                    ClientAlert("User's info unsuccessfully updated.", True)
                End If
            End If

            ''rebind parent agents dropdown
            If hfPAgentID.Value <> ddlPAgents.Value Then
                bindPAgents()
            End If
            'make Folder For agent in SBS
            If SiteType.Equals("SBS") Then
                MakeAgentFolder(sAgentID)
            End If

            RaiseEvent ButtonClick(SafeString(IIf(bSuccess, "SAVE SUCCESSFUL", "SAVE UNSUCCESSFUL")))
        End Sub

        Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
            ClearAgentInfo()
            RaiseEvent ButtonClick("CANCEL")
        End Sub

        Protected Sub ibtGenerateLogin_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ibtGenerateLogin.Click
            Dim sName As String = SafeString(txtName.Text)

            If sName = "" Then
                ClientAlert("Login Name Is Required To Auto Generate Login", True)
                txtName.Focus()
                Return
            End If

            txtLogin.Text = (New CPlayerManager).AutoGenerateLogin(sName, SBCBL.std.GetSiteType)
        End Sub

        Protected Sub txtName_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtName.TextChanged
            If AgentID <> "" Then
                Return
            End If

            ibtGenerateLogin_Click(Nothing, Nothing)
        End Sub

        Protected Sub ddlPAgents_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlPAgents.SelectedIndexChanged
            txtProfitPercentage.Enabled = ddlPAgents.Value <> ""
            txtGrossPercentage.Enabled = ddlPAgents.Value <> ""
            If ddlPAgents.Value <> "" Then
                Dim oAgent As SBCBL.CacheUtils.CAgent = UserSession.Cache.GetAgentInfo(ddlPAgents.Value)
                If oAgent IsNot Nothing Then
                    txtProfitPercentage.Text = SafeString(oAgent.ProfitPercentage)
                    txtGrossPercentage.Text = SafeString(oAgent.GrossPercentage)
                Else
                    txtProfitPercentage.Text = ""
                    txtGrossPercentage.Text = ""
                End If
                
            End If
        End Sub

        Private Sub saveJuiceControll(ByVal psAgentID As String, ByVal psSportType As String, ByVal pnJiuce As Integer, ByVal psContext As String)
            'Dim oGameTypeOnOff As New CGameTypeOnOffManager()
            'If oGameTypeOnOff.CheckExistsJuiceControlByAgent(psAgentID, psSportType, psContext) Then
            '    oGameTypeOnOff.UpdateJuiceControl(psAgentID, psSportType, pnJiuce, psContext)
            'Else
            '    oGameTypeOnOff.InsertJuiceControl(psAgentID, psSportType, pnJiuce, psContext)
            'End If
            'UserSession.Cache.ClearJuiceControl(psAgentID, psSportType, psContext)
            Dim oSysManager As New CSysSettingManager()
            If oSysManager.CheckExistSysSetting(psAgentID & "_Juice", psSportType, psContext) Then
                LogDebug(log, "save juice")
                oSysManager.UpdateValue(psAgentID & "_Juice", psSportType, psContext, pnJiuce)
            Else
                LogDebug(log, "insert juice")
                oSysManager.AddSysSetting(psAgentID & "_Juice", psContext, pnJiuce, psSportType)

            End If
            UserSession.Cache.ClearSysSettings(psAgentID & "_Juice", psSportType)
            UserSession.Cache.ClearJuiceControl(UCase(psAgentID), UCase(psSportType), UCase(psContext), "")
        End Sub

        Public Sub MakeAgentFolder(ByVal psAgentID As String)
            Try
                Dim oDB As New FileDB.CFileDB()
                Dim oHandle As FileDB.CFileHandle = oDB.GetNewFileHandle()
                If oDB.Exist(SBS_SYSTEM + "\" & psAgentID) Then
                    Return
                End If
                oDB.CreateDatabase(SBS_SYSTEM + "\" & psAgentID)
                ''make file 
                Dim oXML As XDocument = <?xml version='1.0' encoding='UTF-8'?><root></root>
                oXML.Save(oHandle.LocalFileName)
                oDB.PutFile(String.Format("{0}\{1}", SBS_SYSTEM, psAgentID), "AgentSettingGame.xml", oHandle)

                oHandle = oDB.GetFile(SBS_SYSTEM & "\" & psAgentID, "AgentSettingGame.xml")
                IO.File.WriteAllText(oHandle.LocalFileName, String.Format("<?xml version=""1.0"" encoding=""UTF-8""?><root>{0}</root>", std.GetTeamplateAgentSetting()))
                oDB.PutFile(String.Format("{0}\{1}", SBS_SYSTEM, psAgentID), "AgentSettingGame.xml", oHandle)

            Catch ex As Exception
                LogError(log, "Cannot make Folder for Agent", ex)
            End Try
            Try
                saveJuiceControll(psAgentID, "Baseball", -4, "Current")
                saveJuiceControll(psAgentID, "Basketball", 0, "Current")
                saveJuiceControll(psAgentID, "Football", 0, "Current")
                saveJuiceControll(psAgentID, "Soccer", -8, "Current")
                saveJuiceControll(psAgentID, "Hockey", -4, "Current")
                saveJuiceControll(psAgentID, "Golf", -8, "Current")
                saveJuiceControll(psAgentID, "Tennis", -8, "Current")
                saveJuiceControll(psAgentID, "Boxing", -8, "Current")
                saveJuiceControll(psAgentID, "Nascar", -8, "Current")

                saveJuiceControll(psAgentID, "Baseball", -4, "1H")
                saveJuiceControll(psAgentID, "Basketball", 0, "1H")
                saveJuiceControll(psAgentID, "Football", 0, "1H")
                saveJuiceControll(psAgentID, "Soccer", -8, "1H")
                saveJuiceControll(psAgentID, "Hockey", -4, "1H")
                saveJuiceControll(psAgentID, "Golf", -8, "1H")
                saveJuiceControll(psAgentID, "Tennis", -8, "1H")
                saveJuiceControll(psAgentID, "Boxing", -8, "1H")
                saveJuiceControll(psAgentID, "Nascar", -8, "1H")

                saveJuiceControll(psAgentID, "Baseball", -4, "2H")
                saveJuiceControll(psAgentID, "Basketball", 0, "2H")
                saveJuiceControll(psAgentID, "Football", 0, "2H")
                saveJuiceControll(psAgentID, "Soccer", -8, "2H")
                saveJuiceControll(psAgentID, "Hockey", -4, "2H")
                saveJuiceControll(psAgentID, "Golf", -8, "2H")
                saveJuiceControll(psAgentID, "Tennis", -8, "2H")
                saveJuiceControll(psAgentID, "Boxing", -8, "2H")
                saveJuiceControll(psAgentID, "Nascar", -8, "2H")
            Catch ex As Exception
                LogError(log, "Cannot make Default Juice for Agent", ex)
            End Try
        End Sub

    End Class

End Namespace