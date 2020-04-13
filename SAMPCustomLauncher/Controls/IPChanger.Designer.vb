<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class IPChanger
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
        Me.Group1 = New SAMPCustomLauncher.Group()
        Me.Text1 = New SAMPCustomLauncher.Text()
        Me.NsLabel1 = New SAMPCustomLauncher.NSLabel()
        Me.Bouton1 = New SAMPCustomLauncher.Bouton()
        Me.ComboBox1 = New System.Windows.Forms.ComboBox()
        Me.Group1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Group1
        '
        Me.Group1.BorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Group1.Controls.Add(Me.ComboBox1)
        Me.Group1.Controls.Add(Me.Text1)
        Me.Group1.Controls.Add(Me.NsLabel1)
        Me.Group1.Controls.Add(Me.Bouton1)
        Me.Group1.Customization = "tZ01/xwcHP//////"
        Me.Group1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Group1.Font = New System.Drawing.Font("Verdana", 8.0!)
        Me.Group1.Image = Nothing
        Me.Group1.Location = New System.Drawing.Point(0, 0)
        Me.Group1.Movable = True
        Me.Group1.Name = "Group1"
        Me.Group1.NoRounding = False
        Me.Group1.Sizable = True
        Me.Group1.Size = New System.Drawing.Size(265, 390)
        Me.Group1.SmartBounds = True
        Me.Group1.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Group1.TabIndex = 0
        Me.Group1.Text = "IP Changer"
        Me.Group1.TransparencyKey = System.Drawing.Color.Empty
        Me.Group1.Transparent = False
        '
        'Text1
        '
        Me.Text1.Customization = "AAAA//////+1nTX/"
        Me.Text1.Font = New System.Drawing.Font("Verdana", 8.0!)
        Me.Text1.Image = Nothing
        Me.Text1.Location = New System.Drawing.Point(89, 44)
        Me.Text1.MaxLength = 32767
        Me.Text1.Multiline = False
        Me.Text1.Name = "Text1"
        Me.Text1.NoRounding = False
        Me.Text1.ReadOnly = False
        Me.Text1.Size = New System.Drawing.Size(164, 24)
        Me.Text1.TabIndex = 1
        Me.Text1.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
        Me.Text1.Transparent = False
        Me.Text1.UseSystemPasswordChar = False
        '
        'NsLabel1
        '
        Me.NsLabel1.Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Bold)
        Me.NsLabel1.Location = New System.Drawing.Point(3, 42)
        Me.NsLabel1.Name = "NsLabel1"
        Me.NsLabel1.Size = New System.Drawing.Size(88, 26)
        Me.NsLabel1.TabIndex = 2
        Me.NsLabel1.Text = "NsLabel1"
        Me.NsLabel1.Value1 = "You Real"
        Me.NsLabel1.Value2 = "IP"
        '
        'Bouton1
        '
        Me.Bouton1.Customization = "tZ01//////8="
        Me.Bouton1.Font = New System.Drawing.Font("Segoe UI", 12.0!)
        Me.Bouton1.Image = Nothing
        Me.Bouton1.Location = New System.Drawing.Point(153, 84)
        Me.Bouton1.Name = "Bouton1"
        Me.Bouton1.NoRounding = False
        Me.Bouton1.Size = New System.Drawing.Size(100, 34)
        Me.Bouton1.TabIndex = 0
        Me.Bouton1.Text = "Change IP"
        Me.Bouton1.Transparent = False
        '
        'ComboBox1
        '
        Me.ComboBox1.FormattingEnabled = True
        Me.ComboBox1.Location = New System.Drawing.Point(13, 89)
        Me.ComboBox1.Name = "ComboBox1"
        Me.ComboBox1.Size = New System.Drawing.Size(125, 21)
        Me.ComboBox1.TabIndex = 3
        '
        'IPChanger
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(28, Byte), Integer), CType(CType(28, Byte), Integer), CType(CType(28, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(265, 390)
        Me.Controls.Add(Me.Group1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "IPChanger"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "IPChanger"
        Me.TopMost = True
        Me.Group1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Group1 As SAMPCustomLauncher.Group
    Friend WithEvents Text1 As SAMPCustomLauncher.Text
    Friend WithEvents NsLabel1 As SAMPCustomLauncher.NSLabel
    Friend WithEvents Bouton1 As SAMPCustomLauncher.Bouton
    Friend WithEvents ComboBox1 As System.Windows.Forms.ComboBox
End Class
