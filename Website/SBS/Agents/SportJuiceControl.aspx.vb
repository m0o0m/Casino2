Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports SBCBL.std
Imports SBCBL.CEnums
Imports SBCBL

Partial Class SBS_Agents_SportJuiceControl
    Inherits SBCBL.UI.CSBCPage

    Private _log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())
    Private _sGameType As String = SBCBL.std.GetSiteType & " GameType"

    Public JSS As Web.Script.Serialization.JavaScriptSerializer = New Web.Script.Serialization.JavaScriptSerializer()

    Public Sports As List(Of CNameValue) = New List(Of CNameValue)
    Public BetTypes As List(Of CNameValue) = New List(Of CNameValue)
    Public JuiceRules As List(Of SportJuice) = New List(Of SportJuice)

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack AndAlso Not IsCallback Then
            'Sports = (From oSetting As CSysSetting In UserSession.Cache.GetAllSysSettings(_sGameType) _
            '                                     Where SafeBoolean(oSetting.Value) And oSetting.SubCategory = "" And Not oSetting.Key.Equals("Other", StringComparison.CurrentCultureIgnoreCase) And Not oSetting.Key.Equals("proposition", StringComparison.CurrentCultureIgnoreCase) _
            '                                     Order By oSetting.SubCategory, oSetting.Key _
            '                                     Select oSetting.Key).ToList
            Sports = GetSportGameTypes()

            BetTypes = EnumUtils.GetEnumDescriptions(Of EBetType)()

            JuiceRules = GetSportJuice()
        End If

    End Sub

#Region "Save Sport Juice"

    <System.Web.Services.WebMethod()> _
    Public Shared Function SaveJuices(ByVal rules As List(Of SportJuice), ByVal agentId As String) As Boolean
        Dim success = False

        Dim sysSettings = GetSportJuiceSettings(rules)
        If sysSettings.Count = 0 Then
            Return success
        End If

        Dim oSysManager As New CSysSettingManager()
        Dim oCache As New CCacheManager()
        Dim cateName = agentId & "_Juice"
        ' delete
        Dim deleteItems = sysSettings.Where(Function(s) String.IsNullOrEmpty(s.Category) AndAlso Not String.IsNullOrEmpty(s.SysSettingID)).ToList()
        For Each dss As CSysSetting In deleteItems
            If Not String.IsNullOrEmpty(dss.SysSettingID) Then
                If oSysManager.DeleteSetting(dss.SysSettingID) Then
                    ' clear cache
                    oCache.ClearSysSettings(cateName, dss.SubCategory)
                    oCache.ClearJuiceControl(UCase(agentId), UCase(dss.SubCategory), UCase(dss.Key), UCase(dss.Orther), dss.OtherType)
                End If
            End If
        Next

        ' add or edit
        Dim saveItems = sysSettings.Except(deleteItems).Where(Function(s) Not String.IsNullOrWhiteSpace(s.SubCategory) AndAlso
                                                                ((String.IsNullOrEmpty(s.Category) AndAlso String.IsNullOrEmpty(s.SysSettingID)) OrElse _
                                                                (Not String.IsNullOrEmpty(s.Category) AndAlso Not String.IsNullOrEmpty(s.SysSettingID)))).ToList()
        For Each ss As CSysSetting In saveItems
            'If oSysManager.CheckExistSysSetting(cateName, ss.SubCategory, ss.Key, ss.Orther, ss.OtherType) Then
            '    oSysManager.UpdateValue(cateName, ss.SubCategory, ss.Key, ss.Value, psOrther:=ss.Orther, psOtherType:=ss.OtherType)
            'Else
            '    oSysManager.AddSysSetting(cateName, ss.Key, ss.Value, ss.SubCategory, psOrther:=ss.Orther, psOtherType:=ss.OtherType)
            'End If
            If Not String.IsNullOrEmpty(ss.SysSettingID) Then
                oSysManager.UpdateByKey(ss.SysSettingID, ss.Key, ss.Value, ss.ItemIndex, ss.SubCategory, ss.Orther, ss.OtherType)
            Else
                oSysManager.AddSysSetting(cateName, ss.Key, ss.Value, ss.SubCategory, ss.ItemIndex, ss.Orther, ss.OtherType)
            End If
            ' clear cache
            oCache.ClearSysSettings(cateName, ss.SubCategory)
            oCache.ClearJuiceControl(UCase(agentId), UCase(ss.SubCategory), UCase(ss.Key), UCase(ss.Orther), ss.OtherType)
        Next
        success = True

        Return success
    End Function

    Private Shared Function GetSportJuiceSettings(ByVal rules As List(Of SportJuice)) As List(Of CSysSetting)
        Dim settings = New List(Of CSysSetting)

        For Each rule As SportJuice In rules
            ' GM = Current
            If rule.GM OrElse Not String.IsNullOrEmpty(rule.GmId) Then
                settings.Add(New CSysSetting With {.SysSettingID = rule.GmId,
                                                   .Category = IIf(rule.GM, rule.GmCate, String.Empty),
                                                   .Orther = rule.GameType,
                                                   .Key = "Current",
                                                   .SubCategory = rule.Sport,
                                                   .Value = SafeDouble(rule.Value),
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
                                                   .Value = SafeDouble(rule.Value),
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
                                                   .Value = SafeDouble(rule.Value),
                                                   .OtherType = SafeInteger(rule.BetType),
                                                   .ItemIndex = SafeInteger(rule.RowIndex) + 1
                                                   })
            End If

        Next

        Return settings
    End Function

#End Region


#Region "Get Sport Juice"

    Private Function GetSportJuice() As List(Of SportJuice)
        Dim oSysManager As New CSysSettingManager()
        Dim cateName = UserSession.UserID & "_Juice"
        Dim dbJuices = oSysManager.GetAllSysSettings(cateName)

        For Each j In dbJuices
            'j.SubCategory = j.SubCategory.ToUpper()
            If j.OtherType Is Nothing Then
                j.OtherType = ""
            End If
        Next

        Dim rules = New List(Of SportJuice)

        Dim groups = From j In dbJuices
                     Order By j.ItemIndex
                     Group By jkey = New With {Key .Sport = j.SubCategory, Key .GameType = j.Orther, Key .BetType = j.OtherType, Key .Value = j.Value}
                     Into juicegroup = Group


        For Each group In groups
            Dim r = New SportJuice With {.Sport = group.jkey.Sport, .GameType = SafeString(group.jkey.GameType), .Value = SafeDouble(group.jkey.Value), .BetType = SafeInteger(group.jkey.BetType)}

            ' GM = Current
            Dim current = group.juicegroup.FirstOrDefault(Function(j) j.Key.Equals("Current", StringComparison.OrdinalIgnoreCase))
            If current IsNot Nothing Then
                r.GM = True
                r.GmId = current.SysSettingID
                r.GmCate = current.Category
            End If
            ' FH = 1H
            Dim fh = group.juicegroup.FirstOrDefault(Function(j) j.Key.Equals("1H", StringComparison.OrdinalIgnoreCase))
            If fh IsNot Nothing Then
                r.FH = True
                r.FhId = fh.SysSettingID
                r.FhCate = fh.Category
            End If
            ' SH = 2H
            Dim sh = group.juicegroup.FirstOrDefault(Function(j) j.Key.Equals("2H", StringComparison.OrdinalIgnoreCase))
            If sh IsNot Nothing Then
                r.SH = True
                r.ShId = sh.SysSettingID
                r.ShCate = sh.Category
            End If

            rules.Add(r)
        Next

        Return rules

    End Function

#End Region

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

End Class
