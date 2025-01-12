using Kino.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kino.ClientControl
{
    public class ClientManager
    {
        private dbHelper dbHelper = new dbHelper();
        public int CreateKlientRecord(string nimi, string email, string telefon)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@nimi", nimi },
                { "@email", email },
                { "@telefon", telefon }
            };

            string query = @"
            INSERT INTO klientid (nimi, email, telefon)
            VALUES (@nimi, @email, @telefon);
            SELECT SCOPE_IDENTITY();";

            object result = dbHelper.ExecuteScalar(query, parameters);

            if (result != null && int.TryParse(result.ToString(), out int klientId))
            {
                return klientId;
            }
            else
            {
                throw new Exception("Не удалось создать клиента.");
            }
        }

        public Client GetKlientData(int klientId)
        {
            try
            {
                string query = @"
                        SELECT klient_id, nimi, email, telefon
                        FROM klientid
                        WHERE klient_id = @klientId";
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@klientId", klientId }
                };
                DataTable result = dbHelper.ExecuteQuery(query, parameters);
                if (result.Rows.Count > 0)
                {
                    DataRow row = result.Rows[0];
                    Client klient = new Client
                    {
                        Klient_id = row["klient_id"].ToString(),
                        Nimi = row["nimi"].ToString(),
                        Email = row["email"].ToString(),
                        Telefon = row["telefon"].ToString()
                    };
                    return klient;
                }
                else
                {
                    Console.WriteLine($"Клиент с ID {klientId} не найден.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении данных клиента: {ex.Message}");
                throw;
            }
        }

        public bool UpdateKlientData(int klientId, Client updatedKlient)
        {
            try
            {
                string query = @"
                    UPDATE klientid
                    SET nimi = @nimi,
                        email = @email,
                        telefon = @telefon
                    WHERE klient_id = @klientId";
                Dictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        { "@nimi", updatedKlient.Nimi },
                        { "@email", updatedKlient.Email },
                        { "@telefon", updatedKlient.Telefon },
                        { "@klientId", klientId }
                    };
                if (dbHelper.ExecuteNonQuery(query, parameters))
                {
                    Console.WriteLine("Данные клиента успешно обновлены.");
                    return true;
                }
                else
                {
                    Console.WriteLine($"Клиент с ID {klientId} не найден или данные не изменились.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении данных клиента: {ex.Message}");
                throw;
            }
        }

        public int? FindKlientId(string nimi = null, string email = null, string telefon = null)
        {
            try
            {
                string query = @"
                    SELECT klient_id
                    FROM klientid
                    WHERE (@nimi IS NULL OR nimi = @nimi)
                      AND (@email IS NULL OR email = @email)
                      AND (@telefon IS NULL OR telefon = @telefon)";

                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@nimi", (object)nimi ?? DBNull.Value },
                    { "@email", (object)email ?? DBNull.Value },
                    { "@telefon", (object)telefon ?? DBNull.Value }
                };

                DataTable result = dbHelper.ExecuteQuery(query, parameters);
                if (result.Rows.Count > 0)
                {
                    return Convert.ToInt32(result.Rows[0]["klient_id"]);
                }
                else
                {
                    Console.WriteLine("Клиент не найден.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при поиске клиента: {ex.Message}");
                throw;
            }
        }

    }
}
