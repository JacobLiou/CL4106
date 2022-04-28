using System;
using System.Collections.Generic;
using System.Text;

namespace VerificationEquipment.Commons
{
    public class Meter
    {
        private string _assetNo;
        private string _address;        //表地址
        private int _const;             //有功常数
        private int _rp_const;          //无功常数
        private string _level;          //有功准确度等级
        private string _rp_level;       //无功准确度等级
        private string _voltage;        //电压
        private string _current;        //电流
        private WiringMode _wiringMode; //接线方式
        private bool _connectFromIT;    //是否经互感器接入
        private Protocal _protocal;     //规约
        private string _saveTime;       //转存时间，格式ddhh
        private string _baudRate;       //波特率
        private CarrieWaveType _carrieWaveType; //载波类型

        /// <summary>
        /// 局编号
        /// </summary>
        public string AssetNo
        {
            get
            {
                return _assetNo;
            }
            set
            {
                _assetNo = value;
            }
        }

        /// <summary>
        /// 表地址
        /// </summary>
        public string Address
        {
            get
            {
                return this._address;
            }
            set
            {
                this._address = value;
            }
        }

        /// <summary>
        /// 有功常数
        /// </summary>
        public int Const
        {
            get
            {
                return this._const;
            }
            set
            {
                this._const = value;
            }
        }

        /// <summary>
        /// 无功常数
        /// </summary>
        public int Rp_Const
        {
            get { return _rp_const; }
            set { _rp_const = value; }
        }

        /// <summary>
        /// 有功准确度等级
        /// </summary>
        public string Level
        {
            get { return _level; }
            set { _level = value; }
        }

        /// <summary>
        /// 无功准确度等级
        /// </summary>
        public string Rp_Level
        {
            get { return _rp_level; }
            set { _rp_level = value; }
        }

        /// <summary>
        /// 电压
        /// </summary>
        public string Voltage
        {
            get { return _voltage; }
            set { _voltage = value; }
        }
        
        /// <summary>
        /// 电流
        /// </summary>
        public string Current
        {
            get { return _current; }
            set { _current = value; }
        }

        /// <summary>
        /// 接线方式
        /// </summary>
        public WiringMode WiringMode
        {
            get { return _wiringMode; }
            set { _wiringMode = value; }
        }

        /// <summary>
        /// 是否经互感器接入
        /// </summary>
        public bool ConnectFromIT
        {
            get { return _connectFromIT; }
            set { _connectFromIT = value; }
        }

        /// <summary>
        /// 规约
        /// </summary>
        public Protocal Protocal
        {
            get { return _protocal; }
            set { _protocal = value; }
        }

        /// <summary>
        /// 转存时间，格式ddhh
        /// </summary>
        public string SaveTime
        {
            get { return _saveTime; }
            set { _saveTime = value; }
        }

        /// <summary>
        /// 波特率
        /// </summary>
        public string BaudRate
        {
            get { return _baudRate; }
            set { _baudRate = value; }
        }

        /// <summary>
        /// 载波类型
        /// </summary>
        public CarrieWaveType CarrieWaveType
        {
            get { return _carrieWaveType; }
            set { _carrieWaveType = value; }
        }

    }
}
