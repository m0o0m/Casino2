Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports SBCBL.std
Imports System.Data
Imports WebsiteLibrary
Namespace SBSSuperAdmin
    Partial Class ManualGames
        Inherits SBCBL.UI.CSBCPage

        Private _sGameType As String = SBCBL.std.GetSiteType & " GameType"

#Region "Properties"

        Public Property ViewLock() As Boolean
            Get
                If ViewState("VIEW_LOCK") Is Nothing Then
                    ViewState("VIEW_LOCK") = False
                End If

                Return CBool(ViewState("VIEW_LOCK"))
            End Get
            Set(ByVal value As Boolean)
                ViewState("VIEW_LOCK") = value

                Dim sMessage As String = SafeString(IIf(value, "Unlock", "Lock"))
                btnLock.Text = sMessage
                btnLock.ToolTip = sMessage & " Games"
            End Set
        End Property

        Public Property IsFinalGames() As Boolean
            Get
                If ViewState("FINAL_GAMES") Is Nothing Then
                    ViewState("FINAL_GAMES") = False
                End If

                Return CBool(ViewState("FINAL_GAMES"))
            End Get
            Set(ByVal value As Boolean)
                ViewState("FINAL_GAMES") = value
            End Set
        End Property

        Public ReadOnly Property IsFirstHalf() As Boolean
            Get
                Return rdGameType.SelectedValue = "FIRSTHALF"
            End Get
        End Property
#End Region

#Region "Page's Event"
        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Update Game Scores"
            SideMenuTabName = "MANUAL_GAME"
            Me.MenuTabName = "GAME MANAGEMENT"
            DisplaySubTitlle(Me.Master, "Update Game Scores")
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                bindGameType()
                ucDateFrom.Value = Now
                ucDateTo.Value = Now

                bindGames()
            End If
        End Sub

        Protected Sub btnLock_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLock.Click
            Dim oGameIDs As List(Of String) = getGameIDs()

            If oGameIDs.Count = 0 Then
                ClientAlert("Select Game(s) To " & LCase(btnLock.Text), True)
                Return
            End If

            If (New CGameManager).LockGames(oGameIDs, Not Me.ViewLock, UserSession.UserID) Then
                dgGames.CurrentPageIndex = 0
                bindGames()
            End If
        End Sub

        Protected Sub dgGames_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgGames.ItemCommand
            Select Case e.CommandName
                Case "UPDATE_GAME"
                    Dim nHomeScore As Integer = SafeInteger(CType(e.Item.FindControl("txtHomeScore"), TextBox).Text)
                    Dim nAwayScore As Integer = SafeInteger(CType(e.Item.FindControl("txtAwayScore"), TextBox).Text)

                    ''Update game
                    Dim oGamgeManager As New CGameManager()
                    Dim hfGameID As HiddenField = CType(e.Item.FindControl("hfGameID"), HiddenField)
                    Dim ddlGameStatus As CDropDownList = CType(e.Item.FindControl("ddlGameStatus"), CDropDownList)
                    oGamgeManager.UpdateGameScore(hfGameID.Value, nHomeScore, nAwayScore, UserSession.UserID, ddlGameStatus.SelectedValue, IsFirstHalf)

                    '' change hiddenfield value
                    If IsFirstHalf Then
                        Dim HFHomeFirstHalf As HiddenField = CType(e.Item.FindControl("HFHomeFirstHalf"), HiddenField)
                        Dim HFAwayFirstHalf As HiddenField = CType(e.Item.FindControl("HFAwayFirstHalf"), HiddenField)
                        HFHomeFirstHalf.Value = nHomeScore.ToString()
                        HFAwayFirstHalf.Value = nAwayScore.ToString()
                    Else
                        Dim HFHomeScore As HiddenField = CType(e.Item.FindControl("HFHomeScore"), HiddenField)
                        Dim HFAwayScore As HiddenField = CType(e.Item.FindControl("HFAwayScore"), HiddenField)
                        HFHomeScore.Value = nHomeScore.ToString()
                        HFAwayScore.Value = nAwayScore.ToString()
                    End If
                    dgGames.CurrentPageIndex = 0
                    bindGames()

            End Select
        End Sub

        Protected Sub dgGames_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgGames.ItemDataBound
            If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
                Dim ddlGameStatus As CDropDownList = CType(e.Item.FindControl("ddlGameStatus"), CDropDownList)
                If IsFirstHalf Then
                    Select Case UCase(SafeString(CType(e.Item.DataItem, DataRowView)("IsFirstHalfFinished")))
                        Case "Y"
                            ddlGameStatus.Value = "Final"
                        Case "N"
                            ddlGameStatus.Value = "CANCELLED"
                        Case Else
                            ddlGameStatus.Value = ""
                    End Select
                Else
                    If Not IsDBNull(CType(e.Item.DataItem, DataRowView)("GameStatus")) Then
                        If Not String.IsNullOrEmpty(CType(e.Item.DataItem, DataRowView)("GameStatus")) AndAlso Not (CType(e.Item.DataItem, DataRowView)("GameStatus")).Equals("CANCELLED") AndAlso Not (CType(e.Item.DataItem, DataRowView)("GameStatus")).Equals("Final") Then
                            ddlGameStatus.Items.Add(CType(e.Item.DataItem, DataRowView)("GameStatus"))
                        End If

                        ddlGameStatus.Value = CType(e.Item.DataItem, DataRowView)("GameStatus")
                    End If
                End If
            End If

        End Sub

        Protected Sub dgGames_PageIndexChanged(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles dgGames.PageIndexChanged
            dgGames.CurrentPageIndex = e.NewPageIndex
            bindGames()
        End Sub

        Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSearch.Click
            If checkConditions() Then
                dgGames.CurrentPageIndex = 0
                ViewLock = chkLock.Checked
                IsFinalGames = (rdGameType.SelectedValue = "FINAL")
                bindGames()
            End If
        End Sub

        Protected Sub btnReset_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnReset.Click
            resetField()
            bindGames()
        End Sub
#End Region

        Private Function checkConditions() As Boolean
            If ucDateFrom.Value = DateTime.MinValue AndAlso ucDateTo.Value = DateTime.MinValue Then
                ClientAlert("Please Choose Date Range", True)
                ucDateFrom.Focus()
                Return False
            End If

            If ucDateFrom.Value <> DateTime.MinValue AndAlso ucDateTo.Value <> DateTime.MinValue Then
                If ucDateFrom.Value > ucDateTo.Value Then
                    ClientAlert("Date To Must Be Greater Than Date From", True)
                    ucDateTo.Focus()
                    Return False
                End If
            End If

            Return True
        End Function

        Private Sub bindGameType()
            'Dim oGameTypes As List(Of CSysSetting) = (From oSetting As CSysSetting In UserSession.Cache.GetAllSysSettings(_sGameType) _
            '                                      Where SafeBoolean(oSetting.Value) And oSetting.SubCategory <> "" _
            '                                      Order By oSetting.Key, oSetting.SubCategory _
            '                                      Select oSetting).ToList
            Dim oGameTypes As Dictionary(Of String, String) = GetGameType()
            ddlGameType.DataSource = oGameTypes
            ddlGameType.DataTextField = "Key"
            ddlGameType.DataValueField = "Key"
            ddlGameType.DataBind()
        End Sub

        Private Sub resetField()
            ddlGameType.Value = ""
            ucDateFrom.Value = SafeDate(GetEasternDate().ToShortDateString())
            ucDateTo.Value = SafeDate(GetEasternDate().ToShortDateString())
            dgGames.CurrentPageIndex = 0
            ViewLock = False
            IsFinalGames = False
            rdGameType.SelectedValue = "GAME"
            chkLock.Checked = False
        End Sub

        Private Sub bindGames()
            Dim oGameManager As New CGameManager()
            Dim oTbl As DataTable = oGameManager.GetGames(ddlGameType.Value, SafeDate(ucDateFrom.Value.ToShortDateString()), SafeDate(ucDateTo.Value.ToShortDateString()), ViewLock, True, IsFinalGames, Not IsFirstHalf)

            If oTbl IsNot Nothing Then
                dgGames.DataSource = oTbl
                dgGames.Visible = True
                dgGames.DataBind()
            Else
                dgGames.Visible = False
            End If
        End Sub

        Private Function getGameIDs() As List(Of String)
            Dim oGameIDs As New List(Of String)

            For Each oItem As DataGridItem In dgGames.Items
                If (CType(oItem.FindControl("chkID"), CheckBox).Checked) Then
                    oGameIDs.Add(SafeString(CType(oItem.FindControl("hfGameID"), HiddenField).Value))
                End If
            Next

            Return oGameIDs
        End Function

        'Protected Sub chkFinal_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkFinal.CheckedChanged, chkFirstHalf.CheckedChanged
        '    btnSearch_Click(sender, e)
        'End Sub

        Protected Sub chkFirstHalfItem_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim chkFirstHalfItem As CheckBox = CType(sender, CheckBox)
            Dim txtHomeScore As TextBox = CType(chkFirstHalfItem.Parent.FindControl("txtHomeScore"), TextBox)
            Dim txtAwayScore As TextBox = CType(chkFirstHalfItem.Parent.FindControl("txtAwayScore"), TextBox)

            If chkFirstHalfItem.Checked Then
                Dim HFHomeFirstHalf As HiddenField = CType(chkFirstHalfItem.Parent.FindControl("HFHomeFirstHalf"), HiddenField)
                Dim HFAwayFirstHalf As HiddenField = CType(chkFirstHalfItem.Parent.FindControl("HFAwayFirstHalf"), HiddenField)
                txtHomeScore.Text = HFHomeFirstHalf.Value
                txtAwayScore.Text = HFAwayFirstHalf.Value
            Else
                Dim HFHomeScore As HiddenField = CType(chkFirstHalfItem.Parent.FindControl("HFHomeScore"), HiddenField)
                Dim HFAwayScore As HiddenField = CType(chkFirstHalfItem.Parent.FindControl("HFAwayScore"), HiddenField)
                txtHomeScore.Text = HFHomeScore.Value
                txtAwayScore.Text = HFAwayScore.Value
            End If
        End Sub

        Protected Sub chkLock_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkLock.CheckedChanged
            btnSearch_Click(sender, e)
        End Sub

        Protected Sub rdGameType_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim rdGameType As RadioButton = CType(sender, RadioButton)
            Dim txtHomeScore As TextBox = CType(rdGameType.Parent.FindControl("txtHomeScore"), TextBox)
            Dim txtAwayScore As TextBox = CType(rdGameType.Parent.FindControl("txtAwayScore"), TextBox)

            If rdGameType.Text = "Final" Then
                Dim HFHomeScore As HiddenField = CType(rdGameType.Parent.FindControl("HFHomeScore"), HiddenField)
                Dim HFAwayScore As HiddenField = CType(rdGameType.Parent.FindControl("HFAwayScore"), HiddenField)
                txtHomeScore.Text = HFHomeScore.Value
                txtAwayScore.Text = HFAwayScore.Value
            Else
                Dim HFHomeFirstHalf As HiddenField = CType(rdGameType.Parent.FindControl("HFHomeFirstHalf"), HiddenField)
                Dim HFAwayFirstHalf As HiddenField = CType(rdGameType.Parent.FindControl("HFAwayFirstHalf"), HiddenField)
                txtHomeScore.Text = HFHomeFirstHalf.Value
                txtAwayScore.Text = HFAwayFirstHalf.Value
            End If
        End Sub

        Protected Sub rdGameType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rdGameType.SelectedIndexChanged
            btnSearch_Click(sender, e)
        End Sub
    End Class

End Namespace