using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace Kino.Database
{
    public class Table
    {
        public string TableName { get; private set; }
        public IReadOnlyList<ColumnMetadata> Columns { get; private set; }
        public IReadOnlyList<RowData> Rows { get; private set; }
        public IReadOnlyDictionary<string, ForeignKeyInfo> ForeignKeys { get; private set; }
        public Table(string tableName, DataTable columnsTable, DataTable foreignKeysTable, DataTable rowsTable)
        {
            TableName = tableName;

            Columns = columnsTable.AsEnumerable()
                .Select(row => new ColumnMetadata(
                    row.Field<string>("COLUMN_NAME"),
                    row.Field<string>("DATA_TYPE")))
                .ToList();

            Rows = rowsTable.AsEnumerable()
                .Select(row => new RowData(
                    Columns.ToDictionary(
                        column => column.Name,
                        column => row[column.Name]
                    )))
                .ToList();

            ForeignKeys = foreignKeysTable.AsEnumerable()
                .GroupBy(row => row.Field<string>("COLUMN_NAME"))
                .ToDictionary(
                    g => g.Key,
                    g => new ForeignKeyInfo(
                        g.First().Field<string>("REFERENCED_TABLE_NAME"),
                        g.First().Field<string>("REFERENCED_COLUMN_NAME")
                    )
                );
        }

        // Получение информации о колонке по имени
        public ColumnMetadata GetColumnMetadata(string columnName)
        {
            var column = Columns.FirstOrDefault(c => c.Name == columnName);
            if (column == null)
            {
                throw new ArgumentException($"Столбец '{columnName}' не найден.");
            }
            return column;
        }

        // Получение значения из строки по имени столбца
        public object GetValue(int rowIndex, string columnName)
        {
            if (rowIndex < 0 || rowIndex >= Rows.Count)
                throw new ArgumentOutOfRangeException(nameof(rowIndex));

            var row = Rows[rowIndex];
            return row[columnName]; // здесь предполагается, что RowData это коллекция ключ-значение
        }
    }

    // Класс для хранения метаданных о столбце (имя, тип)
    public class ColumnMetadata
    {
        public string Name { get; }
        public string DataType { get; }

        public ColumnMetadata(string name, string dataType)
        {
            Name = name;
            DataType = dataType;
        }
    }

    // Класс для работы с данными в строке
    public class RowData
    {
        private readonly Dictionary<string, object> _data;

        public RowData(Dictionary<string, object> data)
        {
            _data = data;
        }

        public object this[string columnName] => _data[columnName];
    }

    // Класс для хранения информации о внешнем ключе
    public class ForeignKeyInfo
    {
        public string ReferencedTable { get; }
        public string ReferencedColumn { get; }

        public ForeignKeyInfo(string referencedTable, string referencedColumn)
        {
            ReferencedTable = referencedTable;
            ReferencedColumn = referencedColumn;
        }

        public override string ToString() =>
            $"Table: {ReferencedTable}, Column: {ReferencedColumn}";
    }



}
