Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports SBCBL.std
Imports System.Data

Namespace SBSAgents

    Partial Class Inc_Agents_TimeLineOff
        Inherits SBCBL.UI.CSBCUserControl
        Private _sGameType As String = SBCBL.std.GetSiteType & " GameType"
        Dim _sCategory As String = SBCBL.std.GetSiteType & " LineOffHour"
        Dim _log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())
        Dim strOffBefore As String = "OffBefore"
        Dim strDisplayBefore As String = "DisplayBefore"
        Dim strGameTotalPointsDisplay As String = "GameTotalPointsDisplay"
        Dim _eContext As EContext
        Dim oSysManager As New CSysSettingManager()

#Region "Property"

        Public Property ContextDisPlay() As EContext
            Get
                Return _eContext
            End Get
            Set(ByVal value As EContext)
                _eContext = value
            End Set
        End Property

        Public ReadOnly Property bCurrent() As Boolean
            Get
                Return IIf(ContextDisPlay = EContext.Current, True, False)

            End Get
        End Property
        Public ReadOnly Property SuperUserID() As String
            Get
                If UserSession.UserType = SBCBL.EUserType.Agent Then
                    Return UserSession.UserID & "LineOffHour"
                Else
                    Return ddlAgents.SelectedValue & "LineOffHour"
                End If
            End Get
        End Property

        Public Property IsSuperAdmin() As Boolean
            Get
                If ViewState("ISSUPERADMIN") IsNot Nothing Then
                    Return CType(ViewState("ISSUPERADMIN"), Boolean)
                Else
                    Return False
                End If
            End Get
            Set(ByVal value As Boolean)
                ViewState("ISSUPERADMIN") = value
            End Set
        End Property


        Private ReadOnly Property UserID() As String
            Get
                If Me.IsSuperAdmin Then
                    Return SafeString(ddlAgents.SelectedValue)
                Else
                    Return UserSession.UserID
                End If
            End Get
        End Property

#End Region

#Region "Page Events"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                trAgents.Visible = Me.IsSuperAdmin
                hfIsSuperAdmin.Value = Me.IsSuperAdmin

                bindGameType()
                '  LoadSecondHalfTime()
                BindTimeOff()
            End If
        End Sub

#End Region

#Region "Controls Events"

        Protected Sub ddlAgents_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlAgents.SelectedIndexChanged
            '  LoadSecondHalfTime()
            Dim oSysSettingManager As New CSysSettingManager()
            'Dim olstGameTypeOnOff As List(Of CGameTypeOnOff)
            ' If UserSession.UserType = SBCBL.EUserType.Agent Then
            'If ddlGameType.SelectedValue.Equals("Soccer", StringComparison.CurrentCultureIgnoreCase) Then
            '    olstGameTypeOnOff = UserSession.Cache.GetGameTypeOnOff(Me.UserID, "Premier", bCurrent)
            'Else
            '    olstGameTypeOnOff = UserSession.Cache.GetGameTypeOnOff(Me.UserID, SafeString(ddlGameType.SelectedValue), bCurrent)
            'End If
            If ddlGameType.SelectedValue.Equals("Soccer", StringComparison.CurrentCultureIgnoreCase) Then
                txtOffMinutes.Text = UserSession.Cache.GetSysSettings(SuperUserID, IIf(bCurrent, "Current", "1H")).GetIntegerValue(strOffBefore, "Japan")
                txtDisplayHours.Text = UserSession.Cache.GetSysSettings(SuperUserID, IIf(bCurrent, "Current", "1H")).GetIntegerValue(strDisplayBefore, "Japan")
                txtDisplayTotalpoints.Text = UserSession.Cache.GetSysSettings(SuperUserID, IIf(bCurrent, "Current", "1H")).GetIntegerValue(strGameTotalPointsDisplay, "Japan")
            Else
                txtOffMinutes.Text = UserSession.Cache.GetSysSettings(SuperUserID, IIf(bCurrent, "Current", "1H")).GetIntegerValue(strOffBefore, ddlGameType.SelectedValue)
                txtDisplayHours.Text = UserSession.Cache.GetSysSettings(SuperUserID, IIf(bCurrent, "Current", "1H")).GetIntegerValue(strDisplayBefore, ddlGameType.SelectedValue)
                txtDisplayTotalpoints.Text = UserSession.Cache.GetSysSettings(SuperUserID, IIf(bCurrent, "Current", "1H")).GetIntegerValue(strGameTotalPointsDisplay, ddlGameType.SelectedValue)
            End If


            'Else
            'If ddlGameType.SelectedValue.Equals("Soccer", StringComparison.CurrentCultureIgnoreCase) Then
            '    olstGameTypeOnOff = UserSession.Cache.GetGameTypeOnOff(ddlAgents.SelectedValue, "Premier", bCurrent)
            'Else
            '    olstGameTypeOnOff = UserSession.Cache.GetGameTypeOnOff(ddlAgents.SelectedValue, SafeString(ddlGameType.SelectedValue), bCurrent)
            'End If
            'End If


            'If olstGameTypeOnOff IsNot Nothing AndAlso olstGameTypeOnOff.Count > 0 Then
            '    Dim oGameTypeOnOff As CGameTypeOnOff = olstGameTypeOnOff(0)
            '    txtOffMinutes.Text = oGameTypeOnOff.OffBefore
            '    txtDisplayHours.Text = oGameTypeOnOff.DisplayBefore
            '    txtDisplayTotalpoints.Text = oGameTypeOnOff.GameTotalPointsDisplay
            'Else
            '    Dim oSys As CSysSetting = oSysSettingManager.GetAllSysSettings(_sCategory).Find(Function(x) x.Key = SafeString(ddlGameType.SelectedValue))
            '    If oSys IsNot Nothing AndAlso oSys.Value <> "" Then
            '        txtOffMinutes.Text = SafeInteger(oSys.Value)
            '        txtDisplayHours.Text = SafeInteger(oSys.SubCategory)
            '        txtDisplayTotalpoints.Text = SafeInteger(oSys.SubCategory)
            '    Else
            '        txtOffMinutes.Text = ""
            '        txtDisplayHours.Text = ""
            '        txtDisplayTotalpoints.Text = ""
            '    End If
            'End If

            BindTimeOff()

        End Sub

        Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click

            If String.IsNullOrEmpty(txtOffMinutes.Text) Then
                ClientAlert("Game time off line not valid")
                Return
            End If
            If String.IsNullOrEmpty(txtDisplayTotalpoints.Text) Then
                ClientAlert("Game time display not valid")
                Return
            End If

            Dim bSuccess As Boolean = False
            If ddlAgents.Visible AndAlso ddlAgents.SelectedValue = "All" Then
                bSuccess = saveAllAgent()
            Else

                If (ddlGameType.SelectedValue.Equals("Soccer", StringComparison.CurrentCultureIgnoreCase)) Then
                    Dim oDicSoccerGameType As Dictionary(Of String, String) = CType(ViewState("oDicSoccerGameType"), Dictionary(Of String, String))
                    For Each strGameType As String In oDicSoccerGameType.Keys
                        If oSysManager.IsExistSysSetting(SuperUserID, IIf(bCurrent, "Current", "1H"), strGameType) Then
                            'If oGameTypeOnOffManager.GetGameTypeOnOffByGameType(Me.UserID, strGameType, bCurrent).Rows.Count > 0 Then
                            'bSuccess = oSysManager.UpdateValue(SuperUserID, _
                            '            SafeString(strGameType), _
                            '            SafeInteger(txtOffMinutes.Text), SafeInteger(txtDisplayHours.Text), SafeInteger(txtDisplayTotalpoints.Text), bCurrent)
                            LogDebug(_log, "update")

                            bSuccess = oSysManager.UpdateValue(SuperUserID, IIf(bCurrent, "Current", "1H"), strOffBefore, txtOffMinutes.Text, "", strGameType)
                            bSuccess = oSysManager.UpdateValue(SuperUserID, IIf(bCurrent, "Current", "1H"), strDisplayBefore, txtDisplayHours.Text, "", strGameType)
                            bSuccess = oSysManager.UpdateValue(SuperUserID, IIf(bCurrent, "Current", "1H"), strGameTotalPointsDisplay, txtDisplayTotalpoints.Text, "", strGameType)
                        Else
                            LogDebug(_log, "insert" & strGameType)
                            'bSuccess = oSysManager.AddSysSetting(SuperUserID, _
                            '                                                            SafeString(strGameType), _
                            '                                                            SafeInteger(txtOffMinutes.Text), _
                            '                                                            SafeInteger(txtDisplayHours.Text), SafeInteger(txtDisplayTotalpoints.Text), bCurrent)
                            'ClientAlert(strGameType, True)
                            bSuccess = oSysManager.AddSysSetting(SuperUserID, strOffBefore, txtOffMinutes.Text, IIf(bCurrent, "Current", "1H"), 0, strGameType)
                            bSuccess = oSysManager.AddSysSetting(SuperUserID, strDisplayBefore, txtDisplayHours.Text, IIf(bCurrent, "Current", "1H"), 0, strGameType)
                            bSuccess = oSysManager.AddSysSetting(SuperUserID, strGameTotalPointsDisplay, txtDisplayTotalpoints.Text, IIf(bCurrent, "Current", "1H"), 0, strGameType)
                        End If
                        UserSession.Cache.ClearGameTypeOnOff(SuperUserID, SafeString(strGameType), bCurrent)
                    Next
                Else
                    'If oGameTypeOnOffManager.GetGameTypeOnOffByGameType(Me.UserID, ddlGameType.SelectedValue, bCurrent).Rows.Count > 0 Then
                    If oSysManager.IsExistSysSetting(SuperUserID, IIf(bCurrent, "Current", "1H"), ddlGameType.SelectedValue) Then
                        'bSuccess = oGameTypeOnOffManager.UpdateGameTypeOnOffByAgent(Me.UserID, _
                        '                                                            SafeString(ddlGameType.SelectedValue), _
                        '                                                            SafeInteger(txtOffMinutes.Text), _
                        '                                                            SafeInteger(txtDisplayHours.Text), SafeInteger(txtDisplayTotalpoints.Text), bCurrent)
                        LogDebug(_log, "update")
                        bSuccess = oSysManager.UpdateValue(SuperUserID, IIf(bCurrent, "Current", "1H"), strOffBefore, txtOffMinutes.Text, "", ddlGameType.SelectedValue)
                        bSuccess = oSysManager.UpdateValue(SuperUserID, IIf(bCurrent, "Current", "1H"), strDisplayBefore, txtDisplayHours.Text, "", ddlGameType.SelectedValue)
                        bSuccess = oSysManager.UpdateValue(SuperUserID, IIf(bCurrent, "Current", "1H"), strGameTotalPointsDisplay, txtDisplayTotalpoints.Text, "", ddlGameType.SelectedValue)
                    Else
                        'bSuccess = oGameTypeOnOffManager.InsertGameTypeOnOffByAgent(Me.UserID, _
                        '                                                            newGUID(), _
                        '                                                            SafeString(ddlGameType.SelectedValue), _
                        '                                                            SafeInteger(txtOffMinutes.Text), _
                        '                                                            SafeInteger(txtDisplayHours.Text), SafeInteger(txtDisplayTotalpoints.Text), bCurrent)
                        LogDebug(_log, "insert")
                        bSuccess = oSysManager.AddSysSetting(SuperUserID, strOffBefore, txtOffMinutes.Text, IIf(bCurrent, "Current", "1H"), 0, ddlGameType.SelectedValue)
                        bSuccess = oSysManager.AddSysSetting(SuperUserID, strDisplayBefore, txtDisplayHours.Text, IIf(bCurrent, "Current", "1H"), 0, ddlGameType.SelectedValue)
                        bSuccess = oSysManager.AddSysSetting(SuperUserID, strGameTotalPointsDisplay, txtDisplayTotalpoints.Text, IIf(bCurrent, "Current", "1H"), 0, ddlGameType.SelectedValue)
                    End If
                End If
                UserSession.Cache.ClearGameTypeOnOff(SuperUserID, SafeString(ddlGameType.SelectedValue), bCurrent)
                UserSession.Cache.ClearSysSettings(SuperUserID, SafeString(IIf(bCurrent, "Current", "1H")))
            End If



            BindTimeOff()

            If bSuccess Then
                ClientAlert("Successfully Saved", True)
            Else
                ClientAlert("Failed to Save Setting", True)
            End If

        End Sub

        Protected Sub ddlGameType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlGameType.SelectedIndexChanged
            Dim oSysSettingManager As New CSysSettingManager()
            'Dim olstGameTypeOnOff As List(Of CGameTypeOnOff)
            'If ddlGameType.SelectedValue.Equals("Soccer", StringComparison.CurrentCultureIgnoreCase) Then
            '    olstGameTypeOnOff = UserSession.Cache.GetGameTypeOnOff(Me.UserID, "Premier", bCurrent)
            'Else
            '    olstGameTypeOnOff = UserSession.Cache.GetGameTypeOnOff(Me.UserID, SafeString(ddlGameType.SelectedValue), bCurrent)
            'End If
            'ClientAlert(UserSession.Cache.GetSysSettings(SuperUserID, IIf(bCurrent, "Current", "1H")).Count, True)
            'UserSession.Cache.ClearSysSettings(SuperUserID, IIf(bCurrent, "Current", "1H"))
            'ClientAlert((UserSession.Cache.GetSysSettings(ddlAgents.SelectedValue & "LineOffHour", "Current").GetIntegerValue("DisplayBefore", ddlGameType.SelectedValue) * 60).ToString(), True)
            'ClientAlert((UserSession.Cache.GetSysSettings(SuperUserID, "Current").GetIntegerValue("DisplayBefore", ddlGameType.SelectedValue) * 60).ToString(), True)
            'ClientAlert(ddlAgents.SelectedValue & "LineOffHour", True)
            'ClientAlert(SuperUserID, True)
            If ddlGameType.SelectedValue.Equals("Soccer", StringComparison.CurrentCultureIgnoreCase) Then
                txtOffMinutes.Text = UserSession.Cache.GetSysSettings(SuperUserID, IIf(bCurrent, "Current", "1H")).GetIntegerValue(strOffBefore, "Japan")
                txtDisplayHours.Text = UserSession.Cache.GetSysSettings(SuperUserID, IIf(bCurrent, "Current", "1H")).GetIntegerValue(strDisplayBefore, "Japan")
                txtDisplayTotalpoints.Text = UserSession.Cache.GetSysSettings(SuperUserID, IIf(bCurrent, "Current", "1H")).GetIntegerValue(strGameTotalPointsDisplay, "Japan")
            Else

                txtOffMinutes.Text = UserSession.Cache.GetSysSettings(SuperUserID, IIf(bCurrent, "Current", "1H")).GetIntegerValue(strOffBefore, ddlGameType.SelectedValue)
                txtDisplayHours.Text = UserSession.Cache.GetSysSettings(SuperUserID, IIf(bCurrent, "Current", "1H")).GetIntegerValue(strDisplayBefore, ddlGameType.SelectedValue)
                txtDisplayTotalpoints.Text = UserSession.Cache.GetSysSettings(SuperUserID, IIf(bCurrent, "Current", "1H")).GetIntegerValue(strGameTotalPointsDisplay, ddlGameType.SelectedValue)
            End If


            'If olstGameTypeOnOff IsNot Nothing AndAlso olstGameTypeOnOff.Count > 0 Then
            '    Dim oGameTypeOnOff As CGameTypeOnOff = olstGameTypeOnOff(0)
            '    txtOffMinutes.Text = oGameTypeOnOff.OffBefore
            '    txtDisplayHours.Text = oGameTypeOnOff.DisplayBefore
            '    txtDisplayTotalpoints.Text = oGameTypeOnOff.GameTotalPointsDisplay
            'Else
            '    Dim oSys As CSysSetting = oSysSettingManager.GetAllSysSettings(_sCategory).Find(Function(x) x.Key = SafeString(ddlGameType.SelectedValue))
            '    If oSys IsNot Nothing AndAlso oSys.Value <> "" Then
            '        txtOffMinutes.Text = SafeInteger(oSys.Value)
            '        txtDisplayHours.Text = SafeInteger(oSys.SubCategory)
            '        txtDisplayTotalpoints.Text = SafeInteger(oSys.SubCategory)
            '    Else
            '        txtOffMinutes.Text = ""
            '        txtDisplayHours.Text = ""
            '        txtDisplayTotalpoints.Text = ""
            '    End If
            'End If

        End Sub

#End Region

#Region "Functions"

        Public Sub bindAgents(ByVal psSuperAdminID As String)
            Dim oAgentManager As New CAgentManager()
            Dim oData As DataTable = oAgentManager.GetAgentsBySuperID(psSuperAdminID, Nothing)
            ddlAgents.DataSource = oData
            ddlAgents.DataTextField = "Login"
            ddlAgents.DataValueField = "AgentID"
            ddlAgents.DataBind()
            ddlAgents.Items.Insert(0, "All")
        End Sub



        Private Sub BindTimeOff()
            ' Dim oGameTypeOnOffManager As New CGameTypeOnOffManager()
            Dim odtGameTypeOnOff As DataTable
            odtGameTypeOnOff = oSysManager.GetSysSettings(SuperUserID, IIf(ContextDisPlay = EContext.Current, "Current", "1H")) 'Me.UserID, SafeBoolean(IIf(ContextDisPlay = EContext.Current, True, False)))
            'dtgTimeOff.DataSource = odtGameTypeOnOff
            dtgTimeOff.DataBind()
            dtgTimeOff.Visible = odtGameTypeOnOff.Rows.Count > 0
        End Sub

        Private Sub bindGameType()
            Dim oDicSoccerGameType As New Dictionary(Of String, String)
            Dim oDicGameType As New Dictionary(Of String, String)
            Dim oGameTypes As Dictionary(Of String, String) = GetGameType()
            For Each kGameType As KeyValuePair(Of String, String) In oGameTypes
                If Not IsSoccer(kGameType.Value) Then
                    oDicGameType(kGameType.Key) = kGameType.Value
                Else
                    oDicSoccerGameType(kGameType.Key) = kGameType.Value
                End If
            Next
            ViewState("oDicSoccerGameType") = oDicSoccerGameType
            oDicGameType("Soccer") = "Soccer"
            ddlGameType.DataSource = oDicGameType
            ddlGameType.DataTextField = "Key"
            ddlGameType.DataValueField = "Key"
            ddlGameType.DataBind()
        End Sub

        Private Function saveAllAgent() As Boolean
            Dim oDicSoccerGameType As Dictionary(Of String, String) = CType(ViewState("oDicSoccerGameType"), Dictionary(Of String, String))
            Dim oAgentManager As New CAgentManager()
            'Dim oGameTypeOnOffManager As New CGameTypeOnOffManager()
            Dim bSuccess As Boolean = False
            Dim oData As DataTable = oAgentManager.GetAgentsBySuperID(UserSession.UserID, Nothing)
            If oData Is Nothing Then
                Return False
            End If
            For Each dr As DataRow In oData.Rows
                If (ddlGameType.SelectedValue.Equals("Soccer", StringComparison.CurrentCultureIgnoreCase)) Then
                    For Each strGameType As String In oDicSoccerGameType.Keys
                        If oSysManager.IsExistSysSetting(SuperUserID, IIf(bCurrent, "Current", "1H"), strGameType) Then
                            'If oGameTypeOnOffManager.GetGameTypeOnOffByGameType(SafeString(dr("AgentID")), strGameType, bCurrent).Rows.Count > 0 Then
                            'bSuccess = oGameTypeOnOffManager.UpdateGameTypeOnOffByAgent(SafeString(dr("AgentID")), _
                            '                                                            SafeString(strGameType), _
                            '                                                            SafeInteger(txtOffMinutes.Text), _
                            '                                                            SafeInteger(txtDisplayHours.Text), SafeInteger(txtDisplayTotalpoints.Text), bCurrent)

                            bSuccess = oSysManager.UpdateValue(SafeString(dr("AgentID")) & "LineOffHour", IIf(bCurrent, "Current", "1H"), strOffBefore, txtOffMinutes.Text, "", strGameType)
                            bSuccess = oSysManager.UpdateValue(SafeString(dr("AgentID")) & "LineOffHour", IIf(bCurrent, "Current", "1H"), strDisplayBefore, txtDisplayHours.Text, "", strGameType)
                            bSuccess = oSysManager.UpdateValue(SafeString(dr("AgentID")) & "LineOffHour", IIf(bCurrent, "Current", "1H"), strGameTotalPointsDisplay, txtDisplayTotalpoints.Text, "", strGameType)
                        Else
                            'bSuccess = oGameTypeOnOffManager.InsertGameTypeOnOffByAgent(SafeString(dr("AgentID")), _
                            '                                                            newGUID(), _
                            '                                                            SafeString(strGameType), _
                            '                                                            SafeInteger(txtOffMinutes.Text), _
                            '                                                            SafeInteger(txtDisplayHours.Text), SafeInteger(txtDisplayTotalpoints.Text), bCurrent)
                            bSuccess = oSysManager.AddSysSetting(SafeString(dr("AgentID")) & "LineOffHour", strOffBefore, txtOffMinutes.Text, IIf(bCurrent, "Current", "1H"), 0, strGameType)
                            bSuccess = oSysManager.AddSysSetting(SafeString(dr("AgentID")) & "LineOffHour", strDisplayBefore, txtDisplayHours.Text, IIf(bCurrent, "Current", "1H"), 0, strGameType)
                            bSuccess = oSysManager.AddSysSetting(SafeString(dr("AgentID")) & "LineOffHour", strGameTotalPointsDisplay, txtDisplayTotalpoints.Text, IIf(bCurrent, "Current", "1H"), 0, strGameType)
                        End If
                        UserSession.Cache.ClearGameTypeOnOff(SafeString(dr("AgentID")), SafeString(strGameType), bCurrent)
                        UserSession.Cache.ClearSysSettings(SafeString(dr("AgentID")) & "LineOffHour", SafeString(IIf(bCurrent, "Current", "1H")))
                    Next

                Else
                    'If oGameTypeOnOffManager.GetGameTypeOnOffByGameType(SafeString(dr("AgentID")), ddlGameType.SelectedValue, bCurrent).Rows.Count > 0 Then
                    If oSysManager.IsExistSysSetting(SafeString(dr("AgentID")) & "LineOffHour", IIf(bCurrent, "Current", "1H"), ddlGameType.SelectedValue) Then

                        'bSuccess = oGameTypeOnOffManager.UpdateGameTypeOnOffByAgent(SafeString(dr("AgentID")), _
                        '                                                            SafeString(ddlGameType.SelectedValue), _
                        '                                                            SafeInteger(txtOffMinutes.Text), _
                        '                                                            SafeInteger(txtDisplayHours.Text), SafeInteger(txtDisplayTotalpoints.Text), bCurrent)

                        bSuccess = oSysManager.UpdateValue(SafeString(dr("AgentID")) & "LineOffHour", IIf(bCurrent, "Current", "1H"), strOffBefore, txtOffMinutes.Text, "", ddlGameType.SelectedValue)
                        bSuccess = oSysManager.UpdateValue(SafeString(dr("AgentID")) & "LineOffHour", IIf(bCurrent, "Current", "1H"), strDisplayBefore, txtDisplayHours.Text, "", ddlGameType.SelectedValue)
                        bSuccess = oSysManager.UpdateValue(SafeString(dr("AgentID")) & "LineOffHour", IIf(bCurrent, "Current", "1H"), strGameTotalPointsDisplay, txtDisplayTotalpoints.Text, "", ddlGameType.SelectedValue)
                    Else
                        '    bSuccess = oGameTypeOnOffManager.InsertGameTypeOnOffByAgent(SafeString(dr("AgentID")), _
                        '                                                                newGUID(), _
                        '                                                                SafeString(ddlGameType.SelectedValue), _
                        '                                                                SafeInteger(txtOffMinutes.Text), _
                        '                                                                SafeInteger(txtDisplayHours.Text), SafeInteger(txtDisplayTotalpoints.Text), bCurrent)
                        bSuccess = oSysManager.AddSysSetting(SafeString(dr("AgentID")) & "LineOffHour", strOffBefore, txtOffMinutes.Text, IIf(bCurrent, "Current", "1H"), 0, ddlGameType.SelectedValue)
                        bSuccess = oSysManager.AddSysSetting(SafeString(dr("AgentID")) & "LineOffHour", strDisplayBefore, txtDisplayHours.Text, IIf(bCurrent, "Current", "1H"), 0, ddlGameType.SelectedValue)
                        bSuccess = oSysManager.AddSysSetting(SafeString(dr("AgentID")) & "LineOffHour", strGameTotalPointsDisplay, txtDisplayTotalpoints.Text, IIf(bCurrent, "Current", "1H"), 0, ddlGameType.SelectedValue)
                    End If
                    UserSession.Cache.ClearGameTypeOnOff(SafeString(dr("AgentID")) & "LineOffHour", SafeString(ddlGameType.SelectedValue), bCurrent)
                    UserSession.Cache.ClearSysSettings(SafeString(dr("AgentID")) & "LineOffHour", SafeString(IIf(bCurrent, "Current", "1H")))

                End If
            Next



            Return bSuccess

        End Function

#End Region

        Protected Sub dtgTimeOff_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dtgTimeOff.ItemCommand
            Try
                'If e.CommandName.ToUpper = "DELETESETTINGS" Then
                '    Dim poSettingID As String = SafeString(e.CommandArgument)
                '    'Dim CGameTypeOnOffLineMng As New CGameTypeOnOffManager()
                '    If oSysManager.DeleteSettingBy Then 'CGameTypeOnOffLineMng.DeleteDisplaySettings(Me.UserID, poSettingID, bCurrent) Then
                '        ClientAlert("Successfully Deleted", True)
                '    Else
                '        ClientAlert("Error In Deleting Game Setting", True)
                '    End If
                '    'Clear Cache
                '    UserSession.Cache.ClearGameTypeOnOff(Me.UserID, SafeString(ddlGameType.SelectedValue), bCurrent)
                '    BindTimeOff()
                'End If

            Catch ex As Exception
                LogError(_log, "Cannot delete display setting.", ex)
            End Try
        End Sub


    End Class

    Public Enum EContext
        FirstHalf = 0
        Current = 1
    End Enum

End Namespace