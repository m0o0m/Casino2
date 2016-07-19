Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports SBCBL.std
Imports System.Data

Namespace SBCSuperAdmin
    Partial Class FixSpreadMoney
        Inherits SBCBL.UI.CSBCUserControl
        Private ReadOnly _Category As String = "FixedSpreadMoney"
        Dim _Key As String = "SpreadMoney"
        Dim _HalfKey As String = "HalfSpreadMoney"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                bindData()
                tbFixedSpread.Visible = UserSession.SuperAdminInfo.IsManager
            End If
        End Sub

        Private Sub bindData()
            txtSpreadMoney.Text = UserSession.Cache.GetSysSettings(_Category).GetValue(_Key)
            txtHalfSpreadMoney.Text = UserSession.Cache.GetSysSettings(_Category).GetValue(_HalfKey)
        End Sub

        Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
            Dim oSysManager As New CSysSettingManager()
            Dim oListSettings As CSysSettingList = UserSession.SysSettings(_Category)
            Dim oSetting As CSysSetting = oListSettings.Find(Function(x) x.Key = _Key)
            Dim nValue As Double = SafeDouble(txtSpreadMoney.Text)
            Dim bExist As Boolean = oSysManager.IsExistedKey(_Category, "", _Key)
            If oSetting Is Nothing Then
                oSysManager.AddSysSetting(_Category, _Key, nValue.ToString())
            Else
                oSysManager.UpdateKeyValue(oSetting.SysSettingID, _Key, nValue.ToString())
            End If

            nValue = SafeDouble(txtHalfSpreadMoney.Text)
            bExist = oSysManager.IsExistedKey(_Category, "", _HalfKey)
            oSetting = oListSettings.Find(Function(x) x.Key = _HalfKey)
            If oSetting Is Nothing Then
                oSysManager.AddSysSetting(_Category, _HalfKey, nValue.ToString())
            Else
                oSysManager.UpdateKeyValue(oSetting.SysSettingID, _HalfKey, nValue.ToString())
            End If
            UserSession.Cache.ClearSysSettings(_Category)
            ClientAlert("Submit successfully", True)
        End Sub

    End Class
End Namespace
