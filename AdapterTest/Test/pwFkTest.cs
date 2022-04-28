using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using pwMeterProtocol;
using pwComPorts;
namespace ClAmMeterController.Test
{
    public partial class pwFkTest : Form
    {
        const int intBw = 6;
        private pwMeterProtocol.DLT645_2007[] m_ccl_dlt6452007 = new DLT645_2007[intBw];
        private pwComPorts.CCL20181[] ccl_CL20181 = new CCL20181[intBw];
        private Thread[] TestThread = new Thread[intBw];
        private pwYcfkClass[] pwYcfk = new pwYcfkClass[intBw];

        private TextBox[] txtBox = new TextBox[12];

        private bool bIsReading = false;
        private bool bIsStop = false;
        int iWaitTimes = 100;
        int[] iCs = new int[intBw];
        int[] iCountS = new int[intBw];
        int[] iCountF = new int[intBw];
        public pwFkTest()
        {
            InitializeComponent();
        }

        private void pwFkTest_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < intBw; i++)
            {
                m_ccl_dlt6452007[i] = new DLT645_2007();

                ccl_CL20181[i] = new CCL20181();
                ccl_CL20181[i].PortOpen("1,193.168.18.1:10003:20000");
                ccl_CL20181[i].Setting = "2400,E,8,1";
                m_ccl_dlt6452007[i].ComPort = ccl_CL20181[i];
                m_ccl_dlt6452007[i].Setting = ccl_CL20181[i].Setting;

                m_ccl_dlt6452007[i].VerifyPasswordType = 1;
                m_ccl_dlt6452007[i].PasswordClass = 0;
                m_ccl_dlt6452007[i].Password = "000000";
                m_ccl_dlt6452007[i].WaitDataRevTime = 1000;
                m_ccl_dlt6452007[i].IntervalTime = 500;
                pwYcfk[i] = new pwYcfkClass();
                pwYcfk[i].sMeterSerial = "0000000000000001";
            }
            txtBox[0] = textBox1;
            txtBox[1] = textBox2;
            txtBox[2] = textBox3;
            txtBox[3] = textBox4;
            txtBox[4] = textBox5;
            txtBox[5] = textBox6;
            txtBox[6] = textBox7;
            txtBox[7] = textBox8;
            txtBox[8] = textBox9;
            txtBox[9] = textBox10;
            txtBox[10] = textBox11;
            txtBox[11] = textBox12;
        }

        private bool ReadAddress(int Bw)
        {
            string cDataStr = "";
            bool bE = false;
            m_ccl_dlt6452007[0].Address = "AAAAAAAAAAAA";
            bool bT = m_ccl_dlt6452007[0].ReadMeterAddress(ref cDataStr);
            return bT;
        }

        #region
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            string sData = "";
            bool bt = m_ccl_dlt6452007[0].ReadData("04000402", 6, ref sData);
            if (bt)
            {
                toolStripTextBox1.Text = sData.PadLeft(16, '0');
                pwYcfk[0].sMeterSerial = toolStripTextBox1.Text;
                label1.Text  = "分散因子 读取成功";
            }
            else
            {
                label1.Text  = "分散因子 读取失败";
            }


        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            pwYcfk[0].bLogin = pwIdentityAuthentication(0,ref pwYcfk[0].sRnad2, ref pwYcfk[0].sESAMSerial);
            if (pwYcfk[0].bLogin)
            {
                label1.Text  = "身份认证成功";
            }
            else
            {
                label1.Text = "身份认证失败";
            }
        }
        #endregion

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (bIsReading)
            {
                MessageBox.Show("Reading......");
                return;
            }
            bIsReading = true;
            bIsStop = false;
            toolStripButton1.Enabled = false;
            toolStripButton2.Enabled = true;

            for (int k = 0; k < intBw; k++)
            {
                ReadAddress(k);
            }


            for (int i = 0; i < intBw; i++)
            {
                iCs[i] = 0;
                iCountS[i] = 0;
                iCountF[i] = 0;

                ParameterizedThreadStart ptt_ParaThreadStart = new ParameterizedThreadStart(TestDll);  //委托函数

                TestThread[i] = new Thread(ptt_ParaThreadStart);

                TestThread[i].Name = "Thread" + i.ToString();

                TestThread[i].IsBackground = true;

            }

            for (int int_Inc = 0; int_Inc < intBw; int_Inc++)
            {
                TestThread[int_Inc].Start(int_Inc);              //运行线程
            }

            //this.BeginInvoke(new MethodInvoker(TestDll));

        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            bIsStop = true;

        }

        private void toolStripTextBox2_Click(object sender, EventArgs e)
        {
            iWaitTimes = Convert.ToInt32(toolStripTextBox1.Text.Trim());   //延时

        }


        private void TestDll(object intBw)
        {
            try
            {
                bool bt;
                int _intBw = Convert.ToInt32(intBw);
                while (true)
                {
                    iCs[_intBw]++;

                    bt = ReadAddress(_intBw);

                    //==============================
                    bt = pwIdentityAuthentication(_intBw,ref pwYcfk[_intBw].sRnad2, ref pwYcfk[_intBw].sESAMSerial);
                    if (bt)
                    {
                        iCountS[_intBw]++;
                        txtBox[_intBw*2].Text = iCountS[_intBw].ToString();
                    }
                    else
                    {
                        iCountF[_intBw]++;
                        txtBox[_intBw * 2 +1 ].Text = iCountF[_intBw].ToString();
                    }
                    label1.Text = "第" + iCs.ToString() + "次测试......身份认证" + (bt ? "成功" : "失败");

                    //==============================
                    string sData = "112233445566";
                    bt = UpdataParameter1(_intBw,"0400040E", "04d682", "24", sData);
                    if (bt)
                    {
                        iCountS[_intBw]++;
                        txtBox[_intBw * 2].Text = iCountS[_intBw].ToString();
                    }
                    else
                    {
                        iCountF[_intBw]++;
                        txtBox[_intBw * 2 + 1].Text = iCountF[_intBw].ToString();
                    }
                    label1.Text = "第" + iCs.ToString() + "次测试......设置客户编号" + (bt ? "成功" : "失败");


                    //============================
                    int _Min = 5;
                    bt = but_SetYx_Click(_intBw,_Min);
                    if (bt)
                    {
                        iCountS[_intBw]++;
                        txtBox[_intBw * 2].Text = iCountS[_intBw].ToString();
                    }
                    else
                    {
                        iCountF[_intBw]++;
                        txtBox[_intBw * 2 + 1].Text = iCountF[_intBw].ToString();
                    }
                    label1.Text = "第" + iCs.ToString() + "次测试......设置身份认证有效时间" + (bt ? "成功" : "失败");

                    //停止
                    if (bIsStop) break;

                    //延时
                    Thread.Sleep(iWaitTimes);

                }


            }
            catch (Exception eError)
            {
                MessageBox.Show(eError.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                bIsReading = false;
                bIsStop = false;
                toolStripButton1.Enabled = true;
                toolStripButton2.Enabled = false;

            }
        }

        #region 身份认证，返回随机数，EMS序列号---------OK

        /// <summary>
        /// ESAM身证认证
        /// </summary>
        /// <param name="sRnad2">返回：随机数2</param>
        /// <param name="sESAMSerial">返回：ESAM序列号</param>
        /// <returns></returns>
        public bool pwIdentityAuthentication(int _intBw, ref string sRnad2, ref  string sESAMSerial)//身份认证
        {//sRnad2 随机数2         sESAMSerial ESAM序列号

            string sRandAndEndata = ""; //发送分散因子，取随机数1及密文1

            sRandAndEndata = pwGetRandAndEndata(_intBw);
            if (sRandAndEndata == "")
            {
                return false;
            }
            bool bt = m_ccl_dlt6452007[_intBw].Fk_IdentityAuthentication(sRandAndEndata, pwYcfk[_intBw].sMeterSerial, ref sRnad2, ref sESAMSerial);
            if (bt)
            {
                pwYcfk[_intBw].sRnad2 = sRandAndEndata;
                pwYcfk[_intBw].sESAMSerial = sESAMSerial;
            }
            return bt;


        }

        /// <summary>
        /// 发送分散因子，取随机数1及密文1
        /// </summary>
        /// <returns></returns>
        public string pwGetRandAndEndata(int _intBw)//取随机数1及密文1
        {
            byte iCount = 0;            //返回值
            string sRandAndEndata = ""; //返回取随机数1及密文1
            string str_LostMessage = "";//返回消息
            label1.Text = "身份认证中........";
            pwYcfk[_intBw].sMeterSerial = toolStripTextBox1.Text.PadLeft(16, '0');//分散因子8byte，0000+表号
            iCount = pwYcfk[_intBw].pwIdentityAuthentication(pwYcfk[_intBw].sMeterSerial, ref sRandAndEndata, ref str_LostMessage);
            if (iCount == 0)
            {
                pwYcfk[_intBw].sRnad1 = sRandAndEndata.Substring(0, 16);
                return sRandAndEndata;
            }
            else
            {
                MessageBox.Show(str_LostMessage);
                return "";
            }

        }

        #endregion

        #region 设置身份认证有效时间，失效-----------OK
        private bool but_SetYx_Click(int _intBw,int intMin)
        {

            try
            {

                string sInputData = intMin.ToString().PadLeft(4, '0');

                #region
                //MAC计算----
                string sInputData2 = "";//输入参数,4 字节随机数+8 字节分散因子+5 字节指令(04d682+LC)+数据明文+4 字节MAC。LC=明文长度+0x04； 
                string dataout = "";//返回值
                string str_LostMessage = "";//返回消息
                sInputData2 = pwYcfk[_intBw].sRnad2;//随机数2
                sInputData2 += pwYcfk[_intBw].sMeterSerial;// Function2.BackString(sMeterSerial);//分散因子
                sInputData2 += "04d682";
                sInputData2 += "2B"; ;//起始偏移量，身份认证有效时间"0x2B"
                sInputData2 += Convert.ToInt32(sInputData.Length / 2 + 4).ToString("X2");//明文长度+4MAC
                sInputData2 += sInputData;// Function2.BackString(sInputData);//数据

                byte iReturn = pwYcfk[_intBw].pwParameterUpdate(sInputData2, ref dataout, ref str_LostMessage);

                if (iReturn == 0)
                {
                    string sMeterInfo = "";
                    string sN1NM = dataout.Substring(dataout.Length - 8) + dataout.Substring(0, dataout.Length - 8);// Function2.BackString(dataout);// dataout;// 

                    bool bt = m_ccl_dlt6452007[_intBw].Fk_IdentityYxWx(sN1NM, ref sMeterInfo);
                    return bt;
                }
                else
                {
                    return false;

                }
                #endregion

            }
            catch
            {
                return false;

            }
            finally
            {
            }

        }
        #endregion

        #region 客户编号//电流互感器变比//电压互感器变比更新
        private bool UpdataParameter1(int _intBw,string sCode, string sFileCode, string sFileLen, string sInputData)//参数1更新
        {
            //标识编码//文件标识//偏移量/输入数据明文
            //电流互感器变比：04000306//04d682//18
            //电压互感器变比：04000307//04d682//1B
            //客户编号：0400040E//04d682//24
            try
            {
                #region UpdataParameter1

                #region
                //MAC计算----
                string sInputData2 = "";//输入参数,4 字节随机数+8 字节分散因子+5 字节指令(04d682+LC)+数据明文+4 字节MAC。LC=明文长度+0x0C； 
                string dataout = "";//返回值
                string str_LostMessage = "";//返回消息
                sInputData2 = pwYcfk[_intBw].sRnad2;//随机数2
                sInputData2 += pwYcfk[_intBw].sMeterSerial;//分散因子sMeterSerial;//
                sInputData2 += sFileCode; //"04d682";
                sInputData2 += sFileLen; ;//起始偏移量，报警金额1偏移量"10"
                sInputData2 += Convert.ToInt32(sInputData.Length / 2 + 4).ToString("X2");//明文长度+4
                sInputData2 += sInputData;//数据sInputData;// 


                byte iReturn = pwYcfk[_intBw].pwParameterUpdate(sInputData2, ref dataout, ref str_LostMessage);

                if (iReturn == 0)
                {

                    string sN1NM = m_ccl_dlt6452007[_intBw].BackString(dataout.Substring(0, dataout.Length - 8)) + m_ccl_dlt6452007[_intBw].BackString(dataout.Substring(dataout.Length - 8));//dataout;//明文＋MAC//// Function2.BackString(dataout);//

                    bool bt = m_ccl_dlt6452007[_intBw].Fk_WriteData_Str(sCode, sN1NM, "99");

                    return bt;
                }
                else
                {
                    return false;

                }
                #endregion
                #endregion
            }
            catch
            {
                return false;

            }
            finally
            {
            }


        }
        #endregion


    }
}
