using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Studentparlamentet_28.Models
{
    public class PersonvalgResultat
    {
        public int id { get; set; }
        public int valgtypeid { get; set; }
        public int stemmer { get; set; }
        public int totalantallStemmer { get; set; }
        public string vinner { get; set; }
        public int antallkandidater { get; set; }

    }

}
