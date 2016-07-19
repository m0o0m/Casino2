Imports SBCBL.std

Namespace SBSAgents

    Partial Class TopMenu
        Inherits SBCBL.UI.CSBCUserControl

        'Protected Sub lbnMenu_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        '    Dim sURL As String = ""

        '    Select Case CType(sender, LinkButton).CommandArgument
        '        Case "HOME"
        '            sURL = "/SBS/CallCenter/Default.aspx"
        '        Case "SELECT_GAME"
        '            sURL = "/SBS/CallCenter/SelectGame.aspx"
        '        Case "OPEN_BET"
        '            sURL = "/SBS/CallCenter/OpenBets.aspx"
        '        Case "HISTORY"
        '            sURL = "/SBS/CallCenter/History.aspx"
        '        Case "ACCOUNT_STATUS"
        '            sURL = "/SBS/CallCenter/AccountStatus.aspx"
        '        Case "GAME_MANAGER"
        '            sURL = "/SBS/CallCenter/LinesMonitor.aspx"
        '    End Select

        '    If sURL = "" Then
        '        ClientAlert("Not Yet Set URL To Redirect", True)
        '        Return
        '    End If

        '    Response.Redirect(sURL)
        'End Sub

        'Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '    If Not IsPostBack Then
        '        liBetAction.Attributes.Add("class", "border_left_menu")
        '        liOpenBet.Attributes.Add("class", "itemMenu")
        '        Select Case Me.Page.MenuTabName
        '            'Case "HOME"
        '            '    lbnHomepage.ForeColor = Drawing.Color.Black
        '            Case "BET_ACTION"
        '                lbnBetAction.ForeColor = Drawing.Color.Yellow
        '                ActiveMenu(liBetAction, True)
        '                'Case "HISTORY"
        '                '    lbnHistory.ForeColor = Drawing.Color.Black
        '            Case "OPEN_BET"
        '                lbnOpenBet.ForeColor = Drawing.Color.Yellow
        '                ActiveMenu(liOpenBet)
        '                'Case "GAME_MANAGER"
        '                '    lbnGameManager.ForeColor = Drawing.Color.Black
        '                'Case "ACCOUNT_STATUS"
        '                '    lbnAccountStatus.ForeColor = Drawing.Color.Black
        '        End Select
        '    End If

        'End Sub

        'Public Sub ActiveMenu(ByVal pli As HtmlControl, Optional ByVal bfirstItem As Boolean = False)
        '    If (bfirstItem) Then
        '        pli.Attributes.Add("class", "menu_active")
        '        pli.Style.Add("Border-Left", "#FFF solid 1px")
        '        Return
        '    End If
        '    pli.Attributes.Add("class", "menu_active")
        '    pli.Style.Add("padding-right", "1px")
        '    pli.Style.Add("margin-left", "-10px")
        '    pli.Style.Add("Border-Left", "#FFF solid 1px")
        'End Sub

    End Class

End Namespace
