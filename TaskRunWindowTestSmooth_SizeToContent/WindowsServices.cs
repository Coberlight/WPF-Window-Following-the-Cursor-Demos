using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;

namespace GiveFeedbackTest
{
    // СПАСИБО, Oleg Kolosov   
    // https://questu.ru/questions/2842667/
    public static class WindowsServices
    {
        const int WS_EX_TRANSPARENT = 0x00000020;
        //const int GWL_EXSTYLE = (-16);
        const int GWL_EXSTYLE = (-20);

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        public static void SetWindowExTransparent(IntPtr hwnd)
        {
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
        }
        public static void RemoveWindowExTransparent(IntPtr hwnd)
        {
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle & ~WS_EX_TRANSPARENT);
        }
        public static void MakePopupNonInteractive(IntPtr hwnd)
        {
            // Получаем текущие стили окна
            int currentStyle = GetWindowLong(hwnd, -16);

            // Добавляем стиль WS_DISABLED, который делает окно неинтерактивным
            int newStyle = currentStyle | 0x8000000;

            // Применяем новый стиль
            SetWindowLong(hwnd, -16, newStyle);
        }
        /// <summary>
        /// метод для установки положения окна вне зависимости от масштаба
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="hWndInsertAfter"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="uFlags"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        public const int SWP_NOSIZE = 0x0001;
        public const int SWP_NOZORDER = 0x0004;

        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        public static readonly IntPtr HWND_TOP = new IntPtr(0);
        public static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        public static void SetWindowPosition(IntPtr handle, int x, int y)
        {
            SetWindowPos(handle, HWND_NOTOPMOST, x, y, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
        }
    }
}
