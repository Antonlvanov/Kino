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
        DatabaseHelper dbHelper = new DatabaseHelper();
        static string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\"));
        static string imageFolder = Path.Combine(projectRoot, @"Posters");

        //SqlConnection conn;
        //SqlCommand cmd;
        //SqlDataAdapter adapter;

        //OpenFileDialog open;
        //SaveFileDialog save;
        //Form popupForm;
        //DataTable laotable;
        //string extension;
        //private byte[] imageData;

        public DB_Haaldamine()
        {
            InitializeComponent();
            LoadTableNames();
        }

        private void LoadTableNames()
        {
            string query = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES " +
                            "WHERE TABLE_TYPE = 'BASE TABLE'";
            DataTable tables = dbHelper.ExecuteQuery(query);
            if (tables != null)
            {
                foreach (DataRow row in tables.Rows)
                {
                    tableListBox.Items.Add(row["TABLE_NAME"].ToString());
                }
            }
            else
            {
                MessageBox.Show("Ошибка при загрузке таблиц.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void vali_btn_Click(object sender, EventArgs e)
        {
            if (tableListBox.SelectedItem != null)
            {
                string selected_table = tableListBox.SelectedItem.ToString();

                try
                {
                    string query = $"SELECT * FROM {selected_table}";
                    DataTable dataTable = dbHelper.ExecuteQuery(query);
                    dataGridView1.DataSource = dataTable;
                    CreateInputFieldsForTable(selected_table);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Viga andmete laadimisel: {ex.Message}", "Viga", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        public void CreateInputFieldsForTable(string tableName)
        {
            flowLayoutPanel1.Controls.Clear();
            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown; 
            flowLayoutPanel1.WrapContents = false;
            flowLayoutPanel1.AutoScroll = true;

            string query = $"SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}'";
            DataTable dataTable = dbHelper.ExecuteQuery(query);

            Font labelFont = new Font("Microsoft Sans Serif", 13f);
            Font fieldFont = new Font("Microsoft Sans Serif", 9.5f);

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                string columnName = dataTable.Rows[i]["COLUMN_NAME"].ToString();
                string dataType = dataTable.Rows[i]["DATA_TYPE"].ToString();
                if (i == 0)
                    continue;

                string labelText = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(columnName.Replace("_", " "));
                Panel panel = new Panel();
                panel.Width = flowLayoutPanel1.Width - 10;
                panel.Height = 30;

                Label label = new Label();
                label.Text = labelText;
                label.Font = labelFont;
                label.AutoSize = false;
                label.Width = 120;
                label.TextAlign = ContentAlignment.MiddleLeft;

                Control inputField;
                if (dataType == "int" || dataType == "decimal" || dataType == "float")
                {
                    NumericUpDown numericUpDown = new NumericUpDown();
                    numericUpDown.Name = columnName;
                    numericUpDown.Width = 60;
                    numericUpDown.Height = 40;
                    numericUpDown.Font = fieldFont;
                    numericUpDown.Maximum = decimal.MaxValue;
                    inputField = numericUpDown;
                }
                else if (dataType == "datetime" || dataType == "date")
                {
                    DateTimePicker dateTimePicker = new DateTimePicker();
                    dateTimePicker.Name = columnName;
                    dateTimePicker.Format = DateTimePickerFormat.Custom;
                    dateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss"; 
                    dateTimePicker.Width = 160;
                    dateTimePicker.Font = fieldFont;
                    inputField = dateTimePicker;
                }
                else
                {
                    TextBox textBox = new TextBox();
                    textBox.Name = columnName;
                    textBox.Width = 100;
                    textBox.Font = fieldFont;
                    inputField = textBox;
                }

                inputField.Margin = new Padding(0, 15, 0, 0);
                inputField.Location = new Point(label.Width + 10, 0);
                panel.Controls.Add(inputField);
                panel.Controls.Add(label);
                flowLayoutPanel1.Controls.Add(panel);
            }
        }
        private void dataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];
            foreach (DataGridViewCell cell in selectedRow.Cells)
            {
                string columnName = cell.OwningColumn.Name;
                Control[] controls = flowLayoutPanel1.Controls.Find(columnName, true);
                if (controls.Length > 0)
                {
                    Control inputField = controls[0];
                    if (inputField is TextBox textBox)
                    {
                        textBox.Text = cell.Value?.ToString() ?? string.Empty;
                    }
                    else if (inputField is NumericUpDown numericUpDown)
                    {
                        if (decimal.TryParse(cell.Value?.ToString(), out decimal numericValue))
                        {
                            numericUpDown.Value = numericValue;
                        }
                        else
                        {
                            numericUpDown.Value = 0;
                        }
                    }
                    else if (inputField is DateTimePicker dateTimePicker)
                    {
                        if (DateTime.TryParse(cell.Value?.ToString(), out DateTime dateValue))
                        {
                            dateTimePicker.Value = dateValue;
                        }
                        else
                        {
                            dateTimePicker.Value = DateTime.Now;
                        }
                    }
                }
            }

            // Обработка изображения (если присутствует)
            if (dataGridView1.Columns.Contains("poster"))
            {
                // Обработка изображения
                if (selectedRow.Cells["poster"].Value != null && !string.IsNullOrEmpty(selectedRow.Cells["poster"].Value.ToString()))
                {
                    try
                    {
                        pictureBox1.Image = Image.FromFile(Path.Combine(imageFolder, selectedRow.Cells["poster"].Value.ToString()));
                        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}");
                    }
                }
                else
                {
                    pictureBox1.Image = null;
                }
            }
        }
    }
}
