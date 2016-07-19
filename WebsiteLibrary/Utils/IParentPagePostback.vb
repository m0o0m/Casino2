'public interface to indicate to a parent page that wants to be notified when the child window closes
'Dependency: CGlobalPage to determine if the ChildClose should be called
'                    self_close.aspx that will create the JS to notify the parent
'                    childxxx.aspx that will call closeMe(new CParentCallBackData('xxx'))
Public Interface IParentPagePostback
    Sub ChildClose(ByVal sData As String)
End Interface


Public Class CParentCallBackData
    Public Data As String
    Public IsPostBackClient As Boolean = False
    Public Sub New(ByVal sData As String, Optional ByVal postBackClient As Boolean = False)
        Data = sData
        IsPostBackClient = postBackClient
    End Sub
End Class