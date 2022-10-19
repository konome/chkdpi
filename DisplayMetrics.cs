using System.Drawing;
using Vanara.PInvoke;
using static Vanara.PInvoke.User32;

namespace Konome.GUI
{
    public static class DisplayMetrics
    {
        public static Point PrimaryMonitor =>
           new(GetSystemMetrics(SystemMetric.SM_CXSCREEN), GetSystemMetrics(SystemMetric.SM_CYSCREEN));

        public static Point WorkingArea
        {
            get
            {
                SystemParametersInfo(SPI.SPI_GETWORKAREA, out RECT rect);
                return new(rect.right, rect.bottom);
            }

        }
    }
}