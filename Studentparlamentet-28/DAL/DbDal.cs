using Studentparlamentet_28.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Data.SqlClient;
using System.Data;
using System.Web.Security;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Data.Entity;

namespace Studentparlamentet_28.DAL
{
    public class DbDal
    {

        // Personvalg
        public List<PersonvalgKandidatResultat> hentPersonvalgResultatFane(int id)
        {
            using (var db = new BrukerContext())
            {
                var slettListe = db.PersonvalgKandidatResultat.Select(k => new PersonvalgKandidatResultat()
                {
                    id = k.ID,
                    valgtypeid = k.ValgtypeID,
                    fornavn = k.Fornavn,
                    etternavn = k.Etternavn,
                    stemmer = k.Stemmer
                }).ToList();

                int teller2 = slettListe.Count(k => k.valgtypeid == id);
                if (teller2 > 0)
                {


                    for (int i = 0; i < teller2; i++)
                    {
                        var slett = db.PersonvalgKandidatResultat.FirstOrDefault(k => k.ValgtypeID == id);
                        db.PersonvalgKandidatResultat.Remove(slett);
                    }
                }
                 var listeAvKandidater = db.Personvalger.Select(k => new Personvalg()
                    {
                        id = k.ID,
                        fornavn = k.Fornavn,
                        etternavn = k.Etternavn,
                        valgtypeid = k.ValgtypeID
                    }).ToList();
                    int teller = listeAvKandidater.Count();

                    var listeAvStemmesedler = db.PersonvalgResultatStemmer.Select(k => new PersonvalgStemmer()
                    {
                        id = k.ID,
                        valgtypeid = k.ValgtypeID,
                        fornavn = k.Fornavn,
                        etternavn = k.Etternavn

                    }).ToList();

                    for (int i = 0; i < teller; i++)
                    {
                        var itemToRemove = listeAvKandidater.FirstOrDefault(r => r.valgtypeid != id);
                        if (itemToRemove != null)
                        {
                            listeAvKandidater.Remove(itemToRemove);

                        }
                    }
                    int antallkandidater = listeAvKandidater.Count();
                    int[] stemmer = new int[antallkandidater];
                    string[] kandidater = new string[antallkandidater];
                    for (int i = 0; i < antallkandidater; i++)
                    {
                        var kandidat = listeAvKandidater.FirstOrDefault();
                        int antallstemmer = listeAvStemmesedler.Count(r => r.valgtypeid == id && r.fornavn == kandidat.fornavn && r.etternavn == kandidat.etternavn);
                        var test = db.PersonvalgKandidatResultat.FirstOrDefault(r => r.Fornavn == kandidat.fornavn && r.Etternavn == kandidat.etternavn && r.ValgtypeID == id);
                        if (test == null)
                        {
                            var kandidatResultat = new PersonvalgKandidatResultat_db()
                            {
                                ValgtypeID = id,
                                Fornavn = kandidat.fornavn,
                                Etternavn = kandidat.etternavn,
                                Stemmer = antallstemmer,
                            };
                            db.PersonvalgKandidatResultat.Add(kandidatResultat);
                            db.SaveChanges();
                        }
                        listeAvKandidater.Remove(kandidat);
                    }

                    var listeResultat = db.PersonvalgKandidatResultat.Select(k => new PersonvalgKandidatResultat()
                    {
                        id = k.ID,
                        valgtypeid = k.ValgtypeID,
                        fornavn = k.Fornavn,
                        etternavn = k.Etternavn,
                        stemmer = k.Stemmer

                    }).ToList();
                    teller = listeResultat.Count();
                    for (int i = 0; i < teller; i++)
                    {
                        var itemRemove = listeResultat.FirstOrDefault(r => r.valgtypeid != id);
                        if (itemRemove != null)
                        {
                            listeResultat.Remove(itemRemove);

                        }
                    }

                    return listeResultat;
                }
            
        }        
        public void SlettPersonvalg(int id)
        {
            var db = new BrukerContext();
            try
            {
                Valgtyper_db personvalg = db.Valgtyper.FirstOrDefault(b => b.ValgtypeID == id);
                db.Valgtyper.Remove(personvalg);
                db.SaveChanges();

                PersonvalgResultat_db personvalgResultat = db.PersonvalgResultat.FirstOrDefault(b => b.ValgtypeID == id);
                db.PersonvalgResultat.Remove(personvalgResultat);
                db.SaveChanges();

                int antallkandidater = db.Personvalger.Count(b => b.ValgtypeID == id);
                if(antallkandidater > 0)
                {
                for(int i = 0; i < antallkandidater; i++)
                    {
                        Personvalg_db fjernKandidat = db.Personvalger.FirstOrDefault(b => b.ValgtypeID == id);
                        db.Personvalger.Remove(fjernKandidat);
                    }   
           
                }

                int antallstemmer = db.Personvalger.Count(b => b.ValgtypeID == id);
                if(antallstemmer > 0)
                {
                    for(int i = 0; i < antallstemmer; i++)
                    {
                       PersonvalgResultatStemmer_db fjernStemme = db.PersonvalgResultatStemmer.FirstOrDefault(b => b.ValgtypeID == id);
                        db.PersonvalgResultatStemmer.Remove(fjernStemme);
                    }
                }

            }
            catch (Exception feil)
            {
               
            }
        }
        public bool StemPersonvalg(int id)
        {
            var db = new BrukerContext();
            int valgtypeID = PersonvalgKjørerID();
            var kandidat = db.Personvalger.FirstOrDefault(b => b.ID == id && b.ValgtypeID == valgtypeID);
            var stemmeseddel = new PersonvalgResultatStemmer_db()
            {
                ValgtypeID = valgtypeID,
                Fornavn = kandidat.Fornavn,
                Etternavn = kandidat.Etternavn
            };
            db.PersonvalgResultatStemmer.Add(stemmeseddel);
            db.SaveChanges();
            return true;



        }
        public List<Personvalg> hentKandidaterPersonvalg(int ID)
        {
            using (var db = new BrukerContext())
            {
                var listeAvKandidater = db.Personvalger.Select(k => new Personvalg()
                {
                    id = k.ID,
                    fornavn = k.Fornavn,
                    etternavn = k.Etternavn,
                    valgtypeid = k.ValgtypeID
                }).ToList();
                int teller = listeAvKandidater.Count();
                for (int i = 0; i < teller; i++)
                {
                    var itemToRemove = listeAvKandidater.FirstOrDefault(r => r.valgtypeid != ID);
                    if (itemToRemove != null)
                    {
                        listeAvKandidater.Remove(itemToRemove);
                    }
                }

                return listeAvKandidater;
            }
        }
        public int hentAntallStemtePersonvalg_int()
        {
            var db = new BrukerContext();
       
            var personvalg = db.Valgtyper.FirstOrDefault(b => b.Valgtype == "Personvalg" && b.Start == true);
            
            if (personvalg != null)
            {
                System.Int32 antall = db.PersonvalgResultatStemmer.Count(b => b.ValgtypeID == personvalg.ValgtypeID);
                return antall;
            }
            else return 0;
        }
        public int antallPersonvalgStemmer(int ID)
        {
            var db = new BrukerContext();
            var listeAvStemmer = db.PersonvalgResultatStemmer.Select(k => new PersonvalgStemmer()
            {
                valgtypeid = k.ValgtypeID,
                fornavn = k.Fornavn,
                etternavn = k.Etternavn
            }).ToList();
            int antall = listeAvStemmer.Count(b => b.valgtypeid == ID);
            return antall;
        }
        public int PersonvalgKjørerID()
        {
            var db = new BrukerContext();
            var personvalg = db.Valgtyper.FirstOrDefault(b => b.Valgtype == "Personvalg" && b.Start == true);
            return personvalg.ValgtypeID;
        }
        public bool stopPersonvalg()
        {
            var db = new BrukerContext();
            var listeAvPersonvalg = db.Valgtyper.Select(k => new Valgtyper()
            {
                valgtypeid = k.ValgtypeID,
                valgtype = k.Valgtype,
                start = k.Start            
            }).ToList();

            int teller = listeAvPersonvalg.Count();

            for(int i = 0; i < teller; i++)
            {
                var personvalg = listeAvPersonvalg.FirstOrDefault(b => b.valgtype == "Personvalg" && b.start == true);

                if (personvalg != null)
                {
                    Valgtyper_db personvalg2 = db.Valgtyper.FirstOrDefault(b => b.ValgtypeID == personvalg.valgtypeid && b.Valgtype == "Personvalg" && b.Start == true);
                    personvalg2.Start = (bool)false;
                    db.SaveChanges();
                    listeAvPersonvalg.Remove(personvalg);
                }
            }
            using (var db2 = new BrukerContext())
            {
                Valgtyper_db Personvalgkjorer = db2.Valgtyper.FirstOrDefault(b => b.Valgtype == "Personvalg" && b.Start == true);
                if (Personvalgkjorer != null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        public bool startPersonvalg(int ID)
        {
            var db = new BrukerContext();
            Valgtyper_db personvalg = db.Valgtyper.FirstOrDefault(b => b.ValgtypeID == ID && b.Valgtype == "Personvalg" && b.Start == false);
            if(personvalg != null)
            {
                personvalg.Start = (bool)true;
                db.SaveChanges();

                using (var db2 = new BrukerContext())
                {
                    Valgtyper_db Personvalgkjorer = db2.Valgtyper.FirstOrDefault(b => b.Valgtype == "Personvalg" && b.Start == true);
                    if (Personvalgkjorer != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
            
        }
        public bool PersonvalgKjorer()
        {
            using (var db = new BrukerContext())
            {
                Valgtyper_db Personvalgkjorer = db.Valgtyper.FirstOrDefault(b => b.Valgtype == "Personvalg" && b.Start == true);
                if (Personvalgkjorer != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public int finnValgtypeID(int id)
        {
            var db = new BrukerContext();
            try
            {
                Personvalg_db kandidat = db.Personvalger.FirstOrDefault(b => b.ID == id);
                return kandidat.ValgtypeID;

            }
            catch (Exception feil)
            {
                return 0;
            }

        }
        public bool Kandidatendre(Kandidat innkandidat)
        {
            var db = new BrukerContext();
            try
            {
                Personvalg_db kandidat = db.Personvalger.FirstOrDefault(b => b.ID == innkandidat.id);
                kandidat.Fornavn = innkandidat.fornavn;
                kandidat.Etternavn = innkandidat.etternavn;
                db.SaveChanges();
                return true;

            }
            catch (Exception feil)
            {
                return false;
            }
        }
        public bool SjekkPassord(Kandidat innkandidat, string brukernavn)
        {
            using (var db = new BrukerContext())
            {

                byte[] passwordhash = lagHash(innkandidat.passord);
                
                Admin_db funnetBruker = db.AdminBrukere.FirstOrDefault(b => b.Passord == passwordhash && b.Brukernavn == brukernavn);
                if (funnetBruker == null) // Sjekker om bruker finnes i systemet og sjekker om bruker allerede er innlogget
                {
                    return false;
                }
                else if (funnetBruker.Innlogget == (bool)true && funnetBruker.Administrator == (bool)true)
                {
                    return true;

                }
                else
                {
                    return false; // ikke en administrator bruker 
                }

            }
        }
        public bool KandidatSlett(Kandidat innkandidat)
        {
            var db = new BrukerContext();
            try
            {
                Personvalg_db kandidat = db.Personvalger.FirstOrDefault(b => b.ID == innkandidat.id);
                db.Personvalger.Remove(kandidat);
                db.SaveChanges();
                return true;
              
            }
            catch (Exception feil)
            {
                return false;
            }
        
        }
        public bool KandidatendreFornavn(Kandidat innkandidat)
        {
            var db = new BrukerContext();
            try
            {
                Personvalg_db kandidat = db.Personvalger.FirstOrDefault(b => b.ID == innkandidat.id);
                kandidat.Fornavn = innkandidat.fornavn;
                db.SaveChanges();
                return true;

            }
            catch (Exception feil)
            {
                return false;
            }
        }
        public bool KandidatendreEtternavn(Kandidat innkandidat)
        {
            var db = new BrukerContext();
            try
            {
                Personvalg_db kandidat = db.Personvalger.FirstOrDefault(b => b.ID == innkandidat.id);
                kandidat.Etternavn = innkandidat.etternavn;
                db.SaveChanges();
                return true;

            }
            catch (Exception feil)
            {
                return false;
            }
        }
        public List<String> hentKandidaterLagretNavn(int ID)
        {
            using (var db = new BrukerContext())
            {
                var listeAvKandidater = db.Personvalger.Select(k => new Kandidat()
                {
                    id = k.ID,
                    fornavn = k.Fornavn,
                    etternavn = k.Etternavn,
                    lagret = k.Lagret,
                    valgtypeid = k.ValgtypeID
                }).ToList();
                int teller = listeAvKandidater.Count();

                for (int i = 0; i < teller; i++)
                {
                    var itemToRemove = listeAvKandidater.FirstOrDefault(r => r.valgtypeid != ID);
                    if (itemToRemove != null)
                    {
                        listeAvKandidater.Remove(itemToRemove);
                    }
                }
                int teller2 = listeAvKandidater.Count();
                List <string> stringarray = new List<string>();
                for (int i = 0; i < teller2; i++)
                {
                    stringarray.Add("Kandidat ID : " + listeAvKandidater.FirstOrDefault().id + " " + "Navn : " + listeAvKandidater.FirstOrDefault().fornavn + " " + listeAvKandidater.FirstOrDefault().etternavn);
                    var remove = listeAvKandidater.FirstOrDefault();
                    if(remove != null)
                    {
                        listeAvKandidater.Remove(remove);
                    }
                          
                }
                return stringarray;
            }
        }

        public List<Kandidat> hentKandidaterLagret(int ID)
        {
            using (var db = new BrukerContext())
            {
                var listeAvKandidater = db.Personvalger.Select(k => new Kandidat()
                {
                    id = k.ID,
                    fornavn = k.Fornavn,
                    etternavn = k.Etternavn,
                    lagret = k.Lagret,
                    valgtypeid = k.ValgtypeID
                }).ToList();
                int teller = listeAvKandidater.Count();

                for (int i = 0; i < teller; i++)
                {
                    var itemToRemove = listeAvKandidater.FirstOrDefault(r => r.valgtypeid != ID);
                    if (itemToRemove != null)
                    {
                        listeAvKandidater.Remove(itemToRemove);
                    }
                }

                return listeAvKandidater;
            }
        }
        public List<PersonvalgResultat> hentPersonvalgResultat()
        {
            using (var db = new BrukerContext())
            {
                
                var listeAvPersonvalg = db.PersonvalgResultat.Select(k => new PersonvalgResultat()
                {
                    id = k.ID,
                    valgtypeid = k.ValgtypeID,
                    stemmer = k.Stemmer,
                    vinner = k.Vinner,
                    totalantallStemmer = k.TotalantallStemmer,
                    antallkandidater = k.AntallKandidater
                }).ToList();
                // Antall Personvalg
                int teller = listeAvPersonvalg.Count();


                for (int i = 0; i < teller; i++)
                {
                    // Et enkelt personvalg
                    var personvalg = listeAvPersonvalg.FirstOrDefault();

                    var listeAvStemmesedler = db.PersonvalgResultatStemmer.Select(k => new PersonvalgStemmer()
                    {
                        id = k.ID,
                        valgtypeid = k.ValgtypeID,
                        fornavn = k.Fornavn,
                        etternavn = k.Etternavn

                    }).ToList();

                    // TotalantallStemmer
                    var totalAntallStemmer = listeAvStemmesedler.Count(k => k.valgtypeid == personvalg.valgtypeid);


                    var listeAvKandidater = db.Personvalger.Select(k => new Personvalg()
                    {
                        id = k.ID,
                        fornavn = k.Fornavn,
                        etternavn = k.Etternavn,
                        valgtypeid = k.ValgtypeID,
                        lagret = k.Lagret,
                        
                    }).ToList();

                    // AntallKandidater
                    var antallkandidater = listeAvKandidater.Count(k => k.valgtypeid == personvalg.valgtypeid);


                    if (totalAntallStemmer > 0)
                    {
                        // Finn vinner 
                        int[] stemmer = new int[antallkandidater];
                        string[] kandidater = new string[antallkandidater];

                        for (int j = 0; j < antallkandidater; j++)
                        {
                            var kandidat = listeAvKandidater.FirstOrDefault(k => k.valgtypeid == personvalg.valgtypeid);
                            int antallstemmerkandidat = listeAvStemmesedler.Count(k => k.valgtypeid == personvalg.valgtypeid && k.fornavn == kandidat.fornavn && k.etternavn == kandidat.etternavn);
                            string kandidatnavn = kandidat.fornavn + " " + kandidat.etternavn;
                            kandidater[j] = kandidatnavn;
                            stemmer[j] = antallstemmerkandidat;
                            listeAvKandidater.Remove(kandidat);
                        }
                        string vinnerKandidat = "";
                        int vinnerstemmer = stemmer.Max();
                        

                        for (int j = 0; j < stemmer.Length; j++)
                        {
                            if (stemmer[j].Equals(vinnerstemmer))
                            {
                                vinnerKandidat = kandidater[j];
                            }

                        }
                        int x = 0;
                        // Metode for for sjekke om det er uavgjort
                        for(int k = 0; k < stemmer.Length; k++)
                        {
                            if(vinnerstemmer == stemmer[k])
                            {
                                x++;
                            }
                        }
                        if(x >= 2)
                        {
                            vinnerKandidat = "";
                            vinnerKandidat = "Uavgjort";
                            vinnerstemmer = 0;

                        }
                        // Legg Vinner, Stemmer, Totalantallstemmer, AntallKandidater  i PersonvalgResultat_db


                        try
                        {
                            PersonvalgResultat_db personvalgresultat = db.PersonvalgResultat.FirstOrDefault(k => k.ValgtypeID == personvalg.valgtypeid);
                            personvalgresultat.Vinner = vinnerKandidat;
                            personvalgresultat.Stemmer = vinnerstemmer;
                            personvalgresultat.TotalantallStemmer = totalAntallStemmer;
                            personvalgresultat.AntallKandidater = antallkandidater;
                            db.SaveChanges();
                            listeAvPersonvalg.Remove(personvalg);

                        }
                        catch (Exception feil)
                        {

                        }
                    }
                    else
                    {
                        try
                        {
                            PersonvalgResultat_db personvalgresultat = db.PersonvalgResultat.FirstOrDefault(k => k.ValgtypeID == personvalg.valgtypeid);
                            personvalgresultat.Vinner = "Ingen forløpig";
                            personvalgresultat.Stemmer = 0;
                            personvalgresultat.TotalantallStemmer = totalAntallStemmer;
                            personvalgresultat.AntallKandidater = antallkandidater;
                            db.SaveChanges();
                            listeAvPersonvalg.Remove(personvalg);

                        }
                        catch (Exception feil)
                        {

                        }
                    }
                }
                // Henter ut liste
                var listeAvPersonvalg2 = db.PersonvalgResultat.Select(k => new PersonvalgResultat()
                {
                    id = k.ID,
                    valgtypeid = k.ValgtypeID,
                    stemmer = k.Stemmer,
                    vinner = k.Vinner,
                    totalantallStemmer = k.TotalantallStemmer,
                    antallkandidater = k.AntallKandidater
                }).ToList();
                return listeAvPersonvalg2;

                }
            
        }
        public bool LagrePersonvalgResultat()
        {

            var db = new BrukerContext();
            // if antall stemmer == 0
            // legg verdiene i tabellene


            // PersonvalgID == ValgtypeID

            var ValgtypeIDliste = db.Valgtyper.Select(k => new Valgtyper()
            {
                valgtypeid = k.ValgtypeID,
                valgtype = k.Valgtype,
                start = k.Start
            }).ToList();

            var listedescendingOrder = ValgtypeIDliste.OrderByDescending(i => i.valgtypeid);

            var valgtypeID = listedescendingOrder.FirstOrDefault(i => i.valgtype == "Personvalg" && i.start == false);

            // Antall avlagte stemmer

            var liste = db.PersonvalgResultatStemmer.Select(k => new PersonvalgStemmer()
            {
                id = k.ID,
                valgtypeid = k.ValgtypeID,
                fornavn = k.Fornavn,
                etternavn = k.Etternavn
            }).ToList();


            int totallstemmer = liste.Count(k => k.valgtypeid == valgtypeID.valgtypeid);

            // Ingen resultat
            if (totallstemmer == 0)
            {
                try
                {
                    var liste2 = db.Personvalger.Select(k => new Personvalg()
                    {
                        id = k.ID,
                        fornavn = k.Fornavn,
                        etternavn = k.Etternavn,
                        valgtypeid = k.ValgtypeID,
                        lagret = k.Lagret
                    }).ToList();

                    var antallkandidater = liste2.Count(k => k.valgtypeid == valgtypeID.valgtypeid);

                    var nyResultat= new PersonvalgResultat_db()
                    {
                        ValgtypeID = valgtypeID.valgtypeid,
                        Vinner = "Ingen forløpig",
                        TotalantallStemmer = 0,
                        AntallKandidater = antallkandidater,
                    };
                    db.PersonvalgResultat.Add(nyResultat);
                    db.SaveChanges();
                    return true;

                }
                catch (Exception feil)
                {
                    return false;
                }
            }

            // Finnes Resultat
            else
            {
                // Antall kandidater

                var liste2 = db.Personvalger.Select(k => new Personvalg()
                {
                    id = k.ID,
                    fornavn = k.Fornavn,
                    etternavn = k.Etternavn,
                    valgtypeid = k.ValgtypeID,
                    lagret = k.Lagret
                }).ToList();

                int antallkandidater = liste2.Count(k => k.valgtypeid == valgtypeID.valgtypeid);

                // Finn vinner

                int[] stemmer = new int[antallkandidater];
                string[] kandidater = new string[antallkandidater];

                for (int i = 0; i < antallkandidater; i++)
                {
                    var kandidat = liste2.FirstOrDefault(k => k.valgtypeid == valgtypeID.valgtypeid);
                    int antallstemmerkandidat = liste.Count(k => k.valgtypeid == valgtypeID.valgtypeid && k.fornavn == kandidat.fornavn && k.etternavn == kandidat.etternavn);
                    liste2.Remove(kandidat);
                    string kandidatnavn = kandidat.fornavn + " " + kandidat.etternavn;
                    kandidater[antallkandidater] = kandidatnavn;
                    stemmer[antallkandidater] = antallstemmerkandidat;

                }
                string vinnerKandidat = "";
                int vinnerstemmer = stemmer.Max();
                for (int i = 0; i < stemmer.Length; i++)
                {
                    if (stemmer[i].Equals(vinnerstemmer))
                    {
                        vinnerKandidat = kandidater[i];
                    }
                }

                // Legg Vinner, Stemmer, Totalantallstemmer, AntallKandidater  i PersonvalgResultat_db


                try
                {
                    PersonvalgResultat_db personvalgresultat = db.PersonvalgResultat.FirstOrDefault(k => k.ValgtypeID == valgtypeID.valgtypeid);
                    personvalgresultat.Vinner = vinnerKandidat;
                    personvalgresultat.Stemmer = vinnerstemmer;
                    personvalgresultat.TotalantallStemmer = totallstemmer;
                    personvalgresultat.AntallKandidater = antallkandidater;

                    db.SaveChanges();
                    return true;
                }
                catch (Exception feil)
                {
                    return false;
                }


            }
            
        }
        public bool LagrePersonvalg()
        {
            var db2 = new BrukerContext();
            var start = new Valgtyper_db()
            {
                Valgtype = "Personvalg",
                Start = false
            };
            db2.Valgtyper.Add(start);
            db2.SaveChanges();

            var db = new BrukerContext();
            

                var liste = db.Valgtyper.Select(k => new Valgtyper()
                {
                    valgtypeid = k.ValgtypeID,
                    valgtype = k.Valgtype,
                    start = k.Start
                }).ToList();

                var listedescendingOrder = liste.OrderByDescending(i => i.valgtypeid);
                 
                var PersonvalgID = listedescendingOrder.FirstOrDefault(i => i.valgtype == "Personvalg" && i.start == false);
                
                var listeAvKandidater = db.Personvalger.Select(k => new Kandidat()
                {
                    id = k.ID,
                    fornavn = k.Fornavn,
                    etternavn = k.Etternavn,
                    lagret = k.Lagret,
                    valgtypeid = k.ValgtypeID
                }).ToList();

                int teller = listeAvKandidater.Count();
                for(int i = 0; i < teller; i++)
                {
                    var listeAvKandidater2 = db.Personvalger.Select(k => new Kandidat()
                     {
                    id = k.ID,
                    fornavn = k.Fornavn,
                    etternavn = k.Etternavn,
                    lagret = k.Lagret,
                    valgtypeid = k.ValgtypeID
                     }).ToList();
                     var itemToChange = listeAvKandidater2.FirstOrDefault(r => r.lagret == false);
                    if(itemToChange != null)
                {
                    if (itemToChange.lagret == false)
                    {
                        try
                        {
                            int id = itemToChange.id;
                            int valgID = PersonvalgID.valgtypeid;

                            Personvalg_db endre = db.Personvalger.FirstOrDefault(r => r.ID == id);
                            endre.ValgtypeID = valgID;
                            endre.Lagret = (bool)true;
                            db.SaveChanges();
                        }
                        catch (Exception feil)
                        {
                            return false;
                        }
                    }

                    


                }
           }
            return true;
        
            

        }
        public bool slettKandidat(int id)
        {
            var db = new BrukerContext();
            try
            {
                Personvalg_db personvalger = db.Personvalger.FirstOrDefault(b => b.ID == id);
                // slett også 
                db.Personvalger.Remove(personvalger);
                db.SaveChanges();
                return true;
            }
            catch (Exception feil)
            {
                return false;
            }
        }
        public List<Kandidat> hentKandidater()
        {
            using (var db = new BrukerContext())
            {
                var listeAvKandidater = db.Personvalger.Select(k => new Kandidat()
                {
                    id = k.ID,
                    fornavn = k.Fornavn,
                    etternavn = k.Etternavn,
                    lagret = k.Lagret
                }).ToList();
                int teller = listeAvKandidater.Count();
                
                for (int i = 0; i < teller; i++)
                {
                    var itemToRemove = listeAvKandidater.FirstOrDefault(r => r.lagret == (bool)true);
                    if (itemToRemove != null)
                    {
                        listeAvKandidater.Remove(itemToRemove);
                    }
                }
               
                return listeAvKandidater;
            }
        }
        public bool leggtilPersonvalgKandidat(Kandidat innKandidat)
        {
            var db = new BrukerContext();

            var nyKandidat = new Personvalg_db()
            {
                Fornavn = innKandidat.fornavn,
                Etternavn = innKandidat.etternavn,
                Lagret = (bool)false,
               
                ValgtypeID = 0
                };
                db.Personvalger.Add(nyKandidat);
                db.SaveChanges();
                return true;
   

  

        }
        // Personvalg
        public Valgtyper hentValg(int id)
        {
            var db = new BrukerContext();

            var enDbValg = db.Valgtyper.FirstOrDefault(b => b.ValgtypeID == id);

            if (enDbValg == null)
            {
                return null;
            }
            else
            {
                var utBruker = new Valgtyper()
                {
                    valgtypeid = enDbValg.ValgtypeID
                };
                return utBruker;
            }
        }
        public bool finnesdetvalg_ID(int id)
        {
            var db = new BrukerContext();
            var antall = db.Valgtyper.FirstOrDefault(b => b.ValgtypeID == id);
            if (antall == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool finnesdetvalg()
        {
            var db = new BrukerContext();
            int antall = db.Valgtyper.Count();
            if (antall <= 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public int antallstemteVotering(int id)
        {
            var db = new BrukerContext();
            int antall = db.BrukereStemt.Count(b => b.ValgtypeID == id && b.Valgtype == "Votering");
            return antall;
        }
        public int antallFor(int id)
        {
            var db = new BrukerContext();
            int antall = db.Voteringer.Count(b => b.ValgtypeID == id && b.svarFor == "for");
            return antall;
        }
        public int antallMot(int id)
        {
            var db = new BrukerContext();
            int antall = db.Voteringer.Count(b => b.ValgtypeID == id && b.svarMot == "mot");
            return antall;
        }
        public int antallBlank(int id)
        {
            var db = new BrukerContext();
            int antall = db.Voteringer.Count(b => b.ValgtypeID == id && b.svarBlank == "blank");
            return antall;
        }
        public int hensisteVotering()
        {
            var db = new BrukerContext();
            var sistevotering = db.Valgtyper.Max(b => b.ValgtypeID);
            if (sistevotering > 0)
            {
                return sistevotering;
            }
            else
            {
                return 0;
            }
        }
        public int hentAntallStemte_int()
        {
            var db = new BrukerContext();
            var valg = VoteringPågår();
            if (valg != null)
            {
                String valgtype = valg.valgtype;
                int valgID = valg.valgtypeid;
                System.Int32 antall = db.BrukereStemt.Count(b => b.Valgtype == valgtype && b.ValgtypeID == valgID);
                return antall;
            }
            else return 0;
        }
        public int hentAntallBrukere_int()
        {
            var db = new BrukerContext();
            System.Int32 antall = db.Brukere.Count();
            return antall;
        }
        public bool GenererBrukere(int antallBrukere)
        {
            var db = new BrukerContext();

            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [Bruker_db]");
            db.SaveChanges();

            PasswordGenerator randPassord = new PasswordGenerator(8).IncludeLowercase().IncludeUppercase();
            String brukernavn = "SP";
            int teller = 1;
            String passord = "";
            

            for (int i = 0; i < antallBrukere; i++)
            {
                passord = randPassord.Next();

                 var nyBruker = new Bruker_db()
                 {
                     Brukernavn = brukernavn + "" + teller,
                     Passord = passord
                 };

                teller++;
                db.Brukere.Add(nyBruker);
                db.SaveChanges();
                
            }

            return true;
        }
        public List<Bruker> hentData()
        {

            using (var db = new BrukerContext())
            {
                var listeAvBrukere = db.Brukere.Select(k => new Bruker()
                {
                    brukernavn = k.Brukernavn,
                    passord = k.Passord,
                    innlogget = k.Innlogget 
                }).ToList();
                return listeAvBrukere;
            } 
        }
        public Bruker lastNedListe()
        {
            return null;
        }
        public Bruker hentEnBruker(string id)
        {
            var db = new BrukerContext();

            var enDbBruker = db.Brukere.FirstOrDefault(b => b.Brukernavn == id);

            if (enDbBruker == null)
            {
                return null;
            }
            else
            {
                var utBruker = new Bruker()
                {
                    brukernavn = enDbBruker.Brukernavn,
                    passord = enDbBruker.Passord
                };
                return utBruker;
            }
        }
        public Admin hentEnAdmin(string id)
        {
            var db = new BrukerContext();

            var enDbAdmin = db.AdminBrukere.FirstOrDefault(b => b.Brukernavn == id);

            if (enDbAdmin == null)
            {
                return null;
            }
            else
            {
                var utBruker = new Admin()
                {
                    brukernavn = enDbAdmin.Brukernavn,
                    passwordhash = enDbAdmin.Passord
                };
                return utBruker;
            }
        }
        public Admin hentRolleAdmin(string rolle)
        {
            var db = new BrukerContext();
            var enDbAdmin = db.AdminBrukere.FirstOrDefault(b=> b.Brukernavn == rolle);
           // var enDbAdmin = db.AdminBrukere.Find(rolle);
            if (enDbAdmin == null)
            {
                return null;
            }
            else
            {
                var utRolle = new Admin()
                {
                    administrator = enDbAdmin.Administrator
                };


            return utRolle;
               
            }
            
       }
        public Bruker hentRolleBruker(string rolle)
        {
            var db = new BrukerContext();
            var enDbAdmin = db.Brukere.FirstOrDefault(b => b.Brukernavn == rolle);
            //  var enDbAdmin = db.Brukere.Find(rolle);
            if (enDbAdmin == null)
            {
                return null;
            }
            else
            {
                var utRolle = new Bruker()
                {
                    administrator = enDbAdmin.Administrator
                };


                return utRolle;

            }

        }
        public bool slettBruker(String id)
        {
            var db = new BrukerContext();
            try
            {
                Bruker_db slettBruker = db.Brukere.FirstOrDefault(b => b.Brukernavn == id);
                db.Brukere.Remove(slettBruker);
                db.SaveChanges();
                return true;
            }
            catch (Exception feil)
            {
                return false;
            }
        }
        public bool loggUtBruker(String id)
        {
            var db = new BrukerContext();
            try
            {
                Bruker_db bruker = db.Brukere.FirstOrDefault(b => b.Brukernavn == id);
                bruker.Innlogget = (bool)false;
                db.SaveChanges();
                return true;
            }
            catch (Exception feil)
            {
                return false;
            }
        }
        private static byte[] lagHash(string passord)
        {
            byte[] inndata, utdata;

            var algoritme = System.Security.Cryptography.SHA256.Create();

            inndata = System.Text.Encoding.ASCII.GetBytes(passord);
            utdata = algoritme.ComputeHash(inndata);
            return utdata;
        }
        public bool bruker_i_db(Bruker innPerson)
        {
            using (var db = new BrukerContext())
            {

                
                Bruker_db funnetBruker = db.Brukere.FirstOrDefault(b => b.Passord == innPerson.passord && b.Brukernavn == innPerson.brukernavn);

                if (funnetBruker == null)
                {
                    return false; // return bruker er ikke i systemet, kontakt systemansvarlig
                }
                else if (funnetBruker.Innlogget == (bool)true) // sjekker om en bruker en innlogget
                {
                    return false; 
                }
                else if (funnetBruker.Administrator == (bool)false) // sjekker om bruker er administrator
                {
                    funnetBruker.Innlogget = (bool)true; // flagger at bruker er innlogget
                    db.SaveChanges();
                    return true;
                }                            
                else
                {
                   return false; // ikke en vanlig bruker
                }
            }

        }
        public bool admin_i_db(Bruker innAdmin)
        {
            using (var db = new BrukerContext())
            {

                byte[] passwordhash = lagHash(innAdmin.passord);
                Admin_db funnetBruker = db.AdminBrukere.FirstOrDefault(b => b.Passord == passwordhash && b.Brukernavn == innAdmin.brukernavn);
                if(funnetBruker == null) // Sjekker om bruker finnes i systemet og sjekker om bruker allerede er innlogget
                {
                   return false;
                }
                else if(funnetBruker.Innlogget == (bool)true)
                {
                    return false;
                    
                }
                               
                else if (funnetBruker.Administrator == (bool)true)
                {
                    funnetBruker.Innlogget = (bool)true;// flagger at bruker er innlogget
                    db.SaveChanges();
                    return true;
                }
                else
                {
                    return false; // ikke en administrator bruker 
                }

            }
        }
        public bool logg_ut_bruker(String id)
        {
            using (var db = new BrukerContext())
            {
                Bruker_db funnetBruker = db.Brukere.FirstOrDefault(b => b.Brukernavn == id);

                if (funnetBruker != null)
                {
                    funnetBruker.Innlogget = (bool)false;
                    db.SaveChanges();
                    return true;
                }
                else if (funnetBruker == null)
                {
                    Admin_db funnetAdmin = db.AdminBrukere.Find(id);
                    if (funnetAdmin != null)
                    {
                        funnetAdmin.Innlogget = (bool)false;
                        db.SaveChanges();
                        return true;
                    }
                    return false;
                }
                return false;
            }
        }
        public bool startVotering()
        {
            var db = new BrukerContext();
            var start = new Valgtyper_db()
            {
                Valgtype = "Votering",
                Start = true
            };
            db.Valgtyper.Add(start);
            db.SaveChanges();
            return true;

        }
        public bool stopVotering()
        {
            var db = new BrukerContext();
            int antall = db.Valgtyper.Where(b => b.Start == true && b.Valgtype == "Votering").Count();
            for (int i = 0; i < antall; i++)
            {
                Valgtyper_db stop = db.Valgtyper.FirstOrDefault(b => b.Start == true && b.Valgtype == "Votering");
                if (stop != null)
                {
                    stop.Start = false;
                    db.SaveChanges();
                }
            }
            return true;

        }
        public bool voteringsvar(String svar)
        {
            var db = new BrukerContext();
            var valgType = VoteringPågår();
           
            if (svar == "for")
            {
                var svarvalg = new Votering_db()
                {
                    svarFor = svar,
                    ValgtypeID = valgType.valgtypeid
                };
                db.Voteringer.Add(svarvalg);
                db.SaveChanges();
                return true;
            }
            else if (svar == "mot")
            {
                var svarvalg = new Votering_db()
                {
                    svarMot = svar,
                    ValgtypeID = valgType.valgtypeid
                };
                db.Voteringer.Add(svarvalg);
                db.SaveChanges();
                return true;
            }
            else if (svar == "blank")
            {
                var svarvalg = new Votering_db()
                {
                    svarBlank = svar,
                    ValgtypeID = valgType.valgtypeid
                };
                db.Voteringer.Add(svarvalg);
                db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
            
        }
        public string voteringBrukerStart()
        {
            var db = new BrukerContext();
            string melding = "";

            var valg = db.Valgtyper.FirstOrDefault(v => v.Valgtype == "Votering" && v.Start == true);

            if(valg != null)
            {
                melding = "Votering";
            }
            else
            {
                melding = "";
            }
            valg = null;
            valg = db.Valgtyper.FirstOrDefault(v => v.Valgtype == "Personvalg" && v.Start == true);

            if (valg != null)
            {
                melding = "Personvalg";
            }
            else
            {
                melding = "";
            }

            return melding;
        }
        public List<Valgtyper> hentValgTyper()
        {
            using (var db = new BrukerContext())
            {
                var listeAvValgTyper = db.Valgtyper.Select(k => new Valgtyper()
                {
                    valgtypeid = k.ValgtypeID,
                    valgtype = k.Valgtype,
                    start = k.Start,

                }).ToList();

               
                return listeAvValgTyper;
            }
        }
        public bool slettValg(int id)
        {
            var db = new BrukerContext();
            try
            {
                Valgtyper_db valgtyper = db.Valgtyper.FirstOrDefault(b => b.ValgtypeID == id);
                // slett også 
                db.Valgtyper.Remove(valgtyper);
                db.SaveChanges();
                return true;
            }
            catch (Exception feil)
            {
                return false;
            }
        }
        public bool admin_i_db_innlogget(Bruker innAdmin, string brukernavn)
        {
            using (var db = new BrukerContext())
            {
                
                byte[] passwordhash = lagHash(innAdmin.passord);
                
                Admin_db funnetBruker = db.AdminBrukere.FirstOrDefault(b => b.Passord == passwordhash && b.Brukernavn == brukernavn);
                if (funnetBruker != null) // Sjekker om bruker finnes i systemet
                {
                    //slettValg(valgnr);
                    return true;
                }
                else
                {
                    return false; // ikke en administrator bruker 
                }

            }
        }
        public Valgtyper VoteringPågår()
        {
            var db = new BrukerContext();

            Valgtyper_db valgtype_db = db.Valgtyper.FirstOrDefault(b => b.Start == true && b.Valgtype == "Votering");

            if (valgtype_db == null)
            {
                return null;
            }
            else
            {
                var valgtype = new Valgtyper()
                {
                    valgtypeid = valgtype_db.ValgtypeID,
                    valgtype = valgtype_db.Valgtype,
                    start = valgtype_db.Start
                };

                return valgtype;
            }
        }
        public string HarBrukerStemt(string brukernavn)
        {
            var db = new BrukerContext();
            Valgtyper valg = VoteringPågår();
            if (valg != null)
            {
                BrukereStemt_db brukerStemt = db.BrukereStemt.FirstOrDefault(b => b.Brukernavn == brukernavn && b.ValgtypeID == valg.valgtypeid && b.Valgtype == valg.valgtype);

                if (brukerStemt != null)
                {
                    return "JA";
                }
                else
                {
                    return "NEI";
                }
            }
            else if(valg == null)
            {
                int valgtypeID = PersonvalgKjørerID();
                if(valgtypeID > 0)
                {
                    BrukereStemt_db brukerStemt = db.BrukereStemt.FirstOrDefault(b => b.Brukernavn == brukernavn && b.ValgtypeID == valgtypeID && b.Valgtype == "Personvalg");
                    if (brukerStemt != null)
                    {
                        return "JA";
                    }
                    else
                    {
                        return "NEI";
                    }
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
            
        }
        public void LagreBrukerStemt(Valgtyper valg, string brukernavn)
        {
            var db = new BrukerContext();

            if (valg == null || brukernavn == null)
            {
                return;
            }
            else
            {
                var brukerStemt = new BrukereStemt_db()
                {
                    Brukernavn = brukernavn,
                    ValgtypeID = valg.valgtypeid,
                    Valgtype = valg.valgtype
                };

                db.BrukereStemt.Add(brukerStemt);
                db.SaveChanges();
            }
        }
        public bool VoteringKjorer()
        {
            using (var db = new BrukerContext())
            {
                Valgtyper_db voteringkjorer = db.Valgtyper.FirstOrDefault(b => b.Valgtype == "Votering" && b.Start == true);
                if (voteringkjorer != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public string hentAntallStemt(int id)
        {
            var db = new BrukerContext();
            int antallStemt = db.BrukereStemt.Where(b => b.ValgtypeID == id).Count();
            int antallbrukere = hentData().Count();

            string utskrift = antallStemt + "/" + antallbrukere;
            return utskrift;
        }
        public int antallValgFullført()
        {
            var db = new BrukerContext();
            int antall = hentValgTyper().Count();

            return antall;
        }

        
    }
}