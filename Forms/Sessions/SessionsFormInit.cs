using Kino.Database;
using Kino.UserControl;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Kino.Forms.Sessions
{
    public partial class SessionsForm : Form
    {
        public DateTime currentDate { get; set; }
        public Button btnPrevDay { get; set; }
        public Button btnNextDay { get; set; }
        public Label lblDate { get; set; }
        public Panel pnlSessions { get; set; }
        public UserManager UserManager { get; set; }
        public dbHelper dbHelper { get; set; }
        private Panel UserPanel { get; set; }
        private PictureBox UserIcon { get; set; }
        private Label UserNameLabel { get; set; }
        private Label UserStatusLabel { get; set; }

        private void InitializeComponent()
        {
            currentDate = DateTime.Today.Date;

            pnlSessions = new Panel()
            {
                BackColor = Color.LightGray,
                AutoScroll = true,
                Location = new Point(17, this.Height / 10),
                Size = new Size(this.Width - 50, (this.Height / 10) * 9 - 50),
                BorderStyle = BorderStyle.FixedSingle,
            };

            btnPrevDay = new Button()
            {
                Size = new Size(50, 50),
                FlatStyle = FlatStyle.Flat,
                BackColor = this.BackColor,
                TabStop = false
            };
            btnPrevDay.FlatAppearance.BorderSize = 0;

            btnNextDay = new Button()
            {
                Size = new Size(50, 50),
                FlatStyle = FlatStyle.Flat,
                BackColor = this.BackColor, 
                TabStop = false
            };
            btnNextDay.FlatAppearance.BorderSize = 0;

            lblDate = new Label()
            {
                Text = currentDate.Date.ToString("dd-MM-yyyy"),
                Size = new Size(200, 50),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 16, FontStyle.Bold),
                BorderStyle = BorderStyle.FixedSingle,
            };

            PositionDateAndButtons();

            InitUserPanel();
            this.Controls.Add(btnPrevDay);
            this.Controls.Add(btnNextDay);
            this.Controls.Add(lblDate);
            this.Controls.Add(pnlSessions);

            LoadSessionsForDate(currentDate);

            btnPrevDay.Paint += (s, e) => DrawTriangle(e.Graphics, btnPrevDay.ClientRectangle, true);
            btnPrevDay.Click += BtnPrevDay_Click;
            btnNextDay.Paint += (s, e) => DrawTriangle(e.Graphics, btnNextDay.ClientRectangle, false);
            btnNextDay.Click += BtnNextDay_Click;
        }

        public void InitUserPanel ()
        {
            UserPanel = new Panel
            {
                Size = new Size(100, 40),
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Location = new Point(17, 20),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.LightGray
            };
            UserIcon = new PictureBox
            {
                Size = new Size(30, 30),
                Location = new Point(12, 5),
                Image = Image.FromFile(Path.Combine(DB_PATHS.ImageFolder, "usericon.png")),
                SizeMode = PictureBoxSizeMode.Zoom
            };
            UserNameLabel = CreateLabel("", new Point(47, 10));
            UserNameLabel.Font = new Font("Segoe UI", 10F, FontStyle.Regular);

            UserStatusLabel = CreateLabel("", new Point(17, 59));
            UserStatusLabel.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            UserStatusLabel.FlatStyle = FlatStyle.Flat;
            UserStatusLabel.BorderStyle = BorderStyle.FixedSingle;
            UserStatusLabel.BackColor = Color.LightGray;
            UserStatusLabel.AutoSize = false;
            UserStatusLabel.Visible = false;

            UserPanel.Controls.Add(UserIcon);
            UserPanel.Controls.Add(UserNameLabel);
            Controls.Add(UserPanel);
            Controls.Add(UserStatusLabel);

            UserIcon.Click += UserPanel_Click;
            UserNameLabel.Click += UserPanel_Click;
            UserPanel.Click += UserPanel_Click;
        }
        private void UserPanel_Click(object sender, EventArgs e)
        {
            if (UserManager.CurrentUser.Role != Role.Guest)
            {
                UserStatusLabel.Visible = !UserStatusLabel.Visible;
            }
        }

        private void PositionDateAndButtons()
        {
            int centerX = this.Width / 2;
            int topMargin = 20;
            lblDate.Location = new Point(centerX - lblDate.Width / 2, topMargin);

            btnPrevDay.Location = new Point(lblDate.Left - btnPrevDay.Width - 10, lblDate.Top + (lblDate.Height - btnPrevDay.Height) / 2);
            btnNextDay.Location = new Point(lblDate.Right + 10, lblDate.Top + (lblDate.Height - btnNextDay.Height) / 2);
        }

        private void DrawTriangle(Graphics g, Rectangle bounds, bool isLeft)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(this.BackColor);

            Point[] trianglePoints;
            if (isLeft)
            {
                trianglePoints = new Point[]
                {
            new Point(bounds.Left, bounds.Top + bounds.Height / 2),
            new Point(bounds.Right, bounds.Top),
            new Point(bounds.Right, bounds.Bottom)
                };
            }
            else
            {
                trianglePoints = new Point[]
                {
            new Point(bounds.Right, bounds.Top + bounds.Height / 2),
            new Point(bounds.Left, bounds.Top),
            new Point(bounds.Left, bounds.Bottom)
                };
            }
            g.FillPolygon(Brushes.Gray, trianglePoints);
        }

        private Label CreateLabel(string text)
        {
            var label = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
            };
            return label;
        }

        private Label CreateLabel(string text, Point location)
        {
            var label = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 20F, FontStyle.Regular),
                AutoSize = true,
                Location = location,
                TextAlign = ContentAlignment.MiddleCenter,
            };
            label.PerformLayout();
            return label;
        }
    }
}
