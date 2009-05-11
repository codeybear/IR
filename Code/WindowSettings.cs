using System;
using System.Drawing;
using System.Windows.Forms;

namespace ImageSearch
{
    /// <summary> Save and restore Window settings for this user </summary>
    public static class WindowSettings
    {
        /// <summary> Record this user's form settings </summary>
        public static void Record(Form form, SplitContainer SplitContainer)
        {
            bool shouldRecordSplitters;
            switch (form.WindowState)
            {
                case FormWindowState.Maximized:
                    RecordWindowPosition(form.RestoreBounds);
                    shouldRecordSplitters = true;
                    break;
                case FormWindowState.Normal:
                    shouldRecordSplitters =
                        RecordWindowPosition(form.Bounds);
                    break;
                default:
                    // Don't record anything when closing while minimized.
                    return;
            }

            Properties.Settings.Default.WindowSettingsState = form.WindowState;

            if (shouldRecordSplitters)
            {
                Properties.Settings.Default.WindowSettingsSplitterDistance = SplitContainer.SplitterDistance;
            }

            Properties.Settings.Default.Save();
        }

        /// <summary> Restore this user's form settings </summary>
        public static void Restore(Form form, SplitContainer SplitContainer)
        {
            Point Location = Properties.Settings.Default.WindowSettingsLocation;
            Size Size = Properties.Settings.Default.WindowSettingsSize;

            // If user settings are missing height and width will be zero
            // should revert to default form size
            bool SizeIsNormal = Size.Width != 0 && Size.Height != 0;

            if (IsOnScreen(Location, Size) && SizeIsNormal)
            {
                form.Location = Location;
                form.Size = Size;
                form.WindowState = Properties.Settings.Default.WindowSettingsState;
                SplitContainer.SplitterDistance = Properties.Settings.Default.WindowSettingsSplitterDistance;
            }
            else
            {
                form.WindowState = Properties.Settings.Default.WindowSettingsState;
            }
        }

        private static bool RecordWindowPosition(Rectangle bounds)
        {
            bool isOnScreen = IsOnScreen(bounds.Location, bounds.Size);
            if (isOnScreen)
            {
                
                Properties.Settings.Default.WindowSettingsLocation = bounds.Location;
                Properties.Settings.Default.WindowSettingsSize = bounds.Size;
            }
            return isOnScreen;
        }

        private static bool IsOnScreen(Point location, Size size)
        {
            return IsOnScreen(location) && IsOnScreen(location + size);
        }

        private static bool IsOnScreen(Point location)
        {
            foreach (var screen in Screen.AllScreens)
            {
                if (screen.WorkingArea.Contains(location))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
