using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using pwFunction.pwSystemModel;
using pwFunction;
namespace pwFunction.pwSystemModel
{
    public partial class UI_SystemManager : Form
    {
        private SystemInfo _SystemCol;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Item">系统信息对象</param>
        public UI_SystemManager(SystemInfo Item)
        {
            InitializeComponent();
            this.SystemCol = Item;
        }

        /// <summary>
        /// 系统信息模型赋值
        /// </summary>
        private SystemInfo SystemCol
        {
            get
            {
                return _SystemCol;
            }
            set
            {
                _SystemCol = value;
                this.DefaultSystemGrid(_SystemCol.SystemMode);      //初始化系统信息列表
            }
        }

        /// <summary>
        /// 确认保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cmd_Ok_Click(object sender, EventArgs e)
        {
            this.SaveSystemGridInfo();
            _SystemCol.Save();
            this.Close();
        }

        /// <summary>
        /// 直接退出，不保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cmd_Close_Click(object sender, EventArgs e)
        {
            this.Close();
            _SystemCol.Load();
        }


        /// <summary>
        /// 仅显示系统配置Tab
        /// </summary>
        /// <param name="OnlySetInfo"></param>
        public void Show(bool OnlySetInfo)
        {
            if (!OnlySetInfo)
            {
                this.Show();
                return;
            }
            for (int i = Tab_Control.TabPages.Count - 1; i > 0; i--)            //将其他部分移除掉
            {
                Tab_Control.TabPages.RemoveAt(i);
            }
            this.Show();
        }

        public void Show(bool OnlySetInfo, bool ShowModel)
        {
            if (!OnlySetInfo)
            {
                if (ShowModel)
                {
                    this.ShowDialog();
                }
                else
                {
                    this.Show();
                }
                return;
            }
            for (int i = Tab_Control.TabPages.Count - 1; i > 0; i--)            //将其他部分移除掉
            {
                Tab_Control.TabPages.RemoveAt(i);
            }

            if (ShowModel)
            {
                this.ShowDialog();
            }
            else
            {
                this.Show();
            }

        }

        #region 系统信息配置
        /// <summary>
        /// 初始化系统信息列表
        /// </summary>
        /// <param name="Item">系统配置信息对象</param>
        private void DefaultSystemGrid(SystemConfigure Item)
        {
            List<string> _Keys = Item.getKeyNames();
            SystemProperty.Item.Clear();
            SystemProperty.ShowCustomProperties = true;
            for (int i = 0; i < _Keys.Count; i++)
            {
                pwInterface.StSystemInfo _Item = Item.getItem(_Keys[i]);
                SystemProperty.Item.Add(_Item.Name, _Item.Value, false, _Item.ClassName, _Item.Description, true);
                SystemProperty.Item[SystemProperty.Item.Count - 1].Tag = _Keys[i];
                string[] _Arr = _Item.DataSource.Split('|');
                if (_Arr.Length > 1)
                    SystemProperty.Item[SystemProperty.Item.Count - 1].Choices = new PropertyGridEx.CustomChoices(_Arr);

            }
           SystemProperty.Refresh();
        }
        /// <summary>
        /// 转化系统配置信息
        /// </summary>
        private void SaveSystemGridInfo()
        {
            _SystemCol.SystemMode.Clear();
            for (int i = 0; i < SystemProperty.Item.Count; i++)
            {
                pwInterface.StSystemInfo _Item = new pwInterface.StSystemInfo();
                _Item.Name = SystemProperty.Item[i].Name;
                _Item.Value = SystemProperty.Item[i].Value.ToString();
                _Item.Description = SystemProperty.Item[i].Description;
                _Item.ClassName = SystemProperty.Item[i].Category;
                if (SystemProperty.Item[i].Choices != null)
                {
                    string _TmpString = "";
                    for (int j = 0; j < SystemProperty.Item[i].Choices.Count; j++)
                    {
                        if (j == 0)
                            _TmpString = SystemProperty.Item[i].Choices[j].ToString();
                        else
                            _TmpString = string.Format("{0}|{1}", _TmpString, SystemProperty.Item[i].Choices[j].ToString());
                    }
                    _Item.DataSource = _TmpString;

                }
                else
                {
                    _Item.DataSource = "";
                }
                _SystemCol.SystemMode.Add(SystemProperty.Item[i].Tag.ToString(), _Item);

            }

        }

        #endregion

    
    }
}