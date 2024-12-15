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
    public partial class DB_Haaldamine : Form
    {
        private DatabaseHelper dbHelper;
        private InputFieldGenerator inputFieldGenerator;

        static string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\"));
        static string imageFolder = Path.Combine(projectRoot, @"Posters");

        public DB_Haaldamine()
        {
            InitializeComponent();
            dbHelper = new DatabaseHelper();
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
        private void filter_btn_Click(object sender, EventArgs e)
        {
            if (tableListBox.SelectedItem != null)
            {
                string selected_table = tableListBox.SelectedItem.ToString();
                FilterData(selected_table);
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
                    inputFieldGenerator.GenerateFields(flowLayoutPanel1, selected_table);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Viga andmete laadimisel: {ex.Message}", "Viga", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void FilterData(string tableName)
        {
            StringBuilder queryBuilder = new StringBuilder($"SELECT * FROM {tableName} WHERE 1 = 1");
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            int controlCount = 0;

            foreach (Control control in flowLayoutPanel1.Controls)
            {
                controlCount++;

                string columnName = control.Name;

                // Используем Find для поиска каждого контрола по имени
                Control[] foundControls = flowLayoutPanel1.Controls.Find(columnName, true);

                if (foundControls.Length > 0)
                {
                    // Получаем первый найденный контрол с таким именем
                    Control foundControl = foundControls[0];

                    if (foundControl is TextBox textBox)
                    {
                        Console.WriteLine($"TextBox found: Name = {columnName}, Value = {textBox.Text}");

                        if (!string.IsNullOrEmpty(textBox.Text))
                        {
                            queryBuilder.Append($" AND {columnName} LIKE @{columnName}");
                            parameters.Add($"@{columnName}", $"%{textBox.Text}%");
                        }
                    }
                    else if (foundControl is ComboBox comboBox)
                    {
                        Console.WriteLine($"ComboBox found: Name = {columnName}, SelectedItem = {comboBox.SelectedItem}");

                        if (comboBox.SelectedItem != null)
                        {
                            queryBuilder.Append($" AND {columnName} = @{columnName}");
                            parameters.Add($"@{columnName}", comboBox.SelectedItem.ToString());
                        }
                    }
                    else if (foundControl is NumericUpDown numericUpDown)
                    {
                        Console.WriteLine($"NumericUpDown found: Name = {columnName}, Value = {numericUpDown.Value}");

                        if (numericUpDown.Value > 0)
                        {
                            queryBuilder.Append($" AND {columnName} = @{columnName}");
                            parameters.Add($"@{columnName}", numericUpDown.Value);
                        }
                    }
                    else if (foundControl is DateTimePicker dateTimePicker)
                    {
                        Console.WriteLine($"DateTimePicker found: Name = {columnName}, Value = {dateTimePicker.Value:yyyy-MM-dd}");

                        if (dateTimePicker.Value != DateTime.Now)
                        {
                            queryBuilder.Append($" AND {columnName} = @{columnName}");
                            parameters.Add($"@{columnName}", dateTimePicker.Value.ToString("yyyy-MM-dd"));
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Unknown control found: Type = {foundControl.GetType().Name}, Name = {foundControl.Name}");
                    }
                }
            }

            Console.WriteLine($"Total controls found: {controlCount}");
            Console.WriteLine($"Generated Query: {queryBuilder}");

            try
            {
                DataTable filteredData = dbHelper.ExecuteQuery(queryBuilder.ToString(), parameters);

                if (filteredData != null)
                {
                    Console.WriteLine($"Filtered rows count: {filteredData.Rows.Count}");

                    dataGridView1.Columns.Clear();
                    dataGridView1.AutoGenerateColumns = false;
                    foreach (DataColumn column in filteredData.Columns)
                    {
                        DataGridViewTextBoxColumn dataGridViewColumn = new DataGridViewTextBoxColumn
                        {
                            DataPropertyName = column.ColumnName,
                            Name = column.ColumnName
                        };
                        dataGridView1.Columns.Add(dataGridViewColumn);
                    }
                    dataGridView1.DataSource = filteredData;
                }
                else
                {
                    MessageBox.Show("No data found for the given filters.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                MessageBox.Show($"Ошибка при фильтрации данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    else if (inputField is ComboBox comboBox)
                    {
                        if (cell.Value != null && cell.Value != DBNull.Value && int.TryParse(cell.Value.ToString(), out int index))
                        {
                            comboBox.SelectedIndex = index - 1;
                        }
                        else
                        {
                            comboBox.SelectedIndex = -1;
                        }
                    }
                    else if (inputField is CheckBox checkBox)
                    {
                        checkBox.Checked = Convert.ToBoolean(cell.Value);
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
