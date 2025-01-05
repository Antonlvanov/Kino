using Kino.UserControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kino.Forms.Start
{
    partial class StartForm
    {
        private Label Kino { get; set; }
        private Button Sessions { get; set; }
        private Button AdminPanel { get; set; }
        private Button Login { get; set; }
        private Button Register { get; set; }
        private Panel UserPanel { get; set; }
        private PictureBox UserIcon { get; set; }
        private Label UserNameLabel { get; set; }
        private Label UserStatusLabel { get; set; }
        private Button LogoutButton { get; set; }
        public UserManager UserManager { get; set; }

    }
}
