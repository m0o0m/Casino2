Imports SBCBL.Managers
Imports SBCBL.std
Imports System.Data
Imports WebsiteLibrary
Imports SBCBL.CacheUtils

Namespace SBCSuperAdmin
    Partial Class GameTypeManager
        Inherits SBCBL.UI.CSBCUserControl

        Private _oMainGameType As List(Of SBCBL.CacheUtils.CSysSetting)
        Private _sBookmaker As String = SBCBL.std.GetSiteType & " Bookmaker"
        Private _sGameType As String = SBCBL.std.GetSiteType & " GameType"
        Private _sBookmakerValue As String = SBCBL.std.GetSiteType & " BookmakerType"
        Private _Category As String = SBCBL.std.GetSiteType & " LineRules"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                BindData()
                LoadShowOneBookMaker()
                LoadTigerSBBookMaker()
            End If
        End Sub

        Public Property BookMaker() As String
            Get
                Return CType(ViewState("BookMaker"), String)
            End Get
            Set(ByVal value As String)
                ViewState("BookMaker") = value
            End Set
        End Property

        'Protected ReadOnly Property GameType() As System.Collections.Generic.Dictionary(Of String, String)
        '    Get
        '        Dim oGameType As New System.Collections.Generic.Dictionary(Of String, String)()

        '        '' Football
        '        oGameType("AFL Football") = "AFL Football"
        '        oGameType("CFL Football") = "CFL Football"
        '        oGameType("NCAA Football") = "NCAA Football"
        '        oGameType("NFL Football") = "NFL Football"
        '        oGameType("NFL Preseason") = "NFL Preseason"
        '        oGameType("UFL Football") = "UFL Football"

        '        '' Basketball
        '        oGameType("NBA Basketball") = "NBA Basketball"
        '        oGameType("NCAA Basketball") = "NCAA Basketball"
        '        oGameType("WNBA Basketball") = "WNBA Basketball"
        '        oGameType("WNCAA Basketball") = "WNCAA Basketball"
        '        '' Baseball
        '        oGameType("MLB AL Baseball") = "MLB AL Baseball"
        '        oGameType("MLB NL Baseball") = "MLB NL Baseball"
        '        oGameType("MLB Baseball") = "MLB Baseball"
        '        oGameType("NCAA Baseball") = "NCAA Baseball"



        '        '' Hockey
        '        oGameType("NHL Hockey") = "NHL Hockey"
        '        oGameType("NCAA Hockey") = "NCAA Hockey"

        '        '' Soccer
        '        oGameType("Argentina") = "Argentina"
        '        oGameType("Brazil") = "Brazil"
        '        oGameType("Bundesliga") = "Bundesliga"
        '        oGameType("Carling Cup") = "Carling Cup"
        '        oGameType("Concacaf") = "Concacaf"
        '        oGameType("Copa America") = "Copa America"
        '        oGameType("Euro Cups") = "Euro Cups"
        '        oGameType("FA Cup") = "FA Cup"
        '        oGameType("La Liga") = "La Liga"
        '        oGameType("Super Liga") = "Super Liga"
        '        oGameType("Ligue 1") = "Ligue 1"
        '        oGameType("Mexican") = "Mexican"
        '        oGameType("MLS") = "MLS"
        '        oGameType("Netherlands") = "Netherlands"
        '        oGameType("Portugal") = "Portugal"
        '        oGameType("Premier") = "Premier"
        '        oGameType("Scotland") = "Scotland"
        '        oGameType("Serie A") = "Serie A"
        '        oGameType("Intl Friendly") = "Intl Friendly"
        '        oGameType("World Cup") = "World Cup"
        '        oGameType("Japan League 2") = "Japan League 2"
        '        oGameType("Japan League 1") = "Japan League 1"
        '        oGameType("Argentina B") = "Argentina B"
        '        oGameType("Brazil B") = "Brazil B"
        '        oGameType("Denmark") = "Denmark"
        '        oGameType("Norway") = "Norway"
        '        oGameType("Segunda") = "Segunda"
        '        oGameType("Euro Under 21") = "Euro Under 21"
        '        oGameType("Serie B") = "Serie B"
        '        oGameType("US Cup") = "US Cup"
        '        oGameType("MLS") = "MLS"
        '        oGameType("Asian Cups") = "Asian Cups"

        '        oGameType("Tennis") = "Tennis"
        '        oGameType("Golf") = "Golf"
        '        '' Other
        '        oGameType("Boxing") = "Boxing"
        '        oGameType("NASCAR") = "NASCAR"

        '        oGameType("Australia") = "Australia"
        '        oGameType("International") = "International"
        '        oGameType("Greece") = "Greece"
        '        oGameType("Japan") = "Japan"
        '        oGameType("Turkey") = "Turkey"
        '        Return oGameType
        '    End Get
        'End Property

        Protected ReadOnly Property PropGameType() As System.Collections.Generic.Dictionary(Of String, String)
            Get
                Dim oGameType As New System.Collections.Generic.Dictionary(Of String, String)()
                '' Proposition
                oGameType("Football") = "Football"
                oGameType("Baseball") = "Baseball"
                oGameType("Basketball") = "Basketball"
                oGameType("Hockey") = "Hockey"
                oGameType("Soccer") = "Soccer"
                Return oGameType
            End Get
        End Property

#Region "Bind data"
        Public Sub BindData()
            Dim oSysManager As New CSysSettingManager()
            _oMainGameType = oSysManager.GetAllSysSettings(_sGameType)

            rptParentNodes.DataSource = _oMainGameType.FindAll(Function(oSysSetting) oSysSetting.SubCategory = "")
            rptParentNodes.DataBind()
        End Sub

        Protected Sub rptParentNodes_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptParentNodes.ItemDataBound
            If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
                Dim ddlBookmaker As CDropDownList = CType(e.Item.FindControl("ddlBookmaker"), CDropDownList)
                ddlBookmaker.DataSource = UserSession.Cache.GetSysSettings(_sBookmaker)
                ddlBookmaker.DataTextField = "Key"
                ddlBookmaker.DataValueField = "Value"
                ddlBookmaker.DataBind()

                Dim chkParentActive As CheckBox = CType(e.Item.FindControl("chkParentActive"), CheckBox)
                Dim ddlGameType As CDropDownList = CType(e.Item.FindControl("ddlGameType"), CDropDownList)

                '' turn to Prop gametype depend on current sport type
                If chkParentActive.Text = "Proposition" Then
                    ddlGameType.DataSource = PropGameType
                Else
                    ddlGameType.DataSource = GetGameType()
                End If

                ddlGameType.DataTextField = "Key"
                ddlGameType.DataValueField = "Value"
                ddlGameType.DataBind()

                If e.Item.ItemIndex = 0 Then
                    Dim lbtnMoveLeft As LinkButton = CType(e.Item.FindControl("lbtnMoveLeft"), LinkButton)
                    lbtnMoveLeft.Visible = False
                End If


                If e.Item.ItemIndex = _oMainGameType.FindAll(Function(oSys) oSys.SubCategory = "").Count - 1 Then
                    Dim lbtnMoveRight As LinkButton = CType(e.Item.FindControl("lbtnMoveRight"), LinkButton)
                    lbtnMoveRight.Visible = False
                End If

                Dim oSysSetting As SBCBL.CacheUtils.CSysSetting = CType(e.Item.DataItem, SBCBL.CacheUtils.CSysSetting)

                chkParentActive.Checked = oSysSetting.Value = "Yes"

                Dim rptSubNodes As Repeater = CType(e.Item.FindControl("rptSubNodes"), Repeater)

                rptSubNodes.DataSource = _oMainGameType.FindAll(Function(oSys) oSys.SubCategory = oSysSetting.SysSettingID)
                rptSubNodes.DataBind()

                '' Disable Reverse checkbox if gameType is Soccer
                Dim bSoccer As Boolean = False
                Dim bOther As Boolean = False
                For Each oSysGameType As SBCBL.CacheUtils.CSysSetting In _oMainGameType.FindAll(Function(oSys) oSys.SubCategory = oSysSetting.SysSettingID)
                    If IsSoccer(oSysGameType.Key) Then
                        bSoccer = True
                        Exit For
                    End If
                    If IsOtherGameType(oSysGameType.Key) Then
                        bOther = True
                        Exit For
                    End If
                Next

            End If
        End Sub

        Protected Sub rptSubNodes_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
            If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
                Dim oSysSetting As SBCBL.CacheUtils.CSysSetting = CType(e.Item.DataItem, SBCBL.CacheUtils.CSysSetting)

                Dim chkSubActive As CheckBox = CType(e.Item.FindControl("chkSubActive"), CheckBox)
                chkSubActive.Text = oSysSetting.Key & " - " & UserSession.Cache.GetSysSettings(_sBookmakerValue).GetValue(oSysSetting.Key)
                chkSubActive.Checked = oSysSetting.Value = "Yes"
                Dim ddlChoiceBookmaker As DropDownList = CType(e.Item.FindControl("ddlChoiceBookmaker"), DropDownList)
                ddlChoiceBookmaker.DataSource = UserSession.Cache.GetSysSettings(_sBookmaker)
                ddlChoiceBookmaker.DataTextField = "Key"
                ddlChoiceBookmaker.DataValueField = "Value"
                ddlChoiceBookmaker.DataBind()
                ddlChoiceBookmaker.SelectedValue = UserSession.Cache.GetSysSettings(_sBookmaker).GetValue(UserSession.Cache.GetSysSettings(_sBookmakerValue).GetValue(oSysSetting.Key))
                Dim oSubSysSetting As SBCBL.CacheUtils.CSysSetting = UserSession.Cache.GetSysSettings(_sBookmakerValue).GetSysSetting(oSysSetting.Key)
                Dim btnUpdate As Button = CType(e.Item.FindControl("btnUpdate"), Button)
                If Not oSubSysSetting Is Nothing Then
                    btnUpdate.CommandArgument = oSubSysSetting.SysSettingID
                End If

                chkSubActive.Attributes("onclick") = String.Format("UpdateGameType('{0}','{1}',this)", e.Item.FindControl("HFSysSetting").ClientID, e.Item.FindControl("HFKey").ClientID)

            End If
        End Sub
#End Region

        Private Function isValid(ByVal ddlGameType As CDropDownList, ByVal ddlBookmaker As CDropDownList, ByVal psSubCategory As String) As Boolean
            If ddlGameType.Value = "" Then
                ClientAlert("Please Choose Game Type.", True)
                ddlGameType.Focus()
                Return False
            End If

            If ddlBookmaker IsNot Nothing AndAlso ddlBookmaker.Value = "" Then
                ClientAlert("Please Choose Bookmaker", True)
                ddlBookmaker.Focus()
                Return False
            End If

            Dim oSysManager As New CSysSettingManager()
            If oSysManager.IsExistedKey(_sGameType, psSubCategory, ddlGameType.Value) Then
                ClientAlert("Name Has Already Existed", True)
                ddlGameType.Focus()
                Return False
            End If

            Return True
        End Function

        Private Function getCheckValue(ByVal chkActive As CheckBox) As String
            If chkActive.Checked Then
                Return "Yes"
            Else
                Return "No"
            End If
        End Function

        Protected Sub rptParentNodes_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptParentNodes.ItemCommand
            Dim oSysManager As New CSysSettingManager()
            Dim oAgentManager As New CAgentManager()
            Select Case e.CommandName
                Case "ADD"
                    Dim chkSubActive As CheckBox = CType(e.Item.FindControl("chkSubActive"), CheckBox)
                    Dim ddlGameType As CDropDownList = CType(e.Item.FindControl("ddlGameType"), CDropDownList)
                    Dim ddlBookmaker As CDropDownList = CType(e.Item.FindControl("ddlBookmaker"), CDropDownList)

                    If Not isValid(ddlGameType, ddlBookmaker, SafeString(e.CommandArgument)) Then
                        Exit Sub
                    End If

                    oSysManager.AddSysSetting(_sGameType, ddlGameType.Value, getCheckValue(chkSubActive), SafeString(e.CommandArgument))
                    oSysManager.AddSysSetting(_sBookmakerValue, ddlGameType.Value, ddlBookmaker.Value, "")

                Case "DELETE"
                    Dim olstKeys As New List(Of String)
                    Dim olstSys As SBCBL.CacheUtils.CSysSettingList = UserSession.Cache.GetSysSettings(_sGameType, SafeString(e.CommandArgument))
                    For Each oSys As SBCBL.CacheUtils.CSysSetting In olstSys
                        olstKeys.Add(oSys.Key)
                    Next

                    '' Delete BookemakerValue first
                    ' oSysManager.DeleteSettingByKey(_sBookmakerValue, olstKeys)
                    For Each skey As String In olstKeys
                        oSysManager.DeleteAllBookMaker(_sBookmakerValue, skey)
                    Next
                    '' Delete GameType
                    oSysManager.DeleteSettingBySubCategory(_sGameType, SafeString(e.CommandArgument))
                    oSysManager.DeleteSetting(SafeString(e.CommandArgument))

                    '' clear cache for all subagent
                    Dim dtAgents As DataTable = oAgentManager.GetAllAgentsBySuperAdminID(UserSession.UserID, Nothing)
                    For Each drAgent As DataRow In dtAgents.Rows
                        UserSession.Cache.ClearSysSettings(SafeString(drAgent("AgentID")) + "_" + _sBookmakerValue)
                    Next

                Case "LEFT"
                    SwapPosition(SafeInteger(e.CommandArgument) - 1, SafeInteger(e.CommandArgument))

                Case "RIGHT"
                    SwapPosition(SafeInteger(e.CommandArgument), SafeInteger(e.CommandArgument) + 1)

            End Select

            '' Clear cache
            UserSession.Cache.ClearAllSysSettings(_sGameType)
            UserSession.Cache.ClearSysSettings(_sBookmakerValue)

            BindData()
        End Sub

        Protected Sub rptSubNodes_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs)
            Dim oSysManager As New CSysSettingManager()
            Dim oAgentManager As New CAgentManager
            Select Case e.CommandName
                Case "DELETE"
                    Dim sCommand() As String = SafeString(e.CommandArgument).Split("|"c)
                    Dim olstKeys As New List(Of String)
                    olstKeys.Add(sCommand(1))

                    '' Delete BookemakerValue first
                    ' oSysManager.DeleteSettingByKey(_sBookmakerValue, olstKeys)
                    oSysManager.DeleteAllBookMaker(_sBookmakerValue, olstKeys(0))
                    '' Delete GameType
                    oSysManager.DeleteSetting(sCommand(0))
                    Dim dtAgents As DataTable = oAgentManager.GetAllAgentsBySuperAdminID(UserSession.UserID, Nothing)
                    '' Clear cache
                    UserSession.Cache.ClearAllSysSettings(_sGameType)
                    UserSession.Cache.ClearSysSettings(_sBookmakerValue)
                    '' clear cache for all subagent
                    For Each drAgent As DataRow In dtAgents.Rows
                        UserSession.Cache.ClearSysSettings(SafeString(drAgent("AgentID")) + "_" + _sBookmakerValue)
                    Next
                    BindData()
                Case "EDIT"
                    editBookMaker(e)
                Case "CANCEL"
                    hiddenEditBookMaker()

                Case "UPDATE"

                    Dim ddlChoiceBookmaker As DropDownList = CType(e.Item.FindControl("ddlChoiceBookmaker"), DropDownList)
                    oSysManager.UpdateBookMaker(SafeString(e.CommandArgument), ddlChoiceBookmaker.SelectedValue, UserSession.UserID)
                    '' Clear cache
                    UserSession.Cache.ClearAllSysSettings(_sGameType)
                    UserSession.Cache.ClearSysSettings(_sBookmakerValue)
                    saveBookmakerAllAgent(CType(e.Item.FindControl("HFKey"), HiddenField).Value, ddlChoiceBookmaker.SelectedValue)
                    BindData()
            End Select


        End Sub

        Private Sub saveBookmakerAllAgent(ByVal psGameType As String, ByVal psBookmaker As String)
            Dim oSysManager As New CSysSettingManager()
            Dim oAgentManager As New CAgentManager()

            Try
                Dim oData As DataTable = oAgentManager.GetAgentsBySuperID(UserSession.UserID, Nothing)
                If oData Is Nothing Then
                    Return
                End If
                For Each dr As DataRow In oData.Rows
                    Dim sAgentID As String = SafeString(dr("AgentID"))
                    Dim odrSystemSetting As DataRow = oSysManager.GetSystemSetting(sAgentID & "_BookmakerType", psGameType)

                    If odrSystemSetting IsNot Nothing Then
                        Dim sSysSettingID As String = SafeString(odrSystemSetting("SysSettingID"))
                        oSysManager.UpdateBookMaker(sSysSettingID, psBookmaker, UserSession.UserID)
                        UserSession.Cache.ClearSysSettings(sAgentID & "_BookmakerType")
                    End If

                    '' Clear cache

                Next
                ClientAlert("Successfully Saved")
            Catch ex As Exception
                ClientAlert("Failed to Save Setting")
            End Try

        End Sub

        Protected Sub btnAddNode_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddNode.Click
            If SafeString(txtNode.Text) = "" Then
                ClientAlert("Please Input Name", True)
                txtNode.Focus()
                Exit Sub
            End If

            Dim oSysManager As New CSysSettingManager()
            If oSysManager.IsExistedKey(_sGameType, "", SafeString(txtNode.Text)) Then
                ClientAlert("Name Has Already Existed", True)
                txtNode.Focus()
                Exit Sub
            End If

            If Not oSysManager.AddSysSetting(_sGameType, SafeString(txtNode.Text), getCheckValue(chkActive), "", rptParentNodes.Items.Count + 1) Then
                ClientAlert("Failed to Save Setting", True)
            Else
                txtNode.Text = ""
                chkActive.Checked = False

                '' Clear cache
                UserSession.Cache.ClearAllSysSettings(_sGameType)
                UserSession.Cache.ClearSysSettings(_sBookmakerValue)

                BindData()
            End If
        End Sub

        Protected Sub chkSubActive_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim oSysManager As New CSysSettingManager
            Dim oChk As CheckBox = CType(sender, CheckBox)
            Dim sSysSettingID As String = CType(oChk.Parent.FindControl("HFSysSetting"), HiddenField).Value

            If oSysManager.UpdateKeyValue(sSysSettingID, oChk.Text.Split("-")(0), getCheckValue(oChk)) Then
                '' Clear cache
                UserSession.Cache.ClearAllSysSettings(_sGameType)
            End If
        End Sub

        Private Sub SwapPosition(ByVal pnFirstP As Integer, ByVal pnSecondP As Integer)
            Dim oSysManager As New CSysSettingManager()
            Dim olstSyssettings As List(Of SBCBL.CacheUtils.CSysSetting) = oSysManager.GetAllSysSettings(_sGameType).FindAll(Function(oSysSetting) oSysSetting.SubCategory = "")

            If pnFirstP < 0 OrElse pnFirstP > olstSyssettings.Count - 1 _
                OrElse pnSecondP < 0 OrElse pnSecondP > olstSyssettings.Count - 1 Then
                Exit Sub
            End If

            oSysManager.UpdateKeyValue(olstSyssettings.Item(pnFirstP).SysSettingID, olstSyssettings.Item(pnFirstP).Key, olstSyssettings.Item(pnFirstP).Value, pnSecondP + 1)

            oSysManager.UpdateKeyValue(olstSyssettings.Item(pnSecondP).SysSettingID, olstSyssettings.Item(pnSecondP).Key, olstSyssettings.Item(pnSecondP).Value, pnFirstP + 1)

            '' Clear cache
            UserSession.Cache.ClearAllSysSettings(_sGameType)
        End Sub

        Private Sub editBookMaker(ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs)
            Dim sSubActive As [String]()
            hiddenEditBookMaker()
            ' Dim rptSubNodes As Repeater = CType(source, Repeater)
            Dim chkSubActive As CheckBox = CType(e.Item.FindControl("chkSubActive"), CheckBox)
            sSubActive = chkSubActive.Text.Split("-"c)
            chkSubActive.Text = sSubActive(0) & "-"
            BookMaker = sSubActive(1)
            Dim ddlChoiceBookmaker As DropDownList = CType(e.Item.FindControl("ddlChoiceBookmaker"), DropDownList)
            ddlChoiceBookmaker.Visible = True
            Dim btnUpdate As Button = CType(e.Item.FindControl("btnUpdate"), Button)
            btnUpdate.Visible = True
            Dim btnCancel As Button = CType(e.Item.FindControl("btnCancel"), Button)
            btnCancel.Visible = True
            Dim btnSubDelete As Button = CType(e.Item.FindControl("btnSubDelete"), Button)
            btnSubDelete.Visible = False
            Dim btnSubEdit As Button = CType(e.Item.FindControl("btnSubEdit"), Button)
            btnSubEdit.Visible = False
        End Sub

        Private Sub hiddenEditBookMaker()
            For i As Integer = 0 To rptParentNodes.Items.Count - 1
                Dim rptSubNodes As Repeater = CType(rptParentNodes.Items(i).FindControl("rptSubNodes"), Repeater)
                For j As Integer = 0 To rptSubNodes.Items.Count - 1
                    Dim ddlChoiceBookmaker As DropDownList = CType(rptSubNodes.Items(j).FindControl("ddlChoiceBookmaker"), DropDownList)
                    ddlChoiceBookmaker.Visible = False
                    Dim btnUpdate As Button = CType(rptSubNodes.Items(j).FindControl("btnUpdate"), Button)
                    btnUpdate.Visible = False
                    Dim btnCancel As Button = CType(rptSubNodes.Items(j).FindControl("btnCancel"), Button)
                    btnCancel.Visible = False
                    Dim btnSubDelete As Button = CType(rptSubNodes.Items(j).FindControl("btnSubDelete"), Button)
                    btnSubDelete.Visible = True
                    Dim btnSubEdit As Button = CType(rptSubNodes.Items(j).FindControl("btnSubEdit"), Button)
                    btnSubEdit.Visible = True
                    Dim chkSubActive As CheckBox = CType(rptSubNodes.Items(j).FindControl("chkSubActive"), CheckBox)
                    If String.IsNullOrEmpty(chkSubActive.Text.Split("-"c)(1)) Then
                        chkSubActive.Text += BookMaker
                    End If
                Next
            Next

        End Sub

        Private Sub LoadShowOneBookMaker()
            Dim oSysManager As New CSysSettingManager()
            Dim sKey As String = "ShowOneBookMaker"
            Dim oShowOneBookMaker As CSysSetting = oSysManager.GetAllSysSettings(_Category).Find(Function(x) x.Key = sKey)
            If oShowOneBookMaker IsNot Nothing Then
                chkAssignBookMaker.Checked = oShowOneBookMaker.Value = "Y"
            End If
        End Sub

        Protected Sub chkAssignBookMaker_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkAssignBookMaker.CheckedChanged
            ''set Display lines of assigned book maker
            Dim sKey As String = "ShowOneBookMaker"
            Dim oSysManager As New CSysSettingManager()
            Dim oShowOneBookMaker As CSysSetting = oSysManager.GetAllSysSettings(_Category).Find(Function(x) x.Key = sKey)

            Dim sValue As String = "N"
            If chkAssignBookMaker.Checked Then
                sValue = "Y"
            End If

            If oShowOneBookMaker IsNot Nothing Then
                oSysManager.UpdateKeyValue(SafeString(oShowOneBookMaker.SysSettingID), sKey, sValue)
            Else
                oSysManager.AddSysSetting(_Category, sKey, sValue)
            End If

            Dim sCacheKey As String = UCase(String.Format("SBC_SYS_SETTINGS_" & _Category))
            Cache.Remove(sCacheKey)

        End Sub

        Private Sub LoadTigerSBBookMaker()
            Dim oSysManager As New CSysSettingManager()
            Dim sKey As String = "TigerSBBookMaker"
            Dim oTigerSBBookMaker As CSysSetting = oSysManager.GetAllSysSettings(_Category).Find(Function(x) x.Key = sKey)
            If oTigerSBBookMaker IsNot Nothing Then
                chkTigerSBBookmaker.Checked = oTigerSBBookMaker.Value = "Y"
            End If
        End Sub

        Protected Sub chkTigerSBBookmaker_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTigerSBBookmaker.CheckedChanged
            ''set Display lines of TigerSB book maker
            Dim sKey As String = "TigerSBBookMaker"
            Dim oSysManager As New CSysSettingManager()
            Dim oTigerSBBookMaker As CSysSetting = oSysManager.GetAllSysSettings(_Category).Find(Function(x) x.Key = sKey)

            Dim sValue As String = "N"
            If chkTigerSBBookmaker.Checked Then
                sValue = "Y"
            End If

            If oTigerSBBookMaker IsNot Nothing Then
                oSysManager.UpdateKeyValue(SafeString(oTigerSBBookMaker.SysSettingID), sKey, sValue)
            Else
                oSysManager.AddSysSetting(_Category, sKey, sValue)
            End If

            Dim sCacheKey As String = UCase(String.Format("SBC_SYS_SETTINGS_" & _Category))
            Cache.Remove(sCacheKey)

        End Sub
    End Class
End Namespace