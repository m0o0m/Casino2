Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports SBCBL.std
Imports System.Data

Namespace SBCSuperAdmins

    Partial Class TimeLineOff
        Inherits SBCBL.UI.CSBCUserControl
        Private _sGameType As String = SBCBL.std.GetSiteType & " GameType"
        Dim _sCategory As String = SBCBL.std.GetSiteType & " LineOffHour"

        Private Sub bindGameType()
            Dim oGameTypes As List(Of CSysSetting) = (From oSetting As CSysSetting In UserSession.Cache.GetAllSysSettings(_sGameType) _
                                                  Where SafeBoolean(oSetting.Value) And oSetting.SubCategory <> "" _
                                                  Order By oSetting.SubCategory, oSetting.Key _
                                                  Select oSetting).ToList

            ddlGameType.DataSource = oGameTypes
            ddlGameType.DataTextField = "Key"
            ddlGameType.DataValueField = "Key"
            ddlGameType.DataBind()
        End Sub

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            If Not IsPostBack Then
                bindGameType()
                BindTimeOff()
                LoadSecondHalfTime()
            End If
        End Sub

        Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
            If ddlGameType.SelectedValue = "" Then
                ClientAlert("Please Choose Game Type.")
                Return
            End If
            Dim nOffMinute As Integer = SafeInteger(txtOffMinutes.Text)
            Dim nDisplayHours As Integer = SafeInteger(txtDisplayHours.Text)
            Dim oSysSettingManager As New CSysSettingManager()
            Dim oSys As CSysSetting = oSysSettingManager.GetAllSysSettings(_sCategory).Find(Function(x) x.Key = ddlGameType.SelectedValue)

            If oSys IsNot Nothing Then
                oSysSettingManager.UpdateKeyValue(oSys.SysSettingID, ddlGameType.SelectedValue, nOffMinute.ToString(), 0, nDisplayHours.ToString())
            Else
                oSysSettingManager.AddSysSetting(_sCategory, ddlGameType.SelectedValue, nOffMinute.ToString(), nDisplayHours.ToString())
            End If
            System.Web.HttpContext.Current.Cache.Remove("TimeLineOff" & _sCategory)
            ClearTimeOffCache()
            BindTimeOff()
            ClientAlert("Successfully Saved", True)
        End Sub

        Private Sub BindTimeOff()
            Dim oSysManager As New SBCBL.Managers.CSysSettingManager()
            Dim oSettings As List(Of SBCBL.CacheUtils.CSysSetting) = oSysManager.GetAllSysSettings(_sCategory).FindAll(Function(x) x.Value <> "0" Or x.SubCategory <> "0")
            dtgTimeOff.DataSource = oSettings
            dtgTimeOff.DataBind()
            dtgTimeOff.Visible = oSettings.Count > 0
        End Sub

        Protected Sub ddlGameType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlGameType.SelectedIndexChanged
            Dim oSysSettingManager As New CSysSettingManager()
            Dim oSys As CSysSetting = oSysSettingManager.GetAllSysSettings(_sCategory).Find(Function(x) x.Key = ddlGameType.SelectedValue)
            If oSys IsNot Nothing AndAlso oSys.Value <> "" Then
                txtOffMinutes.Text = SafeInteger(oSys.Value)
                txtDisplayHours.Text = SafeInteger(oSys.SubCategory)
            Else
                txtOffMinutes.Text = ""
                txtDisplayHours.Text = ""
            End If
        End Sub

        Private Sub LoadSecondHalfTime()
            Dim oSysManager As New CSysSettingManager()
            Dim SecondHTime As Integer = SafeInteger(txt2H.Text)
            Dim sCategory = SBCBL.std.GetSiteType & " 2HTimeOff"
            Dim sKey As String = "TimeOff"
            Dim oListSettings As CSysSettingList = UserSession.SysSettings(sCategory)
            Dim oSetting As CSysSetting = oListSettings.Find(Function(x) x.Key = sKey)

            If oSetting IsNot Nothing Then
                txt2H.Text = oSetting.Value
            End If
        End Sub

        Protected Sub btn2HSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btn2HSave.Click
            '' 2H time off
            Dim oSysManager As New CSysSettingManager()
            Dim SecondHTime As Integer = SafeInteger(txt2H.Text)
            Dim sCategory = SBCBL.std.GetSiteType & " 2HTimeOff"
            Dim sKey As String = "TimeOff"
            Dim oListSettings As CSysSettingList = UserSession.SysSettings(sCategory)
            Dim oSetting As CSysSetting = oListSettings.Find(Function(x) x.Key = sKey)

            Dim nValue As Integer = SafeInteger(txt2H.Text)

            If oSetting Is Nothing Then
                oSysManager.AddSysSetting(sCategory, sKey, nValue.ToString())
            Else
                oSysManager.UpdateKeyValue(oSetting.SysSettingID, sKey, nValue.ToString())
            End If
            UserSession.Cache.ClearSysSettings(sCategory)
            ClearTimeOffCache()
            ClientAlert("Successfully Saved", True)
        End Sub
        Sub ClearTimeOffCache()
            Dim keys As New List(Of String)
            Dim CacheEnum As IDictionaryEnumerator = System.Web.HttpContext.Current.Cache.GetEnumerator()
            While CacheEnum.MoveNext()
                If CacheEnum.Key.ToString().Contains("TimeLineOff") Then
                    System.Web.HttpContext.Current.Cache.Remove(CacheEnum.Key.ToString())
                End If
            End While
        End Sub
    End Class
End Namespace