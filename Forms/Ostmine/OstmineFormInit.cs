using Kino.ClientControl;
using Kino.Database;
using Kino.TicketControl;
using Kino.UserControl;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Kino.Forms.Saal.SaalForm;
using static Kino.Forms.Sessions.SessionsForm;
using static Kino.UserControl.UserManager;

namespace Kino.Forms.Ostmine
{
    partial class OstmineForm : Form
    {
        private dbHelper dbHelper;
        private Session Session;
        private UserManager UserManager;
        private Client klient;
        private ClientManager clientManager = new ClientManager();
        private TicketManager ticketManager = new TicketManager();
        private List<Koht> SelectedSeats;
        List<string> ticketPaths = new List<string>();
        int current_client_id;
        private SmtpClient smtpClient;
        private MailMessage mailMessage;

        private Label nimiLabel;
        private TextBox nimiInput;
        private Label emailLabel;
        private TextBox emailInput;
        private Label phoneLabel;
        private TextBox phoneInput;

        private Label priceLabel;
        private ComboBox priceComboBox;

        private Button makeTickets;
        private Button sendTickets;
        private Button uuendaAndmed;

        private Label piletidLabel;
        private Panel pnlPiletid;

        private Panel UserPanel;
        private PictureBox UserIcon;
        private Label UserNameLabel;
        private Label UserStatusLabel;

        private void InitializeComponent()
        {
            this.Text = "Ostmine";
            this.Size = new Size(420, 350);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            InitUserPanel();
            nimiLabel = CreateLabel("Täisnimi: ", new Point(UserPanel.Left, UserPanel.Bottom+15));
            emailLabel = CreateLabel("Email: ", new Point(UserPanel.Left, nimiLabel.Bottom+10));
            phoneLabel = CreateLabel("Telefon: ", new Point(UserPanel.Left, emailLabel.Bottom+10));
            nimiInput = CreateTextbox("", new Point(nimiLabel.Width-10, nimiLabel.Top));
            emailInput = CreateTextbox("", new Point(nimiLabel.Width-10, emailLabel.Top));
            phoneInput = CreateTextbox("", new Point(nimiLabel.Width-10, phoneLabel.Top));
            nimiInput.BackColor = Color.LightYellow;
            emailInput.BackColor = Color.LightYellow;
            phoneInput.BackColor = Color.LightYellow;

            pnlPiletid = new Panel()
            {
                AutoScroll = false,
                BorderStyle = BorderStyle.None,
                BackColor = Color.LightGray
            };
            pnlPiletid.Size = new Size(Width/2-35, Height-nimiLabel.Top-50);
            pnlPiletid.Location = new Point(Width/2, nimiLabel.Top);

            piletidLabel = CreateLabel("Piletid", new Point(pnlPiletid.Left, UserNameLabel.Bottom));
            piletidLabel.AutoSize = false;
            piletidLabel.Width = pnlPiletid.Width;
            piletidLabel.Location = new Point(pnlPiletid.Left, pnlPiletid.Top-piletidLabel.Height);
            piletidLabel.BorderStyle = BorderStyle.FixedSingle;
            piletidLabel.TextAlign = ContentAlignment.MiddleCenter;
            piletidLabel.BackColor = Color.LightYellow;
            piletidLabel.FlatStyle = FlatStyle.Flat;

            priceLabel = CreateLabel("Hind: ", new Point(UserPanel.Left, phoneLabel.Bottom + 10));
            priceComboBox = CreatePriceComboBox(new Point(nimiLabel.Width - 10, phoneInput.Bottom + 7));

            sendTickets = CreateButton("Saata piletid emaili", new Point(12, pnlPiletid.Bottom - 28));
            makeTickets = CreateButton("Luua piletid", new Point(12, sendTickets.Top - sendTickets.Height - 10));
            uuendaAndmed = CreateButton("Uuenda andmed", new Point(12, makeTickets.Top - makeTickets.Height - 10));

            Controls.Add(nimiLabel);
            Controls.Add(nimiInput);
            Controls.Add(emailLabel);
            Controls.Add(emailInput);
            Controls.Add(phoneLabel);
            Controls.Add(phoneInput);

            Controls.Add(priceLabel);
            Controls.Add(priceComboBox);

            Controls.Add(pnlPiletid);
            Controls.Add(piletidLabel);

            Controls.Add(uuendaAndmed);
            Controls.Add(makeTickets);
            Controls.Add(sendTickets);

            uuendaAndmed.Click += UuendaAndmed_Click;
            makeTickets.Click += MakeTickets_Click;
            sendTickets.Click += SendTickets_Click;
            uuendaAndmed.Visible = false;
            sendTickets.Visible = false;
        }

        public void InitUserPanel()
        {
            UserPanel = new Panel
            {
                Size = new Size(100, 40),
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Location = new Point(10, 10),
                BackColor = Color.LightGray
            };
            UserIcon = new PictureBox
            {
                Size = new Size(30, 30),
                Location = new Point(10, 5),
                Image = Image.FromFile(Path.Combine(DB_PATHS.ImageFolder, "usericon.png")),
                SizeMode = PictureBoxSizeMode.Zoom
            };
            UserNameLabel = CreateLabel("", new Point(47, 10));
            UserNameLabel.Font = new Font("Segoe UI", 10F, FontStyle.Regular);

            UserStatusLabel = CreateLabel("", new Point(10, UserPanel.Bottom));
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

        private Button CreateButton(string text, Point location, Font font = null)
        {
            var button = new Button
            {
                Text = text,
                Font = font ?? new Font("Segoe UI", 9F, FontStyle.Regular),
                AutoSize = true,
                Location = location,
                BackColor = Color.LightYellow,
                FlatStyle = FlatStyle.Flat,
            };
            button.PerformLayout();
            return button;
        }

        private Label CreateLabel(string text, Point location)
        {
            var label = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                AutoSize = true,
                Location = location,
                TextAlign = ContentAlignment.MiddleLeft,
                FlatStyle = FlatStyle.Flat,
            };
            label.PerformLayout();
            return label;
        }

        private TextBox CreateTextbox(string text, Point location)
        {
            var textbox = new TextBox
            {
                Text = text,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Regular),
                AutoSize = true,
                Location = location,
            };
            textbox.PerformLayout();
            return textbox;
        }
        private ComboBox CreatePriceComboBox(Point location)
        {
            ComboBox comboBox = new ComboBox
            {
                Location = location,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                BackColor = Color.LightYellow,
                Width = phoneInput.Width,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            comboBox.Items.Add(string.Empty); 
            comboBox.Items.Add("Lastepilet - 7€");
            comboBox.Items.Add("Sooduspilet - 10€");
            comboBox.Items.Add("Täispilet - 12€");
            comboBox.SelectedIndex = 0;
            return comboBox;
        }
        private decimal GetSelectedPrice(ComboBox comboBox)
        {
            if (comboBox.SelectedIndex != 0 && comboBox.SelectedItem != null)
            {
                string selectedText = comboBox.SelectedItem.ToString();
                var match = System.Text.RegularExpressions.Regex.Match(selectedText, @"\d+(\.\d+)?");
                if (match.Success)
                {
                    return decimal.Parse(match.Value);
                }
            }
            return 0; 
        }

    }
}
