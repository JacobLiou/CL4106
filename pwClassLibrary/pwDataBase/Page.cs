using System;
using System.Collections.Generic;
using System.Text;

namespace pwClassLibrary.DataBase
{
    /// <summary>
    /// 分页类
    /// </summary>
    public class Page
    {
        private uint _CurPage = 1;
        private uint _MaxPage = 1;
        private uint _PageSize = 1;
        private uint _MaxCount = 1;
        /// <summary>
        /// 页容量
        /// </summary>
        public uint PageSize
        {
            get { return _PageSize; }
            set
            {

                _PageSize = value;
                if (_PageSize < 1) _PageSize = 1;
            }
        }
        /// <summary>
        /// 最大数据
        /// </summary>
        public uint MaxCount
        {
            get { return _MaxCount; }
            set
            {
                _MaxCount = value;
            }
        }
        /// <summary>
        /// 当前页
        /// </summary>
        public uint CurPage
        {
            get { return _CurPage; }
            set { _CurPage = value; }
        }
        /// <summary>
        /// 总页数
        /// </summary>
        public uint MaxPage
        {
            get
            {
                double _Pages = (double)(_MaxCount / _PageSize);
                _MaxPage = (uint)Math.Ceiling(_Pages);
                return _MaxPage;
            }
        }
        /// <summary>
        /// 开始记录数
        /// </summary>
        public uint StartNumber
        {
            get
            {
                uint _startPage = (uint)((CurPage - 1) * PageSize + 1);
                return _startPage;
            }
        }
        /// <summary>
        /// 结束记录数
        /// </summary>
        public uint EndNumbwe
        {
            get
            {
                uint _EndNumber = (uint)(StartNumber + PageSize - 1);
                if (_EndNumber > MaxCount)
                {
                    _EndNumber = MaxCount;
                }
                return _EndNumber;
            }
        }
    }
}
