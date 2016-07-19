Imports SBCBL.std
Imports System.DateTime
Imports WebsiteLibrary.DBUtils

Public Class CManipulationService
    Private _log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

    Public Sub ExecuteManipulation()
        '_log.Info("STARTING EXUTE MANIPULATION!")
        Dim nFixedMoney As Double = 0
        Dim nHalfFixedMoney As Double = 0
        Dim oDT As DataTable = GetGamelines(nFixedMoney, nHalfFixedMoney)
        'Dim oDTUpdate As DataTable = oDT.Clone()
        Dim nAverageHSpreadMoney As Double = 0
        Dim nAverageASpreadMoney As Double = 0
        Dim nAverageTotalPointsOver As Double = 0
        Dim nAverageTotalPointsUnder As Double = 0
        Dim nAverageHMoney As Double = 0
        Dim nAverageAMoney As Double = 0
        Dim nAverageHSpread As Double = 0
        Dim nAverageASpread As Double = 0
        Dim nTotalPoints As Double = 0

        Dim nCountSpread As Integer = 0
        Dim nCountSpreadMoney As Integer = 0
        Dim nCountTotalPointMoney As Integer = 0
        Dim nCountTeamTotalPointMoney As Integer = 0
        Dim nCountMoney As Integer = 0
        Dim nCountUpdate As Integer = 0
        Dim nCountTotalPoints As Integer = 0
        Dim nCountTeamTotalPoints As Integer = 0
        Dim nAverageAwayTeamTotalPointsOverMoney As Double = 0
        Dim nAverageAwayTeamTotalPointsUnderMoney As Double = 0
        Dim nAverageHomeTeamTotalPointsOverMoney As Double = 0
        Dim nAverageHomeTeamTotalPointsUnderMoney As Double = 0
        Dim nCountTotalPointAwayMoney As Integer = 0
        Dim nCountTotalPointHomeMoney As Integer = 0
        Dim oListID As New List(Of String)
        Dim oContexts As String() = {"Current", "1H", "2H"}
        Dim oDB As WebsiteLibrary.DBUtils.CSQLDBUtils = Nothing
        '' bUpdateInvoke using to know if oDB can call update to oDTUpdate
        Dim bUpdateInvoke As Boolean = False
        Try
            If oDT.Rows.Count = 0 Then
                Return
            End If

            For nIndex As Integer = oDT.Rows.Count - 1 To 0 Step -1
                Dim oRow As DataRow = oDT.Rows(nIndex)
                Dim sGameID As String = SafeString(oRow("gameID"))
                If oListID.Contains(sGameID) Then
                    Continue For
                End If

                oListID.Add(sGameID)

                For Each sContext As String In oContexts
                    nAverageHSpreadMoney = 0
                    nAverageASpreadMoney = 0
                    nAverageTotalPointsOver = 0
                    nAverageTotalPointsUnder = 0
                    nAverageHMoney = 0
                    nAverageAMoney = 0
                    nAverageHSpread = 0
                    nAverageASpread = 0
                    nTotalPoints = 0

                    nCountSpread = 0
                    nCountSpreadMoney = 0
                    nCountTotalPointMoney = 0
                    nCountTeamTotalPointMoney = 0
                    nCountMoney = 0
                    nCountTotalPoints = 0
                    nCountTeamTotalPoints = 0

                    nAverageAwayTeamTotalPointsOverMoney = 0
                    nAverageAwayTeamTotalPointsUnderMoney = 0
                    nAverageHomeTeamTotalPointsOverMoney = 0
                    nAverageHomeTeamTotalPointsUnderMoney = 0
                    nCountTotalPointAwayMoney = 0
                    nCountTotalPointHomeMoney = 0

                    Dim nOddMoney As Double = SafeDouble(IIf(UCase(sContext) = "CURRENT", nFixedMoney, nHalfFixedMoney))

                    oDT.DefaultView.RowFilter = "gameid=" & SQLString(sGameID) & " and Context = " & SQLString(sContext)
                    Dim oDTSingleGame As DataTable = oDT.DefaultView.ToTable()

                    If oDTSingleGame.Rows.Count <= 1 Then
                        Continue For
                    End If

                    For Each oGameRow As DataRow In oDTSingleGame.Rows
                        If SafeString(oGameRow("Bookmaker")) = "Manipulation" Then
                            Continue For
                        End If

                        If SafeDouble(oGameRow("HomeSpread")) <> 0 AndAlso SafeDouble(oGameRow("AwaySpread")) <> 0 Then
                            nAverageHSpread += SafeDouble(oGameRow("HomeSpread"))
                            nAverageASpread += SafeDouble(oGameRow("AwaySpread"))
                            nCountSpread += 1
                        End If

                        If SafeDouble(oGameRow("HomeSpreadMoney")) <> 0 AndAlso SafeDouble(oGameRow("AwaySpreadMoney")) <> 0 Then
                            nAverageHSpreadMoney += GetOddNumber(SafeDouble(oGameRow("HomeSpreadMoney")))
                            nAverageASpreadMoney += GetOddNumber(SafeDouble(oGameRow("AwaySpreadMoney")))
                            nCountSpreadMoney += 1
                        End If

                        If SafeDouble(oGameRow("TotalPointsOverMoney")) <> 0 And SafeDouble(oGameRow("TotalPointsUnderMoney")) <> 0 Then
                            nAverageTotalPointsOver += GetOddNumber(SafeDouble(oGameRow("TotalPointsOverMoney")))
                            nAverageTotalPointsUnder += GetOddNumber(SafeDouble(oGameRow("TotalPointsUnderMoney")))
                            nCountTotalPointMoney += 1
                        End If

                        If SafeDouble(oGameRow("AwayTeamTotalPointsOverMoney")) <> 0 And SafeDouble(oGameRow("AwayTeamTotalPointsUnderMoney")) <> 0 Then
                            nAverageAwayTeamTotalPointsOverMoney += GetOddNumber(SafeDouble(oGameRow("AwayTeamTotalPointsOverMoney")))
                            nAverageAwayTeamTotalPointsUnderMoney += GetOddNumber(SafeDouble(oGameRow("AwayTeamTotalPointsUnderMoney")))
                            nCountTotalPointAwayMoney += 1
                        End If

                        If SafeDouble(oGameRow("HomeTeamTotalPointsOverMoney")) <> 0 And SafeDouble(oGameRow("HomeTeamTotalPointsUnderMoney")) <> 0 Then
                            nAverageHomeTeamTotalPointsOverMoney += GetOddNumber(SafeDouble(oGameRow("HomeTeamTotalPointsOverMoney")))
                            nAverageHomeTeamTotalPointsUnderMoney += GetOddNumber(SafeDouble(oGameRow("HomeTeamTotalPointsUnderMoney")))
                            nCountTotalPointHomeMoney += 1
                        End If

                        If SafeDouble(oGameRow("AwayMoneyLine")) <> 0 AndAlso SafeDouble(oGameRow("HomeMoneyLine")) <> 0 Then
                            nAverageHMoney += GetOddNumber(SafeDouble(oGameRow("HomeMoneyLine")))
                            nAverageAMoney += GetOddNumber(SafeDouble(oGameRow("AwayMoneyLine")))
                            nCountMoney += 1
                        End If

                        If SafeDouble(oGameRow("TotalPoints")) <> 0 Then
                            nTotalPoints += SafeDouble(oGameRow("TotalPoints"))
                            nCountTotalPoints += 1
                        End If
                    Next

                    nCountUpdate += 1
                    Dim oManipulationRow As DataRow = GetManipulationRow(oDT, sGameID, sContext)

                    If nCountSpread <> 0 Then
                        oManipulationRow("AwaySpread") = RoundOddNumber(nAverageASpread / nCountSpread)
                        oManipulationRow("HomeSpread") = -RoundOddNumber(nAverageASpread / nCountSpread) ''RoundOddNumber(nAverageHSpread / nCountSpread)
                    End If

                    If nCountSpreadMoney <> 0 Then
                        If nOddMoney <> 0 Then
                            nCountSpreadMoney += 1
                            '_log.Info("Fixed Spread Odds: " & GetOddNumber(nFixedMoney).ToString())
                            nAverageHSpreadMoney += GetOddNumber(nOddMoney)
                            nAverageASpreadMoney += GetOddNumber(nOddMoney)
                        End If

                        oManipulationRow("AwaySpreadMoney") = Math.Round(GetFinalOddNumber(SafeDouble(nAverageASpreadMoney / nCountSpreadMoney)), 0)
                        oManipulationRow("HomeSpreadMoney") = Math.Round(GetFinalOddNumber(SafeDouble(nAverageHSpreadMoney / nCountSpreadMoney)), 0)
                    End If

                    If nCountTotalPointMoney <> 0 Then
                        If nOddMoney <> 0 Then
                            nCountTotalPointMoney += 1
                            nAverageTotalPointsOver += GetOddNumber(nOddMoney)
                            nAverageTotalPointsUnder += GetOddNumber(nOddMoney)
                        End If

                        oManipulationRow("TotalPointsOverMoney") = Math.Round(GetFinalOddNumber(SafeDouble(nAverageTotalPointsOver / nCountTotalPointMoney)), 0)
                        oManipulationRow("TotalPointsUnderMoney") = Math.Round(GetFinalOddNumber(SafeDouble(nAverageTotalPointsUnder / nCountTotalPointMoney)), 0)
                    End If

                    If nCountTotalPointAwayMoney <> 0 Then
                        If nOddMoney <> 0 Then
                            nCountTotalPointAwayMoney += 1
                            nAverageAwayTeamTotalPointsOverMoney += GetOddNumber(nOddMoney)
                            nAverageAwayTeamTotalPointsUnderMoney += GetOddNumber(nOddMoney)
                        End If

                        oManipulationRow("AwayTeamTotalPointsOverMoney") = Math.Round(GetFinalOddNumber(SafeDouble(nAverageAwayTeamTotalPointsOverMoney / nCountTotalPointAwayMoney)), 0)
                        oManipulationRow("AwayTeamTotalPointsUnderMoney") = Math.Round(GetFinalOddNumber(SafeDouble(nAverageAwayTeamTotalPointsUnderMoney / nCountTotalPointAwayMoney)), 0)
                    End If

                    If nCountTotalPointHomeMoney <> 0 Then
                        If nOddMoney <> 0 Then
                            nCountTotalPointHomeMoney += 1
                            nAverageHomeTeamTotalPointsOverMoney += GetOddNumber(nOddMoney)
                            nAverageHomeTeamTotalPointsUnderMoney += GetOddNumber(nOddMoney)
                        End If

                        oManipulationRow("HomeTeamTotalPointsOverMoney") = Math.Round(GetFinalOddNumber(SafeDouble(nAverageHomeTeamTotalPointsOverMoney / nCountTotalPointHomeMoney)), 0)
                        oManipulationRow("HomeTeamTotalPointsUnderMoney") = Math.Round(GetFinalOddNumber(SafeDouble(nAverageHomeTeamTotalPointsUnderMoney / nCountTotalPointHomeMoney)), 0)
                    End If


                    If nCountMoney <> 0 Then
                        oManipulationRow("AwayMoneyLine") = Math.Round(GetFinalOddNumber(SafeDouble(nAverageAMoney / nCountMoney)), 0)
                        oManipulationRow("HomeMoneyLine") = Math.Round(GetFinalOddNumber(SafeDouble(nAverageHMoney / nCountMoney)), 0)
                    End If

                    If nCountTotalPoints <> 0 Then
                        oManipulationRow("TotalPoints") = RoundOddNumber(nTotalPoints / nCountTotalPoints)
                    End If
                Next
            Next

            bUpdateInvoke = True
            _log.Info(String.Format("There are {0} Manipulation lines", nCountUpdate.ToString()))
            oDB = New WebsiteLibrary.DBUtils.CSQLDBUtils(SBC_CONNECTION_STRING, "")
            If oDT.Rows.Count > 0 Then
                oDB.UpdateDataTable(oDT, "select top 0 * from Gamelines", True)
            End If
            oDB.commitTransaction()
        Catch ex As Exception
            _log.Error("Can't update Manipulation: " & ex.Message, ex)
            If bUpdateInvoke Then
                oDB.rollbackTransaction()
            End If
        Finally
            If oDB IsNot Nothing Then
                oDB.closeConnection()
            End If
        End Try
    End Sub

    Private Function RoundOddNumber(ByVal pnValue As Double) As Double
        Dim nPositive As Double = pnValue

        If pnValue < 0 Then
            nPositive = -pnValue
        End If

        Dim nDecimal As Double = nPositive - Math.Floor(nPositive)
        Dim nResult As Double = Nothing
        If nDecimal = 0 Then
            nResult = Math.Floor(nPositive)
        ElseIf nDecimal < 0.25 Then
            nResult = Math.Floor(nPositive)
            'ElseIf nDecimal < 0.5 Then
            '    nResult = Math.Floor(nPositive) + 0.5
        ElseIf nDecimal < 0.75 Then
            nResult = Math.Floor(nPositive) + 0.5
        Else
            nResult = Math.Floor(nPositive) + 1
        End If
        If pnValue < 0 Then
            nResult = -nResult
        End If
        Return nResult

    End Function

    Private Function GetOddNumber(ByVal pnValue As Double) As Double
        Dim nValue As Double = pnValue
        If pnValue >= 100 Then
            nValue = pnValue - 100
        ElseIf pnValue <= -100 Then
            nValue = pnValue - (-100)
        End If
        Return nValue
    End Function

    Private Function GetFinalOddNumber(ByVal pnValue As Double) As Double
        Dim nValue As Double = pnValue
        If pnValue >= 0 Then
            nValue = pnValue + 100
        ElseIf pnValue < 0 Then
            nValue = pnValue + (-100)
        End If
        Return nValue
    End Function

    Private Function GetManipulationRow(ByVal oDTDes As DataTable, ByVal psGameID As String, ByVal psContext As String) As DataRow
        oDTDes.DefaultView.RowFilter = "Bookmaker = 'Manipulation' and gameid=" & SQLString(psGameID) & " and Context = " & SQLString(psContext)
        Dim oDTLine As DataTable = oDTDes.DefaultView.ToTable()
        Dim oRow As DataRow = oDTDes.NewRow()
        '' set created date if this line is created by first time
        oRow("CreatedDate") = Now.ToUniversalTime()

        Dim oCreatedDateCris As DateTime = DateTime.MinValue
        oDTDes.DefaultView.RowFilter = "Bookmaker = 'Cris' and gameid=" & SQLString(psGameID) & " and Context = " & SQLString(psContext)
        If (oDTDes.DefaultView.ToTable().Rows.Count > 0) Then
            oCreatedDateCris = SafeDate(oDTDes.DefaultView.ToTable().Rows(0)("CreatedDate"))
        End If

        '' set created date if this line is created by first time
        oRow("CreatedDate") = Now.ToUniversalTime()


        If oDTLine.Rows.Count <> 0 Then
            Dim sGameLineID As String = SafeString(oDTLine.Rows(0)("GameLineID"))
            Dim sGameID As String = SafeString(oDTLine.Rows(0)("GameID"))
            For Each oLineRow As DataRow In oDTDes.Rows
                If SafeString(oLineRow("GameLineID")) = sGameLineID Then
                    oRow = oLineRow
                End If

                If oCreatedDateCris <> DateTime.MinValue AndAlso SafeString(oLineRow("GameID")) = sGameID Then
                    oLineRow("CreatedDate") = oCreatedDateCris
                End If
            Next
        Else
            oRow("GameID") = New Guid(psGameID)
            oRow("GamelineID") = newGUID()
            oRow("LastUpdated") = DateTime.Now.ToUniversalTime()
            oRow("Bookmaker") = "Manipulation"
            oRow("Context") = psContext
            oRow("FeedSource") = "SportOption"
            oDTDes.Rows.Add(oRow)
        End If
        Return oRow
    End Function

    Private Function GetGamelines(ByRef pnFixedMoney As Double, ByRef pnHalfFixedMoney As Double) As DataTable
        Dim oGameDate As DateTime = DateTime.Now.ToUniversalTime().AddHours(-5)
        If DateTime.Now.IsDaylightSavingTime() Then
            oGameDate = oGameDate.AddHours(1)
        End If

        '' game gamelines within 5 hours to compute manipulation
        oGameDate = oGameDate.AddHours(-5)

        Dim oWhere As New CSQLWhereStringBuilder()
        oWhere.AppendANDCondition("((isnull(Gamestatus,'') = '' and context <> '2H') or (context = '2H' and Gamedate> " & SQLString(oGameDate) & "))")
        oWhere.AppendANDCondition("gamedate > " & SQLString(oGameDate))
        oWhere.AppendANDCondition("IsforProp <>'Y'")
        oWhere.AppendANDCondition("GameType in  ('AFL Football','CFL Football','NCAA Football', 'NFL Football','NFL Preseason', 'NBA Basketball', 'NCAA Basketball', 'WNBA Basketball', 'WNCAA Basketball', 'NCAA Basketball', 'WNCAA Basketball')")
        'oWhere.AppendANDCondition("BookMaker in ('CRIS', 'Bet Phoenix', 'Pinnacle', 'Manipulation' )")
        oWhere.AppendANDCondition("BookMaker in ('CRIS', 'Pinnacle', 'Manipulation')")
        '' devide by a list of book maker
        ''oWhere.AppendANDCondition("BookMaker in ()")
        Dim sSQL As String = "select gamelines.* from gamelines inner join games on gamelines.gameid= games.gameid " & oWhere.SQL
        Dim oDB As New WebsiteLibrary.DBUtils.CSQLDBUtils(SBC_CONNECTION_STRING, "")
        Dim oDT As New DataTable
        Try
            _log.Info("Get games for generate Manipulation lines: " & sSQL)
            oDT = oDB.getDataTable(sSQL)
            '' Get fixed money, in order balance spead oods
            sSQL = "select * from dbo.SysSettings where category ='FixedSpreadMoney' and [key] ='SpreadMoney'"
            Dim oDTFixedMoney As DataTable = oDB.getDataTable(sSQL)
            If oDTFixedMoney.Rows.Count > 0 Then
                pnFixedMoney = SafeDouble(oDTFixedMoney.Rows(0)("Value"))
            Else
                pnFixedMoney = 0
            End If

            sSQL = "select * from dbo.SysSettings where category ='FixedSpreadMoney' and [key] ='HalfSpreadMoney'"
            oDTFixedMoney = oDB.getDataTable(sSQL)
            If oDTFixedMoney.Rows.Count > 0 Then
                pnHalfFixedMoney = SafeDouble(oDTFixedMoney.Rows(0)("Value"))
            Else
                pnHalfFixedMoney = 0
            End If
        Catch ex As Exception
            _log.Error("Can't get Games: " & ex.Message, ex)
        Finally
            oDB.closeConnection()
        End Try
        Return oDT
    End Function





    'Private Function GetGamelines(ByRef pnFixedMoney As Double, ByRef pnHalfFixedMoney As Double) As DataTable
    '    Dim oGameDate As DateTime = DateTime.Now.ToUniversalTime().AddHours(-5)
    '    If DateTime.Now.IsDaylightSavingTime() Then
    '        oGameDate = oGameDate.AddHours(1)
    '    End If

    '    '' game gamelines within 5 hours to compute manipulation
    '    oGameDate = oGameDate.AddHours(-5)

    '    Dim oWhere As New CSQLWhereStringBuilder()
    '    oWhere.AppendANDCondition("((isnull(Gamestatus,'') = '' and context <> '2H') or (context = '2H' and Gamedate> " & SQLString(oGameDate) & "))")
    '    oWhere.AppendANDCondition("gamedate > " & SQLString(oGameDate))
    '    oWhere.AppendANDCondition("IsforProp <>'Y'")
    '    oWhere.AppendANDCondition("GameType in  ('AFL Football','CFL Football','NCAA Football', 'NFL Football','NFL Preseason', 'NBA Basketball', 'NCAA Basketball', 'WNBA Basketball', 'WNCAA Basketball', 'NCAA Basketball', 'WNCAA Basketball')")
    '    'oWhere.AppendANDCondition("BookMaker in ('CRIS', 'Bet Phoenix', 'Pinnacle', 'Manipulation' )")
    '    oWhere.AppendANDCondition("BookMaker in ('CRIS', '5Dimes', 'Pinnacle', 'Manipulation')")
    '    '' devide by a list of book maker
    '    ''oWhere.AppendANDCondition("BookMaker in ()")
    '    Dim sSQL As String = "select gamelines.* from gamelines inner join games on gamelines.gameid= games.gameid " & oWhere.SQL
    '    Dim oDB As New WebsiteLibrary.DBUtils.CSQLDBUtils(SBC_CONNECTION_STRING, "")
    '    Dim oDT As New DataTable
    '    Try
    '        _log.Info("Get games for generate Manipulation lines: " & sSQL)
    '        oDT = oDB.getDataTable(sSQL)
    '        '' Get fixed money, in order balance spead oods
    '        sSQL = "select * from dbo.SysSettings where category ='FixedSpreadMoney' and [key] ='SpreadMoney'"
    '        Dim oDTFixedMoney As DataTable = oDB.getDataTable(sSQL)
    '        If oDTFixedMoney.Rows.Count > 0 Then
    '            pnFixedMoney = SafeDouble(oDTFixedMoney.Rows(0)("Value"))
    '        Else
    '            pnFixedMoney = 0
    '        End If

    '        sSQL = "select * from dbo.SysSettings where category ='FixedSpreadMoney' and [key] ='HalfSpreadMoney'"
    '        oDTFixedMoney = oDB.getDataTable(sSQL)
    '        If oDTFixedMoney.Rows.Count > 0 Then
    '            pnHalfFixedMoney = SafeDouble(oDTFixedMoney.Rows(0)("Value"))
    '        Else
    '            pnHalfFixedMoney = 0
    '        End If
    '    Catch ex As Exception
    '        _log.Error("Can't get Games: " & ex.Message, ex)
    '    Finally
    '        oDB.closeConnection()
    '    End Try
    '    Return oDT
    'End Function
End Class
