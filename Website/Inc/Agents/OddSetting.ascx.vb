Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports SBCBL.std
Imports System.Data

Namespace SBSAgents

    Partial Class Inc_Agents_OddSetting
        Inherits SBCBL.UI.CSBCUserControl
        Private _sGameType As String = SBCBL.std.GetSiteType & " GameType"
        Private _SuperBookmakerValue As String = SBCBL.std.GetSiteType & " BookmakerType"
        Private _sBookmaker As String = SBCBL.std.GetSiteType & " Bookmaker"
        Public ReadOnly Property BookMaker() As String
            Get
                'Dim sAgentID As String = UserSession.UserID.ToString
                'Dim sBookMakerType As String = SafeString(UserSession.UserID & "_BookmakerType")
                'Dim sSuperBookmakerValue As String = ""
                'While True
                '    Dim sBookMakerValue As String = UserSession.Cache.GetSysSettings(sBookMakerType).GetValue(ddlGameType.SelectedValue)
                '    If Not String.IsNullOrEmpty(sBookMakerValue) Then
                '        Return sBookMakerValue
                '    End If
                '    If Not String.IsNullOrEmpty(UserSession.Cache.GetAgentInfo(sAgentID).ParentID) Then
                '        sAgentID = UserSession.Cache.GetAgentInfo(sAgentID).ParentID
                '        sBookMakerType = sAgentID + "_BookmakerType"
                '    Else
                '        Return UserSession.SysSettings(_SuperBookmakerValue).GetValue(ddlGameType.SelectedValue)
                '    End If
                'End While
                'Return UserSession.SysSettings(_SuperBookmakerValue).GetValue(ddlGameType.SelectedValue)
                Return ddlBookMaker.SelectedValue
            End Get


        End Property

        Public ReadOnly Property AgentBookmaker() As String
            Get
                Return UserSession.AgentUserInfo.SuperAgentID & " Manipulation"
            End Get
        End Property

        Public Property GameCircle() As Boolean
            Get
                If ViewState("LockGameBet_GameCircle") Is Nothing Then
                    ViewState("LockGameBet_GameCircle") = False
                End If
                Return SafeBoolean(ViewState("LockGameBet_GameCircle"))
            End Get
            Set(ByVal value As Boolean)
                ViewState("LockGameBet_GameCircle") = value
            End Set
        End Property

        Public Property AddedGame() As Boolean
            Get
                If ViewState("LockGameBet_AddedGame") Is Nothing Then
                    ViewState("LockGameBet_AddedGame") = False
                End If
                Return SafeBoolean(ViewState("LockGameBet_AddedGame"))
            End Get
            Set(ByVal value As Boolean)
                ViewState("LockGameBet_AddedGame") = value
            End Set
        End Property

        Public Property GameContext() As String
            Get
                Return SafeString(ViewState("GameContext"))
            End Get
            Set(ByVal value As String)
                ViewState("GameContext") = value
            End Set
        End Property


        Private Sub BindGameType()
            'Dim oGameTypes As List(Of CSysSetting) = (From oSetting As CSysSetting In UserSession.Cache.GetAllSysSettings(_sGameType) _
            '                                      Where SafeBoolean(oSetting.Value) And oSetting.SubCategory <> "" _
            '                                      Order By oSetting.Key, oSetting.SubCategory Ascending _
            '                                      Select oSetting).ToList

            Dim oGameTypes As Dictionary(Of String, String) = GetGameType()
            ddlGameType.DataSource = oGameTypes
            ddlGameType.DataTextField = "Key"
            ddlGameType.DataValueField = "Key"
            ddlGameType.DataBind()
            ' ddlGameType.SelectedValue = "NCAA Football"
        End Sub

        Private Sub BindBookMaker()
            Dim oSysSettingList As CSysSettingList = UserSession.Cache.GetSysSettingAgentBookMakers(_sBookmaker, UserSession.AgentUserInfo.SuperAgentID)
            Dim oSysSetting As CSysSetting = oSysSettingList.Find(Function(x) x.Key.ToString.Equals("Manipulation", StringComparison.CurrentCultureIgnoreCase))
            If oSysSetting IsNot Nothing Then
                oSysSetting.Value = AgentBookmaker
                oSysSetting.Key = "SBS"
            End If
            ddlBookMaker.DataSource = oSysSettingList.FindAll(Function(x) Not x.Key.ToString.Equals("Manipulation", StringComparison.CurrentCultureIgnoreCase))
            ddlBookMaker.DataTextField = "Key"
            ddlBookMaker.DataValueField = "Value"
            ddlBookMaker.DataBind()

        End Sub

        Private Sub BindGame()
            Dim oListgame As New List(Of String)
            oListgame.Add(ddlGameType.SelectedValue)
            rptMain.DataSource = oListgame
            rptMain.DataBind()
            If String.IsNullOrEmpty(GameContext) Then
                GameContext = "Current"
            End If
            'GameType = "NCAA Baseball"
            'GameCircle = False
            'AddedGame = False
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                '' load current filter settings
                lbnFullGame.CssClass = "label label-primary"
                BindGameType()
                BindBookMaker()
                GameContext = "Current"
                BindGame()
            End If
        End Sub

        Protected Sub lbtTab_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            GameContext = CType(sender, LinkButton).CommandArgument.ToString
            lbn1stHalf.CssClass = "label label-default"
            lbnFullGame.CssClass = "label label-default"
            lbn2ndHalf.CssClass = "label label-default"
            Select Case (GameContext)
                Case "Current"
                    lbnFullGame.CssClass = "label label-primary"
                Case "1H"
                    lbn1stHalf.CssClass = "label label-primary"
                Case "2H"
                    lbn2ndHalf.CssClass = "label label-primary"
            End Select
            BindGame()
        End Sub

        Protected Sub rptBets_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
            If e.Item.ItemType = ListItemType.Footer Or e.Item.ItemType = ListItemType.Header Or e.Item.ItemType = ListItemType.Pager Then
                Return
            End If

            Dim rptGameLines As Repeater = CType(e.Item.FindControl("rptGameLines"), Repeater)
            Dim lblNoGameLine As Label = CType(e.Item.FindControl("lblNoGameLine"), Label)
            Dim oDT As DataRow() = CType(e.Item.DataItem, KeyValuePair(Of String, DataRow())).Value
            rptGameLines.DataSource = oDT
            rptGameLines.DataBind()
            lblNoGameLine.Visible = oDT.Length = 0
            rptGameLines.Visible = oDT.Length >= 0
        End Sub

        Protected Sub rptMain_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptMain.ItemDataBound
            Dim sType As String = SafeString(e.Item.DataItem)
            Dim rptBets As Repeater = e.Item.FindControl("rptBets")
            '' load game by game type to each of rptBets
            Dim bHasgame As Boolean = LoadGames(sType, rptBets)
        End Sub

        Protected Sub rptGameLines_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs)
            Dim bResult As Boolean = False
            Select Case e.CommandName
                Case "SAVE"
                    Dim oGameLineManager As New CGameLineManager()
                    Dim oAgentGameSettingMng As New CAgentGameSettingManager()
                    Dim hfGameID As HiddenField = CType(e.Item.FindControl("hfGameID"), HiddenField)
                    Dim hfGameLineID As HiddenField = CType(e.Item.FindControl("hfGameLineID"), HiddenField)
                    Dim chkLineOnOff As CheckBox = CType(e.Item.FindControl("chkLineOnOff"), CheckBox)
                    Dim chkManualSetting As CheckBox = CType(e.Item.FindControl("chkManualSetting"), CheckBox)
                    Dim chkCircle As CheckBox = CType(e.Item.FindControl("chkCircle"), CheckBox)

                    Dim bExistCircle As Boolean = oAgentGameSettingMng.CheckExistCircle(e.CommandArgument, UserSession.UserID.ToString)

                    If bExistCircle = False Then
                        'insert new AgentGameSettings
                        bResult = oAgentGameSettingMng.InsertGameCircle(hfGameID.Value, UserSession.UserID.ToString(), chkCircle.Checked)
                    Else
                        'update AgentGameSettings
                        bResult = oAgentGameSettingMng.UpdateGameCircle(hfGameID.Value, UserSession.UserID.ToString(), chkCircle.Checked)
                    End If

                    Dim sGameLineID As String
                    sGameLineID = hfGameLineID.Value
                    If chkManualSetting.Checked Then

                        If Not oGameLineManager.ExistAgentManual(hfGameLineID.Value, UserSession.UserID.ToString) Then
                            sGameLineID = oGameLineManager.InsertGameLineManual(hfGameLineID.Value, UserSession.UserID.ToString)
                        End If
                        'update(Line)
                        Dim sGameID As String = SafeString(e.CommandArgument)

                        Dim txtAwaySpread, txtAwaySpreadMoney, txtAwayMoneyLine, _
                        txtHomeSpread, txtHomeSpreadMoney, txtHomeMoney, _
                        txtAwayTotal, txtTotalPointsOverMoney, txtTotalPointsUnderMoney As TextBox
                        txtAwaySpread = CType(e.Item.FindControl("txtAwaySpread"), TextBox)
                        txtAwaySpreadMoney = CType(e.Item.FindControl("txtAwaySpreadMoney"), TextBox)
                        txtAwayMoneyLine = CType(e.Item.FindControl("txtAwayMoneyLine"), TextBox)
                        txtHomeSpread = CType(e.Item.FindControl("txtHomeSpread"), TextBox)
                        txtHomeSpreadMoney = CType(e.Item.FindControl("txtHomeSpreadMoney"), TextBox)
                        txtHomeMoney = CType(e.Item.FindControl("txtHomeMoney"), TextBox)
                        txtAwayTotal = CType(e.Item.FindControl("txtAwayTotal"), TextBox)
                        txtTotalPointsOverMoney = CType(e.Item.FindControl("txtTotalPointsOverMoney"), TextBox)
                        txtTotalPointsUnderMoney = CType(e.Item.FindControl("txtTotalPointsUnderMoney"), TextBox)

                        Dim oGameLine As New CGameLine()
                        oGameLine.GameLineID = sGameLineID
                        oGameLine.GameID = sGameID
                        oGameLine.GameLineOff = chkLineOnOff.Checked
                        oGameLine.IsCircle = chkCircle.Checked
                        oGameLine.ManualSetting = True
                        oGameLine.HomeSpread = -SafeDouble(txtAwaySpread.Text)
                        oGameLine.HomeSpreadMoney = SafeDouble(txtHomeSpreadMoney.Text)
                        oGameLine.AwaySpread = SafeDouble(txtAwaySpread.Text)
                        oGameLine.AwaySpreadMoney = SafeDouble(txtAwaySpreadMoney.Text)

                        oGameLine.HomeMoneyLine = SafeDouble(txtHomeMoney.Text)
                        oGameLine.AwayMoneyLine = SafeDouble(txtAwayMoneyLine.Text)

                        oGameLine.TotalPoints = SafeDouble(txtAwayTotal.Text)
                        oGameLine.TotalPointsOverMoney = SafeDouble(txtTotalPointsOverMoney.Text)
                        oGameLine.TotalPointsUnderMoney = SafeDouble(txtTotalPointsUnderMoney.Text)

                        bResult = oGameLineManager.UpdateManualLine(oGameLine)
                        BindGame()
                    Else
                        '                        sGameLineID = hfGameLineID.Value
                        If oGameLineManager.ExistAgentManual(sGameLineID, UserSession.UserID.ToString) Then
                            bResult = oGameLineManager.DeleteGameLine(sGameLineID)
                            BindGame()
                            ' ClientAlert("Game Line successfully updated .", True)
                            Return
                        End If

                    End If
                    If bResult Then
                        ClientAlert("Successfully Updated .", True)
                    Else
                        ClientAlert("Unsuccessfully Updated . Please Try Again", True)
                    End If
            End Select
        End Sub


        Private Function LoadGames(ByVal psGameType As String, ByVal poRepeater As Repeater) As Boolean
            ''Get primary bookmakers by gametype
            Dim oToday = GetEasternDate()
            Dim sBookmaker As String = BookMaker
            Dim lstGameContext As New List(Of String)
            lstGameContext.Add(GameContext)
            ''Get games
            Dim oGameShows As New Dictionary(Of String, DataRow())
            Dim odtGames As DataTable = New SBCBL.Managers.CGameManager().GetAgentAvailableGames(psGameType, oToday, False, False, False, lstGameContext, BookMaker, UserSession.UserID.ToString)
            Dim nTotalGame As Integer = 0
            Dim oListLines As New Dictionary(Of String, DataRow)

            Dim sContext As String = GameContext
            oListLines.Clear()
            Dim oDTFiltered As DataTable = odtGames.DefaultView.ToTable()

            For Each oRow As DataRow In oDTFiltered.Rows
                AddLineToList(oRow, oListLines, odtGames)
            Next
            oGameShows(sContext) = oListLines.Values.ToArray()
            nTotalGame += oListLines.Values.Count
            If oListLines.Count = 0 Then
                oGameShows.Remove(sContext)
            End If
            poRepeater.DataSource = oGameShows
            poRepeater.DataBind()
            Return nTotalGame > 0
        End Function

        Private Sub AddLineToList(ByVal poLineRow As DataRow, ByVal poLines As Dictionary(Of String, DataRow), ByVal odtGames As DataTable)
            Dim sGameID As String = SafeString(poLineRow("GameID"))

            Dim nAwaySpreadMoney As Double = SafeInteger(poLineRow("AwaySpreadMoney"))
            Dim nTotalPointsOverMoney As Double = SafeInteger(poLineRow("TotalPointsOverMoney"))
            Dim nAwayMoneyLine As Double = SafeInteger(poLineRow("AwayMoneyLine"))
            Dim bCanBet As Boolean = nAwaySpreadMoney <> 0 OrElse nTotalPointsOverMoney <> 0 OrElse nAwayMoneyLine <> 0
            If bCanBet Then
                poLines(sGameID) = poLineRow
            End If
            'Return bCanBet
        End Sub

        Protected Function FormatValue(ByVal poValue As Object) As String
            If poValue.GetType() Is GetType(DBNull) Then
                Return ""
            Else
                Return SafeDouble(poValue).ToString()
            End If
        End Function

        Protected Sub ddlGameType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlGameType.SelectedIndexChanged
            BindGame()
        End Sub

        Protected Sub ddlBookMaker_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlBookMaker.SelectedIndexChanged
            BindGame()
        End Sub

        Protected Sub rptGameLines_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)

            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim chkManualSettings As CheckBox = CType(e.Item.FindControl("chkManualSetting"), CheckBox)
                If chkManualSettings IsNot Nothing Then
                    chkManualSettings.Attributes.Add("onclick", "ShowControls(this)")

                    Dim txtAwaySpread, txtAwaySpreadMoney, txtAwayMoneyLine, _
                            txtHomeSpreadMoney, txtHomeMoney, _
                           txtAwayTotal, txtTotalPointsOverMoney, txtTotalPointsUnderMoney As TextBox

                    ' Dim chkCircle As CheckBox = CType(e.Item.FindControl("chkCircle"), CheckBox)
                    Dim chkOffline As CheckBox = CType(e.Item.FindControl("chkLineOnOff"), CheckBox)
                    txtAwaySpread = CType(e.Item.FindControl("txtAwaySpread"), TextBox)
                    txtAwaySpreadMoney = CType(e.Item.FindControl("txtAwaySpreadMoney"), TextBox)
                    txtAwayMoneyLine = CType(e.Item.FindControl("txtAwayMoneyLine"), TextBox)
                    txtHomeSpreadMoney = CType(e.Item.FindControl("txtHomeSpreadMoney"), TextBox)
                    txtHomeMoney = CType(e.Item.FindControl("txtHomeMoney"), TextBox)
                    txtAwayTotal = CType(e.Item.FindControl("txtAwayTotal"), TextBox)
                    txtTotalPointsOverMoney = CType(e.Item.FindControl("txtTotalPointsOverMoney"), TextBox)
                    txtTotalPointsUnderMoney = CType(e.Item.FindControl("txtTotalPointsUnderMoney"), TextBox)
                    If Not chkManualSettings.Checked Then
                        ' disable all textbox and button
                        txtAwaySpread.Enabled = False
                        txtAwaySpreadMoney.Enabled = False
                        txtAwayMoneyLine.Enabled = False
                        txtHomeSpreadMoney.Enabled = False
                        txtHomeMoney.Enabled = False
                        txtAwayTotal.Enabled = False
                        txtTotalPointsOverMoney.Enabled = False
                        txtTotalPointsUnderMoney.Enabled = False
                        ' chkCircle.Enabled = False
                        chkOffline.Enabled = False
                    End If

                End If
            End If
        End Sub
    End Class

End Namespace
