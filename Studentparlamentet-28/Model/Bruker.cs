using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Studentparlamentet_28.Models
{
    public class Bruker
    {
        public int id { get; set; }

        [Required(ErrorMessage = "Brukernavn må oppgis")]
        public string brukernavn { get; set; }

        [Required(ErrorMessage = "Passord må oppgis")]
        public string passord { get; set; }

        public bool innlogget { get; set; }

        public bool administrator { get; set; }
        public byte[] passwordhash { get; set; }


    }
}