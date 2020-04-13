Imports System.IO
Imports System.Collections.Generic
Imports System

Namespace SampQueryService.QueryResult
    Public Class PlayerList
        Inherits SampQueryResult

        Public Players As IEnumerable(Of PlayerInfo)

        Public Sub New()
            Me.OpCode = "d"c
        End Sub

        Friend Overrides Sub Deserialize(ByVal data As Byte())
            Dim pList = New List(Of PlayerInfo)()

            Using stream As MemoryStream = New MemoryStream(data)

                Using reader As BinaryReader = New BinaryReader(stream)
                    Dim maxPlayers As Integer = reader.ReadInt16()

                    For i As Integer = 0 To maxPlayers - 1
                        Dim pInfo = New PlayerInfo()
                        pInfo.ID = reader.ReadByte()
                        Dim usernameLength As Integer = reader.ReadByte()
                        pInfo.UserName = New String(reader.ReadChars(usernameLength))
                        pInfo.Level = reader.ReadInt32()
                        pInfo.ping = reader.ReadInt32()
                        pList.Add(pInfo)
                    Next
                End Using
            End Using

            Players = pList
        End Sub
    End Class
End Namespace
