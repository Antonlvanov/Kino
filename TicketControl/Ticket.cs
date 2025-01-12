using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kino.TicketControl
{
    public class Ticket
    {
        public int? pilet_id { get; set; }
        public string pileti_nimi { get; set; }
        public int klient_id { get; set; }
        public int seanss_id { get; set; }
        public int koht_id { get; set; }
        public decimal hind { get; set; }
    }
}
