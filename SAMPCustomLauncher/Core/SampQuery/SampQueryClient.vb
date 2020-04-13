Imports System
Imports System.Net
Imports System.Threading.Tasks
Imports System.Collections.Generic
Imports System.Linq


Namespace SampQueryService
    Public Class SampQueryClient
        Public Function SendQueryAsync(Of T As {SampQueryService.QueryResult.SampQueryResult, New})(ByVal ip As String, ByVal port As Integer) As Task(Of T)
            Dim cleanIP As IPAddress
            Dim isValidIP = IPAddress.TryParse(ip, cleanIP)
            If Not isValidIP Then Throw New FormatException("String ip is not in a valid format.")
            Return SendQueryAsync(Of T)(cleanIP, port)
        End Function

        Public Function SendQueryAsync(Of T As {SampQueryService.QueryResult.SampQueryResult, New})(ByVal ip As IPAddress, ByVal port As Integer) As Task(Of T)
            Return SendQueryAsync(Of T)(New IPEndPoint(ip, port))
        End Function

        Public Async Function SendQueryAsync(Of T As {SampQueryService.QueryResult.SampQueryResult, New})(ByVal ipEnd As IPEndPoint) As Task(Of T)
            Dim query = New SampQuery(ipEnd)
            Dim obj = New T()
            Dim receivedPacketsTask = query.ReceiveAsync()
            Dim sendQueryTask = query.SendAsync(obj.OpCode)
            Await Task.WhenAll(receivedPacketsTask, sendQueryTask)
            Dim rPackets = receivedPacketsTask.Result

            If rPackets IsNot Nothing Then
                obj.IsCompleted = True
                obj.Deserialize(rPackets)
            End If

            Return obj
        End Function

        Public Async Function SendQueryAsync(Of T As {SampQueryService.QueryResult.SampQueryResult, New})(ByVal ipEnd As IEnumerable(Of IPEndPoint)) As Task(Of IEnumerable(Of T))
            Dim activeTaskList = New List(Of Task(Of T))()

            For Each adress In ipEnd
                Dim duplicatePorts = ipEnd.Where(Function(i) i.Port = adress.Port)
                Dim newQueryTask = SendQueryAsync(Of T)(adress)
                If duplicatePorts.Count() > 1 Then Await newQueryTask
                activeTaskList.Add(newQueryTask)
            Next

            Await Task.WhenAll(activeTaskList)
            Return activeTaskList.[Select](Function(p) p.Result)
        End Function

        Public Function SendRconQueryAsync(ByVal ip As IPAddress, ByVal port As Integer, ByVal password As String, ByVal command As String) As Task
            Return SendRconQueryAsync(New IPEndPoint(ip, port), password, command)
        End Function

        Public Async Function SendRconQueryAsync(ByVal ipEnd As IPEndPoint, ByVal password As String, ByVal command As String) As Task
            Dim query = New SampQuery(ipEnd)
            Dim receivedPacketsTask = query.ReceiveAsync()
            Await query.SendRconAsync(password, command)
        End Function
    End Class
End Namespace
