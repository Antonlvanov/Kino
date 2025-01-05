using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kino.Database
{
    public class dbHelper
    {
        SqlConnection conn;
        SqlCommand cmd;
        SqlDataAdapter adapter;

        public dbHelper()
        {
            FindDB();
        }
        public void FindDB()
        {
            if (File.Exists(DB_PATHS.DBPath))
            {
                conn = new SqlConnection($@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={DB_PATHS.DBPath};Integrated Security=True");
            }
        }
        public int GetLastInsertedId()
        {
            string query = "SELECT SCOPE_IDENTITY()";
            object result = ExecuteScalar(query);
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
    public static class DB_PATHS
    {
        public static string ProjectRoot => Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\"));
        public static string DBName => "Andmebaas.mdf";
        public static string DBPath => Path.Combine(ProjectRoot, "Database", DBName);
        public static string PosterFolder => Path.Combine(ProjectRoot, "Posters");
        public static string ImageFolder => Path.Combine(ProjectRoot, "Images");
    }

}
