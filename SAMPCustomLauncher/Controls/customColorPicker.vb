Imports System.ComponentModel
Imports System.Drawing.Drawing2D
Imports System.Runtime.InteropServices
Imports System.IO

#Region "Depends"
#Region "Themebase"
MustInherit Class customControl
    Inherits Control

#Region "DECLARE"
    Protected G As Graphics, B As Bitmap
    Private DoneCreation As Boolean
    Private InPosition As Boolean
    Protected State As MouseState
    Protected tp As Pen, tb As Brush


#End Region

#Region " Initialization "
    Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw Or ControlStyles.UserPaint Or ControlStyles.SupportsTransparentBackColor, True)

        Font = New Font("Segoe UI", 9S)

    End Sub

    Protected NotOverridable Overrides Sub OnParentChanged(ByVal e As EventArgs)
        If Parent IsNot Nothing Then
            OnCreation()

            DoneCreation = True
        End If
        MyBase.OnParentChanged(e)
    End Sub
#End Region

#Region "Paint"
    Protected NotOverridable Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        If Width = 0 OrElse Height = 0 Then Return
        tp = New Pen(Color.Transparent, 1)
        tb = New SolidBrush(Color.Transparent)

        G = e.Graphics
        PaintHook()
        tp.Dispose() : tb.Dispose()
    End Sub
    Friend Function retbit() As Bitmap
        If Width = 0 OrElse Height = 0 Then Return New Bitmap(0, 0)
        PaintHook()
        Return B
    End Function
#End Region

#Region " Size Handling "
    Protected NotOverridable Overrides Sub OnSizeChanged(ByVal e As EventArgs)


        Invalidate()
        MyBase.OnSizeChanged(e)
    End Sub
    Protected Overrides Sub SetBoundsCore(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByVal specified As BoundsSpecified)
        MyBase.SetBoundsCore(x, y, width, height, specified)
    End Sub
#End Region

#Region " State Handling "
    Dim isdown% = 0
    Protected Overrides Sub OnMouseEnter(ByVal e As EventArgs)
        InPosition = True
        SetState(MouseState.Over)
        For Each c As Control In Parent.Controls
            If TypeOf c Is customControl Then
                DirectCast(c, customControl).leavemouse(e)
                c.Invalidate()
            End If
        Next
        MyBase.OnMouseEnter(e)
    End Sub
    Public Sub leavemouse(e As EventArgs)
        OnMouseLeave(e)
    End Sub
    Protected Overrides Sub OnMouseUp(ByVal e As MouseEventArgs)
        MyBase.OnMouseUp(e)
        If InPosition Then SetState(MouseState.Over)
        isdown = 0
    End Sub
    Protected Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)
        MyBase.OnMouseDown(e)
        If e.Button = MouseButtons.Left Then SetState(MouseState.Down)
        isdown = 1
    End Sub
    Protected Overrides Sub OnMouseLeave(ByVal e As EventArgs)
        InPosition = False
        SetState(MouseState.None)
        MyBase.OnMouseLeave(e)
    End Sub
    Protected Overrides Sub OnEnabledChanged(ByVal e As EventArgs)
        If Enabled Then SetState(MouseState.None) Else SetState(MouseState.Block)
        MyBase.OnEnabledChanged(e)
    End Sub
    Private Sub SetState(ByVal current As MouseState)
        State = current
        Invalidate()
    End Sub
    Protected Overrides Sub OnMouseMove(e As MouseEventArgs)
        MyBase.OnMouseMove(e)
        If isdown Then mousedownmove(e)
    End Sub
    Protected Overridable Sub mousedownmove(e As MouseEventArgs)
    End Sub
#End Region

#Region " Property Helpers "
    Protected Overridable Sub InvalidateBitmap()
        If Width = 0 OrElse Height = 0 Then Return
        B = New Bitmap(Width, Height, Imaging.PixelFormat.Format32bppPArgb)
        G = Graphics.FromImage(B)
    End Sub

#End Region

#Region " User Hooks "

    Protected MustOverride Sub PaintHook()

    Protected Overridable Sub OnCreation()
    End Sub
#End Region


End Class
#End Region

Public Module publicvars

    Public Enum MouseState As Byte
        None = 0
        Over = 1
        Down = 2
        Block = 3
    End Enum
    <DllImport("user32.dll", SetLastError:=True)> Public Function SetWindowPos(ByVal hWnd As IntPtr, ByVal hWndInsertAfter As IntPtr, ByVal X As Integer, ByVal Y As Integer, ByVal cx As Integer, ByVal cy As Integer, ByVal uFlags As UInteger) As Boolean
    End Function

#Region "LAYER"
    Public Const AC_SRC_OVER As Byte = 0
    Public Const AC_SRC_ALPHA As Byte = 1
    Public Const ULW_ALPHA As Int32 = 2

    Public Declare Auto Function CreateCompatibleDC Lib "gdi32.dll" (hDC As IntPtr) As IntPtr
    Public Declare Auto Function GetDC Lib "user32.dll" (hWnd As IntPtr) As IntPtr

    <DllImport("gdi32.dll", ExactSpelling:=True)>
    Public Function SelectObject(hDC As IntPtr, hObj As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll", ExactSpelling:=True)>
    Public Function ReleaseDC(hWnd As IntPtr, hDC As IntPtr) As Integer
    End Function

    Public Declare Auto Function DeleteDC Lib "gdi32.dll" (hDC As IntPtr) As Integer
    Public Declare Auto Function DeleteObject Lib "gdi32.dll" (hObj As IntPtr) As Integer
    Public Declare Auto Function UpdateLayeredWindow Lib "user32.dll" (hwnd As IntPtr, hdcDst As IntPtr, ByRef pptDst As Point32, ByRef psize As Size32, hdcSrc As IntPtr, ByRef pptSrc As Point32, crKey As Int32, ByRef pblend As BLENDFUNCTION, dwFlags As Int32) As Integer
    Public Declare Auto Function ExtCreateRegion Lib "gdi32.dll" (lpXform As IntPtr, nCount As UInteger, rgnData As IntPtr) As IntPtr

    <StructLayout(LayoutKind.Sequential)>
    Public Structure Size32
        Public cx As Int32
        Public cy As Int32

        Public Sub New(x As Int32, y As Int32)
            cx = x
            cy = y
        End Sub
    End Structure

    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure BLENDFUNCTION
        Public BlendOp As Byte
        Public BlendFlags As Byte
        Public SourceConstantAlpha As Byte
        Public AlphaFormat As Byte
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Public Structure Point32
        Public x As Int32
        Public y As Int32

        Public Sub New(x As Int32, y As Int32)
            Me.x = x
            Me.y = y
        End Sub
    End Structure

    Public Sub SetBits(B As System.Drawing.Bitmap, handle As IntPtr, toplocpt As Point32, opacity!)

        Dim oldBits As IntPtr = IntPtr.Zero
        Dim screenDC As IntPtr = GetDC(IntPtr.Zero)
        Dim hBitmap As IntPtr = IntPtr.Zero
        Dim memDc As IntPtr = CreateCompatibleDC(screenDC)

        Try
            Dim topLoc As Point32 = toplocpt
            Dim bitMapSize As New Size32(B.Width, B.Height)
            Dim blendFunc As New BLENDFUNCTION()
            Dim srcLoc As New Point32(0, 0)

            hBitmap = B.GetHbitmap(System.Drawing.Color.FromArgb(0))
            oldBits = SelectObject(memDc, hBitmap)

            blendFunc.BlendOp = AC_SRC_OVER
            blendFunc.SourceConstantAlpha = opacity * 255
            blendFunc.AlphaFormat = AC_SRC_ALPHA
            blendFunc.BlendFlags = 0

            UpdateLayeredWindow(handle, screenDC, topLoc, bitMapSize, memDc, srcLoc, 0, blendFunc, ULW_ALPHA)
        Finally
            If hBitmap <> IntPtr.Zero Then
                SelectObject(memDc, oldBits)
                DeleteObject(hBitmap)
            End If
            ReleaseDC(IntPtr.Zero, screenDC)
            DeleteDC(memDc)
        End Try
    End Sub

#End Region


End Module

Public Class LayeredForm : Inherits Form
    Protected Shadows ParentForm As IntPtr = IntPtr.Zero
    Public Property ClickThrough As Boolean = False
    Public Property Disabled As Boolean = False
    Public Property ToolWindow As Boolean = False

    Public Sub New()
        SetStyle(ControlStyles.OptimizedDoubleBuffer Or ControlStyles.SupportsTransparentBackColor Or ControlStyles.ResizeRedraw Or ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint, True)
        DoubleBuffered = True
        ControlBox = False
    End Sub
    Public Sub New(ByRef Parent As Form)
        Me.New()
        Me.ParentForm = Parent.Handle
        AddOwnedForm(Parent)
        AddHandler Parent.FormClosing, AddressOf Dispose
    End Sub


    Protected Overrides ReadOnly Property CreateParams() As CreateParams
        Get
            Dim cParms As CreateParams = MyBase.CreateParams
            cParms.ExStyle = cParms.ExStyle Or 524288
            If Disabled Then cParms.ExStyle = cParms.ExStyle Or 134217728
            If ClickThrough Then cParms.ExStyle = cParms.ExStyle Or 32
            If ToolWindow Then cParms.ExStyle = cParms.ExStyle Or 128
            Return cParms
        End Get
    End Property
    Public Sub SetBits(B As Bitmap)
        If Not IsHandleCreated Or DesignMode Then Exit Sub

        If Not Bitmap.IsCanonicalPixelFormat(B.PixelFormat) OrElse Not Bitmap.IsAlphaPixelFormat(B.PixelFormat) Then
            Throw New ApplicationException("The picture must be 32bit picture with alpha channel.")
        End If

        Dim oldBits As IntPtr = IntPtr.Zero
        Dim screenDC As IntPtr = GetDC(IntPtr.Zero)
        Dim hBitmap As IntPtr = IntPtr.Zero
        Dim memDc As IntPtr = CreateCompatibleDC(screenDC)

        Try
            Dim topLoc As New Point32(Left, Top)
            Dim bitMapSize As New Size32(B.Width, B.Height)
            Dim blendFunc As New BLENDFUNCTION()
            Dim srcLoc As New Point32(0, 0)

            hBitmap = B.GetHbitmap(Color.FromArgb(0))
            oldBits = SelectObject(memDc, hBitmap)

            blendFunc.BlendOp = AC_SRC_OVER
            blendFunc.SourceConstantAlpha = 255
            blendFunc.AlphaFormat = AC_SRC_ALPHA
            blendFunc.BlendFlags = 0

            UpdateLayeredWindow(Handle, screenDC, topLoc, bitMapSize, memDc, srcLoc,
             0, blendFunc, ULW_ALPHA)
        Finally
            If hBitmap <> IntPtr.Zero Then
                SelectObject(memDc, oldBits)
                DeleteObject(hBitmap)
            End If
            ReleaseDC(IntPtr.Zero, screenDC)
            DeleteDC(memDc)
        End Try
    End Sub
End Class

Public Class ShadowForm
    Inherits LayeredForm

    Public ShadowSize As Integer

#Region "Ctor"
    Public Sub New(ByRef Parent As Form, ByVal ShadowSize As Integer)

        MyBase.New(Parent)

        ToolWindow = 1
        ClickThrough = 1
        Disabled = 1

        ' SetStyle(ControlStyles.Selectable Or ControlStyles.StandardClick Or ControlStyles.StandardDoubleClick Or ControlStyles.Opaque, False)

        ' Me.ShadowSize = ShadowSize

        FormBorderStyle = FormBorderStyle.None
        ShowInTaskbar = False
        Text = ""


        ' AddHandler Parent.Resize, Sub() UpdateBounds()
        ' AddHandler Parent.Move, Sub() UpdateBounds()
        'AddHandler Parent.Layout, Sub() UpdateBounds()

        BackColor = Color.Black
    End Sub
#End Region



    Public Shadows Sub UpdateBounds()
        Dim corx As Integer = CInt((Screen.PrimaryScreen.WorkingArea.Width / 1) - (Form.FromHandle(ParentForm).Width / 1))
        Dim cory As Integer = CInt((Screen.PrimaryScreen.WorkingArea.Height / 1) - (Form.FromHandle(ParentForm).Height / 1))
        SetWindowPos(Handle, ParentForm, Form.FromHandle(ParentForm).Left - ShadowSize, Form.FromHandle(ParentForm).Top - ShadowSize + 5, corx, cory, &H20)
    End Sub


End Class

Public Module helperst
    Friend Function bmp64(s As String) As Bitmap
        s.Replace("data:image/png;base64,", "")
        Return New Bitmap(Image.FromStream(New System.IO.MemoryStream(Convert.FromBase64String(s))))
    End Function
    Friend Function col(a!, r!, g!, b!, sat!) As Color
        Return Color.FromArgb(CByte(CInt(a)), CByte(Math.Min(255, Math.Max(0, CInt(r) * sat))), CByte(Math.Min(255, Math.Max(0, CInt(g) * sat))), CByte(Math.Min(255, Math.Max(0, CInt(b) * sat))))
    End Function
    Friend Function col(a!, r!, g!, b!) As Color
        Return col(a, r, g, b, 1)
    End Function
    Friend Function col(r As Byte, g As Byte, b As Byte) As Color
        Return col(255, r, g, b)
    End Function
    Friend Function col(n As Byte) As Color
        Return col(n, n, n)
    End Function
    Friend Function col(a As Byte, n As Byte) As Color
        Return col(a, n, n, n)
    End Function
    Friend Function col(a As Byte, c As Color) As Color
        Return col(a, c.R, c.G, c.B, 1)
    End Function
    Friend Function hsltorgb(h!, s!, l!) As Color
        Dim c!, k!, m!
        c = (1 - Math.Abs(2 * l - 1)) * s
        k = c * (1 - Math.Abs(((h / 60) Mod 2) - 1))
        m = l - c / 2
        Return cyltorgb(c, k, m, h)
    End Function
    Friend Function hsvtorgb(h!, s!, v!) As Color
        Dim c!, k!, m!
        c = v * s
        k = c * (1 - Math.Abs(((h / 60) Mod 2) - 1))
        m = v - c
        Return cyltorgb(c, k, m, h)
    End Function
    Private Function cyltorgb(c!, k!, m!, h!) As Color
        Dim rd() = {c, k, 0, 0, k, c}
        Dim gd() = {k, c, c, k, 0, 0}
        Dim bd() = {0, 0, k, c, c, k}
        Dim t = Math.Ceiling(h / 60)
        If t <= 0 Then t = 0 Else t -= 1
        Return col((rd(t) + m) * 255, (gd(t) + m) * 255, (bd(t) + m) * 255)
    End Function

    Friend Function rgbtohsv(r!, g!, b!) As Single()
        Dim cmax!, cmin!, rd!, gd!, bd!, del!
        Dim h!, s!, v!

        rd = r / 255 : gd = g / 255 : bd = b / 255
        cmax = Math.Max(Math.Max(rd, gd), bd)
        cmin = Math.Min(Math.Min(rd, gd), bd)
        del = cmax - cmin

        If Not del = 0 Then
            If cmax = rd Then h = 60 * (((gd - bd) / del) Mod 6)
            If cmax = gd Then h = 60 * ((bd - rd) / del + 2)
            If cmax = bd Then h = 60 * ((rd - gd) / del + 4)
        Else
            h = 0
        End If
        If cmax = 0 Then s = 0 Else s = del / cmax

        v = cmax

        If h < 0 Then h = 360 + h 'bug where h becomes -ve

        Return {h, s, v}
    End Function
    Friend Function rescol(c As Color) As Color
        'Dim t = (Val(CStr(c.R)) + Val(CStr(c.G)) + Val(CStr(c.B))) / 3 '				stupid bug
        Dim t = (CInt(CByte(c.R)) + CInt(CByte(c.G)) + CInt(CByte(c.B))) / 3
        If t < 128 Then Return Color.White Else Return Color.Black
    End Function

    'lum = Y=0.3RED+0.59GREEN+0.11Blue   rgb(y,y,y)
    Friend Function invert(c As Color) As Color
        Return col(CByte(CInt(c.A)), CByte(CInt(255 - c.R)), CByte(CInt(255 - c.G)), CByte(CInt(255 - c.B)))
    End Function
    Friend Sub mp(c As Color, ByRef p As Pen)
        p = New Pen(c)
    End Sub

    Friend Sub mb(c As Color, ByRef b As SolidBrush)
        b = New SolidBrush(c)
    End Sub

    Function rct(x!, y!, w!, h!) As Rectangle
        Return New Rectangle(x, y, w, h)
    End Function
    Function rct(lgb As LinearGradientBrush, Optional xshift! = 0, Optional yshift! = 0, Optional WidthShift! = 0, Optional HeightShift! = 0) As Rectangle
        Dim r = lgb.Rectangle
        Return rct(r.X + xshift, r.Y + yshift, r.Width + WidthShift, r.Height + HeightShift)
    End Function
    Friend Function pt(x!, y!) As Point
        Return New Point(x, y)
    End Function
    Friend Function rgbtohex(c As Color) As String
        Return ColorTranslator.ToHtml(c)
    End Function
    Friend Function hextorgb(hex As String) As Color
        Return ColorTranslator.FromHtml(hex)
    End Function
    Public Function BytesToIcon(ByVal bytes As Byte()) As Icon
        Using ms As New MemoryStream(bytes)
            Return New Icon(ms)
        End Using
    End Function
#Region "Math!"
    Public Function clamp#(v#, l#, h#)
        Return Math.Min(h, Math.Max(l, v))
    End Function
#End Region

    <DllImport("gdi32.dll", SetLastError:=True)>
    Public Function GetPixel(hdc As IntPtr,
              nXPos As Integer,
              nYPos As Integer) As UInteger
    End Function
    Public Function GetPixelColor(ByVal x As Integer, ByVal y As Integer) As System.Drawing.Color
        Dim hdc As IntPtr = GetDC(IntPtr.Zero)
        Dim pixel As UInteger = GetPixel(hdc, x, y)
        ReleaseDC(IntPtr.Zero, hdc)
        Dim color As Color = color.FromArgb(CInt(Fix(pixel And &HFF)), CInt(Fix(pixel And &HFF00)) >> 8, CInt(Fix(pixel And &HFF0000)) >> 16)
        Return color
    End Function

    Public Const IMAGE_CURSOR As Integer = &H2
    Public Const LR_LOADFROMFILE As Integer = &H10
    Public CursorPtr As IntPtr = IntPtr.Zero
    <DllImport("user32.dll", EntryPoint:="LoadImageW")> Public Function LoadImageW(ByVal hinst As IntPtr, <MarshalAs(UnmanagedType.LPTStr)> ByVal lpszName As String, ByVal uType As UInteger, ByVal cxDesired As Integer, ByVal cyDesired As Integer, ByVal fuLoad As UInteger) As IntPtr
    End Function
    <DllImport("user32.dll", EntryPoint:="DestroyCursor")> Public Function DestroyCursor(ByVal hCursor As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function


End Module

#End Region

#Region "Components"
<ToolboxItem(False)> Class picker : Inherits customControl
#Region "DECLARE"
    Public Property DontChange As Boolean = False
    Private hp() As Byte = {0, 0}
    Private tbt() As Bitmap = {New Bitmap(20, 360), New Bitmap(20, 200)}
    Private ia() As Byte = {1, 1, 1}
    Public Event ColorChanged()
    Private _h! : Public Property h!
        Get
            Return clamp(_y2, 0, 200) * 1.8
        End Get
        Set(value!)
            If DontChange Then Return
            _h = value
            _y2 = clamp(value, 0, 360) / 360 * 200
            ia(0) = 1
            ia(1) = 1
            Invalidate()
            Debug.WriteLine("p" + CStr(_h))
        End Set
    End Property
    Private _s! : Public Property s!
        Get
            Return clamp(_x, 0, 200) / 200
        End Get
        Set(value!)
            If DontChange Then Return
            _s = clamp(value, 0, 1)
            _x = clamp(_s * 200, 0, 200)
            ia(0) = 1
            Invalidate()
        End Set
    End Property
    Private _v! : Public Property v!
        Get
            Return clamp((200 - _y1), 0, 200) / 200
        End Get
        Set(value!)
            If DontChange Then Return
            _v = clamp(value, 0, 1)
            _y1 = clamp(200 - _v * 200, 0, 200)
            ia(0) = 1
            Invalidate()
        End Set
    End Property
    Private _a! : Public Property a!
        Get
            Return clamp(_y3, 0, 200) / 200 * 255
        End Get
        Set(value!)
            If DontChange Then Return
            _a = clamp(value, 0, 255)
            _y3 = clamp(_a / 255 * 200, 0, 200)
            ia(2) = 1
            Invalidate()
        End Set
    End Property
    Private _x!, _y1!, _y2!, _y3!
#End Region
#Region "CTOR"
    Sub New(h!, s!, v!, a!)
        Me.h = h
        Me.s = s
        Me.v = v
        Me.a = a
        Left = 0 : Top = 0
        Width = 250 : Height = 200
    End Sub
#End Region
#Region "PAINT"
    Protected Overrides Sub PaintHook()

        G.Clear(col(51))

        If ia(0) Then
            G.SetClip(rct(0, 0, 200, 200))
            G.Clear(Color.White)
            'Debug.Write(h)
            Dim l1 As New LinearGradientBrush(rct(0, 0, 200, 200), col(0, 0), hsvtorgb(h, 1, 1), 0)
            Dim l2 As New LinearGradientBrush(rct(l1), col(0, 0), col(0), 90)
            G.FillRectangle(l1, rct(l1))
            G.FillRectangle(l2, rct(l2))

            G.SmoothingMode = 2

            mp(col(150, invert(rescol(hsvtorgb(360 - h, 0, 1 - v)))), tp)
            G.SetClip(rct(_x - 1, _y1 - 1, 3, 3), CombineMode.Exclude)
            G.DrawLine(tp, 0, _y1, 200, _y1)
            G.DrawLine(tp, _x, 0, _x, 200)
            G.ResetClip()
            G.DrawRectangle(tp, rct(_x - 2, _y1 - 2, 4, 4))
        End If 'map


        If Not hp(0) Then
            With Graphics.FromImage(tbt(0))
                .Clear(Color.Transparent)
                For i = 0 To 359
                    mp(hsvtorgb(i, 1, 1), tp)
                    .DrawLine(tp, 0, i, 20, i)
                Next
            End With
            hp(0) = 1
        End If

        If ia(1) Then
            G.SetClip(rct(211, 0, 20, 200))
            G.Clear(Color.White)
            G.DrawImage(tbt(0), 211, 0, 20, 200)
            mp(Color.Black, tp)
            G.SmoothingMode = 2
            G.DrawLine(tp, 211, _y2, 231, _y2)
            mb(Color.Black, tb)
            G.FillPolygon(tb, {pt(210, _y2 - 5), pt(210 + 5, _y2), pt(210, _y2)})
            G.FillPolygon(tb, {pt(210, _y2 + 5), pt(210 + 5, _y2), pt(210, _y2)})
            G.ResetClip()
        End If 'hue


        If ia(2) Then
            G.SetClip(rct(231, 0, 20, 200))
            G.Clear(col(51))
            For i = 0 To 9
                mb(col(50, 255), tb)
                G.FillRectangle(tb, rct(231, i * 20, 10, 10))
                G.FillRectangle(tb, rct(241, i * 20 + 10, 10, 10))
            Next
            Dim l As New LinearGradientBrush(rct(231, 0, 20, 200), col(0, 0), hsvtorgb(h, s, v), 90)
            G.SmoothingMode = 2
            G.FillRectangle(l, rct(l))
            mp(invert(hsvtorgb(h, s, v)), tp)
            G.DrawLine(tp, 231, _y3, 251, _y3)
            mb(invert(rescol(hsvtorgb(360 - h, 0, 1 - v))), tb)
            G.FillPolygon(tb, {pt(230, _y3 - 5), pt(230 + 5, _y3), pt(230, _y3)})
            G.FillPolygon(tb, {pt(230, _y3 + 5), pt(230 + 5, _y3), pt(230, _y3)})
            G.ResetClip()
        End If 'alpha


    End Sub
#End Region
#Region "INPUT"
    Protected Overrides Sub OnClick(e As EventArgs)
        ' MyBase.OnClick(e)
        '  mousedownmove(e)
    End Sub
    Protected Overrides Sub mousedownmove(e As MouseEventArgs)
        MyBase.mousedownmove(e)
        If Not (e.Y > -1 Or e.Y < 211) Then Return
        'ia(0) = 0 : ia(1) = 0 : ia(2) = 0
        If e.X > -1 And e.X < 211 Then
            _x = e.X
            _y1 = e.Y
            ia(0) = 1
            Invalidate(rct(-1, -1, 211, 211))
            Invalidate(rct(231, 0, 20, 200))
        ElseIf e.X > 200 And e.X < 231 Then
            _y2 = e.Y
            ia(1) = 1
            Invalidate(rct(-1, 0, 232, 200))
        ElseIf e.X > 230 And e.X < 251 Then
            _y3 = e.Y
            ia(2) = 1
            Invalidate(rct(231, 0, 20, 200))
        End If
        RaiseEvent ColorChanged()
    End Sub
#End Region
End Class
Class Preview : Inherits LayeredForm

    Public Sub New(ByRef Parent As Form)
        MyBase.New(Parent)
        ShowInTaskbar = 0
        Disabled = 1
        Text = ""
        Top = 0
        Opacity = 1
        FormBorderStyle = 0
        AddHandler Parent.Move, Sub() updatebounds()

    End Sub
    Shadows Sub updatebounds()
        SetWindowPos(Handle, ParentForm, Form.FromHandle(ParentForm).Left + 15, Form.FromHandle(ParentForm).Top + 200 + 9, 250, 45, &H20)
    End Sub
    Protected Overrides Sub OnClick(e As EventArgs)
        MyBase.OnClick(e)
        DirectCast(Form.FromHandle(ParentForm), Form).Close()
        Me.Close()
    End Sub
End Class
Class edback : Inherits LayeredForm
    Public Property color
    Dim tb As Bitmap
    Sub New(ByRef p As Form)
        ParentForm = p.Handle
        Width = Screen.PrimaryScreen.WorkingArea.Width
        Height = Screen.PrimaryScreen.WorkingArea.Height
        Top = 0
        Left = 0
        TopMost = 1
        ShowInTaskbar = 0
        '  Cursor = New Cursor(LoadImageW(IntPtr.Zero, IO.Path.Combine(Application.StartupPath, "eyedropper.cur"), IMAGE_CURSOR, 0, 0, LR_LOADFROMFILE))
        FormBorderStyle = 0
        tb = New Bitmap(Width, Height)
        Graphics.FromImage(tb).Clear(col(0, 0))
        SetBits(tb)
        tb.Dispose()
    End Sub
    Protected Overrides Sub OnClick(e As EventArgs)
        MyBase.OnClick(e)
        color = GetPixelColor(Cursor.Position.X, Cursor.Position.Y)
        DirectCast(Form.FromHandle(ParentForm), customColorPicker).edclick(color)
        Dispose()
    End Sub
End Class
#End Region


Class customColorPicker : Inherits Form
#Region "DECLARE"
    Dim dc% = -1
    Dim p As New picker(255, 0, 0, 0)
    Dim t(5) As NumericUpDown
    Dim pt(3) As TextBox
    Dim hext As TextBox = New TextBox() With {.BackColor = col(255), .ForeColor = col(51), .Left = 387, .Width = 100, .BorderStyle = BorderStyle.FixedSingle, .Top = 85}
    Dim l(3) As Label
    Private Shadow As New ShadowForm(Me, 14)
    Dim pr As preview = New preview(Shadow)
    Public Property Color As Color
    Dim eb As New Button With {.Left = 265, .Top = 200, .FlatStyle = FlatStyle.Flat, .BackColor = System.Drawing.Color.Gray, .Image = bmp64("Qk0mBAAAAAAAADYAAAAoAAAAEgAAABIAAAABABgAAAAAAPADAAAAAAAAAAAAAAAAAAAAAAAA////////////////////////////////////////////////////////////////////////AAD///8YGBj///////////////////////////////////////////////////////////////8AAL+/v/j4+BYWFhYWFv///////////////////////////////////////////////////////wAA////vb299vb2vr6+FRUV////////////////////////////////////////////////////AAD///+Ghob////39/e+vr4VFRX///////////////////////////////////////////////8AAP///////7CwsP////b29r29vRYWFv///////////////////////////////////////////wAA////////////sLCw////9/f3vr6+GBgY////////////////////////////////////////AAD///////////////+urq7////39/e+vr4WFhb///////////////////////////////////8AAP///////////////////7CwsP////b29nx8fBUVFf///zAwMP///////////////////////wAA////////////////////////ra2t////oqKifHx8MDAwMDAwPj4+////////////////////AAD///////////////////////////+tra2oqKgwMDAwMDAwMDA9PT0wMDD///////////////8AAP///////////////////////////////zAwME9PTzAwMD4+Pj09Pf///////////////////wAA////////////////////////////MDAwt7e3h4eHV1dXNjY2U1NTNjc3////////////////AAD///////////////////////////////8+Pj63t7c2Nja3t7eHh4dPT08wMDD///////////8AAP///////////////////////////////////zAwMP///zAwMLe3t4eHh1RUVTU2Nv///////wAA////////////////////////////////////////////////MDAwt7e3MTExPT4+////////AAD///////////////////////////////////////////////////8wMDAwMDD///////////8AAP///////////////////////////////////////////////////////////////////////wAA")}

    Dim bd() As Byte
#End Region

    Sub New()


        TransparencyKey = Color.Fuchsia

        Controls.Add(p)
        AddHandler p.ColorChanged, AddressOf ColorChangedbypicker



        For i = 1 To 4
            t(i - 1) = New NumericUpDown With {.TabIndex = i - 1, .Left = 14 * i + 45 * (i - 1) + 250, .Top = 30, .BorderStyle = BorderStyle.FixedSingle,
                                        .BackColor = col(255), .ForeColor = col(51), .Width = 45, .Font = New Font("Consolas", 10), .Minimum = 0, .Maximum = 255}
            Controls.Add(t(i - 1))
            AddHandler t(i - 1).ValueChanged, AddressOf ColorChangedbytextrgb
        Next


        l(0) = New Label With {.Left = 250 + 14, .Top = 10, .ForeColor = col(51), .Font = New Font("Consolas", 10), .Text = "  R      G       B      A", .Width = 250}
        Controls.Add(l(0))
        l(1) = New Label With {.Left = 250 + 14, .Top = 85, .ForeColor = col(51), .Font = New Font("Consolas", 10), .Text = "HEX :", .Width = 50}
        Controls.Add(l(1))
        l(0) = New Label With {.Left = 250 + 14, .Top = 120, .ForeColor = col(51), .Font = New Font("Consolas", 10), .Text = "  H      S       V      A", .Width = 250}
        Controls.Add(l(0))


        Controls.Add(hext)
        AddHandler hext.TextChanged, AddressOf colorchangebytexthex

        Controls.Add(eb)
        AddHandler eb.Click, AddressOf edstart

        t(4) = New NumericUpDown With {.TabIndex = 4, .Left = 14 * 1 + 45 * (0) + 250, .Top = 144, .BorderStyle = BorderStyle.FixedSingle,
                                        .BackColor = col(255), .ForeColor = col(51), .Width = 45, .Font = New Font("Consolas", 10), .Minimum = 0, .Maximum = 360}
        Controls.Add(t(4))
        AddHandler t(4).ValueChanged, AddressOf colorchangebytexthsv


        For j = 1 To 3
            pt(j - 1) = New TextBox With {.TabIndex = j - 1 + 5, .Left = 14 * (j + 1) + 45 * ((j + 1) - 1) + 250, .Top = 144, .BorderStyle = BorderStyle.FixedSingle,
                                        .BackColor = col(255), .ForeColor = col(51), .Width = 45, .Font = New Font("Consolas", 10)}
            Controls.Add(pt(j - 1))
            AddHandler pt(j - 1).TextChanged, AddressOf colorchangebytexthsv
        Next


    End Sub


#Region "Color Changed"
    Sub changergbtext()
        t(0).Value = Color.R
        t(1).Value = Color.G
        t(2).Value = Color.B
        t(3).Value = Color.A
    End Sub
    Sub changehex()
        Dim hexalpha As String = Hex(Color.A)
        If hexalpha.Length = 1 Then hexalpha = "0" + hexalpha
        hext.Text = "#" + hexalpha + rgbtohex(Color).Remove(0, 1)
    End Sub
    Sub changep()
        Dim hsv() As Single = rgbtohsv(Color.R, Color.G, Color.B)
        p.h = hsv(0)
        p.s = hsv(1)
        p.v = hsv(2)
        p.a = Color.A
    End Sub
    Sub changehsv()
        Dim hsv() As Single = rgbtohsv(Color.R, Color.G, Color.B)
        t(4).Value = hsv(0)
        pt(0).Text = hsv(1)
        pt(1).Text = hsv(2)
        pt(2).Text = Color.A / 255

    End Sub


    Sub ColorChangedbypicker()
        If dc > -1 Then Return
        dc = 0
        Color = col(p.a, hsvtorgb(p.h, p.s, p.v))
        changergbtext()
        changehex()
        changehsv()
        drawpreview()
        dc = -1
    End Sub
    Sub ColorChangedbytextrgb()
        If dc > -1 Then Return
        dc = 1
        p.DontChange = False
        Color = col(t(3).Value, t(0).Value, t(1).Value, t(2).Value)
        changep()
        changehex()
        changehsv()
        drawpreview()
        dc = -1
    End Sub
    Sub colorchangebytexthex()
        If dc > -1 Then Return
        dc = 2
        Dim th$ = hext.Text
        If hext.Text.Length = 1 Then th = hext.Text + "0"
        Color = hextorgb(th)
        changep()
        changergbtext()
        changehsv()
        dc = -1
    End Sub
    Sub colorchangebytexthsv()
        If dc > -1 Then Return
        dc = 4
        Color = col(Val(pt(2).Text) * 255, hsvtorgb(t(4).Value, Val(pt(0).Text), Val(pt(1).Text)))
        changehex()
        changep()
        changergbtext()
        drawpreview()
        dc = -1
    End Sub

    Dim edback1 As edback
    Sub edstart()
        edback1 = New edback(Me)
        dc = 3
        edback1.Show()
    End Sub

    Public Sub edclick(c As Color)
        Color = col(Color.A, c)
        changep()
        changergbtext()
        changehex()
        drawpreview()

        dc = -1

    End Sub
#End Region

#Region "Other"
    Sub bye()
        Close()
    End Sub

#Region "Move"
    Dim mouseOffset As Point

    Private Sub Me_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs) Handles MyBase.MouseDown
        mouseOffset = New Point(-e.X, -e.Y)
    End Sub

    Private Sub Me_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles MyBase.MouseMove

        If e.Button = MouseButtons.Left Then
            Dim mousePos = Control.MousePosition
            mousePos.Offset(mouseOffset.X, mouseOffset.Y)
            Location = mousePos
        End If
    End Sub
#End Region
#Region "LOAD/CLOSE"
    Protected Overrides Sub OnCreateControl()
        MyBase.OnCreateControl()
        FormBorderStyle = 0
        BackColor = col(245)
        SetWindowPos(Handle, vbNull, Cursor.Position.X - 120, Cursor.Position.Y - 120, 500, 245, &H20)
        DrawShadow()
    End Sub
    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
        Dim cursorpath$ = IO.Path.Combine(Application.StartupPath, "eyedropper.cur")
        bd = Convert.FromBase64String("AAACAAEAICAAAAcAFwCoDAAAFgAAACgAAAAgAAAAQAAAAAEAGAAAAAAAgAwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAYGBgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAC/v7/4+PgWFhYWFhYAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAC9vb329va+vr4VFRUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACGhobY6ez39/e+vr4VFRUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACwsLDY6ez29va9vb0WFhYAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACwsLDY6ez39/e+vr4YGBgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACurq7Y6ez39/e+vr4WFhYAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACwsLDY6ez29vZ8fHwVFRUAAAAwMDAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACtra3Y6eyioqJ8fHwwMDAwMDA+Pj4AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACtra2oqKgwMDAwMDAwMDA9PT0wMDAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAwMDBPT08wMDA+Pj49PT0AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAwMDC3t7eHh4dXV1c2NjZTU1M2NzcAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA+Pj63t7c2Nja3t7eHh4dPT08wMDAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAwMDAAAAAwMDC3t7eHh4dUVFU1NjYAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAwMDC3t7cxMTE9Pj4AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAwMDAwMDAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD///////////////////////////////////////////9////+H////w////8H////g////8H////g////8F////gP///8B////g////wH///+A////0H////h////8////////////////////////////////////////////w==")
        If Not File.Exists(cursorpath) Then
            ' IO.File.WriteAllBytes(cursorpath, bd)
        End If
        drawpreview()
        Shadow.Show()
        pr.Visible = 1
        pr.Show()
    End Sub
    Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)
        MyBase.OnFormClosing(e)
        e.Cancel = 1
        pr.Close()
        Close()
    End Sub
#End Region
#Region "SHADOW"
#Region "declare"
    Private Rounding!
#End Region
    Public op# = 1.0#
    Private Sub DrawShadow()
        If DesignMode Or IsHandleCreated = False Then Exit Sub
        Dim B As New Bitmap(CInt(Shadow.Size.Width), CInt(Shadow.Size.Height))
        Dim G As Graphics = Graphics.FromImage(B)
        G.InterpolationMode = 7
        G.SmoothingMode = 2 : G.PixelOffsetMode = 2
        Shadow.BackColor = Color.Black
        Static br As SolidBrush
        With G
            Dim s = Shadow.ShadowSize
            .SetClip(New Rectangle(Shadow.ShadowSize + 2, Shadow.ShadowSize + 2, Width - 4, Height - 5 - 4), CombineMode.Exclude)
            For i = s To 0 Step -1
                Rounding = 5 + s - i
                Dim pth As GraphicsPath = DM.CreateRoundRectangle(i, i, Shadow.Width - 1 - (i * 2), Shadow.Height - 1 - (i * 2) - 3, Rounding, , , , )
                mb(col((i ^ (0.111 * i) * op), Shadow.BackColor), br)
                .FillPath(br, pth)
                pth.Dispose()
            Next


            ''(i ^ (i / 9))          Rounding +  (Shadow.ShadowSize - i)
            'Rounding = 1.5 * (Shadow.ShadowSize)
            ''(Shadow.ShadowSize + i) ^ 1.6
            'mb(col((i ^ (0.117647 * i)), Shadow.BackColor), br)




        End With

        G.Dispose()
        Shadow.SetBits(B)
        B.Dispose()

    End Sub
    'below by blackcap
    Friend Class DM

        Public Shared Function CreateRoundRectangle(ByVal rectangle As Rectangle, ByVal radius As Integer, Optional ByVal TopLeft As Boolean = True, Optional ByVal TopRigth As Boolean = True, Optional ByVal BottomRigth As Boolean = True, Optional ByVal BottomLeft As Boolean = True) As GraphicsPath
            Dim path As New GraphicsPath()
            Dim l As Integer = rectangle.Left
            Dim t As Integer = rectangle.Top
            Dim w As Integer = rectangle.Width
            Dim h As Integer = rectangle.Height
            Dim d As Integer = radius << 1

            If radius <= 0 Then
                path.AddRectangle(rectangle)
                Return path
            End If

            If TopLeft Then
                path.AddArc(l, t, d, d, 180, 90)
                If TopRigth Then path.AddLine(l + radius, t, l + w - radius, t) Else path.AddLine(l + radius, t, l + w, t)
            Else
                If TopRigth Then path.AddLine(l, t, l + w - radius, t) Else path.AddLine(l, t, l + w, t)
            End If

            If TopRigth Then
                path.AddArc(l + w - d, t, d, d, 270, 90)
                If BottomRigth Then path.AddLine(l + w, t + radius, l + w, t + h - radius) Else path.AddLine(l + w, t + radius, l + w, t + h)
            Else
                If BottomRigth Then path.AddLine(l + w, t, l + w, t + h - radius) Else path.AddLine(l + w, t, l + w, t + h)
            End If

            If BottomRigth Then
                path.AddArc(l + w - d, t + h - d, d, d, 0, 90)
                If BottomLeft Then path.AddLine(l + w - radius, t + h, l + radius, t + h) Else path.AddLine(l + w - radius, t + h, l, t + h)
            Else
                If BottomLeft Then path.AddLine(l + w, t + h, l + radius, t + h) Else path.AddLine(l + w, t + h, l, t + h)
            End If

            If BottomLeft Then
                path.AddArc(l, t + h - d, d, d, 90, 90)
                If TopLeft Then path.AddLine(l, t + h - radius, l, t + radius) Else path.AddLine(l, t + h - radius, l, t)
            Else
                If TopLeft Then path.AddLine(l, t + h, l, t + radius) Else path.AddLine(l, t + h, l, t)
            End If

            path.CloseFigure()
            Return path
        End Function
        Public Shared Function CreateRoundRectangle(x As Integer, y As Integer, w As Integer, h As Integer, radius As Integer, Optional ByVal TopLeft As Boolean = True, Optional ByVal TopRigth As Boolean = True, Optional ByVal BottomRigth As Boolean = True, Optional ByVal BottomLeft As Boolean = True) As GraphicsPath
            Return CreateRoundRectangle(New Rectangle(x, y, w, h), radius, TopLeft, TopRigth, BottomRigth, BottomLeft)
        End Function

    End Class
    'above by blackcap
#End Region
#Region "PREVIEW"
    Sub drawpreview()
        Dim pb As New Bitmap(250, 45)
        With Graphics.FromImage(pb)
            .Clear(Color)
        End With
        ' pr.SetBits(pb)
        ' pb.Dispose()
    End Sub
    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        ' MyBase.OnPaint(e)
        '  e.Graphics.FillRectangle(Brushes.Fuchsia, rct(0, 200, 250, 45))
    End Sub
#End Region
#End Region
End Class