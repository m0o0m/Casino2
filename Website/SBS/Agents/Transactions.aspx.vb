Imports SBCBL.std

Namespace SBSAgents.Management
    Partial Class Transactions
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Transactions"
            MenuTabName = "HOME"
            SubMenuActive = "TRANSACTIONS"            
            DisplaySubTitlle(Me.Master, "Transactions")
        End Sub

        Protected Sub lbtTab_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            'setTabActive(CType(sender, LinkButton).CommandArgument)
            Response.Redirect("Transactions.aspx?tab=" & CType(sender, LinkButton).CommandArgument)
        End Sub

        Private Sub setTabActive(ByVal psTabKey As String)

            lbtPlayers.Attributes.Remove("href")
            lbtSubAgents.Attributes.Remove("href")

            lbtPlayers.Attributes.Add("href", "#" + tabContent1.ClientID)
            lbtSubAgents.Attributes.Add("href", "#" + tabContent2.ClientID)

            liPLAYERS.Attributes.Remove("class")
            liSUB_AGENTS.Attributes.Remove("class")

            tabContent1.Attributes.Remove("class")
            tabContent2.Attributes.Remove("class")

            tabContent1.Attributes.Add("class", "tab-pane fade in")
            tabContent2.Attributes.Add("class", "tab-pane fade in")

            ucTransactionSubAgent.UserType = SBCBL.CEnums.ETransactionUserType.SuperAgent

            Select Case True
                Case UCase(psTabKey) = "SUB_AGENTS"
                    liSUB_AGENTS.Attributes.Add("class", "active")
                    tabContent2.Attributes.Add("class", "tab-pane fade in active")
                    ucTransactionPlayer.Visible = False
                    ucTransactionSubAgent.Visible = True
                Case Else
                    liPLAYERS.Attributes.Add("class", "active")
                    tabContent1.Attributes.Add("class", "tab-pane fade in active")
                    ucTransactionPlayer.Visible = True
                    ucTransactionSubAgent.Visible = False
            End Select
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                Dim oMng As New SBCBL.Managers.CAgentManager()
                If oMng.NumOfSubAgents(UserSession.UserID) > 0 Then
                    lbtSubAgents.Visible = True
                    liSUB_AGENTS.Visible = True
                End If

                setTabActive(SBCBL.std.SafeString(Request("tab"))) 'if request tab is empty, default Players tab
            End If
        End Sub
    End Class
End Namespace