using Kino.Database;
using Kino.UserControl;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        private void InitializeComponent()
        {
            currentDate = DateTime.Today.Date;

            pnlSessions = new Panel()
            {
                BackColor = Color.LightGray,
                AutoScroll = true,
                Location = new Point(17, this.Height / 8),
                Size = new Size(this.Width - 50, (this.Height / 8) * 7 - 50),
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

            this.Controls.Add(btnPrevDay);
            this.Controls.Add(btnNextDay);
            this.Controls.Add(lblDate);
            this.Controls.Add(pnlSessions);

            LoadSessionsForDate(currentDate);

            this.Resize += (s, e) =>
            {
                pnlSessions.Size = new Size(this.Width - 50, (this.Height / 7) * 6 - 50);
                pnlSessions.Location = new Point(15, this.Height / 7);
                //sessionPanel.Size = new Size(pnlSessions.Width, pnlSessions.Height/2);
                PositionDateAndButtons(); 
            };

            btnPrevDay.Paint += (s, e) => DrawTriangle(e.Graphics, btnPrevDay.ClientRectangle, true);
            btnPrevDay.Click += BtnPrevDay_Click;
            btnNextDay.Paint += (s, e) => DrawTriangle(e.Graphics, btnNextDay.ClientRectangle, false);
            btnNextDay.Click += BtnNextDay_Click;
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
    }
}
