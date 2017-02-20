using Studentparlamentet_28.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Studentparlamentet_28.DAL
{
    public class DbDal
    {
        public Bruker hentEnBruker(string id)
        {
            var db = new BrukerContext();

            var enDbBruker = db.Brukere.Find(id);

            if (enDbBruker == null)
            {
                return null;
            }
            else
            {
                var utBruker = new Bruker()
                {
                    brukernavn = enDbBruker.Brukernavn,
                    passwordhash = enDbBruker.Passord
                };
                return utBruker;
            }
        }
        public Admin hentEnAdmin(string id)
        {
            var db = new BrukerContext();

            var enDbAdmin = db.AdminBrukere.Find(id);

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

                byte[] passwordhash = lagHash(innPerson.passord);
                Bruker_db funnetBruker = db.Brukere.FirstOrDefault(b => b.Passord == passwordhash && b.Brukernavn == innPerson.brukernavn);

                if (funnetBruker == null)
                {
                    return false;
                }
                else if (funnetBruker.Innlogget == (bool)true) // sjekker om bruker er innlogget allerede
                {
                    funnetBruker.Innlogget = (bool)false;
                    db.SaveChanges();
                    return false;
                }                            
                else
                {

                    funnetBruker.Innlogget = (bool)true; // flagger bruker er innlogget
                    db.SaveChanges();

                    return true;
                }
            }

        }//End of person_i__db(Person innPerson)
        public bool admin_i_db(Bruker innAdmin)
        {
            using (var db = new BrukerContext())
            {

                byte[] passwordhash = lagHash(innAdmin.passord);
                Admin_db funnetBruker = db.AdminBrukere.FirstOrDefault(b => b.Passord == passwordhash && b.Brukernavn == innAdmin.brukernavn);
                if (funnetBruker == null)
                {
                    return false;
                }
                else if (funnetBruker.Innlogget == (bool)true) // sjekker om bruker er innlogget allerede
                {
                    funnetBruker.Innlogget = (bool)false;
                    db.SaveChanges();
                    return false;
                }                            
                else
                {

                    funnetBruker.Innlogget = (bool)true; // flagger bruker er innlogget
                    db.SaveChanges();
                    return true;
                }
            }
        }//End of Admin_i_db(Admin innAdmin)

        public bool logg_ut_bruker(string id)
        {
            using (var db = new BrukerContext())
            {
                Bruker_db funnetBruker = db.Brukere.Find(id);

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
    }
}