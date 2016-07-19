Imports System.Data
Imports SBCBL.std
Imports System.IO
Imports SBCBL.Managers

Namespace SBSSuperAdmin
    Partial Class WhiteLabelSettings
        Inherits SBCBL.UI.CSBCPage
        Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType)

        Private Property CurrentWhiteLabelSettingID() As String
            Get
                Return SafeString(ViewState("CurrentWhiteLabelSettingID"))
            End Get
            Set(ByVal value As String)
                ViewState("CurrentWhiteLabelSettingID") = value
            End Set
        End Property

        Public Sub BindWhteLabels()
            Dim oDT As DataTable = SBCBL.CacheUtils.CWhiteLabelSettings.LoadAll(GetSiteType)
            dtgSettings.DataSource = oDT
            dtgSettings.DataBind()

        End Sub

        Private Sub LoadWhiteLabbel(ByVal psWhitelabelID As String)
            Dim oWhiteLabel As New SBCBL.CacheUtils.CWhiteLabelSettings()
            oWhiteLabel = oWhiteLabel.LoadByID(psWhitelabelID)
            If oWhiteLabel IsNot Nothing Then
                txtCopyright.Text = oWhiteLabel.CopyrightName
                txtSiteURL.Text = oWhiteLabel.SiteURL
                imgLogo.ImageUrl = oWhiteLabel.LogoFileName
                imgFav.ImageUrl = oWhiteLabel.Favicon
                imgWelComeImage.ImageUrl = oWhiteLabel.WelComeImage
                imgBackground.ImageUrl = oWhiteLabel.BackgroundImage
                imgLoginBackground.ImageUrl = oWhiteLabel.BackgroundLoginImage
                imgLoginLogo.ImageUrl = oWhiteLabel.LoginLogo
                imgLeftLoginBackground.ImageUrl = oWhiteLabel.LeftBackgroundLoginImage
                imgRightLoginBackground.ImageUrl = oWhiteLabel.RightBackgroundLoginImage
                imgBottomLoginBackground.ImageUrl = oWhiteLabel.BottomBackgroundLoginImage
                txtSuperAgentPhone.Text = oWhiteLabel.SuperAgentPhone
                txtBackupURL.Text = oWhiteLabel.BackupURL
                If oWhiteLabel.ColorScheme <> "" Then
                    ddlColor.SelectedValue = oWhiteLabel.ColorScheme
                Else
                    ddlColor.SelectedValue = "Blue"
                End If
                If oWhiteLabel.LoginTemplate <> "" Then
                    ddlLoginTemplate.SelectedValue = oWhiteLabel.LoginTemplate
                End If
            End If
            imgLeftLoginBackground.Visible = imgLeftLoginBackground.ImageUrl <> ""
            imgRightLoginBackground.Visible = imgRightLoginBackground.ImageUrl <> ""
            imgBottomLoginBackground.Visible = imgBottomLoginBackground.ImageUrl <> ""
            imgFav.Visible = imgFav.ImageUrl <> ""
            imgLoginLogo.Visible = imgLoginLogo.ImageUrl <> ""
            imgLogo.Visible = imgLogo.ImageUrl <> ""
            imgWelComeImage.Visible = imgWelComeImage.ImageUrl <> ""
            imgBackground.Visible = imgBackground.ImageUrl <> ""
            imgLoginBackground.Visible = imgLoginBackground.ImageUrl <> ""
            btnDeleteBackground.Visible = imgBackground.ImageUrl <> ""
            btnDeleteLoginBackground.Visible = imgLoginBackground.ImageUrl <> ""
            btnDeleteLoginLogo.Visible = imgLoginLogo.ImageUrl <> ""
        End Sub

        Protected Sub dtgSettings_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dtgSettings.ItemCommand
            Select Case UCase(e.CommandName)
                Case "EDIT"
                    CurrentWhiteLabelSettingID = SafeString(e.CommandArgument)
                    LoadWhiteLabbel(CurrentWhiteLabelSettingID)
                    dtgSettings.SelectedIndex = e.Item.ItemIndex

                    btnDeleteBackground.Visible = imgBackground.ImageUrl <> ""
                    btnDeleteLoginBackground.Visible = imgLoginBackground.ImageUrl <> ""
                    btnDeleteLoginLogo.Visible = imgLoginLogo.ImageUrl <> ""
                    btnDeleteLeftLoginBackground.Visible = imgLeftLoginBackground.ImageUrl <> ""
                    btnDeleteLeftLoginBackground.Visible = imgLeftLoginBackground.ImageUrl <> ""
                    btnDeleteRightLoginBackground.Visible = imgRightLoginBackground.ImageUrl <> ""
                    btnDeleteBottomLoginBackground.Visible = imgBottomLoginBackground.ImageUrl <> ""
                Case "DELETE"
                    SBCBL.CacheUtils.CWhiteLabelSettings.Delete(SafeString(e.CommandArgument))
                    Dim sLogo As String = CType(e.Item.FindControl("HFLogo"), HiddenField).Value
                    Dim sFavicon As String = CType(e.Item.FindControl("HFFavicon"), HiddenField).Value
                    Try
                        File.Delete(Server.MapPath(sLogo))
                        File.Delete(Server.MapPath(sFavicon))
                    Catch ex As Exception
                    End Try
                    BindWhteLabels()
                    If CurrentWhiteLabelSettingID = SafeString(e.CommandArgument) Then
                        btnCancel_Click(Nothing, Nothing)
                    End If
            End Select
        End Sub
      
        Protected Sub bnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bnSave.Click
            Dim sURL As String = SafeString(txtSiteURL.Text)
            If sURL.StartsWith("http://") Then
                sURL = sURL.Substring("http://".Length)
            End If

            If sURL.StartsWith("www.") Then
                sURL = sURL.Substring("www.".Length)
            End If

            If sURL = "" Then
                ClientAlert("Please Input Site URL", True)
                Return
            End If
            If Not SBCBL.CacheUtils.CWhiteLabelSettings.CheckDuplicateURL(sURL, CurrentWhiteLabelSettingID) Then
                ClientAlert("Site URL Has Already Existed", True)
                Return
            End If

            Dim oWhiteLabel As New SBCBL.CacheUtils.CWhiteLabelSettings()
            If CurrentWhiteLabelSettingID = "" Then
                oWhiteLabel.WhiteLabelSettingID = newGUID()
            Else
                oWhiteLabel.WhiteLabelSettingID = CurrentWhiteLabelSettingID
            End If

            Dim oFIleDB As New FileDB.CFileDB()

            If FULogo.FileName <> "" Then
                Dim sLogo As String = newGUID() & "_logo" & (New System.IO.FileInfo(FULogo.FileName)).Extension
                FULogo.SaveAs(Server.MapPath("/SBSCommon/" & sLogo))
                oWhiteLabel.LogoFileName = "/SBSCommon/" & sLogo
            End If
            If FUFavicon.FileName <> "" Then
                Dim sFavicon As String = newGUID() & "_favicon" & (New System.IO.FileInfo(FUFavicon.FileName)).Extension
                FUFavicon.SaveAs(Server.MapPath("/SBSCommon/" & sFavicon))
                oWhiteLabel.Favicon = "/SBSCommon/" & sFavicon
            End If
            If FUWelComeImage.FileName <> "" Then
                Dim sWelComeImage As String = newGUID() & "_WelComeImage" & (New System.IO.FileInfo(FUWelComeImage.FileName)).Extension
                FUWelComeImage.SaveAs(Server.MapPath("/SBSCommon/" & sWelComeImage))
                oWhiteLabel.WelComeImage = "/SBSCommon/" & sWelComeImage
            End If

            If FuBackgroundImage.FileName <> "" Then
                Dim sBackgroundImage As String = newGUID() & "_BackgroundImage" & (New System.IO.FileInfo(FuBackgroundImage.FileName)).Extension
                FuBackgroundImage.SaveAs(Server.MapPath("/SBSCommon/" & sBackgroundImage))
                oWhiteLabel.BackgroundImage = "/SBSCommon/" & sBackgroundImage
            End If
            If FuBackgroundLoginImage.FileName <> "" Then
                Dim sBackgroundLoginImage As String = newGUID() & "_BackgroundLoginImage" & (New System.IO.FileInfo(FuBackgroundLoginImage.FileName)).Extension
                FuBackgroundLoginImage.SaveAs(Server.MapPath("/SBSCommon/" & sBackgroundLoginImage))
                oWhiteLabel.BackgroundLoginImage = "/SBSCommon/" & sBackgroundLoginImage
            End If

            If FuLoginLogo.FileName <> "" Then
                Dim sLoginImage As String = newGUID() & "_LoginLogo" & (New System.IO.FileInfo(FuLoginLogo.FileName)).Extension
                FuLoginLogo.SaveAs(Server.MapPath("/SBSCommon/" & sLoginImage))
                oWhiteLabel.LoginLogo = "/SBSCommon/" & sLoginImage
            End If

            If FuLeftBackgroundLoginImage.FileName <> "" Then
                Dim sLoginImage As String = newGUID() & "_LeftBackgroundLoginImage" & (New System.IO.FileInfo(FuLeftBackgroundLoginImage.FileName)).Extension
                FuLeftBackgroundLoginImage.SaveAs(Server.MapPath("/SBSCommon/" & sLoginImage))
                oWhiteLabel.LeftBackgroundLoginImage = "/SBSCommon/" & sLoginImage
            End If

            If FuRightBackgroundLoginImage.FileName <> "" Then
                Dim sLoginImage As String = newGUID() & "_RightBackgroundLoginImage" & (New System.IO.FileInfo(FuRightBackgroundLoginImage.FileName)).Extension
                FuRightBackgroundLoginImage.SaveAs(Server.MapPath("/SBSCommon/" & sLoginImage))
                oWhiteLabel.RightBackgroundLoginImage = "/SBSCommon/" & sLoginImage
            End If

            If FuBottomBackgroundLoginImage.FileName <> "" Then
                Dim sLoginImage As String = newGUID() & "_BottomBackgroundLoginImage" & (New System.IO.FileInfo(FuBottomBackgroundLoginImage.FileName)).Extension
                FuBottomBackgroundLoginImage.SaveAs(Server.MapPath("/SBSCommon/" & sLoginImage))
                oWhiteLabel.BottomBackgroundLoginImage = "/SBSCommon/" & sLoginImage
            End If
            oWhiteLabel.BackupURL = SafeString(txtBackupURL.Text)
            oWhiteLabel.SiteURL = sURL
            oWhiteLabel.CopyrightName = SafeString(txtCopyright.Text)
            oWhiteLabel.SiteType = GetSiteType()
            oWhiteLabel.ColorScheme = ddlColor.SelectedValue
            oWhiteLabel.LoginTemplate = ddlLoginTemplate.SelectedItem.Text
            oWhiteLabel.SuperAgentPhone = SafeString(txtSuperAgentPhone.Text)
            If CurrentWhiteLabelSettingID = "" Then
                oWhiteLabel.InsertNew()
            Else
                oWhiteLabel.Update()
            End If

            ClientAlert("Successfully Saved")
            btnCancel_Click(Nothing, Nothing)
            Dim sKey As String = "WHITE_LABEL_" & sURL
            Cache.Remove(sKey)
            BindWhteLabels()
            Response.Redirect(Request.Url.AbsoluteUri)
        End Sub

        Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
            txtCopyright.Text = ""
            txtSiteURL.Text = ""
            txtSuperAgentPhone.Text = ""
            CurrentWhiteLabelSettingID = ""
            imgFav.Visible = False
            imgLogo.Visible = False
            imgWelComeImage.Visible = False
            imgBackground.Visible = False
            imgLoginBackground.Visible = False
            imgLoginLogo.Visible = False
            dtgSettings.SelectedIndex = -1
            imgLeftLoginBackground.Visible = False
            imgRightLoginBackground.Visible = False
            imgBottomLoginBackground.Visible = False
        End Sub

        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "White Label Settings"
            SideMenuTabName = "WHITE_LABEL"
            Me.MenuTabName = "SYSTEM MANAGEMENT"
            DisplaySubTitlle(Me.Master, "White Label Settings")
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.Page.Form.Enctype = "multipart/form-data"

            If Not IsPostBack Then
                BindWhteLabels()
                BindLoginTemplates()
                Dim oScriptMng As ScriptManager = ScriptManager.GetCurrent(Me.Page)

                'If oScriptMng IsNot Nothing Then
                '    oScriptMng.RegisterPostBackControl(bnSave)
                'End If
            End If


        End Sub

        Protected Sub BindLoginTemplates()
            ddlLoginTemplate.DataSource = (New CWhiteLabelSettingManager()).GetLoginTemplates(Server.MapPath("/"))
            ddlLoginTemplate.DataBind()
        End Sub

        Protected Sub btnDeleteBackground_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDeleteBackground.Click
            Dim oWhiteLabel = New SBCBL.CacheUtils.CWhiteLabelSettings().LoadByID(CurrentWhiteLabelSettingID)
            oWhiteLabel.BackgroundImage = ""
            oWhiteLabel.Update(True)
            Dim sKey As String = "WHITE_LABEL_" & oWhiteLabel.SiteURL
            Cache.Remove(sKey)
            BindWhteLabels()
            Response.Redirect(Request.Url.AbsoluteUri)
        End Sub

        Protected Sub btnDeleteLoginBackground_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDeleteLoginBackground.Click
            Dim oWhiteLabel = New SBCBL.CacheUtils.CWhiteLabelSettings().LoadByID(CurrentWhiteLabelSettingID)
            oWhiteLabel.BackgroundLoginImage = ""
            oWhiteLabel.Update(True)
            Dim sKey As String = "WHITE_LABEL_" & oWhiteLabel.SiteURL
            Cache.Remove(sKey)
            BindWhteLabels()
            Response.Redirect(Request.Url.AbsoluteUri)
        End Sub

        Protected Sub btnDeleteLoginLogo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDeleteLoginLogo.Click
            Dim oWhiteLabel = New SBCBL.CacheUtils.CWhiteLabelSettings().LoadByID(CurrentWhiteLabelSettingID)
            oWhiteLabel.LoginLogo = ""
            oWhiteLabel.Update(True)
            Dim sKey As String = "WHITE_LABEL_" & oWhiteLabel.SiteURL
            Cache.Remove(sKey)
            BindWhteLabels()
            Response.Redirect(Request.Url.AbsoluteUri)
        End Sub

        Protected Sub btnDeleteLeftLoginBackground_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDeleteLeftLoginBackground.Click
            Dim oWhiteLabel = New SBCBL.CacheUtils.CWhiteLabelSettings().LoadByID(CurrentWhiteLabelSettingID)
            oWhiteLabel.LeftBackgroundLoginImage = ""
            oWhiteLabel.Update(True)
            Dim sKey As String = "WHITE_LABEL_" & oWhiteLabel.SiteURL
            Cache.Remove(sKey)
            BindWhteLabels()
            Response.Redirect(Request.Url.AbsoluteUri)
        End Sub

        Protected Sub btnDeleteRightLoginBackground_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDeleteRightLoginBackground.Click
            Dim oWhiteLabel = New SBCBL.CacheUtils.CWhiteLabelSettings().LoadByID(CurrentWhiteLabelSettingID)
            oWhiteLabel.RightBackgroundLoginImage = ""
            oWhiteLabel.Update(True)
            Dim sKey As String = "WHITE_LABEL_" & oWhiteLabel.SiteURL
            Cache.Remove(sKey)
            BindWhteLabels()
            Response.Redirect(Request.Url.AbsoluteUri)
        End Sub

        Protected Sub btnDeleteBottomLoginBackground_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDeleteBottomLoginBackground.Click
            Dim oWhiteLabel = New SBCBL.CacheUtils.CWhiteLabelSettings().LoadByID(CurrentWhiteLabelSettingID)
            oWhiteLabel.BottomBackgroundLoginImage = ""
            oWhiteLabel.Update(True)
            Dim sKey As String = "WHITE_LABEL_" & oWhiteLabel.SiteURL
            Cache.Remove(sKey)
            BindWhteLabels()
            Response.Redirect(Request.Url.AbsoluteUri)
        End Sub
    End Class

End Namespace