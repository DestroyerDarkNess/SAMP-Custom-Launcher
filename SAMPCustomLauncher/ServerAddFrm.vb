Public Class ServerAddFrm

    Private Sub ServerAddFrm_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.Location = Form1.PosicionateInControl(Me, Me.Location)
        ' Me.Location = Form1.Button4.Location
    End Sub

    Private Sub NsButton1_Click(sender As Object, e As EventArgs) Handles NsButton1.Click
        Dim PuetoDefecto As String = "7777"
        Try
            Dim IpFormat As String() = NsTextBox1.Text.Split(":")
            Form1.agregado(NsTextBox1.Text, " ", " ", IpFormat(0), IpFormat(1))
            Me.Close()
        Catch ex As Exception
            Form1.agregado(NsTextBox1.Text & ":" & PuetoDefecto, " ", " ", NsTextBox1.Text, PuetoDefecto)
            Me.Close()
        End Try
    End Sub

    Private Sub NsButton2_Click(sender As Object, e As EventArgs) Handles NsButton2.Click
        NsTextBox1.Text = ""
        Me.Close()
    End Sub

End Class