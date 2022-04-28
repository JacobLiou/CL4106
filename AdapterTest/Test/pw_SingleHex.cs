/****************************************************************************

    Single 和　Hex　相互转换

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
        /// 单精度浮点型转换成Hex串
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
        /// Hex串转换成单精度浮点型
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
//        ////==========以下为4个字节OX转单精度浮点数=====================
//        ////==========================================================

//        public static Single sSingle(string cValue) 
//    {
//        string  tStr;
//        string  sStr;
//        string  cSign;          // 符号位
//        string  cExponent;      //指数位
//        string  cMantissa ;     //尾数位
//        long sExponent;         //移数位
//        long sMantissa;         //尾数
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

//            cSign = sStr.Substring(0,1);                        //符号位
//            cExponent = sStr.Substring(1, 8);                   //指数串
//            cMantissa = sStr.Substring(9);                      //尾数串
            
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
//////==========以下为单精度浮点数转4个字节OX串===================
//////==========================================================

//        public static string cSingle(Single  sValue ) 
//        {

//            string sStr;
//            string sFlag;           //符号位
//            int lExponent;          //移数位
//            string sWs;             //尾数
//            string sZs ;            //整数


//                //if( sValue == 0)
//                //{
//                //    return "00000000";
//                //}

//                //先取的符号位
//                if (sValue < 0) 
//                    sFlag = "1";
//                else 
//                    sFlag = "0";                                 


//                //-----------------------------------------------------------------------

//                sValue = Math.Abs(sValue);                   //
//                string[] stmp = sValue.ToString().Split('.');
                
//                sZs = stmp[0];                           //整数

//                if (stmp.Length < 2)                     //尾数
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
        ////==========以下为8个字节OX转双精度浮点数=====================
        ////==========================================================
            //public double  sDouble(ByVal cValue As String) 
            //{

            //    Dim sStr        As String
            //    Dim cSign       As String   //符号位
            //    Dim cExponent   As String   //指数位
            //    Dim cMantissa   As String   //尾数位
            //    Dim sExponent   As Long     //移数位
            //    Dim sMantissa   As Long     //尾数
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
                    
            //        cSign = Left(sStr, 1)                           //符号位
            //        cExponent = Mid(sStr, 2, 11)                    //指数串
            //        cMantissa = Mid(sStr, 13)                       //尾数串
                    
            //        sExponent = cBitToLong(cExponent) - (2 ^ 10 - 1)     //移位位数
                    
            //        cZs = Left(cMantissa, sExponent)                //整数部份二进制串
            //        cZs = "1" & cZs                                 //加上隐藏位二进制串
                    
            //        cWs = Mid(cMantissa, sExponent + 1)             //尾数部份二进制串
                    
            //        sZs = cBitToLong(cZs)                           //化整
            //        sWs = cBitToLong(cWs) / (2 ^ Len(cWs))          //化零
                    
            //        sDouble = CDbl(sZs + sWs)                       //相加
            //        If cSign = 1 Then sDouble = -sDouble            //判断符号位
                    
                    
            //}

        ////==========================================================
        ////==========以下为双精度浮点数转8个字节OX串===================
        ////==========================================================

        //public string cDouble(ByVal sValue As Double) 
        //{

        //    Dim sStr        As String
        //    Dim cSign       As String   //符号位
        //    Dim cExponent   As String   //指数位
        //    Dim cMantissa   As String   //尾数位
        //    Dim sExponent   As Long     //移数位
        //    Dim sMantissa   As Long     //尾数
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
                
        //        cSign = "0"                                 //先取的符号位
        //        If sValue < 0 Then cSign = "1"
                
        //        //-----------------------------------------------------------------------
                
        //        sValue = Math.Abs(sValue)                   //
        //        sZs = Fix(sValue)                           //整数
        //        sWs = sValue - sZs                          //尾数
                
        //        //-----------------------------------------------------------------------
                
        //        cZs = Hex(sZs)                              //转成16进制
        //        For i = 1 To Len(cZs)                       //转成二进制
        //            sStr = sStr & cBCDToBit(Mid(cZs, i, 1))
        //        Next
                
        //        For i = 1 To Len(sStr)                      //减去前导无效零
        //            If Mid(sStr, i, 1) = "1" Then
        //                sStr = Mid(sStr, i)
        //                Exit For
        //            End If
        //        Next
                
        //        cZs = sStr                                  //取得含有隐藏位的二进制串
        //        cZs = Mid(cZs, 2)                           //减去隐藏位,获得整数位二进制串
                
                
        //        //------------------------------------------------------------------------
                
        //        sLen = Len(cZs)                             //获得移位位数
        //        cExponent = Hex(sLen + 1023)                //双精度再加上1023
        //        For i = 1 To Len(cExponent)
        //            sStr = sStr & cBCDToBit(Mid(cExponent, i, 1))
        //        Next
        //        cExponent = sStr
        //        cExponent = Right("00000000000" & cExponent, 11) //获得移位的二制串,固定长度11位
                
        //        //------------------------------------------------------------------------
                
        //        sStr = cSign & cExponent & cZs              //取得除尾数位的二进制串
        //        sLen = 64 - Len(sStr)                       //取得尾数的二进制串长度
                
        //        sWs = sWs * (2 ^ sLen)
        ////        sWs = Fix(sWs)
        //        cWs = Hex(sWs)                              //调试时溢出,Hex最大只能转成8个F
        //        sStr = ""
        //        For i = 1 To Len(cWs)
        //            sStr = sStr & cBCDToBit(Mid(cWs, i, 1))
        //        Next
        //        cWs = sStr
        //        cWs = Right("0000000000000000000000000000000000000000000000000000" & cWs, sLen)      //取得尾数位二进制串,最长为52位
                    
        //        //--------------------------------------------------------------------------
        //        sStr = cSign & cExponent & cZs & cWs
        //        For i = 1 To 64 Step 4
        //            tStr = tStr & cBitToBCD(Mid(sStr, i, 4))
        //        Next
        //        cDouble = tStr
                    
        //}



        //public double cBitToLong(string cValue ) ////二进制转换成十进制
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


        //public string cBCDToBit(ByVal sByte As String) ////十六进制转换成二进制
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


        //public string cBitToBCD(ByVal sByte As String) ////二进制转换成十六进制
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
