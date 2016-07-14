Imports SBCBL.Managers
Imports SBCBL.std
Imports System.Data
Imports WebsiteLibrary
Imports SBCBL.CacheUtils
Imports System.Linq

Namespace SBSAgents

    Partial Class SportAllowance
        Inherits SBCBL.UI.CSBCUserControl
        Private _sGameType As String = SBCBL.std.GetSiteType & " GameType"
        Private _oMainGameType As List(Of SBCBL.CacheUtils.CSysSetting)
        Private _sBookmaker As String = SBCBL.std.GetSiteType & " Bookmaker"
        ' Private _SuperBookmakerValue As String = "BookmakerType"
        Public _SuperBookmakerValue As String = "BookmakerType"
        Private _sBookmakerType As String = SBCBL.std.GetSiteType & " BookmakerType"

#Region "property"
        Public Property CurrentBookMakerType() As String
            Get
                Return CType(ViewState("CurrentBookMakerType"), String)
            End Get
            Set(ByVal value As String)
                ViewState("CurrentBookMakerType") = value
            End Set
        End Property

        Public Property CurrentAgentID() As String
            Get
                Return CType(ViewState("CurrentAgentID"), String)
            End Get
            Set(ByVal value As String)
                ViewState("CurrentAgentID") = value
            End Set
        End Property

        Public Property BookMaker() As String
            Get
                Return CType(ViewState("BookMaker"), String)
            End Get
            Set(ByVal value As String)
                ViewState("BookMaker") = value
            End Set
        End Property
#End Region

        Public Property ListGame() As Dictionary(Of String, String)
            Get
                If ViewState("ListGame") Is Nothing Then
                    ViewState("ListGame") = New Dictionary(Of String, String)
                End If
                Return CType(ViewState("ListGame"), Dictionary(Of String, String))
            End Get
            Set(ByVal value As Dictionary(Of String, String))
                ViewState("ListGame") = value
            End Set
        End Property

        Public Property ListGameBlock() As Dictionary(Of String, String)
            Get
                If ViewState("ListGameBlock") Is Nothing Then
                    ViewState("ListGameBlock") = New Dictionary(Of String, String)
                End If
                Return CType(ViewState("ListGameBlock"), Dictionary(Of String, String))
            End Get
            Set(ByVal value As Dictionary(Of String, String))
                ViewState("ListGameBlock") = value
            End Set
        End Property

        Public ReadOnly Property AgentCategory() As String
            Get
                Return UserSession.UserID + " SportAllow"
            End Get
        End Property

        Public ReadOnly Property AgentBookmaker() As String
            Get
                Return UserSession.AgentUserInfo.SuperAgentID & " Manipulation"
            End Get
        End Property

        Public ReadOnly Property AgentgameType() As List(Of SBCBL.CacheUtils.CSysSetting)
            Get
                Dim oMainGameType As List(Of SBCBL.CacheUtils.CSysSetting)
                Dim oCacheManager As New CCacheManager()
                oMainGameType = oCacheManager.GetAllSysSettings(AgentCategory)
                If oMainGameType.Count = 0 Then
                    oMainGameType = oCacheManager.GetAllSysSettings(_sGameType)
                End If
                Return oMainGameType
            End Get
        End Property

#Region "PageLoad"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            'If Not IsPostBack Then
            '    CurrentAgentID = UserSession.UserID.ToString()
            '    CurrentBookMakerType = CurrentAgentID + "_" + _SuperBookmakerValue

            '    BindData(UserSession.UserID.ToString())
            'End If
        End Sub
#End Region

#Region "Bind data"
        Public Sub BindData(ByVal psAgentID As String)
            Dim oSysManager As New CSysSettingManager()
            Dim _oMainGameType As List(Of SBCBL.CacheUtils.CSysSetting) = oSysManager.GetAllSysSettings(_sGameType)
            rptParentNodes.DataSource = _oMainGameType.FindAll(Function(oSysSetting) oSysSetting.SubCategory = "")
            rptParentNodes.DataBind()
        End Sub

        '' bind data for bookmaker
        Public Sub BindDataBookmaker(ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
            If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
                Dim oSysSetting As SBCBL.CacheUtils.CSysSetting = CType(e.Item.DataItem, SBCBL.CacheUtils.CSysSetting)
                'Change bookmakertype if agent's  bookmakertype is empty  
                Dim sBookMakerType As String = GetBookMakerType(oSysSetting)
                Dim lblSubActive As Label = CType(e.Item.FindControl("lblSubActive"), Label)
                Dim sBookmaker As String = UserSession.Cache.GetSysSettings(sBookMakerType).GetValue(oSysSetting.Key)
                '  lblSubActive.Text = oSysSetting.Key & " - " & IIf(sBookmaker.Contains("Manipulation"), "SBS", sBookmaker)
                Dim ddlChoiceBookmaker As DropDownList = CType(e.Item.FindControl("ddlChoiceBookmaker"), DropDownList)

                Dim oSysSettingList As CSysSettingList = UserSession.Cache.GetSysSettingAgentBookMakers(_sBookmaker, UserSession.AgentUserInfo.SuperAgentID)
                'Dim lstSysSetting As CSysSetting = oSysSettingList.Find(Function(x) x.Key.ToString.Equals("Manipulation", StringComparison.CurrentCultureIgnoreCase))
                'If lstSysSetting IsNot Nothing Then
                '    lstSysSetting.Value = AgentBookmaker
                '    lstSysSetting.Key = "SBS"
                'End If
                ddlChoiceBookmaker.DataSource = oSysSettingList
                ddlChoiceBookmaker.DataTextField = "Key"
                ddlChoiceBookmaker.DataValueField = "Value"
                ddlChoiceBookmaker.DataBind()
                ddlChoiceBookmaker.SelectedValue = UserSession.Cache.GetSysSettingAgentBookMakers(_sBookmaker, UserSession.AgentUserInfo.SuperAgentID).GetValue(UserSession.Cache.GetSysSettings(sBookMakerType).GetValue(oSysSetting.Key))
                Dim oSubSysSetting As SBCBL.CacheUtils.CSysSetting = UserSession.Cache.GetSysSettings(sBookMakerType).GetSysSetting(oSysSetting.Key)
                Dim btnUpdate As Button = CType(e.Item.FindControl("btnUpdate"), Button)
                If Not oSubSysSetting Is Nothing Then
                    btnUpdate.CommandArgument = oSubSysSetting.SysSettingID & "|" & oSysSetting.Key
                End If

            End If
        End Sub
#End Region

#Region "Event"


        Protected Sub rptParentNodes_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptParentNodes.ItemDataBound
            If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
                Dim oSysSetting As SBCBL.CacheUtils.CSysSetting = CType(e.Item.DataItem, SBCBL.CacheUtils.CSysSetting)
                Dim rptSubNodes As Repeater = CType(e.Item.FindControl("rptSubNodes"), Repeater)
                rptSubNodes.DataSource = AgentgameType.FindAll(Function(oSys) oSys.SubCategory = oSysSetting.SysSettingID)
                rptSubNodes.DataBind()
            End If
        End Sub

        Protected Sub rptSubNodes_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
            If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
                Dim oSysSetting As SBCBL.CacheUtils.CSysSetting = CType(e.Item.DataItem, SBCBL.CacheUtils.CSysSetting)
                ' Dim lblGame As Label = CType(e.Item.FindControl("lblGame"), Label)
                Dim chkDisbleGame As CheckBox = CType(e.Item.FindControl("chkDisbleGame"), CheckBox)
                chkDisbleGame.Checked = IIf(oSysSetting.Value.Equals("Yes"), True, False)
                ' lblGame.Text = oSysSetting.Key
                ListGame(oSysSetting.Key) = oSysSetting.SubCategory
                BindDataBookmaker(e)
            End If
        End Sub

        Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
            Dim oSysManager As New CSysSettingManager()
            Dim oAgentGameType As List(Of SBCBL.CacheUtils.CSysSetting)
            Dim lstGame As List(Of SBCBL.CacheUtils.CSysSetting)
            oAgentGameType = oSysManager.GetAllSysSettings(AgentCategory)
            Dim bSuccess As Boolean = False
            GetGameBlock()
            lstGame = AgentgameType.FindAll(Function(oSysSetting) oSysSetting.SubCategory <> "")
            If oAgentGameType.Count > 0 Then
                ''update value
                For Each oSysSetting As SBCBL.CacheUtils.CSysSetting In lstGame
                    Dim arrValueSubCategoty = ListGame(oSysSetting.Key).Split("|"c)
                    bSuccess = oSysManager.UpdateValue(AgentCategory, arrValueSubCategoty(1), oSysSetting.Key, arrValueSubCategoty(0))
                    If bSuccess Then
                        ClientAlert("Successfully Saved")
                    Else
                        ClientAlert("Failed to Save Setting")
                    End If
                Next
            Else
                ''insert value
                For Each oSysSetting As SBCBL.CacheUtils.CSysSetting In lstGame
                    Dim arrValueSubCategoty = ListGame(oSysSetting.Key).Split("|"c)
                    bSuccess = oSysManager.AddSysSetting(AgentCategory, oSysSetting.Key, arrValueSubCategoty(0), arrValueSubCategoty(1))
                    If bSuccess Then
                        ClientAlert("Successfully Saved")
                    Else
                        ClientAlert("Failed to Save Setting")
                    End If
                Next

            End If
            UserSession.Cache.ClearAllSysSettings(_sGameType)
            UserSession.Cache.ClearSysSettings(AgentCategory)
            UserSession.Cache.ClearAllSysSettings(AgentCategory)
            UserSession.Cache.ClearSysSettings(_sGameType)
        End Sub

        Protected Sub rptSubNodes_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs)
            Dim oSysManager As New CSysSettingManager()
            Select Case e.CommandName
                Case "EDIT"
                    editBookMaker(e)
                Case "CANCEL"
                    hiddenEditBookMaker()
                Case "UPDATE"
                    Dim sBookMakerType As String = ""
                    Dim ddlChoiceBookmaker As DropDownList = CType(e.Item.FindControl("ddlChoiceBookmaker"), DropDownList)
                    Dim sSyStemSettingID As String = SafeString(e.CommandArgument).Split("|")(0)
                    'get key sys for insert new sys
                    Dim sSyStemSettingKey As String = SafeString(e.CommandArgument).Split("|")(1)

                    If oSysManager.CheckExistSysSetting(SafeString(sSyStemSettingID), CurrentBookMakerType) Then
                        oSysManager.UpdateBookMaker(SafeString(sSyStemSettingID), ddlChoiceBookmaker.SelectedValue, UserSession.UserID)
                        sBookMakerType = CurrentBookMakerType
                    Else
                        sBookMakerType = _sBookmakerType
                        oSysManager.AddSysSetting(CurrentBookMakerType, sSyStemSettingKey, ddlChoiceBookmaker.SelectedValue)
                    End If

                    '' Clear cache

                    UserSession.Cache.ClearAllSysSettings(_sGameType)
                    UserSession.Cache.ClearSysSettings(sBookMakerType)
                    UserSession.Cache.ClearSysSettings(CurrentBookMakerType)
                    BindData(CurrentBookMakerType)
            End Select


        End Sub

#End Region

        Public Sub GetGameBlock()
            For i As Integer = 0 To rptParentNodes.Items.Count - 1
                Dim rptSubNodes As Repeater = CType(rptParentNodes.Items(i).FindControl("rptSubNodes"), Repeater)
                For Each itemRepeater As RepeaterItem In rptSubNodes.Items
                    Dim chkDisbleGame As CheckBox = CType(itemRepeater.FindControl("chkDisbleGame"), CheckBox)
                    Dim HFGame As HiddenField = CType(itemRepeater.FindControl("HFGame"), HiddenField)
                    ListGame(HFGame.Value) = IIf(chkDisbleGame.Checked, "Yes", "No") & "|" & ListGame(HFGame.Value)
                Next
            Next
        End Sub

       
#Region "setup bookmaker"


        Private Sub editBookMaker(ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs)
            Dim sSubActive As [String]()
            hiddenEditBookMaker()
            Dim lblSubActive As Label = CType(e.Item.FindControl("lblSubActive"), Label)
            sSubActive = lblSubActive.Text.Split("-"c)
            lblSubActive.Text = sSubActive(0) & "-"
            BookMaker = sSubActive(1)
            Dim ddlChoiceBookmaker As DropDownList = CType(e.Item.FindControl("ddlChoiceBookmaker"), DropDownList)
            ddlChoiceBookmaker.Visible = True
            Dim btnUpdate As Button = CType(e.Item.FindControl("btnUpdate"), Button)
            btnUpdate.Visible = True
            Dim btnCancel As Button = CType(e.Item.FindControl("btnCancel"), Button)
            btnCancel.Visible = True
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
                    Dim btnSubEdit As Button = CType(rptSubNodes.Items(j).FindControl("btnSubEdit"), Button)
                    btnSubEdit.Visible = True
                    Dim lblSubActive As Label = CType(rptSubNodes.Items(j).FindControl("lblSubActive"), Label)
                    If String.IsNullOrEmpty(lblSubActive.Text.Split("-"c)(1)) Then
                        lblSubActive.Text += BookMaker
                    End If
                Next
            Next

        End Sub

        Public Function GetBookMakerType(ByVal poSysSetting As SBCBL.CacheUtils.CSysSetting) As String
            Dim sCurrentAgentID As String = CurrentAgentID
            Dim sBookMakerType As String = CurrentBookMakerType
            While True
                Dim sBookMakerValue As String = UserSession.Cache.GetSysSettings(sBookMakerType).GetValue(poSysSetting.Key)
                If Not String.IsNullOrEmpty(sBookMakerValue) Then
                    Return sBookMakerType
                End If
                If Not String.IsNullOrEmpty(UserSession.Cache.GetAgentInfo(sCurrentAgentID).ParentID) Then
                    sCurrentAgentID = UserSession.Cache.GetAgentInfo(sCurrentAgentID).ParentID
                    sBookMakerType = sCurrentAgentID + "_" + _SuperBookmakerValue
                Else
                    Return _sBookmakerType
                End If
            End While

            Return _sBookmakerType
        End Function

#End Region

    End Class

End Namespace
