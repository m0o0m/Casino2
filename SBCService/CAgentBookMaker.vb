Imports SBCBL.Managers
Imports SBCBL.std
Imports System.DateTime
Imports WebsiteLibrary.DBUtils
Imports SBCService.CServiceStd

Public Class CAgentBookMaker
    Private _log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

    Public Sub UpdateBookMaker(ByVal psBookMaker As String)

        '_log.Info("STARTING EXUTE MANIPULATION!")

        Dim nFixedMoney As Double = 0
        Dim nHalfFixedMoney As Double = 0
        Dim nTeamTotalFixedMoney As Double = -115
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
        Dim nCountMoney As Integer = 0
        Dim nCountUpdate As Integer = 0
        Dim nCountTotalPoints As Integer = 0

        Dim nHomeTeamTotalPoints As Double = 0
        Dim nAwayTeamTotalPoints As Double = 0
        Dim nAverageAwayTeamTotalPointsOverMoney As Double = 0
        Dim nAverageAwayTeamTotalPointsUnderMoney As Double = 0
        Dim nAverageHomeTeamTotalPointsOverMoney As Double = 0
        Dim nAverageHomeTeamTotalPointsUnderMoney As Double = 0
        Dim nCountTotalPointAwayMoney As Integer = 0
        Dim nCountTotalPointHomeMoney As Integer = 0

        Dim oListID As New List(Of String)
        Dim oContexts As String() = {"Current", "1H", "2H", "1Q", "2Q", "3Q", "4Q"}
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
                    nHomeTeamTotalPoints = 0
                    nAwayTeamTotalPoints = 0
                    nCountSpread = 0
                    nCountSpreadMoney = 0
                    nCountTotalPointMoney = 0
                    nCountMoney = 0
                    nCountTotalPoints = 0
                    Dim nOddMoney As Double = SafeDouble(IIf(UCase(sContext) = "CURRENT", nFixedMoney, nHalfFixedMoney))

                    oDT.DefaultView.RowFilter = "gameid=" & SQLString(sGameID) & " and Context = " & SQLString(sContext)
                    Dim oDTSingleGame As DataTable = oDT.DefaultView.ToTable()

                    For Each oGameRow As DataRow In oDTSingleGame.Rows
                        If SafeString(oGameRow("Bookmaker")) = psBookMaker OrElse SafeString(oGameRow("Bookmaker")) = "Vegas1" Then
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

                        ''team total start = beacause bookmaker 5Dimes for all in team total(change 5Dimes to CRIS )
                        If sContext.Equals("Current", StringComparison.CurrentCultureIgnoreCase) Then
                            If SafeDouble(oGameRow("AwayTeamTotalPointsOverMoney")) <> 0 And SafeDouble(oGameRow("AwayTeamTotalPointsUnderMoney")) <> 0 Then
                                nAverageAwayTeamTotalPointsOverMoney = GetOddNumber(SafeDouble(oGameRow("AwayTeamTotalPointsOverMoney")))
                                nAverageAwayTeamTotalPointsUnderMoney = GetOddNumber(SafeDouble(oGameRow("AwayTeamTotalPointsUnderMoney")))
                                nCountTotalPointAwayMoney = 1
                                nAwayTeamTotalPoints = SafeDouble(oGameRow("AwayTeamTotalPoints"))
                            End If

                            If SafeDouble(oGameRow("HomeTeamTotalPointsOverMoney")) <> 0 And SafeDouble(oGameRow("HomeTeamTotalPointsUnderMoney")) <> 0 Then
                                nAverageHomeTeamTotalPointsOverMoney = GetOddNumber(SafeDouble(oGameRow("HomeTeamTotalPointsOverMoney")))
                                nAverageHomeTeamTotalPointsUnderMoney = GetOddNumber(SafeDouble(oGameRow("HomeTeamTotalPointsUnderMoney")))
                                nCountTotalPointHomeMoney = 1
                                nHomeTeamTotalPoints = SafeDouble(oGameRow("HomeTeamTotalPoints"))
                            End If
                        End If

                        '' team total end


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
                    Dim oBookMakerRow As DataRow = GetBookMakerRow(oDT, sGameID, sContext, psBookMaker)
                    Dim oVegasRow As DataRow = GetBookMakerRow(oDT, sGameID, sContext, "Vegas1")

                    If nCountSpread <> 0 Then
                        oBookMakerRow("AwaySpread") = RoundOddNumber(nAverageASpread / nCountSpread)
                        oBookMakerRow("HomeSpread") = -RoundOddNumber(nAverageASpread / nCountSpread) ''RoundOddNumber(nAverageHSpread / nCountSpread)

                        oVegasRow("AwaySpread") = RoundOddNumber(nAverageASpread / nCountSpread)
                        oVegasRow("HomeSpread") = -RoundOddNumber(nAverageASpread / nCountSpread) ''RoundOddNumber(nAverageHSpread / nCountSpread)
                    End If

                    If nCountSpreadMoney <> 0 Then
                        If nOddMoney <> 0 Then
                            nCountSpreadMoney += 1
                            '_log.Info("Fixed Spread Odds: " & GetOddNumber(nFixedMoney).ToString())
                            nAverageHSpreadMoney += GetOddNumber(nOddMoney)
                            nAverageASpreadMoney += GetOddNumber(nOddMoney)
                        End If

                        oBookMakerRow("AwaySpreadMoney") = Math.Round(GetFinalOddNumber(SafeDouble(nAverageASpreadMoney / nCountSpreadMoney)), 0)
                        oBookMakerRow("HomeSpreadMoney") = Math.Round(GetFinalOddNumber(SafeDouble(nAverageHSpreadMoney / nCountSpreadMoney)), 0)

                        oVegasRow("AwaySpreadMoney") = VegasOdds(Math.Round(GetFinalOddNumber(SafeDouble(nAverageASpreadMoney / nCountSpreadMoney)), 0))
                        oVegasRow("HomeSpreadMoney") = VegasOdds(Math.Round(GetFinalOddNumber(SafeDouble(nAverageHSpreadMoney / nCountSpreadMoney)), 0))
                    End If

                    If nCountTotalPointMoney <> 0 Then
                        If nOddMoney <> 0 Then
                            nCountTotalPointMoney += 1
                            nAverageTotalPointsOver += GetOddNumber(nOddMoney)
                            nAverageTotalPointsUnder += GetOddNumber(nOddMoney)
                        End If

                        oBookMakerRow("TotalPointsOverMoney") = Math.Round(GetFinalOddNumber(SafeDouble(nAverageTotalPointsOver / nCountTotalPointMoney)), 0)
                        oBookMakerRow("TotalPointsUnderMoney") = Math.Round(GetFinalOddNumber(SafeDouble(nAverageTotalPointsUnder / nCountTotalPointMoney)), 0)

                        oVegasRow("TotalPointsOverMoney") = VegasOdds(Math.Round(GetFinalOddNumber(SafeDouble(nAverageTotalPointsOver / nCountTotalPointMoney)), 0))
                        oVegasRow("TotalPointsUnderMoney") = VegasOdds(Math.Round(GetFinalOddNumber(SafeDouble(nAverageTotalPointsUnder / nCountTotalPointMoney)), 0))
                    End If

                    If nCountTotalPointAwayMoney <> 0 And sContext.Equals("Current", StringComparison.CurrentCultureIgnoreCase) Then
                        If nTeamTotalFixedMoney <> 0 Then
                            nAverageAwayTeamTotalPointsOverMoney += GetOddNumber(nTeamTotalFixedMoney)
                            nAverageAwayTeamTotalPointsUnderMoney += GetOddNumber(nTeamTotalFixedMoney)
                        End If

                        oBookMakerRow("AwayTeamTotalPointsOverMoney") = Math.Round(GetFinalOddNumber(SafeDouble(nAverageAwayTeamTotalPointsOverMoney / 2)), 0)
                        oBookMakerRow("AwayTeamTotalPointsUnderMoney") = Math.Round(GetFinalOddNumber(SafeDouble(nAverageAwayTeamTotalPointsUnderMoney / 2)), 0)
                        oBookMakerRow("AwayTeamTotalPoints") = nAwayTeamTotalPoints

                        oVegasRow("AwayTeamTotalPointsOverMoney") = VegasOdds(Math.Round(GetFinalOddNumber(SafeDouble(nAverageAwayTeamTotalPointsOverMoney / 2)), 0))
                        oVegasRow("AwayTeamTotalPointsUnderMoney") = VegasOdds(Math.Round(GetFinalOddNumber(SafeDouble(nAverageAwayTeamTotalPointsUnderMoney / 2)), 0))
                        oVegasRow("AwayTeamTotalPoints") = nAwayTeamTotalPoints
                    End If

                    If nCountTotalPointHomeMoney <> 0 And sContext.Equals("Current", StringComparison.CurrentCultureIgnoreCase) Then
                        If nTeamTotalFixedMoney <> 0 Then
                            nAverageHomeTeamTotalPointsOverMoney += GetOddNumber(nTeamTotalFixedMoney)
                            nAverageHomeTeamTotalPointsUnderMoney += GetOddNumber(nTeamTotalFixedMoney)
                        End If

                        oBookMakerRow("HomeTeamTotalPointsOverMoney") = Math.Round(GetFinalOddNumber(SafeDouble(nAverageHomeTeamTotalPointsOverMoney / 2)), 0)
                        oBookMakerRow("HomeTeamTotalPointsUnderMoney") = Math.Round(GetFinalOddNumber(SafeDouble(nAverageHomeTeamTotalPointsUnderMoney / 2)), 0)
                        oBookMakerRow("HomeTeamTotalPoints") = nHomeTeamTotalPoints

                        oVegasRow("HomeTeamTotalPointsOverMoney") = VegasOdds(Math.Round(GetFinalOddNumber(SafeDouble(nAverageHomeTeamTotalPointsOverMoney / 2)), 0))
                        oVegasRow("HomeTeamTotalPointsUnderMoney") = VegasOdds(Math.Round(GetFinalOddNumber(SafeDouble(nAverageHomeTeamTotalPointsUnderMoney / 2)), 0))
                        oVegasRow("HomeTeamTotalPoints") = nHomeTeamTotalPoints
                    End If

                    If nCountMoney <> 0 Then
                        oBookMakerRow("AwayMoneyLine") = Math.Round(GetFinalOddNumber(SafeDouble(nAverageAMoney / nCountMoney)), 0)
                        oBookMakerRow("HomeMoneyLine") = Math.Round(GetFinalOddNumber(SafeDouble(nAverageHMoney / nCountMoney)), 0)

                        oVegasRow("AwayMoneyLine") = VegasOdds(Math.Round(GetFinalOddNumber(SafeDouble(nAverageAMoney / nCountMoney)), 0))
                        oVegasRow("HomeMoneyLine") = VegasOdds(Math.Round(GetFinalOddNumber(SafeDouble(nAverageHMoney / nCountMoney)), 0))
                    End If

                    If nCountTotalPoints <> 0 Then
                        oBookMakerRow("TotalPoints") = RoundOddNumber(nTotalPoints / nCountTotalPoints)

                        oVegasRow("TotalPoints") = RoundOddNumber(nTotalPoints / nCountTotalPoints)
                    End If
                Next
            Next

            bUpdateInvoke = True
            _log.Info(String.Format("There are {0} BookMaker lines", nCountUpdate.ToString()))
            oDB = New WebsiteLibrary.DBUtils.CSQLDBUtils(SBC_CONNECTION_STRING, "")
            If oDT.Rows.Count > 0 Then
                oDB.UpdateDataTable(oDT, "select top 0 * from Gamelines", True)
            End If
            oDB.commitTransaction()
        Catch ex As Exception
            _log.Error("Can't update psBookMaker: " & ex.Message, ex)
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

    Private Function GetBookMakerRow(ByVal oDTDes As DataTable, ByVal psGameID As String, ByVal psContext As String, ByVal psBookMaker As String) As DataRow
        oDTDes.DefaultView.RowFilter = "Bookmaker = '" & psBookMaker & "' and gameid=" & SQLString(psGameID) & " and Context = " & SQLString(psContext)
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
            oRow("Bookmaker") = psBookMaker
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
        oWhere.AppendANDCondition("ISNULL(Gamestatus,'') NOT IN ('Final','CANCELLED') ")
        oWhere.AppendANDCondition("gamedate > " & SQLString(oGameDate))
        oWhere.AppendANDCondition("IsforProp <>'Y'")
        oWhere.AppendANDCondition("GameType in  ('AFL Football','CFL Football','NCAA Football', 'NFL Football','NFL Preseason', 'NBA Basketball', 'NCAA Basketball', 'WNBA Basketball', 'WNCAA Basketball', 'NCAA Basketball', 'WNCAA Basketball')")
        oWhere.AppendANDCondition("BookMaker in ('CRIS','Pinnacle', 'SBS', 'Vegas1')")
        '' devide by a list of book maker
        ''oWhere.AppendANDCondition("BookMaker in ()")
        Dim sSQL As String = "select gamelines.* from gamelines inner join games on gamelines.gameid= games.gameid " & oWhere.SQL
        Dim oDB As New WebsiteLibrary.DBUtils.CSQLDBUtils(SBC_CONNECTION_STRING, "")
        Dim oDT As New DataTable
        Try
            _log.Info("Get games for generate SBS lines: " & sSQL)
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
End Class

