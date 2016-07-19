Imports System.Xml
Imports System.Web
Imports SBCBL.std
Imports SBCBL.Managers
Imports WebsiteLibrary.CXMLUtils
Imports SBCBL.CacheUtils

Namespace Managers
    Public Class CGameTypeOnOffManager
        Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())
        Private Const FIRSTNODE As Integer = 0
        Private GAMETYPE As String = "GameType"
        Private OFFBEFORE As String = "OffBefore"
        Private DISPLAYBEFORE As String = "DisplayBefore"
        Private FILESYSTEM As String = SBS_SYSTEM
        Private SECONDHGAME As String = "SecondHGame"
        Private FILE_NAME As String = "AgentSettingGame.xml"
        Private FIX_SPREAD_MONEY As String = "FixedSpreadMoney"
        Private NUM_FIX_SPREAD_MONEY As String = "NumFixedSpreadMoneyCurrent"
        Private NUM_FIX_SPREAD_MONEYH As String = "NumFixedSpreadMoneyH"
        Private NUM_FIX_SPREAD_MONEY_GAMETOTAL As String = "NumFixedSpreadMoneyGameTotal"
        Private NUM_FLAT_SPREAD_MONEY As String = "NumFlatSpreadMoney"
        Private USE_SPREAD_MONEY As String = "UseFixedSpreadMoney"
        Private USE_FLAT_SPREAD_MONEY As String = "UseFlatSpreadMoney"
        Private USE_DEFAULT_BOOKMAKER As String = "UseDefaultBookMaker"
        Private GAMECIRCLE As String = "GameCircle"
        Private MAXTOTALBET As String = "MaxTotalBet"
        Private PARLAYREVERSE As String = "ParlayReverse"
        Private STRAIGHT As String = "Straight"
        Private ODDRULE As String = "OddRule"
        Private CONDITION As String = "Condition"
        Private GREATERTHAN As String = "GreaterThan"
        Private LESSTHAN As String = "LessThan"
        Private INCREASE As String = "Increase"
        Private LOCKGAME As String = "LockGame"
        Private SPORTTYPE As String = "SportType"
        Private BETTINGPERIOD As String = "BettingPeriod"
        Private AWAYSPREADMONEYGT As String = "AwaySpreadMoneyGT"
        Private AWAYSPEADMONEYLT As String = "AwaySpreadMoneyLT"
        Private HOMESPEADMONEYGT As String = "HomeSpreadMoneyGT"
        Private HOMESPEADMONEYLT As String = "HomeSpreadMoneyLT"
        Private TOTALPOINTGT As String = "TotalPointGT"
        Private TOTALPOINTLT As String = "TotalPointLT"
        Private PARPLAY_MAX_SELECTION As String = "ParplayMaxSelection"
        Private SECONDHOFF As String = "SecondHOff"
        Private TEAMTOTALOFF As String = "TeamTotalOff"
        Private FIRSTHOFF As String = "FirstHOff"
        Private GAMEHALFTIMEDISPLAY As String = "GameHalfTimeDisplay"
        Private GAMEHALFTIMEOFF As String = "GameHalfTimeOff"
        Private GAMETEAMDISPLAY As String = "GameQuarterDisPlay"
        Private GAMETIMEOFFLINE As String = "GameTimeOffLine"
        Private GAMETIMEDISPLAY As String = "GameTimeDisplay"
        Private GAMETOTALPOINTSDISPLAY As String = "GameTotalPointsDisplay"
        Private GAMEQUARTEROFF As String = "GameQuarterOff"
        Private CONTEXT As String = "Context"
        Private OFFLINE As String = "OffLine"
        Private JUICECONTROL As String = "JuiceControl"
        Private JUICEVALUE As String = "JuiceValue"

        Public Function GetALLGameTypeOnOffByAgentID(ByVal poAgentID As String, ByVal bCurrent As Boolean) As DataTable
            Dim sXMLPath As String
            If bCurrent Then
                sXMLPath = "root/CurrentGame"
            Else
                sXMLPath = "root/FirstHalf"
            End If

            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim odtGameTypeOnOff As DataTable = New DataTable()
            odtGameTypeOnOff.Columns.Add("ID")
            odtGameTypeOnOff.Columns.Add(GAMETYPE)
            odtGameTypeOnOff.Columns.Add(OFFBEFORE)
            odtGameTypeOnOff.Columns.Add(DISPLAYBEFORE)
            odtGameTypeOnOff.Columns.Add(GAMETOTALPOINTSDISPLAY)

            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            Try
                If Not oHandle Is Nothing OrElse oDoc.SelectNodes(sXMLPath).Count > 0 Then
                    oDoc.Load(oHandle.LocalFileName)
                    Dim listGameTypeOnOff As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                    For Each oGameTypeOnOff As XmlNode In listGameTypeOnOff
                        For Each xmlNodeGameTypeOnOff As XmlNode In oGameTypeOnOff.ChildNodes
                            Dim drGameTypeOnOff As DataRow = odtGameTypeOnOff.NewRow()
                            drGameTypeOnOff("ID") = SafeString(xmlNodeGameTypeOnOff.Attributes("ID").Value)
                            drGameTypeOnOff(GAMETYPE) = SafeString(xmlNodeGameTypeOnOff.Attributes(GAMETYPE).Value)
                            drGameTypeOnOff(OFFBEFORE) = SafeString(xmlNodeGameTypeOnOff.Attributes(OFFBEFORE).Value)
                            drGameTypeOnOff(DISPLAYBEFORE) = SafeString(xmlNodeGameTypeOnOff.Attributes(DISPLAYBEFORE).Value)
                            If xmlNodeGameTypeOnOff.Attributes.ItemOf(GAMETOTALPOINTSDISPLAY) Is Nothing Then
                                drGameTypeOnOff(GAMETOTALPOINTSDISPLAY) = ""
                            Else
                                drGameTypeOnOff(GAMETOTALPOINTSDISPLAY) = SafeString(xmlNodeGameTypeOnOff.Attributes(GAMETOTALPOINTSDISPLAY).Value)
                            End If
                            odtGameTypeOnOff.Rows.Add(drGameTypeOnOff)
                        Next
                    Next
                    Return odtGameTypeOnOff
                End If
            Catch ex As Exception
            End Try
            Return odtGameTypeOnOff
        End Function

        Public Function GetALLOddRuleByAgentID(ByVal poAgentID As String) As DataTable
            Dim sXMLPath As String = "root/OddRule"
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim odtOddRule As DataTable = New DataTable()
            odtOddRule.Columns.Add("ID")
            odtOddRule.Columns.Add(GREATERTHAN)
            odtOddRule.Columns.Add(LESSTHAN)
            odtOddRule.Columns.Add(INCREASE)
            odtOddRule.Columns.Add(LOCKGAME)
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            Try
                If Not oHandle Is Nothing OrElse oDoc.SelectNodes(sXMLPath).Count > 0 Then
                    oDoc.Load(oHandle.LocalFileName)
                    Dim listGameTypeOnOff As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                    For Each oGameTypeOnOff As XmlNode In listGameTypeOnOff
                        For Each xmlNodeGameTypeOnOff As XmlNode In oGameTypeOnOff.ChildNodes
                            Dim odrOddRule As DataRow = odtOddRule.NewRow()
                            odrOddRule("ID") = SafeString(xmlNodeGameTypeOnOff.Attributes("ID").Value)
                            odrOddRule(GREATERTHAN) = SafeString(xmlNodeGameTypeOnOff.Attributes(GREATERTHAN).Value)
                            odrOddRule(LESSTHAN) = SafeString(xmlNodeGameTypeOnOff.Attributes(LESSTHAN).Value)
                            odrOddRule(INCREASE) = SafeString(xmlNodeGameTypeOnOff.Attributes(INCREASE).Value)
                            odrOddRule(LOCKGAME) = SafeString(xmlNodeGameTypeOnOff.Attributes(LOCKGAME).Value)
                            odtOddRule.Rows.Add(odrOddRule)
                        Next
                    Next
                    Return odtOddRule
                End If
            Catch ex As Exception
            End Try
            Return odtOddRule
        End Function

        Public Function InsertGameTypeOnOffByAgent(ByVal poAgentID As String, ByVal psID As String, ByVal psGameType As String, ByVal pnOffBefore As Integer, ByVal pnDisplayBefore As Integer, ByVal pnDisplayTotalPointBefore As Integer, ByVal bCurrent As Boolean) As Boolean
            Try
                Dim sPath As String
                If bCurrent Then
                    sPath = "CurrentGame"
                Else
                    sPath = "FirstHalf"
                End If
                Dim bEmptyData As Boolean = False
                Dim bCreateNewNode As Boolean = False
                Dim oDoc As New XmlDocument()
                Dim oDB As New FileDB.CFileDB()
                Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
                Dim sXMLPath As String = "root/" & sPath
                Try
                    oDoc.Load(oHandle.LocalFileName)
                    If oDoc.SelectNodes(sXMLPath).Count > 0 Then
                        bCreateNewNode = False
                    Else
                        bCreateNewNode = True
                    End If
                Catch ex As Exception
                    bEmptyData = True
                End Try
                If bCreateNewNode Then
                    oHandle = oDB.GetNewFileHandle()
                End If
                Dim oGameTypeOnOff As XmlNode = oDoc.CreateElement("GameTypeOnOff")
                Dim oGameType As XmlAttribute = oDoc.CreateAttribute(GAMETYPE)
                oGameType.Value = psGameType
                Dim oOffBefore As XmlAttribute = oDoc.CreateAttribute(OFFBEFORE)
                oOffBefore.Value = pnOffBefore.ToString
                Dim oDisplayBefore As XmlAttribute = oDoc.CreateAttribute(DISPLAYBEFORE)
                oDisplayBefore.Value = SafeString(pnDisplayBefore)

                Dim oDisplayTotalPointBefore As XmlAttribute = oDoc.CreateAttribute(GAMETOTALPOINTSDISPLAY)
                oDisplayTotalPointBefore.Value = SafeString(pnDisplayTotalPointBefore)
                oGameTypeOnOff.Attributes.Append(oGameType)
                oGameTypeOnOff.Attributes.Append(oOffBefore)
                oGameTypeOnOff.Attributes.Append(oDisplayBefore)
                oGameTypeOnOff.Attributes.Append(oDisplayTotalPointBefore)
                Dim oAgentID As XmlAttribute = oDoc.CreateAttribute("ID")
                oAgentID.Value = psID
                oGameTypeOnOff.Attributes.Append(oAgentID)
                If bEmptyData Then
                    Dim oCurrentGame As XmlNode = oDoc.CreateElement(sPath)
                    oCurrentGame.AppendChild(oGameTypeOnOff)
                    IO.File.WriteAllText(oHandle.LocalFileName, String.Format("<?xml version=""1.0"" encoding=""UTF-8""?><root>{0}</root>", oCurrentGame.OuterXml))
                    GoTo saveFile
                End If
                If bCreateNewNode Then
                    Dim oCurrentGame As XmlNode = oDoc.CreateElement(sPath)
                    oCurrentGame.AppendChild(oGameTypeOnOff)
                    oDoc.DocumentElement.AppendChild(oCurrentGame)
                    oDoc.Save(oHandle.LocalFileName)
                Else
                    oDoc.SelectSingleNode(sXMLPath).AppendChild(oGameTypeOnOff)
                    oDoc.Save(oHandle.LocalFileName)
                End If
saveFile:
                oDB.PutFile(FILESYSTEM & "\" & poAgentID, FILE_NAME, oHandle)

            Catch ex As Exception
                log.Error("Fail to insert Agent Setting Game", ex)
                Return False
            End Try

            Return True
        End Function

        Public Function Insert2HGameTypeOnOffByAgent(ByVal poAgentID As String, ByVal psID As String, ByVal pnOffBefore As Integer) As Boolean
            Try
                Dim bEmptyData As Boolean = False
                Dim bCreateNewNode As Boolean = False
                Dim oDoc As New XmlDocument()
                Dim oDB As New FileDB.CFileDB()
                Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
                Dim sXMLPath As String = "root/SecondHGame"
                Try
                    oDoc.Load(oHandle.LocalFileName)
                    If oDoc.SelectNodes(sXMLPath).Count > 0 Then
                        bCreateNewNode = False
                    Else
                        bCreateNewNode = True
                    End If

                Catch ex As Exception
                    bEmptyData = True
                End Try
                If bCreateNewNode Then
                    oHandle = oDB.GetNewFileHandle()
                End If
                Dim oGameTypeOnOff As XmlNode = oDoc.CreateElement("GameTypeOnOff")
                Dim oOffBefore As XmlAttribute = oDoc.CreateAttribute(OFFBEFORE)
                oOffBefore.Value = pnOffBefore.ToString
                oGameTypeOnOff.Attributes.Append(oOffBefore)
                Dim oAgentID As XmlAttribute = oDoc.CreateAttribute("ID")
                oAgentID.Value = psID
                oGameTypeOnOff.Attributes.Append(oAgentID)
                If bEmptyData Then
                    Dim oCurrentGame As XmlNode = oDoc.CreateElement("SecondHGame")
                    oCurrentGame.AppendChild(oGameTypeOnOff)
                    IO.File.WriteAllText(oHandle.LocalFileName, String.Format("<?xml version=""1.0"" encoding=""UTF-8""?><root>{0}</root>", oCurrentGame.OuterXml))
                    GoTo saveFile
                End If

                If bCreateNewNode Then
                    Dim oCurrentGame As XmlNode = oDoc.CreateElement("SecondHGame")
                    oCurrentGame.AppendChild(oGameTypeOnOff)
                    oDoc.DocumentElement.AppendChild(oCurrentGame)
                    oDoc.Save(oHandle.LocalFileName)
                Else
                    oDoc.SelectSingleNode("root/SecondHGame").AppendChild(oGameTypeOnOff)
                    oDoc.Save(oHandle.LocalFileName)
                End If
saveFile:
                oDB.PutFile(FILESYSTEM & "\" & poAgentID, FILE_NAME, oHandle)

            Catch ex As Exception
                log.Error("Fail to insert 2h Agent Setting Game", ex)
                Return False
            End Try

            Return True
        End Function

        Public Function InsertGameCircle(ByVal poAgentID As String, ByVal psID As String, ByVal pnStraight As Integer, ByVal pnParlayReverse As Integer) As Boolean
            Try
                Dim bEmptyData As Boolean = False
                Dim bCreateNewNode As Boolean = False
                Dim oDoc As New XmlDocument()
                Dim oDB As New FileDB.CFileDB()
                Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
                Dim sXMLPath As String = "root/GameCircle"
                Try
                    oDoc.Load(oHandle.LocalFileName)
                    If oDoc.SelectNodes(sXMLPath).Count > 0 Then
                        bCreateNewNode = False
                    Else
                        bCreateNewNode = True
                    End If

                Catch ex As Exception
                    bEmptyData = True
                End Try
                If bCreateNewNode Then
                    oHandle = oDB.GetNewFileHandle()
                End If
                Dim oMaxTotalBet As XmlNode = oDoc.CreateElement(MAXTOTALBET)
                Dim oStraight As XmlAttribute = oDoc.CreateAttribute(STRAIGHT)
                oStraight.Value = pnStraight.ToString
                oMaxTotalBet.Attributes.Append(oStraight)
                Dim oParlayReverse As XmlAttribute = oDoc.CreateAttribute(PARLAYREVERSE)
                oParlayReverse.Value = pnParlayReverse.ToString
                oMaxTotalBet.Attributes.Append(oParlayReverse)
                Dim oID As XmlAttribute = oDoc.CreateAttribute("ID")
                oID.Value = psID
                oMaxTotalBet.Attributes.Append(oID)
                Dim oGameCircle As XmlNode = oDoc.CreateElement(GAMECIRCLE)
                oGameCircle.AppendChild(oMaxTotalBet)
                If bEmptyData Then
                    IO.File.WriteAllText(oHandle.LocalFileName, String.Format("<?xml version=""1.0"" encoding=""UTF-8""?><root>{0}</root>", oGameCircle.OuterXml))
                    GoTo saveFile
                End If
                If bCreateNewNode Then
                    oDoc.DocumentElement.AppendChild(oGameCircle)
                    oDoc.Save(oHandle.LocalFileName)
                Else
                    oDoc.SelectSingleNode("root/GameCircle").AppendChild(oMaxTotalBet)
                    oDoc.Save(oHandle.LocalFileName)
                End If
saveFile:
                oDB.PutFile(FILESYSTEM & "\" & poAgentID, FILE_NAME, oHandle)

            Catch ex As Exception
                log.Error("Fail to insert Circle Agent Setting Game", ex)
                Return False
            End Try

            Return True
        End Function

        Public Function InsertOddRule(ByVal poAgentID As String, ByVal psID As String, ByVal pnGreaterThan As Integer, ByVal pnLessThan As Integer, ByVal pnIncrease As Double, ByVal pbLockGame As Boolean) As Boolean
            Try
                Dim bEmptyData As Boolean = False
                Dim bCreateNewNode As Boolean = False
                Dim oDoc As New XmlDocument()
                Dim oDB As New FileDB.CFileDB()
                Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
                Dim sXMLPath As String = "root/OddRule"
                Try
                    oDoc.Load(oHandle.LocalFileName)
                    If oDoc.SelectNodes(sXMLPath).Count > 0 Then
                        bCreateNewNode = False
                    Else
                        bCreateNewNode = True
                    End If

                Catch ex As Exception
                    bEmptyData = True
                End Try
                If bCreateNewNode Then
                    oHandle = oDB.GetNewFileHandle()
                End If
                Dim oCondition As XmlNode = oDoc.CreateElement(CONDITION)
                Dim oGreaterThan As XmlAttribute = oDoc.CreateAttribute(GREATERTHAN)
                oGreaterThan.Value = pnGreaterThan.ToString
                oCondition.Attributes.Append(oGreaterThan)
                Dim oLessThan As XmlAttribute = oDoc.CreateAttribute(LESSTHAN)
                oLessThan.Value = pnLessThan.ToString
                oCondition.Attributes.Append(oLessThan)
                Dim oInCrease As XmlAttribute = oDoc.CreateAttribute(INCREASE)
                oInCrease.Value = pnIncrease.ToString
                oCondition.Attributes.Append(oInCrease)
                Dim oLockGame As XmlAttribute = oDoc.CreateAttribute(LOCKGAME)
                oLockGame.Value = pbLockGame.ToString
                oCondition.Attributes.Append(oLockGame)
                Dim oID As XmlAttribute = oDoc.CreateAttribute("ID")
                oID.Value = psID
                oCondition.Attributes.Append(oID)
                Dim ooddRule As XmlNode = oDoc.CreateElement(ODDRULE)
                ooddRule.AppendChild(oCondition)
                If bEmptyData Then
                    IO.File.WriteAllText(oHandle.LocalFileName, String.Format("<?xml version=""1.0"" encoding=""UTF-8""?><root>{0}</root>", ooddRule.OuterXml))
                    GoTo saveFile
                End If
                If bCreateNewNode Then
                    oDoc.DocumentElement.AppendChild(ooddRule)
                    oDoc.Save(oHandle.LocalFileName)
                Else
                    oDoc.SelectSingleNode("root/OddRule").AppendChild(oCondition)
                    oDoc.Save(oHandle.LocalFileName)
                End If
saveFile:
                oDB.PutFile(FILESYSTEM & "\" & poAgentID, FILE_NAME, oHandle)

            Catch ex As Exception
                log.Error("Fail to insert odd rule to Agent Setting Game", ex)
                Return False
            End Try

            Return True
        End Function

        Public Function InsertRestriction(ByVal poAgentID As String, ByVal psID As String, ByVal psSportType As String, ByVal psBettingPeriod As String, ByVal pnAwaySpreadMoneyGT As Integer, ByVal pnAwaySpreadMoneyLT As Integer, ByVal pnHomeSpreadMoneyGT As Integer, ByVal pnHomeSpreadMoneyLT As Integer, ByVal pnTotalPointGT As Integer, ByVal pnTotalPointLT As Integer) As Boolean
            Try
                Dim bEmptyData As Boolean = False
                Dim bCreateNewNode As Boolean = False
                Dim oDoc As New XmlDocument()
                Dim oDB As New FileDB.CFileDB()
                Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
                Dim sXMLPath As String = "root/GameLineRestriction"
                Try
                    oDoc.Load(oHandle.LocalFileName)
                    If oDoc.SelectNodes(sXMLPath).Count > 0 Then
                        bCreateNewNode = False
                    Else
                        bCreateNewNode = True
                    End If

                Catch ex As Exception
                    bEmptyData = True
                End Try
                If bCreateNewNode Then
                    oHandle = oDB.GetNewFileHandle()
                End If
                Dim oCondition As XmlNode = oDoc.CreateElement(CONDITION)
                Dim oSportType As XmlAttribute = oDoc.CreateAttribute(SPORTTYPE)
                oSportType.Value = psSportType
                oCondition.Attributes.Append(oSportType)
                Dim oBettingPeriod As XmlAttribute = oDoc.CreateAttribute(BETTINGPERIOD)
                oBettingPeriod.Value = psBettingPeriod
                oCondition.Attributes.Append(oBettingPeriod)
                Dim oAwaySpreadMoneyGT As XmlAttribute = oDoc.CreateAttribute(AWAYSPREADMONEYGT)
                oAwaySpreadMoneyGT.Value = pnAwaySpreadMoneyGT.ToString
                oCondition.Attributes.Append(oAwaySpreadMoneyGT)
                Dim oAwaySpreadMoneyLT As XmlAttribute = oDoc.CreateAttribute(AWAYSPEADMONEYLT)
                oAwaySpreadMoneyLT.Value = pnAwaySpreadMoneyLT.ToString
                oCondition.Attributes.Append(oAwaySpreadMoneyLT)
                Dim oHomeSpreadMoneyGT As XmlAttribute = oDoc.CreateAttribute(HOMESPEADMONEYGT)
                oHomeSpreadMoneyGT.Value = pnHomeSpreadMoneyGT.ToString
                oCondition.Attributes.Append(oHomeSpreadMoneyGT)
                Dim oHomeSpreadMoneyLT As XmlAttribute = oDoc.CreateAttribute(HOMESPEADMONEYLT)
                oHomeSpreadMoneyLT.Value = pnHomeSpreadMoneyLT.ToString
                oCondition.Attributes.Append(oHomeSpreadMoneyLT)
                Dim oTotalPointGT As XmlAttribute = oDoc.CreateAttribute(TOTALPOINTGT)
                oTotalPointGT.Value = pnTotalPointGT.ToString
                oCondition.Attributes.Append(oTotalPointGT)
                Dim oTotalPointLT As XmlAttribute = oDoc.CreateAttribute(TOTALPOINTLT)
                oTotalPointLT.Value = pnTotalPointLT.ToString
                oCondition.Attributes.Append(oTotalPointLT)
                Dim oID As XmlAttribute = oDoc.CreateAttribute("ID")
                oID.Value = psID
                oCondition.Attributes.Append(oID)
                Dim oGameLineRestriction As XmlNode = oDoc.CreateElement("GameLineRestriction")
                oGameLineRestriction.AppendChild(oCondition)
                If bEmptyData Then
                    IO.File.WriteAllText(oHandle.LocalFileName, String.Format("<?xml version=""1.0"" encoding=""UTF-8""?><root>{0}</root>", oGameLineRestriction.OuterXml))
                    GoTo saveFile
                End If
                If bCreateNewNode Then
                    oDoc.DocumentElement.AppendChild(oGameLineRestriction)
                    oDoc.Save(oHandle.LocalFileName)
                Else
                    oDoc.SelectSingleNode("root/GameLineRestriction").AppendChild(oCondition)
                    oDoc.Save(oHandle.LocalFileName)
                End If
saveFile:
                oDB.PutFile(FILESYSTEM & "\" & poAgentID, FILE_NAME, oHandle)

            Catch ex As Exception
                log.Error("Fail to insert GameLineRestriction to Agent Setting Game", ex)
                Return False
            End Try

            Return True
        End Function


        Public Function InsertMoneyLineOff(ByVal poAgentID As String, ByVal psID As String, ByVal psSportType As String, ByVal pnAwaySpreadMoneyGT As Integer, ByVal pnAwaySpreadMoneyLT As Integer) As Boolean
            Try
                Dim bEmptyData As Boolean = False
                Dim bCreateNewNode As Boolean = False
                Dim oDoc As New XmlDocument()
                Dim oDB As New FileDB.CFileDB()
                Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
                Dim sXMLPath As String = "root/MoneyLineOff"
                Try
                    oDoc.Load(oHandle.LocalFileName)
                    If oDoc.SelectNodes(sXMLPath).Count > 0 Then
                        bCreateNewNode = False
                    Else
                        bCreateNewNode = True
                    End If

                Catch ex As Exception
                    bEmptyData = True
                End Try
                If bCreateNewNode Then
                    oHandle = oDB.GetNewFileHandle()
                End If
                Dim oCondition As XmlNode = oDoc.CreateElement(CONDITION)
                Dim oSportType As XmlAttribute = oDoc.CreateAttribute(SPORTTYPE)
                oSportType.Value = psSportType
                oCondition.Attributes.Append(oSportType)
                Dim oAwaySpreadMoneyGT As XmlAttribute = oDoc.CreateAttribute(AWAYSPREADMONEYGT)
                oAwaySpreadMoneyGT.Value = pnAwaySpreadMoneyGT.ToString
                oCondition.Attributes.Append(oAwaySpreadMoneyGT)
                Dim oAwaySpreadMoneyLT As XmlAttribute = oDoc.CreateAttribute(AWAYSPEADMONEYLT)
                oAwaySpreadMoneyLT.Value = pnAwaySpreadMoneyLT.ToString
                oCondition.Attributes.Append(oAwaySpreadMoneyLT)
                Dim oID As XmlAttribute = oDoc.CreateAttribute("ID")
                oID.Value = psID
                oCondition.Attributes.Append(oID)
                Dim oMoneyLineOff As XmlNode = oDoc.CreateElement("MoneyLineOff")
                oMoneyLineOff.AppendChild(oCondition)
                If bEmptyData Then
                    IO.File.WriteAllText(oHandle.LocalFileName, String.Format("<?xml version=""1.0"" encoding=""UTF-8""?><root>{0}</root>", oMoneyLineOff.OuterXml))
                    GoTo saveFile
                End If
                If bCreateNewNode Then
                    oDoc.DocumentElement.AppendChild(oMoneyLineOff)
                    oDoc.Save(oHandle.LocalFileName)
                Else
                    oDoc.SelectSingleNode("root/MoneyLineOff").AppendChild(oCondition)
                    oDoc.Save(oHandle.LocalFileName)
                End If
saveFile:
                oDB.PutFile(FILESYSTEM & "\" & poAgentID, FILE_NAME, oHandle)

            Catch ex As Exception
                log.Error("Fail to insert MoneyLineOff to Agent Setting Game", ex)
                Return False
            End Try

            Return True
        End Function

        Public Function InsertFixedSpreadMoney(ByVal poAgentID As String, ByVal psID As String, ByVal pnFixedSpreadMoney As Double, ByVal pnFixedSpreadMoneyH As Double, ByVal pnFixedSpreadMoneyGameTotal As Double, _
                                               ByVal pnFlatSpreadMoney As Double, ByVal psUseSpreadMoney As String) As Boolean
            Try
                Dim bEmptyData As Boolean = False
                Dim bCreateNewNode As Boolean = False
                Dim oDoc As New XmlDocument()
                Dim oDB As New FileDB.CFileDB()
                Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
                Dim sXMLPath As String = "root/FixedSpreadMoney"
                Try
                    oDoc.Load(oHandle.LocalFileName)
                    If oDoc.SelectNodes(sXMLPath).Count > 0 Then
                        bCreateNewNode = False
                    Else
                        bCreateNewNode = True
                    End If

                Catch ex As Exception
                    bEmptyData = True
                End Try
                If bCreateNewNode Then
                    oHandle = oDB.GetNewFileHandle()
                End If

                Dim oNumFixedSpreadMoney As XmlNode = oDoc.CreateElement("NumFixedSpreadMoney")
                Dim oNumFixedSpreadMoneyCurrent As XmlAttribute = oDoc.CreateAttribute(NUM_FIX_SPREAD_MONEY)
                oNumFixedSpreadMoneyCurrent.Value = pnFixedSpreadMoney.ToString
                oNumFixedSpreadMoney.Attributes.Append(oNumFixedSpreadMoneyCurrent)
                Dim oNumFixedSpreadMoneyH As XmlAttribute = oDoc.CreateAttribute(NUM_FIX_SPREAD_MONEYH)
                oNumFixedSpreadMoneyH.Value = pnFixedSpreadMoneyH.ToString
                oNumFixedSpreadMoney.Attributes.Append(oNumFixedSpreadMoneyH)

                Dim oNumFixedSpreadMoneyGameTotal As XmlAttribute = oDoc.CreateAttribute(NUM_FIX_SPREAD_MONEY_GAMETOTAL)
                oNumFixedSpreadMoneyGameTotal.Value = pnFixedSpreadMoneyGameTotal.ToString
                oNumFixedSpreadMoney.Attributes.Append(oNumFixedSpreadMoneyGameTotal)


                Dim oNumFlatSpreadMoney As XmlAttribute = oDoc.CreateAttribute(NUM_FLAT_SPREAD_MONEY)
                oNumFlatSpreadMoney.Value = SafeString(pnFlatSpreadMoney)
                oNumFixedSpreadMoney.Attributes.Append(oNumFlatSpreadMoney)
                Dim oFixedSpreadMoneyID As XmlAttribute = oDoc.CreateAttribute("ID")
                oFixedSpreadMoneyID.Value = psID
                oNumFixedSpreadMoney.Attributes.Append(oFixedSpreadMoneyID)
                Dim oUseFixSpreadMoney As XmlAttribute = oDoc.CreateAttribute(USE_SPREAD_MONEY)
                oUseFixSpreadMoney.Value = SafeString(IIf(psUseSpreadMoney = "Usefixedspreadmoney", True, False))
                oNumFixedSpreadMoney.Attributes.Append(oUseFixSpreadMoney)

                Dim oUseFlatSpreadMoney As XmlAttribute = oDoc.CreateAttribute(USE_FLAT_SPREAD_MONEY)
                oUseFlatSpreadMoney.Value = SafeString(IIf(psUseSpreadMoney = "Useflatspreadmoney", True, False))
                oNumFixedSpreadMoney.Attributes.Append(oUseFlatSpreadMoney)

                Dim oUseDefaultBookMaker As XmlAttribute = oDoc.CreateAttribute(USE_DEFAULT_BOOKMAKER)
                oUseDefaultBookMaker.Value = SafeString(IIf(psUseSpreadMoney = "Usedefaultbookmaker", True, False))
                oNumFixedSpreadMoney.Attributes.Append(oUseDefaultBookMaker)

                If bEmptyData Then
                    Dim oFixedSpreadMoney As XmlNode = oDoc.CreateElement(FIX_SPREAD_MONEY)
                    oFixedSpreadMoney.AppendChild(oNumFixedSpreadMoney)
                    IO.File.WriteAllText(oHandle.LocalFileName, String.Format("<?xml version=""1.0"" encoding=""UTF-8""?><root>{0}</root>", oFixedSpreadMoney.OuterXml))
                    GoTo saveFile
                End If

                If bCreateNewNode Then
                    Dim oFixSpreadMoney As XmlNode = oDoc.CreateElement(FIX_SPREAD_MONEY)
                    oFixSpreadMoney.AppendChild(oNumFixedSpreadMoney)
                    oDoc.DocumentElement.AppendChild(oFixSpreadMoney)
                    oDoc.Save(oHandle.LocalFileName)
                Else
                    oDoc.SelectSingleNode("root/FixedSpreadMoney").AppendChild(oNumFixedSpreadMoney)
                    oDoc.Save(oHandle.LocalFileName)
                End If
saveFile:
                oDB.PutFile(FILESYSTEM & "\" & poAgentID, FILE_NAME, oHandle)

            Catch ex As Exception
                log.Error("Fail to insert 2h Agent Setting Game", ex)
                Return False
            End Try

            Return True
        End Function

        Public Function InsertParplaySetup(ByVal poAgentID As String, ByVal pnParplaySetup As Integer) As Boolean
            Try
                Dim bEmptyData As Boolean = False
                Dim bCreateNewNode As Boolean = False
                Dim oDoc As New XmlDocument()
                Dim oDB As New FileDB.CFileDB()
                Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
                Dim sXMLPath As String = "root/ParplayMaxSelection"
                Try
                    oDoc.Load(oHandle.LocalFileName)
                    If oDoc.SelectNodes(sXMLPath).Count > 0 Then
                        bCreateNewNode = False
                    Else
                        bCreateNewNode = True
                    End If

                Catch ex As Exception
                    bEmptyData = True
                End Try
                If bCreateNewNode Then
                    oHandle = oDB.GetNewFileHandle()
                End If
                Dim oNumParplaySetup As XmlNode = oDoc.CreateElement("NumParplaySetup")
                Dim oMaxSelection As XmlAttribute = oDoc.CreateAttribute(PARPLAY_MAX_SELECTION)
                oMaxSelection.Value = pnParplaySetup.ToString
                oNumParplaySetup.Attributes.Append(oMaxSelection)
                Dim oParplaySetupID As XmlAttribute = oDoc.CreateAttribute("ID")
                oParplaySetupID.Value = newGUID()
                oNumParplaySetup.Attributes.Append(oParplaySetupID)
                If bEmptyData Then
                    Dim oParplayMaxSelection As XmlNode = oDoc.CreateElement(PARPLAY_MAX_SELECTION)
                    oParplayMaxSelection.AppendChild(oNumParplaySetup)
                    IO.File.WriteAllText(oHandle.LocalFileName, String.Format("<?xml version=""1.0"" encoding=""UTF-8""?><root>{0}</root>", oParplayMaxSelection.OuterXml))
                    GoTo saveFile
                End If

                If bCreateNewNode Then
                    Dim oParplayMaxSelection As XmlNode = oDoc.CreateElement(PARPLAY_MAX_SELECTION)
                    oParplayMaxSelection.AppendChild(oNumParplaySetup)
                    oDoc.DocumentElement.AppendChild(oParplayMaxSelection)
                    oDoc.Save(oHandle.LocalFileName)
                Else
                    oDoc.SelectSingleNode("root/ParplayMaxSelection").AppendChild(oNumParplaySetup)
                    oDoc.Save(oHandle.LocalFileName)
                End If
saveFile:
                oDB.PutFile(FILESYSTEM & "\" & poAgentID, FILE_NAME, oHandle)

            Catch ex As Exception
                log.Error("Fail to insert max parplay setup Setting Game", ex)
                Return False
            End Try

            Return True
        End Function

        Public Function InsertGameHalfTimeDisplay(ByVal poAgentID As String, ByVal poGameHalfTimeDisplay As CGameHalfTimeDisplay) As Boolean
            Try
                Dim bEmptyData As Boolean = False
                Dim bCreateNewNode As Boolean = False
                Dim oDoc As New XmlDocument()
                Dim oDB As New FileDB.CFileDB()
                Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
                Dim sXMLPath As String = "root/GameHalfTimeDisplay"
                Try
                    oDoc.Load(oHandle.LocalFileName)
                    If oDoc.SelectNodes(sXMLPath).Count > 0 Then
                        bCreateNewNode = False
                    Else
                        bCreateNewNode = True
                    End If

                Catch ex As Exception
                    bEmptyData = True
                End Try
                If bCreateNewNode Then
                    oHandle = oDB.GetNewFileHandle()
                End If
                Dim oGameDisplaySetup As XmlNode = oDoc.CreateElement("GameDisplaySetup")
                Dim oSportType As XmlAttribute = oDoc.CreateAttribute(SPORTTYPE)
                oSportType.Value = poGameHalfTimeDisplay.SportType
                oGameDisplaySetup.Attributes.Append(oSportType)
                Dim oSecondHOff As XmlAttribute = oDoc.CreateAttribute(SECONDHOFF)
                oSecondHOff.Value = SafeString(poGameHalfTimeDisplay.SecondHOff)
                oGameDisplaySetup.Attributes.Append(oSecondHOff)
                Dim oGameSetupID As XmlAttribute = oDoc.CreateAttribute("ID")
                oGameSetupID.Value = newGUID()
                oGameDisplaySetup.Attributes.Append(oGameSetupID)
                If bEmptyData Then
                    Dim oGameHalfTimeDisplay As XmlNode = oDoc.CreateElement(GAMEHALFTIMEDISPLAY)
                    oGameHalfTimeDisplay.AppendChild(oGameDisplaySetup)
                    IO.File.WriteAllText(oHandle.LocalFileName, String.Format("<?xml version=""1.0"" encoding=""UTF-8""?><root>{0}</root>", oGameHalfTimeDisplay.OuterXml))
                    GoTo saveFile
                End If

                If bCreateNewNode Then
                    Dim oGameHalfTimeDisplay As XmlNode = oDoc.CreateElement(GAMEHALFTIMEDISPLAY)
                    oGameHalfTimeDisplay.AppendChild(oGameDisplaySetup)
                    oDoc.DocumentElement.AppendChild(oGameHalfTimeDisplay)
                    oDoc.Save(oHandle.LocalFileName)
                Else
                    oDoc.SelectSingleNode("root/GameHalfTimeDisplay").AppendChild(oGameDisplaySetup)
                    oDoc.Save(oHandle.LocalFileName)
                End If
saveFile:
                oDB.PutFile(FILESYSTEM & "\" & poAgentID, FILE_NAME, oHandle)

            Catch ex As Exception
                log.Error("Fail to insert  GameHalfTime Display setup Setting Game", ex)
                Return False
            End Try

            Return True
        End Function

        Public Function UpdateGameHalfTimeDisplay(ByVal poAgentID As String, ByVal poGameHalfTimeDisplay As CGameHalfTimeDisplay) As Boolean
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            If Not oHandle Is Nothing Then
                Try
                    oDoc.Load(oHandle.LocalFileName)
                    Dim sXMLPath As String = String.Format("//GameHalfTimeDisplay/GameDisplaySetup[@SportType='{0}']", poGameHalfTimeDisplay.SportType)
                    Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                    oNodes(FIRSTNODE).Attributes(SPORTTYPE).Value = SafeString(poGameHalfTimeDisplay.SportType)
                    oNodes(FIRSTNODE).Attributes(SECONDHOFF).Value = SafeString(poGameHalfTimeDisplay.SecondHOff)
                    oDoc.Save(oHandle.LocalFileName)
                    oDB.PutFile(FILESYSTEM & "\" & poAgentID, FILE_NAME, oHandle)
                Catch ex As Exception
                    log.Error("Fail to update  GameHalfTime Display setup Setting Game", ex)
                    Return False
                End Try
            End If
            Return True
        End Function

        Public Function InsertGameHalfTimeOff(ByVal poAgentID As String, ByVal poGameHalfTimeOff As CGameHalfTimeOFF) As Boolean
            Try
                Dim bEmptyData As Boolean = False
                Dim bCreateNewNode As Boolean = False
                Dim oDoc As New XmlDocument()
                Dim oDB As New FileDB.CFileDB()
                Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
                Dim sXMLPath As String = "root/GameHalfTimeOff"
                Try
                    oDoc.Load(oHandle.LocalFileName)
                    If oDoc.SelectNodes(sXMLPath).Count > 0 Then
                        bCreateNewNode = False
                    Else
                        bCreateNewNode = True
                    End If

                Catch ex As Exception
                    bEmptyData = True
                End Try
                If bCreateNewNode Then
                    oHandle = oDB.GetNewFileHandle()
                End If
                Dim oGameOffSetup As XmlNode = oDoc.CreateElement("GameOffSetup")
                Dim oGameType As XmlAttribute = oDoc.CreateAttribute(GAMETYPE)
                oGameType.Value = poGameHalfTimeOff.GameType
                oGameOffSetup.Attributes.Append(oGameType)
                Dim oFirstHOff As XmlAttribute = oDoc.CreateAttribute(FIRSTHOFF)
                oFirstHOff.Value = SafeString(poGameHalfTimeOff.FirstHOff)
                oGameOffSetup.Attributes.Append(oFirstHOff)
                Dim oSecondHOff As XmlAttribute = oDoc.CreateAttribute(SECONDHOFF)
                oSecondHOff.Value = SafeString(poGameHalfTimeOff.SecondHOff)
                oGameOffSetup.Attributes.Append(oSecondHOff)
                Dim oTeamtotalOff As XmlAttribute = oDoc.CreateAttribute(TEAMTOTALOFF)
                oTeamtotalOff.Value = SafeString(poGameHalfTimeOff.TeamTotalOff)
                oGameOffSetup.Attributes.Append(oTeamtotalOff)
                Dim oGameSetupID As XmlAttribute = oDoc.CreateAttribute("ID")
                oGameSetupID.Value = newGUID()
                oGameOffSetup.Attributes.Append(oGameSetupID)
                If bEmptyData Then
                    Dim oGameHalfTimeOff As XmlNode = oDoc.CreateElement(GAMEHALFTIMEOFF)
                    oGameHalfTimeOff.AppendChild(oGameOffSetup)
                    IO.File.WriteAllText(oHandle.LocalFileName, String.Format("<?xml version=""1.0"" encoding=""UTF-8""?><root>{0}</root>", oGameHalfTimeOff.OuterXml))
                    GoTo saveFile
                End If

                If bCreateNewNode Then
                    Dim oGameHalfTimeOff As XmlNode = oDoc.CreateElement(GAMEHALFTIMEOFF)
                    oGameHalfTimeOff.AppendChild(oGameOffSetup)
                    oDoc.DocumentElement.AppendChild(oGameHalfTimeOff)
                    oDoc.Save(oHandle.LocalFileName)
                Else
                    oDoc.SelectSingleNode("root/GameHalfTimeOff").AppendChild(oGameOffSetup)
                    oDoc.Save(oHandle.LocalFileName)
                End If
saveFile:
                oDB.PutFile(FILESYSTEM & "\" & poAgentID, FILE_NAME, oHandle)

            Catch ex As Exception
                log.Error("Fail to insert  GameHalfTime Off setup Setting Game", ex)
                Return False
            End Try

            Return True
        End Function

        Public Function InsertJuiceControl(ByVal poAgentID As String, ByVal psSportType As String, ByVal pnJuice As Integer, ByVal psContext As String) As Boolean
            Try
                Dim bEmptyData As Boolean = False
                Dim bCreateNewNode As Boolean = False
                Dim oDoc As New XmlDocument()
                Dim oDB As New FileDB.CFileDB()
                Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
                Dim sXMLPath As String = "root/JuiceControl"
                Try
                    oDoc.Load(oHandle.LocalFileName)
                    If oDoc.SelectNodes(sXMLPath).Count > 0 Then
                        bCreateNewNode = False
                    Else
                        bCreateNewNode = True
                    End If

                Catch ex As Exception
                    bEmptyData = True
                End Try
                If bCreateNewNode Then
                    oHandle = oDB.GetNewFileHandle()
                End If
                Dim oJuiceControlSetup As XmlNode = oDoc.CreateElement("JuiceControlSetup")
                Dim oSportType As XmlAttribute = oDoc.CreateAttribute(SPORTTYPE)
                oSportType.Value = psSportType
                oJuiceControlSetup.Attributes.Append(oSportType)
                Dim oContext As XmlAttribute = oDoc.CreateAttribute("Context")
                oContext.Value = psContext
                oJuiceControlSetup.Attributes.Append(oContext)
                Dim oJuiceValue As XmlAttribute = oDoc.CreateAttribute(JUICEVALUE)
                oJuiceValue.Value = SafeString(pnJuice)
                oJuiceControlSetup.Attributes.Append(oJuiceValue)
                Dim oJuiceControlID As XmlAttribute = oDoc.CreateAttribute("ID")
                oJuiceControlID.Value = newGUID()
                oJuiceControlSetup.Attributes.Append(oJuiceControlID)
                If bEmptyData Then
                    Dim oJuiceControl As XmlNode = oDoc.CreateElement(JUICECONTROL)
                    oJuiceControl.AppendChild(oJuiceControlSetup)
                    IO.File.WriteAllText(oHandle.LocalFileName, String.Format("<?xml version=""1.0"" encoding=""UTF-8""?><root>{0}</root>", oJuiceControl.OuterXml))
                    GoTo saveFile
                End If

                If bCreateNewNode Then
                    Dim oJuiceControl As XmlNode = oDoc.CreateElement(JUICECONTROL)
                    oJuiceControl.AppendChild(oJuiceControlSetup)
                    oDoc.DocumentElement.AppendChild(oJuiceControl)
                    oDoc.Save(oHandle.LocalFileName)
                Else
                    oDoc.SelectSingleNode("root/JuiceControl").AppendChild(oJuiceControlSetup)
                    oDoc.Save(oHandle.LocalFileName)
                End If
saveFile:
                oDB.PutFile(FILESYSTEM & "\" & poAgentID, FILE_NAME, oHandle)

            Catch ex As Exception
                log.Error("Fail to insert  juicecontrol  setup Setting Game", ex)
                Return False
            End Try

            Return True
        End Function

        Public Function UpdateJuiceControl(ByVal poAgentID As String, ByVal psSportType As String, ByVal pnJuice As Integer, ByVal psContext As String) As Boolean
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            If Not oHandle Is Nothing Then
                Try
                    oDoc.Load(oHandle.LocalFileName)
                    Dim sXMLPath As String
                    Dim oNodes As XmlNodeList
                    Try
                        sXMLPath = String.Format("//JuiceControl/JuiceControlSetup[@SportType='{0}' and @Context='{1}']", psSportType, psContext)
                        oNodes = oDoc.SelectNodes(sXMLPath)
                        If oNodes Is Nothing Then
                            Return InsertJuiceControl(poAgentID, psSportType, pnJuice, psContext)
                        End If
                        oNodes(FIRSTNODE).Attributes(SPORTTYPE).Value = SafeString(psSportType)
                        oNodes(FIRSTNODE).Attributes(JUICEVALUE).Value = SafeString(pnJuice)
                        If oNodes(FIRSTNODE).Attributes("Context") Is Nothing Then
                            Dim oContext As XmlAttribute = oDoc.CreateAttribute("Context")
                            '  oContext.Value = psContext
                            ' oNodes(FIRSTNODE).Attributes.Append(oContext)
                            InsertJuiceControl(poAgentID, psSportType, pnJuice, psContext)
                            Return True
                        Else
                            oNodes(FIRSTNODE).Attributes("Context").Value = SafeString(psContext)
                        End If
                        oDoc.Save(oHandle.LocalFileName)
                        oDB.PutFile(FILESYSTEM & "\" & poAgentID, FILE_NAME, oHandle)

                    Catch ex As Exception
                        log.Error("Fail to update juice control setup Setting Game", ex)
                        Return InsertJuiceControl(poAgentID, psSportType, pnJuice, psContext)
                        'sXMLPath = String.Format("//JuiceControl/JuiceControlSetup[@SportType='{0}' ]", psSportType)
                        'oNodes = oDoc.SelectNodes(sXMLPath)
                        'oNodes(FIRSTNODE).Attributes(SPORTTYPE).Value = SafeString(psSportType)
                        'oNodes(FIRSTNODE).Attributes(JUICEVALUE).Value = SafeString(pnJuice)
                        'If oNodes(FIRSTNODE).Attributes("Context") Is Nothing Then
                        '    Dim oContext As XmlAttribute = oDoc.CreateAttribute("Context")
                        '    'oContext.Value = psContext
                        '    'oNodes(FIRSTNODE).Attributes.Append(oContext)
                        '    InsertJuiceControl(poAgentID, psSportType, pnJuice, psContext)
                        '    Return True
                        'Else
                        '    oNodes(FIRSTNODE).Attributes("Context").Value = SafeString(psContext)
                        'End If
                        'oDoc.Save(oHandle.LocalFileName)
                        'oDB.PutFile(FILESYSTEM & "\" & poAgentID, FILE_NAME, oHandle)

                    End Try
                    'sXMLPath = String.Format("//JuiceControl/JuiceControlSetup[@SportType='{0}' and @Context='{1}'", psSportType, psContext)
                    'oNodes = oDoc.SelectNodes(sXMLPath)
                    'oNodes(FIRSTNODE).Attributes(SPORTTYPE).Value = SafeString(psSportType)
                    'oNodes(FIRSTNODE).Attributes(JUICEVALUE).Value = SafeString(pnJuice)
                    'If oNodes(FIRSTNODE).Attributes("Context") Is Nothing Then
                    '    Dim oContext As XmlAttribute = oDoc.CreateAttribute("Context")
                    '    oContext.Value = psContext
                    '    oNodes(FIRSTNODE).Attributes.Append(oContext)
                    'Else
                    '    oNodes(FIRSTNODE).Attributes("Context").Value = SafeString(psContext)
                    'End If
                    'oDoc.Save(oHandle.LocalFileName)
                    'oDB.PutFile(FILESYSTEM & "\" & poAgentID, FILE_NAME, oHandle)
                Catch ex As Exception
                    log.Error("Fail to update juice control setup Setting Game", ex)
                    Return False
                End Try
            End If
            Return True
        End Function

        Public Function UpdateGameHalfTimeOff(ByVal poAgentID As String, ByVal poGameHalfTimeOff As CGameHalfTimeOFF) As Boolean

            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            If Not oHandle Is Nothing Then
                Try
                    oDoc.Load(oHandle.LocalFileName)
                    Dim sXMLPath As String = String.Format("//GameHalfTimeOff/GameOffSetup[@GameType='{0}']", poGameHalfTimeOff.GameType)
                    Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                    oNodes(FIRSTNODE).Attributes(GAMETYPE).Value = SafeString(poGameHalfTimeOff.GameType)
                    oNodes(FIRSTNODE).Attributes(FIRSTHOFF).Value = SafeString(poGameHalfTimeOff.FirstHOff)
                    oNodes(FIRSTNODE).Attributes(SECONDHOFF).Value = SafeString(poGameHalfTimeOff.SecondHOff)
                    If oNodes(FIRSTNODE).Attributes(TEAMTOTALOFF) Is Nothing Then
                        Dim oTeamtotalOff As XmlAttribute = oDoc.CreateAttribute(TEAMTOTALOFF)
                        oTeamtotalOff.Value = SafeString(poGameHalfTimeOff.TeamTotalOff)
                        oNodes(FIRSTNODE).Attributes.Append(oTeamtotalOff)
                    Else
                        oNodes(FIRSTNODE).Attributes(TEAMTOTALOFF).Value = SafeString(poGameHalfTimeOff.TeamTotalOff)
                    End If
                    oNodes(FIRSTNODE).Attributes(TEAMTOTALOFF).Value = SafeString(poGameHalfTimeOff.TeamTotalOff)
                    oDoc.Save(oHandle.LocalFileName)
                    oDB.PutFile(FILESYSTEM & "\" & poAgentID, FILE_NAME, oHandle)
                Catch ex As Exception
                    log.Error("Fail to update  GameHalfTime Off setup Setting Game", ex)
                    Return False
                End Try
            End If
            Return True
        End Function


        Public Function UpdateParplaySetup(ByVal poAgentID As String, ByVal pnParplaySetup As Integer) As Boolean
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            If Not oHandle Is Nothing Then
                Try
                    oDoc.Load(oHandle.LocalFileName)
                    Dim sXMLPath As String = "//ParplayMaxSelection/NumParplaySetup"
                    Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                    oNodes(FIRSTNODE).Attributes(PARPLAY_MAX_SELECTION).Value = SafeString(pnParplaySetup)
                    oDoc.Save(oHandle.LocalFileName)
                    oDB.PutFile(FILESYSTEM & "\" & poAgentID, FILE_NAME, oHandle)
                Catch ex As Exception
                    Return False
                End Try
            End If
            Return True
        End Function

        Public Function UpdateGameTypeOnOffByAgent(ByVal poAgentID As String, ByVal psGameType As String, ByVal pnOffBefore As Integer, ByVal pnDisplayBefore As Integer, ByVal pnTotalpointDisplayBefore As Integer, ByVal bCurrent As Boolean) As Boolean
            Dim sPath As String
            If bCurrent Then
                sPath = "CurrentGame"
            Else
                sPath = "FirstHalf"
            End If
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            If Not oHandle Is Nothing Then
                Try
                    oDoc.Load(oHandle.LocalFileName)
                    Dim sXMLPath As String = String.Format("//" & sPath & "/GameTypeOnOff[@GameType='{0}']", psGameType)
                    Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                    oNodes(FIRSTNODE).Attributes(GAMETYPE).Value = SafeString(psGameType)
                    oNodes(FIRSTNODE).Attributes(OFFBEFORE).Value = SafeString(pnOffBefore)
                    oNodes(FIRSTNODE).Attributes(DISPLAYBEFORE).Value = SafeString(pnDisplayBefore)
                    If oNodes(FIRSTNODE).Attributes(GAMETOTALPOINTSDISPLAY) IsNot Nothing Then
                        oNodes(FIRSTNODE).Attributes(GAMETOTALPOINTSDISPLAY).Value = SafeString(pnTotalpointDisplayBefore)
                    Else
                        Dim oAttr As XmlAttribute = oDoc.CreateAttribute(GAMETOTALPOINTSDISPLAY)
                        oAttr.Value = SafeString(pnTotalpointDisplayBefore)
                        oNodes(FIRSTNODE).Attributes.Append(oAttr)
                    End If
                    oDoc.Save(oHandle.LocalFileName)
                    oDB.PutFile(FILESYSTEM & "\" & poAgentID, FILE_NAME, oHandle)
                Catch ex As Exception
                    Return False
                End Try
            End If
            Return True
        End Function

        Public Function DeleteDisplaySettings(ByVal poAgentID As String, ByVal poSettingID As String, ByVal bCurrent As Boolean) As Boolean
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            If Not oHandle Is Nothing Then
                Try
                    Dim sPath As String
                    If bCurrent Then
                        sPath = "CurrentGame"
                    Else
                        sPath = "FirstHalf"
                    End If
                    oDoc.Load(oHandle.LocalFileName)
                    Dim sXMLPath As String = String.Format("//" & sPath & "/GameTypeOnOff[@ID='{0}']", SafeString(poSettingID))
                    Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                    oNodes(FIRSTNODE).ParentNode.RemoveChild(oNodes(FIRSTNODE))
                    oDoc.Save(oHandle.LocalFileName)
                    oDB.PutFile(FILESYSTEM & "\" & poAgentID, FILE_NAME, oHandle)
                Catch ex As Exception
                    LogError(log, "Cannot Delete Display setting with ID", ex)
                    Return False
                End Try
            End If
            Return True
        End Function

        Public Function UpdateFixSpreadMoney(ByVal poAgentID As String, ByVal pnFixedSpreadMoney As Double, ByVal pnFixedSpreadMoneyH As Double, ByVal pnFixedSpreadMoneyGameTotal As Double, _
                                             ByVal pnFlatSpreadMoney As Double, ByVal psUseSpreadMoney As String) As Boolean
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            If Not oHandle Is Nothing Then
                Try
                    oDoc.Load(oHandle.LocalFileName)
                    Dim sXMLPath As String = "root/FixedSpreadMoney/NumFixedSpreadMoney"
                    Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                    oNodes(FIRSTNODE).Attributes(NUM_FIX_SPREAD_MONEY).Value = SafeString(pnFixedSpreadMoney)
                    oNodes(FIRSTNODE).Attributes(NUM_FIX_SPREAD_MONEYH).Value = SafeString(pnFixedSpreadMoneyH)
                    oNodes(FIRSTNODE).Attributes(NUM_FIX_SPREAD_MONEY_GAMETOTAL).Value = SafeString(pnFixedSpreadMoneyGameTotal)
                    If oNodes(FIRSTNODE).Attributes(NUM_FLAT_SPREAD_MONEY) IsNot Nothing Then
                        oNodes(FIRSTNODE).Attributes(NUM_FLAT_SPREAD_MONEY).Value = SafeString(pnFlatSpreadMoney)
                    Else
                        Dim oAttr As XmlAttribute = oDoc.CreateAttribute(NUM_FLAT_SPREAD_MONEY)
                        oAttr.Value = SafeString(pnFlatSpreadMoney)
                        oNodes(FIRSTNODE).Attributes.Append(oAttr)
                    End If
                    If oNodes(FIRSTNODE).Attributes(USE_SPREAD_MONEY) IsNot Nothing Then
                        oNodes(FIRSTNODE).Attributes(USE_SPREAD_MONEY).Value = SafeString(IIf(psUseSpreadMoney = "Usefixedspreadmoney", True, False))
                    Else
                        Dim oAttr As XmlAttribute = oDoc.CreateAttribute(USE_SPREAD_MONEY)
                        oAttr.Value = SafeString(IIf(psUseSpreadMoney = "Usefixedspreadmoney", True, False))
                        oNodes(FIRSTNODE).Attributes.Append(oAttr)
                    End If
                    If oNodes(FIRSTNODE).Attributes(USE_FLAT_SPREAD_MONEY) IsNot Nothing Then
                        oNodes(FIRSTNODE).Attributes(USE_FLAT_SPREAD_MONEY).Value = SafeString(IIf(psUseSpreadMoney = "Useflatspreadmoney", True, False))
                    Else
                        Dim oAttr As XmlAttribute = oDoc.CreateAttribute(USE_FLAT_SPREAD_MONEY)
                        oAttr.Value = SafeString(IIf(psUseSpreadMoney = "Useflatspreadmoney", True, False))
                        oNodes(FIRSTNODE).Attributes.Append(oAttr)
                    End If
                    If oNodes(FIRSTNODE).Attributes(USE_DEFAULT_BOOKMAKER) IsNot Nothing Then
                        oNodes(FIRSTNODE).Attributes(USE_DEFAULT_BOOKMAKER).Value = SafeString(IIf(psUseSpreadMoney = "Usedefaultbookmaker", True, False))
                    Else
                        Dim oAttr As XmlAttribute = oDoc.CreateAttribute(USE_DEFAULT_BOOKMAKER)
                        oAttr.Value = SafeString(IIf(psUseSpreadMoney = "Usedefaultbookmaker", True, False))
                        oNodes(FIRSTNODE).Attributes.Append(oAttr)
                    End If
                    oDoc.Save(oHandle.LocalFileName)
                    oDB.PutFile(FILESYSTEM & "\" & poAgentID, FILE_NAME, oHandle)
                Catch ex As Exception
                    Return False
                    LogError(log, "UpdateFixSpreadMoney", ex)
                End Try
            End If
            Return True
        End Function

        Public Function Update2HGameTypeOnOff(ByVal poAgentID As String, ByVal pnOffBefore As Integer) As Boolean

            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim sXMLPath As String = "root/SecondHGame/GameTypeOnOff"
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            If Not oHandle Is Nothing Then
                Try
                    oDoc.Load(oHandle.LocalFileName)
                    Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                    oNodes(FIRSTNODE).Attributes(OFFBEFORE).Value = SafeString(pnOffBefore)
                    oDoc.Save(oHandle.LocalFileName)
                    oDB.PutFile(FILESYSTEM & "\" & poAgentID, FILE_NAME, oHandle)
                Catch ex As Exception
                    Return False
                End Try
            End If
            Return True
        End Function

        Public Function UpdateCircleGame(ByVal poAgentID As String, ByVal pnStraight As Integer, ByVal pnParlayReverse As Integer) As Boolean
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            If Not oHandle Is Nothing Then
                Try
                    oDoc.Load(oHandle.LocalFileName)
                    Dim sXMLPath As String = "root/GameCircle/MaxTotalBet"
                    Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                    oNodes(FIRSTNODE).Attributes(STRAIGHT).Value = SafeString(pnStraight)
                    oNodes(FIRSTNODE).Attributes(PARLAYREVERSE).Value = SafeString(pnParlayReverse)
                    oDoc.Save(oHandle.LocalFileName)
                    oDB.PutFile(FILESYSTEM & "\" & poAgentID, FILE_NAME, oHandle)
                Catch ex As Exception
                    Return False
                End Try
            End If
            Return True
        End Function

        Public Function UpdateOddRule(ByVal poAgentID As String, ByVal psID As String, ByVal pnGreaterThan As Integer, ByVal pnLessThan As Integer, ByVal pnIncrease As Double, ByVal pbLockGame As Boolean) As Boolean
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            If Not oHandle Is Nothing Then
                Try
                    oDoc.Load(oHandle.LocalFileName)
                    Dim sXMLPath As String = String.Format("root/OddRule/Condition[@ID='{0}']", psID)
                    Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                    oNodes(FIRSTNODE).Attributes(GREATERTHAN).Value = SafeString(pnGreaterThan)
                    oNodes(FIRSTNODE).Attributes(LESSTHAN).Value = SafeString(pnLessThan)
                    oNodes(FIRSTNODE).Attributes(INCREASE).Value = SafeString(pnIncrease)
                    oNodes(FIRSTNODE).Attributes(LOCKGAME).Value = SafeString(pbLockGame)
                    oDoc.Save(oHandle.LocalFileName)
                    oDB.PutFile(FILESYSTEM & "\" & poAgentID, FILE_NAME, oHandle)
                Catch ex As Exception
                    Return False
                End Try
            End If
            Return True
        End Function

        Public Function UpdateRestriction(ByVal poAgentID As String, ByVal psSportType As String, ByVal psBettingPeriod As String, ByVal pnAwaySpreadMoneyGT As Integer, ByVal pnAwaySpreadMoneyLT As Integer, ByVal pnHomeSpreadMoneyGT As Integer, ByVal pnHomeSpreadMoneyLT As Integer, ByVal pnTotalPointGT As Integer, ByVal pnTotalPointLT As Integer) As Boolean
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            If Not oHandle Is Nothing Then
                Try
                    oDoc.Load(oHandle.LocalFileName)
                    Dim sXMLPath As String = "root/GameLineRestriction/Condition"
                    Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                    oNodes(FIRSTNODE).Attributes(SPORTTYPE).Value = SafeString(psSportType)
                    oNodes(FIRSTNODE).Attributes(BETTINGPERIOD).Value = SafeString(psBettingPeriod)
                    oNodes(FIRSTNODE).Attributes(AWAYSPREADMONEYGT).Value = SafeString(pnAwaySpreadMoneyGT)
                    oNodes(FIRSTNODE).Attributes(AWAYSPEADMONEYLT).Value = SafeString(pnAwaySpreadMoneyLT)
                    oNodes(FIRSTNODE).Attributes(HOMESPEADMONEYGT).Value = SafeString(pnHomeSpreadMoneyGT)
                    oNodes(FIRSTNODE).Attributes(HOMESPEADMONEYLT).Value = SafeString(pnHomeSpreadMoneyLT)
                    oNodes(FIRSTNODE).Attributes(TOTALPOINTGT).Value = SafeString(pnTotalPointGT)
                    oNodes(FIRSTNODE).Attributes(TOTALPOINTLT).Value = SafeString(pnTotalPointLT)
                    oDoc.Save(oHandle.LocalFileName)
                    oDB.PutFile(FILESYSTEM & "\" & poAgentID, FILE_NAME, oHandle)
                Catch ex As Exception
                    Return False
                End Try
            End If
            Return True
        End Function

        Public Function UpdateMoneyLineOff(ByVal poAgentID As String, ByVal psSportType As String, ByVal pnAwaySpreadMoneyGT As Integer, ByVal pnAwaySpreadMoneyLT As Integer) As Boolean
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            If Not oHandle Is Nothing Then
                Try
                    oDoc.Load(oHandle.LocalFileName)
                    Dim sXMLPath As String = "root/MoneyLineOff/Condition"
                    Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                    oNodes(FIRSTNODE).Attributes(SPORTTYPE).Value = SafeString(psSportType)
                    oNodes(FIRSTNODE).Attributes(AWAYSPREADMONEYGT).Value = SafeString(pnAwaySpreadMoneyGT)
                    oNodes(FIRSTNODE).Attributes(AWAYSPEADMONEYLT).Value = SafeString(pnAwaySpreadMoneyLT)
                    oDoc.Save(oHandle.LocalFileName)
                    oDB.PutFile(FILESYSTEM & "\" & poAgentID, FILE_NAME, oHandle)
                Catch ex As Exception
                    Return False
                End Try
            End If
            Return True
        End Function

        Public Function GetGameTypeOnOffByID(ByVal poAgentID As String, ByVal psID As String) As DataTable

            Dim odtGameTypeOnOff As DataTable = New DataTable()
            odtGameTypeOnOff.Columns.Add("ID")
            odtGameTypeOnOff.Columns.Add(GAMETYPE)
            odtGameTypeOnOff.Columns.Add(OFFBEFORE)
            odtGameTypeOnOff.Columns.Add(DISPLAYBEFORE)
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            Try
                oDoc.Load(oHandle.LocalFileName)
                Dim sXMLPath As String = String.Format("//CurrentGame/GameTypeOnOff[@ID='{0}']", psID)
                Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                Dim drGameTypeOnOff As DataRow = odtGameTypeOnOff.NewRow()
                drGameTypeOnOff("ID") = SafeString(psID)
                drGameTypeOnOff(OFFBEFORE) = SafeString(oNodes(FIRSTNODE).Attributes(OFFBEFORE).Value)
                drGameTypeOnOff(DISPLAYBEFORE) = SafeString(oNodes(FIRSTNODE).Attributes(DISPLAYBEFORE).Value)
                drGameTypeOnOff(GAMETYPE) = SafeString(oNodes(FIRSTNODE).Attributes(GAMETYPE).Value)
                odtGameTypeOnOff.Rows.Add(drGameTypeOnOff)
            Catch ex As Exception
                Return odtGameTypeOnOff
            End Try
            Return odtGameTypeOnOff
        End Function

        Public Function GetJuiceControlByID(ByVal poAgentID As String, ByVal psSportType As String, ByVal psContext As String) As Integer
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim sXMLPath As String
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            Try
                Try
                    sXMLPath = String.Format("root/JuiceControl/JuiceControlSetup[@SportType='{0}' and @Context='{1}']", psSportType, psContext)
                    oDoc.Load(oHandle.LocalFileName)
                    Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                    If oNodes IsNot Nothing And oNodes.Count > 0 Then
                        Return SafeInteger(oNodes(FIRSTNODE).Attributes(JUICEVALUE).Value)
                    End If
                Catch ex As Exception
                    sXMLPath = String.Format("root/JuiceControl/JuiceControlSetup[@SportType='{0}']", psSportType)
                    oDoc.Load(oHandle.LocalFileName)
                    Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                    If oNodes IsNot Nothing And oNodes.Count > 0 Then
                        Return SafeInteger(oNodes(FIRSTNODE).Attributes(JUICEVALUE).Value)
                    End If
                End Try
               
            Catch ex As Exception
                log.Error("cannot get juice control ", ex)
                Return 0
            End Try
            Return 0
        End Function

        Public Function GetOddRuleByID(ByVal poAgentID As String, ByVal psID As String) As DataTable

            Dim odtOddRule As DataTable = New DataTable()
            odtOddRule.Columns.Add("ID")
            odtOddRule.Columns.Add(GREATERTHAN)
            odtOddRule.Columns.Add(LESSTHAN)
            odtOddRule.Columns.Add(INCREASE)
            odtOddRule.Columns.Add(LOCKGAME)
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            Try
                oDoc.Load(oHandle.LocalFileName)
                Dim sXMLPath As String = String.Format("//OddRule/Condition[@ID='{0}']", psID)
                Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                Dim drGameTypeOnOff As DataRow = odtOddRule.NewRow()
                drGameTypeOnOff("ID") = SafeString(psID)
                drGameTypeOnOff(GREATERTHAN) = SafeString(oNodes(FIRSTNODE).Attributes(GREATERTHAN).Value)
                drGameTypeOnOff(LESSTHAN) = SafeString(oNodes(FIRSTNODE).Attributes(LESSTHAN).Value)
                drGameTypeOnOff(INCREASE) = SafeString(oNodes(FIRSTNODE).Attributes(INCREASE).Value)
                drGameTypeOnOff(LOCKGAME) = SafeString(oNodes(FIRSTNODE).Attributes(LOCKGAME).Value)
                odtOddRule.Rows.Add(drGameTypeOnOff)
            Catch ex As Exception
                Return odtOddRule
            End Try
            Return odtOddRule
        End Function


        Public Function GetGameTypeOnOffByGameType(ByVal poAgentID As String, ByVal psGameType As String, ByVal bCurrent As Boolean) As DataTable
            Dim sPath As String
            If bCurrent Then
                sPath = "CurrentGame"
            Else
                sPath = "FirstHalf"
            End If
            Dim odtGameTypeOnOff As DataTable = New DataTable()
            odtGameTypeOnOff.Columns.Add("ID")
            odtGameTypeOnOff.Columns.Add(GAMETYPE)
            odtGameTypeOnOff.Columns.Add(OFFBEFORE)
            odtGameTypeOnOff.Columns.Add(DISPLAYBEFORE)
            odtGameTypeOnOff.Columns.Add(GAMETOTALPOINTSDISPLAY)
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            Try
                oDoc.Load(oHandle.LocalFileName)
                Dim sXMLPath As String = String.Format("//" & sPath & "/GameTypeOnOff[@GameType='{0}']", psGameType)
                Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                Dim drGameTypeOnOff As DataRow = odtGameTypeOnOff.NewRow()
                drGameTypeOnOff("ID") = SafeString(oNodes(FIRSTNODE).Attributes("ID").Value)
                drGameTypeOnOff(OFFBEFORE) = SafeString(oNodes(FIRSTNODE).Attributes(OFFBEFORE).Value)
                drGameTypeOnOff(DISPLAYBEFORE) = SafeString(oNodes(FIRSTNODE).Attributes(DISPLAYBEFORE).Value)
                drGameTypeOnOff(GAMETYPE) = SafeString(oNodes(FIRSTNODE).Attributes(GAMETYPE).Value)
                If oNodes(FIRSTNODE).Attributes.ItemOf(GAMETOTALPOINTSDISPLAY) Is Nothing Then
                    drGameTypeOnOff(GAMETOTALPOINTSDISPLAY) = ""
                Else
                    drGameTypeOnOff(GAMETOTALPOINTSDISPLAY) = SafeString(oNodes(FIRSTNODE).Attributes(GAMETOTALPOINTSDISPLAY).Value)
                End If
                odtGameTypeOnOff.Rows.Add(drGameTypeOnOff)
                Return odtGameTypeOnOff
            Catch ex As Exception
                Return odtGameTypeOnOff
            End Try
            Return odtGameTypeOnOff
        End Function

        Public Function GetSpreadMoneyValues(ByVal poAgentID As String) As DataTable
            Dim odtSpreadMoney As DataTable = New DataTable()
            odtSpreadMoney.Columns.Add("ID")
            odtSpreadMoney.Columns.Add(NUM_FIX_SPREAD_MONEY)
            odtSpreadMoney.Columns.Add(NUM_FIX_SPREAD_MONEYH)
            odtSpreadMoney.Columns.Add(NUM_FIX_SPREAD_MONEY_GAMETOTAL)
            odtSpreadMoney.Columns.Add(NUM_FLAT_SPREAD_MONEY)
            odtSpreadMoney.Columns.Add(USE_SPREAD_MONEY)
            odtSpreadMoney.Columns.Add(USE_FLAT_SPREAD_MONEY)
            odtSpreadMoney.Columns.Add(USE_DEFAULT_BOOKMAKER)
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Try
                Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
                If oHandle IsNot Nothing Then
                    oDoc.Load(oHandle.LocalFileName)
                    Dim sXMLPath As String = "root/FixedSpreadMoney/NumFixedSpreadMoney"
                    Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                    If oNodes Is Nothing OrElse oNodes.Count = 0 Then
                        Return odtSpreadMoney
                    End If
                    Dim drSpreadMoney As DataRow = odtSpreadMoney.NewRow()
                    drSpreadMoney("ID") = SafeString(oNodes(FIRSTNODE).Attributes("ID").Value)
                    drSpreadMoney(NUM_FIX_SPREAD_MONEY) = SafeString(oNodes(FIRSTNODE).Attributes(NUM_FIX_SPREAD_MONEY).Value)
                    drSpreadMoney(NUM_FIX_SPREAD_MONEYH) = SafeString(oNodes(FIRSTNODE).Attributes(NUM_FIX_SPREAD_MONEYH).Value)
                    drSpreadMoney(NUM_FIX_SPREAD_MONEY_GAMETOTAL) = SafeString(oNodes(FIRSTNODE).Attributes(NUM_FIX_SPREAD_MONEY_GAMETOTAL).Value)
                    drSpreadMoney(NUM_FLAT_SPREAD_MONEY) = SafeString(oNodes(FIRSTNODE).Attributes(NUM_FLAT_SPREAD_MONEY).Value)
                    drSpreadMoney(USE_SPREAD_MONEY) = SafeString(oNodes(FIRSTNODE).Attributes(USE_SPREAD_MONEY).Value)
                    If oNodes(FIRSTNODE).Attributes(USE_FLAT_SPREAD_MONEY) IsNot Nothing Then
                        drSpreadMoney(USE_FLAT_SPREAD_MONEY) = SafeString(oNodes(FIRSTNODE).Attributes(USE_FLAT_SPREAD_MONEY).Value)
                    End If
                    If oNodes(FIRSTNODE).Attributes(USE_DEFAULT_BOOKMAKER) IsNot Nothing Then
                        drSpreadMoney(USE_DEFAULT_BOOKMAKER) = SafeString(oNodes(FIRSTNODE).Attributes(USE_DEFAULT_BOOKMAKER).Value)
                    End If
                    odtSpreadMoney.Rows.Add(drSpreadMoney)
                End If
                Return odtSpreadMoney
            Catch ex As Exception
                LogError(log, "Cannot get Spread Money.", ex)
                Return odtSpreadMoney
            End Try
            Return odtSpreadMoney
        End Function

        Public Function GetRestrictionValues(ByVal poAgentID As String) As DataTable
            Dim odtSpreadMoney As DataTable = New DataTable()
            odtSpreadMoney.Columns.Add("ID")
            odtSpreadMoney.Columns.Add(SPORTTYPE)
            odtSpreadMoney.Columns.Add(BETTINGPERIOD)
            odtSpreadMoney.Columns.Add(AWAYSPREADMONEYGT)
            odtSpreadMoney.Columns.Add(AWAYSPEADMONEYLT)
            odtSpreadMoney.Columns.Add(HOMESPEADMONEYGT)
            odtSpreadMoney.Columns.Add(HOMESPEADMONEYLT)
            odtSpreadMoney.Columns.Add(TOTALPOINTGT)
            odtSpreadMoney.Columns.Add(TOTALPOINTLT)
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            Try
                oDoc.Load(oHandle.LocalFileName)
                Dim sXMLPath As String = "root/GameLineRestriction/Condition"
                Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                Dim drSpreadMoney As DataRow = odtSpreadMoney.NewRow()
                drSpreadMoney("ID") = SafeString(oNodes(FIRSTNODE).Attributes("ID").Value)
                drSpreadMoney(SPORTTYPE) = SafeString(oNodes(FIRSTNODE).Attributes(SPORTTYPE).Value)
                drSpreadMoney(BETTINGPERIOD) = SafeString(oNodes(FIRSTNODE).Attributes(BETTINGPERIOD).Value)
                drSpreadMoney(AWAYSPREADMONEYGT) = SafeString(oNodes(FIRSTNODE).Attributes(AWAYSPREADMONEYGT).Value)
                drSpreadMoney(AWAYSPEADMONEYLT) = SafeString(oNodes(FIRSTNODE).Attributes(AWAYSPEADMONEYLT).Value)
                drSpreadMoney(HOMESPEADMONEYGT) = SafeString(oNodes(FIRSTNODE).Attributes(HOMESPEADMONEYGT).Value)
                drSpreadMoney(HOMESPEADMONEYLT) = SafeString(oNodes(FIRSTNODE).Attributes(HOMESPEADMONEYLT).Value)
                drSpreadMoney(TOTALPOINTGT) = SafeString(oNodes(FIRSTNODE).Attributes(TOTALPOINTGT).Value)
                drSpreadMoney(TOTALPOINTLT) = SafeString(oNodes(FIRSTNODE).Attributes(TOTALPOINTLT).Value)
                odtSpreadMoney.Rows.Add(drSpreadMoney)
                Return odtSpreadMoney
            Catch ex As Exception
                Return odtSpreadMoney
            End Try
            Return odtSpreadMoney
        End Function

        Public Function GetMoneyLineOffValues(ByVal poAgentID As String, ByVal psSportType As String) As DataTable
            Dim odtSpreadMoney As DataTable = New DataTable()
            odtSpreadMoney.Columns.Add("ID")
            odtSpreadMoney.Columns.Add(SPORTTYPE)
            odtSpreadMoney.Columns.Add(AWAYSPREADMONEYGT)
            odtSpreadMoney.Columns.Add(AWAYSPEADMONEYLT)
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            Try
                oDoc.Load(oHandle.LocalFileName)
                Dim sXMLPath As String = String.Format("root/MoneyLineOff/Condition[@SportType='{0}']", psSportType)
                Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                Dim drSpreadMoney As DataRow = odtSpreadMoney.NewRow()
                drSpreadMoney("ID") = SafeString(oNodes(FIRSTNODE).Attributes("ID").Value)
                drSpreadMoney(SPORTTYPE) = SafeString(oNodes(FIRSTNODE).Attributes(SPORTTYPE).Value)
                drSpreadMoney(AWAYSPREADMONEYGT) = SafeString(oNodes(FIRSTNODE).Attributes(AWAYSPREADMONEYGT).Value)
                drSpreadMoney(AWAYSPEADMONEYLT) = SafeString(oNodes(FIRSTNODE).Attributes(AWAYSPEADMONEYLT).Value)
                odtSpreadMoney.Rows.Add(drSpreadMoney)
                Return odtSpreadMoney
            Catch ex As Exception
                Return odtSpreadMoney
            End Try
            Return odtSpreadMoney
        End Function


        Public Function GetCircleGameValues(ByVal poAgentID As String) As DataTable
            Dim odtSpreadMoney As DataTable = New DataTable()
            odtSpreadMoney.Columns.Add("ID")
            odtSpreadMoney.Columns.Add(STRAIGHT)
            odtSpreadMoney.Columns.Add(PARLAYREVERSE)
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            Try
                oDoc.Load(oHandle.LocalFileName)
                Dim sXMLPath As String = "root/GameCircle/MaxTotalBet"
                Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                Dim drSpreadMoney As DataRow = odtSpreadMoney.NewRow()
                drSpreadMoney("ID") = SafeString(oNodes(FIRSTNODE).Attributes("ID").Value)
                drSpreadMoney(STRAIGHT) = SafeString(oNodes(FIRSTNODE).Attributes(STRAIGHT).Value)
                drSpreadMoney(PARLAYREVERSE) = SafeString(oNodes(FIRSTNODE).Attributes(PARLAYREVERSE).Value)
                odtSpreadMoney.Rows.Add(drSpreadMoney)
                Return odtSpreadMoney
            Catch ex As Exception
                Return odtSpreadMoney
            End Try
            Return odtSpreadMoney
        End Function

        Public Function GetValue2HGameTypeOnOff(ByVal poAgentID As String) As Integer
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim sXMLPath As String = "root/SecondHGame/GameTypeOnOff"
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            Try
                oDoc.Load(oHandle.LocalFileName)
                Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                Return SafeInteger(oNodes(FIRSTNODE).Attributes(OFFBEFORE).Value)
            Catch ex As Exception
                Return 0
            End Try
            Return 0
        End Function

        Public Function GetValueParplaySetup(ByVal poAgentID As String) As Integer
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim sXMLPath As String = "//ParplayMaxSelection/NumParplaySetup"
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            Try
                oDoc.Load(oHandle.LocalFileName)
                Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                Return SafeInteger(oNodes(FIRSTNODE).Attributes(PARPLAY_MAX_SELECTION).Value)
            Catch ex As Exception
                Return -1
            End Try
            Return -1
        End Function

        Public Function Get2HGameTypeOnOff(ByVal poAgentID As String) As Integer
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim sXMLPath As String = "root/SecondHGame"
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            Try
                oDoc.Load(oHandle.LocalFileName)
                Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                Return oNodes.Count
            Catch ex As Exception
                Return 0
            End Try
            Return 0
        End Function

        Public Function GetFixSpreadMoney(ByVal poAgentID As String) As Integer
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim sXMLPath As String = "root/FixedSpreadMoney"
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            Try
                oDoc.Load(oHandle.LocalFileName)
                Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                Return oNodes.Count
            Catch ex As Exception
                LogError(log, "Cannot get Fixed Spread Money.", ex)
                Return 0
            End Try
            Return 0
        End Function

        Public Function GetCircleGame(ByVal poAgentID As String) As Integer
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim sXMLPath As String = "root/GameCircle"
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            Try
                oDoc.Load(oHandle.LocalFileName)
                Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                Return oNodes.Count
            Catch ex As Exception
                Return 0
            End Try
            Return 0
        End Function

        Public Function GetOddRule(ByVal poAgentID As String) As Integer
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim sXMLPath As String = "root/OddRule"
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            Try
                oDoc.Load(oHandle.LocalFileName)
                Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                Return oNodes.Count
            Catch ex As Exception
                Return 0
            End Try
            Return 0
        End Function

        Public Function GetRestriction(ByVal poAgentID As String) As Integer
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim sXMLPath As String = "root/GameLineRestriction"
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            Try
                oDoc.Load(oHandle.LocalFileName)
                Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                Return oNodes.Count
            Catch ex As Exception
                Return 0
            End Try
            Return 0
        End Function

        Public Function GetGameHTimeDisplay(ByVal poAgentID As String) As CGameHalfTimeDisplayList
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim oGameHalfTimeDisplayList As New CGameHalfTimeDisplayList
            Dim sXMLPath As String = "root/GameHalfTimeDisplay/GameDisplaySetup"
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            Try
                oDoc.Load(oHandle.LocalFileName)
                Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                For Each oNode As XmlNode In oNodes
                    Dim oCGameHalfTimeDisplay As New CGameHalfTimeDisplay(oNode.Attributes(SPORTTYPE).Value, SafeInteger(oNode.Attributes(SECONDHOFF).Value))
                    oGameHalfTimeDisplayList.Add(oCGameHalfTimeDisplay)
                Next
                Return oGameHalfTimeDisplayList
            Catch ex As Exception
                log.Error("cannot get Game off ", ex)
                Return oGameHalfTimeDisplayList
            End Try
            Return oGameHalfTimeDisplayList
        End Function

#Region "Quarter Display Setting"

        Public Function GetGameTeamDisplay(ByVal poAgentID As String, ByVal psSportType As String) As CGameTeamDisplayList
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim oGameTeamDisplayList As New CGameTeamDisplayList()
            Dim sXMLPath As String = String.Format("root/GameQuarterDisPlay/GameQuarterDisplaySetup[@SportType='{0}']", psSportType)
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            Try
                oDoc.Load(oHandle.LocalFileName)

                Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                For Each oNode As XmlNode In oNodes
                    Dim oGameTeamDisplay As New CGameTeamDisplay(oNode.Attributes(SPORTTYPE).Value, SafeInteger(oNode.Attributes(GAMETIMEOFFLINE).Value), SafeInteger(oNode.Attributes(GAMETIMEDISPLAY).Value))
                    oGameTeamDisplayList.Add(oGameTeamDisplay)
                Next
                Return oGameTeamDisplayList
            Catch ex As Exception
                log.Error("cannot get Game off ", ex)
                Return oGameTeamDisplayList
            End Try
            Return oGameTeamDisplayList
        End Function

        Public Function UpdateGameTeamDisplayByAgent(ByVal poAgentID As String, ByVal psSportType As String, ByVal pnGameTimeOffLine As Integer, ByVal pnGameTimeDisPlay As Integer) As Boolean

            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            If Not oHandle Is Nothing Then
                Try
                    oDoc.Load(oHandle.LocalFileName)
                    Dim sXMLPath As String = String.Format("root/GameQuarterDisPlay/GameQuarterDisplaySetup[@SportType='{0}']", psSportType)
                    Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                    oNodes(FIRSTNODE).Attributes(SPORTTYPE).Value = SafeString(psSportType)
                    oNodes(FIRSTNODE).Attributes(GAMETIMEOFFLINE).Value = SafeString(pnGameTimeOffLine)
                    oNodes(FIRSTNODE).Attributes(GAMETIMEDISPLAY).Value = SafeString(pnGameTimeDisPlay)
                    oDoc.Save(oHandle.LocalFileName)
                    oDB.PutFile(FILESYSTEM & "\" & poAgentID, FILE_NAME, oHandle)
                Catch ex As Exception
                    LogError(log, "Cannot update game team display setup", ex)
                    Return False
                End Try
            End If
            Return True
        End Function

        Public Function InsertGameQuarterDisplayByAgent(ByVal poAgentID As String, ByVal psID As String, ByVal psSportType As String, ByVal pnGameTimeOffLine As Integer, ByVal pnGameTimeDisplay As Integer) As Boolean
            Try
                Dim bEmptyData As Boolean = False
                Dim bCreateNewNode As Boolean = False
                Dim oDoc As New XmlDocument()
                Dim oDB As New FileDB.CFileDB()
                Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
                Dim sXMLPath As String = "root/GameQuarterDisPlay"
                Try
                    oDoc.Load(oHandle.LocalFileName)
                    If oDoc.SelectNodes(sXMLPath).Count > 0 Then
                        bCreateNewNode = False
                    Else
                        bCreateNewNode = True
                    End If
                Catch ex As Exception
                    bEmptyData = True
                End Try
                If bCreateNewNode Then
                    oHandle = oDB.GetNewFileHandle()
                End If

                Dim oSportTypeOnOff As XmlNode = oDoc.CreateElement("GameQuarterDisplaySetup")

                Dim oSportType As XmlAttribute = oDoc.CreateAttribute(SPORTTYPE)
                oSportType.Value = psSportType

                Dim oGameTimeOffLine As XmlAttribute = oDoc.CreateAttribute(GAMETIMEOFFLINE)
                oGameTimeOffLine.Value = pnGameTimeDisplay.ToString

                Dim oGametimeDisplay As XmlAttribute = oDoc.CreateAttribute(GAMETIMEDISPLAY)
                oGametimeDisplay.Value = SafeString(pnGameTimeOffLine)

                oSportTypeOnOff.Attributes.Append(oSportType)
                oSportTypeOnOff.Attributes.Append(oGameTimeOffLine)
                oSportTypeOnOff.Attributes.Append(oGametimeDisplay)

                Dim oAgentID As XmlAttribute = oDoc.CreateAttribute("ID")
                oAgentID.Value = psID
                oSportTypeOnOff.Attributes.Append(oAgentID)

                If bEmptyData Then
                    Dim oCurrentGame As XmlNode = oDoc.CreateElement(GAMETEAMDISPLAY)
                    oCurrentGame.AppendChild(oSportTypeOnOff)
                    IO.File.WriteAllText(oHandle.LocalFileName, String.Format("<?xml version=""1.0"" encoding=""UTF-8""?><root>{0}</root>", oCurrentGame.OuterXml))
                    GoTo saveFile
                End If
                If bCreateNewNode Then
                    Dim oCurrentGame As XmlNode = oDoc.CreateElement(GAMETEAMDISPLAY)
                    oCurrentGame.AppendChild(oSportTypeOnOff)
                    oDoc.DocumentElement.AppendChild(oCurrentGame)
                    oDoc.Save(oHandle.LocalFileName)
                Else
                    oDoc.SelectSingleNode(sXMLPath).AppendChild(oSportTypeOnOff)
                    oDoc.Save(oHandle.LocalFileName)
                End If
saveFile:
                oDB.PutFile(FILESYSTEM & "\" & poAgentID, FILE_NAME, oHandle)

            Catch ex As Exception
                log.Error("Fail to insert Agent Setting Game", ex)
                Return False
            End Try

            Return True
        End Function


        Public Function InsertGameQuarterOffByAgent(ByVal poAgentID As String, ByVal psID As String, ByVal psSportType As String, ByVal psContext As String, ByVal pbOff As Boolean) As Boolean
            Try
                Dim bEmptyData As Boolean = False
                Dim bCreateNewNode As Boolean = False
                Dim oDoc As New XmlDocument()
                Dim oDB As New FileDB.CFileDB()
                Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
                Dim sXMLPath As String = "root/GameQuarterOff"
                Try
                    oDoc.Load(oHandle.LocalFileName)
                    If oDoc.SelectNodes(sXMLPath).Count > 0 Then
                        bCreateNewNode = False
                    Else
                        bCreateNewNode = True
                    End If
                Catch ex As Exception
                    bEmptyData = True
                End Try
                If bCreateNewNode Then
                    oHandle = oDB.GetNewFileHandle()
                End If

                Dim oSportTypeQuarterOnOff As XmlNode = oDoc.CreateElement("GameQuarterOffSetup")

                Dim oSportType As XmlAttribute = oDoc.CreateAttribute(SPORTTYPE)
                oSportType.Value = psSportType

                Dim oContext As XmlAttribute = oDoc.CreateAttribute(CONTEXT)
                oContext.Value = psContext.ToString

                Dim oGametimeOff As XmlAttribute = oDoc.CreateAttribute(OFFLINE)
                oGametimeOff.Value = pbOff.ToString()
                oSportTypeQuarterOnOff.Attributes.Append(oSportType)
                oSportTypeQuarterOnOff.Attributes.Append(oContext)
                oSportTypeQuarterOnOff.Attributes.Append(oGametimeOff)

                Dim oAgentID As XmlAttribute = oDoc.CreateAttribute("ID")
                oAgentID.Value = psID
                oSportTypeQuarterOnOff.Attributes.Append(oAgentID)

                If bEmptyData Then
                    Dim oCurrentGame As XmlNode = oDoc.CreateElement(GAMETIMEOFFLINE)
                    oCurrentGame.AppendChild(oSportTypeQuarterOnOff)
                    IO.File.WriteAllText(oHandle.LocalFileName, String.Format("<?xml version=""1.0"" encoding=""UTF-8""?><root>{0}</root>", oCurrentGame.OuterXml))
                    GoTo saveFile
                End If
                If bCreateNewNode Then
                    Dim oCurrentGame As XmlNode = oDoc.CreateElement(GAMEQUARTEROFF)
                    oCurrentGame.AppendChild(oSportTypeQuarterOnOff)
                    oDoc.DocumentElement.AppendChild(oCurrentGame)
                    oDoc.Save(oHandle.LocalFileName)
                Else
                    oDoc.SelectSingleNode(sXMLPath).AppendChild(oSportTypeQuarterOnOff)
                    oDoc.Save(oHandle.LocalFileName)
                End If
saveFile:
                oDB.PutFile(FILESYSTEM & "\" & poAgentID, FILE_NAME, oHandle)

            Catch ex As Exception
                log.Error("Fail to insert Agent Setting Game", ex)
                Return False
            End Try

            Return True
        End Function


        Public Function UpdateGameQuarterOffByAgent(ByVal poAgentID As String, ByVal psSportType As String, ByVal psContext As String, ByVal pbOff As Boolean) As Boolean
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            If Not oHandle Is Nothing Then
                Try
                    oDoc.Load(oHandle.LocalFileName)
                    Dim sXMLPath As String = String.Format("//GameQuarterOff/GameQuarterOffSetup[@SportType='{0}' and @Context='{1}']", psSportType, psContext)
                    Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                    oNodes(FIRSTNODE).Attributes(OFFLINE).Value = SafeString(pbOff.ToString())
                    oDoc.Save(oHandle.LocalFileName)
                    oDB.PutFile(FILESYSTEM & "\" & poAgentID, FILE_NAME, oHandle)
                Catch ex As Exception
                    log.Error("Fail to update  GameHalfTime Display setup Setting Game", ex)
                    Return False
                End Try
            End If
            Return True
        End Function

        Public Function GetGameQuarterOffByAgent(ByVal poAgentID As String, ByVal psSportType As String, ByVal psContext As String) As Boolean
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim oGameTeamDisplayList As New CGameTeamDisplayList()
            Dim sXMLPath As String = String.Format("//GameQuarterOff/GameQuarterOffSetup[@SportType='{0}' and @Context='{1}']", psSportType, psContext)
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            Try
                oDoc.Load(oHandle.LocalFileName)

                Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                If oNodes IsNot Nothing And oNodes.Count > 0 Then
                    Return SafeBoolean(oNodes(FIRSTNODE).Attributes(OFFLINE).Value)
                End If

            Catch ex As Exception
                log.Error("cannot get Game Quarter off ", ex)
                Return False
            End Try
            Return False
        End Function

#End Region

        Public Function CheckExistsGameQuarterOffByAgent(ByVal poAgentID As String, ByVal psSportType As String, ByVal psContext As String) As Boolean
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim sXMLPath As String = String.Format("root/GameQuarterOff/GameQuarterOffSetup[@SportType='{0}' ]", psSportType)
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            Try
                oDoc.Load(oHandle.LocalFileName)
                Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                If oNodes IsNot Nothing AndAlso oNodes.Count > 0 Then
                    Return True
                Else
                    Return False
                End If
            Catch ex As Exception
                log.Error("cannot check exists Game quarter off ", ex)
                Return False
            End Try
            Return False
        End Function

        Public Function CheckExistsJuiceControlByAgent(ByVal poAgentID As String, ByVal psSportType As String, ByVal psContext As String) As Boolean
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim sXMLPath As String = String.Format("root/JuiceControl/JuiceControlSetup[@SportType='{0}' ]", psSportType)
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            Try
                oDoc.Load(oHandle.LocalFileName)
                Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                If oNodes IsNot Nothing AndAlso oNodes.Count > 0 Then
                    Return True
                Else
                    Return False
                End If
            Catch ex As Exception
                log.Error("cannot check exists Game Juice Control ", ex)
                Return False
            End Try
            Return False
        End Function


        Public Function CheckExistsGameHTimeOff(ByVal poAgentID As String, ByVal psGameType As String) As Boolean
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim sXMLPath As String = String.Format("root/GameHalfTimeOff/GameOffSetup[@GameType='{0}']", psGameType)
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            Try
                oDoc.Load(oHandle.LocalFileName)
                Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                If oNodes IsNot Nothing AndAlso oNodes.Count > 0 Then
                    Return True
                Else
                    Return False
                End If
            Catch ex As Exception
                log.Error("cannot check exists Game off ", ex)
                Return False
            End Try
            Return False
        End Function

        Public Function CheckExistsGameHTimeDisplay(ByVal poAgentID As String, ByVal psSportType As String) As Boolean
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim sXMLPath As String = String.Format("root/GameHalfTimeDisplay/GameDisplaySetup[@SportType='{0}']", psSportType)
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            Try
                oDoc.Load(oHandle.LocalFileName)
                Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                If oNodes IsNot Nothing AndAlso oNodes.Count > 0 Then
                    Return True
                Else
                    Return False
                End If
            Catch ex As Exception
                log.Error("cannot check exists Game Display ", ex)
                Return False
            End Try
            Return False
        End Function

        Public Function GetGameHTimeOff(ByVal poAgentID As String) As CGameHalfTimeOffList
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim oGameHalfTimeOffList As New CGameHalfTimeOffList()
            Dim sXMLPath As String = "root/GameHalfTimeOff/GameOffSetup"
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            Try
                oDoc.Load(oHandle.LocalFileName)
                Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                'oGameHalfTimeOffList
                For Each oNode As XmlNode In oNodes
                    If oNode.Attributes(TEAMTOTALOFF) IsNot Nothing Then
                        Dim oCGameHalfTimeOff As New CGameHalfTimeOFF(oNode.Attributes(GAMETYPE).Value, SafeBoolean(oNode.Attributes(FIRSTHOFF).Value), SafeBoolean(oNode.Attributes(SECONDHOFF).Value), SafeBoolean(oNode.Attributes(TEAMTOTALOFF).Value))
                        oGameHalfTimeOffList.Add(oCGameHalfTimeOff)
                    Else
                        Dim oCGameHalfTimeOff As New CGameHalfTimeOFF(oNode.Attributes(GAMETYPE).Value, SafeBoolean(oNode.Attributes(FIRSTHOFF).Value), SafeBoolean(oNode.Attributes(SECONDHOFF).Value), False)
                        oGameHalfTimeOffList.Add(oCGameHalfTimeOff)
                    End If

                Next
                Return oGameHalfTimeOffList
            Catch ex As Exception
                log.Error("cannot get Game off ", ex)
                Return oGameHalfTimeOffList
            End Try
            Return oGameHalfTimeOffList
        End Function


        Public Function GetMoneyLineOff(ByVal poAgentID As String) As Integer
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim sXMLPath As String = "root/MoneyLineOff"
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            Try
                oDoc.Load(oHandle.LocalFileName)
                Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                Return oNodes.Count
            Catch ex As Exception
                Return 0
            End Try
            Return 0
        End Function

        Public Function DeleteOddsRulesByID(ByVal poAgentID As String, ByVal psID As String) As Boolean

            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            If Not oHandle Is Nothing Then
                Try
                    oDoc.Load(oHandle.LocalFileName)
                    Dim sXMLPath As String = String.Format("//OddRule/Condition[@ID='{0}']", psID)
                    Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                    oNodes(FIRSTNODE).ParentNode.RemoveChild(oNodes(FIRSTNODE))
                    oDoc.Save(oHandle.LocalFileName)
                    oDB.PutFile(FILESYSTEM & "\" & poAgentID, FILE_NAME, oHandle)
                Catch ex As Exception
                    Return False
                End Try
            End If
            Return True
        End Function

        Public Function DeleteXML(ByVal poAgentID As String, ByVal psID As String, ByVal psCategory As String, ByVal psKey As String) As Boolean
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            If Not oHandle Is Nothing Then
                Try
                    oDoc.Load(oHandle.LocalFileName)
                    Dim sXMLPath As String = String.Format("//{1}/{2}[@ID='{0}']", psID, psCategory, psKey)
                    Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                    oNodes(FIRSTNODE).ParentNode.RemoveChild(oNodes(FIRSTNODE))
                    oDoc.Save(oHandle.LocalFileName)
                    oDB.PutFile(FILESYSTEM & "\" & poAgentID, FILE_NAME, oHandle)
                Catch ex As Exception
                    Return False
                End Try
            End If
            Return True
        End Function

        Public Function DeleteXML(ByVal poAgentID As String, ByVal psCategory As String, ByVal psKey As String) As Boolean
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM & "\" & poAgentID, FILE_NAME)
            If Not oHandle Is Nothing Then
                Try
                    oDoc.Load(oHandle.LocalFileName)
                    Dim sXMLPath As String = String.Format("//{0}/{1}", psCategory, psKey)
                    Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                    oNodes(FIRSTNODE).ParentNode.RemoveChild(oNodes(FIRSTNODE))
                    oDoc.Save(oHandle.LocalFileName)
                    oDB.PutFile(FILESYSTEM & "\" & poAgentID, FILE_NAME, oHandle)
                Catch ex As Exception
                    Return False
                End Try
            End If
            Return True
        End Function

    End Class
End Namespace