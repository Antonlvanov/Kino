using Kino.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kino.TicketControl
{
    public class TicketManager
    {
        private dbHelper dbHelper = new dbHelper();
        public bool AddTicketsToDatabase(List<Ticket> tickets)
        {
            bool allTicketsAdded = true;

            foreach (Ticket ticket in tickets)
            {
                try
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        { "@piletiNimi", ticket.pileti_nimi },
                        { "@klientId", ticket.klient_id },
                        { "@seanssId", ticket.seanss_id },
                        { "@kohtId", ticket.koht_id },
                        { "@hind", ticket.hind }
                    };

                    string query = @"
                    INSERT INTO piletid (pileti_nimi, klient_id, seanss_id, koht_id, hind)
                    VALUES (@piletiNimi, @klientId, @seanssId, @kohtId, @hind)";

                    dbHelper.ExecuteNonQuery(query, parameters);
                    Console.WriteLine($"Билет {ticket.pileti_nimi} успешно добавлен в базу данных.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при добавлении билета {ticket.pileti_nimi}: {ex.Message}");
                    allTicketsAdded = false;
                }
            }

            if (tickets.Count == 0)
            {
                Console.WriteLine("Ticket list is empty");
                return false;
            }

            return allTicketsAdded;
        }


    }
}
