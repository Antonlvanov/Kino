using Kino.Database;
using Kino.UserControl;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using static Kino.Forms.Sessions.SessionsForm;


namespace Kino.Forms.Saal
{
    partial class SaalForm : Form
    {
        private dbHelper dbHelper;
        private Session Session;
        private UserManager UserManager;

        private Panel pnlSeats;
        private Label pealkiriLabel;
        private Label saalLabel;
        private Button btnConfirmSelection;

        private Dictionary<Button, int> seatStatus = new Dictionary<Button, int>();
        List<Koht> kohad = new List<Koht>();

        private int seatWidth;
        private int seatHeight;
        private int ridade_arv;
        private int kohad_ridades;
        private int margin;

        private int panelTopMargin = 100;
        private int panelBotMargin = 50;
        private int panelSideMargin = 50;


        private void InitializeComponents()
        {
            this.Text = "Kino saal";
            this.Size = new Size(1000, 750);
            this.BackgroundImage = Image.FromFile((Path.Combine(DB_PATHS.ImageFolder, "CinemaHall.jpg")));
            this.BackgroundImageLayout = ImageLayout.Stretch;

            pnlSeats = new Panel()
            {
                AutoScroll = false,
                BorderStyle = BorderStyle.None,
                BackColor = Color.Transparent
            };

            margin = 5;
            ridade_arv = Convert.ToInt32(Session.saal.ridade_arv);
            kohad_ridades = Convert.ToInt32(Session.saal.kohad_ridades);

            pealkiriLabel = new Label()
            {
                Text = $"{Session.film.pealkiri}",
                AutoSize = true,
                Font = new Font("Segoe UI", 14F, FontStyle.Regular),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.LightYellow,
                FlatStyle = FlatStyle.Flat,
            };
            saalLabel = new Label()
            {
                Text = $"{Session.saal.saali_nimi}",
                AutoSize = true,
                Font = new Font("Segoe UI", 14F, FontStyle.Regular),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.LightYellow,
                FlatStyle = FlatStyle.Flat,
            };

            btnConfirmSelection = new Button()
            {
                Text = "Osta piletid",
                AutoSize = true,
                Anchor = AnchorStyles.Bottom,
                Font = new Font("Segoe UI", 14F, FontStyle.Regular),
                BackColor = Color.LightYellow,
                FlatStyle = FlatStyle.Flat,
            };

            this.Controls.Add(pnlSeats);
            this.Controls.Add(btnConfirmSelection);
            this.Controls.Add(pealkiriLabel);
            this.Controls.Add(saalLabel);
            AdjustSize();

            btnConfirmSelection.Click += BtnConfirmSelection_Click;
        }
        private void AdjustSize()
        {
            int maxSeatWidth = (ClientSize.Width - margin * (kohad_ridades - 1)) / kohad_ridades;
            int maxSeatHeight = (ClientSize.Height - panelTopMargin - panelBotMargin - margin * (ridade_arv - 1)) / ridade_arv;

            seatWidth = seatHeight = Math.Min(maxSeatWidth, maxSeatHeight);

            int panelWidth = seatWidth * kohad_ridades + margin * (kohad_ridades + 1);
            int panelHeight = seatHeight * ridade_arv + margin * (ridade_arv + 1);
            pnlSeats.Size = new Size(panelWidth, panelHeight);
            pnlSeats.Location = new Point(
                panelSideMargin,
                panelTopMargin
            );

            pealkiriLabel.Location = new Point (ClientSize.Width / 2 - pealkiriLabel.Width / 2, panelTopMargin / 2 - (pealkiriLabel.Height+saalLabel.Height)/2);
            saalLabel.Location = new Point(ClientSize.Width / 2 - saalLabel.Width / 2, pealkiriLabel.Bottom + 5);

            btnConfirmSelection.Location = new Point(
                (ClientSize.Width - btnConfirmSelection.Width) / 2,
                pnlSeats.Bottom + panelBotMargin / 2 - btnConfirmSelection.Height
            );

            AdjustWindow();
        }

        public void AdjustWindow()
        {
            int requiredWidth = pnlSeats.Width + panelSideMargin * 2;
            int requiredHeight = pnlSeats.Height + panelTopMargin + panelBotMargin + btnConfirmSelection.Height + margin * 2;

            this.Size = new Size(requiredWidth, requiredHeight);
        }

    }
}
