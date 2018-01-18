'
' DrawEpsonLogo 
' EPSON RC+ 7.0 Sample database project
' Copyright (c) 2014 SEIKO EPSON CORPORATION
'
' < OVERVIEW >
' This is a sample database project which uses a robot to draw the EPSON Logo.
' The point data for the logo is loaded from an Excel file, then stored in a two dimension array.
' The array contains position data (x, y), which is then used by motion commands to draw the EPSON Logo.
'
' < PREPERATION >
' You must set the Local coordinates before starting this demo.
' The WriteLogo function has a Z axis position variable (rZValue = -180.0), but it should be set with your environment.
' This demo was made with the robot shown below.  If you use another robot type, you must set the Local and Position data for your robot.
'
' < SETTINGS >
' Robot: G6-453S
' Local1: -250, 250, 0, 0, 0, 0
' Microsoft Excel 2003 or 2007
'
' < POSITION DATA >
' File: EPSON_LOGO.xls
' Worksheet Name: LOGO
' Header: "Point","X","Y","Description"
' Column Point      : Order of the drowing groups.
' 　　　 X          : X position with Local 1
' 　　　 Y          : Y position with Local 1
'        Description: Drawing group name and IDs.
' 
' < OTHER DATABASE USAGE (Access or SQL) >
' Make position data from EPSON_LOGO.xls file. 

' Please change file name used on this line [OpenDB #501, Excel, "EPSON_LOGO.xls"] in the ReadPositionData function
' 
' (a) For Microsoft Access 2003 or 2007
'     If for example the database name is "C:\MyDataBase\Sample.accdb":
'       OpenDB #501, Access, "c:\MyDataBase\Sample.accdb"
' (b) For Microsoft SQL server 2000 or 2005
'     If for example the database name is Sample on the Local Server:
'       OpenDB #501, SQL, "(LOCAL server name)", "Sample"
'
' Next, get the position data from the Drawing group as shown below.
' From:
'  [....... = SelectDB(#501, "[LOGO$]", ...........)]
' 
' To: if table name is set as LOGO,
'  [....... = SelectDB(#501, "LOGO", ...........)]
'
Integer iEPosCount		'E write position count
Integer iPOLPosCount 	'PO Line write position count
Integer iPOCPosCount 	'PO Curve write position count
Integer iPILPosCount	'PI Line write position count
Integer iPICPosCount	'PI Curve write position count
Integer iSOPosCount		'SO write position count
Integer iSIPosCount		'SI write position count
Integer iOOPosCount		'OO write position count
Integer iOIPosCount		'OI write position count
Integer iNPosCount		'N write position count

Real rEposArray(20, 2)		'E position
Real rPOLposArray(10, 2)	'PO Line position
Real rPOCposArray(20, 2)	'PO Curve position
Real rPILposArray(10, 2)	'PI Line position
Real rPICposArray(10, 2)	'PI Curve position
Real rSOposArray(40, 2)		'SO position
Real rSIposArray(50, 2)		'SI position
Real rOOposArray(50, 2)		'OO position
Real rOIposArray(40, 2)		'OI position
Real rNposArray(20, 2)		'N position

Function main
	Call ReadPositionData
	Call DrawLogo
Fend

'Read positions of EPSON Logo from Excel sheet 
Function ReadPositionData
	Integer i
	Integer p
	String desc$
	
	OpenDB #501, Excel, "EPSON_LOGO.xls"

	'Read E position
	iEPosCount = SelectDB(#501, "[LOGO$]", "Description LIKE 'E%'", "Point ASC")
	For i = 0 To iEPosCount - 1
		Input #501, p, rEposArray(i, 0), rEposArray(i, 1), desc$
	Next
	'Read PO Line position
	iPOLPosCount = SelectDB(#501, "[LOGO$]", "Description LIKE 'POL%'", "Point ASC")
	For i = 0 To iPOLPosCount - 1
		Input #501, p, rPOLposArray(i, 0), rPOLposArray(i, 1), desc$
	Next
	'Read PO Curve position
	iPOCPosCount = SelectDB(#501, "[LOGO$]", "Description LIKE 'POC%'", "Point ASC")
	For i = 0 To iPOCPosCount - 1
		Input #501, p, rPOCposArray(i, 0), rPOCposArray(i, 1), desc$
	Next
	'Read PI Line position
	iPILPosCount = SelectDB(#501, "[LOGO$]", "Description LIKE 'PIL%'", "Point ASC")
	For i = 0 To iPILPosCount - 1
		Input #501, p, rPILposArray(i, 0), rPILposArray(i, 1), desc$
	Next
	'Read PI Curve position
	iPICPosCount = SelectDB(#501, "[LOGO$]", "Description LIKE 'PIC%'", "Point ASC")
	For i = 0 To iPICPosCount - 1
		Input #501, p, rPICposArray(i, 0), rPICposArray(i, 1), desc$
	Next
	'Read SO position
	iSOPosCount = SelectDB(#501, "[LOGO$]", "Description LIKE 'SO%'", "Point ASC")
	For i = 0 To iSOPosCount - 1
		Input #501, p, rSOposArray(i, 0), rSOposArray(i, 1), desc$
	Next
	'Read SI position
	iSIPosCount = SelectDB(#501, "[LOGO$]", "Description LIKE 'SI%'", "Point ASC")
	For i = 0 To iSIPosCount - 1
		Input #501, p, rSIposArray(i, 0), rSIposArray(i, 1), desc$
	Next
	'Read OO position
	iOOPosCount = SelectDB(#501, "[LOGO$]", "Description LIKE 'OO%'", "Point ASC")
	For i = 0 To iOOPosCount - 1
		Input #501, p, rOOposArray(i, 0), rOOposArray(i, 1), desc$
	Next
	'Read OI position
	iOIPosCount = SelectDB(#501, "[LOGO$]", "Description LIKE 'OI%'", "Point ASC")
	For i = 0 To iOIPosCount - 1
		Input #501, p, rOIposArray(i, 0), rOIposArray(i, 1), desc$
	Next
	'Read N position
	iNPosCount = SelectDB(#501, "[LOGO$]", "Description LIKE 'N%'", "Point ASC")
	For i = 0 To iNPosCount - 1
		Input #501, p, rNposArray(i, 0), rNposArray(i, 1), desc$
	Next

	CloseDB #501
	
Fend

'Draw the EPSON Logo 
Function DrawLogo
	Integer i
	Real rZValue
	
	Motor On
	
	'Start position
	Go XY(0, 0, 0, 0) /1
	rZValue = -180.0
	
	'Draw E
	For i = 0 To iEPosCount - 1
		Move XY(rEposArray(i, 0), rEposArray(i, 1), rZValue, 0) /1
	Next
	Move Here +Z(10)
	
	'Draw P outside line 
	For i = 0 To iPOLPosCount - 1
		Move XY(rPOLposArray(i, 0), rPOLposArray(i, 1), rZValue, 0) /1
	Next

	'Draw P outside curve  
	CP On
	For i = 0 To iPOCPosCount - 1
		Move XY(rPOCposArray(i, 0), rPOCposArray(i, 1), rZValue, 0) /1
	Next
	CP Off
	Move Here +Z(10)
	
	'Draw P inside line
	For i = 0 To iPILPosCount - 1
		Move XY(rPILposArray(i, 0), rPILposArray(i, 1), rZValue, 0) /1
	Next

	'Draw P inside curve
	CP On
	For i = 0 To iPICPosCount - 1
		Move XY(rPICposArray(i, 0), rPICposArray(i, 1), rZValue, 0) /1
	Next
	Move Here +Z(10)

	'Draw S outside
	For i = 0 To iSOPosCount - 1
		Move XY(rSOposArray(i, 0), rSOposArray(i, 1), rZValue, 0) /1
	Next

	'Draw S inside
	For i = 0 To iSIPosCount - 1
		Move XY(rSIposArray(i, 0), rSIposArray(i, 1), rZValue, 0) /1
	Next
	Move XY(rSOposArray(0, 0), rSOposArray(0, 1), rZValue, 0) /1
	Move Here +Z(10)

	'Draw O outside
	For i = 0 To iOOPosCount - 1
		Move XY(rOOposArray(i, 0), rOOposArray(i, 1), rZValue, 0) /1
	Next
	Move Here +Z(10)

	'Draw O inside
	For i = 0 To iOIPosCount - 1
		Move XY(rOIposArray(i, 0), rOIposArray(i, 1), rZValue, 0) /1
	Next
	Move Here +Z(10)
	CP Off

	'Draw N
	For i = 0 To iNPosCount - 1
		Move XY(rNposArray(i, 0), rNposArray(i, 1), rZValue, 0) /1
	Next
	
	Go XY(0, 0, 0, 0) /1
	Motor Off

Fend

