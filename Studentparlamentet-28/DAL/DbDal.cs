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
        public List<Valgtyper> hentValgTyperVotering()
        {
            using (var db = new BrukerContext())
            {
                var listeAvValgTyper = db.Valgtyper.Select(k => new Valgtyper()
                {
                    valgtypeid = k.ValgtypeID,
                    valgtype = k.Valgtype,
                    start = k.Start,

                }).Where(k => k.valgtype == "Votering").ToList();



                return listeAvValgTyper;
            }
        }
        public void LeggTilMeldingVotering(string id)
        {
            var db = new BrukerContext();
            Valgtyper_db votering = db.Valgtyper.FirstOrDefault(b => b.Start == true);
            votering.Melding = id;
            db.SaveChanges();
        }
        public bool sjekkValgStemme(int ID)
        {
            var db = new BrukerContext();
            PersonvalgResultatStemmer_db avlagtstemme = db.PersonvalgResultatStemmer.FirstOrDefault(b => b.ValgtypeID == ID);
            if(avlagtstemme != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public string kjørerValg()
        {
            var db = new BrukerContext();
            Valgtyper valg = valgPågår();
            if (valg != null)
            {
                return "Ja";
            }
            else
            {
                return "Nei";
            }
            
        }
        //Preferansevalg
        public List<PreferansevalgValgte> hentValgteKandidater(int valgtypeid)
        {
            var db = new BrukerContext();

            var valgteKandidater = db.PreferansevalgValgt.Where(v => v.ValgtypeID == valgtypeid).ToList();
            List<PreferansevalgValgte> returliste = new List<PreferansevalgValgte>();
            for (int i = 0; i < valgteKandidater.Count(); i++)
            {
                var kandidat = new PreferansevalgValgte()
                {
                    valgtID = valgteKandidater[i].ValgtID,
                    navn = valgteKandidater[i].Navn,
                    valgtypeid = valgteKandidater[i].ValgtypeID
                };
                returliste.Add(kandidat);
            }
            return returliste;
        }

        public List<VaraSTV> hentValgteVaraer(int valgtypeid)
        {
            var db = new BrukerContext();

            var alleredeValgt = hentValgteKandidater(valgtypeid);
            List<VaraSTV> AlleIVaraListe = hentVaralisteMedID(valgtypeid);
            List<VaraSTV> tilgjengeligeKandidater = hentVaralisteMedID(valgtypeid);
            List<VaraSTV> valgteVaraer = new List<VaraSTV>();
            List<VaraSTV> ekskluderteKandidater = new List<VaraSTV>();
            List<KandidatSTV> hjelpGjenståendekandidater = new List<KandidatSTV>();
            List<VaraSTV> kandidaterEkskludert2 = new List<VaraSTV>();

            var preferansevalg = db.PreferanseValg.FirstOrDefault(p => p.ValgtypeID == valgtypeid);
            //int antallVaraRepresentanter = preferansevalg.AntallVaraRepresentanter;
            int antallVaraRepresentanter = preferansevalg.AntallVaraRepresentanter;
            int antallRep = preferansevalg.AntallRepresentanter + 1;
            int antallLedigeplasser = antallVaraRepresentanter;
            int valgtTeller = 0;
            int stemmetallEkskludert = 0;
            double valgtall = BeregnNyttValgTall(antallRep, valgtypeid); //Må endre denne om til sånn at valgtallet beregnes riktig
            //int stemmetallEkskludert = 0;
            bool valgAvsluttes = false;
            bool fortsettRunde = true;

            VaraSTV vara = new VaraSTV();

            //Fjerner de som allerede er valgt
            for (int i = 0; i < tilgjengeligeKandidater.Count(); i++)
            {
                for (int k = 0; k < alleredeValgt.Count(); k++)
                {
                    if (tilgjengeligeKandidater[i].navn == alleredeValgt[k].navn)
                    {
                        tilgjengeligeKandidater.Remove(tilgjengeligeKandidater[i]);
                    }
                }
            }
            //Gå igjennom tilgjengelige og sjekke om noen av disse har større stemmetall enn valgtallet
            bool hjelpBoolNoenValgt = false;
            for (int i = 0; i < tilgjengeligeKandidater.Count(); i++)
            {
                VaraSTV sjekkKandidat = tilgjengeligeKandidater[i];
                if (sjekkKandidat.stemmetall >= valgtall)
                {
                    if (hjelpBoolNoenValgt == false)
                    {
                        valgteVaraer.Add(sjekkKandidat);
                        tilgjengeligeKandidater.Remove(sjekkKandidat);
                        if (valgteVaraer.Count() == antallVaraRepresentanter)
                        {
                            return valgteVaraer;
                        }
                        valgtTeller++;
                        antallLedigeplasser--;
                        antallRep++;
                        valgtall = BeregnNyttValgTall(antallRep, valgtypeid);
                        hjelpBoolNoenValgt = true;
                    }
                }
            }

            bool overførtOverskudd = false;
            while (valgteVaraer.Count() < antallVaraRepresentanter && valgAvsluttes == false)
            {

                if (fortsettRunde == false && vara != null)
                {
                    valgteVaraer.Add(vara);
                    tilgjengeligeKandidater.Remove(vara);
                    if (valgteVaraer.Count() == antallVaraRepresentanter) //Foreløpig test bare
                    {
                        return valgteVaraer;
                    }
                    valgtTeller++;
                    antallLedigeplasser--;
                    antallRep++;
                    overførtOverskudd = false;
                    fortsettRunde = true;
                    valgtall = BeregnNyttValgTall(antallRep, valgtypeid);

                    tilgjengeligeKandidater.Clear();
                    tilgjengeligeKandidater = hentVaralisteMedID(valgtypeid);
                    for (int i = 0; i < tilgjengeligeKandidater.Count(); i++)
                    {
                        for (int k = 0; k < alleredeValgt.Count(); k++)
                        {
                            if (tilgjengeligeKandidater[i].navn == alleredeValgt[k].navn)
                            {
                                tilgjengeligeKandidater.Remove(tilgjengeligeKandidater[i]);
                            }
                        }
                    }

                    for (int i = 0; i < valgteVaraer.Count(); i++)
                    {
                        for (int k = 0; k < tilgjengeligeKandidater.Count(); k++)
                        {
                            if (valgteVaraer[i].navn == tilgjengeligeKandidater[k].navn)
                            {
                                tilgjengeligeKandidater.Remove(tilgjengeligeKandidater[k]);
                            }
                        }
                    }
                    vara = null;
                }

                //Finner laveste stemmetallet i gj.kandidater etter overført etter første runde
                stemmetallEkskludert = tilgjengeligeKandidater[0].stemmetall;
                for (int i = 0; i < tilgjengeligeKandidater.Count(); i++)
                {
                    if (stemmetallEkskludert == 0)
                    {
                        stemmetallEkskludert = tilgjengeligeKandidater[i].stemmetall;
                    }
                }
                for (int i = 0; i < tilgjengeligeKandidater.Count(); i++)
                {
                    int tempStemmetall = tilgjengeligeKandidater[i].stemmetall;
                    if (tempStemmetall < stemmetallEkskludert)
                    {
                        if (tempStemmetall != 0)
                        {
                            stemmetallEkskludert = tempStemmetall;
                        }
                    }
                }
                //Finner høyest stemmetallet
                int størstStemmetall = 0;
                for (int i = 0; i < tilgjengeligeKandidater.Count(); i++)
                {
                    int tempStørstStemmetall = tilgjengeligeKandidater[i].stemmetall;

                    if (størstStemmetall < tempStørstStemmetall)
                    {
                        størstStemmetall = tempStørstStemmetall;
                    }
                }

                //Sjekker om det er mer enn en kandidat igjen som har større enn 0 i stemmer - etterpå
                int tellerForTilgjengelige = 0;
                int indexEnesteTilgjengelig = 0;
                for (int i = 0; i < tilgjengeligeKandidater.Count(); i++)
                {
                    if (tilgjengeligeKandidater[i].stemmetall > 0)
                    {
                        tellerForTilgjengelige++;
                        indexEnesteTilgjengelig = i;
                    }
                }
                if (tellerForTilgjengelige == 1)
                {
                    vara = tilgjengeligeKandidater[indexEnesteTilgjengelig];
                    fortsettRunde = false;
                }

                if (fortsettRunde == true)
                {
                    //Eksludere de med laveste stemmetallet -- Funker -- Må kanskje lage den summe opp og plusse greia
                    List<VaraSTV> alleEksluderte = tilgjengeligeKandidater.FindAll(l => l.stemmetall == stemmetallEkskludert);

                    int hjelpTeller = tilgjengeligeKandidater.Count() - 1;
                    int sumEkskluderteStemmetall = 0;
                    //bool hjelpBool = false;
                    for (int i = 0; i < alleEksluderte.Count(); i++)
                    {
                        VaraSTV kandidatEkskluderes = alleEksluderte[i];
                        sumEkskluderteStemmetall += kandidatEkskluderes.stemmetall;
                        for (int k = 0; k < tilgjengeligeKandidater.Count(); k++)
                        {

                            if (sumEkskluderteStemmetall <= størstStemmetall && kandidatEkskluderes.navn == tilgjengeligeKandidater[k].navn)
                            {
                                ekskluderteKandidater.Add(kandidatEkskluderes);
                            }
                        }
                    }
                }

                //Overføre overskudd
                if (overførtOverskudd == false)
                {
                    for (int i = 0; i < AlleIVaraListe.Count(); i++)
                    {
                        int overskudd = 0;
                        VaraSTV valgtKandidat = AlleIVaraListe[i];
                        int tempValgtall = (int)Math.Ceiling(valgtall);
                        overskudd = valgtKandidat.stemmetall - tempValgtall;

                        if (overskudd > 0)
                        {
                            List<Stemmeseddel_db> overførStemmer = db.Stemmesedler.Where(s => s.KandidatnrEn == valgtKandidat.navn && s.ValgtypeID == valgtypeid).ToList();
                            for (int k = 0; k < overskudd; k++)
                            {
                                Stemmeseddel_db stemme = overførStemmer[k];
                                string seddelNavnet = "";
                                string navnToPåSeddel = stemme.KandidatnrTo;
                                string navnTrePåSeddel = stemme.KandidatnrTre;
                                string navnFirePåSeddel = stemme.KandidatnrFire;
                                string navnFemPåSeddel = stemme.KandidatnrFem;
                                bool funnet = false;

                                if (tilgjengeligeKandidater.FirstOrDefault(b => b.navn == navnToPåSeddel) != null && !funnet)
                                {
                                    seddelNavnet = navnToPåSeddel;
                                    funnet = true;
                                }
                                else if (tilgjengeligeKandidater.FirstOrDefault(b => b.navn == navnTrePåSeddel) != null && !funnet)
                                {
                                    seddelNavnet = navnTrePåSeddel;
                                    funnet = true;
                                }
                                else if (tilgjengeligeKandidater.FirstOrDefault(b => b.navn == navnFirePåSeddel) != null && !funnet)
                                {
                                    seddelNavnet = navnFirePåSeddel;
                                    funnet = true;
                                }
                                else
                                {
                                    overskudd++;
                                }
                                if (seddelNavnet != "")
                                {
                                    int tempStemmetallValgtKandidat = valgtKandidat.stemmetall - 1;

                                    VaraSTV oppdaterKandidat = tilgjengeligeKandidater.FirstOrDefault(b => b.navn == seddelNavnet);
                                    int tempStemmetallet = oppdaterKandidat.stemmetall + 1;
                                    oppdaterKandidat.stemmetall = tempStemmetallet;


                                    valgtKandidat.stemmetall = tempStemmetallValgtKandidat;


                                    for (int j = 0; j < ekskluderteKandidater.Count(); j++)
                                    {
                                        if (ekskluderteKandidater[j].navn == seddelNavnet)
                                        {
                                            ekskluderteKandidater.Remove(ekskluderteKandidater[j]);
                                        }
                                    }

                                    if (tempStemmetallet >= valgtall)
                                    {
                                        vara = oppdaterKandidat;
                                        fortsettRunde = false;
                                    }
                                }
                            }
                        }
                    }
                    overførtOverskudd = true;
                }


                //Eksludere stemmer
                for (int i = 0; i < ekskluderteKandidater.Count(); i++)
                {
                    VaraSTV ekskludertKandidat = tilgjengeligeKandidater.FirstOrDefault(b => b.navn == ekskluderteKandidater[i].navn);
                    if (ekskludertKandidat != null)
                    {
                        tilgjengeligeKandidater.Remove(ekskludertKandidat); //Fjerner riktig kandidat
                    }
                }

                //Ekskluderte stemmer overføres
                if (fortsettRunde == true)
                {
                    for (int i = 0; i < ekskluderteKandidater.Count(); i++)
                    {
                        string navnEkskludert = ekskluderteKandidater[i].navn;
                        int tempStemmetallEksKandidat = ekskluderteKandidater[i].stemmetall - 1;
                        List<Stemmeseddel_db> listeEkskludert = db.Stemmesedler.Where(b => b.KandidatnrEn == navnEkskludert && b.ValgtypeID == valgtypeid).ToList();

                        for (int j = 0; j < listeEkskludert.Count(); j++)
                        {
                            string seddelNavnet = "";
                            string navnToPåSeddel = listeEkskludert[j].KandidatnrTo;
                            string navnTrePåSeddel = listeEkskludert[j].KandidatnrTre;
                            string navnFirePåSeddel = listeEkskludert[j].KandidatnrFire;
                            string navnFemPåSeddel = listeEkskludert[j].KandidatnrFem;
                            bool funnet = false;

                            if (tilgjengeligeKandidater.FirstOrDefault(b => b.navn == navnToPåSeddel) != null && !funnet)
                            {
                                seddelNavnet = navnToPåSeddel;
                                funnet = true;
                            }
                            else if (tilgjengeligeKandidater.FirstOrDefault(b => b.navn == navnTrePåSeddel) != null && !funnet)
                            {
                                seddelNavnet = navnTrePåSeddel;
                                funnet = true;
                            }
                            else if (tilgjengeligeKandidater.FirstOrDefault(b => b.navn == navnFirePåSeddel) != null && !funnet)
                            {
                                seddelNavnet = navnFirePåSeddel;
                                funnet = true;
                            }
                            else if (tilgjengeligeKandidater.FirstOrDefault(b => b.navn == navnFemPåSeddel) != null && !funnet)
                            {
                                seddelNavnet = navnFemPåSeddel;
                                funnet = true;
                            }

                            if (seddelNavnet == "")
                            {
                                //utskriftTest += "Seddelen var tom as";
                            }
                            else
                            {
                                tempStemmetallEksKandidat = ekskluderteKandidater[i].stemmetall - 1;

                                VaraSTV oppdaterKandidat = tilgjengeligeKandidater.FirstOrDefault(b => b.navn == seddelNavnet);
                                int tempStemmetallet = oppdaterKandidat.stemmetall + 1;
                                oppdaterKandidat.stemmetall = tempStemmetallet;


                                ekskluderteKandidater[i].stemmetall = tempStemmetallEksKandidat;

                                //db.SaveChanges();

                                for (int k = 0; k < ekskluderteKandidater.Count(); k++)
                                {
                                    if (ekskluderteKandidater[k].navn == seddelNavnet)
                                    {
                                        ekskluderteKandidater.Remove(oppdaterKandidat);
                                    }
                                }

                                if (tempStemmetallet >= valgtall)
                                {
                                    vara = oppdaterKandidat;
                                    fortsettRunde = false;
                                }
                            }
                        }
                    }
                }

                for (int i = 0; i < ekskluderteKandidater.Count(); i++)
                {
                    kandidaterEkskludert2.Add(ekskluderteKandidater[i]);
                }

                ekskluderteKandidater.Clear();
            }//While stopper her

            return valgteVaraer; //Foreløpig
        }
        public double BeregnNyttValgTall(int antallRep, int valgtypeid)
        {
            var db = new BrukerContext();
            int antallStemmer = db.Stemmesedler.Where(s => s.ValgtypeID == valgtypeid).Count();
            var preferansevalg = db.PreferanseValg.FirstOrDefault(p => p.ValgtypeID == valgtypeid);
            //int antallRepresentanter = preferansevalg.AntallRepresentanter;
            double divider = (double)antallStemmer / (double)antallRep;
            //double divider = antallStemmer / antallRepresentanter;
            double valgtall = divider + 1 + 0.01;

            return valgtall;
        }

        public string HarBrukerStemtSTV(string brukernavn)
        {
            var db = new BrukerContext();
            Valgtyper valg = valgPågår();
            if (valg != null)
            {
                BrukereStemt_db brukerStemt = db.BrukereStemt.FirstOrDefault(b => b.Brukernavn == brukernavn && b.ValgtypeID == valg.valgtypeid && b.Valgtype == "Preferansevalg");

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
        public Valgtyper valgPågår()
        {
            var db = new BrukerContext();

            Valgtyper_db valgtype_db = db.Valgtyper.FirstOrDefault(b => b.Start == true);

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
                    start = valgtype_db.Start,
                    melding = valgtype_db.Melding
                };
                return valgtype;
            }
        }

        public string startLagretPreferansevalg(int valgtypeid, string beskrivelse)
        {
            var db = new BrukerContext();

            var start = db.Valgtyper.FirstOrDefault(v => v.ValgtypeID == valgtypeid);
            start.Start = true;
            db.SaveChanges();

            return "OK";
        }
        public Valgtyper PreferansevalgPågår()
        {
            var db = new BrukerContext();

            Valgtyper_db valgtype_db = db.Valgtyper.FirstOrDefault(b => b.Start == true && b.Valgtype == "Preferansevalg");

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
        public bool preferansevalgSvar(string kandidatEn, string kandidatTo, string kandidatTre, string kandidatFire,
                                       string kandidatFem, string kandidatSeks, string kandidatSju, string kandidatÅtte,
                                       string kandidatNi, string kandidatTi, string kandidatElleve, string kandidatTolv)
        {

            var db = new BrukerContext();
            var valgPågår = PreferansevalgPågår();

            if (kandidatEn != null || kandidatTo != null || kandidatTre != null || kandidatFire != null || kandidatFem != null
               || kandidatSeks != null || kandidatSju != null || kandidatÅtte != null || kandidatNi != null || kandidatTi != null
               || kandidatElleve != null || kandidatTolv != null)
            {
                var svar = new Stemmeseddel_db()
                {
                    KandidatnrEn = kandidatEn,
                    KandidatnrTo = kandidatTo,
                    KandidatnrTre = kandidatTre,
                    KandidatnrFire = kandidatFire,
                    KandidatnrFem = kandidatFem,
                    KandidatnrSeks = kandidatSeks,
                    KandidatnrSju = kandidatSju,
                    KandidatnrÅtte = kandidatÅtte,
                    KandidatnrNi = kandidatNi,
                    KandidatnrTi = kandidatTi,
                    KandidatnrElleve = kandidatElleve,
                    KandidatnrTolv = kandidatTolv,
                    ValgtypeID = valgPågår.valgtypeid
                };
                db.Stemmesedler.Add(svar);
                db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        public List<KandidatSTV> hentKandidaterPreferansevalgBruker()
        {
            Valgtyper valgpågår = PreferansevalgPågår();
            List<KandidatSTV> kandidatliste = hentKandidatlisteMedID(valgpågår.valgtypeid);
            return kandidatliste;
        }
        public string stvBrukerStart()
        {
            var db = new BrukerContext();
            string melding = "";

            var valg = db.Valgtyper.FirstOrDefault(v => v.Valgtype == "Preferansevalg" && v.Start == true);

            if (valg != null)
            {
                melding = "Preferansevalg";
            }
            else
            {
                melding = "Feil";
            }

            return melding;
        }
        public Valgtyper stopPreferansevalg()
        {
            var db = new BrukerContext();

            Valgtyper_db stop = db.Valgtyper.FirstOrDefault(b => b.Start == true && b.Valgtype == "Preferansevalg");
            if (stop == null)
            {
                return null;
            }
            stop.Start = false;

            Preferansevalg_db preferansevalg = db.PreferanseValg.FirstOrDefault(p => p.ValgtypeID == stop.ValgtypeID);
            preferansevalg.Utført = true;
            db.SaveChanges();

            Valgtyper valg = new Valgtyper()
            {
                valgtypeid = stop.ValgtypeID,
                valgtype = stop.Valgtype,
                start = stop.Start
            };
            List<KandidatSTV> valgteKandidater = BeregnPreferansevalgResultat(valg.valgtypeid);
            //BeregnStemmetallFørsteRunde(valg.valgtypeid);

            return valg;
        }
        public int AntallstemtPreferansevalg(int valgtypeid)
        {
            var db = new BrukerContext();
            int stemmeliste = db.Stemmesedler.Where(s => s.ValgtypeID == valgtypeid).Count();

            return stemmeliste;
        }
        public int PreferansevalgAntallKandidater(int valgtypeid)
        {
            var db = new BrukerContext();

            int antallKandidater = db.KandidaterSTV.Where(b => b.ValgtypeID == valgtypeid).Count();
            return antallKandidater;
        }
        public int PreferansevalgAntallDeltatt(int valgtypeid)
        {
            var db = new BrukerContext();

            int antallDeltakere = db.BrukereStemt.Where(b => b.ValgtypeID == valgtypeid).Count();
            return antallDeltakere;
        }
        public double BeregnValgtall(int valgtypeid)
        {
            var db = new BrukerContext();
            int antallStemmer = db.Stemmesedler.Where(s => s.ValgtypeID == valgtypeid).Count();
            var preferansevalg = db.PreferanseValg.FirstOrDefault(p => p.ValgtypeID == valgtypeid);
            int antallRepresentanter = preferansevalg.AntallRepresentanter;
            //double divider = (double)antallStemmer / (double)antallRepresentanter;
            double divider = antallStemmer / antallRepresentanter;
            double valgtall = divider + 1 + 0.01;

            return valgtall;
        }

        public void leggTilValgteKandidater(List<KandidatSTV> valgtekandidater)
        {
            var db = new BrukerContext();
            for (int i = 0; i < valgtekandidater.Count(); i++)
            {
                PreferansevalgValgt_db lagreKandidat = new PreferansevalgValgt_db()
                {
                    Navn = valgtekandidater[i].navn,
                    ValgtypeID = valgtekandidater[i].valgtypeid
                };
                db.PreferansevalgValgt.Add(lagreKandidat);
            }
            db.SaveChanges();
        }

        public List<KandidatSTV> BeregnPreferansevalgResultat(int valgtypeid)
        {
            var db = new BrukerContext();
            BeregnStemmetallFørsteRunde(valgtypeid);
            //Under kalle på en metode som beregner valgtallet - foreløpig hardkoder den inn
            //double valgtallTest = BeregnValgtall(valgtypeid);
            var preferansevalg = db.PreferanseValg.FirstOrDefault(p => p.ValgtypeID == valgtypeid);
            int antallRepresentanter = preferansevalg.AntallRepresentanter;
            double valgtall = BeregnValgtall(valgtypeid);

            //int antallRepresentanter =;
            int antallLedigeplasser = antallRepresentanter;
            int valgtTeller = 0;
            int stemmetallEkskludert = 0;
            int tempAntValgt = 0;
            string utskriftTest = "";

            bool valgAvsluttes = false;

            List<KandidatSTV> listeAvKandidater = hentKandidatlisteMedID(valgtypeid);
            List<KandidatSTV> avsluttValg = new List<KandidatSTV>();
            List<KandidatSTV> valgteKandidater = new List<KandidatSTV>();
            List<KandidatSTV> gjenståendeKandidater = new List<KandidatSTV>();
            List<KandidatSTV> hjelpGjenståendekandidater = new List<KandidatSTV>();
            List<KandidatSTV> ekskluderteKandidater = new List<KandidatSTV>();
            List<KandidatSTV> kandidaterEkskludert2 = new List<KandidatSTV>();

            //Om antall kandidater er mindre enn antall representanter som velges
            /*if (listeAvKandidater.Count() < antallRepresentanter)
            {
                return null;
            }*/

            //Fordele kandidater i riktig lister, etter å ha samlet stemmetallene
            if (listeAvKandidater.Count() == antallRepresentanter)
            {
                for (int i = 0; i < listeAvKandidater.Count(); i++)
                {
                    valgteKandidater.Add(listeAvKandidater[i]);
                }
                leggTilValgteKandidater(valgteKandidater);
                return valgteKandidater;
            }
            //Sjekker om valget har blitt regnet ut før - 
            int hjelpTellerAvsluttet = 0;
            for (int i = 0; i < listeAvKandidater.Count(); i++)
            {
                if (listeAvKandidater[i].stemmetall > 0)
                {
                    avsluttValg.Add(listeAvKandidater[i]);
                    hjelpTellerAvsluttet++;
                }
            }
            if (hjelpTellerAvsluttet == antallRepresentanter)
            {
                leggTilValgteKandidater(valgteKandidater);
                return avsluttValg;
            }

            for (int i = 0; i < listeAvKandidater.Count(); i++)
            {
                KandidatSTV sjekkKandidat = hentEnKandidatNavnogID(valgtypeid, listeAvKandidater[i].navn);
                if (sjekkKandidat.stemmetall >= valgtall)
                {
                    if (valgteKandidater.Count() == antallRepresentanter)
                    {
                        break;
                    }
                    valgteKandidater.Add(sjekkKandidat);
                    valgtTeller++;
                    antallLedigeplasser--;

                }
                else
                {
                    gjenståendeKandidater.Add(sjekkKandidat);
                }
            }
            tempAntValgt = valgteKandidater.Count();

            //Sjekker om ledige plasser er fylt opp etter første runde, om de er avsluttes valget
            if (valgteKandidater.Count() >= antallRepresentanter)
            {
                valgAvsluttes = true;
                for (int i = 0; i < gjenståendeKandidater.Count(); i++)
                {
                    KandidatListeSTV endreKandidat = db.KandidaterSTV.FirstOrDefault(b => b.Navn == gjenståendeKandidater[i].navn && b.ValgtypeID == valgtypeid);
                    endreKandidat.KandidatID = gjenståendeKandidater[i].kandidatListeID;
                    endreKandidat.Navn = gjenståendeKandidater[i].navn;
                    endreKandidat.Stemmetall = 0;
                }
                db.SaveChanges();
                leggTilValgteKandidater(valgteKandidater);
                return valgteKandidater;
            }

            //Om antall gjenstående kandidater er mindre eller lik antall ledige plasser
            if (gjenståendeKandidater.Count() <= antallLedigeplasser)
            {
                //Alle gjenstående kandidater blir valgt
                for (int i = 0; i <= gjenståendeKandidater.Count(); i++)
                {
                    valgteKandidater.Add(gjenståendeKandidater[i]);
                    //gjenståendeKandidater.Remove(gjenståendeKandidater[i]);
                    valgtTeller++;
                    antallLedigeplasser--;
                }
                valgAvsluttes = true;
                leggTilValgteKandidater(valgteKandidater);
                return valgteKandidater;
            }
            tempAntValgt = valgteKandidater.Count();

            while (valgteKandidater.Count() < antallRepresentanter && valgAvsluttes == false)
            {
                //Finner laveste stemmetallet i gj.kandidater etter overført etter første runde
                stemmetallEkskludert = gjenståendeKandidater[0].stemmetall;
                for (int i = 0; i < gjenståendeKandidater.Count(); i++)
                {
                    if (stemmetallEkskludert == 0)
                    {
                        stemmetallEkskludert = gjenståendeKandidater[i].stemmetall;
                    }
                }
                for (int i = 0; i < gjenståendeKandidater.Count(); i++)
                {
                    int tempStemmetall = gjenståendeKandidater[i].stemmetall;
                    if (tempStemmetall < stemmetallEkskludert)
                    {
                        if (tempStemmetall != 0)
                        {
                            stemmetallEkskludert = tempStemmetall;
                        }

                    }
                }
                //Finner høyest stemmetallet
                int størstStemmetall = 0;
                for (int i = 0; i < gjenståendeKandidater.Count(); i++)
                {
                    int tempStørstStemmetall = gjenståendeKandidater[i].stemmetall;

                    if (størstStemmetall < tempStørstStemmetall)
                    {
                        størstStemmetall = tempStørstStemmetall;
                    }
                }

                //Eksludere de med laveste stemmetallet -- Funker -- Må kanskje lage den summe opp og plusse greia
                List<KandidatSTV> alleEksluderte = gjenståendeKandidater.FindAll(l => l.stemmetall == stemmetallEkskludert);

                int hjelpTeller = gjenståendeKandidater.Count() - 1;
                int sumEkskluderteStemmetall = 0;
                bool hjelpBool = false;
                for (int i = 0; i < alleEksluderte.Count(); i++)
                {
                    KandidatSTV kandidatEkskluderes = alleEksluderte[i];
                    sumEkskluderteStemmetall += kandidatEkskluderes.stemmetall;
                    for (int k = 0; k < gjenståendeKandidater.Count(); k++)
                    {
                        //int hjelpAntGjenstående = hjelpTeller - 1;
                        if (gjenståendeKandidater.Count() == 2 && hjelpBool == false)
                        {
                            ekskluderteKandidater.Add(kandidatEkskluderes);
                            hjelpTeller--;
                            hjelpBool = true;
                        }

                        if (hjelpTeller >= antallLedigeplasser)
                        {
                            if (hjelpTeller == antallLedigeplasser)
                            {
                                //For å slette riktig
                                for (int ind = 0; ind < gjenståendeKandidater.Count(); ind++)
                                {
                                    hjelpGjenståendekandidater.Add(gjenståendeKandidater[ind]);
                                }

                                for (int s = 0; s <= gjenståendeKandidater.Count(); s++)
                                {
                                    if (valgteKandidater.Count() == antallRepresentanter)
                                    {
                                        if (hjelpGjenståendekandidater.Count() > 0)
                                        {
                                            for (int index = 0; index < hjelpGjenståendekandidater.Count(); index++)
                                            {
                                                KandidatSTV oppdaterKandidat = hentEnKandidatNavnogID(valgtypeid, hjelpGjenståendekandidater[index].navn);
                                                string kandidatnavn = oppdaterKandidat.navn;
                                                int tempStemmetall = oppdaterKandidat.stemmetall + 1;

                                                KandidatListeSTV endreKandidat = db.KandidaterSTV.FirstOrDefault(b => b.Navn == kandidatnavn && b.ValgtypeID == valgtypeid);
                                                endreKandidat.KandidatID = oppdaterKandidat.kandidatListeID;
                                                endreKandidat.Navn = oppdaterKandidat.navn;
                                                endreKandidat.Stemmetall = 0;

                                            }
                                            db.SaveChanges();
                                        }
                                        leggTilValgteKandidater(valgteKandidater);
                                        return valgteKandidater;
                                    }
                                    valgteKandidater.Add(gjenståendeKandidater[s]);
                                    hjelpGjenståendekandidater.Remove(gjenståendeKandidater[s]);
                                    antallLedigeplasser--;
                                }
                                //return valgteKandidater;
                            }
                            else
                            {
                                if (kandidatEkskluderes.navn == gjenståendeKandidater[k].navn)
                                {
                                    if (sumEkskluderteStemmetall <= størstStemmetall)
                                    {
                                        ekskluderteKandidater.Add(kandidatEkskluderes);
                                        hjelpTeller--;
                                        break;
                                    }
                                }
                            }
                        }

                    }
                }
                //--------------------Alle ekskludert ferdig

                //Overføre overskudd
                if (valgteKandidater.Count() > 0)
                {
                    for (int i = 0; i < valgteKandidater.Count(); i++)
                    {
                        int overskudd = 0;
                        KandidatSTV valgtKandidat = valgteKandidater[i];
                        int tempValgtall = (int)Math.Ceiling(valgtall);
                        overskudd = valgtKandidat.stemmetall - tempValgtall;

                        if (overskudd > 0)
                        {
                            List<Stemmeseddel_db> overførStemmer = db.Stemmesedler.Where(s => s.KandidatnrEn == valgtKandidat.navn && s.ValgtypeID == valgtypeid).ToList();
                            for (int k = 0; k < overskudd; k++)
                            {

                                Stemmeseddel_db stemme = overførStemmer[k];
                                string seddelNavnet = "";
                                string navnToPåSeddel = stemme.KandidatnrTo;
                                string navnTrePåSeddel = stemme.KandidatnrTre;
                                string navnFirePåSeddel = stemme.KandidatnrFire;
                                string navnFemPåSeddel = stemme.KandidatnrFem;
                                bool funnet = false;

                                if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnToPåSeddel) != null && !funnet)
                                {
                                    seddelNavnet = navnToPåSeddel;
                                    funnet = true;
                                }
                                else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnTrePåSeddel) != null && !funnet)
                                {
                                    seddelNavnet = navnTrePåSeddel;
                                    funnet = true;
                                }
                                else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnFirePåSeddel) != null && !funnet)
                                {
                                    seddelNavnet = navnFirePåSeddel;
                                    funnet = true;
                                }
                                else
                                {
                                    overskudd++;
                                }

                                if (seddelNavnet != "")
                                {
                                    int tempStemmetallValgtKandidat = valgtKandidat.stemmetall - 1;

                                    KandidatSTV oppdaterKandidat = gjenståendeKandidater.FirstOrDefault(b => b.navn == seddelNavnet);
                                    int tempStemmetallet = oppdaterKandidat.stemmetall + 1;
                                    oppdaterKandidat.stemmetall = tempStemmetallet;

                                    KandidatListeSTV endreKandidat = db.KandidaterSTV.FirstOrDefault(b => b.Navn == seddelNavnet && b.ValgtypeID == valgtypeid);
                                    endreKandidat.KandidatID = oppdaterKandidat.kandidatListeID;
                                    endreKandidat.Navn = oppdaterKandidat.navn;
                                    endreKandidat.Stemmetall = tempStemmetallet;

                                    KandidatListeSTV endreValgtKandidat = db.KandidaterSTV.FirstOrDefault(b => b.Navn == valgtKandidat.navn && b.ValgtypeID == valgtypeid);
                                    endreValgtKandidat.KandidatID = valgtKandidat.kandidatListeID;
                                    endreValgtKandidat.Navn = valgtKandidat.navn;
                                    endreValgtKandidat.Stemmetall = tempStemmetallValgtKandidat;

                                    valgtKandidat.stemmetall = tempStemmetallValgtKandidat;


                                    for (int j = 0; j < ekskluderteKandidater.Count(); j++)
                                    {
                                        if (ekskluderteKandidater[j].navn == seddelNavnet)
                                        {
                                            ekskluderteKandidater.Remove(ekskluderteKandidater[j]);
                                        }
                                    }

                                    if (tempStemmetallet >= valgtall)
                                    {
                                        valgteKandidater.Add(oppdaterKandidat);
                                        valgtTeller++;
                                        antallLedigeplasser--;
                                        if (valgteKandidater.Count() >= antallRepresentanter)
                                        {
                                            valgAvsluttes = true;
                                            for (int l = 0; l < gjenståendeKandidater.Count(); l++)
                                            {
                                                KandidatListeSTV eKandidat = db.KandidaterSTV.FirstOrDefault(b => b.Navn == gjenståendeKandidater[l].navn && b.ValgtypeID == valgtypeid);
                                                eKandidat.KandidatID = gjenståendeKandidater[l].kandidatListeID;
                                                eKandidat.Navn = gjenståendeKandidater[l].navn;
                                                eKandidat.Stemmetall = 0;
                                            }
                                            db.SaveChanges();
                                            leggTilValgteKandidater(valgteKandidater);
                                            return valgteKandidater;
                                        }
                                    }
                                    funnet = true;
                                }
                            }
                        }
                    }
                    db.SaveChanges();
                }
                tempAntValgt = valgteKandidater.Count();

                //Ekskluder kandidat removes fra gj.liste riktig - Etter overføring
                for (int i = 0; i < ekskluderteKandidater.Count(); i++)
                {
                    KandidatSTV ekskludertKandidat = gjenståendeKandidater.FirstOrDefault(b => b.navn == ekskluderteKandidater[i].navn);
                    if (ekskludertKandidat != null)
                    {
                        gjenståendeKandidater.Remove(ekskludertKandidat); //Fjerner riktig kandidat
                    }
                }
                for (int i = 0; i < gjenståendeKandidater.Count(); i++)
                {
                    if (gjenståendeKandidater[i].stemmetall >= valgtall)
                    {
                        gjenståendeKandidater.Remove(gjenståendeKandidater[i]);
                    }
                }


                //Sjekker om valget kan avsluttes - Etter å ha overført første gang
                if (valgteKandidater.Count() >= antallRepresentanter)
                {
                    valgAvsluttes = true;
                    for (int l = 0; l < gjenståendeKandidater.Count(); l++)
                    {
                        KandidatListeSTV eKandidat = db.KandidaterSTV.FirstOrDefault(b => b.Navn == gjenståendeKandidater[l].navn && b.ValgtypeID == valgtypeid);
                        eKandidat.KandidatID = gjenståendeKandidater[l].kandidatListeID;
                        eKandidat.Navn = gjenståendeKandidater[l].navn;
                        eKandidat.Stemmetall = 0;
                    }
                    db.SaveChanges();
                    leggTilValgteKandidater(valgteKandidater);
                    return valgteKandidater;
                }
                //Om antall gjenstående kandidater er mindre eller lik antall ledige plasser - Etter å ha overført første gang
                if (gjenståendeKandidater.Count() <= antallLedigeplasser)
                {
                    for (int i = 0; i < gjenståendeKandidater.Count(); i++)
                    {
                        valgteKandidater.Add(gjenståendeKandidater[i]);
                        //gjenståendeKandidater.Remove(gjenståendeKandidater[i]);
                        antallLedigeplasser--;
                    }
                    for (int i = 0; i < ekskluderteKandidater.Count(); i++)
                    {
                        string navnEkskludert = ekskluderteKandidater[i].navn;
                        KandidatListeSTV ekskludereKandidat = db.KandidaterSTV.FirstOrDefault(b => b.Navn == navnEkskludert && b.ValgtypeID == valgtypeid);
                        ekskludereKandidat.KandidatID = ekskluderteKandidater[i].kandidatListeID;
                        ekskludereKandidat.Navn = ekskluderteKandidater[i].navn;
                        ekskludereKandidat.Stemmetall = 0;
                    }
                    db.SaveChanges();
                    valgAvsluttes = true;
                    leggTilValgteKandidater(valgteKandidater);
                    return valgteKandidater;
                }

                //Eksluderte sine stemmer overføres -- Funker 
                for (int i = 0; i < ekskluderteKandidater.Count(); i++)
                {
                    string navnEkskludert = ekskluderteKandidater[i].navn;
                    int tempStemmetallEksKandidat = ekskluderteKandidater[i].stemmetall - 1;
                    List<Stemmeseddel_db> listeEkskludert = db.Stemmesedler.Where(b => b.KandidatnrEn == navnEkskludert && b.ValgtypeID == valgtypeid).ToList();

                    for (int j = 0; j < listeEkskludert.Count(); j++)
                    {
                        string seddelNavnet = "";
                        string navnToPåSeddel = listeEkskludert[j].KandidatnrTo;
                        string navnTrePåSeddel = listeEkskludert[j].KandidatnrTre;
                        string navnFirePåSeddel = listeEkskludert[j].KandidatnrFire;
                        string navnFemPåSeddel = listeEkskludert[j].KandidatnrFem;
                        bool funnet = false;

                        if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnToPåSeddel) != null && !funnet)
                        {
                            seddelNavnet = navnToPåSeddel;
                            funnet = true;
                        }
                        else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnTrePåSeddel) != null && !funnet)
                        {
                            seddelNavnet = navnTrePåSeddel;
                            funnet = true;
                        }
                        else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnFirePåSeddel) != null && !funnet)
                        {
                            seddelNavnet = navnFirePåSeddel;
                            funnet = true;
                        }
                        else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnFemPåSeddel) != null && !funnet)
                        {
                            seddelNavnet = navnFemPåSeddel;
                            funnet = true;
                        }

                        if (seddelNavnet == "")
                        {
                            utskriftTest += "Seddelen var tom as";
                        }
                        else
                        {
                            tempStemmetallEksKandidat = ekskluderteKandidater[i].stemmetall - 1;

                            KandidatSTV oppdaterKandidat = gjenståendeKandidater.FirstOrDefault(b => b.navn == seddelNavnet);
                            int tempStemmetallet = oppdaterKandidat.stemmetall + 1;
                            oppdaterKandidat.stemmetall = tempStemmetallet;

                            KandidatListeSTV endreKandidat = db.KandidaterSTV.FirstOrDefault(b => b.Navn == seddelNavnet && b.ValgtypeID == valgtypeid);
                            endreKandidat.KandidatID = oppdaterKandidat.kandidatListeID;
                            endreKandidat.Navn = oppdaterKandidat.navn;
                            endreKandidat.Stemmetall = tempStemmetallet;

                            KandidatListeSTV ekskludereKandidat = db.KandidaterSTV.FirstOrDefault(b => b.Navn == navnEkskludert && b.ValgtypeID == valgtypeid);
                            ekskludereKandidat.KandidatID = ekskluderteKandidater[i].kandidatListeID;
                            ekskludereKandidat.Navn = ekskluderteKandidater[i].navn;
                            ekskludereKandidat.Stemmetall = tempStemmetallEksKandidat;

                            ekskluderteKandidater[i].stemmetall = tempStemmetallEksKandidat;

                            //db.SaveChanges();

                            for (int k = 0; k < ekskluderteKandidater.Count(); k++)
                            {
                                if (ekskluderteKandidater[k].navn == seddelNavnet)
                                {
                                    ekskluderteKandidater.Remove(oppdaterKandidat);
                                }
                            }

                            if (tempStemmetallet >= valgtall)
                            {
                                if (valgteKandidater.FirstOrDefault(b => b.navn == oppdaterKandidat.navn) == null)
                                {
                                    valgteKandidater.Add(oppdaterKandidat);
                                    valgtTeller++;
                                    antallLedigeplasser--;

                                    //Sjekker om valget kan avsluttes - Etter å ha overført første gang
                                    if (valgteKandidater.Count() >= antallRepresentanter)
                                    {
                                        valgAvsluttes = true;
                                        for (int l = 0; l < gjenståendeKandidater.Count(); l++)
                                        {
                                            KandidatListeSTV eKandidat = db.KandidaterSTV.FirstOrDefault(b => b.Navn == gjenståendeKandidater[l].navn && b.ValgtypeID == valgtypeid);
                                            eKandidat.KandidatID = gjenståendeKandidater[l].kandidatListeID;
                                            eKandidat.Navn = gjenståendeKandidater[l].navn;
                                            eKandidat.Stemmetall = 0;
                                        }
                                        db.SaveChanges();
                                        leggTilValgteKandidater(valgteKandidater);
                                        return valgteKandidater;
                                    }
                                }
                            }
                        }
                    }

                    KandidatListeSTV ekskludereKandidatTilslutt = db.KandidaterSTV.FirstOrDefault(b => b.Navn == navnEkskludert && b.ValgtypeID == valgtypeid);
                    ekskludereKandidatTilslutt.KandidatID = ekskluderteKandidater[i].kandidatListeID;
                    ekskludereKandidatTilslutt.Navn = ekskluderteKandidater[i].navn;
                    ekskludereKandidatTilslutt.Stemmetall = 0;

                    ekskluderteKandidater[i].stemmetall = 0;
                }
                db.SaveChanges();

                //Fjerner valgt kandidat fra gjenstående kandidater lista
                for (int i = 0; i < gjenståendeKandidater.Count(); i++)
                {
                    int tempcount = gjenståendeKandidater.Count() - 1;
                    if (gjenståendeKandidater[i].stemmetall >= valgtall)
                    {
                        gjenståendeKandidater.Remove(gjenståendeKandidater[i]);
                    }
                    else if (gjenståendeKandidater[i].stemmetall == 0 && tempcount >= antallLedigeplasser)
                    {
                        gjenståendeKandidater.Remove(gjenståendeKandidater[i]);
                    }
                }

                //Sjekker om valget kan avsluttes - Etter å ha overført første gang
                if (valgteKandidater.Count() >= antallRepresentanter)
                {
                    valgAvsluttes = true;
                    for (int l = 0; l < gjenståendeKandidater.Count(); l++)
                    {
                        KandidatListeSTV eKandidat = db.KandidaterSTV.FirstOrDefault(b => b.Navn == gjenståendeKandidater[l].navn && b.ValgtypeID == valgtypeid);
                        eKandidat.KandidatID = gjenståendeKandidater[l].kandidatListeID;
                        eKandidat.Navn = gjenståendeKandidater[l].navn;
                        eKandidat.Stemmetall = 0;
                    }
                    db.SaveChanges();
                    leggTilValgteKandidater(valgteKandidater);
                    return valgteKandidater;
                }
                //Om antall gjenstående kandidater er mindre eller lik antall ledige plasser - Etter å ha overført første gang
                if (gjenståendeKandidater.Count() <= antallLedigeplasser)
                {
                    for (int i = 0; i < gjenståendeKandidater.Count(); i++)
                    {
                        valgteKandidater.Add(gjenståendeKandidater[i]);
                        //gjenståendeKandidater.Remove(gjenståendeKandidater[i]);
                        antallLedigeplasser--;
                    }
                    for (int i = 0; i < ekskluderteKandidater.Count(); i++)
                    {
                        string navnEkskludert = ekskluderteKandidater[i].navn;
                        KandidatListeSTV ekskludereKandidat = db.KandidaterSTV.FirstOrDefault(b => b.Navn == navnEkskludert && b.ValgtypeID == valgtypeid);
                        ekskludereKandidat.KandidatID = ekskluderteKandidater[i].kandidatListeID;
                        ekskludereKandidat.Navn = ekskluderteKandidater[i].navn;
                        ekskludereKandidat.Stemmetall = 0;
                    }
                    db.SaveChanges();
                    valgAvsluttes = true;
                    leggTilValgteKandidater(valgteKandidater);
                    return valgteKandidater;
                }

                //Overskudd igjen
                //Sjekke om noen har blitt valgt og fikk overskudd etter forrige ekskludering
                if (tempAntValgt < valgteKandidater.Count()) //Om det har kommet inn fler valgte siden sist - overfør deres stemme
                {
                    for (int i = 0; i < valgteKandidater.Count(); i++)
                    {
                        KandidatSTV valgtKandidat = valgteKandidater[i];
                        int tempValgtall = (int)Math.Ceiling(valgtall);
                        int overskudd = valgtKandidat.stemmetall - tempValgtall;
                        overskudd = valgtKandidat.stemmetall - tempValgtall;

                        if (overskudd > 0)
                        {
                            List<Stemmeseddel_db> overførStemmer = db.Stemmesedler.Where(s => s.KandidatnrEn == valgtKandidat.navn && s.ValgtypeID == valgtypeid).ToList();
                            for (int k = 0; k < overskudd; k++)
                            {
                                Stemmeseddel_db stemme = overførStemmer[k];
                                string seddelNavnet = "";
                                string navnToPåSeddel = stemme.KandidatnrTo;
                                string navnTrePåSeddel = stemme.KandidatnrTre;
                                string navnFirePåSeddel = stemme.KandidatnrFire;
                                string navnFemPåSeddel = stemme.KandidatnrFem;
                                bool funnet = false;

                                if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnToPåSeddel) != null && !funnet)
                                {
                                    seddelNavnet = navnToPåSeddel;
                                    funnet = true;
                                }
                                else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnTrePåSeddel) != null && !funnet)
                                {
                                    seddelNavnet = navnTrePåSeddel;
                                    funnet = true;
                                }
                                else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnFirePåSeddel) != null && !funnet)
                                {
                                    seddelNavnet = navnFirePåSeddel;
                                    funnet = true;
                                }
                                else
                                {
                                    overskudd++;
                                }

                                if (seddelNavnet != "")
                                {
                                    int tempStemmetallValgtKandidat = valgtKandidat.stemmetall - 1;

                                    KandidatSTV oppdaterKandidat = gjenståendeKandidater.FirstOrDefault(b => b.navn == seddelNavnet);
                                    int tempStemmetallet = oppdaterKandidat.stemmetall + 1;
                                    oppdaterKandidat.stemmetall = tempStemmetallet;

                                    KandidatListeSTV endreKandidat = db.KandidaterSTV.FirstOrDefault(b => b.Navn == seddelNavnet && b.ValgtypeID == valgtypeid);
                                    endreKandidat.KandidatID = oppdaterKandidat.kandidatListeID;
                                    endreKandidat.Navn = oppdaterKandidat.navn;
                                    endreKandidat.Stemmetall = tempStemmetallet;

                                    KandidatListeSTV endreValgtKandidat = db.KandidaterSTV.FirstOrDefault(b => b.Navn == valgtKandidat.navn && b.ValgtypeID == valgtypeid);
                                    endreValgtKandidat.KandidatID = valgtKandidat.kandidatListeID;
                                    endreValgtKandidat.Navn = valgtKandidat.navn;
                                    endreValgtKandidat.Stemmetall = tempStemmetallValgtKandidat;

                                    valgtKandidat.stemmetall = tempStemmetallValgtKandidat;

                                    db.SaveChanges();

                                    for (int j = 0; j < ekskluderteKandidater.Count(); j++)
                                    {
                                        if (ekskluderteKandidater[j].navn == seddelNavnet)
                                        {
                                            ekskluderteKandidater.Remove(ekskluderteKandidater[j]);
                                        }
                                    }

                                    if (tempStemmetallet >= valgtall)
                                    {
                                        valgteKandidater.Add(oppdaterKandidat);
                                        valgtTeller++;
                                        antallLedigeplasser--;

                                        if (valgteKandidater.Count() >= antallRepresentanter)
                                        {
                                            valgAvsluttes = true;
                                            for (int l = 0; l < gjenståendeKandidater.Count(); l++)
                                            {
                                                KandidatListeSTV eKandidat = db.KandidaterSTV.FirstOrDefault(b => b.Navn == gjenståendeKandidater[l].navn && b.ValgtypeID == valgtypeid);
                                                eKandidat.KandidatID = gjenståendeKandidater[l].kandidatListeID;
                                                eKandidat.Navn = gjenståendeKandidater[l].navn;
                                                eKandidat.Stemmetall = 0;
                                            }
                                            db.SaveChanges();
                                            leggTilValgteKandidater(valgteKandidater);
                                            return valgteKandidater;
                                        }
                                    }
                                    funnet = true;
                                }
                            }
                        }
                    }
                    tempAntValgt = valgteKandidater.Count();

                    //Ekskluder kandidat removes fra gj.liste riktig - Etter overføring
                    for (int i = 0; i < ekskluderteKandidater.Count(); i++)
                    {
                        KandidatSTV ekskludertKandidat = gjenståendeKandidater.FirstOrDefault(b => b.navn == ekskluderteKandidater[i].navn);
                        if (ekskludertKandidat != null)
                        {
                            gjenståendeKandidater.Remove(ekskludertKandidat); //Fjerner riktig kandidat
                        }
                    }

                    //Fjerner valgt kandidat fra gjenstående kandidater lista
                    for (int i = 0; i < gjenståendeKandidater.Count(); i++)
                    {
                        if (gjenståendeKandidater[i].stemmetall >= valgtall)
                        {
                            gjenståendeKandidater.Remove(gjenståendeKandidater[i]);
                        }
                    }
                }

                //Sjekker om valget kan avsluttes - Etter å ha overført første gang
                if (valgteKandidater.Count() >= antallRepresentanter)
                {
                    valgAvsluttes = true;
                    for (int l = 0; l < gjenståendeKandidater.Count(); l++)
                    {
                        KandidatListeSTV eKandidat = db.KandidaterSTV.FirstOrDefault(b => b.Navn == gjenståendeKandidater[l].navn && b.ValgtypeID == valgtypeid);
                        eKandidat.KandidatID = gjenståendeKandidater[l].kandidatListeID;
                        eKandidat.Navn = gjenståendeKandidater[l].navn;
                        eKandidat.Stemmetall = 0;
                    }
                    db.SaveChanges();
                    leggTilValgteKandidater(valgteKandidater);
                    return valgteKandidater;
                }
                //Om antall gjenstående kandidater er mindre eller lik antall ledige plasser - Etter å ha overført første gang
                if (gjenståendeKandidater.Count() <= antallLedigeplasser)
                {
                    for (int i = 0; i < gjenståendeKandidater.Count(); i++)
                    {
                        valgteKandidater.Add(gjenståendeKandidater[i]);
                        //gjenståendeKandidater.Remove(gjenståendeKandidater[i]);
                        antallLedigeplasser--;
                    }
                    for (int i = 0; i < ekskluderteKandidater.Count(); i++)
                    {
                        string navnEkskludert = ekskluderteKandidater[i].navn;
                        KandidatListeSTV ekskludereKandidat = db.KandidaterSTV.FirstOrDefault(b => b.Navn == navnEkskludert && b.ValgtypeID == valgtypeid);
                        ekskludereKandidat.KandidatID = ekskluderteKandidater[i].kandidatListeID;
                        ekskludereKandidat.Navn = ekskluderteKandidater[i].navn;
                        ekskludereKandidat.Stemmetall = 0;
                    }
                    db.SaveChanges();
                    valgAvsluttes = true;
                    leggTilValgteKandidater(valgteKandidater);
                    return valgteKandidater;
                }

                for (int i = 0; i < ekskluderteKandidater.Count(); i++)
                {
                    kandidaterEkskludert2.Add(ekskluderteKandidater[i]);
                }

                ekskluderteKandidater.Clear();
            }//While stoppes
            leggTilValgteKandidater(valgteKandidater);
            return valgteKandidater;
        }

        public Preferansevalg hentPreferanseValg(int id)
        {
            var db = new BrukerContext();

            var enDbValg = db.PreferanseValg.FirstOrDefault(b => b.ValgtypeID == id);

            if (enDbValg == null)
            {
                return null;
            }
            else
            {
                var utBruker = new Preferansevalg()
                {
                    id = enDbValg.Id,
                    beskrivelse = enDbValg.Beskrivelse,
                    antallrepresentanter = enDbValg.AntallRepresentanter,
                    utført = enDbValg.Utført,
                    valgtypeid = enDbValg.ValgtypeID
                };
                return utBruker;
            }
        }
        public KandidatSTV hentEnKandidatNavnogID(int valgtypeid, string navn)
        {
            var db = new BrukerContext();

            var enDbKandidat = db.KandidaterSTV.FirstOrDefault(b => b.Navn == navn && b.ValgtypeID == valgtypeid);
            if (enDbKandidat == null)
            {
                return null;
            }
            else
            {
                var utKandidat = new KandidatSTV()
                {
                    kandidatListeID = enDbKandidat.KandidatID,
                    navn = enDbKandidat.Navn,
                    stemmetall = enDbKandidat.Stemmetall,
                    stemmetallsatt = enDbKandidat.StemmetallSatt,
                    valgtypeid = enDbKandidat.ValgtypeID
                };
                return utKandidat;
            }
        }
        public List<Stemmeseddel> listeAvStemmesedler(int valgtypeid)
        {
            var db = new BrukerContext();
            var listeStemmer = db.Stemmesedler.Where(s => s.ValgtypeID == valgtypeid).ToList();
            List<Stemmeseddel> returListe = new List<Stemmeseddel>();

            for (int i = 0; i < listeStemmer.Count(); i++)
            {
                Stemmeseddel nyStemme = new Stemmeseddel()
                {
                    stemmeseddelID = listeStemmer[i].StemmeseddelID,
                    kandidatnrEn = listeStemmer[i].KandidatnrEn,
                    kandidatnrTo = listeStemmer[i].KandidatnrTo,
                    kandidatnrTre = listeStemmer[i].KandidatnrTre,
                    kandidatnrFire = listeStemmer[i].KandidatnrFire,
                    kandidatnrFem = listeStemmer[i].KandidatnrFem,
                    kandidatnrSeks = listeStemmer[i].KandidatnrSeks,
                    kandidatnrSju = listeStemmer[i].KandidatnrSju,
                    kandidatnrÅtte = listeStemmer[i].KandidatnrÅtte,
                    kandidatnrNi = listeStemmer[i].KandidatnrNi,
                    kandidatnrTi = listeStemmer[i].KandidatnrTi,
                    kandidatnrElleve = listeStemmer[i].KandidatnrElleve,
                    kandidatnrTolv = listeStemmer[i].KandidatnrTolv,
                    valgtypeid = listeStemmer[i].ValgtypeID
                };
                returListe.Add(nyStemme);
            }

            return returListe;
        }
        public VaraSTV hentEnVaraNavnogID(int valgtypeid, string navn)
        {
            var db = new BrukerContext();

            var enDbKandidat = db.PreferansevalgVaraer.FirstOrDefault(b => b.Navn == navn && b.ValgtypeID == valgtypeid);
            if (enDbKandidat == null)
            {
                return null;
            }
            else
            {
                var utKandidat = new VaraSTV()
                {
                    varaKandidatID = enDbKandidat.VaraKandidatID,
                    navn = enDbKandidat.Navn,
                    stemmetall = enDbKandidat.Stemmetall,
                    stemmetallsatt = enDbKandidat.StemmetallSatt,
                    valgtypeid = enDbKandidat.ValgtypeID
                };
                return utKandidat;
            }
        }
        public List<VaraSTV> hentVaralisteMedID(int valgtypeid)
        {
            var db = new BrukerContext();
            //var kandidatliste = db.KandidaterSTV.Where(k => k.ValgtypeID == valgtypeid).ToList();
            var varaListe = db.PreferansevalgVaraer.Where(v => v.ValgtypeID == valgtypeid).ToList();

            List<VaraSTV> returListe = new List<VaraSTV>();
            int antallKandidat = varaListe.Count();
            for (int i = 0; i < antallKandidat; i++)
            {
                VaraSTV nyKandidat = new VaraSTV()
                {
                    varaKandidatID = varaListe[i].VaraKandidatID,
                    navn = varaListe[i].Navn,
                    stemmetall = varaListe[i].Stemmetall,
                    stemmetallsatt = varaListe[i].StemmetallSatt,
                    valgtypeid = varaListe[i].ValgtypeID
                };
                returListe.Add(nyKandidat);
            }
            return returListe;
        }
        public void BeregnStemmetallFørsteRunde(int valgtypeid)
        {
            var db = new BrukerContext();
            List<KandidatSTV> listeAvKandidater = hentKandidatlisteMedID(valgtypeid); //Inneholder liste av kandidater som deltar
            List<VaraSTV> listeVarakandidater = hentVaralisteMedID(valgtypeid); // Kanskje den ikke trengs
            List<Stemmeseddel> stemmesedler = listeAvStemmesedler(valgtypeid); //Inneholder liste av stemmesedler

            for (int i = 0; i < listeAvKandidater.Count(); i++)
            {
                string kandidatnavn = listeAvKandidater[i].navn;
                string varaNavn = listeVarakandidater[i].navn;

                for (int j = 0; j < stemmesedler.Count(); j++)
                {
                    if (kandidatnavn == stemmesedler[j].kandidatnrEn)
                    {
                        KandidatSTV oppdaterKandidat = hentEnKandidatNavnogID(valgtypeid, kandidatnavn);
                        int tempStemmetall = oppdaterKandidat.stemmetall + 1;

                        VaraSTV oppdaterVara = hentEnVaraNavnogID(valgtypeid, kandidatnavn);


                        KandidatListeSTV endreKandidat = db.KandidaterSTV.FirstOrDefault(b => b.Navn == kandidatnavn && b.ValgtypeID == valgtypeid);
                        endreKandidat.KandidatID = oppdaterKandidat.kandidatListeID;
                        endreKandidat.Navn = oppdaterKandidat.navn;
                        endreKandidat.Stemmetall = tempStemmetall;

                        VaraListeSTV endreVaraKandidat = db.PreferansevalgVaraer.FirstOrDefault(b => b.Navn == varaNavn && b.ValgtypeID == valgtypeid);
                        endreVaraKandidat.VaraKandidatID = oppdaterVara.varaKandidatID;
                        endreVaraKandidat.Navn = oppdaterVara.navn;
                        endreVaraKandidat.Stemmetall = tempStemmetall;

                        db.SaveChanges();
                    }
                }
            }//Hoved for-løkke stopp
        }
        public string lagreNyttPreferansevalg(string beskrivelse, int antallRepresentanter, int antallVaraRepresentanter)
        {
            var db = new BrukerContext();

            if (beskrivelse == "")
            {
                return "feil";
            }
            if (antallRepresentanter < 2 || antallRepresentanter == 0)
            {
                return "feil";
            }

            var lagrevalg = new Valgtyper_db()
            {
                Valgtype = "Preferansevalg",
                Start = false
            };
            db.Valgtyper.Add(lagrevalg);
            db.SaveChanges();

            var valg = new Preferansevalg_db()
            {
                Beskrivelse = beskrivelse,
                AntallRepresentanter = antallRepresentanter,
                AntallVaraRepresentanter = antallVaraRepresentanter,
                Utført = false,
                ValgtypeID = lagrevalg.ValgtypeID
            };

            db.PreferanseValg.Add(valg);
            //db.SaveChanges();

            //Setter deltakende kandidater sin ValgId lik Valget som kjører
            var kandidater = db.KandidaterSTV.Where(k => k.StemmetallSatt == false && k.Stemmetall == 0).ToList();
            var varaKandidater = db.PreferansevalgVaraer.Where(v => v.StemmetallSatt == false && v.Stemmetall == 0).ToList();
            for (int i = 0; i < kandidater.Count(); i++)
            {
                kandidater[i].ValgtypeID = lagrevalg.ValgtypeID;
                kandidater[i].StemmetallSatt = true;
                varaKandidater[i].ValgtypeID = lagrevalg.ValgtypeID;
                varaKandidater[i].StemmetallSatt = true;
            }
            db.SaveChanges();

            return "OK";
        }
        public string NullstillKandidatliste(int valgtypeid)
        {
            var db = new BrukerContext();
            List<KandidatListeSTV> kandidater = db.KandidaterSTV.Where(k => k.ValgtypeID == valgtypeid).ToList();
            List<VaraListeSTV> varaer = db.PreferansevalgVaraer.Where(k => k.ValgtypeID == valgtypeid).ToList();
            if (kandidater == null || varaer == null)
            {
                return "Feil";
            }
            for (int i = 0; i < kandidater.Count(); i++)
            {
                db.KandidaterSTV.Remove(kandidater[i]);
            }
            db.SaveChanges();
            for(int i = 0; i < varaer.Count(); i++)
            {
                db.PreferansevalgVaraer.Remove(varaer[i]);
            }
            db.SaveChanges();
            return "OK";
        }
        public List<KandidatSTV> hentKandidatlisteMedID(int valgtypeid)
        {
            var db = new BrukerContext();
            var kandidatliste = db.KandidaterSTV.Where(k => k.ValgtypeID == valgtypeid).ToList();
            List<KandidatSTV> returListe = new List<KandidatSTV>();
            int antallKandidat = kandidatliste.Count();
            for (int i = 0; i < antallKandidat; i++)
            {
                KandidatSTV nyKandidat = new KandidatSTV()
                {
                    kandidatListeID = kandidatliste[i].KandidatID,
                    navn = kandidatliste[i].Navn,
                    stemmetall = kandidatliste[i].Stemmetall,
                    stemmetallsatt = kandidatliste[i].StemmetallSatt,
                    valgtypeid = kandidatliste[i].ValgtypeID
                };
                returListe.Add(nyKandidat);
            }
            return returListe;
        }
        public bool slettPreferanseValg(int id)
        {
            var db = new BrukerContext();
            try
            {
                Valgtyper_db valgtyper = db.Valgtyper.FirstOrDefault(b => b.ValgtypeID == id);
                db.Valgtyper.Remove(valgtyper);
                // slett også 
                Preferansevalg_db preferansevalg = db.PreferanseValg.FirstOrDefault(p => p.ValgtypeID == id);
                db.PreferanseValg.Remove(preferansevalg);

                List<KandidatListeSTV> kandidatListe = db.KandidaterSTV.Where(v => v.ValgtypeID == id).ToList();
                for (int i = 0; i < kandidatListe.Count(); i++)
                {
                    if (kandidatListe[i].ValgtypeID == id)
                    {
                        db.KandidaterSTV.Remove(kandidatListe[i]);
                    }
                }
                db.SaveChanges();
                List<VaraListeSTV> varaListe = db.PreferansevalgVaraer.Where(v => v.ValgtypeID == id).ToList();
                for (int i = 0; i < varaListe.Count(); i++)
                {
                    if (varaListe[i].ValgtypeID == id)
                    {
                        db.PreferansevalgVaraer.Remove(varaListe[i]);
                    }
                }
                db.SaveChanges();
                return true;
            }
            catch (Exception feil)
            {
                return false;
            }
        }
        public List<Preferansevalg> hentAlleUtfortePreferansevalg()
        {
            using (var db = new BrukerContext())
            {
                var liste = db.PreferanseValg.Where(p => p.Utført == true).ToList();
                List<Preferansevalg> returListe = new List<Preferansevalg>();

                for (int i = 0; i < liste.Count(); i++)
                {
                    Preferansevalg etValg = new Preferansevalg()
                    {
                        id = liste[i].Id,
                        beskrivelse = liste[i].Beskrivelse,
                        antallrepresentanter = liste[i].AntallRepresentanter,
                        utført = liste[i].Utført,
                        valgtypeid = liste[i].ValgtypeID
                    };
                    returListe.Add(etValg);
                }
                return returListe;
            }
        }
        public bool slettUtførtePreferanseValg(int id)
        {
            var db = new BrukerContext();
            try
            {
                Valgtyper_db valgtyper = db.Valgtyper.FirstOrDefault(b => b.ValgtypeID == id);
                db.Valgtyper.Remove(valgtyper);
                // slett også 
                Preferansevalg_db preferansevalg = db.PreferanseValg.FirstOrDefault(p => p.ValgtypeID == id);
                db.PreferanseValg.Remove(preferansevalg);

                List<KandidatListeSTV> kandidatListe = db.KandidaterSTV.Where(v => v.ValgtypeID == id).ToList();
                for (int i = 0; i < kandidatListe.Count(); i++)
                {
                    if (kandidatListe[i].ValgtypeID == id)
                    {
                        db.KandidaterSTV.Remove(kandidatListe[i]);
                    }
                }

                List<Stemmeseddel_db> stemmeListe = db.Stemmesedler.Where(v => v.ValgtypeID == id).ToList();
                for (int i = 0; i < stemmeListe.Count(); i++)
                {
                    if (stemmeListe[i].ValgtypeID == id)
                    {
                        db.Stemmesedler.Remove(stemmeListe[i]);
                    }
                }
                db.SaveChanges();
                return true;
            }
            catch (Exception feil)
            {
                return false;
            }
        }
        public List<Preferansevalg> hentAlleIkkeUtfortePreferansevalg()
        {
            using (var db = new BrukerContext())
            {
                var liste = db.PreferanseValg.Where(p => p.Utført == false).ToList();
                List<Preferansevalg> returListe = new List<Preferansevalg>();

                for (int i = 0; i < liste.Count(); i++)
                {
                    Preferansevalg etValg = new Preferansevalg()
                    {
                        id = liste[i].Id,
                        beskrivelse = liste[i].Beskrivelse,
                        antallrepresentanter = liste[i].AntallRepresentanter,
                        utført = liste[i].Utført,
                        valgtypeid = liste[i].ValgtypeID
                    };
                    returListe.Add(etValg);
                }
                return returListe;
            }
        }
        public void slettKandidatFraListe(int id)
        {
            var db = new BrukerContext();

            KandidatListeSTV sletteKandidat = db.KandidaterSTV.FirstOrDefault(k => k.KandidatID == id);
            VaraListeSTV sletteFraVara = db.PreferansevalgVaraer.FirstOrDefault(v => v.VaraKandidatID == id);
            db.KandidaterSTV.Remove(sletteKandidat);
            db.PreferansevalgVaraer.Remove(sletteFraVara);
            db.SaveChanges();
        }
        public string lagreKandidatSTV(string id)
        {
            var db = new BrukerContext();

            if (id == "")
            {
                return "Feil";
            }
            else
            {
                var kandidat = new KandidatListeSTV()
                {
                    Navn = id,
                    StemmetallSatt = false
                };
                var kandidatVaraliste = new VaraListeSTV()
                {
                    Navn = id,
                    StemmetallSatt = false
                };

                List<KandidatListeSTV> kandidaterIkkeSatt = db.KandidaterSTV.Where(k => k.StemmetallSatt == false).ToList();
                for (int i = 0; i < kandidaterIkkeSatt.Count(); i++)
                {
                    if (kandidat.Navn == kandidaterIkkeSatt[i].Navn)
                    {
                        return "Samme navn";
                    }
                }

                db.KandidaterSTV.Add(kandidat);
                db.PreferansevalgVaraer.Add(kandidatVaraliste);
                db.SaveChanges();
                return "OK";
            }
        }
        public bool startPreferansevalg(Valgtyper valg)
        {
            var db = new BrukerContext();
            var start = new Valgtyper_db()
            {
                Valgtype = "Preferansevalg",
                Start = true
            };
            db.Valgtyper.Add(start);
            db.SaveChanges();
            return true;
        }
        public string startPreferansevalg(string beskrivelse, int antallRepresentanter, int antallVaraRepresentanter)
        {
            var db = new BrukerContext();
            if (beskrivelse == "")
            {
                return "feil";
            }
            if (antallRepresentanter < 2 || antallRepresentanter == 0)
            {
                return "feil";
            }

            var start = new Valgtyper_db()
            {
                Valgtype = "Preferansevalg",
                Start = true
            };
            db.Valgtyper.Add(start);
            db.SaveChanges();
            var valg = new Preferansevalg_db()
            {
                Beskrivelse = beskrivelse,
                AntallRepresentanter = antallRepresentanter,
                AntallVaraRepresentanter = antallVaraRepresentanter,
                Utført = false,
                ValgtypeID = start.ValgtypeID
            };

            db.PreferanseValg.Add(valg);
            db.SaveChanges();
            //Setter deltakende kandidater sin ValgId lik Valget som kjører
            var kandidater = db.KandidaterSTV.Where(k => k.StemmetallSatt == false && k.Stemmetall == 0).ToList();
            var varaKandidater = db.PreferansevalgVaraer.Where(v => v.StemmetallSatt == false && v.Stemmetall == 0).ToList();
            for (int i = 0; i < kandidater.Count(); i++)
            {
                kandidater[i].ValgtypeID = start.ValgtypeID;
                kandidater[i].StemmetallSatt = true;
                varaKandidater[i].ValgtypeID = start.ValgtypeID;
                varaKandidater[i].StemmetallSatt = true;
            }
            db.SaveChanges();
            return "OK";
        }
        public Preferansevalg PreferansevalgHarStartet()
        {
            var db = new BrukerContext();

            Valgtyper_db valgtype_db = db.Valgtyper.FirstOrDefault(b => b.Start == true && b.Valgtype == "Preferansevalg");
            Preferansevalg_db pValg = db.PreferanseValg.FirstOrDefault(p => p.ValgtypeID == valgtype_db.ValgtypeID);
            if (valgtype_db == null)
            {
                return null;
            }
            else
            {
                var preferansevalg = new Preferansevalg()
                {
                    id = pValg.Id,
                    beskrivelse = pValg.Beskrivelse,
                    antallrepresentanter = pValg.AntallRepresentanter,
                    utført = pValg.Utført,
                    valgtypeid = pValg.ValgtypeID
                };
                return preferansevalg;
            }
        }
        public bool PreferansevalgKjorer()
        {
            var db = new BrukerContext();

            Valgtyper_db valgtype_db = db.Valgtyper.FirstOrDefault(b => b.Start == true && b.Valgtype == "Preferansevalg");

            if (valgtype_db == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public List<KandidatSTV> listeKandidaterIkkeSatt()
        {
            var db = new BrukerContext();
            var liste = db.KandidaterSTV.Where(k => k.StemmetallSatt == false).ToList();
            List<KandidatSTV> kandidatListe = new List<KandidatSTV>();
            for (int i = 0; i < liste.Count(); i++)
            {
                KandidatSTV nykandidat = new KandidatSTV()
                {
                    kandidatListeID = liste[i].KandidatID,
                    navn = liste[i].Navn,
                    stemmetall = liste[i].Stemmetall,
                    stemmetallsatt = liste[i].StemmetallSatt,
                    valgtypeid = liste[i].ValgtypeID
                };

                kandidatListe.Add(nykandidat);
            }

            return kandidatListe;
        }


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

                int teller2 = slettListe.Count();
                if (teller2 > 0)
                {


                    for (int i = 0; i < teller2; i++)
                    {
                        var slett = db.PersonvalgKandidatResultat.FirstOrDefault();
                        db.PersonvalgKandidatResultat.Remove(slett);
                        db.SaveChanges();
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
        //Endra på denne
        public int PersonvalgKjørerID()
        {
            var db = new BrukerContext();
            var personvalg = db.Valgtyper.FirstOrDefault(b => b.Valgtype == "Personvalg" && b.Start == true);
            ;
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
        public bool SjekkPassordEng(Kandidat innkandidat, string brukernavn)
        {
            using (var db = new BrukerContext())
            {

                byte[] passwordhash = lagHash(innkandidat.passordEng);

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
                    stringarray.Add("<b>KandidatID " + listeAvKandidater.FirstOrDefault().id + " " + listeAvKandidater.FirstOrDefault().fornavn + " " + listeAvKandidater.FirstOrDefault().etternavn + "</b>") ;
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
        public List<PersonvalgResultat> hentPersonvalgResultatEng()
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
                        for (int k = 0; k < stemmer.Length; k++)
                        {
                            if (vinnerstemmer == stemmer[k])
                            {
                                x++;
                            }
                        }
                        if (x >= 2)
                        {
                            vinnerKandidat = "";
                            vinnerKandidat = "Draw";
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
                            personvalgresultat.Vinner = "No votes";
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
                        Vinner = "No Votes",
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
            int tellersjekk = listeAvKandidater.Count(b => b.valgtypeid == 0);
            if(tellersjekk == 0)
            {
                return false;
            }
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
        public bool leggtilPersonvalgKandidatEng(Kandidat innKandidat)
        {
            var db = new BrukerContext();
            var listeAvKandidater = db.Personvalger.Select(k => new Kandidat()
            {
                id = k.ID,
                fornavn = k.Fornavn,
                etternavn = k.Etternavn,
                lagret = k.Lagret,
                valgtypeid = k.ValgtypeID
            }).ToList();
            int teller = listeAvKandidater.Count(b => b.fornavn == innKandidat.fornavn_RequiredEng && b.etternavn == innKandidat.etternavn_RequiredEng && b.valgtypeid == 0);
            if (teller > 0)
            {

                return false;


            }
            else
            {
                var nyKandidat = new Personvalg_db()
                {
                    Fornavn = innKandidat.fornavn_RequiredEng,
                    Etternavn = innKandidat.etternavn_RequiredEng,
                    Lagret = (bool)false,

                    ValgtypeID = 0
                };
                db.Personvalger.Add(nyKandidat);
                db.SaveChanges();
                return true;
            }






        }
        public bool leggtilPersonvalgKandidat(Kandidat innKandidat)
        {
            var db = new BrukerContext();
            var listeAvKandidater = db.Personvalger.Select(k => new Kandidat()
            {
                id = k.ID,
                fornavn = k.Fornavn,
                etternavn = k.Etternavn,
                lagret = k.Lagret,
                valgtypeid = k.ValgtypeID
            }).ToList();
            int teller = listeAvKandidater.Count(b => b.fornavn == innKandidat.fornavn_Required && b.etternavn == innKandidat.etternavn_Required && b.valgtypeid == 0);
            if(teller > 0)
            {

                return false;


            }
            else
            {
                var nyKandidat = new Personvalg_db()
                {
                    Fornavn = innKandidat.fornavn_Required,
                    Etternavn = innKandidat.etternavn_Required,
                    Lagret = (bool)false,

                    ValgtypeID = 0
                };
                db.Personvalger.Add(nyKandidat);
                db.SaveChanges();
                return true;
            }



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
                return melding;
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
                return melding;
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
            Valgtyper valg = valgPågår(); 
            
        
            if (valg.valgtype == "Votering")
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
            
        
            else if (valg.valgtype == "Preferansevalg")
            {
                BrukereStemt_db brukerStemt = db.BrukereStemt.FirstOrDefault(b => b.Brukernavn == brukernavn && b.ValgtypeID == valg.valgtypeid && b.Valgtype == "Preferansevalg");
                if (brukerStemt != null)
                {
                    return "JA";
                }
                else
                {
                    return "NEI";
                }
            }
            else if(valg.valgtype == "Personvalg" )
            {
                int valgID = PersonvalgKjørerID();
                BrukereStemt_db brukerStemt = db.BrukereStemt.FirstOrDefault(b => b.Brukernavn == brukernavn && b.ValgtypeID == valgID && b.Valgtype == "Personvalg");
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