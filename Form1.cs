using ManagedNativeWifi;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Netray_Aero
{
    public partial class Form1 : Form
    {
        [DllImport("dwmapi.dll")]
        private static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);

        [StructLayout(LayoutKind.Sequential)]
        struct MARGINS { public int Left, Right, Top, Bottom; }

        private List<AvailableNetworkPack> foundNetworks = new List<AvailableNetworkPack>();
        private NetworkItem selectedItem = null;
        private bool networkListVisible = true;
        private Button collapseBtn;
        private Panel wifiHeader;
        private int totalItemHeight = 0;
        private int scrollOffset = 0;

        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            MARGINS margins = new MARGINS { Left = 1, Right = 1, Top = 1, Bottom = 1 };
            DwmExtendFrameIntoClientArea(this.Handle, ref margins);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            ControlPaint.DrawBorder(e.Graphics, ClientRectangle,
                Color.FromArgb(100, 100, 100), 1, ButtonBorderStyle.Solid,
                Color.FromArgb(100, 100, 100), 1, ButtonBorderStyle.Solid,
                Color.FromArgb(100, 100, 100), 1, ButtonBorderStyle.Solid,
                Color.FromArgb(100, 100, 100), 1, ButtonBorderStyle.Solid);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false;
            this.Cursor = Cursors.Arrow;
            Network_Panel.Cursor = Cursors.Arrow;
            Network_Panel.MouseEnter += (s, ev) => Network_Panel.Focus();
            this.MouseWheel += (s, me) => ScrollNetworks(me.Delta);

            string connectedSsid = "";
            var connectedNetworks = NativeWifi.EnumerateConnectedNetworkSsids();
            foreach (var n in connectedNetworks)
                connectedSsid = n.ToString();

            Label lblCurrently = new Label();
            lblCurrently.Text = "Currently connected to:";
            lblCurrently.Font = new Font("Segoe UI", 8.5f);
            lblCurrently.ForeColor = Color.FromArgb(60, 60, 60);
            lblCurrently.Location = new Point(8, 8);
            lblCurrently.AutoSize = true;
            lblCurrently.BackColor = Color.Transparent;
            lblCurrently.Cursor = Cursors.Arrow;
            Header_Panel.Controls.Add(lblCurrently);

            PictureBox networkIcon = new PictureBox();
            networkIcon.Name = "networkIcon";
            networkIcon.Size = new Size(32, 32);
            networkIcon.Location = new Point(8, 26);
            networkIcon.BackColor = Color.Transparent;
            networkIcon.SizeMode = PictureBoxSizeMode.StretchImage;
            string connectedCategory = string.IsNullOrEmpty(connectedSsid)
                ? "disconnected" : GetNetworkCategory(connectedSsid);
            networkIcon.Image = GetNetworkIcon(connectedCategory);
            Header_Panel.Controls.Add(networkIcon);

            Label lblSsidName = new Label();
            lblSsidName.Name = "lblSsidName";
            lblSsidName.Text = string.IsNullOrEmpty(connectedSsid) ? "Connections are available" : connectedSsid;
            lblSsidName.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            lblSsidName.ForeColor = Color.FromArgb(30, 30, 30);
            lblSsidName.Location = new Point(48, 28);
            lblSsidName.AutoSize = true;
            lblSsidName.BackColor = Color.Transparent;
            lblSsidName.Cursor = Cursors.Arrow;
            Header_Panel.Controls.Add(lblSsidName);

            Label lblAccess = new Label();
            lblAccess.Name = "lblAccess";
            lblAccess.Text = string.IsNullOrEmpty(connectedSsid) ? "" : "Internet access";
            lblAccess.Font = new Font("Segoe UI", 8f);
            lblAccess.ForeColor = Color.FromArgb(80, 80, 80);
            lblAccess.Location = new Point(48, 46);
            lblAccess.AutoSize = true;
            lblAccess.BackColor = Color.Transparent;
            lblAccess.Cursor = Cursors.Arrow;
            Header_Panel.Controls.Add(lblAccess);

            Button btnRefresh = new Button();
            btnRefresh.Size = new Size(23, 23);
            btnRefresh.Location = new Point(Header_Panel.Width - 26, 4);
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnRefresh.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnRefresh.BackColor = Color.Transparent;
            btnRefresh.Text = "";
            btnRefresh.Image = ResourceManager.LoadImage("refresh.png");
            btnRefresh.ImageAlign = ContentAlignment.MiddleCenter;
            btnRefresh.Cursor = Cursors.Arrow;
            btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRefresh.MouseEnter += (s, ev) => btnRefresh.Image = ResourceManager.LoadImage("refresh_hover.png");
            btnRefresh.MouseLeave += (s, ev) => btnRefresh.Image = ResourceManager.LoadImage("refresh.png");
            btnRefresh.Click += (s, ev) => { UpdateHeader(); ScanNetworks(); };
            Header_Panel.Controls.Add(btnRefresh);

            wifiHeader = new Panel();
            wifiHeader.Size = new Size(Network_Panel.Width, 24);
            wifiHeader.Location = new Point(Network_Panel.Left, Network_Panel.Top);
            wifiHeader.BackColor = Color.White;
            wifiHeader.Cursor = Cursors.Arrow;
            wifiHeader.MouseEnter += (s, ev) => Network_Panel.Focus();
            wifiHeader.Paint += (s, pe) =>
            {
                pe.Graphics.DrawLine(new Pen(Color.FromArgb(180, 180, 180)),
                    0, 0, wifiHeader.Width, 0);
            };
            this.Controls.Add(wifiHeader);

            Network_Panel.Top = wifiHeader.Bottom;
            Network_Panel.Height = Network_Panel.Height - wifiHeader.Height;

            Label lblWifi = new Label();
            lblWifi.Text = "Wi-Fi";
            lblWifi.Font = new Font("Segoe UI", 8.5f);
            lblWifi.ForeColor = Color.FromArgb(100, 100, 100);
            lblWifi.Location = new Point(8, 4);
            lblWifi.AutoSize = true;
            lblWifi.Cursor = Cursors.Arrow;
            lblWifi.BackColor = Color.Transparent;
            lblWifi.Click += (s, pe) => ToggleNetworkList();
            wifiHeader.Controls.Add(lblWifi);

            collapseBtn = new Button();
            collapseBtn.Size = new Size(24, 24);
            collapseBtn.FlatStyle = FlatStyle.Flat;
            collapseBtn.FlatAppearance.BorderSize = 0;
            collapseBtn.FlatAppearance.MouseOverBackColor = Color.Transparent;
            collapseBtn.FlatAppearance.MouseDownBackColor = Color.Transparent;
            collapseBtn.BackColor = Color.Transparent;
            collapseBtn.Image = ResourceManager.LoadImage("Chevron_Up.png");
            collapseBtn.ImageAlign = ContentAlignment.MiddleCenter;
            collapseBtn.Text = "";
            collapseBtn.Location = new Point(wifiHeader.Width - 26, 0);
            collapseBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            collapseBtn.Cursor = Cursors.Arrow;
            collapseBtn.Click += (s, pe) => ToggleNetworkList();
            collapseBtn.MouseEnter += (s, pe) =>
                collapseBtn.Image = ResourceManager.LoadImage(
                    networkListVisible ? "Chevron_Up_Highlight.png" : "Chevron_Down_Highlight.png");
            collapseBtn.MouseLeave += (s, pe) =>
                collapseBtn.Image = ResourceManager.LoadImage(
                    networkListVisible ? "Chevron_Up.png" : "Chevron_Down.png");
            wifiHeader.Controls.Add(collapseBtn);

            bottom_panel.Paint += (s, pe) =>
            {
                Panel p = (Panel)s;
                pe.Graphics.DrawLine(new Pen(Color.FromArgb(180, 180, 180)),
                    0, 0, p.Width, 0);
            };
            bottom_panel.Invalidate();

            ScanNetworks();
            this.Refresh();
        }

        private void ScrollNetworks(int delta)
        {
            int maxScroll = Math.Max(0, totalItemHeight - Network_Panel.Height);
            scrollOffset -= delta / 3;
            scrollOffset = Math.Max(0, Math.Min(scrollOffset, maxScroll));

            int y = -scrollOffset;
            foreach (Control c in Network_Panel.Controls)
            {
                c.Top = y;
                y += c.Height;
            }
            Network_Panel.Invalidate();
        }

        private void UpdateScrollBar()
        {
            totalItemHeight = 0;
            foreach (Control c in Network_Panel.Controls)
                totalItemHeight += c.Height;

            foreach (Control c in Network_Panel.Controls)
                c.Width = Network_Panel.ClientSize.Width;

            Network_Panel.Invalidate();
        }

        public void ApplyScroll(int scrollValue)
        {
            scrollOffset = scrollValue;
            int y = -scrollOffset;
            foreach (Control c in Network_Panel.Controls)
            {
                c.Top = y;
                y += c.Height;
            }
        }

        public int GetScrollValue() { return scrollOffset; }

        public void UpdateScrollBarPublic()
        {
            totalItemHeight = 0;
            foreach (Control c in Network_Panel.Controls)
                totalItemHeight += c.Height;

            int y = -scrollOffset;
            foreach (Control c in Network_Panel.Controls)
            {
                c.Top = y;
                y += c.Height;
            }
            Network_Panel.Invalidate();
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e) { }

        public void UpdateHeader()
        {
            string connectedSsid = "";
            var connectedNetworks = NativeWifi.EnumerateConnectedNetworkSsids();
            foreach (var n in connectedNetworks)
                connectedSsid = n.ToString();

            foreach (Control c in Header_Panel.Controls)
            {
                if (c is Label lbl)
                {
                    if (lbl.Name == "lblSsidName")
                        lbl.Text = string.IsNullOrEmpty(connectedSsid) ? "Connections are available" : connectedSsid;
                    if (lbl.Name == "lblAccess")
                        lbl.Text = string.IsNullOrEmpty(connectedSsid) ? "" : "Internet access";
                }
                if (c is PictureBox pb && pb.Name == "networkIcon")
                {
                    string category = string.IsNullOrEmpty(connectedSsid)
                        ? "disconnected" : GetNetworkCategory(connectedSsid);
                    pb.Image = GetNetworkIcon(category);
                }
            }
        }

        private void ToggleNetworkList()
        {
            networkListVisible = !networkListVisible;
            Network_Panel.Visible = networkListVisible;
            collapseBtn.Image = ResourceManager.LoadImage(
                networkListVisible ? "Chevron_Up.png" : "Chevron_Down.png");
        }

        public void RefreshNetworks() { ScanNetworks(); }

        private void ScanNetworks()
        {
            scrollOffset = 0;
            Network_Panel.Controls.Clear();
            foundNetworks.Clear();
            totalItemHeight = 0;

            string connectedSsid = "";
            var connectedNetworks = NativeWifi.EnumerateConnectedNetworkSsids();
            foreach (var n in connectedNetworks)
                connectedSsid = n.ToString();

            var bssEntries = NativeWifi.EnumerateBssNetworks();
            var networks = NativeWifi.EnumerateAvailableNetworks();
            int y = 0;
            HashSet<string> seen = new HashSet<string>();

            foreach (var network in networks)
            {
                string ssid = network.Ssid.ToString();
                if (string.IsNullOrEmpty(ssid) || seen.Contains(ssid)) continue;
                seen.Add(ssid);
                foundNetworks.Add(network);
                bool isConnected = ssid == connectedSsid;

                int signalLevel = 0;
                foreach (var bss in bssEntries)
                    if (bss.Ssid.ToString() == ssid) { signalLevel = bss.LinkQuality; break; }

                NetworkItem item = new NetworkItem(network, ssid, isConnected, signalLevel);
                item.Width = Network_Panel.ClientSize.Width;
                item.Top = y;
                item.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                item.OnNetworkSelected += Item_OnNetworkSelected;
                item.MouseWheel += (s, me) => ScrollNetworks(me.Delta);
                item.MouseEnter += (s, me) => Network_Panel.Focus();
                Network_Panel.Controls.Add(item);

                y += item.Height;
                totalItemHeight += item.Height;
            }

            UpdateScrollBar();
            Network_Panel.Invalidate();
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
            switch (category)
            {
                case "home": return ResourceManager.LoadImage("icon_home.png");
                case "private": return ResourceManager.LoadImage("icon_private.png");
                case "public": return ResourceManager.LoadImage("icon_public.png");
                default: return ResourceManager.LoadImage("icon_disconnected.png");
            }
        }

        private void Item_OnNetworkSelected(NetworkItem item)
        {
            if (selectedItem != null && selectedItem != item)
                selectedItem.SetSelected(false);
            selectedItem = item;
        }

        private void OpenSharing_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("control.exe", "/name Microsoft.NetworkAndSharingCenter");
        }

        private void bottom_panel_Paint(object sender, PaintEventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("control.exe", "/name Microsoft.NetworkAndSharingCenter");
        }
        private void Network_Panel_Paint(object sender, PaintEventArgs e) { }
    }

    public class NetworkItem : Panel
    {
        private AvailableNetworkPack network;
        private string ssid;
        private bool isSelected = false;
        private bool isConnected;
        private int signalLevel;

        private Label lblSsid;
        private Panel expandPanel;
        private CheckBox chkAutoConnect;
        private Button btnConnect;

        private static Image hoverImage = null;
        private static Image highlightImage = null;
        private static Image signal1 = null;
        private static Image signal2 = null;
        private static Image signal3 = null;
        private static Image signal4 = null;
        private static Image signal5 = null;

        private static void LoadImages()
        {
            if (hoverImage != null) return;
            hoverImage = ResourceManager.LoadImage("hover.png");
            highlightImage = ResourceManager.LoadImage("highlight.png");
            signal1 = ResourceManager.LoadImage("signal_1.png");
            signal2 = ResourceManager.LoadImage("signal_2.png");
            signal3 = ResourceManager.LoadImage("signal_3.png");
            signal4 = ResourceManager.LoadImage("signal_4.png");
            signal5 = ResourceManager.LoadImage("signal_5.png");
        }

        public delegate void NetworkSelectedHandler(NetworkItem item);
        public event NetworkSelectedHandler OnNetworkSelected;

        public NetworkItem(AvailableNetworkPack network, string ssid, bool isConnected, int signalLevel)
        {
            LoadImages();
            this.network = network;
            this.ssid = ssid;
            this.isConnected = isConnected;
            this.signalLevel = signalLevel;

            this.Height = 36;
            this.BackColor = Color.White;
            this.Cursor = Cursors.Arrow;

            lblSsid = new Label();
            lblSsid.Text = ssid;
            lblSsid.Font = new Font("Segoe UI", 9f,
                isConnected ? FontStyle.Bold : FontStyle.Regular);
            lblSsid.ForeColor = Color.FromArgb(0, 102, 204);
            lblSsid.Location = new Point(12, 10);
            lblSsid.AutoSize = true;
            lblSsid.Cursor = Cursors.Arrow;
            lblSsid.BackColor = Color.Transparent;
            lblSsid.Click += (s, e) => ToggleSelect();
            this.Controls.Add(lblSsid);

            if (isConnected)
            {
                Label lblConnected = new Label();
                lblConnected.Text = "Connected";
                lblConnected.Font = new Font("Segoe UI", 9.0f, FontStyle.Bold);
                lblConnected.ForeColor = Color.Black;
                lblConnected.AutoSize = true;
                lblConnected.Location = new Point(150, 11);
                lblConnected.Cursor = Cursors.Arrow;
                lblConnected.BackColor = Color.Transparent;
                this.Controls.Add(lblConnected);
            }

            expandPanel = new Panel();
            expandPanel.Height = 36;
            expandPanel.Top = 36;
            expandPanel.Left = 0;
            expandPanel.BackColor = Color.Transparent;
            expandPanel.Visible = false;
            expandPanel.Cursor = Cursors.Arrow;

            chkAutoConnect = new CheckBox();
            chkAutoConnect.Text = "Connect automatically";
            chkAutoConnect.Font = new Font("Segoe UI", 8.5f);
            chkAutoConnect.Location = new Point(8, 8);
            chkAutoConnect.Checked = true;
            chkAutoConnect.AutoSize = true;
            chkAutoConnect.Cursor = Cursors.Arrow;
            chkAutoConnect.BackColor = Color.Transparent;
            expandPanel.Controls.Add(chkAutoConnect);

            btnConnect = new Button();
            btnConnect.Text = isConnected ? "Disconnect" : "Connect";
            btnConnect.Font = new Font("Segoe UI", 8.5f);
            btnConnect.Size = new Size(72, 24);
            btnConnect.FlatStyle = FlatStyle.System;
            btnConnect.Cursor = Cursors.Arrow;
            btnConnect.Click += BtnConnect_Click;
            expandPanel.Controls.Add(btnConnect);

            this.Controls.Add(expandPanel);
            this.Click += (s, e) => ToggleSelect();
            this.Paint += NetworkItem_Paint;

            this.MouseEnter += (s, e) =>
            {
                if (!isSelected) { this.BackgroundImage = hoverImage; this.BackgroundImageLayout = ImageLayout.Stretch; }
            };
            this.MouseLeave += (s, e) =>
            {
                if (!isSelected) { this.BackgroundImage = null; this.BackColor = Color.White; }
            };
            lblSsid.MouseEnter += (s, e) =>
            {
                if (!isSelected) { this.BackgroundImage = hoverImage; this.BackgroundImageLayout = ImageLayout.Stretch; }
            };
            lblSsid.MouseLeave += (s, e) =>
            {
                if (!isSelected) { this.BackgroundImage = null; this.BackColor = Color.White; }
            };
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            if (this.Parent == null) return;
            this.Parent.SizeChanged += (s, ev) =>
            {
                if (this.Parent == null) return;
                this.Width = this.Parent.ClientSize.Width;
                if (isSelected)
                {
                    expandPanel.Width = this.Width;
                    btnConnect.Location = new Point(this.Width - 82, 6);
                }
                this.Invalidate();
            };
        }

        private void NetworkItem_Paint(object sender, PaintEventArgs e)
        {
            Image signalImage;
            if (signalLevel >= 80) signalImage = signal5;
            else if (signalLevel >= 60) signalImage = signal4;
            else if (signalLevel >= 40) signalImage = signal3;
            else if (signalLevel >= 20) signalImage = signal2;
            else signalImage = signal1;

            int imgSize = 25;
            int imgX = this.Width - imgSize - 6;
            int imgY = (36 - imgSize) / 2;
            if (signalImage != null)
                e.Graphics.DrawImage(signalImage, imgX, imgY, imgSize, imgSize);
        }

        private void ToggleSelect()
        {
            SetSelected(!isSelected);
            OnNetworkSelected?.Invoke(this);
        }

        public void SetSelected(bool selected)
        {
            isSelected = selected;

            if (this.Parent != null)
                this.Parent.SuspendLayout();

            if (selected)
            {
                this.BackgroundImage = highlightImage;
                this.BackgroundImageLayout = ImageLayout.Stretch;
                this.BackColor = Color.Transparent;
                expandPanel.Visible = true;
                expandPanel.Width = this.Width;
                btnConnect.Location = new Point(this.Width - 82, 6);
                this.Height = 72;
            }
            else
            {
                this.BackgroundImage = null;
                this.BackColor = Color.White;
                expandPanel.Visible = false;
                this.Height = 36;
            }

            if (this.Parent != null)
            {
                Form pf = this.FindForm();
                int currentScroll = pf is Form1 f1 ? f1.GetScrollValue() : 0;

                int y = -currentScroll;
                foreach (Control c in this.Parent.Controls)
                {
                    c.Top = y;
                    y += c.Height;
                }
                this.Parent.ResumeLayout();

                if (pf is Form1 form1)
                    form1.UpdateScrollBarPublic();
            }

            this.Invalidate();
        }

        private void RefreshParent()
        {
            Form parentForm = this.FindForm();
            if (parentForm is Form1 form)
            {
                form.UpdateHeader();
                form.RefreshNetworks();
            }
        }

        private string ShowCategoryDialog(string ssid)
        {
            Form dialog = new Form();
            dialog.Text = "Network Type";
            dialog.Size = new Size(280, 160);
            dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
            dialog.StartPosition = FormStartPosition.CenterScreen;
            dialog.MaximizeBox = false;
            dialog.MinimizeBox = false;

            Label lbl = new Label();
            lbl.Text = "What type of network is \"" + ssid + "\"?";
            lbl.Font = new Font("Segoe UI", 9f);
            lbl.Location = new Point(10, 10);
            lbl.Size = new Size(260, 30);
            dialog.Controls.Add(lbl);

            Button btnHome = new Button();
            btnHome.Text = "Home";
            btnHome.Size = new Size(72, 28);
            btnHome.Location = new Point(10, 50);
            btnHome.FlatStyle = FlatStyle.System;
            btnHome.DialogResult = DialogResult.Yes;
            dialog.Controls.Add(btnHome);

            Button btnPrivate = new Button();
            btnPrivate.Text = "Private";
            btnPrivate.Size = new Size(72, 28);
            btnPrivate.Location = new Point(90, 50);
            btnPrivate.FlatStyle = FlatStyle.System;
            btnPrivate.DialogResult = DialogResult.No;
            dialog.Controls.Add(btnPrivate);

            Button btnPublic = new Button();
            btnPublic.Text = "Public";
            btnPublic.Size = new Size(72, 28);
            btnPublic.Location = new Point(170, 50);
            btnPublic.FlatStyle = FlatStyle.System;
            btnPublic.DialogResult = DialogResult.Ignore;
            dialog.Controls.Add(btnPublic);

            var result = dialog.ShowDialog();
            if (result == DialogResult.Yes) return "home";
            if (result == DialogResult.No) return "private";
            return "public";
        }

        private void SetNetworkCategory(string ssid, string category)
        {
            try
            {
                int categoryValue = category == "home" ? 2 : category == "private" ? 1 : 0;
                using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                    @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\NetworkList\Profiles", true))
                {
                    if (key != null)
                    {
                        foreach (string subKeyName in key.GetSubKeyNames())
                        {
                            using (var subKey = key.OpenSubKey(subKeyName, true))
                            {
                                var profileName = subKey?.GetValue("ProfileName")?.ToString();
                                if (profileName == ssid)
                                    subKey.SetValue("Category", categoryValue,
                                        Microsoft.Win32.RegistryValueKind.DWord);
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private string ShowPasswordDialog(string ssid)
        {
            using (PasswordDialog dialog = new PasswordDialog(ssid))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                    return dialog.Password;
                return "";
            }
        }

        private async void BtnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (isConnected)
                {
                    await NativeWifi.DisconnectNetworkAsync(
                        interfaceId: network.InterfaceInfo.Id,
                        timeout: TimeSpan.FromSeconds(10));
                    RefreshParent();
                    MessageBox.Show("Disconnected from " + ssid, "WiFi");
                }
                else
                {
                    string password = "";
                    if (network.IsSecurityEnabled)
                    {
                        password = ShowPasswordDialog(ssid);
                        if (string.IsNullOrEmpty(password)) return;
                    }

                    ConnectProgressForm progress = new ConnectProgressForm(ssid);
                    progress.Show();
                    progress.SetStatus("Getting information from " + ssid + "...");
                    await Task.Delay(1500);
                    progress.SetStatus("Connecting to " + ssid + "...");

                    await NativeWifi.ConnectNetworkAsync(
                        interfaceId: network.InterfaceInfo.Id,
                        profileName: ssid,
                        bssType: network.BssType,
                        timeout: TimeSpan.FromSeconds(10));

                    progress.Close();
                    string category = ShowCategoryDialog(ssid);
                    SetNetworkCategory(ssid, category);
                    RefreshParent();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not complete action: " + ex.Message, "Error");
            }
        }
    }
}