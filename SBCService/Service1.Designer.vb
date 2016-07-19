Imports System.ServiceProcess

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Service1
    Inherits System.ServiceProcess.ServiceBase
    Public Shared CurrentSBCService As Service1

    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    <MTAThread()> _
    <System.Diagnostics.DebuggerNonUserCode()> _
    Shared Sub Main()
        Dim ServicesToRun() As System.ServiceProcess.ServiceBase
        log4net.Config.XmlConfigurator.Configure()
        CurrentSBCService = New Service1()
        ServicesToRun = New System.ServiceProcess.ServiceBase() {CurrentSBCService}
        System.ServiceProcess.ServiceBase.Run(ServicesToRun)
    End Sub

    Private components As System.ComponentModel.IContainer
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.oTimer = New System.Timers.Timer
        Me.TimerPinnacle = New System.Timers.Timer
        Me.oSOCrashTimer = New System.Timers.Timer
        Me.oKeepConnection = New System.Timers.Timer
        CType(Me.oTimer, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TimerPinnacle, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.oSOCrashTimer, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.oKeepConnection, System.ComponentModel.ISupportInitialize).BeginInit()
        '
        'oTimer
        '
        Me.oTimer.Interval = 2000
        '
        'TimerPinnacle
        '
        Me.TimerPinnacle.Enabled = True
        Me.TimerPinnacle.Interval = 900000
        '
        'oSOCrashTimer
        '
        Me.oSOCrashTimer.Enabled = True
        Me.oSOCrashTimer.Interval = 600000
        '
        'oKeepConnection
        '
        Me.oKeepConnection.Interval = 30000
        '
        'Service1
        '
        Me.ServiceName = "Service1"
        CType(Me.oTimer, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TimerPinnacle, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.oSOCrashTimer, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.oKeepConnection, System.ComponentModel.ISupportInitialize).EndInit()

    End Sub
    Friend WithEvents oTimer As System.Timers.Timer
    Friend WithEvents TimerPinnacle As System.Timers.Timer
    Friend WithEvents oSOCrashTimer As System.Timers.Timer
    Friend WithEvents oKeepConnection As System.Timers.Timer

End Class
