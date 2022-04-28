using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Threading;
namespace ClAmMeterController.Test
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


                //Class1 aa = new Class1();

                //aa.Test();
            //Application.Run(new frmMain());
            Application.Run(new frmCL485_WriteSCBH());
            //Application.Run(new frmSetSystemDateTime());
        }

    }
}
