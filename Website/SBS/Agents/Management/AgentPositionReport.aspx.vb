Imports SBCBL.std
Imports SBCBL.Tickets
Imports SBCBL.CacheUtils
Imports System.Data
Imports System.Collections
Partial Class Agents_AgentPositionReport
    Inherits SBCBL.UI.CSBCPage

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        PageTitle = "Agent Position Reports"
        MenuTabName = "REPORT"
        SubMenuActive = "AGENT_POSITION_REPORTS"
        DisplaySubTitlle(Me.Master, "Agent Position Reports")
    End Sub
    Protected Sub lbtTab_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        'setTabActive(CType(sender, LinkButton).CommandArgument)
        Response.Redirect("AgentPositionReport.aspx?tab=" & CType(sender, LinkButton).CommandArgument)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If SBCBL.std.SafeString(Request("tab")) = "DETAILS" Then
            ucAgentPositionReport.Visible = True
            ucNewAgentPositionReport.Visible = False
        Else
            ucNewAgentPositionReport.Visible = True
            ucAgentPositionReport.Visible = False
        End If

        If Not IsPostBack Then
            lbtAllPlayer.Attributes.Remove("href")
            lbtPlayers.Attributes.Remove("href")

            lbtAllPlayer.Attributes.Add("href", "#" + tabContent1.ClientID)
            lbtPlayers.Attributes.Add("href", "#" + tabContent2.ClientID)

            liALL.Attributes.Remove("class")
            liDETAILS.Attributes.Remove("class")

            tabContent1.Attributes.Remove("class")
            tabContent2.Attributes.Remove("class")

            tabContent1.Attributes.Add("class", "tab-pane fade in")
            tabContent2.Attributes.Add("class", "tab-pane fade in")


            If SBCBL.std.SafeString(Request("tab")) = "DETAILS" Then
                liDETAILS.Attributes.Add("class", "active")
                tabContent2.Attributes.Add("class", "tab-pane fade in active")
            Else
                liALL.Attributes.Add("class", "active")
                tabContent1.Attributes.Add("class", "tab-pane fade in active")
            End If
        End If
    End Sub
End Class
