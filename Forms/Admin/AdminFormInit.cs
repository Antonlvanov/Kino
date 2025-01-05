using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Kino.Forms.Admin
{
    partial class AdminForm : Form
    {
        public void Initialize()
        {
            InitializeComponents();
        }
        public void InitializeComponents()
        {
            TableList = new System.Windows.Forms.ListView
            {
                Font = DefaultSettings.TableListFont,
                View = View.Details, 
                FullRowSelect = true,
                HeaderStyle = ColumnHeaderStyle.None,
                Dock = DockStyle.Fill,
                Scrollable = false,
                BorderStyle = BorderStyle.FixedSingle,
                GridLines = true,
                HideSelection = false
            };
            TableList.Columns.Add("", -2, HorizontalAlignment.Center);

            DataGridView = new DataGridView
            {
                Font = DefaultSettings.DataGridFont,
                Dock = DockStyle.Fill,
                Height = (int)(DefaultSettings.DefaultClientSize.Height*0.34),
                AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill,
                AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells,
                ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                AutoGenerateColumns = false
            };

            ControlPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = false,
                BackColor = Color.LightGray,
                ColumnCount = 1,
                RowCount = 2
            };

            ControlPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            ControlPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            ColumnsControlPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = false,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.White
            };
            ControlPanel.Controls.Add(ColumnsControlPanel, 0, 0);

            ButtonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                WrapContents = false,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight
            };
            ControlPanel.Controls.Add(ButtonPanel, 0, 1);

            LisaButton = CreateButton("Lisa");
            KustutaButton = CreateButton("Kustuta");
            UuendaButton = CreateButton("Uuenda");
            FilterButton = CreateButton("Filter");
            InfoButton = CreateButton("Info");

            ButtonPanel.Controls.Add(LisaButton);
            ButtonPanel.Controls.Add(KustutaButton);
            ButtonPanel.Controls.Add(UuendaButton);
            ButtonPanel.Controls.Add(FilterButton);
            ButtonPanel.Controls.Add(InfoButton);

            PictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSize = true
            };

            MainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 3
            };

            MainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            MainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            MainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            MainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

            MainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33));
            MainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33));
            MainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 34));

            MainPanel.Controls.Add(TableList, 0, 0);
            MainPanel.SetRowSpan(TableList, 3);

            MainPanel.Controls.Add(ControlPanel, 1, 0);
            MainPanel.SetColumnSpan(ControlPanel, 2);
            MainPanel.SetRowSpan(ControlPanel, 3);

            MainPanel.Controls.Add(PictureBox, 3, 0);
            MainPanel.SetRowSpan(PictureBox, 3);

            MainPanel.Controls.Add(DataGridView, 0, 2);
            MainPanel.SetColumnSpan(DataGridView, 4);

            Controls.Add(MainPanel);

            TableList.DoubleClick += TalbeSelect_Click;
            DataGridView.RowHeaderMouseClick += new DataGridViewCellMouseEventHandler(GridViewClick);
            FilterButton.Click += FilterData;
            LisaButton.Click += AddToTable;
            KustutaButton.Click += DeleteSelected;
            UuendaButton.Click += UpdateSelectedRow;
            InfoButton.Click += ShowTableInfo;
        }
    }
}
