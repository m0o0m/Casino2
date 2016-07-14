Imports SBCBL.CacheUtils
Imports SBCBL.std
Imports System.Data
Imports SBCBL.Managers

Namespace SBCSuperAdmin
    Partial Class TimeOff2H
        Inherits SBCBL.UI.CSBCUserControl

        Protected ReadOnly Property SecondHalfOffCategory() As String
            Get
                Return "SBS TimeOff2H"
            End Get
        End Property

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                If Not UserSession.SuperAdminInfo.IsManager Then
                    txt2HOff.Enabled = False
                    btnSaveTime.Enabled = False
                End If
                LoadSecondHalfTime()
            End If
        End Sub

        Private Sub LoadSecondHalfTime()
            Dim oSysManager As New CSysSettingManager()
            Dim SecondHTime As Integer = SafeInteger(txt2HOff.Text)

            Dim sKey As String = "TimeOff2H"
            Dim oListSettings As CSysSettingList = UserSession.SysSettings(SecondHalfOffCategory)
            Dim oSetting As CSysSetting = oListSettings.Find(Function(x) x.Key = sKey)

            If oSetting IsNot Nothing Then
                txt2HOff.Text = oSetting.Value
            End If
        End Sub


        Protected Sub btnSaveTime_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSaveTime.Click
            'Dim GameTypeOnOffManager As New CGameTypeOnOffManager()
            'Dim bSuccess As Boolean
            'If SafeInteger(txt2HOff.Text) < 0 Then
            '    ClientAlert("2H Time Off Is Not Valid ", True)
            '    Return
            'End If
            'Dim oGameHalfTimeDisplay As New CGameHalfTimeDisplay(ddlSportType.SelectedValue, SafeInteger(txt2HOff.Text))
            'If GameTypeOnOffManager.CheckExistsGameHTimeDisplay(SuperAgentID, ddlSportType.SelectedValue) Then
            '    bSuccess = GameTypeOnOffManager.UpdateGameHalfTimeDisplay(SuperAgentID, oGameHalfTimeDisplay)
            'Else
            '    bSuccess = GameTypeOnOffManager.InsertGameHalfTimeDisplay(SuperAgentID, oGameHalfTimeDisplay)
            'End If
            'If bSuccess Then
            '    UserSession.Cache.ClearGameHalfTimeDisplay(SuperAgentID)
            '    ClientAlert("Successfully Saved", True)
            'Else
            '    ClientAlert("Failed to Save Setting", True)
            'End If

            If Not UserSession.SuperAdminInfo.IsManager Then
                Return
            End If

            '' 2H time off
            Dim oSysManager As New CSysSettingManager()
            Dim SecondHTime As Integer = SafeInteger(txt2HOff.Text)

            Dim sKey As String = "TimeOff2H"
            Dim oListSettings As CSysSettingList = UserSession.SysSettings(SecondHalfOffCategory)
            Dim oSetting As CSysSetting = oListSettings.Find(Function(x) x.Key = sKey)

            Dim nValue As Integer = SafeInteger(txt2HOff.Text)

            If oSetting Is Nothing Then
                oSysManager.AddSysSetting(SecondHalfOffCategory, sKey, nValue.ToString())
            Else
                oSysManager.UpdateKeyValue(oSetting.SysSettingID, sKey, nValue.ToString())
            End If
            UserSession.Cache.ClearSysSettings(SecondHalfOffCategory)

            ClientAlert("Display game time has been changed.", True)


        End Sub



        'Private Sub SetSportType()
        '    Dim oGameHalfTimeDisplayList As CGameHalfTimeDisplayList = UserSession.Cache.GetGameHalfTimeDisplay(SuperAgentID)
        '    Dim oGameHalfTimeDisplay As CGameHalfTimeDisplay = oGameHalfTimeDisplayList.GetGameHalfTimeDisplay(ddlSportType.SelectedValue)
        '    If oGameHalfTimeDisplay IsNot Nothing Then
        '        txt2HOff.Text = oGameHalfTimeDisplay.SecondHOff
        '    Else
        '        txt2HOff.Text = ""
        '    End If
        'End Sub


    End Class

End Namespace

