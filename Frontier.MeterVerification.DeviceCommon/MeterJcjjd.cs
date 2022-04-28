using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frontier.MeterVerification.DeviceCommon
{
    /// <summary>
    /// 交采集精度误差检定参数
    /// </summary>
    public class MeterJcjjd
    {

        public float fU = 0f; //电压值
        public float fI = 0f; //电流值
        public float fCa = 1.0f; //功率因数
        public bool bC = true;//感性OR容性
        public float fUuplimit = 0f;//电压误差上限
        public float fUdownlimit = 0f;//电压误差下限
        public float fIuplimit = 0f; //电流
        public float fIdownlimit = 0f; //电流下限
        public float fPuplimit = 0f;  // 有功功率上限
        public float fPdownlimit = 0f; //有功功率下限

    }
    /// <summary>
    /// 交采集精度误差检定参数 定义单个点
    /// </summary>
    public class JcjdItemPoint
    {
        public MeterJcjjd xbA = new MeterJcjjd();
        public MeterJcjjd xbB = new MeterJcjjd();
        public MeterJcjjd xbC = new MeterJcjjd();

    }
    /// <summary>
    /// 一项检定数据
    /// </summary>
    public class JcjdOneItem
    {
        public float fUStd =0f;//标准电压值
        public float fUMeter = 0f;//电表电压值
        public float fUErr = 0f; //电压误差

        public float fIStd = 0f;  //标准电流值
        public float fIMeter = 0f;//电表电流值 
        public float fIErr = 0f;  //电流误差

        public float fPStd = 0f;  //标准功率
        public float fPMeter = 0f; //电表功率
        public float fPErr = 0f;   //误差

    }
    /// <summary>
    /// 单个点检定数据
    /// </summary>
    public class JcjdoPoint
    {
        public JcjdOneItem ItemDataA = new JcjdOneItem();
        public JcjdOneItem ItemDataB = new JcjdOneItem();
        public JcjdOneItem ItemDataC = new JcjdOneItem();
    }
}
