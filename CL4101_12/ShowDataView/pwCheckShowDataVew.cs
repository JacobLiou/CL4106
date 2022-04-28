using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using pwCollapseDataGridView;
using pwFunction.pwConst;
using pwInterface;
namespace CL4100.ShowDataView
{
    public partial class pwCheckShowDataVew : Form
    {

        public pwCheckShowDataVew()
        {  
            InitializeComponent();
        }   

        protected override void OnClosing(CancelEventArgs e)
        {
            this.Visible = false;
            e.Cancel = true;
            base.OnClosing(e);
        }

        /// <summary>
        /// �����в���
        /// </summary>
        /// <param name="_name"></param>
        /// <param name="_headertxtx"></param>
        /// <param name="_datapropertyname"></param>
        /// <param name="_width"></param>
        /// <param name="_SortMode"></param>
        /// <param name="_autosizemode"></param>
        /// <param name="_readonly"></param>
        /// <param name="_visible"></param>
        /// <returns></returns>
        private DataGridViewTextBoxColumn SetCol(string _name, string _headertxtx, string _datapropertyname, int _width, 
            DataGridViewColumnSortMode _SortMode, DataGridViewAutoSizeColumnMode _autosizemode, bool _readonly, bool _visible)
        {
            DataGridViewTextBoxColumn cols = new DataGridViewTextBoxColumn();
            cols.Name = _name;
            cols.HeaderText = _headertxtx;
            cols.DataPropertyName = _datapropertyname;
            cols.Width = _width;
            cols.SortMode = _SortMode;
            cols.AutoSizeMode = _autosizemode;
            cols.ReadOnly = _readonly;
            cols.Visible = _visible;
            return  cols;

        }

        /// <summary>
        ///  ˢ����Ŀ����
        /// </summary>
        /// <param name="colDataView"></param>
        /// <param name="MeterInfo"></param>
        private void SetDataGroupView(pwCollapseDataGridView.CollapseDataGridView colDataView, pwFunction.pwMeter.MeterData MeterInfo)
        {
            colDataView.Visible = false;
            colDataView.RowHeadersWidth = 27;
            colDataView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            //colDataView.GridColor = Color.Gainsboro;// Color.Black;//Color.Blue ; //
            colDataView.GroupEvenColor = Color.White;// Color.Gainsboro;
            colDataView.GroupOddColor = Color.White;
            colDataView.ImgCollapse = imageList1.Images[0];
            colDataView.ImgExpand = imageList1.Images[1];
            colDataView.Columns.Clear();
            DataGridViewColumnCollection Cols2 = colDataView.Columns;
            DataGridViewTextBoxColumn cols;

            cols = SetCol("Column1", "����", "intGroupNum", 0, DataGridViewColumnSortMode.NotSortable, DataGridViewAutoSizeColumnMode.None, true,false );
            Cols2.Add(cols);
            cols = SetCol("Column1", "���", "bytLen", 40, DataGridViewColumnSortMode.NotSortable, DataGridViewAutoSizeColumnMode.None, true, true);
            Cols2.Add(cols);
            cols = SetCol("Column2", "��Ŀ����", "strName", 120, DataGridViewColumnSortMode.NotSortable, DataGridViewAutoSizeColumnMode.None, true, true);
            Cols2.Add(cols);
            cols = SetCol("Column3", "��Ŀ����", "strCode", 80, DataGridViewColumnSortMode.NotSortable, DataGridViewAutoSizeColumnMode.None, true, true);
            Cols2.Add(cols);
            cols = SetCol("Column4", "��Ŀ����", "strInputData100", 100, DataGridViewColumnSortMode.NotSortable, DataGridViewAutoSizeColumnMode.Fill, true, true);
            Cols2.Add(cols);

            colDataView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            colDataView.ColumnHeadersHeight = 30;
            colDataView.Rows.Clear();
            NameCodeGroupList codeGroup = this.InitGrouptData(MeterInfo);
            colDataView.BindDataSource<NameCodeGroupList, NameCode>(codeGroup);
            colDataView.Visible = true;
            colDataView.EndEdit();
        }

        /// <summary>
        /// ��ȡ��Ŀ����
        /// </summary>
        /// <returns></returns>
        private NameCodeGroupList InitGrouptData(pwFunction.pwMeter.MeterData MeterInfo)
        {
            NameCodeGroupList Group = new NameCodeGroupList();

            NameCodeItemList[] ListNo = new NameCodeItemList[MeterInfo.MeterResults.Count];
            int intListNubmer = 0;
            byte intItemNubmer = 1;

            //if (MeterInfo.MeterResults.Count > 0)
            //{
                //�������
                foreach (string strKey in MeterInfo.MeterResults.Keys)
                {
                    DataResultBasic _Item = MeterInfo.MeterResults[strKey];
                    ListNo[intListNubmer] = new NameCodeItemList();
                    ListNo[intListNubmer].GroupNum = intListNubmer;
                    ListNo[intListNubmer].Add(new NameCode
                    {
                        intGroupNum = intListNubmer,
                        bytLen = intItemNubmer++,
                        strName = _Item.Me_PrjName,
                        strCode = _Item.Me_Result,
                        strInputData100 = _Item.Me_Value == Variable.CTG_WeiJian ? "" : _Item.Me_Value
                    });
                    Group.Add(ListNo[intListNubmer]);
                    intListNubmer++;

                }
            //}
            return Group;
        }

        /// <summary>
        /// ˢ���������
        /// </summary>
        /// <param name="colDataView"></param>
        /// <param name="MeterInfo"></param>
        private void SetDataErrorView(pwCollapseDataGridView.CollapseDataGridView colDataView, pwFunction.pwMeter.MeterData MeterInfo)
        {
            colDataView.Visible = false;
            colDataView.RowHeadersWidth = 27;
            colDataView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            //colDataView.GridColor = Color.Gainsboro;// Color.Black;//Color.Blue ; //
            colDataView.GroupEvenColor = Color.White;// Color.Gainsboro;
            colDataView.GroupOddColor = Color.White;
            colDataView.ImgCollapse = imageList1.Images[0];
            colDataView.ImgExpand = imageList1.Images[1];
            colDataView.Columns.Clear();
            DataGridViewColumnCollection Cols2 = colDataView.Columns;
            DataGridViewTextBoxColumn cols;

            cols = SetCol("Column1", "����", "intGroupNum", 0, DataGridViewColumnSortMode.NotSortable, DataGridViewAutoSizeColumnMode.None, true,false);
            Cols2.Add(cols);
            cols = SetCol("Column1", "���", "bytLen", 40, DataGridViewColumnSortMode.NotSortable, DataGridViewAutoSizeColumnMode.None, true, true);
            Cols2.Add(cols);
            cols = SetCol("Column1", "����", "strName", 250, DataGridViewColumnSortMode.NotSortable, DataGridViewAutoSizeColumnMode.None, true, true);
            Cols2.Add(cols);
            cols = SetCol("Column1", "����", "strCode", 80, DataGridViewColumnSortMode.NotSortable, DataGridViewAutoSizeColumnMode.None, true, true);
            Cols2.Add(cols);
            cols = SetCol("Column2", "����(ƽ��ֵ|����ֵ|���1|���2|���3|���4|���5|���6|���7|���8|���9|���10|ƫ��)", "strInputData100", 120, DataGridViewColumnSortMode.NotSortable, DataGridViewAutoSizeColumnMode.Fill, true, true);
            Cols2.Add(cols);

            colDataView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            colDataView.ColumnHeadersHeight = 30;
            colDataView.Rows.Clear();
            NameCodeGroupList codeGroup = this.InitErrorData(MeterInfo);
            colDataView.BindDataSource<NameCodeGroupList, NameCode>(codeGroup);
            colDataView.Visible = true;
            colDataView.EndEdit();
        }

        /// <summary>
        /// ��ȡ�������
        /// </summary>
        /// <param name="MeterInfo"></param>
        /// <returns></returns>
        private NameCodeGroupList InitErrorData(pwFunction.pwMeter.MeterData MeterInfo)
        {
            NameCodeGroupList Group = new NameCodeGroupList();
            try
            {
               

                NameCodeItemList[] ListNo = new NameCodeItemList[MeterInfo.MeterResults.Count];
                int intListNubmer = 0;
                byte intItemNubmer = 1;

                if (GlobalUnit.g_Plan.cWcjd.IsCheck == true && MeterInfo.MeterErrors.Count > 0)
                {
                    intListNubmer++;
                    ListNo[intListNubmer] = new NameCodeItemList();
                    ListNo[intListNubmer].GroupNum = intListNubmer;
                    //ListNo[intListNubmer].Add(new NameCode { intGroupNum = intListNubmer, bytLen = intItemNubmer++, strName = enmMeterPrjID.���춨.ToString(), strCode = "", strInputData100 = "" });

                    foreach (MeterErrorItem _ErrorItem in MeterInfo.MeterErrors.Values)
                    {
                        ListNo[intListNubmer].Add(new NameCode { intGroupNum = intListNubmer, bytLen = intItemNubmer++, strName = _ErrorItem.Item_PrjName, strCode = _ErrorItem.Item_Result, strInputData100 = (_ErrorItem.Item_Result == Variable.CTG_WeiJian ? "" : _ErrorItem.ToString()) });
                    }
                    Group.Add(ListNo[intListNubmer]);
                }


                if (GlobalUnit.g_Plan.cDgnSy.IsCheck == true && MeterInfo.MeterDgns.Count > 0)
                {
                    intListNubmer++;
                    ListNo[intListNubmer] = new NameCodeItemList();
                    ListNo[intListNubmer].GroupNum = intListNubmer;
                    //ListNo[intListNubmer].Add(new NameCode { intGroupNum = intListNubmer, bytLen = intItemNubmer++, strName = enmMeterPrjID.�๦�ܼ춨.ToString(), strCode = "", strInputData100 = "" });
                    foreach (MeterDgnItem _DngItem in MeterInfo.MeterDgns.Values)
                    {
                        ListNo[intListNubmer].Add(new NameCode { intGroupNum = intListNubmer, bytLen = intItemNubmer++, strName = _DngItem.Item_PrjName, strCode = _DngItem.Item_Result, strInputData100 = _DngItem.Item_Value });
                    }

                    Group.Add(ListNo[intListNubmer]);
                }


                return Group;
            }
            catch(Exception ex)
            { return Group; }
        }


        /// <summary>
        /// ���á�ˢ������
        /// </summary>
        /// <param name="MeterInfo"></param>
        /// <param name="puType" >�����/��ϸ����</param>
        /// <param name="taiType"></param>
        /// <param name="taiId"></param>
        public void SetData(pwFunction.pwMeter.MeterData MeterInfo)
        {
            //�л�����ؼ�

            this.Text = string.Format("��ϸ����:{0} �� �����������л���λ�ò鿴 �� ˫������չ�������������档",Convert.ToInt32(MeterInfo.intBno+1).ToString("D2"));

            if (MeterInfo.MeterResults.Count > 0) SetDataGroupView(this.collapseDataGridView1, MeterInfo);
            if( MeterInfo.MeterErrors.Count>0) SetDataErrorView(this.collapseDataGridView2, MeterInfo);



        }




    }
}