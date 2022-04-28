using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace pwClassLibrary.pwWin32Api
{
    public class Win32Api
    {



        public delegate bool WNDENUMPROC(int hwnd, int lParam);



        /*
         * Message ID Constants
         */
        public const int WM_PAINT = 0x000F;
        public const int WM_SETFOCUS = 0x0007;
        public const int WM_KILLFOCUS = 0x0008;
        public const int WM_ENABLE = 0x000A;
        public const int WM_SETTEXT = 0x000C;
        public const int WM_GETTEXT = 0x000D;
        public const int WM_GETTEXTLENGTH = 0x000E;
        public const int WM_CLOSE = 0x0010;
        public const int WM_QUIT = 0x0012;

        //获取和设置图标
        public const int WM_GETICON = 0x007F;
        public const int WM_SETICON = 0x0080;

        public const int ICON_SMALL = 0;
        public const int ICON_BIG = 1;
        public const int ICON_SMALL2 = 2;	//XP系统中使用
        /*
         * Button Control Messages
         */
        public const int BM_GETCHECK = 0x00F0;
        public const int BM_SETCHECK = 0x00F1;
        public const int BM_GETSTATE = 0x00F2;
        public const int BM_SETSTATE = 0x00F3;
        public const int BM_SETSTYLE = 0x00F4;
        public const int BM_CLICK = 0x00F5;	//单击按钮
        public const int BM_GETIMAGE = 0x00F6;
        public const int BM_SETIMAGE = 0x00F7;
        public const int BST_UNCHECKED = 0x0000;
        public const int BST_CHECKED = 0x0001;
        public const int BST_INDETERMINATE = 0x0002;
        public const int BST_PUSHED = 0x0004;
        public const int BST_FOCUS = 0x0008;

        /*
         * 键盘消息
         */
        public const int WM_KEYFIRST = 0x0100;
        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;
        public const int WM_CHAR = 0x0102;
        public const int WM_DEADCHAR = 0x0103;
        public const int WM_SYSKEYDOWN = 0x0104;
        public const int WM_SYSKEYUP = 0x0105;
        public const int WM_SYSCHAR = 0x0106;
        public const int WM_SYSDEADCHAR = 0x0107;
        public const int WM_UNICHAR = 0x0109;			//XP系统
        public const int WM_KEYLAST = 0x0109;
        public const int UNICODE_NOCHAR = 0xFFFF;

        /*
         * 鼠标消息ID
         */
        public const int WM_MOUSEFIRST = 0x0200;
        public const int WM_MOUSEMOVE = 0x0200;
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_LBUTTONDBLCLK = 0x0203;
        public const int WM_RBUTTONDOWN = 0x0204;
        public const int WM_RBUTTONUP = 0x0205;
        public const int WM_RBUTTONDBLCLK = 0x0206;
        public const int WM_MBUTTONDOWN = 0x0207;
        public const int WM_MBUTTONUP = 0x0208;
        public const int WM_MBUTTONDBLCLK = 0x0209;
        public const int WM_MOUSEWHEEL = 0x020A;	//滚轮消息
        public const int WM_XBUTTONDOWN = 0x020B;
        public const int WM_XBUTTONUP = 0x020C;
        public const int WM_XBUTTONDBLCLK = 0x020D;

        /*n
         * Key State Masks for Mouse Messages
         */
        public const int MK_LBUTTON = 0x0001;
        public const int MK_RBUTTON = 0x0002;
        public const int MK_SHIFT = 0x0004;
        public const int MK_CONTROL = 0x0008;
        public const int MK_MBUTTON = 0x0010;
        public const int MK_XBUTTON1 = 0x0020;
        public const int MK_XBUTTON2 = 0x0040;
        /*
         * ShowWindow() Constants
         */
        public const int SW_HIDE = 0;
        public const int SW_SHOWNORMAL = 1;
        public const int SW_NORMAL = 1;
        public const int SW_SHOWMINIMIZED = 2;
        public const int SW_SHOWMAXIMIZED = 3;
        public const int SW_MAXIMIZE = 3;
        public const int SW_SHOWNOACTIVATE = 4;		//显示但不激活窗口，一个关键参数
        public const int SW_SHOW = 5;
        public const int SW_MINIMIZE = 6;
        public const int SW_SHOWMINNOACTIVE = 7;
        public const int SW_SHOWNA = 8;
        public const int SW_RESTORE = 9;
        public const int SW_SHOWDEFAULT = 10;
        public const int SW_FORCEMINIMIZE = 11;
        public const int SW_MAX = 11;

        /*
         * GetWindow() Constants
        */
        public const uint GW_HWNDFIRST = 0;
        public const uint GW_HWNDLAST = 1;
        public const uint GW_HWNDNEXT = 2;
        public const uint GW_HWNDPREV = 3;
        public const uint GW_OWNER = 4;
        public const uint GW_CHILD = 5;
        public const uint GW_ENABLEDPOPUP = 6;
        public const uint GW_MAX = 6;	//(低版本系统中定义为5)

        /* Binary raster ops */
        public const int R2_BLACK = 1;   /*  0       */
        public const int R2_NOTMERGEPEN = 2;   /* DPon     */
        public const int R2_MASKNOTPEN = 3;   /* DPna     */
        public const int R2_NOTCOPYPEN = 4;   /* PN       */
        public const int R2_MASKPENNOT = 5;   /* PDna     */
        public const int R2_NOT = 6;   /* Dn       */
        public const int R2_XORPEN = 7;   /* DPx      */
        public const int R2_NOTMASKPEN = 8;   /* DPan     */
        public const int R2_MASKPEN = 9;   /* DPa      */
        public const int R2_NOTXORPEN = 10;  /* DPxn     */
        public const int R2_NOP = 11;  /* D        */
        public const int R2_MERGENOTPEN = 12;  /* DPno     */
        public const int R2_COPYPEN = 13;  /* P        */
        public const int R2_MERGEPENNOT = 14;  /* PDno     */
        public const int R2_MERGEPEN = 15;  /* DPo      */
        public const int R2_WHITE = 16;  /*  1       */
        public const int R2_LAST = 16;

        /* Ternary raster operations 光栅操作码，BitBlt函数的参数 */
        public const int SRCCOPY = 0x00CC0020; /* dest = source                   */
        public const int SRCPAINT = 0x00EE0086; /* dest = source OR dest           */
        public const int SRCAND = 0x008800C6; /* dest = source AND dest          */
        public const int SRCINVERT = 0x00660046; /* dest = source XOR dest          */
        public const int SRCERASE = 0x00440328; /* dest = source AND (NOT dest )   */
        public const int NOTSRCCOPY = 0x00330008; /* dest = (NOT source)             */
        public const int NOTSRCERASE = 0x001100A6; /* dest = (NOT src) AND (NOT dest) */
        public const int MERGECOPY = 0x00C000CA; /* dest = (source AND pattern)     */
        public const int MERGEPAINT = 0x00BB0226; /* dest = (NOT source) OR dest     */
        public const int PATCOPY = 0x00F00021; /* dest = pattern                  */
        public const int PATPAINT = 0x00FB0A09; /* dest = DPSnoo                   */
        public const int PATINVERT = 0x005A0049; /* dest = pattern XOR dest         */
        public const int DSTINVERT = 0x00550009; /* dest = (NOT dest)               */
        public const int BLACKNESS = 0x00000042; /* dest = BLACK                    */
        public const int WHITENESS = 0x00FF0062; /* dest = WHITE                    */

        /* Pen Styles */
        public const int PS_SOLID = 0;
        public const int PS_DASH = 1;       /* -------  */
        public const int PS_DOT = 2;       /* .......  */
        public const int PS_DASHDOT = 3;       /* _._._._  */
        public const int PS_DASHDOTDOT = 4;       /* _.._.._  */
        public const int PS_NULL = 5;
        public const int PS_INSIDEFRAME = 6;
        public const int PS_USERSTYLE = 7;
        public const int PS_ALTERNATE = 8;
        public const int PS_STYLE_MASK = 0x0000000F;

        public const int PS_ENDCAP_ROUND = 0x00000000;
        public const int PS_ENDCAP_SQUARE = 0x00000100;
        public const int PS_ENDCAP_FLAT = 0x00000200;
        public const int PS_ENDCAP_MASK = 0x00000F00;

        public const int PS_JOIN_ROUND = 0x00000000;
        public const int PS_JOIN_BEVEL = 0x00001000;
        public const int PS_JOIN_MITER = 0x00002000;
        public const int PS_JOIN_MASK = 0x0000F000;

        public const int PS_COSMETIC = 0x00000000;
        public const int PS_GEOMETRIC = 0x00010000;
        public const int PS_TYPE_MASK = 0x000F0000;



        
		[StructLayout(LayoutKind.Sequential)]
		public struct RECT 
		{
			public RECT(int _left,int _top,int _right,int _bottom)
			{
				Left=_left;
				Top=_top;
				Right=_right;
				Bottom=_bottom;
			}
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}

		//Declare wrapper managed POINT class.
		[StructLayout(LayoutKind.Sequential)]
		public struct POINT 
		{
			public POINT(int _x,int _y)
			{
				X=_x;
				Y=_y;
			}
			public int X;
			public int Y;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct PAINTSTRUCT
		{ 
			public int  hdc; 
			public bool fErase; 
			public RECT rcPaint; 
			public bool fRestore; 
			public bool fIncUpdate; 
			public byte[] rgbReserved;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct LOGPEN
		{
			public uint lopnStyle;
			POINT lopnWidth;
			int lopnColor;
		}

		/// <summary>
		/// 判断一个点是否位于矩形内
		/// </summary>
		/// <param name="lprc"></param>
		/// <param name="pt"></param>
		/// <returns></returns>
		[DllImport("user32")]
		public static extern bool PtInRect(
			ref RECT lprc,
			POINT pt
			);

		[DllImport("user32", EntryPoint="SetParent")]
		public static extern int SetParent (
			IntPtr hwndChild,
			int hwndNewParent
			);
		[DllImport("user32", EntryPoint="FindWindowA")]
		public static extern int FindWindow(
			string lpClassName,
			string lpWindowName
			);
		[DllImport("user32", EntryPoint="FindWindowExA")]
		public static extern int FindWindowEx (
			int hwndParent,
			int hwndChildAfter,
			string lpszClass,		//窗口类
			string lpszWindow		//窗口标题
			);

		[DllImport("user32.dll", EntryPoint = "SendMessage")]
		public static extern int SendMessage(
			int hWnd, 
			int wMsg, 
			int wParam, 
			IntPtr lParam
			);
		[DllImport("user32.dll", EntryPoint = "SendMessage")]
		public static extern int SendMessage(
			int hWnd, 
			int wMsg, 
			int wParam, 
			int lParam
			);

		[DllImport("user32.dll", EntryPoint = "SendMessageA")]
		public static extern int SendMessage(
			int hWnd, 
			int wMsg, 
			int wParam, 
			string lParam
			);

		[DllImport("user32.dll", EntryPoint = "SendMessageA")]
		public static extern int SendMessage(
			int hWnd, 
			int wMsg, 
			int wParam, 
			StringBuilder lParam
			);

		[DllImport("user32.dll")]
		public static extern int GetWindowThreadProcessId(
			int hWnd,
			ref int lpdwProcessId);

		[DllImport("user32")]
		public static extern int Sleep(
			int dwMilliseconds
			);

		[DllImport("user32.dll")]
		public static extern bool GetWindowRect(
			int hWnd,
			ref RECT lpRect
			);

		[DllImport("user32")]
		public static extern int GetWindowText(
			int hWnd,
			StringBuilder lpString,
			int nMaxCount
			);


		[DllImport("user32.dll")]
		public static extern bool ShowWindow(
			int hWnd,
			int nCmdShow
			);

		[DllImport("user32", EntryPoint="SetWindowLong")]
		public static extern uint SetWindowLong (
			IntPtr hwnd,
			int nIndex,
			uint dwNewLong
			);
		[DllImport("user32", EntryPoint="GetWindowLong")]
		public static extern uint GetWindowLong (
			IntPtr hwnd,
			int nIndex
			);
		[DllImport("user32", EntryPoint="SetLayeredWindowAttributes")]
		public static extern int SetLayeredWindowAttributes (
			IntPtr hwnd,			//目标窗口句柄
			int ColorRefKey,		//透明色
			int bAlpha,				//不透明度
			int dwFlags
			);

		[DllImport("user32")]
		public static extern bool MoveWindow(
			int hWnd,
			int X,
			int Y,
			int nWidth,
			int nHeight,
			bool bRepaint
			);

		//获得窗口类名称，返回值为字符串的字符数量
		[DllImport("user32")]
		public static extern uint RealGetWindowClass(
			int hWnd,
			StringBuilder pszType,		//缓冲区
			uint cchType);				//缓冲区长度

		//枚举屏幕上所有顶级窗口（不会枚举子窗口，除了一些有WS_CHILD的顶级窗口）
		[DllImport("user32")]
		public static extern bool EnumWindows(
			WNDENUMPROC lpEnumFunc,
			int lParam
			);

		[DllImport("user32")]
		public static extern bool EnumChildWindows(
			int hWndParent,
			WNDENUMPROC lpEnumFunc,
			int lParam
			);

		[DllImport("user32")]
		public static extern bool EnumThreadWindows(		//枚举线程窗口
			int dwThreadId,
			WNDENUMPROC lpEnumFunc,
			int lParam
			);

		[DllImport("user32")]
		public static extern int GetParent(
			int hWnd
			);

		[DllImport("user32")]
		public static extern int GetWindow(
			int hWnd,	//基础窗口
			uint uCmd	//关系
			);

        /*
         * 获取鼠标的屏幕坐标，填充到Point
         * WINUSERAPI
            BOOL
            WINAPI
            GetCursorPos(
                    __out LPPOINT lpPoint);
         */
        [DllImport("user32")]
        public static extern bool GetCursorPos(
            out POINT lpPoint
            );

        [DllImport("user32")]
		public static extern int GetDC(
			int hWnd
			);

		[DllImport("user32")]
		public static extern int GetWindowDC(
			int hWnd
			);

		[DllImport("user32")]
		public static extern int ReleaseDC(
			int hWnd,
			int hDC
			);

		[DllImport("user32")]
		public static extern int FillRect(
			int hDC,
			RECT lprc,
			int hBrush
			);

		[DllImport("user32")]
		public static extern bool InvalidateRect(
			int hwnd,
			ref RECT lpRect,
			bool bErase
			);

		//判断一个窗口是否是可见的
		[DllImport("user32")]
		public static extern bool IsWindowVisible(
			int hwnd
			);

		//绘制焦点举行
		[DllImport("user32")]
		public static extern bool DrawFocusRect(
			int hDC,
			ref RECT lprc
			);

		[DllImport("user32")]
		public static extern bool UpdateWindow(
			int hwnd
			);

		[DllImport("user32")]
		public static extern bool EnableWindow(
			int hwnd,
			bool bEnable
			);

		//设置前景窗口，强制其线程成为前台，并激活窗口
		[DllImport("user32")]
		public static extern bool SetForegroundWindow(
			int hwnd
			);

		//设置前景窗口，强制其线程成为前台，并激活窗口
		[DllImport("user32")]
		public static extern bool GetForegroundWindow(
			);

		//获取拥有焦点窗口（唯一拥有键盘输入的窗口）
		[DllImport("user32")]
		public static extern int GetFocus(
			);

		//设置焦点窗口（返回值是前一个焦点窗口）
		[DllImport("user32")]
		public static extern int SetFocus(
			int hwnd
			);

		//根据点查找窗口
		[DllImport("user32")]
		public static extern int WindowFromPoint(
			POINT Point
			);

		[DllImport("user32")]
		public static extern int ChildWindowFromPoint(
			int hWndParent,
			POINT Point
			);

		[DllImport("user32")]
		public static extern bool DestroyIcon(
			int hIcon
			);





		[DllImport("Gdi32")]
		public static extern int SetROP2(
			int hDC,
			int fnDrawMode
			);

		[DllImport("Gdi32")]
		public static extern int GetROP2(
			int hDC
			);

        [DllImport("Gdi32")]
		public static extern bool ValidateRect(
			int hWnd,
			ref RECT lpRect
			);

		[DllImport("Gdi32")]
		public static extern int CreateSolidBrush(
			int crColor
			);

		[DllImport("Gdi32")]
		public static extern int CreateDC(
			string lpszDriver,
			string lpszDevice,
			string lpszOutput,
			int lpInitData		//这个参数实际是一个 DEVMODE 结构的指针
			);

		[DllImport("Gdi32")]
		public static extern int SelectObject(
			int hDC,
			int hGdiObj
			);

		[DllImport("Gdi32")]
		public static extern int DeleteObject(
			int hObject
			);

		[DllImport("Gdi32")]
		public static extern int CreatePen(
			int fnPenStyle,
			int nWidth,
			int crColor
			);

		[DllImport("Gdi32")]
		public static extern int CreatePenIndirect(
			ref LOGPEN lplogpen
			);

		[DllImport("Gdi32")]
		public static extern bool MoveToEx(
			int hDC,
			int X,
			int Y,
			ref POINT lpPoint
			);

		[DllImport("Gdi32")]
		public static extern bool LineTo(
			int hDC,
			int nXEnd,
			int nYEnd
			);

		//无伸展位图传送
		[DllImport("Gdi32")]
		public static extern bool BitBlt(
			int hdcDest,
			int nXDest,
			int nYDest,
			int nWidth,
			int nHeight,
			int hdcSrc,
			int nXsrc,
			int nYsrc,
			int dwRop		//光栅操作码
			);

		//获取特定设备的特定信息，例如屏幕象素高度，宽度
		[DllImport("Gdi32")]
		public static extern bool GetDeviceCaps(
			int hdc,
			int nIndex
			);

		//创建一个匹配的内存DC，返回其句柄
		[DllImport("Gdi32")]
		public static extern int CreateCompatibleDC(
			int hDC		//如果此参数null，则创建于屏幕匹配的内存dc
			);

		//创建一个与某dc匹配的内存位图，返回句柄
		[DllImport("Gdi32")]
		public static extern int CreateCompatibleBitmap(
			int hDC,
			int nWidth,
			int nHeight
			);





        [DllImport("user32.dll",CallingConvention=CallingConvention.Cdecl)]
        public extern static IntPtr GetWindow();

        [DllImport("user32.dll")]
        public extern static bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
        public static uint LWA_COLORKEY = 0x00000001;
        public static uint LWA_ALPHA = 0x00000002;

        #region 阴影效果变量
        //声明Win32 API
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SetClassLong(IntPtr hwnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassLong(IntPtr hwnd, int nIndex);
        //[DllImport("user32.dll")]
        //public extern static uint SetWindowLong(IntPtr hwnd, int nIndex, uint dwNewLong);
        //[DllImport("user32.dll")]
        //public extern static uint GetWindowLong(IntPtr hwnd, int nIndex);
        #endregion

        public enum WindowStyle : int
        {
            GWL_EXSTYLE = -20
        }

        public enum ExWindowStyle : uint
        {
            WS_EX_LAYERED = 0x00080000
        }

        #region 调用系统API函数更改系统时间

        [StructLayout(LayoutKind.Sequential)]
        public struct SystemTime
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMiliseconds;
        }
        [DllImport("Kernel32.dll")]
        public extern static bool SetLocalTime(ref SystemTime sysTime);

        public static bool SetSystemDateTime(string timestr)
        {
            bool flag = false;
            SystemTime sysTime = new SystemTime();
            DateTime dt = Convert.ToDateTime(timestr);
            sysTime.wYear = Convert.ToUInt16(dt.Year);
            sysTime.wMonth = Convert.ToUInt16(dt.Month);
            sysTime.wDay = Convert.ToUInt16(dt.Day);
            sysTime.wHour = Convert.ToUInt16(dt.Hour);
            sysTime.wMinute = Convert.ToUInt16(dt.Minute);
            sysTime.wSecond = Convert.ToUInt16(dt.Second);
            try
            {
                flag = SetLocalTime(ref sysTime);
            }
            catch (Exception e)
            {
                Console.WriteLine("SetSystemDateTime函数执行异常" + e.Message);
            }
            return flag;
        }

        #endregion

    }
}
