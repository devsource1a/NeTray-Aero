using System.Drawing;
using System.Windows.Forms;

namespace Netray_Aero
{
    public partial class ConnectProgressForm : Form
    {
        private Label lblStatus;
        private ProgressBar progressBar;

        public ConnectProgressForm(string ssid)
        {
            this.Text = "Connect to a Network";
            this.Size = new Size(340, 140);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ControlBox = true;

            lblStatus = new Label();
            lblStatus.Font = new Font("Segoe UI", 9f);
            lblStatus.Location = new Point(20, 20);
            lblStatus.Size = new Size(290, 30);
            this.Controls.Add(lblStatus);

            progressBar = new ProgressBar();
            progressBar.Location = new Point(20, 60);
            progressBar.Size = new Size(290, 20);
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.MarqueeAnimationSpeed = 30;
            this.Controls.Add(progressBar);

            Button btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Size = new Size(72, 24);
            btnCancel.Location = new Point(238, 88);
            btnCancel.FlatStyle = FlatStyle.System;
            btnCancel.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancel);
        }

        public void SetStatus(string status)
        {
            lblStatus.Text = status;
            lblStatus.Refresh();
        }
    }
}