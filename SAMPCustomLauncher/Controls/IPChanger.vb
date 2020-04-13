'Imports
Imports System.Management
Imports System.Net
Imports System.Net.NetworkInformation
Imports System.Object
Imports System.IO

Imports SAMPCustomLauncher.Utils

Public Class IPChanger

    'declared in class

    Dim IPAddress As String
    Dim SubnetMask As String
    Dim Gateway As String
    Dim dns1 As String
    Dim dns2 As String

    Private Sub IPChanger_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Text1.Text = GetIPAddress()
    End Sub

    Private Sub Bouton1_Click(sender As Object, e As EventArgs) Handles Bouton1.Click
        MsgBox(GetIPAddress())
    End Sub

    Public Sub a()
        IPAddress = "192.168.0.1" '"192.168.100.100" 'this is the IP string
        SubnetMask = "192.0.0.1" 'subnet string
        Gateway = "192.168.1.1" 'gateway
        dns1 = "8.8.8.8" 'dns1
        dns2 = "8.8.1.1" 'dns2


        Dim objMC As ManagementClass = New ManagementClass("Win32_NetworkAdapterConfiguration")

        Dim objMOC As ManagementObjectCollection = objMC.GetInstances()



        For Each objMO As ManagementObject In objMOC

            If (Not CBool(objMO("IPEnabled"))) Then

                Continue For

            End If



            Try

                Dim objNewIP As ManagementBaseObject = Nothing

                Dim objSetIP As ManagementBaseObject = Nothing

                Dim objNewGate As ManagementBaseObject = Nothing



                objNewIP = objMO.GetMethodParameters("EnableStatic")

                objNewGate = objMO.GetMethodParameters("SetGateways")



                'Set DefaultGateway

                objNewGate("DefaultIPGateway") = New String() {Gateway}

                objNewGate("GatewayCostMetric") = New Integer() {1}



                'Set IPAddress and Subnet Mask

                objNewIP("IPAddress") = New String() {IPAddress}

                objNewIP("SubnetMask") = New String() {SubnetMask}



                objSetIP = objMO.InvokeMethod("EnableStatic", objNewIP, Nothing)

                objSetIP = objMO.InvokeMethod("SetGateways", objNewGate, Nothing)



                Console.WriteLine("Updated IPAddress, SubnetMask and Default Gateway!")



            Catch ex As Exception

                MessageBox.Show("Unable to Set IP : " & ex.Message)

            End Try

        Next objMO

    End Sub

    Private Sub b()
        'this is where you will need to change your dns1 and dns2 - if you want to 
        Try
            Dim c1 As String = ComboBox1.Text
            Try
                Process.Start("cmd", "ipconfig /renew")
                Process.Start("cmd", "ipconfig /release")
                Process.Start("cmd", "/c Netsh Interface ip set dns """ & "127.0.0.1" & """ static " & dns1)
                Process.Start("cmd", "/c Netsh Interface ip add dns """ & "127.0.0.1" & """ index=2 " & dns2)
            Catch ex As Exception

            End Try
        Catch ex As Exception

        End Try
    End Sub

 
End Class