Imports SBCBL.std
Imports SBCBL.CacheUtils
Imports SBCBL.Managers

Imports WebsiteLibrary

Namespace SBCSuperAdmin

    Partial Class editPlayerTemplateLimits
        Inherits SBCBL.UI.CSBCUserControl

#Region "Properties"

        Private Property TemplateLimits() As CPlayerTemplateLimitList
            Get
                If ViewState("PLAYER_TEMPLATE_LIMITS") Is Nothing Then
                    ViewState("PLAYER_TEMPLATE_LIMITS") = New CPlayerTemplateLimitList
                End If

                Return CType(ViewState("PLAYER_TEMPLATE_LIMITS"), CPlayerTemplateLimitList)
            End Get
            Set(ByVal value As CPlayerTemplateLimitList)
                ViewState("PLAYER_TEMPLATE_LIMITS") = value
            End Set
        End Property

        Private Property GameTypes() As List(Of String)
            Get
                If ViewState("GAME_TYPES") Is Nothing Then
                    ViewState("GAME_TYPES") = (New CGameManager).GetGameTypes()
                End If

                Return CType(ViewState("GAME_TYPES"), List(Of String))
            End Get
            Set(ByVal value As List(Of String))
                ViewState("GAME_TYPES") = value
            End Set
        End Property

#End Region

#Region "Public methods"

        Public Sub ResetPlayerTemplateLimits()
            Me.TemplateLimits = Nothing
            bindWagerLimits()
        End Sub

        Public Sub LoadPlayerTemplateLimits(ByVal poTemplateLimits As CPlayerTemplateLimitList)
            Me.TemplateLimits = poTemplateLimits
            bindWagerLimits()
        End Sub

        Public Function GetPlayerTemplateLimits() As CPlayerTemplateLimitList
            Me.GameTypeOLD = ""
            Me.TemplateLimits = Nothing

            For Each oriItem As RepeaterItem In rptWagerLimits.Items
                If Not (oriItem.ItemType = ListItemType.AlternatingItem OrElse oriItem.ItemType = ListItemType.Item) Then
                    Continue For
                End If

                Dim ddlGameType As CDropDownList = CType(oriItem.FindControl("ddlGameTypes"), CDropDownList)

                If ddlGameType.Visible Then
                    Me.GameTypeOLD = SafeString(ddlGameType.Value)
                End If

                Dim sGameType As String = Me.GameTypeOLD
                Dim sContext As String = SafeString(CType(oriItem.FindControl("txtContext"), TextBox).Text)
                Dim nPointSpreadP As Double = SafeDouble(CType(oriItem.FindControl("txtPointSpreadP"), TextBox).Text)
                Dim nTotalPointsP As Double = SafeDouble(CType(oriItem.FindControl("txtTotalPointsP"), TextBox).Text)
                Dim nMoneyLineP As Double = SafeDouble(CType(oriItem.FindControl("txtMoneyLineP"), TextBox).Text)
                Dim nPointSpreadI As Double = SafeDouble(CType(oriItem.FindControl("txtPointSpreadI"), TextBox).Text)
                Dim nTotalPointsI As Double = SafeDouble(CType(oriItem.FindControl("txtTotalPointsI"), TextBox).Text)
                Dim nMoneyLineI As Double = SafeDouble(CType(oriItem.FindControl("txtMoneyLineI"), TextBox).Text)

                If Not isExistedWagerLimit(sGameType, sContext) Then
                    Dim oTempLimit As CPlayerTemplateLimit = addWagerLimit(sGameType, sContext _
                                                                       , nPointSpreadP, nTotalPointsP, nMoneyLineP _
                                                                       , nPointSpreadP, nTotalPointsP, nMoneyLineP)
                    oTempLimit.PlayerTemplateLimitID = SafeString(CType(oriItem.FindControl("hfLimitID"), HiddenField).Value)
                End If
            Next

            Return Me.TemplateLimits
        End Function

#End Region

        Private GameTypeOLD As String = "" 'use for rptWagerLimits.ItemDataBound event

        Private Sub bindWagerLimits()
            Dim oLimits As List(Of CPlayerTemplateLimit) = (From oLimit As CPlayerTemplateLimit In Me.TemplateLimits _
                                                            Order By oLimit.GameType, oLimit.Context _
                                                            Select oLimit).ToList
            rptWagerLimits.DataSource = oLimits
            rptWagerLimits.DataBind()
        End Sub

        Private Function addWagerLimit(ByVal psGameType As String, ByVal psContext As String _
                                , ByVal pnPointSpreadP As Double, ByVal pnTotalPointsP As Double, ByVal pnMoneyLineP As Double _
                                , ByVal poPointSpreadI As Double, ByVal poTotalPointsI As Double, ByVal poMoneyLineI As Double) As CPlayerTemplateLimit


            Dim oTempLimit As New CPlayerTemplateLimit()
            Me.TemplateLimits.Add(oTempLimit)

            With oTempLimit ' please fix here
                .GameType = psGameType
                .Context = psContext
                .MaxParlay = pnPointSpreadP
                .MaxReverse = poPointSpreadI
                .MaxSingle = pnTotalPointsP
                .MaxTeaser = poTotalPointsI
            End With

            Return oTempLimit
        End Function

        Private Function checkWagerLimit(ByVal poGameType As CDropDownList, ByVal poContext As TextBox) As Boolean
            Dim sGameType As String = SafeString(poGameType.Value)
            If sGameType = "" Then
                ClientAlert("Wager Limit Is Required", True)
                poGameType.Focus()
                Return False
            End If

            Dim sContext As String = SafeString(poContext.Text)
            If sContext = "" Then
                ClientAlert("Period Is Required", True)
                poContext.Focus()
                Return False
            End If

            If isExistedWagerLimit(sGameType, sContext) Then
                ClientAlert("Period '" & sContext & "' Has Already Exited", True)
                poContext.Focus()
                Return False
            End If

            Return True
        End Function

        Private Function isExistedWagerLimit(ByVal psGameType As String, ByVal psContext As String) As Boolean
            Return Me.TemplateLimits.SingleOrDefault(Function(oItem) _
                                                         UCase(oItem.GameType) = UCase(psGameType) AndAlso UCase(oItem.Context) = UCase(psContext) _
                                                    ) IsNot Nothing
        End Function

        Private Sub getControlsWagerLimit(ByVal poItem As RepeaterItem, ByRef oGameType As CDropDownList, ByRef oContext As TextBox _
                                  , ByRef oPointSpreadP As TextBox, ByRef oTotalPointsP As TextBox, ByRef oMoneyLineP As TextBox _
                                  , ByRef oPointSpreadI As TextBox, ByRef oTotalPointsI As TextBox, ByRef oMoneyLineI As TextBox)

            oGameType = CType(poItem.FindControl("ddlGameTypes"), CDropDownList)
            oContext = CType(poItem.FindControl("txtContext"), TextBox)
            oPointSpreadP = CType(poItem.FindControl("txtPointSpreadP"), TextBox)
            oTotalPointsP = CType(poItem.FindControl("txtTotalPointsP"), TextBox)
            oMoneyLineP = CType(poItem.FindControl("txtMoneyLineP"), TextBox)
            oPointSpreadI = CType(poItem.FindControl("txtPointSpreadI"), TextBox)
            oTotalPointsI = CType(poItem.FindControl("txtTotalPointsI"), TextBox)
            oMoneyLineI = CType(poItem.FindControl("txtMoneyLineI"), TextBox)
        End Sub

        Protected Sub rptWagerLimits_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptWagerLimits.ItemCommand
            Select Case UCase(e.CommandName)
                Case "ADD_NEW_LIMIT"
                    Dim ddlGameTypes As CDropDownList = Nothing, txtContext As TextBox = Nothing
                    Dim txtPointSpreadP As TextBox = Nothing, txtTotalPointsP As TextBox = Nothing, txtMoneyLineP As TextBox = Nothing
                    Dim txtPointSpreadI As TextBox = Nothing, txtTotalPointsI As TextBox = Nothing, txtMoneyLineI As TextBox = Nothing

                    getControlsWagerLimit(e.Item, ddlGameTypes, txtContext _
                                            , txtPointSpreadP, txtTotalPointsP, txtMoneyLineP _
                                            , txtPointSpreadI, txtTotalPointsI, txtMoneyLineI)

                    If checkWagerLimit(ddlGameTypes, txtContext) Then
                        addWagerLimit(ddlGameTypes.Value, SafeString(txtContext.Text) _
                               , SafeDouble(txtPointSpreadP.Text), SafeDouble(txtTotalPointsP.Text), SafeDouble(txtMoneyLineP.Text) _
                               , SafeDouble(txtPointSpreadI.Text), SafeDouble(txtTotalPointsI.Text), SafeDouble(txtMoneyLineI.Text))

                        bindWagerLimits()
                    End If

                Case "DELETE_LIMIT"
                    Dim oTempLimit As CPlayerTemplateLimit = Me.TemplateLimits.SingleOrDefault( _
                                                                Function(oItem) oItem.PlayerTemplateLimitID = SafeString(e.CommandArgument))
                    If oTempLimit IsNot Nothing Then
                        Me.TemplateLimits.Remove(oTempLimit)
                        bindWagerLimits()
                    End If
            End Select
        End Sub

        Protected Sub rptWagerLimits_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptWagerLimits.ItemDataBound
            Select Case e.Item.ItemType
                Case ListItemType.Footer
                    Dim ddlGameTypes As CDropDownList = CType(e.Item.FindControl("ddlGameTypes"), CDropDownList)
                    ddlGameTypes.DataSource = Me.GameTypes
                    ddlGameTypes.DataBind()

                Case ListItemType.Item, ListItemType.AlternatingItem
                    Dim oTempLimit As CPlayerTemplateLimit = CType(e.Item.DataItem, CPlayerTemplateLimit)

                    ''get controls
                    Dim ddlGameTypes As CDropDownList = Nothing, txtContext As TextBox = Nothing
                    Dim txtPointSpreadP As TextBox = Nothing, txtTotalPointsP As TextBox = Nothing, txtMoneyLineP As TextBox = Nothing
                    Dim txtPointSpreadI As TextBox = Nothing, txtTotalPointsI As TextBox = Nothing, txtMoneyLineI As TextBox = Nothing

                    getControlsWagerLimit(e.Item, ddlGameTypes, txtContext _
                                            , txtPointSpreadP, txtTotalPointsP, txtMoneyLineP _
                                            , txtPointSpreadI, txtTotalPointsI, txtMoneyLineI)

                    ''bind game types dropdown
                    ddlGameTypes.DataSource = Me.GameTypes
                    ddlGameTypes.DataBind()

                    If Me.GameTypeOLD <> UCase(oTempLimit.GameType) Then
                        Me.GameTypeOLD = UCase(oTempLimit.GameType)
                        ddlGameTypes.Visible = True
                    Else
                        ddlGameTypes.Visible = False
                    End If

                    'load data
                    ddlGameTypes.Value = oTempLimit.GameType
                    txtContext.Text = oTempLimit.Context
                    ''Please fix here
                    txtMoneyLineI.Text = FormatNumber(oTempLimit.MaxParlay)
                    txtMoneyLineP.Text = FormatNumber(oTempLimit.MaxReverse)
                    txtPointSpreadI.Text = FormatNumber(oTempLimit.MaxSingle)
                    txtPointSpreadP.Text = FormatNumber(oTempLimit.MaxTeaser)
                    CType(e.Item.FindControl("btnDelete"), Button).CommandArgument = oTempLimit.PlayerTemplateLimitID
                    CType(e.Item.FindControl("hfLimitID"), HiddenField).Value = oTempLimit.PlayerTemplateLimitID
            End Select
        End Sub

    End Class

End Namespace

