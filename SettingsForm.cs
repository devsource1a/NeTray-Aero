using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Netray_Aero
{
    public partial class SettingsForm : Form
    {
        private string currentTheme = "win7";

        public SettingsForm()
        {
            InitializeComponent();
            this.Text = "Netray Settings";
            this.Size = new Size(400, 320);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9f);
            this.BackColor = Color.White;

            // Load current theme
            currentTheme = ThemeManager.CurrentTheme;

            BuildUI();
        }

        private void BuildUI()
        {
            // Title
            Label lblTitle = new Label();
            lblTitle.Text = "Netray Aero — Settings";
            lblTitle.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(30, 30, 30);
            lblTitle.Location = new Point(16, 16);
            lblTitle.AutoSize = true;
            this.Controls.Add(lblTitle);

            // Separator
            Panel sep = new Panel();
            sep.BackColor = Color.FromArgb(200, 200, 200);
            sep.Location = new Point(16, 44);
            sep.Size = new Size(360, 1);
            this.Controls.Add(sep);

            // Theme label
            Label lblTheme = new Label();
            lblTheme.Text = "Theme";
            lblTheme.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            lblTheme.ForeColor = Color.FromArgb(60, 60, 60);
            lblTheme.Location = new Point(16, 56);
            lblTheme.AutoSize = true;
            this.Controls.Add(lblTheme);

            // Win7 theme button
            Panel pnlWin7 = CreateThemePanel(
                "Windows 7 Aero",
                "Classic Aero glass style",
                "win7",
                new Point(16, 76));
            this.Controls.Add(pnlWin7);

            // Vista theme button
            Panel pnlVista = CreateThemePanel(
                "Windows Vista",
                "Vista balloon notification style",
                "vista",
                new Point(16, 136));
            this.Controls.Add(pnlVista);

            // Win8 theme button
            Panel pnlWin8 = CreateThemePanel(
                "Windows 8",
                "Flat metro style with dark colors",
                "win8",
                new Point(16, 196));
            this.Controls.Add(pnlWin8);

            // Apply button
            Button btnApply = new Button();
            btnApply.Text = "Apply";
            btnApply.Size = new Size(80, 28);
            btnApply.Location = new Point(204, 248);
            btnApply.FlatStyle = FlatStyle.System;
            btnApply.Click += (s, e) =>
            {
                ThemeManager.CurrentTheme = currentTheme;
                ThemeManager.SaveTheme();
                MessageBox.Show("Theme saved! Restart the app to apply.", "Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            };
            this.Controls.Add(btnApply);

            // Cancel button
            Button btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Size = new Size(80, 28);
            btnCancel.Location = new Point(296, 248);
            btnCancel.FlatStyle = FlatStyle.System;
            btnCancel.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancel);
        }

        private Panel CreateThemePanel(string name, string description, string themeKey, Point location)
        {
            Panel panel = new Panel();
            panel.Size = new Size(360, 52);
            panel.Location = location;
            panel.Cursor = Cursors.Hand;
            panel.Tag = themeKey;

            bool isSelected = currentTheme == themeKey;
            panel.BackColor = isSelected ? Color.FromArgb(220, 235, 252) : Color.FromArgb(245, 245, 245);

            // Radio indicator
            Panel radio = new Panel();
            radio.Size = new Size(16, 16);
            radio.Location = new Point(10, 18);
            radio.BackColor = Color.White;
            radio.Tag = "radio_" + themeKey;

            // Draw circle
            radio.Paint += (s, pe) =>
            {
                bool sel = currentTheme == (string)panel.Tag;
                pe.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                pe.Graphics.DrawEllipse(new Pen(Color.FromArgb(100, 100, 100), 1.5f), 1, 1, 13, 13);
                if (sel)
                    pe.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(0, 102, 204)), 4, 4, 8, 8);
            };
            panel.Controls.Add(radio);

            // Theme name
            Label lblName = new Label();
            lblName.Text = name;
            lblName.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            lblName.ForeColor = Color.FromArgb(30, 30, 30);
            lblName.Location = new Point(34, 8);
            lblName.AutoSize = true;
            panel.Controls.Add(lblName);

            // Description
            Label lblDesc = new Label();
            lblDesc.Text = description;
            lblDesc.Font = new Font("Segoe UI", 8f);
            lblDesc.ForeColor = Color.FromArgb(120, 120, 120);
            lblDesc.Location = new Point(34, 26);
            lblDesc.AutoSize = true;
            panel.Controls.Add(lblDesc);

            // Border
            panel.Paint += (s, pe) =>
            {
                bool sel = currentTheme == (string)panel.Tag;
                ControlPaint.DrawBorder(pe.Graphics, panel.ClientRectangle,
                    sel ? Color.FromArgb(0, 102, 204) : Color.FromArgb(200, 200, 200),
                    1, ButtonBorderStyle.Solid,
                    sel ? Color.FromArgb(0, 102, 204) : Color.FromArgb(200, 200, 200),
                    1, ButtonBorderStyle.Solid,
                    sel ? Color.FromArgb(0, 102, 204) : Color.FromArgb(200, 200, 200),
                    1, ButtonBorderStyle.Solid,
                    sel ? Color.FromArgb(0, 102, 204) : Color.FromArgb(200, 200, 200),
                    1, ButtonBorderStyle.Solid);
            };

            // Click to select
            Action selectTheme = () =>
            {
                currentTheme = themeKey;
                foreach (Control c in this.Controls)
                {
                    if (c is Panel p && p.Tag is string tag &&
                        (tag == "win7" || tag == "vista" || tag == "win8"))
                    {
                        p.BackColor = currentTheme == tag
                            ? Color.FromArgb(220, 235, 252)
                            : Color.FromArgb(245, 245, 245);
                        p.Invalidate(true);
                    }
                }
            };

            panel.Click += (s, e) => selectTheme();
            foreach (Control c in panel.Controls)
                c.Click += (s, e) => selectTheme();

            return panel;
        }
    }
}