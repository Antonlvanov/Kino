using Kino.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kino.Forms.Admin
{
    partial class AdminForm : Form
    {
        private void LoadTablesToList()
        {
            foreach (DataRow row in dbHelper.ExecuteQuery(Queries.GetAllTables()).Rows)
            {
                TableList.Items.Add(new ListViewItem(row["TABLE_NAME"].ToString()));
            }
            AdjustTablesListSize();
        }
        private void LoadTableGridView()
        {
            DataGridView.Columns.Clear();
            foreach (var column in selectedTable.Columns)
            {
                DataGridViewTextBoxColumn dataGridViewColumn = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = column.Name,
                    Name = column.Name,
                    HeaderText = column.Name
                };
                DataGridView.Columns.Add(dataGridViewColumn);
            }
            foreach (var row in selectedTable.Rows)
            {
                DataGridView.Rows.Add(
                    selectedTable.Columns.Select(c => row[c.Name]).ToArray()
                );
            }
        }

        private void TalbeSelect_Click(object sender, EventArgs e)
        {
            selectedTable = MakeTable(TableList.SelectedItems[0].Text);
            ClearTempControls();
            LoadTableGridView();
            GenerateFields();
            OnResize(EventArgs.Empty);
        }

        public void UpdateSelectedTable()
        {
            selectedTable = MakeTable(TableList.SelectedItems[0].Text);
            LoadTableGridView();
        }

        public Table MakeTable(string tableName)
        {
            DataTable columns = dbHelper.ExecuteQuery(Queries.GetColumnsInfo(tableName));
            DataTable fkeys = dbHelper.ExecuteQuery(Queries.GetForeignKeysInfo(tableName));
            DataTable rows = dbHelper.ExecuteQuery(Queries.GetTableRows(tableName));
            return new Table(tableName, columns, fkeys, rows);
        }

        private void GridViewClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridViewRow selectedRow = DataGridView.Rows[e.RowIndex];
            foreach (DataGridViewCell cell in selectedRow.Cells)
            {
                string columnName = cell.OwningColumn.Name;
                Control inputField = ColumnsControlPanel.Controls.Find(columnName, true).FirstOrDefault();
                if (inputField != null)
                {
                    UpdateInputField(inputField, cell.Value);
                }
            }
            if (DataGridView.Columns.Contains("poster")) { UpdatePictureBox(selectedRow.Cells["poster"]?.Value?.ToString()); }
            else { PictureBox.Image = null; }
        }

        private void DeleteSelected(object sender, EventArgs e)
        {
            var idsToDelete = new List<object>();
            foreach (DataGridViewRow selectedRow in DataGridView.SelectedRows)
            {
                idsToDelete.Add(selectedRow.Cells[0].Value);
            }
            if (idsToDelete.Count == 0) { return; }
            try
            {
                string ids = string.Join(", ", idsToDelete);
                if (dbHelper.ExecuteNonQuery(Queries.DeleteByID(selectedTable.TableName, selectedTable.Columns[0].Name, ids)))
                {
                    UpdateSelectedTable();
                    MessageBox.Show("Kirjed kustutati edukalt.", "Edu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else { MessageBox.Show($"Viga kirjete kustutamisel", "Viga", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Viga kirjete kustutamisel: {ex.Message}", "Viga", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void FilterData(object sender, EventArgs e)
        {
            var filters = new Dictionary<string, string>();
            foreach (Control columnPanel in ColumnsControlPanel.Controls)
            {
                foreach (Control control in columnPanel.Controls)
                {
                    if (control is DateTimePicker) continue;
                    string columnName = control.Name.ToString();
                    string filterValue = GetControlValue(control);
                    if (!string.IsNullOrEmpty(filterValue))
                    {
                        filters[columnName] = filterValue;
                    }
                }
            }
            var filteredRows = new List<RowData>();
            foreach (var row in selectedTable.Rows)
            {
                bool matchesAllFilters = true;
                foreach (var filter in filters)
                {
                    string columnName = filter.Key;
                    string filterValue = filter.Value;
                    string cellValue = row[columnName]?.ToString();

                    if (string.IsNullOrEmpty(cellValue) || cellValue != filterValue)
                    {
                        matchesAllFilters = false;
                        break;
                    }
                }
                if (matchesAllFilters)
                {
                    filteredRows.Add(row);
                }
            }
            FilteredTableGridView(filteredRows);
        }

        private void UpdateSelectedRow(object sender, EventArgs e)
        {
            if (FieldsNotNull())
            {
                if (DataGridView.SelectedRows.Count != 1)
                {
                    MessageBox.Show("Valige värskendamiseks rida (1).", "Viga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                try
                {
                    DataGridViewRow selectedRow = DataGridView.SelectedRows[0];
                    Dictionary<string, object> parameters = new Dictionary<string, object>();

                    var idColumn = selectedTable.Columns[0].Name;
                    var idValue = selectedRow.Cells[idColumn].Value;

                    foreach (Control columnPanel in ColumnsControlPanel.Controls)
                    {
                        foreach (Control control in columnPanel.Controls)
                        {
                            string columnName = control.Name;
                            if (string.IsNullOrEmpty(columnName) || columnName == idColumn)
                                continue;

                            string columnValue = GetControlValue(control);
                            if (!string.IsNullOrEmpty(columnValue))
                            {
                                var columnType = selectedTable.GetColumnMetadata(columnName).DataType;
                                object value = ConvertToColumnType(columnValue, columnType);
                                parameters.Add(columnName, value);
                            }
                        }
                    }
                    parameters[idColumn] = idValue;
                    string columnList = string.Join(", ", parameters.Keys.Where(key => key != idColumn).Select(key => $"{key} = @{key}"));
                    string query = Queries.UpdateByID(selectedTable.TableName, columnList, idColumn);

                    if (dbHelper.ExecuteNonQuery(query, parameters))
                    {
                        UpdateSelectedTable();
                        ClearFields();
                        MessageBox.Show("Kirje värskendamine õnnestus.", "Edu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else { MessageBox.Show($"Viga kirje värskendamisel", "Viga", MessageBoxButtons.OK, MessageBoxIcon.Error);}
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Viga kirje värskendamisel: {ex.Message}", "Viga", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Palun täitke kõik väljad", "Viga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void AddToTable(object sender, EventArgs e)
        {
            if (FieldsNotNull())
            {
                try
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>();

                    foreach (Control columnPanel in ColumnsControlPanel.Controls)
                    {
                        foreach (Control control in columnPanel.Controls)
                        {
                            string columnName = control.Name.ToString();
                            if (string.IsNullOrEmpty(columnName)) continue;
                            string columnValue = GetControlValue(control);
                            if (!string.IsNullOrEmpty(columnValue))
                            {
                                parameters.Add(columnName, columnValue);
                            }
                        }
                    }
                    string columnList = string.Join(", ", parameters.Keys);
                    string valuePlaceholders = string.Join(", ", parameters.Keys.Select(key => $"@{key}"));
                    string query = Queries.InsertIntoTalbe(selectedTable.TableName, columnList, valuePlaceholders);

                    if (dbHelper.ExecuteNonQuery(query, parameters))
                    {
                        UpdateSelectedTable();
                        ClearFields();
                        MessageBox.Show($"Edukalt lisatud", "Lisatud", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else { MessageBox.Show($"Viga kirje värskendamisel", "Viga", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Viga lisamisel: {ex.Message}", "Viga", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Palun täitke kõik väljad", "Viga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ShowTableInfo(object sender, EventArgs e)
        {
            Form metadataForm = new Form
            {
                Text = $"Tabeli metaandmed: {selectedTable.TableName}",
                Size = new Size(500, 400),
                StartPosition = FormStartPosition.CenterParent
            };
            ListView columnsListView = new ListView
            {
                Dock = DockStyle.Top,
                Height = 200,
                View = View.Details,
                FullRowSelect = true
            };
            columnsListView.Columns.Add("Veeru nimi", 150);
            columnsListView.Columns.Add("Andmetüüp", 150);

            foreach (var column in selectedTable.Columns)
            {
                var item = new ListViewItem(column.Name);
                item.SubItems.Add(column.DataType);
                columnsListView.Items.Add(item);
            }
            ListView foreignKeysListView = new ListView
            {
                Dock = DockStyle.Top,
                Height = 150,
                View = View.Details,
                FullRowSelect = true
            };
            foreignKeysListView.Columns.Add("Veeru nimi", 150);
            foreignKeysListView.Columns.Add("Viitab tabelile", 150);
            foreignKeysListView.Columns.Add("Viitab veerule", 150);

            foreach (var fk in selectedTable.ForeignKeys)
            {
                var item = new ListViewItem(fk.Key);
                item.SubItems.Add(fk.Value.ReferencedTable);
                item.SubItems.Add(fk.Value.ReferencedColumn);
                foreignKeysListView.Items.Add(item);
            }

            Label columnsLabel = new Label
            {
                Text = "Tabeli veerud:",
                Dock = DockStyle.Top,
                Padding = new Padding(5),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            Label foreignKeysLabel = new Label
            {
                Text = "Võõrad võtmed:",
                Dock = DockStyle.Top,
                Padding = new Padding(5),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };

            metadataForm.Controls.Add(foreignKeysListView);
            metadataForm.Controls.Add(foreignKeysLabel);
            metadataForm.Controls.Add(columnsListView);
            metadataForm.Controls.Add(columnsLabel);
            metadataForm.ShowDialog();
        }

        private object ConvertToColumnType(string columnValue, string columnType)
        {
            switch (columnType.ToLower())
            {
                case "int":
                    return int.TryParse(columnValue, out var intValue) ? (object)intValue : DBNull.Value;
                case "decimal":
                    return decimal.TryParse(columnValue, out var decimalValue) ? (object)decimalValue : DBNull.Value;
                case "datetime":
                    return DateTime.TryParse(columnValue, out var dateValue) ? (object)dateValue : DBNull.Value;
                case "date":
                    return DateTime.TryParse(columnValue, out var dateOnlyValue) ? (object)dateOnlyValue.Date : DBNull.Value;
                case "nvarchar":
                case "varchar":
                case "text":
                    return columnValue; 
                default:
                    return columnValue; 
            }
        }
    }
}
