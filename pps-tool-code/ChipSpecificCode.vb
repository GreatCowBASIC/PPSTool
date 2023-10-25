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

Module ChipSpecificCode
    'This module contains code with workarounds for specific chips
    'It was found that the way in which some features are implemented on newer chips (e.g. PIC18FxxK42)
    'required workarounds to get things to work.  These are all contained in this file for easy reference

    Sub chipSpecificChecks(PICmodule As String, pinName As String, peripheralName As String)
        Dim row(4) As String
        'Create a hard coded list of i2c pads.  This information is not in the xml files (09/12/18)
        Dim i2cPads As New Collection
        With i2cPads
            .Add(0, "RB1")
            .Add(1, "RB2")
            .Add(2, "RC3")
            .Add(3, "RC4")
            .Add(4, "RD0")  'not implemented on 18F2xK42, but there is no PORTD so it cannot be selected
            .Add(5, "RD1")  'not implemented on 18F2xK42, but there is no PORTD so it cannot be selected
        End With

        'UART on PIC18FxxK42 (and PIC18F25K83)  16/09/18
        'This requires the TX pin to be set as an output to ensure the UART has control over the pin as all times. (See PIC18F24/25K42 datasheet section 33.1)
        'In the xml files, this module is identifed as a "type=uart_protocol".  At present, only v1 of this is in the chips but this may change in the future

        If (checkformodule(PICmodule, "", "uart_protocol", "v1")) Then
            Dim port As String = rcToPort(pinName)
            If Left(peripheralName, 2) = "TX" Then
                row(0) = "Dir " & port & " Out"
                row(1) = "' Make " & peripheralName & " pin an output"
                row(2) = "UART pin directions"
                row(3) = ""
                row(4) = "Dir"
                PPSData.Rows.Add(row)
            End If
            If Left(peripheralName, 2) = "RX" Then
                row(0) = "Dir " & port & " In"
                row(1) = "' Make " & peripheralName & " pin an input"
                row(2) = "UART pin directions"
                row(3) = ""
                row(4) = "Dir"
                PPSData.Rows.Add(row)
            End If

        End If

        'I2C pad control registers for new I2C (nor MSSP) modules. PIC18FxxK42 (and PIC18F25K83) etc.  26/10/18
        If (checkformodule(PICmodule, "", "i2c_8bit", "v1")) Then
            Dim prefix As String
            If PICmodule = "I2C1" Then
                prefix = "HI2C"
            Else
                prefix = "HI2C2"
            End If
            'Set the I2C pins to be outputs (if not already done so)
            If Left(peripheralName, 3) = "SDA" Then
                row(0) = "#Define " & prefix & "_DATA " & rcToPort(pinName)
                row(1) = "'Define a constant"
                row(2) = PICmodule & " extra settings"
                row(3) = ""
                row(4) = peripheralName
                PPSData.Rows.Add(row)
                row(0) = "Dir " & prefix & "_DATA out"
                row(1) = "'Set I2C pin as output"
                row(2) = PICmodule & " extra settings"
                row(3) = ""
                row(4) = peripheralName
                PPSData.Rows.Add(row)
            End If
            If Left(peripheralName, 3) = "SCL" Then
                row(0) = "#Define " & prefix & "_CLOCK " & rcToPort(pinName)
                row(1) = "'Define a constant"
                row(2) = PICmodule & " extra settings"
                row(3) = ""
                row(4) = peripheralName
                PPSData.Rows.Add(row)
                row(0) = "Dir " & prefix & "_CLOCK out"
                row(1) = "'Set I2C pin as output"
                row(2) = PICmodule & " extra settings"
                row(3) = ""
                row(4) = peripheralName
                PPSData.Rows.Add(row)
            End If

            'Set the level for the special I2C pads
            If i2cPads.Contains(pinName) Then
                Dim port As String = rcToPort(pinName)
                row(0) = pinName & "I2C_TH0=1"
                row(1) = "'Set the I2C level for the pin"
                row(2) = PICmodule & " extra settings"
                row(3) = ""
                row(4) = peripheralName
                PPSData.Rows.Add(row)
            End If
            'Set as open drain output
            row(0) = "ODC" & Mid(pinName, 2) & "=1"
            row(1) = "'Set pin as open drain output"
            row(2) = PICmodule & " extra settings"
            row(3) = ""
            row(4) = peripheralName
            PPSData.Rows.Add(row)

            'Add a blank row
            row(0) = ""
            row(1) = ""
            row(2) = PICmodule & " extra settings"
            row(3) = ""
            row(4) = peripheralName
        End If
    End Sub

    Function moduleSpecificCode(PICModule As String) As String
        '29/01/2020 Warning note for new I2C modules on K42 etc. PICs which set the clock speed differently, so GCB can't use the normal values
        Dim nlid As String = vbCrLf + StrDup(4, ">").Replace(">", indent)   'newline and indent
        Select Case PICModule
            Case "'Module: I2C1 extra settings"
                Return PICModule + nlid + "'This is a dedicated PIC I2C module" & nlid & "'The clock speed is 125kHz by default (not 100kHz/400kHz)" & nlid & "'See the HWI2C section of the Help, or refer to the chip specific datasheet,  for how to change the clock speed to the desired frequency"
            Case "'Module: I2C2 extra settings"
                Return PICModule + nlid + "'This is a dedicated PIC I2C module" & nlid & "'The clock speed is 125kHz by default (not 100kHz/400kHz)" & nlid & "'See the HWI2C section of the Help, or refer to the chip specific datasheet,  for how to change the clock speed to the desired frequency"
            Case Else
                Return PICModule
        End Select
    End Function
End Module
