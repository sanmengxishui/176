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
	OpenCom #1    '�򿪴���1
	Do
		Tool 0
	
		Go yi
		
		
		String data$
		Do
				Print #1, "yi"   'ͨ�����ڷ����ַ���A
				Input #1, data$    '����λ�������������ݶ�ȡ�����浽�ַ���data$
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
				Print #1, "er"   'ͨ�����ڷ����ַ���A
				Input #1, data$    '����λ�������������ݶ�ȡ�����浽�ַ���data$
				
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
				Print #1, "san"   'ͨ�����ڷ����ַ���A
				Input #1, data$    '����λ�������������ݶ�ȡ�����浽�ַ���data$
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
'	OpenCom #1    '�򿪴���1
'	Do
'		String data$
'		Print #1, "yao1"   'ͨ�����ڷ����ַ���A
'		Input #1, data$    '����λ�������������ݶ�ȡ�����浽�ַ���data$
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
'		Print #1, "yao2"   'ͨ�����ڷ����ַ���A
'		Input #1, data$    '����λ�������������ݶ�ȡ�����浽�ַ���data$
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
'		Print #1, "yao3"   'ͨ�����ڷ����ַ���A
'		Input #1, data$    '����λ�������������ݶ�ȡ�����浽�ַ���data$
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
	
	

'OpenCom #1    '�򿪴���1
'Do
	'Call communication
'Loop

'Fend
'Function communication
	
	
	
	
	
	
	
	
	
	
'    String toks$(0), data$   '�����ַ�������
'    Integer NGOK, charnum   '�������α���   
'    Real vision_x, vision_y, vision_u    '����ʵ������
'    vrun1:
'    Print #1, "A"   'ͨ�����ڷ����ַ���A
'	Input #1, data$    '����λ�������������ݶ�ȡ�����浽�ַ���data$
'	'stop
'    Print data$  '�����д��ڴ�ӡ���յ����ַ���
'    ParseStr data$, toks$(), ";"   '����λ��������";"��Ϊ�ָ����ֽ����ݣ����ݸ�ʽ:A;x;y;u;NGOK��
'    NGOK = Val(toks$(4))    '���ַ�������ת�������ָ�ֵ������NGOK
'    
'    If NGOK = 1 Then
'    	vision_x = Val(toks$(1))   '���ַ�������ת�������ָ�ֵ������vision_x
'    	vision_y = Val(toks$(2))    '���ַ�������ת�������ָ�ֵ������vision_y
'    	vision_u = Val(toks$(3))    '���ַ�������ת�������ָ�ֵ������vision_u
'    	Print "vision_x=", vision_x, "vision_y=", vision_y, "vision_u=", vision_u   '��ӡ����vision_x,vision_y,vision_u��ֵ
'    Else
'    	Print "��������ʧ��"
'    	GoTo vrun1
'    EndIf
'Fend

