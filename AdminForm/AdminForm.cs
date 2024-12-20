using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kino
{
    public partial class AdminForm : Form
    {
        private DatabaseQueryHelper dbHelper;
        private InputFieldGenerator inputFieldGenerator;

        static string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\"));
        static string imageFolder = Path.Combine(projectRoot, @"Posters");

        public AdminForm()
        {
            InitializeComponent();
            dbHelper = new DatabaseQueryHelper();
            inputFieldGenerator = new InputFieldGenerator(dbHelper);
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
                NaitaAndmed();
                GenerateFieldsAndModel(tableListBox.SelectedItem.ToString());
                //inputFieldGenerator.GenerateFields(flowLayoutPanel1, tableListBox.SelectedItem.ToString());
            }
        }

        private void NaitaAndmed()
        {
            try
            {
                string query = $"SELECT * FROM {tableListBox.SelectedItem.ToString()}";
                DataTable dataTable = dbHelper.ExecuteQuery(query);

                dataGridView1.Columns.Clear();
                dataGridView1.AutoGenerateColumns = false;

                foreach (DataColumn column in dataTable.Columns)
                {
                    DataGridViewTextBoxColumn dataGridViewColumn = new DataGridViewTextBoxColumn();
                    dataGridViewColumn.DataPropertyName = column.ColumnName;
                    dataGridViewColumn.Name = column.ColumnName;
                    dataGridView1.Columns.Add(dataGridViewColumn);
                }

                dataGridView1.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Viga andmete laadimisel: {ex.Message}", "Viga", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DynamicModel GenerateFieldsAndModel(string tableName)
        {
            Dictionary<string, Control> controls = new Dictionary<string, Control>();

            flowLayoutPanel1.Controls.Clear();
            var columnNames = dbHelper.ExecuteQuery($"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}'");

            foreach (DataRow row in columnNames.Rows)
            {
                string columnName = row["COLUMN_NAME"].ToString();

                Label label = new Label { Text = columnName, AutoSize = true };
                flowLayoutPanel1.Controls.Add(label);

                Control inputControl;
                if (columnName.ToLower().Contains("date"))
                {
                    inputControl = new DateTimePicker();
                }
                else if (columnName.ToLower().Contains("id") && columnName != "id")
                {
                    inputControl = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
                    // Заполните ComboBox значениями из связанной таблицы
                }
                else if (columnName.ToLower().Contains("price") || columnName.ToLower().Contains("count"))
                {
                    inputControl = new NumericUpDown { DecimalPlaces = 2, Maximum = 1000000 };
                }
                else
                {
                    inputControl = new TextBox();
                }

                inputControl.Name = columnName;
                flowLayoutPanel1.Controls.Add(inputControl);

                controls[columnName] = inputControl;
            }

            return new DynamicModel(controls);
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
                    else if (inputField is ComboBox comboBox)
                    {
                        if (comboBox.Items.Contains("True") && comboBox.Items.Contains("False"))
                        {
                            if (cell.Value.ToString() == "True")
                            {
                                comboBox.SelectedItem = "True";
                            }
                            else if (cell.Value.ToString() == "False")
                            {
                                comboBox.SelectedItem = "False";
                            }
                        }
                        else
                        {
                            if (cell.Value != null && cell.Value != DBNull.Value && int.TryParse(cell.Value.ToString(), out int index))
                            {
                                comboBox.SelectedIndex = index;
                            }
                            else
                            {
                                comboBox.SelectedIndex = -1;
                            }
                        }
                    }
                }
            }
            if (dataGridView1.Columns.Contains("poster"))
            {
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
