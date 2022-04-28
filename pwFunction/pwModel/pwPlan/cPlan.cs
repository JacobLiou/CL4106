using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.Data;
using pwClassLibrary.DataBase;

namespace pwFunction.pwPlan
{
    public class cPlan 
    {

        //RS485读生产编号 = 100,
        //误差检定 = 200,
        //日计时误差检定 = 300,
        //分相供电测试 = 400,
        //交流采样测试 = 500,
        //读电能表底度 = 600,
        //打包参数下载 = 700,
        //系统清零 = 800,

        public cPlan_ReadScbh cReadScbh;
        public cPlan_Wcjd cWcjd;
        public cPlan_DgnSy cDgnSy;
        public cPlan_SinglePhaseTest cSinglePhaseTest;
        public cPlan_ACSamplingTest cACSamplingTest;
        public cPlan_ReadEnergy cReadEnergy;
        public cPlan_DownPara cDownPara;
        public cPlan_SysClear cSysClear;
        //======
        public cPlan_Wcjd_Point cWcPoint;
        public cPlan_DownPara_Item cDownParaItem;
        //======


        public cPlan()
        {
            cReadScbh = new cPlan_ReadScbh();
            cWcjd = new cPlan_Wcjd();
            cDgnSy = new cPlan_DgnSy();
            cSinglePhaseTest=new cPlan_SinglePhaseTest();
            cACSamplingTest = new cPlan_ACSamplingTest();
            cReadEnergy = new cPlan_ReadEnergy();
            cDownPara = new cPlan_DownPara();
            cSysClear = new cPlan_SysClear();
            cWcPoint = new cPlan_Wcjd_Point();
            cDownParaItem = new cPlan_DownPara_Item();

        }

        ~cPlan()
        {
            cReadScbh = null;
            cWcjd = null;
            cDgnSy = null;
            cSinglePhaseTest = null;
            cACSamplingTest = null;
            cReadEnergy = null;
            cDownPara = null;
            cSysClear = null;
            cWcPoint = null;
            cDownParaItem = null;
        }
        public void Load()
        {
            cReadScbh.Load();
            cWcjd.Load();
            cDgnSy.Load();
            cSinglePhaseTest.Load();
            cACSamplingTest.Load();
            cReadEnergy.Load();
            cDownPara.Load();
            cSysClear.Load();
            cWcPoint.Load();
            cDownParaItem.Load();
        }
        
        public void Clear()
        {
            cReadScbh = null;
            cWcjd = null;
            cDgnSy = null;
            cSinglePhaseTest = null;
            cACSamplingTest = null;
            cReadEnergy = null;
            cDownPara = null;
            cSysClear = null;
            cWcPoint = null;
            cDownParaItem = null;
        }

    }
}
