using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Studentparlamentet_28.Models
{
    public class Votering
    {
        public int id { get; set; }
        public string svarfor { get; set; }
        public string svarmot { get; set; }
        public string svarblank { get; set; }
    }
}
