Function main
	
	Motor On
	Power High
	
	Speed 100
	Accel 100, 100
	
	SpeedS 200
	AccelS 500, 500
	
	Weight 1
	Inertia 0.005
	
	P0 = XY(200, 200, -570, 135, -45, 90)
	P1 = XY(-200, -200, -570, -45, 45, 90)
	P2 = XY(200, -200, -570, 90, 0, 180)
	P3 = XY(-200, 200, -570, 90, 0, 180)
	P4 = XY(200, 200, -570, 135, -45, 90)
	
	P5 = XY(200, 200, -570, 135, -45, 90)
	P6 = XY(-200, -200, -600, 90, 0, 180)
	
	P7 = XY(200, 200, -370, 135, -45, 90)
	P8 = XY(200, 200, -670, 135, -45, 90)
	
	P9 = XY(0, 0, -620, 90, 0, 180)
	
	Go P0
	Go P1
	Go P2
	Go P3
	Go P4
	
	Wait 0.5
	
	'Minimum Joint 1 movement
	Go P1 LJM (1)
	Go P2 LJM (1)
	Go P3 LJM (1)
	Go P4 LJM (1)
	
	Wait 0.5
	
	'Hand orientation (Hand Flag) switch motion
	Go P1 LJM (5)
	Go P2 LJM (5)
	Go P3 LJM (6)
	Go P4 LJM (5)
	
	Wait 0.5
	
	'JumpTLZ:Jump motion for N series
	Go P5
	JumpTLZ P6, -30
	
	Go P7
	Go P8
	
	Wait 0.5
	
	'Elbow orientation (Elbow Flag) switch motion
	Go P7
	Go P8 LJM (7)
	
	Wait 0.5
	
	'Motion with the set value of J1 angle
	Go P9
	J1Angle P9, 90
	Go P9
	Wait 0.5
	J1Angle P9, 45
	Go P9
	
	Go Pulse(0, 0, 0, 0, 0, 0)
	
	
Fend
