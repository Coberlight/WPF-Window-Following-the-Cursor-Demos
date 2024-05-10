using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GiveFeedbackTest
{
    public static class ScreenData
    {
        // Импортируем функции из DLL-библиотеки user32.dll
        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        // Константы для GetDeviceCaps
        private const int HORZRES = 8;
        private const int VERTRES = 10;
        private const int DESKTOPVERTRES = 117;
        private const int VREFRESH = 116;

        public static int GetFPS()
        {
            // Получаем контекст устройства для основного монитора
            IntPtr hDC = GetDC(IntPtr.Zero);
            // Получаем частоту обновления
            int refreshRate = GetDeviceCaps(hDC, VREFRESH);

            // Освобождаем контекст устройства
            ReleaseDC(IntPtr.Zero, hDC);

            // Выводим результат на консоль
            //Debug.WriteLine("Частота обновления основного монитора: " + refreshRate + " Гц");
            return refreshRate;
        }
        public static int GetWidth()
        {
            IntPtr hDC = GetDC(IntPtr.Zero);
            ReleaseDC(IntPtr.Zero, hDC);
            return GetDeviceCaps(hDC, HORZRES);
        }
        public static int GetHeight()
        {
            IntPtr hDC = GetDC(IntPtr.Zero);
            ReleaseDC(IntPtr.Zero, hDC);
            return GetDeviceCaps(hDC, VERTRES);
        }
    }
}
