Function main
Integer Point_no
Call init
Do
Wait Sw(8) = On   '等待输入8有信号
Out 1, 0   '关闭输出8到15所有端口
Point_no = In(2)   '读取输入端口16—23的二进制编码值存到变量Point_no
Select Point_no    '根据Point_no的值选择执行相应程序
Case 1
   Go P1
   Off chuiqi   '关吹气(用标签调用方式操作端口，需预先定义端口标签）
   On 8    '打开输出端口8
   Wait 0.1
Case 2
   Jump P2 C0
   Off 8    '关闭输出端口8
   On chuiqi   '开吹气(用标签调用方式操作端口，需预先定义端口标签）
Case 3
     Move P3
Send
Wait Sw(Move_en) = On And Sw(8) = Off    '等待输入端口Move_en（标签需预先定义）ON，并且输入8 OFF
Out 1, 4    '打开输出10，关闭输出8到15的其他端口
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
