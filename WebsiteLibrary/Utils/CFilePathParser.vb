Public Class CFilePathParser
	'All dir will be of format x:\xxx\xxx\  'with ending "\"

	'sample usage:
	'Dim objFP As New CFilePathParser
	'objFP.FilePath = "c:\config.sys"
	'objFP.Extension = "d"
	'objFP.FileName = ""
	'objFP.Path = "C:\progra~1\"

	'local variable(s) to hold property value(s)
	Sub New()

	End Sub
	Sub New(ByVal FullFilePath As String)
		mvarFilePath = FullFilePath
	End Sub
	Private mvarFilePath As String	'local copy
	Public Property FrontFilename() As String
		Get
			If isFile() Then
				FrontFilename = Left(FileName, Len(FileName) - Len(Extension))
				If Right(FrontFilename, 1) = "." Then
                    Return Left(FrontFilename, Len(FrontFilename) - 1)
				End If
			End If
            Return ""
		End Get
		Set(ByVal Value As String)
			mvarFilePath = Path & Value & "." & Extension
		End Set
	End Property

	Public Property FilePath() As String
		Get
			Return mvarFilePath
		End Get
		Set(ByVal Value As String)
			mvarFilePath = Value
		End Set
	End Property

	Public Property Extension() As String
		Get
			If isFile() Then
				If InStr(FileName, ".") <> 0 Then
					Return Mid(FileName, InStr(FileName, ".") + 1)
                End If
			End If

            Return ""
		End Get
		Set(ByVal Value As String)
			If isFile() Then
				FilePath = Left(FilePath, Len(FilePath) - Len(Extension)) & Value
			End If
		End Set
	End Property

	Public Property FileName() As String
		Get
			If isFile() Then
				Return Right(FilePath, Len(FilePath) - InStrRev(FilePath, "\"))
            End If

            Return ""
		End Get
		Set(ByVal Value As String)
			FilePath = Path & Value
		End Set
	End Property

	Public Property Path() As String
		Get
			If isDir() Then
				Path = FilePath
			Else
				Path = Mid(FilePath, 1, InStrRev(FilePath, "\"))
			End If

		End Get
		Set(ByVal Value As String)
			FilePath = formatDirectory(Value) & FileName
		End Set
	End Property
	Public ReadOnly Property isDir() As Boolean
		Get
			'used when retrieving value of a property, on the right side of an assignment.
			'Syntax: Debug.Print X.isDir
			Return CBool(IIf(Right(FilePath, 1) = "\", True, False))
		End Get
	End Property


	Public ReadOnly Property isFile() As Boolean
		Get
			Return CBool(IIf(Right(FilePath, 1) = "\", False, True))
		End Get

	End Property


	Private Function formatDirectory(ByVal sDir As String) As String
		'makes sure sDir is a directory
		Dim sBuff As String
		sBuff = formatFilePath(sDir)
		If Right(sBuff, 1) <> "\" Then sBuff = sBuff & "\"
		formatDirectory = sBuff
	End Function

	Public Function cleanPath() As String
		'takes a path like C:\files\..\files and changes it to C:\files\
		'"http://www.files.com/file/../../reame.txt" '-> http:\\reame.txt
		'"http://www.files.com/file/../reame.txt" '-> http:\\www.files.com\reame.txt
		'"c:\..\..\..\" '-> "" empty
		'"\..\..\..\" '-> ""
		'"c:\..\files\..\test\"     'test\
		'"c:\temp"                  'c:\temp
		'"c:\temp\..\"              'c:\
		'"c:\temp\..\..\..\..\files" files 'files is a file-- not directory

		Dim arrDirs() As String, i As Integer, j As Integer
		arrDirs = Split(Me.Path, "\")
		For i = 0 To UBound(arrDirs) - 1		  'because the last one is always empty due to the way split(..) works
			If arrDirs(i) = ".." And i > 0 Then
				'remove prev dir recursively
				arrDirs(i) = vbNullChar
				For j = i - 1 To 0 Step -1
					If arrDirs(j) <> vbNullChar Then
						arrDirs(j) = vbNullChar
						Exit For						 'j
					End If
				Next
			ElseIf arrDirs(i) = ".." And i = 0 Then
				arrDirs(i) = vbNullChar
			End If
		Next

		cleanPath = ""
		'recombine the stirng
		For i = 0 To UBound(arrDirs) - 1
			If arrDirs(i) <> vbNullChar Then cleanPath = cleanPath & arrDirs(i) & "\"
		Next
		cleanPath = cleanPath & Me.FileName
	End Function


	Private Function formatFilePath(ByVal sFilePath As String) As String
		formatFilePath = Replace(sFilePath, "/", "\")
    End Function

    'deleted. no longer being in use.
    'Public Function isValidFilename() As Boolean
    '	'becuase there are many diff. flavors of filenames, this function is NOT called from the other functions in the class
    '	'it can be used to verify filename validity on particular platforms independent of the other functions (although FilePath must not be empty)
    '	Dim arrFatSpecialChars() As String = {"$", "%", "?, "?, "_", "@", "{", "}", "~", "`", "!", "#", "(", ")", "."}
    '	Dim arrNTFSSpecialChars() As String = {",", "+", "=", "[", "]", ";"}		  'plus the normal FAT
    '	Dim sFilename As String
    '	Dim i As Integer
    '	isValidFilename = True
    '	sFilename = FileName
    '	'Debug.Print("INCOMPLETE!! NEeds to account for . in filename Only support for FAT currently supported!!")
    '	If isFile() Then
    '		If isAlphaNumeric(sFilename) = True Then
    '			For i = 1 To Len(sFilename)
    '				If Inside(arrFatSpecialChars, charAt(sFilename, i), True) = False Then
    '					isValidFilename = False
    '					Exit Function
    '				End If
    '			Next i
    '		Else
    '			isValidFilename = False
    '			Exit Function
    '		End If
    '	Else
    '		isValidFilename = False
    '	End If
    'End Function

	Private Function isAlphaNumeric(ByVal sSource As String) As Boolean
		Dim i As Integer, iASCValue As Integer
		isAlphaNumeric = True
		For i = 1 To Len(sSource)
			iASCValue = Asc(charAt(sSource, i))
			If (iASCValue >= 48 And iASCValue <= 57) Or (iASCValue >= 65 And iASCValue <= 90) Or (iASCValue >= 97 And iASCValue <= 122) Then
				'do nothing cause it's already true
			Else
				isAlphaNumeric = False
				Exit Function
			End If
		Next
	End Function


	Private Function charAt(ByVal Source As String, ByVal indx As Integer) As String
		'********************************************************************************
		'* Name: chatAt
		'*
		'* Description: Returns the character at index Indx
		'* Parameters: Indx = 1 = start
		'* Sample Usage:
		'* Created: 5/28/00 2:26:13 PM
		'********************************************************************************

		If indx < 1 Or indx > Len(Source) Then Err.Raise(911, "charAt", "Requested character is out of bound.")

		charAt = Mid(Source, indx, 1)

	End Function


	Private Function Inside(ByVal arr As String(), ByVal CheckVal As String, Optional ByVal MatchCase As Boolean = False) As Boolean
		Dim i As Integer
		If MatchCase = True Then
			For i = LBound(arr) To UBound(arr)
				If arr(i) = CheckVal Then
					Inside = True
					Exit Function
				End If
			Next
		Else
			For i = LBound(arr) To UBound(arr)
				If UCase(arr(i)) = UCase(CheckVal) Then
					Inside = True
					Exit Function
				End If
			Next
		End If
		Inside = False
	End Function

End Class