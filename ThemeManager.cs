using System;
using System.IO;

namespace Netray_Aero
{
    public static class ThemeManager
    {
        private static string _currentTheme = "win7";
        private static string ConfigPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "config.ini");

        public static string CurrentTheme
        {
            get { return _currentTheme; }
            set { _currentTheme = value; }
        }

        public static void LoadTheme()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    foreach (string line in File.ReadAllLines(ConfigPath))
                    {
                        if (line.StartsWith("Theme="))
                        {
                            _currentTheme = line.Replace("Theme=", "").Trim();
                            break;
                        }
                    }
                }
            }
            catch { }
        }

        public static void SaveTheme()
        {
            try
            {
                File.WriteAllText(ConfigPath, "Theme=" + _currentTheme);
            }
            catch { }
        }
    }
}