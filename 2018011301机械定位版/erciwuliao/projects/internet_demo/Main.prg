Function main
OpenNet #201 As Client
WaitNet #201
Do
   Call communication
Loop
Fend
Function communication
	String toks$(0), data$   '定义字符串变量
	Integer ccd_ngok, charnum   '定义整形变量   
	Real vision_x, vision_y, vision_u   '定义实数变量
vrun1:     '定义标签
	Print #201, "B"    '给相机发送拍照信号
	vrun2:
	charnum = ChkNet(201)  '检测端口是否收到数据
	If charnum < 0 Then
	   Print "以太网连接失败，重新连接以太网"
	   OpenNet #201 As Client    '打开201端口
       WaitNet #201   '等待端口连接成功
       GoTo vrun1   '无条件跳转到标签vrun1
    ElseIf charnum = 0 Then
    	GoTo vrun2 '无条件跳转到标签vrun2
    EndIf
	Input #201, data$   '从201端口读取相机发过来的数据
    Print "data$=", data$  '打印相机数据
	ParseStr data$, toks$(), ";"   '将相机数据以";"作为分隔符分解数据（数据格式:A;x;y;u;ccd_ngok）
	ccd_ngok = Val(toks$(4))  '将字符串变量转换为数字变量
	If ccd_ngok = 1 Then   '如果视觉拍照成功
		vision_x = Val(toks$(1))  '将字符串变量转换为数字变量
		vision_y = Val(toks$(2))  '将字符串变量转换为数字变量
		vision_u = Val(toks$(3))  '将字符串变量转换为数字变量
		Print "vision_x=", vision_x, "vision_y", vision_y     '打印视觉坐标值
		Print "vision_u", vision_u
	Else
		Print "视觉拍照失败 "
		GoTo vrun1
	EndIf
Fend

