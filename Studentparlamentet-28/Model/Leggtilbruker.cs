using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Studentparlamentet_28.Models
{
    public class Leggtilbruker
    {
        [Required(ErrorMessage = "Skriv inn antall")]
        // maks antall brukere 999
        [RegularExpression("^[0-9]{1,3}$", ErrorMessage = "Skriv inn antall")]
        public string antall { get; set; }

        [Required(ErrorMessage = "Fill inn number")]
        // maks antall brukere 999
        [RegularExpression("^[0-9]{1,3}$", ErrorMessage = "Fill inn number")]
        public string antalleng { get; set; }
    }

}