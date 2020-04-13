'Framework API's
Imports System.IO
Imports System.Text
Imports System.Net
Imports System.ComponentModel
Imports System.Web.Script.Serialization
Imports System.Text.RegularExpressions
Imports System.Security.Principal

'External API's
'Imports SampAPI
Imports SAMPCustomLauncher.SampQueryService.QueryResult
Imports SAMPCustomLauncher.SampQueryService

'Internal class
Imports SAMPCustomLauncher.Utils
Imports SAMPCustomLauncher.ColorManager
Imports SAMPCustomLauncher.CoreGTA
Imports SAMPCustomLauncher.ReadWritingMemory
Imports SAMPCustomLauncher.ProcessElevation
Imports SAMPCustomLauncher.WindowedMode
Imports System.Threading

Public Class Form1

#Region " Declarations "

    Dim SampDllDir As String = "S4Lsalsoft"
    Dim PluginsDir As String = SampDllDir & "\" & "DllPlugins"
    Dim PlayerContadorTIP As String = String.Empty
    Dim PlayerContadorTPort As String = String.Empty
    Dim LauncherDir As String = Application.ExecutablePath
    Dim SandboxieDir As String = My.Computer.FileSystem.SpecialDirectories.ProgramFiles & "\Sandboxie\Start.exe"
    Dim EXE_File As String = GameStart()
    Dim Gta_Dir As String = Path.GetDirectoryName(GameStart())
    Dim DLL_File As String = Gta_Dir & "\samp.dll"
    Dim DLL_D3D_File As String = Gta_Dir & "\d3d9.dll"

#End Region

#Region " Shadow Efect "

    Private Drag As Boolean
    Private MouseX As Integer
    Private MouseY As Integer
    Private Const WM_NCHITTEST As Integer = &H84
    Private Const HTCLIENT As Integer = &H1
    Private Const HTCAPTION As Integer = &H2
    Private m_aeroEnabled As Boolean
    Private Const CS_DROPSHADOW As Integer = &H20000
    Private Const WM_NCPAINT As Integer = &H85
    Private Const WM_ACTIVATEAPP As Integer = &H1C

    <System.Runtime.InteropServices.DllImport("dwmapi.dll")>
    Public Shared Function DwmExtendFrameIntoClientArea(ByVal hWnd As IntPtr, ByRef pMarInset As MARGINS) As Integer
    End Function
    <System.Runtime.InteropServices.DllImport("dwmapi.dll")>
    Public Shared Function DwmSetWindowAttribute(ByVal hwnd As IntPtr, ByVal attr As Integer, ByRef attrValue As Integer, ByVal attrSize As Integer) As Integer
    End Function
    <System.Runtime.InteropServices.DllImport("dwmapi.dll")>
    Public Shared Function DwmIsCompositionEnabled(ByRef pfEnabled As Integer) As Integer
    End Function
    <System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint:="CreateRoundRectRgn")>
    Private Shared Function CreateRoundRectRgn(ByVal nLeftRect As Integer, ByVal nTopRect As Integer, ByVal nRightRect As Integer, ByVal nBottomRect As Integer, ByVal nWidthEllipse As Integer, ByVal nHeightEllipse As Integer) As IntPtr
    End Function

    Public Structure MARGINS
        Public leftWidth As Integer
        Public rightWidth As Integer
        Public topHeight As Integer
        Public bottomHeight As Integer
    End Structure

    Protected Overrides ReadOnly Property CreateParams As CreateParams
        Get
            m_aeroEnabled = CheckAeroEnabled()
            Dim cp As CreateParams = MyBase.CreateParams
            If Not m_aeroEnabled Then cp.ClassStyle = cp.ClassStyle Or CS_DROPSHADOW
            Return cp
        End Get
    End Property

    Private Function CheckAeroEnabled() As Boolean
        If Environment.OSVersion.Version.Major >= 6 Then
            Dim enabled As Integer = 0
            DwmIsCompositionEnabled(enabled)
            Return If((enabled = 1), True, False)
        End If

        Return False
    End Function

    Protected Overrides Sub WndProc(ByRef m As Message)
        Select Case m.Msg
            Case WM_NCPAINT

                If m_aeroEnabled Then
                    Dim v = 2
                    DwmSetWindowAttribute(Me.Handle, 2, v, 4)
                    Dim margins As MARGINS = New MARGINS() With {
                        .bottomHeight = 1,
                        .leftWidth = 0,
                        .rightWidth = 0,
                        .topHeight = 0
                    }
                    DwmExtendFrameIntoClientArea(Me.Handle, margins)
                End If

            Case Else
        End Select

        MyBase.WndProc(m)
        If m.Msg = WM_NCHITTEST AndAlso CInt(m.Result) = HTCLIENT Then m.Result = CType(HTCAPTION, IntPtr)
    End Sub

    Private Sub PanelMove_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs) Handles PanelMove.MouseDown
        Drag = True
        MouseX = Cursor.Position.X - Me.Left
        MouseY = Cursor.Position.Y - Me.Top
    End Sub

    Private Sub PanelMove_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles PanelMove.MouseMove
        If Drag Then
            Me.Top = Cursor.Position.Y - MouseY
            Me.Left = Cursor.Position.X - MouseX
        End If
    End Sub

    Private Sub PanelMove_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs) Handles PanelMove.MouseUp
        Drag = False
    End Sub

    Private Sub PanelHeader_MouseDown(sender As Object, e As MouseEventArgs) Handles PanelHeader.MouseDown
        Drag = True
        MouseX = Cursor.Position.X - Me.Left
        MouseY = Cursor.Position.Y - Me.Top
    End Sub

    Private Sub PanelHeader_MouseMove(sender As Object, e As MouseEventArgs) Handles PanelHeader.MouseMove
        If Drag Then
            Me.Top = Cursor.Position.Y - MouseY
            Me.Left = Cursor.Position.X - MouseX
        End If
    End Sub

    Private Sub PanelHeader_MouseUp(sender As Object, e As MouseEventArgs) Handles PanelHeader.MouseUp
        Drag = False
    End Sub

#End Region

#Region " Injector a Samp.dll "

#Region " Declare's "

    Declare Function OpenProcess Lib "kernel32" (ByVal dwDesiredAccess As UInt32, ByVal bInheritHandle As Int32, ByVal dwProcessId As UInt32) As IntPtr
    Declare Function CloseHandle Lib "kernel32" (ByVal hObject As IntPtr) As Int32
    Declare Function WriteProcessMemory Lib "kernel32" (ByVal hProcess As IntPtr, ByVal lpBaseAddress As IntPtr, ByVal buffer As Byte(), ByVal size As UInt32, ByRef lpNumberOfBytesWritten As IntPtr) As Boolean
    Declare Function GetProcAddress Lib "kernel32" (ByVal hModule As IntPtr, ByVal methodName As String) As IntPtr
    Declare Function GetModuleHandleA Lib "kernel32" (ByVal moduleName As String) As IntPtr
    Declare Function VirtualAllocEx Lib "kernel32" (ByVal hProcess As IntPtr, ByVal lpAddress As IntPtr, ByVal dwSize As IntPtr, ByVal flAllocationType As UInteger, ByVal flProtect As UInteger) As IntPtr
    Declare Function CreateRemoteThread Lib "kernel32" (ByVal hProcess As IntPtr, ByVal lpThreadAttribute As IntPtr, ByVal dwStackSize As IntPtr, ByVal lpStartAddress As IntPtr, ByVal lpParameter As IntPtr, ByVal dwCreationFlags As UInteger, ByVal lpThreadId As IntPtr) As IntPtr
    Declare Function GetPrivateProfileStringA Lib "kernel32" (ByVal lpAppName As String, ByVal lpKeyName As String, ByVal lpDefault As String, ByVal lpReturnedString As System.Text.StringBuilder, ByVal nSize As Integer, ByVal lpFileName As String) As Integer
    Declare Function WritePrivateProfileStringA Lib "kernel32" (ByVal lpAppName As String, ByVal lpKeyName As String, ByVal lpString As String, ByVal lpFileName As String) As Integer

#End Region


    Public Function Injected(ByVal Nickname As String, ByVal IPadress As String, ByVal PortAdress As String)
        Dim Args As String = "-c  -n " & Nickname & " -h " & IPadress & " -p " & PortAdress & " -z"

        ' If Process.GetProcessesByName(IO.Path.GetFileNameWithoutExtension(EXE_File)).Length > 0 Then
        ' For Each proc As Process In Process.GetProcessesByName(IO.Path.GetFileNameWithoutExtension(EXE_File))
        '   proc.Kill()
        '  Next
        '   End If

        Dim p As New Process
        p.StartInfo.WorkingDirectory = IO.Path.GetDirectoryName(EXE_File)
        p.StartInfo.FileName = IO.Path.GetFileName(EXE_File)
        p.StartInfo.Arguments = Args
        p.Start()

        If CreateRemoteThread(p, DLL_File) Then
            InicioPuginsDir()
            Return True
        Else
            Return False
        End If
    End Function

    Public Function InjectedComandline(ByVal Args As String)
        ' If Process.GetProcessesByName(IO.Path.GetFileNameWithoutExtension(EXE_File)).Length > 0 Then
        ' For Each proc As Process In Process.GetProcessesByName(IO.Path.GetFileNameWithoutExtension(EXE_File))
        '   proc.Kill()
        '  Next
        '   End If

        Dim p As New Process
        p.StartInfo.WorkingDirectory = IO.Path.GetDirectoryName(EXE_File)
        p.StartInfo.FileName = IO.Path.GetFileName(EXE_File)
        p.StartInfo.Arguments = Args
        p.Start()


        If CreateRemoteThread(p, DLL_File) Then
            Return True
        Else
            Return False
        End If
    End Function

    Function CreateRemoteThread(ByVal procToBeInjected As Process, ByVal sDllPath As String) As Boolean
        Dim lpLLAddress As IntPtr = IntPtr.Zero
        Dim hndProc As IntPtr = OpenProcess(&H2 Or &H8 Or &H10 Or &H20 Or &H400, 1, CUInt(procToBeInjected.Id))
        If hndProc = IntPtr.Zero Then
            Return False
        End If
        lpLLAddress = GetProcAddress(GetModuleHandleA("kernel32.dll"), "LoadLibraryA")
        If lpLLAddress = CType(0, IntPtr) Then
            Return False
        End If
        Dim lpAddress As IntPtr = VirtualAllocEx(hndProc, CType(Nothing, IntPtr), CType(sDllPath.Length, IntPtr), CUInt(&H1000) Or CUInt(&H2000), CUInt(&H40))
        If lpAddress = CType(0, IntPtr) Then
            Return False
        End If
        Dim bytes As Byte() = System.Text.Encoding.ASCII.GetBytes(sDllPath)
        Dim ipTmp As IntPtr = IntPtr.Zero
        WriteProcessMemory(hndProc, lpAddress, bytes, CUInt(bytes.Length), ipTmp)
        If ipTmp = IntPtr.Zero Then
            Return False
        End If
        Dim ipThread As IntPtr = CreateRemoteThread(hndProc, CType(Nothing, IntPtr), IntPtr.Zero, lpLLAddress, lpAddress, 0, CType(Nothing, IntPtr))
        If ipThread = IntPtr.Zero Then
            Return False
        End If
        Return True
    End Function

    Public Function InjectDLL(ByVal ProcessName As String, ByVal sDllPath As String) As Boolean
        Dim p As Process() = Process.GetProcessesByName(ProcessName)
        If p.Length <> 0 Then
            If Not CreateRemoteThread(p(0), sDllPath) Then
                If p(0).MainWindowHandle <> CType(0, IntPtr) Then
                    CloseHandle(p(0).MainWindowHandle)
                End If
                Return False
            End If
            Return True
        End If
        Return False
    End Function

#End Region

#Region " Funcstions "

    Public Function GetName() As String
        Dim readValue = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\SAMP", "PlayerName", Nothing)
        Return readValue
    End Function

    Public Function GameStart() As String
        Dim readValue = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\SAMP", "gta_sa_exe", Nothing)
        Return readValue
    End Function

    Public Function AddPlayers(Optional ByVal Player As String = "", _
                     Optional ByVal Lvl As String = "", Optional ByVal PingVal As String = "") As Boolean
        Dim lvi As New ListViewItem(Player)
        lvi.SubItems.Add(Lvl)
        lvi.SubItems.Add(PingVal)
        ListView1.Items.Add(lvi)
        'MsgBox(Player, Lvl, Ping)
        Return True
    End Function

    Public Function Modificado(ByVal ItemIdesx As Integer, ByVal Gamemode As String, ByVal Lenguage As String) As Boolean
        listv.Items(ItemIdesx).SubItems(2).Text = Gamemode
        listv.Items(ItemIdesx).SubItems(3).Text = Lenguage
        Return True
    End Function

    Public Function IsConnectionAvailable() As Boolean
        Dim objUrl As New System.Uri("http://www.google.com/")
        Dim objWebReq As System.Net.WebRequest
        objWebReq = System.Net.WebRequest.Create(objUrl)
        Dim objResp As System.Net.WebResponse
        Try

            objResp = objWebReq.GetResponse
            objResp.Close()
            objWebReq = Nothing
            Return True
        Catch ex As Exception
            objWebReq = Nothing
            Return False
        End Try
    End Function

    Public Function agregado(Optional ByVal Hostname As String = "", _
                       Optional ByVal Mode As String = "", Optional ByVal Language As String = "", Optional ByVal IpAdress As String = "", Optional ByVal Port As String = "") As Boolean

        Dim numAleatorio As New Random()
        Dim valorAleatorio As Integer = numAleatorio.Next(180, 500)

        Dim GetPingAsyc As Integer = 0 'MedidorpingSync(IpAdress)

        ' If IsConnectionAvailable() = True Then
        GetPingAsyc = valorAleatorio
        ' End If

        Dim lvi As New ListViewItem(Hostname)
        lvi.SubItems.Add(GetPingAsyc)
        ' lvi.SubItems.Add(Mode)
        ' lvi.SubItems.Add(Language)
        lvi.SubItems.Add(IpAdress)
        lvi.SubItems.Add(Port)
        listv.Items.Add(lvi)
        Return True
    End Function

#End Region

    Public Sub Inicio()
        AnimaStatusBar1.ColorBackground = EtherealSeperator1.ForeColor
        ThirteenTextBox1.Text = GetName()
        StartListaAsyc()
    End Sub

    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        LoadColor()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        On Error Resume Next
        ProcessElevation.Kanka()

        Dim arguments As String() = Environment.GetCommandLineArgs()

        If arguments.Count > 1 Then
            If arguments(1) = "/Connect" Then
                Dim ResultadoDeInjectar As Boolean = InjectedComandline(arguments(2))
                If arguments(3) = "/LoadSpecials" Then
                    Dim SpecialsLoad As Boolean = SpecialOpcionsInicializacion()
                    'MsgBox(arguments(3))
                    LoadPlugins()
                    End
                End If
                End
            End If
        End If

        Dim Welcome As String = vbNewLine & vbNewLine & "------------------------------------------------------------------------------------------------------------------------------" & vbNewLine & _
            "[SA-MP | Remastered ] - By S4Lsalsoft | version 1.0.0.0 beta" & vbNewLine & _
            "Developer: Aincrad | Destroyer" & vbNewLine & _
            "Beta Tester: Baironn" & vbNewLine & _
            "Discord>>> Destroyer#3527 | baironnn#0600" & vbNewLine & _
            "E-mail: S4Lsalsoft@gmail.com" & vbNewLine & _
            ""
        WriteLog(Welcome, InfoType.Information)
        StartColorSetting()

        Timer2.Enabled = True

    End Sub

    Public Sub StartSandboxie(ByVal Args As String)
        Dim p As New Process
        p.StartInfo.WorkingDirectory = IO.Path.GetDirectoryName(SandboxieDir)
        p.StartInfo.FileName = IO.Path.GetFileName(SandboxieDir)
        p.StartInfo.Arguments = LauncherDir & " " & Args
        p.Start()
    End Sub

    Dim Iniciador As Integer = 0

    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        Iniciador += 1
        If Iniciador = 100 Then
            AnimaStatusBar1.Text = "Loading List of Favorite Servers"
            Inicio()
            Timer2.Enabled = False
        End If
    End Sub

#Region " Listar Favoritos "

    Dim Terminate As Boolean = False

    Private Sub MonitorSquery_Tick(sender As Object, e As EventArgs) Handles MonitorSquery.Tick
        If Terminate = True Then
            AnimaStatusBar1.ColorBackground = EtherealSeperator1.ForeColor
            AnimaStatusBar1.Text = "Loaded List, Starting Engine [Request Query] In Background (bug in beta version)"
            MonitorSquery.Stop()
            MonitorSquery.Enabled = False
        End If
    End Sub

    Public Sub StartListaAsyc()
        Dim tsk1 As New Task(ListarServers, TaskCreationOptions.LongRunning)
        tsk1.Start()
    End Sub

    Dim ListarServers As New Action(
  Sub()
      Try
          Using stream As System.IO.FileStream = File.Open(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\GTA San Andreas User Files\SAMP\USERDATA.DAT", FileMode.Open, FileAccess.Read)

              Using reader = New BinaryReader(stream)

                  If stream.Length >= 16 Then
                      Dim samp = New String(reader.ReadChars(4))

                      If samp = "SAMP" Then

                          If reader.ReadUInt32 = 1UI Then
                              Dim sc = reader.ReadInt32
                              For i = 0 To sc - 1
                                  Dim ip As String = Encoding.[Default].GetString(reader.ReadBytes(reader.ReadInt32))
                                  Dim port As UShort = (reader.ReadUInt32)
                                  Dim cn As String = Utils.GuessedStringEncoding(reader.ReadBytes(reader.ReadInt32))
                                  ' Dim cn As String = Encoding.[Default].GetString(reader.ReadBytes(reader.ReadInt32))
                                  Dim sp As String = Encoding.[Default].GetString(reader.ReadBytes(reader.ReadInt32))
                                  Dim rp As String = Encoding.[Default].GetString(reader.ReadBytes(reader.ReadInt32))
                                  ' MsgBox(ip & " | " & port & " | " & cn & " | " & sp & " | " & rp)
                                  Me.Invoke(Sub() agregado(cn, " ", " ", ip, port))
                                  ' MultipleQueriesWithSameQueryResultType(ip, port)
                              Next
                          End If
                      End If
                  End If
              End Using
          End Using
          ' GetServerdata()
      Catch ex As Exception

      End Try
      Terminate = True
  End Sub)


    Private Sub StrafeButton1_Click(sender As Object, e As EventArgs) Handles StrafeButton1.Click
        Dim SampIsReayd As Integer = ReadInteger(ProcessGame, &HA444A0)
        WriteLog(SampIsReayd, InfoType.Information)
        ' MsgBox(IsRunningAsLocalAdmin())

        ' Dim WindowedPlugDll As String = SampDllDir & "\" & "WindowedPlugin.dll"
        ' If InjectDLL(ProcessGame, WindowedPlugDll) Then
        '  WriteLog(Path.GetFileName(WindowedPlugDll) & " : Loaded successfully.", InfoType.Information)
        ' Else
        '  WriteLog(Path.GetFileName(WindowedPlugDll) & " : Error loading module, 'Dll EntryPoint' Load Error!", InfoType.Exception)
        ' End If
    End Sub

    Private Async Function GetPlayerInforQuery(ByVal serverIP As String, ByVal port As Integer) As Task
        Dim sampQuery = New SampQueryClient()
        Dim playerList = Await sampQuery.SendQueryAsync(Of PlayerList)(serverIP, port)

        If playerList.IsCompleted Then
            Dim filteredPlayerList = playerList.Players.Where(Function(p) p.Level > 5).OrderByDescending(Function(p) p.Level)

            For Each Players In filteredPlayerList
                GetPlayersInfoSub(Players.UserName, Players.Level, _
                                  Players.ID, Players.ping)
            Next
        End If
    End Function

    Public Sub GetPlayersInfoSub(Optional ByVal NameP As String = "", Optional ByVal LvelP As String = "", _
                                 Optional ByVal NameID As String = "", Optional ByVal NamePing As String = "")
        Dim lvi As New ListViewItem(NameP)
        lvi.SubItems.Add(LvelP)
        lvi.SubItems.Add(NamePing)
        ListView1.Items.Add(lvi)
    End Sub

    Private Async Function GetServerInforQuery(ByVal ip As String, ByVal Puerto As String) As task
        Dim ipEndList = New List(Of IPEndPoint)()
        Dim ipAddr As System.Net.IPAddress = IPAddress.Parse(ip)
        Dim ipEndPoint As System.Net.IPEndPoint = New IPEndPoint(ipAddr, Puerto)
        ipEndList.Add(ipEndPoint)

        Dim sampQuery = New SampQueryClient()
        Dim playerList = Await sampQuery.SendQueryAsync(Of PlayerList)(ip, Puerto)

        If playerList.IsCompleted Then
            Dim filteredPlayerList = playerList.Players.Where(Function(p) p.Level > 5).OrderByDescending(Function(p) p.Level)

            For Each Players In filteredPlayerList
                GetPlayersInfoSub(Players.UserName, Players.Level, _
                                  Players.ID, Players.ping)
            Next
        End If

        Dim Getinformations As List(Of String) = Nothing

        Dim serverInfoList = Await New SampQueryClient().SendQueryAsync(Of ServerInfo)(ipEndList)

        For Each server In serverInfoList
            If server.IsCompleted Then
                Dim DataServer As Boolean = Await Me.Invoke(Sub() GetInfoSub(ip, Puerto, server.HostName, _
                                                                             server.GameModeName, server.Language, _
                                                                             server.MaxPlayers, server.Players, _
                                                                             server.Password))
            End If
        Next

    End Function

    Private Sub ServerPropertiesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ServerPropertiesToolStripMenuItem.Click
        Dim item As ListViewItem = listv.SelectedItems(0)
        Dim GetProperties = GetServerProperties(item.SubItems(2).Text, item.SubItems(3).Text)
    End Sub

    Private Async Function GetServerProperties(ByVal ip As String, ByVal Puerto As String) As task
        Dim ipEndList = New List(Of IPEndPoint)()
        Dim ipAddr As System.Net.IPAddress = IPAddress.Parse(ip)
        Dim ipEndPoint As System.Net.IPEndPoint = New IPEndPoint(ipAddr, Puerto)
        ipEndList.Add(ipEndPoint)

        Dim Getinformations As List(Of String) = Nothing

        Dim serverInfoList = Await New SampQueryClient().SendQueryAsync(Of ServerInfo)(ipEndList)

        For Each server In serverInfoList
            If server.IsCompleted Then
                Dim DataServer As Boolean = Await Me.Invoke(Sub() GetInfoSub(ip, Puerto, server.HostName, _
                                                                             server.GameModeName, server.Language, _
                                                                             server.MaxPlayers, server.Players, _
                                                                             server.Password))
            End If
        Next

    End Function

    Public Sub GetServerPropertiesSub(Optional ByVal IpAdress As String = "", Optional ByVal Port As String = "", Optional ByVal Hostname As String = "", _
                       Optional ByVal Mode As String = "", Optional ByVal Language As String = "", _
                       Optional ByVal MaxPlayer As String = "", Optional ByVal Player As String = "", _
                       Optional ByVal Password As Boolean = False)

        ServerPropierties.hostlvl.Text = Hostname
        ServerPropierties.Modelvl.Text = IpAdress & ":" & Port
        ServerPropierties.gamemodelvl.Text = Mode
        ServerPropierties.Languagelvl.Text = Language
        ServerPropierties.Playerlvl.Text = Player & "/" & MaxPlayer
        ServerPropierties.ShowDialog()

    End Sub

    Public Sub GetInfoSub(Optional ByVal IpAdress As String = "", Optional ByVal Port As String = "", Optional ByVal Hostname As String = "", _
                       Optional ByVal Mode As String = "", Optional ByVal Language As String = "", _
                       Optional ByVal MaxPlayer As String = "", Optional ByVal Player As String = "", _
                       Optional ByVal Password As Boolean = False)
        ThirteenTextBox2.Text = IpAdress & ":" & Port
        Hostnamelvl.Text = Hostname
        modelvl.Text = Mode
        Languagelvl.Text = Language
        Playerslvl.Text = Player & "/" & MaxPlayer


        ' Timelvl.Text = Time
        ' Maplvl.Text = Map
        '  WebURLlvl.Text = WebURL
        ' Versionlvl.Text = Version

        If Password = True Then
            Panel4.BackgroundImage = My.Resources.Locked
        Else
            Panel4.BackgroundImage = My.Resources.open
        End If
    End Sub

#End Region

#Region " Ping Monitor Asyc "


    Private Function MedidorpingSync(ByVal ip As String) As String
        Try
            Dim sw As New Stopwatch
            If My.Computer.Network.IsAvailable() Then
                sw.Start()
                My.Computer.Network.Ping(ip)
                sw.Stop()
                MedidorpingSync = sw.ElapsedMilliseconds '& " ms"
            Else
                MedidorpingSync = 0 '"Request Timed out."
            End If
        Catch ex As Exception
            Return "0"
        End Try
    End Function



#End Region

#Region "Medidor de Ping New"

    Dim IpAddr As String

    Dim ValuePing As Integer

    Private Sub listv_MouseClick(sender As Object, e As EventArgs) Handles listv.MouseClick
        If listv.SelectedItems.Count > 0 Then
            Dim item As ListViewItem = listv.SelectedItems(0)
            PlayerContadorTIP = item.SubItems(2).Text
            PlayerContadorTPort = item.SubItems(3).Text
        End If
    End Sub

    Private Sub listv_SelectedIndexChanged(sender As Object, e As EventArgs) Handles listv.SelectedIndexChanged
        On Error Resume Next
        If listv.SelectedItems.Count > 0 Then
            InicioGrapt()
            Dim item As ListViewItem = listv.SelectedItems(0)
            IpAddr = item.SubItems(2).Text
            Dim GetServerinformations = GetServerInforQuery(item.SubItems(2).Text, item.SubItems(3).Text)
            ListView1.Items.Clear()
            PlayerContadorTIP = item.SubItems(2).Text
            PlayerContadorTPort = item.SubItems(3).Text
            Button5.PerformClick()
            ' PlayerTimer.Enabled = True
            '  Dim GetPlayerinformations = GetPlayerInforQuery(item.SubItems(2).Text, item.SubItems(3).Text)
            StartScanProcessAsyc()
        End If
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Dim GetPlayerinformations = GetPlayerInforQuery(PlayerContadorTIP, PlayerContadorTPort)
    End Sub

    Dim PlayerContadorT As Integer = 0
    Private Sub PlayerTimer_Tick(sender As Object, e As EventArgs) Handles PlayerTimer.Tick
        PlayerContadorT += 1
        If PlayerContadorT = 50 Then
            Dim GetPlayerinformations = GetPlayerInforQuery(PlayerContadorTIP, PlayerContadorTPort)
            PlayerContadorTIP = String.Empty
            PlayerContadorTPort = String.Empty
            PlayerContadorT = 0
            PlayerTimer.Stop()
            PlayerTimer.Enabled = False
        End If
    End Sub

    Private Sub listv_MouseDoubleClick(sender As Object, e As EventArgs) Handles listv.MouseDoubleClick
        On Error Resume Next
        If listv.SelectedItems.Count > 0 Then
            Dim item As ListViewItem = listv.SelectedItems(0)
            Dim GetPlayerinformations = GetPlayerInforQuery(item.SubItems(2).Text, item.SubItems(3).Text)
            Conect(ThirteenTextBox1.Text, item.SubItems(2).Text, item.SubItems(3).Text)
        End If
    End Sub

    Dim act As New Action(
   Sub()
       Try
           Do While True
               Display(MedidorpingSync(IpAddr))
           Loop
       Catch ex As Exception

       End Try
   End Sub)

    Public Sub StartScanProcessAsyc()
        Dim tsk As New Task(act, TaskCreationOptions.LongRunning)
        tsk.Start()
    End Sub

    Public Sub Display(ByVal Ping As String)
        '  MsgBox(IpAddr & "    " & Ping)
        ValuePing = Ping - 300
    End Sub

#End Region

#Region " Help "

    Private Sub HelpTopicsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles HelpTopicsToolStripMenuItem.Click
        Process.Start("https://wiki.sa-mp.com/")
    End Sub

    Private Sub OficialPageToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OficialPageToolStripMenuItem.Click
        Process.Start("http://www.sa-mp.com/")
    End Sub

    Private Sub AboutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AboutToolStripMenuItem.Click
        MsgBox("Create By S4Lsalsoft" & vbNewLine & "Developer : Destroyer" & vbNewLine & "Beta Tester : Baironn")
    End Sub

#End Region

#Region " StripMenuItem "

    Private Sub WebURLlvl_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles WebURLlvl.LinkClicked
        Process.Start(WebURLlvl.Text)
    End Sub

    Private Sub Sandboxtest()
        If SandboxieCheck.Checked = True Then
            Dim item As ListViewItem = listv.SelectedItems(0)
            ConectSandbox(ThirteenTextBox1.Text, item.SubItems(2).Text, item.SubItems(3).Text)
        End If
    End Sub

    Private Sub ConnectToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ConnectToolStripMenuItem.Click
        If listv.SelectedItems.Count > 0 Then
            Dim item As ListViewItem = listv.SelectedItems(0)
            Conect(ThirteenTextBox1.Text, item.SubItems(2).Text, item.SubItems(3).Text)
        Else
            If Not PlayerContadorTIP = String.Empty Then
                Conect(ThirteenTextBox1.Text, PlayerContadorTIP, PlayerContadorTPort)
            Else
                MsgBox("Please Select Server")
            End If
        End If
    End Sub

    Private Sub AddServerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddServerToolStripMenuItem.Click
        ServerAddFrm.ShowDialog()
    End Sub

    Private Sub DeleteServerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteServerToolStripMenuItem.Click
        Dim item As ListViewItem = listv.SelectedItems(0)
        item.Remove()
    End Sub

    Private Sub RefreshServerListToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RefreshServerListToolStripMenuItem.Click
        listv.Items.Clear()
        Inicio()
    End Sub

    Private Sub CopyServerInfoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyServerInfoToolStripMenuItem.Click
        Dim item As ListViewItem = listv.SelectedItems(0)
        Clipboard.Clear()
        Clipboard.SetText("Server Hostname : " & item.SubItems(0).Text & _
                          "IP: " & item.SubItems(2).Text & ":" & item.SubItems(3).Text)
    End Sub

    Public Sub ConectSandbox(ByVal Nickname As String, ByVal IPadress As String, ByVal PortAdress As String)
        If listv.SelectedItems.Count > 0 Then
            If My.Computer.FileSystem.FileExists(GameStart()) = True Then
                Dim Argumentossandbox As String = "/conect " & """" & "-c  -n " & Nickname & " -h " & IPadress & " -p " & PortAdress & " -z" & """"
                StartSandboxie(Argumentossandbox)
            Else
                WriteLog("gta_sa.exe was not found.", InfoType.Critical)
                ' MsgBox("gta_sa.exe No Encontrado")
            End If
        Else
            MsgBox("Please Select Server")
        End If
    End Sub

    Public Sub Conect(ByVal Nickname As String, ByVal IPadress As String, ByVal PortAdress As String)
        On Error Resume Next
        Dim OSIdenty As Integer = GTA_Samp_Windowed()

        If listv.SelectedItems.Count > 0 Then
            Dim Args As String = String.Empty
            If My.Computer.FileSystem.FileExists(GameStart()) = True Then
                If My.Computer.FileSystem.FileExists(DLL_D3D_File) = True Then
                    Args = "/Connect " & """" & "-c  -n " & Nickname & " -h " & IPadress & " -p " & PortAdress & " -z" & """"
                    LauncherComandlineAdmin(Args)
                    WriteLog("It detected 'd3d9.dll', Possible Sobeit or ENB Series. Acquiring Administrator Privileges to Start!", InfoType.Information)
                ElseIf WindowsmodeCheck.Checked = True Then
                    If OSIdenty = 7 Then
                        GoTo SaltoSampWindowe
                    Else
                        Dim args1 As String = Args & " /LoadSpecials"
                        LauncherComandlineAdmin(args1)
                    End If
                Else
SaltoSampWindowe:
                    Dim Result As Boolean = Injected(Nickname, IPadress, PortAdress)
                    If Result = True Then
                        SampWindowed.Enabled = WindowsmodeCheck.Checked
                        ' LoadSpecialOptions()
                        ' LoadPlugins()
                    Else
                        WriteLog("Failed to Start,Failed to inject SAMP.DLL", InfoType.Critical)
                    End If
                End If
            Else
                WriteLog("gta_sa.exe was not found.", InfoType.Critical)
                ' MsgBox("gta_sa.exe No Encontrado")
            End If
        Else
            MsgBox("Please Select Server")
        End If
    End Sub

    Public Function LauncherComandlineAdmin(ByVal Argumend As String) As Boolean
        Try
            Dim process As System.Diagnostics.Process = Nothing
            Dim processStartInfo As System.Diagnostics.ProcessStartInfo
            processStartInfo = New System.Diagnostics.ProcessStartInfo()
            processStartInfo.FileName = LauncherDir
            processStartInfo.Verb = "runas"

            processStartInfo.Arguments = Argumend
            processStartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal
            processStartInfo.UseShellExecute = True
            process = System.Diagnostics.Process.Start(processStartInfo)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function


    Public Function SpecialOpcionsInicializacion() As Boolean
        Dim OSIdenty As Integer = GTA_Samp_Windowed()
        If OSIdenty = 7 Then
            WriteLog("Loading Module for Windows 7.", InfoType.Information)
            Dim ProcID As Integer = 0
            ProcID = Get_Process_PID("gta_sa.exe")
            AppActivate(ProcID)
            My.Computer.Keyboard.SendKeys("%{ENTER}", True)
            Return True
        ElseIf OSIdenty = 8 Then
            WriteLog("Loading Module for Windows 8.", InfoType.Information)
            If Not My.Computer.FileSystem.FileExists(DLL_D3D_File) = True Then
                If Process.GetProcessesByName(ProcessGame).Length = 1 Then
                    Dim WindowedPlugDll As String = SampDllDir & "\" & "SAWindow.dll"
                    If My.Computer.FileSystem.FileExists(WindowedPlugDll) = True Then
                        'My.Computer.FileSystem.CopyFile(WindowedPlugDll, DLL_D3D_File)
                        Dim pr As Process() = Process.GetProcessesByName(ProcessGame)
                        If InjectDLL(ProcessGame, WindowedPlugDll) Then
                            WriteLog(Path.GetFileName(WindowedPlugDll) & " : Loaded successfully.", InfoType.Information)
                            Return True
                        Else
                            WriteLog(Path.GetFileName(WindowedPlugDll) & " : Error loading module, 'Dll EntryPoint' Load Error!", InfoType.Exception)
                        End If
                    Else
                        WriteLog("Plugin to Run Window Mode (WindowedPlugin.dll) was not found.", InfoType.Exception)
                    End If
                End If
            End If

        ElseIf OSIdenty = 10 Then
            WriteLog("Loading Module for Windows 10.", InfoType.Information)
            If Not My.Computer.FileSystem.FileExists(DLL_D3D_File) = True Then
                If Process.GetProcessesByName(ProcessGame).Length = 1 Then
                    Dim WindowedPlugDll As String = SampDllDir & "\" & "SAWindow.dll"
                    If My.Computer.FileSystem.FileExists(WindowedPlugDll) = True Then
                        'My.Computer.FileSystem.CopyFile(WindowedPlugDll, DLL_D3D_File)
                        Dim pr As Process() = Process.GetProcessesByName(ProcessGame)
                        If InjectDLL(ProcessGame, WindowedPlugDll) Then
                            WriteLog(Path.GetFileName(WindowedPlugDll) & " : Loaded successfully.", InfoType.Information)
                            Return True
                        Else
                            WriteLog(Path.GetFileName(WindowedPlugDll) & " : Error loading module, 'Dll EntryPoint' Load Error!", InfoType.Exception)
                        End If
                    Else
                        WriteLog("Plugin to Run Window Mode (WindowedPlugin.dll) was not found.", InfoType.Exception)
                    End If
                End If
            End If
        End If

        Return False
    End Function

#End Region

#Region " Grap "

    Dim MIARRAY As ArrayList
    Dim MIX As Integer
    Dim MIRANDOM As Random

    Dim TEXTO As Integer = 5
    Dim FUENTE As Font
    Dim DIBUJO As Graphics
    Dim VALOR As Integer
    Dim RECTANGULO As Rectangle

    Public Sub InicioGrapt()
        Me.DoubleBuffered = True

        MIARRAY = New ArrayList

        For I = 0 To 59
            MIARRAY.Add(0)
        Next

        DIBUJO = PictureBox1.CreateGraphics
        DIBUJO.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
        FUENTE = New Font("VERDANA", TEXTO, FontStyle.Bold)
        Timer1.Interval = 500
        Timer1.Start()
    End Sub

    Public Sub DrawGrgphic(ByVal ValuePoint As Integer, ByVal LineColor As Color)
        Dim NUEVO As Integer = ValuePoint
        Dim TRAZO = New Pen(LineColor, 1)
        MIRANDOM = New Random 'SIMULAMOS LAS LECTURAS CON UN NUMERO ALEATORIO
        NUEVO = MIRANDOM.Next(PictureBox1.Height / 3, PictureBox1.Height * 2 / 3) 'VALORES MINIMO Y MAXIMO ESPERADOS

        'LabelVALOR.Text = NUEVO 'PRESENTA EL VALOR

        MIARRAY.Add(NUEVO) 'AÑADIMOS LA NUEVA LECTURA AL ARRAY

        If MIARRAY.Count > 60 Then 'SI YA HAY 60 ELIMINA EL MAS ANTIGUO
            MIARRAY.RemoveAt(0)
            PictureBox1.Refresh() 'BORRA EL PICTUREBOX
            DIBUJO.DrawLine(Pens.Gray, 0, CInt(PictureBox1.Height / 3), PictureBox1.Width, CInt(PictureBox1.Height / 3)) 'ESCALA MAXIMO
            DIBUJO.DrawLine(Pens.Gray, 0, CInt(PictureBox1.Height * 2 / 3), PictureBox1.Width, CInt(PictureBox1.Height * 2 / 3)) 'ESCALA MINIMO
        End If

        MIX = 0 'PARA QUE EMPIECE A DIBUJAR A PARTIR DE X=0

        For I = 0 To MIARRAY.Count - 2 'DIBUJA TODAS LAS LECTURAS QUE HAY EN EL ARRAY
            VALOR = MIARRAY(I) 'LECTURA DE DATOS

            '  If CheckBoxBARRAS.Checked Then 'DIBUJA BARRAS
            'RECTANGULO = New Rectangle(MIX, PictureBox1.Height - VALOR - TEXTO, 3, VALOR) 'RECTANGULO DE LA GRAFICA DE BARRAS
            '   DIBUJO.FillRectangle(Brushes.Blue, RECTANGULO)
            '   End If
            '   If CheckBoxLINEAS.Checked Then 'DIBUJA LINEAS
            DIBUJO.DrawLine(TRAZO, MIX, PictureBox1.Height - VALOR - TEXTO, MIX + 10, PictureBox1.Height - MIARRAY(I + 1) - TEXTO)
            '   End If
            '  If CheckBoxTEXTO.Checked Then 'ESCRIBE EL VALOR
            '  DIBUJO.DrawString(VALOR, FUENTE, Brushes.Red, MIX - TEXTO, PictureBox1.Height - VALOR - TEXTO * 2)
            '  End If

            MIX += 10 'AVANZA 10 PIXELES(PROPORCIONALIDAD CON PICTUREBOX.WIDTH)
        Next

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        On Error Resume Next
        If listv.SelectedItems.Count > 0 Then
            Dim item As ListViewItem = listv.SelectedItems(0)

            item.SubItems(1).Text = ValuePing
            If ValuePing > 250 Then
                DrawGrgphic(ValuePing, Color.Red)
            ElseIf ValuePing < 250 And ValuePing > 150 Then
                DrawGrgphic(ValuePing, Color.Yellow)
            ElseIf ValuePing < 150 Then
                DrawGrgphic(ValuePing, Color.Lime)
            End If
        End If
    End Sub

#End Region

#Region " BOTONES "

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If listv.SelectedItems.Count > 0 Then
            Dim item As ListViewItem = listv.SelectedItems(0)
            Conect(ThirteenTextBox1.Text, item.SubItems(2).Text, item.SubItems(3).Text)
        Else
            If Not PlayerContadorTIP = String.Empty Then
                Conect(ThirteenTextBox1.Text, PlayerContadorTIP, PlayerContadorTPort)
            Else
                MsgBox("Please Select Server")
            End If
        End If
    End Sub

#End Region

#Region " setting UI "

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        PanelOpenT.Enabled = True
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        PanelCloseT.Enabled = True
    End Sub

    Dim Velocidad As Integer = 10

    Dim OpenCount As Integer = 0

    Private Sub PanelOpenT_Tick(sender As Object, e As EventArgs) Handles PanelOpenT.Tick
        OpenCount += Velocidad
        If OpenCount = 430 Then
            Button3.Visible = True
            OpenCount = 0
            PanelOpenT.Stop()
            PanelOpenT.Enabled = False
        Else
            SettingPanel.Width = OpenCount
        End If
    End Sub

    Dim CloseCount As Integer = 420

    Private Sub PanelCloseT_Tick(sender As Object, e As EventArgs) Handles PanelCloseT.Tick
        CloseCount -= Velocidad
        If CloseCount = -10 Then
            Button3.Visible = False
            CloseCount = 420
            PanelCloseT.Stop()
            PanelCloseT.Enabled = False
        Else
            SettingPanel.Width = CloseCount
        End If
    End Sub

#End Region

#Region " Samp Windowed "

    Dim intentos As Integer = 0
    Dim TiempoEspera As Integer = 0

    Private Sub SampWindowed_Tick(sender As Object, e As EventArgs) Handles SampWindowed.Tick
        On Error Resume Next
        TiempoEspera += 1
        If TiempoEspera = 500 Then
            Dim ProcID As Integer = Get_Process_PID("gta_sa.exe")
            AppActivate(ProcID)
            My.Computer.Keyboard.SendKeys("%{ENTER}", True)
            intentos += 1
            If intentos = 2 Then
                SampWindowed.Stop()
                SampWindowed.Enabled = False
            Else
                TiempoEspera = 0
            End If
        End If
    End Sub

#End Region

#Region " Color System Changer "

    Private Sub StartColorSetting()
        AbrirFormEnPanel(c1, PanelFormularios)

    End Sub

    Dim c1 As New customColorPicker()

    Private Sub AbrirFormEnPanel(ByVal Formhijo As Object, ByVal panelh As Object)
        Dim fh As Form = TryCast(Formhijo, Form)
        Dim pn As Panel = TryCast(panelh, Panel)
        fh.TopLevel = False
        fh.FormBorderStyle = FormBorderStyle.None
        fh.Dock = DockStyle.Fill
        panelh.Controls.Add(fh)
        panelh.Tag = fh
        fh.Show()
        fh.Update()
        fh.Refresh()
    End Sub


    Private Sub ColorGui_Tick(sender As Object, e As EventArgs) Handles ColorGui.Tick
        On Error Resume Next
        If PanelFormularios.Visible = True Then
            Dim mycolor As Color = c1.Color()
            Dim ItemS As String = ThirteenComboBox1.SelectedItem
            If ItemS = "Background" Then
                BackColorEx = mycolor
                ThirteenForm1.BackColor = BackColorEx
                ThirteenTextBox1.BackColor = BackColorEx
                My.Settings.ColorMain = BackColorEx
                My.Settings.Save()
                ' ThirteenForm1.Update()
                ' ThirteenForm1.Refresh()
                'ThirteenForm1.Invalidate()
            ElseIf ItemS = "Linebase" Then
                LineColorEx = mycolor
                EtherealSeperator1.ForeColor = LineColorEx
                ThirteenForm1.AccentColor = LineColorEx
                AscButton_Big1.GlowColor = LineColorEx
                AnimaStatusBar1.ColorBackground = LineColorEx
                My.Settings.LineColor = LineColorEx
                My.Settings.Save()
            ElseIf ItemS = "Setting Dialog" Then
                SettingColor = mycolor
                NsGroupBox1.BackColor = SettingColor
                SettingPanel.BackColor = SettingColor
                My.Settings.SettingC = SettingColor
                My.Settings.Save()
            ElseIf ItemS = "Fore Color" Then
                ForeColorEX = mycolor
                NsGroupBox1.ForeColor = ForeColorEX
                SettingPanel.ForeColor = ForeColorEX
                ThirteenForm1.ForeColor = ForeColorEX
                ThirteenTextBox1.ForeColor = ForeColorEX
                My.Settings.ForeC = ForeColorEX
                My.Settings.Save()
            End If
        End If
        MenuStripZ1.BackColor = Color.FromArgb(28, 28, 28)
    End Sub

    Private Sub ThirteenButton1_Click(sender As Object, e As EventArgs) Handles ThirteenButton1.Click
        If ThirteenButton1.Text = "Open" Then
            ThirteenButton1.Text = "Close"
            PanelFormularios.Size = New Size(250, 202)
            PanelFormularios.Visible = True
        ElseIf ThirteenButton1.Text = "Close" Then
            ThirteenButton1.Text = "Open"
            PanelFormularios.Size = New Size(250, 0)
            PanelFormularios.Visible = False
        End If
    End Sub

    Private Sub ThirteenComboBox1_TabIndexChanged(sender As Object, e As EventArgs) Handles ThirteenComboBox1.TabIndexChanged
        If ColorGui.Enabled = True Then
            ColorGui.Enabled = False
        Else
            ColorGui.Enabled = True
        End If
    End Sub

    Private Sub ThirteenButton2_Click(sender As Object, e As EventArgs) Handles ThirteenButton2.Click
        My.Settings.ColorMain = Color.FromArgb(28, 28, 28)
        My.Settings.LineColor = Color.FromArgb(255, 103, 26)
        My.Settings.SettingC = Color.FromArgb(81, 81, 81)
        My.Settings.ForeC = Color.White
        My.Settings.Save()
        LoadColor()
    End Sub

#End Region

#Region " Monitor.sacnr API "

    <Serializable()>
    Public NotInheritable Class MyType
        Public Property ServerID As String
        Public Property IP As String
        Public Property Port As Integer
        Public Property Hostname As String
        Public Property Gamemode As String
        Public Property Language As String
        Public Property Map As String
        Public Property MaxPlayers As Integer
        Public Property Players As Integer
        Public Property Version As String
        Public Property Password As Integer
        Public Property Time As String
        Public Property WebURL As String
        Public Property Rank As Integer
        Public Property AvgPlayers As String
        Public Property HostedTab As Integer
        Public Property LastUpdate As String
        Public Property TotalServers As Integer
        Public Sub New()
        End Sub

        Public Overrides Function ToString() As String
            Return New JavaScriptSerializer().Serialize(Me).ToString
        End Function
    End Class

    Public Sub GetServerdata(ByVal IpAdress As String, ByVal PortAdress As String)
        Dim WebClient = New WebClient
        AddHandler WebClient.DownloadStringCompleted, AddressOf webClient_DownloadStringCompleted
        WebClient.DownloadStringAsync(New Uri("http://monitor.sacnr.com/api/?IP=" & IpAdress & "&Port=" & PortAdress & "&Action=info"))
    End Sub

    Public Sub webClient_DownloadStringCompleted(ByVal sender As Object, ByVal e As DownloadStringCompletedEventArgs)
        On Error Resume Next

        Dim JsonCode As String = e.Result

        If Not JsonCode = "" Then
            ' Dim obj As MyType = Newtonsoft.Json.JsonConvert.DeserializeObject(Of MyType)(JsonCode)
            ' Dim obj1 As MyType = New JavaScriptSerializer().Deserialize(Of MyType)(e.Result)

            Dim DataDesserialize As List(Of String) = Nothing
            Dim regex As Regex = New Regex("\"".*?\""")

            Dim vectoraux = regex.Matches(JsonCode)

            For Each item As Match In vectoraux
                DataDesserialize.Add(item.Value.ToString)
            Next

            Dim ServerID As String = DataDesserialize(2)
            Dim IP As String = DataDesserialize(4)
            Dim Port As Integer = DataDesserialize(6)
            Dim Hostname As String = DataDesserialize(8)
            Dim Gamemode As String = DataDesserialize(10)
            Dim Language As String = DataDesserialize(12)
            Dim Map As String = DataDesserialize(14)
            Dim MaxPlayers As Integer = DataDesserialize(16)
            Dim Players As Integer = DataDesserialize(18)
            Dim Version As String = DataDesserialize(20)
            Dim Password As Integer = DataDesserialize(22)
            Dim Time As String = DataDesserialize(24)
            Dim WebURL As String = DataDesserialize(26)
            Dim Rank As Integer = DataDesserialize(28)
            Dim AvgPlayers As String = DataDesserialize(30)
            Dim HostedTab As Integer = DataDesserialize(32)
            Dim LastUpdate As String = DataDesserialize(34)
            Dim TotalServers As Integer = DataDesserialize(36)
            ' MsgBox(ServerID)
            ServerIDlvl.Text = ServerID
            ThirteenTextBox2.Text = IP & ":" & Port
            Playerslvl.Text = Players & "/" & MaxPlayers
            modelvl.Text = Gamemode
            Hostnamelvl.Text = Hostname
            Timelvl.Text = Time
            Maplvl.Text = Map
            WebURLlvl.Text = WebURL
            Versionlvl.Text = Version
            Languagelvl.Text = Language
            If Password = 0 Then
                Panel4.BackgroundImage = My.Resources.open
            Else
                Panel4.BackgroundImage = My.Resources.Locked
            End If
            ' Modificado(LItem, obj.Gamemode, obj.Players & "/" & obj.MaxPlayers)
        End If

    End Sub

#End Region

#Region " CenterForm function "

    Function CenterForm(ByVal Form_to_Center As Form, ByVal Form_Location As Point) As Point
        Dim FormLocation As New Point
        FormLocation.X = (Me.Left + (Me.Width - Form_to_Center.Width) / 2) ' set the X coordinates.
        FormLocation.Y = (Me.Top + (Me.Height - Form_to_Center.Height) / 2) ' set the Y coordinates.
        Return FormLocation ' return the Location to the Form it was called from.
    End Function

    Function PosicionateInControl(ByVal Form_to_Center As Form, ByVal Form_Location As Point) As Point
        Dim FormLocation As New Point
        FormLocation.X = (Button4.Left + (Button4.Width - Form_to_Center.Width) / 2) ' set the X coordinates.
        FormLocation.Y = (Button4.Top + (Button4.Height - Form_to_Center.Height) / 2) ' set the Y coordinates.
        Return FormLocation ' return the Location to the Form it was called from.
    End Function

#End Region

#Region " Plugins "

    Dim PluginsDll As List(Of String) = Nothing

    Private Sub InicioPuginsDir()
        WriteLog("Installing pre-game hooks...", InfoType.Information)

        If Not My.Computer.FileSystem.DirectoryExists(PluginsDir) = True Then
            My.Computer.FileSystem.CreateDirectory(PluginsDir)
        End If

        WriteLog("Working directory: " & PluginsDir, InfoType.Information)

        Try
            For Each archivos As String In My.Computer.FileSystem.GetFiles(PluginsDir, FileIO.SearchOption.SearchAllSubDirectories, "*.dll")
                If archivos.Count > 0 Then
                    PluginsDll.Add(archivos)
                End If
            Next
        Catch ex As Exception
            WriteLog("Error : " & ex.Message, InfoType.Critical)
        End Try
    End Sub

    Private Sub LoadPlugins()
        Try
            If PluginsDll.Count > 0 Then
                For Each PluginDll As String In PluginsDll
                    WriteLog("Loading script '" & PluginDll & "'...", InfoType.Information)
                    If InjectDLL(ProcessGame, PluginDll) Then
                        WriteLog(Path.GetFileName(PluginDll) & " : Loaded successfully.", InfoType.Information)
                    Else
                        WriteLog(Path.GetFileName(PluginDll) & " : Error loading module, 'Dll EntryPoint' Load Error!", InfoType.Exception)
                    End If
                Next
            End If
        Catch ex As Exception
            WriteLog("Error : " & ex.Message, InfoType.Critical)
        End Try
    End Sub

#End Region

    Private Sub SandboxieCheck_CheckedChanged(sender As Object, e As EventArgs) Handles SandboxieCheck.CheckedChanged

        If Not My.Computer.FileSystem.FileExists(SandboxieDir) = True Then
            SandboxieCheck.Checked = False
            MsgBox("Sandboxie not found On your PC, Please Install!")
        End If

    End Sub

    Private Sub PanelBanner_Tick(sender As Object, e As EventArgs) Handles PanelBanner.Tick
        If Not Hostnamelvl.Text = "Host" Then
            PictureBox2.Visible = False
            PanelBanner.Enabled = False
        End If
    End Sub


    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If Panel6.Height = 0 Then
            Panel6.Height = 390
        ElseIf Panel6.Height = 390 Then
            Panel6.Height = 0
        End If
    End Sub


#Region " IP Changer"




#End Region

    
End Class


