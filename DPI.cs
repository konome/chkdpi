using System.Drawing;
using Vanara.PInvoke;
using static Vanara.PInvoke.Gdi32;
using static Vanara.PInvoke.User32;

namespace Konome.GUI
{
    public class DPI
    {
        public enum DpiAwareness
        {
            UNAWARE = -1,
            SYSTEM = -2,
            PER_MONITOR = -3,
            PER_MONITOR_V2 = -4,
            GDI_SCALED = -5
        }

        public enum DpiAwarenessContext
        {
            DPI_UNAWARE = 24592,
            DPI_SYSTEM_AWARE = 24593,
            DPI_PER_MONITOR_AWARE = 18,
            DPI_PER_MONITOR_AWARE_V2 = 34,
            DPI_UNAWARE_GDI_SCALED = 1073766416
        }

        /// <summary>
        /// Get the DPI awareness for the current thread.
        /// </summary>
        /// <returns>
        /// True if the current dpi awareness matches the awareness context comparison.
        /// </returns>
        public static bool GetDpiAwareness(out IntPtr ctx, DpiAwarenessContext compare)
        {
            // DPI Awareness context handle:
            // Unaware        = 24592
            // System         = 24593
            // Per monitor v1 = 18
            // Per monitor v2 = 34
            // GDI scaling    = 1073766416
            ctx = (IntPtr)GetThreadDpiAwarenessContext();
            return AreDpiAwarenessContextsEqual(ctx, (IntPtr)compare);
        }

        /// <inheritdoc cref="SetDpiAwareness(DpiAwareness, out IntPtr)"/>
        public static bool SetDpiAwareness(DpiAwareness value) => SetDpiAwareness(value, out _);

        /// <summary>
        /// Sets the DPI awareness for the current thread.
        /// </summary>
        /// <returns>
        /// True when DPI AWARE or GDI SCALED. If the method returns False, it means the current thread is DPI UNAWARE.
        /// </returns>
        public static bool SetDpiAwareness(DpiAwareness mode, out IntPtr context)
        {
            SetThreadDpiAwarenessContext((IntPtr)mode);

            switch (mode)
            {
                case DpiAwareness.SYSTEM:
                    return GetDpiAwareness(out context, DpiAwarenessContext.DPI_SYSTEM_AWARE);
                case DpiAwareness.PER_MONITOR:
                    return GetDpiAwareness(out context, DpiAwarenessContext.DPI_PER_MONITOR_AWARE);
                case DpiAwareness.PER_MONITOR_V2:
                    return GetDpiAwareness(out context, DpiAwarenessContext.DPI_PER_MONITOR_AWARE_V2);
                case DpiAwareness.GDI_SCALED:
                    return GetDpiAwareness(out context, DpiAwarenessContext.DPI_UNAWARE_GDI_SCALED);
            }

            if (!GetDpiAwareness(out context, DpiAwarenessContext.DPI_UNAWARE))
                throw new Exception("Cannot set dpi awareness context");

            return false;
        }

        public static Dictionary<DpiAwarenessContext, IntPtr> EnumerateDpiAwarenessContext()
        {
            Dictionary<DpiAwarenessContext, IntPtr> result = new();

            DpiAwarenessContext old_ctx = (DpiAwarenessContext)(IntPtr)GetThreadDpiAwarenessContext();

            // Unaware
            SetDpiAwareness(DpiAwareness.UNAWARE, out IntPtr context);
            AddResult(context);
            // System
            SetDpiAwareness(DpiAwareness.SYSTEM, out context);
            AddResult(context);
            // Per monitor
            SetDpiAwareness(DpiAwareness.PER_MONITOR, out context);
            AddResult(context);
            // Per monitor V2
            SetDpiAwareness(DpiAwareness.PER_MONITOR_V2, out context);
            AddResult(context);
            // GDI caled
            SetDpiAwareness(DpiAwareness.GDI_SCALED, out context);
            AddResult(context);

            void AddResult(in IntPtr context) => result.Add((DpiAwarenessContext)context, context);

            // Set dpi awareness back to old_ctx.
            switch (old_ctx)
            {
                case DpiAwarenessContext.DPI_UNAWARE:
                    SetDpiAwareness(DpiAwareness.UNAWARE);
                    break;
                case DpiAwarenessContext.DPI_SYSTEM_AWARE:
                    SetDpiAwareness(DpiAwareness.SYSTEM);
                    break;
                case DpiAwarenessContext.DPI_PER_MONITOR_AWARE:
                    SetDpiAwareness(DpiAwareness.PER_MONITOR);
                    break;
                case DpiAwarenessContext.DPI_PER_MONITOR_AWARE_V2:
                    SetDpiAwareness(DpiAwareness.PER_MONITOR_V2);
                    break;
                case DpiAwarenessContext.DPI_UNAWARE_GDI_SCALED:
                    SetDpiAwareness(DpiAwareness.GDI_SCALED);
                    break;
            }

            return result;
        }

        public static Point GetSystemDpi()
        {
            HDC screen = GetDC(IntPtr.Zero);
            int dpix = GetDeviceCaps(screen, DeviceCap.LOGPIXELSX);
            int dpiy = GetDeviceCaps(screen, DeviceCap.LOGPIXELSY);
            ReleaseDC(IntPtr.Zero, screen);

            return new Point(dpix, dpiy);
        }
    }
}
