using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Studentparlamentet_28.Models
{
    public class Leggtilbruker
    {
    [Required(ErrorMessage = "Oppgi antall")]
        
        // maks antall brukere 999
        [RegularExpression("^[0-9]{1,3}$", ErrorMessage = "0-999")]
        public string antall { get; set; }
    }

}