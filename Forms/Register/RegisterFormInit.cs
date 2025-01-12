using Kino.ClientControl;
using Kino.UserControl;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Kino.Forms.Register
{
    public partial class RegisterForm : Form
    {
        public Label Title { get; set; }
        public Label UsernameLabel { get; set; }
        public TextBox UsernameInput { get; set; }
        public Label EmailLabel { get; set; }
        public TextBox EmailInput { get; set; }
        public Label PasswordLabel { get; set; }
        public TextBox PasswordInput { get; set; }
        public Label ConfirmPasswordLabel { get; set; }
        public TextBox ConfirmPasswordInput { get; set; }
        public Button RegisterButton { get; set; }

        public CheckBox ClientCheckBox { get; set; }
        public Label ClientNameLabel { get; set; }
        public TextBox ClientNameInput { get; set; }
        public Label ClientPhoneLabel { get; set; }
        public TextBox ClientPhoneInput { get; set; }

        public UserManager UserManager { get; set; }
        public ClientManager clientManager = new ClientManager();

        public void Initialize()
        {
            this.ClientSize = new Size(400, 470);
            this.Text = "Register";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            InitializeComponents();
            LayoutComponents();
        }

        public void InitializeComponents()
        {
            Title = CreateLabel("Registreerimine", new Point(0, 0), new Font("Segoe UI", 18F, FontStyle.Regular));
            UsernameLabel = CreateLabel("Login:", new Point(0, 0));
            UsernameInput = CreateTextBox(new Point(0, 0));
            EmailLabel = CreateLabel("Email:", new Point(0, 0));
            EmailInput = CreateTextBox(new Point(0, 0));
            PasswordLabel = CreateLabel("Parool:", new Point(0, 0));
            PasswordInput = CreatePasswordTextBox(new Point(0, 0));
            ConfirmPasswordLabel = CreateLabel("Kinnita parool:", new Point(0, 0));
            ConfirmPasswordInput = CreatePasswordTextBox(new Point(0, 0));
            RegisterButton = CreateButton("Registreeri", new Point(0, 0));

            ClientCheckBox = new CheckBox
            {
                Text = "Klient",
                Location = new Point(0, 0),
                Font = new Font("Segoe UI", 12F),
                AutoSize = true
            };
            ClientCheckBox.CheckedChanged += ClientCheckBox_CheckedChanged;

            ClientNameLabel = CreateLabel("Täisnimi:", new Point(0, 0));
            ClientNameInput = CreateTextBox(new Point(0, 0));
            ClientPhoneLabel = CreateLabel("Telefon:", new Point(0, 0));
            ClientPhoneInput = CreateTextBox(new Point(0, 0));

            Controls.Add(Title);
            Controls.Add(UsernameLabel);
            Controls.Add(UsernameInput);
            Controls.Add(EmailLabel);
            Controls.Add(EmailInput);
            Controls.Add(PasswordLabel);
            Controls.Add(PasswordInput);
            Controls.Add(ConfirmPasswordLabel);
            Controls.Add(ConfirmPasswordInput);
            Controls.Add(ClientCheckBox);
            Controls.Add(RegisterButton);

            Controls.Add(ClientNameLabel);
            Controls.Add(ClientNameInput);
            Controls.Add(ClientPhoneLabel);
            Controls.Add(ClientPhoneInput);

            ToggleClientFields(false);
            RegisterButton.Click += RegisterButton_Click;
        }

        public void LayoutComponents()
        {
            int formWidth = this.ClientSize.Width;
            int currentY = 20;
            int spacing = 20;

            Title.Location = new Point((formWidth - Title.Width) / 2, currentY);
            currentY += Title.Height + spacing * 2;

            int labelWidth = 120;
            int inputWidth = 150;
            int totalWidth = labelWidth + spacing + inputWidth;
            int startX = (formWidth - totalWidth) / 2;

            UsernameLabel.Location = new Point(startX, currentY);
            UsernameInput.Location = new Point(startX + labelWidth + spacing, currentY);
            currentY += UsernameLabel.Height + spacing;

            EmailLabel.Location = new Point(startX, currentY);
            EmailInput.Location = new Point(startX + labelWidth + spacing, currentY);
            currentY += EmailLabel.Height + spacing;

            PasswordLabel.Location = new Point(startX, currentY);
            PasswordInput.Location = new Point(startX + labelWidth + spacing, currentY);
            currentY += PasswordLabel.Height + spacing;

            ConfirmPasswordLabel.Location = new Point(startX, currentY);
            ConfirmPasswordInput.Location = new Point(startX + labelWidth + spacing, currentY);
            currentY += ConfirmPasswordLabel.Height + spacing;

            ClientCheckBox.Location = new Point((formWidth - ClientCheckBox.Width) / 2, currentY);
            currentY += ClientCheckBox.Height + spacing;

            ClientNameLabel.Location = new Point(startX, currentY);
            ClientNameInput.Location = new Point(startX + labelWidth + spacing, currentY);
            currentY += ClientNameLabel.Height + spacing;

            ClientPhoneLabel.Location = new Point(startX, currentY);
            ClientPhoneInput.Location = new Point(startX + labelWidth + spacing, currentY);
            currentY += ClientPhoneLabel.Height + spacing*2;

            RegisterButton.Location = new Point((formWidth - RegisterButton.Width) / 2, currentY);
        }

        private void ClientCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ToggleClientFields(ClientCheckBox.Checked);
        }

        private void ToggleClientFields(bool isVisible)
        {
            ClientNameLabel.Visible = isVisible;
            ClientNameInput.Visible = isVisible;
            ClientPhoneLabel.Visible = isVisible;
            ClientPhoneInput.Visible = isVisible;
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
                Width = 150,
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
