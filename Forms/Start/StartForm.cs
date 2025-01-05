using Kino.Forms.Admin;
using Kino.Forms.Login;
using Kino.Forms.Register;
using Kino.Forms.Sessions;
using Kino.UserControl;
using System;
using System.Windows.Forms;

namespace Kino.Forms.Start
{
    public partial class StartForm : Form
    {
        public StartForm()
        {
            Initialize();
            UserManager = new UserManager();
            UpdateFormForUser();
        }

        public void Login_Click(object sender, EventArgs e)
        {
            LoginForm loginForm = new LoginForm(UserManager);
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                UpdateFormForUser();
            }
        }
        private void LogoutButton_Click(object sender, EventArgs e)
        {
            UserManager.Logout();
            UpdateFormForUser();
        }
        private void Register_Click(object sender, System.EventArgs e)
        {
            RegisterForm registerForm = new RegisterForm(UserManager);
            registerForm.ShowDialog();
        }
        public void AdminPanel_Click(object sender, EventArgs e)
        {
            AdminForm adminForm = new AdminForm();
            adminForm.ShowDialog();
        }
        public void Sessions_Click(object sender, EventArgs e)
        {
            SessionsForm sessions = new SessionsForm();//UserManager
            sessions.ShowDialog();
        }

        public void UpdateFormForUser()
        {
            if (UserManager.CurrentUser.Role == Role.Guest)
            {
                AdminPanel.Visible = false;
                Login.Visible = true;
                Register.Visible = true;

                LogoutButton.Visible = false;
                UserStatusLabel.Visible = false;
                UserNameLabel.Text = "Küla";
            }
            if (UserManager.CurrentUser.Role != Role.Guest)
            {
                AdminPanel.Visible = UserManager.CurrentUser.Role == Role.Admin;
                Login.Visible = false;
                Register.Visible = false;

                UserNameLabel.Text = UserManager.CurrentUser.UserName;
                UserStatusLabel.Text = $"Roll: {UserManager.CurrentUser.Role.ToString()}";
            }
        }
        private void UserPanel_Click(object sender, EventArgs e)
        {
            if (UserManager.CurrentUser.Role != Role.Guest)
            {
                UserStatusLabel.Visible = !UserStatusLabel.Visible;
                LogoutButton.Visible = !LogoutButton.Visible;
            }
        }

    }
}
