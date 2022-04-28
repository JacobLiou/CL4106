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
using System.IO;
namespace CL4100
{

    public partial class pw_Main : Form
    {
        #region 变量声明
        private Thread threadDo = null;                                     //检定线程
        private Display[] DisMeter = new Display[0];                        //灯泡
        public static event DelegateEventResuleChange OnEventResuleChange;  //结果改变事件
        public static event DelegateEventStatusChange OnEventStatusChange;  //状态改变事件
        public static event DelegateEventTxmChange OnEventTxmChange;        //输入框

        public static event DelegateEventTxmtextSet OnEventTxmtextSet;        //文本框回传

        private Stopwatch sth_SpaceTicker = new Stopwatch();                //记时时钟
        private ListView m_listView = new ListView();                       //消息记录器
        bool[] m_bln_Selected = new bool[GlobalUnit.g_BW];  //选中操作  
        /// <summary>
        /// 显示详细信息、误差限的窗体
        /// </summary>
        private CL4100.ShowDataView.pwCheckShowDataVew ui_Popup = null;

        public static pw_Main p;
        //private Mutex m_Main_Mutex = new Mutex(false, "CL4101_02_Main");
        //private stPrjParameter m_stPrjParameter;                                                    //项目参数结构
        #endregion

        #region 初始化

        public pw_Main()
        {
            InitializeComponent();


            //主界面　
            this.TopMost = true;
            this.TopMost = false;
            this.WindowState = FormWindowState.Maximized;
            StatusMain_Proc.Value = 0;
            //this.Text += " " + GlobalUnit.g_Desktype.ToString();
            StatusMain_Mode.Text = GlobalUnit.IsDemo ? "演示模式" : "正式模式";

            initializeVariable();
            p = this;

            splitContainer4.Panel1Collapsed = true;
            //menuStrip.Visible = false;
            menuStrip.Visible = true;
            //splitContainer1.IsSplitterFixed = false;
        }

        private void InitProgram()
        {
            #region Program
            #region  开始加载
            pwClassLibrary.TopWaiting.ShowWaiting("系统正在加载...");
            #endregion

            #region 判断系统文件是否存在
            string[] _DLLFile = new string[] {"pwClassLibrary.dll", "pwInterface.dll", "pwFunction.dll", 
                                                "pwComPorts.dll", "pwAmMeterController.dll","pwMeterProtocol.dll" ,
                                                "pwCollapseDataGridView.dll", "PropertyGridEx.dll", "Interop.ADOX.dll"};

            foreach (string _dllfile in _DLLFile)
            {
                if (!System.IO.File.Exists(string.Format(@"{0}\{1}", System.Windows.Forms.Application.StartupPath, _dllfile)))
                {
                    pwClassLibrary.TopWaiting.ShowWaiting("系统文件：" + _dllfile.ToString() + "不存在或损坏，系统无法加载！");
                    Application.Exit();
                }
                else
                {
                    pwClassLibrary.TopWaiting.ShowWaiting("正在检查系统文件：" + _dllfile.ToString() + "......");
                }
            }
            #endregion


            #region 删除1周前数据
            pwClassLibrary.TopWaiting.ShowWaiting("正在删除一周前历史数据文件......");
            if (System.IO.Directory.Exists(System.Windows.Forms.Application.StartupPath + pwFunction.pwConst.Variable.CONST_COMMUNICATIONDATA))
            {
                string[] fileList = System.IO.Directory.GetFileSystemEntries(System.Windows.Forms.Application.StartupPath + pwFunction.pwConst.Variable.CONST_COMMUNICATIONDATA);
                // 遍历所有的文件和目录,大于7天删除
                foreach (string file in fileList)
                {
                    DateTime datimeCreat = System.IO.Directory.GetCreationTime(file);
                    if (datimeCreat < DateTime.Today.AddDays(-7f))
                        System.IO.Directory.Delete(file, true);
                }
            }
            else
            {
                System.IO.Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + pwFunction.pwConst.Variable.CONST_COMMUNICATIONDATA);
            }
            #endregion

            #region 创建当天目录
            string _DelDirectoryTaday = System.Windows.Forms.Application.StartupPath + pwFunction.pwConst.Variable.CONST_COMMUNICATIONDATA
                + DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString(); ;
            if (!System.IO.Directory.Exists(_DelDirectoryTaday))
            {
                System.IO.Directory.CreateDirectory(_DelDirectoryTaday);
            }
            #endregion



            #region 加载系统配置
            if (pwFunction.pwConst.GlobalUnit.g_SystemConfig == null)
            {
                pwClassLibrary.TopWaiting.ShowWaiting("正在加载系统配置......");
                pwFunction.pwConst.GlobalUnit.g_SystemConfig = new pwFunction.pwSystemModel.SystemInfo();
            }
            pwFunction.pwConst.GlobalUnit.g_SystemConfig.Load();   //刷新
            #endregion

            #region 加载工单配置
            if (pwFunction.pwConst.GlobalUnit.g_Work == null)
            {
                pwClassLibrary.TopWaiting.ShowWaiting("正在加载工单配置......");
                pwFunction.pwConst.GlobalUnit.g_Work = new pwFunction.pwWork.cWork();
            }
            pwFunction.pwConst.GlobalUnit.g_Work.Load();   //刷新
            #endregion

            #region 创建当前工单检定数据本地存储目录
            string _DelDirectoryWorkData = System.Windows.Forms.Application.StartupPath + pwFunction.pwConst.Variable.CONST_METERDATA + pwFunction.pwConst.GlobalUnit.g_Work.WorkSN;
            if (!System.IO.Directory.Exists(_DelDirectoryWorkData))
            {
                System.IO.Directory.CreateDirectory(_DelDirectoryWorkData);
            }
            #endregion


            #region 加载产品配置
            if (pwFunction.pwConst.GlobalUnit.g_Products == null)
            {
                pwClassLibrary.TopWaiting.ShowWaiting("正在加载产品配置......");
                pwFunction.pwConst.GlobalUnit.g_Products = new pwFunction.pwProducts.cProducts();
            }
            pwFunction.pwConst.GlobalUnit.g_Products.Load();   //刷新
            #endregion

            #region 加载方案配置
            if (pwFunction.pwConst.GlobalUnit.g_Plan == null)
            {
                pwClassLibrary.TopWaiting.ShowWaiting("正在加载方案配置......");
                pwFunction.pwConst.GlobalUnit.g_Plan = new pwFunction.pwPlan.cPlan();
            }
            pwFunction.pwConst.GlobalUnit.g_Plan.Load();   //刷新
            #endregion

            #region 加载表配置，从工单及产品
            if (pwFunction.pwConst.GlobalUnit.g_Meter == null)
            {
                pwClassLibrary.TopWaiting.ShowWaiting("正在初始化表参数......");
                pwFunction.pwConst.GlobalUnit.g_Meter = new pwFunction.pwMeter.MeterBasic(pwFunction.pwConst.GlobalUnit.g_BW);
            }
            pwFunction.pwConst.GlobalUnit.g_Meter.InitData();   //初始化结果数据
            #endregion

            #region 初始化提示消息
            pwClassLibrary.TopWaiting.ShowWaiting("正在加载提示消息......");
            pwFunction.pwConst.GlobalUnit.g_MsgControl = new pwClassLibrary.VerifyMsgControl();          //消息队列
            pwFunction.pwConst.GlobalUnit.g_MsgControl.SleepTime = 20;                                   //消息轮循间隔(ms)，此越小处理消息速度越快
            pwFunction.pwConst.GlobalUnit.g_MsgControl.IsMsg = true;
            pwClassLibrary.Check.m_MsgControl = pwFunction.pwConst.GlobalUnit.g_MsgControl;
            #endregion

            //#region 初始化数据消息
            //pwClassLibrary.TopWaiting.ShowWaiting("正在加载数据消息......");
            //pwFunction.pwConst.GlobalUnit.g_DataControl = new pwClassLibrary.VerifyMsgControl();         //消息队列
            //pwFunction.pwConst.GlobalUnit.g_DataControl.SleepTime = 1000;                                //消息轮循间隔(ms)，此越小处理消息速度越快
            //pwFunction.pwConst.GlobalUnit.g_DataControl.IsMsg = false;
            //#endregion

            //#region 初始化日志
            //pwClassLibrary.TopWaiting.ShowWaiting("正在加载化日志......");
            //pwFunction.pwConst.GlobalUnit.g_Log = new pwClassLibrary.RunLog();                           //日志队列
            //#endregion

            #region 初始化检定控制器
            pwClassLibrary.TopWaiting.ShowWaiting("正在加载检定控制器......");
            Adapter g_Adapter = new Adapter();
            #endregion

            #region 结束加载
            pwClassLibrary.TopWaiting.HideWaiting();
            #endregion
            #endregion
        }

        private void InitMain()
        {
            #region Main_Load

            //Load
            pwClassLibrary.TopWaiting.ShowWaiting("正在启动主界面......");

            //初始化监视器
            pwClassLibrary.TopWaiting.ShowWaiting("正在加载监视器......");
            this.BeginInvoke(new MethodInvoker(InitMonitor));

            //标准表读取事件
            Adapter.OnReadStaInfo += new OnEventReadStdInfo(updateStdInfo);

            //工单产品显示
            pwClassLibrary.TopWaiting.ShowWaiting("正在加载工单及产品......");
            this.BeginInvoke(new MethodInvoker(DisplayWorkAndProducts));

            //方案显示
            pwClassLibrary.TopWaiting.ShowWaiting("正在加载方案......");
            this.BeginInvoke(new MethodInvoker(InitPLan));

            //初始化结果界面（灯泡显示界面）
            pwClassLibrary.TopWaiting.ShowWaiting("正在加载灯泡显示......");
            this.BeginInvoke(new MethodInvoker(thLoadUserControl));

            //初始详细数据显示界面
            pwClassLibrary.TopWaiting.ShowWaiting("正在加载数据显示界面......");
            this.BeginInvoke(new MethodInvoker(LoadDataView));

            //检定项目改变事件
            Adapter.OnEventItemChange += new DelegateEventItemChange(DisplayRow);

            //消息事件
            m_listView = listView1;
            m_listView.Items.Clear();
            m_listView.Columns[2].Width = m_listView.Parent.Width - 180;
            MessageToolStripMenuItem.Checked = true;
            splitContainer3.Panel2Collapsed = false;

            pwFunction.pwConst.GlobalUnit.g_MsgControl.ShowMsg += new pwClassLibrary.VerifyMsgControl.OnShowMsg(myShowMsg);
            Thread myMsgThread = new Thread(pwFunction.pwConst.GlobalUnit.g_MsgControl.DoWork);
            myMsgThread.Start();

            ////数据事件
            //pwFunction.pwConst.GlobalUnit.g_DataControl.ShowMsg += new pwFunction.VerifyMsgControl.OnShowMsg(myShowData);
            //Thread myDataThread = new Thread(pwFunction.pwConst.GlobalUnit.g_DataControl.DoWork);
            //myDataThread.Start();

            pwClassLibrary.TopWaiting.ShowWaiting("系统已全部加载完成！");
            //Thread.Sleep(50);
            pwClassLibrary.TopWaiting.HideWaiting();
            #endregion


            for (int i = 0; i < GlobalUnit.g_BW; i++)
            {
                m_bln_Selected[i] = true;
                GlobalUnit.g_Meter.MData[i].bolIsCheck = true;
            }        
        }

        private void pw_Main_Shown(object sender, EventArgs e)
        {
            try
            {

                InitProgram();

                InitMain();

                #region 联机
                联机ToolStripMenuItem_Click(sender, e);
                #endregion
            }
            catch
            {
            }
            finally
            {
                pwClassLibrary.TopWaiting.HideWaiting();
            }

            //pw_TTTxm tttxm = new pw_TTTxm();
            //tttxm.ShowDialog();
        }

        private void pw_MDIParent_Resize(object sender, EventArgs e)
        {
            StatusMain_Text.Width = this.Width - StatusMain_Proc.Width - 85 - StatusMain_TxtStatus1.Width - StatusMain_TxtStatus2.Width - StatusMain_Mode.Width;
            if (UIMonitor != null)
            {
                UIMonitor.Btn_Expend_Click(sender, e);
            }

        }

        private void pw_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (GlobalUnit.g_Status == enmStatus.进行中)
            {
                DialogResult result = MessageBox.Show("Do you want to Quit ?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (result == DialogResult.Yes)
                {
                    pwFunction.pwConst.GlobalUnit.ApplicationIsOver = true;
                    pwFunction.pwConst.GlobalUnit.ForceVerifyStop = true;
                    GlobalUnit.g_Status = enmStatus.停止;

                    e.Cancel = false;
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                pwFunction.pwConst.GlobalUnit.ApplicationIsOver = true;
                pwFunction.pwConst.GlobalUnit.ForceVerifyStop = true;
                GlobalUnit.g_Status = enmStatus.停止;

            }

        }

        #region 过程数据UI 作废
        private void InitRowdataGridView()
        {


            //this.dataGridView2.Columns.Clear();
            //DataGridViewColumnCollection Cols2 = this.dataGridView2.Columns;
            //DataGridViewTextBoxColumn cols;
            //for (int i = 0; i < 24; i++)
            //{
            //    cols = new DataGridViewTextBoxColumn();
            //    cols.Name = "ColumnNo" + i.ToString();
            //    cols.HeaderText = Convert.ToInt32(i + 1).ToString("D2") + "表";
            //    cols.Width = 43;//this.dataGridView2.Width / 24;
            //    Cols2.Add(cols);

            //    //Cols2.Add("Column" + i.ToString(), i.ToString("D2") + "表");

            //}


            //this.dataGridView2.Rows.Clear();
            //DataGridViewRowCollection rows2 = this.dataGridView2.Rows;
            //DataGridViewRow rowss;
            //object[] sRowText = new object[] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            //for (int i = 0; i < 24; i++)
            //{
            //    rowss = new DataGridViewRow();
            //    rowss.SetValues(sRowText);
            //    rows2.Add(rowss);
            //}
            //dataGridView2.ReadOnly = true;
        }
        #endregion

        #endregion

        #region 工单UI
        private void toolStrip1_Resize(object sender, EventArgs e)
        {
            toolStripLabel4.Visible = false;
            toolStripTextBox4.Visible = false;

            int iLen;
            iLen = this.Width - toolStripLabel6.Width;
            iLen = iLen - toolStripTextBox5.Width - toolStripLabel5.Width;
            //iLen = iLen - toolStripTextBox4.Width - toolStripLabel4.Width;
            iLen = iLen - toolStripTextBox3.Width - toolStripLabel5.Width;
            iLen = iLen - toolStripTextBox2.Width - toolStripLabel6.Width;
            iLen = iLen - toolStripTextBox1.Width - toolStripLabel1.Width;
            toolStripTextBox6.Width = iLen - 35;
        }

        private void DisplayWorkAndProducts()
        {

            if (pwFunction.pwConst.GlobalUnit.g_Work != null)
            {
                toolStripTextBox1.Text = pwFunction.pwConst.GlobalUnit.g_Work.WorkSN;
                toolStripTextBox2.Text = pwFunction.pwConst.GlobalUnit.g_Work.CustomerName;
                toolStripTextBox3.Text = pwFunction.pwConst.GlobalUnit.g_Work.ProductsName;
                toolStripTextBox4.Text = pwFunction.pwConst.GlobalUnit.g_Work.ProductsSN;
                toolStripTextBox5.Text = pwFunction.pwConst.GlobalUnit.g_Work.ProductsModel;
            }
            if (pwFunction.pwConst.GlobalUnit.g_Products != null)
            {
                toolStripTextBox6.Text = pwFunction.pwConst.GlobalUnit.g_Products.Jion();
            }

        }
        #endregion

        #region 方案UI
        public void InitPLan()
        {
            this.collapseDataGridView1.Visible = false;
            this.collapseDataGridView1.RowHeadersWidth = 0x1b;
            this.collapseDataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.collapseDataGridView1.GroupEvenColor = Color.White;
            this.collapseDataGridView1.GroupOddColor = Color.White;
            this.collapseDataGridView1.ImgCollapse = this.imageList1.Images[0];
            this.collapseDataGridView1.ImgExpand = this.imageList1.Images[1];
            this.collapseDataGridView1.Columns.Clear();
            DataGridViewColumnCollection columns = this.collapseDataGridView1.Columns;
            DataGridViewTextBoxColumn dataGridViewColumn = new DataGridViewTextBoxColumn();
            dataGridViewColumn.Name = "Column1";
            dataGridViewColumn.HeaderText = "分组";
            dataGridViewColumn.DataPropertyName = "intGroupNum";
            dataGridViewColumn.Width = 0;
            dataGridViewColumn.ReadOnly = true;
            dataGridViewColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridViewColumn.Visible = false;
            columns.Add(dataGridViewColumn);
            dataGridViewColumn = new DataGridViewTextBoxColumn();
            dataGridViewColumn.Name = "Column2";
            dataGridViewColumn.HeaderText = "序号";
            dataGridViewColumn.DataPropertyName = "bytLen";
            dataGridViewColumn.Width = 40;
            dataGridViewColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridViewColumn.ReadOnly = true;
            columns.Add(dataGridViewColumn);
            dataGridViewColumn = new DataGridViewTextBoxColumn();
            dataGridViewColumn.Name = "Column3";
            dataGridViewColumn.HeaderText = "名称";
            dataGridViewColumn.DataPropertyName = "strName";
            dataGridViewColumn.Width = (this.collapseDataGridView1.Width - columns[1].Width) - 30;
            dataGridViewColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridViewColumn.ReadOnly = true;
            columns.Add(dataGridViewColumn);
            this.collapseDataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.collapseDataGridView1.ColumnHeadersHeight = 30;
            this.collapseDataGridView1.Rows.Clear();
            NameCodeGroupList groupList = this.InitGrouptData();
            this.collapseDataGridView1.BindDataSource<NameCodeGroupList, NameCode>(groupList);
            this.collapseDataGridView1.Visible = true;
            this.collapseDataGridView1.EndEdit();
            foreach (DataGridViewRow row in this.collapseDataGridView1.Rows)
            {
                if (row.Cells[2].Value.ToString() == enmMeterPrjID.误差检定.ToString())
                {
                    for (int i = 1; i < row.Cells.Count; i++)
                    {
                        row.Cells[i].Style.BackColor = Color.GreenYellow;
                    }
                }
            }

        }

        /// <summary>
        /// 获取方案数据
        /// </summary>
        /// <returns></returns>
        private NameCodeGroupList InitGrouptData()
        {
            NameCodeGroupList list = new NameCodeGroupList();
            NameCodeItemList[] listArray = new NameCodeItemList[30];
            int index = 0;
            byte num2 = 0;
            if (GlobalUnit.g_Plan.cReadScbh.IsCheck)
            {
                index = 0;
                listArray[index] = new NameCodeItemList();
                listArray[index].GroupNum = index;
                NameCode item = new NameCode();
                item.intGroupNum = index;
                num2 = (byte)(num2 + 1);
                item.bytLen = num2;
                item.strName = enmMeterPrjID.RS485读生产编号.ToString();
                listArray[index].Add(item);
                list.Add(listArray[index]);
            }
            if (GlobalUnit.g_Plan.cWcjd.IsCheck)
            {
                index++;
                listArray[index] = new NameCodeItemList();
                listArray[index].GroupNum = index;
                NameCode code3 = new NameCode();
                code3.intGroupNum = index;
                num2 = (byte)(num2 + 1);
                code3.bytLen = num2;
                code3.strName = enmMeterPrjID.误差检定.ToString();
                listArray[index].Add(code3);
                for (int i = 0; i < GlobalUnit.g_Plan.cWcPoint._WcPoint.Count; i++)
                {
                    NameCode code2 = new NameCode();
                    code2.intGroupNum = index;
                    num2 = (byte)(num2 + 1);
                    code2.bytLen = num2;
                    code2.strName = GlobalUnit.g_Plan.cWcPoint._WcPoint[i].PrjName;
                    listArray[index].Add(code2);
                }
                list.Add(listArray[index]);
            }
            if (GlobalUnit.g_Plan.cDgnSy.IsCheck)
            {
                index++;
                listArray[index] = new NameCodeItemList();
                listArray[index].GroupNum = index;
                NameCode code4 = new NameCode();
                code4.intGroupNum = index;
                num2 = (byte)(num2 + 1);
                code4.bytLen = num2;
                code4.strName = enmMeterPrjID.日计时误差检定.ToString();
                listArray[index].Add(code4);
                list.Add(listArray[index]);
            }
            if (GlobalUnit.g_Plan.cSinglePhaseTest.IsCheck)
            {
                index++;
                listArray[index] = new NameCodeItemList();
                listArray[index].GroupNum = index;
                NameCode code5 = new NameCode();
                code5.intGroupNum = index;
                num2 = (byte)(num2 + 1);
                code5.bytLen = num2;
                code5.strName = enmMeterPrjID.分相供电测试.ToString();
                listArray[index].Add(code5);
                list.Add(listArray[index]);
            }
            if (GlobalUnit.g_Plan.cACSamplingTest.IsCheck)
            {
                index++;
                listArray[index] = new NameCodeItemList();
                listArray[index].GroupNum = index;
                NameCode code6 = new NameCode();
                code6.intGroupNum = index;
                num2 = (byte)(num2 + 1);
                code6.bytLen = num2;
                code6.strName = enmMeterPrjID.交流采样测试.ToString();
                listArray[index].Add(code6);
                list.Add(listArray[index]);
            }
            if (GlobalUnit.g_Plan.cReadEnergy.IsCheck)
            {
                index++;
                listArray[index] = new NameCodeItemList();
                listArray[index].GroupNum = index;
                NameCode code7 = new NameCode();
                code7.intGroupNum = index;
                num2 = (byte)(num2 + 1);
                code7.bytLen = num2;
                code7.strName = enmMeterPrjID.读电能表底度.ToString();
                listArray[index].Add(code7);
                list.Add(listArray[index]);
            }
            if (GlobalUnit.g_Plan.cDownPara.IsCheck)
            {
                index++;
                listArray[index] = new NameCodeItemList();
                listArray[index].GroupNum = index;
                NameCode code8 = new NameCode();
                code8.intGroupNum = index;
                num2 = (byte)(num2 + 1);
                code8.bytLen = num2;
                code8.strName = enmMeterPrjID.打包参数下载.ToString();
                listArray[index].Add(code8);
                list.Add(listArray[index]);
            }
            if (GlobalUnit.g_Plan.cSysClear.IsCheck)
            {
                index++;
                listArray[index] = new NameCodeItemList();
                listArray[index].GroupNum = index;
                NameCode code9 = new NameCode();
                code9.intGroupNum = index;
                num2 = (byte)(num2 + 1);
                code9.bytLen = num2;
                code9.strName = enmMeterPrjID.系统清零.ToString();
                listArray[index].Add(code9);
                list.Add(listArray[index]);
            }
            return list;
        }
        #endregion

        #region 监视器

        /// <summary>
        /// 监视器
        /// </summary>
        public Monitor UIMonitor;

        private void InitMonitor()
        {
            UIMonitor = new Monitor();
            UIMonitor.DanXiangTai = false;
            this.Controls.Add(UIMonitor);
            this.Controls.SetChildIndex(UIMonitor, 0);
            UIMonitor.Left = this.Width - 40;//30
            UIMonitor.Visible = false;

        }

        public void showMonitor(pwInterface.stPower tagPower)
        {
            if (this.IsHandleCreated && !this.IsDisposed)
            {
                this.BeginInvoke(new OnShowMonitor(dtShowMonitor), tagPower);
            }
        }

        /// <summary>
        /// 设置监视器数据
        /// </summary>
        /// <param name="tagPower"></param>
        private void dtShowMonitor(pwInterface.stPower tagPower)
        {
            UIMonitor.SetMonitorData(tagPower);
        }

        /// <summary>
        /// 标准表更新信息
        /// </summary>
        /// <param name="strInfo"></param>
        private void updateStdInfo(stPower strStdInfo)
        {
            try
            {
                this.showMonitor(GlobalUnit.g_StdMeter);
            }
            catch
            {

            }
        }
        #endregion

        #region 消息显示
        /// <summary>
        /// 消息处理函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="E"></param>
        private void myShowMsg(object sender, object E)
        {
            try
            {
                pwClassLibrary.EventMessageArgs Message = E as pwClassLibrary.EventMessageArgs;

                if (splitContainer3.Panel2Collapsed == false)
                {

                    int intCount = listView1.Items.Count;
                    ListViewItem lv = new ListViewItem(Convert.ToString(intCount + 1));
                    lv.SubItems.Add(DateTime.Now.ToLongTimeString());
                    lv.SubItems.Add(Message.Message);
                    m_listView.Items.Add(lv);
                    m_listView.Items[intCount].EnsureVisible();

                    StatusMain_Text.Text = GlobalUnit.g_Status.ToString();
                }
                else
                {
                    StatusMain_Text.Text = Message.Message;
                }

                //StatusMain_Text.Text = GlobalUnit.g_Status.ToString(); //Message.Message;

                StatusMain_TxtStatus2.Text = GlobalUnit.g_PowerStatus.ToString();


                /*
                    消息处理过程
                */
            }
            catch
            {
                return;
            }
        }



        /// <summary>
        /// 检定数据处理
        /// </summary>
        /// <param name="sourceAdpater">消息发出者</param>
        /// <param name="VerifyDataArgs">消息参数</param>
        private void myShowData(object sender, object E)
        {
            //pwFunction.EventMessageArgs Message = E as pwFunction.EventMessageArgs;

            //StatusMain_Text.Text = Message.Message;

            /*
                数据处理过程
            */
        }

        /// <summary>
        /// 灯泡组件
        /// </summary>
        private void thLoadUserControl()//object obj
        {
            try
            {
                Panel _CurPanel = splitContainer3.Panel1;//panel2;//

                //_CurPanel.Dock = DockStyle.Fill;
                int intBwSum = pwFunction.pwConst.GlobalUnit.g_BW;//表位总数
                int intRowCount = pwFunction.pwConst.GlobalUnit.GetConfig(pwFunction.pwConst.Variable.CTC_DESBWCOUNT, 12); ;       //每行位表数

                int intRow = 0;             //行数
                int intCol = 0;             //列数

                intCol = intRowCount;
                intRow = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(intBwSum / intRowCount)));


                TableLayoutPanel LayPanel = new TableLayoutPanel();
                LayPanel.RowCount = intRow;
                for (int i = 0; i < intRow; i++)
                {
                    LayPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, Convert.ToSingle(60 / intRow)));
                }
                LayPanel.ColumnCount = intCol;
                for (int i = 0; i < intCol; i++)
                {
                    LayPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, Convert.ToSingle(80 / intCol)));

                }

                LayPanel.Dock = DockStyle.Top;
                LayPanel.AutoSize = true;
                _CurPanel.Controls.Add(LayPanel);
                _CurPanel.BackColor = Color.Gray;
                DisMeter = new Display[intBwSum];

                #region 判断是否扫描输入
                //bool IsShaomiao = false ;
                //if (!pwFunction.pwConst.GlobalUnit.WcReadTableNo)
                //{
                //    IsShaomiao=true ;
                //}
                
                #endregion

                for (int i = 0; i < intBwSum; i++)
                {
                    DisMeter[i] = new Display(i, GlobalUnit.g_Meter.MData[i].bolIsCheck, pwFunction.pwConst.GlobalUnit.WcReadTableNo);
                    DisMeter[i].OnEventCheckedChange += new DelegateEventChenkedChange(OnEventCheckedChanged);
                    DisMeter[i].OnEventBwChange += new DelegateEventBwChange(OnEventBwChanged);
                    DisMeter[i].OnEvenTxtChange += new DelegateEventTxtChange(OnEventTxtChanged);
                    DisMeter[i].Dock = DockStyle.Fill;
                    LayPanel.Controls.Add(DisMeter[i]);
                }

            }
            catch (System.Exception errorrrr)
            {
                MessageBox.Show(errorrrr.Message);
            }
        }

        /// <summary>
        /// 选中事件处理
        /// </summary>
        /// <param name="Bw"></param>
        /// <param name="Checked"></param>
        private void OnEventCheckedChanged(int Bw, bool Checked)
        {
            GlobalUnit.g_Meter.MData[Bw].bolIsCheck = Checked;
            Adapter.blnSelected[Bw] = Checked;
            m_bln_Selected[Bw] = Checked;
            ////是否能更新到下级组件，需要调试，后面全部加入全检功能，些项作废
            //bool[] m_bln_Selected = new bool[GlobalUnit.g_BW];  //选中操作                      
            //for (int i = 0; i < GlobalUnit.g_BW; i++)           //需增加界面选中功能及处理事件
            //{
            //    m_bln_Selected[i] = GlobalUnit.g_Meter.MData[Bw].bolIsCheck;

            //}
            //Adapter.blnSelected = m_bln_Selected; 
            //bool IsShaomiao = false;
            //if (!pwFunction.pwConst.GlobalUnit.WcReadTableNo)
            //{
            //    IsShaomiao = true;
               
            //}
            DisMeter[Bw].GetEnblan(pwFunction.pwConst.GlobalUnit.WcReadTableNo);
        }
        #endregion

        private void OnEventTxtChanged(int Bw, string strTxm)
        {
            //GlobalUnit.g_Meter.MData[Bw].chrTxm = strTxm;
            
            if (strTxm.Length >= Convert.ToInt16(pwFunction.pwConst.GlobalUnit.WcTableNoLenth))
            {
                #region 默认全选轮流

                //if (Bw == GlobalUnit.g_BW - 1)
                //{
                //    Bw = -1;
                //}
                //DisMeter[Bw+1].GetTxtFocus();

                #endregion
                bool IsShaomiao = false;


                DisMeter[Bw].GetEnblan(pwFunction.pwConst.GlobalUnit.WcReadTableNo);
                #region 判断下一个是否选中
                
                int index = Bw + 1;
                if (index < GlobalUnit.g_BW)
                {
                    if (!pwFunction.pwConst.GlobalUnit.WcReadTableNo)
                    {
                        IsShaomiao = true;
                      
                    }
                    else
                    {
                        while (!GlobalUnit.g_Meter.MData[index].bolIsCheck)
                        {
                            index++;
                            if (index == GlobalUnit.g_BW)
                            {
                                Control.CheckForIllegalCrossThreadCalls = false;
                                threadDo = new Thread(new ThreadStart(pwExecutePlan));
                                threadDo.Start();
                                return;
                            }
                        }

                       
                        DisMeter[index].GetTxtFocus();
                    }
                    //DisMeter[index].GetEnblan(IsShaomiao);

                }

                
            }
                #endregion
        }


        #region 详细数据显示
        private void LoadDataView()
        {
            ui_Popup = new CL4100.ShowDataView.pwCheckShowDataVew();

            ui_Popup.TopLevel = false;

            panel2.Controls.Add(ui_Popup);//this

            panel2.Controls.SetChildIndex(ui_Popup, 0);//this

            ui_Popup.Visible = false;

            ui_Popup.TopMost = false;

            if (ui_Popup.Visible == true)
            {
                ui_Popup.Visible = false;
                return;
            }
        }
        private void OnEventBwChanged(int Bw)
        {
            //if (Data_Error.SelectedRows.Count < 1)
            //{
            //    MessageBox.Show("请先选中需要查询的表", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    return;
            //}
            //ui_Popup_MeterIndex = Data_Error.SelectedRows[0].Index;
            object ojb = Bw;
            lock (ui_Popup)
            {
                ui_Popup.Width = panel2.Width;
                ui_Popup.Location = new Point(0, panel2.Height);
                //ui_Popup.Dock = DockStyle.Bottom;
                ui_Popup.Text = "详细数据";
                ui_Popup.Visible = true;
                pwClassLibrary.pwProtocol.ThreadPool.QueueUserWorkItem(new WaitCallback(thTab_XiangXi_Click), ojb);
                //ThreadPool.QueueUserWorkItem(new WaitCallback(thTab_XiangXi_Click), ojb);
            }

        }

        private void thTab_XiangXi_Click(object obj)
        {
            object[] objpara = new object[] { obj };
            Point Loca = new Point(0, this.Height);
            int Y = this.Height - ui_Popup.Height;
            while (Loca.Y > Y)
            {
                Loca.Y -= 100;
                if (Loca.Y < Y)
                    Loca.Y = Y;
                this.BeginInvoke(new EventInvokSetUiPopUpLocation(InvokSetUiPopUpLocation), Loca);
                System.Threading.Thread.Sleep(100);
            }
            System.Threading.Thread.Sleep(150);
            //this.BeginInvoke(new MethodInvoker(InvokTab_XiangXi_Click));
            this.BeginInvoke(new EventInvokTab_XiangXi_Click(InvokTab_XiangXi_Click), objpara);
        }

        private delegate void EventInvokTab_XiangXi_Click(object obj);
        private void InvokTab_XiangXi_Click(object obj)
        {
            int Bw = Convert.ToInt32(obj);
            panel2_Resize(this, new EventArgs());
            ui_Popup.SetData(GlobalUnit.g_Meter.MData[Bw]);
        }

        delegate void EventInvokSetUiPopUpLocation(Point Loca);

        private void InvokSetUiPopUpLocation(Point Loca)
        {
            ui_Popup.Location = Loca;
        }

        private void panel2_Resize(object sender, EventArgs e)
        {
            if (ui_Popup == null) return;
            if (!ui_Popup.Visible) return;

            ui_Popup.Width = panel2.Width;
            ui_Popup.Location = new Point(0, panel2.Height - ui_Popup.Height);
        }

        #endregion

        #region 菜单栏
        #region 系统





        private void SystemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //#if !DEBUG 
            if (!bolIsSystemConfigUser()) return;
            //#endif
            pwFunction.pwSystemModel.UI_SystemManager systemConfig = new pwFunction.pwSystemModel.UI_SystemManager(pwFunction.pwConst.GlobalUnit.g_SystemConfig);
            systemConfig.ShowDialog();
        }

        private void 台架配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!bolIsSystemConfigUser()) return;
            Frontier.MeterVerification.Communication.SetEquipParamDlg pwfrmAdapterParameter = new Frontier.MeterVerification.Communication.SetEquipParamDlg();
            pwfrmAdapterParameter.ShowDialog();

        }

        private void 端口配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!bolIsSystemConfigUser()) return;
            ClAmMeterController.frmParameter pwfrmParameter = new ClAmMeterController.frmParameter(pwFunction.pwConst.GlobalUnit.g_BW);
            pwfrmParameter.ShowDialog();
        }

        private void 载入方案ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                MOShemaForm f = new MOShemaForm(WcfServerUrl);
                f.Activate();
                if (f.ShowDialog() == DialogResult.OK)
                {
                    #region
                    //splitContainer3.Panel1.Controls.Clear();
                    //InitProgram();
                    //// pw_Main();
                    //#region Main_Load

                    ////Load
                    //pwClassLibrary.TopWaiting.ShowWaiting("正在启动主界面......");

                    ////标准表读取事件
                    //Adapter.g_ComAdpater.OnReadStaInfo += new OnEventReadStdInfo(updateStdInfo);

                    ////工单产品显示
                    //pwClassLibrary.TopWaiting.ShowWaiting("正在加载工单及产品......");
                    //this.BeginInvoke(new MethodInvoker(DisplayWorkAndProducts));

                    ////方案显示
                    //pwClassLibrary.TopWaiting.ShowWaiting("正在加载方案......");
                    //this.BeginInvoke(new MethodInvoker(InitPLan));

                    ////初始化结果界面（灯泡显示界面）
                    //pwClassLibrary.TopWaiting.ShowWaiting("正在加载灯泡显示......");
                    //this.BeginInvoke(new MethodInvoker(thLoadUserControl));

                    ////初始详细数据显示界面
                    //pwClassLibrary.TopWaiting.ShowWaiting("正在加载数据显示界面......");
                    //this.BeginInvoke(new MethodInvoker(LoadDataView));

                    ////检定项目改变事件
                    //Adapter.OnEventItemChange += new DelegateEventItemChange(DisplayRow);

                    ////消息事件
                    //m_listView = listView1;
                    //m_listView.Items.Clear();
                    //m_listView.Columns[2].Width = m_listView.Parent.Width - 180;
                    //MessageToolStripMenuItem.Checked = false;
                    //splitContainer3.Panel2Collapsed = true;

                    //pwFunction.pwConst.GlobalUnit.g_MsgControl.ShowMsg += new pwClassLibrary.VerifyMsgControl.OnShowMsg(myShowMsg);
                    //Thread myMsgThread = new Thread(pwFunction.pwConst.GlobalUnit.g_MsgControl.DoWork);
                    //myMsgThread.Start();
                    //pwClassLibrary.TopWaiting.ShowWaiting("系统已全部加载完成！");
                    //pwClassLibrary.TopWaiting.HideWaiting();
                    //#endregion
                    #endregion

                    #region 重新加载方案

                    #region 加载工单配置
                    pwClassLibrary.TopWaiting.ShowWaiting("正在加载当前工单检定方案......");
                    pwFunction.pwConst.GlobalUnit.g_Work = null;
                    pwFunction.pwConst.GlobalUnit.g_Work = new pwFunction.pwWork.cWork();
                    pwFunction.pwConst.GlobalUnit.g_Work.Load();   //刷新
                    #endregion

                    #region 创建当前工单检定数据本地存储目录
                    string _DelDirectoryWorkData = System.Windows.Forms.Application.StartupPath + pwFunction.pwConst.Variable.CONST_METERDATA + pwFunction.pwConst.GlobalUnit.g_Work.WorkSN;
                    if (!System.IO.Directory.Exists(_DelDirectoryWorkData))
                    {
                        System.IO.Directory.CreateDirectory(_DelDirectoryWorkData);
                    }
                    #endregion

                    #region 加载产品配置
                    pwFunction.pwConst.GlobalUnit.g_Products = null;
                    pwFunction.pwConst.GlobalUnit.g_Products = new pwFunction.pwProducts.cProducts();
                    pwFunction.pwConst.GlobalUnit.g_Products.Load();   //刷新
                    #endregion

                    #region 加载方案配置
                    pwFunction.pwConst.GlobalUnit.g_Plan = null;
                    pwFunction.pwConst.GlobalUnit.g_Plan = new pwFunction.pwPlan.cPlan();
                    pwFunction.pwConst.GlobalUnit.g_Plan.Load();   //刷新
                    #endregion

                    #region 加载表配置，从工单及产品
                    pwFunction.pwConst.GlobalUnit.g_Meter = null;
                    pwFunction.pwConst.GlobalUnit.g_Meter = new pwFunction.pwMeter.MeterBasic(pwFunction.pwConst.GlobalUnit.g_BW);
                    pwFunction.pwConst.GlobalUnit.g_Meter.InitData();   //初始化结果数据
                    #endregion


                    //工单产品显示
                    //pwClassLibrary.TopWaiting.ShowWaiting("正在加载工单及产品......");
                    this.BeginInvoke(new MethodInvoker(DisplayWorkAndProducts));

                    //方案显示
                    //pwClassLibrary.TopWaiting.ShowWaiting("正在加载方案......");
                    this.BeginInvoke(new MethodInvoker(InitPLan));

                    //初始化结果界面（灯泡显示界面）
                    //pwClassLibrary.TopWaiting.ShowWaiting("正在加载灯泡显示......");
                    //splitContainer3.Panel1.Controls.Clear();
                    //this.BeginInvoke(new MethodInvoker(thLoadUserControl));

                    #endregion
                }
            }
            catch
            {
                return;
            }
            finally
            {
                pwClassLibrary.TopWaiting.HideWaiting();
            }
        }

        private void 联机ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (bolIsConnected()) 
            Adapter.Link();
            StatusMain_TxtStatus1.Text = Adapter.isConnected ? "联机" : "脱机";
            Status_Light.Image = Adapter.isConnected ? imageList1.Images[2] : imageList1.Images[3];

        }

        private void 脱机ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bolIsConnected()) Adapter.LinkOff();
            StatusMain_TxtStatus1.Text = Adapter.isConnected ? "联机" : "脱机";
            Status_Light.Image = Adapter.isConnected ? imageList1.Images[2] : imageList1.Images[3];
        }

        private void 升源ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bolIsConnected()) Adapter.PowerOnOnlyU();
        }

        private void 关源ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bolIsConnected()) Adapter.PowerOff();
        }


        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            pwFunction.pwConst.GlobalUnit.ApplicationIsOver = true;
            pwFunction.pwConst.GlobalUnit.ForceVerifyStop = true;
            GlobalUnit.g_Status = enmStatus.停止;
            this.Close();
        }
        #endregion

        #region 执行
        private void DoIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            threadDo = new Thread(new ThreadStart(pwExecutePlan));
            threadDo.Start();
        }
        private void StopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GlobalUnit.g_MsgControl.OutMessage("检定停止，操作完毕", false);
            GlobalUnit.g_Status = enmStatus.停止;
            GlobalUnit.ForceVerifyStop = true;
            Adapter.Stop();
        }

        /// <summary>
        /// 是否已经联接到台体
        /// </summary>
        /// <returns></returns>
        private bool bolIsConnected()
        {
            if (!Adapter.isConnected)
            {
                GlobalUnit.g_MsgControl.OutMessage("联接失败，请检查硬件接线......", false);
                pwClassLibrary.TopWaiting.ShowWaiting("联接失败,请检查硬件接线");
                Adapter.DelayTime(500);
                pwClassLibrary.TopWaiting.HideWaiting();
                return false;
            }
            else
            {
                return true;
            }
        }

        private void pwExecutePlan()
        {
            bool bolIsDoEnd = false;
            sth_SpaceTicker.Reset();
            sth_SpaceTicker.Start();       //开始记时             
            int intTicker = 0;
            try
            {
                //if (!bolIsConnected()) return;

               

                #region 传递要检
                bool isAllOk = true;
                string strNo = "表位 ：";
                for (int i = 0; i < GlobalUnit.g_BW; i++)  //GlobalUnit.g_BW         //需增加界面选中功能及处理事件
                {
                    //if (DisMeter[i].)
                    //m_bln_Selected[i] = true;
                    //GlobalUnit.g_Meter.MData[i].bolIsCheck = true;
                    //if (OnEventStatusChange != null) OnEventStatusChange(i, enmStatus.空闲);
                    if (pwFunction.pwConst.GlobalUnit.WcReadTableNo)
                    {
                        if (GlobalUnit.g_Meter.MData[i].bolIsCheck)
                        {
                            if (!string.IsNullOrEmpty(DisMeter[i].GetTextValue))
                            {
                                GlobalUnit.g_Meter.MData[i].chrTxm = DisMeter[i].GetTextValue;
                            }
                            else
                            {
                                if (OnEventTxmtextSet != null) OnEventTxmtextSet(i, "请输入生产编号！");
                                isAllOk = false;
                            }
                            for (int K = 0; K < GlobalUnit.g_BW; K++)
                            {
                                if (K != i)
                                {
                                    if (DisMeter[K].GetTextValue == DisMeter[i].GetTextValue)
                                    {
                                        if (OnEventTxmtextSet != null) OnEventTxmtextSet(i, "存在重复生产编号！");
                                        if (OnEventTxmtextSet != null) OnEventTxmtextSet(K, "存在重复生产编号！");
                                        isAllOk = false;
                                    }
                                }
                            }
                        }
                    }
                }

                if (!isAllOk)
                {
                   return ;
                     
                }
                Adapter.blnSelected = m_bln_Selected;
                #endregion

                if (GlobalUnit.g_Status == enmStatus.进行中) return;

                #region 处理工具栏及状态
                GlobalUnit.g_MsgControl.OutMessage("正在处理执行方案参数，请稍候.......", false);
                GlobalUnit.ForceVerifyStop = false;
                GlobalUnit.g_Status = enmStatus.进行中;
                menuStrip.Enabled = false;
                LinkToolStripButton.Enabled = false;
                DoToolStripButton.Enabled = false;
                SelectAlltoolStripButton.Enabled = false;
                StopToolStripButton.Enabled = true;
                this.collapseDataGridView1.EndEdit();
                StatusMain_Proc.Value = 0;
                StatusMain_Proc.Maximum = GetProgressBarMaximum();
                m_listView.Items.Clear();
                #endregion


                #region 点亮灯炮
                for (int i = 0; i < GlobalUnit.g_BW; i++)
                {
                    if (OnEventStatusChange != null) OnEventStatusChange(i, enmStatus.进行中);
                }
                Adapter.DelayTime(100);
                #endregion

                #region 清除数据
                GlobalUnit.g_Meter.InitData();
                StatusMain_Proc.Value++;
                #endregion


                #region 检定

                Adapter.Verify();
                #endregion


                #region 显示灯炮检定结果
                bolIsDoEnd = true;
                GlobalUnit.g_Status = enmStatus.结束;
                StatusMain_Proc.Value = StatusMain_Proc.Maximum;
                for (int i = 0; i < GlobalUnit.g_BW; i++)//需增加：方案需检定项目但未检，结果处理
                {
                    if (!GlobalUnit.g_Meter.bAllPrjTestOK(GlobalUnit.g_Meter.MData[i].MeterResults, 0))
                    {
                        GlobalUnit.g_Meter.MData[i].bolResult = false;
                        GlobalUnit.g_Meter.MData[i].chrResult = Variable.CTG_BuHeGe;
                    }
                    bool _bResult = GlobalUnit.g_Meter.MData[i].bolAlreadyTest && GlobalUnit.g_Meter.MData[i].bolResult && GlobalUnit.g_Meter.MData[i].bolIsCheck;
                    if (pw_Main.OnEventResuleChange != null) pw_Main.OnEventResuleChange(i, _bResult);
                }

                #endregion

                #region 保存检定结果,显示检定结果界面

                //base.BeginInvoke(new MethodInvoker(this.Save));//保存需要15-25秒时间，如这段时间关机，有可能丢失数据

                Save();

                GlobalUnit.g_Meter.Save();

                this.BeginInvoke(new MethodInvoker(DisplayJLFrm));//显示检定结果界面

                if (StatusMain_Proc.Value < StatusMain_Proc.Maximum) StatusMain_Proc.Value++;
                #endregion


                intTicker = Convert.ToInt32(sth_SpaceTicker.ElapsedMilliseconds / 1000);
                GlobalUnit.g_MsgControl.OutMessage("检定方案执行完毕，共耗时：" + intTicker.ToString() + "秒，请察看检定报告", false);

            }
            catch (System.Exception err)
            {
                GlobalUnit.g_Status = enmStatus.错误;
                GlobalUnit.g_MsgControl.OutMessage(err.Message, false);
                object sender = new object();
                EventArgs e = new EventArgs();
                SelectAlltoolStripButton_Click(sender, e);
                bolIsDoEnd = false;
                return;
            }
            finally
            {

                #region 检定结束处理，显示检定结果
                menuStrip.Enabled = true;
                LinkToolStripButton.Enabled = true;
                DoToolStripButton.Enabled = true;
                SelectAlltoolStripButton.Enabled = true;
                StopToolStripButton.Enabled = false;
                if (Adapter.isConnected) Adapter.PowerOff();
                pwClassLibrary.TopWaiting.HideWaiting();
                GlobalUnit.g_Status = enmStatus.空闲;

                #endregion
            }
        }
        private void Save()
        {
            GlobalUnit.g_Meter.SaveToDB(this.WcfServerUrl, this.OrBitUserName, this.toolStripTextBox4.Text.Trim());
        }


        private void DisplayJLFrm()
        {
            pw_Report frm_Report = new pw_Report();
            frm_Report.ShowDialog();
        }

        private void DisplayRow(string PrjName)
        {
            DataGridViewRowCollection Disprow = this.collapseDataGridView1.Rows;
            for (int i = 0; i < Disprow.Count; i++)
            {
                if (PrjName == Disprow[i].Cells[2].Value.ToString())
                {
                    Disprow[i].Selected = true;
                    break;
                }
            }
            if (StatusMain_Proc.Value < StatusMain_Proc.Maximum) StatusMain_Proc.Value++;
            GlobalUnit.g_MsgControl.OutMessage("正在执行项目：" + PrjName + "......请稍候", false);

        }

        private int GetProgressBarMaximum()
        {
            int Sum = this.collapseDataGridView1.Rows.Count;
            return Sum + 10;//
        }
        #endregion

        #region 辅助调试


        private void JdDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(DisplayFile));
        }
        private void JdReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(DisplayJLFrm));

        }
        private void PowerViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //ClAmMeterController.Test.frmMain pwTestfrmMain = new ClAmMeterController.Test.frmMain();
            //pwTestfrmMain.ShowDialog();
        }

        #endregion

        #region 视图
        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            splitContainer1.IsSplitterFixed = !toolBarToolStripMenuItem.Checked;
            toolStrip.Visible = toolBarToolStripMenuItem.Checked;
            toolStrip1.Visible = toolBarToolStripMenuItem.Checked;
            splitContainer1.IsSplitterFixed = toolBarToolStripMenuItem.Checked;
            splitContainer1.Panel1Collapsed = !toolBarToolStripMenuItem.Checked;
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StatusMain.Visible = statusBarToolStripMenuItem.Checked;
        }

        private void MessageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            splitContainer3.Panel2Collapsed = MessageToolStripMenuItem.Checked;
            MessageToolStripMenuItem.Checked = !MessageToolStripMenuItem.Checked;

        }

        #endregion

        #region 全选
        private void SelectAlltoolStripButton_Click(object sender, EventArgs e)
        {

            MOShemaForm f = new MOShemaForm(WcfServerUrl);
            f.Activate();
            if (f.ShowDialog() == DialogResult.OK)
            {
                #region 重新加载方案

                #region 加载工单配置
                pwClassLibrary.TopWaiting.ShowWaiting("正在加载当前工单检定方案......");
                pwFunction.pwConst.GlobalUnit.g_Work = null;
                pwFunction.pwConst.GlobalUnit.g_Work = new pwFunction.pwWork.cWork();
                pwFunction.pwConst.GlobalUnit.g_Work.Load();   //刷新
                #endregion

                #region 创建当前工单检定数据本地存储目录
                string _DelDirectoryWorkData = System.Windows.Forms.Application.StartupPath + pwFunction.pwConst.Variable.CONST_METERDATA + pwFunction.pwConst.GlobalUnit.g_Work.WorkSN;
                if (!System.IO.Directory.Exists(_DelDirectoryWorkData))
                {
                    System.IO.Directory.CreateDirectory(_DelDirectoryWorkData);
                }
                #endregion

                #region 加载产品配置
            pwFunction.pwConst.GlobalUnit.g_Products = null;
            pwFunction.pwConst.GlobalUnit.g_Products = new pwFunction.pwProducts.cProducts();
            pwFunction.pwConst.GlobalUnit.g_Products.Load();   //刷新
                #endregion

            #region 加载方案配置
            pwFunction.pwConst.GlobalUnit.g_Plan = null;
            pwFunction.pwConst.GlobalUnit.g_Plan = new pwFunction.pwPlan.cPlan();
            pwFunction.pwConst.GlobalUnit.g_Plan.Load();   //刷新
            #endregion

            #region 加载表配置，从工单及产品
            pwFunction.pwConst.GlobalUnit.g_Meter = null;
            pwFunction.pwConst.GlobalUnit.g_Meter = new pwFunction.pwMeter.MeterBasic(pwFunction.pwConst.GlobalUnit.g_BW);
            pwFunction.pwConst.GlobalUnit.g_Meter.InitData();   //初始化结果数据
            #endregion


            //工单产品显示                   
            this.BeginInvoke(new MethodInvoker(DisplayWorkAndProducts));
            //方案显示                    
            this.BeginInvoke(new MethodInvoker(InitPLan));
                #endregion
            }

            pwFunction.pwSystemModel.UI_SystemManager systemConfig = new pwFunction.pwSystemModel.UI_SystemManager(pwFunction.pwConst.GlobalUnit.g_SystemConfig);
            systemConfig.ShowDialog();
            Frontier.MeterVerification.Communication.SetEquipParamDlg pwfrmAdapterParameter = new Frontier.MeterVerification.Communication.SetEquipParamDlg();
            pwfrmAdapterParameter.ShowDialog();
            ClAmMeterController.frmParameter pwfrmParameter = new ClAmMeterController.frmParameter(pwFunction.pwConst.GlobalUnit.g_BW);
            pwfrmParameter.ShowDialog();          


            bool[] m_bln_Selected = new bool[GlobalUnit.g_BW];  //选中操作                      
            for (int i = 0; i < GlobalUnit.g_BW; i++)           //需增加界面选中功能及处理事件
            {
                m_bln_Selected[i] = true;
                GlobalUnit.g_Meter.MData[i].bolIsCheck = true;
                if (OnEventStatusChange != null) OnEventStatusChange(i, enmStatus.空闲);
            }
            Adapter.blnSelected = m_bln_Selected;
        }
        #endregion

        #region 关于
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pw_AboutBox frm_about = new pw_AboutBox();
            frm_about.ShowDialog();
        }
        #endregion

        #endregion

        #region 工具栏
        private void DoToolStripButton_Click(object sender, EventArgs e)
        {
            DoIconsToolStripMenuItem_Click(sender, e);
        }

        private void StopToolStripButton_Click(object sender, EventArgs e)
        {
            StopToolStripMenuItem_Click(sender, e);
        }

        #region 数据报表
        private void DataToolStripButton_Click(object sender, EventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(DisplayFile));
        }

        private void DisplayFile()
        {
            //声明一个程序信息类 
            System.Diagnostics.ProcessStartInfo Info = new System.Diagnostics.ProcessStartInfo();

            string _XmlFilePath = Application.StartupPath + pwFunction.pwConst.Variable.CONST_METERDATAXML;

            //设置外部程序名 
            Info.FileName = "iexplore.exe";


            //设置外部程序的启动参数（命令行参数）为test.txt 
            Info.Arguments = _XmlFilePath;


            ////设置外部程序工作目录为   C:\ 
            //Info.WorkingDirectory = "C:\\ ";


            //声明一个程序类 
            System.Diagnostics.Process Proc;


            try
            {
                // 
                //启动外部程序 
                // 
                Proc = System.Diagnostics.Process.Start(Info);
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                Console.WriteLine("系统找不到指定的程序文件。\r{0} ", e);
                return;
            }



        }
        #endregion

        private void ExitoolStripButton_Click(object sender, EventArgs e)
        {
            ExitToolsStripMenuItem_Click(sender, e);
        }


        #endregion



        private void toolStripLabel7_Click(object sender, EventArgs e)
        {
            //if (!bolIsSystemConfigUser()) return;
            pwFunction.pwSystemModel.UI_SystemManager systemConfig = new pwFunction.pwSystemModel.UI_SystemManager(pwFunction.pwConst.GlobalUnit.g_SystemConfig);
            systemConfig.ShowDialog();
        }

        

    }
}
