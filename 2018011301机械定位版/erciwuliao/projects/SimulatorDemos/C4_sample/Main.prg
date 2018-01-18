'
' C4 Sample Project
'
' Use these programs with the C4 Sample virtual controller
'
'
' Sample Program 1
' Robot works on the center table.
'
Function main
	Integer i
	
	Motor On
	Power High
	Speed 100, 50, 50
	Accel 100, 100, 50, 50, 50, 50
	SpeedS 2000, 1000, 1000
	AccelS 20000, 20000, 10000, 10000, 10000, 10000
	
	Go XY(0, 450, 260, 90, 0, 180)
	
	For i = 0 To 2
		Jump3 Here -TLZ(50), P0 -TLZ(50), P0
		Wait 0.1
		Jump3 Here -TLZ(50), P1 -TLZ(50), P1
		Wait 0.1
		Jump3 Here -TLZ(50), P2 -TLZ(50), P2
	Next
	
	Go Here -TLZ(50)
	Go XY(0, 450, 260, 90, 0, 180)
Fend
'
' Sample Program 2
' Robot executes pick and place from Pallet 1 to Pallet 2.
'
Function main1
	Integer i
	
	Motor On
	Power High
	Speed 100, 50, 50
	Accel 100, 100, 50, 50, 50, 50
	SpeedS 2000, 1000, 1000
	AccelS 20000, 20000, 10000, 10000, 10000, 10000
	
	Pallet 1, P10, P11, P12, P13, 3, 3
	Pallet 2, P15, P16, P17, P18, 3, 3
	
	
	Go XY(0, 450, 260, 90, 0, 180)
	
	For i = 1 To 9
		If (i Mod 2) > 0 Then
			Jump3 Here -TLZ(50), Pallet(1, i) -TLZ(50), Pallet(1, i)
			Wait 0.1
			Jump3 Here -TLZ(50), Pallet(2, i) -TLZ(50), Pallet(2, i)
		EndIf
	Next
	
	Go Here -TLZ(50)
	Go XY(0, 450, 260, 90, 0, 180)
Fend
'
' Sample Program 3
' Robot collides with a part.
'
Function main2
	Integer i
	
	Motor On
	Power High
	Speed 100, 50, 50
	Accel 100, 100, 50, 50, 50, 50
	SpeedS 2000, 1000, 1000
	AccelS 20000, 20000, 10000, 10000, 10000, 10000
	
	Go XY(0, 450, 260, 90, 0, 180)
	
	For i = 0 To 2
		Jump3 Here -TLZ(50), P0 -TLZ(50), P0
		Wait 0.1
		Jump3 Here -TLZ(50), P1 -TLZ(50), P1
		Wait 0.1
		Jump3 Here -TLZ(50), P5 -TLZ(50), P5
	Next

	Go Here -TLZ(50)
	Go XY(0, 500, 260, 90, 0, 180)
Fend

