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

Public Class MainScreen

    Dim PPSInput, PPSOutput As New ArrayList
    Dim cmbEvents As Boolean = True

    Private Sub Form1_Load() Handles Me.Load

        'Read any command line arguments
        Dim arguments As String() = Environment.GetCommandLineArgs()
        Dim cmdPIC, GCBFileName As New String("")

        'Check for 2 command line arguments - file and PIC
        'If file is present, this overrides PIC
        For Each arg As String In arguments
            If arg.Substring(0, 5) = "file=" Then
                'A filename was passed as a command line argument
                GCBFileName = arg.Split("=")(1)
                'Extract the chip number from the GCB file.  Returns false if not found
                cmdPIC = getChipFromFile(GCBFileName)
                If cmdPIC <> "False" Then Exit For
            End If

            If arg.Substring(0, 4) = "PIC=" Then
                'A PIC chip was passed as a command line option
                Debug.Print(System.Reflection.MethodInfo.GetCurrentMethod().ToString & ": " & arg & " passed at command line")
                'a PIC number was passed on the command line so save it
                cmdPIC = arg.Split("=")(1)
                Exit For
            End If
        Next

        'Set up the form for the data
        Dim bs As New BindingSource
        'as we are potentially changing the combobox, disable the event handler
        cmbEvents = False
        'clear all the comboboxes ready for the new data
        With Me
            .cmbPICs.DataSource = Nothing
            .cmbInputPeripheral.DataSource = Nothing
            .cmbInputPin.DataSource = Nothing
            .cmbOutputPeripheral.DataSource = Nothing
            .cmbOutputPin.DataSource = Nothing
            PPSData.Reset()
        End With

        'Find the XML files and load into a list
        generate_device_list()
        'check whether any XML files were found and whether the list is populated
        If PICs.Count = 0 Then
            'no files found
            MsgBox("No XML files were found in " & xmlFolder, vbOKOnly + vbCritical, "Invalid XML folder")
            Exit Sub
        Else
            bs.DataSource = PICs
            With Me.cmbPICs
                .DisplayMember = "Key"
                .DataSource = bs

                'Check whether the dropdown needs to be changed - either to the PIC number supplied on the command line or the last used PIC
                If cmdPIC <> "" Then
                    'a PIC number was passed on the command line so use it
                    'check whether the PIC is in the dropdown list
                    If PICs.ContainsKey(cmdPIC) Then
                        Debug.Print(System.Reflection.MethodInfo.GetCurrentMethod().ToString & ": Command line PIC number is OK - changing dropdown")
                        'change the selection
                        .Text = cmdPIC
                        'save the setting for next time
                        With My.Settings
                            .PIC = cmdPIC
                            .Save()
                        End With
                        'Load the PPS settings
                        get_PPS()
                    Else
                        Debug.Print(System.Reflection.MethodInfo.GetCurrentMethod().ToString & ": Command line PIC (" & cmdPIC & ") is not in the list - reset dropdown")
                        .SelectedIndex = -1
                    End If

                    'If no command line argument was passed then see if there is a saved value to use
                ElseIf My.Settings.PIC <> "" Then
                    Debug.Print(System.Reflection.MethodInfo.GetCurrentMethod().ToString & ": Nothing passed at command line, using saved setting")
                    'change the selection
                    .Text = My.Settings.PIC
                    'manually get the info from the XML file
                    'there is no need to save the last PIC setting here, because we are using the current setting
                    get_PPS()
                Else
                    Debug.Print(System.Reflection.MethodInfo.GetCurrentMethod().ToString & ": No command line or saved setting found")
                    .SelectedIndex = -1
                End If

                'Update the file version variable
                xmlFileVersion = getCurrentXMLVersion()

            End With
        End If
        'no further changing of the combobox, so turn events back on
        cmbEvents = True

        'add empty columns to the datatable
        With PPSData.Columns
            .Add("PPSSetting")
            .Add("Comment")
            .Add("PICModule")
            .Add("PinName")
            .Add("Register")
        End With
        'set the default sorting
        PPSData.DefaultView.Sort = "PICModule ASC, PPSSetting ASC"

        'set the location of the template file
        templateFolder = Application.StartupPath

        'check the template file exists and is valid
        validateTemplate(templateFolder & "\OutputTemplate.txt")

    End Sub

    Private Sub btnInputAdd_Click(sender As Object, e As EventArgs) Handles btnInputAdd.Click
        Dim row(4)
        Dim register, comment, PICModule As String
        Dim setting As Byte
        'Check the dropdowns are not blank
        If cmbInputPeripheral.SelectedIndex >= 0 And cmbInputPin.SelectedIndex >= 0 Then
            'copy the current PPS input to the PPS config and update the textbox
            'example is CCP1PPS,RA1
            register = cmbInputPeripheral.Text & "PPS"
            'for bidirectional pins, the PPS register will need to be looked up in the modules list
            For Each itm In PICmodules
                If itm(0) = cmbInputPeripheral.Text And itm(3) <> Nothing Then
                    register = itm(3) & "PPS"
                End If
            Next
            setting = pinsList(cmbInputPin.Text)

            comment = "// " & cmbInputPin.Text & " > " & cmbInputPeripheral.Text
            PICModule = getModuleName(register.Substring(0, register.Length - 3))

            row(0) = register & " = 0x" & String.Format("{0:X4}", setting)
            row(1) = comment
            row(2) = PICModule
            row(3) = cmbInputPin.Text
            row(4) = cmbInputPeripheral.Text
            PPSData.Rows.Add(row)

            'check whether the PPS input is bidirectional
            If checkBiDi(cmbInputPeripheral.Text) Then
                'add the corresponding output as well
                register = cmbInputPin.Text & "PPS"
                setting = outputsList(cmbInputPeripheral.Text)
                comment = "// " & cmbInputPeripheral.Text & " > " & cmbInputPin.Text & " (bi-directional)"

                row(0) = register & " = 0x" & String.Format("{0:X4}", setting)
                row(1) = comment
                row(2) = PICModule
                row(3) = cmbInputPin.Text
                row(4) = cmbInputPeripheral.Text
                PPSData.Rows.Add(row)

                Me.staStatusText.Text = cmbInputPeripheral.Text & " also added as an output on the same pin"

            End If

            'peripheral specific checks.  Moved to seperate module 26/10/18
            chipSpecificChecks(PICModule, cmbInputPin.Text, cmbInputPeripheral.Text)

            'update the text in the output window
            updateOutputBox()
            'make the pin grey in the dropdown list

            'Redraw the graphic
            drawchip()
        End If
    End Sub
    Private Sub btnOutputAdd_Click(sender As Object, e As EventArgs) Handles btnOutputAdd.Click
        Dim row(4)
        Dim register, comment, PICmodule As String
        Dim setting As Byte
        'Check the dropdowns are not blank
        If cmbOutputPeripheral.SelectedIndex >= 0 And cmbOutputPin.SelectedIndex >= 0 Then
            'copy the current PPS output to the PPS config and update the textbox
            'example is RA5PPS,PWM5

            register = cmbOutputPin.Text & "PPS"
            setting = outputsList(cmbOutputPeripheral.Text)
            comment = "// " & cmbOutputPeripheral.Text & " > " & cmbOutputPin.Text
            PICmodule = getModuleName(cmbOutputPeripheral.Text)

            row(0) = register & " = 0x" & String.Format("{0:X4}", setting)
            row(1) = comment
            row(2) = PICmodule
            row(3) = cmbOutputPin.Text
            row(4) = cmbOutputPeripheral.Text
            PPSData.Rows.Add(row)

            'check whether the PPS output is bidirectional
            If checkBiDi(cmbOutputPeripheral.Text) Then
                'add the corresponding input as well
                register = cmbOutputPeripheral.Text & "PPS"
                'for bidirectional pins, the PPS register will need to be looked up in the modules list
                For Each itm In PICmodules
                    If itm(0) = cmbOutputPeripheral.Text And itm(3) <> Nothing Then
                        register = itm(3) & "PPS"
                    End If
                Next
                setting = pinsList(cmbOutputPin.Text)
                comment = "// " & cmbOutputPin.Text & " > " & cmbOutputPeripheral.Text & " (bi-directional)"

                row(0) = register & " = 0x" & String.Format("{0:X4}", setting)
                row(1) = comment
                row(2) = PICmodule
                row(3) = cmbOutputPin.Text
                row(4) = cmbOutputPeripheral.Text
                PPSData.Rows.Add(row)


                Me.staStatusText.Text = cmbOutputPeripheral.Text & " also added as an input on the same pin"

            End If

            'peripheral specific checks.  Added 16/09/18  Moved to seperate module 26/10/18
            chipSpecificChecks(PICmodule, cmbOutputPin.Text, cmbOutputPeripheral.Text)


            'update the text in the output window
            updateOutputBox()
            'Redraw the graphic
            drawchip()
        End If
    End Sub



    Private Sub resetOutput() Handles btnClear.Click
        'Clears the txtOutput box
        txtOutput.Clear()
        PPSData.Clear()
        'reset the diagram
        drawchip()
    End Sub

    Private Sub FileLocationsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FileLocationsToolStripMenuItem.Click
        If chooseXMLFolder() = vbCancel Then Exit Sub
        'reload the list of PICs and update the form
        Form1_Load()
    End Sub


    Sub updateOutputBox()
        Dim output As String
        'if no valid template file is found, don't update anything
        If validTemplate Then
            'update the text
        Else
            'do nothing
            Exit Sub
        End If

        'format the data
        output = formatOutput()

        'update the textbox
        txtOutput.Text = output

    End Sub

    Function formatOutput()

        Dim templateFile, formattedOutput, PPSTemplate As String
        Dim PPSSection As String = ""
        Dim prevModule As String = ""
        Dim PPSStart, PPSEnd As Integer
        templateFile = Application.StartupPath & "\OutputTemplate.txt"

        'formats the output in accordance with the template
        'syntax:
        '   [Sections]
        '   'Comments
        '   #Variables
        '   The variables between [PPS] and [PPSEnd] are repeated for each PPS setting

        'the template variable was already loaded when the template file was validated.
        'make a copy of it to manipulate here
        formattedOutput = template

        'Now take the template, and effectively do a find and replace on it

        'Add the headers to the template file
        formattedOutput = formattedOutput.Replace("#Header", "// Generated by PIC PPS Tool for GCBASIC")
        formattedOutput = formattedOutput.Replace("#ToolVersion", "// PIC PPS Tool version: " & Application.ProductVersion)
        formattedOutput = formattedOutput.Replace("#XMLVersion", "// PinManager data: " & xmlFileVersion)
        'Add a line indicating which chip the PPS output was generated for
        formattedOutput = formattedOutput.Replace("#PPSToolPart", cmbPICs.Text)



        'add the PPSLock/Unlock if the tick box is ticked
        If Me.chkLOCK.CheckState = CheckState.Checked Then
            formattedOutput = formattedOutput.Replace("#UNLOCKPPS", "UNLOCKPPS")
            formattedOutput = formattedOutput.Replace("#LOCKPPS", "LOCKPPS")
        Else
            formattedOutput = formattedOutput.Replace("#UNLOCKPPS" & vbCrLf, "")
            formattedOutput = formattedOutput.Replace("#LOCKPPS" & vbCrLf, "")
        End If

        'read the [PPS] section, excluding the tags
        PPSStart = InStr(formattedOutput, "[PPS]") + 6
        PPSEnd = InStr(formattedOutput, "[PPSEnd]") - 1
        PPSTemplate = formattedOutput.Substring(PPSStart, PPSEnd - PPSStart)
        'sort the PPSdata array according to module name
        Dim tmpDataView As New DataView(PPSData) With {
            .Sort = "PICModule"
        }

        For Each rw In tmpDataView.ToTable.Rows
            'Go through each PPS setting and replace each variable in the teomplate file
            Dim tempStr = PPSTemplate
            tempStr = tempStr.Replace("#PPSSetting", rw(0))
            tempStr = tempStr.Replace("#Comment", rw(1))
            'if the module name has not changed, don't include the module name
            If rw(2) = prevModule Then
                'remove the line
                tempStr = tempStr.Replace(StrDup(3, ">").Replace(">", indent) & "#ModuleName" & vbCrLf & StrDup(2, ">").Replace(">", indent), "")
            Else
                If prevModule = "" Then
                    tempStr = tempStr.Replace("#ModuleName", moduleSpecificCode("// Module: " & rw(2)))
                Else
                    tempStr = tempStr.Replace(StrDup(2, ">").Replace(">", indent) & "#ModuleName", moduleSpecificCode("// Module: " & rw(2)))
                End If
            End If

            'add this to the PPS settings section
            PPSSection += tempStr
            'save the name of the previous module for comparison next time
            prevModule = rw(2)
        Next
        'Insert the PPS section into the output
        formattedOutput = formattedOutput.Replace(PPSTemplate, PPSSection)
        'Remove the PPS start/end tags
        formattedOutput = formattedOutput.Replace("[PPS]", "")
        formattedOutput = formattedOutput.Replace("[PPSEnd]", "")

        Return formattedOutput

    End Function

    Private Sub chkLOCK_CheckedChanged(sender As Object, e As EventArgs) Handles chkLOCK.CheckedChanged

        'update the output
        updateOutputBox()

    End Sub

    Private Sub cmbPICs_Changed() Handles cmbPICs.SelectedIndexChanged

        'if the event handler flag is off, don't do anything
        If cmbEvents = False Then
            Exit Sub
        Else
            'Before changing anything, check that there isn't any output
            'If Me.gridOutput.RowCount <> 0 Then
            If txtOutput.Text <> "" Then
                'prompt the user for a decision
                If MsgBox("There are already some PPS settings in the output box.  If the chip is changed, these settings may not be compatible.  Do you want to clear the existing output", vbYesNo + vbQuestion, "Existing output") = vbYes Then
                    resetOutput()
                End If

            End If
            'clear all the lists
            'Reset the data lists
            inputsList = Nothing
            pinsList = Nothing
            remapPins = Nothing
            outputsList = Nothing
            PICmodules = Nothing
            pinGroupList = Nothing

            'find the XML file and load the data
            get_PPS()
            'Save the chosen PIC (even if there is no data for it)
            With My.Settings
                .PIC = cmbPICs.Text
                .Save()
            End With
        End If

    End Sub

    Private Sub btnCopy_Click(sender As Object, e As EventArgs) Handles btnCopy.Click
        'Copy the text output to the clipboard
        If txtOutput.Text.Length > 0 Then
            Clipboard.SetDataObject(txtOutput.Text)
            MsgBox("Output copied to clipboard", vbOKOnly + vbInformation, "Copy output to clipboard")
        Else
            MsgBox("No output to copy to clipboard", vbOKOnly + vbCritical, "Copy output to clipboard")
        End If
    End Sub

    Private Sub AboutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AboutToolStripMenuItem.Click
        'Shows the about box
        AboutBox1.ShowDialog()
    End Sub

    Private Sub Form_Resize(sender As Control, e As EventArgs) Handles Me.Resize
        'Stretches the txtOutput control so it fills the additional window space
        Dim dX, dY As Integer
        'When the form is initialising it throws a resize event, but the minimum size property is not set
        'Therefore ignore any resize events where the minimum size is zero

        If Me.MinimumSize.Width <> 0 Then

            dX = Me.Width - Me.MinimumSize.Width
            dY = Me.Height - Me.MinimumSize.Height

            ' The control is not achored, so it always stays at the same top-left position and there is no need to relocate it
            'Resize the textArea
            txtOutput.Width = 270 + dX
            txtOutput.Height = 360 + dY
        End If
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        'After the given timeout, clear the text on the status bar
        Me.Timer1.Stop()
        Me.staStatusText.Text = ""
    End Sub

    Private Sub cmbInputPeripheral_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbInputPeripheral.SelectedIndexChanged
        'If the event occurs at runtime, don't call the sub
        If cmbEvents = False Then Exit Sub
        'If the combobox is blank (e.g. if a new PIC has been selected) don't call the sub
        If cmbInputPeripheral.SelectedIndex = -1 Then Exit Sub
        'call the sub
        setPinList(cmbInputPeripheral.Text, cmbInputPin)
    End Sub

    Private Sub cmbOutputPeripheral_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbOutputPeripheral.SelectedIndexChanged
        'If the event occurs at runtime, don't call the sub
        If cmbEvents = False Then Exit Sub
        'If the combobox is blank (e.g. if a new PIC has been selected) don't call the sub
        If cmbOutputPeripheral.SelectedIndex <= 0 Then Exit Sub
        'call the sub
        setPinList(cmbOutputPeripheral.Text, cmbOutputPin)
    End Sub

    Private Sub AdvancedSettingsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AdvancedSettingsToolStripMenuItem.Click
        Dim advancedSettingsForm As Form
        advancedSettingsForm = New AdvancedSettings
        advancedSettingsForm.ShowDialog()
        advancedSettingsForm = Nothing

    End Sub


    Private Sub staStatusText_TextChanged(sender As Object, e As EventArgs) Handles staStatusText.TextChanged
        'Set the timer running so that the text is cleared after the timeout
        With Me.Timer1
            .Interval = 2500
            .Start()
        End With
    End Sub

    Private Sub cmbPinList_DrawItem(sender As ComboBox, e As DrawItemEventArgs) Handles cmbInputPin.DrawItem, cmbOutputPin.DrawItem, cmbInputPeripheral.DrawItem, cmbOutputPeripheral.DrawItem
        'Colours any pins in the dropdowns orange if they have already been assigned
        Dim brush As New SolidBrush(Color.Transparent)
        Dim newFont = e.Font
        Dim item = sender.Items(e.Index)
        Dim result As Boolean
        Try
            'Work out which combobox raised the event, as there is a different check for the two types
            If sender.Name = "cmbInputPin" Or sender.Name = "cmbOutputPin" Then
                result = checkPinAssignment(item.key, PPSData)
            Else
                result = checkPeripheralAssignment(item.key, PPSData)
            End If

            'If the peripheral/pin has already been used, make it italic
            If result Then
                newFont = New System.Drawing.Font(newFont, FontStyle.Italic)
                brush.Color = Color.ForestGreen
            Else
                newFont = New System.Drawing.Font(newFont, FontStyle.Regular)
                brush.Color = e.ForeColor
            End If

            e.DrawBackground()
            e.Graphics.DrawString(item.key, newFont, brush, e.Bounds)
            e.DrawFocusRectangle()
        Catch ex As Exception
            'Do nothing
        End Try



    End Sub

    Private Sub Form1_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Me.KeyPress
        If e.KeyChar = ChrW(4) Then 'd = dump
            If My.Computer.Keyboard.CtrlKeyDown Then
                dump_output()
            End If
        End If
        If e.KeyChar = ChrW(2) Then 'b
            If My.Computer.Keyboard.CtrlKeyDown Then
                tryAll()
            End If
        End If
        If e.KeyChar = ChrW(12) Then 'l
            If My.Computer.Keyboard.CtrlKeyDown Then
                xmlInfo()
            End If
        End If
        e.Handled = True
    End Sub
End Class
