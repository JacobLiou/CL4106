using System;
using System.Collections.Generic;
using System.Text;

namespace VerificationEquipment.Commons
{
    /// <summary>
    /// 采样数据回调句柄
    /// </summary>
    /// <param name="sampleIndex">采样次数，从1开始编号</param>
    /// <param name="datas">采样数据，数组长度同设备表位数量。对空表位或未出采样值的表位，返回null</param>
    public delegate void ReturnSampleDatasDelegate(int sampleIndex ,string[] datas);
    /// <summary>
    /// 采样数据回调委托
    /// </summary>
    /// <param name="datas">二维数据1表位号、2结论</param>
    /// <param name="reSult"></param>
    public delegate void ReturnSampleDataErrDelegate(string[,]datas,int [] reSult);
}
