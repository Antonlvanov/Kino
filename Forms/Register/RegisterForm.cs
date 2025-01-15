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

namespace Kino.Forms.Register
{
    public partial class RegisterForm : Form
    {
        public RegisterForm(UserManager _userManager)
        {
            Initialize();
            UserManager = _userManager;
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            string username = UsernameInput.Text.Trim();
            string email = EmailInput.Text.Trim();
            string password = PasswordInput.Text;
            string confirmPassword = ConfirmPasswordInput.Text;
            bool isKlient = ClientCheckBox.Checked;

            if (password != confirmPassword)
            {
                MessageBox.Show("Paroolid ei ühti. Proovige uuesti.", "Viga", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Palun täitke kõik nõutud väljad.", "Viga", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int? klientId = null;
            Role role = Role.Klient;
            if (!isKlient)
            {
                bool registrationSuccess = UserManager.AddUser(username, password, email, role);
                if (registrationSuccess)
                {
                    MessageBox.Show("Registreerimine õnnestus!", "Õnnestus", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Sama nime või e-posti aadressiga kasutaja on juba olemas.", "Viga", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            if (isKlient)
            {
                string nimi = ClientNameInput.Text;
                string telefon = ClientPhoneInput.Text;
                try
                {
                    klientId = clientManager.CreateKlientRecord(nimi, email, telefon);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Viga kliendi lisamisel: {ex.Message}", "Viga", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                bool registrationSuccess = UserManager.AddUser(username, password, email, role, klientId?.ToString());
                if (registrationSuccess)
                {
                    MessageBox.Show("Registreerimine õnnestus!", "Õnnestus", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Sama nime või e-posti aadressiga kasutaja on juba olemas", "Viga", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
