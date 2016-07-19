Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports SBCBL.std
Imports System.Data

Namespace SBSCallCenterAgents
    Partial Class LinesMonitor
        Inherits SBCBL.UI.CSBCPage

        Private _sBookmaker As String = SBCBL.std.GetSiteType & " Bookmaker"
        Private _sGameType As String = SBCBL.std.GetSiteType & " GameType"
        Private _sBookmakerType As String = SBCBL.std.GetSiteType & " BookmakerType"
        Private _sHalfCategory As String = "HalfLineOff"

        Public Property IsManual() As Boolean
            Get
                Return SafeBoolean(ViewState("_IS_MANUAL"))
            End Get
            Set(ByVal value As Boolean)
                ViewState("_IS_MANUAL") = value
            End Set
        End Property

        Public Property BookMaker() As String
            Get
                If Session("LockGameBet_BookMaker") Is Nothing Then
                    Session("LockGameBet_BookMaker") = "5Dimes"
                End If
                Return SafeString(Session("LockGameBet_BookMaker"))
            End Get
            Set(ByVal value As String)
                Session("LockGameBet_BookMaker") = value
            End Set
        End Property

        Public Property GameType() As String
            Get
                If Session("LockGameBet_GameType") Is Nothing Then
                    Session("LockGameBet_GameType") = "NCAA Football"
                End If
                Return SafeString(Session("LockGameBet_GameType"))
            End Get
            Set(ByVal value As String)
                Session("LockGameBet_GameType") = value
            End Set
        End Property

        Public Property GameContext() As String
            Get
                If Session("LockGameBet_GameContext") Is Nothing Then
                    Session("LockGameBet_GameContext") = ""
                End If
                Return SafeString(Session("LockGameBet_GameContext"))
            End Get
            Set(ByVal value As String)
                Session("LockGameBet_GameContext") = value
            End Set
        End Property

        Public Property GameCircle() As Boolean
            Get
                If Session("LockGameBet_GameCircle") Is Nothing Then
                    Session("LockGameBet_GameCircle") = False
                End If
                Return SafeBoolean(Session("LockGameBet_GameCircle"))
            End Get
            Set(ByVal value As Boolean)
                Session("LockGameBet_GameCircle") = value
            End Set
        End Property

        Public Property AddedGame() As Boolean
            Get
                If Session("LockGameBet_AddedGame") Is Nothing Then
                    Session("LockGameBet_AddedGame") = False
                End If
                Return SafeBoolean(Session("LockGameBet_AddedGame"))
            End Get
            Set(ByVal value As Boolean)
                Session("LockGameBet_AddedGame") = value
            End Set
        End Property

        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Lines Monitor"
            SideMenuTabName = "LINES_MONITOR"
            SubMenuActive = "LINES_MONITOR"
            Me.MenuTabName = "GAME"
            DisplaySubTitlle(Me.Master, "Lines Monitor")

            If Not IsPostBack Then
                BindBookmaker()
                bindGameType()
            End If
        End Sub

        Public Sub BindBookmaker()
            ddlBookmaker.DataSource = UserSession.Cache.GetSysSettings(_sBookmaker)
            ddlBookmaker.DataBind()
        End Sub


        Private Sub bindGameType()
            Dim oGameTypes As List(Of CSysSetting) = (From oSetting As CSysSetting In UserSession.Cache.GetAllSysSettings(_sGameType) _
                                                  Where SafeBoolean(oSetting.Value) And oSetting.SubCategory <> "" _
                                                  Order By oSetting.Key, oSetting.SubCategory Ascending _
                                                  Select oSetting).ToList

            ddlGameType.DataSource = oGameTypes
            ddlGameType.DataTextField = "Key"
            ddlGameType.DataValueField = "Key"
            ddlGameType.DataBind()
            'ddlGameType.SelectedValue = "NCAA Football"
        End Sub

        Private Sub BindGame()
            Dim oListgame As New List(Of String)
            If ddlGameType.SelectedValue = "" Then
                For Each oItem As ListItem In ddlGameType.Items
                    oListgame.Add(oItem.Value)
                Next
            Else
                oListgame.Add(ddlGameType.SelectedValue)
            End If

            rptMain.DataSource = oListgame
            rptMain.DataBind()

            '' save current filter settings
            GameContext = ddlContext.SelectedValue
            GameType = ddlGameType.SelectedValue
            BookMaker = ddlBookmaker.SelectedValue
            GameCircle = chkCircle.Checked
            AddedGame = chkAdded.Checked
        End Sub

        Protected Sub rptGameLines_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptMain.ItemCommand
            Select Case e.CommandName
                Case "LOCK_BET"
                    Dim sGameID As String = SafeString(e.CommandArgument)
                    Dim bLock As Boolean = SafeBoolean(IIf(CType(e.CommandSource, LinkButton).Text = "Y", False, True))
                    Dim oGameManager As New SBCBL.Managers.CGameManager()
                    Dim HFContext As HiddenField = CType(e.Item.FindControl("HFContext"), HiddenField)

                    If Not oGameManager.LockGameBets(sGameID, HFContext.Value, bLock, UserSession.UserID) Then
                        ClientAlert("Can't Set Game Offline")
                    Else
                        CType(e.CommandSource, LinkButton).Text = SafeString(IIf(bLock, "Y", "N"))
                    End If
                Case "LOCK_ML"
                    Dim sGameID As String = SafeString(e.CommandArgument)
                    Dim bLock As Boolean = SafeBoolean(IIf(CType(e.CommandSource, LinkButton).Text = "Y", False, True))
                    Dim oGameManager As New SBCBL.Managers.CGameManager()
                    Dim HFContext As HiddenField = CType(e.Item.FindControl("HFContext"), HiddenField)

                    If Not oGameManager.LockGameMoneyLine(sGameID, HFContext.Value, bLock, UserSession.UserID) Then
                        ClientAlert("Can't Lock Money Line")
                    Else
                        CType(e.CommandSource, LinkButton).Text = SafeString(IIf(bLock, "Y", "N"))
                    End If
                Case "LOCK_TPOINT"
                    Dim sGameID As String = SafeString(e.CommandArgument)
                    Dim bLock As Boolean = SafeBoolean(IIf(CType(e.CommandSource, LinkButton).Text = "Y", False, True))
                    Dim oGameManager As New SBCBL.Managers.CGameManager()
                    Dim HFContext As HiddenField = CType(e.Item.FindControl("HFContext"), HiddenField)

                    If Not oGameManager.LockGameTotalPoint(sGameID, HFContext.Value, bLock, UserSession.UserID) Then
                        ClientAlert("Can't lock money line for this game now.")
                    Else
                        CType(e.CommandSource, LinkButton).Text = SafeString(IIf(bLock, "Y", "N"))
                    End If
                Case "UPDATE_LINE"
                    Dim sGameID As String = SafeString(e.CommandArgument)

                    Dim txtAwaySpread, txtAwaySpreadMoney, txtAwayMoneyLine, _
                    txtHomeSpread, txtHomeSpreadMoney, txtHomeMoney, _
                    txtAwayTotal, txtTotalPointsOverMoney, txtTotalPointsUnderMoney As TextBox

                    Dim HFContext As HiddenField = CType(e.Item.FindControl("HFContext"), HiddenField)

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
                    oGameLine.GameID = sGameID
                    oGameLine.Context = HFContext.Value
                    oGameLine.FeedSource = SBCBL.CFileDBKeys.TIGER_SB_BOOKMAKER
                    oGameLine.Bookmaker = SBCBL.CFileDBKeys.TIGER_SB_BOOKMAKER

                    oGameLine.HomeSpread = -SafeDouble(txtAwaySpread.Text)
                    oGameLine.HomeSpreadMoney = SafeDouble(txtHomeSpreadMoney.Text)

                    oGameLine.AwaySpread = SafeDouble(txtAwaySpread.Text)
                    oGameLine.AwaySpreadMoney = SafeDouble(txtAwaySpreadMoney.Text)

                    oGameLine.HomeMoneyLine = SafeDouble(txtHomeMoney.Text)
                    oGameLine.AwayMoneyLine = SafeDouble(txtAwayMoneyLine.Text)

                    oGameLine.TotalPoints = SafeDouble(txtAwayTotal.Text)
                    oGameLine.TotalPointsOverMoney = SafeDouble(txtTotalPointsOverMoney.Text)
                    oGameLine.TotalPointsUnderMoney = SafeDouble(txtTotalPointsUnderMoney.Text)

                    '' Update Line
                    Dim oGameLineManager As New CGameLineManager()
                    If oGameLineManager.SaveManualLine(oGameLine, SafeString(UserSession.UserID)) Then
                        ClientAlert("Successfully Updated", True)
                    Else
                        ClientAlert("Unsuccessfully Updated", True)
                    End If
                Case "REMOVE_WARN"
                    Dim oGameLineManager As New CGameLineManager()
                    oGameLineManager.RemoveWarning(SafeString(e.CommandArgument))
                    e.Item.FindControl("lblWarn").Visible = False
                    e.Item.FindControl("lbnWarn").Visible = False

                Case "LOCK_ALL_GAME"
                    Dim sContext As String = SafeString(e.CommandArgument)

                    Dim oManager As New CGameManager()
                    oManager.LockAllGames(ddlGameType.Value, GetEasternDate(), sContext.Split("|")(0), SBCBL.CEnums.ELockType.Game, SafeBoolean(sContext.Split("|")(1)), UserSession.UserID)
                    BindGame()
                Case "LOCK_ML_ALL_GAME"
                    Dim sContext As String = SafeString(e.CommandArgument)

                    Dim oManager As New CGameManager()
                    oManager.LockAllGames(ddlGameType.Value, GetEasternDate(), sContext.Split("|")(0), SBCBL.CEnums.ELockType.MoneyLine, SafeBoolean(sContext.Split("|")(1)), UserSession.UserID)
                    BindGame()
                Case "LOCK_TPOINT_ALL_GAME"
                    Dim sContext As String = SafeString(e.CommandArgument)

                    Dim oManager As New CGameManager()
                    oManager.LockAllGames(ddlGameType.Value, GetEasternDate(), sContext.Split("|")(0), SBCBL.CEnums.ELockType.TotalPoint, SafeBoolean(sContext.Split("|")(1)), UserSession.UserID)
                    BindGame()
            End Select
        End Sub

        Private Function ClearRow(ByVal poItem As RepeaterItem) As Boolean

            CType(poItem.FindControl("txtAwaySpread"), TextBox).Text = ""
            CType(poItem.FindControl("txtAwaySpreadMoney"), TextBox).Text = ""

            CType(poItem.FindControl("txtAwayTotal"), TextBox).Text = ""
            CType(poItem.FindControl("txtTotalPoints2"), TextBox).Text = ""
            CType(poItem.FindControl("txtTotalPointsOverMoney"), TextBox).Text = ""
            CType(poItem.FindControl("txtAwayMoneyLine"), TextBox).Text = ""
            CType(poItem.FindControl("txtHomeSpread"), TextBox).Text = ""
            CType(poItem.FindControl("txtHomeSpreadMoney"), TextBox).Text = ""
            CType(poItem.FindControl("txtTotalPointsUnderMoney"), TextBox).Text = ""
            CType(poItem.FindControl("txtHomeMoney"), TextBox).Text = ""
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
            Dim bHasgame As Boolean = LoadGames(sType, rptBets)

            Dim chk1H As CheckBox = CType(e.Item.FindControl("chk1H"), CheckBox)
            Dim chk2H As CheckBox = CType(e.Item.FindControl("chk2H"), CheckBox)

            If bHasgame Then
                Dim sSubHCategory As String = "1H"
                Dim oSysmanager As New SBCBL.CacheUtils.CCacheManager()
                Dim oSys As CSysSetting = oSysmanager.GetSysSettings(_sHalfCategory, sSubHCategory).Find(Function(x) x.Key = sType)

                If oSys IsNot Nothing Then
                    chk1H.Checked = (UCase(oSys.Value) = "TRUE")
                End If

                sSubHCategory = "2H"
                oSys = oSysmanager.GetSysSettings(_sHalfCategory, sSubHCategory).Find(Function(x) x.Key = sType)

                If oSys IsNot Nothing Then
                    chk2H.Checked = (UCase(oSys.Value) = "TRUE")
                End If
            Else
                chk1H.Visible = False
                chk2H.Visible = False
            End If
        End Sub

        Protected Sub rptGameLines_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
            If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim lbnBetLock As LinkButton = CType(e.Item.FindControl("lbnBetLock"), LinkButton)
                Dim lbnMLLock As LinkButton = CType(e.Item.FindControl("lbnMLLock"), LinkButton)
                Dim lbnTPointLock As LinkButton = CType(e.Item.FindControl("lbnTPointLock"), LinkButton)

                Dim oRow As DataRow = CType(e.Item.DataItem, DataRow)
                Dim sContext As String = UCase(SafeString(oRow("Context")))
                Dim sIsLock As String = ""
                ' Is Game lock?
                Select Case sContext
                    Case "CURRENT"
                        sIsLock = SafeString(IIf(SBCBL.std.SafeString(oRow("IsGameBetLocked")) = "Y", "Y", "N"))
                    Case "1H"
                        sIsLock = SafeString(IIf(SBCBL.std.SafeString(oRow("IsFirstHalfLocked")) = "Y", "Y", "N"))
                    Case "2H"
                        sIsLock = SafeString(IIf(SBCBL.std.SafeString(oRow("IsSecondHalfLocked")) = "Y", "Y", "N"))
                End Select
                lbnBetLock.Text = sIsLock

                ' Is Game 1H lock?
                Select Case sContext
                    Case "CURRENT"
                        sIsLock = SafeString(IIf(SBCBL.std.SafeString(oRow("IsGameMLLocked")) = "Y", "Y", "N"))
                    Case "1H"
                        sIsLock = SafeString(IIf(SBCBL.std.SafeString(oRow("IsFirstHalfMLLocked")) = "Y", "Y", "N"))
                    Case "2H"
                        sIsLock = SafeString(IIf(SBCBL.std.SafeString(oRow("IsSecondHalfMLLocked")) = "Y", "Y", "N"))
                End Select
                lbnMLLock.Text = sIsLock

                ' Is Game 2H lock?
                Select Case sContext
                    Case "CURRENT"
                        sIsLock = SafeString(IIf(SBCBL.std.SafeString(oRow("IsGameTPointLocked")) = "Y", "Y", "N"))
                    Case "1H"
                        sIsLock = SafeString(IIf(SBCBL.std.SafeString(oRow("IsFirstHalfTPointLocked")) = "Y", "Y", "N"))
                    Case "2H"
                        sIsLock = SafeString(IIf(SBCBL.std.SafeString(oRow("IsSecondHalfTPointLocked")) = "Y", "Y", "N"))
                End Select
                lbnTPointLock.Text = sIsLock
            End If
        End Sub


        Private Function LoadGames(ByVal psGameType As String, ByVal poRepeater As Repeater) As Boolean
            ''Get primary bookmakers by gametype
            Dim oAgentManager As New CAgentManager()
            Dim oData As DataTable = oAgentManager.GetAgentsBySuperID(UserSession.UserID, Nothing)
            Dim oToday = GetEasternDate()
            Dim sBookmaker As String = ddlBookmaker.SelectedValue

            ''Get games
            Dim oGameShows As New Dictionary(Of String, DataRow())
            Dim odtGames As DataTable = New SBCBL.Managers.CGameManager().GetAvailableGames(psGameType, oToday, False, False, ddlContext.SelectedValue)
            Dim nTotalGame As Integer = 0
            Dim oListLines As New Dictionary(Of String, DataRow)

            oGameShows.Add("Current", Nothing)
            oGameShows.Add("1H", Nothing)
            oGameShows.Add("2H", Nothing)

            For Each sContext As String In oGameShows.Keys.ToArray()
                '' reset the list of line for new Context
                oListLines.Clear()
                Dim sWhere As String = CreateContextFilterQuery(psGameType, sContext)
                If sWhere <> "" Then
                    sWhere &= " and Context = " & SQLString(sContext)
                Else
                    sWhere = "Context = " & SQLString(sContext)
                End If

                If IsManual AndAlso Not SBCBL.CFileDBKeys.TIGER_SB_BOOKMAKER.Equals(sBookmaker, StringComparison.CurrentCultureIgnoreCase) Then
                    ' sWhere &= " and ( Bookmaker = " & SQLString(SBCBL.CFileDBKeys.TIGER_SB_BOOKMAKER) & " or  Bookmaker =" & SQLString(sBookmaker) & ")"
                    If oData IsNot Nothing Then
                        ' sWhere &= String.Format(" and Bookmaker in ('{0}','{1}')", sBookmaker, oData(0)("AgentID"))
                        sWhere &= " and ( Bookmaker = " & SQLString(oData(0)("AgentID")) & " or  Bookmaker =" & SQLString(sBookmaker) & ")"
                    Else
                        sWhere &= " and Bookmaker = " & SQLString(sBookmaker)
                    End If
                Else
                        sWhere &= " and Bookmaker = " & SQLString(sBookmaker)
                End If



                odtGames.DefaultView.RowFilter = sWhere
                Dim oDTFiltered As DataTable = odtGames.DefaultView.ToTable()

                For Each oRow As DataRow In oDTFiltered.Rows
                    AddLineToList(oRow, oListLines, sBookmaker, odtGames)
                Next

                oGameShows(sContext) = oListLines.Values.ToArray()
                nTotalGame += oListLines.Values.Count
                If oListLines.Count = 0 Then
                    oGameShows.Remove(sContext)
                End If
            Next

            poRepeater.DataSource = oGameShows
            poRepeater.DataBind()
            Return nTotalGame > 0
        End Function

        '' this create filter to avoide weird line
        Private Function CreateContextFilterQuery(ByVal psGameType As String, ByVal psContext As String) As String
            Dim sCategory As String = SBCBL.std.GetSiteType & " LineRules"
            Dim oSysManager As New CSysSettingManager()
            Dim sGameTypeKey As String = psGameType.Replace(" ", "_")
            Dim sSubCategory As String = sGameTypeKey & "_" & psContext
            Dim oListWhere As New List(Of String)

            Dim oListSettings As CSysSettingList = UserSession.SysSettings(sCategory, sSubCategory)
            Dim oAwaySpreadMoneyGT As CSysSetting = oListSettings.Find(Function(x) x.Key = "AwaySpreadMoneyGT")
            Dim nAwaySpreadMoneyGT As Double = 0
            If oAwaySpreadMoneyGT IsNot Nothing Then
                nAwaySpreadMoneyGT = SafeDouble(oAwaySpreadMoneyGT.Value)
            End If
            Dim oAwaySpreadMoneyLT As CSysSetting = oListSettings.Find(Function(x) x.Key = "AwaySpreadMoneyLT")
            Dim nAwaySpreadMoneyLT As Double = 0
            If oAwaySpreadMoneyLT IsNot Nothing Then
                nAwaySpreadMoneyLT = SafeDouble(oAwaySpreadMoneyLT.Value)
            End If

            Dim oTotalPointGT As CSysSetting = oListSettings.Find(Function(x) x.Key = "TotalPointGT")
            Dim nTotalPointGT As Double = 0
            If oTotalPointGT IsNot Nothing Then
                nTotalPointGT = SafeDouble(oTotalPointGT.Value)
            End If

            Dim oTotalPointLT As CSysSetting = oListSettings.Find(Function(x) x.Key = "TotalPointLT")
            Dim nTotalPointLT As Double = 0
            If oTotalPointLT IsNot Nothing Then
                nTotalPointLT = SafeDouble(oTotalPointLT.Value)
            End If

            If nAwaySpreadMoneyGT <> 0 Then
                oListWhere.Add("AwaySpreadMoney > " & SafeString(nAwaySpreadMoneyGT))
            End If

            If nAwaySpreadMoneyLT <> 0 Then
                oListWhere.Add("AwaySpreadMoney < " & SafeString(nAwaySpreadMoneyLT))
            End If

            If nTotalPointGT <> 0 Then
                oListWhere.Add("TotalPoints  > " & SafeString(nTotalPointGT))
            End If
            If (nTotalPointLT <> 0) Then
                oListWhere.Add("TotalPoints < " & nTotalPointLT.ToString())
            End If

            '' game circled
            If chkCircle.Checked Then
                oListWhere.Add("isnull(IsCircle,'N') ='Y'")
            End If

            If chkAdded.Checked Then
                oListWhere.Add("isnull(IsAddedGame,'N') ='Y'")
            End If

            Return String.Join(" and ", oListWhere.ToArray())
        End Function

        Private Sub AddLineToList(ByVal poLineRow As DataRow, ByVal poLines As Dictionary(Of String, DataRow), ByVal psPrimaryBookmaker As String, ByVal odtGames As DataTable)
            Dim sGameID As String = SafeString(poLineRow("GameID"))
            If IsManual Then
                psPrimaryBookmaker = SBCBL.CFileDBKeys.TIGER_SB_BOOKMAKER
            End If
            If IsManual AndAlso (Not SafeString(poLineRow("Bookmaker")).Equals(ddlBookmaker.SelectedValue, StringComparison.CurrentCultureIgnoreCase) AndAlso SafeString(poLineRow("Bookmaker")).Equals(SBCBL.CFileDBKeys.TIGER_SB_BOOKMAKER, StringComparison.CurrentCultureIgnoreCase) AndAlso SafeInteger(odtGames.Compute("count(Context)", "GameID=" & SQLString(sGameID) & " and (Bookmaker=" & SQLString(SBCBL.CFileDBKeys.TIGER_SB_BOOKMAKER) & " or Bookmaker=" & SQLString(ddlBookmaker.SelectedValue) & ")")) < 2) Then
                Return
            End If
            '' If has line with primary bookmaker, just return
            If poLines.ContainsKey(sGameID) AndAlso SafeString(poLines(sGameID)("Bookmaker")) = psPrimaryBookmaker Then
                Return
            End If

            Dim nAwaySpreadMoney As Double = SafeInteger(poLineRow("AwaySpreadMoney"))
            Dim nTotalPointsOverMoney As Double = SafeInteger(poLineRow("TotalPointsOverMoney"))
            Dim nAwayMoneyLine As Double = SafeInteger(poLineRow("AwayMoneyLine"))
            Dim bCanBet As Boolean = nAwaySpreadMoney <> 0 OrElse nTotalPointsOverMoney <> 0 OrElse nAwayMoneyLine <> 0
            If bCanBet Then
                poLines(sGameID) = poLineRow
            End If
            'Return bCanBet
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

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                '' load current filter settings
                ddlContext.SelectedValue = GameContext
                ddlGameType.SelectedValue = GameType
                ddlBookmaker.SelectedValue = BookMaker
                chkCircle.Checked = GameCircle
                chkAdded.Checked = AddedGame

                ddlContext.Enabled = Not chkCircle.Checked
                ddlGameType.Enabled = Not chkCircle.Checked

                BindGame()
            End If
        End Sub

        Protected Function FormatValue(ByVal poValue As Object) As String
            If poValue.GetType() Is GetType(DBNull) Then
                Return ""
            Else
                Return SafeDouble(poValue).ToString()
            End If
        End Function

        Protected Sub ddlBookmaker_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlBookmaker.SelectedIndexChanged
            BindGame()
        End Sub

        Protected Sub ddlContext_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlContext.SelectedIndexChanged, ddlGameType.SelectedIndexChanged
            BindGame()
        End Sub

        Protected Sub chkCircle_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkCircle.CheckedChanged
            ddlContext.Enabled = Not chkCircle.Checked
            ddlGameType.Enabled = Not chkCircle.Checked
            BindGame()
        End Sub

        Protected Sub btnAutomatic_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAutomatic.Click
            IsManual = False
            BindGame()
        End Sub

        Protected Sub btnManual_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnManual.Click
            IsManual = True
            BindGame()
        End Sub

        Protected Sub chkOffline_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim chk As CheckBox = CType(sender, CheckBox)
            Dim oSysManager As New SBCBL.Managers.CSysSettingManager()
            Dim oCacheManager As New SBCBL.CacheUtils.CCacheManager()
            Dim sSubCategory As String = ""
            Dim HFGameType As HiddenField = CType(chk.Parent.FindControl("HFGameType"), HiddenField)
            Dim sValue = IIf(chk.Checked, "TRUE", "FALSE")

            If chk.ID = "chk1H" Then
                sSubCategory = "1H"
            Else
                sSubCategory = "2H"
            End If

            Dim oSys As CSysSetting = oCacheManager.GetSysSettings(_sHalfCategory, sSubCategory).Find(Function(x) x.Key = HFGameType.Value)
            If oSys Is Nothing Then
                oSysManager.AddSysSetting(_sHalfCategory, HFGameType.Value, sValue, sSubCategory)
            Else
                oSysManager.UpdateKeyValue(oSys.SysSettingID, HFGameType.Value, sValue, 0, sSubCategory)
            End If

            oCacheManager.ClearSysSettings(_sHalfCategory, "1H")
            oCacheManager.ClearSysSettings(_sHalfCategory, "2H")
        End Sub

        Protected Sub chkAdded_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkAdded.CheckedChanged
            BindGame()
        End Sub

        Protected Sub btnReload_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnReload.Click
            BindGame()
        End Sub

    End Class
End Namespace