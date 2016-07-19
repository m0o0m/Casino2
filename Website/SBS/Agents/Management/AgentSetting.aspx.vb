Imports SBCBL.std

Namespace SBSAgents
    Partial Class AgentSetting
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Agents Setting"
            MenuTabName = "GAME_MANAGEMENT"
            SubMenuActive = "GAMES_SETTING"
            DisplaySubTitlle(Me.Master, "Agents Setting")
            lbnSportAllow.CssClass = "selected"
            
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            ResetTabSettings()

            If Not IsPostBack Then
                liSPORT_ALLOW.Attributes.Add("class", "active")
                tabContent1.Attributes.Add("class", "tab-pane fade in active")
            End If
        End Sub

        Protected Sub lbtTab_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            ResetTabSettings()

            liSPORT_ALLOW.Attributes.Remove("class")
            liDISPLAY.Attributes.Remove("class")
            liRISK_CONTROL.Attributes.Remove("class")
            liPARLAY_SETUP.Attributes.Remove("class")
            liPARLAY_ALLOW.Attributes.Remove("class")
            liPARLAY_BW_ALLOW.Attributes.Remove("class")

            tabContent1.Attributes.Remove("class")
            tabContent2.Attributes.Remove("class")
            tabContent3.Attributes.Remove("class")
            tabContent4.Attributes.Remove("class")
            tabContent5.Attributes.Remove("class")
            tabContent6.Attributes.Remove("class")

            tabContent1.Attributes.Add("class", "tab-pane fade in")
            tabContent2.Attributes.Add("class", "tab-pane fade in")
            tabContent3.Attributes.Add("class", "tab-pane fade in")
            tabContent4.Attributes.Add("class", "tab-pane fade in")
            tabContent5.Attributes.Add("class", "tab-pane fade in")
            tabContent6.Attributes.Add("class", "tab-pane fade in")

            lbnDisplay.CssClass = ""
            lbnRiskControl.CssClass = ""
            lbnParlayAllow.CssClass = ""
            lbnParlayAllowBWgame.CssClass = ""
            lbnParlaySetup.CssClass = ""
            lbnDisplay.CssClass = ""
            lbnSportAllow.CssClass = ""
            

            Select Case UCase(CType(sender, LinkButton).CommandArgument)
                Case "SPORT_ALLOW"
                    liSPORT_ALLOW.Attributes.Add("class", "active")
                    tabContent1.Attributes.Add("class", "tab-pane fade in active")
                Case "DISPLAY"
                    liDISPLAY.Attributes.Add("class", "active")
                    tabContent2.Attributes.Add("class", "tab-pane fade in active")
                    If UserSession.AgentUserInfo.IsSuperAgent Then
                        ucTeaserAllow.Visible = True
                    Else
                        ucTeaserAllow.Visible = False
                    End If
                Case "RISK_CONTROL"
                    liRISK_CONTROL.Attributes.Add("class", "active")
                    tabContent3.Attributes.Add("class", "tab-pane fade in active")
                    ucFixedSpreadMoney.BindCurrentUserData()
                    ucGameCircledSettings.GetCircleSettings(UserSession.UserID)
                    If UserSession.AgentUserInfo.IsSuperAgent Then
                        ucJuiceControl.Visible = True
                    Else
                        ucJuiceControl.Visible = False
                    End If
                Case "PARLAY_SETUP"
                    liPARLAY_SETUP.Attributes.Add("class", "active")
                    tabContent4.Attributes.Add("class", "tab-pane fade in active")
                Case "PARLAY_ALLOW"
                    liPARLAY_ALLOW.Attributes.Add("class", "active")
                    tabContent5.Attributes.Add("class", "tab-pane fade in active")
                Case "PARLAY_BW_ALLOW"
                    liPARLAY_BW_ALLOW.Attributes.Add("class", "active")
                    tabContent6.Attributes.Add("class", "tab-pane fade in active")
            End Select
        End Sub

        Protected Sub ResetTabSettings()
            lbnSportAllow.Attributes.Remove("href")
            lbnDisplay.Attributes.Remove("href")
            lbnRiskControl.Attributes.Remove("href")
            lbnParlaySetup.Attributes.Remove("href")
            lbnParlayAllow.Attributes.Remove("href")
            lbnParlayAllowBWgame.Attributes.Remove("href")

            lbnSportAllow.Attributes.Add("href", "#" + tabContent1.ClientID)
            lbnDisplay.Attributes.Add("href", "#" + tabContent2.ClientID)
            lbnRiskControl.Attributes.Add("href", "#" + tabContent2.ClientID)
            lbnParlaySetup.Attributes.Add("href", "#" + tabContent4.ClientID)
            lbnParlayAllow.Attributes.Add("href", "#" + tabContent5.ClientID)
            lbnParlayAllowBWgame.Attributes.Add("href", "#" + tabContent6.ClientID)

           
        End Sub
    End Class
End Namespace

