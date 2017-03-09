﻿using Studentparlamentet_28.BLL;
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

namespace Studentparlamentet_28.Controllers
{
    public class BrukerController : Controller
    {
        //Uferdig Norske view
        
        public ActionResult Preferansevalg()
        {
            return View();
        }
        public ActionResult Personvalg()
        {
            return View();
        }
        public ActionResult Resultat()
        {
            return View();
        }
        public ActionResult VoteringAdmin()
        {
            return View();
        }


        //Uferdig Engelske view
        
        public ActionResult VoteringAdminEng()
        {
            return View();
        }


        //Norske Views
        
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
            else
            {
                return View();

            }


        }
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
            }

            return RedirectToAction("Index");
        }
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
        [Authorize(Roles = "True")] // sikkerhetsmekanisme med cookie informasjon og sessionID
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
        [Authorize(Roles = "True")] // sikkerhetsmekanisme med cookie informasjon og sessionID
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

        //Funksjonalitet for Norsk og Engelsk views

        [Authorize(Roles = "True")] // sikkerhetsmekanisme med cookie informasjon og sessionID
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
        [HttpPost]
        [Authorize(Roles = "True")] // sikkerhetsmekanisme med cookie informasjon og sessionID
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
        [Authorize(Roles = "True")] // sikkerhetsmekanisme med cookie informasjon og sessionID
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
        public ActionResult LoggUt(string id)
         {
         var db = new BrukerBLL();
         db.logg_ut_bruker(id);
         Session.Abandon();
         return RedirectToAction("index");
         }


        //Engelske Views

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
        [Authorize(Roles = "True")] // sikkerhetsmekanisme med cookie informasjon og sessionID
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
        [Authorize(Roles = "True")] // sikkerhetsmekanisme med cookie informasjon og sessionID
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
    }
}
 