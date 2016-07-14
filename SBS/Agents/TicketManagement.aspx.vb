Imports SBCBL.std

Partial Class SBS_Agents_TicketManagement
    Inherits SBCBL.UI.CSBCPage

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        PageTitle = "Manage Ticket"
        MenuTabName = "HOME"
        SubMenuActive = "MANAGETICKET"
        DisplaySubTitlle(Me.Master, "Manage Ticket")
    End Sub
End Class
