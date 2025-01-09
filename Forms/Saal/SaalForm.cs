using Kino.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;
using static Kino.Forms.Saal.SaalForm;
using static Kino.Forms.Sessions.SessionsForm;

namespace Kino.Forms.Saal
{
    public partial class SaalForm : Form
    {
        public SaalForm(Session session)
        {
            InitializeComponents();
            Session = session;
            dbHelper = new dbHelper();
            LoadSeatsFromDatabase();
            CreateSeats();
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

        public void CreateSeats()
        {
            int buttonSize = 50;
            int margin = 5;

            foreach (Koht koht in kohad)
            {
                Button seatButton = new Button()
                {
                    Size = new Size(buttonSize, buttonSize),
                    Location = new Point((koht.koht_ridades - 1) * (buttonSize + margin), (koht.rida - 1) * (buttonSize + margin)),
                    BackColor = koht.broneeritud == false ? Color.LightGreen : Color.Gray,
                    Tag = koht.koht_id,
                    Name = koht.koht_nimi,
                    Enabled = koht.broneeritud == false ? false : true,
                };
                seatButton.Click += Seat_Click;
                pnlSeats.Controls.Add(seatButton);
            }
        }
        private void Seat_Click(object sender, EventArgs e)
        {
            Button clickedSeat = sender as Button;
            if (clickedSeat == null) return;

            int seatID = (int)clickedSeat.Tag; 

            if (clickedSeat.BackColor == Color.Gray)
            {
                return;
            }

            // Переключение выделения
            if (seatStatus.ContainsKey(clickedSeat) && seatStatus[clickedSeat] == 1)
            {
                seatStatus[clickedSeat] = 0; // Снимаем выделение
                clickedSeat.BackColor = Color.LightGreen;
            }
            else
            {
                seatStatus[clickedSeat] = 1; // Устанавливаем выделение
                clickedSeat.BackColor = Color.Blue; // Цвет выделенного места
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
