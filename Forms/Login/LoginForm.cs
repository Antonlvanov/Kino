using Kino.UserControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kino.Forms.Login
{
    public partial class LoginForm : Form
    {
        public LoginForm(UserManager userManager)
        {
            UserManager = userManager;
            this.ClientSize = new Size(250, 250);
            this.Text = "Sisse loogimine";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            InitializeComponents();
            LayoutComponents();
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            string username = UsernameInput.Text.Trim();
            string password = PasswordInput.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Palun täitke kõik väljad.", "Viga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (UserManager.LoginUser(username, password))
            {
                MessageBox.Show($"Tere tulemast, {username}!", "Edukas sisselogimine", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Vale kasutajanimi või parool.", "Viga", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
