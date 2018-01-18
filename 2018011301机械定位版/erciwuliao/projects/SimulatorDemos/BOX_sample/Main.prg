'This is a sample program using Box command.


Function main

	Motor On
	Power High
	
	Speed 20
	Weight 2
	Accel 100, 100

	Home
	
	Box 1, -300, 300, 500, 700, 100, 250	'inside
	Box 2, -350, 350, 450, 750, 0, 350		'outside

	Xqt func_BOX2, Normal
	Xqt func_BOX1, NoEmgAbort
	
	OnErr GoTo errhandle
		
	Go P1
			
errstart:

	Call func_BOX1_2	'Call the function for a motion after the abortion.

errhandle:
	Print Err
	If Err = ERROR_DOINGMOTION Then
		Print "Robot is moving."
		EResume errstart
	ElseIf Err = ERROR_NOMOTION Then
		Print "Robot is not moving."
		EResume errstart
	EndIf

	Quit All
	
Fend
Function func_BOX2
	
	'Robot enters into Pause state when TCP is in Box 2. 
	Wait GetRobotInsideBox(2) = 1
	Xqt func_BOX2_2, NoPause
	Pause
		
Fend
Function func_BOX2_2

	'Power is forced to be low when robot is Pause state. 	
	Wait PauseOn = True
	Power Low, Forced
	
Fend
Function func_BOX1

	'Robot motion is aborted when robot enters Box 1 and is Pause state.	
	Wait GetRobotInsideBox(1) = 1
	Pause
    AbortMotion 1
	
Fend
Function func_BOX1_2
	
	'Write program in this function after the robot motion is aborted.	
	Home
	Quit func_BOX1
	Exit Function
	
Fend



