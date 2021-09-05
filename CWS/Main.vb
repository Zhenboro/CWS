Public Class Main

    Private Sub Main_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Hide()
        CheckForIllegalCrossThreadCalls = False
        Init()
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
        End Try
    End Sub

End Class
