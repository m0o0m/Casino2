Imports SBCBL.std
Imports SBCBL.CacheUtils
Imports SBCBL.Managers

Imports WebsiteLibrary

Namespace SBCSuperAdmin

    Partial Class editPlayerTemplate
        Inherits SBCBL.UI.CSBCUserControl

#Region "Public methods"

        Public Sub ResetPlayerTemplate()
            txtTemplateName.Text = ""
            txtAccountBalance.Text = ""
            txtmaxcredit.Text = ""
            txtmaxcasino.Text = ""
            txtminbetphone.Text = ""
            txtminbedinternet.Text = ""
            txtmax1q.Text = ""
            txtmax2q.Text = ""
            txtmax3q.Text = ""
            txtmax4q.Text = ""
            txtmax1h.Text = ""
            txtmax2h.Text = ""
            txtmaxsingle.Text = ""
            txtmaxparlay.Text = ""
            txtmaxreverse.Text = ""
            txtmaxteaser.Text = ""

        End Sub

        Public Sub LoadPlayerTemplate(ByVal poTemplate As CPlayerTemplate)
            With poTemplate
                txtTemplateName.Text = .TemplateName
                txtAccountBalance.Text = FormatNumber(.AccountBalance)
                txtmaxcredit.Text = FormatNumber(.CreditMaxAmount)
                txtmaxcasino.Text = FormatNumber(.CasinoMaxAmount)
                txtminbetphone.Text = FormatNumber(.CreditMinBetPhone)
                txtminbedinternet.Text = FormatNumber(.CreditMinBetInternet)
                txtmax1q.Text = FormatNumber(.Max1Q)
                txtmax2q.Text = FormatNumber(.Max2Q)
                txtmax3q.Text = FormatNumber(.Max3Q)
                txtmax4q.Text = FormatNumber(.Max4Q)
                txtmax1h.Text = FormatNumber(.Max1H)
                txtmax2h.Text = FormatNumber(.Max2H)
                txtmaxsingle.Text = FormatNumber(.MaxSingle)
                txtmaxparlay.Text = FormatNumber(.CreditMaxParlay)
                txtmaxreverse.Text = FormatNumber(.CreditMaxReverseActionParlay)
                txtmaxteaser.Text = FormatNumber(.CreditMaxTeaserParlay)
            End With
        End Sub

        Public Function GetPlayerTemplate() As CPlayerTemplate
            Dim oPlayerTemplate As New CPlayerTemplate()

            With oPlayerTemplate
                If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                    .SuperAdminID = UserSession.SuperAdminInfo.UserID
                Else
                    .SuperAdminID = UserSession.AgentUserInfo.SuperAdminID
                End If

                .TemplateName = SafeString(txtTemplateName.Text)
                .AccountBalance = SafeDouble(txtAccountBalance.Text)
                .CreditMaxAmount = SafeDouble(txtmaxcredit.Text)
                .CasinoMaxAmount = SafeDouble(txtmaxcasino.Text)
                .CreditMaxParlay = SafeDouble(txtmaxparlay.Text)
                .CreditMaxReverseActionParlay = SafeDouble(txtmaxreverse.Text)
                .CreditMaxTeaserParlay = SafeDouble(txtmaxteaser.Text)
                .CreditMinBetInternet = SafeDouble(txtminbedinternet.Text)
                .CreditMinBetPhone = SafeDouble(txtminbetphone.Text)
                '.CreditWagerPerGame = SafeDouble(txt.Text)
                .LastSaveBy = UserSession.UserID
                .Max1Q = SafeDouble(txtmax1q.Text)
                .Max2Q = SafeDouble(txtmax2q.Text)
                .Max3Q = SafeDouble(txtmax3q.Text)
                .Max4Q = SafeDouble(txtmax4q.Text)
                .Max1H = SafeDouble(txtmax1h.Text)
                .Max2H = SafeDouble(txtmax2h.Text)
                .MaxSingle = SafeDouble(txtmaxsingle.Text)

                .LastSavedDate = Date.Now.ToUniversalTime
            End With

            Return oPlayerTemplate
        End Function

        Public Sub SetTemplateNameFocus()
            txtTemplateName.Focus()
        End Sub

#End Region

    End Class

End Namespace
