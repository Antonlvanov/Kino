using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kino.Forms.Start
{
    partial class StartForm
    {
        public void LayoutComponents()
        {
            int formWidth = this.ClientSize.Width;
            int currentY = 20;
            int spacing = 20;

            Kino.Location = new Point((formWidth - Kino.Width) / 2, currentY);
            currentY += Kino.Height + spacing * 3;

            Sessions.Location = new Point((formWidth - Sessions.Width) / 2, currentY);
            currentY += Sessions.Height + spacing;

            AdminPanel.Location = new Point((formWidth - AdminPanel.Width) / 2, currentY);
            currentY += AdminPanel.Height + spacing * 2;

            int totalWidth = Login.Width + Register.Width + spacing;
            int startX = (formWidth - totalWidth) / 2;

            Login.Location = new Point(startX, currentY);
            Register.Location = new Point(startX + Login.Width + spacing, currentY);
        }

        private Button CreateButton(string text, Point location, Font font = null)
        {
            var button = new Button
            {
                Text = text,
                Font = font ?? new Font("Segoe UI", 12F, FontStyle.Regular),
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
                Font = new Font("Segoe UI", 20F, FontStyle.Regular),
                AutoSize = true,
                Location = location,
                TextAlign = ContentAlignment.MiddleCenter,
            };
            label.PerformLayout();
            return label;
        }
    }
}
