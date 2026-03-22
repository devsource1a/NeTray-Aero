using ManagedNativeWifi;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Netray_Aero
{
    public partial class VistaForm : Form
    {
        [DllImport("dwmapi.dll")]
        private static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);
        [StructLayout(LayoutKind.Sequential)]
        struct MARGINS { public int Left, Right, Top, Bottom; }

        private string connectedSsid = "";

        public VistaForm()
        {
            InitializeComponent();
            this.Size = new Size(230, 130);
            this.BackColor = Color.FromArgb(235, 243, 255);
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.DoubleBuffered = true;
            this.Cursor = Cursors.Arrow;

            var connectedNetworks = NativeWifi.EnumerateConnectedNetworkSsids();
            foreach (var n in connectedNetworks)
                connectedSsid = n.ToString();

            BuildUI();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            MARGINS margins = new MARGINS { Left = 1, Right = 1, Top = 1, Bottom = 1 };
            DwmExtendFrameIntoClientArea(this.Handle, ref margins);
        }

        private void BuildUI()
        {
            bool isConnected = !string.IsNullOrEmpty(connectedSsid);

            PictureBox icon = new PictureBox();
            icon.Size = new Size(40, 40);
            icon.Location = new Point(10, 12);
            icon.BackColor = Color.Transparent;
            icon.SizeMode = PictureBoxSizeMode.StretchImage;
            try
            {
                string cat = isConnected ? GetNetworkCategory(connectedSsid) : "disconnected";
                icon.Image = GetNetworkIcon(cat);
            }
            catch { }
            this.Controls.Add(icon);

            Label lblTitle = new Label();
            lblTitle.Text = isConnected ? connectedSsid : "Not Connected";
            lblTitle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(20, 20, 20);
            lblTitle.Location = new Point(56, 10);
            lblTitle.Size = new Size(162, 18);
            lblTitle.BackColor = Color.Transparent;
            this.Controls.Add(lblTitle);

            Label lblSub = new Label();
            lblSub.Text = isConnected ? "Internet access" : "Wireless networks are available.";
            lblSub.Font = new Font("Segoe UI", 8f);
            lblSub.ForeColor = Color.FromArgb(80, 80, 80);
            lblSub.Location = new Point(56, 28);
            lblSub.Size = new Size(162, 30);
            lblSub.BackColor = Color.Transparent;
            this.Controls.Add(lblSub);

            Panel sep = new Panel();
            sep.BackColor = Color.FromArgb(180, 200, 230);
            sep.Location = new Point(0, 62);
            sep.Size = new Size(Width, 1);
            this.Controls.Add(sep);

            Panel footer = new Panel();
            footer.Location = new Point(0, 63);
            footer.Size = new Size(Width, Height - 63);
            footer.BackColor = Color.FromArgb(220, 233, 250);
            footer.Cursor = Cursors.Arrow;
            this.Controls.Add(footer);

            LinkLabel lnkConnect = new LinkLabel();
            lnkConnect.Text = "Connect to a network";
            lnkConnect.Font = new Font("Segoe UI", 8.5f);
            lnkConnect.LinkColor = Color.FromArgb(0, 70, 180);
            lnkConnect.ActiveLinkColor = Color.FromArgb(0, 70, 180);
            lnkConnect.LinkBehavior = LinkBehavior.HoverUnderline;
            lnkConnect.Location = new Point(50, 6);
            lnkConnect.AutoSize = true;
            lnkConnect.BackColor = Color.Transparent;
            lnkConnect.Cursor = Cursors.Arrow;
            lnkConnect.LinkClicked += (s, e) =>
                System.Diagnostics.Process.Start("control.exe", "/name Microsoft.NetworkAndSharingCenter /page ConnectToNetwork");
            footer.Controls.Add(lnkConnect);

            LinkLabel lnkSharing = new LinkLabel();
            lnkSharing.Text = "Network and Sharing Center";
            lnkSharing.Font = new Font("Segoe UI", 8.5f);
            lnkSharing.LinkColor = Color.FromArgb(0, 70, 180);
            lnkSharing.ActiveLinkColor = Color.FromArgb(0, 70, 180);
            lnkSharing.LinkBehavior = LinkBehavior.HoverUnderline;
            lnkSharing.Location = new Point(40, 26);
            lnkSharing.AutoSize = true;
            lnkSharing.BackColor = Color.Transparent;
            lnkSharing.Cursor = Cursors.Arrow;
            lnkSharing.LinkClicked += (s, e) =>
                System.Diagnostics.Process.Start("control.exe", "/name Microsoft.NetworkAndSharingCenter");
            footer.Controls.Add(lnkSharing);
        }

        private string GetNetworkCategory(string ssid)
        {
            try
            {
                using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                    @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\NetworkList\Profiles"))
                {
                    if (key != null)
                    {
                        foreach (string subKeyName in key.GetSubKeyNames())
                        {
                            using (var subKey = key.OpenSubKey(subKeyName))
                            {
                                var profileName = subKey?.GetValue("ProfileName")?.ToString();
                                if (profileName == ssid)
                                {
                                    var category = subKey?.GetValue("Category");
                                    if (category != null)
                                    {
                                        switch ((int)category)
                                        {
                                            case 0: return "public";
                                            case 1: return "private";
                                            case 2: return "home";
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { }
            return string.IsNullOrEmpty(ssid) ? "disconnected" : "private";
        }

        private Image GetNetworkIcon(string category)
        {
            string filename;
            switch (category)
            {
                case "home": filename = "icon_home.png"; break;
                case "private": filename = "icon_private.png"; break;
                case "public": filename = "icon_public.png"; break;
                default: filename = "icon_disconnected.png"; break;
            }
            try
            {
                return Image.FromFile(System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, filename));
            }
            catch { return null; }
        }

        private void VistaForm_Load(object sender, EventArgs e)
        {

        }
    }
}