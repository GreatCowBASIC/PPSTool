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

Public NotInheritable Class AdvancedSettings

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        'Save the settings
        With My.Settings
            If btnCacheYes.Checked Then
                .cachePPS = True
            Else
                .cachePPS = False
                'also clear the cache
                .cachedPPS = New System.Collections.Specialized.StringCollection
            End If
            .XMLSource = cmbSearchMethod.Text
            .Save()
        End With
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub AdvancedSettings_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'Read the current settings and populate boxes
        Me.cmbSearchMethod.SelectedText = My.Settings.XMLSource
        If My.Settings.cachePPS Then
            Me.btnCacheYes.Checked = True
        Else
            Me.btnCacheNo.Checked = True
        End If
    End Sub

End Class
