Imports WebsiteLibrary.DBUtils
Imports System.Xml
Imports SBCBL.std
Imports System.Text
Imports System.Data
Imports SBCService.CServiceStd
Imports System.Xml.XPath
Imports SBCEngine

Public Class Service1

    Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())
    Private _LastExcuted As DateTime = DateTime.MinValue
    Private _SOManager As CSOManager

#Region "BASIC NT SERVICE FRAMEWORK"
    Private Sub oTimer_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles oTimer.Elapsed
        execService()
    End Sub

    Protected Overrides Sub OnStart(ByVal args() As String)
        Try
            ' log.Info(Me.ServiceName & " STARTED SERVICE" & Date.Now.ToString)

            oTimer.Enabled = True
            oKeepConnection.Enabled = False

            If CServiceStd.EXECUTE_SPORTOPTIONS Then
                InitSportOption()
            End If

            TimerPinnacle.Enabled = True

            '' Reconnect server after crash
            oSOCrashTimer.Interval = SPORTOPTIONS_CRASH_TIME * 60000
            oSOCrashTimer.Enabled = False
        Catch ex As Exception
            log.Error(ex)
        End Try
    End Sub

    Protected Overrides Sub OnShutdown()
        Try
            If _SOManager IsNot Nothing Then _SOManager.EndConnect()
            oSOCrashTimer.Enabled = False
            oTimer.Enabled = False
            TimerPinnacle.Enabled = False
            oKeepConnection.Enabled = False
            ' log.Info(Me.ServiceName & " STOPPED SERVICE" & Date.Now.ToString)
        Catch ex As Exception
            log.Error(ex)
        End Try
        MyBase.OnShutdown()
    End Sub

    Protected Overrides Sub OnStop()
        Try
            If _SOManager IsNot Nothing Then _SOManager.EndConnect()
            oSOCrashTimer.Enabled = False
            oTimer.Enabled = False
            TimerPinnacle.Enabled = False
            oKeepConnection.Enabled = False
            ' log.Info(Me.ServiceName & " STOPPED SERVICE" & Date.Now.ToString)
        Catch ex As Exception
            log.Error(ex)
        End Try
        MyBase.OnStop()
    End Sub
     
#End Region

    Private Sub InitSportOption()
        Dim oThreadLines As New System.Threading.Thread(AddressOf Me.BeginGetData)
        oThreadLines.Start()
    End Sub

    ''' <summary>
    ''' Base function that is called every 2 seconds to execute
    ''' </summary>
    ''' 
    Private Sub execService()
        If CServiceStd.EXECUTE_CALCULATE_FINAL_RESULT Then
            Dim oThreadResult As New System.Threading.Thread(AddressOf execCalculateFinalResult)
            oThreadResult.Start()
        End If

        If CServiceStd.EXECUTE_LOCK_GAME Then
            '' Lock game that gamedate is over, run every minute
            If _LastExcuted = DateTime.MinValue OrElse _LastExcuted.Minute < Now.Minute Then
                _LastExcuted = DateTime.Now
                execLockGame()
            End If
        End If

        If CServiceStd.EXECUTE_CASINO_ACCOUNT_BALANCE Then
            Dim oThreadReset As New System.Threading.Thread(AddressOf syncCasinoAccount)
            oThreadReset.Start()
        End If

        If CServiceStd.EXECUTE_RESET_ACCOUNT_BALANCE Then
            Dim oThreadReset As New System.Threading.Thread(AddressOf resetAccountBalance)
            oThreadReset.Start()
        End If

        If CServiceStd.EXECUTE_WEEKLY_CHARGE Then
            Dim oThreadReset As New System.Threading.Thread(AddressOf weeklyCharge)
            oThreadReset.Start()
        End If

        If EXECUTE_MANIPULATION_UPDATE Then
            Dim oThreadReset As New System.Threading.Thread(AddressOf execMakeAgentBookMaker)
            oThreadReset.Start()
        End If

        '' filter game by preset
        'CGameRule.FilterGameByPreset()

    End Sub

    ''' <summary>
    ''' Lock game when eastern time is past gamedate time.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub execLockGame()
        Dim oDate As DateTime = DateTime.Now.ToUniversalTime().AddHours(-5)
        If Now.IsDaylightSavingTime() Then
            oDate = oDate.AddHours(1)
        End If

        Dim sSQL As String = "update Games set IsLock = 'Y' where GameDate < " & SQLString(oDate)
        Dim oDB As New WebsiteLibrary.DBUtils.CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

        log.Debug("Locking all game that started: " & sSQL)
        Try
            oDB.executeNonQuery(sSQL)
        Catch ex As Exception
        Finally
            oDB.closeConnection()
        End Try
    End Sub

#Region "Manipulation"
    Public Sub execMakeAgentBookMaker()
        Try

            If (SPORTS_BOOKMAKER_AGENT_EXECUTION_START > -1) Then
                'service timer already spawned a thread to collect SPORTS_BOOKMAKER_AGENT_ data and 
                'it still hasn't completed, so check to make sure it's not taking too long
                Dim nTimeExecuting As Double = Timer - SPORTS_BOOKMAKER_AGENT_EXECUTION_START
                If nTimeExecuting > SPORTSINSIGHTS_MAX_TIMEOUT Then
                    Throw New TimeoutException("SPORTS BOOKMAKER AGENT Exceeded Max Timeout: " & nTimeExecuting)
                End If
                Return
            End If
            SPORTS_BOOKMAKER_AGENT_EXECUTION_START = Timer 'this thread is now master processing thread for SPORTS_BOOKMAKER_AGENT
            Dim oStatedTime As DateTime = Now

            Dim oAgentBookmaker As New CAgentBookMaker
            oAgentBookmaker.UpdateBookMaker("SBS")

            ' log.Info("SPORTS BOOKMAKER AGENT Executed time: " & Now.Subtract(oStatedTime).TotalMilliseconds & " ms")
        Catch err As Exception
            CatchSBCServiceError(log, err)
        End Try
        'reset timer means we're done with the processing thread and another one can start!
        SPORTS_BOOKMAKER_AGENT_EXECUTION_START = -1
    End Sub
#End Region

#Region "Final Result"
    Private Sub execCalculateFinalResult()
        Dim odbSQL As WebsiteLibrary.DBUtils.CSQLDBUtils = Nothing

        Try
            If (EXECUTE_CALCULATE_FINAL_RESULT_START > -1) Then
                'service timer already spawned a thread to execute calculate final result and 
                'it still hasn't completed, so check to make sure it's not taking too long
                Dim nTimeExecuting As Double = Timer - EXECUTE_CALCULATE_FINAL_RESULT_START

                If nTimeExecuting > EXECUTE_CALCULATE_FINAL_RESULT_MAX_TIMEOUT Then
                    Throw New TimeoutException("EXECUTE CALCULATE FINAL RESULT MAX TIMEOUT: " & nTimeExecuting)
                End If

                Return
            End If

            EXECUTE_CALCULATE_FINAL_RESULT_START = Timer
            Dim oStartedTime As DateTime = Now

            odbSQL = New WebsiteLibrary.DBUtils.CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Dim oGameResultManager As New CGameResultManager(odbSQL)
            oGameResultManager.CalculateFinalResult()

            'END CHECK------------'
            Dim oEndTimeSpan As TimeSpan = Now.Subtract(oStartedTime)
            'LogInfo(log, "Executed Calculate Final Result Milliseconds: " & oEndTimeSpan.TotalMilliseconds & " ms")

        Catch ex As Exception
            LogError(log, "Error at Executed Calculate Final Result: " & ex.Message, ex)
        Finally
            If odbSQL IsNot Nothing Then odbSQL.closeConnection()
        End Try

        'reset timer means we're done with the processing thread and another one can start!
        EXECUTE_CALCULATE_FINAL_RESULT_START = -1
    End Sub

#End Region

#Region "Reset Account Balance"

    Private Sub resetAccountBalance()
        Try
            Dim oDateResetBalanceAmount As Date = Date.Now.ToUniversalTime.AddHours(EXECUTE_RESET_ACCOUNT_BALANCE_TIME_ZONE)
            If oDateResetBalanceAmount.DayOfWeek = DayOfWeek.Monday AndAlso _
            oDateResetBalanceAmount.Hour = EXECUTE_RESET_ACCOUNT_BALANCE_HOUR AndAlso _
            oDateResetBalanceAmount.Minute = EXECUTE_RESET_ACCOUNT_BALANCE_MINUTE Then
                '' This feature should run only once at mid night of Monday
                If RUN_ONCE_RESET_ACCOUNT_BALANCE Then
                    Exit Sub
                End If

                If (EXECUTE_RESET_ACCOUNT_BALANCE_START > -1) Then
                    'service timer already spawned a thread to execute calculate final result and 
                    'it still hasn't completed, so check to make sure it's not taking too long
                    Dim nTimeExecuting As Double = Timer - EXECUTE_RESET_ACCOUNT_BALANCE_START

                    If nTimeExecuting > EXECUTE_RESET_ACCOUNT_BALANCE_MAX_TIMEOUT Then
                        Throw New TimeoutException("EXECUTE RESET ACCOUNT BALANCE MAX TIMEOUT: " & nTimeExecuting)
                    End If

                    Return
                End If

                EXECUTE_RESET_ACCOUNT_BALANCE_START = Timer
                Dim oStartedTime As DateTime = Now

                Dim oCResetAccountBalance As CResetAccountBalance = New CResetAccountBalance()
                oCResetAccountBalance.ResetAccountBalance(oDateResetBalanceAmount)

                If CServiceStd.EXECUTE_RESET_ACCOUNT_BALANCE Then
                    Dim oCasinoManager As New CSyncCasino()
                    oCasinoManager.InitialCasinoAmount()
                End If

                'END CHECK------------'
                Dim oEndTimeSpan As TimeSpan = Now.Subtract(oStartedTime)
                'LogInfo(log, "Executed Reset Account Balance Milliseconds: " & oEndTimeSpan.TotalMilliseconds & " ms")
                RUN_ONCE_RESET_ACCOUNT_BALANCE = True

            Else
                RUN_ONCE_RESET_ACCOUNT_BALANCE = False

            End If
        Catch ex As Exception
            CatchSBCServiceError(log, ex)
        End Try

        'reset timer means we're done with the processing thread and another one can start!
        EXECUTE_RESET_ACCOUNT_BALANCE_START = -1

    End Sub

#End Region

#Region "Weekly charge"

    Private Sub weeklyCharge()
        Try
            Dim oDateWeeklyCharge As Date = Date.Now.ToUniversalTime.AddHours(EXECUTE_WEEKLY_CHARGE_TIME_ZONE)
            If oDateWeeklyCharge.DayOfWeek = DayOfWeek.Monday AndAlso _
            oDateWeeklyCharge.Hour = EXECUTE_WEEKLY_CHARGE_HOUR AndAlso _
            oDateWeeklyCharge.Minute = EXECUTE_WEEKLY_CHARGE_MINUTE Then
                '' This feature should run only once at mid night of Monday
                If RUN_ONCE_WEEKLY_CHARGE Then
                    Exit Sub
                End If

                If (EXECUTE_WEEKLY_CHARGE_START > -1) Then
                    'service timer already spawned a thread to execute calculate final result and 
                    'it still hasn't completed, so check to make sure it's not taking too long
                    Dim nTimeExecuting As Double = Timer - EXECUTE_WEEKLY_CHARGE_START

                    If nTimeExecuting > EXECUTE_WEEKLY_CHARGE_MAX_TIMEOUT Then
                        Throw New TimeoutException("EXECUTE WEEKLY CHARGE MAX TIMEOUT: " & nTimeExecuting)
                    End If

                    Return
                End If

                EXECUTE_WEEKLY_CHARGE_START = Timer
                Dim oStartedTime As DateTime = Now

                Dim oMaintenanceCharge As CMaintenanceCharge = New CMaintenanceCharge()
                oMaintenanceCharge.WeeklyCharge(oDateWeeklyCharge)

                'END CHECK------------'
                Dim oEndTimeSpan As TimeSpan = Now.Subtract(oStartedTime)
                'LogInfo(log, "Executed Weekly Charge Milliseconds: " & oEndTimeSpan.TotalMilliseconds & " ms")
                RUN_ONCE_WEEKLY_CHARGE = True

            Else
                RUN_ONCE_WEEKLY_CHARGE = False

            End If
        Catch ex As Exception
            CatchSBCServiceError(log, ex)
        End Try

        'reset timer means we're done with the processing thread and another one can start!
        EXECUTE_WEEKLY_CHARGE_START = -1

    End Sub

#End Region

#Region "Synchronize Casino Account Balance"

    Private Sub syncCasinoAccount()
        Try
            Dim oDateSyncCasino As Date = Date.Now.ToUniversalTime.AddHours(EXECUTE_CASINO_ACCOUNT_BALANCE_TIME_ZONE)
            If oDateSyncCasino.Hour = EXECUTE_CASINO_ACCOUNT_BALANCE_HOUR AndAlso _
            oDateSyncCasino.Minute = EXECUTE_CASINO_ACCOUNT_BALANCE_MINUTE Then
                '' This feature should run only once at mid night 
                If RUN_ONCE_CASINO_ACCOUNT_BALANCE Then
                    Exit Sub
                End If

                If (EXECUTE_CASINO_ACCOUNT_BALANCE_START > -1) Then
                    'service timer already spawned a thread to execute calculate final result and 
                    'it still hasn't completed, so check to make sure it's not taking too long
                    Dim nTimeExecuting As Double = Timer - EXECUTE_CASINO_ACCOUNT_BALANCE_START

                    If nTimeExecuting > EXECUTE_CASINO_ACCOUNT_BALANCE_MAX_TIMEOUT Then
                        Throw New TimeoutException("EXECUTE CASINO ACCOUNT BALANCE MAX TIMEOUT: " & nTimeExecuting)
                    End If

                    Return
                End If

                EXECUTE_CASINO_ACCOUNT_BALANCE_START = Timer
                Dim oStartedTime As DateTime = Now

                Dim oDate As Date = GetEasternDate()
                Dim oSyncCasino As CSyncCasino = New CSyncCasino()
                oSyncCasino.SyncCasinoAmount(oDate)

                'END CHECK------------'
                Dim oEndTimeSpan As TimeSpan = Now.Subtract(oStartedTime)
                'LogInfo(log, "Executed Casino Account Balance Milliseconds: " & oEndTimeSpan.TotalMilliseconds & " ms")
                RUN_ONCE_CASINO_ACCOUNT_BALANCE = True

            Else
                RUN_ONCE_CASINO_ACCOUNT_BALANCE = False

            End If
        Catch ex As Exception
            CatchSBCServiceError(log, ex)
        End Try

        'reset timer means we're done with the processing thread and another one can start!
        EXECUTE_CASINO_ACCOUNT_BALANCE_START = -1
    End Sub

#End Region

    Private Sub TimerPinacle_Elapsed(ByVal sender As System.Object, ByVal e As System.Timers.ElapsedEventArgs) Handles TimerPinnacle.Elapsed
        If EXECUTE_PINNACLE_PROPS Then
            '' run  PINNACLE Props one time every hour
            Dim oPinancle As New CPinnacleService()
            oPinancle.ExecutePinnacleFeed()
        End If
    End Sub

    Public Sub BeginGetData()
        oSOCrashTimer.Enabled = False

        Dim nCount As Integer = 0
        Dim nSleep As Integer = SPORTOPTIONS_RESUME_TIME * 60000
        While (nCount < SPORTOPTIONS_MAX_RESUME) ' Try to reconnect Server
            Try
                _SOManager = New CSOManager
                oKeepConnection.Enabled = True
                ' This function will exec response XML from server forever
                ' It will stop when some exceptions occur
                _SOManager.BeginConnect(SPORTOPTIONS_URL, SPORTOPTIONS_PORT)

            Catch exUser As SportOptionUserNameException
                CatchSBCServiceError(log, exUser)

            Catch exDisconnect As SportOptionDisconnectException
                CatchSBCServiceError(log, exDisconnect)

            Catch ex As Exception
                CatchSBCServiceError(log, ex)

            Finally
                If _SOManager IsNot Nothing Then _SOManager.EndConnect()
            End Try

            oKeepConnection.Enabled = False
            ' Reconnect server after specific time
            System.Threading.Thread.Sleep(nSleep)

            '' Reset Crash times when the range of crash time greater than 10 minutes
            If SPORTOPTIONS_LAST_CRASH = Date.MinValue Then
                nCount = 0
            Else
                Dim oEndTimeSpan As TimeSpan = Now.Subtract(SPORTOPTIONS_LAST_CRASH)
                If oEndTimeSpan.TotalMinutes > (2 * SPORTOPTIONS_RESUME_TIME) Then
                    nCount = 0
                End If
            End If

            SPORTOPTIONS_LAST_CRASH = Now
            nCount += 1
            ' log.Info(String.Format("Try to reconnect server {0} times", nCount))
        End While

        '' Still not connect to server after specific times
        '' Reconnect Server after SPORTOPTIONS_CRASH_TIME
        oSOCrashTimer.Enabled = True
    End Sub

    Private Sub oSOCrashTimer_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles oSOCrashTimer.Elapsed
        If CServiceStd.EXECUTE_SPORTOPTIONS Then
            ' log.Info("Try to reconnect server after crash")
            InitSportOption()
        End If
    End Sub

    Private Sub oKeepConnection_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles oKeepConnection.Elapsed
        If _SOManager IsNot Nothing Then
            Try
                _SOManager.KeepConnection()

            Catch ex As Exception
                LogError(log, "Fail to keep SportOption connection", ex)
            End Try

        End If
    End Sub
End Class



