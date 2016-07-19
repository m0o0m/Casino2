Imports System.Data
Imports SBCBL.std

Namespace SBCCallCenterAgents

    Partial Class LeftContent
        Inherits SBCBL.UI.CSBCUserControl

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not Me.IsPostBack Then
                bindMenus()

                setMenuActive(Me.Page.SideMenuTabName)
            End If
        End Sub

        Private Sub bindMenus()
            Dim odtMenus As New DataTable
            odtMenus.Columns.Add("MenuText", GetType(String))
            odtMenus.Columns.Add("MenuToolTip", GetType(String))
            odtMenus.Columns.Add("MenuValue", GetType(String)) 'as the same value of Me.Page.SideMenuTabName, use for set menu active
            odtMenus.Columns.Add("MenuUrl", GetType(String))


            odtMenus.Rows.Add(New Object() {"Lines Monitor", "Lock/Unlock game bettings", "LINES_MONITOR", "LinesMonitor.aspx"})
            odtMenus.Rows.Add(New Object() {"Setup Quater Lines", "Manual Muti Game Lines", "MANUAL_LINE", "SetupQuarter.aspx"})
            odtMenus.Rows.Add(New Object() {"Update Game Scores", "Manual Muti Games", "MANUAL_GAME", "UpdateGameScores.aspx"})
            odtMenus.Rows.Add(New Object() {"Update Quarter Scores", "Manual Games Quaters", "MANUAL_QUATERS", "UpdateQuarterScores.aspx"})

            rptMenus.DataSource = odtMenus
            rptMenus.DataBind()
        End Sub

        Private Sub setMenuActive(ByVal psMenuValue As String)
            For Each oriItem As RepeaterItem In rptMenus.Items
                If Not (oriItem.ItemType = ListItemType.AlternatingItem OrElse oriItem.ItemType = ListItemType.Item) Then
                    Continue For
                End If
                Dim lbnMenu As LinkButton = CType(oriItem.FindControl("lbnMenu"), LinkButton)

                lbnMenu.CssClass = SafeString(IIf(psMenuValue = lbnMenu.CommandArgument, "selected", ""))
            Next
        End Sub

        Protected Sub rptMenus_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptMenus.ItemCommand
            Dim sMenuUrl As String = SafeString(e.CommandName)

            If sMenuUrl <> "" Then
                Response.Redirect(sMenuUrl)
            Else
                ClientAlert("Not Yet Set URL To Redirect", True)
            End If
        End Sub

    End Class

End Namespace