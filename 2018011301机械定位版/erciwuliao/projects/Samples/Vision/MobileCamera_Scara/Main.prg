'
' EPSON RC+ 7.0 Sample Project
'
' Pick and Place with mobile camera demo
'
' To run, click the RUN tool bar button, then click Start
'
#define PickZ (-100.0)
#define PlaceZ (-100.0)

Function main
	Boolean found
	Real x, y, u
	Integer i, count
	
	Motor On
	
	Power High
	Speed 20
	Accel 40, 40
	
	'Camera view position	
	camViewPt = XY(60, 260, 0, 0)
	'Position to put parts
	placePt = XY(200, 280, PlaceZ, 0)
	'Standby position
	standbyPt = XY(170, 230, 0, 0)
	
	For i = 0 To 10
		Jump camViewPt
		Wait 0.1
		count = 0;
		Do
			VRun findparts
			VGet findparts.Geom01.RobotXYU, found, x, y, u
			count = count + 1;
			If count > 100 Then
				Print "No parts were found"
				Exit For
			EndIf
		Loop Until found
		pickPt = XY(x, y, PickZ, u)
		Jump pickPt
		Wait 0.1
		Jump placePt
		Wait 0.1
	Next
		
	Jump standbyPt
	
	Motor Off
Fend

