using Kino.Database;
using Kino.Forms.Admin;
using Kino.Properties;
using Kino.UserControl;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Kino.Forms.Start
{
    public partial class StartForm : Form
    {
        public void Initialize()
        {
            this.ClientSize = new Size(350, 350);
            this.Text = "Kino";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackgroundImage = Image.FromFile(Path.Combine(DB_PATHS.ImageFolder, "StartFormBackground2.png"));
            this.BackgroundImageLayout = ImageLayout.Stretch;
            InitializeComponents();
            LayoutComponents();
        }
        public void InitializeComponents()
        {
            UserPanel = new Panel
            {
                Size = new Size(100, 40),
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Location = new Point(10, 20),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.LightYellow,
            };
            UserIcon = new PictureBox
            {
                Size = new Size(30, 30),
                Location = new Point(5, 5),
                Image = Image.FromFile(Path.Combine(DB_PATHS.ImageFolder, "usericon.png")),
                SizeMode = PictureBoxSizeMode.Zoom
            };
            UserNameLabel = CreateLabel("", new Point(40, 10));
            UserNameLabel.Font = new Font("Segoe UI", 10F, FontStyle.Regular);

            UserPanel.Controls.Add(UserIcon);
            UserPanel.Controls.Add(UserNameLabel);
            Controls.Add(UserPanel);

            UserStatusLabel = CreateLabel("", new Point(10, 59));
            UserStatusLabel.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            UserStatusLabel.BackColor = Color.LightYellow;
            UserStatusLabel.FlatStyle = FlatStyle.Flat;
            UserStatusLabel.BorderStyle = BorderStyle.FixedSingle;
            UserStatusLabel.AutoSize = false;
            UserStatusLabel.Visible = false;

            LogoutButton = CreateButton("Loogi välja", new Point(10, 79));
            LogoutButton.Font = new Font("Segoe UI", 9.25F, FontStyle.Regular);
            LogoutButton.Visible = false;
            UserStatusLabel.Width = LogoutButton.Width+8;

            Controls.Add(UserStatusLabel);
            Controls.Add(LogoutButton);

            Kino = CreateLabel("Kino", new Point(0, 0));
            Kino.BackColor = Color.Transparent;
            Kino.FlatStyle = FlatStyle.Flat; 
            Sessions = CreateButton("Filmi Kava", new Point(0, 0), new Font("Segoe UI", 15F, FontStyle.Regular));
            AdminPanel = CreateButton("Haaldamine", new Point(0, 0), new Font("Segoe UI", 15F, FontStyle.Regular));
            Login = CreateButton("Sisse loogi", new Point(0, 0));
            Register = CreateButton("Registreeri", new Point(0, 0));

            Controls.Add(UserPanel);
            Controls.Add(Kino);
            Controls.Add(Sessions);
            Controls.Add(AdminPanel);
            Controls.Add(Login);
            Controls.Add(Register);

            UserIcon.Click += UserPanel_Click;
            UserNameLabel.Click += UserPanel_Click;

            Login.Click += Login_Click;
            Register.Click += Register_Click;
            AdminPanel.Click += AdminPanel_Click;
            LogoutButton.Click += LogoutButton_Click;
            Sessions.Click += Sessions_Click;
        }
    }
}
