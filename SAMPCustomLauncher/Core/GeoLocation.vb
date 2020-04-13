#Region " GeoLocation "

' [ GeoLocation ]
'
' // By Elektro H@cker
'
' Examples :
'
' Dim GeoInfo As GeoLocation.GeoInfo = GeoLocation.Locate("84.126.113.11")
' Dim GeoInfo As GeoLocation.GeoInfo = GeoLocation.Locate("84.126.113.11.dyn.user.ono.com")
' MsgBox(GeoInfo.Country) ' result: Spain
' MsgBox(GeoInfo.City)    ' Result: Valencia

Public Class GeoLocation

    Public Class GeoInfo
        Public Property Latitude() As String
        Public Property Lognitude() As String
        Public Property City() As String
        Public Property State() As String
        Public Property Country() As String
        Public Property Host() As String
        Public Property Ip() As String
        Public Property Code() As String
    End Class

    Public Shared Function Locate(ByVal IP As String) As GeoInfo

        Try

            Dim request = TryCast(Net.WebRequest.Create(New Uri("http://www.geoiptool.com/data.php/en/?IP=" & IP)), Net.HttpWebRequest)

            If request IsNot Nothing Then

                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0; SLCC1; .NET CLR 2.0.50727)"

                Dim _geoloc As New GeoInfo

                Using webResponse = TryCast(request.GetResponse(), Net.HttpWebResponse)
                    If webResponse IsNot Nothing Then

                        Using reader = New IO.StreamReader(webResponse.GetResponseStream())

                            Dim doc = New Xml.XmlDocument()

                            doc.Load(reader)

                            Dim nodes = doc.GetElementsByTagName("marker")

                            Dim marker = TryCast(nodes(0), Xml.XmlElement)

                            _geoloc.City = marker.GetAttribute("city")
                            _geoloc.Country = marker.GetAttribute("country")
                            _geoloc.Code = marker.GetAttribute("code")
                            _geoloc.Host = marker.GetAttribute("host")
                            _geoloc.Ip = marker.GetAttribute("ip")
                            _geoloc.Latitude = marker.GetAttribute("lat")
                            _geoloc.Lognitude = marker.GetAttribute("lng")

                            Return _geoloc

                        End Using

                    End If
                End Using
            End If

            Return New GeoInfo()

        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try

    End Function

End Class

#End Region