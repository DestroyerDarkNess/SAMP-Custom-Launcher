Public Class ServerPropierties

    Private Sub EtherealButton1_Click(sender As Object, e As EventArgs) Handles EtherealButton1.Click
        Me.Close()
    End Sub

    Private Sub ServerPropierties_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.Location = Form1.CenterForm(Me, Me.Location)
    End Sub

    Private Sub AscButton_Big1_Click(sender As Object, e As EventArgs) Handles AscButton_Big1.Click
        Dim IpFormat As String() = Modelvl.Text.Split(":")
        Form1.Conect(Form1.ThirteenTextBox1.Text, IpFormat(0), IpFormat(1))
        Me.Close()
    End Sub
End Class