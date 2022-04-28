using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CL4100
{
    public partial class CoppyShema : Form
    {
        public CoppyShema()
        {
            InitializeComponent();
        }

        private void CoppyShema_Load(object sender, EventArgs e)
        {
            BindModel();
        }
        private void BindModel()
        {
            string sql = "exec GetModelNameForProductType @ProductFamilyName='电能表'";
            DataTable ModelNameDT = pw_Main.PUC.GetDataSetWithSQLString(sql).Tables[0];
            if (lbModel.Items.Count != 0)
            {
                lbModel.Items.Clear();
            }
            for (int i = 0; i < ModelNameDT.Rows.Count; i++)
            {
                lbModel.Items.Add(ModelNameDT.Rows[i][0].ToString().Trim());
            }
            if (lbModelNew.Items.Count != 0)
            {
                lbModelNew.Items.Clear();
            }
            for (int i = 0; i < ModelNameDT.Rows.Count; i++)
            {
                lbModelNew.Items.Add(ModelNameDT.Rows[i][0].ToString().Trim());
            }
        }

        private void lbModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string sql = "exec GetProductName @ProductModelName='" + lbModel.Text.Trim() + "'";
                DataTable ProductNameDT = pw_Main.PUC.GetDataSetWithSQLString(sql).Tables[0];
                if (lbProductName.Items.Count != 0)
                {
                    lbProductName.Items.Clear();
                }
                for (int i = 0; i < ProductNameDT.Rows.Count; i++)
                {
                    lbProductName.Items.Add(ProductNameDT.Rows[i][1].ToString().Trim());
                }
            }
            catch
            {
                //MessageBox.Show(ex.Message);
            }
        }

        private void lbModelNew_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string sql = "exec GetProductName @ProductModelName='" + lbModelNew.Text.Trim() + "'";
                DataTable ProductNameDT = pw_Main.PUC.GetDataSetWithSQLString(sql).Tables[0];
                if (lbProductNameNew.Items.Count != 0)
                {
                    lbProductNameNew.Items.Clear();
                }
                for (int i = 0; i < ProductNameDT.Rows.Count; i++)
                {
                    lbProductNameNew.Items.Add(ProductNameDT.Rows[i][1].ToString().Trim());
                }
            }
            catch
            {
                //MessageBox.Show(ex.Message);
            }
        }

        private void lbProductName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string sql = "select SchemaNameNew,SchemaDescription from CLTestSchema where SchemaType='30' and MOName='" + lbModel.Text.Trim() + "' and SchemaName='" + lbProductName.Text.Trim() + "' and SchemaArea='海外' order by SchemaConfigureDate desc";
                DataTable SchemaNameDT = pw_Main.PUC.GetDataSetWithSQLString(sql).Tables[0];//
                if (lbShemaName.Items.Count != 0)
                {
                    lbShemaName.Items.Clear();
                }
                if (SchemaNameDT.Rows.Count != 0)//是否存在方案
                {
                    for (int i = 0; i < SchemaNameDT.Rows.Count; i++)
                    {
                        lbShemaName.Items.Add(SchemaNameDT.Rows[i][0].ToString());
                    }
                }
            }
            catch
            {

            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (lbModel.SelectedIndex == -1)
            {
                MessageBox.Show("请选择源方案的表型号", "提示");
                return;
            }
            if (lbProductName.SelectedIndex == -1)
            {
                MessageBox.Show("请选择源方案的成品料号", "提示");
                return;
            }
            if (lbShemaName.SelectedIndex == -1)
            {
                MessageBox.Show("请选择源方案的方案名称", "提示");
                return;
            }
            if (lbModelNew.SelectedIndex == -1)
            {
                MessageBox.Show("请选择目的方案的表型号", "提示");
                return;
            }
            if (lbProductNameNew.SelectedIndex == -1)
            {
                MessageBox.Show("请选择目的方案的成品料号", "提示");
                return;
            }

            if (txtShemaName.Text.Trim() == "")
            {
                MessageBox.Show("请输入目的方案的方案名称", "提示");
                return;
            }
            string SQL = "select SchemaNameNew from CLTestSchema where SchemaType='30' and SchemaName='" + lbProductNameNew.Text + "' and MOName='" + lbModelNew.Text + "' and SchemaNameNew='" + txtShemaName.Text + "' and SchemaArea='海外'";
            DataTable dt = pw_Main.PUC.GetDataSetWithSQLString(SQL).Tables[0];
            if (dt.Rows.Count == 1)
            {
                MessageBox.Show("目的方案名已经存在，请重新输入", "提示");
                txtShemaName.Text = string.Empty;
                return;
            }
            SQL = @"exec CoppyCLTestSchema @SchemaNO='"
                + lbModel.Text + "-30" + "',@SchemaName='"
                + lbProductName.Text + "',@MOName='"
                + lbModel.Text + "',@SchemaType='30',@SchemaConfigureUser='"
                + pw_Main.PUC.OrBitUserName + "',@SchemaNameNew='"
                + lbShemaName.Text + "',@SchemaArea='海外',@SchemaConfigureDate='"
                + System.DateTime.Now + "',@SchemaNO1='"
                + lbModelNew.Text + "-30" + "',@SchemaName1='"
                + lbProductNameNew.Text + "',@MOName1='"
                + lbModelNew.Text + "',@SchemaNameNew1='"
                + txtShemaName.Text + "',@SchemaDescription='"
                + txtSchemaDescription.Text + "'";
            pw_Main.PUC.GetDataSetWithSQLString(SQL);
            string SQL1 = "select SchemaNameNew from CLTestSchema where SchemaType='30' and SchemaName='" + lbProductNameNew.Text + "' and MOName='" + lbModelNew.Text + "' and SchemaNameNew='" + txtShemaName.Text + "' and SchemaArea='海外'";
            dt = pw_Main.PUC.GetDataSetWithSQLString(SQL1).Tables[0];
            if (dt.Rows.Count == 1)
            {
                MessageBox.Show("复制成功,请点击确定返回主界面，选择复制的方案名称进行更改", "提示");

            }
            else
            {
                MessageBox.Show("复制失败", "提示");
            }
            this.Close();
        }
    }
}
