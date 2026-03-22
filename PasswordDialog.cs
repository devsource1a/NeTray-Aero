using System.Drawing;
using System.Windows.Forms;

namespace Netray_Aero
{
    public partial class PasswordDialog : Form
    {
        private Label lbl;
        private Label lblKey;
        private TextBox txtPassword;
        private CheckBox chkHide;
        private Button btnOk;
        private Button btnCancel;

        public string Password => txtPassword.Text;

        public PasswordDialog(string ssid)
        {
            this.Text = "Connect to a Network";
            this.Size = new Size(390, 200);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = SystemColors.Control;

            // "Type the network security key" — top instruction label
            lbl = new Label();
            lbl.Text = "Type the network security key";
            lbl.Font = new Font("Segoe UI", 9f, FontStyle.Regular);
            lbl.Location = new Point(16, 16);
            lbl.AutoSize = true;
            this.Controls.Add(lbl);

            // Horizontal separator line (mimics the Aero rule)
            var separator = new Label();
            separator.BorderStyle = BorderStyle.Fixed3D;
            separator.Location = new Point(16, 38);
            separator.Size = new Size(348, 2);
            this.Controls.Add(separator);

            // "Security key:" label — left-aligned, vertically centered with textbox
            lblKey = new Label();
            lblKey.Text = "Security key:";
            lblKey.Font = new Font("Segoe UI", 9f);
            lblKey.Location = new Point(16, 58);
            lblKey.AutoSize = true;
            this.Controls.Add(lblKey);

            // Password TextBox — to the right of the label
            txtPassword = new TextBox();
            txtPassword.Location = new Point(110, 55);
            txtPassword.Width = 200;
            txtPassword.PasswordChar = '\0'; // Visible by default (matches screenshot)
            this.Controls.Add(txtPassword);

            // "Hide characters" checkbox — below the textbox, unchecked by default
            chkHide = new CheckBox();
            chkHide.Text = "Hide characters";
            chkHide.Font = new Font("Segoe UI", 8.5f);
            chkHide.Location = new Point(110, 82);
            chkHide.Checked = false; // Unchecked = characters visible (matches screenshot)
            chkHide.AutoSize = true;
            chkHide.CheckedChanged += (s, e) =>
            {
                // Checked = hide characters; Unchecked = show characters
                txtPassword.PasswordChar = chkHide.Checked ? '*' : '\0';
            };
            this.Controls.Add(chkHide);

            // OK button
            btnOk = new Button();
            btnOk.Text = "OK";
            btnOk.Size = new Size(75, 26);
            btnOk.Location = new Point(215, 130);
            btnOk.FlatStyle = FlatStyle.System;
            btnOk.DialogResult = DialogResult.OK;
            this.Controls.Add(btnOk);

            // Cancel button
            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Size = new Size(75, 26);
            btnCancel.Location = new Point(296, 130);
            btnCancel.FlatStyle = FlatStyle.System;
            btnCancel.DialogResult = DialogResult.Cancel;
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
        }

        private void PasswordDialog_Load(object sender, System.EventArgs e)
        {
        }
    }
}