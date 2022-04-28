using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CL4100
{
    public class CParam
    {
        //型号
        private static string _modelname;
        // 物料编码
        private static string _productname;
        // 方案名称
        private static string _fanname;
        //最大电流
        private static double _imax = 60;
        //基本电流
        private static double _ibasic = 5;

        #region 设置型号
        public static string ModelName
        {
            get
            {
                return _modelname;
            }

            set
            {
                _modelname = value;
            }
        }
        #endregion

        #region 设置物料编码
        public static string ProductName
        {
            get
            {
                return _productname;
            }

            set
            {
                _productname = value;
            }
        }
        #endregion

        #region 设置方案名称
        public static string FanName
        {
            get
            {
                return _fanname;
            }

            set
            {
                _fanname = value;
            }
        }
        #endregion

        #region 设置最大电流
        public static double IMax
        {
            get
            {
                return _imax;
            }

            set
            {
                _imax = value;
            }
        }
        #endregion

        #region 设置基本电流
        public static double IBasic
        {
            get
            {
                return _ibasic;
            }

            set
            {
                _ibasic = value;
            }
        }
        #endregion




        //public static string user = ""; //定义变量 
        //public CParam()
        //{
        //    user = "abcd";//赋值构造 
        //}
        //public string User
        //{
        //    get
        //    {
        //        return user;
        //    }
        //    set
        //    {
        //        user = value;
        //    }
        //} 

        //        GlobalParams frm=new GlobalParams (); 
        //frm.User="efg"; //修改该静态变量的值 
    }
}
