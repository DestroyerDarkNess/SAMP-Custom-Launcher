Imports System
Imports System.IO

Namespace SampQueryService.QueryResult
    Public Class ServerInfo
        Inherits SampQueryResult

        Public Property Password As Boolean
        Public Property Players As Integer
        Public Property MaxPlayers As Integer
        Public Property HostName As String
        Public Property GameModeName As String
        Public Property Language As String

        Public Sub New()
            Me.OpCode = "i"c
        End Sub

        Friend Overrides Sub Deserialize(ByVal data As Byte())
            Using stream As MemoryStream = New MemoryStream(data)

                Using reader As BinaryReader = New BinaryReader(stream)
                    Dim length As Integer
                    Password = Convert.ToBoolean(reader.ReadByte())
                    Players = reader.ReadInt16()
                    MaxPlayers = reader.ReadInt16()
                    length = reader.ReadInt32()
                    HostName = New String(reader.ReadChars(length))
                    length = reader.ReadInt32()
                    GameModeName = New String(reader.ReadChars(length))
                    length = reader.ReadInt32()
                    Language = New String(reader.ReadChars(length))
                End Using
            End Using
        End Sub
    End Class
End Namespace
