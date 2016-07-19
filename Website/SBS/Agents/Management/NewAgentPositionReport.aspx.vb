Imports SBCBL.std
Imports SBCBL.Tickets
Imports SBCBL.CacheUtils
Imports System.Data
Imports System.Collections

Partial Class SBS_Agents_Management_NewAgentPositionReport
    Inherits SBCBL.UI.CSBCPage

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        PageTitle = "Agent Position Reports"
        MenuTabName = "REPORT"
        SubMenuActive = "AGENT_POSITION_REPORTS"
        DisplaySubTitlle(Me.Master, "Agent Position Reports")
    End Sub


End Class
