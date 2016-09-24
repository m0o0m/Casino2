Imports SBCBL
Imports SBCBL.CacheUtils
Imports SBCBL.CEnums
Imports SBCBL.std
Imports System.Data
Imports SBCBL.Managers

Partial Class SBS_Agents_PlayerJuiceControl
    Inherits SBCBL.UI.CSBCPage

    Private _log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())
    Private _sGameType As String = std.GetSiteType & " GameType"

    Public JSS As Web.Script.Serialization.JavaScriptSerializer = New Web.Script.Serialization.JavaScriptSerializer()

    Public Sports As List(Of CNameValue) = New List(Of CNameValue)
    Public BetTypes As List(Of CNameValue) = New List(Of CNameValue)
    Public JuiceRules As List(Of PlayerJuice) = New List(Of PlayerJuice)

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack AndAlso Not IsCallback Then
            bindPlayers()

            BindPages()
        End If

    End Sub

#Region "Save Player Juice"

    <System.Web.Services.WebMethod()> _
    Public Shared Function SaveJuices(ByVal rules As List(Of PlayerJuice), ByVal playerId As String) As Boolean
        Dim success = False

        Dim sysSettings = GetPlayerJuiceSettings(rules)
        If sysSettings.Count = 0 Then
            Return success
        End If

        Dim oSysManager As New CSysSettingManager()
        Dim oCache As New CCacheManager()
        Dim cateName = playerId & "_Juice"
        ' delete
        Dim deleteItems = sysSettings.Where(Function(s) String.IsNullOrEmpty(s.Category) AndAlso Not String.IsNullOrEmpty(s.SysSettingID)).ToList()
        For Each dss As CSysSetting In deleteItems
            If Not String.IsNullOrEmpty(dss.SysSettingID) Then
                If oSysManager.DeleteSetting(dss.SysSettingID) Then
                    ' clear cache
                    oCache.ClearSysSettings(cateName, dss.SubCategory)
                    oCache.ClearPlayerJuiceControl(UCase(playerId), UCase(dss.SubCategory), UCase(dss.Key), UCase(dss.Orther), dss.OtherType)
                End If
            End If
        Next

        ' add or edit
        Dim saveItems = sysSettings.Except(deleteItems).Where(Function(s) Not String.IsNullOrWhiteSpace(s.SubCategory) AndAlso ((String.IsNullOrEmpty(s.Category) AndAlso String.IsNullOrEmpty(s.SysSettingID)) OrElse _
                                                                  (Not String.IsNullOrEmpty(s.Category) AndAlso Not String.IsNullOrEmpty(s.SysSettingID)))).ToList()
        For Each ss As CSysSetting In saveItems
            If Not String.IsNullOrEmpty(ss.SysSettingID) Then
                oSysManager.UpdateByKey(ss.SysSettingID, ss.Key, ss.Value, ss.ItemIndex, ss.SubCategory, ss.Orther, ss.OtherType)
            Else
                oSysManager.AddSysSetting(cateName, ss.Key, ss.Value, ss.SubCategory, ss.ItemIndex, ss.Orther, ss.OtherType)
            End If
            ' clear cache
            oCache.ClearSysSettings(cateName, ss.SubCategory)
            oCache.ClearPlayerJuiceControl(UCase(playerId), UCase(ss.SubCategory), UCase(ss.Key), UCase(ss.Orther), ss.OtherType)
        Next
        success = True

        Return success
    End Function

    Private Shared Function GetPlayerJuiceSettings(ByVal rules As List(Of PlayerJuice)) As List(Of CSysSetting)
        Dim settings = New List(Of CSysSetting)

        For Each rule As PlayerJuice In rules
            ' GM = Current
            If rule.GM OrElse Not String.IsNullOrEmpty(rule.GmId) Then
                settings.Add(New CSysSetting With {.SysSettingID = rule.GmId,
                                                   .Category = IIf(rule.GM, rule.GmCate, String.Empty),
                                                   .Orther = rule.GameType,
                                                   .Key = "Current",
                                                   .SubCategory = rule.Sport,
                                                   .Value = SafeDouble(rule.GmValue),
                                                   .OtherType = SafeInteger(rule.BetType),
                                                   .ItemIndex = SafeInteger(rule.RowIndex) + 1
                                                   })
            End If

            ' FH = 1H
            If rule.FH OrElse Not String.IsNullOrEmpty(rule.FhId) Then
                settings.Add(New CSysSetting With {.SysSettingID = rule.FhId,
                                                   .Category = IIf(rule.FH, rule.FhCate, String.Empty),
                                                   .Orther = rule.GameType,
                                                   .Key = "1H",
                                                   .SubCategory = rule.Sport,
                                                   .Value = SafeDouble(rule.FhValue),
                                                   .OtherType = SafeInteger(rule.BetType),
                                                   .ItemIndex = SafeInteger(rule.RowIndex) + 1
                                                   })

            End If

            ' SH = 2H
            If rule.SH OrElse Not String.IsNullOrEmpty(rule.ShId) Then
                settings.Add(New CSysSetting With {.SysSettingID = rule.ShId,
                                                   .Category = IIf(rule.SH, rule.ShCate, String.Empty),
                                                   .Orther = rule.GameType,
                                                   .Key = "2H",
                                                   .SubCategory = rule.Sport,
                                                   .Value = SafeDouble(rule.ShValue),
                                                   .OtherType = SafeInteger(rule.BetType),
                                                   .ItemIndex = SafeInteger(rule.RowIndex) + 1
                                                   })
            End If

        Next

        Return settings.Where(Function(s) Not String.IsNullOrWhiteSpace(s.SubCategory)).ToList()
    End Function

#End Region

#Region "Get Player Juice"

    Private Function GetPlayerJuice(ByVal playerId As String) As List(Of PlayerJuice)
        Dim rules = New List(Of PlayerJuice)

        If (String.IsNullOrWhiteSpace(playerId)) Then
            Return rules
        End If

        Dim oSysManager As New CSysSettingManager()
        Dim cateName = playerId & "_Juice"
        Dim dbJuices = oSysManager.GetAllSysSettings(cateName)

        For Each j In dbJuices
            'j.SubCategory = j.SubCategory.ToUpper()
            If j.OtherType Is Nothing Then
                j.OtherType = ""
            End If
        Next

        Dim groups = From j In dbJuices
                     Order By j.ItemIndex
                     Group By jkey = New With {Key .Sport = j.SubCategory, Key .GameType = j.Orther, Key .BetType = j.OtherType}
                     Into juicegroup = Group

        For Each group In groups
            Dim r = New PlayerJuice With {.Sport = group.jkey.Sport, .GameType = SafeString(group.jkey.GameType), .BetType = SafeInteger(group.jkey.BetType)}

            ' GM = Current
            Dim current = group.juicegroup.FirstOrDefault(Function(j) j.Key.Equals("Current", StringComparison.OrdinalIgnoreCase))
            If current IsNot Nothing Then
                r.GM = True
                r.GmId = current.SysSettingID
                r.GmCate = current.Category
                r.GmValue = SafeDouble(current.Value)
            Else
                r.GmValue = 0
            End If
            ' FH = 1H
            Dim fh = group.juicegroup.FirstOrDefault(Function(j) j.Key.Equals("1H", StringComparison.OrdinalIgnoreCase))
            If fh IsNot Nothing Then
                r.FH = True
                r.FhId = fh.SysSettingID
                r.FhCate = fh.Category
                r.FhValue = SafeDouble(fh.Value)
            Else
                r.FhValue = 0
            End If
            ' SH = 2H
            Dim sh = group.juicegroup.FirstOrDefault(Function(j) j.Key.Equals("2H", StringComparison.OrdinalIgnoreCase))
            If sh IsNot Nothing Then
                r.SH = True
                r.ShId = sh.SysSettingID
                r.ShCate = sh.Category
                r.ShValue = SafeDouble(sh.Value)
            Else
                r.ShValue = 0
            End If

            rules.Add(r)
        Next

        Return rules

    End Function

#End Region

#Region "Bind Data & Events"

    Protected Sub ddlPlayer_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlPlayer.SelectedIndexChanged
        BindPages()
    End Sub

    Private Sub BindPages()
        'Sports = (From oSetting As CSysSetting In UserSession.Cache.GetAllSysSettings(_sGameType) _
        '                                     Where SafeBoolean(oSetting.Value) And oSetting.SubCategory = "" And Not oSetting.Key.Equals("Other", StringComparison.CurrentCultureIgnoreCase) And Not oSetting.Key.Equals("proposition", StringComparison.CurrentCultureIgnoreCase) _
        '                                     Order By oSetting.SubCategory, oSetting.Key _
        '                                     Select oSetting.Key).ToList
        Sports = GetSportGameTypes()

        BetTypes = EnumUtils.GetEnumDescriptions(Of EBetType)()

        JuiceRules = GetPlayerJuice(ddlPlayer.SelectedValue)
    End Sub

    Private Sub bindPlayers()
        Dim dtsPlayers As DataTable
        dtsPlayers = (New CPlayerManager).GetPlayers(UserSession.AgentUserInfo.UserID, Nothing)

        ddlPlayer.DataSource = dtsPlayers
        ddlPlayer.DataTextField = "FullName"
        ddlPlayer.DataValueField = "PlayerID"
        ddlPlayer.DataBind()
    End Sub

    Private Function GetSportGameTypes() As List(Of CNameValue)
        Dim sgts As New List(Of CNameValue)
        Dim sports = (From oSetting As CSysSetting In UserSession.Cache.GetAllSysSettings(_sGameType) _
                                             Where SafeBoolean(oSetting.Value) And oSetting.SubCategory = "" And Not oSetting.Key.Equals("Other", StringComparison.CurrentCultureIgnoreCase) And Not oSetting.Key.Equals("proposition", StringComparison.CurrentCultureIgnoreCase) _
                                             Order By oSetting.SubCategory, oSetting.Key _
                                             Select oSetting.Key).ToList

        For Each s As String In sports
            Dim gameTypes As String()
            Select Case s.ToLower()
                Case "football"
                    gameTypes = std.FOOTBALL_GAMES.Split(New String() {";"}, StringSplitOptions.RemoveEmptyEntries)
                Case "basketball"
                    gameTypes = std.BASKETBALL_GAMES.Split(New String() {";"}, StringSplitOptions.RemoveEmptyEntries)
                Case "soccer"
                    gameTypes = std.SOCCER_GAMES.Split(New String() {";"}, StringSplitOptions.RemoveEmptyEntries)
                Case "baseball"
                    gameTypes = std.BASEBALL_GAMES.Split(New String() {";"}, StringSplitOptions.RemoveEmptyEntries)
                Case "hockey"
                    gameTypes = std.HOCKEY_GAMES.Split(New String() {";"}, StringSplitOptions.RemoveEmptyEntries)
                Case Else
                    gameTypes = New String() {}
            End Select

            '' add sport
            sgts.Add(New CNameValue With {.Name = String.Format("{0} - {1}", s, "All Game").ToUpper(), .Value = s, .Description = ""})
            For Each g As String In gameTypes.OrderBy(Function(x) x)
                sgts.Add(New CNameValue With {.Name = String.Format("{0} - {1}", s, g).ToUpper(), .Value = s, .Description = g})
            Next
        Next

        Return sgts
    End Function

#End Region

End Class
