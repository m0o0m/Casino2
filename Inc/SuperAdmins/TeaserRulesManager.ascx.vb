Imports SBCBL.std
Imports SBCBL.Managers
Imports System.Data

Namespace SBCSuperAdmin
    Partial Class TeaserRulesManager
        Inherits SBCBL.UI.CSBCUserControl
        Public Event ButtonClick(ByVal sButtonType As String)

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
                btnLock.ToolTip = sMessage & " TeaserRules"

                btnViewLock.Text = "View " & sMessage
                btnViewLock.ToolTip = "View " & sMessage & " TeaserRules"
            End Set
        End Property

#End Region

#Region "Page events"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                bindTeaserRuels()
            End If
        End Sub

#End Region

        Private CountTicketsByTeaserRule As DataTable = Nothing

        Private Sub bindTeaserRuels()
            Dim odtRules As DataTable = (New CTeaserRuleManager).GetTeaserRules(Me.ViewLock, SBCBL.std.GetSiteType)

            If odtRules IsNot Nothing Then
                Dim oSqlTeaserRuleIDs As List(Of String) = _
                    (From oRow In odtRules.AsEnumerable _
                     Let sTeaserRuleID = SafeString(oRow.Field(Of Guid)("TeaserRuleID")) _
                     Where sTeaserRuleID <> "" _
                     Select SQLString(sTeaserRuleID)).Distinct.ToList()

                Me.CountTicketsByTeaserRule = (New CTicketManager).CountTicketsByTeaserRule(oSqlTeaserRuleIDs)
            End If

            dgTeaserRules.DataSource = odtRules
            dgTeaserRules.DataBind()

            btnChangedIndex.Visible = dgTeaserRules.Items.Count > 0
        End Sub

        Protected Sub dgTeaserRules_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgTeaserRules.ItemCommand
            Dim sTeaserRuleID As String = SafeString(e.CommandArgument)

            Select Case UCase(e.CommandName)
                Case "EDITTEASERRULE"
                    If Not ucTeaserRuleEdit.LoadTeaserRuleInfo(sTeaserRuleID) Then
                        ClientAlert("Can't Load Rule", True)
                        Return
                    End If

                    dgTeaserRules.SelectedIndex = e.Item.ItemIndex

                Case "DELETETEASERRULE"
                    If (New CTeaserRuleManager).DeleteTeaserRule(sTeaserRuleID, UserSession.UserID) Then
                        dgTeaserRules.SelectedIndex = -1
                        ucTeaserRuleEdit.ClearTeaserRuleInfo()

                        Dim oSettingMng As New CSysSettingManager
                        oSettingMng.DeleteSettingBySubCategory(SBCBL.std.GetSiteType & " TEASER ODDS", sTeaserRuleID)

                        UserSession.Cache.ClearSysSettings(SBCBL.std.GetSiteType & " TEASER ODDS", sTeaserRuleID)

                        bindTeaserRuels()
                        RaiseEvent ButtonClick("CHANGE TEASER RULES")
                    End If
            End Select
        End Sub

        Protected Sub dgTeaserRules_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgTeaserRules.ItemDataBound
            If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
                Dim lbtDel As LinkButton = CType(e.Item.FindControl("lbtDel"), LinkButton)
                Dim sConfirm As String = "Are you sure you want to delete this rule?"

                If Me.CountTicketsByTeaserRule IsNot Nothing Then
                    Dim nCount As Int32 = SafeInteger(Me.CountTicketsByTeaserRule.Compute("SUM(CountTickets)", "TeaserRuleID=" & SQLString(lbtDel.CommandArgument)))

                    If nCount > 0 Then
                        sConfirm = "There are " & FormatNumber(nCount, 0) & " bet(s) used this rule. " & sConfirm
                    End If
                End If

                lbtDel.OnClientClick = "return confirm('" & sConfirm & "');"
            End If
        End Sub

        Protected Sub ucTeaserRulesEdit_ButtonClick(ByVal sButtonType As String) Handles ucTeaserRuleEdit.ButtonClick
            dgTeaserRules.SelectedIndex = -1
            ucTeaserRuleEdit.ClearTeaserRuleInfo()

            Select Case UCase(sButtonType)
                Case "SAVE UNSUCCESSFUL"
                    ClientAlert("Failed to Save Setting", True)

                Case "SAVE SUCCESSFUL"
                    bindTeaserRuels()
                    RaiseEvent ButtonClick("CHANGE TEASER RULES")
            End Select
        End Sub

        Protected Sub btnViewLock_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnViewLock.Click
            Me.ViewLock = Not Me.ViewLock

            dgTeaserRules.SelectedIndex = -1
            bindTeaserRuels()
        End Sub

        Private Function getSqlTeaserRuleIDs() As List(Of String)
            Dim oSqlTeaserRuleIDs As New List(Of String)

            For Each oItem As DataGridItem In dgTeaserRules.Items
                If (CType(oItem.FindControl("chkID"), CheckBox).Checked) Then
                    oSqlTeaserRuleIDs.Add(SafeString(CType(oItem.FindControl("lbtEdit"), LinkButton).CommandArgument))
                End If
            Next

            Return oSqlTeaserRuleIDs
        End Function

        Protected Sub btnLock_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLock.Click
            Dim oSqlTeaserRuleIDs As List(Of String) = getSqlTeaserRuleIDs()

            If oSqlTeaserRuleIDs.Count = 0 Then
                ClientAlert("Select Teaser Rules to " & LCase(btnLock.Text), True)
                Return
            End If

            If (New CTeaserRuleManager).LockTeaserRules(oSqlTeaserRuleIDs, Not Me.ViewLock, UserSession.UserID) Then
                dgTeaserRules.SelectedIndex = -1
                bindTeaserRuels()

                RaiseEvent ButtonClick("CHANGE TEASER RULES")
            End If
        End Sub

        Protected Sub btnChangedIndex_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnChangedIndex.Click
            Dim oIndexes As New Hashtable

            For Each oItem As DataGridItem In dgTeaserRules.Items
                Dim sTeaserRuleID As String = SafeString(CType(oItem.FindControl("lbtEdit"), LinkButton).CommandArgument)
                Dim nTeaserRuleIndex As Integer = SafeInteger(CType(oItem.FindControl("txtIndex"), TextBox).Text)

                oIndexes.Add(New Guid(SafeString(sTeaserRuleID)), nTeaserRuleIndex)
            Next

            Dim oMng As New CTeaserRuleManager
            oMng.ChangeTeaserRuleIndexes(oIndexes)

            dgTeaserRules.SelectedIndex = -1
            ucTeaserRuleEdit.ClearTeaserRuleInfo()

            bindTeaserRuels()
            RaiseEvent ButtonClick("CHANGE TEASER RULES")
        End Sub



    End Class

End Namespace