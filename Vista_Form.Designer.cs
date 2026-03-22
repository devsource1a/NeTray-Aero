namespace Netray_Aero
{
    partial class VistaForm
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
            this.SuspendLayout();
            // 
            // VistaForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.ControlBox = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VistaForm";
            this.Load += new System.EventHandler(this.VistaForm_Load);
            this.ResumeLayout(false);

        }
    }
}