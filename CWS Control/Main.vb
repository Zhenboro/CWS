Imports System.IO
Imports System.Net
Imports System.Runtime.InteropServices
Imports System.Text
Public Class Main
    Dim Host As String = ""
    Dim HostRootDirectory As String = "/public_html/CWS"
    Dim HostUser As String = ""
    Dim HostPassword As String = ""
    Public DIRCommons As String = "C:\Users\" & Environment.UserName & "\AppData\Local\CWS"
    Private Sub Main_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If My.Computer.FileSystem.DirectoryExists(DIRCommons) = False Then
            My.Computer.FileSystem.CreateDirectory(DIRCommons)
        End If
        If My.Computer.FileSystem.DirectoryExists(DIRCommons & "\Control") = True Then
            My.Computer.FileSystem.DeleteDirectory(DIRCommons & "\Control", FileIO.DeleteDirectoryOption.DeleteAllContents)
        End If
        My.Computer.FileSystem.CreateDirectory(DIRCommons & "\Control")
        IndexTheFiles()
    End Sub

    Sub IndexTheFiles()
        Try
            Dim request As FtpWebRequest = CType(WebRequest.Create(Host & HostRootDirectory & "/Users"), FtpWebRequest)
            request.Method = WebRequestMethods.Ftp.ListDirectory
            request.Credentials = New NetworkCredential(HostUser, HostPassword)
            Dim response As FtpWebResponse = CType(request.GetResponse(), FtpWebResponse)
            Dim responseStream As Stream = response.GetResponseStream()
            Dim reader As StreamReader = New StreamReader(responseStream)
            'Console.WriteLine(reader.ReadToEnd())
            'hare algo muy feo
            Dim lineas As String = reader.ReadToEnd()
            Dim TextBOS As New TextBox
            TextBOS.Text = lineas
            For Each linea As String In TextBOS.Lines
                ListBox1.Items.Add(linea)
            Next
            ListBox1.Items.Remove(".")
            ListBox1.Items.Remove("..")
            'no es mi estilo, pero me ha estado costando pensar.
            reader.Close()
            response.Close()
        Catch ex As Exception
            AddToLog("[IndexTheFiles@Main]Error: ", ex.Message, True)
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
    Private Sub BtnEditar_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            'descargar el fichero
            Dim remoteFilePath As String = Host & HostRootDirectory & "/Users/" & ListBox1.SelectedItem
            Dim localFilePath As String = DIRCommons & "\Control\" & ListBox1.SelectedItem
            My.Computer.Network.DownloadFile(remoteFilePath, localFilePath, HostUser, HostPassword)
            'abrirlo
            TextBox1.Text = GetIniValue("Status", "IsEnabled", localFilePath)
            TextBox2.Text = GetIniValue("Status", "IsReading", localFilePath)
            TextBox3.Text = GetIniValue("Startup", "CommandLine", localFilePath)
            TextBox4.Text = GetIniValue("Startup", "ArgumentLine", localFilePath)
            TextBox5.Text = GetIniValue("Commands", "OWN", localFilePath)
            TextBox6.Text = GetIniValue("Commands", "CMD", localFilePath)
            TextBox7.Text = LeerFicheroDesdeLinea(11, localFilePath)
        Catch ex As Exception
            AddToLog("[BtnEditar_Click@Main]Error: ", ex.Message, True)
        End Try
    End Sub
    Private Function LeerFicheroDesdeLinea(ByVal numeroLinea As Integer, ByVal nombreFichero As String) As String
        Dim fichero As New System.IO.FileInfo(nombreFichero)
        LeerFicheroDesdeLinea = ""
        If fichero.Exists Then
            Dim sr As System.IO.StreamReader
            Dim lineaActual As Integer = 1
            Try
                sr = New System.IO.StreamReader(fichero.FullName)
                While lineaActual < numeroLinea And Not sr.EndOfStream
                    sr.ReadLine()
                    lineaActual += 1
                End While
                LeerFicheroDesdeLinea = sr.ReadToEnd
            Catch ex As Exception
                MsgBox("No se pudo ejecutar la operación")
            Finally
                If sr IsNot Nothing Then
                    sr.Close()
                    sr.Dispose()
                End If
            End Try
        End If
    End Function
    Private Sub BtnEnviar_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Try
            Dim remoteFilePath As String = Host & HostRootDirectory & "/Users/" & ListBox1.SelectedItem
            Dim localFilePath As String = DIRCommons & "\Control\" & ListBox1.SelectedItem
            'guardar el fichero
            Dim content As String = "# CWS Private" &
                vbCrLf & "[Status]" &
                vbCrLf & "IsEnabled=" & TextBox1.Text &
                vbCrLf & "IsReading=" & TextBox2.Text &
                vbCrLf & "[Startup]" &
                vbCrLf & "CommandLine=" & TextBox3.Text &
                vbCrLf & "ArgumentLine=" & TextBox4.Text &
                vbCrLf & "[Commands]" &
                vbCrLf & "OWN=" & TextBox5.Text &
                vbCrLf & "CMD=" & TextBox6.Text &
                vbCrLf & "[Response]" &
                vbCrLf & ""
            If My.Computer.FileSystem.FileExists(localFilePath) Then
                My.Computer.FileSystem.DeleteFile(localFilePath)
            End If
            My.Computer.FileSystem.WriteAllText(localFilePath, content, False)
            'subirlo
            My.Computer.Network.UploadFile(localFilePath, remoteFilePath, HostUser, HostPassword)
            MsgBox("Fichero correctamente subido")
        Catch ex As Exception
            AddToLog("[BtnEnviar_Click@Main]Error: ", ex.Message, True)
        End Try
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        Label8.Text = "Target: " & ListBox1.SelectedItem
    End Sub
End Class
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