using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Studentparlamentet_28.Models
{
    public class Kandidat
    {
        [Required(ErrorMessage = "Passord må oppgis")]
        public string passord { get; set; }
        [Required(ErrorMessage = "KandidatID må oppgis")]
        public int id { get; set; }
        public string fornavn { get; set; }
              
        public string etternavn { get; set; }

        public int valgtypeid { get; set; }

        public bool lagret { get; set; }
   
    }


}