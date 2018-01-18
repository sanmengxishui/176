'
' EPSON RC+ 7.0 Sample Project
'
' GUI Builder demo 1
'
' Setup:
'   Teach the pick and place points
'   Setup remote outputs for EStopOn (bit 4) and SafeguardOn (bit 5)
'
' To run, click the RUN tool bar button, then click Start
'
Global Preserve Integer g_Speed
Global Preserve Integer g_Accel
Global Preserve Real g_VacDelay
Global Long g_CycleCount


Function main
	
	If Motor = Off Then Motor On
		
	Power High
	Speed g_Speed
	Accel g_Accel, g_Accel
	
	Do
		LogStatus "Moving to pick"
		Jump pick
		On Vacuum
		Wait g_VacDelay
		LogStatus "Moving to place"
		Jump place
		Off Vacuum
		Wait g_VacDelay
	Loop
	
Fend


