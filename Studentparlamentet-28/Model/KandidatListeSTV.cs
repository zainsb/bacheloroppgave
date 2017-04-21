using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Studentparlamentet_28.Models
{
    public class KandidatSTV
    {
        public int kandidatListeID { get; set; }
        public string navn { get; set; }
        public int stemmetall { get; set; }
        public bool stemmetallsatt { get; set; }
        public int valgtypeid { get; set; }
    }
}