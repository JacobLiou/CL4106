using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
namespace pwClassLibrary.DataBase
{
    public class CreateMdb
    {
       

        private DataControl DBControl=null;

        private OleDbConnection _Con = null;

        private string DbPath="";

        public CreateMdb(string MdbPath)
        {
            ADOX.CatalogClass Adc = new ADOX.CatalogClass();

            Adc.Create("Provider=Microsoft.Jet.OLEDB.4.0;Data Source="+MdbPath+";Jet OLEDB:Engine Type=5");
            Adc = null;

            DbPath = MdbPath;

            DBControl = new DataControl(MdbPath);
            _Con = DBControl.Con;
        }


        #region ---------------------创建误差限数据库------------------
        
        /// <summary>
        /// 创建误差限数据库
        /// </summary>
        /// <returns></returns>
        public bool CreateWcLimitMDB()
        { 
            if(_Con==null) return false;
            OleDbCommand _Cmd = new OleDbCommand();
            _Cmd.Connection = _Con;
            try
            {
                _Cmd.CommandText = "Create Table GuiCheng(ID autoincrement(1,1) primary key,Name text(20))";        //规程表

                _Cmd.ExecuteNonQuery();

                _Cmd.CommandText = "Create Table MeterLevel(ID autoincrement(1,1) primary key,Name text(50))";      //表等级表

                _Cmd.ExecuteNonQuery();

                _Cmd.CommandText = "Create Table WcLimitName(ID autoincrement(1,1) primary key,Name text(50))";     //误差限名称表

                _Cmd.ExecuteNonQuery();

                _Cmd.CommandText = "Create Table PcLimit(ID autoincrement(1,1),WcLimitNameID Long,GuiChengID Long,MeterLevelID Long,PcLimit single,primary key(WcLimitNameID,GuiChengID,MeterLevelID))";

                _Cmd.ExecuteNonQuery();         //偏差限表

                _Cmd.CommandText = "Create Table WcLimit(ID autoincrement(1,1),WcLimitNameID Long,"
                                                                             + "GuiChengID Long,"
                                                                             + "MeterLevelID Long,"
                                                                             + "YjID Long,"
                                                                             + "GlysID Long,"
                                                                             + "CurrentID Long,"
                                                                             + "Hgq yesno,"
                                                                             + "YouGong yesno,"
                                                                             + "Limit text(50),primary key(WcLimitNameID,GuiChengID,MeterLevelID,YjID,GlysID,CurrentID,Hgq,YouGong))";

                _Cmd.ExecuteNonQuery();     //误差限表


                _Cmd.CommandText = "ALTER TABLE WcLimit ADD CONSTRAINT Relation1 FOREIGN KEY(WcLimitNameID) REFERENCES WcLimitName(ID) on update cascade on delete cascade";

                _Cmd.ExecuteNonQuery();     //创建关系

                _Cmd.CommandText = "ALTER TABLE PcLimit ADD CONSTRAINT Relation2 FOREIGN KEY(WcLimitNameID) REFERENCES WcLimitName(ID) on update cascade on delete cascade";

                _Cmd.ExecuteNonQuery();     //创建关系

                _Cmd.CommandText = "ALTER TABLE WcLimit ADD CONSTRAINT Relation3 FOREIGN KEY(GuiChengID) REFERENCES GuiCheng(ID) on update cascade on delete cascade";

                _Cmd.ExecuteNonQuery();     //创建关系

                _Cmd.CommandText = "ALTER TABLE PcLimit ADD CONSTRAINT Relation4 FOREIGN KEY(GuiChengID) REFERENCES GuiCheng(ID) on update cascade on delete cascade";

                _Cmd.ExecuteNonQuery();     //创建关系

                _Cmd.CommandText = "ALTER TABLE WcLimit ADD CONSTRAINT Relation5 FOREIGN KEY(MeterLevelID) REFERENCES MeterLevel(ID) on update cascade on delete cascade";

                _Cmd.ExecuteNonQuery();     //创建关系

                _Cmd.CommandText = "ALTER TABLE PcLimit ADD CONSTRAINT Relation6 FOREIGN KEY(MeterLevelID) REFERENCES MeterLevel(ID) on update cascade on delete cascade";

                _Cmd.ExecuteNonQuery();     //创建关系

            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(string.Format("错误描述：{0}",e.ToString()));

                _Cmd = null;

                _Con.Close();
                _Con.Dispose();
                DBControl = null;

                System.IO.File.Delete(DbPath);

                return false;
            }

            _Cmd.CommandText = "INSERT INTO GuiCheng(Name)VALUES('JJG596-1999')";

            _Cmd.ExecuteNonQuery();

            _Cmd.CommandText = "INSERT INTO GuiCheng(Name)VALUES('JJG307-1988')";

            _Cmd.ExecuteNonQuery();

            _Cmd.CommandText = "INSERT INTO GuiCheng(Name)VALUES('JJG307-2006')";

            _Cmd.ExecuteNonQuery();

            _Cmd.CommandText = "INSERT INTO MeterLevel(Name)VALUES('0.2')";

            _Cmd.ExecuteNonQuery();

            _Cmd.CommandText = "INSERT INTO MeterLevel(Name)VALUES('0.5')";

            _Cmd.ExecuteNonQuery();

            _Cmd.CommandText = "INSERT INTO MeterLevel(Name)VALUES('1.0')";

            _Cmd.ExecuteNonQuery();

            _Cmd.CommandText = "INSERT INTO MeterLevel(Name)VALUES('2.0')";

            _Cmd.ExecuteNonQuery();

            _Cmd.CommandText = "INSERT INTO MeterLevel(Name)VALUES('3.0')";

            _Cmd.ExecuteNonQuery();

            _Cmd = null;

            _Con.Close();

            _Con.Dispose();

            DBControl = null;

            return true;

        }
        
        #endregion 


        #region -----------------创建信息数据库-------------

        public bool CreateDataDb()
        {
            if (_Con == null) return false;
            OleDbCommand _Cmd = new OleDbCommand();
            _Cmd.Connection = _Con;

            try
            {
                _Cmd.CommandText = "CREATE TABLE CreateIDTable(Lng_AutoID autoincrement(1,1),sTrTmpValue Text(1))";

                _Cmd.ExecuteNonQuery();

                #region ---------创建MeterBasicInfo----------------
                _Cmd.CommandText = "CREATE TABLE [MeterBasicInfo]([Mb_LngMyId] long primary key,"
                                                                + "[Mb_intBno] Integer,"
                                                                + "[Mb_chrJlbh] Text(40),"
                                                                + "[Mb_chrCcbh] Text(40),"
                                                                + "[Mb_chrTxm] Text(40),"
                                                                + "[Mb_chrAddr] Text(20),"
                                                                + "[Mb_chrZzcj] Text(40),"
                                                                + "[Mb_chrBxh] Text(20),"
                                                                + "[Mb_chrBcs] Text(20),"
                                                                + "[Mb_chrBlx] Text(16),"
                                                                + "[Mb_chrBdj] Text(10),"
                                                                + "[Mb_chrCcrq] Text(10),"
                                                                + "[Mb_chrSjdw] Text(50),"
                                                                + "[Mb_chrZsbh] Text(20),"
                                                                + "[Mb_chrBmc] Text(40),"
                                                                + "[Mb_intClfs] Integer,"
                                                                + "[Mb_chrUb] Text(15),"
                                                                + "[Mb_chrIb] Text(15),"
                                                                + "[Mb_chrHz] Text(10),"
                                                                + "[Mb_BlnZnq] yesno,"
                                                                + "[Mb_BlnHgq] yesno,"
                                                                + "[Mb_chrTestType] Text(20),"
                                                                + "[Mb_datJdrq] datetime,"
                                                                + "[Mb_datJjrq] datetime,"
                                                                + "[Mb_chrWd] Text(8),"
                                                                + "[Mb_chrSd] Text(8),"
                                                                + "[Mb_chrResult] Text(6),"
                                                                + "[Mb_chrJyy] Text(20),"
                                                                + "[Mb_chrHyy] Text(20),"
                                                                + "[Mb_chrZhuGuan] Text(20),"
                                                                + "[Mb_BlnToServer] yesno,"
                                                                + "[Mb_BlnToMis] yesno,"
                                                                + "[Mb_chrQianFeng1] Text(20),"
                                                                + "[Mb_chrQianFeng2] Text(20),"
                                                                + "[Mb_chrQianFeng3] Text(20),"
                                                                + "[Mb_chrOther1] Text(50),"
                                                                + "[Mb_chrOther2] Text(50),"
                                                                + "[Mb_chrOther3] Text(50),"
                                                                + "[Mb_chrOther4] Text(50),"
                                                                + "[Mb_chrOther5] Text(50))";

                _Cmd.ExecuteNonQuery();

                #endregion

                #region ---------创建MeterResult-------------
                _Cmd.CommandText = "CREATE TABLE [MeterResult]([Mr_LngMyID] long,"
                                                             +"[Mr_PrjID] Text(5),"
                                                             +"[Mr_PrjName] Text(50),"
                                                             +"[Mr_Result] Text(20),"
                                                             +"[Mr_Time] Text(50),"
                                                             +"[Mr_Current] Text(50),primary key(Mr_LngMyID,Mr_PrjID))";
                _Cmd.ExecuteNonQuery();
                #endregion 

                #region ---------创建MeterError---------------

                _Cmd.CommandText = "CREATE TABLE [MeterError]([Me_AutoID] autoincrement(1,1),"
                                                          + "[Me_LngMyID] Long,"
                                                          + "[Me_PrjId] Text(10),"
                                                          + "[Me_PrjName] Text(50),"
                                                          + "[Me_Result] Text(10),"
                                                          + "[Me_Glys] Text(20),"
                                                          + "[Me_xib] Text(20),"
                                                          + "[Me_xU] Text(10),"
                                                          + "[Me_WcLimit] Text(20),"
                                                          + "[Me_Qs] Integer,"
                                                          + "[Me_Pl] Text(10),"
                                                          + "[Me_Wc] memo,primary key(Me_LngMyID,Me_PrjID,Me_AutoID))";
                _Cmd.ExecuteNonQuery();

                #endregion 

                #region ---------创建MeterDgn--------------

                _Cmd.CommandText = "CREATE TABLE [MeterDgn]([Md_LngMyID] Long,[Md_PrjID] Text(10),[Md_PrjName] Text(50),[Md_ChrValue] Text(100),primary key(Md_LngMyID,Md_PrjID))";

                _Cmd.ExecuteNonQuery();

                #endregion 

                #region---------创建MeterZZData------------

                _Cmd.CommandText = "CREATE TABLE [MeterZZData]([Mz_AutoID] autoincrement(1,1),"
                                                             +"[Mz_lngMyID] Long,"
                                                             +"[Mz_PrjID] Text(10),"
                                                             +"[Mz_StartTime] Text(10),"
                                                             +"[Mz_xIb] Text(20),"
                                                             +"[Mz_Glys] Text(10),"
                                                             +"[Mz_Start] Text(50),"
                                                             +"[Mz_End] Text(50),"
                                                             +"[Mz_Diff] Text(50),"
                                                             +"[Mz_Wc] Text(50),"
                                                             +"[Mz_Result] Text(50),"
                                                             +"[Mz_U] Text(50),"
                                                             + "[Mz_i] Text(50),primary key([Mz_lngMyID],[Mz_AutoID]))";


                _Cmd.ExecuteNonQuery();
                #endregion

                #region ---------创建MeterSpecialErr--------------

                _Cmd.CommandText = "CREATE TABLE [MeterSpecialErr]([Mse_AutoID] autoincrement(1,1),"
                                                                 +"[Mse_LngMyID] Long,"
                                                                 +"[Mse_PrjName] Text(20),"
                                                                 +"[Mse_Result] Text(10),"
                                                                 +"[Mse_Glfx] Integer,"
                                                                 +"[Mse_Ub] Text(30),"
                                                                 +"[Mse_Ib] Text(30),"
                                                                 +"[Mse_Phase] Text(30),"
                                                                 +"[Mse_Pl] Text(10),"
                                                                 +"[Mse_Nxx] integer,"
                                                                 +"[Mse_XieBo] integer,"
                                                                 +"[Mse_WcLimit] Text(10),"
                                                                 +"[Mse_Qs] integer,"
                                                                 +"[Mse_Wc] memo,primary key(Mse_LngMyID,Mse_AutoID))";

                _Cmd.ExecuteNonQuery();
                #endregion

                #region 2010-03-30 Add by Gqs 
                #region ---------创建MeterErrAccord---------------

                _Cmd.CommandText = "CREATE TABLE [MeterErrAccord]([Mea_AutoID] autoincrement(1,1),"
                                                          + "[Mea_LngMyID] Long,"
                                                          + "[Mea_PrjId] Text(10),"
                                                          + "[Mea_PrjName] Text(50),"
                                                          + "[Mea_Result] Text(10),"
                                                          + "[Mea_Glys] Text(20),"
                                                          + "[Mea_xib] Text(20),"
                                                          + "[Mea_xU] Text(10),"
                                                          + "[Mea_WcLimit] Text(20),"
                                                          + "[Mea_Qs] Integer,"
                                                          + "[Mea_Pl] Text(10),"
                                                          + "[Mea_Wc1] memo,"
                                                          + "[Mea_Wc2] memo,primary key(Mea_LngMyID,Mea_PrjID,Mea_AutoID))";
                _Cmd.ExecuteNonQuery();

                #endregion 

                #region ---------创建MeterProtocol---------------
                _Cmd.CommandText = "CREATE TABLE [MeterProtocol]([Mp_LngMyID] Long,"
                                                          + "[Mp_PrjID] Text(10),"
                                                          + "[Mp_PrjName] Text(50),"
                                                          + "[Mp_ChrValue] Text(100),"
                                                          + "[Mp_Result] Text(10),primary key(Mp_LngMyID,Mp_PrjID))";
                _Cmd.ExecuteNonQuery();
                #endregion
                #endregion

                #region  add 2010-08-10 by jeson wong
                #region ---------创建MeterFreeze--------------
                _Cmd.CommandText = "CREATE TABLE [MeterFreeze]([Md_LngMyID] Long,[Md_PrjID] Text(10),[Md_PrjName] Text(50),[Md_ChrValue] Text(100),primary key(Md_LngMyID,Md_PrjID))";

                _Cmd.ExecuteNonQuery();
                #endregion 

                #region ---------创建MeterPower--------------
                _Cmd.CommandText = "CREATE TABLE [MeterPower]([Md_LngMyID] Long,[Md_PrjID] Text(10),[Md_PrjName] Text(50),[Md_ChrValue] Text(100),primary key(Md_LngMyID,Md_PrjID))";

                _Cmd.ExecuteNonQuery();
                #endregion 

                #region ---------创建MeterFK--------------
                _Cmd.CommandText = "CREATE TABLE [MeterFK]([Md_LngMyID] Long,[Md_PrjID] Text(10),[Md_PrjName] Text(50),[Md_ChrValue] Text(100),primary key(Md_LngMyID,Md_PrjID))";

                _Cmd.ExecuteNonQuery();
                #endregion 
                #endregion

                #region ---------创建MeterExtend--------------
                _Cmd.CommandText = "CREATE TABLE [MeterExtend]([Med_LngMyID] Long,[Med_KeyID] Text(10),[Med_Value] Text(50),primary key(Med_LngMyID,Med_KeyID))";

                _Cmd.ExecuteNonQuery();
                #endregion

                #region-------------创建关系-----------------
                _Cmd.CommandText = "ALTER TABLE MeterResult ADD CONSTRAINT Relation1 FOREIGN KEY(Mr_LngMyID) REFERENCES MeterBasicInfo(Mb_LngMyID) on update cascade on delete cascade";

                _Cmd.ExecuteNonQuery();     //创建关系

                _Cmd.CommandText = "ALTER TABLE MeterError ADD CONSTRAINT Relation2 FOREIGN KEY(Me_LngMyID) REFERENCES MeterBasicInfo(Mb_LngMyID) on update cascade on delete cascade";

                _Cmd.ExecuteNonQuery();     //创建关系

                _Cmd.CommandText = "ALTER TABLE MeterDgn ADD CONSTRAINT Relation3 FOREIGN KEY(Md_LngMyID) REFERENCES MeterBasicInfo(Mb_LngMyID) on update cascade on delete cascade";

                _Cmd.ExecuteNonQuery();     //创建关系

                _Cmd.CommandText = "ALTER TABLE MeterZZData ADD CONSTRAINT Relation4 FOREIGN KEY(Mz_LngMyID) REFERENCES MeterBasicInfo(Mb_LngMyID) on update cascade on delete cascade";

                _Cmd.ExecuteNonQuery();     //创建关系

                _Cmd.CommandText = "ALTER TABLE MeterSpecialErr ADD CONSTRAINT Relation5 FOREIGN KEY(Mse_LngMyID) REFERENCES MeterBasicInfo(Mb_LngMyID) on update cascade on delete cascade";

                _Cmd.ExecuteNonQuery();     //创建关系

                _Cmd.CommandText = "ALTER TABLE MeterExtend ADD CONSTRAINT Relation6 FOREIGN KEY(Med_LngMyID) REFERENCES MeterBasicInfo(Mb_LngMyID) on update cascade on delete cascade";

                _Cmd.ExecuteNonQuery();     //创建关系

                _Cmd.CommandText = "ALTER TABLE MeterErrAccord ADD CONSTRAINT Relation7 FOREIGN KEY(Mea_LngMyID) REFERENCES MeterBasicInfo(Mb_LngMyID) on update cascade on delete cascade";

                _Cmd.ExecuteNonQuery();     //创建关系

                _Cmd.CommandText = "ALTER TABLE MeterFreeze ADD CONSTRAINT Relation8 FOREIGN KEY(Md_LngMyID) REFERENCES MeterBasicInfo(Mb_LngMyID) on update cascade on delete cascade";

                _Cmd.ExecuteNonQuery();     //创建关系

                _Cmd.CommandText = "ALTER TABLE MeterPower ADD CONSTRAINT Relation9 FOREIGN KEY(Md_LngMyID) REFERENCES MeterBasicInfo(Mb_LngMyID) on update cascade on delete cascade";

                _Cmd.ExecuteNonQuery();     //创建关系

                _Cmd.CommandText = "ALTER TABLE MeterFK ADD CONSTRAINT Relation10 FOREIGN KEY(Md_LngMyID) REFERENCES MeterBasicInfo(Mb_LngMyID) on update cascade on delete cascade";

                _Cmd.ExecuteNonQuery();     //创建关系
                #endregion 

            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(string.Format("错误描述：{0}", e.ToString()));

                _Cmd = null;

                _Con.Close();
                _Con.Dispose();
                DBControl = null;

                System.IO.File.Delete(DbPath);

                return false;
            }

            _Cmd = null;

            _Con.Close();

            _Con.Dispose();

            DBControl = null;

            return true;
        }

        #endregion 

    }
}
