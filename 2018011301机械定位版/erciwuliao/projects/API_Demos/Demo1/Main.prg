
Global Preserve Integer g_EventNumber
Global Preserve Integer g_CyclesToRun
Global Preserve Integer g_CycleCount

Function main

	Integer i
	If g_CyclesToRun < 10 Then
		g_CyclesToRun = 10
	EndIf
	For i = 1 To g_CyclesToRun
		Print "This is an RC+ 7.0 API demo project"
		Print i
		SPELCom_Event g_EventNumber, "Here is an event from main", i
		Wait .1
		g_CycleCount = g_CycleCount + 1
	Next i

Fend

Function main1

	Integer i
	If g_CyclesToRun < 10 Then
		g_CyclesToRun = 10
	EndIf
	For i = 1 To g_CyclesToRun
		Print "This is an RC+ 7.0 API demo project"
		Print i
		SPELCom_Event g_EventNumber, "Here is an event from main1", i
		Wait .1
		g_CycleCount = g_CycleCount + 1
	Next i

Fend
Function main2

	Integer i
	If g_CyclesToRun < 10 Then
		g_CyclesToRun = 10
	EndIf
	For i = 1 To g_CyclesToRun
		Print "This is an RC+ 7.0 API demo project"
		Print i
		SPELCom_Event g_EventNumber, "Here is an event from main2", i
		Wait .1
		g_CycleCount = g_CycleCount + 1
	Next i

Fend
