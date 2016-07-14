Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports SBCBL.std
Imports System.Data

Namespace SBSSuperAdmin
    Partial Class ManualQuaters
        Inherits SBCBL.UI.CSBCPage

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
#End Region

#Region "Page's Event"
        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Update Quarter Scores"
            SideMenuTabName = "MANUAL_QUATERS"
            Me.MenuTabName = "GAME MANAGEMENT"
            DisplaySubTitlle(Me.Master, "Update Quarter Scores")
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                ucDateFrom.Value = Now
                ucDateTo.Value = Now

                bindGames()
            End If
        End Sub

        'Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        '    btnUpdate.Visible = Not Me.IsFinalGames
        'End Sub

        'Protected Sub btnUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        '    Dim oGamgeManager As New CGameManager()
        '    For Each oItem As DataGridItem In dgGames.Items
        '        If oItem.ItemType = ListItemType.Item OrElse oItem.ItemType = ListItemType.AlternatingItem Then
        '            Dim hfGameID As HiddenField = CType(oItem.FindControl("hfGameID"), HiddenField)
        '            Dim txtHomeScore As TextBox = CType(oItem.FindControl("txtHomeScore"), TextBox)
        '            Dim txtAwayScore As TextBox = CType(oItem.FindControl("txtAwayScore"), TextBox)
        '            'Dim chkMarkFinal As CheckBox = CType(oItem.FindControl("chkMarkFinal"), CheckBox)
        '            'Dim chkFirstHalfItem As CheckBox = CType(oItem.FindControl("chkFirstHalfItem"), CheckBox)
        '            Dim rdFinal As RadioButton = CType(oItem.FindControl("rdFinal"), RadioButton)
        '            'Dim bFirstHalfUpdate As Boolean = chkFirstHalfItem.Checked
        '            Dim sStatus As String = ""
        '            'If chkMarkFinal IsNot Nothing Then
        '            '    sStatus = SafeString(IIf(chkMarkFinal.Checked, "Final", ""))
        '            'End If

        '            If txtHomeScore.Text = "" And txtAwayScore.Text = "" Then
        '                Continue For
        '            End If

        '            If rdFinal.Checked Then
        '                oGamgeManager.UpdateGameScore(hfGameID.Value, SafeInteger(txtHomeScore.Text), SafeInteger(txtAwayScore.Text), UserSession.UserID, True, False)
        '            Else
        '                oGamgeManager.UpdateGameScore(hfGameID.Value, SafeInteger(txtHomeScore.Text), SafeInteger(txtAwayScore.Text), UserSession.UserID, False, True)
        '            End If
        '        End If
        '    Next

        '    dgGames.CurrentPageIndex = 0
        '    bindGames()
        'End Sub

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
                Case "CHANGE_SCORE_GAME_FINAL"
                    Dim nHomeScore1Q As Integer = SafeInteger(CType(e.Item.FindControl("txtHomeScore1Q"), TextBox).Text)
                    Dim nHomeScore2Q As Integer = SafeInteger(CType(e.Item.FindControl("txtHomeScore2Q"), TextBox).Text)
                    Dim nHomeScore3Q As Integer = SafeInteger(CType(e.Item.FindControl("txtHomeScore3Q"), TextBox).Text)
                    Dim nHomeScore4Q As Integer = SafeInteger(CType(e.Item.FindControl("txtHomeScore4Q"), TextBox).Text)
                    Dim nAwayScore1Q As Integer = SafeInteger(CType(e.Item.FindControl("txtAwayScore1Q"), TextBox).Text)
                    Dim nAwayScore2Q As Integer = SafeInteger(CType(e.Item.FindControl("txtAwayScore2Q"), TextBox).Text)
                    Dim nAwayScore3Q As Integer = SafeInteger(CType(e.Item.FindControl("txtAwayScore3Q"), TextBox).Text)
                    Dim nAwayScore4Q As Integer = SafeInteger(CType(e.Item.FindControl("txtAwayScore4Q"), TextBox).Text)

                    If Not (New CGameManager).ChangeScoreForGameQuaters(SafeString(e.CommandArgument), UserSession.UserID, nHomeScore1Q, nHomeScore2Q, nHomeScore3Q, _
                                                                        nHomeScore4Q, nAwayScore1Q, nAwayScore2Q, nAwayScore3Q, nAwayScore4Q) Then
                        ClientAlert("Can't Change Score", True)
                    End If
            End Select
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


        Private Sub resetField()
            ddlGameType.SelectedValue = ""
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
            Dim bPlayed As Boolean = True
            Dim oTbl As DataTable = oGameManager.GetGames(ddlGameType.SelectedValue, SafeDate(ucDateFrom.Value.ToShortDateString()), _
                                                        SafeDate(ucDateTo.Value.ToShortDateString()), ViewLock, bPlayed, IsFinalGames, True)

            If oTbl IsNot Nothing Then
                '' just only NBA Basketball and NFL Football have quaters
                oTbl.DefaultView.RowFilter = "GameType in ('NBA Basketball', 'NFL Football', 'NCAA Football')"
                oTbl = oTbl.DefaultView.ToTable()
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