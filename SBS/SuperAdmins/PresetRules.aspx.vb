Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports SBCBL.std
Imports System.Data

Namespace SBSSuperAdmin
    Partial Class PresetRules
        Inherits SBCBL.UI.CSBCPage
        Private _Category As String = SBCBL.std.GetSiteType & " LineRules"

        Private _sGameType As String = SBCBL.std.GetSiteType & " GameType"

#Region "Page Events"

        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Manage Rules"
            SideMenuTabName = "PRESET_RULES"
            Me.MenuTabName = "GAME MANAGEMENT"
            DisplaySubTitlle(Me.Master, "Manage Rules")
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                bindGameType()
                LoadRules()

                ucFixedSpreadMoney.IsSuperAdmin = True
                ucFixedSpreadMoney.BindCurrentUserData()
                ucFixedSpreadMoney.bindAgents(UserSession.UserID)

                ucGameCircledSettings.IsSuperAdmin = True
                ucGameCircledSettings.bindAgents(UserSession.UserID)
                ucGameCircledSettings.GetCircleSettings(UserSession.UserID)

                ucTimeLineOff.IsSuperAdmin = True
                ucTimeLineOffHalf.IsSuperAdmin = True
                ucTimeLineOff.bindAgents(UserSession.UserID)
                ucTimeLineOffHalf.bindAgents(UserSession.UserID)

                ucTeamTotalDisplaySetup1.IsSuperAdmin = True
                ucTeamTotalDisplaySetup1.bindAgents(UserSession.UserID)
            End If
        End Sub

#End Region

#Region "Controls Events"

        Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
            If ddlSportType.SelectedValue = "" Then
                ClientAlert("Please Choose Game Type.", True)
                Return
            End If

            If ddlContext.SelectedValue = "" Then
                ClientAlert("Please Choose Game Context.", True)
                Return
            End If

            Dim oSysManager As New CSysSettingManager()
            Dim sKey As String = "AwaySpreadMoneyGT"
            Dim sGameTypeKey As String = ddlSportType.SelectedValue.Replace(" ", "_")
            Dim sSubCategory As String = sGameTypeKey & "_" & ddlContext.SelectedValue
            Dim oListSettings As CSysSettingList = UserSession.SysSettings(_Category, SafeString(sSubCategory))
            Dim oSetting As CSysSetting = oListSettings.Find(Function(x) x.Key = sKey)

            Dim nValue As Double = SafeDouble(txtAwaySpreadGT.Text)
            'Dim bExist As Boolean = oSysManager.IsExistedKey(_Category, sSubCategory, sKey)

            If oSetting Is Nothing Then
                oSysManager.AddSysSetting(_Category, sKey, nValue.ToString(), sSubCategory)
            Else
                oSysManager.UpdateKeyValue(oSetting.SysSettingID, sKey, nValue.ToString())
            End If

            sKey = "AwaySpreadMoneyLT"
            nValue = SafeDouble(txtAwaySpreadLT.Text)
            oSetting = oListSettings.Find(Function(x) x.Key = sKey)

            If oSetting Is Nothing Then
                oSysManager.AddSysSetting(_Category, sKey, nValue.ToString(), sSubCategory)
            Else
                oSetting = oListSettings.Find(Function(x) x.Key = sKey)
                oSysManager.UpdateKeyValue(oSetting.SysSettingID, sKey, nValue.ToString())
            End If

            sKey = "HomeSpreadMoneyGT"
            nValue = SafeDouble(txtHomeSpreadGT.Text)
            oSetting = oListSettings.Find(Function(x) x.Key = sKey)

            If oSetting Is Nothing Then
                oSysManager.AddSysSetting(_Category, sKey, nValue.ToString(), sSubCategory)
            Else
                oSetting = oListSettings.Find(Function(x) x.Key = sKey)
                oSysManager.UpdateKeyValue(oSetting.SysSettingID, sKey, nValue.ToString())
            End If

            sKey = "HomeSpreadMoneyLT"
            nValue = SafeDouble(txtHomeSpreadLT.Text)
            oSetting = oListSettings.Find(Function(x) x.Key = sKey)

            If oSetting Is Nothing Then
                oSysManager.AddSysSetting(_Category, sKey, nValue.ToString(), sSubCategory)
            Else
                oSetting = oListSettings.Find(Function(x) x.Key = sKey)
                oSysManager.UpdateKeyValue(oSetting.SysSettingID, sKey, nValue.ToString())
            End If

            sKey = "TotalPointGT"
            nValue = SafeDouble(txtTotalPointGT.Text)
            oSetting = oListSettings.Find(Function(x) x.Key = sKey)
            If SafeString(txtTotalPointGT.Text) = "" Then
                oSysManager.DeleteSettingByKey(_Category, sKey)
            Else
                If oSetting Is Nothing Then
                    oSysManager.AddSysSetting(_Category, sKey, nValue.ToString(), sSubCategory)
                Else
                    oSetting = oListSettings.Find(Function(x) x.Key = sKey)
                    oSysManager.UpdateKeyValue(oSetting.SysSettingID, sKey, nValue.ToString())
                End If
            End If


            sKey = "TotalPointLT"
            nValue = SafeDouble(txtTotalPointLT.Text)
            oSetting = oListSettings.Find(Function(x) x.Key = sKey)

            If SafeString(txtTotalPointLT.Text) = "" Then
                oSysManager.DeleteSettingByKey(_Category, sKey)
            Else
                If oSetting Is Nothing Then
                    oSysManager.AddSysSetting(_Category, sKey, nValue.ToString(), sSubCategory)
                Else
                    oSetting = oListSettings.Find(Function(x) x.Key = sKey)
                    oSysManager.UpdateKeyValue(oSetting.SysSettingID, sKey, nValue.ToString())
                End If
            End If

            Dim sCacheKey As String = UCase(String.Format("SBC_SYS_SETTINGS_{0}_{1}", _Category, sSubCategory))
            Cache.Remove(sCacheKey)
            sKey = "ContextFilterQuery_" & ddlSportType.Value & "_" & ddlContext.SelectedValue
            Cache.Remove(sKey)
            ClientAlert("Successfully Saved.", True)
        End Sub

        Protected Sub ddlContext_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlContext.SelectedIndexChanged
            LoadRules()
        End Sub

        Protected Sub ddlSportType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlSportType.SelectedIndexChanged
            ResetGameType()
        End Sub

#End Region

#Region "Private Functions"

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

        Private Sub LoadRules()
            If ddlContext.SelectedValue = "" Or ddlSportType.SelectedValue = "" Then
                Return
            End If

            Dim oSysManager As New CSysSettingManager()
            Dim sGameTypeKey As String = ddlSportType.SelectedValue.Replace(" ", "_")
            Dim sSubCategory As String = sGameTypeKey & "_" & ddlContext.SelectedValue

            Dim oListSettings As CSysSettingList = UserSession.SysSettings(_Category, sSubCategory)
            Dim oAwaySpreadMoneyGT As CSysSetting = oListSettings.Find(Function(x) x.Key = "AwaySpreadMoneyGT")
            Dim nAwaySpreadMoneyGT As Double = 0
            If oAwaySpreadMoneyGT IsNot Nothing Then
                nAwaySpreadMoneyGT = SafeDouble(oAwaySpreadMoneyGT.Value)
            End If
            Dim oAwaySpreadMoneyLT As CSysSetting = oListSettings.Find(Function(x) x.Key = "AwaySpreadMoneyLT")
            Dim nAwaySpreadMoneyLT As Double = 0
            If oAwaySpreadMoneyLT IsNot Nothing Then
                nAwaySpreadMoneyLT = SafeDouble(oAwaySpreadMoneyLT.Value)
            End If
            Dim oHomeSpreadMoneyGT As CSysSetting = oListSettings.Find(Function(x) x.Key = "HomeSpreadMoneyGT")
            Dim nHomeSpreadMoneyGT As Double = 0
            If oHomeSpreadMoneyGT IsNot Nothing Then
                nHomeSpreadMoneyGT = SafeDouble(oHomeSpreadMoneyGT.Value)
            End If
            Dim oHomeSpreadMoneyLT As CSysSetting = oListSettings.Find(Function(x) x.Key = "HomeSpreadMoneyLT")
            Dim nHomeSpreadMoneyLT As Double = 0
            If oHomeSpreadMoneyLT IsNot Nothing Then
                nHomeSpreadMoneyLT = SafeDouble(oHomeSpreadMoneyLT.Value)
            End If

            Dim oTotalPointGT As CSysSetting = oListSettings.Find(Function(x) x.Key = "TotalPointGT")
            Dim nTotalPointGT As Double = 0
            If oTotalPointGT IsNot Nothing Then
                txtTotalPointGT.Text = oTotalPointGT.Value
            Else
                txtTotalPointGT.Text = "0"
            End If

            Dim oTotalPointLT As CSysSetting = oListSettings.Find(Function(x) x.Key = "TotalPointLT")
            Dim nTotalPointLT As Double = 0
            If oTotalPointLT IsNot Nothing Then
                txtTotalPointLT.Text = oTotalPointLT.Value
            Else
                txtTotalPointLT.Text = "0"
            End If

            txtAwaySpreadGT.Text = nAwaySpreadMoneyGT.ToString()
            txtAwaySpreadLT.Text = nAwaySpreadMoneyLT.ToString()
            txtHomeSpreadGT.Text = nHomeSpreadMoneyGT.ToString()
            txtHomeSpreadLT.Text = nHomeSpreadMoneyLT.ToString()
        End Sub

        Private Sub ResetGameType()
            ddlContext.SelectedValue = ""
            txtAwaySpreadGT.Text = ""
            txtAwaySpreadLT.Text = ""
            txtTotalPointGT.Text = ""
            txtTotalPointLT.Text = ""
            txtHomeSpreadGT.Text = ""
            txtHomeSpreadLT.Text = ""
        End Sub

#End Region

    End Class
End Namespace