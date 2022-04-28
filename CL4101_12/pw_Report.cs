using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using pwCollapseDataGridView;
using pwFunction.pwConst;
using pwInterface;
namespace CL4100
{
    public partial class pw_Report : Form
    {
        public pw_Report()
        {
            InitializeComponent();
            this.Load += new EventHandler(DisplayJL);

        }

        private void pw_Report_Load(object sender, EventArgs e)
        {
            this.Width = 880;
            this.Height = 670;
        }
        #region 方案UI
        public void DisplayJL(object sender, EventArgs e)
        {
            this.collapseDataGridView1.Visible = false;
            this.collapseDataGridView1.RowHeadersWidth = 27;
            this.collapseDataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.collapseDataGridView1.GroupEvenColor = Color.LawnGreen;// Color.Gainsboro;//
            this.collapseDataGridView1.GroupOddColor = Color.OrangeRed;// Color.White;// 
            this.collapseDataGridView1.ImgCollapse = imageList1.Images[0];
            this.collapseDataGridView1.ImgExpand = imageList1.Images[1];
            this.collapseDataGridView1.Columns.Clear();
            DataGridViewColumnCollection Cols2 = this.collapseDataGridView1.Columns;
            DataGridViewTextBoxColumn cols;

            cols = new DataGridViewTextBoxColumn();
            cols.Name = "Column1";
            cols.HeaderText = "分组";
            cols.DataPropertyName = "intGroupNum";
            cols.Width = 0;
            cols.ReadOnly = true;

            cols.SortMode = DataGridViewColumnSortMode.NotSortable;
            cols.Visible = false ;
            Cols2.Add(cols);

            cols = new DataGridViewTextBoxColumn();
            cols.Name = "Column2";
            cols.HeaderText = "";
            cols.DataPropertyName = "strCode";
            cols.Width = 100;
            cols.SortMode = DataGridViewColumnSortMode.NotSortable;
            cols.ReadOnly = true;
            Cols2.Add(cols);

            cols = new DataGridViewTextBoxColumn();
            cols.Name = "Column3";
            cols.HeaderText = "表位号";
            cols.DataPropertyName = "strFormat";
            cols.Width = 100;
            cols.SortMode = DataGridViewColumnSortMode.NotSortable;
            cols.ReadOnly = true;
            Cols2.Add(cols);

            cols = new DataGridViewTextBoxColumn();
            cols.Name = "Column4";
            cols.HeaderText = "测试结论";
            cols.DataPropertyName = "strName";
            cols.Width = 100;
            //cols.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            cols.SortMode = DataGridViewColumnSortMode.NotSortable;
            cols.ReadOnly = true;
            Cols2.Add(cols);


            cols = new DataGridViewTextBoxColumn();
            cols.Name = "Column5";
            cols.HeaderText = "不合格原因";
            cols.DataPropertyName = "strUnit";
            cols.Width = this.collapseDataGridView1.Width - Cols2[1].Width - Cols2[2].Width - Cols2[3].Width - 30;
            cols.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            cols.SortMode = DataGridViewColumnSortMode.NotSortable;
            cols.ReadOnly = true;
            Cols2.Add(cols);


            this.collapseDataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.collapseDataGridView1.ColumnHeadersHeight = 30;
            this.collapseDataGridView1.Rows.Clear();
            NameCodeGroupList codeGroup = this.InitGrouptData();
            this.collapseDataGridView1.BindDataSource<NameCodeGroupList, NameCode>(codeGroup);
            this.collapseDataGridView1.Visible = true;
            this.collapseDataGridView1.EndEdit();


        }

        /// <summary>
        /// 获取方案数据
        /// </summary>
        /// <returns></returns>
        private NameCodeGroupList InitGrouptData()
        {
            NameCodeGroupList Group = new NameCodeGroupList();

            NameCodeItemList[] ListFalse = new NameCodeItemList[5];//不合格表位
            NameCodeItemList[] ListTrue = new NameCodeItemList[5];//合格表位
            //NameCodeItemList[] ListNull = new NameCodeItemList[5];//未检表位

            int intHGCount = 0;
            int intBHGCount = 0;
            for (int i = 0; i < GlobalUnit.g_BW; i++)
            {
                if (GlobalUnit.g_Meter.MData[i].bolIsCheck && GlobalUnit.g_Meter.MData[i].bolAlreadyTest && GlobalUnit.g_Meter.MData[i].chrResult == Variable.CTG_HeGe)
                {
                    intHGCount++;
                }
                else
                {
                    intBHGCount++;
                }
            }

            int intListNubmer = 0;
            byte intItemNubmer = 1;

            intListNubmer = 0;
            ListFalse[intListNubmer] = new NameCodeItemList();
            ListFalse[intListNubmer].GroupNum = intListNubmer;
            ListFalse[intListNubmer].Add(new NameCode { intGroupNum = intListNubmer, strCode = "不合格表", strFormat = "总共：" + intBHGCount.ToString() + "块", strName = "", strUnit = "" });
            for (int i = 0; i < GlobalUnit.g_BW; i++)
            {
                if (GlobalUnit.g_Meter.MData[i].bolIsCheck && GlobalUnit.g_Meter.MData[i].bolAlreadyTest && GlobalUnit.g_Meter.MData[i].chrResult == Variable.CTG_HeGe)
                {
                }
                else 
                {
                    ListFalse[intListNubmer].Add(new NameCode { intGroupNum = intListNubmer, strCode = "", strFormat = Convert.ToByte(i + 1).ToString() + "表位", strName = GlobalUnit.g_Meter.MData[i].chrResult, strUnit = GlobalUnit.g_Meter.MData[i].chrRexplain });
                }
            }
            Group.Add(ListFalse[intListNubmer]);
            

            intListNubmer = 1;
            ListTrue[intListNubmer] = new NameCodeItemList();
            ListTrue[intListNubmer].GroupNum = intListNubmer;
            ListTrue[intListNubmer].Add(new NameCode { intGroupNum = intListNubmer, strCode = "合格表", strFormat = "总共：" + intHGCount.ToString()+ "块", strName = "", strUnit = "" });
            for (int i = 0; i < GlobalUnit.g_BW; i++)
            {
                if (GlobalUnit.g_Meter.MData[i].bolIsCheck && GlobalUnit.g_Meter.MData[i].bolAlreadyTest && GlobalUnit.g_Meter.MData[i].chrResult == Variable.CTG_HeGe)
                {
                    ListTrue[intListNubmer].Add(new NameCode { intGroupNum = intListNubmer, strCode = "", strFormat = Convert.ToByte(i + 1).ToString() + "表位", strName = GlobalUnit.g_Meter.MData[i].chrResult, strUnit = GlobalUnit.g_Meter.MData[i].chrRexplain });
                }
            }
            Group.Add(ListTrue[intListNubmer]);


            //intListNubmer = 2;
            //ListNull[intListNubmer] = new NameCodeItemList();
            //ListNull[intListNubmer].GroupNum = intListNubmer;
            //ListNull[intListNubmer].Add(new NameCode { intGroupNum = intListNubmer, strCode = "未检表位", strFormat = "", strName = "", strUnit = "" });
            //for (int i = 0; i < GlobalUnit.g_BW; i++)
            //{
            //    if (GlobalUnit.g_Meter.MData[i].bolAlreadyTest == false)
            //    {
            //        ListNull[intListNubmer].Add(new NameCode { intGroupNum = intListNubmer, strCode = "", strFormat = Convert.ToByte(i + 1).ToString() + "表位", strName = "未检", strUnit = GlobalUnit.g_Meter.MData[i].chrRexplain });
            //    }
            //}
            //Group.Add(ListNull[intListNubmer]);




            return Group;
        }
        #endregion

        private void pw_Report_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("不合格表记是否取出？\r\n\r\n请取出不合格表记后，再关闭检定报告", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (result == DialogResult.Yes)
            {

                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }

        }

    }
}
