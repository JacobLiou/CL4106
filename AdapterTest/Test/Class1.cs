using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
namespace ClAmMeterController.Test
{
    public class Class1
    {
        private  object Locked = new object();

        public  void SaveCommunicationData(object obj)
        {
            lock (Locked)
            {
                try
                {
                    string CONST_COMMUNICATIONDATA = "\\Comm\\";
                    string m_filepath = Application.StartupPath  //写文件目录
                            + CONST_COMMUNICATIONDATA
                            + DateTime.Now.ToShortDateString();

                    string str_Frame = "F0FF00000000B004000011000000000098088813305B030054C3000054C3000030F4470330F4470300000000000000000F0000000000000000000000000000000000000000000000";


                    #region 写入文件

                    string sfilename = m_filepath + "\\" + "CommunicationData.txt";
                    StreamWriter sw = new StreamWriter(@sfilename, true, System.Text.Encoding.Unicode);
                    sw.WriteLine(str_Frame);
                    sw.Close();
                    #endregion
                }
                catch (Exception e)
                {
                    return;
                }
            }
        }

        public void Test()
        {

            object obj;


            string CONST_COMMUNICATIONDATA = "\\Comm\\";
            string m_filepath = Application.StartupPath  //写文件目录
                    + CONST_COMMUNICATIONDATA
                    + DateTime.Now.ToShortDateString();

            #region 创建当天目录
            for (int intCom = 0; intCom < 24; intCom++)
            {
                string _DelDirectory = m_filepath + "\\" + intCom.ToString("D2");
                if (!System.IO.Directory.Exists(_DelDirectory))
                {
                    System.IO.Directory.CreateDirectory(_DelDirectory);
                }
            }
            #endregion

            for (int intCom = 0; intCom < 24; intCom++)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(SaveCommunicationData));

            }

            Application.Run(new frmMain());
        }



    }
}
