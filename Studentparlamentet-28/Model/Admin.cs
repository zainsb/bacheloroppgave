using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Studentparlamentet_28.Models
{
    public class Admin
    {

        [Required(ErrorMessage = "Brukernavn må oppgis")]
        public string brukernavn { get; set; }

        [Required(ErrorMessage = "Passord må oppgis")]
        public string passord { get; set; }

        public bool innlogget { get; set; }

        public bool administrator { get; set; }

        public byte[] passwordhash { get; set; }

        [Display(Name = "Skriv inn riktig Epost")]
        [Required(ErrorMessage = "Skriv inn riktig Epost")]
        [EmailAddress(ErrorMessage = "Skriv inn riktig Epost")]
        public string email { get; set; }
        [Display(Name = "Not a valid Email")]
        [Required(ErrorMessage = "Not a valid Email")]
        [EmailAddress(ErrorMessage = "Not a valid Email")]
        public string emailEng { get; set; }
    }
}