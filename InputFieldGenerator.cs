using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Kino
{
    public class InputFieldGenerator
    {
        private DatabaseQueryHelper dbHelper;

        private Font labelFont = new Font("Microsoft Sans Serif", 13f);
        private Font fieldFont = new Font("Microsoft Sans Serif", 9.5f);

        static string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\"));
        static string imageFolder = Path.Combine(projectRoot, @"Posters");

        public InputFieldGenerator(DatabaseQueryHelper dbHelper)
        {
            this.dbHelper = dbHelper;
        }

        public void GenerateFields(FlowLayoutPanel panel, string tableName)
        {
            panel.Controls.Clear();

            DataTable columnsTable = GetColumnsInfo(tableName);
            DataTable foreignKeysTable = GetForeignKeysInfo(tableName);

            foreach (DataRow column in columnsTable.Rows.Cast<DataRow>().Skip(1))
            {
                string columnName = column["COLUMN_NAME"].ToString();
                string dataType = column["DATA_TYPE"].ToString();

                Panel fieldPanel = CreateFieldPanel(panel.Width);
                Label label = CreateLabel(columnName);

                Control inputField = CreateInputField(columnName, dataType, foreignKeysTable);
                inputField.Margin = new Padding(0, 15, 0, 0);
                inputField.Location = new Point(label.Width + 10, 0);

                // controls
                fieldPanel.Controls.Add(label);
                fieldPanel.Controls.Add(inputField);
                panel.Controls.Add(fieldPanel);
            }
            if (foreignKeysTable.Rows.Count > 0)
            {
                Button uniteButton = CreateUniteButton(panel.Width, panel.Height);
                uniteButton.Click += (s, e) => TransformForeignKeyFields(panel, foreignKeysTable);
                panel.Controls.Add(uniteButton);
            }
        }


        private void TransformForeignKeyFields(FlowLayoutPanel panel, DataTable foreignKeysTable)
        {
            foreach (Control control in panel.Controls)
            {
                if (control is Panel fieldPanel)
                {
                    Label label = fieldPanel.Controls.OfType<Label>().FirstOrDefault();
                    Control inputField = fieldPanel.Controls.OfType<Control>().FirstOrDefault(c => !(c is Label));

                    if (inputField != null && foreignKeysTable.Select($"COLUMN_NAME = '{inputField.Name}'").Length > 0)
                    {
                        // Replace input field with ComboBox
                        DataRow foreignKeyRow = foreignKeysTable.Select($"COLUMN_NAME = '{inputField.Name}'").First();
                        Control comboBox = (ComboBox)CreateComboBox(inputField.Name, foreignKeyRow);
                        comboBox.Location = inputField.Location;

                        fieldPanel.Controls.Remove(inputField);
                        fieldPanel.Controls.Add(comboBox);
                        // Update label text
                        if (label != null)
                        {
                            label.Text = label.Text.Replace(" Id", "").TrimEnd();
                        }
                    }
                }
            }
        }
        // create mathods
        private ComboBox CreateComboBox(string columnName, DataRow foreignKeyRow)
        {
            string referencedTable = foreignKeyRow["REFERENCED_TABLE_NAME"].ToString();
            string referencedColumn = foreignKeyRow["REFERENCED_COLUMN_NAME"].ToString();

            string displayColumnQuery = $@"
                SELECT COLUMN_NAME 
                FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_NAME = '{referencedTable}' AND COLUMN_NAME NOT LIKE '%id'";
            DataTable displayColumns = dbHelper.ExecuteQuery(displayColumnQuery);
            string displayColumn = displayColumns.Rows[0]["COLUMN_NAME"].ToString();

            string query = $"SELECT {referencedColumn}, {displayColumn} FROM {referencedTable}";
            DataTable dataSource = dbHelper.ExecuteQuery(query);

            DataRow emptyRow = dataSource.NewRow();
            emptyRow[referencedColumn] = DBNull.Value;
            emptyRow[displayColumn] = "";
            dataSource.Rows.InsertAt(emptyRow, 0);

            return new ComboBox
            {
                Name = columnName,
                Width = 160,
                Font = fieldFont,
                DataSource = dataSource,
                DisplayMember = displayColumn,
                ValueMember = referencedColumn
            };
        }
        private Control CreateInputField(string columnName, string dataType, DataTable foreignKeysTable)
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
            else if(dataType == "bit")
            {
                return CreateBitComboBox(columnName);
            }
            else
            {
                return CreateTextBox(columnName);
            }
        }
        private Button CreateUniteButton(int panelWidth, int panelHeight)
        {
            return new Button
            {
                Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                Name = "unite_btn",
                AutoSize = true,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                TabIndex = 0,
                Text = "Ühenda väljad",
                UseVisualStyleBackColor = true
            };
        }

        // QUERIES
        private DataTable GetColumnsInfo(string tableName)
        {
            string query = $"SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}'";
            return dbHelper.ExecuteQuery(query);
        }

        private DataTable GetForeignKeysInfo(string tableName)
        {
            string query = $@"
                SELECT
                    fk.name AS CONSTRAINT_NAME,
                    OBJECT_NAME(fk.parent_object_id) AS TABLE_NAME,
                    c.name AS COLUMN_NAME,
                    OBJECT_NAME(fk.referenced_object_id) AS REFERENCED_TABLE_NAME,
                    rc.name AS REFERENCED_COLUMN_NAME
                FROM sys.foreign_keys AS fk
                INNER JOIN sys.foreign_key_columns AS fkc
                    ON fk.object_id = fkc.constraint_object_id
                INNER JOIN sys.columns AS c
                    ON fkc.parent_object_id = c.object_id AND fkc.parent_column_id = c.column_id
                INNER JOIN sys.columns AS rc
                    ON fkc.referenced_object_id = rc.object_id AND fkc.referenced_column_id = rc.column_id
                WHERE OBJECT_NAME(fk.parent_object_id) = '{tableName}'";
            return dbHelper.ExecuteQuery(query);
        }

        /// EZ BELOW
        private Panel CreateFieldPanel(int width)
        {
            return new Panel
            {
                Width = width - 10,
                Height = 30
            };
        }

        private Label CreateLabel(string columnName)
        {
            return new Label
            {
                Text = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(columnName.Replace("_", " ")),
                Font = labelFont,
                AutoSize = false,
                Width = 120,
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        private NumericUpDown CreateNumericUpDown(string columnName)
        {
            return new NumericUpDown
            {
                Name = columnName,
                Width = 60,
                Font = fieldFont,
                Maximum = decimal.MaxValue
            };
        }

        private DateTimePicker CreateDateTimePicker(string columnName)
        {
            return new DateTimePicker
            {
                Name = columnName,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "yyyy-MM-dd HH:mm:ss",
                Width = 160,
                Font = fieldFont
            };
        }

        private DateTimePicker CreateDatePicker(string columnName)
        {
            return new DateTimePicker
            {
                Name = columnName,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "yyyy-MM-dd",
                ShowUpDown = true,
                Width = 160,
                Font = fieldFont
            };
        }

        private CheckBox CreateCheckBox(string columnName)
        {
            return new CheckBox
            {
                Name = columnName,
                Size = new Size(25,25),
                Font = fieldFont
            };
        }

        private ComboBox CreateBitComboBox(string columnName)
        {
            return new ComboBox
            {
                Name = columnName,
                Width = 60,
                Font = fieldFont,
                Items = { "", "True", "False" }
            };
        }

        private TextBox CreateTextBox(string columnName)
        {
            return new TextBox
            {
                Name = columnName,
                Width = 200,
                Font = fieldFont
            };
        }
    }
}
