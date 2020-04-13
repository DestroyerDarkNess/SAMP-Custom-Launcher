Public Class frmShadow
    Private m_width As Integer = 0
    Private m_Height As Integer = 0
    Private m_color As System.Drawing.Color

    Public Sub New(ByVal owner As Control)
        InitializeComponent()
        Me.Opacity = 0.5
        Me.FormBorderStyle = Windows.Forms.FormBorderStyle.None
        Me.Owner = owner
    End Sub

    Public Property shWidth() As Integer
        Get
            Return m_width
        End Get
        Set(ByVal value As Integer)
            m_width = value + 5
            Me.Width = m_width
        End Set
    End Property

    Public Property shHeight() As Integer
        Get
            Return m_Height
        End Get
        Set(ByVal value As Integer)
            m_Height = value + 5
            Me.Height = m_Height
        End Set
    End Property

    Public Property shColor() As System.Drawing.Color
        Get
            Return m_color
        End Get
        Set(ByVal value As System.Drawing.Color)
            m_color = value
            Me.BackColor = value
        End Set
    End Property

    Public Sub CreateShadow()
        Dim reg As New Region(New Rectangle(0, 0, m_width, m_Height))
        Dim oPath As New System.Drawing.Drawing2D.GraphicsPath
        Dim olPath As New System.Drawing.Drawing2D.GraphicsPath
        Dim obPath As New System.Drawing.Drawing2D.GraphicsPath
        Dim otPath As New System.Drawing.Drawing2D.GraphicsPath
        Dim shpath() As Point = {New Point(m_width - 5, 5), New Point(m_width, 10), New Point(m_width, m_Height - 5), New Point(m_width - 5, m_Height), New Point(10, m_Height), New Point(5, m_Height - 5), New Point(m_width - 5, m_Height - 5)}

        oPath.AddLines(shpath)
        reg.Intersect(oPath)

        olPath.AddArc(3, Me.Height - 7, 8, 7, 45, 180)
        reg.Union(olPath)

        obPath.AddArc(m_width - 7, m_Height - 7, 7, 7, 315, 180)
        reg.Union(obPath)

        otPath.AddArc(m_width - 7, 3, 7, 8, 225, 180)
        reg.Union(otPath)

        Me.Region = reg
        Me.Show()
    End Sub



End Class