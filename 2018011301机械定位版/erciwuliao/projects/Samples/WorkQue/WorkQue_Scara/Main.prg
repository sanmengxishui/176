Function main
	Integer queID
	
	Motor On
	Power High
	Speed 100
	Accel 100, 100
	
	Local 1, XY(0, 0, -145, 0)

	queID = 1
	WorkQue_AutoRemove queID, True
	Go WaitingPoint
	
	Do
		Jump WaitingPoint LimZ (-100)
		Wait 0.1
		VRun FindParts
		VGet FindParts.Geom01.AllRobotXYU, queID
				
		Do While WorkQue_Len(queID)
			Jump WorkQue_Get(queID) /L LimZ (-100) C0
			Wait 0.1
			Jump PlacePos LimZ (-100) C0
			Wait 0.1
		Loop
	Loop
Fend

