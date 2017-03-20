using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Studentparlamentet_28.Models
{
    public class Valgtyper
    {
        public int valgtypeid { get; set; }
        public string valgtype { get; set; }
        public bool start { get; set; }

    }

}
