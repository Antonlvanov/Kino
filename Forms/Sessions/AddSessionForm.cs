using Kino.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static Kino.Forms.Saal.SaalForm;
using static Kino.Forms.Sessions.SessionsForm;

namespace Kino.Forms.Sessions
{
    public partial class AddSessionForm : Form
    {
        private dbHelper dbHelper;
        private List<Film> films;
        private List<SessionsForm.Saal> saalid;

        public AddSessionForm()
        {
            InitializeComponent();
            dbHelper = new dbHelper();
            LoadFilms();
            LoadSaals();
        }

        private void InitializeComponent()
        {
            this.Text = "Lisa uus seanss";
            this.Size = new Size(400, 270);

            Font labelFont = new Font("Segoe UI", 13F, FontStyle.Regular);
            Font inputFont = new Font("Segoe UI", 11F, FontStyle.Regular);

            Label lblFilm = new Label { Text = "Film:", Location = new Point(10, 20), AutoSize = true, Font = labelFont };
            ComboBox cbFilm = new ComboBox { Name = "cbFilm", Location = new Point(100, 20), Width = 250, Font = inputFont };
            Label lblSaal = new Label { Text = "Saal:", Location = new Point(10, 60), AutoSize = true, Font = labelFont };
            ComboBox cbSaal = new ComboBox { Name = "cbSaal", Location = new Point(100, 60), Width = 70, Font = inputFont };
            Label lblDate = new Label { Text = "Kuupäev:", Location = new Point(10, 100), AutoSize = true, Font = labelFont };
            DateTimePicker dtpDate = new DateTimePicker { Name = "dtpDate", Location = new Point(100, 100), Width = 125, Format = DateTimePickerFormat.Custom, CustomFormat = "dd-MM-yyyy", Font = inputFont };
            Label lblStartTime = new Label { Text = "Kellaeg:", Location = new Point(10, 140), AutoSize = true, Font = labelFont };
            DateTimePicker dtpStartTime = new DateTimePicker { Name = "dtpStartTime", Location = new Point(100, 140), Width = 125, Format = DateTimePickerFormat.Custom, CustomFormat = "HH:mm", Font = inputFont };
            Button btnAddSession = new Button { Text = "Lisa seanss", Font = labelFont };
            btnAddSession.Location = new Point(ClientSize.Width/2-btnAddSession.Width/2, 180);
            btnAddSession.Height = 33;

            btnAddSession.Click += BtnAddSession_Click;

            this.Controls.Add(lblFilm);
            this.Controls.Add(cbFilm);
            this.Controls.Add(lblSaal);
            this.Controls.Add(cbSaal);
            this.Controls.Add(lblDate);
            this.Controls.Add(dtpDate);
            this.Controls.Add(lblStartTime);
            this.Controls.Add(dtpStartTime);
            this.Controls.Add(btnAddSession);
        }

        private void LoadFilms()
        {
            films = new List<Film>();
            string query = "SELECT film_id, pealkiri, kestus FROM filmid";
            DataTable result = dbHelper.ExecuteQuery(query);

            foreach (DataRow row in result.Rows)
            {
                films.Add(new Film
                {
                    film_id = row["film_id"].ToString(),
                    kestus = row["kestus"].ToString(),
                    pealkiri = row["pealkiri"].ToString()
                });
            }

            ComboBox cbFilm = this.Controls["cbFilm"] as ComboBox;
            cbFilm.DataSource = films;
            cbFilm.DisplayMember = "pealkiri";
            cbFilm.ValueMember = "film_id";
        }

        private void LoadSaals()
        {
            saalid = new List<SessionsForm.Saal>();
            string query = "SELECT saal_id, saali_nimi, ridade_arv, kohad_ridades FROM saalid";
            DataTable result = dbHelper.ExecuteQuery(query);

            foreach (DataRow row in result.Rows)
            {
                saalid.Add(new SessionsForm.Saal
                {
                    saal_id = row["saal_id"].ToString(),
                    saali_nimi = row["saali_nimi"].ToString(),
                    ridade_arv = row["ridade_arv"].ToString(),
                    kohad_ridades = row["kohad_ridades"].ToString()
                });
            }

            ComboBox cbSaal = this.Controls["cbSaal"] as ComboBox;
            cbSaal.DataSource = saalid;
            cbSaal.DisplayMember = "saali_nimi";
            cbSaal.ValueMember = "saal_id";
        }

        private void BtnAddSession_Click(object sender, EventArgs e)
        {
            ComboBox cbFilm = this.Controls["cbFilm"] as ComboBox;
            ComboBox cbSaal = this.Controls["cbSaal"] as ComboBox;
            DateTimePicker dtpStartTime = this.Controls["dtpStartTime"] as DateTimePicker;
            DateTimePicker dtpDate = this.Controls["dtpDate"] as DateTimePicker;

            string filmId = cbFilm.SelectedValue.ToString();
            string saalId = cbSaal.SelectedValue.ToString();
            string seansi_nimi = $"{cbFilm.Text} - {cbSaal.Text} - {dtpStartTime.Value}";

            DateTime date = dtpDate.Value;
            DateTime startTime = dtpStartTime.Value;
            DateTime endTime = dtpStartTime.Value.AddMinutes(int.Parse(films.First(f => f.film_id == filmId).kestus));
            startTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, startTime.Hour, startTime.Minute, 0);
            endTime = new DateTime(endTime.Year, endTime.Month, endTime.Day, endTime.Hour, endTime.Minute, 0);

            string query = @"
            INSERT INTO seansid (seansi_nimi, film_id, saal_id, kuupaev, alus_aeg, lopp_aeg)
            OUTPUT INSERTED.seanss_id
            VALUES (@seansi_nimi, @film_id, @saal_id, @kuupaev, @alus_aeg, @lopp_aeg)";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@seansi_nimi", seansi_nimi },
                { "@film_id", filmId },
                { "@saal_id", saalId },
                { "@kuupaev", date },
                { "@alus_aeg", startTime },
                { "@lopp_aeg", endTime }
            };

            int seanssId = dbHelper.GetLastInsertedId(query, parameters);
            if (seanssId > 0)
            {
                CreateSeats(seanssId, saalId);
                MessageBox.Show("Seanss edukalt lisatud!", "Edu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Viga seansi lisamisel.", "Viga", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CreateSeats(int seanssId, string saalId)
        {
            var saal = saalid.First(s => s.saal_id == saalId);
            int ridadeArv = int.Parse(saal.ridade_arv);
            int kohadRidades = int.Parse(saal.kohad_ridades);

            for (int rida = 1; rida <= ridadeArv; rida++)
            {
                for (int koht = 1; koht <= kohadRidades; koht++)
                {
                    string kohtNimi = $"R{rida}-K{koht}";
                    string query = @"
                        INSERT INTO kohad (koht_nimi, seanss_id, rida, koht_ridades, broneeritud)
                        VALUES (@koht_nimi, @seanss_id, @rida, @koht_ridades, @broneeritud)";
                    Dictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        { "@koht_nimi", kohtNimi },
                        { "@seanss_id", seanssId },
                        { "@rida", rida },
                        { "@koht_ridades", koht },
                        { "@broneeritud", false }
                    };
                    dbHelper.ExecuteNonQuery(query, parameters);
                }
            }
        }
    }
}