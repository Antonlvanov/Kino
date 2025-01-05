using Kino.Database;
using System;
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
        // Constructors
        private Button CreateButton(string text)
        {
            return new Button
            {
                Text = text,
                Font = DefaultSettings.ButtonFont,
                AutoSize = true,
                Margin = new Padding(5),
                BackColor = Color.White,
            };
        }
        private FlowLayoutPanel CreateFieldPanel(int cpHeight)
        {
            return new FlowLayoutPanel
            {
                Width = ColumnsControlPanel.Width,
                Height = cpHeight,
                BackColor = Color.LightGray
            };
        }

        private Label CreateLabel(string columnName)
        {
            return new Label
            {
                Margin = new Padding(0, 5, 0, 0),
                Text = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(columnName.Replace("_", " ")),
                Font = DefaultSettings.LabelFont,
                TextAlign = ContentAlignment.MiddleLeft,
            };
        }

        private NumericUpDown CreateNumericUpDown(string columnName)
        {
            return new NumericUpDown
            {
                Name = columnName,
                Font = DefaultSettings.InputFont,
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
                Font = DefaultSettings.InputFont
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
                Font = DefaultSettings.InputFont
            };
        }

        private CheckBox CreateCheckBox(string columnName)
        {
            return new CheckBox
            {
                Name = columnName,
                Size = new Size(25, 25),
                Font = DefaultSettings.InputFont
            };
        }

        private NumericUpDown CreateDecimalNumericUpDown(string columnName)
        {
            return new NumericUpDown
            {
                Name = columnName,
                Font = DefaultSettings.InputFont,
                Maximum = decimal.MaxValue,
                DecimalPlaces = 2,
                Increment = 0.01m,    
                Minimum = decimal.MinValue, 
                Value = 0m            
            };
        }

        private ComboBox CreateForeignComboBox(string columnName, DataTable dataSource, string displayColumn, string referencedColumn)
        {
            return new ComboBox
            {
                Name = columnName,
                Font = DefaultSettings.InputFont,
                DataSource = dataSource,
                DisplayMember = displayColumn,
                ValueMember = referencedColumn
            };
        }

        private ComboBox CreateBitComboBox(string columnName)
        {
            var comboBox = new ComboBox
            {
                Name = columnName,
                Font = DefaultSettings.InputFont,
                DisplayMember = "Key",   
                ValueMember = "Value" 
            };

            comboBox.DataSource = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("", null), 
                new KeyValuePair<string, object>("False", 0), 
                new KeyValuePair<string, object>("True", 1)  
            };

            return comboBox;
        }


        private TextBox CreateTextBox(string columnName)
        {
            return new TextBox
            {
                Anchor = AnchorStyles.Top,
                Dock = DockStyle.Top,
                Name = columnName,
                Font = DefaultSettings.InputFont
            };
        }

        // Methods

        private void FilteredTableGridView(List<RowData> filteredRows)
        {
            DataGridView.Rows.Clear();
            foreach (var row in filteredRows)
            {
                DataGridView.Rows.Add(
                    selectedTable.Columns.Select(c => row[c.Name]).ToArray()
                );
            }
        }

        private string GetControlValue(Control control)
        {
            if (control is TextBox textBox)
                return textBox.Text;
            if (control is ComboBox comboBox)
            {
                return comboBox.SelectedIndex > 0 ? comboBox.SelectedValue.ToString() : string.Empty;
            }
            if (control is NumericUpDown numericUpDown)
                return numericUpDown.Value > 0 ? numericUpDown.Value.ToString() : string.Empty;
            if (control is DateTimePicker dateTimePicker)
                return dateTimePicker.Value.ToString("yyyy-MM-dd HH:mm:ss");
            return string.Empty;
        }

        private void UpdateInputField(Control inputField, object value)
        {
            switch (inputField)
            {
                case TextBox textBox:
                    textBox.Text = value?.ToString() ?? string.Empty;
                    break;
                case NumericUpDown numericUpDown:
                    numericUpDown.Value = decimal.TryParse(value?.ToString(), out decimal numericValue) ? numericValue : 0;
                    break;
                case DateTimePicker dateTimePicker:
                    dateTimePicker.Value = DateTime.TryParse(value?.ToString(), out DateTime dateValue) ? dateValue : DateTime.Now;
                    break;
                case ComboBox comboBox:
                    UpdateComboBox(comboBox, value);
                    break;
            }
        }

        private void UpdateComboBox(ComboBox comboBox, object value)
        {
            if (value == null)
            {
                comboBox.SelectedIndex = -1;
                return;
            }
            if (comboBox.DataSource != null)
            {
                comboBox.SelectedValue = value;
            }
            if (value.ToString() == "True")
            {
                comboBox.SelectedValue = 1;
            }
            else if (value.ToString() == "False")
            {
                comboBox.SelectedValue = 0;
            }
        }

        private void UpdatePictureBox(string posterValue)
        {
            if (!string.IsNullOrEmpty(posterValue))
            {
                try
                {
                    PictureBox.Image = Image.FromFile(Path.Combine(DB_PATHS.PosterFolder, posterValue));
                    PictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Viga pildi laadimisel: {ex.Message}");
                }
            }
            else
            {
                PictureBox.Image = null;
            }
        }
        private void ClearFields()
        {
            foreach (Control control in ColumnsControlPanel.Controls)
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
                    numericUpDown.Text = string.Empty;
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
            foreach (Control columnPanel in ColumnsControlPanel.Controls)
            {
                foreach (Control control in columnPanel.Controls)
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
                    if (control is DateTimePicker dateTimePicker && string.IsNullOrWhiteSpace(dateTimePicker.Text))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
