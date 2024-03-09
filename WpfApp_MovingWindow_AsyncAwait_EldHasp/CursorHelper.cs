using System.Runtime.InteropServices;

public static class CursorHelper
{
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetCursorPos(out IntPoint pPoint);
    private struct IntPoint
    {
        public int X;
        public int Y;

        public IntPoint(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    public static System.Windows.Point GetScreenPosition()
    {
        GetCursorPos(out IntPoint pPoint);
        return new System.Windows.Point(pPoint.X, pPoint.Y);
    }
}