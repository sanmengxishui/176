'
' G6 Sample Project
'
' Use these programs with the G6 Sample virtual controller
'
' Sample Program 1
' Robot works on the center table.
'
Function main
	Integer i
	
	Motor On
	Power High
	Speed 100
	Accel 100, 100
	
	For i = 0 To 2
		Jump P0
		Wait 0.1
		Jump P1
		Wait 0.1
		Jump P2
	Next
	
	Jump P1 :Z(0)
Fend
'
' Sample Program 2
' Robot executes pick and place from Pallet 1 to Pallet 2.
'
Function main1
	Integer i
	
	Motor On
	Power High
	Speed 100
	Accel 100, 100
	
	Pallet 1, P10, P11, P12, P13, 3, 3
	Pallet 2, P15, P16, P17, P18, 3, 3
	
	For i = 1 To 9
		If (i Mod 2) > 0 Then
			Jump Pallet(1, i)
			Wait 0.1
			Jump Pallet(2, i)
		EndIf
	Next
	
	Jump P1 :Z(0)
Fend
'
' Sample Program 3
' Robot collides with a part.
'
Function main2
	Integer i
	
	Motor On
	Power High
	Speed 100
	Accel 100, 100
	
	For i = 0 To 2
		Jump P0
		Wait 0.1
		Jump P1
		Wait 0.1
		Jump P5
	Next
	
	Jump P1 :Z(0)
Fend

