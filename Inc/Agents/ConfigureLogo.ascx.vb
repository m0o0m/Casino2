Imports System.IO
Imports SBCBL.std
Namespace SBSAgents

    Partial Class ConfigureLogo
        Inherits SBCBL.UI.CSBCUserControl
        Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())
        Private oWhiteLabel As New SBCBL.CacheUtils.CWhiteLabelSettings()
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.Page.Form.Enctype = "multipart/form-data"
            ShowLogo()
            ClearCache()
        End Sub

        Public Function ShowLogo() As String
            oWhiteLabel = oWhiteLabel.LoadByUrl(Request.Url.Host)
            If oWhiteLabel IsNot Nothing Then
                Dim sURLLogo = oWhiteLabel.LogoFileName
                If System.IO.File.Exists(MapPath(sURLLogo)) Then
                    Return "<img src='" & sURLLogo & "'/>"
                Else
                    Return "<h3>Upload your Logo</h3>"
                End If
            End If
            Return ""
           
        End Function

        Public Function ShowBackground() As String
            Dim sURLLogo As String = ""
            oWhiteLabel = oWhiteLabel.LoadByUrl(Request.Url.Host)
            If (Not String.IsNullOrEmpty(UserSession.AgentUserInfo.BackgroundImage) AndAlso Not UserSession.AgentUserInfo.BackgroundImage.Equals("Remove", StringComparison.CurrentCultureIgnoreCase)) Then
                sURLLogo = UserSession.AgentUserInfo.BackgroundImage
            ElseIf oWhiteLabel IsNot Nothing AndAlso Not UserSession.AgentUserInfo.BackgroundImage.Equals("Remove", StringComparison.CurrentCultureIgnoreCase) Then
                sURLLogo = oWhiteLabel.BackgroundImage
            End If
            If System.IO.File.Exists(MapPath(sURLLogo)) Then
                Return "<img width='100' height='100' src='" & sURLLogo & "'/>"
            Else
                Return "<h3>Upload Your Image Background</h3>"
            End If
            Return ""
        End Function

        Public Function ShowLogoLoginImage() As String
            oWhiteLabel = oWhiteLabel.LoadByUrl(Request.Url.Host)
            If oWhiteLabel IsNot Nothing Then
                Dim sURLLogo = oWhiteLabel.LoginLogo
                If System.IO.File.Exists(MapPath(sURLLogo)) Then
                    Return "<img width='100' height='100' src='" & sURLLogo & "'/>"
                Else
                    Return "<h3>Upload Your Logo Login Image</h3>"
                End If
            End If
            Return ""
        End Function

        Public Function ShowLeftBackgroundLoginImage() As String
            oWhiteLabel = oWhiteLabel.LoadByUrl(Request.Url.Host)
            If oWhiteLabel IsNot Nothing Then
                Dim sURLLogo = oWhiteLabel.LeftBackgroundLoginImage
                If System.IO.File.Exists(MapPath(sURLLogo)) Then
                    Return "<img width='100' height='100' src='" & sURLLogo & "'/>"
                Else
                    Return "<h3>Upload Your Left BackgroundLogin Image</h3>"
                End If
            End If
            Return ""
        End Function

        Public Function ShowRightBackgroundLoginImage() As String
            oWhiteLabel = oWhiteLabel.LoadByUrl(Request.Url.Host)
            If oWhiteLabel IsNot Nothing Then
                Dim sURLLogo = oWhiteLabel.RightBackgroundLoginImage
                If System.IO.File.Exists(MapPath(sURLLogo)) Then
                    Return "<img width='100' height='100' src='" & sURLLogo & "'/>"
                Else
                    Return "<h3>Upload Your Right BackgroundLogin Image</h3>"
                End If
            End If
            Return ""
        End Function

        Public Function ShowBottomBackgroundLoginImage() As String
            oWhiteLabel = oWhiteLabel.LoadByUrl(Request.Url.Host)
            If oWhiteLabel IsNot Nothing Then
                Dim sURLLogo = oWhiteLabel.BottomBackgroundLoginImage
                If System.IO.File.Exists(MapPath(sURLLogo)) Then
                    Return "<img width='100' height='100' src='" & sURLLogo & "'/>"
                Else
                    Return "<h3>Upload Your Bottom BackgroundLogin Image</h3>"
                End If
            End If
            Return ""
        End Function

        Private Sub ClearCache()
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1))
            Response.Cache.SetCacheability(HttpCacheability.NoCache)
            Response.Cache.SetNoStore()
        End Sub

        Public Function ShowLoginBackground() As String
            oWhiteLabel = oWhiteLabel.LoadByUrl(Request.Url.Host)
            If oWhiteLabel IsNot Nothing Then
                Dim sURLLogo = oWhiteLabel.BackgroundLoginImage
                If System.IO.File.Exists(MapPath(sURLLogo)) Then
                    Return "<img width='100' height='100' src='" & sURLLogo & "'/>"
                Else
                    Return "<h3>Upload Your Background Login Image</h3>"
                End If
            End If
            Return ""
        End Function

        Protected Sub btnSaveLogo_ServerClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSaveLogo.Click
            Dim oFIleDB As New FileDB.CFileDB()
            If FULogo.FileName <> "" Then
                AddFile()
            Else
                ClientAlert("Please, Insert Logo Image ")
            End If
        End Sub

        Public Function AddFile() As Boolean
            Try
                Dim filejpg = (New System.IO.FileInfo(FULogo.FileName)).Extension.ToLower().IndexOf(".jpg")
                Dim filegif = (New System.IO.FileInfo(FULogo.FileName)).Extension.ToLower().IndexOf(".gif")
                Dim filepng = (New System.IO.FileInfo(FULogo.FileName)).Extension.ToLower().IndexOf(".png")
                Dim sLogoUrl As String = newGUID()
                If filejpg > -1 OrElse filegif > -1 OrElse filepng > -1 Then
                    oWhiteLabel = oWhiteLabel.LoadByUrl(Request.Url.Host)
                    oWhiteLabel.LogoFileName = "/SBSCommon/" & sLogoUrl & "_Logo.png"
                    oWhiteLabel.Update()
                    FULogo.PostedFile.SaveAs(Server.MapPath("/SBSCommon/" & sLogoUrl & "_Logo.png"))
                    ClearCache()
                    Dim sKey As String = "WHITE_LABEL_" & oWhiteLabel.SiteURL
                    Cache.Remove(sKey)
                    Response.Redirect(Request.Url.AbsoluteUri)
                Else
                    ClientAlert("File Logo Is Not Valid", True)
                End If
                Return True
            Catch ex As Exception
                LogError(log, "Unsuccessfully Uploaded", ex)
                Return False
            End Try
            Return False
        End Function

#Region "background"
        Protected Sub btnSaveBackground_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSaveBackground.Click
            If FuBackgroundImage.FileName <> "" Then
                Dim oAgentManager As New SBCBL.Managers.CAgentManager()
                oWhiteLabel = oWhiteLabel.LoadByUrl(Request.Url.Host)
                Dim sLogoUrl As String = newGUID()
                Dim sBackgroundImage As String = sLogoUrl & "_BackgroundImage" & (New System.IO.FileInfo(FuBackgroundImage.FileName)).Extension
                If oAgentManager.UpdateBackgroundImage(UserSession.UserID, "/SBSCommon/" & sBackgroundImage, UserSession.UserID) Then
                    FuBackgroundImage.SaveAs(Server.MapPath("/SBSCommon/" & sBackgroundImage))
                Else
                    ClientAlert("Please,Contact Admin To Setup SiteOption", True)
                    Return
                End If
                Response.Redirect(Request.Url.AbsoluteUri)
            Else
                ClientAlert("Please,Select Image To Upload Your Image Background ")
            End If
        End Sub

        Protected Sub btnRemoveBackground_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRemoveBackground.Click
            Dim oAgentManager As New SBCBL.Managers.CAgentManager()
            oAgentManager.UpdateBackgroundImage(UserSession.UserID, "Remove", UserSession.UserID)
            Response.Redirect(Request.Url.AbsoluteUri)
        End Sub
#End Region

#Region "login background"
        Protected Sub btnSaveLoginBackground_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSaveLoginBackground.Click
            If FuBackgroundLoginImage.FileName <> "" Then
                Dim sLogoUrl As String = newGUID()
                oWhiteLabel = oWhiteLabel.LoadByUrl(Request.Url.Host)
                Dim sBackgroundLoginImage As String = sLogoUrl & "_BackgroundLoginImage" & (New System.IO.FileInfo(FuBackgroundLoginImage.FileName)).Extension
                FuBackgroundLoginImage.SaveAs(Server.MapPath("/SBSCommon/" & sBackgroundLoginImage))
                oWhiteLabel.BackgroundLoginImage = "/SBSCommon/" & sBackgroundLoginImage
                oWhiteLabel.Update()
                Dim sKey As String = "WHITE_LABEL_" & oWhiteLabel.SiteURL
                Cache.Remove(sKey)
                Response.Redirect(Request.Url.AbsoluteUri)
            Else
                ClientAlert("Please,Select Image To Upload Your Login Image Background ")
            End If
        End Sub

        Protected Sub btnRemoveLoginBackground_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRemoveLoginBackground.Click
            oWhiteLabel = oWhiteLabel.LoadByUrl(Request.Url.Host)
            oWhiteLabel.BackgroundLoginImage = ""
            oWhiteLabel.Update(True)
            Dim sKey As String = "WHITE_LABEL_" & oWhiteLabel.SiteURL
            Cache.Remove(sKey)
            Response.Redirect(Request.Url.AbsoluteUri)
        End Sub

        Protected Sub btnSaveLoginLeftBackground_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSaveLoginLeftBackground.Click
            If FuLeftBackgroundLoginImage.FileName <> "" Then
                Dim sLogoUrl As String = newGUID()
                oWhiteLabel = oWhiteLabel.LoadByUrl(Request.Url.Host)
                Dim sBackgroundLoginImage As String = sLogoUrl & "_LeftBackgroundLoginImage" & (New System.IO.FileInfo(FuLeftBackgroundLoginImage.FileName)).Extension
                FuLeftBackgroundLoginImage.SaveAs(Server.MapPath("/SBSCommon/" & sBackgroundLoginImage))
                oWhiteLabel.LeftBackgroundLoginImage = "/SBSCommon/" & sBackgroundLoginImage
                oWhiteLabel.Update()
                Dim sKey As String = "WHITE_LABEL_" & oWhiteLabel.SiteURL
                Cache.Remove(sKey)
                Response.Redirect(Request.Url.AbsoluteUri)
            Else
                ClientAlert("Please,Select Image To Upload Your Login Left Image Background ")
            End If
        End Sub

        Protected Sub btnRemoveLoginLeftBackground_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRemoveLoginLeftBackground.Click
            oWhiteLabel = oWhiteLabel.LoadByUrl(Request.Url.Host)
            oWhiteLabel.LeftBackgroundLoginImage = ""
            oWhiteLabel.Update(True)
            Dim sKey As String = "WHITE_LABEL_" & oWhiteLabel.SiteURL
            Cache.Remove(sKey)
            Response.Redirect(Request.Url.AbsoluteUri)
        End Sub

        Protected Sub btnSaveLoginRightBackground_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSaveLoginRightBackground.Click
            If FuRightBackgroundLoginImage.FileName <> "" Then
                Dim sLogoUrl As String = newGUID()
                oWhiteLabel = oWhiteLabel.LoadByUrl(Request.Url.Host)
                Dim sBackgroundLoginImage As String = sLogoUrl & "_RightBackgroundLoginImage" & (New System.IO.FileInfo(FuRightBackgroundLoginImage.FileName)).Extension
                FuRightBackgroundLoginImage.SaveAs(Server.MapPath("/SBSCommon/" & sBackgroundLoginImage))
                oWhiteLabel.RightBackgroundLoginImage = "/SBSCommon/" & sBackgroundLoginImage
                oWhiteLabel.Update()
                Dim sKey As String = "WHITE_LABEL_" & oWhiteLabel.SiteURL
                Cache.Remove(sKey)
                Response.Redirect(Request.Url.AbsoluteUri)
            Else
                ClientAlert("Please,Select Image To Upload Your Login Right Image Background ")
            End If
        End Sub

        Protected Sub btnRemoveLoginRightBackground_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRemoveLoginRightBackground.Click
            oWhiteLabel = oWhiteLabel.LoadByUrl(Request.Url.Host)
            oWhiteLabel.RightBackgroundLoginImage = ""
            oWhiteLabel.Update(True)
            Dim sKey As String = "WHITE_LABEL_" & oWhiteLabel.SiteURL
            Cache.Remove(sKey)
            Response.Redirect(Request.Url.AbsoluteUri)
        End Sub

        Protected Sub btnSaveLoginBottomBackground_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSaveLoginBottomBackground.Click
            If FuBottomBackgroundLoginImage.FileName <> "" Then
                Dim sLogoUrl As String = newGUID()
                oWhiteLabel = oWhiteLabel.LoadByUrl(Request.Url.Host)
                Dim sBackgroundLoginImage As String = sLogoUrl & "_BottomBackgroundLoginImage" & (New System.IO.FileInfo(FuBottomBackgroundLoginImage.FileName)).Extension
                FuBottomBackgroundLoginImage.SaveAs(Server.MapPath("/SBSCommon/" & sBackgroundLoginImage))
                oWhiteLabel.BottomBackgroundLoginImage = "/SBSCommon/" & sBackgroundLoginImage
                oWhiteLabel.Update()
                Dim sKey As String = "WHITE_LABEL_" & oWhiteLabel.SiteURL
                Cache.Remove(sKey)
                Response.Redirect(Request.Url.AbsoluteUri)
            Else
                ClientAlert("Please,Select Image To Upload Your Login Bottom Image Background ")
            End If
        End Sub

        Protected Sub btnRemoveLoginBottomBackground_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRemoveLoginBottomBackground.Click
            oWhiteLabel = oWhiteLabel.LoadByUrl(Request.Url.Host)
            oWhiteLabel.BottomBackgroundLoginImage = ""
            oWhiteLabel.Update(True)
            Dim sKey As String = "WHITE_LABEL_" & oWhiteLabel.SiteURL
            Cache.Remove(sKey)
            Response.Redirect(Request.Url.AbsoluteUri)
        End Sub





#End Region




#Region "login Logo image "
        Protected Sub btnSaveLogoLogin_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSaveLogoLogin.Click
            If FuLogoLoginImage.FileName <> "" Then
                Dim sLogoUrl As String = newGUID()
                oWhiteLabel = oWhiteLabel.LoadByUrl(Request.Url.Host)
                Dim sLogoLoginImage As String = sLogoUrl & "_LogoLoginImage" & (New System.IO.FileInfo(FuLogoLoginImage.FileName)).Extension
                FuLogoLoginImage.SaveAs(Server.MapPath("/SBSCommon/" & sLogoLoginImage))
                oWhiteLabel.LoginLogo = "/SBSCommon/" & sLogoLoginImage
                oWhiteLabel.Update()
                Dim sKey As String = "WHITE_LABEL_" & oWhiteLabel.SiteURL
                Cache.Remove(sKey)
                Response.Redirect(Request.Url.AbsoluteUri)
            Else
                ClientAlert("Please,Select Image To Upload Your Logo Login ")
            End If
        End Sub

        Protected Sub btnRemoveLogoLogin_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRemoveLogoLogin.Click
            oWhiteLabel = oWhiteLabel.LoadByUrl(Request.Url.Host)
            oWhiteLabel.LoginLogo = ""
            oWhiteLabel.Update(True)
            Dim sKey As String = "WHITE_LABEL_" & oWhiteLabel.SiteURL
            Cache.Remove(sKey)
            Response.Redirect(Request.Url.AbsoluteUri)
        End Sub
#End Region

    End Class

End Namespace

