#define MODE_ESTOPMODE 1
#define MODE_READY 2
#define MODE_RUNNING 3
#define MODE_PAUSE 4
#define MODE_SGOPEN 5

Function frmMain_btnStart_Click(Sender$ As String)
	
	Integer answer

	MsgBox "Ready to start?", MB_YESNO + MB_ICONQUESTION, "GUI Builder Demo", answer
	If answer = IDNO Then
		Exit Function
	EndIf
	
	EnableButtons MODE_RUNNING

	MemOn InCycle
	
	Xqt main
Fend

Function frmMain_btnStop_Click(Sender$ As String)

	Quit main
	EnableButtons MODE_READY
	MemOff InCycle
	
Fend

Function frmMain_Load(Sender$ As String)

	MemOff InCycle
	If g_Speed = 0 Then
		g_Speed = 50
	EndIf
	If g_Accel = 0 Then
		g_Accel = 50
	EndIf
	If g_VacDelay = 0 Then
		g_VacDelay = .05
	EndIf
	
	Xqt monitor, NoEmgAbort

Fend

Function LogStatus(txt$ As String)
	
	GSet frmMain.txtStatus.AppendText, txt$ + CRLF
	
Fend

Function frmMain_btnPause_Click(Sender$ As String)

	GSet frmMain.btnPause.Enabled, False
	Pause
Fend

Function frmMain_btnCont_Click(Sender$ As String)

	GSet frmMain.btnCont.Enabled, False
	Cont
Fend

Function frmMain_btnReset_Click(Sender$ As String)

	Reset
Fend

Function frmMain_btnSetup_Click(Sender$ As String)
	
	Integer result
	String txt$
	
	result = GShowDialog(frmSetup)
	If result = DIALOGRESULT_OK Then
		GGet frmSetup.cmbSpeed.Text, txt$
		g_Speed = Val(txt$)
		GGet frmSetup.cmbAccel.Text, txt$
		g_Accel = Val(txt$)
		GGet frmSetup.txtVacDelay.Text, txt$
		g_VacDelay = Val(txt$)
	EndIf
Fend

Function monitor

	Boolean estopDetected
	Boolean sgopenDetected
	Boolean sgcloseDetected
	Boolean pauseDetected
	Boolean errorDetected
	
	MemOff EStopIsOn
	MemOff ErrorIsOn
	MemOff SafetyIsOn
	
	Do
		Select True
			Case EStopOn And Not estopDetected
				estopDetected = True
				EnableButtons MODE_ESTOPMODE
				MemOn EStopIsOn
				MemOff InCycle
				LogStatus "EStop occurred"
			Case Not EStopOn And estopDetected
				estopDetected = False
				EnableButtons MODE_READY
				MemOff EStopIsOn
				LogStatus "Ready to Cycle"
			Case SafetyOn And Not sgopenDetected
				sgopenDetected = True
				EnableButtons MODE_SGOPEN
				MemOn safetyisOn
			Case Not SafetyOn And sgopenDetected
				sgopenDetected = False
				If MemSw(InCycle) Then
					EnableButtons MODE_PAUSE
				EndIf
				MemOff safetyIsOn
			Case PauseOn And Not ErrorOn And Not SafetyOn And Not pauseDetected
				pauseDetected = True
				If MemSw(InCycle) Then
					EnableButtons MODE_PAUSE
				EndIf
			Case Not PauseOn And pauseDetected
				pauseDetected = False
				EnableButtons MODE_RUNNING
			Case ErrorOn And Not errorDetected
				errorDetected = True
				MemOn ErrorIsOn
				MemOff InCycle
				EnableButtons MODE_ESTOPMODE
			Case Not ErrorOn And errorDetected
				errorDetected = False
				MemOff ErrorIsOn
				EnableButtons MODE_READY
		Send
		Wait .1
	Loop
	
Fend
Function EnableButtons(mode As Integer)
	
	Select mode
		Case MODE_ESTOPMODE
			GSet frmMain.btnStart.Enabled, False
			GSet frmMain.btnStop.Enabled, False
			GSet frmMain.btnPause.Enabled, False
			GSet frmMain.btnCont.Enabled, False
			GSet frmMain.btnReset.Enabled, True
			GSet frmMain.btnSetup.Enabled, True
	
		Case MODE_READY
			GSet frmMain.btnStart.Enabled, True
			GSet frmMain.btnStop.Enabled, False
			GSet frmMain.btnPause.Enabled, False
			GSet frmMain.btnCont.Enabled, False
			GSet frmMain.btnReset.Enabled, True
			GSet frmMain.btnSetup.Enabled, True
			
		Case MODE_RUNNING
			GSet frmMain.btnStart.Enabled, False
			GSet frmMain.btnStop.Enabled, True
			GSet frmMain.btnPause.Enabled, True
			GSet frmMain.btnReset.Enabled, False
			GSet frmMain.btnSetup.Enabled, False
			
		Case MODE_PAUSE
			GSet frmMain.btnPause.Enabled, False
			GSet frmMain.btnCont.Enabled, True
			
		Case MODE_SGOPEN
			GSet frmMain.btnPause.Enabled, False
			GSet frmMain.btnCont.Enabled, False
			
	Send
		
Fend

