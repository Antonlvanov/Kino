using Kino.Database;
using Kino.Forms.Ostmine;
using Kino.UserControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;
using static Kino.Forms.Saal.SaalForm;
using static Kino.Forms.Sessions.SessionsForm;

namespace Kino.Forms.Saal
{
    public partial class SaalForm : Form
    {
        public SaalForm(UserManager userManager, Session session)
        {
            Session = session;
            dbHelper = new dbHelper();
            UserManager = userManager;
            InitializeComponents();
            LoadSeatsFromDatabase();
            CreateSeats();
            AdjustSize();
            SelectTwoSeatsNearCenter();
        }

        public void CreateSeats()
        {
            pnlSeats.Controls.Clear();
            foreach (Koht koht in kohad)
            {
                Button seatButton = new Button()
                {
                    BackgroundImage = koht.broneeritud ? Image.FromFile(Path.Combine(DB_PATHS.ImageFolder, "seattaken.png")): Image.FromFile(Path.Combine(DB_PATHS.ImageFolder, "seat.png")),
                    BackgroundImageLayout = ImageLayout.Stretch,
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(seatWidth, seatHeight),
                    Location = new Point(
                        margin + (koht.koht_ridades - 1) * (seatWidth + margin),
                        margin + (koht.rida - 1) * (seatHeight + margin)
                    ),
                    Tag = koht.koht_id,
                    Name = koht.koht_nimi,
                    Enabled = !koht.broneeritud,
                };
                seatButton.Click += Seat_Click;
                pnlSeats.Controls.Add(seatButton);
            }
        }

        private void LoadSeatsFromDatabase()
        {
            int ridade_arv = Convert.ToInt32(Session.saal.ridade_arv);
            int kohad_ridades = Convert.ToInt32(Session.saal.kohad_ridades);
            string saali_nimi = Session.saal.saali_nimi;
            DataTable kohadTable = dbHelper.ExecuteQuery(Queries.GetSeatsForSeanss(Session.seanss_id));
            foreach (DataRow row in kohadTable.Rows)
            {
                Koht koht = new Koht
                {
                    koht_id = Convert.ToInt32(row["koht_id"]),
                    koht_nimi = row["koht_nimi"].ToString(),
                    seanss_id = Convert.ToInt32(row["seanss_id"]),
                    rida = Convert.ToInt32(row["rida"]),
                    koht_ridades = Convert.ToInt32(row["koht_ridades"]),
                    broneeritud = Convert.ToBoolean(row["broneeritud"])
                };
                kohad.Add(koht);
            }
        }

        private void BtnConfirmSelection_Click(object sender, EventArgs e)
        {
            List<Koht> selectedSeats = new List<Koht>();
            foreach (var seat in seatStatus)
            {
                if (seat.Value == 1)
                {
                    int seatId = (int)seat.Key.Tag;
                    Koht selectedKoht = kohad.FirstOrDefault(k => k.koht_id == seatId);
                    if (selectedKoht != null)
                    {
                        selectedSeats.Add(selectedKoht);
                    }
                }
            }
            if (selectedSeats.Count == 0)
            {
                MessageBox.Show("Valige vähemalt üks asukoht.", "Tähelepanu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            OstmineForm ostmineForm = new OstmineForm(UserManager, Session, selectedSeats);
            ostmineForm.Show();
        }

        private void SelectTwoSeatsNearCenter()
        {
            int rowsCount = Convert.ToInt32(Session.saal.ridade_arv);
            int seatsPerRow = Convert.ToInt32(Session.saal.kohad_ridades);
            int centerRow = rowsCount / 2;
            int centerSeat = seatsPerRow / 2;

            var freeSeats = kohad.Where(k => !k.broneeritud)
                                 .OrderBy(k => Math.Abs(k.rida - centerRow) + Math.Abs(k.koht_ridades - centerSeat))
                                 .ToList();

            for (int i = 0; i < freeSeats.Count - 1; i++)
            {
                Koht firstSeat = freeSeats[i];
                Koht secondSeat = freeSeats[i + 1];

                if (Math.Abs(firstSeat.rida - secondSeat.rida) == 0 && Math.Abs(firstSeat.koht_ridades - secondSeat.koht_ridades) == 1)
                {
                    Button firstSeatButton = pnlSeats.Controls
                        .OfType<Button>()
                        .FirstOrDefault(b => (int)b.Tag == firstSeat.koht_id);
                    Button secondSeatButton = pnlSeats.Controls
                        .OfType<Button>()
                        .FirstOrDefault(b => (int)b.Tag == secondSeat.koht_id);

                    if (firstSeatButton != null && secondSeatButton != null)
                    {
                        firstSeatButton.BackColor = Color.LightYellow;
                        secondSeatButton.BackColor = Color.LightYellow;
                        seatStatus[firstSeatButton] = 1;
                        seatStatus[secondSeatButton] = 1;
                    }
                    break;
                }
            }
        }


        private void Seat_Click(object sender, EventArgs e)
        {
            Button clickedSeat = sender as Button;
            if (clickedSeat == null) return;

            int seatID = (int)clickedSeat.Tag; 

            if (clickedSeat.Enabled == false)
            {
                return;
            }

            if (seatStatus.ContainsKey(clickedSeat) && seatStatus[clickedSeat] == 1)
            {
                seatStatus[clickedSeat] = 0;
                clickedSeat.BackColor = Color.Transparent;
            }
            else
            {
                seatStatus[clickedSeat] = 1; 
                clickedSeat.BackColor = Color.LightYellow;
            }
        }

        public class Koht
        {
            public int koht_id { get; set; }
            public string koht_nimi { get; set; }
            public int seanss_id { get; set; }
            public int rida { get; set; }
            public int koht_ridades { get; set; }
            public bool broneeritud { get; set; }
        }
    }
}
