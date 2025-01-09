using Kino.Database;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static Kino.Forms.Sessions.SessionsForm;


namespace Kino.Forms.Saal
{
    partial class SaalForm : Form
    {
        private Panel pnlSeats;
        private Button btnConfirmSelection;
        private Dictionary<Button, int> seatStatus = new Dictionary<Button, int>(); 
        private dbHelper dbHelper = new dbHelper();
        private Session Session = new Session();
        List<Koht> kohad = new List<Koht>();

        private void InitializeComponents()
        {
            this.Text = "Кинозал - Выбор мест";
            this.Size = new Size(800, 600);

            pnlSeats = new Panel()
            {
                AutoScroll = true,
                Size = new Size(700, 500),
                Location = new Point(50, 20),
                BorderStyle = BorderStyle.FixedSingle,
            };

            btnConfirmSelection = new Button()
            {
                Text = "Osta piletid",
                Location = new Point(300, 530),
            };

            this.Controls.Add(pnlSeats);
            this.Controls.Add(btnConfirmSelection);
        }


    }
}
