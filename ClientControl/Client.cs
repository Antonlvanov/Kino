using Kino.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kino.ClientControl
{
    public class Client
    {
        public string Klient_id { get; set; }
        public string Nimi { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
    }
}
