Imports System.Data
Imports SBCBL.std

Namespace SBSSuperAdmins

    Partial Class PartnerTopMenu
        Inherits SBCBL.UI.CSBCUserControl

#Region "Page's Events"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                BindMenu()
            End If

        End Sub

        Protected Sub rptSubMenu_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs)
            Select Case UCase(e.CommandName)
                Case "CLICK"
                    Dim sURL As String = SafeString(e.CommandArgument)
                    If sURL <> "" Then
                        Response.Redirect(sURL)
                    End If
            End Select
        End Sub

        Protected Sub rptMenu_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptMenu.ItemDataBound
            If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
                Dim oSubMenu As KeyValuePair(Of String, DataTable) = CType(e.Item.DataItem, KeyValuePair(Of String, DataTable))
                Dim rptSubMenu As Repeater = CType(e.Item.FindControl("rptSubMenu"), Repeater)
                Dim lbtn As LinkButton = CType(e.Item.FindControl("lbtnMenu"), LinkButton)

                rptSubMenu.DataSource = oSubMenu.Value
                rptSubMenu.DataBind()
            End If
        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            SetMenuActive(Me.Page.MenuTabName, Me.Page.SideMenuTabName)
        End Sub

#End Region

#Region "Functions"

        Private Sub BindMenu()
            Dim oMenu As New Dictionary(Of String, DataTable)
            Dim oSubMenu As DataTable

            '' Sub SysTem Manager
            oSubMenu = GetMenuTable()

         
            '' Sub Reports Manager
            oSubMenu = GetMenuTable()

            'Agent Balance
            oSubMenu.Rows.Add(New Object() {"Agent Balance Reports", "Agent Balance Reports", "SAGENT_BALANCE_REPORTS", "SuperAgentBalance.aspx"})

            'Player Balance
            oSubMenu.Rows.Add(New Object() {"Player Balance Reports", "Player Balance Reports", "SPLAYER_BALANCE_REPORTS", "SuperPlayerBalance.aspx"})

            'Pending tickets
            oSubMenu.Rows.Add(New Object() {"Pending Wagers Reports", "Pending Wagers Reports", "PENDING_WAGERS_REPORTS", "PendingTickets.aspx"})

            'P/L Reports
            oSubMenu.Rows.Add(New Object() {"P/L Reports", "P/L Reports", "PL_REPORTS", "PLReports.aspx"})

            'IP Reports
            oSubMenu.Rows.Add(New Object() {"IP Reports", "IP Reports", "IP_REPORTS", "IPReports.aspx"})

            'Position Reports
            oSubMenu.Rows.Add(New Object() {"Agent Position Reports", "Agent Position Reports", "SUPER_POSITION_REPORTS", "SuperPositionReport.aspx"})

            'Transactions Manager
            oSubMenu.Rows.Add(New Object() {"Transactions Manager", "Transactions Manager", "TRANSACTIONS", "Transactions.aspx"})

            oMenu.Add("REPORTS MANAGEMENT", oSubMenu)

            '' Sub Account status
            oSubMenu = GetMenuTable()

            'Change Password
            oSubMenu.Rows.Add(New Object() {"Change Password", "Change Password", "CHANGEPASSWORD", "PartnerChangePassword.aspx"})


            oMenu.Add("ACCOUNT STATUS", oSubMenu)

            rptMenu.DataSource = oMenu
            rptMenu.DataBind()
        End Sub

        Private Function GetMenuTable() As DataTable
            Dim odtMenus As New DataTable
            odtMenus.Columns.Add("MenuText", GetType(String))
            odtMenus.Columns.Add("MenuToolTip", GetType(String))
            odtMenus.Columns.Add("MenuValue", GetType(String)) 'as the same value of Me.Page.SideMenuTabName, use for set menu active
            odtMenus.Columns.Add("MenuUrl", GetType(String))

            Return odtMenus
        End Function

        Private Sub SetMenuActive(ByVal psMenu As String, ByVal psSubMenu As String)
            For Each oMenu As RepeaterItem In rptMenu.Items
                If Not (oMenu.ItemType = ListItemType.AlternatingItem OrElse oMenu.ItemType = ListItemType.Item) Then
                    Continue For
                End If

                '' Set selected main menu
                Dim liMenu As HtmlControl = CType(oMenu.FindControl("liMenu"), HtmlControl)
                If liMenu.Attributes("menu") = psMenu Then
                    liMenu.Attributes("class") = "selected"
                Else
                    liMenu.Attributes("class") = ""
                End If

                Dim rptSubMenu As Repeater = CType(oMenu.FindControl("rptSubMenu"), Repeater)
                For Each oSubMenu As RepeaterItem In rptSubMenu.Items
                    If Not (oSubMenu.ItemType = ListItemType.AlternatingItem OrElse oSubMenu.ItemType = ListItemType.Item) Then
                        Continue For
                    End If

                    '' Set selected sub menu
                    Dim tdSubMenu As HtmlControl = CType(oSubMenu.FindControl("tdSubMenu"), HtmlControl)
                    If tdSubMenu.Attributes("menu") = psSubMenu Then
                        tdSubMenu.Attributes("class") = "selected"
                    Else
                        tdSubMenu.Attributes("class") = ""
                    End If
                Next
            Next
        End Sub

#End Region

    End Class

End Namespace
