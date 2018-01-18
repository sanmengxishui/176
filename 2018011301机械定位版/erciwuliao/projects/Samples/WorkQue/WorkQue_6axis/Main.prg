Function main
	Integer queID
	
	Motor On
	Power High
	Speed 100
	Accel 100, 100
	SpeedS 2000
	AccelS 5000, 5000
	
	Local 1, XY(0, 0, 160, 0, 0, -180)
	
	queID = 1
	WorkQue_AutoRemove queID, False
	
	Go WaitingPoint
	
	Do
		Jump3 Here -TLZ(50), WaitingPoint -TLZ(50), WaitingPoint
		Wait 0.1
		VRun FindParts
		VGet FindParts.Geom01.AllRobotXYU, queID
				
		Do While WorkQue_Len(queID)
			Jump3 Here -TLZ(50), WorkQue_Get(queID) -TLZ(50), WorkQue_Get(queID) C0
			WorkQue_Remove queID
			Wait 0.1
			Jump3 Here -TLZ(50), PlacePos -TLZ(50), PlacePos C0
			Wait 0.1
		Loop
	Loop
Fend

