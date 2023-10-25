Imports Microsoft.Win32

Public Module GetDotNetVersion
    'Based on https://docs.microsoft.com/en-us/dotnet/framework/migration-guide/how-to-determine-which-versions-are-installed?redirectedfrom=MSDN#net_d

    Public Function Get45PlusFromRegistry() As Integer
        Const subkey As String = "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\"

        Using ndpKey As RegistryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey)
            If ndpKey IsNot Nothing AndAlso ndpKey.GetValue("Release") IsNot Nothing Then
                Return ndpKey.GetValue("Release")    'returns the build number (see below) or 0 if the version is not one of those listed below
            Else
                'Key not found in registry
                Return 0
            End If
        End Using
    End Function

    ' Checking the version using >= will enable forward compatibility.
    Private Function CheckFor45PlusVersion(releaseKey As Integer) As String
        If releaseKey >= 528040 Then
            Return "4.8 or later"
        ElseIf releaseKey >= 461808 Then
            Return "4.7.2"
        ElseIf releaseKey >= 461308 Then
            Return "4.7.1"
        ElseIf releaseKey >= 460798 Then
            Return "4.7"
        ElseIf releaseKey >= 394802 Then
            Return "4.6.2"
        ElseIf releaseKey >= 394254 Then
            Return "4.6.1"
        ElseIf releaseKey >= 393295 Then
            Return "4.6"
        ElseIf releaseKey >= 379893 Then
            Return "4.5.2"
        ElseIf releaseKey >= 378675 Then
            Return "4.5.1"
        ElseIf releaseKey >= 378389 Then
            Return "4.5"
        End If
        ' This code should never execute. A non-null release key should mean
        ' that 4.5 or later is installed.
        Return False
    End Function
End Module