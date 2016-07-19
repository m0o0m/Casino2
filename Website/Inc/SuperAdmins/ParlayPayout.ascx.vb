Imports System.Data
Imports System.Xml

Imports SBCBL.std
Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports WebsiteLibrary.CXMLUtils
Imports FileDB

Namespace SBCSuperAdmins

    Partial Class ParlayPayout
        Inherits SBCBL.UI.CSBCUserControl

        Private CATEGORY_CURRENT As String = SBCBL.std.GetSiteType & "_PARLAY_PAYOUT_CURRENT"
        Private CATEGORY_TIGERSB As String = SBCBL.std.GetSiteType & "_PARLAY_PAYOUT_TIGERSB"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not Me.IsPostBack Then
                bindParlayPayouts()
            End If
        End Sub

#Region "Bind Payouts"

        Private PayoutCurrent As New CSysSettingList
        Private PayoutTigerSB As New CSysSettingList

        Private Sub bindParlayPayouts()
            Dim oSysMng As New CSysSettingManager

            Me.PayoutCurrent = oSysMng.GetAllSysSettings(CATEGORY_CURRENT)
            Me.PayoutTigerSB = oSysMng.GetAllSysSettings(CATEGORY_TIGERSB)

            Dim oTeams As New List(Of String)
            oTeams.AddRange("2;3;4;5;6;7;8;9;10;11;12;13;14;15".Split(";"c))

            If Me.PayoutCurrent.Count = 0 Then
                Dim oPayoutCurrent As New List(Of String)
                oPayoutCurrent.AddRange("2.654;5.958;12.283;24.359;47.413;91.424;175.446;335.852;642.082;1226.701;2342.793;4473.514;8541.254;16306.94".Split(";"c))

                setSysSettings(Me.PayoutCurrent, CATEGORY_CURRENT, oTeams, oPayoutCurrent)

                oSysMng.UpdateSysSettings(CATEGORY_CURRENT, Me.PayoutCurrent.ToList())
                UserSession.Cache.ClearAllSysSettings(CATEGORY_CURRENT)
            End If

            If Me.PayoutTigerSB.Count = 0 Then
                Dim oPayoutTigerSB As New List(Of String)
                oPayoutTigerSB.AddRange("2.6;6;11;22;40;70;100;200;400;750;1400;2500;4500;9000".Split(";"c))

                setSysSettings(Me.PayoutTigerSB, CATEGORY_TIGERSB, oTeams, oPayoutTigerSB)

                oSysMng.UpdateSysSettings(CATEGORY_TIGERSB, Me.PayoutTigerSB.ToList())
                UserSession.Cache.ClearAllSysSettings(CATEGORY_TIGERSB)
            End If

            rptTeams.DataSource = oTeams
            rptTeams.DataBind()
        End Sub

        Private Sub setSysSettings(ByRef oSettings As CSysSettingList, ByVal psCategory As String, ByVal poTeams As List(Of String), ByVal poPayouts As List(Of String))
            For nIndex As Int32 = 0 To poTeams.Count - 1
                Dim oSetting As New CSysSetting
                oSetting.Category = psCategory
                oSetting.Key = poTeams(nIndex) + " Teams"
                oSetting.Value = poPayouts(nIndex)
                oSetting.SubCategory = ""
                oSetting.ItemIndex = nIndex + 1

                oSettings.Add(oSetting)
            Next
        End Sub

#End Region

        Protected Sub rptTeams_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptTeams.ItemDataBound
            If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
                Dim sTeam As String = SafeString(e.Item.DataItem) & " Teams"

                ''current
                Dim oSetting As CSysSetting = Me.PayoutCurrent.GetSysSetting(sTeam)

                If oSetting IsNot Nothing Then
                    CType(e.Item.FindControl("hfCurrentID"), HiddenField).Value = oSetting.SysSettingID
                    CType(e.Item.FindControl("txtCurrent"), TextBox).Text = oSetting.Value
                End If

                ''tiger sb
                oSetting = Me.PayoutTigerSB.GetSysSetting(sTeam)

                If oSetting IsNot Nothing Then
                    CType(e.Item.FindControl("hfTigerSBID"), HiddenField).Value = oSetting.SysSettingID
                    CType(e.Item.FindControl("txtTigerSB"), TextBox).Text = oSetting.Value
                End If
            End If
        End Sub

        Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
            Me.PayoutCurrent.Clear()
            Me.PayoutTigerSB.Clear()

            For nIndex As Int32 = 0 To rptTeams.Items.Count - 1
                Dim oItem As RepeaterItem = rptTeams.Items(nIndex)

                If Not (oItem.ItemType = ListItemType.AlternatingItem OrElse oItem.ItemType = ListItemType.Item) Then
                    Continue For
                End If

                Dim sTeams As String = CType(oItem.FindControl("lblTeam"), Label).Text & " Teams"

                ''curent
                Dim sCurrentID As String = CType(oItem.FindControl("hfCurrentID"), HiddenField).Value
                Dim sPayoutCurrent As String = SafeDouble(CType(oItem.FindControl("txtCurrent"), TextBox).Text).ToString()

                addSysSetting(Me.PayoutCurrent, sCurrentID, CATEGORY_CURRENT, sTeams, sPayoutCurrent, nIndex + 1)

                ''tiger sb
                Dim sTigerSBID As String = CType(oItem.FindControl("hfTigerSBID"), HiddenField).Value
                Dim sPayoutTigerSB As String = SafeDouble(CType(oItem.FindControl("txtTigerSB"), TextBox).Text).ToString()

                addSysSetting(Me.PayoutTigerSB, sTigerSBID, CATEGORY_TIGERSB, sTeams, sPayoutTigerSB, nIndex + 1)
            Next

            ''update payouts
            Dim oSysMng As New CSysSettingManager

            ''curent: it's not editable
            'oSysMng.UpdateSysSettings(CATEGORY_CURRENT, Me.PayoutCurrent.ToList())
            'UserSession.Cache.ClearAllSysSettings(CATEGORY_CURRENT)

            ''tiger sb
            oSysMng.UpdateSysSettings(CATEGORY_TIGERSB, Me.PayoutTigerSB.ToList())
            UserSession.Cache.ClearAllSysSettings(CATEGORY_TIGERSB)
        End Sub

        Private Sub addSysSetting(ByRef oSettings As CSysSettingList, ByVal psSettingID As String, ByVal psCategory As String, ByVal psTeam As String, ByVal psPayout As String, ByVal pnIndex As Int32)
            Dim oSetting As New CSysSetting
            oSetting.SysSettingID = SafeString(IIf(psSettingID <> "", psSettingID, newGUID))
            oSetting.Category = psCategory
            oSetting.Key = psTeam
            oSetting.Value = psPayout
            oSetting.SubCategory = ""
            oSetting.ItemIndex = pnIndex

            oSettings.Add(oSetting)
        End Sub

    End Class

End Namespace


