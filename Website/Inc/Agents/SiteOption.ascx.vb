Imports System.Data
Imports SBCBL.std
Imports System.IO
Imports SBCBL.Managers

Namespace SBSAgents
    Partial Class SiteOption
        Inherits SBCBL.UI.CSBCUserControl
        Private oWhiteLabel As New SBCBL.CacheUtils.CWhiteLabelSettings()
        Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

        Protected Sub BindLoginTemplates()
            ddlLoginTemplate.DataSource = (New CWhiteLabelSettingManager()).GetLoginTemplates(Server.MapPath("/"))
            ddlLoginTemplate.DataBind()
        End Sub

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            If Not IsPostBack Then
                BindLoginTemplates()
            End If
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                oWhiteLabel = oWhiteLabel.LoadByUrl(Request.Url.Host)
                If String.IsNullOrEmpty(UserSession.AgentUserInfo.ColorScheme) Then
                    ddlColor.SelectedValue = oWhiteLabel.ColorScheme
                Else
                    ddlColor.SelectedValue = UserSession.AgentUserInfo.ColorScheme
                End If
                ddlLoginTemplate.SelectedValue = oWhiteLabel.LoginTemplate
                txtSuperAgentPhone.Text = oWhiteLabel.SuperAgentPhone
            End If
        End Sub

        Protected Sub bnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bnSave.Click
            oWhiteLabel = oWhiteLabel.LoadByUrl(Request.Url.Host)
            Dim oAgentManager As New CAgentManager()

            If Not oAgentManager.UpdateColorScheme(UserSession.UserID, ddlColor.SelectedValue, UserSession.UserID) Then
                ClientAlert("Please,Contact Admin To Setup SiteOption", True)
                Return
            End If
            If SafeString(txtSuperAgentPhone.Text).Length > 19 Then
                ClientAlert("Phone not Valid")
                Return
            End If
            If oWhiteLabel IsNot Nothing Then
                oWhiteLabel.LoginTemplate = ddlLoginTemplate.SelectedItem.Text
                oWhiteLabel.SuperAgentPhone = SafeString(txtSuperAgentPhone.Text)
                oWhiteLabel.Update()
                Dim sKey As String = "WHITE_LABEL_" & oWhiteLabel.SiteURL
                Cache.Remove(sKey)
                ClientAlert("Save Successfully ", True)
            Else
                ClientAlert("Please,Contact Admin To Setup SiteOption", True)
            End If

        End Sub
    End Class
End Namespace