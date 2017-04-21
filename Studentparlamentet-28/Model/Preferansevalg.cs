using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Studentparlamentet_28.Models
{
    public class Preferansevalg
    {
        public int id { get; set; }


        public string beskrivelse { get; set; }
        public int antallrepresentanter { get; set; }
        public bool utført { get; set; }
        public int valgtypeid { get; set; }
    }
}
