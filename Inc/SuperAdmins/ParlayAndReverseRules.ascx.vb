Imports SBCBL.Managers
Imports SBCBL.std
Imports SBCBL.CacheUtils
Imports System.Data
Imports System.Collections.Generic

Namespace SBCSuperAdmin
    Partial Class ParlayAndReverseRules
        Inherits SBCBL.UI.CSBCUserControl

        Dim oBetType As New System.Collections.Generic.Dictionary(Of String, String)()
        Private nNumBetType As Integer = 9
        Private nSysSettingKey As Integer = 0
        Private ncheckNotValid As Integer = 7
        Private nSecondHalfColumnShow As Integer = 3
        Private nSecondHalfRowShow As Integer = 8
        Private i As Integer = 1
        Private Yes As String = "Y"
        Private No As String = "N"

        Dim oCache As New CCacheManager()
        Dim oSysManager As New CSysSettingManager()

        Private ReadOnly Property ParlayType() As String
            Get
                Dim sParlayType As String = " ParlayType"
                If BetweenGames Then
                    sParlayType = " BWParlayType"
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

                Return SBCBL.std.GetSiteType & sReverseType
            End Get
        End Property

        Private ReadOnly Property ParlayRules() As String
            Get
                Dim sParlayRules As String = " ParlayRules"
                If BetweenGames Then
                    sParlayRules = " BWParlayRules"
                End If

                Return SBCBL.std.GetSiteType & sParlayRules
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

        Protected ReadOnly Property SysSettingKey() As String()
            Get
                Dim arrKey As String() = New String(35) {"Spread_Total", "Spread_ML", "Total_ML", "Spread_1HSpread", "Total_1HSpread", "ML_1HSpread", "Spread_1HTotal", "Total_1HTotal", "ML_1HTotal", "1HSpread_1HTotal", "Spread_1HML", "Total_1HML", "ML_1HML", "1HSpread_1HML", "1HTotal_1HML", "Spread_2HSpread", "Total_2HSpread", "ML_2HSpread", "1HSpread_2HSpread", "1HTotal_2HSpread", "1HML_2HSpread", "Spread_2HTotal", "Total_2HTotal", "ML_2HTotal", "1HSpread_2HTotal", "1HTotal_2HTotal", "1HML_2HTotal", "2HSpread_2HTotal", "Spread_2HML", "Total_2HML", "ML_2HML", "1HSpread_2HML", "1HTotal_2HML", "1HML_2HML", "2HSpread_2HML", "2HTotal_2HML"}
                Return arrKey
            End Get

        End Property

        Protected ReadOnly Property ListBetType() As List(Of String)
            Get
                Dim lstBetType As New List(Of String)
                lstBetType.Add("Football")
                lstBetType.Add("Basketball")
                lstBetType.Add("Soccer")

                If Not BetweenGames Then
                    lstBetType.Add("Baseball")
                    lstBetType.Add("Hockey")
                End If

                Return lstBetType
            End Get
        End Property


        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                ParlayDataBind()
            End If
        End Sub

        Public Sub ParlayDataBind()
            rptPRRules.DataSource = ListBetType
            rptPRRules.DataBind()

        End Sub

        Protected Sub rptPRRules_ItemCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptPRRules.ItemDataBound
            Dim grPRRules As DataGrid = CType(e.Item.FindControl("grPRRules"), DataGrid)
            Select Case CType(e.Item.DataItem, String)
                Case "Football"
                    BetType.Item("Spread") = "Spread"
                    grPRRules.DataSource = BetType
                    GameType = "Football"
                Case "Basketball"
                    BetType.Item("Spread") = "Spread"
                    grPRRules.DataSource = BetType
                    GameType = "Basketball"
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

                            If (i >= ncheckNotValid AndAlso j < ncheckNotValid) Then
                                CType(e.Item.Cells(j).Controls(1), CheckBox).Visible = False
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
            End If
            If nSysSettingKey >= SysSettingKey.Length Then
                nSysSettingKey = 0
            End If
            If i > nNumBetType Then
                i = 1
            End If

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

        End Sub

        Public Sub SaveParlay(ByVal sender As Object)
            Dim sParlayRules As String = ""
            Dim sParlayCategory As String = ParlayType
            Dim sReverseCategory As String = ReverseType
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
                sParlayRules = ParlayRules
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
        End Sub


    End Class
End Namespace