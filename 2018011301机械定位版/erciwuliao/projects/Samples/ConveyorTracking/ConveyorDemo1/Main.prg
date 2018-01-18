'
' EPSON RC+ 7.0 Sample Project
'
' Conveyor Tracking demo
'
Global Integer r1, r2

Function Main
	Trap Error Xqt CycleStop
	Trap Abort Xqt CycleStop
	Trap Pause Xqt CycleStop

	Call Init			'Initialize
	Wait 1				'Allow conveyor to start running
	Xqt VisionTask		'
	Wait MemSw(VisionReady) = On	'Wait vision task ready
	
	MemOff StopRB1
	
	Xqt MonitorStopSw	'Stop switch checking task

	Xqt RB1				'Upstream Robot task
	Xqt RB2				'Downstream Robot task
	
	Wait MemSw(StopCycle) = On		'Wait Stop Button
	Wait MemSw(StopCycle) = Off

	Quit All

Fend

Function RB1
	Robot 1
	OnErr GoTo ErrHandler
	Integer ErrNum
	Double OffX, OffY, ZPick
	
	'Offset value for the queue.
	OffX = 0
	OffY = 0
	ZPick = 0
	
WaitParts:
	Do
		'Move Robot to the home position.
		If MemSw(StopRB2) = On Then	'Robot 2 is home.
			Off Vacuum1
			Off blow1
			Speed 10
			Jump P0			'Goto Home
			Speed 100
			MemOn StopRB1
			Quit RB1		'Quit own task
		EndIf

		Wait Cnv_QueLen(1, CNV_QUELEN_PICKUPAREA) > 0		'Waiting part is comming.
      
        Jump Cnv_QueGet(1) +X(OffX) +Y(OffY) -Z(ZPick) /L C0 ! D1; Off blow1; D20; On Vacuum1 !	'Start Tracking and vacume on.
				
		Cnv_QueRemove 1, 0							'Remove queue data
		Jump P1 C0 ! D95; Off Vacuum1; On Blow1 !		'Rejecting part
		
	Loop
	
ErrHandler:
	ErrNum = Err
	'Move queue to the downword conveyor
	If ErrNum = 4406 Then					'Only recover queue when Error #4406 is happened.
		Cnv_QueMove 1, 0					'Move queue to the downword conveyor
		
		EResume WaitParts					'Reset error condition
	'Shows an error
	Else
		Print "Error!!"
		Print "No.", Err, ":", ErrMsg$(Err)
		Print "Line :", Erl(0)
		Call CycleStop
	EndIf
Fend

Function RB2
	Robot 2
	OnErr GoTo ErrHandler
	Integer ErrNum
	Double OffX, OffY, ZPick
	
	'Offset value for the queue.
	OffX = 0
	OffY = 0
	ZPick = 0

WaitParts:
	Do
		'Move Robot 2 to the home position
		If MemSw(StopCycle) = On Then
			Off Vacuum2
			Off blow2
			Speed 10
			Jump P0			'Goto home position.
			Speed 100
			MemOn StopRB2
			Quit RB2		'Quit own task.
		EndIf
		
		Wait Cnv_QueLen(2, CNV_QUELEN_PICKUPAREA) > 0	'Waiting part is comming.

        Jump Cnv_QueGet(2) +X(OffX) +Y(OffY) -Z(ZPick) /L C0 ! D1; Off Blow2; D20; On Vacuum2 ! 'Start Tracking and vacume on.
				
		Cnv_QueRemove 2, 0									'Remove queue data
		Jump P1 +Z(0.1) C0 ! D95; Off Vacuum2; On Blow2 !	'Rejecting part
		
	Loop
	
ErrHandler:
	ErrNum = Err
	'Remove queue
	If ErrNum = 4406 Then	'Only recover queue when Error #4406 is happened.
		Cnv_QueRemove 2, 0					'Remove queue data.
	
		EResume WaitParts					'Reset error condition.
	Else
		Print "Error!!"
		Print "No.", Err, ":", ErrMsg$(Err)
		Print "Line :", Erl(0)
		Call CycleStop
	EndIf
Fend

Function VisionTask
	Boolean found
	Integer i, state, NumF, v_count
	Double x, y, u
	Double OffX, OffY, ZPick
	
	Cnv_QueReject 1, 23	'Que rejecting distance is 23 mm.

	'Offset values for the queue
	OffX = 0
	OffY = 0
	ZPick = 0
	
	Off Vis_Trigger;	Off Cv_Trigger	'Reset external trigger signals

	Wait 0.15
	
	MemOn VisionReady	'Tell other tasks that vision is ready
	
	Do
		
		On LED		'Light On
		Wait .025

		VRun FindParts01	'Start Vision sequence.
		
		On Vis_Trigger; On Cv_Trigger	'Set External triggers
	    
		Do					'Wait for vision acquire to finish
			Wait 0.3
			VGet FindParts01.AcquireState, state
		Loop Until state = 3
		
		VGet FindParts01.Geom01.NumberFound, NumF	'Get the number of found parts.
		For i = 1 To NumF
			VSet FindParts01.Geom01.CurrentResult, i
			VGet FindParts01.Geom01.CameraXYU, found, x, y, u
			Cnv_QueAdd 1, Cnv_Point(1, x, y) +X(OffX) +Y(OffY) :Z(ZPick)	'Add queue data
		Next
		
		Off Vis_Trigger; Off Cv_Trigger		'Reset External triggers
		Wait 0.05
	Loop
Fend

Function CycleStop
	Integer i

	Quit VisionTask
	Quit RB1
	Quit RB2
	
	For i = 4 To 10	'Stop all conveyors.
		Off i
	Next
	
	Quit All
	
Fend

Function Init
	Integer i
	
	MemOff 0

	Cnv_QueRemove 1, All	'Clear queue data of the #1 conveyor.
	
	Cnv_QueRemove 2, All	'Clear queue data of the #2 conveyor.

	Robot 2
	Motor On
	Jump P0		'Move to home position
	Off Vacuum2
	Power High
	Speed 100
	Accel 120, 120
	LimZ -15
	Weight 0.3
	Arch 0, 8, 8
	Fine 50000, 50000, 50000, 50000
	
	Robot 1
	Motor On
	Jump P0		'Move to home position
	Off Vacuum1
	Power High
 	Speed 100
	Accel 120, 120
	LimZ -42
	Weight 0.3
	
 
	Arch 0, 8, 12
	Fine 50000, 50000, 50000, 50000

	Wait 2
	On Vacuum1
	On Vacuum2
	For i = 4 To 9	'All conveyors are running.
		On i
	Next
	Wait 1
Fend

Function MonitorStopSw
	Integer i
	
	MemOff StopCycle		'Clear cycle stop flag.
	Wait Sw(StopSw) = On	'Waiting Stop button.
	MemOn StopCycle			'Set cycle stop flag.

	Do
		If MemSw(StopRB1) = On And MemSw(StopRB2) = On Then
			MemOff StopRB1
			MemOff StopRB2
			Off StopLamp	'Off Stop indicator
			Exit Do
		Else				'Robot is still working, so waiting robot is stopped.
			On StopLamp
			Wait 0.5
			Off StopLamp
			Wait 0.5
		EndIf
		
		Wait 0.01
	Loop
	
	For i = 4 To 10	'Stop all conveyors.
		Off i
	Next

	MemOff StopCycle
	
Fend





