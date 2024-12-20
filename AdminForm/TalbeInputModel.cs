using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kino
{
    public class TalbeInputModel
    {
        private Dictionary<string, Control> fieldControls;

        public TalbeInputModel(Dictionary<string, Control> controls)
        {
            fieldControls = controls;
        }

        public class ColumnInfo
        {
            public string Name { get; set; }
            public string DataType { get; set; }
        }

        public class ForeignKeyInfo
        {
            public string ColumnName { get; set; }
            public string ReferencedTable { get; set; }
            public string ReferencedColumn { get; set; }
        }


        // Получение значения поля
        public object GetValue(string fieldName)
        {
            if (fieldControls.TryGetValue(fieldName, out var control))
            {
                if (control is TextBox textBox)
                    return textBox.Text;
                else if (control is ComboBox comboBox)
                    return comboBox.SelectedIndex > 0 ? comboBox.SelectedItem : null;
                else if (control is NumericUpDown numericUpDown)
                    return numericUpDown.Value;
                else if (control is DateTimePicker dateTimePicker)
                    return dateTimePicker.Value;
            }
            return null;
        }

        // Установка значения поля
        public void SetValue(string fieldName, object value)
        {
            if (fieldControls.TryGetValue(fieldName, out var control))
            {
                if (control is TextBox textBox)
                    textBox.Text = value?.ToString();
                else if (control is ComboBox comboBox)
                    comboBox.SelectedItem = value;
                else if (control is NumericUpDown numericUpDown)
                    numericUpDown.Value = Convert.ToDecimal(value);
                else if (control is DateTimePicker dateTimePicker)
                    dateTimePicker.Value = Convert.ToDateTime(value);
            }
        }

        //public List<ColumnInfo> GetColumnsInfo()
        //{
        //    return _tableSchema.Columns.Select(c => new ColumnInfo
        //    {
        //        Name = c.Name,
        //        DataType = c.DataType
        //    }).ToList();
        //}

        //public List<ForeignKeyInfo> GetForeignKeysInfo()
        //{
        //    return _tableSchema.ForeignKeys.Select(fk => new ForeignKeyInfo
        //    {
        //        ColumnName = fk.ColumnName,
        //        ReferencedTable = fk.ReferencedTable,
        //        ReferencedColumn = fk.ReferencedColumn
        //    }).ToList();
        //}


        // Получение всех значений
        public Dictionary<string, object> GetAllValues()
        {
            return fieldControls.ToDictionary(
                kvp => kvp.Key,
                kvp => GetValue(kvp.Key)
            );
        }
    }

}
