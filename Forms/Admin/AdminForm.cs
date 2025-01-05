using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Kino.Database;

namespace Kino.Forms.Admin
{
    public partial class AdminForm : Form
    {
        private dbHelper dbHelper;
        private Queries Queries;

        public AdminForm()
        {
            InitializeComponents();
            this.Size = DefaultSettings.DefaultClientSize;
            dbHelper = new dbHelper();
            Queries = new Queries();
            LoadTablesToList();
        }
        
    }
}
