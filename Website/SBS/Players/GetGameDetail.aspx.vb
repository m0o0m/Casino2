Imports System.Activities.Statements
Imports System.Data
Imports System.Net
Imports System.Web.Services
Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports SBCBL.std

Partial Class SBS_Players_GetGameDetail
    Inherits System.Web.UI.Page

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

                Dim sHomTeam = IIf(homeRotationNumber > 0, string.format("<b>[{0}]</b>{1}", homeRotationNumber, homeTeam), "")
                Dim sAwayTeam = IIf(awayRotationNumber > 0, string.format("<br/><b>[{0}]</b>{1}", awayRotationNumber, awayTeam), "")
                Dim sDraw = IIf(drawRotationNumber > 0, string.format("<br/><b>[{0}]</b>{1}", awayRotationNumber, awayTeam), "")

                sDescription = String.Format("<td class='baseline'>{0} {1} {2}</td>", sHomTeam, sAwayTeam, sDraw)

                'Select Case UCase(SafeString(row("BetType")))
                '    Case "SPREAD"
                '        sDescription = String.Format("<td>{0} {1} {2}</td>", ))
                '    Case "TOTALPOINTS"
                '        If SafeString(oTicketBet("HomePitcher_TicketBets")) <> "" AndAlso SafeString(oTicketBet("AwayPitcher_TicketBets")) <> "" Then
                '            sHomeTeam = SafeString(oTicketBet("AwayTeam")) & "/" & sHomeTeam
                '            sDescription = getDetailByTotalPoints(sHomeTeam, "", oTicketBet)
                '        Else
                '            sHomeTeam = sAwayTeam & "/" & sHomeTeam
                '            sDescription = getDetailByTotalPoints(sHomeTeam, "", oTicketBet)
                '        End If
                '    Case "TEAMTOTALPOINTS"
                '        sDescription = getDetailByTeamTotalPoints(IIf(SafeString(oTicketBet("TeamTotalName")).Equals("away"), sAwayTeam, sHomeTeam), "", oTicketBet)
                '    Case "MONEYLINE"
                '        sDescription = getDetailByMoneyLine(sHomeTeam, sAwayTeam, oTicketBet)
                '    Case "DRAW"
                '        sDescription = getDetailByDraw(sHomeTeam, sAwayTeam, oTicketBet)
                'End Select

                html += sDescription
            End If

            
	    Next

        html += "</tr>"
        html += "</table>"

        Return html

    End Function

End Class
