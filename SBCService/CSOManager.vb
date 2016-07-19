Imports SBCBL.std
Imports SBCService.CServiceStd
Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml
Imports WebsiteLibrary.DBUtils
Imports SBCBL.Managers
Imports SBCBL.CacheUtils

Public Class CSOManager
    'Private _log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

    Private _oSocket As Socket
    ' Size of receive buffer.
    Public Const BufferSize As Integer = 256
    ' Receive buffer.
    Private _oBuffer(BufferSize) As Byte
    ' Received data string.
    Private _oSB As New StringBuilder

    Private _oTeamMapDocument As XmlDocument = Nothing
    Private _oLastTimeout As DateTime = DateTime.MinValue
    Private _SportOptionGameTypes As Dictionary(Of String, String)

    Private ReadOnly Property SportOptionGameTypes() As Dictionary(Of String, String)
        Get
            If _SportOptionGameTypes Is Nothing Then
                _SportOptionGameTypes = New Dictionary(Of String, String)()
                _SportOptionGameTypes.Add("1", "NFL Football")
                _SportOptionGameTypes.Add("2", "NCAA Football")
                _SportOptionGameTypes.Add("3", "NBA Basketball")
                _SportOptionGameTypes.Add("4", "NCAA Basketball")
                _SportOptionGameTypes.Add("5", "MLB Baseball")
                _SportOptionGameTypes.Add("6", "NCAA Baseball")
                _SportOptionGameTypes.Add("7", "NHL Hockey")
                _SportOptionGameTypes.Add("8", "WNBA Basketball")
                _SportOptionGameTypes.Add("39|9|38|29|40", "Soccer")
                _SportOptionGameTypes.Add("10", "Boxing")
                _SportOptionGameTypes.Add("11", "Golf")
                _SportOptionGameTypes.Add("12", "Tennis")
                _SportOptionGameTypes.Add("13", "AFL Football")
                _SportOptionGameTypes.Add("15", "NCAA Hockey")
                _SportOptionGameTypes.Add("17", "WNCAA Basketball")
                _SportOptionGameTypes.Add("18", "CFL Football")
                _SportOptionGameTypes.Add("20", "MLB Baseball")
                _SportOptionGameTypes.Add("21", "MLB Baseball")
                _SportOptionGameTypes.Add("22", "MLS")
                _SportOptionGameTypes.Add("23", "Premier")
                _SportOptionGameTypes.Add("24", "Serie A")
                _SportOptionGameTypes.Add("25", "La Liga")
                _SportOptionGameTypes.Add("29", "Mexican")
                _SportOptionGameTypes.Add("30", "Bundesliga")
                _SportOptionGameTypes.Add("31", "Ligue 1")
                _SportOptionGameTypes.Add("32", "Netherlands")
                _SportOptionGameTypes.Add("34", "Argentina")
            End If
            Return _SportOptionGameTypes
        End Get
    End Property

    '' map by sportsbook_id
    Dim _SportOptionBookmakers As Dictionary(Of String, String)
    Private ReadOnly Property SportOptionBookmakers() As Dictionary(Of String, String)
        Get
            If _SportOptionBookmakers Is Nothing Then
                _SportOptionBookmakers = New Dictionary(Of String, String)()
                _SportOptionBookmakers.Add("CRIS", "1")
                _SportOptionBookmakers.Add("BetJamaica", "9")
                _SportOptionBookmakers.Add("5Dimes", "14")
                _SportOptionBookmakers.Add("Pinnacle", "17")
                _SportOptionBookmakers.Add("Bet Phoenix", "43")
                _SportOptionBookmakers.Add("Bodog", "5")
            End If
            Return _SportOptionBookmakers
        End Get
    End Property

    '' this property is using for map game context from sportoption to our DB
    Dim _oGameContext As Dictionary(Of String, String)
    Public ReadOnly Property GameContexts() As Dictionary(Of String, String)
        Get
            If _oGameContext Is Nothing Then
                _oGameContext = New Dictionary(Of String, String)
                _oGameContext.Add("GAME", "Current")
                _oGameContext.Add("FIRST_HALF", "1H")
                _oGameContext.Add("SECOND_HALF", "2H")
                _oGameContext.Add("TEAM_TOTAL", "Team Total")

                _oGameContext.Add("FIRST_QUARTER", "1Q")
                _oGameContext.Add("SECOND_QUARTER", "2Q")
                _oGameContext.Add("THIRD_QUARTER", "3Q")
                _oGameContext.Add("FOURTH_QUARTER", "4Q")
            End If
            Return _oGameContext
        End Get
    End Property

    Public Sub BeginConnect(ByVal psIP As String, ByVal pnPort As Integer)
        Dim oEndPoint As New IPEndPoint(IPAddress.Parse(psIP), pnPort)
        _oSocket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        Dim oIP As IPAddress = IPAddress.Parse(psIP)
        Dim oEPoint As IPEndPoint = New IPEndPoint(oIP, pnPort)

        _oSocket.Connect(oEPoint)

        '' Send UserName & Password First
        Dim sMessageSend As String = SPORTOPTIONS_USER & ControlChars.Lf & SPORTOPTIONS_PASSWORD & ControlChars.Lf & SPORTOPTIONS_VERSION & ControlChars.Lf
        SendMessage(sMessageSend)

        '' Exec Receive from Server response
        ' This will loop forever
        ExecReceived()
    End Sub

    Public Sub EndConnect()
        If _oSocket IsNot Nothing Then
            '_log.Info("Some problems occur. Send EXIT command to close connection")
            _oSocket.Close()
        End If
    End Sub

    Private Sub SendMessage(ByVal psMessage As String)
        Dim oBufferSend As Byte() = System.Text.Encoding.ASCII.GetBytes(psMessage)
        If _oSocket IsNot Nothing AndAlso _oSocket.Connected Then
            _oSocket.Send(oBufferSend, oBufferSend.Length, 0)
        End If
    End Sub

    Public Sub KeepConnection()
        '' Send X Command to Server to keep connection
        SendMessage("X" & ControlChars.Lf)
        '_log.Info("Send X Command to Server to keep connection")
    End Sub

    Private Function BeginReceive() As Boolean

        Dim nByteReceives As Integer
        nByteReceives = _oSocket.Receive(_oBuffer, 0, BufferSize, SocketFlags.None)
        Dim sReceived As String = Encoding.ASCII.GetString(_oBuffer, 0, nByteReceives)
        '_log.Debug("Last Message: " & sReceived)

        If Regex.IsMatch(sReceived, "TIME_SYNC\n[0-9]{13}\n") OrElse Regex.IsMatch(sReceived, "X\n") Then
            sReceived = Regex.Replace(sReceived, "TIME_SYNC\n[0-9]{13}\n", "")
            sReceived = Regex.Replace(sReceived, "X\n", "")
        End If

        _oSB.Append(sReceived)

        ' Exec Receive Message
        Select Case True
            Case sReceived = "" '' Received TIME_SYNC OR X
                Return False

            Case sReceived.EndsWith("VALID" & ControlChars.Lf), sReceived.EndsWith("GET_SCHEDULE" & ControlChars.Lf)
                Return False

            Case sReceived.EndsWith("</Schedule>" & ControlChars.Lf)
                '' Exec Receive Schedule XML
                Return False

            Case sReceived.EndsWith("</Events>" & ControlChars.Lf)
                '' Exec Receive Events XML
                Return False

            Case sReceived.EndsWith("</Scores>" & ControlChars.Lf)
                '' Exec Receive Scores XML
                Return False

            Case sReceived.EndsWith("</Changes>" & ControlChars.Lf)
                '' Exec Receive Changes XML
                Return False

            Case sReceived.EndsWith("</Lines>" & ControlChars.Lf)
                '' Exec Receive Lines XML
                Return False

            Case sReceived.EndsWith("EXIT" & ControlChars.Lf)
                '_log.Info("Unexec XML: " & _oSB.ToString())

                '' Lost connection from server
                Throw New SportOptionDisconnectException("Disconnect from server")

            Case sReceived.EndsWith("INVALID" & ControlChars.Lf)
                '' Wrong Login name or password
                Throw New SportOptionUserNameException("Wrong UserName or Password")

            Case Else
                Return True

        End Select

    End Function

    ''' <summary>
    ''' This function is used to read full XML response from server
    ''' </summary>
    ''' <returns>False: Read Full XML | True: Do not End XML Response</returns>
    ''' <remarks></remarks>
    Private Function EndReceive() As String
        Dim sResult As String = ""

        While BeginReceive()
            '' Continue Listen to Server to the end of XML          
        End While

        sResult = _oSB.ToString()

        '' Reset for later receive
        _oSB = New StringBuilder()

        sResult = Regex.Replace(sResult, "TIME_SYNC\n[0-9]{13}\n", "")
        sResult = Regex.Replace(sResult, "X\n", "")
        sResult = Regex.Replace(sResult, "\n", "")

        Return sResult
    End Function

    Private Sub ExecReceived()

        While True
            Dim sFullReceived As String = EndReceive()

            ' Exec Receive Message
            Select Case True
                Case sFullReceived.EndsWith("VALID"), sFullReceived.EndsWith("GET_SCHEDULE")
                    '_log.Debug("Unexec XML: " & sFullReceived)

                    '' Exec new schedule here
                    ' Send GET_SCHEDULE command to get today's schedule
                    SendMessage("GET_SCHEDULE" & ControlChars.Lf)

                    '' Exec Receive XML
                    ExecXML(EndReceive())

                    '' GET_SB_LINES for each Bookmaker
                    ' Create list of Bookmaker
                    Dim oLstBookMakers() As EBookMaker = {EBookMaker.Cris, EBookMaker.BetPhoenix, EBookMaker.FiveDimes, EBookMaker.BetJamaica, EBookMaker.Pinnacle, EBookMaker.Bodog}
                    For Each eBookMaker As EBookMaker In oLstBookMakers
                        SendMessage("GET_SB_LINES" & ControlChars.Lf & eBookMaker & ControlChars.Lf)

                        '' Exec Receive XML
                        ExecXML(EndReceive())
                    Next

                Case Else
                    '' Exec Receive XML
                    ExecXML(sFullReceived)

            End Select
        End While

    End Sub

    Private Sub ExecXML(ByVal psReceiveXML As String)
        '_log.Info("Full XML Received: " & psReceiveXML)

        Dim nSIndex As Integer
        Dim sExecXML As String

        If psReceiveXML.IndexOf("<Schedule ") >= 0 Then
            '' Exec Receive Schedule XML
            nSIndex = psReceiveXML.IndexOf("<Schedule ")

            ' Schedule XML
            sExecXML = psReceiveXML.Substring(nSIndex, psReceiveXML.LastIndexOf("</Schedule>") - nSIndex + 11)
            '_log.Info("Full Schedule XML Received: " & sExecXML)

            ExecSportOptionSchedule(sExecXML)

        End If

        If psReceiveXML.IndexOf("<Lines>") >= 0 Then
            '' Exec Receive Lines XML
            nSIndex = psReceiveXML.IndexOf("<Lines>")

            ' Lines XML
            sExecXML = psReceiveXML.Substring(nSIndex, psReceiveXML.LastIndexOf("</Lines>") - nSIndex + 8)
            '_log.Info("Full Lines XML Received from server: " & sExecXML)

            ExecSportOptionLineFeed(sExecXML)

        End If

        If psReceiveXML.IndexOf("<Events>") >= 0 Then
            '' Exec Receive Events XML
            nSIndex = psReceiveXML.IndexOf("<Events>")

            ' Events XML
            sExecXML = psReceiveXML.Substring(nSIndex, psReceiveXML.LastIndexOf("</Events>") - nSIndex + 9)
            '_log.Info("Full Lines XML Received: " & sExecXML)

            ExecSportOptionSchedule(sExecXML)

        End If

        If psReceiveXML.IndexOf("<Scores>") >= 0 Then
            '' Exec Receive Scores XML
            nSIndex = psReceiveXML.IndexOf("<Scores>")

            ' Scores XML
            sExecXML = psReceiveXML.Substring(nSIndex, psReceiveXML.LastIndexOf("</Scores>") - nSIndex + 9)
            '_log.Info("Full Lines XML Received: " & sExecXML)

            ExecSportOptionSchedule(sExecXML)

        End If

        If psReceiveXML.IndexOf("<Changes>") >= 0 Then
            '' Exec Receive Changes XML
            nSIndex = psReceiveXML.IndexOf("<Changes>")

            ' Changes XML
            sExecXML = psReceiveXML.Substring(nSIndex, psReceiveXML.LastIndexOf("</Changes>") - nSIndex + 10)
            '_log.Info("Full Changes XML Received: " & sExecXML)

            ExecSportOptionLineFeed(sExecXML)
        End If

    End Sub

    Public Sub ExecSportOptionLineFeed(ByVal psXML As String)
        '_log.Debug("Processing Line XML: " & psXML)
        Dim oRoot As New XmlDocument()
        Try
            oRoot.LoadXml("<LineBigData></LineBigData>")
            oRoot.DocumentElement.InnerXml = psXML
        Catch ex As Exception
            '' invalid xml data now, just return  
            '_log.Error("invalid xml data now: " & ex.Message, ex)
            '_log.Error("invalid xml data: " & psXML)
            Return
        End Try

        Dim oDB As New WebsiteLibrary.DBUtils.CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

        Dim oToday As DateTime = SafeDate(DateTime.Now.ToShortDateString()).ToUniversalTime().AddHours(-5)
        If (Now.IsDaylightSavingTime()) Then
            oToday = oToday.AddHours(1)
        End If

        Dim sSQL As String = "select * from Games where gameDate >= " & SQLString(oToday)
        '_log.Debug("SQL for get SportOption Games: " & sSQL)

        Try
            Dim oDT As DataTable = oDB.getDataTable(sSQL)
            For Each oRow As DataRow In oDT.Rows
                If SafeString(oRow("IsForProp")) = "Y" Then
                    'ProcessPropLine(oRow, oRoot, oDB)
                    ' remove game series now
                Else
                    ProcessGameLine(oRow, oRoot, oDB)
                End If
            Next
        Catch ex As Exception
            '_log.Error("Error at ExecSportOptionLineFeed: " & ex.Message, ex)
        Finally
            oDB.closeConnection()
        End Try
    End Sub

    ''' <summary>
    ''' Seek the XML DOC and process gameline for games
    ''' </summary>
    ''' <param name="poRow"></param>
    ''' <param name="poRoot"></param>
    ''' <param name="poDB"></param>
    ''' <remarks></remarks>
    Private Sub ProcessGameLine(ByVal poRow As DataRow, ByVal poRoot As XmlDocument, ByVal poDB As CSQLDBUtils)
        '' in SPORTOPTION, game number = awaynss, we can use this relation for get data
        Dim sAwayNss As String = SafeString(poRow("AwayRotationNumber"))
        Dim sHomeNss As String = SafeString(poRow("HomeRotationNumber"))
        Dim sDrawNss As String = SafeString(poRow("DrawRotationNumber"))
        Dim sGameType As String = SafeString(poRow("GameType"))
        'Dim sGameContext As String = SafeString(poRow("Context"))

        Dim oGame As New CGame()
        oGame.GameType = sGameType
        Dim sXPathTeamLine As String = "LineBigData/Lines/Event_Team_Line[@event_number='" & sAwayNss & "' and @is_latest='true']"
        Dim sXPathLine As String = "LineBigData/Lines/Event_Line[@number='" & sAwayNss & "' and @is_latest='true']"
        Dim sXPathChangesTeamLines As String = "LineBigData/Changes/Event_Team_Line[@event_number='" & sAwayNss & "']"
        Dim sXPathChangesLines As String = "LineBigData/Changes/Event_Line[@number='" & sAwayNss & "']"

        Dim sFeedSource As String = "sportoption"
        Dim sBookerName As String = ""
        Dim sContext As String = ""
        Dim nLinceCount As Integer = 0

        '' get all gameline in XML for this game record
        Dim oTeamLineNodes As XmlNodeList = poRoot.SelectNodes(sXPathTeamLine)
        Dim oLineNodes As XmlNodeList = poRoot.SelectNodes(sXPathLine)
        Dim oChangesLines As XmlNodeList = poRoot.SelectNodes(sXPathChangesLines)
        Dim oChangesTeamLines As XmlNodeList = poRoot.SelectNodes(sXPathChangesTeamLines)
        Dim oTeamLineJoin As New List(Of XmlElement)()
        Dim oLineJoin As New List(Of XmlElement)()

        Dim nTotalGameLine As Integer = oTeamLineNodes.Count + oLineNodes.Count + oChangesLines.Count + oChangesTeamLines.Count

        If nTotalGameLine = 0 Then
            '' there is go hame line for this game, return            
            Return
        End If
        '' Merge  element from <Lines> and <Changes> to one list
        For Each oLineNode As XmlElement In oLineNodes
            oLineJoin.Add(oLineNode)
        Next
        For Each oLineNode As XmlElement In oChangesLines
            oLineJoin.Add(oLineNode)
        Next

        For Each oLineNode As XmlElement In oTeamLineNodes
            oTeamLineJoin.Add(oLineNode)
        Next
        For Each oLineNode As XmlElement In oChangesTeamLines
            oTeamLineJoin.Add(oLineNode)
        Next

        '' processing game line, generally, we have 3 types of Event_Line: GAME, FIRST_HALF, SECOND_HALF
        For Each oLineNode As XmlElement In oLineJoin
            If Not GameContexts.ContainsKey(oLineNode.GetAttribute("line_type")) Then
                Continue For
            End If

            sContext = GameContexts(oLineNode.GetAttribute("line_type"))

            '' ignore if this game Is Tennis and context <> current
            If sGameType = "Tennis" And LCase(sContext) <> "current" Then
                Continue For
            End If

            '' Only save quarter lines for Football and Basketball
            If (Not (IsFootball(sGameType) OrElse IsBasketball(sGameType))) AndAlso _
            (LCase(sContext) = "1q" OrElse LCase(sContext) = "2q" OrElse LCase(sContext) = "3q" OrElse LCase(sContext) = "4q") Then
                Continue For
            End If

            Dim sSportsbook_id As String = oLineNode.GetAttribute("sportsbook_id")
            sBookerName = SportOptionBookmakers.Where(Function(x) x.Value = sSportsbook_id)(0).Key

            Dim oLine As CLine = oGame.GetLine(sFeedSource, sBookerName, sContext)
            oLine.TotalPoints = SafeDouble(oLineNode.GetAttribute("points"))
            oLine.TotalPointsUnderMoney = SafeDouble(oLineNode.GetAttribute("under_money"))
            oLine.TotalPointsOverMoney = SafeDouble(oLineNode.GetAttribute("over_money"))
            ''set bookmaker CRIS for all quarter game 
            If sBookerName.Equals("5Dimes", StringComparison.CurrentCultureIgnoreCase) And (LCase(sContext) = "1q" OrElse LCase(sContext) = "2q" OrElse LCase(sContext) = "3q" OrElse LCase(sContext) = "4q") Then
                For Each osBookmaker As String In SportOptionBookmakers.Keys
                    oLine = oGame.GetLine(sFeedSource, osBookmaker, sContext)
                    oLine.TotalPoints = SafeDouble(oLineNode.GetAttribute("points"))
                    oLine.TotalPointsUnderMoney = SafeDouble(oLineNode.GetAttribute("under_money"))
                    oLine.TotalPointsOverMoney = SafeDouble(oLineNode.GetAttribute("over_money"))
                Next

            End If

        Next

        '' processing game team line, generally, we have same 3 types of Event_Line: GAME, FIRST_HALF, SECOND_HALF
        '' and devide to moneyline and not moneyline
        For Each oTeamLineNode As XmlElement In oTeamLineJoin
            If Not GameContexts.ContainsKey(oTeamLineNode.GetAttribute("line_type")) Then
                Continue For
            End If

            sContext = GameContexts(oTeamLineNode.GetAttribute("line_type"))

            '' ignore if this game Is Tennis and context <> current
            If sGameType = "Tennis" And LCase(sContext) <> "current" Then
                Continue For
            End If

            '' Only save quarter lines for Football and Basketball
            If (Not (IsFootball(sGameType) OrElse IsBasketball(sGameType))) AndAlso _
            (LCase(sContext) = "1q" OrElse LCase(sContext) = "2q" OrElse LCase(sContext) = "3q" OrElse LCase(sContext) = "4q") Then
                Continue For
            End If

            Dim bMoneyLine As Boolean = SafeString(oTeamLineNode.GetAttribute("ML")) = "true"
            Dim sSportsbook_id As String = oTeamLineNode.GetAttribute("sportsbook_id")
            sBookerName = SportOptionBookmakers.Where(Function(x) x.Value = sSportsbook_id)(0).Key

            Dim oLine As CLine = oGame.GetLine(sFeedSource, sBookerName, sContext)
            If oTeamLineNode.ParentNode IsNot Nothing AndAlso oTeamLineNode.ParentNode.Name = "Changes" Then
                oLine.IsChange = True
            End If

            Dim sTeamNumber As String = SafeString(oTeamLineNode.GetAttribute("number"))

            ''update team total get 5dimes for all bookmaker
            If LCase(sContext).Equals("team total", StringComparison.CurrentCultureIgnoreCase) Then
                For Each osBookmaker As String In SportOptionBookmakers.Keys
                    oLine = oGame.GetLine(sFeedSource, osBookmaker, "current")
                    If sTeamNumber = sAwayNss Then
                        oLine.AwayTeamTotalPoints = SafeDouble(oTeamLineNode.GetAttribute("points"))
                        oLine.AwayTeamTotalPointsOverMoney = SafeDouble(oTeamLineNode.GetAttribute("money"))
                        oLine.AwayTeamTotalPointsUnderMoney = SafeDouble(oTeamLineNode.GetAttribute("under_money"))
                    Else
                        oLine.HomeTeamTotalPoints = SafeDouble(oTeamLineNode.GetAttribute("points"))
                        oLine.HomeTeamTotalPointsOverMoney = SafeDouble(oTeamLineNode.GetAttribute("money"))
                        oLine.HomeTeamTotalPointsUnderMoney = SafeDouble(oTeamLineNode.GetAttribute("under_money"))
                    End If
                Next

            End If

            '' processing for moneyline game
            If bMoneyLine Then
                If sTeamNumber = sAwayNss Then
                    '' away team
                    oLine.AwayMoneyLine = SafeDouble(oTeamLineNode.GetAttribute("money"))
                ElseIf sTeamNumber = sHomeNss Then
                    '' home team
                    oLine.HomeMoneyLine = SafeDouble(oTeamLineNode.GetAttribute("money"))
                ElseIf sDrawNss <> "" And sDrawNss <> "-1" And sTeamNumber = sDrawNss Then
                    '_log.Info("FOUND DRAW LINE: XML " & oTeamLineNode.ParentNode.OuterXml)
                    '' draw money
                    Dim nDrawMoney As Double = SafeDouble(oTeamLineNode.GetAttribute("money"))
                    If nDrawMoney <> 0 Then
                        oLine.DrawMoneyLine = nDrawMoney
                    End If
                End If
            ElseIf Not bMoneyLine AndAlso Not LCase(sContext).Equals("team total", StringComparison.CurrentCultureIgnoreCase) Then
                '' not money line team
                If sTeamNumber = sAwayNss Then
                    '' away team
                    oLine.AwaySpread = SafeDouble(oTeamLineNode.GetAttribute("points"))
                    oLine.AwaySpreadMoney = SafeDouble(oTeamLineNode.GetAttribute("money"))
                ElseIf sTeamNumber = sHomeNss Then
                    '' home team
                    oLine.HomeSpread = SafeDouble(oTeamLineNode.GetAttribute("points"))
                    oLine.HomeSpreadMoney = SafeDouble(oTeamLineNode.GetAttribute("money"))
                End If
            End If

            ''set bookmaker CRIS for all quarter game 
            If sBookerName.Equals("5Dimes", StringComparison.CurrentCultureIgnoreCase) And (LCase(sContext) = "1q" OrElse LCase(sContext) = "2q" OrElse LCase(sContext) = "3q" OrElse LCase(sContext) = "4q") Then
                For Each osBookmaker As String In SportOptionBookmakers.Keys
                    Dim _oLine As CLine = oGame.GetLine(sFeedSource, osBookmaker, sContext)
                    _oLine.IsChange = oLine.IsChange
                    '' processing for moneyline game
                    If bMoneyLine Then
                        If sTeamNumber = sAwayNss Then
                            '' away team
                            _oLine.AwayMoneyLine = SafeDouble(oTeamLineNode.GetAttribute("money"))
                        ElseIf sTeamNumber = sHomeNss Then
                            '' home team
                            _oLine.HomeMoneyLine = SafeDouble(oTeamLineNode.GetAttribute("money"))
                        End If
                    ElseIf Not bMoneyLine AndAlso Not LCase(sContext).Equals("team total", StringComparison.CurrentCultureIgnoreCase) Then
                        '' not money line team
                        If sTeamNumber = sAwayNss Then
                            '' away team
                            _oLine.AwaySpread = SafeDouble(oTeamLineNode.GetAttribute("points"))
                            _oLine.AwaySpreadMoney = SafeDouble(oTeamLineNode.GetAttribute("money"))
                        ElseIf sTeamNumber = sHomeNss Then
                            '' home team
                            _oLine.HomeSpread = SafeDouble(oTeamLineNode.GetAttribute("points"))
                            _oLine.HomeSpreadMoney = SafeDouble(oTeamLineNode.GetAttribute("money"))
                            '' process line money, cut off 10 if money >= 130
                            CGameRule.ProcessGameLine(sGameType, _oLine)
                        End If
                    End If
                Next

            End If

            '' process line money, cut off 10 if money >= 130
            CGameRule.ProcessGameLine(sGameType, oLine)
        Next

        '_log.Info("Total gamelines found for Game for Bookmaker: " & sBookerName & ". Game " & SafeString(poRow("AwayTeam")) & " @ " & SafeString(poRow("HomeTeam")) & " : " & SafeString(poRow("GameID")) & " ---- lines: " & nTotalGameLine.ToString())
        '' save gameline after all processing

        oGame.SaveLinesToDB(poDB, SafeString(poRow("GameID")), sGameType)
    End Sub

    Public Sub ExecSportOptionSchedule(ByVal psXML As String)
        '_log.Debug("Processing XML: " & psXML)
        Dim oRoot As New XmlDocument()
        Try
            oRoot.LoadXml("<ScheduleBigData></ScheduleBigData>")
            oRoot.DocumentElement.InnerXml = psXML
        Catch ex As Exception
            '' invalid xml data now, just return  
            '_log.Error("Invalid xml data now: " & ex.Message, ex)
            '_log.Error("Invalid xml data: " & psXML)
            Return
        End Try

        Dim oDB As New WebsiteLibrary.DBUtils.CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

        Try
            '_log.Debug("START PROCESS SPORTOPTION XML RESPONSED DATA. Current Schedule XML: " & psXML)
            For Each oItem As System.Collections.Generic.KeyValuePair(Of String, String) In SportOptionGameTypes
                Dim sGameType As String = oItem.Value
                Dim sLeagueID As String = oItem.Key

                '' Loop all subLeagueID
                For Each sSubLeagueID As String In sLeagueID.Split("|"c)
                    Dim oOuterEvents As XmlNodeList = oRoot.SelectNodes("ScheduleBigData/Events[Event/@subleague_id='" & sSubLeagueID & "']")
                    Dim oScheduleEventGroups As XmlNodeList = oRoot.SelectNodes("ScheduleBigData/Schedule/Groups/Group[@subleague_id ='" & sSubLeagueID & "']")

                    Dim oListEvents As New List(Of XmlNode)
                    Dim oListEuroCups As New List(Of XmlNode)
                    'Dim oListEuroCups As New List(Of XmlNode)
                    '' if league ID is 9, it can be Euro or worldcup or UEFA
                    'If sLeagueID <> "9" Then
                    '' there is just only 1 event nested in <Events>
                    If oOuterEvents IsNot Nothing Then
                        For Each oOuterEvent As XmlNode In oOuterEvents
                            oListEvents.Add(oOuterEvent.ChildNodes(0))
                        Next
                    End If

                    If oScheduleEventGroups IsNot Nothing Then
                        For Each oScheduleEvents As XmlNode In oScheduleEventGroups
                            For Each oEvent As XmlNode In oScheduleEvents.ChildNodes
                                oListEvents.Add(oEvent)
                            Next
                        Next
                    End If

                    ProcessGameGroup(oRoot, sGameType, oListEvents, oDB)
                Next
                
            Next

            ProcessGameScores(oRoot, oDB)
        Catch ex As Exception
            '_log.Error("Error at ExecSportOptionSchedule node process: " & ex.Message, ex)
        Finally
            oDB.closeConnection()
        End Try
    End Sub

    Private Sub ProcessGameGroup(ByVal poDocument As XmlDocument, ByVal psGameType As String, ByVal poListEvents As List(Of XmlNode), ByVal poDB As WebsiteLibrary.DBUtils.CSQLDBUtils)
        '_log.Info("Processing Game Type: " & psGameType & ". Event child node counts: " & poListEvents.Count.ToString())
        For Each oEventNode As XmlNode In poListEvents
            Try
                Dim bIsAdded As Boolean = False
                Dim bIsExtraGame As Boolean = False
                Dim sGameType As String = psGameType
                Dim sDescription As String = ""

                '' Rightn now, just process <Event> node
                If oEventNode.Name <> "Event" Then
                    Continue For
                End If

                '' DO not save Series Prices games now -- ONLY FOR TEMPORARY WORK
                If SafeBoolean(CType(oEventNode, XmlElement).GetAttribute("series_price")) OrElse SafeBoolean(CType(oEventNode, XmlElement).GetAttribute("in_game")) Then
                    Continue For
                End If

                ''  for_prop= true, ignore this!
                If CType(oEventNode, XmlElement).GetAttribute("for_prop") = "true" Then
                    '_log.Info("Found prop game: " & oEventNode.OuterXml)
                    'ProcessGameProp(oEventNode, psGameType, poDB)
                    Continue For
                End If

                ''add description''''
                If oEventNode.ParentNode.Name.Equals("Group", StringComparison.CurrentCultureIgnoreCase) And oEventNode.ParentNode.FirstChild.Name.Equals("Group_Header", StringComparison.CurrentCultureIgnoreCase) Then
                    sDescription = CType(oEventNode.ParentNode.FirstChild, XmlElement).GetAttribute("text")
                End If

                If psGameType = "Soccer" Then
                    If oEventNode.ParentNode.Name = "Group" And CType(oEventNode.ParentNode, XmlElement).GetAttribute("description").Contains("Europa") Then
                        sGameType = "Europa League"
                    End If

                    If oEventNode.ParentNode.Name = "Group" And CType(oEventNode.ParentNode, XmlElement).GetAttribute("description").Contains("UEFA") Then
                        sGameType = "Champions League"
                    End If

                    If oEventNode.ParentNode.Name = "Group" And CType(oEventNode.ParentNode, XmlElement).GetAttribute("description").Contains("Euro ") Then
                        sGameType = "Euro"
                    End If

                    If oEventNode.ParentNode.Name = "Group" AndAlso CType(oEventNode.ParentNode, XmlElement).GetAttribute("description").Contains("World Cup") Then
                        sGameType = "World Cup"
                    End If
                    If oEventNode.ParentNode.Name = "Group" AndAlso CType(oEventNode.ParentNode, XmlElement).GetAttribute("description").Contains("Brazil") Then
                        sGameType = "Brazil"
                    End If
                    If oEventNode.ParentNode.Name = "Group" AndAlso CType(oEventNode.ParentNode, XmlElement).GetAttribute("description").Contains("Mexican") Then
                        sGameType = "Mexican"
                    End If
                    If oEventNode.ParentNode.Name = "Group" AndAlso CType(oEventNode.ParentNode, XmlElement).GetAttribute("description").Contains("Superliga") Then
                        sGameType = "Super Liga"
                    End If
                    If oEventNode.ParentNode.Name = "Group" AndAlso CType(oEventNode.ParentNode, XmlElement).GetAttribute("description").Contains("Copa America") Then
                        sGameType = "Copa America"
                    End If
                    If oEventNode.ParentNode.Name = "Group" AndAlso CType(oEventNode.ParentNode, XmlElement).GetAttribute("description").Contains("Holland ") Then
                        sGameType = "Netherlands"
                    End If
                    If oEventNode.ParentNode.Name = "Group" AndAlso CType(oEventNode.ParentNode, XmlElement).GetAttribute("description").Contains("Concacaf") Then
                        sGameType = "Concacaf"
                    End If

                    If UCase(sDescription).Contains("CARLING CUP") Then
                        sGameType = "Carling Cup"
                    End If

                    If UCase(sDescription).Contains("FA CUP") Then
                        sGameType = "FA Cup"
                    End If
                End If

                If sGameType = "Soccer" Then
                    Continue For
                End If

                '' DO not save Series Prices games now -- ONLY FOR TEMPORARY WORK
                If UCase(sDescription).Contains("IN-GAME") OrElse UCase(sDescription).Contains("SERIES PRICES") Then
                    Continue For
                End If

                If oEventNode.ParentNode.Name = "Group" AndAlso (LCase(CType(oEventNode.ParentNode, XmlElement).GetAttribute("description")).Contains("added") OrElse _
                      LCase(CType(oEventNode.ParentNode, XmlElement).GetAttribute("description")).Contains("extra")) Then
                    bIsAdded = True
                    bIsExtraGame = (LCase(CType(oEventNode.ParentNode, XmlElement).GetAttribute("description")).Contains("extra"))
                End If

                If psGameType = "NFL Football" Then
                    If oEventNode.ParentNode.Name = "Group" AndAlso LCase(sDescription).Contains("preseason") Then
                        ''_log.Debug("Found NFL Preseason: " & oEventNode.ParentNode.OuterXml)
                        sGameType = "NFL Preseason"
                    End If
                End If

                Dim oEventData As XmlElement = CType(oEventNode, XmlElement)
                Dim oGame As New CGame()
                Dim sGameStatus As String = ""
                Dim sEvent_number As String = SafeString(oEventData.GetAttribute("event_number"))

                '' no team found, return
                If oEventNode.ChildNodes.Count = 0 Then
                    '_log.Error("Not found Team in game event no: " & SafeString(oEventData.GetAttribute("number")) & " --- " & oEventData.OuterXml)
                    Continue For
                End If

                '' A game node can be have 2 - 4 nodes, the first is away team, the second is home team
                Dim oAwayTeamNode As XmlElement = CType(oEventNode.ChildNodes(0), XmlElement)
                Dim oHomeTeamNode As XmlElement = CType(oEventNode.ChildNodes(1), XmlElement)
                Dim oDrawNode As XmlElement = CType(oEventNode.SelectSingleNode("Team[@name='Draw']"), XmlElement)

                '' Convert SportOption Time to EST time (GMT - 8 to GMT - 5)
                oGame.IsUpdateGameDate = True
                oGame.GameDate = SafeDate(oEventData.GetAttribute("start_time")).AddHours(3)
                oGame.IsCircle = oEventData.GetAttribute("circled") = "true"
                oGame.GameType = sGameType
                oGame.GameStatus = MapGameStatus(oHomeTeamNode.GetAttribute("status"))
                oGame.LastUpdated = Date.Now.ToUniversalTime()
                oGame.IsAddedGame = bIsAdded
                oGame.IsExtraGame = bIsExtraGame
                oGame.Description = sDescription
                '' save Draw NSS
                If oDrawNode IsNot Nothing Then
                    oGame.DrawRotationNumber = SafeInteger(oDrawNode.GetAttribute("number"))
                End If

                '' Team info
                oGame.HomeTeam.RotationNumber = SafeInteger(oHomeTeamNode.GetAttribute("number"))
                If Not oHomeTeamNode.GetAttribute("name").Contains(oHomeTeamNode.GetAttribute("nick_name")) Then
                    oGame.HomeTeam.Team = oHomeTeamNode.GetAttribute("name") + " " + oHomeTeamNode.GetAttribute("nick_name")
                Else
                    oGame.HomeTeam.Team = oHomeTeamNode.GetAttribute("name")
                End If

                Dim sAbbr As String = oHomeTeamNode.GetAttribute("abbr")

                Dim bMapHome As Boolean = False
                oGame.HomeTeam.Team = MapTeamNameABBR(sGameType, oGame.HomeTeam.Team, sAbbr, bMapHome)

                oGame.AwayTeam.RotationNumber = SafeInteger(oAwayTeamNode.GetAttribute("number"))
                If Not oAwayTeamNode.GetAttribute("name").Contains(oAwayTeamNode.GetAttribute("nick_name")) Then
                    oGame.AwayTeam.Team = oAwayTeamNode.GetAttribute("name") + " " + oAwayTeamNode.GetAttribute("nick_name")
                Else
                    oGame.AwayTeam.Team = oAwayTeamNode.GetAttribute("name")
                End If
                sAbbr = oAwayTeamNode.GetAttribute("abbr")
                Dim bMapAway As Boolean = False
                oGame.AwayTeam.Team = MapTeamNameABBR(sGameType, oGame.AwayTeam.Team, sAbbr, bMapAway)

                If oAwayTeamNode.GetAttribute("pitcher") <> "" Then
                    oGame.AwayPitcher = oAwayTeamNode.GetAttribute("pitcher")
                    oGame.AwayPitcherRightHand = Not oAwayTeamNode.GetAttribute("left_handed") = "true"
                    oGame.HomePitcher = oHomeTeamNode.GetAttribute("pitcher")
                    oGame.HomePitcherRightHand = Not oHomeTeamNode.GetAttribute("left_handed") = "true"
                End If

                '' Do not save In-Games for Football and Basketball
                If IsFootball(sGameType) OrElse IsBasketball(sGameType) Then
                    If oGame.AwayTeam.RotationNumber > 9000 Then
                        Continue For
                    End If
                End If

                '' This game does not have enoguh team now, just ignore!
                If SafeString(oGame.HomeTeam.Team) = "" OrElse SafeString(oGame.AwayTeam.Team) = "" Then
                    Continue For
                End If

                '_log.Info("Ready to save: Hometeam " & oGame.HomeTeam.Team & " - Away team: " & oGame.AwayTeam.Team & " -- Date: " & SafeString(oGame.GameDate))
                If bMapAway And bMapHome Then
                    oGame.SaveToDB(poDB, CGame.FindTeamBy.Both)
                Else
                    oGame.SaveToDB(poDB, CGame.FindTeamBy.NSS)
                End If

            Catch ex As Exception
                '_log.Error("!!!!! Error with gamesave!. Error: " & ex.Message, ex)
                '_log.Error("!!!!! Error with gamesave!. XML: " & oEventNode.OuterXml)
            End Try
        Next
    End Sub

    Private Sub ProcessGameScores(ByVal poRoot As XmlDocument, ByVal poDB As WebsiteLibrary.DBUtils.CSQLDBUtils)
        Dim oDate As DateTime = SBCBL.std.GetEasternDate()
        'oDate = oDate.AddHours(-12)

        Dim sSQL As String = "select * from games where gamestatus <> 'Final' and gamedate > " & SQLString(oDate.AddHours(-24)) & " and gamedate < " & SQLString(oDate.AddHours(1))
        '_log.Debug("Get game for update score. SQL: " & sSQL)
        Dim oDT As DataTable = poDB.getDataTable(sSQL)
        Dim nMatch As Integer = 0
        Dim oUpdate As WebsiteLibrary.DBUtils.CSQLUpdateStringBuilder
        Dim oOuterScores As XmlNodeList = poRoot.SelectNodes("ScheduleBigData/Scores")
        '_log.Info("Found number of Score nodes: " & oOuterScores.Count.ToString())
        For Each oScoreNode As XmlElement In oOuterScores
            Try
                Dim oEvent_Score As XmlElement = CType(oScoreNode.SelectSingleNode("Event_Score"), XmlElement)
                Dim oAwayTeamNode As XmlElement = CType(oScoreNode.SelectSingleNode("Event_Team_Score[@number='" & oEvent_Score.GetAttribute("number") & "']"), XmlElement)
                Dim nMatch1 As Integer = 0

                Dim oGameRow As DataRow = Nothing
                For Each oRow As DataRow In oDT.Rows
                    If SafeString(oRow("AwayRotationNumber")) = oAwayTeamNode.GetAttribute("number") Then 'AndAlso SafeDate(oRow("GameDate")).ToShortDateString() = oTimeStamp.ToShortDateString() Then
                        oGameRow = oRow
                        nMatch += 1
                        nMatch1 += 1
                    End If
                Next

                If oGameRow Is Nothing Then
                    '_log.Error("!!!!!!!!!!! WARNG: Can't found game Score. XML: " & oScoreNode.OuterXml)
                    Return
                End If

                If nMatch1 > 1 Then
                    '_log.Error("!!!!!!!!!!! WARNG: Found " & nMatch.ToString() & " games to update Ssore. XML: " & oScoreNode.OuterXml)
                    Return
                End If

                '' TRAP SCORE FOR PROP
                If SafeString(oGameRow("IsForProp")) = "Y" Then
                    '_log.Info("!!! FOUND SCORE FOR PROP. XML: " & oScoreNode.OuterXml)
                End If

                Dim oHomeTeamNode As XmlElement = CType(oScoreNode.SelectSingleNode("Event_Team_Score[@number='" & SafeString(oGameRow("HomeRotationNumber")) & "']"), XmlElement)
                oUpdate = New WebsiteLibrary.DBUtils.CSQLUpdateStringBuilder("Games", "where  gameid = " & SQLString(oGameRow("GameID")))
                Dim nHomeScore As Integer = SafeInteger(oHomeTeamNode.GetAttribute("score"))
                Dim nAwayScore As Integer = SafeInteger(oAwayTeamNode.GetAttribute("score"))

                If nHomeScore <> -1 And nAwayScore <> -1 Then
                    oUpdate.AppendString("AwayScore", SQLString(nAwayScore))
                    oUpdate.AppendString("HomeScore", SQLString(nHomeScore))
                End If

                '' update game status realtime
                If oEvent_Score.GetAttribute("status") <> "" Then
                    Dim sGameStatus As String = SafeString(oEvent_Score.GetAttribute("status"))

                    oUpdate.AppendString("GameStatus", SQLString(sGameStatus))

                    ' game status change to time, mean 2H is begun
                    If SafeString(oEvent_Score.GetAttribute("timer")) = "Half" AndAlso sGameStatus = "Time" Then
                        If SafeDate(oGameRow("SecondHalfTime")) = Date.MinValue Then
                            '_log.Info("SecondHalfTime for game: " & SafeString(oGameRow("GameID")).ToString())
                            oUpdate.AppendString("SecondHalfTime", SQLString(DateTime.Now.ToUniversalTime()))
                        End If
                    End If

                    ' update is first half finish
                    If UCase(sGameStatus) = "2ND H" OrElse UCase(sGameStatus) = "3RD Q" Then
                        oUpdate.AppendString("IsFirstHalfFinished", "'Y'")

                        If SafeString(oGameRow("PropDescription")) = "" Then
                            oUpdate.AppendString("PropDescription", SQLString("Closed 2H line at: " & Now.ToUniversalTime()))
                        End If
                    ElseIf UCase(sGameStatus) = "TIME" And SafeString(oGameRow("GameType")) = "NFL Football" Then
                        oUpdate.AppendString("IsFirstHalfFinished", "'N'")
                    End If
                End If
                Dim nHalf As Integer = 1
                '' save score for game half
                If SafeInteger(oEvent_Score.GetAttribute("period")) <> 0 And oEvent_Score.GetAttribute("status") <> "Final" Then
                    nHalf = GamePeriodToHalf(SafeString(oGameRow("GameType")), SafeInteger(oEvent_Score.GetAttribute("period")))

                    If nHalf = 1 Then
                        oUpdate.AppendString("HomeFirstHalfScore", SQLString(nHomeScore))
                        oUpdate.AppendString("AwayFirstHalfScore", SQLString(nAwayScore))
                        'oUpdate.AppendString("IsFirstHalfFinished", "'N'")
                    ElseIf nHalf = 2 Then
                        oUpdate.AppendString("IsFirstHalfFinished", "'Y'")
                    End If

                    '' update quater score
                    Select Case SafeInteger(oEvent_Score.GetAttribute("period"))
                        Case 0, 1
                            oUpdate.AppendString("HomeFirstQScore", SafeString(nHomeScore))
                            oUpdate.AppendString("AwayFirstQScore", SafeString(nAwayScore))
                        Case 2
                            oUpdate.AppendString("HomeSecondQScore", SafeString(nHomeScore - SafeInteger(oGameRow("HomeFirstQScore"))))
                            oUpdate.AppendString("AwaySecondQScore", SafeString(nAwayScore - SafeInteger(oGameRow("AwayFirstQScore"))))
                        Case 3
                            oUpdate.AppendString("HomeThirdQScore", SafeString(nHomeScore - SafeInteger(oGameRow("HomeFirstHalfScore"))))
                            oUpdate.AppendString("AwayThirdQScore", SafeString(nAwayScore - SafeInteger(oGameRow("AwayFirstHalfScore"))))
                        Case 4
                            oUpdate.AppendString("HomeFourQScore", SafeString(nHomeScore - SafeInteger(oGameRow("HomeFirstHalfScore")) - SafeInteger(oGameRow("HomeThirdQScore"))))
                            oUpdate.AppendString("AwayFourQScore", SafeString(nAwayScore - SafeInteger(oGameRow("AwayFirstHalfScore")) - SafeInteger(oGameRow("AwayThirdQScore"))))
                    End Select

                    oUpdate.AppendString("CurrentQuater", SafeString(oEvent_Score.GetAttribute("period")))
                    '' if this is MLB game, look at it's status
                ElseIf SafeString(oGameRow("GameType")) = "MLB Baseball" And oEvent_Score.GetAttribute("status") = "6th" Then
                    oUpdate.AppendString("HomeFirstHalfScore", SQLString(nHomeScore))
                    oUpdate.AppendString("AwayFirstHalfScore", SQLString(nAwayScore))
                    '' In 1half break time, we need update 1H score to avoid game score return late
                ElseIf oEvent_Score.GetAttribute("timer") = "Half" And oEvent_Score.GetAttribute("status") = "Time" Then
                    oUpdate.AppendString("HomeFirstHalfScore", SQLString(nHomeScore))
                    oUpdate.AppendString("AwayFirstHalfScore", SQLString(nAwayScore))
                    oUpdate.AppendString("IsFirstHalfFinished", "'N'")
                ElseIf SafeInteger(oEvent_Score.GetAttribute("period")) = 0 Then
                    oUpdate.AppendString("HomeFourQScore", SafeString(nHomeScore - SafeInteger(oGameRow("HomeFirstHalfScore")) - SafeInteger(oGameRow("HomeThirdQScore"))))
                    oUpdate.AppendString("AwayFourQScore", SafeString(nAwayScore - SafeInteger(oGameRow("AwayFirstHalfScore")) - SafeInteger(oGameRow("AwayThirdQScore"))))
                End If

                '_log.Info("Update Score: " & SafeString(oGameRow("AwayTeam")) & " @ " & SafeString(oGameRow("HomeTeam")) & " --- " & SafeString(oGameRow("AwayScore")) & " : " & SafeString(oGameRow("HomeScore")) & " Half: " & nHalf.ToString())
                '_log.Info("Update score SQL: " & oUpdate.SQL)
                poDB.executeNonQuery(oUpdate.SQL)
            Catch ex As Exception
                '_log.Error("Error with update game score. Error: " & ex.Message, ex)
                '_log.Error("Error with update game score. XML " & oScoreNode.OuterXml)
            End Try
        Next
    End Sub

    Private Function MapGameStatus(ByVal psGameStatus As String) As String
        Dim sStatus As String = psGameStatus
        Select Case UCase(psGameStatus)
            Case "SUSPENDED", "POSTPONED", "PONED", "SUSP", "PUSH"
                sStatus = "CANCELLED"
        End Select
        Return sStatus
    End Function

    Private Function MapTeamNameABBR(ByVal psGameType As String, ByVal psXMLTeamName As String, ByVal psAbbr As String, ByRef pbFound As Boolean) As String
        pbFound = False
        If _oTeamMapDocument Is Nothing Then
            Dim sFilePath As String = New FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName & "\team_map.xml"
            Dim sContent As String = ""
            If File.Exists(sFilePath) Then
                sContent = File.ReadAllText(sFilePath)
            End If

            sContent = sContent.Replace("'", "&apos;").Replace("""", "'").Replace(vbLf, "")
            _oTeamMapDocument = New XmlDocument()
            _oTeamMapDocument.LoadXml(sContent)
        End If

        If _oTeamMapDocument Is Nothing Then
            Return psXMLTeamName
        End If

        Dim sOutTeam As String = ""
        Dim sXPath As String = "FeedMappings/Feed[@name='sportsoptions']/TeamNames_ABBR/Team[@type='" & psGameType & "' and @ABBR='" & psAbbr & "']"
        Dim oXMNode As XmlElement = CType(_oTeamMapDocument.SelectSingleNode(sXPath), XmlElement)
        If oXMNode Is Nothing OrElse oXMNode.GetAttribute("value") = "" Then
            pbFound = True
            Return psXMLTeamName
        End If

        Return oXMNode.GetAttribute("value")
    End Function
End Class


Public Class SportOptionUserNameException
    Inherits System.Exception

    Public Sub New(ByVal psMessage As String)
        MyBase.New(psMessage)
    End Sub
End Class

Public Class SportOptionDisconnectException
    Inherits System.Exception

    Public Sub New(ByVal psMessage As String)
        MyBase.New(psMessage)
    End Sub

End Class

Public Enum EBookMaker
    Cris = 1
    BetJamaica = 9
    FiveDimes = 14
    Pinnacle = 17
    BetPhoenix = 43
    Bodog = 5
End Enum