Imports SBCBL.std
Imports SBCBL.Managers
Imports System.Data
Imports SBCBL

Namespace SBSAgents
    Partial Class Inc_Agents_AgentEdit
        Inherits SBCBL.UI.CSBCUserControl
        Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())
        Public Event ButtonClick(ByVal sButtonType As String)
        'Private _SiteType As String
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

        Public Property SiteType() As CEnums.ESiteType
            Get
                Return SafeString(ViewState("SiteType"))
            End Get
            Set(ByVal value As CEnums.ESiteType)
                ViewState("SiteType") = value
            End Set
        End Property
#End Region

#Region "Page Events"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not Me.IsPostBack Then

                trBettingProfile.Visible = GetBettingProfileSubAgent(UserSession.AgentUserInfo.UserID)
                trPlayerTemplate.Visible = GetPlayerTemplateSubAgent(UserSession.AgentUserInfo.UserID)
                trMaxCreditSetting.Visible = GetHasCrediLimitSetting(UserSession.AgentUserInfo.UserID)
                trSubAgentEnable.Visible = GetAddNewSubAgent(UserSession.AgentUserInfo.UserID)
                bindPAgents()
                bindTimeZones()
                bindTemplates()
                If SiteType = CEnums.ESiteType.SBC Then
                    trCasino.Visible = False
                End If
                ClearAgentInfo()
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

        Public Sub bindPAgents()
            ddlPAgents.DataSource = loadAgents()
            ddlPAgents.DataTextField = "AgentName"
            ddlPAgents.DataValueField = "AgentID"
            ddlPAgents.DataBind()

        End Sub

        Private Function loadAgents() As DataTable
            Dim odtAgents As New DataTable
            odtAgents.Columns.Add("AgentID", GetType(Guid))
            odtAgents.Columns.Add("AgentName", GetType(String))

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
                ClientAlert("Login Name Is Required", True)
                txtName.Focus()
                Return False
            End If

            If (New CAgentManager).IsExistedLogin(txtLogin.Text, AgentID, std.GetSiteType) Then
                ClientAlert("Login Name Is Existed. Please Try A Different Name", True)
                txtLogin.Text = "" : txtLogin.Focus() : Return False
            End If

            If SafeString(txtSpecialKey.Text) = "" OrElse Not ValidLogin(txtSpecialKey.Text) Then
                ClientAlert("Assigned Group Letters Is Not Valid", True)
                txtSpecialKey.Focus()
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

            If UCase(Me.AgentID) = UCase(ddlPAgents.Value) Then
                ClientAlert("Please Pick Another Agent Parent", True)
                ddlPAgents.Focus()
                Return False
            End If

            If (New CAgentManager).IsExistedSpecialKey(SafeString(txtSpecialKey.Text), AgentID) Then
                ClientAlert("Assigned Group Letters Is Existed. Please Choose A Different One", True)
                txtSpecialKey.Focus()
                Return False
            End If

            If (chk10Acc.Checked Or chk20Acc.Checked) And ddlTemplates.SelectedValue = "" Then
                ClientAlert("Please Select Preset Player Template.", True)
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
            ddlPAgents.Value = UserSession.UserID
            hfPAgentID.Value = ""
            txtSpecialKey.Text = ""
            txtCurrentPlayerNumber.Text = ""
            txtSpecialKey.Enabled = True
            chk10Acc.Checked = False
            chk20Acc.Checked = False
            chk10Acc.Enabled = True
            chk20Acc.Enabled = True
            chkCasino.Checked = False
            ddlTemplates.SelectedValue = ""
            ddlTemplates.Enabled = True
            ddlTemplates.SelectedValue = ""
            ddlTemplates.Enabled = True
            chkIsEnableBettingProfile.Checked = False
            chkEnablePlayerTemplate.Checked = False
            chkEnablePlayerBlock.Checked = False
            chkMaxCreditSetting.Checked = False
            chkSubAgentEnable.Checked = False
            If ddlPAgents.Value <> "" Then
                Dim oAgent As SBCBL.CacheUtils.CAgent = UserSession.Cache.GetAgentInfo(ddlPAgents.Value)
                If oAgent IsNot Nothing Then
                    hfProfitPercentage.Value = SafeString(oAgent.ProfitPercentage)
                    hfGrossPercentage.Value = SafeString(oAgent.GrossPercentage)
                Else
                    hfProfitPercentage.Value = ""
                    hfGrossPercentage.Value = ""
                End If

            End If
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
                ddlTimeZone.Value = SafeString(.Rows(0)("TimeZone"))
                chkIsBettingLocked.Checked = SafeString(.Rows(0)("IsBettingLocked")) = "Y"
                chkIsLocked.Checked = SafeString(.Rows(0)("IsLocked")) = "Y"
                txtLockReason.Text = SafeString(.Rows(0)("LockReason"))
                hfPAgentID.Value = SafeString(.Rows(0)("ParentID"))
                txtCurrentPlayerNumber.Text = SafeInteger(.Rows(0)("CurrentPlayerNumber"))
                txtSpecialKey.Text = SafeString(.Rows(0)("SpecialKey"))
                ddlPAgents.Value = hfPAgentID.Value
                chkEnablePlayerTemplate.Checked = SafeBoolean(.Rows(0)("IsEnablePlayerTemplate"))
                chkIsEnableBettingProfile.Checked = SafeBoolean(.Rows(0)("IsEnableBettingProfile"))
                chkEnablePlayerBlock.Checked = SafeBoolean(.Rows(0)("IsEnableBlockPlayer"))
                chkCasino.Checked = SafeBoolean(.Rows(0)("HasCasino"))
                chkMaxCreditSetting.Checked = SafeBoolean(.Rows(0)("HasCrediLimitSetting"))
                chkSubAgentEnable.Checked = SafeBoolean(.Rows(0)("AddNewSubAgent"))
                psdPassword.Password = SafeString(.Rows(0)("Password"))
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

            Dim bSuccess As Boolean = False
            Dim oAgentManager As New CAgentManager()
            Dim nNumberAcc As Integer = 0

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
                "", SafeSingle(hfProfitPercentage.Value), SafeSingle(hfGrossPercentage.Value), SafeString(txtSpecialKey.Text), _
                    SafeInteger(txtCurrentPlayerNumber.Text), UserSession.UserID, SafeBoolean(chkEnablePlayerTemplate.Checked), _
                    SafeBoolean(chkEnablePlayerBlock.Checked), SafeBoolean(chkCasino.Checked), chkSubAgentEnable.Checked, (SafeBoolean(chkMaxCreditSetting.Checked)), IIf(chkIsEnableBettingProfile.Checked, "Y", "N"))
                Dim oCPlayermanager As New CPlayerManager()
                oCPlayermanager.UpdatePlayerCasino(sAgentID, SafeBoolean(chkCasino.Checked), UserSession.UserID)
                ClientAlert("Successfully Updated", True)
            Else 'Insert
                sAgentID = newGUID()
                bSuccess = oAgentManager.InsertAgent(sAgentID, SafeString(txtName.Text), SafeString(txtLogin.Text), _
                SafeString(psdPassword.Password), SafeInteger(ddlTimeZone.Value), chkIsBettingLocked.Checked, _
                chkIsLocked.Checked, SafeString(txtLockReason.Text), ddlPAgents.Value, "", _
                SafeSingle(hfProfitPercentage.Value), SafeSingle(hfGrossPercentage.Value), SafeString(txtSpecialKey.Text), _
                    SafeInteger(txtCurrentPlayerNumber.Text), UserSession.UserID, SiteType.ToString(), _
                    SafeBoolean(chkEnablePlayerTemplate.Checked), SafeBoolean(chkEnablePlayerBlock.Checked), chkRequireChangePass.Checked, SafeBoolean(chkCasino.Checked), chkSubAgentEnable.Checked, (SafeBoolean(chkMaxCreditSetting.Checked)), chkIsEnableBettingProfile.Checked)

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
              
                ClientAlert("Successfully Saved", True)
            End If

            Dim oPlayerManager As New CPlayerManager()
            oPlayerManager.CreatePresetPlayers(sAgentID, nNumberAcc, SafeString(txtSpecialKey.Text), SafeInteger(txtCurrentPlayerNumber.Text), SafeInteger(ddlTimeZone.Value), _
                                                ddlTemplates.SelectedValue, UserSession.UserID, SiteType.ToString(), chkRequireChangePass.Checked)

            ''rebind parent agents dropdown
            If hfPAgentID.Value <> ddlPAgents.Value Then
                bindPAgents()
            End If
            'make Folder For agent in SBS
            'If SiteType = CEnums.ESiteType.SBS Then
            '    MakeAgentFolder(sAgentID)
            'End If
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
            If ddlPAgents.Value <> "" Then
                Dim oAgent As SBCBL.CacheUtils.CAgent = UserSession.Cache.GetAgentInfo(ddlPAgents.Value)
                If oAgent IsNot Nothing Then
                    hfProfitPercentage.Value = SafeString(oAgent.ProfitPercentage)
                    hfGrossPercentage.Value = SafeString(oAgent.GrossPercentage)
                Else
                    hfProfitPercentage.Value = ""
                    hfGrossPercentage.Value = ""
                End If

            End If
        End Sub

        'Public Sub MakeAgentFolder(ByVal psAgentID As String)
        '    Try
        '        Dim oDB As New FileDB.CFileDB()
        '        Dim oHandle As FileDB.CFileHandle = oDB.GetNewFileHandle()
        '        oDB.CreateDatabase(SBS_COMMON + "\" & psAgentID)
        '        ''make file 
        '        Dim oXML As XDocument = <?xml version='1.0' encoding='UTF-8'?><root></root>
        '        oXML.Save(oHandle.LocalFileName)
        '        oDB.PutFile(String.Format("{0}\{1}", SBS_COMMON, psAgentID), "AgentSettingGame.xml", oHandle)
        '    Catch ex As Exception
        '        LogError(log, "Cannot make Folder for Agent", ex)
        '    End Try
        'End Sub


        


    End Class

End Namespace