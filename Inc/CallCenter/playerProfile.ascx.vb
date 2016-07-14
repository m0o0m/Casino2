Imports SBCBL.std
Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports FileDB

Namespace SBCCallCenterAgents

    Partial Class Inc_CallCenter_playerProfile
        Inherits SBCBL.UI.CSBCUserControl


        Public Event LoginSucceed(ByVal oPlayer As CPlayer)
        Public Event Logout()

        Protected ReadOnly Property BalanceAmount() As String
            Get
                If Player IsNot Nothing Then
                    Return FormatNumber(Player.BalanceAmount, GetRoundMidPoint())
                End If

                Return ""
            End Get
        End Property

        Private Property PlayerName() As String
            Get
                Return SafeString(ViewState("__PLAYER_NAME"))
            End Get
            Set(ByVal value As String)
                ViewState("__PLAYER_NAME") = value
            End Set
        End Property

        Private ReadOnly Property Player() As CPlayer
            Get
                If UserSession.CCAgentUserInfo.PlayerID <> "" Then
                    Dim oCacheManager As CCacheManager = New CCacheManager()
                    Return oCacheManager.GetPlayerInfo(UserSession.CCAgentUserInfo.PlayerID)
                Else
                    Return Nothing
                End If

            End Get
        End Property

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If UserSession.CCAgentUserInfo.PlayerID <> "" Then
                Dim oCacheManager As CCacheManager = New CCacheManager()
                Dim oPlayer As CPlayer = oCacheManager.GetPlayerInfo(UserSession.CCAgentUserInfo.PlayerID)
                Me.PlayerName = oPlayer.PhoneLogin
                lblName.Text = Me.PlayerName
                tblLogin.Visible = Me.PlayerName = ""
                pnlProfile.Visible = Not tblLogin.Visible
                Return
            End If
            If Not Me.IsPostBack Then
                txtPhoneLogin.Focus()
                'ucContentFileDB.LoadFileDBContent("CCAGENT_LETTER_TEAMPLATE")
            End If

        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            lblName.Text = Me.PlayerName

            tblLogin.Visible = Me.PlayerName = ""
            pnlProfile.Visible = Not tblLogin.Visible

            ShowPlayerStatus()
        End Sub

        Private Function playerLogin() As CPlayer
            If SafeString(txtPhoneLogin.Text) = "" Then
                ClientAlert("Phone Login Is Required", True)
                txtPhoneLogin.Focus() : Return Nothing
            End If
            If SafeString(txtPhonePassowrd.Text) = "" Then
                ClientAlert("Phone Password Is Required", True)
                txtPhonePassowrd.Focus() : Return Nothing
            End If

            Dim oPlayer As CPlayer = (New CPlayerManager).GetPlayerByPhone(SafeString(txtPhoneLogin.Text), SafeString(txtPhonePassowrd.Text), SBCBL.std.GetSiteType)

            If oPlayer Is Nothing Then
                ClientAlert("Unable To Log In. Please Check Phone Login And Phone Password", True)
                txtPhoneLogin.Focus() : Return Nothing
            End If
            If oPlayer.IsLocked Then
                ClientAlert("This Player Has Been Locked. Please Contact Administrator.", True)
                txtPhoneLogin.Focus() : Return Nothing
            End If
            If oPlayer.IsBettingLocked Then
                ClientAlert("This Player Has Been Betting Locked. Please Contact Administrator.", True)
                txtPhoneLogin.Focus() : Return Nothing
            End If

            txtPhoneLogin.Text = ""
            txtPhonePassowrd.Text = ""

            Me.PlayerName = oPlayer.PhoneLogin

            Return oPlayer
        End Function

        Protected Sub btnLogin_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLogin.Click
            Dim oPlayer As CPlayer = playerLogin()
            If oPlayer IsNot Nothing Then
                RaiseEvent LoginSucceed(oPlayer)
            End If
        End Sub

        Protected Sub lbnLogout_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lbnLogout.Click
            Me.PlayerName = ""
            txtPhoneLogin.Focus()
            UserSession.CCAgentUserInfo.PlayerID = ""
            RaiseEvent Logout()
            Response.Redirect("Default.aspx")
        End Sub

        Public Sub ShowPlayerStatus()
            If Player IsNot Nothing Then
                pnlPlayerStatus.Visible = True
                lblCreditLimit.Text = FormatNumber(Player.Template.CreditMaxAmount, SBCBL.std.GetRoundMidPoint())
            End If

        End Sub

    End Class

End Namespace

