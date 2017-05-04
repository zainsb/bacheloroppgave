using Studentparlamentet_28.BLL;
using Studentparlamentet_28.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Data.SqlClient;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types
using System.IO;
using System.Web.Script.Serialization;

namespace Studentparlamentet_28.Controllers
{
    public class BrukerController : Controller
    {
        //Hent Resultat
        public ActionResult LastNedResultatStemmesedler()
        {
            // Lokal løsning med memoryStream
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    List<String> votering = new List<String>();
                    List<String> personvalg = new List<String>();
                    List<String> preferansevalg = new List<String>();
                    List<String> voteringseddel = new List<String>();
                    List<String> personvalgseddel = new List<String>();
                    List<String> preferansevalgseddel = new List<String>();

                    using (StreamReader reader = new StreamReader(Server.MapPath("/resultat/resultatIDer.txt")))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (line != "")
                            {
                                int ID = Convert.ToInt32(line);
                                if (db.valgtype(ID) == "Votering")
                                {
                                    votering.Add(line);
                                }
                                else if (db.valgtype(ID) == "Personvalg")
                                {
                                    personvalg.Add(line);
                                }
                                // Preferansevalg
                                else if (db.valgtype(ID) == "Preferansevalg")
                                {
                                    preferansevalg.Add(line);
                                }
                            }

                        }
                        

                    }
                    using (StreamReader reader = new StreamReader(Server.MapPath("/resultat/stemmeseddelIDer.txt")))
                    {
                        string line2;
                        while ((line2 = reader.ReadLine()) != null)
                        {
                            if (line2 != "")
                            {
                                int ID = Convert.ToInt32(line2);

                                if (db.valgtype(ID) == "Votering")
                                {
                                    voteringseddel.Add(line2);
                                }
                                else if (db.valgtype(ID) == "Personvalg")
                                {
                                    personvalgseddel.Add(line2);
                                }
                                // Preferansevalg
                                else if (db.valgtype(ID) == "Preferansevalg")
                                {
                                    preferansevalgseddel.Add(line2);
                                }
                            }

                        }




                    }

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (var doc = new iTextSharp.text.Document(PageSize.A4, 50, 50, 50, 50))
                        {
                            PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                            PdfPTable table = new PdfPTable(12);
                            BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
                            Font tablefont = new Font(bfTimes, 24, Font.BOLD);
                            Font tablefont2 = new Font(bfTimes, 14);
                            Font tablefont4 = new Font(bfTimes, 14, Font.BOLD);
                            Font tablefont3 = new Font(bfTimes, 18, Font.BOLD);

                            // antallvalgte valgtyper resultater
                            int tellerVotering = votering.Count();
                            int tellerPersonvalg = personvalg.Count();
                            int tellerPreferansevalg = preferansevalg.Count();
                            // antallvalgte valgtyper seddler
                            int tellerVoteringseddler = voteringseddel.Count();
                            int tellerPersonvalgseddler = personvalgseddel.Count();
                            int tellerPreferansevalgseddler = preferansevalgseddel.Count();

                            if (tellerVotering > 0 || tellerPersonvalg > 0 || tellerPreferansevalg > 0 )
                            {
                                PdfPCell cell = new PdfPCell(new Phrase(" \n Resultater \n ", tablefont));
                                cell.Colspan = 12;
                                cell.HorizontalAlignment = 1;
                                table.AddCell(cell);

                            }
                            else
                            {
                                PdfPCell cell = new PdfPCell(new Phrase(" \n Ingen valgte resultater \n ", tablefont));
                                cell.Colspan = 12;
                                cell.HorizontalAlignment = 1;
                                table.AddCell(cell);

                            }


                            // legg til Voteringer
                            if (tellerVotering > 0)
                            {
                                for (int i = 0; i < tellerVotering; i++)
                                {
                                    var valg = votering[i];
                                    int valgID = Convert.ToInt32(valg);
                                    // Overskrift med valgNr
                                    PdfPCell cell2 = new PdfPCell(new Phrase(" \n Votering med valgNr \n " + valgID + "\n \n", tablefont3));
                                    cell2.Colspan = 12;
                                    cell2.HorizontalAlignment = 1;
                                    table.AddCell(cell2);

                                    // For Stemmer
                                    var a = new PdfPCell(new Paragraph("For stemmer", tablefont4));
                                    a.FixedHeight = 50f;
                                    a.Colspan = 2;
                                    table.AddCell(a);
                                    // Antall for stemmer

                                    var antallFor = db.antallFor(valgID);
                                    var b = new PdfPCell(new Paragraph(antallFor.ToString(), tablefont2));
                                    b.FixedHeight = 50f;
                                    table.AddCell(b);
                                    // Mot stemmer
                                    var c = new PdfPCell(new Paragraph("Mot stemmer", tablefont4));
                                    c.FixedHeight = 50f;
                                    c.Colspan = 2;
                                    table.AddCell(c);
                                    // Antall mot stemmer
                                    var antallMot = db.antallMot(valgID);
                                    var d = new PdfPCell(new Paragraph(antallMot.ToString(), tablefont2));
                                    d.FixedHeight = 50f;
                                    table.AddCell(d);
                                    // Antall blank
                                    var e = new PdfPCell(new Paragraph("Blanke stemmer", tablefont4));
                                    e.FixedHeight = 50f;
                                    e.Colspan = 2;
                                    table.AddCell(e);
                                    // Antall blanke stemmer
                                    var antallBlank = db.antallBlank(valgID);
                                    var f = new PdfPCell(new Paragraph(antallBlank.ToString(), tablefont2));
                                    f.FixedHeight = 50f;
                                    table.AddCell(f);
                                    // Antall avlagte stemmer
                                    var h = new PdfPCell(new Paragraph("Antall avlagte stemmer", tablefont4));
                                    h.FixedHeight = 50f;
                                    h.Colspan = 2;
                                    table.AddCell(h);
                                    var antallAvlagte = db.antallstemteVotering(valgID);
                                    var j = new PdfPCell(new Paragraph(antallAvlagte.ToString(), tablefont2));
                                    j.FixedHeight = 50f;
                                    table.AddCell(j);



                                }
                            }



                            // legg til Personvalgene
                            if (tellerPersonvalg > 0)
                            {
                                for (int i = 0; i < tellerPersonvalg; i++)
                                {

                                    var valg = personvalg[i];
                                    int valgID = Convert.ToInt32(valg);
                                    // Overskrift med valgNr
                                    PdfPCell cell2 = new PdfPCell(new Phrase(" \n Personvalg med valgNr \n " + valgID + "\n \n", tablefont3));
                                    cell2.Colspan = 12;
                                    cell2.HorizontalAlignment = 1;
                                    table.AddCell(cell2);

                                    PersonvalgResultat valginfo = db.hentinfoPersonvalg(valgID);

                                    string vinner = "Vinner";
                                    var a1 = new PdfPCell(new Paragraph(vinner, tablefont4));
                                    a1.FixedHeight = 50f;
                                    a1.Colspan = 6;
                                    table.AddCell(a1);


                                    string vinnerNavn = valginfo.vinner;
                                    var b1 = new PdfPCell(new Paragraph(vinnerNavn, tablefont2));
                                    b1.FixedHeight = 50f;
                                    b1.Colspan = 6;
                                    table.AddCell(b1);

                                    string antallstemmer = "Antall stemmer vinner";
                                    var c1 = new PdfPCell(new Paragraph(antallstemmer, tablefont4));
                                    c1.FixedHeight = 50f;
                                    c1.Colspan = 6;
                                    table.AddCell(c1);

                                    int nr = valginfo.stemmer;
                                    string antallstemmerNr = Convert.ToString(nr);
                                    var d1 = new PdfPCell(new Paragraph(antallstemmerNr, tablefont2));
                                    d1.FixedHeight = 50f;
                                    d1.Colspan = 6;
                                    table.AddCell(d1);


                                    string totalantall = "Antall avlagte stemmer";
                                    var f1 = new PdfPCell(new Paragraph(totalantall, tablefont4));
                                    f1.FixedHeight = 50f;
                                    f1.Colspan = 6;
                                    table.AddCell(f1);

                                    int nr2 = valginfo.totalantallStemmer;
                                    string totalantallStemmer = Convert.ToString(nr2);
                                    var e1 = new PdfPCell(new Paragraph(totalantallStemmer, tablefont2));
                                    e1.FixedHeight = 50f;
                                    e1.Colspan = 6;
                                    table.AddCell(e1);


                                    string kandidater = "Antall kandidater";
                                    var h1 = new PdfPCell(new Paragraph(kandidater, tablefont4));
                                    h1.FixedHeight = 50f;
                                    h1.Colspan = 6;
                                    table.AddCell(h1);

                                    int nr3 = valginfo.antallkandidater;
                                    string antallkandidater = Convert.ToString(nr3);
                                    var g1 = new PdfPCell(new Paragraph(antallkandidater, tablefont2));
                                    g1.FixedHeight = 50f;
                                    g1.Colspan = 6;
                                    table.AddCell(g1);

                                }
                            }


                            // legg til Preferansevalg
                            if (tellerPreferansevalg > 0)
                            {
                                for (int i = 0; i < tellerPreferansevalg; i++)
                                {
                                    var valg = preferansevalg[i];
                                    int valgID = Convert.ToInt32(valg);
                                    // Overskrift med valgNr
                                    PdfPCell cell7 = new PdfPCell(new Phrase(" \n Preferansevalg med valgNr \n " + valgID + "\n \n", tablefont3));
                                    cell7.Colspan = 12;
                                    cell7.HorizontalAlignment = 1;
                                    table.AddCell(cell7);

                                    List<PreferansevalgValgte> valgteKandidater = db.hentValgteKandidater(valgID);
                                    int antallTeller = valgteKandidater.Count();

                                    int antall = db.AntallstemtPreferansevalg(valgID);
                                        string avlagtestemmer = "Antall avlagte stemmer";
                                        var a2 = new PdfPCell(new Paragraph(avlagtestemmer, tablefont4));
                                        a2.FixedHeight = 50f;
                                        a2.Colspan = 8;
                                        table.AddCell(a2);
                                    if(antall > 0)
                                    { 
                                        string antall_string = Convert.ToString(antall);
                                        var b2 = new PdfPCell(new Paragraph(antall_string, tablefont2));
                                        b2.FixedHeight = 50f;
                                        b2.Colspan = 4;
                                        table.AddCell(b2);
                                    }
                                    else
                                    {
                                        string antall_string = Convert.ToString(0);
                                        var b2 = new PdfPCell(new Paragraph(antall_string, tablefont2));
                                        b2.FixedHeight = 50f;
                                        b2.Colspan = 4;
                                        table.AddCell(b2);
                                    }
                                    string beskrivelse = "Antall kandidater";
                                    var a3 = new PdfPCell(new Paragraph(beskrivelse, tablefont4));
                                    a3.FixedHeight = 50f;
                                    a3.Colspan = 8;
                                    table.AddCell(a3);

                                    int antallkandidater = db.PreferansevalgAntallDeltatt(valgID);
                                    string antalldb = Convert.ToString(antallkandidater);
                                    var a4 = new PdfPCell(new Paragraph(antalldb, tablefont2));
                                    a4.FixedHeight = 50f;
                                    a4.Colspan = 4;
                                    table.AddCell(a4);


                                    if (antallTeller > 0)
                                    {
                                        string kandidater = "Valget kandidater";
                                        var x = new PdfPCell(new Paragraph(kandidater, tablefont4));
                                        x.FixedHeight = 50f;
                                        x.Colspan = 12;
                                        table.AddCell(x);
                                        string kandidat = "";
                                        for(int k = 0; k < antallTeller; k++)
                                        {
                                            kandidat = valgteKandidater[k].navn;
                                            var z = new PdfPCell(new Paragraph(kandidat, tablefont2));
                                            z.FixedHeight = 50f;
                                            z.Colspan = 12;
                                            table.AddCell(z);
                                        }

                                    }



                                }
                            }

                                if (tellerVoteringseddler > 0 || tellerPersonvalgseddler > 0 || tellerPreferansevalgseddler > 0)
                                {
                                    PdfPCell cell4 = new PdfPCell(new Phrase(" \n Stemmesedler \n ", tablefont));
                                    cell4.Colspan = 12;
                                    cell4.HorizontalAlignment = 1;
                                    table.AddCell(cell4);

                                }
                                else
                                {
                                    PdfPCell cell = new PdfPCell(new Phrase(" \n Ingen valgte Stemmesedler \n ", tablefont));
                                    cell.Colspan = 12;
                                    cell.HorizontalAlignment = 1;
                                    table.AddCell(cell);

                                }

                                // legg til Voteringseddler
                                if (tellerVoteringseddler > 0)
                                {


                                    for (int i = 0; i < tellerVoteringseddler; i++)
                                    {


                                        var valg = voteringseddel[i];
                                        int valgID = Convert.ToInt32(valg);
                                        // Overskrift med valgNr
                                        PdfPCell cell3 = new PdfPCell(new Phrase(" \n Votering stemmesedler med valgNr \n " + valgID + "\n \n", tablefont3));
                                        cell3.Colspan = 12;
                                        cell3.HorizontalAlignment = 1;
                                        table.AddCell(cell3);

                                        List<Votering> allestemmesedler = db.hentVoteringer(valgID);
                                        int teller6 = allestemmesedler.Count();
                                        int teller7 = 1;
                                        for (int j = 0; j < teller6; j++)
                                        {
                                            string stemme = "";
                                            string vinner = "Stemmeseddel " + Convert.ToString(teller7);
                                            var b1 = new PdfPCell(new Paragraph(vinner, tablefont4));
                                            b1.FixedHeight = 50f;
                                            b1.Colspan = 4;
                                            table.AddCell(b1);
                                            teller7++;

                                            if (allestemmesedler[j].svarfor == "for")
                                            {
                                                stemme = "for";
                                                var a1 = new PdfPCell(new Paragraph(stemme, tablefont2));
                                                a1.FixedHeight = 50f;
                                                a1.Colspan = 8;
                                                table.AddCell(a1);
                                            }
                                            else if (allestemmesedler[j].svarmot == "mot")
                                            {
                                                stemme = "mot";
                                                var a1 = new PdfPCell(new Paragraph(stemme, tablefont2));
                                                a1.FixedHeight = 50f;
                                                a1.Colspan = 8;
                                                table.AddCell(a1);
                                            }
                                            else if (allestemmesedler[j].svarblank == "blank")
                                            {
                                                stemme = "blank";
                                                var a1 = new PdfPCell(new Paragraph(stemme, tablefont2));
                                                a1.FixedHeight = 50f;
                                                a1.Colspan = 8;
                                                table.AddCell(a1);
                                            }


                                        }



                                    }
                                }
                                // legg til personvalgsedler

                                if (tellerPersonvalgseddler > 0)
                                {

                                    for (int i = 0; i < tellerPersonvalgseddler; i++)
                                    {
                                        var valg = personvalgseddel[i];
                                        int valgID = Convert.ToInt32(valg);
                                        // Overskrift med valgNr
                                        PdfPCell cell5 = new PdfPCell(new Phrase(" \n Personvalg stemmesedler med valgNr \n " + valgID + "\n \n", tablefont3));
                                        cell5.Colspan = 12;
                                        cell5.HorizontalAlignment = 1;
                                        table.AddCell(cell5);
                                        List<PersonvalgStemmer> personvalgsedler = db.hentPersonvalg(valgID);
                                        int teller8 = personvalgsedler.Count();
                                        int teller7 = 1;
                                        for (int k = 0; k < teller8; k++)
                                        {
                                            string stemme = "";

                                            string vinner = "Stemmeseddel " + Convert.ToString(teller7);
                                            var b1 = new PdfPCell(new Paragraph(vinner, tablefont4));
                                            b1.FixedHeight = 50f;
                                            b1.Colspan = 4;
                                            table.AddCell(b1);
                                            teller7++;

                                            stemme = personvalgsedler[k].fornavn + " " + personvalgsedler[k].etternavn;
                                            var a1 = new PdfPCell(new Paragraph(stemme, tablefont2));
                                            a1.FixedHeight = 50f;
                                            a1.Colspan = 8;
                                            table.AddCell(a1);
                                        }



                                    }
                                }
                                // legg til preferansevalgsedler                                                                                 
                                if (tellerPreferansevalgseddler > 0)
                                {
                                    for (int i = 0; i < tellerPreferansevalgseddler; i++)
                                    {
                                        var valg = preferansevalgseddel[i];
                                        int valgID = Convert.ToInt32(valg);
                                        // Overskrift med valgNr
                                        PdfPCell cell6 = new PdfPCell(new Phrase(" \n Preferansevalg stemmesedler med valgNr \n " + valgID + "\n \n", tablefont3));
                                        cell6.Colspan = 12;
                                        cell6.HorizontalAlignment = 1;
                                        table.AddCell(cell6);
                                        List<Stemmeseddel> preferansevalgsedler = db.preferansevalgsedler(valgID);
                                        int sedlerTeller = preferansevalgsedler.Count();
                                        if(sedlerTeller > 0)
                                        {
                                        string valgtKandidat = "";
                                        string valgtKandidatNavn = "";

                                        for (int j = 0; j < sedlerTeller; j++)
                                        {
                                            int stemmeID = preferansevalgsedler[j].stemmeseddelID;
                                            PdfPCell cell7 = new PdfPCell(new Phrase(" \n Stemmesedler med stemmeID \n " + stemmeID + "\n \n", tablefont3));
                                            cell7.Colspan = 12;
                                            cell7.HorizontalAlignment = 1;
                                            table.AddCell(cell7);

                                            valgtKandidat = "Kandidatvalg 1";
                                            var b1 = new PdfPCell(new Paragraph(valgtKandidat, tablefont4));
                                            b1.FixedHeight = 50f;
                                            b1.Colspan = 4;
                                            table.AddCell(b1);
                                        
                                            if(preferansevalgsedler[j].kandidatnrEn != null)
                                            {
                                                valgtKandidatNavn = preferansevalgsedler[j].kandidatnrEn;
                                                var a1 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a1.FixedHeight = 50f;
                                                a1.Colspan = 8;
                                                table.AddCell(a1);
                                            }
                                            else
                                            {
                                                valgtKandidatNavn = "";
                                                var a1 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a1.FixedHeight = 50f;
                                                a1.Colspan = 8;
                                                table.AddCell(a1);
                                            }
                             

                                            valgtKandidat = "Kandidatvalg 2";
                                            var b2 = new PdfPCell(new Paragraph(valgtKandidat, tablefont4));
                                            b2.FixedHeight = 50f;
                                            b2.Colspan = 4;
                                            table.AddCell(b2);

                                            if (preferansevalgsedler[j].kandidatnrTo != null)
                                            {
                                                valgtKandidatNavn = preferansevalgsedler[j].kandidatnrTo;
                                                var a2 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a2.FixedHeight = 50f;
                                                a2.Colspan = 8;
                                                table.AddCell(a2);
                                            }
                                            else
                                            {
                                                valgtKandidatNavn = "";
                                                var a2 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a2.FixedHeight = 50f;
                                                a2.Colspan = 8;
                                                table.AddCell(a2);
                                            }
                                            valgtKandidat = "Kandidatvalg 3";
                                            var b3 = new PdfPCell(new Paragraph(valgtKandidat, tablefont4));
                                            b3.FixedHeight = 50f;
                                            b3.Colspan = 4;
                                            table.AddCell(b3);

                                            if (preferansevalgsedler[j].kandidatnrTre != null)
                                            {
                                                valgtKandidatNavn = preferansevalgsedler[j].kandidatnrTre;
                                                var a3 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a3.FixedHeight = 50f;
                                                a3.Colspan = 8;
                                                table.AddCell(a3);
                                            }
                                            else
                                            {
                                                valgtKandidatNavn = "";
                                                var a3 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a3.FixedHeight = 50f;
                                                a3.Colspan = 8;
                                                table.AddCell(a3);
                                            }

                                            valgtKandidat = "Kandidatvalg 4";
                                            var b4 = new PdfPCell(new Paragraph(valgtKandidat, tablefont4));
                                            b4.FixedHeight = 50f;
                                            b4.Colspan = 4;
                                            table.AddCell(b4);

                                            if (preferansevalgsedler[j].kandidatnrFire != null)
                                            {
                                                valgtKandidatNavn = preferansevalgsedler[j].kandidatnrFire;
                                                var a4 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a4.FixedHeight = 50f;
                                                a4.Colspan = 8;
                                                table.AddCell(a4);
                                            }
                                            else
                                            {
                                                valgtKandidatNavn = "";
                                                var a4 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a4.FixedHeight = 50f;
                                                a4.Colspan = 8;
                                                table.AddCell(a4);
                                            }
                                            valgtKandidat = "Kandidatvalg 5";
                                            var b5 = new PdfPCell(new Paragraph(valgtKandidat, tablefont4));
                                            b5.FixedHeight = 50f;
                                            b5.Colspan = 4;
                                            table.AddCell(b5);

                                            if (preferansevalgsedler[j].kandidatnrFem != null)
                                            {
                                                valgtKandidatNavn = preferansevalgsedler[j].kandidatnrFem;
                                                var a5 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a5.FixedHeight = 50f;
                                                a5.Colspan = 8;
                                                table.AddCell(a5);
                                            }
                                                else
                                            {
                                                valgtKandidatNavn = "";
                                                var a5 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a5.FixedHeight = 50f;
                                                a5.Colspan = 8;
                                                table.AddCell(a5);
                                            }

                                            valgtKandidat = "Kandidatvalg 6";
                                            var b6 = new PdfPCell(new Paragraph(valgtKandidat, tablefont4));
                                            b6.FixedHeight = 50f;
                                            b6.Colspan = 4;
                                            table.AddCell(b6);

                                            if (preferansevalgsedler[j].kandidatnrSeks != null)
                                            {
                                                valgtKandidatNavn = preferansevalgsedler[j].kandidatnrSeks;
                                                var a6 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a6.FixedHeight = 50f;
                                                a6.Colspan = 8;
                                                table.AddCell(a6);
                                            }
                                            else
                                            {
                                                valgtKandidatNavn = "";
                                                var a6 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a6.FixedHeight = 50f;
                                                a6.Colspan = 8;
                                                table.AddCell(a6);
                                            }

                                            valgtKandidat = "Kandidatvalg 7";
                                            var b7 = new PdfPCell(new Paragraph(valgtKandidat, tablefont4));
                                            b7.FixedHeight = 50f;
                                            b7.Colspan = 4;
                                            table.AddCell(b7);


                                            if (preferansevalgsedler[j].kandidatnrSju != null)
                                            {
                                                valgtKandidatNavn = preferansevalgsedler[j].kandidatnrSju;
                                                var a7 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a7.FixedHeight = 50f;
                                                a7.Colspan = 8;
                                                table.AddCell(a7);
                                            }
                                            else
                                            {
                                                valgtKandidatNavn = "";
                                                var a7 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a7.FixedHeight = 50f;
                                                a7.Colspan = 8;
                                                table.AddCell(a7);
                                            }
                                            valgtKandidat = "Kandidatvalg 8";
                                            var b8 = new PdfPCell(new Paragraph(valgtKandidat, tablefont4));
                                            b8.FixedHeight = 50f;
                                            b8.Colspan = 4;
                                            table.AddCell(b8);

                                            if(preferansevalgsedler[j].kandidatnrÅtte != null)
                                            {
                                                valgtKandidatNavn = preferansevalgsedler[j].kandidatnrÅtte;
                                                var a8 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a8.FixedHeight = 50f;
                                                a8.Colspan = 8;
                                                table.AddCell(a8);
                                            }
                                            else
                                            {
                                                valgtKandidatNavn = "";
                                                var a8 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a8.FixedHeight = 50f;
                                                a8.Colspan = 8;
                                                table.AddCell(a8);
                                            }

                                            valgtKandidat = "Kandidatvalg 9";
                                            var b9 = new PdfPCell(new Paragraph(valgtKandidat, tablefont4));
                                            b9.FixedHeight = 50f;
                                            b9.Colspan = 4;
                                            table.AddCell(b9);

                                            if (preferansevalgsedler[j].kandidatnrNi != null)
                                            {
                                                valgtKandidatNavn = preferansevalgsedler[j].kandidatnrNi;
                                                var a9 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a9.FixedHeight = 50f;
                                                a9.Colspan = 8;
                                                table.AddCell(a9);
                                            }
                                            else
                                            {
                                                valgtKandidatNavn = "";
                                                var a9 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a9.FixedHeight = 50f;
                                                a9.Colspan = 8;
                                                table.AddCell(a9);
                                            }
                                            valgtKandidat = "Kandidatvalg 10";
                                            var b10 = new PdfPCell(new Paragraph(valgtKandidat, tablefont4));
                                            b10.FixedHeight = 50f;
                                            b10.Colspan = 4;
                                            table.AddCell(b10);

                                            if (preferansevalgsedler[j].kandidatnrTi != null)
                                            {
                                                valgtKandidatNavn = preferansevalgsedler[j].kandidatnrTi;
                                                var a10 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a10.FixedHeight = 50f;
                                                a10.Colspan = 8;
                                                table.AddCell(a10);
                                            }
                                            else
                                            {
                                                valgtKandidatNavn = "";
                                                var a10 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a10.FixedHeight = 50f;
                                                a10.Colspan = 8;
                                                table.AddCell(a10);
                                            }
                                            valgtKandidat = "Kandidatvalg 11";
                                            var b11 = new PdfPCell(new Paragraph(valgtKandidat, tablefont4));
                                            b11.FixedHeight = 50f;
                                            b11.Colspan = 4;
                                            table.AddCell(b11);

                                            if (preferansevalgsedler[j].kandidatnrElleve != null)
                                            {
                                                valgtKandidatNavn = preferansevalgsedler[j].kandidatnrElleve;
                                                var a11 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a11.FixedHeight = 50f;
                                                a11.Colspan = 8;
                                                table.AddCell(a11);
                                            }
                                            else
                                            {
                                                valgtKandidatNavn = "";
                                                var a11 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a11.FixedHeight = 50f;
                                                a11.Colspan = 8;
                                                table.AddCell(a11);
                                            }
                                            valgtKandidat = "Kandidatvalg 12";
                                            var b12 = new PdfPCell(new Paragraph(valgtKandidat, tablefont4));
                                            b12.FixedHeight = 50f;
                                            b12.Colspan = 4;
                                            table.AddCell(b12);

                                            if (preferansevalgsedler[j].kandidatnrTolv != null)
                                            {
                                                valgtKandidatNavn = preferansevalgsedler[j].kandidatnrTolv;
                                                var a12 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a12.FixedHeight = 50f;
                                                a12.Colspan = 8;
                                                table.AddCell(a12);
                                            }
                                            else
                                            {
                                                valgtKandidatNavn = "";
                                                var a12 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a12.FixedHeight = 50f;
                                                a12.Colspan = 8;
                                                table.AddCell(a12);
                                            }


                                            }
                                        }
                                    }
                                }
                                doc.Open();
                                doc.Add(table);
                                doc.Close();
                                byte[] filedata = ms.ToArray();
                                using (StreamWriter writer3 = new StreamWriter(Server.MapPath("/resultat/resultatIDer.txt")))
                                {
                                    writer3.WriteLine("");
                                }
                                using (StreamWriter writer2 = new StreamWriter(Server.MapPath("/resultat/stemmeseddelIDer.txt")))
                                {
                                    writer2.WriteLine("");
                                }

                                return File(filedata, "application/pdf", "Rapport.pdf");
                            }


                        }

                    }
                return RedirectToAction("Index");
            }


                return RedirectToAction("Index");
            }

        public ActionResult LastNedResultatStemmesedlerEng()
        {
            // Lokal løsning med memoryStream
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    List<String> votering = new List<String>();
                    List<String> personvalg = new List<String>();
                    List<String> preferansevalg = new List<String>();
                    List<String> voteringseddel = new List<String>();
                    List<String> personvalgseddel = new List<String>();
                    List<String> preferansevalgseddel = new List<String>();

                    using (StreamReader reader = new StreamReader(Server.MapPath("/resultat/resultatIDer.txt")))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (line != "")
                            {
                                int ID = Convert.ToInt32(line);
                                if (db.valgtype(ID) == "Votering")
                                {
                                    votering.Add(line);
                                }
                                else if (db.valgtype(ID) == "Personvalg")
                                {
                                    personvalg.Add(line);
                                }
                                // Preferansevalg
                                else if (db.valgtype(ID) == "Preferansevalg")
                                {
                                    preferansevalg.Add(line);
                                }
                            }

                        }




                    }
                    using (StreamReader reader = new StreamReader(Server.MapPath("/resultat/stemmeseddelIDer.txt")))
                    {
                        string line2;
                        while ((line2 = reader.ReadLine()) != null)
                        {
                            if (line2 != "")
                            {
                                int ID = Convert.ToInt32(line2);

                                if (db.valgtype(ID) == "Votering")
                                {
                                    voteringseddel.Add(line2);
                                }
                                else if (db.valgtype(ID) == "Personvalg")
                                {
                                    personvalgseddel.Add(line2);
                                }
                                // Preferansevalg
                                else if (db.valgtype(ID) == "Preferansevalg")
                                {
                                    preferansevalgseddel.Add(line2);
                                }
                            }

                        }




                    }

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (var doc = new iTextSharp.text.Document(PageSize.A4, 50, 50, 50, 50))
                        {
                            PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                            PdfPTable table = new PdfPTable(12);
                            BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
                            Font tablefont = new Font(bfTimes, 24, Font.BOLD);
                            Font tablefont2 = new Font(bfTimes, 14);
                            Font tablefont4 = new Font(bfTimes, 14, Font.BOLD);
                            Font tablefont3 = new Font(bfTimes, 18, Font.BOLD);

                            // antallvalgte valgtyper resultater
                            int tellerVotering = votering.Count();
                            int tellerPersonvalg = personvalg.Count();
                            int tellerPreferansevalg = preferansevalg.Count();
                            // antallvalgte valgtyper seddler
                            int tellerVoteringseddler = voteringseddel.Count();
                            int tellerPersonvalgseddler = personvalgseddel.Count();
                            int tellerPreferansevalgseddler = preferansevalgseddel.Count();

                            if (tellerVotering > 0 || tellerPersonvalg > 0 || tellerPreferansevalg > 0)
                            {
                                PdfPCell cell = new PdfPCell(new Phrase(" \n Result \n ", tablefont));
                                cell.Colspan = 12;
                                cell.HorizontalAlignment = 1;
                                table.AddCell(cell);

                            }
                            else
                            {
                                PdfPCell cell = new PdfPCell(new Phrase(" \n No chosen election or vote \n ", tablefont));
                                cell.Colspan = 12;
                                cell.HorizontalAlignment = 1;
                                table.AddCell(cell);

                            }


                            // legg til Voteringer
                            if (tellerVotering > 0)
                            {
                                for (int i = 0; i < tellerVotering; i++)
                                {
                                    var valg = votering[i];
                                    int valgID = Convert.ToInt32(valg);
                                    // Overskrift med valgNr
                                    PdfPCell cell2 = new PdfPCell(new Phrase(" \n Vote with VoteNr \n " + valgID + "\n \n", tablefont3));
                                    cell2.Colspan = 12;
                                    cell2.HorizontalAlignment = 1;
                                    table.AddCell(cell2);

                                    // For Stemmer
                                    var a = new PdfPCell(new Paragraph("For votes", tablefont4));
                                    a.FixedHeight = 50f;
                                    a.Colspan = 2;
                                    table.AddCell(a);
                                    // Antall for stemmer

                                    var antallFor = db.antallFor(valgID);
                                    var b = new PdfPCell(new Paragraph(antallFor.ToString(), tablefont2));
                                    b.FixedHeight = 50f;
                                    table.AddCell(b);
                                    // Mot stemmer
                                    var c = new PdfPCell(new Paragraph("Against votes", tablefont4));
                                    c.FixedHeight = 50f;
                                    c.Colspan = 2;
                                    table.AddCell(c);
                                    // Antall mot stemmer
                                    var antallMot = db.antallMot(valgID);
                                    var d = new PdfPCell(new Paragraph(antallMot.ToString(), tablefont2));
                                    d.FixedHeight = 50f;
                                    table.AddCell(d);
                                    // Antall blank
                                    var e = new PdfPCell(new Paragraph("Blank votes", tablefont4));
                                    e.FixedHeight = 50f;
                                    e.Colspan = 2;
                                    table.AddCell(e);
                                    // Antall blanke stemmer
                                    var antallBlank = db.antallBlank(valgID);
                                    var f = new PdfPCell(new Paragraph(antallBlank.ToString(), tablefont2));
                                    f.FixedHeight = 50f;
                                    table.AddCell(f);
                                    // Antall avlagte stemmer
                                    var h = new PdfPCell(new Paragraph("Number of votes", tablefont4));
                                    h.FixedHeight = 50f;
                                    h.Colspan = 2;
                                    table.AddCell(h);
                                    var antallAvlagte = db.antallstemteVotering(valgID);
                                    var j = new PdfPCell(new Paragraph(antallAvlagte.ToString(), tablefont2));
                                    j.FixedHeight = 50f;
                                    table.AddCell(j);



                                }
                            }



                            // legg til Personvalgene
                            if (tellerPersonvalg > 0)
                            {
                                for (int i = 0; i < tellerPersonvalg; i++)
                                {

                                    var valg = personvalg[i];
                                    int valgID = Convert.ToInt32(valg);
                                    // Overskrift med valgNr
                                    PdfPCell cell2 = new PdfPCell(new Phrase(" \n Person Election with ElectionNr \n " + valgID + "\n \n", tablefont3));
                                    cell2.Colspan = 12;
                                    cell2.HorizontalAlignment = 1;
                                    table.AddCell(cell2);

                                    PersonvalgResultat valginfo = db.hentinfoPersonvalg(valgID);

                                    string vinner = "Winner";
                                    var a1 = new PdfPCell(new Paragraph(vinner, tablefont4));
                                    a1.FixedHeight = 50f;
                                    a1.Colspan = 6;
                                    table.AddCell(a1);


                                    string vinnerNavn = valginfo.vinner;
                                    var b1 = new PdfPCell(new Paragraph(vinnerNavn, tablefont2));
                                    b1.FixedHeight = 50f;
                                    b1.Colspan = 6;
                                    table.AddCell(b1);

                                    string antallstemmer = "Number of votes to winner";
                                    var c1 = new PdfPCell(new Paragraph(antallstemmer, tablefont4));
                                    c1.FixedHeight = 50f;
                                    c1.Colspan = 6;
                                    table.AddCell(c1);

                                    int nr = valginfo.stemmer;
                                    string antallstemmerNr = Convert.ToString(nr);
                                    var d1 = new PdfPCell(new Paragraph(antallstemmerNr, tablefont2));
                                    d1.FixedHeight = 50f;
                                    d1.Colspan = 6;
                                    table.AddCell(d1);


                                    string totalantall = "Total number of votes";
                                    var f1 = new PdfPCell(new Paragraph(totalantall, tablefont4));
                                    f1.FixedHeight = 50f;
                                    f1.Colspan = 6;
                                    table.AddCell(f1);

                                    int nr2 = valginfo.totalantallStemmer;
                                    string totalantallStemmer = Convert.ToString(nr2);
                                    var e1 = new PdfPCell(new Paragraph(totalantallStemmer, tablefont2));
                                    e1.FixedHeight = 50f;
                                    e1.Colspan = 6;
                                    table.AddCell(e1);


                                    string kandidater = "Number of Candidates";
                                    var h1 = new PdfPCell(new Paragraph(kandidater, tablefont4));
                                    h1.FixedHeight = 50f;
                                    h1.Colspan = 6;
                                    table.AddCell(h1);

                                    int nr3 = valginfo.antallkandidater;
                                    string antallkandidater = Convert.ToString(nr3);
                                    var g1 = new PdfPCell(new Paragraph(antallkandidater, tablefont2));
                                    g1.FixedHeight = 50f;
                                    g1.Colspan = 6;
                                    table.AddCell(g1);

                                }
                            }


                            // legg til Preferansevalg
                            if (tellerPreferansevalg > 0)
                            {
                                for (int i = 0; i < tellerPreferansevalg; i++)
                                {
                                    var valg = preferansevalg[i];
                                    int valgID = Convert.ToInt32(valg);
                                    // Overskrift med valgNr
                                    PdfPCell cell7 = new PdfPCell(new Phrase(" \n S.TV with electionNr \n " + valgID + "\n \n", tablefont3));
                                    cell7.Colspan = 12;
                                    cell7.HorizontalAlignment = 1;
                                    table.AddCell(cell7);

                                    List<PreferansevalgValgte> valgteKandidater = db.hentValgteKandidater(valgID);
                                    int antallTeller = valgteKandidater.Count();

                                    int antall = db.AntallstemtPreferansevalg(valgID);
                                    string avlagtestemmer = "Total number of Votes";
                                    var a2 = new PdfPCell(new Paragraph(avlagtestemmer, tablefont4));
                                    a2.FixedHeight = 50f;
                                    a2.Colspan = 8;
                                    table.AddCell(a2);
                                    if (antall > 0)
                                    {
                                        string antall_string = Convert.ToString(antall);
                                        var b2 = new PdfPCell(new Paragraph(antall_string, tablefont2));
                                        b2.FixedHeight = 50f;
                                        b2.Colspan = 4;
                                        table.AddCell(b2);
                                    }
                                    else
                                    {
                                        string antall_string = Convert.ToString(0);
                                        var b2 = new PdfPCell(new Paragraph(antall_string, tablefont2));
                                        b2.FixedHeight = 50f;
                                        b2.Colspan = 4;
                                        table.AddCell(b2);
                                    }
                                    string beskrivelse = "Number of Candidates";
                                    var a3 = new PdfPCell(new Paragraph(beskrivelse, tablefont4));
                                    a3.FixedHeight = 50f;
                                    a3.Colspan = 8;
                                    table.AddCell(a3);

                                    int antallkandidater = db.PreferansevalgAntallDeltatt(valgID);
                                    string antalldb = Convert.ToString(antallkandidater);
                                    var a4 = new PdfPCell(new Paragraph(antalldb, tablefont2));
                                    a4.FixedHeight = 50f;
                                    a4.Colspan = 4;
                                    table.AddCell(a4);


                                    if (antallTeller > 0)
                                    {
                                        string kandidater = "Chosen Candidates";
                                        var x = new PdfPCell(new Paragraph(kandidater, tablefont4));
                                        x.FixedHeight = 50f;
                                        x.Colspan = 12;
                                        table.AddCell(x);
                                        string kandidat = "";
                                        for (int k = 0; k < antallTeller; k++)
                                        {
                                            kandidat = valgteKandidater[k].navn;
                                            var z = new PdfPCell(new Paragraph(kandidat, tablefont2));
                                            z.FixedHeight = 50f;
                                            z.Colspan = 12;
                                            table.AddCell(z);
                                        }

                                    }



                                }
                            }

                            if (tellerVoteringseddler > 0 || tellerPersonvalgseddler > 0 || tellerPreferansevalgseddler > 0)
                            {
                                PdfPCell cell4 = new PdfPCell(new Phrase(" \n Voteslips \n ", tablefont));
                                cell4.Colspan = 12;
                                cell4.HorizontalAlignment = 1;
                                table.AddCell(cell4);

                            }
                            else
                            {
                                PdfPCell cell = new PdfPCell(new Phrase(" \n No chosen Voteslip \n ", tablefont));
                                cell.Colspan = 12;
                                cell.HorizontalAlignment = 1;
                                table.AddCell(cell);

                            }

                            // legg til Voteringseddler
                            if (tellerVoteringseddler > 0)
                            {


                                for (int i = 0; i < tellerVoteringseddler; i++)
                                {


                                    var valg = voteringseddel[i];
                                    int valgID = Convert.ToInt32(valg);
                                    // Overskrift med valgNr
                                    PdfPCell cell3 = new PdfPCell(new Phrase(" \n Vote voteslip with VoteNr \n " + valgID + "\n \n", tablefont3));
                                    cell3.Colspan = 12;
                                    cell3.HorizontalAlignment = 1;
                                    table.AddCell(cell3);

                                    List<Votering> allestemmesedler = db.hentVoteringer(valgID);
                                    int teller6 = allestemmesedler.Count();
                                    int teller7 = 1;
                                    for (int j = 0; j < teller6; j++)
                                    {
                                        string stemme = "";
                                        string vinner = "Voteslip " + Convert.ToString(teller7);
                                        var b1 = new PdfPCell(new Paragraph(vinner, tablefont4));
                                        b1.FixedHeight = 50f;
                                        b1.Colspan = 4;
                                        table.AddCell(b1);
                                        teller7++;

                                        if (allestemmesedler[j].svarfor == "for")
                                        {
                                            stemme = "for";
                                            var a1 = new PdfPCell(new Paragraph(stemme, tablefont2));
                                            a1.FixedHeight = 50f;
                                            a1.Colspan = 8;
                                            table.AddCell(a1);
                                        }
                                        else if (allestemmesedler[j].svarmot == "mot")
                                        {
                                            stemme = "against";
                                            var a1 = new PdfPCell(new Paragraph(stemme, tablefont2));
                                            a1.FixedHeight = 50f;
                                            a1.Colspan = 8;
                                            table.AddCell(a1);
                                        }
                                        else if (allestemmesedler[j].svarblank == "blank")
                                        {
                                            stemme = "blank";
                                            var a1 = new PdfPCell(new Paragraph(stemme, tablefont2));
                                            a1.FixedHeight = 50f;
                                            a1.Colspan = 8;
                                            table.AddCell(a1);
                                        }


                                    }



                                }
                            }
                            // legg til personvalgsedler

                            if (tellerPersonvalgseddler > 0)
                            {

                                for (int i = 0; i < tellerPersonvalgseddler; i++)
                                {
                                    var valg = personvalgseddel[i];
                                    int valgID = Convert.ToInt32(valg);
                                    // Overskrift med valgNr
                                    PdfPCell cell5 = new PdfPCell(new Phrase(" \n Person Election voteslip with electionNr \n " + valgID + "\n \n", tablefont3));
                                    cell5.Colspan = 12;
                                    cell5.HorizontalAlignment = 1;
                                    table.AddCell(cell5);
                                    List<PersonvalgStemmer> personvalgsedler = db.hentPersonvalg(valgID);
                                    int teller8 = personvalgsedler.Count();
                                    int teller7 = 1;
                                    for (int k = 0; k < teller8; k++)
                                    {
                                        string stemme = "";

                                        string vinner = "Voteslip " + Convert.ToString(teller7);
                                        var b1 = new PdfPCell(new Paragraph(vinner, tablefont4));
                                        b1.FixedHeight = 50f;
                                        b1.Colspan = 4;
                                        table.AddCell(b1);
                                        teller7++;

                                        stemme = personvalgsedler[k].fornavn + " " + personvalgsedler[k].etternavn;
                                        var a1 = new PdfPCell(new Paragraph(stemme, tablefont2));
                                        a1.FixedHeight = 50f;
                                        a1.Colspan = 8;
                                        table.AddCell(a1);
                                    }



                                }
                            }
                            // legg til preferansevalgsedler                                                                                 
                            if (tellerPreferansevalgseddler > 0)
                            {
                                for (int i = 0; i < tellerPreferansevalgseddler; i++)
                                {
                                    var valg = preferansevalgseddel[i];
                                    int valgID = Convert.ToInt32(valg);
                                    // Overskrift med valgNr
                                    PdfPCell cell6 = new PdfPCell(new Phrase(" \n S.T.V voteslip with ElectionNr \n " + valgID + "\n \n", tablefont3));
                                    cell6.Colspan = 12;
                                    cell6.HorizontalAlignment = 1;
                                    table.AddCell(cell6);
                                    List<Stemmeseddel> preferansevalgsedler = db.preferansevalgsedler(valgID);
                                    int sedlerTeller = preferansevalgsedler.Count();
                                    if (sedlerTeller > 0)
                                    {
                                        string valgtKandidat = "";
                                        string valgtKandidatNavn = "";

                                        for (int j = 0; j < sedlerTeller; j++)
                                        {
                                            int stemmeID = preferansevalgsedler[j].stemmeseddelID;
                                            PdfPCell cell7 = new PdfPCell(new Phrase(" \n Voteslip with ID \n " + stemmeID + "\n \n", tablefont3));
                                            cell7.Colspan = 12;
                                            cell7.HorizontalAlignment = 1;
                                            table.AddCell(cell7);

                                            valgtKandidat = "Candidate 1";
                                            var b1 = new PdfPCell(new Paragraph(valgtKandidat, tablefont4));
                                            b1.FixedHeight = 50f;
                                            b1.Colspan = 4;
                                            table.AddCell(b1);

                                            if (preferansevalgsedler[j].kandidatnrEn != null)
                                            {
                                                valgtKandidatNavn = preferansevalgsedler[j].kandidatnrEn;
                                                var a1 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a1.FixedHeight = 50f;
                                                a1.Colspan = 8;
                                                table.AddCell(a1);
                                            }
                                            else
                                            {
                                                valgtKandidatNavn = "";
                                                var a1 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a1.FixedHeight = 50f;
                                                a1.Colspan = 8;
                                                table.AddCell(a1);
                                            }


                                            valgtKandidat = "Candidate 2";
                                            var b2 = new PdfPCell(new Paragraph(valgtKandidat, tablefont4));
                                            b2.FixedHeight = 50f;
                                            b2.Colspan = 4;
                                            table.AddCell(b2);

                                            if (preferansevalgsedler[j].kandidatnrTo != null)
                                            {
                                                valgtKandidatNavn = preferansevalgsedler[j].kandidatnrTo;
                                                var a2 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a2.FixedHeight = 50f;
                                                a2.Colspan = 8;
                                                table.AddCell(a2);
                                            }
                                            else
                                            {
                                                valgtKandidatNavn = "";
                                                var a2 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a2.FixedHeight = 50f;
                                                a2.Colspan = 8;
                                                table.AddCell(a2);
                                            }
                                            valgtKandidat = "Candidate 3";
                                            var b3 = new PdfPCell(new Paragraph(valgtKandidat, tablefont4));
                                            b3.FixedHeight = 50f;
                                            b3.Colspan = 4;
                                            table.AddCell(b3);

                                            if (preferansevalgsedler[j].kandidatnrTre != null)
                                            {
                                                valgtKandidatNavn = preferansevalgsedler[j].kandidatnrTre;
                                                var a3 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a3.FixedHeight = 50f;
                                                a3.Colspan = 8;
                                                table.AddCell(a3);
                                            }
                                            else
                                            {
                                                valgtKandidatNavn = "";
                                                var a3 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a3.FixedHeight = 50f;
                                                a3.Colspan = 8;
                                                table.AddCell(a3);
                                            }

                                            valgtKandidat = "Candidate 4";
                                            var b4 = new PdfPCell(new Paragraph(valgtKandidat, tablefont4));
                                            b4.FixedHeight = 50f;
                                            b4.Colspan = 4;
                                            table.AddCell(b4);

                                            if (preferansevalgsedler[j].kandidatnrFire != null)
                                            {
                                                valgtKandidatNavn = preferansevalgsedler[j].kandidatnrFire;
                                                var a4 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a4.FixedHeight = 50f;
                                                a4.Colspan = 8;
                                                table.AddCell(a4);
                                            }
                                            else
                                            {
                                                valgtKandidatNavn = "";
                                                var a4 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a4.FixedHeight = 50f;
                                                a4.Colspan = 8;
                                                table.AddCell(a4);
                                            }
                                            valgtKandidat = "Candidate 5";
                                            var b5 = new PdfPCell(new Paragraph(valgtKandidat, tablefont4));
                                            b5.FixedHeight = 50f;
                                            b5.Colspan = 4;
                                            table.AddCell(b5);

                                            if (preferansevalgsedler[j].kandidatnrFem != null)
                                            {
                                                valgtKandidatNavn = preferansevalgsedler[j].kandidatnrFem;
                                                var a5 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a5.FixedHeight = 50f;
                                                a5.Colspan = 8;
                                                table.AddCell(a5);
                                            }
                                            else
                                            {
                                                valgtKandidatNavn = "";
                                                var a5 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a5.FixedHeight = 50f;
                                                a5.Colspan = 8;
                                                table.AddCell(a5);
                                            }

                                            valgtKandidat = "Candidate 6";
                                            var b6 = new PdfPCell(new Paragraph(valgtKandidat, tablefont4));
                                            b6.FixedHeight = 50f;
                                            b6.Colspan = 4;
                                            table.AddCell(b6);

                                            if (preferansevalgsedler[j].kandidatnrSeks != null)
                                            {
                                                valgtKandidatNavn = preferansevalgsedler[j].kandidatnrSeks;
                                                var a6 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a6.FixedHeight = 50f;
                                                a6.Colspan = 8;
                                                table.AddCell(a6);
                                            }
                                            else
                                            {
                                                valgtKandidatNavn = "";
                                                var a6 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a6.FixedHeight = 50f;
                                                a6.Colspan = 8;
                                                table.AddCell(a6);
                                            }

                                            valgtKandidat = "Candidate 7";
                                            var b7 = new PdfPCell(new Paragraph(valgtKandidat, tablefont4));
                                            b7.FixedHeight = 50f;
                                            b7.Colspan = 4;
                                            table.AddCell(b7);


                                            if (preferansevalgsedler[j].kandidatnrSju != null)
                                            {
                                                valgtKandidatNavn = preferansevalgsedler[j].kandidatnrSju;
                                                var a7 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a7.FixedHeight = 50f;
                                                a7.Colspan = 8;
                                                table.AddCell(a7);
                                            }
                                            else
                                            {
                                                valgtKandidatNavn = "";
                                                var a7 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a7.FixedHeight = 50f;
                                                a7.Colspan = 8;
                                                table.AddCell(a7);
                                            }
                                            valgtKandidat = "Candidate 8";
                                            var b8 = new PdfPCell(new Paragraph(valgtKandidat, tablefont4));
                                            b8.FixedHeight = 50f;
                                            b8.Colspan = 4;
                                            table.AddCell(b8);

                                            if (preferansevalgsedler[j].kandidatnrÅtte != null)
                                            {
                                                valgtKandidatNavn = preferansevalgsedler[j].kandidatnrÅtte;
                                                var a8 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a8.FixedHeight = 50f;
                                                a8.Colspan = 8;
                                                table.AddCell(a8);
                                            }
                                            else
                                            {
                                                valgtKandidatNavn = "";
                                                var a8 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a8.FixedHeight = 50f;
                                                a8.Colspan = 8;
                                                table.AddCell(a8);
                                            }

                                            valgtKandidat = "Candidate 9";
                                            var b9 = new PdfPCell(new Paragraph(valgtKandidat, tablefont4));
                                            b9.FixedHeight = 50f;
                                            b9.Colspan = 4;
                                            table.AddCell(b9);

                                            if (preferansevalgsedler[j].kandidatnrNi != null)
                                            {
                                                valgtKandidatNavn = preferansevalgsedler[j].kandidatnrNi;
                                                var a9 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a9.FixedHeight = 50f;
                                                a9.Colspan = 8;
                                                table.AddCell(a9);
                                            }
                                            else
                                            {
                                                valgtKandidatNavn = "";
                                                var a9 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a9.FixedHeight = 50f;
                                                a9.Colspan = 8;
                                                table.AddCell(a9);
                                            }
                                            valgtKandidat = "Candidate 10";
                                            var b10 = new PdfPCell(new Paragraph(valgtKandidat, tablefont4));
                                            b10.FixedHeight = 50f;
                                            b10.Colspan = 4;
                                            table.AddCell(b10);

                                            if (preferansevalgsedler[j].kandidatnrTi != null)
                                            {
                                                valgtKandidatNavn = preferansevalgsedler[j].kandidatnrTi;
                                                var a10 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a10.FixedHeight = 50f;
                                                a10.Colspan = 8;
                                                table.AddCell(a10);
                                            }
                                            else
                                            {
                                                valgtKandidatNavn = "";
                                                var a10 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a10.FixedHeight = 50f;
                                                a10.Colspan = 8;
                                                table.AddCell(a10);
                                            }
                                            valgtKandidat = "Candidate 11";
                                            var b11 = new PdfPCell(new Paragraph(valgtKandidat, tablefont4));
                                            b11.FixedHeight = 50f;
                                            b11.Colspan = 4;
                                            table.AddCell(b11);

                                            if (preferansevalgsedler[j].kandidatnrElleve != null)
                                            {
                                                valgtKandidatNavn = preferansevalgsedler[j].kandidatnrElleve;
                                                var a11 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a11.FixedHeight = 50f;
                                                a11.Colspan = 8;
                                                table.AddCell(a11);
                                            }
                                            else
                                            {
                                                valgtKandidatNavn = "";
                                                var a11 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a11.FixedHeight = 50f;
                                                a11.Colspan = 8;
                                                table.AddCell(a11);
                                            }
                                            valgtKandidat = "Candidate 12";
                                            var b12 = new PdfPCell(new Paragraph(valgtKandidat, tablefont4));
                                            b12.FixedHeight = 50f;
                                            b12.Colspan = 4;
                                            table.AddCell(b12);

                                            if (preferansevalgsedler[j].kandidatnrTolv != null)
                                            {
                                                valgtKandidatNavn = preferansevalgsedler[j].kandidatnrTolv;
                                                var a12 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a12.FixedHeight = 50f;
                                                a12.Colspan = 8;
                                                table.AddCell(a12);
                                            }
                                            else
                                            {
                                                valgtKandidatNavn = "";
                                                var a12 = new PdfPCell(new Paragraph(valgtKandidatNavn, tablefont2));
                                                a12.FixedHeight = 50f;
                                                a12.Colspan = 8;
                                                table.AddCell(a12);
                                            }


                                        }
                                    }
                                }
                            }
                            doc.Open();
                            doc.Add(table);
                            doc.Close();
                            byte[] filedata = ms.ToArray();
                            using (StreamWriter writer3 = new StreamWriter(Server.MapPath("/resultat/resultatIDer.txt")))
                            {
                                writer3.WriteLine("");
                            }
                            using (StreamWriter writer2 = new StreamWriter(Server.MapPath("/resultat/stemmeseddelIDer.txt")))
                            {
                                writer2.WriteLine("");
                            }

                            return File(filedata, "application/pdf", "Raport.pdf");
                        }


                    }

                }
                return RedirectToAction("IndexEng");
            }


            return RedirectToAction("IndexEng");
        }
        public void LastNedResultat(int[] valgider, int[] valgiderstemmeseddel)
        {
            if (valgider != null)
            {
                using (StreamWriter writer = new StreamWriter(Server.MapPath("/resultat/resultatIDer.txt")))
                {
                    int teller = valgider.Count();
                    if (teller > 0)
                    {
                        for (int i = 1; i < teller; i++)
                        {
                            if (valgider[i] != 0)
                            {
                                writer.WriteLine(valgider[i]);
                            }

                        }
                    }



                }
                if(valgiderstemmeseddel != null)
                {
                    using (StreamWriter writer = new StreamWriter(Server.MapPath("/resultat/stemmeseddelIDer.txt")))
                    {
                        int teller = valgiderstemmeseddel.Count();
                        if (teller > 0)
                        {
                            for (int i = 1; i < teller; i++)
                            {
                                if (valgiderstemmeseddel[i] != 0)
                                {
                                    writer.WriteLine(valgiderstemmeseddel[i]);
                                }

                            }
                        }



                    }
                }



            }
            else if (valgiderstemmeseddel != null)
            {
                using (StreamWriter writer = new StreamWriter(Server.MapPath("/resultat/stemmeseddelIDer.txt")))
                {
                    int teller = valgiderstemmeseddel.Count();
                    if (teller > 0)
                    {
                        for (int i = 1; i < teller; i++)
                        {
                            if (valgiderstemmeseddel[i] != 0)
                            {
                                writer.WriteLine(valgiderstemmeseddel[i]);
                            }

                        }
                    }



                }
            }




        }
        [Authorize(Roles = "true")]
        public ActionResult stemmeseddelPersonvalgEng(int id)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    List<PersonvalgStemmer> tabell = db.hentPersonvalg(id);
                    List<PersonvalgStemmer> tabell2 = tabell.OrderByDescending(k => k.etternavn).ToList();
                    return View("../user/stemmeseddelPersonvalgEng", tabell2);

                }

            }
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "true")]
        public ActionResult stemmeseddelPersonvalg(int id)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    List<PersonvalgStemmer> tabell = db.hentPersonvalg(id);
                    List<PersonvalgStemmer> tabell2 = tabell.OrderByDescending(k => k.etternavn).ToList();
                    return View(tabell2);

                }

            }
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "true")]
        public ActionResult stemmeseddelVoteringEng(int id)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    List<Votering> tabell = db.hentVoteringer(id);
                    return View("../user/stemmeseddelVoteringEng", tabell);
                }
            }
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "true")]
        public ActionResult stemmeseddelVotering(int id)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    List<Votering> tabell = db.hentVoteringer(id);
                    return View(tabell);
                }
            }
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "true")]
        public ActionResult StemmesedlerEng(String valgtype, int valgtypeid)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    if (valgtype == "Votering")
                    {
                        return RedirectToAction("stemmeseddelVoteringEng", new { id = valgtypeid });
                    }
                    else if (valgtype == "Personvalg")
                    {
                        return RedirectToAction("stemmeseddelPersonvalgEng", new { id = valgtypeid });
                    }
                    else if (valgtype == "Preferansevalg")
                    {
                        return RedirectToAction("StemmesedlerPreferansevalg", new { valgtypeid = valgtypeid });
                    }

                }

            }
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "true")]
        public ActionResult Stemmesedler(String valgtype, int valgtypeid)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    if (valgtype == "Votering")
                    {
                        return RedirectToAction("stemmeseddelVotering", new { id = valgtypeid });
                    }
                    else if (valgtype == "Personvalg")
                    {
                        return RedirectToAction("stemmeseddelPersonvalg", new { id = valgtypeid });
                    }
                    else if(valgtype == "Preferansevalg")
                    {
                        return RedirectToAction("StemmesedlerPreferansevalg", new { valgtypeid = valgtypeid });
                    }

                }

            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "true")]
        public ActionResult StemmesedlerPreferansevalg(int valgtypeid)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();

                    List<Stemmeseddel> stemmesedler = db.stemmesedlerMedID(valgtypeid);
                    return View(stemmesedler);
                }
            }

            return RedirectToAction("Index");
        }
        [Authorize(Roles = "true")]
        public ActionResult StemmesedlerPreferansevalgEng(int valgtypeid)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();

                    List<Stemmeseddel> stemmesedler = db.stemmesedlerMedID(valgtypeid);
                    return View("../user/StemmesedlerPreferansevalgEng", stemmesedler);
                }
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "true")]
        public ActionResult PersonvalgResultat_2Eng(int id)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    List<PersonvalgKandidatResultat> tabell = db.hentPersonvalgResultatFane(id);
                    var tabell2 = tabell.OrderByDescending(i => i.stemmer).ToList();

                    return View("../user/PersonvalgResultat_2Eng", tabell2);
                }
            }

            return RedirectToAction("IndexEng");


        }
        [Authorize(Roles = "true")]
        public ActionResult PersonvalgResultat_2(int id)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    List<PersonvalgKandidatResultat> tabell = db.hentPersonvalgResultatFane(id);
                    var tabell2 = tabell.OrderByDescending(i => i.stemmer).ToList();

                    return View(tabell2);
                }
            }

            return RedirectToAction("Index");


        }
        [Authorize(Roles = "true")]
        // Hent Resultat
        public ActionResult ResultatVoteringID_2Eng(int id)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    Valgtyper Valg = db.hentValg(id);
                    return View("../user/ResultatVoteringID_2Eng", Valg);
                }
            }

            return RedirectToAction("IndexEng");

        }
        [Authorize(Roles = "true")]
        // Hent Resultat
        public ActionResult ResultatVoteringID_2(int id)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    Valgtyper Valg = db.hentValg(id);
                    return View(Valg);
                }
            }

            return RedirectToAction("Index");

        }
        [Authorize(Roles = "true")]
        public ActionResult HentResultatKnapp(String valgtype, int valgtypeid)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    if (valgtype == "Votering")
                    {
                        return RedirectToAction("ResultatVoteringID_2", new { id = valgtypeid });

                    }
                    else if (valgtype == "Personvalg")
                    {
                        return RedirectToAction("PersonvalgResultat_2", new { id = valgtypeid });
                    }
                    else if (valgtype == "Preferansevalg")
                    {
                        return RedirectToAction("ResultatPreferansevalg2", new { valgtypeid = valgtypeid });
                    }

                }
                
                return RedirectToAction("HentResultat");
            }


            return RedirectToAction("Index");
        }
        [Authorize(Roles = "true")]
        public ActionResult HentResultatKnappEng(String valgtype, int valgtypeid)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    if (valgtype == "Votering")
                    {
                        return RedirectToAction("ResultatVoteringID_2Eng", new { id = valgtypeid });

                    }
                    else if (valgtype == "Personvalg")
                    {
                        return RedirectToAction("PersonvalgResultat_2Eng", new { id = valgtypeid });
                    }
                    else if(valgtype == "Preferansevalg")
                    {
                        return RedirectToAction("ResultatPreferansevalgEng2", new { valgtypeid = valgtypeid });
                    }

                }


                return RedirectToAction("HentResultatEng");
            }


            return RedirectToAction("IndexEng");
        }

        // BrukertestForbedring
        [Authorize(Roles = "true")]
        public void LeggTilMeldingVotering(string id)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {

                    var db = new BrukerBLL();
                    db.LeggTilMeldingVotering(id);
                }
            }

        
     
        }
    
        public string kjørerValg()
        {

                    var db = new BrukerBLL();
                    string melding = db.kjørerValg();
                    var jsonSerializer = new JavaScriptSerializer();
                    return jsonSerializer.Serialize(melding);
            
        }

        //PREFERANSEVALG
        
        public ActionResult ResultatPreferansevalgTilPDF(int valgtypeid)
        {
            var db = new BrukerBLL();
            Preferansevalg preferansevalg = db.hentPreferanseValg(valgtypeid);
            MemoryStream ms = db.ResultatPreferansevalgTilPDF(valgtypeid, preferansevalg.kvoteKlasseEn, preferansevalg.kvoteKlasseTo,
                                                              preferansevalg.klasseEnProsent, preferansevalg.klasseToProsent);

            byte[] filedata = ms.ToArray();
            return File(filedata, "application/pdf", "ResultatPreferansevalg.pdf");
        }

        [Authorize(Roles = "true")]
        public string hentValgteKandidater(int valgtypeid)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {

                    var db = new BrukerBLL();
                    List<PreferansevalgValgte> alleKandidater = db.hentValgteKandidater(valgtypeid);

                    var jsonSerializer = new JavaScriptSerializer();
                    string json = jsonSerializer.Serialize(alleKandidater);
                    return json;
                }
            }
            return "feil";

        }
        
        [Authorize(Roles = "true")]
        public string startLagretPreferansevalg(int valgtypeid, string beskrivelse)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    string melding = db.startLagretPreferansevalg(valgtypeid, beskrivelse);
                    var jsonSerializer = new JavaScriptSerializer();
                    return jsonSerializer.Serialize(melding);
                }
            }
            return "feil";

        }
        
        public ActionResult Preferansevalgsvar(string kandidatEn, string kandidatTo, string kandidatTre, string kandidatFire,
                                       string kandidatFem, string kandidatSeks, string kandidatSju, string kandidatÅtte,
                                       string kandidatNi, string kandidatTi, string kandidatElleve, string kandidatTolv)
        {
            if (Session["LoggetInn"] != null)
            {

                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    String iD = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
                    bool ok = db.preferansevalgSvar(kandidatEn, kandidatTo, kandidatTre, kandidatFire, kandidatFem,
                                                 kandidatSeks, kandidatSju, kandidatÅtte, kandidatNi, kandidatTi,
                                                 kandidatElleve, kandidatTolv);

                    Valgtyper valg = db.PreferansevalgPågår();
                    string brukernavn = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;

                    if (valg != null)
                    {
                        db.LagreBrukerStemt(valg, brukernavn);
                    }

                    if (ok == true)
                    {
                        return RedirectToAction("BrukerLoggetInn", new { id = iD });
                    }
                    else
                    {
                        return RedirectToAction("BrukerLoggetInn", new { id = iD });
                    }
                }
            }
            return RedirectToAction("index");
        }


        public string hentKandidaterMedID()
        {

                    var db = new BrukerBLL();
                    List<KandidatSTV> alleKandidater = db.hentKandidaterPreferansevalgBruker();

                    var jsonSerializer = new JavaScriptSerializer();
                    string json = jsonSerializer.Serialize(alleKandidater);
                    return json;


        }
        [Authorize(Roles = "false")]
        public ActionResult PreferansevalgBruker()
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    return View();
                }
            }
            return RedirectToAction("Index");
         
        }
        [Authorize(Roles = "false")]
        public ActionResult PreferansevalgBrukerEng()
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    return View("../User/PreferansevalgBrukerEng");
                }
            }
            return RedirectToAction("IndexEng");
            
        }
        [Authorize(Roles = "false")]
        public string stvMelding()
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();

                    string melding = db.stvBrukerStart();
                    var jsonSerializer = new JavaScriptSerializer();
                    if (melding == "Preferansevalg")
                    {
                        return jsonSerializer.Serialize(melding);
                    }
                    return jsonSerializer.Serialize(melding);
                }
            }
            return "feil";

        }
        [Authorize(Roles = "true")]
        public string stopPreferansevalg()
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    Valgtyper valg = db.stopPreferansevalg();
                    var jsonSerializer = new JavaScriptSerializer();
                    return jsonSerializer.Serialize(valg);
                    
                }
            }
            return "feil";

        }
        [Authorize(Roles = "true")]
        public string AntallstemtPreferansevalg(int valgtypeid)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    int melding = db.AntallstemtPreferansevalg(valgtypeid);
                    var jsonSerializer = new JavaScriptSerializer();
                    return jsonSerializer.Serialize(melding);
                }
            }
            return "feil";
     
        }
        [Authorize(Roles = "true")]
        public string hentInnAntallDeltakere()
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    int melding = db.hentAntallBrukere_int();
                    var jsonSerializer = new JavaScriptSerializer();
                    return jsonSerializer.Serialize(melding);
                }
            }
            return "feil";

        }
        [Authorize(Roles = "true")]
        public string antallKandidaterIPreferansevalg(int valgtypeid)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();

                    int antallKandidater = db.PreferansevalgAntallKandidater(valgtypeid);
                    var jsonSerializer = new JavaScriptSerializer();

                    return jsonSerializer.Serialize(antallKandidater);
                }
            }
            return "feil";

        }
        [Authorize(Roles = "true")]
        public string antallDeltokIPreferansevalg(int valgtypeid)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();

                    int antallDeltakere = db.PreferansevalgAntallDeltatt(valgtypeid);
                    var jsonSerializer = new JavaScriptSerializer();

                    return jsonSerializer.Serialize(antallDeltakere);
                }
            }
            return "feil";

        }
        [Authorize(Roles = "true")]
        public string hentPrefereransevalgDetaljer(int valgtypeid)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();

                    Preferansevalg valg = db.hentPreferanseValg(valgtypeid);
                    var jsonSerializer = new JavaScriptSerializer();

                    return jsonSerializer.Serialize(valg);
                }
            }
            return "feil";

        }
        [Authorize(Roles = "true")]
        public ActionResult ResultatPreferansevalg2(int valgtypeid)
        {
            if (Session["LoggetInn"] != null)
            {
                var db = new BrukerBLL();
                Preferansevalg valg = db.hentPreferanseValg(valgtypeid);
                return View(valg);
            }
            else
            {
                return RedirectToAction("index");
            }
        }

        [Authorize(Roles = "true")]
        public ActionResult ResultatPreferansevalg(int valgtypeid)
        {
            if (Session["LoggetInn"] != null)
            {
                var db = new BrukerBLL();
                Preferansevalg valg = db.hentPreferanseValg(valgtypeid);
                return View(valg);
            }
            else
            {
                return RedirectToAction("index");
            }
        }
        [Authorize(Roles = "true")]
        public ActionResult ResultatPreferansevalgEng2(int valgtypeid)
        {
            if (Session["LoggetInn"] != null)
            {
                var db = new BrukerBLL();
                Preferansevalg valg = db.hentPreferanseValg(valgtypeid);
                return View("../User/ResultatPreferansevalgEng2", valg);
            }
            else
            {
                return RedirectToAction("indexEng");
            }
        }
        [Authorize(Roles = "true")]
        public ActionResult ResultatPreferansevalgEng(int valgtypeid)
        {
            if (Session["LoggetInn"] != null)
            {
                var db = new BrukerBLL();
                Preferansevalg valg = db.hentPreferanseValg(valgtypeid);
                return View("../User/ResultatPreferansevalgEng", valg);
            }
            else
            {
                return RedirectToAction("index");
            }
        }
        [Authorize(Roles = "true")]
        public string lagreNyttPreferansevalg(string beskrivelse, int antallRepresentanter, string klasse1, string klasse2, int prosent1, int prosent2)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();

                    string melding = db.lagreNyttPreferansevalg(beskrivelse, antallRepresentanter, klasse1, klasse2, prosent1, prosent2);
                    var jsonSerializer = new JavaScriptSerializer();
                    return jsonSerializer.Serialize(melding);
                }
            }
            return "feil";
        }
        [Authorize(Roles = "true")]
        public string NullstillKandidatliste(int valgtypeid)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    string melding = db.NullstillKandidatliste(valgtypeid);
                    var jsonSerializer = new JavaScriptSerializer();
                    return jsonSerializer.Serialize(melding);
                }
            }
            return "feil";
        }
        [Authorize(Roles = "true")]
        public ActionResult ForhåndslagreNyttPreferansevalgEng()
        {
            if (Session["LoggetInn"] != null)
            {
                var db = new BrukerBLL();
                List<KandidatSTV> listeKandidater = db.listeKandidaterIkkeSatt();
                return View("../User/ForhåndslagreNyttPreferansevalgEng", listeKandidater);
            }
            else
            {
                return RedirectToAction("index");
            }
        }
        [Authorize(Roles = "true")]
        public ActionResult ForhåndslagreNyttPreferansevalg()
        {
            if (Session["LoggetInn"] != null)
            {
                var db = new BrukerBLL();
                List<KandidatSTV> listeKandidater = db.listeKandidaterIkkeSatt();
                return View(listeKandidater);
            }
            else
            {
                return RedirectToAction("index");
            }
        }
        [Authorize(Roles = "true")]
        public ActionResult UtførLagretPreferansevalg(int valgtypeid)
        {
            if (Session["LoggetInn"] != null)
            {
                var db = new BrukerBLL();
                List<KandidatSTV> listeKandidater = db.hentKandidatlisteMedID(valgtypeid);
                return View(listeKandidater);
            }
            else
            {
                return RedirectToAction("index");
            }
        }
        [Authorize(Roles = "true")]
        public ActionResult UtførLagretPreferansevalgEng(int valgtypeid)
        {
            if (Session["LoggetInn"] != null)
            {
                var db = new BrukerBLL();
                List<KandidatSTV> listeKandidater = db.hentKandidatlisteMedID(valgtypeid);
                return View("../User/UtførLagretPreferansevalgEng", listeKandidater);
            }
            else
            {
                return RedirectToAction("index");
            }
        }
        public void slettPreferanseValg(int id)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    db.slettPreferanseValg(id);
                }
            }
        }
        [Authorize(Roles = "true")]
        public ActionResult AlleUtfortePreferansevalg()
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    List<Preferansevalg> alleUtforte = db.hentAlleUtfortePreferansevalg();
                    return View(alleUtforte);
                }
            }

            return RedirectToAction("Index");
        }
        [Authorize(Roles = "true")]
        [HttpPost]
        public ActionResult AlleUtfortePreferansevalg(Bruker innAdmin, int id)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    string brukernavn = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;

                    if (db.admin_i_db_innlogget(innAdmin, brukernavn) == (bool)true)
                    {
                        db.slettUtførtePreferanseValg(id);
                        return RedirectToAction("AlleUtfortePreferansevalg", new { id = "" });
                    }
                    else
                    {
                        return RedirectToAction("AlleUtfortePreferansevalg", new { id = id }); // feil Passord
                    }
                }
            }

            return RedirectToAction("Index");
        }
        [Authorize(Roles = "true")]
        public ActionResult AlleUtfortePreferansevalgEng()
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    List<Preferansevalg> alleUtforte = db.hentAlleUtfortePreferansevalg();
                    return View("../User/AlleUtfortePreferansevalgEng", alleUtforte);
                }
            }

            return RedirectToAction("Index");
        }
        [Authorize(Roles = "true")]
        [HttpPost]
        public ActionResult AlleUtfortePreferansevalgEng(Kandidat innKandidat, int id)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    Bruker innAdmin = new Bruker();
                    innAdmin.passord = innKandidat.passordEng;
                    string brukernavn = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;

                    if (db.admin_i_db_innlogget(innAdmin, brukernavn) == (bool)true)
                    {
                        db.slettUtførtePreferanseValg(id);
                        return RedirectToAction("AlleUtfortePreferansevalgEng", new { id = "" });
                    }
                    else
                    {
                        return RedirectToAction("AlleUtfortePreferansevalgEng", new { id = id }); // feil Passord
                    }
                }
            }

            return RedirectToAction("Index");
        }
        [Authorize(Roles = "true")]
        public ActionResult ForhåndslagredePreferansevalg()
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    List<Preferansevalg> alleIkkeUtforte = db.hentAlleIkkeUtfortePreferansevalg();
                    return View(alleIkkeUtforte);
                }
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        [Authorize(Roles = "true")]
        public ActionResult ForhåndslagredePreferansevalg(Bruker innAdmin, int id)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    string brukernavn = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;

                    if (db.admin_i_db_innlogget(innAdmin, brukernavn) == (bool)true)
                    {
                        db.slettForhåndslagredePreferanseValg(id);
                        return RedirectToAction("ForhåndslagredePreferansevalg", new { id = "" });
                    }
                    else
                    {
                        return RedirectToAction("ForhåndslagredePreferansevalg", new { id = id });
                    }
                }
            }

            return RedirectToAction("Index");
        }
        [Authorize(Roles = "true")]
        public ActionResult ForhåndslagredePreferansevalgEng()
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    List<Preferansevalg> alleIkkeUtforte = db.hentAlleIkkeUtfortePreferansevalg();
                    return View("../User/ForhåndslagredePreferansevalgEng", alleIkkeUtforte);
                }
            }

            return RedirectToAction("Index");
        }
        [Authorize(Roles = "true")]
        [HttpPost]
        public ActionResult ForhåndslagredePreferansevalgEng(Kandidat innKandidat, int id)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    string brukernavn = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
                    Bruker innAdmin = new Bruker();
                    innAdmin.passord = innKandidat.passordEng;
                    if (db.admin_i_db_innlogget(innAdmin, brukernavn) == (bool)true)
                    {
                        db.slettForhåndslagredePreferanseValg(id);
                        return RedirectToAction("ForhåndslagredePreferansevalgEng", new { id = "" });
                    }
                    else
                    {
                        return RedirectToAction("ForhåndslagredePreferansevalgEng", new { id = id });
                    }
                }
            }

            return RedirectToAction("Index");
        }
        [Authorize(Roles = "true")]
        public ActionResult slettKandidatFraListe(int id)
        {
            if (Session["LoggetInn"] != null)
            {
                var db = new BrukerBLL();
                db.slettKandidatFraListe(id);
                List<KandidatSTV> listeKandidater = db.listeKandidaterIkkeSatt();
                return RedirectToAction("PreferansevalgAdmin");
            }
            else
            {
                return RedirectToAction("index");
            }
        }
        [Authorize(Roles = "true")]
        public string lagreKandidatIListe(string id, string klasse)
        {
            var db = new BrukerBLL();

            string melding = db.lagreKandidatSTV(id, klasse);
            var jsonSerializer = new JavaScriptSerializer();
            return jsonSerializer.Serialize(melding);
        }
        [Authorize(Roles = "true")]
        public string startPreferansevalg(string beskrivelse, int antallRepresentanter, 
                                    string klasse1, string klasse2, int prosent1, int prosent2)
        {
            var db = new BrukerBLL();

            string melding = db.startPreferansevalg(beskrivelse, antallRepresentanter,
                                                klasse1, klasse2, prosent1, prosent2);
            var jsonSerializer = new JavaScriptSerializer();
            return jsonSerializer.Serialize(melding);
        }
        [Authorize(Roles = "true")]
        public ActionResult PreferansevalgStartet()
        {
            var db = new BrukerBLL();
            if (Session["LoggetInn"] != null)
            {
                Preferansevalg valg = db.PreferansevalgHarStartet();
                return View(valg);
            }
            else
            {
                return RedirectToAction("index");
            }
        }
        [Authorize(Roles = "true")]
        public ActionResult PreferanseStartetEng()
        {
            var db = new BrukerBLL();
            if (Session["LoggetInn"] != null)
            {
                Preferansevalg valg = db.PreferansevalgHarStartet();
                return View("../User/PreferanseStartetEng", valg);
            }
            else
            {
                return RedirectToAction("index");
            }
        }
        [Authorize(Roles = "true")]
        public string PreferansevalgKjorer()
        {
            var db = new BrukerBLL();
            bool ok = db.PreferansevalgKjorer();
            if (ok == true)
            {
                return "true";
            }
            else
            {
                return "false";
            }
        }
        [Authorize(Roles = "true")]
        public ActionResult PreferansevalgAdmin()
        {
            if (Session["LoggetInn"] != null)
            {
                var db = new BrukerBLL();
                List<KandidatSTV> listeKandidater = db.listeKandidaterIkkeSatt();
                return View(listeKandidater);
            }
            else
            {
                return RedirectToAction("index");
            }
        }
        [Authorize(Roles = "true")]
        public ActionResult PreferansevalgAdminEng()
        {
            if (Session["LoggetInn"] != null)
            {
                var db = new BrukerBLL();
                List<KandidatSTV> listeKandidater = db.listeKandidaterIkkeSatt();
                return View("../User/PreferansevalgAdminEng", listeKandidater);
            }
            else
            {
                return RedirectToAction("index");
            }
        }


        // PERSONVALG

        [Authorize(Roles = "true")]
        public ActionResult PersonvalgEng()
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {

                    var db = new BrukerBLL();
                    return View("../User/PersonvalgEng");
                }
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        [Authorize(Roles = "true")]
        public ActionResult PersonvalgEng(Kandidat innKandidat)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    bool ok = db.leggtilPersonvalgKandidatEng(innKandidat);
                    if (ok)
                    {
                        return RedirectToAction("PersonvalgEng", new { id = "" });
                    }
                    else
                    {
                        return RedirectToAction("PersonvalgEng", new { id = "kandidatfinnes" }); // feilmelding kandidat eksisterer
                    }

                }
            }

            return RedirectToAction("IndexEng");




        }
        [HttpPost]
        [Authorize(Roles = "true")]
        public ActionResult Personvalg(Kandidat innKandidat)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    bool ok = db.leggtilPersonvalgKandidat(innKandidat);
                    if (ok)
                    {
                        return RedirectToAction("Personvalg", new { id = "" });
                    }
                    else
                    {
                        return RedirectToAction("Personvalg", new { id = "kandidatfinnes" }); // feilmelding kandidat eksisterer
                    }
                }
            }

            return RedirectToAction("Index");




        }

        [Authorize(Roles = "true")]
        public ActionResult Personvalg()
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {

                    var db = new BrukerBLL();
                    List<Kandidat> tabell = db.hentKandidater();
                    return View();
                }
            }

            return RedirectToAction("Index");
        }
        [Authorize(Roles = "true")]
        public ActionResult lagretPersonvalgEng()
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    List<PersonvalgResultat> tabell = db.hentPersonvalgResultatEng();
                    return View("../user/lagretPersonvalgEng", tabell);
                }
            }

            return RedirectToAction("Index");

        }
        [Authorize(Roles = "true")]
        [HttpPost]
        public ActionResult lagretPersonvalgEng(Kandidat innKandidat, int valgtypeID2, int valgtypeID3)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    if (valgtypeID3 == -1)
                    {
                        // sjekk om Personvalget har stemmer if so redirect og allert
                        bool avlagtStemme = db.sjekkValgStemme(valgtypeID2);
                        if (avlagtStemme == false)
                        {
                            bool ok = db.leggtilPersonvalgKandidatPersonvalgEng(innKandidat, valgtypeID2);
                            return RedirectToAction("lagretPersonvalgEng", new { id4 = "e" + valgtypeID2 });
                        }
                        else
                        {
                            return RedirectToAction("lagretPersonvalgEng", new { id4 = "f" + valgtypeID2 });
                        }



                    }
                    if (valgtypeID3 > 0) // slett Personvalg og resultat
                    {

                        string brukernavn = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
                        bool ok = db.SjekkPassordEng(innKandidat, brukernavn);
                        if (ok != false)
                        {
                            var slett = SlettPersonvalg(valgtypeID3);
                            return RedirectToAction("lagretPersonvalgEng");
                        }
                        else // feil passord
                        {
                            return RedirectToAction("lagretPersonvalgEng", new { id2 = "feilPassord2", id3 = valgtypeID3 });
                        }
                    }

                    int ID2 = db.finnValgtypeID(innKandidat.id);

                    if (ID2 == 0) // Ikke eksisterende kandidatID
                    {
                        return RedirectToAction("lagretPersonvalgEng", new { id2 = "feilKandiatnr", id3 = valgtypeID2 });
                    }
                    else // eksisterende kandidatID
                    {
                        int ID = valgtypeID2;
                        string brukernavn = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
                        bool ok = db.SjekkPassordEng(innKandidat, brukernavn);


                        if (ok == true) // passord riktig
                        {
                            // sjekk om det er avlagt stemme i valget
                            // hvis send feilmelding
                            // 
                            bool avlagtstemme = db.sjekkValgStemme(ID);
                            if (avlagtstemme == true)
                            {
                                return RedirectToAction("lagretPersonvalgEng", new { id2 = "feilendre", id3 = ID });
                            }
                            else
                            {

                                if (innKandidat.etternavn != null && innKandidat.fornavn != null)
                                {
                                    bool ok2 = db.Kandidatendre(innKandidat);
                                    return RedirectToAction("lagretPersonvalgEng", new { id2 = ID });
                                }
                                else if (innKandidat.etternavn != null)
                                {
                                    bool ok2 = db.KandidatendreEtternavn(innKandidat);
                                    return RedirectToAction("lagretPersonvalgEng", new { id2 = ID });

                                }
                                else if (innKandidat.fornavn != null)
                                {
                                    bool ok2 = db.KandidatendreFornavn(innKandidat);
                                    return RedirectToAction("lagretPersonvalgEng", new { id2 = ID });

                                }
                                else
                                {
                                    bool ok2 = db.KandidatSlett(innKandidat);

                                    return RedirectToAction("lagretPersonvalgEng", new { id2 = ID });


                                }

                            }

                        }
                        else if (ok == false) // passordfeil
                        {
                            return RedirectToAction("lagretPersonvalgEng", new { id2 = "feilPassord", id3 = ID });
                        }
                    }
                    // kandidatID 0, finnes ikke

                    return RedirectToAction("lagretPersonvalgEng", new { id2 = "feilKandiatnr", id3 = valgtypeID2 });
                }
            }

            return RedirectToAction("IndexEng");
           


        }

        [Authorize(Roles = "true")]
        public ActionResult lagretPersonvalg()
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    List<PersonvalgResultat> tabell = db.hentPersonvalgResultat();
                    return View(tabell);
                }
            }

            return RedirectToAction("Index");
   
        }
        [Authorize(Roles = "true")]
        [HttpPost]
        public ActionResult lagretPersonvalg(Kandidat innKandidat, int valgtypeID2, int valgtypeID3)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();

                    if (valgtypeID3 == -1)
                    {
                        // sjekk om Personvalget har stemmer if so redirect og allert
                        bool avlagtStemme = db.sjekkValgStemme(valgtypeID2);
                        if (avlagtStemme == false)
                        {
                            bool ok = db.leggtilPersonvalgKandidatPersonvalg(innKandidat, valgtypeID2);
                            return RedirectToAction("lagretPersonvalg", new { id4 = "e" + valgtypeID2 });
                        }
                        else
                        {
                            return RedirectToAction("lagretPersonvalg", new { id4 = "f" + valgtypeID2 });
                        }



                    }


                    if (valgtypeID3 > 0) // slett Personvalg og resultat
                    {

                        string brukernavn = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
                        bool ok = db.SjekkPassord(innKandidat, brukernavn);
                        if (ok != false)
                        {
                            var slett = SlettPersonvalg(valgtypeID3);
                            return RedirectToAction("lagretPersonvalg");
                        }
                        else // feil passord
                        {
                            return RedirectToAction("lagretPersonvalg", new { id2 = "feilPassord2", id3 = valgtypeID3 });
                        }
                    }

                    int ID2 = db.finnValgtypeID(innKandidat.id);

                    if (ID2 == 0) // Ikke eksisterende kandidatID
                    {
                        return RedirectToAction("lagretPersonvalg", new { id2 = "feilKandiatnr", id3 = valgtypeID2 });
                    }
                    else // eksisterende kandidatID
                    {
                        int ID = valgtypeID2;
                        string brukernavn = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
                        bool ok = db.SjekkPassord(innKandidat, brukernavn);


                        if (ok == true) // passord riktig
                        {
                            // sjekk om det er avlagt stemme i valget
                            // hvis send feilmelding
                            // 
                            bool avlagtstemme = db.sjekkValgStemme(ID);
                            if (avlagtstemme == true)
                            {
                                return RedirectToAction("lagretPersonvalg", new { id2 = "feilendre", id3 = ID });
                            }
                            else
                            {

                                if (innKandidat.etternavn != null && innKandidat.fornavn != null)
                                {
                                    bool ok2 = db.Kandidatendre(innKandidat);
                                    return RedirectToAction("lagretPersonvalg", new { id2 = ID });
                                }
                                else if (innKandidat.etternavn != null)
                                {
                                    bool ok2 = db.KandidatendreEtternavn(innKandidat);
                                    return RedirectToAction("lagretPersonvalg", new { id2 = ID });

                                }
                                else if (innKandidat.fornavn != null)
                                {
                                    bool ok2 = db.KandidatendreFornavn(innKandidat);
                                    return RedirectToAction("lagretPersonvalg", new { id2 = ID });

                                }
                                else
                                {
                                    bool ok2 = db.KandidatSlett(innKandidat);

                                    return RedirectToAction("lagretPersonvalg", new { id2 = ID });


                                }

                            }

                        }
                        else if (ok == false) // passordfeil
                        {
                            return RedirectToAction("lagretPersonvalg", new { id2 = "feilPassord", id3 = ID });
                        }
                    }
                    // kandidatID 0, finnes ikke

                    return RedirectToAction("lagretPersonvalg", new { id2 = "feilKandiatnr", id3 = valgtypeID2 });
                }
            }

            return RedirectToAction("Index");
            


        }
        
        [Authorize(Roles = "true")]
        public ActionResult PersonvalgResultatEng(int id)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    List<PersonvalgKandidatResultat> tabell = db.hentPersonvalgResultatFane(id);
                    var tabell2 = tabell.OrderByDescending(i => i.stemmer).ToList();
                    return View("../user/PersonvalgResultatEng", tabell2);
                }
            }

            return RedirectToAction("IndexEng");
    
        }
        [Authorize(Roles = "true")]
        public ActionResult PersonvalgResultat(int id)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    List<PersonvalgKandidatResultat> tabell = db.hentPersonvalgResultatFane(id);
                    var tabell2 = tabell.OrderByDescending(i => i.stemmer).ToList();

                    return View(tabell2);
                }
            }

            return RedirectToAction("Index");


        }
        [Authorize(Roles = "true")]
        public ActionResult SlettPersonvalg(int id)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    db.SlettPersonvalg(id);
                    return RedirectToAction("lagretPersonvalg");
                }
            }

            return RedirectToAction("Index");

        }

        [Authorize(Roles = "false")]
        public ActionResult PersonvaglregisterStemme()
        {
            if (Session ["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var RadioButton = Request.Form["radio"];
                    var db = new BrukerBLL();
                    if (RadioButton != null)
                    {
                        string svar = RadioButton.ToString();
                        int id = Convert.ToInt32(svar);
                        if(id != 0)
                        {
                            var stem = db.StemPersonvalg(id);
                            if (stem == true)
                            {
                                string brukernavn = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
                                int valgtypeID = db.PersonvalgKjørerID();
                                var valg = new Valgtyper()
                                {
                                    valgtypeid = valgtypeID,
                                    valgtype = "Personvalg"

                                };
                                db.LagreBrukerStemt(valg, brukernavn);
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                return RedirectToAction("Index");

                            }
                        }
                        else if(id == 0) // blank stemme
                        {
                            var stem = db.StemPersonvalg(id);
                            string brukernavn = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
                            int valgtypeID = db.PersonvalgKjørerID();
                            var valg = new Valgtyper()
                            {
                                valgtypeid = valgtypeID,
                                valgtype = "Personvalg"

                            };
                            db.LagreBrukerStemt(valg, brukernavn);
                            return RedirectToAction("Index");
                        }
                        

                    }
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "false")]
        public ActionResult PersonvalgStemmeEng()
        {


            var db = new BrukerBLL();
            int ID = db.PersonvalgKjørerID();

            List<Personvalg> tabell = db.hentKandidaterPersonvalg(ID);
            List<Personvalg> randomList = new List<Personvalg>();

            Random r = new Random();
            int randomIndex = 0;
            while (tabell.Count > 0)
            {
                randomIndex = r.Next(0, tabell.Count); //Choose a random object in the list
                randomList.Add(tabell[randomIndex]); //add it to the new, random list
                tabell.RemoveAt(randomIndex); //remove to avoid duplicates
            }
            return View("../user/PersonvalgStemmeEng", randomList);
        }
        [Authorize(Roles = "false")]
        public ActionResult PersonvalgStemme()
        {

            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    int ID = db.PersonvalgKjørerID();

                    List<Personvalg> tabell = db.hentKandidaterPersonvalg(ID);
                    List<Personvalg> randomList = new List<Personvalg>();

                    Random r = new Random();
                    int randomIndex = 0;
                    while (tabell.Count > 0)
                    {
                        randomIndex = r.Next(0, tabell.Count); //Choose a random object in the list
                        randomList.Add(tabell[randomIndex]); //add it to the new, random list
                        tabell.RemoveAt(randomIndex); //remove to avoid duplicates
                    }
                    return View(randomList);
                }
            }

            return RedirectToAction("Index");

        }
        [Authorize(Roles = "true")]
        public int hentAntallStemtePersonvalg_int()
        {
            
                if (Session["LoggetInn"] != null)
                {
                    bool loggetinn = (bool)Session["LoggetInn"];
                    if (loggetinn)
                    {
                        var db = new BrukerBLL();
                        return db.hentAntallStemtePersonvalg_int();
                    }
                }

                return 0;
            
        }
        [Authorize(Roles = "true")]
        public ActionResult lagretPersonvalgstop()
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    string stop = stopPersonvalg();

                    return RedirectToAction("lagretPersonvalg");
                }
            }

            return RedirectToAction("Index");

        }
        [Authorize(Roles = "true")]
        public int antallPersonvalgStemmer(int ID)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    var ok = db.antallPersonvalgStemmer(ID);
                    return ok;
                }
            }

            return 0;
 
        }
        [Authorize(Roles = "true")]
        public int PersonvalgKjørerID()
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    var ok = db.PersonvalgKjørerID();
                    return ok;
                }
            }

            return 0;
   

        }
        [Authorize(Roles = "true")]
        public string stopPersonvalg()
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    bool ok = db.stopPersonvalg();
                    if (ok == true)
                    {
                        return "true";
                    }
                    else
                    {
                        return "false";
                    }
                }
            }

            return "false";


        }
        [Authorize(Roles = "true")]
        public string startPersonvalg(int ID)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    bool ok = db.startPersonvalg(ID);
                    if (ok == true)
                    {
                        return "true";
                    }
                    else
                    {
                        return "false";
                    }

                }
            }

            return "false";

        }
        [Authorize(Roles = "true")]
        public string PersonvalgKjorer()
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    bool ok = db.PersonvalgKjorer();
                    if (ok == true)
                    {
                        return "true";
                    }
                    else
                    {
                        return "false";
                    }
                }
            }

            return "false";


        }
        [Authorize(Roles = "true")]
        public JsonResult HentKandidaterLagretID(int ID)
      {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();

                    List<String> navn = db.hentKandidaterLagretNavn(ID);


                    List<string> test = new List<string>();
                    int teller = navn.Count();
                    for (int i = 0; i < teller; i++)
                    {
                        test.Add(navn[i]);

                    }


                    return Json(test, JsonRequestBehavior.AllowGet);
                }
            }

            return null;
           
      }
        [Authorize(Roles = "true")]
        public JsonResult HentKandidaterLagret(int ID)
      {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();

                    List<String> navn = db.hentKandidaterLagretNavn(ID);


                    List<string> test = new List<string>();
                    int teller = navn.Count();
                    for (int i = 0; i < teller; i++)
                    {
                        test.Add(navn[i]);

                    }


                    return Json(test, JsonRequestBehavior.AllowGet);
                }
            }

            return null;
        }
      [Authorize(Roles = "true")]
      public ActionResult lagretPersonvalgModal(int id2)
      {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    List<PersonvalgResultat> tabell = db.hentPersonvalgResultat();
                    return View(tabell);
                }
            }

            return RedirectToAction("Index");

      }
      [Authorize(Roles = "true")]
      public ActionResult LagrePersonvalgEng()
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    var sjekk = db.LagrePersonvalg();
                    if (sjekk == false)
                    {
                        return RedirectToAction("PersonvalgEng", new { id = "feil" });
                    }
                    else
                    {
                        db.LagrePersonvalgResultat();
                        return RedirectToAction("PersonvalgEng", new { id = "sucess" });
                    }
                }
            }

            return RedirectToAction("IndexEng");


      
        }
      [Authorize(Roles = "true")]
      public ActionResult LagrePersonvalg()
      {

            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    var sjekk = db.LagrePersonvalg();
                    if (sjekk == false)
                    {
                        return RedirectToAction("Personvalg", new { id = "feil" });
                    }
                    else
                    {
                        db.LagrePersonvalgResultat();
                        return RedirectToAction("Personvalg", new { id = "sucess" });
                    }
                }
            }

            return RedirectToAction("Index");

            
     
          }
      [Authorize(Roles = "true")]
      public ActionResult slettKandidatEng(int id)
        {

            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    db.slettKandidat(id);
                    return RedirectToAction("PersonvalgEng");
                }
            }

            return RedirectToAction("Index");



        }
      [Authorize(Roles = "true")]
      public ActionResult slettKandidat(int id)
      {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    db.slettKandidat(id);
                    return RedirectToAction("Personvalg");
                }
            }

            return RedirectToAction("Index");


        }
      [Authorize(Roles = "true")]
      public ActionResult HentListeKandidatEng()
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    List<Kandidat> tabell = db.hentKandidater();
                    // fill some data for your model here
                    return PartialView("../User/PersonvalgPartialEng", tabell);
                }
            }

            return RedirectToAction("IndexEng");
        }
      [Authorize(Roles = "true")]
      public ActionResult HentListeKandidat()
      {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    List<Kandidat> tabell = db.hentKandidater();
                    // fill some data for your model here
                    return PartialView("PersonvalgPartial", tabell);
                }
            }

            return RedirectToAction("Index");
        }


    // PERSONVALG


      // start

      [HttpPost]
      public ActionResult Index(Bruker innlogget, string returnUrl)
      {
          // Ser om Model er valid or not

          if (ModelState.IsValid)
          {

              var db = new BrukerBLL();

              if (db.admin_i_db(innlogget) == (bool)true)
              {
                  String brukernavn = innlogget.brukernavn;
                  Session["LoggetInn"] = true;
                  FormsAuthentication.SetAuthCookie(brukernavn, false);

                  if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                      && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                  {
                      return Redirect(returnUrl);
                  }

                  else
                  {
                      return RedirectToAction("AdminLoggetInn", new { id = innlogget.brukernavn });
                  }



              }
              else if (db.bruker_i_db(innlogget) == (bool)true)
              {
                  String brukernavn = innlogget.brukernavn;
                  Session["LoggetInn"] = true;
                  FormsAuthentication.SetAuthCookie(brukernavn, false);

                  if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                      && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                  {
                      return Redirect(returnUrl);
                  }

                  else
                  {
                      return RedirectToAction("BrukerLoggetInn", new { id = innlogget.brukernavn });
                  }



              }
              else
              {
                    ViewBag.Innlogget = false;
                    return View();
              }
          }
          else
          {
              return View();
          }
      }
      public ActionResult Index()
      {

          if (Session["LoggetInn"] != null)

          {
              String iD = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
              var db = new BrukerBLL();
              var roles = db.hentRolleAdmin(iD);
              if (roles != null)
              {
                  return RedirectToAction("AdminLoggetInn", new { id = iD });
              }
              else
              {
                  return RedirectToAction("BrukerLoggetInn", new { id = iD });
              }

          }

          else if (FormsAuthentication.CookiesSupported == true && Session["LoggetInn"] == null)
          {

              if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
              {
                  string id = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
                  var db = new BrukerBLL();
                  var loggut = db.logg_ut_bruker(id);
                  return View();
              }
              else
              {
                  return View();
              }
          }
          else
          {
              return View();
          }

      }

      // start english
      public ActionResult IndexEng()
      {
          if (Session["LoggetInn"] != null)

          {
              String iD = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
              var db = new BrukerBLL();
              var roles = db.hentRolleAdmin(iD);
              if (roles != null)
              {
                  return RedirectToAction("AdminLoggetInnEng", new { id = iD });
              }
              else
              {
                  return RedirectToAction("BrukerLoggetInnEng", new { id = iD });
              }

          }
            else if (FormsAuthentication.CookiesSupported == true && Session["LoggetInn"] == null)
            {

                if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
                {
                    string id = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
                    var db = new BrukerBLL();
                    var loggut = db.logg_ut_bruker(id);
                    return View("../User/IndexEng");
                }
                else
                {
                    return View("../User/IndexEng");
                }
            }
            else
          {
              return View("../User/IndexEng");
          }
      }
      [HttpPost]
      public ActionResult IndexEng(Bruker innlogget, string returnUrl)
      {
          // Ser om Model er valid or not

          if (ModelState.IsValid)
          {

              var db = new BrukerBLL();

              if (db.admin_i_db(innlogget) == (bool)true)
              {
                  String brukernavn = innlogget.brukernavn;
                  Session["LoggetInn"] = true;

                  FormsAuthentication.SetAuthCookie(brukernavn, false);
                  
                  if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                      && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                  {
                      return Redirect(returnUrl);
                  }

                  else
                  {
                      return RedirectToAction("AdminLoggetInnEng", new { id = innlogget.brukernavn });
                  }


              }
              else if (db.bruker_i_db(innlogget) == (bool)true)
              {
                  String brukernavn = innlogget.brukernavn;
                  Session["LoggetInn"] = true;

                  FormsAuthentication.SetAuthCookie(brukernavn, false);

                  if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                      && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                  {
                      return Redirect(returnUrl);
                  }
                  else
                  {
                      return RedirectToAction("BrukerLoggetInnEng", new { id = innlogget.brukernavn });
                  }

              }
              else
              {
                    ViewBag.Innlogget = false;
                    return View("../User/IndexEng");
              }
          }
          else
          {
              return View("../User/IndexEng");
          }

      }

      // ActionResult Administrator

      [Authorize(Roles = "true")] // sikkerhetsmekanisme med cookie informasjon og sessionID
      public ActionResult AdminLoggetInn(string id)
      {

          if (Session["LoggetInn"] != null)
          {
              bool loggetinn = (bool)Session["LoggetInn"];
              if (loggetinn)
              {
                  var db = new BrukerBLL();
                  var bruker = db.hentEnAdmin(id);

                  return View(bruker);
              }
              else
              {
                  return RedirectToAction("Index");
              }
          }
          else
          {
              return RedirectToAction("Index");
          }
      }
      [Authorize(Roles = "true")] 
      public ActionResult LeggTilBruker()
      {
          if (Session["LoggetInn"] != null)
          {
              bool loggetinn = (bool)Session["LoggetInn"];
              if (loggetinn)
              {

                  return View();
              }
          }

          return RedirectToAction("Index");
      }
      [Authorize(Roles = "true")]
      [HttpPost]
      public ActionResult VisListe(Bruker innAdmin, int id)
      {
          if (Session["LoggetInn"] != null)
          {
              bool loggetinn = (bool)Session["LoggetInn"];
              if (loggetinn)
              {
                  var db = new BrukerBLL();
                  string brukernavn = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;

                  if (db.admin_i_db_innlogget(innAdmin, brukernavn) == (bool)true)
                  {
                      db.slettBrukerint(id);
                        return RedirectToAction("VisListe", new { id = "" });
                    }

              }
              return RedirectToAction("VisListe", new { id = id });
          }

          return RedirectToAction("Index");

      }
      [Authorize(Roles = "true")] 
      public ActionResult VisListe()
      {
          if (Session["LoggetInn"] != null)
          {
              bool loggetinn = (bool)Session["LoggetInn"];
              if (loggetinn)
              {

                  var db = new BrukerBLL();
                  List<Bruker> tabell = db.hentData();

                  return View(tabell);
              }
          }

          return RedirectToAction("Index");

      }
        [Authorize(Roles = "true")]
        public ActionResult HentResultatEng()
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    List<Valgtyper> listeValg = db.hentValgTyper();
                    return View("../user/HentResultatEng", listeValg);
                }
            }

            return RedirectToAction("Index");
        }
        [Authorize(Roles = "true")]
      public ActionResult HentResultat()
      {
          if (Session["LoggetInn"] != null)
          {
              bool loggetinn = (bool)Session["LoggetInn"];
              if (loggetinn)
              {
                  var db = new BrukerBLL();
                  List<Valgtyper> listeValg = db.hentValgTyper();
                  return View(listeValg);
              }
          }

          return RedirectToAction("Index");
      }
      [Authorize(Roles = "true")]
      public ActionResult Preferansevalg()
      {
          if (Session["LoggetInn"] != null)
          {
              bool loggetinn = (bool)Session["LoggetInn"];
              if (loggetinn)
              {
                  return View();
              }
          }

          return RedirectToAction("Index");

      }


      // ActionResult Administrator Engelsk

      [Authorize(Roles = "true")] // sikkerhetsmekanisme med cookie informasjon og sessionID
      public ActionResult AdminLoggetInnEng(string id)
      {

          if (Session["LoggetInn"] != null)
          {
              bool loggetinn = (bool)Session["LoggetInn"];
              if (loggetinn)
              {

                  var db = new BrukerBLL();
                  var bruker = db.hentEnAdmin(id);

                  return View("../User/AdminLoggetInnEng", bruker);
              }
          }

          return RedirectToAction("IndexEng");
      }
      [Authorize(Roles = "true")] 
      public ActionResult LeggTilBrukerEng()
      {
          if (Session["LoggetInn"] != null)
          {
              bool loggetinn = (bool)Session["LoggetInn"];
              if (loggetinn)
              {

                  return View("../User/LeggTilBrukerEng");
              }
          }

          return RedirectToAction("Index");

      }
      [Authorize(Roles = "true")]
      [HttpPost]
      public ActionResult VisListeEng(Kandidat innKandidat, int id)
      {
          if (Session["LoggetInn"] != null)
          {
              bool loggetinn = (bool)Session["LoggetInn"];
              if (loggetinn)
              {
                  var db = new BrukerBLL();
                  string brukernavn = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
                    Bruker innAdmin = new Bruker();
                    innAdmin.passord = innKandidat.passordEng;
                  if (db.admin_i_db_innlogget(innAdmin, brukernavn) == (bool)true)
                  {
                      db.slettBrukerint(id);
                      return RedirectToAction("VisListeEng", new { id = ""});
                  }

              }
              return RedirectToAction("VisListeEng", new { id = id });
          }

          return RedirectToAction("IndexEng");

      }

      [Authorize(Roles = "true")] 
      public ActionResult VisListeEng()
      {
          if (Session["LoggetInn"] != null)
          {
              bool loggetinn = (bool)Session["LoggetInn"];
              if (loggetinn)
              {

                  var db = new BrukerBLL();
                  List<Bruker> tabell = db.hentData();

                  return View("../User/VisListeEng", tabell);
              }
          }

          return RedirectToAction("Index");
      }
     


      // ActionResult Administrator funksjoner
      [HttpPost]
      [Authorize(Roles = "true")] // sikkerhetsmekanisme med cookie informasjon og sessionID
      public ActionResult GenererListe(int antall)
      {

          if (Session["LoggetInn"] != null)
          {
              var db = new BrukerBLL();
              bool loggetinn = (bool)Session["LoggetInn"];
              if (loggetinn)
              {
                  bool ok = db.GenererBrukere(antall);
                  if (ok)
                  {
                      return RedirectToAction("VisListe");
                  }
                  else
                  {
                      return RedirectToAction("LeggTilBruker");
                  }
              }
          }
          return RedirectToAction("Index");
      }
        [HttpPost]
        [Authorize(Roles = "true")] // sikkerhetsmekanisme med cookie informasjon og sessionID
        public ActionResult GenererListeEng(int antalleng)
        {

            if (Session["LoggetInn"] != null)
            {
                var db = new BrukerBLL();
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    bool ok = db.GenererBrukere(antalleng);
                    if (ok)
                    {
                        return RedirectToAction("VisListeEng");
                    }
                    else
                    {
                        return RedirectToAction("LeggTilBrukerEng");
                    }
                }
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Authorize(Roles = "true")]
        public ActionResult LeggTilNyBruker()
        {

            if (Session["LoggetInn"] != null)
            {
                var db = new BrukerBLL();
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    bool ok = db.GenererEnNyBruker();
                    if (ok)
                    {
                        return RedirectToAction("VisListe");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Authorize(Roles = "true")]
        public ActionResult LeggTilNyBrukerEng()
        {

            if (Session["LoggetInn"] != null)
            {
                var db = new BrukerBLL();
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    bool ok = db.GenererEnNyBruker();
                    if (ok)
                    {
                        return RedirectToAction("VisListeEng");
                    }
                    else
                    {
                        return RedirectToAction("IndexEng");
                    }
                }
            }
            return RedirectToAction("IndexEng");
        }
        [Authorize(Roles = "true")]
        public ActionResult LastNedListe()
        {
            // Lokal løsning med memoryStream
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {

                    var db = new BrukerBLL();
                    List<Bruker> tabell = db.hentData();

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (var doc = new iTextSharp.text.Document(PageSize.A4, 50, 50, 50, 50))
                        {
                            PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                            PdfPTable table = new PdfPTable(4);
                            BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
                            Font tablefont = new Font(bfTimes, 30);
                            Font tablefont2 = new Font(bfTimes, 24);
                            PdfPCell cell = new PdfPCell(new Phrase(" \n Brukernavn og Passord \n ", tablefont));
                            cell.Colspan = 4;
                            cell.HorizontalAlignment = 1;
                            table.AddCell(cell);
                            for (int i = 0; i < tabell.Count; i++)
                            {
                                // brukernavn
                                var b = new PdfPCell(new Paragraph(tabell[i].brukernavn.ToString(), tablefont2));
                                b.FixedHeight = 60f;
                                b.Colspan = 2;
                                table.AddCell(b);
                                // passord
                                var p = new PdfPCell(new Paragraph(tabell[i].passord.ToString(), tablefont2));
                                p.FixedHeight = 60f;
                                p.Colspan = 2;
                                table.AddCell(p);
                            }
                            doc.Open();
                            doc.Add(table);
                            doc.Close();
                        }
                        byte[] filedata = ms.ToArray();
                        return File(filedata, "application/pdf", "BrukernavnOgPassord.pdf");
                    }
                }
            }

            return RedirectToAction("Index");
        }
        [Authorize(Roles = "true")]
        public ActionResult LastNedListeEng()
        {
            // Lokal løsning med memoryStream
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {

                    var db = new BrukerBLL();
                    List<Bruker> tabell = db.hentData();

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (var doc = new iTextSharp.text.Document(PageSize.A4, 50, 50, 50, 50))
                        {
                            PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                            PdfPTable table = new PdfPTable(4);
                            BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
                            Font tablefont = new Font(bfTimes, 30);
                            Font tablefont2 = new Font(bfTimes, 24);
                            PdfPCell cell = new PdfPCell(new Phrase(" \n Username and Password \n ", tablefont));
                            cell.Colspan = 4;
                            cell.HorizontalAlignment = 1;
                            table.AddCell(cell);
                            for (int i = 0; i < tabell.Count; i++)
                            {
                                // brukernavn
                                var b = new PdfPCell(new Paragraph(tabell[i].brukernavn.ToString(), tablefont2));
                                b.FixedHeight = 60f;
                                b.Colspan = 2;
                                table.AddCell(b);
                                // passord
                                var p = new PdfPCell(new Paragraph(tabell[i].passord.ToString(), tablefont2));
                                p.FixedHeight = 60f;
                                p.Colspan = 2;
                                table.AddCell(p);
                            }
                            doc.Open();
                            doc.Add(table);
                            doc.Close();
                        }
                        byte[] filedata = ms.ToArray();
                        return File(filedata, "application/pdf", "UsernameAndPassword.pdf");
                    }
                }
            }

            return RedirectToAction("Index");
        }
        public ActionResult LoggUt(String id)
        {
            var db = new BrukerBLL();
            db.logg_ut_bruker(id);
            Session.Abandon();
            return RedirectToAction("index");
        }
        [Authorize(Roles = "true")]
        public ActionResult SlettBruker(String id)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {

                    var db = new BrukerBLL();
                    bool slettOK = db.slettBruker(id);
                    if (slettOK)
                    {
                        return RedirectToAction("VisListe");
                    }
                    return RedirectToAction("VisListe");
                }
            }

            return RedirectToAction("Index");

        }
        [Authorize(Roles = "true")]
        public ActionResult LoggUtBrukerEng(String id)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {

                    var db = new BrukerBLL();
                    bool ok = db.loggUtBruker(id);
                    if (ok)
                    {
                        return RedirectToAction("VisListeEng");
                    }
                    return RedirectToAction("LeggTilBrukerEng");
                }
            }

            return RedirectToAction("IndexEng");

        }
        [Authorize(Roles = "true")]
        public ActionResult LoggUtBruker(String id)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {

                    var db = new BrukerBLL();
                    bool ok = db.loggUtBruker(id);
                    if (ok)
                    {
                        return RedirectToAction("VisListe");
                    }
                    return RedirectToAction("LeggTilBruker");
                }
            }

            return RedirectToAction("Index");
            
        }

        public ActionResult Tilbake_VoteringAdmin()
        {
            string brukernavn = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            if (brukernavn != null)
            {
                return RedirectToAction("VoteringAdmin");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult Tilbake()
        {
            string brukernavn = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            if (brukernavn != null)
            {
                return RedirectToAction("AdminLoggetInn", new { id = brukernavn });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        public ActionResult TilbakeEng()
        {
            string brukernavn = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            if (brukernavn != null)
            {
                return RedirectToAction("AdminLoggetInnEng", new { id = brukernavn });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // ActionResult Bruker

        [Authorize(Roles = "false")] // sikkerhetsmekanisme med cookie informasjon og sessionID
        public ActionResult BrukerLoggetInn(string id)
        {

            if (Session["LoggetInn"] != null)
            {

                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    var bruker = db.hentEnBruker(id);
                    return View(bruker);
                }
            }

            return RedirectToAction("Index");
        }


        [Authorize(Roles = "true")]
        public ActionResult VoteringAdminEng()
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    return View("../User/VoteringAdminEng");
                }
            }

            return RedirectToAction("Index");

        }
        [Authorize(Roles = "true")]
        public ActionResult VoteringAdmin()
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    return View();
                }
            }

            return RedirectToAction("Index");
        }
        [Authorize(Roles = "true")]
        public ActionResult ResultatVotering()
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    return View();
                }
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        [Authorize(Roles = "true")]
        public ActionResult Resultat(Kandidat innAdmin, int id)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    string brukernavn = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
               
                        Bruker bruker = new Bruker();
                        bruker.passord = innAdmin.passord;
                        if (db.admin_i_db_innlogget(bruker, brukernavn) == (bool)true)
                        {
                            db.slettValg(id);
                            return RedirectToAction("Resultat", new { id = "" });
                        }
                        else
                        {
                            return RedirectToAction("Resultat", new { id = id });
                        }
                    }

                }
            

            return RedirectToAction("Index");


        }
        [Authorize(Roles = "true")]
        public ActionResult Resultat()
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    List<Valgtyper> alleValgTyper = db.hentValgTyperVotering();
                    return View(alleValgTyper);
                }

            }

            return RedirectToAction("Index");

        }
        [Authorize(Roles = "true")]
        public ActionResult ResultatVoteringEng()
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    return View("../User/ResultatVoteringEng");
                }
            }

            return RedirectToAction("Index");

        }
        [HttpPost]
        [Authorize(Roles = "true")]
        public ActionResult ResultatEng(Kandidat innAdmin, int id)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    string brukernavn = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
                   
                    bool ok = db.SjekkPassordEng(innAdmin, brukernavn);
                    if (ok == true) // passord riktig
                    {
      
                            db.slettValg(id);
                            return RedirectToAction("ResultatEng", new { id = "" });
                        }
                        else
                        {
                            return RedirectToAction("ResultatEng", new { id = id });
                        }
                    }


                }
            

            return RedirectToAction("IndexEng");

        }
        [Authorize(Roles = "true")]
        public ActionResult ResultatEng()
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    List<Valgtyper> alleValgTyper = db.hentValgTyper();
                    return View("../User/ResultatEng", alleValgTyper);
                }
            }

            return RedirectToAction("Index");

        }
        [Authorize(Roles = "true")]
        public ActionResult ResultatVoteringID(int id)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    Valgtyper Valg = db.hentValg(id);
                    return View(Valg);
                }
            }

            return RedirectToAction("Index");

        }
        [Authorize(Roles = "true")]
        public ActionResult ResultatVoteringIDEng(int id)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    Valgtyper Valg = db.hentValg(id);
                    return View("../User/ResultatVoteringIDEng", Valg);
                }
            }

            return RedirectToAction("Index");

        }
        [Authorize(Roles = "false")]
        public ActionResult Votering()
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    var votering = db.valgPågår();
                    return View(votering);
                }
            }

            return RedirectToAction("Index");

        }
        [Authorize(Roles = "false")]
        public ActionResult VoteringEng()
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    var votering = db.valgPågår();
                    return View("../User/VoteringEng", votering);
                }
            }

            return RedirectToAction("Index");

        }




        [Authorize(Roles = "false")]
        public ActionResult Voteringsvar()
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var RadioButton = Request.Form["radio"];
                    var db = new BrukerBLL();
                    if (RadioButton != null)
                    {
                        string svar = RadioButton.ToString();
                        bool ok = db.voteringsvar(svar);
                        Valgtyper valg = db.VoteringPågår();
                        string brukernavn = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;

                        if (valg != null)
                        {
                            // legg brukernavn og valgtypeid og Valgtype i en egen tabell
                            db.LagreBrukerStemt(valg, brukernavn);
                        }

                        //Må se på det her etterpå
                        if (ok == true)
                        {
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            return RedirectToAction("Index");
                        }
                    }
                    return RedirectToAction("Votering");
                }
            }

            return RedirectToAction("Index");

        }
        [Authorize(Roles = "false")]
        public ActionResult BrukerLoggetInnEng(string id)
        {

            if (Session["LoggetInn"] != null)
            {

                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    var bruker = db.hentEnBruker(id);
                    return View("../User/BrukerLoggetInnEng", bruker);
                }
            }

            return RedirectToAction("IndexEng");
        }
        // Funksjoner og Metoder
        public void startVotering()
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    bool ok = db.startVotering();


                }
            }


        }  
        public void stopVotering()
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    bool ok = db.stoppVotering();

                }
            }


        } 
        public void slettValg(int id)
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {
                    var db = new BrukerBLL();
                    db.slettValg(id);

                }
            }


        }
        public string vinnerVoteringIDEng(int id)
        {
            double forProsent = prosentFor_ID(id);
            double motProsent = prosentMot_ID(id);
            double blankProsent = prosentBlank_ID(id);
            // for vinner
            if (forProsent > motProsent && forProsent > blankProsent)
            {
                return "For";
            }
            // mot vinner
            else if (motProsent > forProsent && motProsent > blankProsent)
            {
                return "Against";
            }
            //blank vinner
            else if (blankProsent > forProsent && blankProsent > motProsent)
            {
                return "Blank";
            }
            // for == mot && for == blank  
            else if (forProsent == motProsent && forProsent == blankProsent)
            {
                return "Equal for For, Against and Blank";
            }
            // for == mot
            else if (forProsent == motProsent)
            {
                return "Equal For and Againts";
            }
            // for == blank
            else if (forProsent == blankProsent)
            {
                return "Equal For and Againts";
            }

            else
            {
                return "No election result";
            }



        }
        public string vinnerVoteringEng()
        {
            double forProsent = prosentFor();
            double motProsent = prosentMot();
            double blankProsent = prosentBlank();
            // for vinner
            if (forProsent > motProsent && forProsent > blankProsent)
            {
                return "For";
            }
            // mot vinner
            else if (motProsent > forProsent && motProsent > blankProsent)
            {
                return "Against";
            }
            //blank vinner
            else if (blankProsent > forProsent && blankProsent > motProsent)
            {
                return "Blank";
            }
            // for == mot && for == blank  
            else if (forProsent == motProsent && forProsent == blankProsent)
            {
                return "Equal for For, Against and Blank";
            }
            // for == mot
            else if (forProsent == motProsent)
            {
                return "Equal For and Againts";
            }
            // for == blank
            else if (forProsent == blankProsent)
            {
                return "Equal For and Againts";
            }

            else
            {
                return "No election result";
            }



        }            
        public string finnesdetvalg_ID(int id)
        {
            var db = new BrukerBLL();
            bool valg = db.finnesdetvalg_ID(id);
            if(valg == true)
            {
                return "true";
            }
            else
            {
                return "false";
            }
        }
        public double prosentFor_ID(int id)
        {
            double antallstemmer = antallstemteVotering(id);
            //hent for stemmer
            double antallfor = antallFor(id);
            // regn ut og retuner
            double prosent = Math.Round((antallfor / antallstemmer * 100), 1);
            return prosent;
        }
        public double prosentMot_ID(int id)
        {
            //hent antall stemte
            double antallstemmer = antallstemteVotering(id);
            //hent for stemmer
            double antallmot = antallMot(id);
            // regn ut og retuner
            double prosent = Math.Round((antallmot / antallstemmer * 100), 1);
            return prosent;
        }
        public double prosentBlank_ID(int id)
        {
            //hent antall stemte
            double antallstemmer = antallstemteVotering(id);
            //hent for stemmer
            double antallblank = antallBlank(id);
            // regn ut og retuner
            double prosent = Math.Round((antallblank / antallstemmer * 100), 1);
            return prosent;
        }
        public int antallForDiv_ID(int id)
        {
            return antallFor(id);
        }
        public int antallMotDiv_ID(int id)
        {
            return antallMot(id);
        }
        public int antallBlankDiv_ID(int id)
        {
            return antallBlank(id);
        }
        public string vinnerVotering_ID(int id)
        {
            double forProsent = prosentFor_ID(id);
            double motProsent = prosentMot_ID(id);
            double blankProsent = prosentBlank_ID(id);
            // for vinner
            if (forProsent > motProsent && forProsent > blankProsent)
            {
                return "For";
            }
            // mot vinner
            else if (motProsent > forProsent && motProsent > blankProsent)
            {
                return "Mot";
            }
            //blank vinner
            else if (blankProsent > forProsent && blankProsent > motProsent)
            {
                return "Blank";
            }
            // for == mot && for == blank  
            else if (forProsent == motProsent && forProsent == blankProsent)
            {
                return "Likt FOr, Mot og Blank";
            }
            // for == mot
            else if (forProsent == motProsent)
            {
                return "Likt For og Mot";
            }
            // for == blank
            else if (forProsent == blankProsent)
            {
                return "Likt For og Mot";
            }

            else
            {
                return "Ingen valgresultat";
            }
            
        }
        public string vinnerVotering_IDEng(int id)
        {
            double forProsent = prosentFor_ID(id);
            double motProsent = prosentMot_ID(id);
            double blankProsent = prosentBlank_ID(id);
            // for vinner
            if (forProsent > motProsent && forProsent > blankProsent)
            {
                return "For";
            }
            // mot vinner
            else if (motProsent > forProsent && motProsent > blankProsent)
            {
                return "Against";
            }
            //blank vinner
            else if (blankProsent > forProsent && blankProsent > motProsent)
            {
                return "Blank";
            }
            // for == mot && for == blank  
            else if (forProsent == motProsent && forProsent == blankProsent)
            {
                return "Equal For, Against and Blank";
            }
            // for == mot
            else if (forProsent == motProsent)
            {
                return "Equal For and Against For og Mot";
            }
            // for == blank
            else if (forProsent == blankProsent)
            {
                return "Equal For and Against";
            }

            else
            {
                return "No Election Result";
            }

        }
        public int hentAntallStemte_int()
        {
            var db = new BrukerBLL();
            return db.hentAntallStemte_int();
        }
        public int hentAntallBrukere_int()
        {
            var db = new BrukerBLL();
            return db.hentAntallBrukere_int();

        }
        public string vinnerVotering()
        {
            double forProsent = prosentFor();
            double motProsent = prosentMot();
            double blankProsent = prosentBlank();
            // for vinner
            if (forProsent > motProsent && forProsent > blankProsent)
            {
                return "For";
            }
            // mot vinner
            else if (motProsent > forProsent && motProsent > blankProsent)
            {
                return "Mot";
            }
            //blank vinner
            else if (blankProsent > forProsent && blankProsent > motProsent)
            {
                return "Blank";
            }
            // for == mot && for == blank  
            else if (forProsent == motProsent && forProsent == blankProsent)
            {
                return "Likt For, Mot og Blank";
            }
            // for == mot
            else if (forProsent == motProsent)
            {
                return "Likt For og Mot";
            }
            // for == blank
            else if (forProsent == blankProsent)
            {
                return "Likt For og Mot";
            }

            else
            {
                return "Ingen valgresultat";
            }



        }
        public int antallForDiv()
        {
            int id = hentsisteVotering();
            return antallFor(id);
        }
        public int antallMotDiv()
        {
            int id = hentsisteVotering();
            return antallMot(id);
        }
        public int antallBlankDiv()
        {
            int id = hentsisteVotering();
            return antallBlank(id);
        }
        public double prosentFor()
        {
            //hent siste votering
            int id = hentsisteVotering();
            //hent antall stemte
            double antallstemmer = antallstemteVotering(id);
            //hent for stemmer
            double antallfor = antallFor(id);
            // regn ut og retuner
            double prosent = Math.Round((antallfor / antallstemmer * 100), 1);
            return prosent;
        }
        public double prosentMot()
        {
            //hent siste votering
            int id = hentsisteVotering();
            //hent antall stemte
            double antallstemmer = antallstemteVotering(id);
            //hent for stemmer
            double antallmot = antallMot(id);
            // regn ut og retuner
            double prosent = Math.Round((antallmot / antallstemmer * 100), 1);
            return prosent;
        }
        public double prosentBlank()
        {
            //hent siste votering
            int id = hentsisteVotering();
            //hent antall stemte
            double antallstemmer = antallstemteVotering(id);
            //hent for stemmer
            double antallblank = antallBlank(id);
            // regn ut og retuner
            double prosent = Math.Round((antallblank / antallstemmer * 100), 1);
            return prosent;
        }
        public int antallstemteVotering(int id)
        {
            var db = new BrukerBLL();
            return db.antallstemteVotering(id);
        }
        public int antallFor(int id)
        {
            var db = new BrukerBLL();
            return db.antallFor(id);
        }
        public int antallMot(int id)
        {
            var db = new BrukerBLL();
            return db.antallMot(id);

        }
        public int antallBlank(int id)
        {
            var db = new BrukerBLL();
            return db.antallBlank(id);
        }
        public int hentsisteVotering()
        {
            var db = new BrukerBLL();
            return db.hentsisteVotering();
        }
        public string finnesdetvalg()
        {
            var db = new BrukerBLL();
            bool valg = db.finnesdetvalg();
            if(valg == true)
            {
                return "true";
            }
            else
            {
                return "false";
            }

        }
        public string HentResultatAntallStemt(int id)
        {
            var db = new BrukerBLL();

            string melding = db.hentAntallStemt(id);
            var jsonSerializer = new JavaScriptSerializer();

            return jsonSerializer.Serialize(melding);
        }
        public string HentAntallValg()
        {
            var db = new BrukerBLL();
            int antall = db.antallValgFullført();
            var jsonSerializer = new JavaScriptSerializer();
            return jsonSerializer.Serialize(antall);
        }
        public string VoteringKjorer()
        {

            var db = new BrukerBLL();
            bool ok = db.VoteringKjorer();
            if (ok == true)
            {
                return "true";
            }
            else
            {
                return "false";
            }

        }
        [Authorize(Roles = "false")]
        public string VoteringMelding()
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {

                    var db = new BrukerBLL();

                    string melding = db.voteringBrukerStart();
                    // check om bruker har stemt
                    var jsonSerializer = new JavaScriptSerializer();
                    if (melding == "Votering")
                    {
                        return jsonSerializer.Serialize(melding);
                    }
                    else if (melding == "Personvalg")
                    {
                        return jsonSerializer.Serialize(melding);
                    }
                    else if (melding == "Preferansevalg")
                    {
                        return jsonSerializer.Serialize(melding);
                    }

                    return jsonSerializer.Serialize(melding);
                }
            }
            return "feil";
        }
        [Authorize(Roles = "false")]
        public string BrukerStemtMld()
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {

                    var db = new BrukerBLL();
                    string brukernavn = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
                    string mld = db.HarBrukerStemt(brukernavn);
                    var jsonSerializer = new JavaScriptSerializer();
                    return jsonSerializer.Serialize(mld);
                }
            }
            return "feil";

        }
        [Authorize(Roles = "false")]
        public string BrukerStemtMldSTV()
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {

                    var db = new BrukerBLL();
                    string brukernavn = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
                    string mld = db.HarBrukerStemtSTV(brukernavn);
                    var jsonSerializer = new JavaScriptSerializer();
                    return jsonSerializer.Serialize(mld);
                }
            }
            return "feil";

        }
    }
}
 