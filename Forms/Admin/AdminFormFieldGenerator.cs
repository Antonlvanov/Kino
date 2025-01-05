using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kino.Database;

namespace Kino.Forms.Admin
{
    public partial class AdminForm : Form
    {
        public void GenerateFields()
        {
            foreach (var column in selectedTable.Columns.Skip(1))
            {
                ColumnPanel = CreateFieldPanel(ColumnsControlPanel.Height / selectedTable.Columns.Count - 1);
                Label label = CreateLabel(column.Name);
                Control inputField = CreateInputField(column.Name, column.DataType);

                ColumnPanel.Controls.Add(label);
                ColumnPanel.Controls.Add(inputField);
                ColumnsControlPanel.Controls.Add(ColumnPanel);
            }
            if (selectedTable.ForeignKeys.Any())
            {
                Button uniteButton = CreateButton("Ühenda väljad");
                uniteButton.Click += (s, e) => TransformForeignKeyFields();
                ButtonPanel.Controls.Add(uniteButton);
            }
            OnResize(EventArgs.Empty);
        }

        private void TransformForeignKeyFields()
        {
            foreach (FlowLayoutPanel ColumnPanel in ColumnsControlPanel.Controls)
            {
                Label label = ColumnPanel.Controls.OfType<Label>().FirstOrDefault();
                Control inputField = ColumnPanel.Controls.OfType<Control>().FirstOrDefault(c => !(c is Label));

                if (selectedTable.ForeignKeys.ContainsKey(inputField.Name))
                {
                    ForeignKeyInfo foreignKeyInfo = selectedTable.ForeignKeys[inputField.Name];
                    Control comboBox = ForeignComboBox(inputField.Name);

                    comboBox.Location = inputField.Location;
                    ColumnPanel.Controls.Remove(inputField);
                    ColumnPanel.Controls.Add(comboBox);

                    label.Text = label.Text.Replace(" Id", "").TrimEnd();
                }
            }
            OnResize(EventArgs.Empty);
        }
        private ComboBox ForeignComboBox(string columnName)
        {
            ForeignKeyInfo foreignKeyInfo = selectedTable.ForeignKeys[columnName];
            string referencedTable = foreignKeyInfo.ReferencedTable;
            string referencedColumn = foreignKeyInfo.ReferencedColumn;

            string displayColumn = dbHelper.ExecuteScalar(Queries.GetReferencedDisplayColumn(referencedTable)).ToString();
            DataTable dataSource = dbHelper.ExecuteQuery(Queries.GetReferencedTableData(referencedColumn, displayColumn, referencedTable));

            dataSource.Rows.InsertAt(dataSource.NewRow(), 0);
            return CreateForeignComboBox(columnName, dataSource, displayColumn, referencedColumn);
        }

        public void ClearTempControls()
        {
            PictureBox.Image = null;
            ColumnsControlPanel.Controls.Clear();
            Control uniteButton = ButtonPanel.Controls.Cast<Control>().FirstOrDefault(c => c is Button && c.Text == "Ühenda väljad");
            if (uniteButton != null) { ButtonPanel.Controls.Remove(uniteButton); }
        }

        private Control CreateInputField(string columnName, string dataType)
        {
            if (dataType == "int")
            {
                return CreateNumericUpDown(columnName);
            }
            else if (dataType == "datetime")
            {
                return CreateDateTimePicker(columnName);
            }
            else if (dataType == "date")
            {
                return CreateDatePicker(columnName);
            }
            else if (dataType == "decimal")
            {
                return CreateDecimalNumericUpDown(columnName);
            }
            else if (dataType == "bit")
            {
                return CreateBitComboBox(columnName);
            }
            else
            {
                return CreateTextBox(columnName);
            }
        }
    }
}
