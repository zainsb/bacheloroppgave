using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Studentparlamentet_28.Models
{
    public class PersonvalgStemmer
    {
        public int id { get; set; }
        public int valgtypeid { get; set; }
        public string fornavn { get; set; }
        public string etternavn { get; set; }

    }

}
