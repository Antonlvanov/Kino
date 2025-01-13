using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;
using static QuestPDF.Settings;
using Kino.UserControl;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using static Kino.Forms.Saal.SaalForm;
using static Kino.Forms.Sessions.SessionsForm;
using System.IO;
using Kino.Database;
using Kino.TicketControl;

namespace Kino.Forms.Ostmine
{
    public partial class OstmineForm : Form
    {
        public OstmineForm(UserManager userManager, Session session, List<Koht> selectedSeats)
        {
            License = LicenseType.Community;
            UserManager = userManager;
            Session = session;
            SelectedSeats = selectedSeats;
            InitializeComponent();
            UpdateFormForUser();
        }
        private void MakeTickets_Click(object sender, EventArgs e)
        {
            if (FieldsNotNull())
            {
                int klient_id = CreateOrFindClient();
                if (klient_id != 0)
                {
                    foreach (var seat in SelectedSeats)
                    {
                        MakeTicketPDF(seat);
                    }
                    //if (AddTicketsToDB(klient_id))
                    //{
                        
                    //}
                }
            }
        }

        private bool AddTicketsToDB(int klient_id)
        {
            List<Ticket> tickets = new List<Ticket>();
            foreach (var seat in SelectedSeats)
            {
                Ticket ticket = new Ticket
                {
                    pileti_nimi = $"Pilet {seat.rida}-{seat.koht_ridades}",
                    klient_id = klient_id, 
                    seanss_id = Session.seanss_id,
                    koht_id = seat.koht_id,
                    hind = GetSelectedPrice(priceComboBox)
                };
                tickets.Add(ticket);
            }
            if (ticketManager.AddTicketsToDatabase(tickets))
            {
                MessageBox.Show("Piletid edukalt andmebaasi lisatud.", "Edu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            else
            {
                MessageBox.Show("Viga piletite lisamisel andmebaasi.", "Viga", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public int CreateOrFindClient()
        {
            if (UserManager.CurrentUser.Klient_ID != null)
            {
                return Convert.ToInt32(UserManager.CurrentUser.Klient_ID);
            }
            int klient_id = 0;
            string nimi = nimiInput.Text.Trim();
            string email = emailInput.Text.Trim();
            string telefon = phoneInput.Text.Trim();

            int? existingKlientId = clientManager.FindKlientId(nimi, email, telefon);
            if (existingKlientId == null)
            {
                try
                {
                    klient_id = clientManager.CreateKlientRecord(nimi, email, telefon);
                    return klient_id;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Viga kliendikirje loomisel: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        private void MakeTicketPDF(Koht seat)
        {
            string ticketFileName = $"{DB_PATHS.TicketsFolder}/Ticket_Seanss_{Session.seanss_id}_Koht_{seat.koht_id}.pdf";
            string posterlocation = Path.Combine(DB_PATHS.PosterFolder, Session.film.poster);

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);

                    // Разделение контента на два столбца (текст и изображение)
                    page.Content().Row(row =>
                    {
                        // Столбец для текста
                        row.RelativeColumn(1).Column(column =>
                        {
                            column.Item().Text($"Pilet: {seat.koht_nimi}").FontSize(20).Bold();
                            column.Item().Text("");
                            column.Item().Text($"Film: {Session.film.pealkiri}").FontSize(14);
                            column.Item().Text($"Kuupäev: {Session.kuupaev.ToShortDateString()}").FontSize(14);
                            column.Item().Text($"Kellaeg: {Session.alus_aeg:HH:mm} - {Session.lopp_aeg:HH:mm}").FontSize(14);
                            column.Item().Text($"Saal: {Session.saal.saali_nimi}").FontSize(14);
                            column.Item().Text($"Koht: {seat.rida} - {seat.koht_ridades}").FontSize(14);
                            column.Item().Text($"Hind: {GetSelectedPrice(priceComboBox)} €").FontSize(14);
                            column.Item().Text("");
                            column.Item().Text("");
                            column.Item().Text("Ostja andmed:").Bold().FontSize(12);
                            column.Item().Text($"Nimi: {nimiInput.Text}").FontSize(12);
                            column.Item().Text($"Email: {emailInput.Text}").FontSize(12);
                            column.Item().Text($"Telefon: {phoneInput.Text}").FontSize(12);
                        });
                        row.RelativeColumn(1).Column(imageColumn =>
                        {
                            imageColumn.Item().AlignRight().Element(imageContainer =>
                            {
                                imageContainer
                                    .Width(230)
                                    .Height(300)
                                    .Image(posterlocation, ImageScaling.FitArea);
                            });
                        });
                    });
                });
            }).GeneratePdf(ticketFileName);

            MessageBox.Show($"Билет успешно сохранен:", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void SalvestaAndmed_Click(object sender, EventArgs e)
        {
            klient.Nimi = nimiInput.Text;
            klient.Email = emailInput.Text;
            klient.Telefon = phoneInput.Text;
            if (clientManager.UpdateKlientData(Convert.ToInt32(UserManager.CurrentUser.Klient_ID),klient))
            {
                MessageBox.Show("Kliendi andmed uuendatud", "Edu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else {
                MessageBox.Show("Viga andmete värskendamisel", "Viga", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private bool FieldsNotNull()
        {
            if (!string.IsNullOrEmpty(nimiInput.Text.Trim()) && !string.IsNullOrEmpty(nimiInput.Text.Trim())
                && !string.IsNullOrEmpty(nimiInput.Text.Trim()) && priceComboBox.SelectedIndex != 0)
            {
                return true;
            }
            MessageBox.Show("Palun täitke kõik väljad: nimi, email ja telefon.", "Tähelepanu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        public void UpdateFormForUser()
        {
            if (UserManager.CurrentUser.Role == Role.Guest)
            {
                UserStatusLabel.Visible = false;
                UserNameLabel.Text = "Küla";
                salvestaAndmed.Visible = false;
            }
            if (UserManager.CurrentUser.Role != Role.Guest)
            {
                salvestaAndmed.Visible = true;
                UserNameLabel.Text = UserManager.CurrentUser.UserName;
                UserStatusLabel.Text = $"Roll: {UserManager.CurrentUser.Role}";
                if (UserManager.CurrentUser.Role == Role.Klient && UserManager.CurrentUser.Klient_ID!=null)
                {
                    klient = clientManager.GetKlientData(Convert.ToInt32(UserManager.CurrentUser.Klient_ID));
                    nimiInput.Text = klient.Nimi;
                    emailInput.Text = klient.Email;
                    phoneInput.Text = klient.Telefon;
                }
            }
        }


    }
}
