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
using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using System.Drawing;

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
                current_client_id = CreateOrFindClient();
                if (current_client_id != 0)
                {
                    foreach (var seat in SelectedSeats)
                    {
                        ticketPaths.Add(MakeTicketPDF(seat));
                    }
                    TicketsToPanel();
                    sendTickets.Visible = true;
                    makeTickets.Enabled = false;
                }
            }
        }
        private void SendTickets_Click(object sender, EventArgs e) // Loome ostetud piletite kohta andmebaasis kirjed ja saadame piletid meili teel
        {
            if (FieldsNotNull())
            {
                if (AddTicketsToDB())
                {
                    SendEmail();
                }
            }
        }

        private void TicketsToPanel()
        {
            int yOffset = 5;
            foreach (var ticketPath in ticketPaths)
            {
                Button ticketButton = new Button()
                {
                    Text = Path.GetFileName(ticketPath),
                    AutoSize = false,
                    Size = new System.Drawing.Size(pnlPiletid.Width, 30),
                    Location = new Point(0, yOffset),
                    Tag = ticketPath,
                    BackColor = System.Drawing.Color.LightYellow,
                    TextAlign = ContentAlignment.MiddleCenter,
                    FlatStyle = FlatStyle.Flat
                };

                ticketButton.Click += (sender, e) =>
                {
                    string path = (sender as Button)?.Tag.ToString();
                    if (!string.IsNullOrEmpty(path) && File.Exists(path))
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = path,
                            UseShellExecute = true
                        });
                    }
                    else
                    {
                        MessageBox.Show("Ticket file not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };

                pnlPiletid.Controls.Add(ticketButton);
                yOffset += 29;
            }
        }

        private bool AddTicketsToDB()
        {
            Console.WriteLine("Seats selected "+SelectedSeats.Count);
            List<Ticket> tickets = new List<Ticket>();
            foreach (var seat in SelectedSeats)
            {
                Ticket ticket = new Ticket
                {
                    pileti_nimi = $"Pilet {seat.rida}-{seat.koht_ridades}",
                    klient_id = current_client_id, 
                    seanss_id = Session.seanss_id,
                    koht_id = seat.koht_id,
                    hind = GetSelectedPrice(priceComboBox)
                };
                tickets.Add(ticket);
            }
            if (ticketManager.AddTicketsToDatabase(tickets))
            {
                Console.WriteLine("All tickets added");
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

        private string MakeTicketPDF(Koht seat)
        {
            string ticketFileName = $"{DB_PATHS.TicketsFolder}/Pilet_Seanss_{Session.seanss_id}_Koht_{seat.koht_id}.pdf";
            string posterlocation = Path.Combine(DB_PATHS.PosterFolder, Session.film.poster);
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.Content().Row(row =>
                    {
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
                            column.Item().Text($"Nimi: {klient.Nimi}").FontSize(12);
                            column.Item().Text($"Email: {klient.Email}").FontSize(12);
                            column.Item().Text($"Telefon: {klient.Telefon}").FontSize(12);
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
            return ticketFileName;
        }

        private void SendEmail()
        {
            try
            {
                using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtpClient.Credentials = new NetworkCredential("anton9032@gmail.com", "smd");
                    smtpClient.EnableSsl = true;
                    using (MailMessage mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress("anton9032@gmail.com");
                        mailMessage.To.Add(klient.Email);
                        mailMessage.Subject = $"Piletid filmile '{Session.film.pealkiri}'";
                        mailMessage.Body = $@"
                        <html>
                        <body>
                            <h1>Täname Teid ostu eest!</h1>
                            <p>Kinnitatud piletid filmile:</p>
                            <h2>{Session.film.pealkiri}</h2>
                            <ul>
                                <li><strong>Kuupäev:</strong> {Session.kuupaev.ToShortDateString()}</li>
                                <li><strong>Algusaeg:</strong> {Session.alus_aeg:HH:mm}</li>
                                <li><strong>Seansi lõpp:</strong> {Session.lopp_aeg:HH:mm}</li>
                                <li><strong>Saal:</strong> {Session.saal.saali_nimi}</li>
                            </ul>
                            <p>Piletid on lisatud sellele meilile. Kui teil on küsimusi, võtke meiega ühendust.</p>
                            <p>Lugupidamisega,<br>Teie kino meeskond</p>
                        </body>
                        </html>";
                        mailMessage.IsBodyHtml = true;

                        foreach (string pilet in ticketPaths)
                        {
                            mailMessage.Attachments.Add(new Attachment(pilet));
                        }

                        smtpClient.Send(mailMessage);
                        MessageBox.Show("Письмо успешно отправлено.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (SmtpException smtpEx)
            {
                MessageBox.Show($"Ошибка SMTP: {smtpEx.StatusCode}\n{smtpEx.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Общая ошибка: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void UuendaAndmed_Click(object sender, EventArgs e)
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
                uuendaAndmed.Visible = false;
            }
            if (UserManager.CurrentUser.Role != Role.Guest)
            {
                uuendaAndmed.Visible = true;
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
