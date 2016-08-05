Imports System.Activities.Statements
Imports System.Data
Imports System.Net
Imports System.Linq
Imports System.Web.Services
Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports SBCBL.std

Partial Class SBS_Players_GetGameDetail
    Inherits SBCBL.UI.CSBCPage
    Public ReadOnly Property GetPlayerId() As String
        Get
            Return UserSession.PlayerUserInfo.UserID
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    <WebMethod()>
    Public Shared Function GetGameDetail(ByVal ticketId As String) As String
        Dim data = (New CTicketManager).GetOpenTicketsByTicketId(ticketId)
        Dim html = "<div class='row-detail-caption pdTB10 pdLR2'><b class='fz13 clr-white'>Game Detail</b></div>"
        html += "<table class='full-w'cellspacing='0' cellpadding='0' border='0'>"
        html += "<tr>"
        For Each row As DataRow In data.Rows
            If UCase(SafeString(row("IsForProp"))).Equals("Y") Then
                html += String.Format("<td>{0}</td>", SafeString(row("PropDescription")))
            Else
                Dim sDescription As String = ""
                Dim homeRotationNumber = SafeInteger(row("HomeRotationNumber"))
                Dim awayRotationNumber = SafeInteger(row("AwayRotationNumber"))
                Dim drawRotationNumber = SafeInteger(row("DrawRotationNumber"))
                Dim homeTeam = SafeString(row("HomeTeam"))
                Dim awayTeam = SafeString(row("AwayTeam"))
                Dim draw = "Draw"

                Dim betType = UCase(SafeString(row("BetType")))
                If betType.Equals("SPREAD") Or betType.Equals("MONEYLINE") Then
                    homeTeam = SafeString(IIf(homeRotationNumber > 0, String.Format("<b>[{0}]</b>{1}", homeRotationNumber, homeTeam), ""))
                    awayTeam = SafeString(IIf(awayRotationNumber > 0, String.Format("<br /><b>[{0}]</b>{1}", awayRotationNumber, awayTeam), ""))
                    draw = SafeString(IIf(drawRotationNumber > 0, String.Format("<br /><b>[{0}]</b>{1}", drawRotationNumber, draw), ""))
                Else
                    homeTeam = SafeString(IIf(homeRotationNumber > 0, homeTeam, ""))
                    awayTeam = SafeString(IIf(awayRotationNumber > 0, "<br />" & awayTeam, ""))
                    draw = SafeString(IIf(drawRotationNumber > 0, "<br />" & draw, ""))
                End If

                sDescription = String.Format("<td class='baseline'>{0} {1} {2}</td>", homeTeam, awayTeam, draw)

                html += sDescription
            End If

        Next

        html += "</tr>"
        html += "</table>"

        Return html

    End Function

    <WebMethod()>
    Public Shared Function GetGameDetailForHistory(ByVal ticketId As String) As String

        Dim data = (New CTicketManager).GetHistoryTicketsByPlayerWithTicketStatus(ticketId)
        Dim grouped = data.AsEnumerable().GroupBy(Function(rw) rw("GameID")).[Select](Function(grouping) New With {Key .GameId = grouping.Key.ToString(), Key .Row = grouping})

        Dim html = "<div class='row-detail-caption pdTB10 pdLR2'><b class='fz13 clr-white'>Game Detail</b></div>"
        html += "<table class='full-w'cellspacing='0' cellpadding='0' border='0'>"
        html += "<tr>"
        For Each g In grouped
            Dim sDescription As String = ""
            For Each row As DataRow In g.Row

                If UCase(SafeString(row("IsForProp"))).Equals("Y") Then
                    sDescription += String.Format("<div>{0}</div>", SafeString(row("PropDescription")))
                Else

                    Dim homeRotationNumber = SafeInteger(row("HomeRotationNumber"))
                    Dim awayRotationNumber = SafeInteger(row("AwayRotationNumber"))
                    Dim drawRotationNumber = SafeInteger(row("DrawRotationNumber"))
                    Dim homeTeam = SafeString(row("HomeTeam"))
                    Dim awayTeam = SafeString(row("AwayTeam"))
                    Dim draw = "Draw"
                    Dim homeScore = ""
                    Dim awayScore = ""

                    ' Scores
                    Dim sScores As String = "({0})"
                    Select Case LCase(SafeString(row("Context")))
                        Case "current"
                            homeScore = String.Format(sScores, SafeString(row("HomeScore")))
                            awayScore = String.Format(sScores, SafeString(row("AwayScore")))
                        Case "1h"
                            homeScore = String.Format(sScores, SafeString(row("HomeFirstHalfScore")))
                            awayScore = String.Format(sScores, SafeString(row("AwayFirstHalfScore")))

                        Case "2h"
                            homeScore = String.Format(sScores, SafeString(SafeInteger(row("HomeScore")) - SafeInteger(row("HomeFirstHalfScore"))))
                            awayScore = String.Format(sScores, SafeString(SafeInteger(row("AwayScore")) - SafeInteger(row("AwayFirstHalfScore"))))

                        Case "1q"
                            homeScore = String.Format(sScores, SafeString(row("HomeFirstQScore")))
                            awayScore = String.Format(sScores, SafeString(row("AwayFirstQScore")))

                        Case "2q"
                            homeScore = String.Format(sScores, SafeString(row("HomeSecondQScore")))
                            awayScore = String.Format(sScores, SafeString(row("AwaySecondQScore")))

                        Case "3q"
                            homeScore = String.Format(sScores, SafeString(row("HomeThirdQScore")))
                            awayScore = String.Format(sScores, SafeString(row("AwayThirdQScore")))

                        Case "4q"
                            homeScore = String.Format(sScores, SafeString(row("HomeFourQScore")))
                            awayScore = String.Format(sScores, SafeString(row("AwayFourQScore")))

                        Case Else
                            homeScore = ""
                            awayScore = ""
                    End Select


                    Dim betType = UCase(SafeString(row("BetType")))
                    If betType.Equals("SPREAD") Or betType.Equals("MONEYLINE") Then
                        homeTeam = SafeString(IIf(homeRotationNumber > 0, String.Format("<b>[{0}]</b>{1}{2}", homeRotationNumber, homeTeam, homeScore), ""))
                        awayTeam = SafeString(IIf(awayRotationNumber > 0, String.Format("<b>[{0}]</b>{1}{2}", awayRotationNumber, awayTeam, awayScore), ""))
                        draw = SafeString(IIf(drawRotationNumber > 0, String.Format("<b>[{0}]</b>{1}", drawRotationNumber, draw), ""))
                    Else
                        homeTeam = SafeString(IIf(homeRotationNumber > 0, String.Format("<b>[{0}]</b>{1}{2}", homeRotationNumber, homeTeam, homeScore), ""))
                        awayTeam = SafeString(IIf(awayRotationNumber > 0, String.Format("<b>[{0}]</b>{1}{2}", awayRotationNumber, awayTeam, awayScore), ""))
                        draw = SafeString(IIf(drawRotationNumber > 0, draw, ""))
                    End If

                    
                    sDescription += String.Format("<div>{0}</div> <div>{1}</div> <div>{2}</div>", homeTeam, awayTeam, draw)
                End If
                
            Next

            html += String.Format("<td class='baseline'>{0}</td>", sDescription)
        Next

        html += "</tr>"
        html += "</table>"

        Return html

    End Function

End Class
