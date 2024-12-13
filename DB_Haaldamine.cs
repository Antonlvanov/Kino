﻿using System;
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
        private DatabaseQueryHelper dbHelper;
        private InputFieldGenerator inputFieldGenerator;

        static string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\"));
        static string imageFolder = Path.Combine(projectRoot, @"Posters");

        public DB_Haaldamine()
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
                NaitaAndmed();
                inputFieldGenerator.GenerateFields(flowLayoutPanel1, selected_table);
            }
        }

        private void NaitaAndmed()
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Viga andmete laadimisel: {ex.Message}", "Viga", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Lisa_btn_Click(object sender, EventArgs e)
        {
            if (FieldsNotNull())
            {
                try
                {
                    string tableName = tableListBox.SelectedItem.ToString();
                    Console.WriteLine($"Selected table: {tableName}");

                    // Получаем список колонок из dataGridView1
                    var columnNames = dataGridView1.Columns.Cast<DataGridViewColumn>()
                                       .Select(col => col.Name)
                                       .Where(name => !string.Equals(name, "id", StringComparison.OrdinalIgnoreCase)) // Исключаем автоинкрементное поле "id"
                                       .ToList();

                    Console.WriteLine($"Columns retrieved for table {tableName}: {string.Join(", ", columnNames)}");

                    // Формируем список параметров для запроса
                    Dictionary<string, object> parameters = new Dictionary<string, object>();
                    foreach (string columnName in columnNames)
                    {
                        Control control = flowLayoutPanel1.Controls.Find(columnName, true).FirstOrDefault();

                        if (control is TextBox textBox)
                        {
                            parameters.Add(columnName, textBox.Text);
                            Console.WriteLine($"TextBox value for {columnName}: {textBox.Text}");
                        }
                        else if (control is ComboBox comboBox)
                        {
                            if (comboBox.SelectedIndex > 0)
                            {
                                parameters.Add(columnName, comboBox.SelectedIndex);
                                Console.WriteLine($"ComboBox value for {columnName}: {comboBox.SelectedIndex}");
                            }
                            else
                            {
                                throw new Exception($"Поле '{columnName}' не имеет выбранного значения.");
                            }
                        }
                        else if (control is NumericUpDown numericUpDown)
                        {
                            parameters.Add(columnName, numericUpDown.Value);
                            Console.WriteLine($"NumericUpDown value for {columnName}: {numericUpDown.Value}");
                        }
                        else if (control is DateTimePicker dateTimePicker)
                        {
                            parameters.Add(columnName, dateTimePicker.Value);
                            Console.WriteLine($"DateTimePicker value for {columnName}: {dateTimePicker.Value}");
                        }
                        else
                        {
                            Console.WriteLine($"No matching control found for {columnName}.");
                        }
                    }

                    // Проверяем сформированные параметры
                    foreach (var param in parameters)
                    {
                        Console.WriteLine($"Parameter: {param.Key} = {param.Value}");
                    }

                    // Генерируем запрос
                    string columnList = string.Join(", ", parameters.Keys);
                    string valuePlaceholders = string.Join(", ", parameters.Keys.Select(key => $"@{key}"));
                    string query = $"INSERT INTO {tableName} ({columnList}) VALUES ({valuePlaceholders})";

                    Console.WriteLine($"Generated query: {query}");

                    try { dbHelper.ExecuteNonQuery(query, parameters); }
                    catch (Exception ex) { Console.WriteLine(ex.Message); }

                    ClearFields();
                    NaitaAndmed();
                    MessageBox.Show($"Success", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    MessageBox.Show($"Ошибка при добавлении: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                Console.WriteLine("Fields are not completely filled.");
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void ClearFields()
        {
            foreach (Control control in flowLayoutPanel1.Controls)
            {
                if (control is TextBox textBox)
                {
                    textBox.Text = string.Empty;
                }
                else if (control is ComboBox comboBox)
                {
                    if (comboBox.Items.Count > 0)
                    {
                        comboBox.SelectedIndex = 0; 
                    }
                }
                else if (control is NumericUpDown numericUpDown)
                {
                    numericUpDown.Value = 0; 
                }
                else if (control is DateTimePicker dateTimePicker)
                {
                    dateTimePicker.Value = DateTime.Now; 
                }
                else if (control is PictureBox pictureBox)
                {
                    pictureBox.Image?.Dispose();
                    pictureBox.Image = null;
                }
            }
        }

        private bool FieldsNotNull()
        {
            foreach (Control control in flowLayoutPanel1.Controls)
            {
                if (control is TextBox textBox && string.IsNullOrWhiteSpace(textBox.Text))
                {
                    return false; 
                }
                else if (control is ComboBox comboBox && comboBox.SelectedIndex <= 0)
                {
                    return false; 
                }
                else if (control is NumericUpDown numericUpDown && numericUpDown.Value == 0)
                {
                    return false; 
                }
            }
            return true;
        }


        private void FilterData(string selectedTable)
        {
            try
            {
                if (dataGridView1.DataSource is DataTable dataTable)
                {
                    List<string> filters = new List<string>();

                    foreach (DataColumn column in dataTable.Columns)
                    {
                        string columnName = column.ColumnName;

                        Control[] controls = flowLayoutPanel1.Controls.Find(columnName, true);
                        if (controls.Length > 0)
                        {
                            Control inputField = controls[0];

                            if (inputField is TextBox textBox && !string.IsNullOrEmpty(textBox.Text))
                            {
                                filters.Add($"[{columnName}] LIKE '%{textBox.Text}%'");
                            }
                            else if (inputField is NumericUpDown numericUpDown && numericUpDown.Value > 0)
                            {
                                filters.Add($"[{columnName}] = {numericUpDown.Value}");
                            }
                            //else if (inputField is DateTimePicker dateTimePicker)
                            //{
                            //    filters.Add($"[{columnName}] = #{dateTimePicker.Value:yyyy-MM-dd}#");
                            //}
                            else if (inputField is ComboBox comboBox)
                            {
                                if (comboBox.Items.Contains("True") && comboBox.Items.Contains("False"))
                                {
                                    if (comboBox.SelectedItem is string selectedBit && !string.IsNullOrEmpty(selectedBit))
                                    {
                                        filters.Add($"[{columnName}] = {(selectedBit == "True" ? 1 : 0)}");
                                    }
                                }
                                else if (comboBox.SelectedIndex > 0)
                                {
                                    filters.Add($"[{columnName}] = '{comboBox.SelectedValue}'");
                                }
                            }
                        }
                    }
                    string filterExpression = string.Join(" AND ", filters);

                    if (!string.IsNullOrEmpty(filterExpression))
                    {
                        dataTable.DefaultView.RowFilter = filterExpression;
                    }
                    else
                    {
                        dataTable.DefaultView.RowFilter = string.Empty;
                    }
                }
                else
                {
                    MessageBox.Show("Источник данных недоступен или некорректен.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
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
