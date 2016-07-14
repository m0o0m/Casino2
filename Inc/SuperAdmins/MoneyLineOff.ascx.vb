Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports SBCBL.std
Imports System.Data

Namespace SBCSuperAdmins

    Partial Class MoneyLineOff
        Inherits SBCBL.UI.CSBCUserControl
        Private _sGameType As String = SBCBL.std.GetSiteType & " GameType"
        Private _log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

        Private Sub LoadLineOff()
            Dim sCategory = SBCBL.std.GetSiteType & " MoneyLineOff"
            Dim sKey As String = ddlSportType.SelectedValue
            Dim oListSettings As CSysSettingList = UserSession.SysSettings(sCategory, "GT")
            Dim oSetting As CSysSetting = oListSettings.Find(Function(x) x.Key = sKey)

            If oSetting IsNot Nothing Then
                txtAwaySpreadGT.Text = oSetting.Value
            Else
                txtAwaySpreadGT.Text = ""
            End If

            oListSettings = UserSession.SysSettings(sCategory, "LT")
            oSetting = oListSettings.Find(Function(x) x.Key = sKey)
            If oSetting IsNot Nothing Then
                txtAwaySpreadLT.Text = oSetting.Value
            Else
                txtAwaySpreadLT.Text = ""
            End If

        End Sub

        Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
            Dim oSysManager As New CSysSettingManager()
            Dim sCategory = SBCBL.std.GetSiteType & " MoneyLineOff"
            Dim sKey As String = ddlSportType.SelectedValue
            Dim oListSettings As CSysSettingList = UserSession.SysSettings(sCategory, "GT")
            Dim oSetting As CSysSetting = oListSettings.Find(Function(x) x.Key = sKey)
            Dim nValue As Integer = SafeInteger(txtAwaySpreadGT.Text)

            If oSetting Is Nothing Then
                oSysManager.AddSysSetting(sCategory, sKey, nValue.ToString(), "GT")
            Else
                oSysManager.UpdateKeyValue(oSetting.SysSettingID, sKey, nValue.ToString(), 0, "GT")
            End If

            oListSettings = UserSession.SysSettings(sCategory, "LT")
            oSetting = oListSettings.Find(Function(x) x.Key = sKey)
            nValue = SafeInteger(txtAwaySpreadLT.Text)

            If oSetting Is Nothing Then
                oSysManager.AddSysSetting(sCategory, sKey, nValue.ToString(), "LT")
            Else
                oSysManager.UpdateKeyValue(oSetting.SysSettingID, sKey, nValue.ToString(), 0, "LT")
            End If

            UserSession.Cache.ClearSysSettings(sCategory, "GT")
            UserSession.Cache.ClearSysSettings(sCategory, "LT")

            ClientAlert("Successfully Saved", True)
        End Sub

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            bindGameType()
        End Sub

        Private Sub bindGameType()
            Dim oGameTypes As List(Of CSysSetting) = (From oSetting As CSysSetting In UserSession.Cache.GetAllSysSettings(_sGameType) _
                                                  Where SafeBoolean(oSetting.Value) And oSetting.SubCategory = "" _
                                                  Order By oSetting.SubCategory, oSetting.Key _
                                                  Select oSetting).ToList
            Dim lstSporttype As New List(Of String)
            lstSporttype.Add("Football")
            lstSporttype.Add("Basketball")
            lstSporttype.Add("Baseball")
            lstSporttype.Add("Hockey")
            lstSporttype.Add("Soccer")
            lstSporttype.Add("Tennis")
            lstSporttype.Add("Golf")
            lstSporttype.Add("Other")
            For Each osys In oGameTypes
                If Not lstSporttype.Contains(osys.Key) Then
                    lstSporttype.Add(osys.Key)
                End If
            Next

            ddlSportType.DataSource = lstSporttype
            'ddlSportType.DataTextField = "Key"
            'ddlSportType.DataValueField = "Key"
            ddlSportType.DataBind()
        End Sub

        Protected Sub ddlSportType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlSportType.SelectedIndexChanged
            If ddlSportType.SelectedValue <> "" Then
                LoadLineOff()
            End If
        End Sub
    End Class
End Namespace