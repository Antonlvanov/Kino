using Kino.UserControl;
using Microsoft.Win32;
using System.Drawing;
using System.Windows.Forms;

namespace Kino.Forms.Login
{
    public partial class LoginForm : Form
    {
        public Label Title { get; set; }
        public Label UsernameLabel { get; set; }
        public TextBox UsernameInput { get; set; }
        public Label PasswordLabel { get; set; }
        public TextBox PasswordInput { get; set; }
        public Button LoginButton { get; set; }

        public UserManager UserManager { get; set; }

        public void InitializeComponents()
        {
            // Create controls
            Title = CreateLabel("Logi Sisse", new Point(0, 0), new Font("Segoe UI", 18F, FontStyle.Regular));
            UsernameLabel = CreateLabel("Login:", new Point(0, 0));
            UsernameInput = CreateTextBox(new Point(0, 0));
            PasswordLabel = CreateLabel("Parool:", new Point(0, 0));
            PasswordInput = CreatePasswordTextBox(new Point(0, 0));
            LoginButton = CreateButton("Sisend", new Point(0, 0));

            // Add controls to form
            Controls.Add(Title);
            Controls.Add(UsernameLabel);
            Controls.Add(UsernameInput);
            Controls.Add(PasswordLabel);
            Controls.Add(PasswordInput);
            Controls.Add(LoginButton);

            LoginButton.Click += LoginButton_Click;
        }

        public void LayoutComponents()
        {
            int formWidth = this.ClientSize.Width;
            int currentY = 20; 
            int spacing = 20; 

            Title.Location = new Point((formWidth - Title.Width) / 2, currentY);
            currentY += Title.Height + spacing * 2;

            int totalWidth = UsernameLabel.Width + UsernameInput.Width + spacing;
            int startX = (formWidth - totalWidth) / 2;

            UsernameLabel.Location = new Point(startX, currentY);
            UsernameInput.Location = new Point(startX + UsernameLabel.Width + spacing, currentY);
            currentY += UsernameLabel.Height + spacing;

            totalWidth = UsernameLabel.Width + UsernameInput.Width + spacing;
            startX = (formWidth - totalWidth) / 2;
            PasswordLabel.Location = new Point(startX, currentY);
            PasswordInput.Location = new Point(startX + UsernameLabel.Width + spacing, currentY);
            currentY += PasswordLabel.Height + spacing * 2;

            LoginButton.Location = new Point((formWidth - LoginButton.Width) / 2, currentY);
            currentY += LoginButton.Height + spacing;
        }

        private Label CreateLabel(string text, Point location, Font font = null)
        {
            return new Label
            {
                Text = text,
                Location = location,
                Font = font ?? new Font("Segoe UI", 12F, FontStyle.Regular),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
            };
        }

        private TextBox CreateTextBox(Point location)
        {
            return new TextBox
            {
                Location = location,
                Width = 100,
                Font = new Font("Segoe UI", 12F, FontStyle.Regular),
            };
        }

        private TextBox CreatePasswordTextBox(Point location)
        {
            var textBox = CreateTextBox(location);
            textBox.UseSystemPasswordChar = true;
            return textBox;
        }

        private Button CreateButton(string text, Point location)
        {
            return new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                AutoSize = true,
                Location = location,
                BackColor = Color.White,
            };
        }
    }
}
