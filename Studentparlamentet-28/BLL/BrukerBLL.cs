using Studentparlamentet_28.DAL;
using Studentparlamentet_28.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Studentparlamentet_28.BLL
{
    public class BrukerBLL
    {
        public Bruker hentEnBruker(string id)
        {
            var brukerDal = new DbDal();
            return brukerDal.hentEnBruker(id);
        }

        public bool bruker_i_db(Bruker innPerson)
        {
            var brukerDal = new DbDal();
            return brukerDal.bruker_i_db(innPerson);
        }
    }
}
