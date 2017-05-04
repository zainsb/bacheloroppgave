using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Studentparlamentet_28.Models
{
    public class OverførtTilSeddel
    {
        public int seddelID { get; set; }
        public string kandidatnrEn { get; set; }
        public string kandidatnrTo { get; set; }
        public string kandidatnrTre { get; set; }
        public string kandidatnrFire { get; set; }
        public string kandidatnrFem { get; set; }
        public string kandidatnrSeks { get; set; }
        public string kandidatnrSju { get; set; }
        public string kandidatnrÅtte { get; set; }
        public string kandidatnrNi { get; set; }
        public string kandidatnrTi { get; set; }
        public string kandidatnrElleve { get; set; }
        public string kandidatnrTolv { get; set; }

        public string overførtFra { get; set; }
        public string overførtTil { get; set; }
        public double overførtSum { get; set; }
        public int rundenr { get; set; }
        public bool bleValgt { get; set; }
    }
}
