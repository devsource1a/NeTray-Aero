using System;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace Netray_Aero
{
    public static class ResourceManager
    {
        private static Assembly _assembly = Assembly.GetExecutingAssembly();

        public static Image LoadImage(string filename)
        {
            // Try embedded Resources folder first
            try
            {
                var stream = _assembly.GetManifestResourceStream("Netray_Aero.Resources." + filename);
                if (stream != null)
                {
                    var ms = new MemoryStream();
                    stream.CopyTo(ms);
                    ms.Position = 0;
                    return Image.FromStream(ms);
                }
            }
            catch { }

            // Fallback to Resources folder on disk
            try
            {
                string path = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "Resources", filename);
                if (File.Exists(path))
                    return Image.FromFile(path);
            }
            catch { }

            return null;
        }

        public static Icon LoadIcon(string filename)
        {
            // Try embedded Resources folder first
            try
            {
                var stream = _assembly.GetManifestResourceStream("Netray_Aero.Resources." + filename);
                if (stream != null)
                {
                    var ms = new MemoryStream();
                    stream.CopyTo(ms);
                    ms.Position = 0;
                    return new Icon(ms);
                }
            }
            catch { }

            // Fallback to Resources folder on disk
            try
            {
                string path = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "Resources", filename);
                if (File.Exists(path))
                    return new Icon(path);
            }
            catch { }

            return null;
        }
    }
}