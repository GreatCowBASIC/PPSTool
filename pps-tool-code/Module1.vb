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
Imports System.Xml
Imports System.Text.RegularExpressions
Imports System.Net
Imports System.Security.Cryptography
Imports System.Reflection
Imports System.Web.Script.Serialization

Module Module1
    Public PICs As New SortedList
    Public inputsList, pinsList, outputsList, pinGroupList, remapPins As SortedList
    Public PICmodules As ArrayList
    Public xmlFolder, templateFolder, template As String
    Public validTemplate As Boolean
    Public PPSData As New DataTable
    Dim allpins As New DataTable
    Public indent = Space(4)        'indentation for code
    Public xmlFileVersion As String
    Public doc As XmlDocument
    Public ppsMode As Integer
    Dim folderHash As String

    Dim blackBrush As New SolidBrush(Color.Black)
    Dim redBrush As New SolidBrush(Color.DarkRed)
    Dim blackPen = New Pen(Brushes.Black, 1)
    Dim fillBrush = New SolidBrush(Color.Transparent)

    Sub generate_device_list()
        'list of places to look for the XML files
        Dim xmlFolders(3) As String
        xmlFolders(0) = xmlFolder   'the current setting
        xmlFolders(1) = My.Settings.XMLLocation
        xmlFolders(2) = Application.StartupPath & "\"
        xmlFolders(3) = Application.StartupPath & "\devices\"

        'check whether the cachedPPS list has been initialised
        If My.Settings.cachedPPS Is Nothing Then
            My.Settings.cachedPPS = New System.Collections.Specialized.StringCollection
        End If

        'Go through each folder and see if any of them are any good
        For Each Module1.xmlFolder In xmlFolders
            Debug.Print(System.Reflection.MethodInfo.GetCurrentMethod().ToString & ": Trying folder: " & xmlFolder)
            If testXmlFolderPath(xmlFolder) = True Then
                'Save the last used path for next time
                With My.Settings
                    .XMLLocation = xmlFolder
                    .Save()
                End With
                'As we have found a working folder, exit this sub
                Exit Sub
            End If
        Next

        'None of the predined folders worked, therefore prompt for a location
        If chooseXMLFolder() = vbCancel Then
            'no folder was chosen so exit

            Exit Sub
        Else
            'check whether this folder works
            If testXmlFolderPath(xmlFolder) = False Then
                'folder does not contain the XML files.  Show error message and exit
                MsgBox("No XML files were found in " & xmlFolder, vbOKOnly + vbCritical, "Invalid XML folder")
            Else
                'Files were found, therefore save this folder for next time
                With My.Settings
                    .XMLLocation = xmlFolder
                    .Save()
                End With
            End If
        End If

    End Sub
    Function testXmlFolderPath(xmlFolder As String)
        'Checks xmlFolder for either the manifest file or a list of valid XML files
        'If the folder is OK, load all the chips into an array

        'Check whether the chips should be found from the manifest file, or from the directory listing
        Dim xmlSource As String
        If My.Settings.XMLSource = "" Then
            'Default if no setting has been chosen
            My.Settings.XMLSource = "XML files"
            My.Settings.Save()
            Debug.Print("No saved PIC source, defaulting to XML files")
        End If
        xmlSource = My.Settings.XMLSource



        Select Case xmlSource
            Case "Manifest file"
                'look for the manifest.txt file
                If File.Exists(xmlFolder & "\manifest.txt") Then
                    Debug.Print("Reading PIC numbers from manifest file")
                    readPicNames(xmlSource)
                    Return True
                Else
                    'file does not exist
                    Return False
                    Exit Function
                End If
            Case "XML files"
                'See whether the folder contains any XML files starting with 'PIC_" and ending in 'description.xml'
                If FileSystem.Dir(xmlFolder & "\PIC*_description.xml") <> "" Then
                    Debug.Print("Reading PIC numbers from XML file names")
                    readPicNames(xmlSource)
                    Return True
                Else
                    'no files present
                    Return False
                    Exit Function
                End If

            Case Else
                Return False
                Exit Function
        End Select

    End Function
    Sub readPicNames(source As String)
        Dim picArray() As String
        Dim fileNameSuffix As String = ""

        Dim PICName As String
        PICs = New SortedList

        picArray = New String() {}

        'compute the hash of the folder and print to debug line
        folderHash = md5ForFolder(xmlFolder, "PIC*.xml")
        Debug.Print("FOLDER HASH: " & folderHash)

        Select Case source
            Case "Manifest file"
                'Extracts all the PIC names from the manifest file into an array
                picArray = File.ReadAllText(xmlFolder & "\manifest.txt").Split(vbLf)
            Case "XML files"
                'get list of files in the folder
                picArray = Directory.GetFiles(xmlFolder, "PIC*_description.xml").Select(Function(fp) Path.GetFileName(fp).Substring(0, InStr(Path.GetFileName(fp), "_") - 1)).ToArray
        End Select

        'if caching is selected, check whether a cache is available for this folder
        If My.Settings.cachePPS Then
            If My.Settings.cachedPPS Is Nothing Then GoTo noCache   'This should never happen because it is initialised at runtime if necessary
            If My.Settings.cachedPPS.Count = 0 Then GoTo noCache
            'The first element in the collection will be the hash of the folder
            If My.Settings.cachedPPS(0) = folderHash Then
                'a cache exists so just use this to create the list of PICs
                picArray = New String(My.Settings.cachedPPS.Count - 1) {}
                'clear the existing list
                PICs.Clear()
                For i = 1 To My.Settings.cachedPPS.Count - 1
                    PICName = My.Settings.cachedPPS(i)
                    PICs.Add(PICName.Substring(3), PICName & "_description.xml")
                Next
            Else
                'cache exists but the folder hash does not match
                GoTo noCache
            End If
        Else
noCache:
            'caching is disabled or cache is empty so read all the files
            createPICList(picArray, folderHash)
        End If


    End Sub
    Sub createPICList(picArray As String(), folderHash As String)
        Dim chipNumber As String
        Dim xmlDoc As XmlDocument
        Dim xmlLoadDialog As LoadingXML

        'this can take a second so show the holding dialog
        xmlLoadDialog = New LoadingXML
        With xmlLoadDialog
            'set up the progress bar
            xmlLoadDialog.xmlProgress.Value = 0
            xmlLoadDialog.xmlProgress.Maximum = picArray.Count
            'show the dialog
            .Left = MainScreen.Left + MainScreen.Width / 2 - .Width / 2
            .Top = MainScreen.Top + MainScreen.Height / 2 - .Height / 2
            .Show()
            .Focus()
            Application.DoEvents()
        End With

        'add the PICs to the SortedList containing the PICs
        For Each PICName In picArray
            'skip any blanks
            If PICName = "" Then GoTo skip
            chipNumber = PICName.Substring(3) 'zero based, therefore start at character 4

            Try
                'Check chip for PPS
                xmlDoc = New XmlDocument
                xmlDoc.Load(xmlFolder & "\" & PICName & "_description.xml")
                If checkForPPS(xmlDoc) > 0 Then
                    'Add to the list
                    PICs.Add(chipNumber.ToUpper, PICName & "_description.xml")
                End If

                'Uncomment this to print a list of chips which don't have any pin assignments for PDIP packages
                'If checkForPDIP(xmlDoc) = False Then
                'Debug.Print(chipNumber & " contains no PDIP package")
                'End If
            Catch ex As Exception
                Select Case Err.Number
                    Case 5
                        'Duplicate item in the list - ignore because it doens't get added
                        Debug.Print(System.Reflection.MethodInfo.GetCurrentMethod().ToString & ": Duplicate item " & PICName & " not added to list")
                    Case 53
                        'File could not be loaded
                        Debug.Print(System.Reflection.MethodInfo.GetCurrentMethod().ToString & ": Could not load " & xmlFolder & "\" & PICName & "_decription.xml" & vbCrLf & "Error text: " & ex.Message)
                    Case Else
                        'Display an error message
                        MsgBox("Error: adding " & PICName & " to the List." & vbCrLf & "Error number: " & Err.Number & ": " & Err.Description)
                End Select
            End Try
skip:
            'increment the progress bar

            xmlLoadDialog.xmlProgress.Value = xmlLoadDialog.xmlProgress.Value + 1
        Next
        'cache the results if the setting is enabled
        If My.Settings.cachePPS Then
            With My.Settings.cachedPPS
                .Clear()
                .Add(folderHash)
                For Each pic In PICs
                    .Add("PIC" & pic.key)
                Next
            End With
            My.Settings.Save()
        End If
        'hide the dialog
        xmlLoadDialog.Dispose()
        xmlDoc = Nothing

    End Sub
    Function checkForPDIP(ByRef xmlDoc As XmlDocument)
        Dim xmlNodes As XmlNodeList = xmlDoc.SelectNodes("/device/pins/pin/pinNumber[contains(@package,'DIP')]")

        If xmlNodes.Count = 0 Then
            Return False
        Else
            Return True
        End If
    End Function
    Function checkForPPS(ByRef xmlDoc As XmlDocument)
        Dim ele As XmlNodeList
        'Reads the xmlDoc and checks whether there are any PPS settings.
        'Returns 0 if not, 1 if it does and 2 if there are restrictions on which pin can be used for each peripheral
        Dim deviceName = xmlDoc.SelectSingleNode(" / device").Attributes("name").Value.ToString


        If xmlDoc.SelectNodes("/device/pins/pin[@pps]").Count = 0 Then
            'no PPS data and no PPS groups, therefore assume it doesn't support PPS.
            Debug.Print(System.Reflection.MethodInfo.GetCurrentMethod().ToString & ":    " & xmlFolder & "\" & deviceName & "_decription.xml" & " contains no PPS data - ignore")
            Return 0
            Exit Function
        Else
            'check whether the PIC uses PPS groups
            ele = xmlDoc.SelectNodes("/device/manager[@name='Pin Manager']/additional_properties[@name='remap']/pinAlias/ppsSupport")
            If ele.Count > 0 Then
                Debug.Print(System.Reflection.MethodInfo.GetCurrentMethod().ToString & ":    " & xmlFolder & "\" & deviceName & "_decription.xml" & " uses PPS groups")
                Return 2
                Exit Function
            End If
            Debug.Print(System.Reflection.MethodInfo.GetCurrentMethod().ToString & ":    " & xmlFolder & "\" & deviceName & "_decription.xml" & " does not use PPS groups")
            'File does contain PPS information, but no grouping
            Return 1
        End If

        ele = Nothing


    End Function
    Sub get_PPS()

        Dim XMLFile, PIC As String
        Dim pGroups(2)
        Dim xEle As XmlElement
        Dim inputs, pins, outputs, modules As XmlNodeList
        Dim inputsBS, IpinsBS, OpinsBS, outputsBS As New BindingSource
        Dim pinSubset As String

        'Clear the comboboxes on the main form
        'this may not be needed any more?
        With MainScreen
            .cmbInputPin.DataSource = Nothing
            .cmbInputPeripheral.DataSource = Nothing
            .cmbOutputPin.DataSource = Nothing
            .cmbOutputPeripheral.DataSource = Nothing
        End With

        'Reset the data lists
        inputsList = New SortedList
        pinsList = New SortedList
        outputsList = New SortedList
        pinGroupList = New SortedList
        remapPins = New SortedList
        allpins = New DataTable
        PICmodules = New ArrayList

        'clear the image
        With MainScreen.picChip.CreateGraphics
            .Clear(MainScreen.picChip.BackColor)
            .Dispose()
        End With

        'add the columns to the pin list
        With allPins
            .Columns.Add("pinNumber", System.Type.GetType("System.Int16"))
            .Columns.Add("pinName", System.Type.GetType("System.String"))
        End With


        'Retrieve the selected PIC from the main form
        PIC = MainScreen.cmbPICs.Text
        Debug.Print(System.Reflection.MethodInfo.GetCurrentMethod().ToString & ": PIC read in the getPPS function: " & PIC)
        XMLFile = PICs.Values(MainScreen.cmbPICs.SelectedIndex)

        'Load the XML file corresponding to the selected PIC
        'This should never fail because the file was previously loaded by the readPicName/checkForPPS subs
        doc = New XmlDocument
        Try
            doc.Load(xmlFolder & "\" & XMLFile)
        Catch ex As Exception
            MsgBox("Could not load " & xmlFolder & "\" & XMLFile, vbOKOnly + vbCritical, "XML file not found")
            Exit Sub
        End Try


        'Go to /device/manager['Pin Manager']
        'If there is no Pin Manager node this will throw an error.  Should never happen because these files are for the Pin Manager program!
        With doc

            'get the list of pins for the diagram.  Use pin numbers associated with the PDIP package if available
            If checkForPDIP(doc) Then ' doc.SelectNodes("/device/pins/pin/pinNumber[contains(@package,'PDIP')]/..").Count > 0 Then
                pinSubset = "[contains(@package,'DIP')]"
            Else
                pinSubset = ""
            End If
            pins = .SelectNodes("/device/pins/pin/pinNumber" & pinSubset & "/..")

            'Check that there is a PPS attribute for some of the pins, otherwise it doesn't support PPS.
            If .SelectNodes("/device/pins/pin[@pps]").Count = 0 Then
                MsgBox("No PPS data found for " & PIC, vbOKOnly + vbCritical, "No PPS data found")
                Exit Sub
            End If

            'get the list of Input peripheral names from 
            '/device/manager[Pin Manager]/additional_properties/pinAlias[@direction='input']
            inputs = .SelectNodes("/device/manager[@name='Pin Manager']/additional_properties/pinAlias[@direction='input']")
            'inputs = .SelectNodes("/device/manager[@name='Pin Manager']/additional_properties/pinAlias[not(@alias = following-sibling::pinAlias/@alias)][@direction='input']")

            'get the list of peripheral outputs from /device/manager[Pin Manager]/additional_properties[output]
            outputs = .SelectNodes("/device/manager[@name='Pin Manager']/additional_properties/pinAlias[@direction='output']")
            'outputs = .SelectNodes("/device/manager[@name='Pin Manager']/additional_properties/pinAlias[not(@alias = following-sibling::pinAlias/@alias)][@direction='output']")

            'get the list of pin connections and their corresponding modules
            modules = .SelectNodes("/device/manager[@name='Pin Manager']/additional_properties/pinAlias")

            'Added 09/04/18
            'gets a list of nodes where the PPS name is not the same as the pin name
            'Introduced with PIF18FxxJ13/53 chips
            loadRemapList(.SelectNodes("/device/pins/pin[@pps!=@name]"))

        End With

        'get the data from the XML and put it into a table for easy lookup
        'Name is the input name
        For Each xEle In inputs
            'check for duplicates before adding as some XML files contain duplicates
            If Not (inputsList.ContainsKey(xEle.Attributes("alias").Value.ToString)) Then inputsList.Add(xEle.Attributes("alias").Value.ToString, "")
        Next
        xEle = Nothing

        'Do the same for the outputs
        '   alias is the output name
        '   module is the name of the module in the chip (not included in the table)
        '   value is the setting for RxyPPS
        '   direction is ignored because we've already filtered for outputs
        For Each xEle In outputs
            If Not (outputsList.ContainsKey(xEle.Attributes("alias").Value.ToString)) Then outputsList.Add(xEle.Attributes("alias").Value.ToString, stringToNumber(xEle.Attributes("value").Value.ToString))
        Next
        xEle = Nothing

        'Do the pins
        For Each xEle In pins
            Dim pinName As String
            Dim pinNumber As Integer

            pinName = xEle.Attributes("name").Value.ToString
            'use pin numbers relating to PDIP packages if avaiable
            pinNumber = xEle.SelectSingleNode("pinNumber" & pinSubset).Attributes("value").Value.ToString

            allPins.Rows.Add(pinNumber, pinName)

            'add the pins with PPS settings to the list of pins to use in the application
            If xEle.HasAttribute("pps") Then
                'add a check for duplicates
                pinsList.Add(xEle.Attributes("pps").Value.ToString, stringToNumber(xEle.Attributes("ppsValue").Value.ToString))
            End If

        Next
        xEle = Nothing

        'Deleted previously commented out code 09/04/18



        'Finally the modules list
        'Use an ArrayList so the module and direction can be looked up later
        For Each xEle In modules
            Dim tempArray(3)
            tempArray(0) = xEle.Attributes("alias").Value.ToString
            tempArray(1) = xEle.Attributes("module").Value.ToString
            tempArray(2) = xEle.Attributes("direction").Value.ToString
            If xEle.HasAttribute("remapTo") Then
                'some inputs have multiple functions e.g.   RX = DT (async EUSART)
                '                                           SSP1CLK can be SCL or SCK
                tempArray(3) = xEle.Attributes("remapTo").Value.ToString
            End If
            PICmodules.Add(tempArray)
        Next
        xEle = Nothing

        'populate the comboboxes on the main form
        inputsBS.DataSource = inputsList
        outputsBS.DataSource = outputsList
        'seperate sources for the input/output lists so they don't change simultaneously
        IpinsBS.DataSource = pinsList
        OpinsBS.DataSource = pinsList
        With MainScreen
            With .cmbInputPeripheral
                .DisplayMember = "Key"
                .DataSource = inputsBS
                .SelectedIndex = -1
            End With
            With .cmbOutputPeripheral
                .DisplayMember = "Key"
                .DataSource = outputsBS
                .SelectedIndex = -1
            End With
            With .cmbInputPin
                .DisplayMember = "Key"
                .DataSource = IpinsBS
                .SelectedIndex = -1
            End With
            With .cmbOutputPin
                .DisplayMember = "Key"
                .DataSource = OpinsBS
                .SelectedIndex = -1
            End With

            'if PPS groups are used, then disable the pin combo until a peripheral is selected
            'this is a bit counter intuitive as we've just set the options to the list of pins...
            If checkForPPS(doc) = 2 Then
                .cmbInputPin.Enabled = False
                .cmbOutputPin.Enabled = False
            End If

        End With

        inputs = Nothing
        pins = Nothing
        outputs = Nothing
        modules = Nothing
        xEle = Nothing

        'draw the new PIC chip
        drawchip()

    End Sub

    Sub validateTemplate(templateFile As String)
        Dim PPSStart, PPSEnd, PPSLock, PPSUnlock, PPSSetting As Integer
        Dim lnStart As String

        'Assume the template file is invalid unless proven otherwise
        validTemplate = False

        'try and load the template file
        If File.Exists(templateFile) Then
            template = System.IO.File.ReadAllText(templateFile)
        Else
            'File not found - show error and stop doing anything else
            MsgBox("The template format file could Not be found (" & templateFile & ")", vbOKOnly + vbCritical, "Missing template file")
            Exit Sub
        End If


        'Check the template file is OK
        'Look for the [PPS] and [PPSEnd] tags, and make sure there is at least the #PPSSetting variable between them
        PPSUnlock = InStr(template, "#PPSUNLOCK")
        PPSStart = InStr(template, "[PPS]")
        PPSSetting = InStr(template, "#PPSSetting")
        PPSEnd = InStr(template, "[PPSEnd]")
        PPSLock = InStr(template, "#PPSLOCK")
        'check [PPS]#variable[PPSEnd] is present
        If PPSStart * PPSSetting * PPSEnd <= 0 Then
            'one of them is not present so show error
            MsgBox("The output template file Is incorrectly formatted.  Either the PPSStart, PPSEnd Is missing, Or the PPSSettings are not included.", vbOKOnly, "Malformed template")
            Exit Sub

        End If
        'check the [PPS]#variable[PPSEnd] sequence
        If PPSStart > PPSSetting Or PPSSetting > PPSEnd Then
            'all out of sequence so show error
            MsgBox("The output template file is incorrectly formatted.  The PPSSettings variable must be after PPSStart and before PPSEnd.", vbOKOnly, "Malformed template")
            Exit Sub
        Else
            validTemplate = True
        End If
        'Strip out any lines that don't start with ' [ # vbTab or a space
        Dim permitted() As String = {"//", "'", "[", "#", vbTab, " "}

        Dim tmp() = template.Split(vbCrLf)
        For Each ln In tmp
            lnStart = ln.Substring(0, 1)
            If Not Array.IndexOf(permitted, lnStart) Then
                'Remove the line
                ln = ""
            End If
        Next

        'No further check undertaken on special characters etc. because that would restrict user comments

        'End of file validation

    End Sub

    Function stringToNumber(value As String) As Byte
        'converts a text to a number.
        'Assumes Hex values start With 0x, binary with 0b and decimal has no prefix
        If value.Length < 2 Then
            Return Convert.ToByte(value, 10)
            Exit Function
        End If
        Select Case value.Substring(0, 2)
            Case "0x"   'hex
                Return Convert.ToByte(value, 16)
            Case "0b"   'binary
                Return Convert.ToByte(value, 2)
            Case Else
                Return Convert.ToByte(value, 10)
        End Select
    End Function
    Function chooseXMLFolder()
        'Picks a folder containing the xml files
        Dim fld As New FolderBrowserDialog
        With fld
            .SelectedPath = xmlFolder
            .Description = "Pick the folder containing the MCC XML files"
            If .ShowDialog() <> vbCancel Then
                xmlFolder = .SelectedPath
                Return vbOK
            Else
                Return vbCancel
            End If

        End With
        fld.Dispose()

    End Function
    Function getModuleName(PPS As String)
        'looks up the name of the module from a PPS reference.
        'all the data is included in the modules arrayList
        'check both the alias and the remapTo columns to pick up any remapped (bidirectional) peripherals
        For Each itm In PICmodules
            If itm(0) = PPS Or itm(3) = PPS Then
                Return itm(1).ToString
                Exit Function
            End If
        Next
        'no pin found
        Return False
        Exit Function


    End Function
    Function checkBiDi(PPS As String)
        Dim remap As XmlNodeList
        Dim inp As Boolean = False
        'Checks whether the PPS item is a bidirectional pin
        'Check that there is both an input and an output setting in the table

        'Pre 21/11/17 assumes that if the same peripheral is defined as an input and an output
        'it is bidirectional.  This is true for say SDA but not true for CCP1 where the input and output
        'are seperate things.

        '21/11/17 Checks whether there is anything in the "remap" field to determine whether there is
        'a complementary input

        '25/11/17 Query the XML file and see whether 'PPS' appears more than once
        'then check there is 1 input and 1 output

        remap = doc.SelectNodes("/device/manager/additional_properties/pinAlias[@alias='" & PPS & "']")
        If remap.Count = 2 Then
            If remap(0).Attributes("direction").Value.ToString <> remap(1).Attributes("direction").Value.ToString Then
                inp = True
            End If
        End If

        Return inp
        remap = Nothing


    End Function
    Sub drawchip()

        Dim bmp As Bitmap = New Bitmap(MainScreen.picChip.Width, MainScreen.picChip.Height, Imaging.PixelFormat.Format32bppArgb)
        Dim canvas As Graphics = Graphics.FromImage(bmp)
        Dim myFont As Font
        Dim xMax, yMax, margin, markX, markY As Integer
        Dim chipWidth, chipHeight, numPins, fontSize As Single
        Dim outline As New ArrayList
        Dim pinName As String

        If MainScreen.picChip.Image IsNot Nothing Then MainScreen.picChip.Image.Dispose()

        numPins = allpins.Rows.Count
        If checkForPDIP(doc) Then 'And numPins > 0 Then       'only works for PDIP chips

            'clear the canvas
            canvas.Clear(MainScreen.picChip.BackColor)

            xMax = MainScreen.picChip.Width
            yMax = MainScreen.picChip.Height

            margin = 25 'minimum margin in pixels

            chipHeight = (yMax - 2 * margin) * 0.75
            chipWidth = xMax - 2 * margin

            'if the aspect ratio is greater than 1:[(n/2)-1] then reduce so it looks sensible (really only applies to 8 pin chips)
            If chipWidth / chipHeight > ((numPins / 2) - 1) Then
                chipWidth = chipHeight * ((numPins / 2) - 1)
            End If

            'size of a pin/space/overhang
            markX = chipWidth / (numPins + 1)
            markY = (yMax - 2 * margin - chipHeight) / 4

            'reset the chip width to match the horizontal pin spacing
            chipWidth = markX * (numPins + 1)

            'set the text width to match the pin width
            fontSize = Math.Min(markX, (margin - markY) / 3) / canvas.DpiX * 72 * 1.2
            myFont = New Font("Arial", fontSize)

            Debug.Print(System.Reflection.MethodInfo.GetCurrentMethod().ToString & ": Chip has: " & allPins.Rows.Count & "pins")

            With canvas
                'draw the chip
                .DrawRectangle(blackPen, margin, margin, chipWidth, chipHeight)

                'draw the dimple
                Dim arcSize = chipHeight / 4
                .DrawArc(blackPen, New Rectangle(margin - arcSize / 2, margin + 1.5 * arcSize, arcSize, arcSize), 90, -180)

                For i = 1 To numPins
                    Dim x_pin, y_pin As Single
                    Dim x_lbl, y_lbl As Single
                    Dim rect As New Rectangle
                    'pins 1- n/2 are on the bottom
                    'pins n/2 - n are on the top
                    Dim filter As String = "pinNumber = " & i
                    Dim result = allpins.Select(filter)
                    '02/08/2022 Catch an exception when one of the pins isn't listed in the <pins> section of the xml file
                    'e.g. PIC16F15213 in v 1.81.8 is missing pin 1 (VDD) in the definition
                    If result.Length > 0 Then
                        pinName = result.First.Item(1)
                    Else
                        pinName = ""
                    End If

                    'choose a colour if the pin is VCC/VSS
                    fillBrush.color = choosePinColour(pinName)

                    If i <= numPins / 2 Then
                        'bottom row, left to right
                        'draw the pin
                        x_pin = margin + markX + 2 * (i - 1) * markX
                        y_pin = margin + chipHeight
                        rect = New Rectangle(x_pin, y_pin, markX, markY)
                        .FillRectangle(fillBrush, rect)
                        .DrawRectangle(blackPen, rect)

                        'label the pin
                        x_lbl = margin + markX + 2 * (i - 1) * markX + markX / 4
                        y_lbl = margin + chipHeight + 5 * markY    'add a small offset so the text doens't overlap the pin
                    Else
                        'top row, right to left
                        'draw the pin                        x_pin = margin + markX + 2 * (numPins - (i)) * markX
                        y_pin = margin - markY
                        rect = New Rectangle(x_pin, y_pin, markX, markY)
                        .FillRectangle(fillBrush, rect)
                        .DrawRectangle(blackPen, rect)

                        'label the pin
                        x_lbl = margin + markX + 2 * (numPins - (i)) * markX + markX / 4
                        y_lbl = margin + chipHeight - 14 * markY    'add a small offset so the text doens't overlap the pin
                    End If
                    'Rotate the text before drawing it
                    .RotateTransform(270)
                    .TranslateTransform(x_lbl, y_lbl, Drawing2D.MatrixOrder.Append)
                    .DrawString(pinName, myFont, blackBrush, New Rectangle(0, 0, markY * 5, markX))   'height and width are reversed because the box is rotated
                    .ResetTransform()
                Next

                'add periperal inputs to the diagram
                For Each rw In PPSData.Rows
                    Exit For
                    Dim pinNumber As Integer
                    Dim x1, y1 As Single
                    'lookup the pin number from the name
                    Dim filter As String = "pinName = " & rw(3)
                    Dim result = allPins.Select(filter)
                    pinNumber = result.First.Item(1)
                    If pinNumber <= numPins / 2 Then
                        'bottom row
                        x1 = margin + markX + 2 * (pinNumber - 1) * markX + markX / 4
                        y1 = margin + chipHeight / 2 - 5
                    Else
                        'top row
                        x1 = margin + markX + 2 * (numPins - (pinNumber)) * markX + markX / 4
                        y1 = margin + chipHeight / 2 - 5
                    End If
                    .RotateTransform(270)
                    .TranslateTransform(x1, y1, Drawing2D.MatrixOrder.Append)
                    .DrawString(rw(2), myFont, redBrush, New Rectangle(0, 0, markY * 5, markX))   'height and width are reversed because the box is rotated
                    .ResetTransform()
                Next

            End With

        Else
            'no PDIP package was found for the chip so display a message for this.

            'clear the canvas
            canvas.Clear(MainScreen.picChip.BackColor)

            xMax = MainScreen.picChip.Width
            yMax = MainScreen.picChip.Height

            'set the text width
            fontSize = 10 / canvas.DpiX * 72
            myFont = New Font("Arial", fontSize)
            canvas.DrawString("Cannot display pinout for this device", myFont, redBrush, New Rectangle(xMax / 2 - 200 / 2, yMax / 2 - fontSize, 200, fontSize * 2))
        End If

        canvas.Dispose()
        MainScreen.picChip.Image = bmp
        bmp = Nothing

    End Sub
    Function getChipFromFile(ByVal fileName As String)
        Dim gcbFile() As String
        'Opens filename and attempts to extract the chip name from the file
        'Returns the chip name, or false if not found


        'Is the path set
        If fileName Is Nothing Then
            Return False
            Exit Function
        End If
        'Is the path blank
        If String.IsNullOrWhiteSpace(fileName) Then
            Return False
            Exit Function
        End If
        ' Determines if there are bad characters in the name.
        For Each badChar As Char In System.IO.Path.GetInvalidPathChars
            If InStr(fileName, badChar) > 0 Then
                Return False
                Exit Function
            End If
        Next
        'check for a .gcb extension
        If Path.GetExtension(fileName) <> ".gcb" Then
            Return False
            Exit Function
        End If
        'see if the file exists
        If File.Exists(fileName) = False Then
            Return False
            Exit Function
        End If
        'open the file and look for a line starting with #chip
        gcbFile = File.ReadAllLines(fileName)
        For Each ln In gcbFile
            'skip and lines that start with a comment
            If Left(Trim(ln), 1) = ";" Then GoTo NextLine
            If Left(Trim(ln), 1) = "'" Then GoTo NextLine
            If Left(Trim(ln), 2) = "//" Then GoTo NextLine
            'find a line that starts with #chip
            If Left(Trim(ln), 5) = "#chip" Then
                'line contains the #chip definition
                'extract what looks like a PIC number from this line using regex
                Dim reg As New Regex("(16|18)F[0-9]{3,5}")
                'REGEX1 /(?![#chip ])(.*)(?=,)/
                'regex2 /(16|18)F[0-9]{3,5}/
                If reg.IsMatch(ln) Then
                    'match found.  Return the value and exit.
                    Return reg.Match(ln).ToString
                    Exit Function
                End If
            End If
NextLine:
        Next
        'If we've got this far, then nothing has been found so return False
        Return False


    End Function

    Sub setPinList(peripheral As String, pinCombo As ComboBox)
        Dim bs As New BindingSource
        Dim validPins As XmlNodeList
        Dim validPinsList As New SortedList
        Dim pinName, pinDir As String
        Dim pinSetting As Integer

        If checkForPPS(doc) = 2 Then
            'display a message in the status bar
            MainScreen.staStatusText.Text = "Chip has restrictions on pin assignments.  Choose a peripheral to get started."
            'lookup the peripheral in the XML file and return the list of pins that can be used for it 
            If pinCombo.Name = "cmbOutputPin" Then
                pinDir = "output"
            Else
                pinDir = "input"
            End If
            validPins = doc.SelectNodes("/device/manager[@name='Pin Manager']/additional_properties/pinAlias[@alias='" & peripheral & "'][@direction='" & pinDir & "']/ppsSupport")

            validPinsList.Clear()
            For Each pin In validPins
                'lookup the setting for this pin in the pinsList which has already been created
                pinName = pin.Attributes("pps").Value.ToString
                pinSetting = pinsList(pinName)
                'sometimes, duplicate pins are included in the XML files so check before adding!
                '02/08/2022 - Also check that the peripheral doesn't list a pin which doesn't exist
                If validPinsList.ContainsKey(pinName) = False And pinsList.ContainsKey(pinName) Then
                    validPinsList.Add(pinName, pinsList(pinName))
                End If
            Next
            'set the sorted list as the datasource of a binding source
            bs.DataSource = validPinsList
            With pinCombo
                .DisplayMember = "Key"
                .DataSource = bs
                .SelectedIndex = -1
                .Enabled = True
            End With
        Else
            'no pin group restrictions so don't change anything
            'enable the dropdown if it's disabled
            pinCombo.Enabled = True
        End If

        'Deleted previously commented out code - 09/04/18

        validPins = Nothing
        validPinsList = Nothing

    End Sub
    'Function checkForUpdateSF() As String
    '    'Deprecated 02/08/2022


    '    'Downloads the version.txt on the GCB SF pages and compares with the version.txt in the XML folder
    '    'Returns a string with the current version of the XML files
    '    Dim versionUrl As String = "https://downloads.sourceforge.net/project/gcbasic/Support%20Files/PPSTool/version.txt"
    '    Dim reader As StreamReader

    '    'Now open the file at the redirected URL
    '    Dim client As WebClient = New WebClient()
    '    'Sourceforge blocks connections using TLS1.0 so specify TLS1.2 if available
    '    Try
    '        ServicePointManager.SecurityProtocol = 3072 'SecurityProtocolType.Tls12 = TLS1.2
    '        reader = New StreamReader(client.OpenRead(versionUrl))
    '        Dim latestVersion As String = reader.ReadLine
    '        Return parseVersionSF(latestVersion)
    '    Catch
    '        Return "not available (connection error)"
    '    End Try


    '    client.Dispose()
    '    client = Nothing
    '    reader = Nothing

    'End Function
    Function checkForUpdateMC() As MCCVersion

        'Opens the MCC page on the Microchip website and returns the latest version number
        'Returns a string with the current version of the XML files
        'Dim versionUrl As Uri = New Uri("https://www.microchip.com/mplab/mplab-code-configurator")
        'Dim versionUrl As Uri = New Uri("https://www.microchip.com/mccbasic/api/values")
        Dim versionUrl As Uri = New Uri("https://www.microchip.com/mcc_libraries_xml")      'new URL for updates 24/07/2022
        Dim MCC As New MCCVersion

        'Now open the file at the redirected URL

        'Sourceforge blocks connections using TLS1.0 so specify TLS1.2 if available
        Try
            ServicePointManager.SecurityProtocol = 3072 'SecurityProtocolType.Tls12 = TLS1.2

            MCC = parseVersionXML(versionUrl)
        Catch
            Debug.Print(Err.Description)
            MCC.version = "not available (connection error)"
        End Try

        Return MCC

    End Function

    Function parseVersionXML(versionURL As Uri) As MCCVersion
        'Added July 2022
        'Microchip updated their web site again and stopped updating the API after version 1.81.0
        'New version of MCC has been released and uses an XML file which lists all library versions, and the latest version has a "latest" attribute
        'Therefore, search through the XML file for the <library ILibrary="com.microchip.mcc.mcu8.Mcu8PeripheralLibrary" isLatestVersion="false">

        Dim MCC As New MCCVersion
        Dim xml As New XmlDocument
        Dim json As New JavaScriptSerializer()

        'default values, in case nothing is found
        MCC.version = "Not available (4)"
        MCC.downloadUrl = Nothing

        xml.Load(versionURL.ToString)

        Dim node As XmlNode = xml.SelectSingleNode("//library[@ILibrary='com.microchip.mcc.mcu8.Mcu8PeripheralLibrary'][@isLatestVersion='true']")

        MCC.version = "v" & node.Attributes("version").Value.ToString
        MCC.downloadUrl = node.SelectSingleNode("jar_url").InnerText

        Return MCC
        MCC = Nothing
    End Function

    Function parseVersionSF(filename As String)
        'Parses the version.txt downloaded from the GCB SF page
        Dim reg As New Regex("(v.*).jar")
        If reg.IsMatch(filename) Then
            Dim matches = reg.Match(filename).Groups
            Return matches(1).ToString
            Exit Function
        Else
            'no match found
            Return "Not available (2)"
        End If
    End Function

    Class MCCVersion
        Public version As String
        Public downloadUrl As String
    End Class
    Function parseVersionMC(webtext As String) As MCCVersion
        'Updated Feb 2020
        'Microchip updated their web page so the table is only populated by a script which requires a browser to run (i.e. the data isn't in the html source)
        'However the data is read from an API which return a JSON string containing all the data

        Dim MCC As New MCCVersion
        Dim json As New JavaScriptSerializer()
        Dim filedb, vnt, api As Dictionary(Of String, Object)

        'default values, in case nothing is found
        MCC.version = "Not available (4)"
        MCC.downloadUrl = Nothing

        filedb = json.Deserialize(Of Dictionary(Of String, Object))(webtext)

        For Each vnt In filedb.Item("Variants")
            If vnt.ContainsKey("Content__Variant") Then
                If vnt("Content__Variant") = "Current" Then

                    Dim apidetails As ArrayList = vnt("APIDetails")
                    For Each api In apidetails
                        If api("Content__Details__title") = "PIC10 / PIC12 / PIC16 / PIC18 MCUs" Then
                            MCC.version = "v" & api("Content__Details__version")
                            MCC.downloadUrl = api("Content__Details__download").ToString.Replace("http://", "https://")      'The API gives a http:// address but it should be https:// otherwise there's an extra redirect to follow
                            Exit For        'assumes the first match is the correct one
                        End If
                    Next
                End If
            End If

        Next

        Return MCC
        MCC = Nothing
    End Function

    Public Function getCurrentXMLVersion() As String
        'primarily uses hash of xml files, falls back to checking for a version.txt file
        'Dim curHash As String
        'Dim versionUrl As String = "https://sourceforge.net/p/pps-tool/code/HEAD/tree/%20pps-tool-code/MCCVersions.xmlq?format=raw"
        Dim xmlMCC As New XmlDocument
        Dim curversion As XmlNodeList

        'check whether the folder hash has already been calculated
        If folderHash = "" Then
            folderHash = md5ForFolder(xmlFolder, "PIC*.xml")
        End If
        'load the XML file with the information on the hashes
        'as the file is an embedded resource, it needs to be accessed as a stream
        Dim ass As Assembly = Assembly.GetExecutingAssembly
        Dim res As Stream = ass.GetManifestResourceStream("PIC_PPS.MCCVersions.xml")
        xmlMCC.LoadXml(New StreamReader(res).ReadToEnd)

        'search for a version name matching the hash of the current files
        curversion = xmlMCC.SelectNodes("/mccData/release[@hashvalue='" & folderHash & "']")
        If curversion.Count > 0 Then
            'N.B. some sub versions have no change in the XML files, therefore you get 2 results
            Return curversion.Item(curversion.Count - 1).Attributes("name").Value.ToString
        Else
            'see if a version.txt exists (indicates that it came from the GCB website)
            If File.Exists(xmlFolder & "\version.txt") Then
                'read contents extract the version number
                Return parseVersionSF(File.ReadAllText(xmlFolder & "\version.txt"))
            Else
                'the version.txt file does not exist
                Return "Not available (3)"
            End If

        End If

    End Function

    Function md5ForFolder(path As String, pattern As String)
        'calculates a hash value for the xml files in a folder

        Dim hasher As MD5 = MD5.Create
        Dim fileBytes() As Byte

        Dim HashDialog As LoadingXML

        'Added 09/05/18 Check that the folder exists first
        If Directory.Exists(path) Then
            Dim files = Directory.GetFiles(path, pattern, SearchOption.TopDirectoryOnly).OrderBy(Function(fn) fn).ToList

            'show a progress dialog because sometimes this takes a few seconds
            HashDialog = New LoadingXML
            With HashDialog
                'change the label
                HashDialog.Label1.Text = "Getting list of files..."
                'set up the progress bar
                HashDialog.xmlProgress.Value = 0
                HashDialog.xmlProgress.Maximum = files.Count
                'show the dialog
                .Left = MainScreen.Left + MainScreen.Width / 2 - .Width / 2
                .Top = MainScreen.Top + MainScreen.Height / 2 - .Height / 2
                .Show()
                .Focus()
                Application.DoEvents()
            End With

            'lhash each file, including the date modified value
            For i = 0 To files.Count - 1
                'Don't include any meta information as this changes when the file gets archived on the Microchip website
                fileBytes = File.ReadAllBytes(files(i))

                If i = files.Count - 1 Then
                    'last file in the list
                    hasher.TransformFinalBlock(fileBytes, 0, fileBytes.Length)
                Else
                    'not the last file
                    hasher.TransformBlock(fileBytes, 0, fileBytes.Length, fileBytes, 0)
                End If
                'increment the progress bar
                HashDialog.xmlProgress.Value = HashDialog.xmlProgress.Value + 1
            Next
            md5ForFolder = BitConverter.ToString(hasher.Hash).Replace("-", "").ToLower
            HashDialog.Close()
            HashDialog.Dispose()
            HashDialog = Nothing
        Else
            md5ForFolder = ""
        End If


    End Function

    Function md5ForFile(path As String) As String
        Dim Bytes() As Byte
        Dim sb As New System.Text.StringBuilder()

        Bytes = File.ReadAllBytes(path)

        'Get md5 hash
        Bytes = MD5.Create().ComputeHash(Bytes)

        'Loop though the byte array and convert each byte to hex.
        For x As Integer = 0 To Bytes.Length - 1
            sb.Append(Bytes(x).ToString("x2"))
        Next

        'Return md5 hash.
        Return sb.ToString()
    End Function

    Function checkPinAssignment(pinName As String, ByRef PPSData As DataTable) As Boolean
        'check whether the datatable is initialised
        If PPSData.Columns.Count = 0 Then
            Return False
            Exit Function
        End If
        'Check whether there is any data in the table
        If PPSData.Rows.Count = 0 Then
            Return False
            Exit Function
        End If

        If PPSData.Select("PinName = '" & pinName & "'").Count = 0 Then
            'PinName has not been used
            Return False
        Else
            'PinName has been used
            Return True
        End If
    End Function
    Function checkPeripheralAssignment(PPSRegister As String, ByRef PPSData As DataTable) As Boolean
        'check whether the datatable is initialised
        If PPSData.Columns.Count = 0 Then
            Return False
            Exit Function
        End If
        'Check whether there is any data in the table
        If PPSData.Rows.Count = 0 Then
            Return False
            Exit Function
        End If

        If PPSData.Select("Register = '" & PPSRegister & "'").Count = 0 Then
            'PPSSetting has not been used
            Return False
        Else
            'PPSSetting has been used
            Return True
        End If
    End Function

    Function choosePinColour(pinName As String) As Color
        Debug.Print(pinName)
        Select Case pinName.ToUpper
            Case "VSS"
                Return Color.FromArgb(50, 50, 50) 'Almost black
            Case "AVSS"
                Return Color.FromArgb(5, 5, 55) 'Navy Blue
            Case "VSS2"
                Return Color.FromArgb(60, 60, 60) 'Almost black
            Case "VSS3"
                Return Color.FromArgb(70, 70, 70) 'Almost black
            Case "VSS4"
                Return Color.FromArgb(80, 80, 80) 'Almost black
            Case "VSS5"
                Return Color.FromArgb(90, 90, 90) 'Almost black
            Case "VDD"
                Return Color.FromArgb(200, 80, 80) 'Pinky red
            Case "AVDD"
                Return Color.FromArgb(150, 70, 10) 'Brown
            Case "VDD2"
                Return Color.FromArgb(210, 90, 90) 'Pinky red
            Case "VDD3"
                Return Color.FromArgb(220, 100, 100) 'Pinky red
            Case "VDD4"
                Return Color.FromArgb(230, 110, 110) 'Pinky red
            Case "VDD5"
                Return Color.FromArgb(240, 110, 110) 'Pinky red
            Case Else
                'see if pin is allocated to a peripheral
                If checkPinAssignment(pinName, PPSData) Then
                    Return Color.FromArgb(34, 139, 34) 'Forest green
                ElseIf checkPinAssignment(remapPin(pinName), PPSData) Then
                    Return Color.FromArgb(65, 105, 225) 'Royal blue
                Else
                    Return Color.Transparent
                End If
        End Select
    End Function

    Function remapPin(RAn As String)
        'Added 09/04/18
        'On some chips (e.g. PIC18FxxJ13/53), pins are denoted by RPn not RAn
        'If this is the case, then the RPn pin name needs to be determined from the RAn
        'This can then be used for updating the diagram

        'Check whether the remap table has been initialised and contains any data
        If remapPins.Count = 0 Then
            Return False
            Exit Function
        End If

        'Check whether there is a corresponding pin name (there should be)
        If remapPins.ContainsKey(RAn) Then
            Return remapPins.Item(RAn)
        Else
            Return False
            Exit Function
        End If

    End Function

    Sub loadRemapList(nodeList As XmlNodeList)
        Dim xEle As XmlElement
        'Added 09/04/18
        'Populates a list with the corresponding RAn pin reference for each RPn pin reference
        'If the chip does not use Remappable Pins, the table is just left empty.
        If nodeList.Count = 0 Then
            'No remappable pins
            Exit Sub
        End If

        'populate the list
        For Each xEle In nodeList
            remapPins.Add(xEle.Attributes("name").Value.ToString, xEle.Attributes("pps").Value.ToString)
        Next

    End Sub

    Sub dump_output()
        'Iterates through all options in the dropdowns
        Dim i As Integer = 0
        For Each peripheral In MainScreen.cmbInputPeripheral.Items
            MainScreen.cmbInputPeripheral.SelectedItem = peripheral
            If i >= MainScreen.cmbInputPin.Items.Count - 1 Then
                i = 0
            Else
                i = i + 1
            End If
            MainScreen.cmbInputPin.SelectedIndex = i
            MainScreen.btnInputAdd.PerformClick()
        Next
        i = 0
        For Each peripheral In MainScreen.cmbOutputPeripheral.Items
            MainScreen.cmbOutputPeripheral.SelectedItem = peripheral
            If i >= MainScreen.cmbOutputPin.Items.Count - 1 Then
                i = 0
            Else
                i = i + 1
            End If
            MainScreen.cmbOutputPin.SelectedIndex = i
            MainScreen.btnOutputAdd.PerformClick()
        Next
    End Sub

    Sub tryAll()
        For Each pic In MainScreen.cmbPICs.Items
            MainScreen.cmbPICs.SelectedItem = pic
            dump_output()
            MainScreen.btnClear.PerformClick()
        Next
    End Sub

    Sub xmlInfo()
        'Added 28/10/2018
        'Displays a hash of each xml file and the modification date for debugging
        Dim output As New System.Text.StringBuilder
        Dim hasher As MD5 = MD5.Create

        output.AppendLine("Dump of xml files in use")

        For Each PICName In PICs
            'skip any blanks
            If PICName.value = "" Then GoTo skip

            Try
                output.AppendLine(PICName.key & ": " & md5ForFile(xmlFolder & "\" & PICName.value) & " " & File.GetLastWriteTime(xmlFolder & "\" & PICName.value))
            Catch ex As Exception
                Select Case Err.Number
                    Case 5
                        'Duplicate item in the list - ignore because it doens't get added
                        Debug.Print(System.Reflection.MethodInfo.GetCurrentMethod().ToString & ": Duplicate item " & PICName.key & " not added to list")
                    Case 53
                        'File could not be loaded
                        Debug.Print(System.Reflection.MethodInfo.GetCurrentMethod().ToString & ": Could not load " & xmlFolder & "\" & PICName.key & "_decription.xml" & vbCrLf & "Error text: " & ex.Message)
                    Case Else
                        'Display an error message
                        MsgBox("Error: adding " & PICName.key & " to the List." & vbCrLf & "Error number: " & Err.Number & ": " & Err.Description)
                End Select
            End Try
skip:
            MainScreen.txtOutput.Clear()
            MainScreen.txtOutput.Text = output.ToString
        Next
    End Sub

    Function rcToPort(RC As String) As String
        'Added 16/09/18
        'Converts a pin name Rxy to a port reference for GCB PORTx.y
        Return "PORT" & Mid(RC, 2, 1) & "." & Mid(RC, 3)
    End Function

    Function checkformodule(moduleName As String, moduleGroup As String, moduleType As String, moduleVersion As String) As Boolean
        'Added 16/09/18
        'Checks whether moduleName matches moduleGroup && moduleType && moduleVersion
        Dim qry As String = "/device/modules/module[@name='" & moduleName & "'"

        If moduleGroup <> "" Then
            qry &= " and @group='" & moduleGroup & "'"
        End If
        If moduleType <> "" Then
            qry &= " and @type='" & moduleType & "'"
        End If
        If moduleVersion <> "" Then
            qry &= " and @version='" & moduleVersion & "'"
        End If
        qry &= "]"

        If doc.SelectNodes(qry).Count > 0 Then
            Return True
        Else
            Return False
        End If

    End Function

End Module


