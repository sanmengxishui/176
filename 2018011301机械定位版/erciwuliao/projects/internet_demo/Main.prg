Function main
OpenNet #201 As Client
WaitNet #201
Do
   Call communication
Loop
Fend
Function communication
	String toks$(0), data$   '�����ַ�������
	Integer ccd_ngok, charnum   '�������α���   
	Real vision_x, vision_y, vision_u   '����ʵ������
vrun1:     '�����ǩ
	Print #201, "B"    '��������������ź�
	vrun2:
	charnum = ChkNet(201)  '���˿��Ƿ��յ�����
	If charnum < 0 Then
	   Print "��̫������ʧ�ܣ�����������̫��"
	   OpenNet #201 As Client    '��201�˿�
       WaitNet #201   '�ȴ��˿����ӳɹ�
       GoTo vrun1   '��������ת����ǩvrun1
    ElseIf charnum = 0 Then
    	GoTo vrun2 '��������ת����ǩvrun2
    EndIf
	Input #201, data$   '��201�˿ڶ�ȡ���������������
    Print "data$=", data$  '��ӡ�������
	ParseStr data$, toks$(), ";"   '�����������";"��Ϊ�ָ����ֽ����ݣ����ݸ�ʽ:A;x;y;u;ccd_ngok��
	ccd_ngok = Val(toks$(4))  '���ַ�������ת��Ϊ���ֱ���
	If ccd_ngok = 1 Then   '����Ӿ����ճɹ�
		vision_x = Val(toks$(1))  '���ַ�������ת��Ϊ���ֱ���
		vision_y = Val(toks$(2))  '���ַ�������ת��Ϊ���ֱ���
		vision_u = Val(toks$(3))  '���ַ�������ת��Ϊ���ֱ���
		Print "vision_x=", vision_x, "vision_y", vision_y     '��ӡ�Ӿ�����ֵ
		Print "vision_u", vision_u
	Else
		Print "�Ӿ�����ʧ�� "
		GoTo vrun1
	EndIf
Fend

