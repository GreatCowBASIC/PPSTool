'PPS Tool
'Copyright Peter Everett 2020
'
'This file Is part Of PPS Tool.

'PPS Tool Is free software: you can redistribute it And/Or modify
'it under the terms Of the GNU General Public License As published by
'the Free Software Foundation, either version 3 Of the License, Or
'(at your option) any later version.

'PPS Tool Is distributed In the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of
'MERCHANTABILITY Or FITNESS FOR A PARTICULAR PURPOSE.  See the
'GNU General Public License For more details.

'You should have received a copy Of the GNU General Public License
'along with PPS Tool.  If Not, see < http:  //www.gnu.org/licenses/>.

Imports System.IO

Public NotInheritable Class AboutBox1
    Public reloadRequired As Boolean = False    'whether the XML files nees to be reloaded
    Private Sub AboutBox1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Set the title of the form.
        Dim ApplicationTitle As String
        If My.Application.Info.Title <> "" Then
            ApplicationTitle = My.Application.Info.Title
        Else
            ApplicationTitle = System.IO.Path.GetFileNameWithoutExtension(My.Application.Info.AssemblyName)
        End If
        Me.Text = String.Format("About {0}", ApplicationTitle)
        ' Initialize all of the text displayed on the About Box.
        ' TODO: Customize the application's assembly information in the "Application" pane of the project 
        '    properties dialog (under the "Project" menu).
        Me.LabelProductName.Text = My.Application.Info.ProductName
        Me.LabelVersion.Text = String.Format("Version {0}", My.Application.Info.Version.ToString)
        Me.LabelManifestVer.Text = "XML file version " & xmlFileVersion 'manifestDate.ToShortDateString
        Me.LabelCopyright.Text = My.Application.Info.Copyright
        Me.LabelLicence.Text = "Released under GPL v3 licence"
        Me.TextBoxDescription.Text = My.Application.Info.Description
    End Sub

    Private Sub OKButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OKButton.Click
        Me.Close()
        If reloadRequired Then
            generate_device_list()
            reloadRequired = False
        End If
    End Sub

    Private Sub btUpdateClick(sender As Object, e As EventArgs) Handles btnUpdates.Click

        Dim latest As MCCVersion = checkForUpdateMC()
        Dim current As String = getCurrentXMLVersion()
        Dim exitCode As Integer = -1
        Dim downloadfolder As String = ""

        If current = latest.version Then
            MsgBox("Version in use is " & getCurrentXMLVersion() & vbCrLf & "Latest version is " & latest.version & vbCrLf & vbCrLf & "XML files are up to date", 64, "XML update check")
        Else
            'check whether .net framework 4.5 or greater is installed, as this is needed to run the automatic updater
            If Get45PlusFromRegistry() < 378389 Then
                'it's not available, therefore just show the information box
                MsgBox("Version in use is " & getCurrentXMLVersion() & vbCrLf & "Latest version is " & latest.version & vbCrLf & vbCrLf & "A newer version of the XML files is available.", 64, "XML update check")
            Else
                'it is available, therefore show the dialog with the option to automatically update
                If MsgBox("Version in use is " & getCurrentXMLVersion() & vbCrLf & "Latest version is " & latest.version & vbCrLf & vbCrLf & "A newer version of the XML files is available.  Download now? (replaces existing files)", 68, "XML update check") = vbYes Then
                    'Pick the output directory
                    Dim fld As New FolderBrowserDialog
                    With fld
                        .SelectedPath = xmlFolder
                        .Description = "Pick the folder to download the latest files to (any existing XML files will be overwritten)"
                        If .ShowDialog() <> vbCancel Then
                            downloadfolder = .SelectedPath
                            'Run the updater
                            Using pProcess As Process = New System.Diagnostics.Process
                                pProcess.StartInfo.FileName = "MCC Updater.exe"
                                pProcess.StartInfo.Arguments = """" & downloadfolder & """"
                                pProcess.StartInfo.UseShellExecute = True
                                pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal
                                pProcess.StartInfo.CreateNoWindow = True
                                pProcess.Start()
                                pProcess.WaitForExit()
                                exitCode = pProcess.ExitCode
                            End Using
                            Select Case exitCode
                                Case 0
                                    MsgBox("Files downloaded and extracted successfully." & vbCrLf & "Select the new location using the settings menu." & vbCrLf & "The version number may not be reported by PPS Tool until the next update is released.", MsgBoxStyle.Information)
                                    xmlFolder = downloadfolder
                                    reloadRequired = True
                                Case 1
                                    MsgBox("There was a problem accessing the folder for downloading the files into.", MsgBoxStyle.Critical)
                                Case 2
                                    MsgBox("The URI supplied at runtime Is Not valid", MsgBoxStyle.Critical, "Error 2")      'This should never happen because we don't pass the URI from PPS tool
                                Case 10
                                    MsgBox("A problem occurred while downloading the zip file", MsgBoxStyle.Critical, "Error 10")
                                Case 11
                                    MsgBox("The XML files could Not be extracted from the zip file", MsgBoxStyle.Critical, "Error 11")
                                Case 20
                                    MsgBox("Could not get MCC version from Microchip's website", MsgBoxStyle.Critical, "Error 20")
                                Case 21
                                    MsgBox("Could not determine the current version from Microchip's website", MsgBoxStyle.Critical, "Error 21")
                                Case 404
                                    MsgBox("The file could not be found on Microchip's website", MsgBoxStyle.Critical, "Error 404")
                                Case Else
                                    MsgBox("The update encountered a problem.  The error code was: " & exitCode, MsgBoxStyle.Critical, "Error" & exitCode)
                            End Select

                        End If

                    End With
                    fld.Dispose()
                End If
            End If
        End If
    End Sub

    Function readCurrentVersion()
        'NO LONGER REQUIRED
        'opens the version.txt file in the devices folder and reads the version
        Dim fn As String = xmlFolder & "\version.txt"

        'check that the file exists
        If File.Exists(fn) Then
            'open the file
            Return getCurrentXMLVersion(File.ReadAllText(fn))
            Exit Function
        Else
            'file not found
            Return "Not available (1)"
            Exit Function
        End If

    End Function



End Class
