Imports System.Data
Imports System.Xml

Imports SBCBL.std
Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports WebsiteLibrary.CXMLUtils
Imports FileDB

Namespace SBCSuperAdmins

    Partial Class TeaserOddsManager
        Inherits SBCBL.UI.CSBCUserControl

        Public Sub LoadTeaserOdds()
            bindTeaserOdds()
        End Sub

        Private ReadOnly Property Category() As String
            Get
                Return SBCBL.std.GetSiteType & " TEASER ODDS"
            End Get
        End Property

        Private TeaserRules As DataTable
        Private SysSettings As List(Of CSysSetting) = Nothing

        Private Sub bindTeaserOdds()
            ''Get Sys Settings
            Me.SysSettings = (New CSysSettingManager).GetAllSysSettings(Me.Category)

            ''Teaser Rules
            Me.TeaserRules = New CTeaserRuleManager().GetTeaserRules(SBCBL.std.GetSiteType)

            dltRuleTitles.DataSource = Me.TeaserRules
            dltRuleTitles.DataBind()

            ''Teams
            rptTeams.DataSource = From sTeam As Char In "23456789" Select sTeam
            rptTeams.DataBind()
        End Sub

        Protected Sub dltRuleTitles_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataListItemEventArgs) Handles dltRuleTitles.ItemDataBound
            If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
                Dim oRule As DataRowView = CType(e.Item.DataItem, DataRowView)

                Dim lblRuleName As Label = CType(e.Item.FindControl("lblRuleName"), Label)
                lblRuleName.Text = SafeString(oRule("TeaserRuleName"))
                lblRuleName.Text += "<br />" & SafeString(IIf(UCase(SafeString(oRule("IsTiesLose"))) = "Y", "(Tie=Lose)", "(Tie=Push)"))
                If UCase(SafeString(oRule("IsLocked"))) = "Y" Then
                    lblRuleName.Text += "<b>*</b>"
                End If
            End If
        End Sub

        Protected Sub rptTeams_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptTeams.ItemDataBound
            If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
                Dim dltRules As DataList = CType(e.Item.FindControl("dltRules"), DataList)

                dltRules.DataSource = Me.TeaserRules
                dltRules.DataBind()
            End If
        End Sub

        Protected Sub dltRules_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataListItemEventArgs)
            If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
                Dim nTeams As Integer = SafeInteger(CType(e.Item.Parent.Parent, RepeaterItem).DataItem)
                Dim oTeaserRule As DataRowView = CType(e.Item.DataItem, DataRowView)

                Dim txtOdds As TextBox = CType(e.Item.FindControl("txtOdds"), TextBox)

                If nTeams < SafeInteger(oTeaserRule("MinTeam")) OrElse nTeams > SafeInteger(oTeaserRule("MaxTeam")) Then
                    txtOdds.Enabled = False
                    txtOdds.Style("background-color") = "#EEEEEE"
                End If

                If Me.SysSettings IsNot Nothing Then
                    Dim sRuleID As String = UCase(SafeString(CType(e.Item.FindControl("hfRuleID"), HiddenField).Value))
                    Dim oSysSetting As CSysSetting = Me.SysSettings.Find(Function(x) UCase(x.SubCategory) = sRuleID AndAlso SafeInteger(x.Key) = nTeams)

                    If oSysSetting IsNot Nothing Then
                        CType(e.Item.FindControl("hfSysSettingID"), HiddenField).Value = oSysSetting.SysSettingID
                        txtOdds.Text = oSysSetting.Value

                        Dim ibtClearOdds As ImageButton = CType(e.Item.FindControl("ibtClearOdds"), ImageButton)
                        ibtClearOdds.Visible = (Not txtOdds.Enabled)
                        ibtClearOdds.CommandArgument = oSysSetting.SysSettingID
                    End If
                End If
            End If
        End Sub

        Protected Sub dltRules_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataListCommandEventArgs)
            Select Case e.CommandName
                Case "CLEAR_ODDS"
                    If New CSysSettingManager().DeleteSetting(SafeString(e.CommandArgument)) Then
                        Dim sTeaserRuleID As String = SafeString(CType(e.Item.FindControl("hfRuleID"), HiddenField).Value)
                        UserSession.Cache.ClearSysSettings(Me.Category, sTeaserRuleID)

                        CType(e.Item.FindControl("txtOdds"), TextBox).Text = ""
                        CType(e.Item.FindControl("ibtClearOdds"), ImageButton).Visible = False
                    End If
            End Select
        End Sub

        Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
            Me.SysSettings = New List(Of CSysSetting)

            For Each oriTeam As RepeaterItem In rptTeams.Items
                Dim nTeam As Integer = SafeInteger(CType(oriTeam.FindControl("lblTeam"), Label).Text)
                Dim dltRules As DataList = CType(oriTeam.FindControl("dltRules"), DataList)

                For Each odliRule As DataListItem In dltRules.Items
                    Dim sOdds As String = SafeString(CType(odliRule.FindControl("txtOdds"), TextBox).Text)

                    If sOdds <> "" Then
                        Dim oSysSetting As New CSysSetting()
                        Me.SysSettings.Add(oSysSetting)

                        Dim sSysSettingID As String = SafeString(CType(odliRule.FindControl("hfSysSettingID"), HiddenField).Value)
                        If sSysSettingID <> "" Then oSysSetting.SysSettingID = sSysSettingID

                        oSysSetting.Category = Me.Category
                        oSysSetting.SubCategory = SafeString(CType(odliRule.FindControl("hfRuleID"), HiddenField).Value)
                        oSysSetting.Key = nTeam.ToString
                        oSysSetting.Value = sOdds
                    End If
                Next
            Next

            Try
                Dim oMng As New CSysSettingManager()
                oMng.UpdateSysSettings(Me.Category, Me.SysSettings)

                For Each oSetting As CSysSetting In Me.SysSettings
                    UserSession.Cache.ClearSysSettings(Me.Category, oSetting.SubCategory)
                Next

            Catch ex As Exception
                ClientAlert("Failed to Save Setting", True)
            Finally
                Me.SysSettings = Nothing
            End Try
        End Sub

    End Class

End Namespace


