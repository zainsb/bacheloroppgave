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

            else if (FormsAuthentication.CookiesSupported == true)
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
                    return RedirectToAction("AdminLoggetInn", new { id = iD });
                }
                else
                {
                    return RedirectToAction("BrukerLoggetInn", new { id = iD });
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
        public ActionResult VisListe(Bruker innAdmin, String id)
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
                        db.slettBruker(id);
                        List<Bruker> tabell = db.hentData();
                        return View(tabell);
                    }
                   
                }
                return RedirectToAction("VisListe");
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
        public ActionResult Resultat(Bruker innAdmin, int id)
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
                        db.slettValg(id);
                        return RedirectToAction("Resultat");
                    }
                    else
                    {
                        return RedirectToAction("Resultat");
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
                    List<Valgtyper> alleValgTyper = db.hentValgTyper();
                    return View(alleValgTyper);
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
        [Authorize(Roles = "true")]
        public ActionResult Personvalg()
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
        public ActionResult VisListeEng(Bruker innAdmin, String id)
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
                        db.slettBruker(id);
                        return RedirectToAction("VisListeEng");
                    }

                }
                return RedirectToAction("VisListeEng");
            }

            return RedirectToAction("Index");

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
        public ActionResult ResultatEng(Bruker innAdmin, int id)
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
                        db.slettValg(id);
                        return RedirectToAction("ResultatEng");
                    }
                    else
                    {
                        return RedirectToAction("ResultatEng");
                    }
                }
            }

            return RedirectToAction("Index");
            
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
                            PdfPTable table = new PdfPTable(3);
                            BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
                            Font tablefont = new Font(bfTimes, 24);
                            Font tablefont2 = new Font(bfTimes, 16);
                            PdfPCell cell = new PdfPCell(new Phrase(" \n Brukernavn og Passord \n ", tablefont));
                            cell.Colspan = 3;
                            cell.HorizontalAlignment = 1;
                            table.AddCell(cell);
                            int teller = 0;
                            for (int i = 0; i < tabell.Count; i++)
                            {
                                teller++;
                                // nummer
                                table.AddCell(new PdfPCell(new Paragraph(teller.ToString(), tablefont2)));
                                // brukernavn
                                table.AddCell(new PdfPCell(new Paragraph(tabell[i].brukernavn.ToString(), tablefont2)));
                                // passord
                                table.AddCell(new PdfPCell(new Paragraph(tabell[i].passord.ToString(), tablefont2)));
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


            // Azure løsning med cloud
            /*
           // Retrieve storage account from connection string.
           CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
               CloudConfigurationManager.GetSetting("StorageConnectionString"));

           // Create the blob client.
           CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

           // Retrieve reference to a previously created container.
           CloudBlobContainer container = blobClient.GetContainerReference("myprivatecontainer");

           // Retrieve reference to a blob named "myblob.txt".
           CloudBlockBlob blockBlob = container.GetBlockBlobReference("BrukernavnOgPassord.pdf");

           var db = new BrukerBLL();
           List<Bruker> tabell = db.hentData();

           using (MemoryStream ms = new MemoryStream())
           {
               using (var doc = new iTextSharp.text.Document())
               {
                   PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                   PdfPTable table = new PdfPTable(3);
                   PdfPCell cell = new PdfPCell(new Phrase("Brukernavn og Passord"));
                   cell.Colspan = 3;
                   cell.HorizontalAlignment = 1;
                   table.AddCell(cell);
                   int teller = 0;
                   for (int i = 0; i < tabell.Count; i++)
                   {
                       teller++;
                       // nummer
                       table.AddCell(teller.ToString());
                       // brukernavn
                       table.AddCell(tabell[i].brukernavn.ToString());
                       // passord
                       table.AddCell(tabell[i].passord.ToString());

                   }
                   doc.Open();
                   doc.Add(table);
                   doc.Close();
               }
               var byteArray = ms.ToArray();
               var blobName = "BrukernavnOgPassord.pdf";
               var blob = container.GetBlockBlobReference(blobName);
               blob.Properties.ContentType = "application/pdf";
               blob.UploadFromByteArray(byteArray, 0, byteArray.Length);
           }
           var memStream = new MemoryStream();
           blockBlob.DownloadToStream(memStream);
           return File(memStream.ToArray(), blockBlob.Properties.ContentType);
           */

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
        [Authorize(Roles = "false")]
        public ActionResult Votering()
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


        // ActionResult Bruker Engelsk
        [Authorize(Roles = "false")] // sikkerhetsmekanisme med cookie informasjon og sessionID
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
        [Authorize(Roles = "false")]
        public ActionResult VoteringEng()
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetinn = (bool)Session["LoggetInn"];
                if (loggetinn)
                {

                    return View("../User/VoteringEng");
                }
            }

            return RedirectToAction("Index");

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
                return "ingen valgresultat";
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
        public string VoteringMelding()
        {
            var db = new BrukerBLL();

            string melding = db.voteringBrukerStart();
            // check om bruker har stemt
            var jsonSerializer = new JavaScriptSerializer();
            if (melding == "Votering")
            {
                return jsonSerializer.Serialize(melding);
            }

            return jsonSerializer.Serialize(melding);
        }
        public string BrukerStemtMld()
        {
            var db = new BrukerBLL();
            string brukernavn = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            string mld = db.HarBrukerStemt(brukernavn);
            var jsonSerializer = new JavaScriptSerializer();
            return jsonSerializer.Serialize(mld);
        }
        
    }
}
 