Imports System.Collections.Generic
Imports System.IO

Namespace SampQueryService.QueryResult
    Public Class ServerRules
        Inherits SampQueryResult

        Public Property MapName As String
        Public Property Weather As Integer
        Public Property WebUrl As String
        Public Property WorldTime As ServerTime

        Public Sub New()
            Me.OpCode = "r"c
        End Sub

        Friend Overrides Sub Deserialize(ByVal data As Byte())
            Dim resultList = New List(Of Rule)()

            Using stream As MemoryStream = New MemoryStream(data)

                Using reader As BinaryReader = New BinaryReader(stream)
                    Dim ruleCount As Integer = reader.ReadInt16()

                    For i As Integer = 0 To ruleCount - 1
                        Dim rule = New Rule()
                        Dim ruleLength As Integer = reader.ReadByte()
                        rule.Name = New String(reader.ReadChars(ruleLength))
                        ruleLength = reader.ReadByte()
                        rule.Value = New String(reader.ReadChars(ruleLength))
                        resultList.Add(rule)
                    Next
                End Using
            End Using

            AssignRulesToProperties(resultList)
        End Sub

        Private Sub AssignRulesToProperties(ByVal ruleList As IEnumerable(Of Rule))
            For Each rule In ruleList
                Select Case rule.Name
                    Case "weather"
                        Weather = Integer.Parse(rule.Value)
                    Case "mapname"
                        MapName = rule.Value
                    Case "weburl"
                        WebUrl = rule.Value
                    Case "worldtime"
                        Dim timeString = rule.Value.Split(":"c)
                        Dim time = New ServerTime() With {
                            .Hour = Integer.Parse(timeString(0)),
                            .Minute = Integer.Parse(timeString(1))
                        }
                        WorldTime = time
                    Case Else
                End Select
            Next
        End Sub
    End Class
End Namespace
