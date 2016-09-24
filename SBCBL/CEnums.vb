Imports System.ComponentModel

Public Enum EUserType
    SuperAdmin
    Agent
    CallCenterAgent
    Player
End Enum

Public Class CEnums
    Public Enum ESiteType
        SBS
        SBC
    End Enum

    Public Enum ETypeOfBet
        Internet
        Phone
    End Enum

    Public Enum ETransactionType
        Deposit
        Withdraw
    End Enum

    Public Enum ETransactionUserType
        SuperAdmin
        SuperAgent
        Agent
    End Enum

    Public Enum ELockType
        Game
        MoneyLine
        TotalPoint
    End Enum

    Public Enum EBetType
        <Description("Spread Juice")>
        Spread = 1

        <Description("Total Juice")>
        Total = 2

        <Description("Spread Juice & Total Juice")>
        SpreadAndTotal = 3

        <Description("Money Line Juice")>
        Money = 4
    End Enum

End Class


