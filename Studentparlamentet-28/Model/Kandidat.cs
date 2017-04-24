using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Studentparlamentet_28.Models
{
    public class Kandidat
    {
        [Required(ErrorMessage = "Write Password")]
        public string passordEng { get; set; }
        [Required(ErrorMessage = "Passord må oppgis")]
        public string passord { get; set; }
        [Required(ErrorMessage = "KandidatID må oppgis")]
        public int id { get; set; }
        public string fornavn { get; set; }
              
        public string etternavn { get; set; }

        public int valgtypeid { get; set; }

        public bool lagret { get; set; }
        [Required(ErrorMessage = "Oppgi fornavn")]
        public string fornavn_Required { get; set; }
        [Required(ErrorMessage = "Oppgi etternavn")]
        public string etternavn_Required { get; set; }

        [Required(ErrorMessage = "Fill in firstname")]
        public string fornavn_RequiredEng { get; set; }
        [Required(ErrorMessage = "Fill in lastname")]
        public string etternavn_RequiredEng { get; set; }

    }


}