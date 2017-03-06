using Studentparlamentet_28.BLL;
using Studentparlamentet_28.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

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
        public ActionResult LeggTilBrukerEng()
        {
            return View();
        }

        public ActionResult Votering()
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