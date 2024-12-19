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
    public class DatabaseQueryHelper
    {
        static string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\"));
        static string db_name = "Andmebaas.mdf";
        static string db_path = Path.Combine(projectRoot, "Andmebaas.mdf");
        static string imageFolder = Path.Combine(projectRoot, @"Posters");

        SqlConnection conn;
        SqlCommand cmd;
        SqlDataAdapter adapter;

        public DatabaseQueryHelper ()
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
        public int GetLastInsertedId()
        {
            string query = "SELECT SCOPE_IDENTITY()";
            object result = ExecuteScalar(query); // ExecuteScalar возвращает одиночное значение
            return result != null ? Convert.ToInt32(result) : 0;
        }
        /// Executes an SQL query and returns the results as a DataTable.
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

        /// SQL command without returning a result (e.g. INSERT, UPDATE, DELETE)
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
        /// Executes an SQL query and returns a single value.
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
