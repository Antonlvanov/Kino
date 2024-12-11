using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kino
{
    public partial class DB_Haaldamine : Form
    {
        static string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\"));
        static string db_name = "Andmebaas.mdf";
        static string db_path = Path.Combine(projectRoot, "Andmebaas.mdf");
        static string imageFolder = Path.Combine(projectRoot, @"Pildid");

        SqlConnection conn;
        SqlCommand cmd;
        SqlDataAdapter adapter;

        OpenFileDialog open;
        SaveFileDialog save;
        Form popupForm;
        DataTable laotable;
        string extension;
        private byte[] imageData;

        public DB_Haaldamine()
        {
            InitializeComponent();
        }

        public void FindDB()
        {
            if (File.Exists(db_path))
            {
                conn = new SqlConnection($@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={db_path};Integrated Security=True");
            }
        }
    }
}
