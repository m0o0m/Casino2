Imports WebsiteLibrary.DBUtils
Imports SBCBL.std
Imports System.Data
Imports SBCBL.CacheUtils
Imports SBCBL.Managers

Namespace SBCWebsite
    Partial Class PlayerLimit
        Inherits SBCBL.UI.CSBCUserControl
        Protected _PostBackString As String = String.Empty
        Dim log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

#Region "Save"

        Public Event SavePlayerTemplate As System.EventHandler
        Public Event UpdateLimitUseDefaultValues As System.EventHandler

        Public Function SaveCopy(ByVal psPlayerTemplateID As String) As Boolean
            Try
                ucFootball.SaveCopy(psPlayerTemplateID)
                ucBaseball.SaveCopy(psPlayerTemplateID)
                ucBasketball.SaveCopy(psPlayerTemplateID)
                ucHockey.SaveCopy(psPlayerTemplateID)
                ucSoccer.SaveCopy(psPlayerTemplateID)
                ucOther.SaveCopy(psPlayerTemplateID)
                ucGolf.SaveCopy(psPlayerTemplateID)
                ucTennis.SaveCopy(psPlayerTemplateID)
                Try
                    '  ClientAlert(Session("PlayerID"), True)
                    UserSession.Cache.ClearPlayerInfo(Session("PlayerID"))
                Catch ex As Exception

                End Try
                Return True
            Catch ex As Exception
                LogError(log, "Save Limit Copy ", ex)
            End Try
            Return False
        End Function

        Public Function Save() As Boolean
            Try
                ucFootball.Save()
                ucBaseball.Save()
                ucBasketball.Save()
                ucHockey.Save()
                ucSoccer.Save()
                ucOther.Save()
                ucGolf.Save()
                ucTennis.Save()
                ClientAlert(Session("CurrentPlayer"))
                UserSession.Cache.ClearPlayerInfo(Session("CurrentPlayer"))
                Return True
            Catch ex As Exception
                LogError(log, "save limit ", ex)
            End Try
            Return False
        End Function

#End Region

#Region "Load"
        Public Sub LoadPlayerSetting(ByVal poPlayerTemplateID As String)
            ucFootball.SportType = "Football"
            ucBasketball.SportType = "Basketball"
            ucHockey.SportType = "Hockey"
            ucBaseball.SportType = "Baseball"
            ucSoccer.SportType = "Soccer"
            ucGolf.SportType = "Golf"
            ucTennis.SportType = "Tennis"
            ucOther.SportType = "Other"
            ucFootball.LoadPlayerSettingData(poPlayerTemplateID)
            ucBaseball.LoadPlayerSettingData(poPlayerTemplateID)
            ucBasketball.LoadPlayerSettingData(poPlayerTemplateID)
            ucHockey.LoadPlayerSettingData(poPlayerTemplateID)
            ucSoccer.LoadPlayerSettingData(poPlayerTemplateID)
            ucOther.LoadPlayerSettingData(poPlayerTemplateID)
            ucGolf.LoadPlayerSettingData(poPlayerTemplateID)
            ucTennis.LoadPlayerSettingData(poPlayerTemplateID)
        End Sub

        Public Sub LoadDefaultLimitValues(ByVal poDefaultValues As Dictionary(Of String, String))
            ucFootball.SetDefaultLimitValues(poDefaultValues)
            ucBaseball.SetDefaultLimitValues(poDefaultValues)
            ucBasketball.SetDefaultLimitValues(poDefaultValues)
            ucHockey.SetDefaultLimitValues(poDefaultValues)
            ucSoccer.SetDefaultLimitValues(poDefaultValues)
            ucGolf.SetDefaultLimitValues(poDefaultValues)
            ucTennis.SetDefaultLimitValues(poDefaultValues)
            ucOther.SetDefaultLimitValues(poDefaultValues)
        End Sub
#End Region

#Region "Controls Event"
        Protected Sub btnUpdateLimitTop_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateLimitTop.Click
            RaiseEvent SavePlayerTemplate(Me, New EventArgs())
        End Sub

        Protected Sub btnUpdateLimitBottom_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateLimitBottom.Click
            RaiseEvent SavePlayerTemplate(Me, New EventArgs())
        End Sub
#End Region

#Region "Tabs"
        Private Sub setTabActive(ByVal psTabKey As String)
            'lbnFootball.Attributes.Remove("href")
            'lbnBasketball.Attributes.Remove("href")
            'lbnBaseball.Attributes.Remove("href")
            'lbnHockey.Attributes.Remove("href")
            'lbnSoccer.Attributes.Remove("href")
            'lbnGolf.Attributes.Remove("href")
            'lbnTennis.Attributes.Remove("href")
            'lbnOther.Attributes.Remove("href")

            'lbnFootball.Attributes.Add("href", "#" + tabContent1.ClientID)
            'lbnBasketball.Attributes.Add("href", "#" + tabContent2.ClientID)
            'lbnBaseball.Attributes.Add("href", "#" + tabContent3.ClientID)
            'lbnHockey.Attributes.Add("href", "#" + tabContent4.ClientID)
            'lbnSoccer.Attributes.Add("href", "#" + tabContent5.ClientID)
            'lbnGolf.Attributes.Add("href", "#" + tabContent6.ClientID)
            'lbnTennis.Attributes.Add("href", "#" + tabContent7.ClientID)
            'lbnOther.Attributes.Add("href", "#" + tabContent8.ClientID)

            ucBaseball.Visible = False
            ucBasketball.Visible = False
            ucFootball.Visible = False
            ucHockey.Visible = False
            ucOther.Visible = False
            ucSoccer.Visible = False
            ucGolf.Visible = False
            ucTennis.Visible = False
            Select Case UCase(psTabKey)
                Case "BASEBALL"
                    liBASEBALL.Attributes.Add("class", "active")
                    tabContent3.Attributes.Add("class", "tab-pane fade in active")
                    ucBaseball.Visible = True
                Case "BASKETBALL"
                    liBASKETBALL.Attributes.Add("class", "active")
                    tabContent2.Attributes.Add("class", "tab-pane fade in active")
                    ucBasketball.Visible = True
                    ucBasketball.FindControl("divMaxTeaser").Visible = True
                Case "FOOTBALL"
                    liFOOTBALL.Attributes.Add("class", "active")
                    tabContent1.Attributes.Add("class", "tab-pane fade in active")
                    ucFootball.Visible = True
                    ucFootball.FindControl("divMaxTeaser").Visible = True
                Case "HOCKEY"
                    liHOCKEY.Attributes.Add("class", "active")
                    tabContent4.Attributes.Add("class", "tab-pane fade in active")
                    ucHockey.Visible = True
                Case "OTHER"
                    liOTHER.Attributes.Add("class", "active")
                    tabContent8.Attributes.Add("class", "tab-pane fade in active")
                    ucOther.Visible = True
                Case "GOLF"
                    liGOLF.Attributes.Add("class", "active")
                    tabContent6.Attributes.Add("class", "tab-pane fade in active")
                    ucGolf.Visible = True
                Case "TENNIS"
                    liTENNIS.Attributes.Add("class", "active")
                    tabContent7.Attributes.Add("class", "tab-pane fade in active")
                    ucTennis.Visible = True
                Case "SOCCER"
                    liSOCCER.Attributes.Add("class", "active")
                    tabContent5.Attributes.Add("class", "tab-pane fade in active")
                    ucSoccer.Visible = True
                    ucSoccer.FindControl("divSoccer").Visible = True
            End Select
        End Sub

        Protected Sub lbnTab_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            liFOOTBALL.Attributes.Remove("class")
            liBASKETBALL.Attributes.Remove("class")
            liBASEBALL.Attributes.Remove("class")
            liHOCKEY.Attributes.Remove("class")
            liSOCCER.Attributes.Remove("class")
            liGOLF.Attributes.Remove("class")
            liTENNIS.Attributes.Remove("class")
            liOTHER.Attributes.Remove("class")

            tabContent1.Attributes.Remove("class")
            tabContent2.Attributes.Remove("class")
            tabContent3.Attributes.Remove("class")
            tabContent4.Attributes.Remove("class")
            tabContent5.Attributes.Remove("class")
            tabContent6.Attributes.Remove("class")
            tabContent7.Attributes.Remove("class")
            tabContent8.Attributes.Remove("class")

            tabContent1.Attributes.Add("class", "tab-pane fade in")
            tabContent2.Attributes.Add("class", "tab-pane fade in")
            tabContent3.Attributes.Add("class", "tab-pane fade in")
            tabContent4.Attributes.Add("class", "tab-pane fade in")
            tabContent5.Attributes.Add("class", "tab-pane fade in")
            tabContent6.Attributes.Add("class", "tab-pane fade in")
            tabContent7.Attributes.Add("class", "tab-pane fade in")
            tabContent8.Attributes.Add("class", "tab-pane fade in")

            setTabActive(CType(sender, LinkButton).CommandArgument)
        End Sub
#End Region

#Region "Page Events"
        
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not Me.IsPostBack Then
                setTabActive("Football")
            End If
        End Sub
#End Region

    End Class

End Namespace

