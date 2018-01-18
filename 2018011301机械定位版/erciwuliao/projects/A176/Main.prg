Global String SWVersion$ '版本号
Global String SetMes$
Global Preserve Real ModX(3), ModY(3), ModR(3) '定义实数变量
Global String PutOrGet$ '是开始取料还是放料
'定义取料产品的状态，0表示没有料，1表示OK料，2表示需要复测料，3表示最终NG料，4表示拍照读码失败料，5表示BinA料，6表示BinB料
Global Real Mod1CGstu, Mod2CGstu
Global Int32 ToA162Mod1, ToA162Mod2 '定义用于给定A162产品状态，0表示未测试过，1表示复测产品
Global String ReNGCG$ '复测试料
Global Int32 sque '跳转的位置
Global Short GetCGTime '机械手取料时间
Global Short PutCGTime '机械手放料时间
Global Int32 ReCG '定义查询是否需要复测试的料
Global Short cylinder_AlarmTime '气缸报警时间
Global Int32 NGTrayCount(5) 'NG料盘一共是六个每一个料盘放6片
Global Integer NgCount ' 一个NG坑可以放几片料
Global Integer CurrentNGTray '当前放在哪个坑
Global Int32 OKTrayCount 'CG的OK数,目前是6片
Global Integer OkCount '一个坑可以放几片OK料
Global Integer errnum
Global Boolean IsOriange
Global Preserve Short PrMod1CG '工位1保存从A162机台取下的料的情况和Mod1CGstu一致
Global Preserve Short PrMod2CG '工位2保存从A162机台取下的料的情况和Mod2CGstu一致
Global Preserve Short PrA162CurMod '发生故障时A162所处的转盘号
Global Preserve Short PrStep '机械手所处的是抓料还是放料的哪一步
Global Boolean FirstBegin '是否跳过去复测位置取料
Global Short CurRolMod '当前转盘的工位
Global Boolean TkPh1OK '1工位拍照是否OK
Global Boolean TkPh2OK '2工位拍照是否OK
Global Boolean EndLot '设备A176是否点击了EndLot,传入的参数是“AbleEndLot”
Global Boolean A162EndLot 'A162是否点击了EndLot，传入的参数是“A162EndLot”

Function main
	Int32 timeci '前面几次是否开始最高加速
	timeci = 0
    SWVersion$ = "18.01.13.12" '改为机械定位
	IsOriange = False
	CollisionDetect Off '急停时,刹不住取消报警
	Call Init '初始化程序参数

	Wait Sw(5) = On Or Sw(6) = On  '回原完成，等待程序启动
	
	If Sw(5) = On Then '如果勾选了GRR，又按了启动，那么开始跑GRR
		If ReTestGRR = 0 Then
			Call TestOperation
			Exit Function
		EndIf
	EndIf

	If Sw(6) = On Then '回原完成以后是否进行标定相机
		If jiudianbiaoding = 0 Then
			Call ReceMes("BiaodingOK") '通知电脑标定成功
			Exit Function
		EndIf
		Call ReceMes("BiaodingNG") '通知电脑标定失败
		Exit Function
	EndIf
	
	TmReset 0
	
	If ReA162CurrectMod("A162CurrectMod") < 0 Then '检查A162设备是否已经启动，如果没有启动那么就停止运行
		Exit Function
	EndIf
	
	sque = OperetionCG("") '返回值就能代表需要返回的是哪一步
	If sque < 0 Then '机械手复位完成后初次运行，处理手爪的料
		ReceMes("ReCGInitNG") '返回值是小于0的说明机械手爪的料的复测平台料有冲突，必须将平台料取走
		Exit Function
	EndIf
	
	Power Low '设定运行功率为高功率
	Speed 40    '定义jump、go等PTP运动速度，单位为百分比，最大为100
	Accel 50, 50  '定义jump、go等PTP运动加速度，单位为百分比，最大为120
	SpeedS 100  '定义move、arc、arc3等CP运动速度，单位mm/s,最大2000
	AccelS 200, 200  '定义move、arc、arc3等CP运动速度，单位mm/s2,最大20000
	
	'自动流程开始运行
	Do
		OnErr GoTo errHandler
		Tool 0 '切换成机械手坐标
		Int32 res
		Select sque
			Case 0
				If timeci < 2 Then
					timeci = timeci + 1
				Else
					If ReSpeed("RobotSpeed") < 0 Then '速度可由软件设置
						Print "速度读取失败"
						Exit Function
					EndIf
				EndIf

				res = SeqGetCount() '读取当前每个料槽的数据情况
				If res = 0 Then
					sque = 1
				ElseIf res < 0 Then
					Exit Function '返回小于零的值那么就退出自动，等待PC指示
				EndIf
			Case 1
				res = SeqPutGetCG() '此处的取料与复测试位置相反，
				If res = 0 Then
					sque = 2
				ElseIf res < 0 Then
					Exit Function '返回小于零的值那么就退出自动，等待PC指示
				EndIf
			Case 2
				res = SeqPutGetReCG() '从复测试NG处拿料
				If res = 0 Then
					sque = 3
				ElseIf res = 1 Then
					sque = 1 '如果都是从复测试位置取下的料那么就没有必要再去取料位，直接去照相位
				ElseIf res < 0 Then
					Exit Function '返回小于零的值那么就退出自动，等待PC指示
				EndIf
			Case 3
				FirstBegin = True
				res = SeqTakePhone1() '相机拍照1
				If res = 0 Then
					sque = 4
				ElseIf res = -1 Then
					sque = 1
				Else
					Exit Function '返回小于零的值那么就退出自动，等待PC指示	
				EndIf
			Case 4
				FirstBegin = True
				res = SeqTakePhone2() '相机拍照2
				If res = 0 Then
					sque = 5
				ElseIf res = -1 Then
					sque = 1
				Else
					Exit Function '返回小于零的值那么就退出自动，等待PC指示	
				EndIf
			Case 5
				FirstBegin = True
				res = SeqExchageMes() '与A162交换料的信息，再决定是先取料还是先放料
				If res = 0 Then
					sque = 6 '正常情况下开始取料
				ElseIf res = 1 Then
					sque = 5 '等于1说明当前的情况是没有结果但是却有料
				ElseIf res < 0 Then
					Exit Function '返回小于零的值那么就退出自动，等待PC指示
				EndIf
			Case 6
				FirstBegin = True
				res = SeqGetGetCG() '下料部分取料
				If res = 0 Then
					sque = 7
				ElseIf res < 0 Then
					Exit Function '返回小于零的值那么就退出自动，等待PC指示
				EndIf
			Case 7
				FirstBegin = True
				res = SeqPutPutCG() '放料部分放料
				If res = 0 Then
					sque = 8
				ElseIf res < 0 Then
					Exit Function '返回小于零的值那么就退出自动，等待PC指示
				EndIf
			Case 8
				FirstBegin = True
				res = SeqPutFristNGCG() '放至第一次NG料
				If res = 0 Then
					sque = 9 '返回值是0说明没有二次NG料
				ElseIf res < 0 Then
					Exit Function '返回小于零的值那么就退出自动，等待PC指示
				EndIf
			Case 9
				FirstBegin = True
				res = SeqPutSecondNGCG() '放至第二次NG料
				If res = 0 Then
					sque = 10
				ElseIf res = 2 Then
					sque = 9
				ElseIf res < 0 Then
					Exit Function '返回小于零的值那么就退出自动，等待PC指示
				EndIf
			Case 10
				FirstBegin = True
				res = SeqPutOKCG() '放至OK料
				If res = 0 Then
					sque = 0
				ElseIf res = 2 Then
					sque = 10
				ElseIf res = 5 Then
					sque = 5
				ElseIf res < 0 Then
					Exit Function '返回小于零的值那么就退出自动，等待PC指示
				EndIf
			Default
				Print "wu"
		Send
errHandler:
    errnum = Err
    If errnum > 0 Then
         Print #201, Err, ErrMsg$(Err)
    EndIf

	Loop
Fend
Function Init '初始化程序
	String rece$
	If Motor = Off Then    '如果伺服没有ON则打开伺服		
		Motor On  '打开伺服
	EndIf
	
	'OpenCom #1 '打开串口#1
	'CloseNet #201
	Wait 0.5
	OpenNet #201 As Client  '打开201端口
	WaitNet #201   '等待端口连接成功
	
	If ChkCom(1) > 0 Then
		Input #201, rece$
	EndIf
	Tool 0 '转换成机械手坐标
	sque = 0 '机械手所走的步数

	Call ReceMes("RobotOriginNG") '清空回原标志位
	
	NGTrayCount(0) = 0 '第1个坑放BinA产品
	NGTrayCount(1) = 0 '第2个坑放BinB产品
	NGTrayCount(2) = 0 '第3个坑放扫码拍照失败产品
	NGTrayCount(3) = 0 '第4个坑放NG产品
	NGTrayCount(4) = 0 '第5个坑放NG(专供工位1)产品
	NGTrayCount(5) = 0 '第6个坑放NG(专供工位1)产品
	NgCount = 21 '一个坑可以放几片二次NG料
	
	OKTrayCount = 0 'CG的OK数,目前是6片
	OkCount = 25 '一个坑可以放几片OK料
	
	GetCGTime = 0.2 '机械手取料时间
	PutCGTime = 0.2 '机械手放料时间
	cylinder_AlarmTime = 3 '气缸报警时间
	
	FirstBegin = True '不跳过检测复测平台检查
	TkPh1OK = False '需要拍照
	TkPh2OK = False '
	
	If Sw(20) = Off Then
		Off 12
	EndIf
	If Sw(21) = Off Then
		Off 13
	EndIf
	If Sw(22) = Off Then
		Off 14
	EndIf
	If Sw(23) = Off Then
		Off 15
	EndIf
	
	'所有气缸初始化，吸气不初始化
	Off 4 '机械手取料完成给PLC信号
	Off 5 '
	
	Off 8
	Off 9
	Off 10
	Off 11
	Wait Sw(13) = On And Sw(15) = On And Sw(17) = On And Sw(19) = On, cylinder_AlarmTime '气缸报警3S
	If TW Then
	 	Call ReceMes("GetModAirNG")
	 	Quit main
	EndIf

	Power Low '设定运行功率为高功率
	Speed 40    '定义jump、go等PTP运动速度，单位为百分比，最大为100
	Accel 50, 50  '定义jump、go等PTP运动加速度，单位为百分比，最大为120
	SpeedS 500  '定义move、arc、arc3等CP运动速度，单位mm/s,最大2000
	AccelS 800, 800  '定义move、arc、arc3等CP运动速度，单位mm/s2,最大20000
	
	Mod1CGstu = 0 '工位1产品状态清零
	Mod2CGstu = 0 '工位2产品状态清零
    
    ToA162Mod1 = 0 '给于A162机台的信号，0表示未测试，1表示第一次NG
    ToA162Mod2 = 0
    
    ReCG = 0 '需要互补的拿料
    ReNGCG$ = "0" '复测试料
    
	Call daijidian '走到待机位置
	Call ReceMes("RobotOriginOK") '回原完成标志位
Fend
Function ReceMes(ByVal setdata$ As String) As Int32
	String rece$
	Do
		Print #201, setdata$   '通过串口发送字符串A
		Input #201, rece$    '将上位机发过来的数据读取出来存到字符串data$
		Print rece$
		If setdata$ = rece$ Then
			Exit Do
		ElseIf rece$ = "A162Stopping" Then
			ReceMes = -3 'A162设备已停止
			Exit Do
		Else
		EndIf
	Loop
	ReceMes = 0
Fend
Function ReTakePhoneCG(ByVal setdata$ As String, ByVal t1 As Int32) As Int32
	String toks$(0), data$   '定义字符串变量
	Do
		Print "拍照" + Str$(t1)
	    Integer NGOK, charnum   '定义整形变量   
	    Print #201, setdata$ + ";" + Str$(t1)   '通过串口发送字符串A
	    'Wait 0.2
		Input #201, data$    '将上位机发过来的数据读取出来存到字符串data$
	    Print data$  '在运行窗口打印接收到的字符串
	    ParseStr data$, toks$(), ";"   '将上位机数据以";"作为分隔符分解数据（数据格式:A;x;y;u;NGOK）

		If UBound(toks$) = 2 Then '如果分开的数据是等于4位则可以
			ModX(t1) = Val(toks$(0))   '将字符串数据转换成数字赋值给变量vision_x
	    	ModY(t1) = Val(toks$(1))    '将字符串数据转换成数字赋值给变量vision_y
	    	ModR(t1) = Val(toks$(2))    '将字符串数据转换成数字赋值给变量vision_u
	    	Print "ModX=", ModX(t1), " ModY=", ModY(t1), " ModR=", ModR(t1)   '打印变量vision_x,vision_y,vision_u的值
	    	ReTakePhoneCG = 0
	    	Exit Function
	    ElseIf data$ = "TakePhoneNG" Then '如果返回值是这那么拍照失败
	        ReTakePhoneCG = -1
	    	Print "拍照失败"
	    	Exit Function
	    EndIf
	Loop
Fend
Function ReceCGStatu$() As String
	String toks$(0), data$, setdata$  '定义字符串变量
	Do
	    Integer NGOK, charnum   '定义整形变量   
	    setdata$ = "compelet;" + Str$(ToA162Mod1) + ";" + Str$(ToA162Mod2) + ";end"
	    
	    Print #201, setdata$   '通过串口发送字符串A
	    Print "交换信息" + setdata$
		Input #201, data$    '将上位机发过来的数据读取出来存到字符串data$
	    Print data$  '在运行窗口打印接收到的字符串
	    ParseStr data$, toks$(), ";"   '将上位机数据以";"作为分隔符分解数据（数据格式:A;x;y;u;NGOK）

		If UBound(toks$) = 4 Then '如果分开的数据是等于4位则可以
        '收回的数据，0表示没有产品，1表示OK产品，2表示一次NG，3表示二次NG
	        If toks$(0) = "compeleted" And toks$(4) = "end" Then
	        	ReceCGStatu$ = toks$(0)   '是开始取料还是放料
				Mod1CGstu = Val(toks$(1))   '将字符串数据转换成数字赋值给变量vision_x
		    	Mod2CGstu = Val(toks$(2))    '将字符串数据转换成数字赋值给变量vision_y
		    	CurRolMod = Val(toks$(3))   '当前A162所在的转盘位置
		    	If Mod1CGstu <> 0 And Mod2CGstu <> 0 Then '如果只有一种情况，一个为空，一个不为空，这个时候应该是要报警的
		    		ReceCGStatu$ = "Y"
		    	Else
		    		ReceCGStatu$ = "N"
		    	EndIf
		    	Print "Mod1CGstu=", Mod1CGstu, "Mod2CGstu=", Mod2CGstu
		    	'改变复测试位的取料状态
		    	If Mod1CGstu = 2 And Mod2CGstu = 2 Then
		    		ReCG = 3 '复测试位全测试
		    	ElseIf Mod1CGstu = 2 And Mod2CGstu <> 2 Then
		    		ReCG = 2 '复测试1位测试
		    	ElseIf Mod1CGstu <> 2 And Mod2CGstu = 2 Then
		    		ReCG = 1 '复测试2位测试
		    	EndIf
		    		ReCG = 0
		    	Exit Do
	        EndIf
	    ElseIf data$ = "A162Stopping" Then
	    	ReceCGStatu$ = "-3" 'A162设备未开启
	    	Exit Do
        EndIf
        	Print "接收数据失败"
	Loop
Fend
Function ReNGCGstu(ByVal setdata$ As String) As Int32
	String data$   '定义字符串变量
	Do
	    Print #201, setdata$   '通过串口发送字符串A
		Input #201, data$    '将上位机发过来的数据读取出来存到字符串data$
	    Print data$  '在运行窗口打印接收到的字符串
		If data$ = "0" Or data$ = "1" Or data$ = "2" Or data$ = "3" Then
		'0表示没有复测试料，1表示1工位有复测试料，2表示2工位有复测试料,3表示两个工位都有
		    ReNGCGstu = Val(data$)
		    Print data$
		    Exit Function
	    Else
	    	Print "复测试工位数据接收失败"
	    EndIf
	Loop
Fend
Function ReReadCount(ByVal setdata$ As String) As Int32
	ReReadCount = 0
	String toks$(0), data$   '定义字符串变量
	Do
	    Print #201, setdata$   '通过串口发送字符串A
		Input #201, data$    '将上位机发过来的数据读取出来存到字符串data$
	    Print data$  '在运行窗口打印接收到的字符串
	    ParseStr data$, toks$(), ";"   '将上位机数据以";"作为分隔符分解数据（数据格式:A;x;y;u;NGOK）
		If UBound(toks$) = 7 Then '如果分开的数据是等于5位则可以
			If toks$(0) = setdata$ Then
				NGTrayCount(0) = Val(toks$(1))   '将字符串数据转换成数字赋值给变量
		    	NGTrayCount(1) = Val(toks$(2))    '将字符串数据转换成数字赋值给变量
		    	NGTrayCount(2) = Val(toks$(3))   '将字符串数据转换成数字赋值给变量
		    	NGTrayCount(3) = Val(toks$(4))
		    	OKTrayCount = Val(toks$(5))
		    	NGTrayCount(4) = Val(toks$(6))
		    	NGTrayCount(5) = Val(toks$(7))
		    	Exit Do
			EndIf
	    ElseIf data$ = "ReadCountNG" Then
	        ReReadCount = -1
	    	Print "读取NG"
	    	Exit Do
	    EndIf
	Loop
Fend
Function ReWriteCount() As Int32
	ReWriteCount = 0
	String setdata$, data$   '定义字符串变量
	Do
		setdata$ = "WriteCount" + ";" + Str$(NGTrayCount(0)) + ";" + Str$(NGTrayCount(1)) + ";" + Str$(NGTrayCount(2)) + ";" + Str$(NGTrayCount(3)) + ";" + Str$(OKTrayCount) + ";" + Str$(NGTrayCount(4)) + ";" + Str$(NGTrayCount(5))
	
	    Print #201, setdata$   '通过串口发送字符串A
	    Print setdata$
		Input #201, data$    '将上位机发过来的数据读取出来存到字符串data$
	    Print data$  '在运行窗口打印接收到的字符
	    
	    If "WriteCount" = data$ Then
	    	ReWriteCount = 0
	    	Print "获取成功"
	    	Exit Do
	    ElseIf data$ = "WriteCountNG" Then
	        ReWriteCount = -1
	    	Print "获取NG"
	    	Exit Do
	    EndIf
	Loop
Fend
Function ReA162CurrectMod(ByVal setdata$ As String) As Int32
	ReA162CurrectMod = -1
	String toks$(0), data$   '定义字符串变量
	Do
	    Print #201, setdata$   '通过串口发送字符串A
		Input #201, data$    '将上位机发过来的数据读取出来存到字符串data$
	    Print data$  '在运行窗口打印接收到的字符串
	    ParseStr data$, toks$(), ";"   '将上位机数据以";"作为分隔符分解数据（数据格式:A;x;y;u;NGOK）
		If UBound(toks$) = 3 And toks$(0) = setdata$ Then '
			ReA162CurrectMod = Val(toks$(1))
			CurRolMod = Val(toks$(2))
	    	Exit Do
	    ElseIf data$ = "CurrectModNG" Then
	    	Print "读取当前A162工位是否有料失败"
	    	Exit Do
	    ElseIf data$ = "A162Stopping" Then
	    	ReA162CurrectMod = -3 'A162设备未开启
	    	Exit Do
	    EndIf
	Loop
Fend
Function ReA162EndLot(ByVal setdata$ As String) As Int32
	ReA162EndLot = -1
	String toks$(0), data$   '定义字符串变量
	Do
	    Print #201, setdata$  '通过串口发送字符串A
		Input #201, data$    '将上位机发过来的数据读取出来存到字符串data$
	    Print data$  '在运行窗口打印接收到的字符串
	    ParseStr data$, toks$(), ";"   '将上位机数据以";"作为分隔符分解数据（数据格式:A;x;y;u;NGOK）
		If UBound(toks$) = 2 Then
			If toks$(0) = setdata$ And toks$(2) = "end" Then
				If toks$(1) = "True" Then
					If toks$(0) = "AbleEndLot" Then
						EndLot = True
					ElseIf toks$(0) = "A162EndLot" Then
						A162EndLot = True
					EndIf
				EndIf
				If toks$(1) = "False" Then
					If toks$(0) = "AbleEndLot" Then
						EndLot = False
					ElseIf toks$(0) = "A162EndLot" Then
						A162EndLot = False
					EndIf
				EndIf
				ReA162EndLot = 0
				Exit Do
			EndIf
		ElseIf data$ = "A162Stopping" Then
	    	ReA162EndLot = -3 'A162设备未开启
	    	Exit Do
	    ElseIf data$ = "UnableEndLot" Then
	    	ReA162EndLot = 0
	    	Exit Do
	    EndIf
	Loop
Fend
Function ReNGTray(ByVal setdata$ As String) As Int32
	ReNGTray = -1
	Integer cierror
	cierror = 0
	PrStep = 0
	Do
		ReNGTray = ReReadCount("ReadCount")
		If ReNGTray < 0 Then
			cierror = cierror + 1
			If cierror = 5 Then
				ReNGTray = -1
				Exit Function
			EndIf
		EndIf
		Integer ci '循环所有NG料盘，查询是否有超过容量的
		Integer Mod1ci '获得工位1应该对应的料盘
		Boolean NGTrayOK
		For ci = 0 To 3 '循环1次4个NG料坑
			If NGTrayCount(ci) >= NgCount Then
				NGTrayOK = True '有料盘放满还未清空
				Exit For
			EndIf
			For Mod1ci = 4 To 5 '循环第5和第6个盒子是否已经将NG料盒放满
				If NGTrayCount(Mod1ci) < NgCount Then
					CurrentNGTray = Mod1ci '获得当前1工位1爪放置的NG料位置
					Exit For
				EndIf
				CurrentNGTray = 3 '如果工位要放置的两个料盘都满了，那么就放到工位2的料盘里面去
			Next
			NGTrayOK = False 'NG料盘还没有放满，还可以继续放料		
		Next
		'Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
        If NGTrayOK Or OKTrayCount >= OkCount Then
			'ReceMes("ALLNGTrayCount") '所有NG料盘放满，没有地方了
		Else
			Exit Do
		EndIf
		Wait 1
	Loop
Fend
Function ReSpeed(ByVal setdata$ As String) As Int32
	ReSpeed = -1
	String toks$(0), data$   '定义字符串变量
	Do
	    Print #201, setdata$  '通过串口发送字符串A
		Input #201, data$    '将上位机发过来的数据读取出来存到字符串data$
	    Print data$  '在运行窗口打印接收到的字符串
	    ParseStr data$, toks$(), ";"   '将上位机数据以";"作为分隔符分解数据（数据格式:A;x;y;u;NGOK）
		If UBound(toks$) = 3 Then
			If toks$(0) = setdata$ Then
				If toks$(1) = "High" Then
					Power High '设定运行功率为高功率
				ElseIf toks$(1) = "Low" Then
					Power Low '设定运行功率为高功率
				Else
					Power Low '设定运行功率为高功率
				EndIf
				
				If 0 <= Val(toks$(2)) And Val(toks$(2)) <= 100 Then
					Speed Val(toks$(2))    '定义jump、go等PTP运动速度，单位为百分比，最大为100
				Else
					Speed 5    '定义jump、go等PTP运动速度，单位为百分比，最大为100
				EndIf
					
				If 0 <= Val(toks$(3)) And Val(toks$(3)) <= 120 Then
					Accel Val(toks$(3)), Val(toks$(3))  '定义jump、go等PTP运动加速度，单位为百分比，最大为120
				Else
					Accel 10, 10  '定义jump、go等PTP运动加速度，单位为百分比，最大为120
				EndIf
				
				SpeedS 100  '定义move、arc、arc3等CP运动速度，单位mm/s,最大2000
				AccelS 200, 200  '定义move、arc、arc3等CP运动速度，单位mm/s2,最大20000
				
				ReSpeed = 0
				Exit Do
			EndIf
	    EndIf
	Loop
Fend
Function ReVacuum(ByVal setdata$ As String) As Int32
	ReVacuum = -1
	String toks$(0), data$   '定义字符串变量
	Do
	    Print #201, "Vacuum;" + setdata$ + ";end"  '通过串口发送字符串A
		Input #201, data$    '将上位机发过来的数据读取出来存到字符串data$
	    Print data$  '在运行窗口打印接收到的字符串
	    ParseStr data$, toks$(), ";"   '将上位机数据以";"作为分隔符分解数据（数据格式:A;x;y;u;NGOK）
		If UBound(toks$) = 2 Then
			If toks$(0) = "Vacuum" And toks$(1) = setdata$ Then '发送回来的工位等于我收到的工位。OK
				ReVacuum = 0
				Exit Do
			EndIf
		ElseIf data$ = "A162Stopping" Then
	    	ReVacuum = -3 'A162设备未开启
	    	Exit Do
	    EndIf
	Loop
Fend

Function SeqGetCount() As Int32 '获取各个料槽的当前的状态
	'Wait Sw(8) = On '等待PLC给信号，机械手抓料
	SeqGetCount = 0
	PrStep = 0
	Do
		SeqGetCount = ReReadCount("ReadCount")
		If SeqGetCount < 0 Then
			SeqGetCount = -1 '读取数据失败
			Exit Function
		EndIf
		Integer ci '循环所有NG料盘，查询是否有超过容量的
		Integer Mod1ci '获得工位1应该对应的料盘
		Boolean NGTrayOK
		For ci = 0 To 3 '循环1次4个NG料坑
			If NGTrayCount(ci) >= NgCount Then
				NGTrayOK = True '有料盘放满还未清空
				Exit For
			EndIf
			For Mod1ci = 4 To 5 '循环第5和第6个盒子是否已经将NG料盒放满
				If NGTrayCount(Mod1ci) < NgCount Then
					CurrentNGTray = Mod1ci '获得当前1工位1爪放置的NG料位置
					Exit For
				EndIf
				CurrentNGTray = 3 '如果工位要放置的两个料盘都满了，那么就放到工位2的料盘里面去
			Next
			NGTrayOK = False 'NG料盘还没有放满，还可以继续放料		
		Next
		If NGTrayOK Or OKTrayCount >= OkCount Then
			'NG料盒和OK料盒都满了，这个时候继续循环，直到清空料盒
		Else
			Exit Do
		EndIf
		Wait 1 '在查询等待的时候不需要时时查询	
	Loop
	
	If FirstBegin Then '首次开机运行，判定是否有需要复测的NG料	
		ReCG = ReNGCGstu("ReNGCGstu") '返回值是0，没有需要复测的料，返回值大于0需要复测
		If ReCG = 0 Then
		    ToA162Mod1 = 0
		    ToA162Mod2 = 0
		EndIf
		If ReCG = 1 Or ReCG = 2 Or ReCG = 3 Then
			If ReCG = 1 Then
			    ToA162Mod1 = 1
			    ToA162Mod2 = 0
			EndIf
			If ReCG = 2 Then
			    ToA162Mod1 = 0
			    ToA162Mod2 = 1
			EndIf
			If ReCG = 3 Then
			    ToA162Mod1 = 1
			    ToA162Mod2 = 1
			EndIf
		EndIf
	EndIf
Fend
Function SeqPutGetCG() As Int32 '放料部分拿料
	Print "time14:", Tmr(0)
	TmReset 0
	
	SeqPutGetCG = 0
	PrStep = 1 '记录现在的步数是第一步
	If ToA162Mod1 = 0 Or ToA162Mod2 = 0 Then
		Go InitPos
	    'Wait Sw(8) = On '等待PLC给信号，机械手抓料
	     Do
			If ReA162EndLot("AbleEndLot") < 0 Then
				SeqPutGetCG = -2 '读取EndLot失败
				Exit Function
			EndIf
	    
	    	If EndLot Then
	    		Exit Do
	    	EndIf
	    	
	    	If Sw(8) = On Then
	    		Exit Do
	    	EndIf
	    Loop
	    If EndLot = False Then
	    	Wait Sw(8) = On '以免发生撞击的可能
	    	
	    	Go PutGetCGPos
	    	Print "time2:", Tmr(0)
			TmReset 0
			
			If ToA162Mod1 = 0 And ToA162Mod2 = 0 Then
			    SeqPutGetCG = PutGetCGRes(3, True) '从取料处取料全取   
			ElseIf ToA162Mod1 = 1 And ToA162Mod2 = 0 Then '机械手1号工位已经有料，则2号补料
			    SeqPutGetCG = PutGetCGRes(2, True) '从取料处二工位补料   
			ElseIf ToA162Mod1 = 0 And ToA162Mod2 = 1 Then '机械手全部工位没有料，则1号补料
				SeqPutGetCG = PutGetCGRes(1, True) '从取料处一工位补料	
			ElseIf ToA162Mod1 = 1 And ToA162Mod2 = 1 Then '不需要补料
				SeqPutGetCG = PutGetCGRes(0, True) '从取料处取料不取	
			EndIf
	    EndIf
	    
	    Print "time3:", Tmr(0)
		TmReset 0
	    
		Go InitPos CP
		Go daiji CP
		If EndLot = False Then
			On 4 '通知PLC拿料完成			
		EndIf
		Go PutGet1passCGPos CP ! Off 4 ! '一边走一边关闭取料完成
		Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
		Go PutGet3passCGPos CP
	Else
		Go PutGet1passCGPos CP
		Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
		Go PutGet3passCGPos CP
	EndIf
Fend
Function SeqPutGetReCG() As Int32 '从复测试位置取料
	Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
    SeqPutGetReCG = 0
    If FirstBegin Then '第一次运行屏蔽去复测位置取料
	    If ToA162Mod1 = 1 Or ToA162Mod2 = 1 Then
	    	Go PutGetReCGPos
	    	If ToA162Mod1 = 1 And ToA162Mod2 = 1 Then
				SeqPutGetReCG = PutGetCGRes(3, True) '从复测试位置取料
			EndIf
			If ToA162Mod1 = 1 And ToA162Mod2 = 0 Then
				SeqPutGetReCG = PutGetCGRes(1, True) '从复测试位置取料
			EndIf
			If ToA162Mod1 = 0 And ToA162Mod2 = 1 Then
				SeqPutGetReCG = PutGetCGRes(2, True) '从复测试位置取料
			EndIf
	    EndIf
	EndIf
	FirstBegin = True
	If EndLot Or A162EndLot Then
		If Sw(20) = On Or Sw(21) = On Then
			Go PutGet3passCGPos CP
			ReceMes("NGTrayCome") '需要NG料盒	
			Wait Sw(10) = On '等待NG料盘的通知放料，PLC信号
			If Sw(20) = On Then
				Go NG1PhotoPos
				Call PutPutCGRes(1, False, False)
				NGTrayCount(2) = NGTrayCount(2) + 1
			EndIf
			If Sw(21) = On Then
				Go NG2PhotoPos
				Call PutPutCGRes(2, False, False)
				NGTrayCount(2) = NGTrayCount(2) + 1
			EndIf
			ReWriteCount() '将数据记录到PLC数据
			Call ReceMes("NGTrayGo") '告诉运料盒，料已经放完	
		EndIf
	EndIf
Fend
Function SeqTakePhone1() As Int32 '拍照1部分
'	If EndLot Then '如果点击了EndLot那么就不需要拍照
'		SeqTakePhone1 = 0
'		Exit Function
'	EndIf
'	
'	Tool 0
'	SeqTakePhone1 = 0
'	Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
'	Go Ph1Pos
'		Print "time4:", Tmr(0)
'		TmReset 0
'	WaitPos
'	Wait 0.1
'	SeqTakePhone1 = ReTakePhoneCG("TakePhone1", 0) '一次拍照确定角度
'	If SeqTakePhone1 < 0 Then
'		TakeNGPhotoPut(1)
'		SeqTakePhone1 = -1
'		Exit Function
'	EndIf
'	
'	Print "time5.1:", Tmr(0)
'	TmReset 0
'	
''	Tool 1
''	Go Here -U(ModR(0))
''	WaitPos
''	Wait 0.1
''	Tool 0
''	SeqTakePhone1 = ReTakePhoneCG("TakePhone1", 1) '二次拍照确定XY位置
''	If SeqTakePhone1 < 0 Then
''		TakeNGPhotoPut(1)
''		SeqTakePhone1 = -1
''		Exit Function
''	EndIf
''	
''	Print "time5.2:", Tmr(0)
''	TmReset 0
	
	SeqTakePhone1 = 0
Fend
Function SeqTakePhone2() As Int32 '拍照2部分
'	If EndLot Then '如果点击了EndLot那么就不需要拍照
'		SeqTakePhone2 = 0
'		Exit Function
'	EndIf
'	
'	Tool 0
'	Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
'	Go Ph2Pos
'	WaitPos
'	Wait 0.1
'	SeqTakePhone2 = ReTakePhoneCG("TakePhone2", 2) '一次拍照确定角度
'	If SeqTakePhone2 < 0 Then
'		TakeNGPhotoPut(2)
'		SeqTakePhone2 = -1
'		Exit Function
'	EndIf
'	
'	Print "time6.1:", Tmr(0)
'	TmReset 0
'
'	'Tool 2
''	Go Here -U(ModR(2))
''	WaitPos
''	Wait 0.1
''	Tool 0
''	SeqTakePhone2 = ReTakePhoneCG("TakePhone2", 3) '二次拍照确定XY位置
''	If SeqTakePhone2 < 0 Then
''		TakeNGPhotoPut(2)
''		SeqTakePhone2 = -1
''		Exit Function
''	EndIf
''	
''	Print "time6.2:", Tmr(0)
''	TmReset 0
'	
	SeqTakePhone2 = 0
Fend
Function SeqExchageMes() As Int32 '下料等待位
	Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
	SeqExchageMes = 0 '通讯正确
	Go WaitGetGetPos
	
	If EndLot Then '需要EndLot
		ToA162Mod1 = 3 '表示没有料
		ToA162Mod2 = 3 '表示没有料
	EndIf
	String ress$
	ress$ = ReceCGStatu$() '通讯开始,交换相互之间的产品信息
	If ress$ = "-3" Then
		SeqExchageMes = -3 'A162设备停止
		Exit Function
	EndIf
	PrMod1CG = Mod1CGstu '工位1记忆保存
	PrMod2CG = Mod2CGstu '工位2记忆保存
	PrA162CurMod = CurRolMod '转盘所在的位置记忆保存
	Int32 rs
	rs = ReA162CurrectMod("A162CurrectMod") '查询当前A162当前工位的放料情况
	If (Mod1CGstu <> 0 Or Mod2CGstu <> 0) And rs = 0 Then
		Mod1CGstu = 0
		Mod2CGstu = 0
		ress$ = "N"
		Call ReceMes("NGTrayGo") '告诉运料盒，料已经放完
	EndIf
	If ress$ = "Y" Then '看返回的值,Y表示有料可以取
		If Mod1CGstu <> 0 Or Mod2CGstu <> 0 Then
			SeqExchageMes = 0 '有料可以取，执行到下一步取料
			
			Print "time7:", Tmr(0)
			TmReset 0
			
			Exit Function
		EndIf
	ElseIf ress$ = "N" Then '看返回的值,N表示没有料可以取，直接上料
	    'A162当前工位料0表示2个工位都没有料，1表示只有1工位有料，2表示只有2工位有料，3表示2个工位都有料
	    If rs = 0 Then
	    	SeqExchageMes = 0 '依照流程走即可
	    	If EndLot Then '在没有料的情况再EndLot的情况下，再去读取EndLot，如果已经false了，那么就没有必要继续了
				If ReA162EndLot("A162EndLot") < 0 Then
					SeqExchageMes = -2 '读取EndLot失败
					Exit Function
				Else
					If A162EndLot Then
						Go PutGet3passCGPos CP
						Go PutGet1passCGPos CP
						Go daiji
						SeqExchageMes = -5 '回到待机点完成
						Exit Function
					EndIf
				EndIf
	    	EndIf
	    ElseIf rs = 3 Then
	    	If EndLot Then
	    		Mod1CGstu = 4 '设定为待测产品
	    		Mod2CGstu = 4
	    		SeqExchageMes = 0 '按下了EndLot以后如果还有料那么也不测试了
	    		Exit Function
	    	EndIf
	    	
	    	SeqExchageMes = 1 '查询的结果等于1说明，结果没有，但是却有料，这时候只要启动设备就可以了
	    	ToA162Mod1 = 0
	    	ToA162Mod2 = 0
	    	ress$ = ReceCGStatu$() '通讯开始,交换相互之间的产品信息
	    	If ress$ <> "N" Then
	    		SeqExchageMes = -1 '通讯有错误
				Exit Function
			ElseIf ress$ = "-3" Then
				SeqExchageMes = -3 '设备已停止
				Exit Function
	    	EndIf
	    	Wait 0.3
	    	Call ReceMes("A162Begin");
	    	Exit Function
	    Else
	    	SeqExchageMes = -2 '通讯错误或者A162感应器异常	
	    EndIf
	    Go WaitPutPutPos CP
	Else
		SeqExchageMes = -1 '通讯有错误
		Exit Function
	EndIf
	
	Print "time7:", Tmr(0)
	TmReset 0
Fend
Function SeqGetGetCG() As Int32 '拿料部分
	Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
	SeqGetGetCG = 0 '表示一切正常
	If Mod1CGstu <> 0 Or Mod1CGstu <> 0 Then
		PrStep = 2
		Go WaitGetGetPos CP
	    'Go GetGetCGpassPos CP
		Go GetGetCGPos
		
		Print "time8:", Tmr(0)
		TmReset 0
		
		If Mod1CGstu <> 0 And Mod2CGstu <> 0 Then
			SeqGetGetCG = GetGetCGRes(3) '两工位全取料
			
			Print "time9:", Tmr(0)
			TmReset 0
			
			'Go GetGetCGPassPos CP
			Go WaitGetGetPos CP
			
			Print "time10.1:", Tmr(0)
			TmReset 0
			
		    Exit Function
		EndIf
		If Mod1CGstu <> 0 Then
			SeqGetGetCG = GetGetCGRes(1) '只有工位1取料
		EndIf
		If Mod2CGstu <> 0 Then
			SeqGetGetCG = GetGetCGRes(2) '只有工位2取料
		EndIf
		
		Print "time9:", Tmr(0)
		TmReset 0
		
		'Go GetGetCGPassPos CP
		Go WaitGetGetPos CP
		
		Print "time10.1:", Tmr(0)
		TmReset 0
		
	EndIf
Fend
Function SeqPutPutCG() As Int32 '放料部分
	If EndLot = False Then
		PrStep = 3
	EndIf
	Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
	If EndLot Then
		Go WaitPutPutPos CP
		Go PutGet4passCGPos CP
	Else
		SeqPutPutCG = 0
		Go WaitPutPutPos CP
		
		Print "time10.2:", Tmr(0)
		TmReset 0
		
'	    	'Go PutPutCGpassPos
'	    Tool 1 '换成工具1的坐标
'	    Pass P(28 + CurRolMod) -U(ModR(0))  'CurRolMod从零开始计算，p28为1位置1工位，以此类推到P31点
'	    
'	    Print "time10.3:", Tmr(0)
'		TmReset 0
'	    
'		Go Here -X(ModX(0)) -Y(ModY(0))  '至工位1放料，并加上偏移量
'		Tool 0 ' 换成工具机械手坐标

		Go P(28 + CurRolMod) 'CurRolMod从零开始计算，p28为1位置1工位，以此类推到P31点
		
		Print "time10.3:", Tmr(0)
		TmReset 0
		
		PutPutCGRes(3, True, True) '两个爪一起下料
		
		Print "time11:", Tmr(0)
		TmReset 0
		
'		Tool 2 '换成工具2坐标
'		Pass P(32 + CurRolMod) -U(ModR(2))  'CurRolMod从零开始计算，p32为1位置2工位，以此类推到P35点
'		Go Here -X(ModX(2)) -Y(ModY(2))  '至工位2放料，并加上偏移量
'		Tool 0 '换成机械手坐标
'		PutPutCGRes(2, True, True) '放料位2号爪下料
'			'Go PutPutCGpassPos CP

		Go WaitPutPutPos CP
		
		Print "time12:", Tmr(0)
		TmReset 0
		
		Go PutGet4passCGPos CP
		
		Print "time13.1:", Tmr(0)
		TmReset 0
		
	EndIf
	If EndLot Then
		If ReA162EndLot("A162EndLot") < 0 Then
			SeqPutPutCG = -2 '读取EndLot失败
			Exit Function
		Else
			If A162EndLot Then
				SeqPutPutCG = ReceMes("A162Begin");
				If SeqPutPutCG < 0 Then
					SeqPutPutCG = -3 'A162机台未启动
					Exit Function
				EndIf
				If Mod1CGstu = 0 And Mod2CGstu = 0 Then '如果发现没有需要取得料那么转头直接回去咯
					Go WaitGetGetPos CP
				EndIf
				Exit Function
			Else
				If Mod1CGstu = 0 And Mod2CGstu = 0 Then '如果发现没有需要取得料那么转头直接回去咯
					Go WaitGetGetPos CP
				EndIf
			EndIf
		EndIf
	Else
		SeqPutPutCG = ReceMes("A162Begin");
			If SeqPutPutCG < 0 Then
				SeqPutPutCG = -3 'A162机台未启动
				Exit Function
			EndIf
		If Mod1CGstu = 0 And Mod2CGstu = 0 Then '如果发现没有需要取得料那么转头直接回去咯
			Go PutGet3passCGPos CP
		EndIf
	EndIf
Fend
Function SeqPutFristNGCG() As Int32 '放一次NG料
	PrStep = 4
	Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
	SeqPutFristNGCG = 0
	If Mod1CGstu = 2 Or Mod2CGstu = 2 Then
		If EndLot Then
			If Mod1CGstu = 2 Then
				Mod1CGstu = 4
			EndIf
			If Mod2CGstu = 2 Then
				Mod2CGstu = 4
			EndIf
			Exit Function
		EndIf
		If Mod1CGstu = 2 Then '1工位1次测试NG料
			Go FMod1NGPos
			GetPutCGRes(1)
			Mod1CGstu = 0
			ToA162Mod2 = 1
			PrMod1CG = 0
		EndIf
		If Mod2CGstu = 2 Then '2工位1次测试NG料
		    Go FMod2NGPos
			GetPutCGRes(2)
			Mod2CGstu = 0
			ToA162Mod1 = 1
			PrMod2CG = 0
		EndIf
		If Mod1CGstu = 3 Or Mod2CGstu = 3 Or Mod1CGstu = 4 Or Mod2CGstu = 4 Or Mod1CGstu = 5 Or Mod2CGstu = 5 Or Mod1CGstu = 6 Or Mod2CGstu = 6 Then
			Exit Function
		EndIf
		Go PutGet4passCGPos CP '可以换向的位置
	EndIf
Fend
Function SeqPutSecondNGCG() As Int32 '放二次NG料
	PrStep = 4
	Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
	SeqPutSecondNGCG = 0
	If Mod1CGstu = 3 Or Mod2CGstu = 3 Or Mod1CGstu = 4 Or Mod2CGstu = 4 Or Mod1CGstu = 5 Or Mod2CGstu = 5 Or Mod1CGstu = 6 Or Mod2CGstu = 6 Then
		ReceMes("NGTrayCome")
		Wait Sw(10) = On '等待NG料盘的通知放料，PLC信号
		'1工位放料NG料盘
		If Mod1CGstu = 3 Then '1工位等于3说明是最终失败料，放进3个坑里面的一个坑
			Go P(40 + CurrentNGTray) +Z(0.4 * NGTrayCount(CurrentNGTray))
			GetPutCGRes(1)
			NGTrayCount(CurrentNGTray) = NGTrayCount(CurrentNGTray) + 1
			Mod1CGstu = 0
			ToA162Mod2 = 0
			PrMod1CG = 0
		EndIf
		If Mod1CGstu = 4 Then '1工位等于4说明是拍照失败料，放进3号坑
			Go P(42) +Z(0.4 * NGTrayCount(2))
			GetPutCGRes(1)
			NGTrayCount(2) = NGTrayCount(2) + 1
			Mod1CGstu = 0
			ToA162Mod2 = 0
			PrMod1CG = 0
		EndIf
		If Mod1CGstu = 5 Then '1工位等于5说明是BinA料，放进1号坑
			Go P(40) +Z(0.4 * NGTrayCount(0))
			GetPutCGRes(1)
			NGTrayCount(0) = NGTrayCount(0) + 1
			Mod1CGstu = 0
			ToA162Mod2 = 0
			PrMod1CG = 0
		EndIf
		If Mod1CGstu = 6 Then '1工位等于6说明是BinB料，放进2号坑
			Go P(41) +Z(0.4 * NGTrayCount(1))
			GetPutCGRes(1)
			NGTrayCount(1) = NGTrayCount(1) + 1
			Mod1CGstu = 0
			ToA162Mod2 = 0
			PrMod1CG = 0
		EndIf
		
		'2工位放料NG料盘
		If Mod2CGstu = 3 Then '2工位等于3说明是最终失败料，放进4号坑
			Go P(53) +Z(0.4 * NGTrayCount(3))
			GetPutCGRes(2)
			NGTrayCount(3) = NGTrayCount(3) + 1
			Mod2CGstu = 0
			ToA162Mod1 = 0
			PrMod2CG = 0
		EndIf
		If Mod2CGstu = 4 Then '2工位等于4说明是拍照失败料，放进3号坑
			Go P(52) +Z(0.4 * NGTrayCount(2))
			GetPutCGRes(2)
			NGTrayCount(2) = NGTrayCount(2) + 1
			Mod2CGstu = 0
			ToA162Mod1 = 0
			PrMod2CG = 0
		EndIf
		If Mod2CGstu = 5 Then '2工位等于5说明是BinA料，放进1号坑
			Go P(50) +Z(0.4 * NGTrayCount(0))
			GetPutCGRes(2)
			NGTrayCount(0) = NGTrayCount(0) + 1
			Mod2CGstu = 0
			ToA162Mod1 = 0
			PrMod2CG = 0
		EndIf
		If Mod2CGstu = 6 Then '2工位等于6说明是BinB料，放进2号坑
			Go P(51) +Z(0.4 * NGTrayCount(1))
			GetPutCGRes(2)
			NGTrayCount(1) = NGTrayCount(1) + 1
			Mod2CGstu = 0
			ToA162Mod1 = 0
			PrMod2CG = 0
		EndIf
		Call ReceMes("NGTrayGo") '告诉运料盒，料已经放完
		Go PutGet4passCGPos CP '可以换向的位置
	EndIf
Fend
Function SeqPutOKCG() As Int32 '放OK料
	PrStep = 4
	Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
	SeqPutOKCG = 0
	Go PutGet3passCGPos CP '最后这一步走完，反正都要换向
	
	Print "time13.2:", Tmr(0)
	TmReset 0
	
	If Mod1CGstu = 1 Or Mod2CGstu = 1 Then
		Call ReceMes("NGTrayHome")
		If Mod1CGstu = 1 Then '1工位测试OK料
			If OKTrayCount < OkCount Then
				If OKTrayCount < 10 Then
					Go OKMod1Pos
				Else
					Go OKMod1Pos +Z(0.4 * (OKTrayCount - 9))
				EndIf
				GetPutCGRes(1)
				OKTrayCount = OKTrayCount + 1
				SeqPutOKCG = ReWriteCount() '数据保存到PLC
				Mod1CGstu = 0
				ToA162Mod2 = 0
				PrMod1CG = 0
				
				Print "time13.3:", Tmr(0)
				TmReset 0
					
			Else
				Go OKSaftyPos
				Call ReceMes("OKTrayOK") '这个时候所有的料已满,通知PLC取料
				Call ReceMes("ALLOKTrayCount") 'PLC是否将所有料放完
				OKTrayCount = 0
				SeqPutOKCG = 2
				Exit Function
			EndIf
			
			If OKTrayCount >= OkCount Then
			    Go OKSaftyPos
			    Call ReceMes("OKTrayOK") '这个时候所有的料已满,通知PLC取料
				Call ReceMes("ALLOKTrayCount") 'PLC是否将所有料放完
			    OKTrayCount = 0
			    SeqPutOKCG = 2
				Exit Function
			EndIf
		EndIf
		
		If Mod2CGstu = 1 Then '2工位测试OK料
			If OKTrayCount < OkCount Then
				If OKTrayCount < 10 Then
					Go OKMod2Pos
				Else
					Go OKMod2Pos +Z(0.4 * (OKTrayCount - 9))
				EndIf
				GetPutCGRes(2)
				OKTrayCount = OKTrayCount + 1
				SeqPutOKCG = ReWriteCount() '数据保存到PLC
				Mod2CGstu = 0
				ToA162Mod1 = 0
				PrMod2CG = 0
				
				Print "time13.4:", Tmr(0)
				TmReset 0

			Else
				Go OKSaftyPos
				OKTrayCount = 0
				Call ReceMes("OKTrayOK") '这个时候所有的料已满,通知PLC取料
				Call ReceMes("ALLOKTrayCount") 'PLC是否将所有料放完
			EndIf
			If OKTrayCount >= OkCount Then
			    Go OKSaftyPos
			    OKTrayCount = 0
			    Call ReceMes("OKTrayOK") '这个时候所有的料已满,通知PLC取料
			    Call ReceMes("ALLOKTrayCount") 'PLC是否将所有料放完
			EndIf
		EndIf
	EndIf
	SeqPutOKCG = ReWriteCount() '数据保存到PLC
	If ReNGCGstu("ReNGCGstu") = 3 Then '如果复测平台都是需要复测的料那么就没有必要回去了
		Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
		Go PutGet3passCGPos
		Exit Function
	EndIf
	If EndLot = True And A162EndLot = False Then
		Go PutGet1passCGPos CP
		Go daiji CP
		SeqPutOKCG = -5
		Exit Function
	EndIf
	If EndLot Then
		If ReNGTray("") < 0 Then 'NG数据是否清零，即是查询数据也是读取数据
			SeqPutOKCG = -2
			Exit Function
		EndIf
		SeqPutOKCG = 5
		Exit Function
	EndIf
	Go PutGet1passCGPos CP
	Go daiji CP
Fend
Function PutGetCGRes(ByVal re As Int32, ByVal jiajinbool As Boolean) As Int32 '放料工位取料’必须记录是在哪里取得料，0表示取料处，1表示复测试位置
	PutGetCGRes = 0
	If re = 1 Then '工位1爪抓料
	 	On 8 '放料位取料1气缸伸
	 	On 12 '放料位取料1真空 
	 	Wait Sw(12) = On, cylinder_AlarmTime '气缸报警3S
	 	If TW Then
	 		Call ReceMes("PutModAirNG")
	 		Quit main
	 	EndIf
	 	Wait Sw(20) = On, GetCGTime '等待取料时间
	 	If jiajinbool Then
	 		Call ReceMes("jiajinOpen") '1工位夹紧气缸打开	
	 	EndIf
	 	Off 8 '放料位取料1气缸缩
	 	Wait Sw(13) = On, cylinder_AlarmTime '气缸报警3S
	 	If TW Then
	 		Call ReceMes("PutModAirNG")
	 		Quit main
	 	EndIf
	 	If Sw(20) = Off Then '放料位1号真空检测
	 		Call ReceMes("PutGetCGNG1") '通知电脑取料报警
	 		PutGetCGRes = -1
	 	EndIf
	 	Exit Function
	ElseIf re = 2 Then '工位2爪抓料
	    On 9 '放料位取料2气缸伸
	 	On 13 '放料位取料2真空
	 	Wait Sw(14) = On, cylinder_AlarmTime '气缸报警3S
	 	If TW Then
	 		Call ReceMes("PutModAirNG")
	 		Quit main
	 	EndIf
	 	Wait Sw(21) = On, GetCGTime '等待取料时间
	 	If jiajinbool Then
	 		Call ReceMes("jiajinOpen") '2工位夹紧气缸打开	
	 	EndIf
	 	Off 9 '放料位取料2气缸缩
	 	Wait Sw(15) = On, cylinder_AlarmTime '气缸报警3S
	 	If TW Then
	 		Call ReceMes("PutModAirNG")
	 		Quit main
	 	EndIf
	 	If Sw(21) = Off Then '放料位2号真空检测
	 		Call ReceMes("PutGetCGNG2") '通知电脑取料报警
	 		PutGetCGRes = -2
	 	EndIf
	 	Exit Function
	ElseIf re = 3 Then '两工位爪抓料
	 	On 8 '放料位取料1气缸伸
	 	On 9 '放料位取料2气缸伸
	 	On 12 '放料位取料1真空
	 	On 13 '放料位取料2真空
	 	Wait Sw(12) = On And Sw(14) = On, cylinder_AlarmTime '气缸报警3S
	 	If TW Then
	 		Call ReceMes("PutModAirNG")
	 		Quit main
	 	EndIf
	 	Wait Sw(20) = On And Sw(21) = On, GetCGTime '等待取料时间
	 	If jiajinbool Then
	 		Call ReceMes("jiajinOpen") '两个工位夹紧气缸打开	
	 	EndIf
	 	Off 8 '放料位取料1气缸缩
	 	Off 9 '放料位取料1气缸缩
	 	Wait Sw(13) = On And Sw(15) = On, cylinder_AlarmTime '气缸报警3S
	 	If TW Then
	 		Call ReceMes("PutModAirNG")
	 		Quit main
	 	EndIf
	 	If Sw(20) = Off Or Sw(21) = Off Then '放料位1号真空检测
	 		Call ReceMes("PutGetCGNG1") '通知电脑取料报警
	 		PutGetCGRes = -1
	 	EndIf
	 	'If Sw(21) = Off Then '放料位2号真空检测
	 		'Call ReceMes("PutGetCGNG2") '通知电脑取料报警
	 		'PutGetCGRes = -2
	 	'EndIf
	 	Exit Function
	EndIf
Fend
Function GetGetCGRes(ByVal re As Int32) As Int32 '取料工位取料
	GetGetCGRes = 0 '返回值为0，表示一切正常
	If re = 1 Then
	 	On 10 '取料1气缸伸出
		On 14 '取料1真空
		Wait Sw(16) = On, cylinder_AlarmTime '气缸报警3S
	 	If TW Then
	 		Call ReceMes("GetModAirNG")
	 		Quit main
	 	EndIf
		Wait Sw(22) = On, GetCGTime
		Off 10 '取料1气缸缩回	
		Wait Sw(17) = On, cylinder_AlarmTime '气缸报警3S
	 	If TW Then
	 		Call ReceMes("GetModAirNG")
	 		GetGetCGRes = -1 '通知电脑取料错误
	 		Quit main
	 	EndIf
		If Sw(22) = Off Then
		    Call ReceMes("GetGetCGNG1") '通知电脑取料报警
		    GetGetCGRes = -1 '通知电脑取料错误
		EndIf
	    Exit Function
	ElseIf re = 2 Then
		On 11 '取料2气缸伸出
		On 15 '取料2真空
		Wait Sw(18) = On, cylinder_AlarmTime '气缸报警3S
	 	If TW Then
	 		Call ReceMes("GetModAirNG")
	 		GetGetCGRes = -1 '通知电脑取料错误
	 		Quit main
	 	EndIf
		Wait Sw(23) = On, GetCGTime
		Off 11 '取料2气缸缩回	
		Wait Sw(19) = On, cylinder_AlarmTime '气缸报警3S
	 	If TW Then
	 		Call ReceMes("GetModAirNG")
	 		GetGetCGRes = -1 '通知电脑取料错误
	 		Quit main
	 	EndIf
		If Sw(23) = Off Then
	    	Call ReceMes("GetGetCGNG2") '通知电脑取料报警
	    	GetGetCGRes = -2 '通知电脑取料错误
	    EndIf
	    Exit Function
	ElseIf re = 3 Then
	 	On 10 '取料1气缸伸出
	 	On 11 '取料2气缸伸出
	 	On 14 '取料1真空
	 	On 15 '取料2真空
	 	Wait Sw(16) = On And Sw(18) = On, cylinder_AlarmTime '气缸报警3S
	 	If TW Then
	 		Call ReceMes("GetModAirNG")
	 		GetGetCGRes = -1 '通知电脑取料错误
	 		Quit main
	 	EndIf
	 	Wait Sw(22) = On And Sw(23) = On, GetCGTime
	 	Off 10 '取料1气缸缩回
	 	Off 11 '取料2气缸缩回
	 	Wait Sw(17) = On And Sw(19) = On, cylinder_AlarmTime '气缸报警3S
	 	If TW Then
	 		Call ReceMes("GetModAirNG")
	 		Quit main
	 	EndIf
	 	If Sw(22) = Off Then
		    Call ReceMes("GetGetCGNG1") '通知电脑取料报警
		    GetGetCGRes = -1 '通知电脑取料错误
		EndIf
		If Sw(23) = Off Then
	    	Call ReceMes("GetGetCGNG2") '通知电脑取料报警
	    	GetGetCGRes = -2 '通知电脑取料错误
	    EndIf
	    Exit Function
	EndIf
Fend
Function PutPutCGRes(ByVal re As Int32, ByVal GoMini As Boolean, ByVal IsVacuum As Boolean) As Int32 '放料工位放
	'参数1是哪个爪，参数2是否往前推进，参数3是否让当前对应A162工位吸气
	PutPutCGRes = 0 '返回值为0，表示一切正常
	If re = 1 Then
	 	On 8 '1放料气缸伸出 
		Wait Sw(12) = On, cylinder_AlarmTime '气缸报警3S
	 	If TW Then
	 		Call ReceMes("PutModAirNG")
	 		Quit main
	 	EndIf
		Off 12 '1放料真空关闭
		If GoMini Then '是否需要拖着产品走一点
			Go Here +X(3)  '放了气之后再拖着产品向前走两个毫米，确保它能够到达销钉底部
		EndIf
		If IsVacuum Then 'ReVacuum(ByVal setdata$ As String)
			If ReVacuum("1") < 0 Then
				Quit main
			EndIf
		EndIf
		Wait PutCGTime '延时
		Off 8 '1放料气缸上升
		Wait Sw(13) = On, cylinder_AlarmTime '气缸报警3S
	 	If TW Then
	 		Call ReceMes("PutModAirNG")
	 		Quit main
	 	EndIf
	    Exit Function
	ElseIf re = 2 Then
		On 9 '2放料气缸伸出
		Wait Sw(14) = On, cylinder_AlarmTime '气缸报警3S
	 	If TW Then
	 		Call ReceMes("PutModAirNG")
	 		Quit main
	 	EndIf
		Off 13 '2放料真空关闭
		If GoMini Then '是否需要拖着产品走一点
			Go Here +X(3)  '放了气之后再拖着产品向前走两个毫米，确保它能够到达销钉底部
		EndIf
		If IsVacuum Then
			If ReVacuum("2") < 0 Then
				Quit main
			EndIf
		EndIf
		Wait PutCGTime '延时
		Off 9 '2放料气缸上升
		Wait Sw(15) = On, cylinder_AlarmTime '气缸报警3S
	 	If TW Then
	 		Call ReceMes("PutModAirNG")
	 		Quit main
	 	EndIf
	    Exit Function
	ElseIf re = 3 Then
	 	On 8 '1放料气缸伸出 
	 	On 9 '2放料气缸伸出
	 	Wait Sw(12) = On And Sw(14) = On, cylinder_AlarmTime '气缸报警3S
	 	If TW Then
	 		Call ReceMes("PutModAirNG")
	 		Quit main
	 	EndIf
	 	Off 12 '1放料真空关闭
	 	Off 13 '2放料真空关闭
	 	If GoMini Then '是否需要拖着产品走一点
			Go Here +X(3)  '放了气之后再拖着产品向前走两个毫米，确保它能够到达销钉底部
		EndIf
	 	If IsVacuum Then
			If ReVacuum("3") < 0 Then
				Quit main
			EndIf
		EndIf
	 	Wait PutCGTime '延时
	 	Off 8 '1放料气缸上升
	 	Off 9 '2放料气缸上升
	 	Wait Sw(13) = On And Sw(15) = On, cylinder_AlarmTime '气缸报警3S
	 	If TW Then
	 		Call ReceMes("PutModAirNG")
	 		Quit main
	 	EndIf
	    Exit Function
	EndIf
Fend
Function GetPutCGRes(ByVal re As Int32) As Int32 '取料工位放料
	GetPutCGRes = 0 '返回值为0，表示一切正常
	If re = 1 Then
	 	On 10 '1取料气缸伸出 
		Wait Sw(16) = On, cylinder_AlarmTime '气缸报警3S
	 	If TW Then
	 		Call ReceMes("GetModAirNG")
	 		Quit main
	 	EndIf
		Off 14 '1取料真空关闭
		Wait PutCGTime '延时
		Off 10 '1取料气缸上升
		Wait Sw(17) = On, cylinder_AlarmTime '气缸报警3S
	 	If TW Then
	 		Call ReceMes("GetModAirNG")
	 		Quit main
	 	EndIf
	    Exit Function
	ElseIf re = 2 Then
		On 11 '2取料气缸伸出
		Wait Sw(18) = On, cylinder_AlarmTime '气缸报警3S
	 	If TW Then
	 		Call ReceMes("GetModAirNG")
	 		Quit main
	 	EndIf
		Off 15 '2取料真空关闭
		Wait PutCGTime '延时
		Off 11 '2取料气缸上升
		Wait Sw(19) = On, cylinder_AlarmTime '气缸报警3S
	 	If TW Then
	 		Call ReceMes("GetModAirNG")
	 		Quit main
	 	EndIf
	    Exit Function
	ElseIf re = 3 Then
	 	On 10 '1取料气缸伸出 
	 	On 11 '2取料气缸伸出
	 	Wait Sw(16) = On And Sw(18) = On, cylinder_AlarmTime '气缸报警3S
	 	If TW Then
	 		Call ReceMes("GetModAirNG")
	 		Quit main
	 	EndIf
	 	Off 14 '1取料真空关闭
	 	Off 15 '2取料真空关闭
	 	Wait PutCGTime '延时
	 	Off 10 '1取料气缸上升
	 	Off 11 '2取料气缸上升
	 	Wait Sw(17) = On And Sw(19) = On, cylinder_AlarmTime '气缸报警3S
	 	If TW Then
	 		Call ReceMes("GetModAirNG")
	 		Quit main
	 	EndIf
	 	Exit Function
	EndIf
Fend
Function daijidian
	    If Hand = Lefty Then
	        If CY(Here) > 360 And CX(Here) > -400 And CX(Here) < 150 Then
	             Go Here :Z(-10)
	             Go shuoshidian :U(CU(Here)) /L
	             Go shuoshidian :U(CU(Here)) /R
	             Go shuoshidian /R
	        Else
		    	Print "手臂错误,人工换手臂"
		    	Pause
	    	EndIf
	       EndIf
		If CX(Here) <= 0 And CY(Here) <= 60 Then
		    If Abs(CU(Here) - 50) > 90 Then
		    	Jump pass_ng :U(CU(Here)) CP LimZ -10
		    	Go pass_ng CP
		    EndIf
		    Jump daiji LimZ -10
		ElseIf CX(Here) <= 0 And CY(Here) > 60 Then
		    If Abs(CU(Here) - 50) > 45 Then
		    	Jump PutGet2passCGPos :U(CU(Here)) CP LimZ -10
				Jump PutGet2passCGPos CP LimZ -10
		    EndIf
		    Jump PutGet1passCGPos CP LimZ -10
			Jump daiji LimZ -10
		ElseIf CX(Here) > 300 Then
		    If Abs(CU(Here) - (-37.397)) > 45 Then
		    	Move Here :Z(-10) -X(150) CP
				Jump WaitGetGetPos :U(CU(Here)) CP LimZ -10
				Jump WaitGetGetPos CP LimZ -10
				Jump PutGet1passCGPos CP LimZ -10
				Jump daiji LimZ -10
		    Else
		    Move Here :Z(-10) -X(150) CP
			Jump WaitGetGetPos CP LimZ -10
			Jump PutGet1passCGPos CP LimZ -10
			Jump daiji LimZ -10
			EndIf
		Else
			If CU(Here) - (-37.397) > 45 Then
				Jump WaitGetGetPos :U(CU(Here)) CP LimZ -10
				Jump WaitGetGetPos CP LimZ -10
				Jump PutGet1passCGPos CP LimZ -10
				Jump daiji LimZ -10
		    Else
			Jump WaitGetGetPos CP LimZ -10
			Jump PutGet1passCGPos CP LimZ -10
			Jump daiji LimZ -10
			EndIf
		EndIf
'			If 30 >= CU(Here) And CU(Here) >= -40 Then
'			    Go Here :Z(0)
'				Pass quliao_pass_2
'				Pass quliao_pass_1
'                Go daiji
'		    ElseIf 30 >= CU(Here) And CU(Here) >= 13 Then
'		        Go Here :Z(0)
'				Pass quliao_pass_1
'				Go daiji
'		    ElseIf 60 >= CU(Here) And CU(Here) >= 30 And CX(Here) <= -400 Then
'		        Go Here +X(150) :Z(0)
'		        
'		        Go daiji
'		    ElseIf 60 >= CU(Here) And CU(Here) >= 30 And CX(Here) >= -400 Then
'		        Go Here :Z(0)
'		        Go daiji
'		    Else
'		    	Go Here :Z(0)
'		    	Go quliao_pass_3 :U(CU(Here))
'                Go quliao_pass_3
'                Go daiji
'			EndIf
'		ElseIf 0 >= CX(Here) And CX(Here) >= -350 And CY(Here) <= 60 Then
'		       Go Here :Z(0)
'		       Go daiji
'		
'		ElseIf 800 >= CX(Here) And CX(Here) >= -400 And CY(Here) >= 150 Then
'		       If CX(Here) < 50 Then
'		        If CU(Here) < 60 And CU(Here) > -130 And CX(Here) > -240 Then
'				        Go Here :Z(0)
'				       	Pass pass_1_1
'				        Pass pass_1_0
'				       	Go daiji
'				 ElseIf CU(Here) < 60 And CU(Here) > -130 And CX(Here) < -240 Then
'				        Go Here :Z(0)
'				        Go Here :X(-180) :Y(390)
'				       	Pass pass_1_1
'				        Pass pass_1_0
'				       	Go daiji
'				 Else
'				 	    Go Here :Z(0)
'				 	    Pass zhongjiandian1 :U(CU(Here))
'				 	    Pass zhongjiandian1
'				       	Pass pass_1_1
'				       	Pass pass_1_0
'				       	Go daiji
'				 EndIf
'		       	ElseIf CX(Here) > 50 Then
'		       	Go Here :Z(0)
'		       	   If CU(pass_1_2) - 7 <= CU(Here) And CU(Here) <= CU(pass_1_2) + 7 And CY(Here) <= 450 And CY(Here) >= 150 Then
'		       	       Go Here :Y(300)
'		       	       Pass pass_1_2
'		       	       Pass pass_1_1
'		       	       Pass pass_1_0
'		       	       Go daiji
'		       	   ElseIf CX(Here) > 50 And CY(Here) > 450 Then
'		       	        Go Here :Z(0)
'		       	        Go Here :X(50) :Z(0)
'		       	    	Pass zhongjiandian1 :U(CU(Here))
'		       	    	Pass zhongjiandian1
'		       	       Pass pass_1_2
'		       	       Pass pass_1_1
'		       	       Pass pass_1_0
'		       	       Go daiji
'		       	     
'		       	   Else
'		       	    	Pass zhongjiandian2 :U(CU(Here))
'		       	    	Pass zhongjiandian2
'		       	       Pass pass_1_2
'		       	       Pass pass_1_1
'		       	       Pass pass_1_0
'		       	       Go daiji
'		       	     
'		       	     EndIf
'		       	
'		       EndIf
'		
'		ElseIf CX(Here) >= 5 And CY(Here) <= 150 Then
'		               Go Here :Z(0)
'		      		   Pass zhongjiandian2 :U(CU(Here))
'		       	       Pass zhongjiandian2
'		       	      Pass pass_1_2
'		       	       Pass pass_1_1
'		       	      Pass pass_1_0
'		       	       Go daiji
'		
'		EndIf
			
Fend
'产品九点标定
Function jiudianbiaoding As Int32
	jiudianbiaoding = 0
	Tool 0 '机械手坐标
	'从初始位置开始
	Go InitPos CP
	'Wait Sw(8) = On '等待到的信号延时0.2秒
	Go PutGetCGPos
	PutGetCGRes(3, False) '从复测试位置取料
	Go InitPos CP
	Go daiji CP
	Go PutGet1passCGPos CP
	'Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
	Go PutGet3passCGPos CP
	'到达相机拍照位置
	'Go WaitGetGetPos
	Int32 ki
	Int32 kj

	For ki = 1 To 9 Step 1  '标定第一个标定九个位置
		Tool 1
		Go P(ki + 100)
		If ReBiaoding("A" + Str$(ki)) < 0 Then
			jiudianbiaoding = -1
			Exit Function
		EndIf
	Next
	
	Tool 0
    For kj = 1 To 9 Step 1
    	Tool 2
    	Go P(kj + 109)
    	If ReBiaoding("B" + Str$(kj)) < 0 Then
    		jiudianbiaoding = -1
    		Exit Function
    	EndIf
    Next
    Tool 0
Fend
'标定通讯
Function ReBiaoding(ByVal setdata$ As String) As Int32
	ReBiaoding = 0
	String rece$
	Do
		Print #201, "BiaoDing;" + setdata$ + ";" + Str$(CX(Here)) + ";" + Str$(CY(Here)) '通过串口发送字符串A
		Wait 0.5
		Input #201, rece$  '将上位机发过来的数据读取出来存到字符串data$
		If setdata$ = rece$ Then
			Exit Function
		ElseIf setdata$ = "BiaodingNG" Then '如果收到的数据是标定失败，那么直接退出程序
			ReBiaoding = -1
			Exit Function
		EndIf
	Loop
Fend
'任务挂起:Halt
'任务恢复:Resume
'全部暂停:Pause
'任务停止:Quit
'TMOut 5'设置超时时间
'MemOn 10'打开记忆IO有128个点，保持性
'Wait MemSw(10) = On'等到记忆点
'Wait Sw(12) = On'
'If TW Then'报警超时
  'Print "Time out occurred"
'EndIf

'放料工位1气缸伸出报警	   
'Function PutMod1shen
'	TmReset 0
'	Do
'		If Sw(12) = On Then
'			Exit Do
'		EndIf
'		If Tmr(0) > cylinder_AlarmTime Then
'		    Call ReceMes("PutModAirNG")
'			Print "报警，退出"
'			Quit main
'			Exit Do
'		EndIf
'	Loop
'Fend

'首次开机运行
Function OperetionCG(ByVal setdata$ As String) As Int32
	OperetionCG = 0
	FirstBegin = False '第一次运行屏蔽去复测平台取料
	Int32 ReeCG '查询复测平台上是否有料
	Int32 ReA162 '查询A162平台CG
	CurrentNGTray = 3
	ReeCG = ReNGCGstu("ReNGCGstu") '返回值是0表示复测平台没有料，1表示只有1工位有料，2表示只有2工位有料，3表示两个工位都有料
	Wait 0.2
	
	Boolean OKTrayGO '是否查询一次OK料盒满，移走
	OKTrayGO = True
	
	If Sw(20) = On And (ReeCG = 1 Or ReeCG = 3) Then
		OperetionCG = -1 '复测平台料和气爪料有冲突
		Exit Function
	EndIf
	If Sw(21) = On And (ReeCG = 2 Or ReeCG = 3) Then
		OperetionCG = -1 '复测平台料和气爪料有冲突
		Exit Function
	EndIf

	'将数据从PLC中读出计数部分
	Integer cierror
	cierror = 0
	Do
		OperetionCG = ReReadCount("ReadCount")
		If OperetionCG < 0 Then
			cierror = cierror + 1
			If cierror = 5 Then
				OperetionCG = -1
				Exit Function
			EndIf
		EndIf
		Integer ci '循环所有NG料盘，查询是否有超过容量的
		Integer Mod1ci '获得工位1应该对应的料盘
		Boolean NGTrayOK
		For ci = 0 To 3 '循环1次4个NG料坑
			If NGTrayCount(ci) >= NgCount Then
				NGTrayOK = True '有料盘放满还未清空
				Exit For
			EndIf
			For Mod1ci = 4 To 5 '循环第5和第6个盒子是否已经将NG料盒放满
				If NGTrayCount(Mod1ci) < NgCount Then
					CurrentNGTray = Mod1ci '获得当前1工位1爪放置的NG料位置
					Exit For
				EndIf
				CurrentNGTray = 3 '如果工位要放置的两个料盘都满了，那么久放到工位2的料盘里面去
			Next
			NGTrayOK = False 'NG料盘还没有放满，还可以继续放料		
		Next
		'Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
		If NGTrayOK Or OKTrayCount >= OkCount Then
			If OKTrayGO And OKTrayCount >= OkCount Then '开机运行的时候如果发现OK料盒满了，那么发送一次取走信号，后面不再发送
				Call ReceMes("OKTrayOK") '这个时候所有的料已满,通知PLC取料
				OKTrayGO = True
			EndIf
		Else
			Exit Do
		EndIf
	Loop


	Boolean PutCGCG
	Int32 CurA162Mod '表示当前A162工位是否有料0表示无料，1表示1工位有料，2表示2工位有料，3表示两个工位都有料
	
	If PrStep = 1 Then '放料位取料的时候报警
		Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
		If ReA162EndLot("AbleEndLot") >= 0 Then
			If A162EndLot Then
				OperetionCG = 0
				FirstBegin = True '第一次运行屏蔽去复测平台取料
				Exit Function
			EndIf
		Else
			OperetionCG = -2 '通讯错误
			Exit Function
		EndIf

		If Sw(22) = Off And Sw(23) = Off Then
			If ReeCG = 0 Then
				If Sw(20) = Off Then
					ToA162Mod1 = 0
				Else
					ToA162Mod1 = 1
				EndIf
				If Sw(21) = Off Then
					ToA162Mod2 = 0
				Else
					ToA162Mod2 = 1
				EndIf
				FirstBegin = False '第一次运行去复测平台取料
				OperetionCG = 0
				Exit Function
			EndIf
			If ReeCG = 1 And Sw(20) = Off Then
				ToA162Mod1 = 1
				If Sw(21) = Off Then
					ToA162Mod2 = 0
					OperetionCG = 0
				Else
					ToA162Mod2 = 0
					Go PutGet1passCGPos CP
					Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
					Go PutGet3passCGPos CP
					OperetionCG = 2
				EndIf
				FirstBegin = True '第一次运行去复测平台取料
				Exit Function
			EndIf
			If ReeCG = 2 And Sw(21) = Off Then
				ToA162Mod2 = 1
				If Sw(20) = Off Then
					ToA162Mod1 = 0
					OperetionCG = 0
				Else
					ToA162Mod1 = 0
					Go PutGet1passCGPos CP
					Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
					Go PutGet3passCGPos CP
					OperetionCG = 2
				EndIf
				FirstBegin = True '第一次运行去复测平台取料
				Exit Function
			EndIf
			If ReeCG = 3 And Sw(20) = Off And Sw(21) = Off Then
				ToA162Mod1 = 1
				ToA162Mod2 = 1
				Go PutGet1passCGPos CP
				Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
				Go PutGet3passCGPos CP
				FirstBegin = True '第一次运行去复测平台取料
				OperetionCG = 2
				Exit Function
			EndIf
		EndIf
	EndIf
	
	String ress$ '接收当前A162工位的产品信息
	If PrStep = 2 Then '取料位取料报警
		Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
		'记得要写上超时
		ReA162 = ReA162CurrectMod("A162CurrectMod") '获得当前A162当前工位产品情况，0表示没有料，1表示只有1工位有料，2表示只有2工位有料，3表示两个工位都有料
		If ReA162 < 0 Then
			OperetionCG = -1
			Exit Function
		EndIf
		If CurRolMod = PrA162CurMod Then
			If Sw(20) = On And Sw(21) = On Then
				ToA162Mod1 = 0
				ToA162Mod2 = 0
				ress$ = ReceCGStatu$()
				If ress$ = "-3" Then
					OperetionCG = -3 'A162设备未开启
				EndIf
				If Mod1CGstu = PrMod1CG And Mod2CGstu = PrMod2CG Then
					FirstBegin = True '第一次运行去复测平台取料
					If (Sw(22) = On And Sw(23) = Off And ReA162 = 2) Or (Sw(22) = Off And Sw(23) = On And ReA162 = 1) Or (Sw(22) = Off And Sw(23) = Off And ReA162 = 3) Then
						Go PutGet1passCGPos CP
						Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
						Go PutGet3passCGPos CP
						Go WaitGetGetPos CP
						OperetionCG = 6
						Exit Function
					EndIf
					If Sw(22) = On And Sw(23) = On And ReA162 = 0 Then
						Go PutGet1passCGPos CP
						Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
						Go PutGet3passCGPos CP
						Go WaitGetGetPos CP
						OperetionCG = 7
						Exit Function
					EndIf
				EndIf
			EndIf
		EndIf
	EndIf

	If PrStep = 3 Then '放料位放料报警
		Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
		FirstBegin = True '第一次运行去复测平台取料
		'带两片料，不带料
		If (Sw(22) = On And Sw(23) = On) Or (Sw(22) = Off And Sw(23) = Off) Then
			'记得要写上超时
			ReA162 = ReA162CurrectMod("A162CurrectMod") '获得当前A162当前工位产品情况，0表示没有料，1表示只有1工位有料，2表示只有2工位有料，3表示两个工位都有料
			If ReA162 < 0 Then
				OperetionCG = -1
				Exit Function
			EndIf
			If CurRolMod = PrA162CurMod Then
				ToA162Mod1 = 0
				ToA162Mod2 = 0
				ress$ = ReceCGStatu$()
				If ress$ = "-3" Then
					OperetionCG = -3 'A162设备未开启
					Exit Function
				EndIf
				If Mod1CGstu = PrMod1CG And Mod2CGstu = PrMod2CG Then
					FirstBegin = True '第一次运行去复测平台取料
					If (Sw(20) = On And Sw(21) = Off And ReA162 = 2) Or (Sw(20) = Off And Sw(21) = On And ReA162 = 1) Or (Sw(20) = On And Sw(21) = On And ReA162 = 0) Then
						Go PutGet1passCGPos CP
						Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
						Go PutGet3passCGPos CP
						Go PutGet4passCGPos CP
						Go WaitPutPutPos
						OperetionCG = 7
						Exit Function
					EndIf
					If Sw(20) = Off And Sw(21) = Off And ReA162 = 3 Then
						Go PutGet1passCGPos CP
						Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
						Go PutGet3passCGPos CP
						Go PutGet4passCGPos CP
						Go WaitPutPutPos
						OperetionCG = 7
						Exit Function
					EndIf
				EndIf
			EndIf
		EndIf
	EndIf
	
	If PrStep = 4 Then '取料位放料报警
		Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
		If Sw(20) = Off And Sw(21) = Off Then
			FirstBegin = True '第一次运行去复测平台取料
			If Sw(22) = On Then
				Mod1CGstu = PrMod1CG
			Else
				Mod1CGstu = 0
			EndIf
			If Sw(23) = On Then
				Mod2CGstu = PrMod2CG
			Else
				Mod2CGstu = 0
			EndIf
			If Mod1CGstu = 0 And Mod2CGstu = 0 Then
				OperetionCG = 0
				Exit Function
			Else
				Go PutGet1passCGPos CP
				Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
				Go PutGet3passCGPos CP
				Go PutGet4passCGPos CP
				OperetionCG = 8
				Exit Function
				If Mod1CGstu = 3 Or Mod2CGstu = 3 Or Mod1CGstu = 4 Or Mod2CGstu = 4 Or Mod1CGstu = 5 Or Mod2CGstu = 5 Or Mod1CGstu = 6 Or Mod2CGstu = 6 Then
					ReceMes("NGTrayCome") '需要NG料盒	
				EndIf
'				If Mod1CGstu = 2 Or Mod2CGstu = 2 Or Mod1CGstu = 3 Or Mod2CGstu = 3 Or Mod1CGstu = 4 Or Mod2CGstu = 4 Or Mod1CGstu = 5 Or Mod2CGstu = 5 Or Mod1CGstu = 6 Or Mod2CGstu = 6 Then
'					Go PutGet1passCGPos CP
'		    		Go PutGet3passCGPos CP
'		    		OperetionCG = 8
'					Exit Function
'				EndIf
'				If (Mod1CGstu = 0 And Mod2CGstu = 1) Or (Mod1CGstu = 1 And Mod2CGstu = 0) Or (Mod1CGstu = 1 And Mod2CGstu = 1) Then
'					Go PutGet1passCGPos CP
'		    		Go PutGet3passCGPos CP
'		    		Go PutGet4passCGPos CP
'		    		OperetionCG = 10
'		    		Exit Function
'				EndIf
			EndIf
		EndIf
	EndIf


	If ReA162EndLot("AbleEndLot") >= 0 Then
		If A162EndLot Then
			EndLot = True
			OperetionCG = 0
			FirstBegin = True '第一次运行屏蔽去复测平台取料
			Exit Function
		EndIf
	Else
		OperetionCG = -2 '通讯错误
		Exit Function
	EndIf


	OperetionCG = 0
'如果以上的情况不满足，那么就直接将料放到不明确料盒里面
	'首先判定取料手抓是否有料
	If Sw(22) = On Or Sw(23) = On Then
	    Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
	    ReceMes("NGTrayCome") '初次运行有不明料，需要NG料盒
	    Go PutGet1passCGPos CP
	    Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
	    Go PutGet3passCGPos CP
	    Go PutGet4passCGPos CP

		If Sw(22) = On Then '取料手爪1工位
			Go P42 +Z(0.4 * NGTrayCount(2)) '将当前的料放到照相失败的料盒中
			GetPutCGRes(1) '将1工位料卸下
			NGTrayCount(2) = NGTrayCount(2) + 1
			ReWriteCount() '将数据记录到PLC数据
		EndIf

		If Sw(23) = On Then '取料手爪2工位
			Go P52 +Z(0.4 * NGTrayCount(2)) '将当前的料放到照相失败的料盒中
			GetPutCGRes(2) '将2工位料卸下
			NGTrayCount(2) = NGTrayCount(2) + 1
			ReWriteCount() '将数据记录到PLC数据
		EndIf
		
		Go PutGet4passCGPos CP
		Go PutGet3passCGPos CP
		Go PutGet1passCGPos CP
		Go daiji CP
'		If Sw(20) = Off And Sw(21) = Off And ReeCG <> 3 Then
'			Go PutGet1passCGPos CP
'			Go daiji CP
'			ToA162Mod1 = 0
'		    ToA162Mod2 = 0
'		    FirstBegin = True '第一次运行去复测平台取料
'		    Exit Function
'		EndIf
	EndIf
	Call ReceMes("NGTrayGo") '告诉运料盒，料已经放完
	'判断放料手爪是否有料
	If Sw(20) = On Or Sw(21) = On Then '
		Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
		If ReeCG = 0 Then
			If Sw(20) = Off Then
				ToA162Mod1 = 0
			Else
				ToA162Mod1 = 1
			EndIf
			If Sw(21) = Off Then
				ToA162Mod2 = 0
			Else
				ToA162Mod2 = 1
			EndIf
			FirstBegin = False '第一次运行去复测平台取料
			OperetionCG = 0
			Exit Function
		EndIf
		If ReeCG = 1 And Sw(20) = Off Then
			ToA162Mod1 = 1
			If Sw(21) = Off Then
				ToA162Mod2 = 0
				OperetionCG = 0
			Else
				ToA162Mod2 = 0
				Go PutGet1passCGPos CP
				Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
				Go PutGet3passCGPos CP
				OperetionCG = 2
			EndIf
			FirstBegin = True '第一次运行去复测平台取料
			Exit Function
		EndIf
		If ReeCG = 2 And Sw(21) = Off Then
			ToA162Mod2 = 1
			If Sw(20) = Off Then
				ToA162Mod1 = 0
				OperetionCG = 0
			Else
				ToA162Mod1 = 0
				Go PutGet1passCGPos CP
				Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
				Go PutGet3passCGPos CP
				OperetionCG = 2
			EndIf
			FirstBegin = True '第一次运行去复测平台取料
			Exit Function
		EndIf
		If ReeCG = 3 And Sw(20) = Off And Sw(21) = Off Then
			ToA162Mod1 = 1
			ToA162Mod2 = 1
			Go PutGet1passCGPos CP
			Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
			Go PutGet3passCGPos CP
			FirstBegin = True '第一次运行去复测平台取料
			OperetionCG = 2
			Exit Function
		EndIf
		OperetionCG = -1
	Else
		FirstBegin = True '第一次运行去复测平台取料
		OperetionCG = 0
	EndIf
Fend
Function TakeNGPhotoPut(ByVal yiorer As Short) As Int32 '是丢那一边的料
	Tool 0
	Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
	TakeNGPhotoPut = 0
	ReceMes("NGTrayCome") '需要NG料盒
	Wait Sw(10) = On '等待NG料盘的通知放料，PLC信号
	FirstBegin = False '是否去取复测平台的料(不去)
	'将拍照失败的的CG先丢进坑里面
	If yiorer = 1 Then '1爪拍照失败
		Go NG1PhotoPos +Z(0.4 * NGTrayCount(2))
		ToA162Mod1 = 0
		ToA162Mod2 = 1
    ElseIf yiorer = 2 Then '2爪拍照失败
		Go NG2PhotoPos +Z(0.4 * NGTrayCount(2))
		ToA162Mod1 = 1
		ToA162Mod2 = 0
	Else
		TakeNGPhotoPut = -1
	EndIf
	PutPutCGRes(yiorer, False, False)
	NGTrayCount(2) = NGTrayCount(2) + 1
	ReWriteCount() '将数据记录到PLC数据
	Call ReceMes("NGTrayGo") '告诉运料盒，料已经放完
	'回到取料位补料
	Go PutGet1passCGPos CP
	Go daiji CP
	Go InitPos CP
Fend

'''''''''''''''''''''机械手各测试类运行'''''''''''''''''''''''''
Global Short shangliaoci '当前上料次数
Global Short jiaohuanci '当前交换料次数
Global Short xialiaoci '当前下料次数
Global Short lianxuci '连续跑

Global Short shangliaoMax '上料最多次数
Global Short jiaohuanMax '交换料最多次
Global Short xialiaoMax '下料最多次
Global Short lianxuMax '连续跑片次最多次

Global Int32 squeTest

Function ReTestGRR() As Int32
	ReTestGRR = 0
	String toks$(0), data$   '定义字符串变量
	Do
	    Print #201, "GRR"   '通过串口发送字符串A
		Input #201, data$    '将上位机发过来的数据读取出来存到字符串data$
	    Print data$  '在运行窗口打印接收到的字符串
	    ParseStr data$, toks$(), ";"   '将上位机数据以";"作为分隔符分解数据（数据格式:A;x;y;u;NGOK）
		'返回回来的数据进行分解"GRR;0;0;0;0;0"
		If UBound(toks$) = 5 Then '如果分开的数据是等于4位则可以
			shangliaoMax = Val(toks$(1))   '
	    	jiaohuanMax = Val(toks$(2))   '
	    	xialiaoMax = Val(toks$(3))   '
	    	lianxuMax = Val(toks$(4)) '
	    	ReTestGRR = 0
	    	Exit Function
	    ElseIf data$ = "GRRNO" Then '
	        ReTestGRR = 1 '返回1表示不需要跑GRR
	    	Exit Function
	    EndIf
	Loop
Fend
Function ReTestMessage(ByVal messageInt As Short) As Int32
	ReTestMessage = 0
	String toks$(0), data$   '定义字符串变量
	Do
	    Print "TestMessage;" + Str$(messageInt)
	    Print #201, "TestMessage;" + Str$(messageInt)   '通过串口发送字符串A
		Input #201, data$    '将上位机发过来的数据读取出来存到字符串data$
	    Print data$  '在运行窗口打印接收到的字符串
	    ParseStr data$, toks$(), ";" '
		'UBound(toks$) = 5
		If data$ = "GRRRun" Then '确认放好了片
	    	ReTestMessage = 0
	    	Exit Function
	    ElseIf data$ = "GRRPause" Then '确认不跑GRR
	        ReTestMessage = -1 '
	    	Exit Function
	    EndIf
	Loop
Fend
Function TestOperation() As Int32
	Power High '设定运行功率为高功率
	Speed 30    '定义jump、go等PTP运动速度，单位为百分比，最大为100
	Accel 30, 30  '定义jump、go等PTP运动加速度，单位为百分比，最大为120
	SpeedS 100  '定义move、arc、arc3等CP运动速度，单位mm/s,最大2000
	AccelS 200, 200  '定义move、arc、arc3等CP运动速度，单位mm/s2,最大20000
	
	TestOperation = 0
	
	shangliaoci = 0 '一般情况下跑4次
	jiaohuanci = 0 '一般情况下4次
	xialiaoci = 0 '一般情况4次
	lianxuci = 0 '看情况跑
	
	squeTest = 0 '跑到哪一步
	
	If Sw(20) = On Or Sw(21) = On Or Sw(22) = On Or Sw(23) = On Then
		TestOperation = -1 '测试程序起跑之前首先要保证手爪上面都没有料
		Exit Function
	EndIf
	
	If ReA162CurrectMod("A162CurrectMod") < 0 Then '检查A162设备是否已经启动，如果没有启动那么就停止运行
		TestOperation = -1 '
		Exit Function
	EndIf
	
	Do
'		OnErr GoTo errHandler
		Tool 0 '切换成机械手坐标
		Int32 res
		Select squeTest
			Case 0
				res = SeqDialogBox_test() '等待提示框的选择
				If res = 0 Then
					squeTest = 1
				Else
					TestOperation = -1
					Exit Function '返回小于零的值那么就退出自动，等待PC指示
				EndIf
			Case 1
				res = SeqPutGetCG_test() '从取料处取料
				If res = 0 Then
					squeTest = 2
				Else
					TestOperation = -1
					Exit Function '返回小于零的值那么就退出自动，等待PC指示
				EndIf
			Case 2
				res = SeqTakePhone1_test() '相机拍照1
				If res = 0 Then
					squeTest = 3
				Else
					TestOperation = -1
					Exit Function '返回小于零的值那么就退出自动，等待PC指示	
				EndIf
			Case 3
				res = SeqTakePhone2_test() '相机拍照2
				If res = 0 Then
					squeTest = 4
				Else
					TestOperation = -1
					Exit Function '返回小于零的值那么就退出自动，等待PC指示	
				EndIf
			Case 4
				res = SeqMachineCG_test() '检测A162机台是否有料
				If res = 0 Then
					squeTest = 5 '正常情况下开始上料到机械手
				Else
					TestOperation = -1
					Exit Function '返回小于零的值那么就退出自动，等待PC指示
				EndIf
			Case 5
				res = SeqPutPutCG_test() '将料放到A162机台上
				If res = 0 Then
					squeTest = 6 '正常情况启动机台
				Else
					TestOperation = -1
					Exit Function '返回小于零的值那么就退出自动，等待PC指示
				EndIf
			Case 6
				res = SeqStartMachine_test() '启动机台
				If res = 0 Then
					squeTest = 7 '到下料步
				Else
					TestOperation = -1
					Exit Function '返回小于零的值那么就退出自动，等待PC指示
				EndIf
			Case 7
				res = SeqPutGetTestCG_test() '将机台上的料取下
				If res = 0 Then
					squeTest = 8 '将料送至初始位置
				Else
					TestOperation = -1
					Exit Function '返回小于零的值那么就退出自动，等待PC指示
				EndIf
			Case 8
				res = SeqPutRemoteCG_test() '将料送至取料处
				If res = 0 Then
					squeTest = 0
				Else
					TestOperation = -1
					Exit Function '返回小于零的值那么就退出自动，等待PC指示
				EndIf
			Default
				Print "wu"
		Send
'errHandler:
'    errnum = Err
'    If errnum > 0 Then
'         Print #201, Err, ErrMsg$(Err)
'    EndIf

	Loop
Fend
Function SeqDialogBox_test() As Int32 '放料部分拿料'0
	SeqDialogBox_test = 0
	If shangliaoci < shangliaoMax Then
        SeqDialogBox_test = ReTestMessage(0) '0表示机械手取料
        ToA162Mod1 = 0
		ToA162Mod2 = 0
        Exit Function
	EndIf
	If jiaohuanci < jiaohuanMax Then
        SeqDialogBox_test = ReTestMessage(1) '1表示机械手换料
        ToA162Mod1 = 0
		ToA162Mod2 = 0
        Exit Function
	EndIf
	
	ToA162Mod1 = 1
	ToA162Mod2 = 1
	If xialiaoci >= xialiaoMax Then
		SeqDialogBox_test = ReTestMessage(2) '2表示机械手跑完了GRR
		SeqDialogBox_test = -1 '表示GRR跑完
		Exit Function
	EndIf
Fend
Function SeqPutGetCG_test() As Int32 '放料部分拿料'1
	SeqPutGetCG_test = 0
	Call ReceMes("NGTrayGo") '告诉运料盒，料已经放完
	If ToA162Mod1 = 0 Or ToA162Mod2 = 0 Then
		Go InitPos
    	Wait Sw(8) = On '以免发生撞击的可能
    	Go PutGetCGPos
		If ToA162Mod1 = 0 And ToA162Mod2 = 0 Then
		    SeqPutGetCG_test = PutGetCGRes(3, False) '从取料处取料全取   
		ElseIf ToA162Mod1 = 1 And ToA162Mod2 = 0 Then '机械手1号工位已经有料，则2号补料
		    SeqPutGetCG_test = PutGetCGRes(2, False) '从取料处二工位补料   
		ElseIf ToA162Mod1 = 0 And ToA162Mod2 = 1 Then '机械手全部工位没有料，则1号补料
			SeqPutGetCG_test = PutGetCGRes(1, False) '从取料处一工位补料	
		ElseIf ToA162Mod1 = 1 And ToA162Mod2 = 1 Then '不需要补料
			SeqPutGetCG_test = PutGetCGRes(0, False) '从取料处取料不取	
		EndIf
		Go InitPos CP
		Go daiji CP
		On 4 '通知PLC拿料完成			
		Go PutGet1passCGPos CP ! Off 4 ! '一边走一边关闭取料完成
		Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
		Go PutGet3passCGPos CP
	Else
		Go daiji CP
		Go PutGet1passCGPos CP
		Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
		Go PutGet3passCGPos CP
	EndIf
Fend
Function SeqTakePhone1_test() As Int32 '拍照1部分'2
'	If shangliaoci >= shangliaoMax And jiaohuanci >= jiaohuanMax Then '如果交换的上料的次数都完成那么下料就没有必要再拍照
'		SeqTakePhone1_test = 0
'		Exit Function
'	EndIf
'	Tool 0
'	SeqTakePhone1_test = 0
'	Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
'	Go Ph1Pos
'
'	WaitPos
'	'Wait 0.1
'	SeqTakePhone1_test = ReTakePhoneCG("TakePhone1", 0) '一次拍照确定角度
'	If SeqTakePhone1_test < 0 Then
'		'TakeNGPhotoPut(1)
'		SeqTakePhone1_test = -1
'		Exit Function
'	EndIf
	SeqTakePhone1_test = 0
Fend
Function SeqTakePhone2_test() As Int32 '拍照2部分'3
'	If shangliaoci >= shangliaoMax And jiaohuanci >= jiaohuanMax Then '如果交换的上料的次数都完成那么下料就没有必要再拍照
'		SeqTakePhone2_test = 0
'		Exit Function
'	EndIf
'	Tool 0
'	Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
'	Go Ph2Pos
'	WaitPos
'	Wait 0.1
'	SeqTakePhone2_test = ReTakePhoneCG("TakePhone2", 2) '一次拍照确定角度
'	If SeqTakePhone2_test < 0 Then
'		'TakeNGPhotoPut(2)
'		SeqTakePhone2_test = -1
'		Exit Function
'	EndIf
	SeqTakePhone2_test = 0
Fend
Function SeqMachineCG_test() As Int32 '获得当前A162机台上料的状态'4
	Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
	SeqMachineCG_test = 0 '通讯正确
	Go WaitGetGetPos
	
	String ress$
	ress$ = ReceCGStatu$() '通讯开始,交换相互之间的产品信息
	If ress$ = "-3" Then
		SeqMachineCG_test = -3 'A162设备停止
		Exit Function
	EndIf
	Int32 rs
	rs = ReA162CurrectMod("A162CurrectMod") '查询当前A162当前工位的放料情况
	 'A162当前工位料0表示2个工位都没有料，1表示只有1工位有料，2表示只有2工位有料，3表示2个工位都有料
	If shangliaoci < shangliaoMax Then
		If rs = 0 Then '前4次上料的时候发现机台没有料那么可以
		
		Else
			SeqMachineCG_test = -1
		EndIf
	EndIf
Fend
Function SeqPutPutCG_test() As Int32 '放料部分'5
	Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
	SeqPutPutCG_test = 0
	If shangliaoci >= shangliaoMax And jiaohuanci >= jiaohuanMax Then
		Exit Function
	EndIf
	Go WaitPutPutPos CP
	
	Go P(28 + CurRolMod) 'CurRolMod从零开始计算，p28为1位置1工位，以此类推到P31点
	PutPutCGRes(3, True, True) '两个爪一起下料
	
'    'Go PutPutCGpassPos
'    Tool 1 '换成工具1的坐标
'    Pass P(28 + CurRolMod) -U(ModR(0))  'CurRolMod从零开始计算，p28为1位置1工位，以此类推到P31点
'	Go Here -X(ModX(0)) -Y(ModY(0))  '至工位1放料，并加上偏移量
'	Tool 0 ' 换成工具机械手坐标
'	PutPutCGRes(1, True, True) '放料位1号爪下料
'
'	Tool 2 '换成工具2坐标
'	Pass P(32 + CurRolMod) -U(ModR(2))  'CurRolMod从零开始计算，p32为1位置2工位，以此类推到P35点
'	Go Here -X(ModX(2)) -Y(ModY(2))  '至工位2放料，并加上偏移量
'	Tool 0 '换成机械手坐标
'	PutPutCGRes(2, True, True) '放料位2号爪下料
'	'Go PutPutCGpassPos CP

	Go WaitPutPutPos CP
Fend
Function SeqStartMachine_test() As Int32 '启动A162机台'6
	Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
	SeqStartMachine_test = 0
	Short i
	i = 0
	String ress$
	
	'上料4次及连续跑
	If shangliaoci < shangliaoMax Then
		shangliaoci = shangliaoci + 1
		ress$ = ReceCGStatu$() '通讯开始,交换相互之间的产品信息
		If ress$ = "-3" Then
			SeqStartMachine_test = -3 'A162设备停止
			Exit Function
		EndIf
		Wait 0.1
		Call ReceMes("A162Begin");
		If shangliaoci >= shangliaoMax Then
			For i = 1 To (lianxuMax - 1)
				ress$ = ReceCGStatu$() '通讯开始,交换相互之间的产品信息
				If ress$ = "-3" Then
					SeqStartMachine_test = -3 'A162设备停止
					Exit Function
				EndIf
				Go P(28 + CurRolMod) 'CurRolMod从零开始计算，p28为1位置1工位，以此类推到P31点
				SeqStartMachine_test = PutGetCGRes(3, False)
				Go WaitPutPutPos
				Go P(28 + CurRolMod) 'CurRolMod从零开始计算，p28为1位置1工位，以此类推到P31点
				SeqStartMachine_test = PutPutCGRes(3, False, True) '两个爪一起下料
				Go WaitPutPutPos
				Call ReceMes("A162Begin");
			Next
			SeqStartMachine_test = 0 '连续跑完以后开始下料交换通道
			Exit Function
		Else
			Exit Function
		EndIf
	EndIf
	
	'交换4次及连续跑
	If jiaohuanci < jiaohuanMax Then
		jiaohuanci = jiaohuanci + 1
		ress$ = ReceCGStatu$() '通讯开始,交换相互之间的产品信息
		If ress$ = "-3" Then
			SeqStartMachine_test = -3 'A162设备停止
			Exit Function
		EndIf
		Wait 0.1
		Call ReceMes("A162Begin");
		If jiaohuanci >= jiaohuanMax Then
			For i = 1 To (lianxuMax - 1)
				ress$ = ReceCGStatu$() '通讯开始,交换相互之间的产品信息
				If ress$ = "-3" Then
					SeqStartMachine_test = -3 'A162设备停止
					Exit Function
				EndIf
				Go P(28 + CurRolMod) 'CurRolMod从零开始计算，p28为1位置1工位，以此类推到P31点
				SeqStartMachine_test = PutGetCGRes(3, False)
				Go WaitPutPutPos
				Go P(28 + CurRolMod) 'CurRolMod从零开始计算，p28为1位置1工位，以此类推到P31点
				SeqStartMachine_test = PutPutCGRes(3, False, True) '两个爪一起下料
				Go WaitPutPutPos
				Call ReceMes("A162Begin");
			Next
			SeqStartMachine_test = 0 '连续跑完以后开始下料交换通道
			Exit Function
		Else
			Exit Function
		EndIf
	EndIf
Fend
Function SeqPutGetTestCG_test() As Int32 '放一次NG料'8
	Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
	SeqPutGetTestCG_test = 0 '表示一切正常

	String ress$
	ress$ = ReceCGStatu$() '通讯开始,交换相互之间的产品信息
	If ress$ = "-3" Then
		SeqPutGetTestCG_test = -3 'A162设备停止
		Exit Function
	EndIf

	If shangliaoci >= shangliaoMax Then
		If (Mod1CGstu <> 0 Or Mod2CGstu <> 0) Then
			Call ReceMes("NGTrayGo") '告诉运料盒，料已经放完
			Go WaitPutPutPos CP
			Go P(28 + CurRolMod) 'CurRolMod从零开始计算，p28为1位置1工位，以此类推到P31点
			If Mod1CGstu <> 0 And Mod2CGstu <> 0 Then
				SeqPutGetTestCG_test = PutGetCGRes(3, False) 'GetGetCGRes(3) '两工位全取料
				'Go GetGetCGPassPos CP
				Go WaitPutPutPos CP
				Call ReceMes("NGTrayGo") '告诉运料盒，料已经放完
				
				If jiaohuanci >= jiaohuanMax Then
					If xialiaoci = 0 Then
						Wait 2
						SeqPutGetTestCG_test = ReTestMessage(3) '勾选EndLot
						If ReA162EndLot("AbleEndLot") < 0 Then
							SeqPutGetTestCG_test = -2 '读取EndLot失败
							Exit Function
						EndIf
						
					EndIf
					Wait 2
					If xialiaoci < 3 Then
						Call ReceMes("A162Begin")
					EndIf
					If xialiaoci = xialiaoMax - 1 Then
						If ReA162EndLot("A162EndLot") < 0 Then
							SeqPutGetTestCG_test = -1 '读取EndLot失败
							Exit Function
						EndIf
						Wait 1
					EndIf
					xialiaoci = xialiaoci + 1
				EndIf
			    Exit Function
			EndIf
			If Mod1CGstu <> 0 Then
				SeqPutGetTestCG_test = PutGetCGRes(1, False) '只有工位1取料
			EndIf
			If Mod2CGstu <> 0 Then
				SeqPutGetTestCG_test = PutGetCGRes(2, False) '只有工位2取料
			EndIf
			'Go GetGetCGPassPos CP
			Go WaitPutPutPos CP
			Go WaitGetGetPos CP
		Else
			SeqPutGetTestCG_test = -1 '出错
			Exit Function
		EndIf
	EndIf
Fend
Function SeqPutRemoteCG_test() As Int32 '放一次NG料'8
	Wait Sw(11) = On 'OK料盘可放料，也是机械手过来的安全信号
	SeqPutRemoteCG_test = 0
	Go WaitGetGetPos CP
	Go PutGet3passCGPos CP
	Go PutGet1passCGPos CP
	Go daiji CP
	If shangliaoci >= shangliaoMax Then
		Go InitPos
		Wait Sw(8) = On '这个地方一定要满足可以取料不然可能有造成撞击
		Go GRRPutPutPos
		PutPutCGRes(3, False, False)
		Go InitPos CP
	EndIf
Fend

