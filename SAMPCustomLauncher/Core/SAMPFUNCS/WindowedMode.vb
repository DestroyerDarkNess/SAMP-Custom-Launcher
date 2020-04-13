Public Class WindowedMode

    Public Shared Function GetOS() As String
        Try
            Dim OS As String = My.Computer.Info.OSFullName
            Return OS
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Public Shared Function DeterminedOS(ByVal OS As String) As String
        Try
            If InStr(1, OS, "7") > 0 Then
                Return "Win7"
            ElseIf InStr(1, OS, "8") > 0 Then
                Return "Win8"
            ElseIf InStr(1, OS, "10") > 0 Then
                Return "Win10"
            End If
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Public Shared Function GTA_Samp_Windowed() As Integer
        Dim OSResult As String = DeterminedOS(GetOS)
        If OSResult = "Win7" Then
            Return 7
        ElseIf OSResult = "Win8" Then
            Return 8
        ElseIf OSResult = "Win10" Then
            Return 10
        End If
        Return 0
    End Function

End Class
