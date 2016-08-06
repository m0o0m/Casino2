Imports System.Data

Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports SBCBL.std

Namespace SBSPlayer
    Partial Class WeekBalance
        Inherits SBCBL.UI.CSBCPage

        'Private Sub bindWeek()
        '    Dim oDate As Date = GetEasternMondayOfCurrentWeek()
        '    Dim olstWeek As New Dictionary(Of String, String)

        '    For nIndex As Integer = 1 To 8
        '        Dim oTemp As Date = oDate.AddDays((nIndex - 1) * -7)
        '        olstWeek.Add(oTemp.ToString("MM/dd/yyyy") & " - " & oTemp.AddDays(6).ToString("MM/dd/yyyy"), oTemp.ToString("MM/dd/yyyy"))
        '    Next

        '    ddlWeeks.DataSource = olstWeek
        '    ddlWeeks.DataTextField = "Key"
        '    ddlWeeks.DataValueField = "value"
        '    ddlWeeks.DataBind()
        'End Sub

        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Weekly Figures"
            MenuTabName = "BALANCE"
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                'txtDateFrom.Text = GetLastMondayOfDate(GetEasternDate()).ToShortDateString
                'txtDateTo.Text = GetEasternDate().ToShortDateString
                'bindWeek()
                LoadPlayerDashboard()
            End If
        End Sub

        Private Sub LoadPlayerDashboard()
            ' to avoid confusing, we will use EST time in Dashboard
            'Dim oStarDate As Date = SafeDate(txtDateFrom.Text)
            'Dim oEndDate As Date = SafeDate(txtDateTo.Text)

            'If oStarDate = Date.MinValue Then
            '    oStarDate = GetLastMondayOfDate(GetEasternDate())
            '    txtDateFrom.Text = oStarDate.ToShortDateString()
            'End If
            'If oEndDate = Date.MinValue Then
            '    oEndDate = GetEasternDate()
            '    txtDateFrom.Text = oEndDate.ToShortDateString()
            'End If

            'oEndDate = oEndDate.AddDays(1)

            Dim oTickets As DataTable = new DataTable()
            Dim listMonday = GetListMonday()
            For Each monday As DateTime In listMonday
                Dim oStarDate As Date = monday
                Dim oEndDate As Date = monday.AddDays(6)
                Dim weekBalance As DataTable = (New CPlayerManager()).GetPlayerDashboard(UserSession.UserID, oStarDate, oEndDate, UserSession.PlayerUserInfo.TimeZone)
                oTickets.Merge(weekBalance)
            Next

            'Dim oStarDate As Date = SafeDate(ddlWeeks.SelectedValue)
            'Dim oEndDate As Date = SafeDate(oStarDate.AddDays(6))
            'Dim oTickets As DataTable = (New CPlayerManager()).GetPlayerDashboard(UserSession.UserID, oStarDate, oEndDate, UserSession.PlayerUserInfo.TimeZone)

            dgPlayers.DataSource = oTickets
            dgPlayers.DataBind()

            If oTickets.Rows.Count >= 1 Then
                lblXML1.Text = BuildChartXML(oTickets)
            End If
        End Sub

        Private Function BuildChartXML(ByVal poTable As DataTable) As String
            Dim oChart As XElement = <chart lineThickness="1" showValues="0" formatNumberScale="1" anchorRadius="2"
                                         divLineAlpha="20" divLineIsDashed="1" showAlternateHGridColor="1" shadowAlpha="40"
                                         labelStep="2" numvdivlines="5" chartRightMargin="35" bgAngle="270" bgAlpha="10,10"/>

            '' Bind Categories
            Dim oCategories As XElement = <categories/>
            oChart.Add(oCategories)

            For Each oClm As DataColumn In poTable.Columns
                If oClm.ColumnName = "Pending" OrElse oClm.ColumnName = "Title" OrElse oClm.ColumnName = "Net" OrElse oClm.ColumnName = "ThisMonday" Then
                    Continue For
                End If

                oCategories.Add(<category label=<%= oClm.ColumnName %>/>)
            Next

            '' Bind data
            For Each oRow As DataRow In poTable.Rows
                Dim oDataset As XElement = <dataset seriesName=<%= oRow("Title") %>/>
                oChart.Add(oDataset)

                For Each oClm As DataColumn In poTable.Columns
                    If oClm.ColumnName = "Title" OrElse oClm.ColumnName = "Net" Then
                        Continue For
                    End If

                    oDataset.Add(<set value=<%= SafeString(oRow(oClm.ColumnName)) %>/>)
                Next

            Next

            Return oChart.ToString().Replace("'", "&rsquo;").Replace("""", "'")
        End Function

        'Protected Sub txtDateRange_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtDateFrom.TextChanged, txtDateTo.TextChanged
        '    dgPlayers.CurrentPageIndex = 0
        '    LoadPlayerDashboard()
        'End Sub

        Protected Sub dgPlayers_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgPlayers.ItemCommand
            Select Case e.CommandName
                Case "VIEW_WEEK_HISTORY"
                    Response.Redirect("History.aspx?dateRange=" & SafeString(e.CommandArgument))
                Case "VIEW_HISTORY"
                    Dim odate = SafeDate(e.CommandArgument)
                    Response.Redirect("History.aspx?dateRange=" & SafeDate(e.CommandArgument).ToShortDateString() & "-" & SafeDate(e.CommandArgument).ToShortDateString())
            End Select
        End Sub

        Protected Sub SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)
            dgPlayers.CurrentPageIndex = 0
            LoadPlayerDashboard()
        End Sub

        Dim RoundMidPoint As Double = GetRoundMidPoint()

        Protected Sub dgPlayers_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgPlayers.ItemDataBound
            If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
                Dim currentMonday = FirstDayInWeek(Today)
                Dim monday = SafeDate(e.Item.DataItem("ThisMonday"))
                CType(e.Item.FindControl("ltrWeekOf"), Literal).Text = IIf(currentMonday = monday, "Current Week", monday.ToString("MM/dd/yyyy"))
                CType(e.Item.FindControl("lbnMon"), LinkButton).Text = SafeInteger(e.Item.DataItem(1))
                CType(e.Item.FindControl("lbnTues"), LinkButton).Text = SafeInteger(e.Item.DataItem(2))
                CType(e.Item.FindControl("lbnWed"), LinkButton).Text = SafeInteger(e.Item.DataItem(3))
                CType(e.Item.FindControl("lbnThurs"), LinkButton).Text = SafeInteger(e.Item.DataItem(4))
                CType(e.Item.FindControl("lbnFri"), LinkButton).Text = SafeInteger(e.Item.DataItem(5))
                CType(e.Item.FindControl("lbnSat"), LinkButton).Text = SafeInteger(e.Item.DataItem(6))
                CType(e.Item.FindControl("lbnSun"), LinkButton).Text = SafeInteger(e.Item.DataItem(7))
                Dim pending = SafeDouble(e.Item.DataItem("Pending"))
                If pending > 0 Then
                    Dim sDate As String() = SafeString(e.Item.DataItem(0)).Split("-".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    Dim sLink As String = String.Format("window.location='OpenBet.aspx?PlayerID={0}", SafeString(UserSession.UserID))
                    sLink += "&SDate={0}&EDate={1}'"
                    sLink = String.Format(sLink, sDate(0), sDate(1))
                    Dim lblPending As Label = CType(e.Item.FindControl("lblPending"), Label)
                    lblPending.Text = FormatNumber(pending, SBCBL.std.GetRoundMidPoint)
                    lblPending.CssClass = "week-balance-peding"
                    lblPending.Attributes.Add("onClick", sLink)
                End If

            End If
        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            If GetSiteType.Equals("SBS") Then
                trchart.Visible = False
            End If
        End Sub

        Public Function FirstDayInWeek(ByVal datetime As DateTime) As DateTime
            Dim monday As DateTime = datetime.AddDays((Today.DayOfWeek - DayOfWeek.Monday) * -1)

            Return monday
        End Function

        Protected Function GetListMonday() As List(Of DateTime)
            Dim currentMonday As DateTime = FirstDayInWeek(Today)
            Dim lasMonday As DateTime = FirstDayInWeek(Today.AddMonths(-3))
            Dim listMonday = new List(Of DateTime)

            ' Add current week
            listMonday.Add(currentMonday)

            Dim tempMonday = currentMonday.AddDays(-7)
            While (lasMonday < tempMonday)
                listMonday.Add(tempMonday)

                tempMonday = tempMonday.AddDays(-7)
            End While
            Return listMonday
        End Function
    End Class

End Namespace