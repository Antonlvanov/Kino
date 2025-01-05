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
                MessageBox.Show("Пароли не совпадают. Попробуйте снова.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int? klientId = null;
            Role role = Role.Klient;
            if (!isKlient)
            {
                bool registrationSuccess = UserManager.AddUser(username, password, email, role);
                if (registrationSuccess)
                {
                    MessageBox.Show("Регистрация прошла успешно!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Пользователь с таким именем или email уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            if (isKlient)
            {
                string nimi = ClientNameInput.Text;
                string telefon = ClientPhoneInput.Text;
                try
                {
                    klientId = UserManager.CreateKlientRecord(nimi, email, telefon);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении клиента: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                bool registrationSuccess = UserManager.AddUser(username, password, email, role, klientId?.ToString());
                if (registrationSuccess)
                {
                    MessageBox.Show("Регистрация прошла успешно!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Пользователь с таким именем или email уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
