namespace Netray_Aero
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.bottom_panel = new System.Windows.Forms.Panel();
            this.OpenNet = new System.Windows.Forms.LinkLabel();
            this.Network_Panel = new System.Windows.Forms.Panel();
            this.Header_Panel = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.bottom_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // bottom_panel
            // 
            this.bottom_panel.BackColor = System.Drawing.Color.AliceBlue;
            this.bottom_panel.Controls.Add(this.pictureBox1);
            this.bottom_panel.Controls.Add(this.OpenNet);
            this.bottom_panel.Location = new System.Drawing.Point(1, 377);
            this.bottom_panel.Name = "bottom_panel";
            this.bottom_panel.Size = new System.Drawing.Size(267, 41);
            this.bottom_panel.TabIndex = 0;
            this.bottom_panel.Paint += new System.Windows.Forms.PaintEventHandler(this.bottom_panel_Paint);
            // 
            // OpenNet
            // 
            this.OpenNet.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.OpenNet.AutoSize = true;
            this.OpenNet.Font = new System.Drawing.Font("Microsoft Tai Le", 8.5F);
            this.OpenNet.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.OpenNet.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.OpenNet.Location = new System.Drawing.Point(40, 12);
            this.OpenNet.Name = "OpenNet";
            this.OpenNet.Size = new System.Drawing.Size(188, 16);
            this.OpenNet.TabIndex = 0;
            this.OpenNet.TabStop = true;
            this.OpenNet.Text = "Open Network and Sharing Center";
            this.OpenNet.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.OpenNet.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // Network_Panel
            // 
            this.Network_Panel.Location = new System.Drawing.Point(1, 76);
            this.Network_Panel.Name = "Network_Panel";
            this.Network_Panel.Size = new System.Drawing.Size(267, 301);
            this.Network_Panel.TabIndex = 1;
            this.Network_Panel.Paint += new System.Windows.Forms.PaintEventHandler(this.Network_Panel_Paint);
            // 
            // Header_Panel
            // 
            this.Header_Panel.Location = new System.Drawing.Point(1, 1);
            this.Header_Panel.Name = "Header_Panel";
            this.Header_Panel.Size = new System.Drawing.Size(267, 69);
            this.Header_Panel.TabIndex = 2;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(267, 14);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(269, 419);
            this.ControlBox = false;
            this.Controls.Add(this.Header_Panel);
            this.Controls.Add(this.Network_Panel);
            this.Controls.Add(this.bottom_panel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.bottom_panel.ResumeLayout(false);
            this.bottom_panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel bottom_panel;
        private System.Windows.Forms.Panel Network_Panel;
        private System.Windows.Forms.LinkLabel OpenNet;
        private System.Windows.Forms.Panel Header_Panel;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}