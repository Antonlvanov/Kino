using Kino.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kino.Forms.Admin
{
    public partial class AdminForm : Form
    {
        public ListView TableList { get; set; }
        public Table selectedTable {  get; set; }
        public DataGridView DataGridView { get; set; }
        public TableLayoutPanel MainPanel { get; set; }
        public TableLayoutPanel ControlPanel { get; set; }
        public FlowLayoutPanel ColumnsControlPanel { get; set; }
        public FlowLayoutPanel ColumnPanel { get; set; }
        public FlowLayoutPanel ButtonPanel { get; set; }
        public PictureBox PictureBox { get; set; }
        public Button FilterButton { get; set; }
        public Button UuendaButton { get; set; }
        public Button KustutaButton { get; set; }
        public Button LisaButton { get; set; }
        public Button InfoButton { get; set; }
        public DataTable CurrentTable { get; set; }

    }
    public static class DefaultSettings
    {
        public static Size DefaultClientSize => new Size(1040, 680);
        public static Font TableListFont => new Font("Segoe UI", 25F, FontStyle.Regular);
        public static Font DataGridFont => new Font("Segoe UI", 12F, FontStyle.Regular);
        public static Font ButtonFont => new Font("Segoe UI", 12F, FontStyle.Regular);
        public static Font LabelFont => new Font("Segoe UI", 12F, FontStyle.Regular);
        public static Font InputFont => new Font("Segoe UI", 12F, FontStyle.Regular);
        public static Size DefaultButtonSize => new Size(75, 30);
    }
}
