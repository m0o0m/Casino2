Imports System.Data

Imports SBCBL.std
Imports SBCBL.CacheUtils
Imports SBCBL.Managers

Namespace SBSSuperAdmin

    Partial Class PlayerTemplates
        Inherits SBCBL.UI.CSBCPage

        Dim _log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

#Region "Page events"

        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Me.PageTitle = "Templates Settings"
            Me.SideMenuTabName = "PLAYER_TEMPLATES"
            Me.MenuTabName = "SYSTEM MANAGEMENT"
            DisplaySubTitlle(Me.Master, "Templates Settings")
            Me.SideMenuTabName = "PLAYER_TEMPLATES"
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not Me.IsPostBack Then
                ucEditTemplate.ResetPlayerTemplate()
                ucEditLimits.LoadPlayerSetting(SafeString(Guid.Empty))
                bindTemplates()
                setTabActive("Template")
            End If
        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            btnCancel.Visible = hfCopyFromTemplateID.Value <> ""
            btnCopy.Visible = hfTemplateID.Value <> ""
            If hfTemplateID.Value <> "" Then
                btnSave.Text = "Save"
                btnSave.ToolTip = "Save changes"
            Else
                btnSave.Text = "Create new"
                btnSave.ToolTip = "Create new this template"
            End If
        End Sub

#End Region

#Region "Tabs"

        Private Sub setTabActive(ByVal psTabKey As String)
            tTempl.Attributes("class") = ""
            tTemplLimits.Attributes("class") = ""
            ucEditTemplate.Visible = False
            ucEditLimits.Visible = False
            Select Case UCase(psTabKey)
                Case "TEMPLATE"
                    tTempl.Attributes("class") = "active"
                    ucEditTemplate.Visible = True

                Case "LIMITS"
                    tTemplLimits.Attributes("class") = "active"
                    ucEditLimits.Visible = True
            End Select
        End Sub

        Protected Sub lbtTab_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            setTabActive(CType(sender, LinkButton).CommandArgument)
        End Sub

#End Region

        Private Sub bindTemplates()
            grdTemplates.DataSource = (New CPlayerTemplateManager).GetPlayerTemplates(SBCBL.std.GetSiteType)
            grdTemplates.DataBind()

            grdTemplates.Visible = grdTemplates.Items.Count > 0
            lblAlert.Visible = Not grdTemplates.Visible

        End Sub

        Private Sub resetTemplate()
            setTabActive("Template")

            hfTemplateID.Value = ""
            grdTemplates.SelectedIndex = -1

            ucEditTemplate.ResetPlayerTemplate()
            ucEditTemplate.SetTemplateNameFocus()
            ucEditLimits.LoadPlayerSetting(SafeString(Guid.Empty))
        End Sub

        Private Function checkTemplate(ByVal poTemplate As CPlayerTemplate) As Boolean
            If poTemplate.TemplateName = "" Then
                ucEditTemplate.SetTemplateNameFocus()
                setTabActive("Template")

                ClientAlert("Template Name Is Required", True)
                Return False
            End If

            If (New CPlayerTemplateManager).IsExistedTemplateName(poTemplate.TemplateName, SBCBL.std.GetSiteType, hfTemplateID.Value) _
                And hfTemplateID.Value = "" Then
                LogDebug(_log, "Player TemplateID:" & hfTemplateID.Value)

                ClientAlert("Template Name '" & poTemplate.TemplateName & "' Has Already Existed", True)
                ucEditTemplate.SetTemplateNameFocus()
                Return False
            End If

            Return True
        End Function

        Private Function saveTemplate(ByVal poTemplate As CPlayerTemplate) As Boolean
            Dim oTemplateMng As New CPlayerTemplateManager
            ''set player template id
            If hfTemplateID.Value <> "" Then
                poTemplate.PlayerTemplateID = hfTemplateID.Value
            End If
            ''save template
            Dim bSuccess As Boolean = False

            If hfTemplateID.Value <> "" Then
                bSuccess = oTemplateMng.UpdatePlayerTemplate(poTemplate)
            Else
                bSuccess = oTemplateMng.InsertPlayerTemplate(poTemplate, SBCBL.std.GetSiteType)
            End If

            If Not bSuccess Then
                ClientAlert("Failed to Save Setting", True)
                Return False
            End If
            ''save player template limits
            If hfTemplateID.Value <> "" Then
                bSuccess = ucEditLimits.Save
            Else
                bSuccess = ucEditLimits.SaveCopy(poTemplate.PlayerTemplateID)
            End If

            If Not bSuccess Then
                ClientAlert("Failed to Save Setting", True)
                Return False
            End If

            ClientAlert("Successfully Saved", True)
            Return True
        End Function

        Protected Sub grdTemplates_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles grdTemplates.ItemCommand
            Select Case UCase(e.CommandName)
                Case "EDIT_TEMPLATE"
                    Dim sPlayerTemplateID As String = SafeString(e.CommandArgument)
                    LogDebug(_log, "PlayerTemplateID before: " & sPlayerTemplateID)
                    Dim oTemplate As CPlayerTemplate = (New CPlayerTemplateManager).GetPlayerTemplate(sPlayerTemplateID)

                    If oTemplate Is Nothing Then
                        ClientAlert("Can't Load Template", True)
                        Return
                    End If

                    hfTemplateID.Value = sPlayerTemplateID
                    hfCopyFromTemplateID.Value = sPlayerTemplateID
                    grdTemplates.SelectedIndex = e.Item.ItemIndex

                    ucEditTemplate.LoadPlayerTemplate(oTemplate)
                    ucEditLimits.LoadPlayerSetting(sPlayerTemplateID)
                Case "COPY_TEMPLATE"
                    If grdTemplates.SelectedIndex >= 0 Then
                        hfCopyFromTemplateID.Value = SafeString(e.CommandArgument)
                        btnCopy_Click(Nothing, Nothing)
                    Else
                        Dim sPlayerTemplateID As String = SafeString(e.CommandArgument)
                        Dim oTemplate As CPlayerTemplate = (New CPlayerTemplateManager).GetPlayerTemplate(sPlayerTemplateID)

                        If oTemplate Is Nothing Then
                            ClientAlert("Can't Load Template", True)
                            Return
                        End If

                        ''reset template value
                        oTemplate.PlayerTemplateID = newGUID()
                        oTemplate.TemplateName = "Copy of " & oTemplate.TemplateName

                        ucEditTemplate.LoadPlayerTemplate(oTemplate)
                        ucEditLimits.LoadPlayerSetting(sPlayerTemplateID)
                    End If
            End Select
        End Sub

        Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
            Dim oTemplate As CPlayerTemplate = ucEditTemplate.GetPlayerTemplate()

            If Not checkTemplate(oTemplate) Then
                Return
            End If

            If saveTemplate(oTemplate) Then
                '' save limit template with default values
                SetDefaultValuesForLimitSetting(oTemplate)
                ucEditLimits.Save()
                resetTemplate()
                bindTemplates()
            End If
        End Sub

        Public Function SetDefaultValuesForLimitSetting(ByVal poPlayerTemplate As CPlayerTemplate) As Boolean
            Dim oDefaultValues As New Dictionary(Of String, String)
            With poPlayerTemplate
                oDefaultValues.Add("Max Single", .MaxSingle)
                oDefaultValues.Add("Max Parlay", .CreditMaxParlay)
                oDefaultValues.Add("Max Reverse", .CreditMaxReverseActionParlay)
                oDefaultValues.Add("Max Teaser", .CreditMaxTeaserParlay)
                oDefaultValues.Add("1H", .Max1H)
                oDefaultValues.Add("2H", .Max2H)
                oDefaultValues.Add("1Q", .Max1Q)
                oDefaultValues.Add("2Q", .Max2Q)
                oDefaultValues.Add("3Q", .Max3Q)
                oDefaultValues.Add("4Q", .Max4Q)
            End With
            ucEditLimits.LoadDefaultLimitValues(oDefaultValues)
        End Function

        Protected Sub btnCopy_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCopy.Click
            Dim oTemplate As CPlayerTemplate = ucEditTemplate.GetPlayerTemplate()
            resetTemplate()
            ''reset values
            oTemplate.PlayerTemplateID = newGUID()
            oTemplate.TemplateName = "Copy of " & oTemplate.TemplateName
            ''load data
            ucEditTemplate.LoadPlayerTemplate(oTemplate)
            ucEditLimits.LoadPlayerSetting(SafeString(hfCopyFromTemplateID.Value))
        End Sub

        Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
            resetTemplate()
        End Sub

        Protected Sub SaveSetting(ByVal sender As Object, ByVal e As System.EventArgs) Handles ucEditLimits.SavePlayerTemplate
            If ucEditLimits.Save() Then
                ClientAlert("Sucessfully Saved", True)
            Else
                ClientAlert("Failed to Save Setting", True)
            End If
        End Sub

    End Class

End Namespace

