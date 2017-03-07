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

namespace Studentparlamentet_28.Controllers
{
    public class BrukerController : Controller
    {
        //Kommentar- Danish1
        //zain
        //Danish
        // DONTpain

    
        public ActionResult LeggTilBruker()
        {
            return View();
        }
        public ActionResult LastNedListe()
        {
             // Azure løsning med cloud

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

           


            /*
           // Lokal løsning
           System.IO.FileStream fs = new System.IO.FileStream(Server.MapPath("~/pdf") + "\\" + "BrukernavnOgPassord2.pdf", System.IO.FileMode.Create);
           PdfWriter writer = PdfWriter.GetInstance(document, fs);
           fs.Close();
           */

            /*
           // Løsning med lagring i MemoryStream
           var db = new BrukerBLL();
           MemoryStream memStream = new MemoryStream();
           return File(memStream.ToArray(), "application/pdf");
             Document document = new Document(PageSize.A4, 25, 25, 30, 30);

             PdfWriter writer = PdfWriter.GetInstance(document, storageAccount );
             List<Bruker> tabell = db.hentData();
             PdfPTable table = new PdfPTable(3);
             PdfPCell cell = new PdfPCell(new Phrase("Brukernavn og Passord"));
             cell.Colspan = 3;
             cell.HorizontalAlignment = 1;

             table.AddCell(cell);
             int teller = 0;
             for(int i = 0; i < tabell.Count; i++ )
             {
                teller++;
                // nummer
                table.AddCell(teller.ToString());
                // brukernavn
                table.AddCell(tabell[i].brukernavn.ToString());
                // passord
                table.AddCell(tabell[i].passord.ToString());

             }                                  
             document.Open();
             document.Add(table);
             document.Close();


             String path = Server.MapPath("~/pdf/BrukernavnOgPassord2.pdf");
             return File(path, "application/pdf", "BrukernavnOgPassord2.pdf"); 
             */


        }

        public ActionResult LeggTilBrukerEng()  
 {
 return View();
 }

 public ActionResult Votering()
 {
 return View();
 }
 public ActionResult VoteringEng()
 {
 return View();
 }

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
 public ActionResult Index()
 {

 if (Session["LoggetInn"] != null)

 {
    String iD = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
    var db = new BrukerBLL();
    var roles = db.hentRolleAdmin(iD);
    if(roles != null)
    {
        return RedirectToAction("AdminLoggetInn", new { id = iD });
    }
    else
    {
        return RedirectToAction("BrukerLoggetInn", new { id = iD });
    }
    //finn admin
    // if admin
    // else if bruker 
 }
 else
 {
               return View();

 }


 }
 public ActionResult IndexEng()
 {
 return View();
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

 public ActionResult LogOff()
 {
 FormsAuthentication.SignOut();
 return RedirectToAction("Index", "Bruker");
 }

 [Authorize(Roles = "false")] // sikkerhetsmekanisme med cookie informasjon
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
 [Authorize(Roles = "false")] // sikkerhetsmekanisme med cookie informasjon
 public ActionResult BrukerLoggetInnEng(string id)
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

 [Authorize(Roles = "true")] // sikkerhetsmekanisme med cookie informasjon
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
 [Authorize(Roles = "true")] // sikkerhetsmekanisme med cookie informasjon
 public ActionResult AdminLoggetInnEng(string id)
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
 public ActionResult LoggUt(string id)
 {
 var db = new BrukerBLL();
 db.logg_ut_bruker(id);
 Session.Abandon();
 return RedirectToAction("index");
 }
 }
 }
 