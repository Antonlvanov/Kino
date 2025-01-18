using Kino.UserControl;
using Kino.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Kino.Forms.Saal;
using static Kino.Forms.Sessions.SessionsForm;

namespace Kino.Forms.Sessions
{
    public partial class SessionsForm : Form
    {
        public SessionsForm(UserManager userManager) 
        {
            UserManager = userManager;
            dbHelper = new dbHelper();
            this.Text = "Kino kava";
            this.Size = new Size(700, 900);
            InitializeComponent();
            UpdateFormForUser();
        }

        private void BtnPrevDay_Click(object sender, EventArgs e)
        {
            currentDate = currentDate.Date.AddDays(-1);
            lblDate.Text = currentDate.ToString("dd-MM-yyyy");
            LoadSessionsForDate(currentDate);
        }

        private void BtnNextDay_Click(object sender, EventArgs e)
        {
            currentDate = currentDate.Date.AddDays(1);
            lblDate.Text = currentDate.ToString("dd-MM-yyyy");
            LoadSessionsForDate(currentDate);
        }
        private void CreateSeanss_Click(object sender, EventArgs e)
        {
            AddSessionForm addSessionForm = new AddSessionForm();
            addSessionForm.ShowDialog();
        }


        public void UpdateFormForUser()
        {
            if (UserManager.CurrentUser.Role == Role.Guest)
            {
                UserStatusLabel.Visible = false;
                UserNameLabel.Text = "Küla";
            }
            if (UserManager.CurrentUser.Role != Role.Guest)
            {
                UserNameLabel.Text = UserManager.CurrentUser.UserName;
                UserStatusLabel.Text = $"Roll: {UserManager.CurrentUser.Role.ToString()}";
            }
        }

        private void LoadSessionsForDate(DateTime date)
        {
            var sessions = GetSessionsForDate(date);

            pnlSessions.Controls.Clear();

            int currentYPosition = 10; 

            foreach (var session in sessions)
            {
                var sessionPanel = CreateSessionPanel(session, currentYPosition);
                pnlSessions.Controls.Add(sessionPanel);

                currentYPosition += sessionPanel.Height + 10;  
            }
        }

        private Panel CreateSessionPanel(Session session, int topPosition)
        {
            var sessionPanel = new Panel
            {
                Size = new Size(pnlSessions.Width - 40, pnlSessions.Height / 2 - 30), 
                Location = new Point(10, topPosition),
                Margin = new Padding(10),
                BackColor = Color.White,
                Padding = new Padding(10),
                BorderStyle = BorderStyle.FixedSingle
            };

            var pictureBox = new PictureBox
            {
                ImageLocation = Path.Combine(DB_PATHS.PosterFolder, session.film.poster),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Height = sessionPanel.Height - 20,
                Location = new Point(10, 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            pictureBox.Width = (pictureBox.Height / 40) * 27;

            var infoPanel = new Panel
            {
                Height = sessionPanel.Height - 20,
                Width = sessionPanel.Width - pictureBox.Width - 30,
                Location = new Point(pictureBox.Width + 20, 10),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10),
                AutoScroll = true
            };

            var filmi_nimi = new Label
            {
                Text = session.film.pealkiri.ToUpper(),
                Font = new Font("Segoe UI", 14F, FontStyle.Regular),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Width = infoPanel.Width - 20,
                Height = 30,
            };

            var zanr = CreateLabel("Zhanr: " + session.film.zanr);
            var kestlus = CreateLabel("Kestlus: " + session.alus_aeg.ToString("HH:mm") + " - " + session.lopp_aeg.ToString("HH:mm"));

            var kirjeldus = new Label
            {
                Text = "Kirjeldus: " + session.film.kirjeldus,
                Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                AutoSize = false,
                TextAlign = ContentAlignment.TopLeft,
                Width = infoPanel.Width - 20,
                MaximumSize = new Size(infoPanel.Width - 20, 0),
                Height = 60,
            };

            Button btnBuy = new Button
            {
                Text = "Osta",
                Dock = DockStyle.Bottom,
                Margin = new Padding(5),
                BackColor = Color.LightYellow,
                FlatStyle = FlatStyle.Flat,
                TabStop = false,
            };
            btnBuy.Click += (s, e) => BuyTicket(session);

            filmi_nimi.Top = 10;
            zanr.Top = filmi_nimi.Bottom + 5;
            kestlus.Top = zanr.Bottom + 5;
            kirjeldus.Top = kestlus.Bottom + 5;

            infoPanel.Controls.Add(filmi_nimi);
            infoPanel.Controls.Add(zanr);
            infoPanel.Controls.Add(kestlus);
            infoPanel.Controls.Add(kirjeldus);
            infoPanel.Controls.Add(btnBuy);

            sessionPanel.Controls.Add(pictureBox);
            sessionPanel.Controls.Add(infoPanel);

            return sessionPanel;
        }

        private List<Session> GetSessionsForDate(DateTime date)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@Date", date.ToString("yyyy-MM-dd") } 
            };
            DataTable resultTable = dbHelper.ExecuteQuery(Queries.GetSessionsDataForDate(), parameters);

            List<Session> sessions = new List<Session>();
            foreach (DataRow row in resultTable.Rows)
            {
                Session session = new Session
                {
                    seanss_id = Convert.ToInt32(row["seanss_id"]),
                    seansi_nimi = row["seansi_nimi"].ToString(),
                    kuupaev = DateTime.Parse(row["kuupaev"].ToString()),
                    alus_aeg = DateTime.Parse(row["alus_aeg"].ToString()),
                    lopp_aeg = DateTime.Parse(row["lopp_aeg"].ToString()),
                    film = new Film
                    {
                        film_id = row["film_id"].ToString(),
                        pealkiri = row["pealkiri"].ToString(),
                        poster = row["poster"].ToString(),
                        zanr = row["zanr"].ToString(),
                        kestus = row["kestus"].ToString(),
                        kirjeldus = row["kirjeldus"].ToString()
                    },
                    saal = new Saal
                    {
                        saal_id = row["saal_id"].ToString(),
                        saali_nimi = row["saali_nimi"].ToString(),
                        ridade_arv = row["ridade_arv"].ToString(),
                        kohad_ridades = row["kohad_ridades"].ToString()
                    }
                };
                sessions.Add(session);
            }
            return sessions;
        }

        private void BuyTicket(Session session)
        {
            SaalForm saalForm = new SaalForm(UserManager, session);
            saalForm.Show();
        }

        public class Session
        {
            public int seanss_id {  get; set; }
            public string seansi_nimi { get; set; }
            public Film film { get; set; }
            public Saal saal { get; set; }
            public DateTime kuupaev { get; set; }
            public DateTime alus_aeg { get; set; }
            public DateTime lopp_aeg { get; set; }
        }
        public class Film
        {
            public string film_id { get; set; }
            public string pealkiri { get; set; }
            public string poster { get; set; }
            public string zanr { get; set; }
            public string kestus { get; set; }
            public string kirjeldus { get; set; }
        }
        public class Saal
        {
            public string saal_id { get; set; }
            public string saali_nimi { get; set; }
            public string ridade_arv { get; set; }
            public string kohad_ridades { get; set; }
        }
    }
}
