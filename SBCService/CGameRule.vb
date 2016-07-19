Imports System.Collections.Generic
Imports SBCService.CServiceStd
Imports WebsiteLibrary.CSBCStd
Imports WebsiteLibrary.DBUtils
Imports SBCBL.Managers
Imports SBCBL.CacheUtils
Imports SBCBL.std

Public Class CGameRule
    Public Shared Sub ProcessGameLine(ByVal psGameType As String, ByVal poGameLine As CLine)
        Dim _SoccerGameType As New List(Of String)(New String() {"Bundesliga", "Brazil", "Bundesliga", "Euro Cups", "La Liga", "Ligue 1", "Mexican", _
                                                                 "MLS", "Netherlands", "Portugal", "Premier", "Scotland", "Serie A", "Super Liga", "Euro", _
                                                                 "Champions League", "Europa League", "Copa America", "Carling Cup", "FA Cup", "Concacaf"})
        If _SoccerGameType.Contains(psGameType) Then
            If poGameLine.AwaySpreadMoney >= 130 Then
                poGameLine.AwaySpreadMoney -= 10
                poGameLine.HomeSpreadMoney -= 10
            End If
            If poGameLine.TotalPointsOverMoney >= 130 Then
                poGameLine.TotalPointsOverMoney -= 10
                poGameLine.TotalPointsUnderMoney -= 10
            End If
        End If

        '' -5% juice for TeamTotal and quarter lines
        poGameLine.HomeTeamTotalPointsUnderMoney = Reduce5Percentage(poGameLine.HomeTeamTotalPointsUnderMoney)
        poGameLine.HomeTeamTotalPointsOverMoney = Reduce5Percentage(poGameLine.HomeTeamTotalPointsOverMoney)
        poGameLine.AwayTeamTotalPointsUnderMoney = Reduce5Percentage(poGameLine.AwayTeamTotalPointsUnderMoney)
        poGameLine.AwayTeamTotalPointsOverMoney = Reduce5Percentage(poGameLine.AwayTeamTotalPointsOverMoney)

        If poGameLine.Context = "1Q" OrElse poGameLine.Context = "2Q" OrElse poGameLine.Context = "3Q" OrElse poGameLine.Context = "4Q" Then
            poGameLine.HomeSpreadMoney = Reduce5Percentage(poGameLine.HomeSpreadMoney)
            poGameLine.AwaySpreadMoney = Reduce5Percentage(poGameLine.AwaySpreadMoney)
            poGameLine.HomeMoneyLine = Reduce5Percentage(poGameLine.HomeMoneyLine)
            poGameLine.AwayMoneyLine = Reduce5Percentage(poGameLine.AwayMoneyLine)
            poGameLine.TotalPointsOverMoney = Reduce5Percentage(poGameLine.TotalPointsOverMoney)
            poGameLine.TotalPointsUnderMoney = Reduce5Percentage(poGameLine.TotalPointsUnderMoney)
            If poGameLine.DrawMoneyLine <> -1 Then
                poGameLine.DrawMoneyLine = Reduce5Percentage(poGameLine.DrawMoneyLine)
            End If
            poGameLine.PropMoneyLine = Reduce5Percentage(poGameLine.PropMoneyLine)
        End If
        FilterGameLineByPreset(psGameType, poGameLine)
    End Sub

    Public Shared Sub LockDiv2Game(ByVal psGameID As String, ByVal poDB As WebsiteLibrary.DBUtils.CSQLDBUtils)
        Dim olog As log4net.ILog = log4net.LogManager.GetLogger(GetType(CGameRule))
        Dim sSQL As String = "update Games set IsGameBetLocked = 'Y',IsFirstHalfLocked = 'Y', IsSecondHalfLocked = 'Y' where " & _
                             " GameID = " & SQLString(psGameID) & " and (IsGameBetLocked is null and IsFirstHalfLocked is null and IsSecondHalfLocked is null)"
        Try
            poDB.executeNonQuery(sSQL)
        Catch ex As Exception
            olog.Error("Cant lock Div2 Game: " & ex.Message, ex)
        End Try
    End Sub

    Public Shared Sub CheckWarningGameLine(ByVal psGameType As String, ByVal poLine As CLine, ByVal pnOldAwaySpread As Double, ByVal pnOldHomeSpread As Double, _
                                           ByVal pnOldTotalPoints As Double, ByVal psOldWarn As String, ByVal poSQL As ISQLEditStringBuilder)
        Dim oListSoccer As New List(Of String)(New String() {"Argentina", "Brazil", "Bundesliga", "Euro Cups", "La Liga", "Ligue 1", "Mexican", _
            "MLS", "Netherlands", "Portugal", "Premier", "Scotland", "Serie A", "Soccer", "Super Liga", "World Cup", "Euro", "Champions League", "Europa League", "Copa America", "Carling Cup", "FA Cup", "Concacaf"})

        If oListSoccer.Contains(psGameType) AndAlso (poLine.Bookmaker = "Pinnacle" OrElse poLine.Bookmaker = "Bet Phoenix") Then
            Dim bWarn As Boolean = False
            Dim sWarn As String = ""
            Dim oListWarn As New List(Of String)
            If psOldWarn <> "" Then
                oListWarn.Add(psOldWarn)
            End If

            If Math.Abs(pnOldAwaySpread - poLine.AwaySpread) >= 0.5 Then
                bWarn = True
                oListWarn.Add(String.Format("AwaySpread changed from {0} to {1}.", SafeString(pnOldAwaySpread), SafeString(poLine.AwaySpread)))
            End If

            If Math.Abs(pnOldHomeSpread - poLine.HomeSpread) >= 0.5 Then
                bWarn = True
                oListWarn.Add(String.Format("HomeSpread changed from {0} to {1}.", SafeString(pnOldHomeSpread), SafeString(poLine.HomeSpread)))
            End If

            If Math.Abs(pnOldTotalPoints - poLine.TotalPoints) >= 0.5 Then
                bWarn = True
                oListWarn.Add(String.Format("TotalPoints changed from {0} to {1}.", SafeString(pnOldTotalPoints), SafeString(poLine.TotalPoints)))
            End If

            If bWarn Then
                poSQL.AppendString("IsWarn", SQLString("Y"))
                'poSQL.AppendString("WarnReason", "SUBSTRING(WarnReason + " & SQLString(String.Join(". <br/>", oListWarn.ToArray())) & ",0,200)")
                Dim sReason As String = String.Join("<br/>", oListWarn.ToArray())
                If sReason.Length > 200 Then
                    sReason = sReason.Substring(0, 200)
                End If

                poSQL.AppendString("WarnReason", SQLString(sReason))
            End If
        End If
    End Sub

    Private Shared _ListSettings As CSysSettingList = Nothing
    Private Shared _GetSettingDate As DateTime = DateTime.MinValue

    Private Shared ReadOnly Property PresetSettings() As CSysSettingList
        Get
            Dim sCategory As String = "SBC LineRules"
            If _ListSettings Is Nothing OrElse DateTime.Now.Subtract(_GetSettingDate).TotalHours > 5 Then
                _ListSettings = (New CSysSettingManager).GetAllSysSettings(sCategory)
                _GetSettingDate = DateTime.Now
            End If

            Return _ListSettings
        End Get
    End Property

    Private Shared _ListGameTypes As CSysSettingList = Nothing
    Private Shared _GetGameTypeDate As DateTime = DateTime.MinValue

    Private Shared ReadOnly Property GameTypes() As CSysSettingList
        Get
            Dim sGameTypeCat As String = "SBC GameType"
            If _ListGameTypes Is Nothing OrElse DateTime.Now.Subtract(_GetGameTypeDate).TotalHours > 5 Then
                _ListGameTypes = (New CSysSettingManager).GetAllSysSettings(sGameTypeCat)
                _GetGameTypeDate = DateTime.Now
            End If
            Return _ListGameTypes
        End Get
    End Property


    Private Shared _ListOffTimeCategory As CSysSettingList = Nothing
    Private Shared _GetOffTimeCategory As DateTime = DateTime.MinValue

    Private Shared ReadOnly Property CategoryOffTimes() As CSysSettingList
        Get
            Dim sOffTimeCategory As String = SBCBL.std.GetSiteType & " LineOffHour"
            If _ListOffTimeCategory Is Nothing OrElse DateTime.Now.Subtract(_GetOffTimeCategory).TotalHours > 5 Then
                _ListOffTimeCategory = (New CSysSettingManager).GetAllSysSettings(sOffTimeCategory)
                _GetOffTimeCategory = DateTime.Now
            End If
            Return _ListOffTimeCategory
        End Get
    End Property

    Public Shared Sub FilterGameLineByPreset(ByVal psGameType As String, ByVal poLine As CLine)
        Dim oLog As log4net.ILog = log4net.LogManager.GetLogger(GetType(CGameRule))
        Try
            Dim oSysManager As New CSysSettingManager()
            Dim oCachemanager As New CCacheManager()
            Dim oListWhere As New List(Of String)
            Dim bIsLockedByPreset As Boolean = False
            Dim oSysGame As CSysSetting = GameTypes.Find(Function(x) x.Key = psGameType AndAlso x.SubCategory <> "")
            If oSysGame Is Nothing Then
                Return
            End If

            Dim oSysSport As CSysSetting = GameTypes.Find(Function(x) x.SysSettingID = oSysGame.SubCategory)

            If oSysSport Is Nothing Then
                Return
            End If

            Dim sGameTypeKey As String = oSysSport.Key.Replace(" ", "_")
            Dim sSubCategory As String = sGameTypeKey & "_" & poLine.Context
            Dim oListSettings As List(Of CSysSetting) = PresetSettings.FindAll(Function(x) x.SubCategory = sSubCategory)

            If oListSettings Is Nothing OrElse oListSettings.Count = 0 Then
                Return
            End If

            Dim oSpreadMoneyGT As CSysSetting = oListSettings.Find(Function(x) x.Key = "AwaySpreadMoneyGT")
            Dim nAwaySpreadMoneyGT As Double = 0
            If oSpreadMoneyGT IsNot Nothing Then
                nAwaySpreadMoneyGT = SafeDouble(oSpreadMoneyGT.Value)
            End If
            Dim oSpreadMoneyLT As CSysSetting = oListSettings.Find(Function(x) x.Key = "AwaySpreadMoneyLT")
            Dim nAwaySpreadMoneyLT As Double = 0
            If oSpreadMoneyLT IsNot Nothing Then
                nAwaySpreadMoneyLT = SafeDouble(oSpreadMoneyLT.Value)
            End If

            oSpreadMoneyGT = oListSettings.Find(Function(x) x.Key = "HomeSpreadMoneyGT")
            Dim nHomeSpreadMoneyGT As Integer = 0
            If oSpreadMoneyGT IsNot Nothing Then
                nHomeSpreadMoneyGT = SafeInteger(oSpreadMoneyGT.Value)
            End If

            oSpreadMoneyLT = oListSettings.Find(Function(x) x.Key = "HomeSpreadMoneyLT")
            Dim nHomeSpreadMoneyLT As Integer = 0
            If oSpreadMoneyLT IsNot Nothing Then
                nHomeSpreadMoneyLT = SafeInteger(oSpreadMoneyLT.Value)
            End If

            Dim oTotalPointGT As CSysSetting = oListSettings.Find(Function(x) x.Key = "TotalPointGT")
            Dim nTotalPointGT As Double = -1000
            If oTotalPointGT IsNot Nothing Then
                nTotalPointGT = SafeDouble(oTotalPointGT.Value)
            End If

            Dim oTotalPointLT As CSysSetting = oListSettings.Find(Function(x) x.Key = "TotalPointLT")
            Dim nTotalPointLT As Double = -1000
            If oTotalPointLT IsNot Nothing Then
                nTotalPointLT = SafeDouble(oTotalPointLT.Value)
            End If

            If nAwaySpreadMoneyGT <> 0 And nAwaySpreadMoneyLT <> 0 Then
                If poLine.AwaySpreadMoney < nAwaySpreadMoneyGT And poLine.AwaySpreadMoney > nAwaySpreadMoneyLT Then
                    bIsLockedByPreset = True
                End If
            End If

            If nHomeSpreadMoneyGT <> 0 And nHomeSpreadMoneyLT <> 0 Then
                If poLine.HomeSpreadMoney < nHomeSpreadMoneyGT And poLine.HomeSpreadMoney > nHomeSpreadMoneyLT Then
                    bIsLockedByPreset = True
                End If
            End If

            If nTotalPointGT <> -1000 And (nTotalPointLT <> -1000) Then
                If poLine.TotalPoints < nTotalPointGT And poLine.TotalPoints > nTotalPointLT Then
                    bIsLockedByPreset = True
                End If
            End If
            If (bIsLockedByPreset) Then
                oLog.Info("Lock by preset gameLineID: " & poLine.GameLineID.ToString())
            End If

            poLine.IsLockedByPreset = bIsLockedByPreset
        Catch ex As Exception
            oLog.Error("Cant do game filter: " & ex.Message, ex)
        End Try
    End Sub
End Class
