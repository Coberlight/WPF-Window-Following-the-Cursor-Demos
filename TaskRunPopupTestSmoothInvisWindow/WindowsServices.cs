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
    }
}
