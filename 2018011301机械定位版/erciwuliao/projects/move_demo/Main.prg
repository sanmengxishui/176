Function main
Call init
Do
Go P1
Jump P2 C0
Move P3
Arc P4, P5
Go P1 +Z(20) :Y(100)
Loop
Fend
Function init
	If Motor = Off Then    '如果伺服没有ON则打开伺服
		Motor On  '打开伺服
	EndIf
	Power High   '设定运行功率为高功率
	Speed 10    '定义jump、go等PTP运动速度，单位为百分比，最大为100
	Accel 50, 50  '定义jump、go等PTP运动加速度，单位为百分比，最大为120
	SpeedS 500  '定义move、arc、arc3等CP运动速度，单位mm/s,最大2000
	AccelS 2000, 2000  '定义move、arc、arc3等CP运动速度，单位mm/s2,最大20000
	Jump daiji
Fend


