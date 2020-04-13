Imports System.IO
Imports SAMPCustomLauncher.ColorManager
Imports System.Security.Principal
Imports System.Net
Imports System.Text

Public Class Utils

#Region " Userdata.dat SAMP manager "

    Public Shared Function GuessedStringEncoding(ByVal bytes As Byte()) As String
        Dim ret As String = Nothing
        Dim stream As New MemoryStream(bytes)

        If bytes IsNot Nothing Then


            Try
                Dim charset_name As String = System.Text.Encoding.[Default].GetString(bytes)

                If charset_name IsNot Nothing Then
                    Dim encoding = System.Text.Encoding.GetEncoding(charset_name)

                    If encoding IsNot Nothing Then
                        ret = encoding.GetString(bytes)
                    End If
                End If

            Catch e As Exception
                Console.[Error].WriteLine(e)
            End Try

            If ret Is Nothing Then

                Try
                    ret = System.Text.Encoding.[Default].GetString(bytes)
                Catch e As Exception
                    Console.[Error].WriteLine(e)
                End Try
            End If
        End If

        If ret Is Nothing Then
            ret = ""
        End If

        Return ret
    End Function

#End Region

#Region " Get Process PID Function "

    ' [ Get Process PID Function ]
    '
    ' // By Elektro H@cker
    '
    ' Examples :
    ' MsgBox(Get_Process_PID("cmd"))
    ' MsgBox(Get_Process_PID("cmd.exe"))

    Public Shared Function Get_Process_PID(ByVal ProcessName As String) As IntPtr
        If ProcessName.ToLower.EndsWith(".exe") Then ProcessName = ProcessName.Substring(0, ProcessName.Length - 4)
        Dim ProcessArray = Process.GetProcessesByName(ProcessName)
        If ProcessArray.Length = 0 Then Return Nothing Else Return ProcessArray(0).Id
    End Function

#End Region

#Region " GUI Color "

    Public Shared Sub LoadColor()
        BackColorEx = My.Settings.ColorMain
        Form1.ThirteenForm1.BackColor = BackColorEx
        Form1.ThirteenTextBox1.BackColor = BackColorEx

        ' ThirteenForm1.Update()
        ' ThirteenForm1.Refresh()
        'ThirteenForm1.Invalidate()

        LineColorEx = My.Settings.LineColor
        Form1.EtherealSeperator1.ForeColor = LineColorEx
        Form1.ThirteenForm1.AccentColor = LineColorEx
        Form1.AscButton_Big1.GlowColor = LineColorEx
        Form1.AnimaStatusBar1.ColorBackground = LineColorEx

        SettingColor = My.Settings.SettingC
        Form1.NsGroupBox1.BackColor = SettingColor
        Form1.SettingPanel.BackColor = SettingColor

        ForeColorEX = My.Settings.ForeC
        Form1.NsGroupBox1.ForeColor = ForeColorEX
        Form1.SettingPanel.ForeColor = ForeColorEX
        Form1.ThirteenForm1.ForeColor = ForeColorEX
        Form1.ThirteenTextBox1.ForeColor = ForeColorEX

    End Sub


#End Region

#Region " Write Log "

    ' [ Write Log Function ]
    '
    ' // By Elektro H@cker
    '
    ' Examples :
    ' WriteLog("Application started", InfoType.Information)
    ' WriteLog("Application got mad", InfoType.Critical)

    Shared LogFile = CurDir() & "\" & System.Reflection.Assembly.GetExecutingAssembly.GetName().Name & ".log"

    Public Enum InfoType
        Information
        Exception
        Critical
        None
    End Enum

    Public Shared Function WriteLog(ByVal Message As String, ByVal InfoType As InfoType) As Boolean
        Dim LocalDate As String = My.Computer.Clock.LocalTime.ToString.Split(" ").First
        Dim LocalTime As String = My.Computer.Clock.LocalTime.ToString.Split(" ").Last
        Dim LogDate As String = "[ " & LocalDate & " ] " & " [ " & LocalTime & " ]  "
        Dim MessageType As String = Nothing

        Select Case InfoType
            Case InfoType.Information : MessageType = "Information: "
            Case InfoType.Exception : MessageType = "Error: "
            Case InfoType.Critical : MessageType = "Critical: "
            Case InfoType.None : MessageType = ""
        End Select

        Try
            My.Computer.FileSystem.WriteAllText(LogFile, vbNewLine & LogDate & MessageType & Message & vbNewLine, True)
            Return True
        Catch ex As Exception
            'Return False
            Throw New Exception(ex.Message)
        End Try

    End Function

#End Region

#Region " Detect Admin "

    Public Shared Function IsRunningAsLocalAdmin() As Boolean
        Dim cur As WindowsIdentity = WindowsIdentity.GetCurrent()
        For Each role As IdentityReference In cur.Groups
            If role.IsValidTargetType(GetType(SecurityIdentifier)) Then
                Dim sid As SecurityIdentifier = DirectCast(role.Translate(GetType(SecurityIdentifier)), SecurityIdentifier)
                If sid.IsWellKnown(WellKnownSidType.AccountAdministratorSid) OrElse sid.IsWellKnown(WellKnownSidType.BuiltinAdministratorsSid) Then
                    Return True
                End If

            End If
        Next
        Return False
    End Function

#End Region


    Public Shared Function GetIPAddress() As String
        
    End Function

End Class

