using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing ;

namespace pwClassLibrary
{
    /// <summary>
    /// ��Ļ�������
    /// </summary>
    public class Screen
    {

        /// <summary>
        /// ����ָ��������ڵڼ�����Ļ
        /// </summary>
        /// <param name="Pos"></param>
        /// <returns></returns>
        public static int GetScreenIndex(Point Pos)
        {
            return GetScreenIndex(Pos.X, Pos.Y);
        }

        /// <summary>
        /// ����ָ��������ڵڼ�����Ļ
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        public static int GetScreenIndex(int X,int Y)
        {
            System.Windows.Forms.Screen[] ArScreen = System.Windows.Forms.Screen.AllScreens;
            int index;
            for (index = ArScreen.Length - 1; index > 0; index--)
            {
                if (ArScreen[index].WorkingArea.Left <= X && ArScreen[index].WorkingArea.Right >= X
                    && ArScreen[index].WorkingArea.Top <= Y && ArScreen[index].WorkingArea.Bottom >= Y)
                {
                    break;
                }
            }
            return index;
        }

        /// <summary>
        /// ��ȡ�����������ڵ���Ļ
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        public static System.Windows.Forms.Screen GetScreen(int X, int Y)
        {
            return System.Windows.Forms.Screen.AllScreens[GetScreenIndex(X, Y)];
        }


    }
}
