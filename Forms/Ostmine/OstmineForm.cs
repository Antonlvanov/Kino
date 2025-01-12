using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout;
using iText.Layout.Properties;
using Kino.UserControl;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using static Kino.Forms.Saal.SaalForm;
using static Kino.Forms.Sessions.SessionsForm;
using static Kino.UserControl.UserManager;
using Kino.Database;
using Kino.TicketControl;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Kino.Forms.Ostmine
{
    public partial class OstmineForm : Form
    {
        public OstmineForm(UserManager userManager, Session session, List<Koht> selectedSeats)
        {
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

                    }
                    if (AddTicketsToDB(klient_id))
                    {
                        
                    }
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


        //private void MakeTicketPDF(Koht seat)
        //{
        //    string ticketFileName = $"{DB_PATHS.TicketsFolder}/Ticket_Seanss_{Session.seanss_id}_Koht_{seat.koht_id}.pdf";

        //    using (var writer = new iText.Kernel.Pdf.PdfWriter(ticketFileName))
        //    {
        //        using (var pdf = new iText.Layout.Document(new iText.Kernel.Pdf.PdfDocument(writer)))
        //        {
        //            // Шрифты и стили
        //            var font = iText.IO.Font.Constants.StandardFonts.HELVETICA;
        //            var normalFont = iText.Kernel.Font.PdfFontFactory.CreateFont(font);

        //            // Таблица для информации
        //            var table = new iText.Layout.Element.Table(2).UseAllAvailableWidth();
        //            table.AddCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph("Film:").SetFont(normalFont).SetBold()));
        //            table.AddCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph(Session.film.pealkiri).SetFont(normalFont)));
        //            table.AddCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph("Seansi kuupäev:").SetFont(normalFont).SetBold()));
        //            table.AddCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph(Session.kuupaev.ToShortDateString()).SetFont(normalFont)));
        //            table.AddCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph("Seansi aeg:").SetFont(normalFont).SetBold()));
        //            table.AddCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph($"{Session.alus_aeg:HH:mm} - {Session.lopp_aeg:HH:mm}").SetFont(normalFont)));
        //            table.AddCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph("Saal:").SetFont(normalFont).SetBold()));
        //            table.AddCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph(Session.saal.saali_nimi).SetFont(normalFont)));
        //            table.AddCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph("Koht:").SetFont(normalFont).SetBold()));
        //            table.AddCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph($"{seat.rida}, {seat.koht_ridades}").SetFont(normalFont)));
        //            table.AddCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph("Hind:").SetFont(normalFont).SetBold()));
        //            table.AddCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph("10€").SetFont(normalFont))); // пример цены

        //            pdf.Add(table);

        //            if (!string.IsNullOrEmpty(Session.film.poster))
        //            {
        //                try
        //                {
        //                    var image = new iText.Layout.Element.Image(iText.IO.Image.ImageDataFactory.Create(Session.film.poster));
        //                    image.SetWidth(200);
        //                    pdf.Add(image);
        //                }
        //                catch (Exception ex)
        //                {
        //                    MessageBox.Show($"Не удалось загрузить постер фильма: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                }
        //            }

        //            pdf.Add(new iText.Layout.Element.Paragraph("\n"));
        //            pdf.Add(new iText.Layout.Element.Paragraph("Head vaatamist!").SetFont(normalFont).SetBold());
        //        }
        //    }

        //    MessageBox.Show($"Билет для места {seat.koht_nimi} успешно создан: {ticketFileName}", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //}

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
