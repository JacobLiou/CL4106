/****************************************************************************

    Single �͡�Hex���໥ת��

*******************************************************************************/

using System;
using System.Text;
using System.Runtime.InteropServices;
namespace ClAmMeterController.Test
{



    public class pw_SingleHex
    {
        [DllImport("VBFloat.dll")]
        private static extern void SingleToBytes(float f, ref  byte pbyt1, ref  byte pbyt2, ref  byte pbyt3, ref  byte pbyt4);

        [DllImport("VBFloat.dll")]
        private static extern float BytesToSingle(byte byt1, byte byt2, byte byt3, byte byt4);


        /// <summary>
        /// �����ȸ�����ת����Hex��
        /// </summary>
        /// <param name="fValues"></param>
        /// <returns></returns>
        public static string pwSingleToHex(float fValues)
        {
            byte[] pbyt =new byte[]{0x0,0x0,0x0,0x0} ;

            SingleToBytes(fValues, ref pbyt[3], ref pbyt[2], ref pbyt[1], ref pbyt[0]);

            string sHexstr = "";
            for (int i = 0; i < pbyt.Length; i++)
            {
                sHexstr += pbyt[i].ToString("X2");
            }
            return sHexstr;
        }


        /// <summary>
        /// Hex��ת���ɵ����ȸ�����
        /// </summary>
        /// <param name="sValues"></param>
        /// <returns></returns>
        public static Single pwHexToSingle(string sValues)
        {
            if (sValues.Length != 8) sValues = sValues.PadRight(8, '0');

            byte[] pbyt = new byte[] { 0x0, 0x0, 0x0, 0x0 };
            for (int i = 0; i < pbyt.Length; i++)
            {
                pbyt[i] = Convert.ToByte(sValues.Substring(i * 2, 2),16);
            }

            float fValues = 0f;
            fValues = BytesToSingle(pbyt[3], pbyt[2], pbyt[1], pbyt[0]);

            return fValues;
        }

        #region
        //        ////==========================================================
//        ////==========����Ϊ4���ֽ�OXת�����ȸ�����=====================
//        ////==========================================================

//        public static Single sSingle(string cValue) 
//    {
//        string  tStr;
//        string  sStr;
//        string  cSign;          // ����λ
//        string  cExponent;      //ָ��λ
//        string  cMantissa ;     //β��λ
//        long sExponent;         //����λ
//        long sMantissa;         //β��
//        string  cWs;
//        string  cZs;
//        double  sWs;
//        long sZs;
//        int i;

//            cValue =cValue.PadLeft(8,'0');
//            if (cValue == "00000000")
//            {
//                return 0 ;
//            }
//            Int32 intValue=Convert.ToInt32(cValue,16);
//            sStr=Convert.ToString(intValue ,2);

//            cSign = sStr.Substring(0,1);                        //����λ
//            cExponent = sStr.Substring(1, 8);                   //ָ����
//            cMantissa = sStr.Substring(9);                      //β����
            
//            tStr = "1";
//            tStr=tStr.PadLeft(127,'0');
//            cMantissa = tStr + cMantissa;
//            sExponent = Convert.ToInt64(cExponent,2);
            
//            cZs = cMantissa.Substring(0,(int)sExponent);
//            cWs = cMantissa.Substring((int)sExponent);
            

//            sZs = Convert.ToInt64(cZs,2);
//            sWs = Convert.ToInt64(cWs,2) / Math.Pow(2, cWs.Length);

//            Single  sstmpSingle = Convert.ToSingle(sZs + sWs);
//            if( cSign == "1")
//                return -sstmpSingle;
//            else
//                return sstmpSingle;
        
        
//        }

//////==========================================================
//////==========����Ϊ�����ȸ�����ת4���ֽ�OX��===================
//////==========================================================

//        public static string cSingle(Single  sValue ) 
//        {

//            string sStr;
//            string sFlag;           //����λ
//            int lExponent;          //����λ
//            string sWs;             //β��
//            string sZs ;            //����


//                //if( sValue == 0)
//                //{
//                //    return "00000000";
//                //}

//                //��ȡ�ķ���λ
//                if (sValue < 0) 
//                    sFlag = "1";
//                else 
//                    sFlag = "0";                                 


//                //-----------------------------------------------------------------------

//                sValue = Math.Abs(sValue);                   //
//                string[] stmp = sValue.ToString().Split('.');
                
//                sZs = stmp[0];                           //����

//                if (stmp.Length < 2)                     //β��
//                    sWs = "0";
//                else
//                    sWs = stmp[1];                       

//                //-----------------------------------------------------------------------

//                sZs = Convert.ToString(Convert.ToInt32(sZs), 2);
//                sWs = Convert.ToString(Convert.ToInt32(sWs), 2);

//                lExponent = sZs.Length - 1;
//                lExponent = lExponent + 127;

//                sWs = sZs.Substring(1) + sWs;
//                sWs = sWs.PadRight(23, '0');


//                sStr = sFlag + Convert.ToString(lExponent, 2).PadRight(8, '0') + sWs;

//                string sReStr=Convert.ToString(Convert.ToUInt32(sStr,2), 16);
//                return sReStr;


        //        }

        #endregion
        #region
        ////==========================================================
        ////==========����Ϊ8���ֽ�OXת˫���ȸ�����=====================
        ////==========================================================
            //public double  sDouble(ByVal cValue As String) 
            //{

            //    Dim sStr        As String
            //    Dim cSign       As String   //����λ
            //    Dim cExponent   As String   //ָ��λ
            //    Dim cMantissa   As String   //β��λ
            //    Dim sExponent   As Long     //����λ
            //    Dim sMantissa   As Long     //β��
            //    Dim cWs         As String
            //    Dim cZs         As String
            //    Dim sWs         As Double
            //    Dim sZs         As Long
            //    Dim i           As Integer
                
            //        On Error Resume Next
            //        If Len(cValue) <> 16 Then Exit Function
            //        If cValue = "0000000000000000" Then
            //            sDouble = 0
            //            Exit Function
            //        End If
            //        For i = 1 To Len(cValue)
            //            sStr = sStr & cBCDToBit(Mid(cValue, i, 1))
            //        Next
                    
            //        cSign = Left(sStr, 1)                           //����λ
            //        cExponent = Mid(sStr, 2, 11)                    //ָ����
            //        cMantissa = Mid(sStr, 13)                       //β����
                    
            //        sExponent = cBitToLong(cExponent) - (2 ^ 10 - 1)     //��λλ��
                    
            //        cZs = Left(cMantissa, sExponent)                //�������ݶ����ƴ�
            //        cZs = "1" & cZs                                 //��������λ�����ƴ�
                    
            //        cWs = Mid(cMantissa, sExponent + 1)             //β�����ݶ����ƴ�
                    
            //        sZs = cBitToLong(cZs)                           //����
            //        sWs = cBitToLong(cWs) / (2 ^ Len(cWs))          //����
                    
            //        sDouble = CDbl(sZs + sWs)                       //���
            //        If cSign = 1 Then sDouble = -sDouble            //�жϷ���λ
                    
                    
            //}

        ////==========================================================
        ////==========����Ϊ˫���ȸ�����ת8���ֽ�OX��===================
        ////==========================================================

        //public string cDouble(ByVal sValue As Double) 
        //{

        //    Dim sStr        As String
        //    Dim cSign       As String   //����λ
        //    Dim cExponent   As String   //ָ��λ
        //    Dim cMantissa   As String   //β��λ
        //    Dim sExponent   As Long     //����λ
        //    Dim sMantissa   As Long     //β��
        //    Dim cWs         As String
        //    Dim cZs         As String
        //    Dim sWs         As Double
        //    Dim sZs         As Long
        //    Dim tStr        As String
        //    Dim sLen        As Integer
        //    Dim i           As Integer
            
            
        //        If sValue = 0 Then
        //            cDouble = "0000000000000000"
        //            Exit Function
        //        End If
                
        //        cSign = "0"                                 //��ȡ�ķ���λ
        //        If sValue < 0 Then cSign = "1"
                
        //        //-----------------------------------------------------------------------
                
        //        sValue = Math.Abs(sValue)                   //
        //        sZs = Fix(sValue)                           //����
        //        sWs = sValue - sZs                          //β��
                
        //        //-----------------------------------------------------------------------
                
        //        cZs = Hex(sZs)                              //ת��16����
        //        For i = 1 To Len(cZs)                       //ת�ɶ�����
        //            sStr = sStr & cBCDToBit(Mid(cZs, i, 1))
        //        Next
                
        //        For i = 1 To Len(sStr)                      //��ȥǰ����Ч��
        //            If Mid(sStr, i, 1) = "1" Then
        //                sStr = Mid(sStr, i)
        //                Exit For
        //            End If
        //        Next
                
        //        cZs = sStr                                  //ȡ�ú�������λ�Ķ����ƴ�
        //        cZs = Mid(cZs, 2)                           //��ȥ����λ,�������λ�����ƴ�
                
                
        //        //------------------------------------------------------------------------
                
        //        sLen = Len(cZs)                             //�����λλ��
        //        cExponent = Hex(sLen + 1023)                //˫�����ټ���1023
        //        For i = 1 To Len(cExponent)
        //            sStr = sStr & cBCDToBit(Mid(cExponent, i, 1))
        //        Next
        //        cExponent = sStr
        //        cExponent = Right("00000000000" & cExponent, 11) //�����λ�Ķ��ƴ�,�̶�����11λ
                
        //        //------------------------------------------------------------------------
                
        //        sStr = cSign & cExponent & cZs              //ȡ�ó�β��λ�Ķ����ƴ�
        //        sLen = 64 - Len(sStr)                       //ȡ��β���Ķ����ƴ�����
                
        //        sWs = sWs * (2 ^ sLen)
        ////        sWs = Fix(sWs)
        //        cWs = Hex(sWs)                              //����ʱ���,Hex���ֻ��ת��8��F
        //        sStr = ""
        //        For i = 1 To Len(cWs)
        //            sStr = sStr & cBCDToBit(Mid(cWs, i, 1))
        //        Next
        //        cWs = sStr
        //        cWs = Right("0000000000000000000000000000000000000000000000000000" & cWs, sLen)      //ȡ��β��λ�����ƴ�,�Ϊ52λ
                    
        //        //--------------------------------------------------------------------------
        //        sStr = cSign & cExponent & cZs & cWs
        //        For i = 1 To 64 Step 4
        //            tStr = tStr & cBitToBCD(Mid(sStr, i, 4))
        //        Next
        //        cDouble = tStr
                    
        //}



        //public double cBitToLong(string cValue ) ////������ת����ʮ����
        //{
        //    double sValue=0;

        //        for( int i = 0 ;i< cValue.Length-1;i++)
        //        {
        //            if( Mid(cValue, i, 1) = "1" )
        //            {
        //                sValue = sValue + 2 ^ (Len(cValue) - i);
        //            }
        //        }
        //        cBitToLong = sValue;

        //}


        //public string cBCDToBit(ByVal sByte As String) ////ʮ������ת���ɶ�����
        //{
        //    Select Case sByte
        //        Case "0"
        //            cBCDToBit = "0000"
        //        Case "1"
        //            cBCDToBit = "0001"
        //        Case "2"
        //            cBCDToBit = "0010"
        //        Case "3"
        //            cBCDToBit = "0011"
        //        Case "4"
        //            cBCDToBit = "0100"
        //        Case "5"
        //            cBCDToBit = "0101"
        //        Case "6"
        //            cBCDToBit = "0110"
        //        Case "7"
        //            cBCDToBit = "0111"
        //        Case "8"
        //            cBCDToBit = "1000"
        //        Case "9"
        //            cBCDToBit = "1001"
        //        Case "A"
        //            cBCDToBit = "1010"
        //        Case "B"
        //            cBCDToBit = "1011"
        //        Case "C"
        //            cBCDToBit = "1100"
        //        Case "D"
        //            cBCDToBit = "1101"
        //        Case "E"
        //            cBCDToBit = "1110"
        //        Case "F"
        //            cBCDToBit = "1111"
        //    End Select
        //}


        //public string cBitToBCD(ByVal sByte As String) ////������ת����ʮ������
        //{
        //    Select Case sByte
        //        Case "0000"
        //            cBitToBCD = "0"
        //        Case "0001"
        //            cBitToBCD = "1"
        //        Case "0010"
        //            cBitToBCD = "2"
        //        Case "0011"
        //            cBitToBCD = "3"
        //        Case "0100"
        //            cBitToBCD = "4"
        //        Case "0101"
        //            cBitToBCD = "5"
        //        Case "0110"
        //            cBitToBCD = "6"
        //        Case "0111"
        //            cBitToBCD = "7"
        //        Case "1000"
        //            cBitToBCD = "8"
        //        Case "1001"
        //            cBitToBCD = "9"
        //        Case "1010"
        //            cBitToBCD = "A"
        //        Case "1011"
        //            cBitToBCD = "B"
        //        Case "1100"
        //            cBitToBCD = "C"
        //        Case "1101"
        //            cBitToBCD = "D"
        //        Case "1110"
        //            cBitToBCD = "E"
        //        Case "1111"
        //            cBitToBCD = "F"
        //    End Select
        //}
        #endregion
    }
}
