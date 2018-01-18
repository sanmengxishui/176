Global Int32 k
'Global Double xd
'Global Double yd
'Global Double ud
Global Real xd, yd, ud

Global String toks$(0)

Function main
	
	If Motor = Off Then
		Motor On
		Power High
		Speed 200
		Accel 100, 100
		
	EndIf
	OpenCom #1    '打开串口1
	Do
		Tool 0
	
		Go yi
		
		
		String data$
		Do
				Print #1, "yi"   '通过串口发送字符串A
				Input #1, data$    '将上位机发过来的数据读取出来存到字符串data$
				If data$ = "yi" Then
				    Print data$
					Exit Do
				EndIf
				Print data$
		Loop
		Go er
		Go san
		Go si
		Go wu
		Go liu
		
		Do
				Print #1, "er"   '通过串口发送字符串A
				Input #1, data$    '将上位机发过来的数据读取出来存到字符串data$
				
				If data$ = "NG" Then
				Print data$
				Else
					ParseStr data$, toks$(), ";"
					xd = Val(toks$(0))
					yd = Val(toks$(1))
					ud = Val(toks$(2))
					
					Print data$
					Exit Do
				EndIf
		Loop
		Go qi
		
		Do
				Print #1, "san"   '通过串口发送字符串A
				Input #1, data$    '将上位机发过来的数据读取出来存到字符串data$
				If data$ = "san" Then
				    Print data$
					Exit Do
				EndIf
				
		Loop
		Tool 1
		Go ba
		Wait 1
		Go jiu +X(xd) +Y(yd) +U(ud)
		Wait 3
	
	
	Loop
	
Fend
	
	
	
	
	
	
'	If Motor = Off Then
'		Motor Off
'	EndIf
'	OpenCom #1    '打开串口1
'	Do
'		String data$
'		Print #1, "yao1"   '通过串口发送字符串A
'		Input #1, data$    '将上位机发过来的数据读取出来存到字符串data$
'		Print data$
'		
'		If data$ = "OK1" Then
'		    'Print #1, "OK1"
'		Else
'			'Print #1, "NG1"
'		EndIf
'	
'		Tool 0
'		Go yi
'		Wait 0.5
'		
'		Print #1, "yao2"   '通过串口发送字符串A
'		Input #1, data$    '将上位机发过来的数据读取出来存到字符串data$
'		Print data$
'		
'		If data$ = "OK2" Then
'		    'Print #1, "OK2"
'		Else
'			'Print #1, "NG2"
'		EndIf
'
'		Tool 0
'		Go er
'		Wait 0.5
'		
'		Print #1, "yao3"   '通过串口发送字符串A
'		Input #1, data$    '将上位机发过来的数据读取出来存到字符串data$
'		Print data$
'		
'		If data$ = "OK3" Then
'		    'Print #1, "OK3"
'		Else
'			'Print #1, "NG3"
'		EndIf
'		
'		
'		Tool 1
'		Jump er1
'	Loop
	
	

'OpenCom #1    '打开串口1
'Do
	'Call communication
'Loop

'Fend
'Function communication
	
	
	
	
	
	
	
	
	
	
'    String toks$(0), data$   '定义字符串变量
'    Integer NGOK, charnum   '定义整形变量   
'    Real vision_x, vision_y, vision_u    '定义实数变量
'    vrun1:
'    Print #1, "A"   '通过串口发送字符串A
'	Input #1, data$    '将上位机发过来的数据读取出来存到字符串data$
'	'stop
'    Print data$  '在运行窗口打印接收到的字符串
'    ParseStr data$, toks$(), ";"   '将上位机数据以";"作为分隔符分解数据（数据格式:A;x;y;u;NGOK）
'    NGOK = Val(toks$(4))    '将字符串数据转换成数字赋值给变量NGOK
'    
'    If NGOK = 1 Then
'    	vision_x = Val(toks$(1))   '将字符串数据转换成数字赋值给变量vision_x
'    	vision_y = Val(toks$(2))    '将字符串数据转换成数字赋值给变量vision_y
'    	vision_u = Val(toks$(3))    '将字符串数据转换成数字赋值给变量vision_u
'    	Print "vision_x=", vision_x, "vision_y=", vision_y, "vision_u=", vision_u   '打印变量vision_x,vision_y,vision_u的值
'    Else
'    	Print "接收数据失败"
'    	GoTo vrun1
'    EndIf
'Fend

