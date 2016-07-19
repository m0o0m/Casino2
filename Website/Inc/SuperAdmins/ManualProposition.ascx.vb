Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports SBCBL.std
Imports System.Data

Namespace SBCSuperAdmin


    Partial Class ManualProposition
        Inherits SBCBL.UI.CSBCUserControl

        Private _sGameType As String = SBCBL.std.GetSiteType & " GameType"


#Region "Property"

        Public Property CollapsedGames() As List(Of String)
            Get
                If ViewState("CollapsedGames") Is Nothing Then
                    ViewState("CollapsedGames") = New List(Of String)
                End If
                Return CType(ViewState("CollapsedGames"), List(Of String))
            End Get
            Set(ByVal value As List(Of String))
                ViewState("CollapsedGames") = value
            End Set
        End Property

        Public Property ListGameTypeKey() As List(Of String)
            Get
                If ViewState("ListGameTypeKey") Is Nothing Then
                    ViewState("ListGameTypeKey") = New List(Of String)
                End If
                Return CType(ViewState("ListGameTypeKey"), List(Of String))
            End Get
            Set(ByVal value As List(Of String))
                ViewState("ListGameTypeKey") = value
            End Set
        End Property

        Public Property SiteType() As String
            Get
                Return SafeString(ViewState("SiteType"))
            End Get
            Set(ByVal value As String)
                ViewState("SiteType") = value
            End Set
        End Property

#End Region

#Region "Page events"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                BindGameType()
                BindGameLines(ddlGameType.SelectedValue)
            End If
        End Sub
#End Region


#Region "Bind Data"

        Public Sub BindGameType()
            Dim oGameTypes As List(Of CSysSetting) = SBCBL.Utils.CSerializedObjectCloner.Clone(Of List(Of CSysSetting))((From oSetting As CSysSetting In UserSession.Cache.GetAllSysSettings(_sGameType) _
                                          Where SafeBoolean(oSetting.Value) And oSetting.SubCategory = "" And oSetting.Key <> "Proposition" _
                                          Order By oSetting.Key, oSetting.ItemIndex Ascending _
                                          Select oSetting).ToList)
            Dim oSysManager As New CSysSettingManager()
            Dim oSysProp = oSysManager.GetAllSysSettings(_sGameType).Find(Function(x) x.Key = "Proposition")
            If oSysProp IsNot Nothing Then
                oGameTypes = oSysManager.GetAllSysSettings(_sGameType).FindAll(Function(x) x.SubCategory = oSysProp.SysSettingID And UCase(x.Value) = "YES")
                ddlGameType.DataTextField = "Key"
                ddlGameType.DataValueField = "Key"
                ddlGameType.DataSource = oGameTypes
                ddlGameType.DataBind()

            End If

        End Sub

        Public Sub BindGameLines(ByVal psSelectedGameType As String)
            '' store current result to LastRecords for compare later
            Dim oListGameType As New List(Of String)
            oListGameType.Add(psSelectedGameType)
            rptProps.DataSource = oListGameType
            rptProps.DataBind()
        End Sub

#End Region

#Region "ItemCommand Event"
        Protected Sub rptPropLines_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
            Dim oDTGame As DataTable = CType(e.Item.DataItem, KeyValuePair(Of String, DataTable)).Value
            If oDTGame.Rows.Count = 0 Then
                e.Item.FindControl("divProp").Visible = False
                Return
            End If

            CType(e.Item.FindControl("lblGameDate"), Literal).Text = SafeString(oDTGame.Rows(0)("GameDate"))
            CType(e.Item.FindControl("lblPropDes"), Literal).Text = SafeString(oDTGame.Rows(0)("PropDescription"))
            Dim rptPropTeams As Repeater = CType(e.Item.FindControl("rptPropTeams"), Repeater)

            rptPropTeams.DataSource = oDTGame
            rptPropTeams.DataBind()
        End Sub

        Protected Sub rptPropTeams_ItemdataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
            Dim lblPropMoneyLine As Label = CType(e.Item.FindControl("lblPropMoneyLine"), Label)
            Dim nMoneyLine As Double = SafeDouble(CType(e.Item.DataItem, DataRowView)("PropMoneyLine"))
            lblPropMoneyLine.Text = SafeString(nMoneyLine)
            lblPropMoneyLine.Enabled = nMoneyLine <> 0

            Dim rdWin As RadioButton = CType(e.Item.FindControl("rdWin"), RadioButton)
            Dim rdLose As RadioButton = CType(e.Item.FindControl("rdLose"), RadioButton)
            Dim rdCancel As RadioButton = CType(e.Item.FindControl("rdCancel"), RadioButton)
            Dim rdPending As RadioButton = CType(e.Item.FindControl("rdPending"), RadioButton)
            Dim sPropStatus As String = SafeString(CType(e.Item.DataItem, DataRowView)("PropStatus"))

            Select Case sPropStatus
                Case "WIN"
                    rdWin.Checked = True
                    Exit Select
                Case "LOSE"
                    rdLose.Checked = True
                    Exit Select
                Case "CANCELLED"
                    rdCancel.Checked = True
                    Exit Select
                Case String.Empty
                    rdPending.Checked = True
                    Exit Select
            End Select

        End Sub

#End Region

        Protected Sub rptProps_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptProps.ItemDataBound
            Dim sType As String = SafeString(e.Item.DataItem)
            Dim rptPropLines As Repeater = e.Item.FindControl("rptPropLines")
            '' load game by game type to each of rptBets
            LoadPropGames(sType, rptPropLines)
        End Sub

        Private Sub LoadPropGames(ByVal psSportType As String, ByVal poRepeater As Repeater)
            Dim oToday = GetEasternDate()
            Dim oGameShows As New Dictionary(Of String, DataTable)
            Dim odtGames As DataTable = New SBCBL.Managers.CGameManager().GetAvailablePropsPending(psSportType, SiteType)
            Dim oListGameIDs As New List(Of String)

            For Each oRow As DataRow In odtGames.Rows
                If oListGameIDs.Contains(SafeString(oRow("GameID"))) Then
                    Continue For
                End If
                odtGames.DefaultView.RowFilter = "GameID = " & SQLString(SafeString(oRow("GameID"))) & " and Bookmaker= 'Pinnacle' and isnull(Propmoneyline,0) <>0 "
                Dim oDTLines = odtGames.DefaultView.ToTable()
                
                oGameShows(SafeString(oRow("GameID"))) = oDTLines
                oListGameIDs.Add(SafeString(oRow("GameID")))
            Next

            poRepeater.DataSource = oGameShows
            poRepeater.DataBind()
            poRepeater.Visible = oGameShows.Count > 0
        End Sub

        Public Function CheckCondition(ByVal prptPropTeams As Repeater) As Boolean
            Dim bWin As Boolean = False
            Dim bLose As Boolean = False
            Dim bCancelled As Boolean = False
            Dim bPending As Boolean = False

            Dim rptPropTeams As Repeater = prptPropTeams
            For i As Integer = 0 To rptPropTeams.Items.Count - 1

                Dim rdWin As RadioButton = CType(rptPropTeams.Items(i).FindControl("rdWin"), RadioButton)
                Dim rdLose As RadioButton = CType(rptPropTeams.Items(i).FindControl("rdLose"), RadioButton)
                Dim rdCancel As RadioButton = CType(rptPropTeams.Items(i).FindControl("rdCancel"), RadioButton)
                Dim rdPending As RadioButton = CType(rptPropTeams.Items(i).FindControl("rdPending"), RadioButton)

                If rdWin.Checked AndAlso bWin Then
                    ClientAlert("Only One Team Win")
                    Return False
                ElseIf rdWin.Checked Then
                    bWin = True
                End If

                If rdCancel.Checked AndAlso bWin Then
                    ClientAlert("Please Choose Only Win Or Cancel")
                    Return False
                ElseIf rdCancel.Checked Then
                    bCancelled = True
                End If

                If rdWin.Checked AndAlso bCancelled Then
                    ClientAlert("Please Choose Only Win Or Cancel")
                    Return False
                ElseIf rdWin.Checked Then
                    bWin = True
                End If

                If rdLose.Checked Then
                    bLose = True
                End If

                If rdPending.Checked Then
                    bPending = True
                End If
            Next

            If Not bPending AndAlso bLose AndAlso Not bCancelled AndAlso Not bWin Then
                ClientAlert("At Least, Must Have One Team Win Or Cancel")
                Return False
            End If

            Return True

        End Function

        Public Sub SaveGameLine(ByVal prptPropTeams As Repeater)
            Dim bWin As Boolean = False
            Dim bLose As Boolean = False
            Dim bCancelled As Boolean = False
            Dim rptPropTeams As Repeater = prptPropTeams
            Dim oGameLineManager As CGameLineManager = New CGameLineManager()
            Dim sGameID As String = ""
            For i As Integer = 0 To prptPropTeams.Items.Count - 1

                Dim rdWin As RadioButton = CType(rptPropTeams.Items(i).FindControl("rdWin"), RadioButton)
                Dim rdLose As RadioButton = CType(rptPropTeams.Items(i).FindControl("rdLose"), RadioButton)
                Dim rdCancel As RadioButton = CType(rptPropTeams.Items(i).FindControl("rdCancel"), RadioButton)
                Dim rdPending As RadioButton = CType(rptPropTeams.Items(i).FindControl("rdPending"), RadioButton)

                Dim hfGameLineID As HiddenField = CType(rptPropTeams.Items(i).FindControl("hfGameLineID"), HiddenField)
                Dim hfGameID As HiddenField = CType(rptPropTeams.Items(i).FindControl("hfGameID"), HiddenField)
                Dim sPropStatus As String = ""

                If rdWin.Checked Then
                    sPropStatus = "WIN"
                End If
                If rdLose.Checked Then
                    sPropStatus = "LOSE"
                End If
                If rdCancel.Checked Then
                    sPropStatus = "CANCELLED"
                End If
                If rdPending.Checked Then
                    sPropStatus = ""
                End If

                sGameID = hfGameID.Value
                oGameLineManager.UpdatePropStatus(hfGameLineID.Value, sPropStatus, UserSession.UserID)
            Next
            Dim oGameManager As CGameManager = New CGameManager()
            oGameManager.UpdateGameStatus(sGameID, True, UserSession.UserID, GetEasternDate())
            BindGameLines(ddlGameType.SelectedValue)
        End Sub

#Region "Event"
        Protected Sub ddlGameType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlGameType.SelectedIndexChanged
            BindGameLines(ddlGameType.SelectedValue)
        End Sub

        Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim rpItem As RepeaterItem = CType(CType(sender, Button).Parent.Parent, RepeaterItem)

            Dim rptPropTeams As Repeater = rpItem.FindControl("rptPropTeams")

            If CheckCondition(rptPropTeams) Then
                SaveGameLine(rptPropTeams)
            End If
        End Sub

        Protected Sub SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)
            BindGameLines(ddlGameType.SelectedValue)
        End Sub

#End Region

    End Class
End Namespace