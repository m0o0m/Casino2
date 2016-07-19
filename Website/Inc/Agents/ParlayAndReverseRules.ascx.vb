Imports SBCBL.Managers
Imports SBCBL.std
Imports SBCBL.CacheUtils
Imports System.Data
Imports System.Collections.Generic

Namespace SBSAgents
    Partial Class ParlayAndReverseRules
        Inherits SBCBL.UI.CSBCUserControl

        Dim oBetType As New System.Collections.Generic.Dictionary(Of String, String)()
        Private nNumBetType As Integer = 9
        Private nSysSettingKey As Integer = 0
        Private ncheckNotValid As Integer = 7
        Private nSecondHalfColumnShow As Integer = 3
        Private nSecondHalfRowShow As Integer = 8
        Private i As Integer = 1
        Private k As Integer = 0
        Private h As Integer = 0
        Private l As Integer = 0
        Private Yes As String = "Y"
        Private No As String = "N"
        Dim oCache As New CCacheManager()
        Dim oSysManager As New CSysSettingManager()
        Private MAX_PARLAY_IN_GAME As String = "MAX_PARLAY_IN_GAME_MVP"

#Region "Property"

        Private ReadOnly Property SuperAgentID() As String
            Get
                If UserSession.UserType = SBCBL.EUserType.Agent Then
                    Return UserSession.UserID
                Else
                    Return ddlAgents.SelectedValue
                End If
            End Get
        End Property

        Private ReadOnly Property AgentParlayType() As String
            Get
                Dim sParlayType As String = " ParlayType"
                If BetweenGames Then
                    sParlayType = " BWParlayType"
                End If
                Return SuperAgentID & sParlayType
            End Get
        End Property

        Private ReadOnly Property AgentReverseType() As String
            Get
                Dim sReverseType As String = " ReverseType"
                If BetweenGames Then
                    sReverseType = " BWReverseType"
                End If
                Return SuperAgentID & sReverseType
            End Get
        End Property

        Private ReadOnly Property ParlayType() As String
            Get
                Dim sParlayType As String = " ParlayType"
                If BetweenGames Then
                    sParlayType = " BWParlayType"
                End If
                If oSysManager.IsExistedCategory(SuperAgentID & sParlayType) Then
                    Return SuperAgentID & sParlayType
                End If
                Return SBCBL.std.GetSiteType & sParlayType

            End Get
        End Property

        Private ReadOnly Property ReverseType() As String
            Get
                Dim sReverseType As String = " ReverseType"
                If BetweenGames Then
                    sReverseType = " BWReverseType"
                End If

                If oSysManager.IsExistedCategory(SuperAgentID & sReverseType) Then
                    Return SuperAgentID & sReverseType
                End If
                Return SBCBL.std.GetSiteType & sReverseType
            End Get
        End Property

        Private ReadOnly Property ParlayRules() As String
            Get
                Dim sParlayRules As String = " ParlayRules"
                If BetweenGames Then
                    sParlayRules = " BWParlayRules"
                End If
                If oSysManager.IsExistedCategory(SuperAgentID & sParlayRules) Then
                    Return SuperAgentID & sParlayRules
                End If
                Return SBCBL.std.GetSiteType & sParlayRules
            End Get
        End Property

        Private ReadOnly Property AgentParlayRules() As String
            Get
                Dim sParlayRules As String = " ParlayRules"
                If BetweenGames Then
                    sParlayRules = " BWParlayRules"
                End If

                Return SuperAgentID & sParlayRules
            End Get
        End Property

        Public Property BetweenGames() As Boolean
            Get
                Return CType(ViewState("_BETWEEN_GAMES"), Boolean)
            End Get
            Set(ByVal value As Boolean)
                ViewState("_BETWEEN_GAMES") = value
            End Set
        End Property

        Public Property GameType() As String
            Get
                Return CType(ViewState("GameType"), String)
            End Get
            Set(ByVal value As String)
                ViewState("GameType") = value
            End Set
        End Property

        Protected Property BetType() As Dictionary(Of String, String)
            Set(ByVal value As Dictionary(Of String, String))
                oBetType = value
            End Set
            Get
                If oBetType.Count = 0 Then
                    oBetType("Spread") = "Spread"
                    oBetType("Total") = "Total Points"
                    oBetType("ML") = "Money Line"
                    oBetType("1HSpread") = "1H Spread"
                    oBetType("1HTotal") = "1H Total Points"
                    oBetType("1HML") = "1H Money Line"
                    oBetType("2HSpread") = "2H Spread"
                    oBetType("2HTotal") = "2H Total Points"
                    oBetType("2HML") = "2H Money Line"
                End If
                Return oBetType
            End Get
        End Property

        '' key for 1/2 under grid
        Protected ReadOnly Property SysSettingKey() As String()
            Get
                Dim arrKey As String() = New String(35) {"Spread_Total", "Spread_ML", "Total_ML", "Spread_1HSpread", "Total_1HSpread", "ML_1HSpread", "Spread_1HTotal", "Total_1HTotal", "ML_1HTotal", "1HSpread_1HTotal", "Spread_1HML", "Total_1HML", "ML_1HML", "1HSpread_1HML", "1HTotal_1HML", "Spread_2HSpread", "Total_2HSpread", "ML_2HSpread", "1HSpread_2HSpread", "1HTotal_2HSpread", "1HML_2HSpread", "Spread_2HTotal", "Total_2HTotal", "ML_2HTotal", "1HSpread_2HTotal", "1HTotal_2HTotal", "1HML_2HTotal", "2HSpread_2HTotal", "Spread_2HML", "Total_2HML", "ML_2HML", "1HSpread_2HML", "1HTotal_2HML", "1HML_2HML", "2HSpread_2HML", "2HTotal_2HML"}
                Return arrKey
            End Get

        End Property
        '' key for 1hspread row 4 last 3 checkbox
        Protected ReadOnly Property SysSettingKeySTM() As String()
            Get
                Dim arrKey As String() = New String(2) {"1HSpread_1HSpread", "1HTotal_1HSpread", "1HML_1HSpread"}
                Return arrKey
            End Get
        End Property

        Protected ReadOnly Property SysSettingKeyHH() As String()
            Get
                Dim arrKey As String() = New String(1) {"1HTotal_1HTotal", "1HML_1HTotal"}
                Return arrKey
            End Get
        End Property

        Protected ReadOnly Property SysSettingKey2HH() As String()
            Get
                Dim arrKey As String() = New String(1) {"2HSpread_2HSpread", "2HTotal_2HSpread"}
                Return arrKey
            End Get
        End Property

        Protected ReadOnly Property ListBetType() As List(Of String)
            Get
                Dim lstBetType As New List(Of String)
                lstBetType.Add("Football")
                lstBetType.Add("Basketball")
                lstBetType.Add("Soccer")
                'If Not BetweenGames Then
                lstBetType.Add("Baseball")
                lstBetType.Add("Hockey")
                ' End If

                Return lstBetType
            End Get
        End Property

#End Region

        Private Sub bindAgents(ByVal psSuperAdminID As String)
            Dim oAgentManager As New CAgentManager()
            Dim oData As DataTable = oAgentManager.GetAgentsBySuperID(psSuperAdminID, Nothing)
            ddlAgents.DataSource = oData
            ddlAgents.DataTextField = "FullName"
            ddlAgents.DataValueField = "AgentID"
            ddlAgents.DataBind()
        End Sub

#Region "Page Events"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                    dvAgents.Visible = True
                    bindAgents(UserSession.UserID)
                Else
                    dvAgents.Visible = False
                End If
                ParlayDataBind()
            End If
        End Sub
#End Region

#Region "Controls Events"

        Protected Sub rptPRRules_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptPRRules.ItemCommand
            Dim txtMaxParlayInGame As TextBox = e.Item.FindControl("txtMaxParlayInGame")
            If String.IsNullOrEmpty(txtMaxParlayInGame.Text) Then
                ClientAlert("Please, input Max Spread Allowed", True)
                Return
            End If
            Dim sPortType As String = e.CommandArgument
            If BetweenGames Then
                Return
            End If
            Select Case e.CommandName
                Case "MAX_PARLAY_IN_GAME"
                    If oSysManager.IsExistedKey(MAX_PARLAY_IN_GAME, "", sPortType) Then
                        oSysManager.UpdateValue(MAX_PARLAY_IN_GAME, "", sPortType, txtMaxParlayInGame.Text)
                        UserSession.Cache.ClearSysSettings(MAX_PARLAY_IN_GAME)
                    Else
                        oSysManager.AddSysSetting(MAX_PARLAY_IN_GAME, sPortType, txtMaxParlayInGame.Text)
                        UserSession.Cache.ClearSysSettings(MAX_PARLAY_IN_GAME)
                    End If
                    ClientAlert("Save Successful", True)
            End Select
        End Sub

        Protected Sub rptPRRules_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptPRRules.ItemDataBound
            Dim grPRRules As DataGrid = CType(e.Item.FindControl("grPRRules"), DataGrid)
            Dim pnMaxParlayInGame = CType(e.Item.FindControl("pnMaxParlayInGame"), Panel)
            Select Case CType(e.Item.DataItem, String)
                Case "Football"
                    BetType.Item("Spread") = "Spread"
                    grPRRules.DataSource = BetType
                    GameType = "Football"
                    If Not BetweenGames AndAlso UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                        pnMaxParlayInGame.Visible = True
                    End If
                Case "Basketball"
                    BetType.Item("Spread") = "Spread"
                    grPRRules.DataSource = BetType
                    GameType = "Basketball"
                    If Not BetweenGames AndAlso UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                        pnMaxParlayInGame.Visible = True
                    End If
                Case "Baseball"
                    BetType.Item("Spread") = "Run Line"
                    grPRRules.DataSource = BetType
                    GameType = "Baseball"
                Case "Hockey"
                    BetType.Item("Spread") = "Spread"
                    grPRRules.DataSource = BetType
                    GameType = "Hockey"
                Case "Soccer"
                    BetType.Item("Spread") = "Spread"
                    grPRRules.DataSource = BetType
                    GameType = "Soccer"
            End Select
            grPRRules.DataBind()

            grPRRules.Columns(9).Visible = Not BetweenGames 'Parlay
            grPRRules.Columns(10).Visible = Not BetweenGames 'Reverse
        End Sub

        Protected Sub grPRRules_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs)
            '' set Game Type in header
            If e.Item.ItemType = ListItemType.Header Then
                e.Item.Cells(0).Text = GameType

                Select Case GameType
                    Case "Football"
                        e.Item.Cells(1).Text = "Spread"
                    Case "Basketball"
                        e.Item.Cells(1).Text = "Spread"
                    Case "Baseball"
                        e.Item.Cells(1).Text = "Run Line"
                    Case "Hockey"
                        e.Item.Cells(1).Text = "Spread"
                    Case "Soccer"
                        e.Item.Cells(1).Text = "Spread"

                End Select

            End If

            If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
                BindParlayAndReverse(e)

                ''show 1hspread-1hspread and 1htotal-1htotal and 2hspread-2hspread and 2htotal-2htotal for football,Basketball,Soccer
                If i = 4 And BetweenGames Then
                    CType(e.Item.Cells(4).Controls(1), CheckBox).Visible = True
                    CType(e.Item.Cells(4).Controls(1), CheckBox).Checked = False
                    Dim drParlay As DataRow = oSysManager.GetValue(ParlayRules, GameType, "1HSpread_1HSpread")
                    CType(e.Item.Cells(4).Controls(3), HiddenField).Value = "1HSpread_1HSpread"
                    If drParlay IsNot Nothing Then
                        If SafeString(drParlay("Value")).Equals(Yes) Then
                            CType(e.Item.Cells(4).Controls(1), CheckBox).Checked = True
                        End If

                    End If
                End If
                If i = 5 And BetweenGames Then
                    CType(e.Item.Cells(5).Controls(1), CheckBox).Visible = True
                    CType(e.Item.Cells(5).Controls(1), CheckBox).Checked = False
                    Dim drParlay As DataRow = oSysManager.GetValue(ParlayRules, GameType, "1HTotal_1HTotal")
                    CType(e.Item.Cells(5).Controls(3), HiddenField).Value = "1HTotal_1HTotal"
                    If drParlay IsNot Nothing Then
                        If SafeString(drParlay("Value")).Equals(Yes) Then
                            CType(e.Item.Cells(5).Controls(1), CheckBox).Checked = True
                        End If

                    End If
                End If
                If i = 7 And BetweenGames Then
                    CType(e.Item.Cells(7).Controls(1), CheckBox).Visible = True
                    CType(e.Item.Cells(7).Controls(1), CheckBox).Checked = False
                    Dim drParlay As DataRow = oSysManager.GetValue(ParlayRules, GameType, "2HSpread_2HSpread")
                    CType(e.Item.Cells(7).Controls(3), HiddenField).Value = "2HSpread_2HSpread"
                    If drParlay IsNot Nothing Then
                        If SafeString(drParlay("Value")).Equals(Yes) Then
                            CType(e.Item.Cells(7).Controls(1), CheckBox).Checked = True
                        End If

                    End If
                End If
                If i = 8 And BetweenGames Then
                    CType(e.Item.Cells(8).Controls(1), CheckBox).Visible = True
                    CType(e.Item.Cells(8).Controls(1), CheckBox).Checked = False
                    Dim drParlay As DataRow = oSysManager.GetValue(ParlayRules, GameType, "2HTotal_2HTotal")
                    CType(e.Item.Cells(8).Controls(3), HiddenField).Value = "2HTotal_2HTotal"
                    If drParlay IsNot Nothing Then
                        If SafeString(drParlay("Value")).Equals(Yes) Then
                            CType(e.Item.Cells(8).Controls(1), CheckBox).Checked = True
                        End If

                    End If
                End If
                ''end

                ''show spread-spread and total-total
                If i = 1 And BetweenGames Then
                    CType(e.Item.Cells(1).Controls(1), CheckBox).Visible = True
                    CType(e.Item.Cells(1).Controls(1), CheckBox).Checked = False
                    Dim drParlay As DataRow = oSysManager.GetValue(ParlayRules, GameType, "Spread_Spread")
                    CType(e.Item.Cells(1).Controls(3), HiddenField).Value = "Spread_Spread"
                    If drParlay IsNot Nothing Then
                        If SafeString(drParlay("Value")).Equals(Yes) Then
                            CType(e.Item.Cells(1).Controls(1), CheckBox).Checked = True
                        End If

                    End If
                End If
                If i = 2 And BetweenGames Then
                    CType(e.Item.Cells(2).Controls(1), CheckBox).Visible = True
                    CType(e.Item.Cells(2).Controls(1), CheckBox).Checked = False
                    Dim drParlay As DataRow = oSysManager.GetValue(ParlayRules, GameType, "Total_Total")
                    CType(e.Item.Cells(2).Controls(3), HiddenField).Value = "Total_Total"
                    If drParlay IsNot Nothing Then
                        If SafeString(drParlay("Value")).Equals(Yes) Then
                            CType(e.Item.Cells(2).Controls(1), CheckBox).Checked = True
                        End If

                    End If
                End If

                ''end
                CType(e.Item.Cells(0).Controls(3), HiddenField).Value = GameType
                If i <= nNumBetType AndAlso i > 1 Then
                    For j As Integer = 1 To i - 1
                        If Not BetweenGames Then
                            If i >= ncheckNotValid AndAlso j < ncheckNotValid Then
                                'If j < nSecondHalfColumnShow AndAlso i <= nSecondHalfRowShow Then '' condition for show 4 check box in 2h
                                '    CType(e.Item.Cells(j).Controls(1), CheckBox).Visible = True
                                '    GoTo checkValue
                                'Else
                                '    CType(e.Item.Cells(j).Controls(1), CheckBox).Visible = False
                                'End If
                                CType(e.Item.Cells(j).Controls(1), CheckBox).Visible = False
                            Else
checkValue:
                                CType(e.Item.Cells(j).Controls(1), CheckBox).Visible = True
                                CType(e.Item.Cells(j).Controls(1), CheckBox).Checked = False

                                CType(e.Item.Cells(j).Controls(3), HiddenField).Value = SysSettingKey(nSysSettingKey)
                                Dim drParlay As DataRow = oSysManager.GetValue(ParlayRules, GameType, SysSettingKey(nSysSettingKey))
                                If drParlay IsNot Nothing Then
                                    If SafeString(drParlay("Value")).Equals(Yes) Then
                                        CType(e.Item.Cells(j).Controls(1), CheckBox).Checked = True
                                    End If

                                End If

                            End If
                        Else

                            '' show checkbox for between game
                            ''show ML_ML for football,Basketball,Soccer start
                            If (GameType.Equals("Football", StringComparison.CurrentCultureIgnoreCase)) Or (GameType.Equals("Basketball", StringComparison.CurrentCultureIgnoreCase)) Or (GameType.Equals("Soccer", StringComparison.CurrentCultureIgnoreCase)) Then
                                If i >= 3 Then
                                    If (i = 3) Then 'money line
                                        CType(e.Item.Cells(j + 1).Controls(1), CheckBox).Visible = True
                                        CType(e.Item.Cells(j + 1).Controls(1), CheckBox).Checked = False
                                        CType(e.Item.Cells(j + 1).Controls(3), HiddenField).Value = "ML_ML"
                                        Dim drParlay As DataRow = oSysManager.GetValue(ParlayRules, GameType, "ML_ML")
                                        If drParlay IsNot Nothing Then
                                            If SafeString(drParlay("Value")).Equals(Yes) Then
                                                CType(e.Item.Cells(j + 1).Controls(1), CheckBox).Checked = True
                                            End If
                                        End If
                                    End If

                                End If
                            End If
                            'show ML_ML for football,Basketball,Soccer end
                            If (GameType.Equals("Baseball", StringComparison.CurrentCultureIgnoreCase)) Or (GameType.Equals("Hockey", StringComparison.CurrentCultureIgnoreCase)) Then
                                If i >= 3 Then
                                    If (i = 3) Then 'money line
                                        CType(e.Item.Cells(j + 1).Controls(1), CheckBox).Visible = True
                                        CType(e.Item.Cells(j + 1).Controls(1), CheckBox).Checked = False
                                        CType(e.Item.Cells(j + 1).Controls(3), HiddenField).Value = "ML_ML"
                                        Dim drParlay As DataRow = oSysManager.GetValue(ParlayRules, GameType, "ML_ML")
                                        If drParlay IsNot Nothing Then
                                            If SafeString(drParlay("Value")).Equals(Yes) Then
                                                CType(e.Item.Cells(j + 1).Controls(1), CheckBox).Checked = True
                                            End If
                                        End If
                                    ElseIf i = 4 Then 'row :1hspread column :1H Spread,1htotalpoint,1hmoney line
                                        CType(e.Item.Cells(j + 3).Controls(1), CheckBox).Visible = True
                                        CType(e.Item.Cells(j + 3).Controls(1), CheckBox).Checked = False
                                        CType(e.Item.Cells(j + 3).Controls(3), HiddenField).Value = SysSettingKeySTM(k)
                                        Dim drParlay As DataRow = oSysManager.GetValue(ParlayRules, GameType, SysSettingKeySTM(k))
                                        If drParlay IsNot Nothing Then
                                            If SafeString(drParlay("Value")).Equals(Yes) Then
                                                CType(e.Item.Cells(j + 3).Controls(1), CheckBox).Checked = True
                                            End If
                                        End If
                                        k += 1
                                    ElseIf i = 5 AndAlso j >= 3 Then 'row :1H Total Points column :1htotalpoint,1hmoney line
                                        CType(e.Item.Cells(j + 2).Controls(1), CheckBox).Visible = True
                                        CType(e.Item.Cells(j + 2).Controls(1), CheckBox).Checked = False
                                        CType(e.Item.Cells(j + 2).Controls(3), HiddenField).Value = SysSettingKeyHH(h)
                                        Dim drParlay As DataRow = oSysManager.GetValue(ParlayRules, GameType, SysSettingKeyHH(h))
                                        If drParlay IsNot Nothing Then
                                            If SafeString(drParlay("Value")).Equals(Yes) Then
                                                CType(e.Item.Cells(j + 2).Controls(1), CheckBox).Checked = True
                                            End If
                                        End If
                                        h += 1
                                    ElseIf i = 6 AndAlso j >= 5 Then '1H Money Line,1H Money Line
                                        CType(e.Item.Cells(j + 1).Controls(1), CheckBox).Visible = True
                                        CType(e.Item.Cells(j + 1).Controls(1), CheckBox).Checked = False
                                        CType(e.Item.Cells(j + 1).Controls(3), HiddenField).Value = "1HML_1HML"
                                        Dim drParlay As DataRow = oSysManager.GetValue(ParlayRules, GameType, "1HML_1HML")
                                        If drParlay IsNot Nothing Then
                                            If SafeString(drParlay("Value")).Equals(Yes) Then
                                                CType(e.Item.Cells(j + 1).Controls(1), CheckBox).Checked = True
                                            End If
                                        End If
                                    ElseIf i = 7 AndAlso j >= 5 Then '' row:2H Spread column: 2hspread,2H Total Points
                                        CType(e.Item.Cells(j + 2).Controls(1), CheckBox).Visible = True
                                        CType(e.Item.Cells(j + 2).Controls(1), CheckBox).Checked = False
                                        CType(e.Item.Cells(j + 2).Controls(3), HiddenField).Value = SysSettingKey2HH(l)
                                        Dim drParlay As DataRow = oSysManager.GetValue(ParlayRules, GameType, SysSettingKey2HH(l))
                                        If drParlay IsNot Nothing Then
                                            If SafeString(drParlay("Value")).Equals(Yes) Then
                                                CType(e.Item.Cells(j + 2).Controls(1), CheckBox).Checked = True
                                            End If
                                        End If
                                        l += 1

                                    ElseIf i = 8 AndAlso j >= 6 Then '' only once 2H Total Points,2H Total Points
                                        CType(e.Item.Cells(j + 2).Controls(1), CheckBox).Visible = True
                                        CType(e.Item.Cells(j + 2).Controls(1), CheckBox).Checked = False
                                        CType(e.Item.Cells(j + 2).Controls(3), HiddenField).Value = "2HTotal_2HTotal"
                                        Dim drParlay As DataRow = oSysManager.GetValue(ParlayRules, GameType, "2HTotal_2HTotal")
                                        If drParlay IsNot Nothing Then
                                            If SafeString(drParlay("Value")).Equals(Yes) Then
                                                CType(e.Item.Cells(j + 2).Controls(1), CheckBox).Checked = True
                                            End If
                                        End If
                                        'CType(e.Item.Cells(j + 2).Controls(1), CheckBox).Style.Add("border", "red solid 1px")
                                    End If
                                End If
                            End If
                            If (i >= ncheckNotValid AndAlso j < ncheckNotValid) Then
                                CType(e.Item.Cells(j).Controls(1), CheckBox).Visible = False
                                If (GameType.Equals("Baseball", StringComparison.CurrentCultureIgnoreCase)) Or (GameType.Equals("Hockey", StringComparison.CurrentCultureIgnoreCase)) Then
                                    ''for baseball
                                    ' If (i < (ncheckNotValid + 2) AndAlso (j = 4 OrElse j = 5 OrElse j = 1 OrElse j = 2)) Then ''condition show 4 check box
                                    CType(e.Item.Cells(j).Controls(1), CheckBox).Visible = True
                                    CType(e.Item.Cells(j).Controls(1), CheckBox).Checked = False
                                    CType(e.Item.Cells(j).Controls(3), HiddenField).Value = SysSettingKey(nSysSettingKey)
                                    Dim drParlay As DataRow = oSysManager.GetValue(ParlayRules, GameType, SysSettingKey(nSysSettingKey))
                                    If drParlay IsNot Nothing Then
                                        If SafeString(drParlay("Value")).Equals(Yes) Then
                                            CType(e.Item.Cells(j).Controls(1), CheckBox).Checked = True
                                        End If

                                    End If
                                    'End If
                                Else
                                    '' for hockey
                                    If (i < (ncheckNotValid + 2) AndAlso (j = 4 OrElse j = 5 OrElse j = 1 OrElse j = 2)) Then ''condition show 4 check box
                                        CType(e.Item.Cells(j).Controls(1), CheckBox).Visible = True
                                        CType(e.Item.Cells(j).Controls(1), CheckBox).Checked = False
                                        CType(e.Item.Cells(j).Controls(3), HiddenField).Value = SysSettingKey(nSysSettingKey)
                                        Dim drParlay As DataRow = oSysManager.GetValue(ParlayRules, GameType, SysSettingKey(nSysSettingKey))
                                        If drParlay IsNot Nothing Then
                                            If SafeString(drParlay("Value")).Equals(Yes) Then
                                                CType(e.Item.Cells(j).Controls(1), CheckBox).Checked = True
                                            End If

                                        End If
                                    End If
                                End If

                            Else

                                CType(e.Item.Cells(j).Controls(1), CheckBox).Visible = True
                                CType(e.Item.Cells(j).Controls(1), CheckBox).Checked = False

                                CType(e.Item.Cells(j).Controls(3), HiddenField).Value = SysSettingKey(nSysSettingKey)
                                Dim drParlay As DataRow = oSysManager.GetValue(ParlayRules, GameType, SysSettingKey(nSysSettingKey))
                                If drParlay IsNot Nothing Then
                                    If SafeString(drParlay("Value")).Equals(Yes) Then
                                        CType(e.Item.Cells(j).Controls(1), CheckBox).Checked = True
                                    End If

                                End If

                            End If
                            '' end show checkbox for between game
                        End If
                        nSysSettingKey += 1
                    Next
                End If
                i = i + 1
                h = 0
                k = 0
                l = 0
            End If
            If nSysSettingKey >= SysSettingKey.Length Then
                nSysSettingKey = 0
            End If
            If i > nNumBetType Then
                i = 1
            End If

        End Sub

        Protected Sub SaveALLParlaySetting(ByVal sender As Object, ByVal e As System.EventArgs)
            For i As Integer = 0 To rptPRRules.Items.Count - 1
                Dim grRule As DataGrid = CType(rptPRRules.Items(i).FindControl("grPRRules"), DataGrid)
                For Each itemDataGrid As DataGridItem In grRule.Items
                    For Each tableCell As TableCell In itemDataGrid.Controls
                        For Each obj As Object In tableCell.Controls
                            If obj.GetType Is GetType(CheckBox) Then
                                If BetweenGames AndAlso (CType(obj, CheckBox).ID.Equals("chkParlay") OrElse CType(obj, CheckBox).ID.Equals("chkReverse")) Then
                                    Continue For
                                End If
                                If CType(obj, CheckBox).Visible Then
                                    SaveParlay(obj)
                                End If
                            End If
                        Next
                    Next
                Next
            Next
            rptPRRules.DataSource = ListBetType
            rptPRRules.DataBind()
            ClientAlert("Successfully Saved", True)
        End Sub

#End Region

#Region "Functions"

        Public Sub ParlayDataBind()
            rptPRRules.DataSource = ListBetType
            rptPRRules.DataBind()

        End Sub

        Public Sub BindParlayAndReverse(ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs)
            Dim sParlayCategory As String = ParlayType
            Dim sReverseCategory As String = ReverseType
            Dim chkParlay As CheckBox = CType(e.Item.FindControl("chkParlay"), CheckBox)
            Dim chkReverse As CheckBox = CType(e.Item.FindControl("chkReverse"), CheckBox)
            Dim sHfParlayId = chkParlay.ID.Replace("chk", "HF").ToString()
            Dim sParlayKey As String = CType(e.Item.FindControl(sHfParlayId), HiddenField).Value
            Dim sHfReverseId = chkReverse.ID.Replace("chk", "HF").ToString()
            Dim sSeverseKey As String = CType(e.Item.FindControl(sHfReverseId), HiddenField).Value
            Dim drParlay As DataRow = oSysManager.GetValue(sParlayCategory, GameType, sParlayKey)
            Dim drReverse As DataRow = oSysManager.GetValue(sReverseCategory, GameType, sSeverseKey)
            chkParlay.Checked = False
            chkReverse.Checked = False
            If drParlay IsNot Nothing Then
                If SafeString(drParlay("Value")).Equals(Yes) Then
                    chkParlay.Checked = True
                End If

            End If
            If drReverse IsNot Nothing Then
                If SafeString(drReverse("Value")).Equals(Yes) Then
                    chkReverse.Checked = True
                End If
            End If
        End Sub

        Public Sub SaveParlay(ByVal sender As Object)
            Dim sParlayRules As String = ""
            Dim sParlayCategory As String = AgentParlayType
            Dim sReverseCategory As String = AgentReverseType
            Dim chkClick As CheckBox = CType(sender, CheckBox)
            Dim sSubCategories As String = ""
            Dim item As DataGridItem = CType(CType(sender, CheckBox).Parent.Parent, DataGridItem)
            Dim nItemIndex As Integer = item.ItemIndex
            Dim chkParlay As CheckBox = CType(item.FindControl("chkParlay"), CheckBox)
            Dim chkReverse As CheckBox = CType(item.FindControl("chkReverse"), CheckBox)
            sSubCategories = CType(item.FindControl("HFGameType"), HiddenField).Value
            If Not chkParlay.Checked AndAlso Not chkReverse.Checked AndAlso (chkClick.ID.Equals(chkParlay.ID) OrElse chkClick.ID.Equals(chkReverse.ID)) Then
                chkClick.Checked = True
                ClientAlert("If You Don't Check On Parlay Or Reverse, Parlay Will Be Set Default")
                Return
            End If
            If chkClick.ID.Equals(chkParlay.ID) Then
                sParlayRules = sParlayCategory
            ElseIf chkClick.ID.Equals(chkReverse.ID) Then
                sParlayRules = sReverseCategory
            Else
                sParlayRules = AgentParlayRules
            End If
            Dim sHfId = chkClick.ID.Replace("chk", "HF").ToString()
            Dim sKey As String = CType(item.FindControl(sHfId), HiddenField).Value
            If chkClick.Checked Then
                If oSysManager.IsExistedKey(sParlayRules, sSubCategories, sKey) Then
                    oSysManager.UpdateValue(sParlayRules, sSubCategories, sKey, Yes)
                Else
                    oSysManager.AddSysSetting(sParlayRules, sKey, Yes, sSubCategories)
                End If
            Else
                If oSysManager.IsExistedKey(sParlayRules, sSubCategories, sKey) Then
                    oSysManager.UpdateValue(sParlayRules, sSubCategories, sKey, No)
                Else
                    oSysManager.AddSysSetting(sParlayRules, sKey, No, sSubCategories)
                End If
            End If
            UserSession.Cache.ClearSysSettings(sParlayRules, sSubCategories)
            UserSession.Cache.ClearSysSettings(sParlayRules)
            UserSession.Cache.ClearSysSettings(AgentParlayType, sSubCategories)
            UserSession.Cache.ClearSysSettings(AgentParlayType)
            UserSession.Cache.ClearSysSettings(AgentReverseType, sSubCategories)
            UserSession.Cache.ClearSysSettings(AgentReverseType)
        End Sub

#End Region

        Protected Sub ddlAgents_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlAgents.SelectedIndexChanged
            ParlayDataBind()
        End Sub

        Public Function getMaxParlayInGame(ByVal psSportType As String) As Single
            Dim lstCategory = UserSession.Cache.GetSysSettings(MAX_PARLAY_IN_GAME)
            Dim nMaxParlayInGame As Double
            If lstCategory IsNot Nothing Then
                nMaxParlayInGame = SafeSingle(lstCategory.GetValue(psSportType))
            End If
            Return nMaxParlayInGame
        End Function

    End Class
End Namespace