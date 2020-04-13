Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading.Tasks

Namespace SampQueryService
    Friend Class SampQuery
        Private _client As UdpClient
        Private _ipEndP As IPEndPoint

        Public Sub New(ByVal endPoint As IPEndPoint)
            _client = New UdpClient(endPoint.Port)
            _ipEndP = endPoint
        End Sub

        Public Async Function SendAsync(ByVal opCode As Char) As Task(Of Boolean)
            Dim datagram As Byte()

            Using stream As MemoryStream = New MemoryStream()

                Using writer As BinaryWriter = New BinaryWriter(stream)
                    Dim ip = _ipEndP.Address.GetAddressBytes()
                    writer.Write("SAMP".ToCharArray())

                    For Each pack In ip
                        writer.Write(pack)
                    Next

                    writer.Write(CUShort(_ipEndP.Port))
                    writer.Write(opCode)
                End Using

                datagram = stream.ToArray()
            End Using

            Dim result = Await _client.SendAsync(datagram, datagram.Length, _ipEndP)
            If result <> 11 Then Return False
            Return True
        End Function

        Public Async Function SendRconAsync(ByVal password As String, ByVal command As String) As Task
            Dim datagram As Byte()

            Using stream As MemoryStream = New MemoryStream()

                Using writer As BinaryWriter = New BinaryWriter(stream)
                    Dim ip = _ipEndP.Address.GetAddressBytes()
                    writer.Write("SAMP".ToCharArray())

                    For Each pack In ip
                        writer.Write(pack)
                    Next

                    writer.Write(CUShort(_ipEndP.Port))
                    writer.Write("x"c)
                    writer.Write(CUShort(password.Length))
                    writer.Write(password.ToCharArray())
                    writer.Write(CUShort(command.Length))
                    writer.Write(command.ToCharArray())
                End Using

                datagram = stream.ToArray()
            End Using

            Dim result = Await _client.SendAsync(datagram, datagram.Length, _ipEndP)
        End Function

        Public Async Function ReceiveAsync() As Task(Of Byte())
            Dim receiveTask = _client.ReceiveAsync()
            Dim timeoutTask = TimeOutAsync()
            Await Task.WhenAny(receiveTask, timeoutTask)
            _client.Client.Dispose()
            If Not receiveTask.IsCompleted Then Return Nothing
            Dim packets = receiveTask.Result
            Dim cleanPackets As Byte()

            If packets.Buffer.Length > 11 Then
                cleanPackets = New Byte(packets.Buffer.Length - 11 - 1) {}
                Dim cleanPacketsCount As Integer = 0

                For i As Integer = 11 To packets.Buffer.Length - 1
                    cleanPackets(cleanPacketsCount) = packets.Buffer(i)
                    cleanPacketsCount += 1
                Next
            Else
                cleanPackets = Nothing
            End If

            Return cleanPackets
        End Function

        Private Function TimeOutAsync() As Task
            Return Task.Delay(1500)
        End Function
    End Class
End Namespace
