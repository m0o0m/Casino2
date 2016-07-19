Imports System.Xml
Imports System.Data
Imports SBCBL.std
Imports SBCService.CServiceStd

Public Class CMySportsBookService
    Private _log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

    Private _SportTypes As Dictionary(Of String, String)

    Private ReadOnly Property SportTypes() As Dictionary(Of String, String)
        Get
            If _SportTypes Is Nothing Then
                _SportTypes = New Dictionary(Of String, String)()
                _SportTypes.Add("Super Bowl", "520")
                _SportTypes.Add("Bodog", "5")
                _SportTypes.Add("5Dimes", "14")
                _SportTypes.Add("Pinnacle", "17")
                _SportTypes.Add("Bet Phoenix", "43")
            End If
            Return _SportTypes
        End Get
    End Property

    Public Function ExecuteMySportsBookFeed() As Boolean
        _log.Info("START REQUEST PINNACLE PROPS XML.")
        Dim oDB As New WebsiteLibrary.DBUtils.CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
        Try
            Dim sXML As String = SBCBL.std.webGet(PINNACLE_FEED_URL)
            Dim oDoc As New XmlDocument()
            oDoc.LoadXml(sXML)
            'event[starts-with(league,'M')]
            Dim oListEvents As XmlNodeList = oDoc.SelectNodes("pinnacle_line_feed/events/event")
            Dim sGameType As String = ""

            For Each oEvent As XmlElement In oListEvents
                Dim oGame As New CGame()
                oGame.IsUpdateGameDate = True

                Dim bSaveGame As Boolean = True
                oGame.GameType = sGameType
                oGame.GameDate = SafeDate(oEvent.SelectSingleNode("event_datetimeGMT").InnerText)
                oGame.IsForProp = True
                oGame.PropDescription = SafeString(oEvent.SelectSingleNode("league").InnerText) & " - " & SafeString(oEvent.SelectSingleNode("description").InnerText)
                oGame.ExtPropNumber = SafeString(oEvent.SelectSingleNode("gamenumber").InnerText)

                '' save Teams to game lines
                Dim oListParticipants As XmlNodeList = oEvent.SelectNodes("participants/participant")

                For Each oParticipants As XmlElement In oListParticipants
                    If oParticipants.SelectSingleNode("odds/moneyline_value") Is Nothing Then
                        bSaveGame = False
                        Continue For
                    End If
                    Dim sPropRotationNumber As String = oParticipants.SelectSingleNode("rotnum").InnerText
                    Dim oGameLine As CLine = oGame.GetPropLine("sportoption", "", sPropRotationNumber)
                    oGameLine.Context = "Proposition"
                    oGameLine.PropParticipantName = oParticipants.SelectSingleNode("participant_name").InnerText

                    oGameLine = oGame.GetPropLine("sportoption", "Pinnacle", sPropRotationNumber)
                    oGameLine.PropParticipantName = oParticipants.SelectSingleNode("participant_name").InnerText
                    oGameLine.Context = "Proposition"
                    oGameLine.PropMoneyLine = SafeDouble(oParticipants.SelectSingleNode("odds/moneyline_value").InnerText)
                Next

                '' cancel to save this game
                If Not bSaveGame Then
                    Continue For
                End If

                oGame.SavePropToDB(oDB)
                oGame.SavePropLinesToDB(oDB)
            Next

        Catch ex As Exception

            _log.Error("Cant save Pinnacle Props: " & ex.Message, ex)
        Finally
            oDB.closeConnection()
        End Try
    End Function
End Class
