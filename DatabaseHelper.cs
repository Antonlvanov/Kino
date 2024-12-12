using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kino
{
    public class DatabaseHelper
    {
        static string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\"));
        static string db_name = "Andmebaas.mdf";
        static string db_path = Path.Combine(projectRoot, "Andmebaas.mdf");
        static string imageFolder = Path.Combine(projectRoot, @"Posters");

        SqlConnection conn;
        SqlCommand cmd;
        SqlDataAdapter adapter;

        public DatabaseHelper ()
        {
            FindDB();
        }
        public void FindDB()
        {
            if (File.Exists(db_path))
            {
                conn = new SqlConnection($@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={db_path};Integrated Security=True");
            }
        }
        /// <summary>
        /// Выполняет SQL-запрос и возвращает результаты в виде DataTable.
        /// </summary>
        public DataTable ExecuteQuery(string query, Dictionary<string, object> parameters = null)
        {
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                using (cmd = new SqlCommand(query, conn))
                {
                    // Добавляем параметры, если они есть
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue(param.Key, param.Value);
                        }
                    }
                    using (adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable resultTable = new DataTable();
                        adapter.Fill(resultTable);
                        return resultTable;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка выполнения запроса: {ex.Message}");
                return null;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// Выполняет SQL-команду без возврата результата (например, INSERT, UPDATE, DELETE).
        /// </summary>
        public bool ExecuteNonQuery(string query, Dictionary<string, object> parameters = null)
        {
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                using (cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue(param.Key, param.Value);
                        }
                    }
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка выполнения команды: {ex.Message}");
                return false;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
        /// <summary>
        /// Выполняет SQL-запрос и возвращает одиночное значение.
        /// </summary>
        public object ExecuteScalar(string query, Dictionary<string, object> parameters = null)
        {
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                using (cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue(param.Key, param.Value);
                        }
                    }

                    return cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка выполнения команды: {ex.Message}");
                return null;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
    }
}
