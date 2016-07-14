Imports SBCBL.std
Imports System.Data


Namespace SBSSuperAdmin
    Partial Class ManualLines
        Inherits SBCBL.UI.CSBCPage

        'Private _sBookmakerType As String = SBCBL.std.GetSiteType & " BookmakerType"

        'Private ReadOnly Property GameTypes() As List(Of String)
        '    Get

        '        Dim oListgames As New List(Of String)()
        '        For Each oItem As ListItem In cblGameTypes.Items
        '            If oItem.Selected Then
        '                oListgames.Add(oItem.Value)
        '            End If
        '        Next

        '        Return oListgames
        '    End Get
        'End Property

        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Quater Lines"
            SideMenuTabName = "MANUAL_LINE"
            Me.MenuTabName = "GAME MANAGEMENT"
            DisplaySubTitlle(Me.Master, "Quater Lines")
        End Sub

        'Private Sub BindGame()
        '    rptMain.DataSource = GameTypes
        '    rptMain.DataBind()
        'End Sub

        'Protected Sub rptGameLines_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptMain.ItemCommand
        '    Select Case e.CommandName
        '        Case "SAVE"
        '            If SaveLineByRow(e.Item, True) Then
        '                ClientAlert("Line has been save successfull.")
        '            Else
        '                ClientAlert("Line can't be saved.")
        '            End If
        '        Case "DELETE"
        '            Dim sLineID As String = CType(e.Item.FindControl("hfGameLineID"), HiddenField).Value
        '            Dim oManager As New SBCBL.Managers.CGameLineManager()
        '            If oManager.DeleteGameLine(sLineID) Then

        '                ClientAlert("Line has been deleted successfull.")
        '                ClearRow(e.Item)
        '            Else
        '                ClientAlert("Line can't be deleted.")
        '            End If
        '    End Select
        'End Sub

        'Private Function SaveLineByRow(ByVal poItem As RepeaterItem, Optional ByVal pbWarn As Boolean = False) As Boolean
        '    Dim oLine As New SBCBL.CacheUtils.CGameLine()
        '    oLine.GameID = CType(poItem.FindControl("hfGameID"), HiddenField).Value
        '    oLine.GameLineID = CType(poItem.FindControl("hfGameLineID"), HiddenField).Value
        '    oLine.FeedSource = "MANUAL"
        '    oLine.Bookmaker = "MANUAL"
        '    oLine.Context = CType(poItem.FindControl("hfContext"), HiddenField).Value
        '    oLine.AwaySpread = SafeDouble(CType(poItem.FindControl("txtAwaySpread"), TextBox).Text)
        '    oLine.AwaySpreadMoney = SafeDouble(CType(poItem.FindControl("txtAwaySpreadMoney"), TextBox).Text)
        '    oLine.TotalPoints = SafeDouble(CType(poItem.FindControl("txtAwayTotal"), TextBox).Text)
        '    oLine.TotalPointsOverMoney = SafeDouble(CType(poItem.FindControl("txtTotalPointsOverMoney"), TextBox).Text)
        '    oLine.AwayMoneyLine = SafeDouble(CType(poItem.FindControl("txtAwayMoneyLine"), TextBox).Text)
        '    oLine.HomeSpread = SafeDouble(CType(poItem.FindControl("txtHomeSpread"), TextBox).Text)
        '    oLine.HomeSpreadMoney = SafeDouble(CType(poItem.FindControl("txtHomeSpreadMoney"), TextBox).Text)
        '    oLine.TotalPointsUnderMoney = SafeDouble(CType(poItem.FindControl("txtTotalPointsUnderMoney"), TextBox).Text)
        '    oLine.HomeMoneyLine = SafeDouble(CType(poItem.FindControl("txtHomeMoney"), TextBox).Text)

        '    Dim sErrorField As String = ""
        '    If Not ValidateLine(oLine, sErrorField) And pbWarn Then
        '        ClientAlert("Please input " & sErrorField)
        '        Return False
        '    End If

        '    Dim oGameLineManager As New SBCBL.Managers.CGameLineManager()
        '    Return oGameLineManager.SaveLine(oLine)
        'End Function

        'Private Function ClearRow(ByVal poItem As RepeaterItem) As Boolean

        '    CType(poItem.FindControl("txtAwaySpread"), TextBox).Text = ""
        '    CType(poItem.FindControl("txtAwaySpreadMoney"), TextBox).Text = ""

        '    CType(poItem.FindControl("txtAwayTotal"), TextBox).Text = ""
        '    CType(poItem.FindControl("txtTotalPoints2"), TextBox).Text = ""
        '    CType(poItem.FindControl("txtTotalPointsOverMoney"), TextBox).Text = ""
        '    CType(poItem.FindControl("txtAwayMoneyLine"), TextBox).Text = ""
        '    CType(poItem.FindControl("txtHomeSpread"), TextBox).Text = ""
        '    CType(poItem.FindControl("txtHomeSpreadMoney"), TextBox).Text = ""
        '    CType(poItem.FindControl("txtTotalPointsUnderMoney"), TextBox).Text = ""
        '    CType(poItem.FindControl("txtHomeMoney"), TextBox).Text = ""
        'End Function

        'Public Function ValidateLine(ByVal poLine As SBCBL.CacheUtils.CGameLine, ByRef poMissField As String) As Boolean
        '    Dim bResult As Boolean = True
        '    Dim sMissField As String = ""
        '    If poLine.AwaySpreadMoney = 0 Then
        '        sMissField = "Away Spread Money"
        '        bResult = False
        '    ElseIf poLine.TotalPointsOverMoney = 0 Then
        '        sMissField = "Total Points Over Money"
        '        bResult = False
        '    ElseIf poLine.HomeSpreadMoney = 0 Then
        '        sMissField = "Home Spread Money"
        '        bResult = False
        '    ElseIf poLine.TotalPointsUnderMoney = 0 Then
        '        sMissField = "Total Points Under Money"
        '        bResult = False
        '    End If
        '    poMissField = sMissField
        '    Return bResult
        'End Function

        'Protected Sub rptMain_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptMain.ItemDataBound
        '    Dim sType As String = SafeString(e.Item.DataItem)
        '    Dim rptBets As Repeater = e.Item.FindControl("rptBets")

        '    '' load game by game type to each of rptBets
        '    LoadGames(sType, rptBets)

        'End Sub

        'Private Function LoadGames(ByVal psGameType As String, ByVal poRepeater As Repeater) As Boolean
        '    ''Get primary bookmakers by gametype

        '    Dim oToday = GetEasternDate()
        '    Dim sBookmaker As String = UserSession.SysSettings(_sBookmakerType).GetValue(psGameType)
        '    ' '' if doesn't found book maker, just return, SINCE NOW WE DOESNT FILTER BY BOOK MAKER
        '    If sBookmaker = "" Then
        '        poRepeater.DataSource = Nothing
        '        poRepeater.DataBind()
        '        Return False
        '    End If

        '    'cblGameTypes.Items.fi
        '    Dim oListGameTypes As New List(Of String)
        '    Dim oContexts As New List(Of String)
        '    Dim bRequireOdd As Boolean = chkOdds.Checked

        '    For Each oItem As ListItem In cblGameTypes.Items
        '        If oItem.Selected Then
        '            oListGameTypes.Add(oItem.Value)
        '        End If
        '    Next

        '    For Each oItem As ListItem In cblContext.Items
        '        If oItem.Selected Then
        '            oContexts.Add(oItem.Value)
        '        End If
        '    Next

        '    ''Get games
        '    Dim oGameShows As New Dictionary(Of String, DataTable)
        '    Dim odtGames As DataTable = New SBCBL.Managers.CGameManager().GetAvailableGamesQuaters(psGameType, oToday, oContexts.ToArray(), bRequireOdd)

        '    For Each sContext As String In oContexts
        '        oGameShows.Add(SafeString(sContext), Nothing)
        '    Next

        '    Dim oListTempRow As New List(Of DataRow)

        '    Dim oList As New List(Of Guid)
        '    For Each oRow As DataRow In odtGames.Rows
        '        If oList.Contains(oRow("GameID")) Then
        '            Continue For
        '        End If

        '        oList.Add(oRow("GameID"))

        '        For Each sContext As String In oContexts
        '            Dim sWhere As String = String.Format("GameID= {0} and Context = {1} ", SQLString(oRow("GameID")), SQLString(sContext))
        '            odtGames.DefaultView.RowFilter = sWhere
        '            If odtGames.DefaultView.ToTable().Rows.Count = 0 Then
        '                Dim oGameID As Guid = CType(oRow("GameID"), Guid)
        '                oListTempRow.Add(CreateNewGameLine(odtGames, oRow, oGameID, sContext))
        '            End If
        '        Next
        '    Next

        '    If Not bRequireOdd Then
        '        For Each oRow As DataRow In oListTempRow
        '            odtGames.Rows.Add(oRow)
        '        Next
        '    End If


        '    For Each sContext As String In oGameShows.Keys.ToArray()
        '        Dim sWhere As String = "Context = " & SQLString(sContext)

        '        odtGames.DefaultView.RowFilter = sWhere
        '        Dim oDTFiltered As DataTable = odtGames.DefaultView.ToTable()

        '        '' sorting the result
        '        oDTFiltered.DefaultView.Sort = "GameDate,HomeRotationNumber"

        '        oGameShows(sContext) = oDTFiltered.DefaultView.ToTable()
        '    Next

        '    poRepeater.DataSource = oGameShows
        '    poRepeater.DataBind()
        'End Function

        'Private Function CreateNewGameLine(ByVal poDT As DataTable, ByVal poRowGame As DataRow, ByVal poGameID As Guid, ByVal psContext As String) As DataRow
        '    Dim oRow As DataRow = poDT.NewRow()
        '    oRow("GameLineID") = newGUID()
        '    oRow("GameID") = poGameID
        '    oRow("LastUpdated") = DateTime.Now.ToUniversalTime()
        '    oRow("Context") = psContext
        '    oRow("Bookmaker") = "MANUAL"
        '    oRow("FeedSource") = "MANUAL"

        '    oRow("GameDate") = poRowGame("GameDate")
        '    oRow("GameType") = poRowGame("GameType")
        '    oRow("HomeTeam") = poRowGame("HomeTeam")
        '    oRow("HomeRotationNumber") = poRowGame("HomeRotationNumber")
        '    oRow("AwayTeam") = poRowGame("AwayTeam")
        '    oRow("AwayRotationNumber") = poRowGame("AwayRotationNumber")
        '    oRow("AwayRotationNumber") = poRowGame("AwayRotationNumber")
        '    Return oRow
        'End Function

        'Protected Sub rptBets_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
        '    If e.Item.ItemType = ListItemType.Footer Or e.Item.ItemType = ListItemType.Header Or e.Item.ItemType = ListItemType.Pager Then
        '        Return
        '    End If

        '    Dim rptGameLines As Repeater = CType(e.Item.FindControl("rptGameLines"), Repeater)
        '    Dim lblNoGameLine As Label = CType(e.Item.FindControl("lblNoGameLine"), Label)

        '    Dim oDT As DataTable = CType(e.Item.DataItem, KeyValuePair(Of String, DataTable)).Value
        '    rptGameLines.DataSource = oDT
        '    rptGameLines.DataBind()
        '    lblNoGameLine.Visible = oDT.Rows.Count = 0
        '    rptGameLines.Visible = oDT.Rows.Count >= 0
        'End Sub

        'Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '    If Not IsPostBack Then
        '        BindGame()
        '    End If
        'End Sub

        'Protected Sub btnSave1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave1.Click, btnSave2.Click
        '    Dim bSuccess As Boolean = True
        '    For Each oItemMain As RepeaterItem In rptMain.Items
        '        Dim rptBets As Repeater = CType(oItemMain.FindControl("rptBets"), Repeater)

        '        For Each oItemGame As RepeaterItem In rptBets.Items
        '            Dim rptGameLines As Repeater = CType(oItemGame.FindControl("rptGameLines"), Repeater)
        '            For Each oItem As RepeaterItem In rptGameLines.Items
        '                If oItem.ItemType <> ListItemType.Item Or oItem.ItemType = ListItemType.Separator Then
        '                    Continue For
        '                End If

        '                Dim bEdit As Boolean = True
        '                '' INORGE if SA doesnt enter any infor in this row
        '                Select Case ""
        '                    Case SafeString(CType(oItem.FindControl("txtAwaySpreadMoney"), TextBox).Text), _
        '                        SafeString(CType(oItem.FindControl("txtHomeSpreadMoney"), TextBox).Text), _
        '                        SafeString(CType(oItem.FindControl("txtTotalPointsOverMoney"), TextBox).Text), _
        '                        SafeString(CType(oItem.FindControl("txtTotalPointsUnderMoney"), TextBox).Text)

        '                        Continue For
        '                End Select


        '                If Not SaveLineByRow(oItem) Then
        '                    bSuccess = False
        '                End If
        '            Next

        '        Next
        '    Next

        '    If Not bSuccess Then
        '        ClientAlert("There are some lines can't be saved.")
        '    Else
        '        ClientAlert("Game lines have been saved.")
        '    End If
        'End Sub

        'Protected Sub btnReload1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnReload1.Click, btnReload2.Click

        'End Sub

        'Protected Function FormatValue(ByVal poValue As Object) As String
        '    If poValue.GetType() Is GetType(DBNull) Then
        '        Return ""
        '    Else
        '        Return SafeDouble(poValue).ToString()
        '    End If
        'End Function

        'Protected Sub cblGameTypes_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cblGameTypes.SelectedIndexChanged, cblContext.SelectedIndexChanged
        '    BindGame()
        'End Sub

        'Protected Sub chkOdds_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkOdds.CheckedChanged
        '    BindGame()
        'End Sub

        
    End Class
End Namespace