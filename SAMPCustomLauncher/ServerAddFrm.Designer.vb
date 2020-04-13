<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ServerAddFrm
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
        Me.NsTheme1 = New SAMPCustomLauncher.NSTheme()
        Me.NsButton2 = New SAMPCustomLauncher.NSButton()
        Me.NsButton1 = New SAMPCustomLauncher.NSButton()
        Me.NsLabel1 = New SAMPCustomLauncher.NSLabel()
        Me.NsTextBox1 = New SAMPCustomLauncher.NSTextBox()
        Me.NsTheme1.SuspendLayout()
        Me.SuspendLayout()
        '
        'NsTheme1
        '
        Me.NsTheme1.AccentOffset = 42
        Me.NsTheme1.BackColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.NsTheme1.BorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.NsTheme1.Colors = New SAMPCustomLauncher.Bloom(-1) {}
        Me.NsTheme1.Controls.Add(Me.NsButton2)
        Me.NsTheme1.Controls.Add(Me.NsButton1)
        Me.NsTheme1.Controls.Add(Me.NsLabel1)
        Me.NsTheme1.Controls.Add(Me.NsTextBox1)
        Me.NsTheme1.Customization = ""
        Me.NsTheme1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.NsTheme1.Font = New System.Drawing.Font("Verdana", 8.0!)
        Me.NsTheme1.Image = Nothing
        Me.NsTheme1.Location = New System.Drawing.Point(0, 0)
        Me.NsTheme1.Movable = True
        Me.NsTheme1.Name = "NsTheme1"
        Me.NsTheme1.NoRounding = False
        Me.NsTheme1.Sizable = True
        Me.NsTheme1.Size = New System.Drawing.Size(266, 137)
        Me.NsTheme1.SmartBounds = True
        Me.NsTheme1.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.NsTheme1.TabIndex = 0
        Me.NsTheme1.Text = "Add Server "
        Me.NsTheme1.TransparencyKey = System.Drawing.Color.Empty
        Me.NsTheme1.Transparent = False
        '
        'NsButton2
        '
        Me.NsButton2.Location = New System.Drawing.Point(136, 100)
        Me.NsButton2.Name = "NsButton2"
        Me.NsButton2.Size = New System.Drawing.Size(79, 25)
        Me.NsButton2.TabIndex = 3
        Me.NsButton2.Text = "   Cancel"
        '
        'NsButton1
        '
        Me.NsButton1.Location = New System.Drawing.Point(39, 100)
        Me.NsButton1.Name = "NsButton1"
        Me.NsButton1.Size = New System.Drawing.Size(79, 25)
        Me.NsButton1.TabIndex = 2
        Me.NsButton1.Text = "      OK"
        '
        'NsLabel1
        '
        Me.NsLabel1.Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Bold)
        Me.NsLabel1.Location = New System.Drawing.Point(12, 38)
        Me.NsLabel1.Name = "NsLabel1"
        Me.NsLabel1.Size = New System.Drawing.Size(242, 21)
        Me.NsLabel1.TabIndex = 1
        Me.NsLabel1.Text = "NsLabel1"
        Me.NsLabel1.Value1 = "Enter New Server"
        Me.NsLabel1.Value2 = "      | Host:Port"
        '
        'NsTextBox1
        '
        Me.NsTextBox1.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.NsTextBox1.Location = New System.Drawing.Point(12, 65)
        Me.NsTextBox1.MaxLength = 32767
        Me.NsTextBox1.Multiline = False
        Me.NsTextBox1.Name = "NsTextBox1"
        Me.NsTextBox1.ReadOnly = False
        Me.NsTextBox1.Size = New System.Drawing.Size(242, 28)
        Me.NsTextBox1.TabIndex = 0
        Me.NsTextBox1.Text = "add"
        Me.NsTextBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
        Me.NsTextBox1.UseSystemPasswordChar = False
        '
        'ServerAddFrm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(266, 137)
        Me.Controls.Add(Me.NsTheme1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ServerAddFrm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "ServerAddFrm"
        Me.NsTheme1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents NsTheme1 As SAMPCustomLauncher.NSTheme
    Friend WithEvents NsButton2 As SAMPCustomLauncher.NSButton
    Friend WithEvents NsButton1 As SAMPCustomLauncher.NSButton
    Friend WithEvents NsLabel1 As SAMPCustomLauncher.NSLabel
    Friend WithEvents NsTextBox1 As SAMPCustomLauncher.NSTextBox
End Class
