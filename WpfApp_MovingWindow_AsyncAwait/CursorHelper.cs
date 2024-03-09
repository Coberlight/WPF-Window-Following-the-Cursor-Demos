using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing;
using System.Windows;

namespace WpfApp_MovingWindow
{
    public static class CursorHelper
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out POINT lpPoint);
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }
        public static Point GetCursorPosition()
        {
            POINT lpPoint;
            if (GetCursorPos(out lpPoint))
            {
                return new Point(lpPoint.X, lpPoint.Y);
            }
            else
            {
                throw new SystemException("Не удалось получить позицию курсора");
            }
        }
    }
}
