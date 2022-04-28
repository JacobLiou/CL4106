using System;
using System.Collections.Generic;
using System.Text;

namespace Frontier.MeterVerification.KLDevice
{
    /// <summary>
    /// Ҫ���в���Ĭ���޲ι���ĵ�������
    /// </summary>
    /// <typeparam name="T">����������</typeparam>
    public class SingletonBase<T>
        where T : new()
    {

        private static  T instance;
        private static object syncRoot = new Object();
        /// <summary>
        /// ��ȡ����ĵ���ʵ��
        /// </summary>
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new T();
                    }
                }
                return instance;
            }
        }


    }
}
