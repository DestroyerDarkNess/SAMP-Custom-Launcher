<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DomainInfoForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.ThirteenForm1 = New SAMPCustomLauncher.ThirteenForm()
        Me.NsLabel1 = New SAMPCustomLauncher.NSLabel()
        Me.EtherealButton1 = New SAMPCustomLauncher.EtherealButton()
        Me.ThirteenForm1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ThirteenForm1
        '
        Me.ThirteenForm1.AccentColor = System.Drawing.Color.DodgerBlue
        Me.ThirteenForm1.BackColor = System.Drawing.Color.White
        Me.ThirteenForm1.ColorLine = System.Drawing.Color.Empty
        Me.ThirteenForm1.ColorScheme = SAMPCustomLauncher.ThirteenForm.ColorSchemes.Dark
        Me.ThirteenForm1.Controls.Add(Me.EtherealButton1)
        Me.ThirteenForm1.Controls.Add(Me.NsLabel1)
        Me.ThirteenForm1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ThirteenForm1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!)
        Me.ThirteenForm1.Location = New System.Drawing.Point(0, 0)
        Me.ThirteenForm1.Name = "ThirteenForm1"
        Me.ThirteenForm1.Size = New System.Drawing.Size(505, 353)
        Me.ThirteenForm1.TabIndex = 0
        Me.ThirteenForm1.Text = "Host Information"
        '
        'NsLabel1
        '
        Me.NsLabel1.Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Bold)
        Me.NsLabel1.Location = New System.Drawing.Point(3, 39)
        Me.NsLabel1.Name = "NsLabel1"
        Me.NsLabel1.Size = New System.Drawing.Size(107, 51)
        Me.NsLabel1.TabIndex = 0
        Me.NsLabel1.Text = "NsLabel1"
        Me.NsLabel1.Value1 = "NET"
        Me.NsLabel1.Value2 = "SEAL"
        '
        'EtherealButton1
        '
        Me.EtherealButton1.BackColor = System.Drawing.Color.Transparent
        Me.EtherealButton1.ButtonStyle = SAMPCustomLauncher.EtherealButton.Style.DarkClear
        Me.EtherealButton1.Location = New System.Drawing.Point(464, 3)
        Me.EtherealButton1.Name = "EtherealButton1"
        Me.EtherealButton1.RoundRadius = 5
        Me.EtherealButton1.Size = New System.Drawing.Size(38, 20)
        Me.EtherealButton1.TabIndex = 1
        Me.EtherealButton1.Text = "X"
        '
        'DomainInfoForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(505, 353)
        Me.Controls.Add(Me.ThirteenForm1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "DomainInfoForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.Text = "DomainInfoForm"
        Me.TransparencyKey = System.Drawing.Color.Fuchsia
        Me.ThirteenForm1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ThirteenForm1 As SAMPCustomLauncher.ThirteenForm
    Friend WithEvents EtherealButton1 As SAMPCustomLauncher.EtherealButton
    Friend WithEvents NsLabel1 As SAMPCustomLauncher.NSLabel
End Class
