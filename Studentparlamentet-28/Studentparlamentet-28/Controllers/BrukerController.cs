using Studentparlamentet_28.BLL;
using Studentparlamentet_28.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Studentparlamentet_28.Controllers
{
    public class BrukerController : Controller
    {
        
        public ActionResult Index()
        {
            Session.Abandon();
            return View();
        }

        [HttpPost]
        public ActionResult Index(Bruker innlogget)
        {
            
            var db = new BrukerBLL();
            if (db.bruker_i_db(innlogget))
            {
                Session["LoggetInn"] = true;
                return RedirectToAction("BrukerLoggetInn", new { id = innlogget.brukernavn });
            }
            else if (db.admin_i_db(innlogget))
            {
                Session["LoggetInn"] = true;
                return RedirectToAction("AdminLoggetInn", new { id = innlogget.brukernavn });
            }
            else
            {
                return View();
            }
            
        }

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

    }
}