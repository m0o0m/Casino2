Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports SBCBL.std

Namespace SBSSuperAdmin
    Partial Class AllowBetting
        Inherits SBCBL.UI.CSBCPage
        Public Property TableName() As String
            Get
                Return SafeString(ViewState("TableName"))
            End Get
            Set(ByVal value As String)
                ViewState("TableName") = value
            End Set
        End Property

        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Me.PageTitle = "Betting Setting"
            Me.SideMenuTabName = "ALLOW_BETTING"
            Me.MenuTabName = "SYSTEM MANAGEMENT"
            DisplaySubTitlle(Me.Master, "Betting Setting")
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                Dim oCache As New CCacheManager
                lblBettingEnble.Text = SafeString(oCache.GetSysSettings("BettingSetup", "BettingEnable").GetBooleanValue("BettingEnable"))
                hfBettingID.Value = oCache.GetSysSettings("BettingSetup", "BettingEnable").GetSysSetting("BettingEnable").SysSettingID
                lblOverrideBetting.Text = SafeString(oCache.GetSysSettings("BettingSetup", "OverrideBettingEnable").GetBooleanValue("OverrideBettingEnable"))
                hfOverrideBettingID.Value = oCache.GetSysSettings("BettingSetup", "OverrideBettingEnable").GetSysSetting("OverrideBettingEnable").SysSettingID

                loadDST()
                loadImpersonate()
                BindDataDelete()
            End If
        End Sub

        Protected Sub btnChangeBetting_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnChangeBetting.Click
            Dim sValue As String = SafeString(IIf(lblBettingEnble.Text = "Yes", "No", "Yes"))
            If (New CSysSettingManager).UpdateKeyValue(hfBettingID.Value, "BettingEnable", sValue) Then
                lblBettingEnble.Text = sValue
                Dim oCache As New CCacheManager
                oCache.ClearSysSettings("BettingSetup", "BettingEnable")

                ClientAlert("Successfully Changed", True)
            Else
                ClientAlert("Unsuccessfully Changed", True)
            End If
        End Sub

        Protected Sub btnOverrideBetting_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnOverrideBetting.Click
            Dim sValue As String = SafeString(IIf(lblOverrideBetting.Text = "Yes", "No", "Yes"))
            If (New CSysSettingManager).UpdateKeyValue(hfOverrideBettingID.Value, "OverrideBettingEnable", sValue) Then
                lblOverrideBetting.Text = sValue
                Dim oCache As New CCacheManager
                oCache.ClearSysSettings("BettingSetup", "OverrideBettingEnable")

                ClientAlert("Successfully Changed", True)
            Else
                ClientAlert("Unsuccessfully Changed", True)
            End If
        End Sub

        Protected Sub chkDST_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkDST.CheckedChanged
            Dim bValue As Boolean = SafeBoolean(chkDST.Checked)
            SaveSettings("DayLight", "DayLight", bValue.ToString())
        End Sub

        Protected Sub chkImpersonate_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkImpersonate.CheckedChanged
            Dim bValue As Boolean = SafeBoolean(chkImpersonate.Checked)
            SaveSettings("ImpersonateUser", "Impersonate", bValue.ToString())
        End Sub

        Private Sub loadDST()
            chkDST.Checked = SafeBoolean(UserSession.Cache.GetSysSettings("DayLight").GetValue("DayLight"))
        End Sub

        Private Sub loadImpersonate()
            chkImpersonate.Checked = SafeBoolean(UserSession.Cache.GetSysSettings("ImpersonateUser").GetValue("Impersonate"))
        End Sub

        Private Sub SaveSettings(ByVal psCategory As String, ByVal psKey As String, ByVal psValue As String)
            Dim oSysManager As New CSysSettingManager()
            Dim oListSettings As CSysSettingList = UserSession.SysSettings(psCategory)
            Dim oSetting As CSysSetting = oListSettings.Find(Function(x) x.Key = psKey)

            Dim bExist As Boolean = oSysManager.IsExistedKey(psCategory, "", psKey)
            If oSetting Is Nothing Then
                oSysManager.AddSysSetting(psCategory, psKey, psValue)
            Else
                oSysManager.UpdateKeyValue(oSetting.SysSettingID, psKey, psValue)
            End If
            UserSession.Cache.ClearSysSettings(psCategory)
        End Sub

#Region "delete data"
        Public Sub ddlSportType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlSportType.SelectedIndexChanged
            BindDataDelete()
            dgRowDelete.DataSource = Nothing
            dgRowDelete.DataBind()
            'BindGameDelete(TableName)
        End Sub

        Public Sub btnProcess_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim oDataManager As New CDataManager()
            Dim bDelete As Boolean
            bDelete = oDataManager.DeleteData(ddlSportType.SelectedValue)
            dgRowDelete.CurrentPageIndex = 0
            BindGameDelete(TableName)
            BindDataDelete()
            If bDelete Then
                ClientAlert("Purging Data Successfull ")
            Else
                ClientAlert("Purging Data Fail ")
            End If


        End Sub

        Public Sub BindDataDelete()
            Dim oDataManager As New CDataManager()
            lblGameDelete.Text = oDataManager.RowDeleteDataGame(ddlSportType.SelectedValue)
            lblGameLineDelete.Text = oDataManager.RowDeleteDataGameLine(ddlSportType.SelectedValue)
            lblTicketDelete.Text = oDataManager.RowDeleteDataTicKet(ddlSportType.SelectedValue)
            lblTicketBetDelete.Text = oDataManager.RowDeleteDataTicKetBet(ddlSportType.SelectedValue)
        End Sub

        Public Sub ViewDetailDelete(ByVal sender As Object, ByVal e As System.EventArgs)
            TableName = UCase(CType(sender, LinkButton).CommandArgument)
            dgRowDelete.CurrentPageIndex = 0
            BindGameDelete(TableName)
        End Sub

        Public Sub BindGameDelete(ByVal sTable As String)
            Dim oDataManager As New CDataManager()
            Select Case sTable
                Case "GAME"
                    dgRowDelete.DataSource = oDataManager.GetData("Games", ddlSportType.SelectedValue)
                    dgRowDelete.DataBind()
                Case "GAMELINE"
                    dgRowDelete.DataSource = oDataManager.GetData("GameLines", ddlSportType.SelectedValue)
                    dgRowDelete.DataBind()
                Case "TICKET"
                    dgRowDelete.DataSource = oDataManager.GetData("TicKets", ddlSportType.SelectedValue)
                    dgRowDelete.DataBind()
                Case "TICKETBET"
                    dgRowDelete.DataSource = oDataManager.GetData("TicKetBets", ddlSportType.SelectedValue)
                    dgRowDelete.DataBind()
            End Select
        End Sub
#End Region

    End Class
End Namespace