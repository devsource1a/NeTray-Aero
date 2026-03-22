using System;
using System.Windows.Forms;

namespace Netray_Aero
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.ThreadException += (s, ex) =>
                    System.IO.File.WriteAllText("crash.log", ex.Exception.ToString());
                AppDomain.CurrentDomain.UnhandledException += (s, ex) =>
                    System.IO.File.WriteAllText("crash.log", ex.ExceptionObject.ToString());
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                ThemeManager.LoadTheme();
                Application.Run(new TrayApp());
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(
                    ex.Message + "\n\n" + ex.StackTrace,
                    "Startup Error");
            }
        }
    }
        public class TrayApp : ApplicationContext
    {
        private NotifyIcon trayIcon;
        private Form1 popup;
        private ContextMenuStrip trayMenu;
        private System.Windows.Forms.Timer trayIconTimer;

        public TrayApp()
        {
            // Use system icon first - we'll swap it after
            trayIcon = new NotifyIcon();
            trayIcon.Icon = System.Drawing.SystemIcons.Application;
            trayIcon.Visible = true;
            trayIcon.Text = "Netray Aero";
            trayIcon.ContextMenuStrip = trayMenu;
            trayIcon.Click += TrayIcon_Click;

            popup = new Form1();
            popup.ShowInTaskbar = false;
            popup.TopMost = true;

            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Open", null, (s, e) => ShowPopup());
            trayMenu.Items.Add("-");
            trayMenu.Items.Add("Settings", null, (s, e) => OpenSettings());
            trayMenu.Items.Add("-");
            trayMenu.Items.Add("Exit", null, (s, e) => ExitApp());

            trayIcon.ContextMenuStrip = trayMenu;
            popup.Deactivate += (s, e) => popup.Hide();

            trayIconTimer = new System.Windows.Forms.Timer();
            trayIconTimer.Interval = 3000;
            trayIconTimer.Tick += (s, e) => UpdateTrayIcon();
            trayIconTimer.Start();

            UpdateTrayIcon();
        }

        private void UpdateTrayIcon()
        {
            try
            {
                string connectedSsid = "";
                var connectedNetworks = ManagedNativeWifi.NativeWifi.EnumerateConnectedNetworkSsids();
                foreach (var n in connectedNetworks)
                    connectedSsid = n.ToString();

                string iconFile;

                if (string.IsNullOrEmpty(connectedSsid))
                {
                    bool anyAvailable = false;
                    var available = ManagedNativeWifi.NativeWifi.EnumerateAvailableNetworks();
                    foreach (var n in available)
                    {
                        if (!string.IsNullOrEmpty(n.Ssid.ToString()))
                        {
                            anyAvailable = true;
                            break;
                        }
                    }
                    iconFile = anyAvailable ? "available.ico" : "wifi_off.ico";
                }
                else
                {
                    int signalLevel = 0;
                    var bssEntries = ManagedNativeWifi.NativeWifi.EnumerateBssNetworks();
                    foreach (var bss in bssEntries)
                    {
                        if (bss.Ssid.ToString() == connectedSsid)
                        {
                            signalLevel = bss.LinkQuality;
                            break;
                        }
                    }

                    if (signalLevel >= 80) iconFile = "signal_5.ico";
                    else if (signalLevel >= 60) iconFile = "signal_4.ico";
                    else if (signalLevel >= 40) iconFile = "signal_3.ico";
                    else if (signalLevel >= 20) iconFile = "signal_2.ico";
                    else iconFile = "signal_1.ico";
                }

                var icon = ResourceManager.LoadIcon(iconFile);
                if (icon != null) trayIcon.Icon = icon;
                trayIcon.Text = string.IsNullOrEmpty(connectedSsid)
                    ? "Not connected" : "Connected: " + connectedSsid;
            }
            catch
            {
                trayIcon.Icon = System.Drawing.SystemIcons.Application;
            }
        }

        private void TrayIcon_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = e as MouseEventArgs;
            if (me != null && me.Button == MouseButtons.Left)
                ShowPopup();
        }

        private void ShowPopup()
        {
            if (popup.Visible)
            {
                popup.Hide();
                return;
            }

            var screen = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
            popup.StartPosition = FormStartPosition.Manual;
            popup.Left = screen.Right - popup.Width - 15;
            popup.Top = screen.Bottom - popup.Height - 10;

            popup.Show();
            popup.BringToFront();
            popup.Activate();
        }

        private void OpenSettings()
        {
            SettingsForm settings = new SettingsForm();
            settings.ShowDialog();
        }

        private void ExitApp()
        {
            trayIcon.Visible = false;
            Application.Exit();
        }
    }
}