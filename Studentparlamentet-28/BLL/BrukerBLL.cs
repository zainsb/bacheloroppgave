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
        // BrukertestForbedring
        public bool sjekkValgStemme(int ID)
        {
            var brukerdal = new DbDal();
            return brukerdal.sjekkValgStemme(ID);
        }
        public string kjørerValg()
        {
            var brukerdal = new DbDal();
            return brukerdal.kjørerValg();
        }
        //Preferansevalg
        public string HarBrukerStemtSTV(string brukernavn)
        {
            var brukerdal = new DbDal();
            return brukerdal.HarBrukerStemtSTV(brukernavn);
        }
        public string startLagretPreferansevalg(int valgtypeid, string beskrivelse, int antallRepresentanter)
        {
            var brukerDal = new DbDal();
            return brukerDal.startLagretPreferansevalg(valgtypeid, beskrivelse, antallRepresentanter);
        }
        public Valgtyper PreferansevalgPågår()
        {
            var brukderDal = new DbDal();
            return brukderDal.PreferansevalgPågår();
        }

        public bool preferansevalgSvar(string kandidatEn, string kandidatTo, string kandidatTre, string kandidatFire,
                                       string kandidatFem, string kandidatSeks, string kandidatSju, string kandidatÅtte,
                                       string kandidatNi, string kandidatTi, string kandidatElleve, string kandidatTolv)
        {
            var brukderDal = new DbDal();
            return brukderDal.preferansevalgSvar(kandidatEn, kandidatTo, kandidatTre, kandidatFire, kandidatFem,
                                                 kandidatSeks, kandidatSju, kandidatÅtte, kandidatNi, kandidatTi,
                                                 kandidatElleve, kandidatTolv);
        }

        public List<KandidatSTV> hentKandidaterPreferansevalgBruker()
        {
            var brukerdal = new DbDal();
            return brukerdal.hentKandidaterPreferansevalgBruker();
        }
        public string stvBrukerStart()
        {
            var brukderDal = new DbDal();
            return brukderDal.stvBrukerStart();
        }
        public Valgtyper stopPreferansevalg()
        {
            var brukderDal = new DbDal();
            return brukderDal.stopPreferansevalg();
        }
        public int AntallstemtPreferansevalg(int valgtypeid)
        {
            var brukerdal = new DbDal();
            return brukerdal.AntallstemtPreferansevalg(valgtypeid);
        }
        public int PreferansevalgAntallKandidater(int valgtypeid)
        {
            var brukerdal = new DbDal();
            return brukerdal.PreferansevalgAntallKandidater(valgtypeid);
        }
        public int PreferansevalgAntallDeltatt(int valgtypeid)
        {
            var brukerdal = new DbDal();
            return brukerdal.PreferansevalgAntallDeltatt(valgtypeid);
        }
        public List<KandidatSTV> BeregnPreferansevalgResultat(int valgtypeid)
        {
            var brukderDal = new DbDal();
            return brukderDal.BeregnPreferansevalgResultat(valgtypeid);
        }
        public Preferansevalg hentPreferanseValg(int id)
        {
            var brukerDal = new DbDal();
            return brukerDal.hentPreferanseValg(id);
        }
        public void BeregnStemmetallFørsteRunde(int valgtypeid)
        {
            var brukderDal = new DbDal();
            brukderDal.BeregnStemmetallFørsteRunde(valgtypeid);
        }
        public string lagreNyttPreferansevalg(string beskrivelse, int antallRepresentanter)
        {
            var brukderDal = new DbDal();
            return brukderDal.lagreNyttPreferansevalg(beskrivelse, antallRepresentanter);
        }
        public string NullstillKandidatliste(int valgtypeid)
        {
            var brukerdal = new DbDal();
            return brukerdal.NullstillKandidatliste(valgtypeid);
        }
        public List<KandidatSTV> hentKandidatlisteMedID(int valgtypeid)
        {
            var brukerdal = new DbDal();
            return brukerdal.hentKandidatlisteMedID(valgtypeid);
        }
        public bool slettPreferanseValg(int id)
        {
            var brukerDal = new DbDal();
            bool ok = brukerDal.slettPreferanseValg(id);
            return ok;
        }
        public List<Preferansevalg> hentAlleUtfortePreferansevalg()
        {
            var brukerdal = new DbDal();
            return brukerdal.hentAlleUtfortePreferansevalg();
        }
        public bool slettUtførtePreferanseValg(int id)
        {
            var brukerDal = new DbDal();
            bool ok = brukerDal.slettUtførtePreferanseValg(id);
            return ok;
        }
        public List<Preferansevalg> hentAlleIkkeUtfortePreferansevalg()
        {
            var brukerdal = new DbDal();
            return brukerdal.hentAlleIkkeUtfortePreferansevalg();
        }
        public void slettKandidatFraListe(int id)
        {
            var brukerdal = new DbDal();
            brukerdal.slettKandidatFraListe(id);
        }
        public string lagreKandidatSTV(string id)
        {
            var brukerDal = new DbDal();
            return brukerDal.lagreKandidatSTV(id);
        }
        public string startPreferansevalg(string beskrivelse, int antallRepresentanter)
        {
            var brukderDal = new DbDal();
            return brukderDal.startPreferansevalg(beskrivelse, antallRepresentanter);
        }
        public bool startPreferansevalg(Valgtyper valg)
        {
            var brukerDal = new DbDal();
            return brukerDal.startPreferansevalg(valg);
        }
        public Preferansevalg PreferansevalgHarStartet()
        {
            var brukerdal = new DbDal();
            return brukerdal.PreferansevalgHarStartet();
        }
        public bool PreferansevalgKjorer()
        {
            var brukderDal = new DbDal();
            return brukderDal.PreferansevalgKjorer();
        }
        public List<KandidatSTV> listeKandidaterIkkeSatt()
        {
            var brukderDal = new DbDal();
            return brukderDal.listeKandidaterIkkeSatt();
        }

        //Personvalg
        public List<PersonvalgKandidatResultat> hentPersonvalgResultatFane(int id)
        {
            var brukerDal = new DbDal();
            List<PersonvalgKandidatResultat> tabell = brukerDal.hentPersonvalgResultatFane(id);
            return tabell;
        }
        public void SlettPersonvalg(int id)
        {
            var brukerDal = new DbDal();
            brukerDal.SlettPersonvalg(id);
        }
        public bool StemPersonvalg(int id)
        {
            var brukerDal = new DbDal();
            return brukerDal.StemPersonvalg(id);
        }
        public List<Personvalg> hentKandidaterPersonvalg(int ID)
        {
            var brukerDal = new DbDal();
            List<Personvalg> tabell = brukerDal.hentKandidaterPersonvalg(ID);
            return tabell;
        }
        public int hentAntallStemtePersonvalg_int()
        {
            var brukerDal = new DbDal();
            int antall = brukerDal.hentAntallStemtePersonvalg_int();
            return antall;
        }
        public int antallPersonvalgStemmer(int ID)
        {
            var brukerDal = new DbDal();
            return brukerDal.antallPersonvalgStemmer(ID);
        }
        public int PersonvalgKjørerID()
        {
            var brukerDal = new DbDal();
            return brukerDal.PersonvalgKjørerID();
        }
        public bool stopPersonvalg()
        {
            var brukerDal = new DbDal();
            return brukerDal.stopPersonvalg();
        }
        public bool startPersonvalg(int ID)
        {
            var brukerDal = new DbDal();
            return brukerDal.startPersonvalg(ID);
        }
        public bool PersonvalgKjorer()
        {
            var brukerDal = new DbDal();
            return brukerDal.PersonvalgKjorer();
        }
        public int finnValgtypeID(int id)
        {
            var brukerDal = new DbDal();
            int ok = brukerDal.finnValgtypeID(id);
            return ok;
        }
        public bool Kandidatendre(Kandidat innkandidat)
        {
            var brukerDal = new DbDal();
            bool ok = brukerDal.Kandidatendre(innkandidat);
            return ok;
        }
        public bool SjekkPassord(Kandidat innkandidat, string brukernavn)
        {
            var brukerDal = new DbDal();
            bool ok = brukerDal.SjekkPassord(innkandidat, brukernavn);
            return ok;
        }
        public bool SjekkPassordEng(Kandidat innkandidat, string brukernavn)
        {
            var brukerDal = new DbDal();
            bool ok = brukerDal.SjekkPassordEng(innkandidat, brukernavn);
            return ok;
        }
        public bool KandidatSlett(Kandidat innkandidat)
        {
            var brukerDal = new DbDal();
            bool ok = brukerDal.KandidatSlett(innkandidat);
            return ok;
        }
        public bool KandidatendreFornavn(Kandidat innkandidat)
        {
            var brukerDal = new DbDal();
            bool ok = brukerDal.KandidatendreFornavn(innkandidat);
            return ok;
        }
        public bool KandidatendreEtternavn(Kandidat innkandidat)
        {
            var brukerDal = new DbDal();
            bool ok = brukerDal.KandidatendreEtternavn(innkandidat);
            return ok;
        }
        public List<String> hentKandidaterLagretNavn(int ID)
        {
            var brukerDal = new DbDal();
            List<String> tabell = brukerDal.hentKandidaterLagretNavn(ID);
            return tabell;
        }
        public List<PersonvalgResultat> hentPersonvalgResultat()
        {
            var brukerDal = new DbDal();
            List<PersonvalgResultat> tabell = brukerDal.hentPersonvalgResultat();
            return tabell;
        }
        public List<PersonvalgResultat> hentPersonvalgResultatEng()
        {
            var brukerDal = new DbDal();
            List<PersonvalgResultat> tabell = brukerDal.hentPersonvalgResultatEng();
            return tabell;
        }
        public bool LagrePersonvalgResultat()
        {
            var brukerDal = new DbDal();
            bool ok = brukerDal.LagrePersonvalgResultat();
            return ok;
        }
        public bool LagrePersonvalg()
        {
            var brukerDal = new DbDal();
            bool ok = brukerDal.LagrePersonvalg();
            return ok;
        }
        public bool slettKandidat(int id)
        {
            var brukerDal = new DbDal();
            bool ok = brukerDal.slettKandidat(id);
            return ok;
        }
        public List<Kandidat> hentKandidater()
        {
            var brukerDal = new DbDal();
            List<Kandidat> tabell = brukerDal.hentKandidater();
            return tabell;
        }
        public bool leggtilPersonvalgKandidat(Kandidat innKandidat)
        {
            var brukerDal = new DbDal();
            return brukerDal.leggtilPersonvalgKandidat(innKandidat);
        }
        public bool leggtilPersonvalgKandidatEng(Kandidat innKandidat)
        {
            var brukerDal = new DbDal();
            return brukerDal.leggtilPersonvalgKandidatEng(innKandidat);
        }

        public Valgtyper hentValg(int id)
        {
            var brukerDal = new DbDal();
            Valgtyper valgtyper = brukerDal.hentValg(id);
            return valgtyper;
        }
        public bool finnesdetvalg_ID(int id)
        {
            var brukerDal = new DbDal();
            return brukerDal.finnesdetvalg_ID(id);
        }
       public bool finnesdetvalg()
        {
            var brukerDal = new DbDal();
            return brukerDal.finnesdetvalg();
        }
        public int antallstemteVotering(int id)
        {
            var brukerDal = new DbDal();
            return brukerDal.antallstemteVotering(id);
        }
        public int antallFor(int id)
        {
            var brukerDal = new DbDal();
            return brukerDal.antallFor(id);
        }
        public int antallMot(int id)
        {
            var brukerDal = new DbDal();
            return brukerDal.antallMot(id);
        }
        public int antallBlank(int id)
        {
            var brukerDal = new DbDal();
            return brukerDal.antallBlank(id);
        }
        public int hentsisteVotering()
        {
            var brukerDal = new DbDal();
            return brukerDal.hensisteVotering();
        }

        public int hentAntallStemte_int()
        {
            var brukerDal = new DbDal();
            int antall = brukerDal.hentAntallStemte_int();
            return antall;
        }
        public int hentAntallBrukere_int()
        {
            var brukerDAl = new DbDal();
            int antall = brukerDAl.hentAntallBrukere_int();
            return antall;
        }
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
        public bool VoteringKjorer()
        {
            var brukerDal = new DbDal();
            return brukerDal.VoteringKjorer();
        }

        public string hentAntallStemt(int id)
        {
            var brukerDal = new DbDal();
            return brukerDal.hentAntallStemt(id);
        }
        public int antallValgFullført()
        {
            var brukerDal = new DbDal();
            return brukerDal.antallValgFullført();
        }
        
    }
}
