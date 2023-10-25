<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MainScreen
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainScreen))
        Me.cmbPICs = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.btnInputAdd = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.cmbInputPeripheral = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.cmbInputPin = New System.Windows.Forms.ComboBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.btnOutputAdd = New System.Windows.Forms.Button()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.cmbOutputPeripheral = New System.Windows.Forms.ComboBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.cmbOutputPin = New System.Windows.Forms.ComboBox()
        Me.btnClear = New System.Windows.Forms.Button()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.mnuSettings = New System.Windows.Forms.ToolStripMenuItem()
        Me.FileLocationsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AdvancedSettingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.chkLOCK = New System.Windows.Forms.CheckBox()
        Me.txtOutput = New System.Windows.Forms.TextBox()
        Me.btnCopy = New System.Windows.Forms.Button()
        Me.staStatusStrip = New System.Windows.Forms.StatusStrip()
        Me.staStatusText = New System.Windows.Forms.ToolStripStatusLabel()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.picChip = New System.Windows.Forms.PictureBox()
        Me.HelpProvider1 = New System.Windows.Forms.HelpProvider()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.staStatusStrip.SuspendLayout()
        CType(Me.picChip, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'cmbPICs
        '
        Me.cmbPICs.FormattingEnabled = True
        Me.HelpProvider1.SetHelpKeyword(Me.cmbPICs, "GUI.htm#PIC")
        Me.HelpProvider1.SetHelpNavigator(Me.cmbPICs, System.Windows.Forms.HelpNavigator.Topic)
        Me.cmbPICs.Location = New System.Drawing.Point(43, 25)
        Me.cmbPICs.Name = "cmbPICs"
        Me.HelpProvider1.SetShowHelp(Me.cmbPICs, True)
        Me.cmbPICs.Size = New System.Drawing.Size(121, 21)
        Me.cmbPICs.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(13, 28)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(24, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "PIC"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.btnInputAdd)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.cmbInputPeripheral)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.cmbInputPin)
        Me.HelpProvider1.SetHelpKeyword(Me.GroupBox1, "GUI.htm#PPS_Inputs")
        Me.HelpProvider1.SetHelpNavigator(Me.GroupBox1, System.Windows.Forms.HelpNavigator.Topic)
        Me.GroupBox1.Location = New System.Drawing.Point(16, 64)
        Me.GroupBox1.Name = "GroupBox1"
        Me.HelpProvider1.SetShowHelp(Me.GroupBox1, True)
        Me.GroupBox1.Size = New System.Drawing.Size(324, 83)
        Me.GroupBox1.TabIndex = 3
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "PPS Inputs"
        '
        'btnInputAdd
        '
        Me.btnInputAdd.Location = New System.Drawing.Point(243, 50)
        Me.btnInputAdd.Name = "btnInputAdd"
        Me.btnInputAdd.Size = New System.Drawing.Size(75, 23)
        Me.btnInputAdd.TabIndex = 4
        Me.btnInputAdd.Text = "Add"
        Me.btnInputAdd.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(107, 26)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(80, 13)
        Me.Label3.TabIndex = 3
        Me.Label3.Text = "Peripheral input"
        '
        'cmbInputPeripheral
        '
        Me.cmbInputPeripheral.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
        Me.cmbInputPeripheral.FormattingEnabled = True
        Me.HelpProvider1.SetHelpKeyword(Me.cmbInputPeripheral, "GUI.htm#PPS_Inputs")
        Me.HelpProvider1.SetHelpNavigator(Me.cmbInputPeripheral, System.Windows.Forms.HelpNavigator.Topic)
        Me.cmbInputPeripheral.Location = New System.Drawing.Point(193, 23)
        Me.cmbInputPeripheral.Name = "cmbInputPeripheral"
        Me.HelpProvider1.SetShowHelp(Me.cmbInputPeripheral, True)
        Me.cmbInputPeripheral.Size = New System.Drawing.Size(125, 21)
        Me.cmbInputPeripheral.TabIndex = 2
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(7, 26)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(22, 13)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Pin"
        '
        'cmbInputPin
        '
        Me.cmbInputPin.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
        Me.cmbInputPin.FormattingEnabled = True
        Me.HelpProvider1.SetHelpKeyword(Me.cmbInputPin, "GUI.htm#PPS_Inputs")
        Me.HelpProvider1.SetHelpNavigator(Me.cmbInputPin, System.Windows.Forms.HelpNavigator.Topic)
        Me.cmbInputPin.Location = New System.Drawing.Point(35, 23)
        Me.cmbInputPin.Name = "cmbInputPin"
        Me.HelpProvider1.SetShowHelp(Me.cmbInputPin, True)
        Me.cmbInputPin.Size = New System.Drawing.Size(57, 21)
        Me.cmbInputPin.TabIndex = 0
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.btnOutputAdd)
        Me.GroupBox2.Controls.Add(Me.Label4)
        Me.GroupBox2.Controls.Add(Me.cmbOutputPeripheral)
        Me.GroupBox2.Controls.Add(Me.Label5)
        Me.GroupBox2.Controls.Add(Me.cmbOutputPin)
        Me.HelpProvider1.SetHelpKeyword(Me.GroupBox2, "GUI.htm#PPS_Outputs")
        Me.HelpProvider1.SetHelpNavigator(Me.GroupBox2, System.Windows.Forms.HelpNavigator.Topic)
        Me.GroupBox2.Location = New System.Drawing.Point(16, 153)
        Me.GroupBox2.Name = "GroupBox2"
        Me.HelpProvider1.SetShowHelp(Me.GroupBox2, True)
        Me.GroupBox2.Size = New System.Drawing.Size(324, 83)
        Me.GroupBox2.TabIndex = 4
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "PPS Outputs"
        '
        'btnOutputAdd
        '
        Me.btnOutputAdd.Location = New System.Drawing.Point(243, 50)
        Me.btnOutputAdd.Name = "btnOutputAdd"
        Me.btnOutputAdd.Size = New System.Drawing.Size(75, 23)
        Me.btnOutputAdd.TabIndex = 4
        Me.btnOutputAdd.Text = "Add"
        Me.btnOutputAdd.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(7, 25)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(87, 13)
        Me.Label4.TabIndex = 3
        Me.Label4.Text = "Peripheral output"
        '
        'cmbOutputPeripheral
        '
        Me.cmbOutputPeripheral.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
        Me.cmbOutputPeripheral.FormattingEnabled = True
        Me.HelpProvider1.SetHelpKeyword(Me.cmbOutputPeripheral, "GUI.htm#PPS_Outputs")
        Me.HelpProvider1.SetHelpNavigator(Me.cmbOutputPeripheral, System.Windows.Forms.HelpNavigator.Topic)
        Me.cmbOutputPeripheral.Location = New System.Drawing.Point(100, 22)
        Me.cmbOutputPeripheral.Name = "cmbOutputPeripheral"
        Me.HelpProvider1.SetShowHelp(Me.cmbOutputPeripheral, True)
        Me.cmbOutputPeripheral.Size = New System.Drawing.Size(124, 21)
        Me.cmbOutputPeripheral.TabIndex = 2
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(230, 25)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(22, 13)
        Me.Label5.TabIndex = 1
        Me.Label5.Text = "Pin"
        '
        'cmbOutputPin
        '
        Me.cmbOutputPin.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
        Me.cmbOutputPin.FormattingEnabled = True
        Me.HelpProvider1.SetHelpKeyword(Me.cmbOutputPin, "GUI.htm#PPS_Outputs")
        Me.HelpProvider1.SetHelpNavigator(Me.cmbOutputPin, System.Windows.Forms.HelpNavigator.Topic)
        Me.cmbOutputPin.Location = New System.Drawing.Point(258, 22)
        Me.cmbOutputPin.Name = "cmbOutputPin"
        Me.HelpProvider1.SetShowHelp(Me.cmbOutputPin, True)
        Me.cmbOutputPin.Size = New System.Drawing.Size(57, 21)
        Me.cmbOutputPin.TabIndex = 0
        '
        'btnClear
        '
        Me.btnClear.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.HelpProvider1.SetHelpKeyword(Me.btnClear, "GUI.htm#clear")
        Me.HelpProvider1.SetHelpNavigator(Me.btnClear, System.Windows.Forms.HelpNavigator.Topic)
        Me.btnClear.Location = New System.Drawing.Point(674, 394)
        Me.btnClear.Name = "btnClear"
        Me.HelpProvider1.SetShowHelp(Me.btnClear, True)
        Me.btnClear.Size = New System.Drawing.Size(50, 23)
        Me.btnClear.TabIndex = 6
        Me.btnClear.Text = "Clear"
        Me.btnClear.UseVisualStyleBackColor = True
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuSettings})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(733, 24)
        Me.MenuStrip1.TabIndex = 7
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'mnuSettings
        '
        Me.mnuSettings.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileLocationsToolStripMenuItem, Me.AdvancedSettingsToolStripMenuItem, Me.AboutToolStripMenuItem})
        Me.mnuSettings.Name = "mnuSettings"
        Me.mnuSettings.Size = New System.Drawing.Size(61, 20)
        Me.mnuSettings.Text = "Settings"
        '
        'FileLocationsToolStripMenuItem
        '
        Me.FileLocationsToolStripMenuItem.Name = "FileLocationsToolStripMenuItem"
        Me.FileLocationsToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.FileLocationsToolStripMenuItem.Text = "File locations..."
        '
        'AdvancedSettingsToolStripMenuItem
        '
        Me.AdvancedSettingsToolStripMenuItem.Name = "AdvancedSettingsToolStripMenuItem"
        Me.AdvancedSettingsToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.AdvancedSettingsToolStripMenuItem.Text = "Advanced settings..."
        '
        'AboutToolStripMenuItem
        '
        Me.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        Me.AboutToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.AboutToolStripMenuItem.Text = "About..."
        '
        'chkLOCK
        '
        Me.chkLOCK.AutoSize = True
        Me.HelpProvider1.SetHelpKeyword(Me.chkLOCK, "GUI.htm#LockPPS")
        Me.HelpProvider1.SetHelpNavigator(Me.chkLOCK, System.Windows.Forms.HelpNavigator.Topic)
        Me.chkLOCK.Location = New System.Drawing.Point(16, 242)
        Me.chkLOCK.Name = "chkLOCK"
        Me.HelpProvider1.SetShowHelp(Me.chkLOCK, True)
        Me.chkLOCK.Size = New System.Drawing.Size(208, 17)
        Me.chkLOCK.TabIndex = 9
        Me.chkLOCK.Text = "Show LOCK/UNLOCKPPS statements"
        Me.chkLOCK.UseVisualStyleBackColor = True
        '
        'txtOutput
        '
        Me.txtOutput.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HelpProvider1.SetHelpKeyword(Me.txtOutput, "GUI.htm#output")
        Me.HelpProvider1.SetHelpNavigator(Me.txtOutput, System.Windows.Forms.HelpNavigator.Topic)
        Me.txtOutput.Location = New System.Drawing.Point(355, 27)
        Me.txtOutput.Multiline = True
        Me.txtOutput.Name = "txtOutput"
        Me.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.HelpProvider1.SetShowHelp(Me.txtOutput, True)
        Me.txtOutput.Size = New System.Drawing.Size(366, 360)
        Me.txtOutput.TabIndex = 10
        '
        'btnCopy
        '
        Me.btnCopy.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.HelpProvider1.SetHelpKeyword(Me.btnCopy, "GUI.htm#copy")
        Me.HelpProvider1.SetHelpNavigator(Me.btnCopy, System.Windows.Forms.HelpNavigator.Topic)
        Me.btnCopy.Location = New System.Drawing.Point(618, 394)
        Me.btnCopy.Name = "btnCopy"
        Me.HelpProvider1.SetShowHelp(Me.btnCopy, True)
        Me.btnCopy.Size = New System.Drawing.Size(50, 23)
        Me.btnCopy.TabIndex = 11
        Me.btnCopy.Text = "Copy"
        Me.btnCopy.UseVisualStyleBackColor = True
        '
        'staStatusStrip
        '
        Me.staStatusStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.staStatusText})
        Me.staStatusStrip.Location = New System.Drawing.Point(0, 424)
        Me.staStatusStrip.Name = "staStatusStrip"
        Me.staStatusStrip.Size = New System.Drawing.Size(733, 22)
        Me.staStatusStrip.TabIndex = 12
        '
        'staStatusText
        '
        Me.staStatusText.Name = "staStatusText"
        Me.staStatusText.Size = New System.Drawing.Size(21, 17)
        Me.staStatusText.Text = "txt"
        '
        'Timer1
        '
        Me.Timer1.Interval = 2500
        '
        'picChip
        '
        Me.HelpProvider1.SetHelpKeyword(Me.picChip, "GUI.htm#Pinout")
        Me.HelpProvider1.SetHelpNavigator(Me.picChip, System.Windows.Forms.HelpNavigator.Topic)
        Me.picChip.Location = New System.Drawing.Point(17, 269)
        Me.picChip.Name = "picChip"
        Me.HelpProvider1.SetShowHelp(Me.picChip, True)
        Me.picChip.Size = New System.Drawing.Size(322, 152)
        Me.picChip.TabIndex = 13
        Me.picChip.TabStop = False
        '
        'HelpProvider1
        '
        Me.HelpProvider1.HelpNamespace = "help.chm"
        '
        'MainScreen
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(733, 446)
        Me.Controls.Add(Me.picChip)
        Me.Controls.Add(Me.staStatusStrip)
        Me.Controls.Add(Me.btnCopy)
        Me.Controls.Add(Me.txtOutput)
        Me.Controls.Add(Me.chkLOCK)
        Me.Controls.Add(Me.btnClear)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.cmbPICs)
        Me.Controls.Add(Me.MenuStrip1)
        Me.HelpButton = True
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.KeyPreview = True
        Me.MainMenuStrip = Me.MenuStrip1
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(650, 485)
        Me.Name = "MainScreen"
        Me.Text = "PIC PPS Tool for GCBASIC"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.staStatusStrip.ResumeLayout(False)
        Me.staStatusStrip.PerformLayout()
        CType(Me.picChip, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents cmbPICs As ComboBox
    Friend WithEvents Label1 As Label
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents btnInputAdd As Button
    Friend WithEvents Label3 As Label
    Friend WithEvents cmbInputPeripheral As ComboBox
    Friend WithEvents Label2 As Label
    Friend WithEvents cmbInputPin As ComboBox
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents btnOutputAdd As Button
    Friend WithEvents Label4 As Label
    Friend WithEvents cmbOutputPeripheral As ComboBox
    Friend WithEvents Label5 As Label
    Friend WithEvents cmbOutputPin As ComboBox
    Friend WithEvents btnClear As Button
    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents mnuSettings As ToolStripMenuItem
    Friend WithEvents FileLocationsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents chkLOCK As CheckBox
    Friend WithEvents txtOutput As TextBox
    Friend WithEvents btnCopy As Button
    Friend WithEvents AboutToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents staStatusStrip As StatusStrip
    Friend WithEvents staStatusText As ToolStripStatusLabel
    Friend WithEvents Timer1 As Timer
    Friend WithEvents picChip As PictureBox
    Friend WithEvents HelpProvider1 As HelpProvider
    Friend WithEvents AdvancedSettingsToolStripMenuItem As ToolStripMenuItem
End Class
