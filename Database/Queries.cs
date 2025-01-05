using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kino.Database
{
    public class Queries
    {
        public static string GetSessionsDataForDate()
        {
            return @"
                    SELECT 
                        s.seansi_nimi,
                        s.kuupaev,
                        s.alus_aeg,
                        s.lopp_aeg,
                        f.film_id,
                        f.pealkiri,
                        f.poster,
                        f.zanr,
                        f.kestus,
                        f.kirjeldus,
                        sa.saal_id,
                        sa.saali_nimi,
                        sa.ridade_arv,
                        sa.kohad_ridades
                    FROM seansid s
                    INNER JOIN filmid f ON s.film_id = f.film_id
                    INNER JOIN saalid sa ON s.saal_id = sa.saal_id
                    WHERE CONVERT(DATE, s.kuupaev, 105) = @Date";
        }

        public static string InsertUser()
        {
            return @"
            INSERT INTO kasutajad (userName, passwordHash, salt, email, role, klient_id)
            VALUES (@userName, @passwordHash, @salt, @email, @role, @klient_id);";
        }

        public static string GetAllUsers()
        {
            return @"
            SELECT userName, passwordHash, salt, email, role, klient_id
            FROM kasutajad;";
        }

        public string GetTableRows(string table)
        {
            return $"SELECT * FROM {table}";
        }

        public string GetAllTables()
        {
            return "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";
        }

        public string InsertIntoTalbe(string tableName, string columnList, string values)
        {
            return $"INSERT INTO {tableName} ({columnList}) VALUES ({values})";
        }

        public string DeleteByID(string tableName, string idColumn, string ids)
        {
            return $"DELETE FROM {tableName} WHERE {idColumn} IN ({ids})";
        }

        public string UpdateByID(string tableName, string columnList, string idColumn)
        {
            return $@"UPDATE {tableName} SET {columnList} WHERE {idColumn} = @{idColumn}";
        }

        public string GetColumnsInfo(string tableName)
        {
            return $"SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}'";
        }


        public string GetForeignKeysInfo(string tableName)
        {
            return $@"
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
        }

        public string GetReferencedDisplayColumn(string referencedTable)
        {
            return $@"
                SELECT TOP 1 COLUMN_NAME 
                FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_NAME = '{referencedTable}' AND COLUMN_NAME NOT LIKE '%id'";
        }

        public string GetReferencedTableData(string referencedColumn, string displayColumn, string referencedTable)
        {

            return $"SELECT {referencedColumn}, {displayColumn} FROM {referencedTable}";
        }
    }
}
