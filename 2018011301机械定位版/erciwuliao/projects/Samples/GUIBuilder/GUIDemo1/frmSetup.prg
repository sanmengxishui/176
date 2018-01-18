Function frmSetup_TextBox1_KeyPress(Sender$ As String, ByRef Key$ As String)

	If InStr("0123456789.", key$) < 0 And Asc(key$) <> 8 Then
		Key$ = ""
	EndIf
Fend

Function frmSetup_Load(Sender$ As String)

	Integer i, value
	
	For i = 1 To 10
		value = i * 10
		GSet frmSetup.cmbSpeed.AddItem, Str$(value)
		GSet frmSetup.cmbAccel.AddItem, Str$(value)
	Next
	i = g_Speed /10
	value = i - 1
	GSet frmSetup.cmbSpeed.SelectedIndex, value
	i = g_Accel /10
	value = i - 1
	GSet frmSetup.cmbAccel.SelectedIndex, value
	GSet frmSetup.txtVacDelay.Text, Str$(g_VacDelay)
Fend

Function frmSetup_btnOK_Click(Sender$ As String)

	GSet frmSetup.DialogResult, DIALOGRESULT_OK
	GClose frmSetup
	
Fend

Function frmSetup_btnCancel_Click(Sender$ As String)
	
	GSet frmSetup.DialogResult, DIALOGRESULT_CANCEL
	GClose frmSetup
Fend

