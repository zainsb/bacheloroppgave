using Studentparlamentet_28.DAL;
using Studentparlamentet_28.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Studentparlamentet_28.BLL
{
    public class BrukerBLL
    {
        public bool GenererBrukere(int antallBrukere)
        {
            var brukerDal = new DbDal();
            return brukerDal.GenererBrukere(antallBrukere);
        }

        public List<Bruker> hentData()
        {
            var brukerDal = new DbDal();
            List<Bruker> tabell = brukerDal.hentData();
            return tabell;
        }
        public Bruker lastNedListe()
        {
            var brukerDal = new DbDal();
            return brukerDal.lastNedListe();
        }
        public Bruker hentEnBruker(string id)
        {
            var brukerDal = new DbDal();
            return brukerDal.hentEnBruker(id);
        }
        public Admin hentEnAdmin(string id)
        {
            var brukerDal = new DbDal();
            return brukerDal.hentEnAdmin(id);
        }
        public Admin hentRolleAdmin(string rolle)
        {
            var brukerDal = new DbDal();
            return brukerDal.hentRolleAdmin(rolle);
        }
        public Bruker hentRolleBruker(string rolle)
        {
            var brukerDal = new DbDal();
            return brukerDal.hentRolleBruker(rolle);
        }

        public bool slettBruker(String id)
        {
            var brukerDal = new DbDal();
            return brukerDal.slettBruker(id);
        }

        public bool loggUtBruker(String id)
        {
            var brukerDal = new DbDal();
            return brukerDal.loggUtBruker(id);
        }

        public bool bruker_i_db(Bruker innPerson)
        {
            var brukerDal = new DbDal();
            return brukerDal.bruker_i_db(innPerson);
        }
        public bool admin_i_db(Bruker innAdmin)
        {
            var brukerDal = new DbDal();
            return brukerDal.admin_i_db(innAdmin);
        }
        public bool logg_ut_bruker(String id)
        {
            var brukerDal = new DbDal();

            return brukerDal.logg_ut_bruker(id);
        }
        public bool startVotering()
        {
            var brukerDal = new DbDal();
            return brukerDal.startVotering();
        }
        public bool stoppVotering()
        {
            var brukerDal = new DbDal();
            return brukerDal.stopVotering();
        }
        public bool voteringsvar(String svar)
        {
            var brukerDal = new DbDal();
            return brukerDal.voteringsvar(svar);
        }

        public string voteringBrukerStart()
        {
            var brukerDal = new DbDal();
            return brukerDal.voteringBrukerStart();
        }
        public List<Valgtyper> hentValgTyper()
        {
            var brukerDal = new DbDal();
            List<Valgtyper> valgtyper = brukerDal.hentValgTyper();
            return valgtyper;
        }
        public bool slettValg(int id)
        {
            var brukerDal = new DbDal();
            bool ok = brukerDal.slettValg(id);
            return ok;
        }
        public bool admin_i_db_innlogget(Bruker innAdmin, string brukernavn)
        {
            var brukerDal = new DbDal();
            return brukerDal.admin_i_db_innlogget(innAdmin, brukernavn);
        }


        public Valgtyper VoteringPågår()
        {
            var brukerDal = new DbDal();
            return brukerDal.VoteringPågår();
        }
        //Ny
        public void LagreBrukerStemt(Valgtyper valg, string brukernavn)
        {
            var brukerDal = new DbDal();
            brukerDal.LagreBrukerStemt(valg, brukernavn);
        }

        public string HarBrukerStemt(string brukernavn)
        {
            var brukerDal = new DbDal();
            return brukerDal.HarBrukerStemt(brukernavn);
        }

    }
}
