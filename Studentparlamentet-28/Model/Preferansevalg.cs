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
        public string kvoteKlasseEn { get; set; }
        public string kvoteKlasseTo { get; set; }
        public int klasseEnProsent { get; set; }
        public int klasseToProsent { get; set; }
        public int valgtypeid { get; set; }
    }
}
