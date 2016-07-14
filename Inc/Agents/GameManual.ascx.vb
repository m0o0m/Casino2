Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports SBCBL.std
Imports System.Data

Namespace SBSAgents
    Partial Class GameManual
        Inherits SBCBL.UI.CSBCUserControl
        Private _SuperBookmakerValue As String = SBCBL.std.GetSiteType & " BookmakerType"
        Dim oSysManager As New CSysSettingManager()

#Region "Properties"

        Private ReadOnly Property SuperAgentID() As String
            Get
                If UserSession.UserType = SBCBL.EUserType.Agent Then
                    Return UserSession.UserID & "QuaterOff"
                Else
                    Return ddlAgent.SelectedValue & "QuaterOff"
                End If
            End Get
        End Property

        Private ReadOnly Property BookMaker() As String
            Get
                If UserSession.UserType = SBCBL.EUserType.Agent Then
                    Return UserSession.UserID
                Else
                    Return ddlAgent.SelectedValue
                End If
            End Get
        End Property

        Private ReadOnly Property GameTypes() As List(Of String)
            Get
                Dim oListgames As New List(Of String)()
                'For Each oItem As ListItem In cblGameTypes.Items
                '    If oItem.Selected Then
                '        oListgames.Add(oItem.Value)
                '    End If
                'Next
                oListgames.Add(ddlGameType.SelectedValue)
                oListgames.Sort()
                Return oListgames
            End Get
        End Property

#End Region

#Region "Bind Data"
        Private Sub BindGame()
            rptMain.DataSource = GameTypes
            rptMain.DataBind()
        End Sub

        Public Sub bindAgents(ByVal psSuperAdminID As String)
            Dim oAgentManager As New CAgentManager()
            Dim oData As DataTable = oAgentManager.GetAgentsBySuperID(psSuperAdminID, Nothing)

            ddlAgent.DataSource = oData
            ddlAgent.DataTextField = "FullName"
            ddlAgent.DataValueField = "AgentID"
            ddlAgent.DataBind()
            ddlAgent.Items.Insert(0, "All")
        End Sub

        Public Sub BindGameQuarterOff()
            Dim oCacheManager = New CCacheManager()
            'cblQuarterOff.Items(0).Selected = oCacheManager.GetGameQuarterOff(SuperAgentID, ddlGameType.SelectedValue, "1QOff")
            'cblQuarterOff.Items(1).Selected = oCacheManager.GetGameQuarterOff(SuperAgentID, ddlGameType.SelectedValue, "2QOff")
            'cblQuarterOff.Items(2).Selected = oCacheManager.GetGameQuarterOff(SuperAgentID, ddlGameType.SelectedValue, "3QOff")
            'cblQuarterOff.Items(3).Selected = oCacheManager.GetGameQuarterOff(SuperAgentID, ddlGameType.SelectedValue, "4QOff")

            cblQuarterOff.Items(0).Selected = UserSession.Cache.GetSysSettings(SuperAgentID).GetBooleanValue("1QOff") 'oCacheManager.GetGameQuarterOff(SuperAgentID, ddlGameType.SelectedValue, "1QOff")
            cblQuarterOff.Items(1).Selected = UserSession.Cache.GetSysSettings(SuperAgentID).GetBooleanValue("2QOff")
            cblQuarterOff.Items(2).Selected = UserSession.Cache.GetSysSettings(SuperAgentID).GetBooleanValue("3QOff")
            cblQuarterOff.Items(3).Selected = UserSession.Cache.GetSysSettings(SuperAgentID).GetBooleanValue("4QOff")
        End Sub


#End Region


        Protected Sub rptGameLines_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptMain.ItemCommand
            Select Case e.CommandName
                Case "SAVE"
                    If SaveLineByRow(e.Item, True) Then
                        ClientAlert("Successfully Saved")
                    Else
                        ClientAlert("Failed to Save Setting")
                    End If
                Case "DELETE"
                    Dim sLineID As String = CType(e.Item.FindControl("hfGameLineID"), HiddenField).Value
                    Dim oManager As New SBCBL.Managers.CGameLineManager()
                    If oManager.DeleteGameQuarterLine(sLineID, SuperAgentID) Then
                        ClientAlert("Successfully Deleted.")
                        ClearRow(e.Item)
                    Else
                        ClientAlert("Can't Delete")
                    End If
            End Select
            BindGame()
        End Sub

        Private Function SaveLineByRow(ByVal poItem As RepeaterItem, Optional ByVal pbWarn As Boolean = False) As Boolean
            Dim oLine As New SBCBL.CacheUtils.CGameLine()
            oLine.GameID = CType(poItem.FindControl("hfGameID"), HiddenField).Value
            oLine.GameLineID = CType(poItem.FindControl("hfGameLineID"), HiddenField).Value
            oLine.FeedSource = "MANUAL"
            oLine.Bookmaker = BookMaker
            oLine.Context = CType(poItem.FindControl("hfContext"), HiddenField).Value
            oLine.AwaySpread = SafeDouble(CType(poItem.FindControl("txtAwaySpread"), TextBox).Text)
            oLine.AwaySpreadMoney = SafeDouble(CType(poItem.FindControl("txtAwaySpreadMoney"), TextBox).Text)
            oLine.TotalPoints = SafeDouble(CType(poItem.FindControl("txtAwayTotal"), TextBox).Text)
            oLine.TotalPointsOverMoney = SafeDouble(CType(poItem.FindControl("txtTotalPointsOverMoney"), TextBox).Text)
            'oLine.AwayMoneyLine = SafeDouble(CType(poItem.FindControl("txtAwayMoneyLine"), TextBox).Text)
            oLine.HomeSpread = SafeDouble(CType(poItem.FindControl("txtHomeSpread"), TextBox).Text)
            oLine.HomeSpreadMoney = SafeDouble(CType(poItem.FindControl("txtHomeSpreadMoney"), TextBox).Text)
            oLine.TotalPointsUnderMoney = SafeDouble(CType(poItem.FindControl("txtTotalPointsUnderMoney"), TextBox).Text)
            'oLine.HomeMoneyLine = SafeDouble(CType(poItem.FindControl("txtHomeMoney"), TextBox).Text)

            Dim sErrorField As String = ""
            If Not ValidateLine(oLine, sErrorField) And pbWarn Then
                ClientAlert("Please Input " & sErrorField)
                Return False
            End If

            Dim oGameLineManager As New SBCBL.Managers.CGameLineManager()
            Return oGameLineManager.SaveLine(oLine)
        End Function

        Private Function ClearRow(ByVal poItem As RepeaterItem) As Boolean
            CType(poItem.FindControl("txtAwaySpread"), TextBox).Text = ""
            CType(poItem.FindControl("txtAwaySpreadMoney"), TextBox).Text = ""
            CType(poItem.FindControl("txtAwayTotal"), TextBox).Text = ""
            CType(poItem.FindControl("txtTotalPoints2"), TextBox).Text = ""
            CType(poItem.FindControl("txtTotalPointsOverMoney"), TextBox).Text = ""
            'CType(poItem.FindControl("txtAwayMoneyLine"), TextBox).Text = ""
            CType(poItem.FindControl("txtHomeSpread"), TextBox).Text = ""
            CType(poItem.FindControl("txtHomeSpreadMoney"), TextBox).Text = ""
            CType(poItem.FindControl("txtTotalPointsUnderMoney"), TextBox).Text = ""
            'CType(poItem.FindControl("txtHomeMoney"), TextBox).Text = ""
        End Function

        Public Function ValidateLine(ByVal poLine As SBCBL.CacheUtils.CGameLine, ByRef poMissField As String) As Boolean
            Dim bResult As Boolean = True
            Dim sMissField As String = ""
            If poLine.AwaySpreadMoney = 0 Then
                sMissField = "Away Spread Money"
                bResult = False
            ElseIf poLine.TotalPointsOverMoney = 0 Then
                sMissField = "Total Points Over Money"
                bResult = False
            ElseIf poLine.HomeSpreadMoney = 0 Then
                sMissField = "Home Spread Money"
                bResult = False
            ElseIf poLine.TotalPointsUnderMoney = 0 Then
                sMissField = "Total Points Under Money"
                bResult = False
            End If
            poMissField = sMissField
            Return bResult
        End Function

        Protected Sub rptMain_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptMain.ItemDataBound
            Dim sType As String = SafeString(e.Item.DataItem)
            Dim rptBets As Repeater = e.Item.FindControl("rptBets")

            '' load game by game type to each of rptBets
            LoadGames(sType, rptBets)

        End Sub

        ''get BookMaker for load game
        Private Function GetAgentBookMaker(ByVal psGameType As String) As String
            Return GetBookMakerType(psGameType, SuperAgentID, SafeString(SuperAgentID & "_BookmakerType"))
        End Function

        Public Function GetBookMakerType(ByVal psKey As String, ByVal psAgentID As String, ByVal psBookMakerType As String) As String
            Dim sAgentID As String = psAgentID
            Dim sBookMakerType As String = psBookMakerType
            While True
                Dim sBookMakerValue As String = UserSession.Cache.GetSysSettings(sBookMakerType).GetValue(psKey)
                If Not String.IsNullOrEmpty(sBookMakerValue) Then
                    Return sBookMakerType
                End If
                If Not String.IsNullOrEmpty(UserSession.Cache.GetAgentInfo(sAgentID).ParentID) Then
                    sAgentID = UserSession.Cache.GetAgentInfo(sAgentID).ParentID
                    sBookMakerType = sAgentID + "_BookmakerType"
                Else
                    Return _SuperBookmakerValue
                End If
            End While

            Return _SuperBookmakerValue
        End Function

        Private Function LoadGames(ByVal psGameType As String, ByVal poRepeater As Repeater) As Boolean
            ''Get primary bookmakers by gametype

            Dim oToday = GetEasternDate()
            ' Dim sBookmaker As String = UserSession.SysSettings(_sBookmakerType).GetValue(psGameType)
            Dim sBookmaker As String = UserSession.SysSettings(GetAgentBookMaker(psGameType)).GetValue(psGameType)
            ' '' if doesn't found book maker, just return, SINCE NOW WE DOESNT FILTER BY BOOK MAKER
            If sBookmaker = "" Then
                poRepeater.DataSource = Nothing
                poRepeater.DataBind()
                Return False
            End If
            'cblGameTypes.Items.fi
            Dim oListGameTypes As New List(Of String)
            Dim oContexts As New List(Of String)
            ' Dim bRequireOdd As Boolean = chkOdds.Checked
            'For Each oItem As ListItem In cblGameTypes.Items
            '    If oItem.Selected Then
            '        oListGameTypes.Add(oItem.Value)
            '    End If
            'Next
            oListGameTypes.Add(ddlGameType.SelectedValue)
            For Each oItem As ListItem In cblContext.Items
                If oItem.Selected Then
                    oContexts.Add(oItem.Value)
                End If
            Next
            ''Get games
            Dim oGameShows As New Dictionary(Of String, DataTable)
            Dim odtGames As DataTable = New SBCBL.Managers.CGameManager().GetAgentAvailableGames(psGameType, oToday, True, True, True, oContexts, sBookmaker, SuperAgentID, True)
            'New SBCBL.Managers.CGameManager().GetAvailableGamesQuaters(psGameType, oToday, oContexts.ToArray(), False, SuperAgentID, sBookmaker)

            For Each sContext As String In oContexts
                oGameShows.Add(SafeString(sContext), Nothing)
            Next

            Dim oListTempRow As New List(Of DataRow)

            Dim oList As New List(Of Guid)
            If odtGames Is Nothing Then
                Return False
            End If
            For Each oRow As DataRow In odtGames.Rows
                If oList.Contains(oRow("GameID")) Then
                    Continue For
                End If

                oList.Add(oRow("GameID"))

                For Each sContext As String In oContexts
                    Dim sWhere As String = String.Format("GameID= {0} and Context = {1} ", SQLString(oRow("GameID")), SQLString(sContext))
                    odtGames.DefaultView.RowFilter = sWhere
                    If odtGames.DefaultView.ToTable().Rows.Count = 0 Then
                        Dim oGameID As Guid = CType(oRow("GameID"), Guid)
                        oListTempRow.Add(CreateNewGameLine(odtGames, oRow, oGameID, sContext))
                    End If
                Next
            Next

            'If Not bRequireOdd Then
            For Each oRow As DataRow In oListTempRow
                odtGames.Rows.Add(oRow)
            Next
            '  End If

            For Each sContext As String In oGameShows.Keys.ToArray()
                Dim sWhere As String = "Context = " & SQLString(sContext)

                odtGames.DefaultView.RowFilter = sWhere
                Dim oDTFiltered As DataTable = odtGames.DefaultView.ToTable()
                '' sorting the result
                oDTFiltered.DefaultView.Sort = "GameDate,HomeRotationNumber"

                oGameShows(sContext) = oDTFiltered.DefaultView.ToTable()
            Next
            If oGameShows.Count <= 0 Then
                poRepeater.Visible = False
            Else
                poRepeater.DataSource = oGameShows
                poRepeater.DataBind()
            End If

        End Function

        Private Function CreateNewGameLine(ByVal poDT As DataTable, ByVal poRowGame As DataRow, ByVal poGameID As Guid, ByVal psContext As String) As DataRow
            Dim oRow As DataRow = poDT.NewRow()
            oRow("GameLineID") = newGUID()
            oRow("GameID") = poGameID
            oRow("LastUpdated") = DateTime.Now.ToUniversalTime()
            oRow("Context") = psContext
            oRow("Bookmaker") = BookMaker
            oRow("FeedSource") = "MANUAL"

            oRow("GameDate") = poRowGame("GameDate")
            oRow("GameType") = poRowGame("GameType")
            oRow("HomeTeam") = poRowGame("HomeTeam")
            oRow("HomeRotationNumber") = poRowGame("HomeRotationNumber")
            oRow("AwayTeam") = poRowGame("AwayTeam")
            oRow("AwayRotationNumber") = poRowGame("AwayRotationNumber")
            oRow("AwayRotationNumber") = poRowGame("AwayRotationNumber")
            Return oRow
        End Function

        Protected Sub rptBets_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
            If e.Item.ItemType = ListItemType.Footer Or e.Item.ItemType = ListItemType.Header Or e.Item.ItemType = ListItemType.Pager Then
                Return
            End If

            Dim rptGameLines As Repeater = CType(e.Item.FindControl("rptGameLines"), Repeater)
            Dim lblNoGameLine As Label = CType(e.Item.FindControl("lblNoGameLine"), Label)

            Dim oDT As DataTable = CType(e.Item.DataItem, KeyValuePair(Of String, DataTable)).Value
            rptGameLines.DataSource = oDT
            rptGameLines.DataBind()
            '  HiddenNoGame.
            lblNoGameLine.Visible = False
            lblNoGameLine.Parent.Visible = Not (oDT.Rows.Count = 0)
            'lblNoGameLine.Visible = oDT.Rows.Count = 0
            rptGameLines.Visible = oDT.Rows.Count >= 0
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                    tdAgents.Visible = True
                    bindAgents(UserSession.UserID)
                ElseIf UserSession.UserType = SBCBL.EUserType.CallCenterAgent Then
                    tdAgents.Visible = True
                    bindAgents(UserSession.CCAgentUserInfo.CreatedBy)
                ElseIf UserSession.UserType = SBCBL.EUserType.Agent Then
                    tdAgents.Visible = False
                End If
                If UserSession.UserType = SBCBL.EUserType.SuperAdmin AndAlso ddlAgent.SelectedValue = "All" Then
                    AllAgent(True)
                    Return
                End If
                AllAgent(False)
                BindGame()
                BindGameQuarterOff()
            End If
        End Sub

        Protected Sub btnSave1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave1.Click ', btnSave2.Click
            Dim bSuccess As Boolean = True
            For Each oItemMain As RepeaterItem In rptMain.Items
                Dim rptBets As Repeater = CType(oItemMain.FindControl("rptBets"), Repeater)
                For Each oItemGame As RepeaterItem In rptBets.Items
                    Dim rptGameLines As Repeater = CType(oItemGame.FindControl("rptGameLines"), Repeater)
                    For Each oItem As RepeaterItem In rptGameLines.Items
                        If oItem.ItemType <> ListItemType.Item Or oItem.ItemType = ListItemType.Separator Then
                            Continue For
                        End If
                        Dim bEdit As Boolean = True
                        '' INORGE if SA doesnt enter any infor in this row
                        Select Case ""
                            Case SafeString(CType(oItem.FindControl("txtAwaySpreadMoney"), TextBox).Text), _
                                SafeString(CType(oItem.FindControl("txtHomeSpreadMoney"), TextBox).Text), _
                                SafeString(CType(oItem.FindControl("txtTotalPointsOverMoney"), TextBox).Text), _
                                SafeString(CType(oItem.FindControl("txtTotalPointsUnderMoney"), TextBox).Text)
                                Continue For
                        End Select
                        If Not SaveLineByRow(oItem) Then
                            bSuccess = False
                        End If
                    Next

                Next
            Next

            If Not bSuccess Then
                ClientAlert("Can't Saved")
            Else
                ClientAlert("Successfully saved")
            End If
        End Sub

        Protected Sub btnReload1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnReload1.Click ', btnReload2.Click
            BindGame()
        End Sub

        Protected Function FormatValue(ByVal poValue As Object) As String
            If poValue.GetType() Is GetType(DBNull) Then
                Return ""
            Else
                Return SafeDouble(poValue).ToString()
            End If
        End Function

        Protected Sub ddlGameTypes_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlGameType.SelectedIndexChanged, cblContext.SelectedIndexChanged
            If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                AllAgent(ddlAgent.SelectedValue.Equals("All"))
                If ddlAgent.SelectedValue.Equals("All") Then
                    Return
                End If
            End If
            BindGame()
            BindGameQuarterOff()
        End Sub

        Protected Sub ddlAgent_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlAgent.SelectedIndexChanged
            AllAgent(ddlAgent.SelectedValue.Equals("All"))
            If ddlAgent.SelectedValue.Equals("All") Then
                Return
            End If
            BindGame()
            BindGameQuarterOff()
        End Sub

        Protected Sub cblQuarterOff_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cblQuarterOff.SelectedIndexChanged
            Dim oCacheManager = New CCacheManager()
            For Each item As ListItem In cblQuarterOff.Items
                SaveQuarterOff(item.Value, item.Selected)
                oCacheManager.ClearGameQuarterOff(SuperAgentID, ddlGameType.SelectedValue, item.Value)
            Next
        End Sub

        Private Sub SaveQuarterOff(ByVal psContext As String, ByVal pbSelected As Boolean)
            'Dim oGameTypeOnOff = New CGameTypeOnOffManager()
            'If oGameTypeOnOff.CheckExistsGameQuarterOffByAgent(SuperAgentID, ddlGameType.SelectedValue, psContext) Then
            '    oGameTypeOnOff.UpdateGameQuarterOffByAgent(SuperAgentID, ddlGameType.SelectedValue, psContext, pbSelected)
            'Else
            '    oGameTypeOnOff.InsertGameQuarterOffByAgent(SuperAgentID, newGUID, ddlGameType.SelectedValue, psContext, pbSelected)
            'End If
            If oSysManager.IsExistedCategory(SuperAgentID) Then
                oSysManager.UpdateValue(SuperAgentID, ddlGameType.SelectedValue, psContext, pbSelected)
            Else
                oSysManager.AddSysSetting(SuperAgentID, psContext, pbSelected, ddlGameType.SelectedValue)
            End If
            UserSession.Cache.ClearSysSettings(SuperAgentID, ddlGameType.SelectedValue)
        End Sub

        ''''''''''''''''''''''''''''save off quarter for all agent start'''''''''''''''''''''''''''''''''''''
        Private Sub AllAgent(ByVal pdShow As Boolean)
            tdFilterQuarter.Visible = Not pdShow
            tblGame.Visible = Not pdShow
            tdOffAllQuarter.Visible = pdShow
        End Sub

        Private Sub SaveQuarterOffAllAgent(ByVal pbOff As Boolean)
            'Dim oGameTypeOnOff = New CGameTypeOnOffManager()
            Dim oAgentManager = New CAgentManager()
            Dim oCacheManager = New CCacheManager()
            Dim arrContext As String() = {"1QOff", "2QOff", "3QOff", "4QOff"}
            Dim oData As DataTable = oAgentManager.GetAgentsBySuperID(UserSession.UserID, Nothing)
            If oData Is Nothing Then
                Return
            End If
            Try
                For Each dr As DataRow In oData.Rows
                    Dim sAgentID As String = SafeString(dr("AgentID"))

                    For Each sQuarterContext As String In arrContext
                        'If oGameTypeOnOff.CheckExistsGameQuarterOffByAgent(sAgentID, ddlGameType.SelectedValue, sQuarterContext) Then
                        '    oGameTypeOnOff.UpdateGameQuarterOffByAgent(sAgentID, ddlGameType.SelectedValue, sQuarterContext, pbOff)
                        'Else
                        '    oGameTypeOnOff.InsertGameQuarterOffByAgent(sAgentID, newGUID, ddlGameType.SelectedValue, sQuarterContext, pbOff)
                        'End If
                        If oSysManager.IsExistedCategory(sAgentID & "QuaterOff") Then
                            oSysManager.UpdateValue(sAgentID, ddlGameType.SelectedValue, sQuarterContext, pbOff)
                        Else
                            oSysManager.AddSysSetting(sAgentID, sQuarterContext, pbOff, ddlGameType.SelectedValue)
                        End If
                        UserSession.Cache.ClearSysSettings(sAgentID, ddlGameType.SelectedValue)
                        'oCacheManager.ClearGameQuarterOff(sAgentID, ddlGameType.SelectedValue, sQuarterContext)
                    Next


                Next
            Catch ex As Exception
                ClientAlert("Save Fail", True)
            End Try
            ClientAlert("Save Successful", True)
        End Sub

        Protected Sub btnSaveAll_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSaveAll.Click
            If rdQuarter.SelectedValue = "" Then
                ClientAlert("Please ,Select Quarter On Or Off", True)
                Return
            End If
            SaveQuarterOffAllAgent(rdQuarter.SelectedValue.Equals("QuarterOff", StringComparison.CurrentCultureIgnoreCase))
        End Sub

        ''''''''''''''''''''''''''''save off quarter for all agent end'''''''''''''''''''''''''''''''''''''


    End Class
End Namespace
