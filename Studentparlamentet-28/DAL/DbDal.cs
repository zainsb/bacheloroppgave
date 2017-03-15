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


namespace Studentparlamentet_28.DAL
{
    public class DbDal
    {


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

        }//End of person_i__db(Person innPerson)
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
        }//End of Admin_i_db(Admin innAdmin)

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
            if (svar == "for")
            {
                var svarvalg = new Votering_db()
                {
                    svarFor = svar
                };
                db.Voteringer.Add(svarvalg);
                db.SaveChanges();
                return true;
            }
            else if (svar == "mot")
            {
                var svarvalg = new Votering_db()
                {
                    svarMot = svar
                };
                db.Voteringer.Add(svarvalg);
                db.SaveChanges();
                return true;
            }
            else if (svar == "blank")
            {
                var svarvalg = new Votering_db()
                {
                    svarBlank = svar
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

    }
}