Function main
Integer Point_no
Call init
Do
Wait Sw(8) = On   '�ȴ�����8���ź�
Out 1, 0   '�ر����8��15���ж˿�
Point_no = In(2)   '��ȡ����˿�16��23�Ķ����Ʊ���ֵ�浽����Point_no
Select Point_no    '����Point_no��ֵѡ��ִ����Ӧ����
Case 1
   Go P1
   Off chuiqi   '�ش���(�ñ�ǩ���÷�ʽ�����˿ڣ���Ԥ�ȶ���˿ڱ�ǩ��
   On 8    '������˿�8
   Wait 0.1
Case 2
   Jump P2 C0
   Off 8    '�ر�����˿�8
   On chuiqi   '������(�ñ�ǩ���÷�ʽ�����˿ڣ���Ԥ�ȶ���˿ڱ�ǩ��
Case 3
     Move P3
Send
Wait Sw(Move_en) = On And Sw(8) = Off    '�ȴ�����˿�Move_en����ǩ��Ԥ�ȶ��壩ON����������8 OFF
Out 1, 4    '�����10���ر����8��15�������˿�
Loop
Fend
Function init
	If Motor = Off Then    '����ŷ�û��ON����ŷ�
		Motor On  '���ŷ�
	EndIf
	Power High   '�趨���й���Ϊ�߹���
	Speed 10    '����jump��go��PTP�˶��ٶȣ���λΪ�ٷֱȣ����Ϊ100
	Accel 50, 50  '����jump��go��PTP�˶����ٶȣ���λΪ�ٷֱȣ����Ϊ120
	SpeedS 500  '����move��arc��arc3��CP�˶��ٶȣ���λmm/s,���2000
	AccelS 2000, 2000  '����move��arc��arc3��CP�˶��ٶȣ���λmm/s2,���20000
	Jump daiji
Fend
