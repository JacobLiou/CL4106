
using System.IO;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using pwCollapseDataGridView;
using pwFunction.pwConst;
using pwInterface;
using System.Data.SqlClient;
using System.Xml;
using System.Xml.Linq;
namespace CL4100
{
    public partial class MOShemaForm : Form
    {
        public string WCLPathServer;
        public MOShemaForm()
        {
            InitializeComponent();
        }
        public MOShemaForm(string WcfServerUrl1)
        {
            InitializeComponent();
            WCLPathServer = WcfServerUrl1;
        }
        private void but_Clear_Click(object sender, EventArgs e)
        {
            try
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, " 出错啦! ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void but_Confirm_Click(object sender, EventArgs e)
        {
            try
            {

                if (txtMOName.Text == "") return;
                //if (txtMOName.Text.Substring(2, 1) != "2") return;
                this.Enabled = false;

                OrBitADCService.ADCService adc = new OrBitADCService.ADCService();
                string PathServer = WCLPathServer.Substring(0, WCLPathServer.LastIndexOf("/")) + "/DataService.svc";
                string MOName = txtMOName.Text.Trim();
                string configFileMath = Application.StartupPath;
                string StrSQL = "exec Txn_ReturnTestValue_DoMethod @MOName='" + MOName + "',@SchemaType='30'";
                DataTable CheckDT = adc.GetDataSetWithSQLString(PathServer, StrSQL).Tables[0];
                string I_ReturnMessage = CheckDT.Rows[0]["I_ReturnMessage"].ToString();
                if (I_ReturnMessage.Trim() != "")
                {
                    MessageBox.Show(I_ReturnMessage);
                    return;
                }
                string FileXML = CheckDT.Rows[0]["FileXML"].ToString();
                string ProductName = CheckDT.Rows[0]["ProductName"].ToString();
                string ProductDescription = CheckDT.Rows[0]["ProductDescription"].ToString();
                string CustomerName = CheckDT.Rows[0]["CustomerShortName"].ToString();

                #region 初始化配置文件
                if (File.Exists(configFileMath + "\\WorkPlan\\WorkPlan.xml"))
                {
                    File.Delete(configFileMath + "\\WorkPlan\\WorkPlan.xml");
                }
                StreamWriter sw = new StreamWriter(configFileMath + "\\WorkPlan\\WorkPlan.xml");
                sw.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" + FileXML);
                sw.Close();
                sw.Dispose();
                XDocument xmldoc = XDocument.Load(configFileMath + "\\WorkPlan\\WorkPlan.xml");
                XElement WorkInformation = xmldoc.Root.Elements("Work").Where(el => el.Attribute("Description").Value == "工单信息").First();
                WorkInformation.Elements("Work").Where(el => el.Attribute("ID").Value == "WorkSN").First().Attribute("Value").Value = MOName;
                WorkInformation.Elements("Work").Where(el => el.Attribute("ID").Value == "ProductsName").First().Attribute("Value").Value = ProductDescription;
                WorkInformation.Elements("Work").Where(el => el.Attribute("ID").Value == "ProductsSN").First().Attribute("Value").Value = ProductName;
                WorkInformation.Elements("Work").Where(el => el.Attribute("ID").Value == "CustomerName").First().Attribute("Value").Value = CustomerName;
                xmldoc.Save(configFileMath + "\\WorkPlan\\WorkPlan.xml");
                #endregion
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (SystemException Errorer)
            {
                MessageBox.Show( "调用出错，无测试方案。" + Errorer.Message);
                return;
            }
            finally
            {
                this.Enabled = true;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string WcfServerUrl = "http://10.98.99.6:800/browserWCFService/BrowserService.svc";//"http://localhost/WCFService";

            string PathServer = WcfServerUrl.Substring(0, WcfServerUrl.LastIndexOf("/")) + "/DataService.svc";

            OrBitADCService.ADCService adc = new OrBitADCService.ADCService();

            //string StrSQL = "exec GetNowSystemTime";
            string StrSQL = @"select top 1 S_ClientBarcode from CL_AfterloadingProduction_Record where S_ClientBarcode='"
                            + "1" + "' and S_WorkOrderID='"
                            + "2" + "' and S_ProductionID='"
                            + "3" + "' and D_DateTime='"
                            + System.DateTime.Now  + "'";

            DataSet ds1 = adc.GetDataSetWithSQLString(PathServer, StrSQL);

            if (ds1 != null && ds1.Tables[0].Rows.Count == 0)
            {
                //logInfo = "保存到数据库 失败！！";
                //bln_Result = false;
                //MessageBox.Show(logInfo);
                //break;
            }
            else
            {
                //bln_Result = true;
            }


            //DataTable NowSystemTimeDT = adc.GetDataSetWithSQLString(PathServer, StrSQL).Tables[0];
            //System.DateTime _curTime = Convert.ToDateTime(NowSystemTimeDT.Rows[0][0].ToString());
            //this.Text = _curTime.ToString();

        }

        private void txtMOName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                but_Confirm_Click(sender, e);
            }

        }


    }
}




