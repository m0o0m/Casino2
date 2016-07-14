Imports SBCBL.std
Partial Class Agents_PLReport
    Inherits SBCBL.UI.CSBCPage

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        PageTitle = "P/L Reports"
        MenuTabName = "REPORT"
        SubMenuActive = "PL_REPORTS"
        DisplaySubTitlle(Me.Master, "P/L Reports")
    End Sub

    Private Sub setTabActive(ByVal psTabKey As String)
        lbtTabDaily.Attributes.Remove("href")
        lbtTabWeekly.Attributes.Remove("href")
        lbtYTD.Attributes.Remove("href")

        lbtTabDaily.Attributes.Add("href", "#" + tabContent1.ClientID)
        lbtTabWeekly.Attributes.Add("href", "#" + tabContent2.ClientID)
        lbtYTD.Attributes.Add("href", "#" + tabContent3.ClientID)

        liDAILY.Attributes.Remove("class")
        liWEEKLY.Attributes.Remove("class")
        liYTD.Attributes.Remove("class")

        tabContent1.Attributes.Remove("class")
        tabContent2.Attributes.Remove("class")
        tabContent3.Attributes.Remove("class")

        tabContent1.Attributes.Add("class", "tab-pane fade in")
        tabContent2.Attributes.Add("class", "tab-pane fade in")
        tabContent3.Attributes.Add("class", "tab-pane fade in")

        ucPLReport1.Visible = False
        ucPLReport2.Visible = False
        ucPLReport3.Visible = False

        Select Case True
            Case UCase(psTabKey) = "WEEKLY"
                liWEEKLY.Attributes.Add("class", "active")
                tabContent2.Attributes.Add("class", "tab-pane fade in active")

                ucPLReport2.Visible = True
                ucPLReport2.IsDailyReport = False
                ucPLReport2.IsYTD = False
                ucPLReport2.bindReport()

            Case UCase(psTabKey) = "YTD"
                liYTD.Attributes.Add("class", "active")
                tabContent3.Attributes.Add("class", "tab-pane fade in active")

                ucPLReport3.Visible = True
                ucPLReport3.IsDailyReport = False
                ucPLReport3.IsYTD = True
                ucPLReport3.bindReport()
            Case Else
                liDAILY.Attributes.Add("class", "active")
                tabContent1.Attributes.Add("class", "tab-pane fade in active")

                ucPLReport1.Visible = True
                ucPLReport1.IsDailyReport = True
                ucPLReport1.IsYTD = False
                ucPLReport1.bindReport()
        End Select
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            setTabActive(SafeString(Request("tab"))) 'if request tab is empty, default Agent tab
        End If
    End Sub

    Protected Sub lbtTab_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        'setTabActive(CType(sender, LinkButton).CommandArgument)
        Response.Redirect("PLReport.aspx?tab=" & CType(sender, LinkButton).CommandArgument)
    End Sub

End Class
