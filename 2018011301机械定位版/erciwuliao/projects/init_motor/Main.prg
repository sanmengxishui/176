Global Short i
Function main
	For i = 1 To 40
		Print "打印第" + Str$(i)
	Next
'	Do
'	TmReset 0
'	Wait 1.254
'	Print "time1:", Tmr(0)
'	'TmReset 0
'	Wait 1.346
'	Print "time2:", Tmr(0)
'	'TmReset 0
'	Wait 1.890
'	Print "time3:", Tmr(0)
'	Loop
Fend
Function initmotor
	If Motor = Off Then
		Motor On
	EndIf
	Power High
	Speed 10 '定义Jump以及Go的速度最大速度是百分之百
	Accel 5, 5 '定义Jump以及Go的加速度最大的加速度是120
Fend


