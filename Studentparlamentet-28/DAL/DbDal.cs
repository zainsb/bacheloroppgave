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
using System.Diagnostics;
using System.IO;
using iTextSharp.text.pdf.draw;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace Studentparlamentet_28.DAL
{
    public class DbDal
    {
        // ENDRE ADMINISTRATORKONTO

        public string encrypt(string encryptString)
        {
            string EncryptionKey = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
            });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptString = Convert.ToBase64String(ms.ToArray());
                }
            }
            return encryptString;
        }
        public string Decrypt(string cipherText)
        {
            string EncryptionKey = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
           });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
        public bool glemtPassord(Admin innBruker)
        {
            var db = new BrukerContext();
            if (innBruker.emailEng != null)
            {
                var email = db.AdminBrukere.FirstOrDefault(b => b.Email == innBruker.emailEng); // sjekk om epost er i db
                if (email != null) //if true start funksjon send epost
                {
                    //var enkryptPassord = encrypt("Admin");

                    Admin_db admin = db.AdminBrukere.FirstOrDefault(b => b.Email == innBruker.emailEng);
                    admin.Innlogget = false;             // sett innlogget kolonne til false
                                                         //   admin.Passordemail = enkryptPassord;
                    db.SaveChanges();

                    // henter ut dekryptert passord og dekrypterer det 
                    var kryptertPassord = db.AdminBrukere.FirstOrDefault(b => b.Email == innBruker.emailEng);

                    var dekryptertPassord = Decrypt(kryptertPassord.Passordemail);


                    // funksjon som sender epost med passord

                    // legg til email.email ----->
                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(email.Email, "Enquiry");
                    mail.To.Add("s188319@stud.hioa.no");
                    mail.IsBodyHtml = true;
                    mail.Subject = "Glemt passord ValgogVotering";
                    mail.Body = "Her er passordet ditt " + dekryptertPassord;
                    mail.Priority = MailPriority.High;

                    SmtpClient smtp = new SmtpClient("smtp.office365.com", 587);
                    //smtp.UseDefaultCredentials = true;
                    smtp.Credentials = new System.Net.NetworkCredential("s188319@stud.hioa.no", "Fronter1993"); // Sender fra Tommy sin mail men vi skal benytte orgkons@studentparlamentet.no
                    smtp.EnableSsl = true;
                    //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                    smtp.Send(mail);
                    return true;
                }

                else
                {
                    return false;
                }
            }
            else
            {
                var email = db.AdminBrukere.FirstOrDefault(b => b.Email == innBruker.email); // sjekk om epost er i db
                if (email != null) //if true start funksjon send epost
                {
                    //var enkryptPassord = encrypt("Admin");

                    Admin_db admin = db.AdminBrukere.FirstOrDefault(b => b.Email == innBruker.email);
                    admin.Innlogget = false;             // sett innlogget kolonne til false
                                                         //   admin.Passordemail = enkryptPassord;
                    db.SaveChanges();

                    // henter ut dekryptert passord og dekrypterer det 
                    var kryptertPassord = db.AdminBrukere.FirstOrDefault(b => b.Email == innBruker.email);

                    var dekryptertPassord = Decrypt(kryptertPassord.Passordemail);


                    // funksjon som sender epost med passord

                    // legg til email.email ----->
                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(email.Email, "Enquiry");
                    mail.To.Add("s188319@stud.hioa.no");
                    mail.IsBodyHtml = true;
                    mail.Subject = "Glemt passord ValgogVotering";
                    mail.Body = "Her er passordet ditt " + dekryptertPassord;
                    mail.Priority = MailPriority.High;

                    SmtpClient smtp = new SmtpClient("smtp.office365.com", 587);
                    //smtp.UseDefaultCredentials = true;
                    smtp.Credentials = new System.Net.NetworkCredential("s188319@stud.hioa.no", "Fronter1993");
                    smtp.EnableSsl = true;
                    //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                    smtp.Send(mail);
                    return true;
                }

                else
                {
                    return false;
                }
            }



        }
        public void tømAlleDatabaser()
        {
            var db = new BrukerContext();
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [Bruker_db]");
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [Votering_db]");
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [Valgtyper_db]");
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [BrukereStemt_db]");
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [Personvalg_db]");
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [PersonvalgResultat_db]");
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [PersonvalgKandidatResultat_db]");
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [PersonvalgResultatStemmer_db]");
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [Preferansevalg_db]");
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [KandidatListeSTV]");
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [Stemmeseddel_db]");
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [VaraListeSTV]");
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [PreferansevalgValgt_db]");

            db.SaveChanges();
        }
        public string endreBrukernavnAdmin(string gammeltBrukernavn, string nyttBrukernavn)
        {

            var db = new BrukerContext();

            Admin_db gammel = db.AdminBrukere.FirstOrDefault(a => a.Brukernavn == gammeltBrukernavn);
            if (gammel == null)
            {
                return "Feil";
            }

            Admin_db oppdatertAdmin = new Admin_db()
            {
                Brukernavn = nyttBrukernavn,
                Passord = gammel.Passord,
                Innlogget = gammel.Innlogget,
                Administrator = gammel.Administrator,
                Email = gammel.Email,
                Passordemail = gammel.Passordemail
            };
            db.AdminBrukere.Remove(gammel);
            db.AdminBrukere.Add(oppdatertAdmin);
            db.SaveChanges();
            return "OK";

        }
        public string endrePassordAdmin(string brukernavn, string gammeltPassord, string nyttpassord)
        {
            string passordEmail = encrypt(nyttpassord);   // Legger passorder i passordemail kolonne for gjenoppretting    
            var db = new BrukerContext();

            byte[] passwordhash = lagHash(nyttpassord);
            byte[] gammeltPasswordhash = lagHash(gammeltPassord);

            Admin_db gammel = db.AdminBrukere.FirstOrDefault(a => a.Brukernavn == brukernavn);
            Admin_db testAdmin = db.AdminBrukere.FirstOrDefault(a => a.Passord == gammeltPasswordhash);
            if (gammel == null)
            {
                return "Feil";
            }
            if (testAdmin == null)
            {
                return "Gammelt passord feil";
            }
            else
            {
                Admin_db oppdatertAdmin = new Admin_db()
                {
                    Brukernavn = gammel.Brukernavn,
                    Passord = passwordhash,
                    Innlogget = gammel.Innlogget,
                    Administrator = gammel.Administrator,
                    Email = gammel.Email,
                    Passordemail = passordEmail // <--
                };
                db.AdminBrukere.Remove(gammel);
                db.AdminBrukere.Add(oppdatertAdmin);
                db.SaveChanges();
                return "OK";
            }

        }

        // VOTERING

        public bool sjekkValgStemme(int ID)
        {
            var db = new BrukerContext();
            PersonvalgResultatStemmer_db avlagtstemme = db.PersonvalgResultatStemmer.FirstOrDefault(b => b.ValgtypeID == ID);
            if (avlagtstemme != null)
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
        public bool GenererEnNyBruker()
        {
            var db = new BrukerContext();

            PasswordGenerator randPassord = new PasswordGenerator(8).IncludeLowercase().IncludeUppercase();
            String brukernavn = "SP";
            int teller = db.Brukere.Count();
            int spNummer = teller + 1;
            String passord = "";

            for (int i = 0; i < 1; i++)
            {
                passord = randPassord.Next();

                var nyBruker = new Bruker_db()
                {
                    Brukernavn = brukernavn + "" + spNummer,
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
                    id = k.Id,
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
            var enDbAdmin = db.AdminBrukere.FirstOrDefault(b => b.Brukernavn == rolle);
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
        public bool slettBrukerint(int id)
        {
            var db = new BrukerContext();
            try
            {
                Bruker_db slettBruker = db.Brukere.FirstOrDefault(b => b.Id == id);
                db.Brukere.Remove(slettBruker);
                db.SaveChanges();
                return true;
            }
            catch (Exception feil)
            {
                return false;
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
                if (funnetBruker == null) // Sjekker om bruker finnes i systemet og sjekker om bruker allerede er innlogget
                {
                    return false;
                }
                else if (funnetBruker.Innlogget == (bool)true)
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

            if (valg != null)
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
        public bool admin_i_db_innloggetEng(Bruker innAdmin, string brukernavn)
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
            else if (valg.valgtype == "Personvalg")
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
        public List<Votering> hentVoteringer(int id)
        {
            using (var db = new BrukerContext())
            {
                var voteringer = db.Voteringer.Select(k => new Votering()
                {
                    valgtypeid = k.ValgtypeID,
                    svarfor = k.svarFor,
                    svarmot = k.svarMot,
                    svarblank = k.svarBlank
                }).Where(k => k.valgtypeid == id).ToList();
                return voteringer;
            }
        }
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

        // Personvalg

        public PersonvalgResultat hentinfoPersonvalg(int id)
        {
            using (var db = new BrukerContext())
            {
                PersonvalgResultat_db value = db.PersonvalgResultat.FirstOrDefault(b => b.ValgtypeID == id);
                var utValue = new PersonvalgResultat()
                {
                    valgtypeid = value.ValgtypeID,
                    vinner = value.Vinner,
                    stemmer = value.Stemmer,
                    totalantallStemmer = value.TotalantallStemmer,
                    antallkandidater = value.AntallKandidater
                };
                return utValue;

            }
        }
        public string valgtype(int id)
        {
            using (var db = new BrukerContext())
            {
                var value = db.Valgtyper.FirstOrDefault(b => b.ValgtypeID == id);
                string valgtype = Convert.ToString(value.Valgtype);
                return valgtype;
            }
        }
        public List<PersonvalgStemmer> hentPersonvalg(int id)
        {
            using (var db = new BrukerContext())
            {

                    var personvalg = db.PersonvalgResultatStemmer.Select(k => new PersonvalgStemmer()
                    {
                        id = k.ID,
                        valgtypeid = k.ValgtypeID,
                        fornavn = k.Fornavn,
                        etternavn = k.Etternavn
                    }).Where(k => k.valgtypeid == id).ToList();
                    
                    return personvalg;
                

            }
        }
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
                int teller3 = listeResultat.Count();
                for (int i = 0; i < teller3; i++)
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
                if (antallkandidater > 0)
                {
                    for (int i = 0; i < antallkandidater; i++)
                    {
                        Personvalg_db fjernKandidat = db.Personvalger.FirstOrDefault(b => b.ValgtypeID == id);
                        db.Personvalger.Remove(fjernKandidat);
                    }

                }

                int antallstemmer = db.Personvalger.Count(b => b.ValgtypeID == id);
                if (antallstemmer > 0)
                {
                    for (int i = 0; i < antallstemmer; i++)
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

            for (int i = 0; i < teller; i++)
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
            if (personvalg != null)
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
                List<string> stringarray = new List<string>();
                for (int i = 0; i < teller2; i++)
                {
                    stringarray.Add("<b>KandidatID " + listeAvKandidater.FirstOrDefault().id + " " + listeAvKandidater.FirstOrDefault().fornavn + " " + listeAvKandidater.FirstOrDefault().etternavn + "</b>");
                    var remove = listeAvKandidater.FirstOrDefault();
                    if (remove != null)
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

                    var nyResultat = new PersonvalgResultat_db()
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
            db2.Valgtyper.Add(start); // lager nytt Personvalg
            db2.SaveChanges();

            var db = new BrukerContext();


            var liste = db.Valgtyper.Select(k => new Valgtyper()
            {
                valgtypeid = k.ValgtypeID,
                valgtype = k.Valgtype,
                start = k.Start
            }).ToList(); // henter alle valg

            var listedescendingOrder = liste.OrderByDescending(i => i.valgtypeid); // lister de i ut basert på valgtypeID

            var PersonvalgID = listedescendingOrder.FirstOrDefault(i => i.valgtype == "Personvalg" && i.start == false); // henter inn siste personvalg som ble lagt inn

            var listeAvKandidater = db.Personvalger.Select(k => new Kandidat()
            {
                id = k.ID,
                fornavn = k.Fornavn,
                etternavn = k.Etternavn,
                lagret = k.Lagret,
                valgtypeid = k.ValgtypeID
            }).ToList(); // henter alle kandidater
            int tellersjekk = listeAvKandidater.Count(b => b.valgtypeid == 0); // sjekker om det er kandidater
            if (tellersjekk == 0)
            {
                return false;
            }
            int teller = listeAvKandidater.Count(); // teller kandidater
            for (int i = 0; i < teller; i++)
            {
                var listeAvKandidater2 = db.Personvalger.Select(k => new Kandidat()
                {
                    id = k.ID,
                    fornavn = k.Fornavn,
                    etternavn = k.Etternavn,
                    lagret = k.Lagret,
                    valgtypeid = k.ValgtypeID
                }).ToList(); // henter alle kandidater




                var itemToChange = listeAvKandidater2.FirstOrDefault(r => r.lagret == false); // hente rut bruker som ikke er lagret
                if (itemToChange != null)
                {
                    if (itemToChange.lagret == false)
                    {
                        try
                        {
                            int id = itemToChange.id;
                            int valgID = PersonvalgID.valgtypeid;

                            Personvalg_db endre = db.Personvalger.FirstOrDefault(r => r.ID == id);
                            endre.ValgtypeID = valgID; // legger til valgtypeID
                            endre.Lagret = (bool)true; // markerer at bruker er lagret
                            db.SaveChanges();
                        }
                        catch (Exception feil)
                        {
                            return false;
                        }
                    }




                }
            }
            try
            {
                int valgID = PersonvalgID.valgtypeid; // legger til blank "Kandidat
                var blank = new Personvalg_db()
                {
                    Fornavn = "Blank",
                    Etternavn = "Blank",
                    ValgtypeID = valgID,
                    Lagret = true,
                };

                db.Personvalger.Add(blank);
                db.SaveChanges();
                return true;
            }

            catch (Exception feil)
            {
                return false;
            }

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
        public bool leggtilPersonvalgKandidatPersonvalgEng(Kandidat innKandidat, int id)
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
                    Lagret = (bool)true,

                    ValgtypeID = id
                };
                db.Personvalger.Add(nyKandidat);
                db.SaveChanges();
                return true;
            }



        }
        public bool leggtilPersonvalgKandidatPersonvalg(Kandidat innKandidat, int id)
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
            if (teller > 0)
            {

                return false;


            }
            else
            {
                var nyKandidat = new Personvalg_db()
                {
                    Fornavn = innKandidat.fornavn_Required,
                    Etternavn = innKandidat.etternavn_Required,
                    Lagret = (bool)true,

                    ValgtypeID = id
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
            if (teller > 0)
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



        //Preferansevalg

        public int alleStemmesedlerPreferansevalg(int valgtypeid)
        {
            var db = new BrukerContext();
            int antallStemmer = db.Stemmesedler.Where(s => s.ValgtypeID == valgtypeid).Count();
            return antallStemmer;
        }
        public List<Stemmeseddel> stemmesedlerMedID(int valgtypeid)
        {
            var db = new BrukerContext();
            var stemmedler_db = db.Stemmesedler.Where(s => s.ValgtypeID == valgtypeid).ToList();
            List<Stemmeseddel> stemmesedler = new List<Stemmeseddel>();
            for (int i = 0; i < stemmedler_db.Count(); i++)
            {
                Stemmeseddel enStemmeseddel = new Stemmeseddel()
                {
                    stemmeseddelID = stemmedler_db[i].StemmeseddelID,
                    kandidatnrEn = stemmedler_db[i].KandidatnrEn,
                    kandidatnrTo = stemmedler_db[i].KandidatnrTo,
                    kandidatnrTre = stemmedler_db[i].KandidatnrTre,
                    kandidatnrFire = stemmedler_db[i].KandidatnrFire,
                    kandidatnrFem = stemmedler_db[i].KandidatnrFem,
                    kandidatnrSeks = stemmedler_db[i].KandidatnrSeks,
                    kandidatnrSju = stemmedler_db[i].KandidatnrSju,
                    kandidatnrÅtte = stemmedler_db[i].KandidatnrÅtte,
                    kandidatnrNi = stemmedler_db[i].KandidatnrNi,
                    kandidatnrTi = stemmedler_db[i].KandidatnrTi,
                    kandidatnrElleve = stemmedler_db[i].KandidatnrElleve,
                    kandidatnrTolv = stemmedler_db[i].KandidatnrTolv,
                    valgtypeid = valgtypeid
                };
                stemmesedler.Add(enStemmeseddel);
            }
            return stemmesedler;
        }
        public MemoryStream ResultatPreferansevalgTilPDF(int valgtypeid, string klasse1, string klasse2, int prosent1, int prosent2) //Kanskje gjøre om senere når jeg regner ut med kvotering
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (var doc = new iTextSharp.text.Document(PageSize.A4, 50, 50, 50, 50))
                {
                    PdfWriter writer = PdfWriter.GetInstance(doc, ms);

                    var db = new BrukerContext();

                    double valgtall = BeregnValgtall(valgtypeid);
                    var preferansevalg = db.PreferanseValg.FirstOrDefault(p => p.ValgtypeID == valgtypeid);
                    int antallRepresentanter = preferansevalg.AntallRepresentanter;
                    int antallLedigeplasser = antallRepresentanter;
                    int runde = 1;
                    int høyesteEkskluderes = 0;
                    double stemmetallEkskludert = 0;
                    double totaltOverskudd = 0;
                    double stemmetallOgOverskudd = 0;
                    bool avsluttValg = false;
                    bool fortsett = true;
                    bool ikkeEkskluderFler = false;
                    DateTime dato = DateTime.Today;


                    List<VaraSTV> listeAvKandidater = hentVaralisteMedID(valgtypeid);
                    List<VaraSTV> valgteKandidater = new List<VaraSTV>();
                    List<VaraSTV> gjenståendeKandidater = new List<VaraSTV>();
                    List<VaraSTV> ekskluderteKandidater = new List<VaraSTV>();
                    List<VaraSTV> kandidaterEkskludert2 = new List<VaraSTV>();
                    List<OverførtTilSeddel> overførteOverskudd = new List<OverførtTilSeddel>();
                    List<Stemmeseddel> sedlerPreferansevalg = preferansevalgsedler(valgtypeid); //Henter inn alle sedler for dette valget

                    //Kvotering
                    bool kvoteringsvalg = false;
                    double tempMinsteKvoteKlasse1 = (double)prosent1 * antallRepresentanter / 100;
                    int minsteKvoteKlasse1 = (int)Math.Ceiling(tempMinsteKvoteKlasse1);
                    int antallTilMinsteFylt1 = minsteKvoteKlasse1;

                    double tempMinsteKvoteKlasse2 = (double)prosent2 * antallRepresentanter / 100;
                    int minsteKvoteKlasse2 = (int)Math.Ceiling(tempMinsteKvoteKlasse2);
                    int antallTilMinsteFylt2 = minsteKvoteKlasse2;

                    int maksKvoteKlasse1 = antallRepresentanter - minsteKvoteKlasse2;
                    int maksKvoteKlasse2 = antallRepresentanter - minsteKvoteKlasse1;
                    int antallLedigeKvote1 = maksKvoteKlasse1;
                    int antallLedigeKvote2 = maksKvoteKlasse2;


                    if (klasse1 != "tom" && klasse2 != "tom" && klasse1 != "Klasse" && klasse2 != "Klasse" && klasse1 != "Choose candidate" && klasse2 != "Choose candidate")
                    {
                        kvoteringsvalg = true;
                    }

                    //Document - Info som alltid skal være med
                    doc.Open();
                    Font boldFontOverskrift = new Font(null, 16, Font.BOLD);
                    BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
                    Font tablefont = new Font(bfTimes, 24, Font.BOLD);
                    Font tablefont2 = new Font(bfTimes, 14);
                    Font tablefont4 = new Font(bfTimes, 14, Font.BOLD);
                    Font tablefont3 = new Font(bfTimes, 18, Font.BOLD);
                    Paragraph overskrift = new Paragraph("Preferansevalg resultat", boldFontOverskrift);
                    overskrift.Alignment = Element.ALIGN_CENTER;
                    doc.Add(overskrift);

                    //Lager table for PDF cells
                    PdfPTable table = new PdfPTable(1);
                    PdfPCell cell;
                    cell = new PdfPCell();
                    cell.AddElement(new Paragraph(" "));
                    cell.AddElement(new Paragraph("Antall deltagere: " + alleStemmesedlerPreferansevalg(valgtypeid)));
                    cell.AddElement(new Paragraph("Antall plasser: " + antallRepresentanter));
                    cell.AddElement(new Paragraph("Valgtall: " + valgtall));
                    cell.AddElement(new Paragraph(" "));
                    cell.AddElement(new Paragraph("Dato: " + dato.ToString("dd/MM/yyyy")));
                    cell.PaddingBottom = 20;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthLeft = 0;
                    cell.BorderWidthRight = 0;
                    table.AddCell(cell);

                    //Første runde
                    cell = new PdfPCell();
                    Font boldFont = new Font(null, 14, Font.BOLD);

                    Paragraph rundeoverskrift1 = new Paragraph("Runde 1.", boldFont);
                    cell.AddElement(rundeoverskrift1);

                    //Om noen erklært valgt legge til i Valgte kandidater liste ellers i gjenstående
                    for (int i = 0; i < listeAvKandidater.Count(); i++)
                    {
                        if (listeAvKandidater[i].stemmetall >= valgtall)
                        {
                            //Kvotering
                            if (kvoteringsvalg == true)
                            {
                                if (listeAvKandidater[i].klasse == klasse1 && antallLedigeKvote1 > 0)
                                {
                                    valgteKandidater.Add(listeAvKandidater[i]);
                                    antallTilMinsteFylt1--;
                                    antallLedigeKvote1--;
                                    antallLedigeplasser--;
                                }
                                else if (listeAvKandidater[i].klasse == klasse2 && antallLedigeKvote2 > 0)
                                {
                                    valgteKandidater.Add(listeAvKandidater[i]);
                                    antallTilMinsteFylt2--;
                                    antallLedigeKvote2--;
                                    antallLedigeplasser--;
                                }
                            }
                            else
                            {
                                //Om ikke kvotering
                                valgteKandidater.Add(listeAvKandidater[i]);
                                antallLedigeplasser--;
                            }
                        }
                        else
                        {
                            gjenståendeKandidater.Add(listeAvKandidater[i]);
                        }
                    }

                    if (valgteKandidater.Count() > 0)
                    {
                        for (int i = 0; i < valgteKandidater.Count(); i++)
                        {
                            Paragraph nyttParagraf = new Paragraph("- Kandidat " + valgteKandidater[i].navn + " har stemmetall lik " +
                                                                    valgteKandidater[i].stemmetall + " og er dermed valgt");
                            cell.AddElement(nyttParagraf);
                        }
                    }

                    //Om ingen ble valgt første runde, ekskludere alle med null
                    if (valgteKandidater.Count() == 0)
                    {
                        //Kvotering - Funker 100%
                        if (kvoteringsvalg == true)
                        {
                            //Dette kjøres om det er kvotering
                            List<VaraSTV> hjelpelistegjenståendeNull = new List<VaraSTV>();
                            for (int i = 0; i < gjenståendeKandidater.Count(); i++)
                            {
                                if (gjenståendeKandidater[i].stemmetall > 0)
                                {
                                    hjelpelistegjenståendeNull.Add(gjenståendeKandidater[i]);
                                }
                                else if (gjenståendeKandidater[i].stemmetall == 0)//Sjekker om jeg stryker en kandidat, vil det være mulig å fylle opp minstekvoten
                                {
                                    if (gjenståendeKandidater[i].klasse == klasse1)
                                    {
                                        int tempTeller = 0;
                                        int tempTellerSjekkOppMot = antallLedigeKvote1 - 1;
                                        //Sjekker hvor mange av den klasse det er igjen
                                        for (int k = 0; k < gjenståendeKandidater.Count(); k++)
                                        {
                                            if (gjenståendeKandidater[k].klasse == klasse1)
                                            {
                                                tempTeller++;
                                            }
                                        }
                                        if (tempTeller <= antallTilMinsteFylt1)
                                        {
                                            hjelpelistegjenståendeNull.Add(gjenståendeKandidater[i]);
                                        }
                                        else
                                        {
                                            Paragraph nyttParagraf = new Paragraph("- " + gjenståendeKandidater[i].navn + " strykes fra valget, ettersom ingen ble valgt i første runde og kandidaten har stemmetall lik null");
                                            cell.AddElement(nyttParagraf);
                                        }

                                    }
                                    else if (gjenståendeKandidater[i].klasse == klasse2)
                                    {
                                        int tempTeller = 0;
                                        int tempTellerSjekkOppMot = antallLedigeKvote2 - 1;
                                        //Sjekker hvor mange av den klasse det er igjen
                                        for (int k = 0; k < gjenståendeKandidater.Count(); k++)
                                        {
                                            if (gjenståendeKandidater[k].klasse == klasse2)
                                            {
                                                tempTeller++;
                                            }
                                        }
                                        if (tempTeller <= antallTilMinsteFylt2)
                                        {
                                            hjelpelistegjenståendeNull.Add(gjenståendeKandidater[i]);
                                        }
                                        else
                                        {
                                            Paragraph nyttParagraf = new Paragraph("- " + gjenståendeKandidater[i].navn + " strykes fra valget, ettersom ingen ble valgt i første runde og kandidaten har stemmetall lik null");
                                            cell.AddElement(nyttParagraf);
                                        }
                                    }
                                }
                            }
                            gjenståendeKandidater.Clear();
                            gjenståendeKandidater = hjelpelistegjenståendeNull;
                        }
                        else
                        {
                            //Dette kjøres om det ikke er kvotering
                            List<VaraSTV> hjelpelistegjenståendeNull = new List<VaraSTV>();
                            for (int i = 0; i < gjenståendeKandidater.Count(); i++)
                            {
                                if (gjenståendeKandidater[i].stemmetall > 0)
                                {
                                    hjelpelistegjenståendeNull.Add(gjenståendeKandidater[i]);
                                }
                                else
                                {
                                    Paragraph nyttParagraf = new Paragraph("- " + gjenståendeKandidater[i].navn + " strykes fra valget, ettersom ingen ble valgt i første runde og kandidaten har stemmetall lik null");
                                    cell.AddElement(nyttParagraf);
                                }
                            }
                            gjenståendeKandidater.Clear();
                            gjenståendeKandidater = hjelpelistegjenståendeNull;
                        }
                    }


                    //Antall ledige plasser er fylt ut
                    if (valgteKandidater.Count() == antallRepresentanter)
                    {
                        
                        cell.AddElement(new Paragraph("  "));
                        Paragraph nyttParagraf = new Paragraph("Antall faste valgrepresentanter som skal velges er fylt, valget avsluttes.");
                        cell.AddElement(nyttParagraf);
                        cell.PaddingBottom = 20;
                        cell.BorderWidthTop = 0;
                        cell.BorderWidthLeft = 0;
                        cell.BorderWidthRight = 0;
                        table.AddCell(cell);

                        //Legger til stemmesedler for valget
                        string valgtKandidat = "";
                        string valgtKandidatNavn = "";
                        int sedlerTeller = sedlerPreferansevalg.Count();

                        for (int j = 0; j < sedlerTeller; j++)
                        {
                            int stemmeID = sedlerPreferansevalg[j].stemmeseddelID;
                            PdfPCell cell7 = new PdfPCell(new Phrase(" \n Stemmesedler med stemmeID \n " + stemmeID + "\n \n", tablefont3));
                            cell7.Colspan = 12;
                            cell7.HorizontalAlignment = 1;
                            table.AddCell(cell7);

                            valgtKandidat = "Kandidatvalg 1";
                            var b1 = new PdfPCell(new Paragraph(valgtKandidat, tablefont4));
                            b1.FixedHeight = 50f;
                            b1.Colspan = 4;
                            table.AddCell(b1);

                            if (sedlerPreferansevalg[j].kandidatnrEn != null)
                            {
                                valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrEn;
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

                            if (sedlerPreferansevalg[j].kandidatnrTo != null)
                            {
                                valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrTo;
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

                            if (sedlerPreferansevalg[j].kandidatnrTre != null)
                            {
                                valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrTre;
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

                            if (sedlerPreferansevalg[j].kandidatnrFire != null)
                            {
                                valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrFire;
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

                            if (sedlerPreferansevalg[j].kandidatnrFem != null)
                            {
                                valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrFem;
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

                            if (sedlerPreferansevalg[j].kandidatnrSeks != null)
                            {
                                valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrSeks;
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


                            if (sedlerPreferansevalg[j].kandidatnrSju != null)
                            {
                                valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrSju;
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

                            if (sedlerPreferansevalg[j].kandidatnrÅtte != null)
                            {
                                valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrÅtte;
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

                            if (sedlerPreferansevalg[j].kandidatnrNi != null)
                            {
                                valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrNi;
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

                            if (sedlerPreferansevalg[j].kandidatnrTi != null)
                            {
                                valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrTi;
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

                            if (sedlerPreferansevalg[j].kandidatnrElleve != null)
                            {
                                valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrElleve;
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

                            if (sedlerPreferansevalg[j].kandidatnrTolv != null)
                            {
                                valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrTolv;
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

                        doc.Add(table);
                        doc.Close();
                        return ms;
                    }
                    else if (valgteKandidater.Count() > 0)
                    {
                        cell.AddElement(new Paragraph("  "));
                        Paragraph nyttParagraf = new Paragraph("Det ble valgt " + valgteKandidater.Count() + " denne runden. Valget fortsetter.");
                        cell.AddElement(nyttParagraf);
                        cell.PaddingBottom = 20;
                        cell.BorderWidthTop = 0;
                        cell.BorderWidthLeft = 0;
                        cell.BorderWidthRight = 0;
                        table.AddCell(cell);
                    }
                    else if (valgteKandidater.Count() == 0)
                    {
                        cell.AddElement(new Paragraph("  "));
                        Paragraph nyttParagraf = new Paragraph("Det ble ikke valgt noen representanter denne runden. Valget fortsetter.");
                        cell.AddElement(nyttParagraf);
                        cell.PaddingBottom = 20;
                        cell.BorderWidthTop = 0;
                        cell.BorderWidthLeft = 0;
                        cell.BorderWidthRight = 0;
                        table.AddCell(cell);
                    }

                    cell.PaddingBottom = 20;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthLeft = 0;
                    cell.BorderWidthRight = 0;


                    //Starter senere runder --KK
                    while (valgteKandidater.Count() <= antallRepresentanter && avsluttValg == false)
                    {
                        //Øk rundenummer       
                        bool noenValgt = false;
                        bool noenOverførtOverskudd = false;
                        List<VaraSTV> hjelpValgteKandidater = new List<VaraSTV>();
                        runde++;
                        ikkeEkskluderFler = false;
                        fortsett = true;

                        //Document
                        cell = new PdfPCell();
                        Paragraph rundeoverskrift = new Paragraph("Runde " + runde + ".", boldFont);
                        cell.AddElement(rundeoverskrift);

                        //Om gjenstående kandidater er mindre eller lik antall ledige plasser
                        if (kvoteringsvalg == true)
                        {
                            if (gjenståendeKandidater.Count() <= antallLedigeplasser)
                            {
                                if (gjenståendeKandidater.Count() <= antallLedigeplasser)
                                {
                                    //Alle gjenstående kandidater blir valgt
                                    for (int i = 0; i < gjenståendeKandidater.Count(); i++)
                                    {
                                        if (gjenståendeKandidater[i].klasse == klasse1 && valgteKandidater.Count() < antallRepresentanter)
                                        {
                                            if (antallLedigeKvote1 > 0)
                                            {
                                                valgteKandidater.Add(gjenståendeKandidater[i]);
                                                hjelpValgteKandidater.Add(gjenståendeKandidater[i]);
                                                noenValgt = true;
                                                antallLedigeKvote1--;
                                                antallLedigeplasser--;
                                            }
                                        }
                                        else if (gjenståendeKandidater[i].klasse == klasse2)
                                        {
                                            if (antallLedigeKvote2 > 0 && valgteKandidater.Count() < antallRepresentanter)
                                            {
                                                valgteKandidater.Add(gjenståendeKandidater[i]);
                                                hjelpValgteKandidater.Add(gjenståendeKandidater[i]);
                                                noenValgt = true;
                                                antallLedigeKvote2--;
                                                antallLedigeplasser--;
                                            }
                                        }
                                    }
                                    avsluttValg = true;

                                    cell.AddElement(new Paragraph("  "));
                                    Paragraph nyttParagraf = new Paragraph("Antall gjenstående kandidater er minde enn eller lik antall ledige plasser, valget avsluttes.");
                                    cell.AddElement(nyttParagraf);
                                    cell.PaddingBottom = 20;
                                    cell.BorderWidthTop = 0;
                                    cell.BorderWidthLeft = 0;
                                    cell.BorderWidthRight = 0;
                                    table.AddCell(cell);
                                    PdfPCell alleValgteCell = new PdfPCell();
                                    Paragraph allevalgteP = new Paragraph("Valgte kandidater: ", boldFont);
                                    alleValgteCell.AddElement(allevalgteP);
                                    for (int i = 0; i < valgteKandidater.Count(); i++)
                                    {
                                        Paragraph leggTilValgtP = new Paragraph(valgteKandidater[i].navn);
                                        alleValgteCell.AddElement(leggTilValgtP);
                                    }
                                    table.AddCell(alleValgteCell);


                                    //Legger til stemmesedler for valget
                                    string valgtKandidat = "";
                                    string valgtKandidatNavn = "";
                                    int sedlerTeller = sedlerPreferansevalg.Count();

                                    for (int j = 0; j < sedlerTeller; j++)
                                    {
                                        int stemmeID = sedlerPreferansevalg[j].stemmeseddelID;
                                        PdfPCell cell7 = new PdfPCell(new Phrase(" \n Stemmesedler med stemmeID \n " + stemmeID + "\n \n", tablefont3));
                                        cell7.Colspan = 12;
                                        cell7.HorizontalAlignment = 1;
                                        table.AddCell(cell7);

                                        valgtKandidat = "Kandidatvalg 1";
                                        var b1 = new PdfPCell(new Paragraph(valgtKandidat, tablefont4));
                                        b1.FixedHeight = 50f;
                                        b1.Colspan = 4;
                                        table.AddCell(b1);

                                        if (sedlerPreferansevalg[j].kandidatnrEn != null)
                                        {
                                            valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrEn;
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

                                        if (sedlerPreferansevalg[j].kandidatnrTo != null)
                                        {
                                            valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrTo;
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

                                        if (sedlerPreferansevalg[j].kandidatnrTre != null)
                                        {
                                            valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrTre;
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

                                        if (sedlerPreferansevalg[j].kandidatnrFire != null)
                                        {
                                            valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrFire;
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

                                        if (sedlerPreferansevalg[j].kandidatnrFem != null)
                                        {
                                            valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrFem;
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

                                        if (sedlerPreferansevalg[j].kandidatnrSeks != null)
                                        {
                                            valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrSeks;
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


                                        if (sedlerPreferansevalg[j].kandidatnrSju != null)
                                        {
                                            valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrSju;
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

                                        if (sedlerPreferansevalg[j].kandidatnrÅtte != null)
                                        {
                                            valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrÅtte;
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

                                        if (sedlerPreferansevalg[j].kandidatnrNi != null)
                                        {
                                            valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrNi;
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

                                        if (sedlerPreferansevalg[j].kandidatnrTi != null)
                                        {
                                            valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrTi;
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

                                        if (sedlerPreferansevalg[j].kandidatnrElleve != null)
                                        {
                                            valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrElleve;
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

                                        if (sedlerPreferansevalg[j].kandidatnrTolv != null)
                                        {
                                            valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrTolv;
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
                                    doc.Add(table);
                                    doc.Close();
                                    return ms;
                                }
                            }
                        }
                        else
                        {
                            if (gjenståendeKandidater.Count() <= antallLedigeplasser)
                            {
                                if (gjenståendeKandidater.Count() <= antallLedigeplasser)
                                {
                                    //Alle gjenstående kandidater blir valgt
                                    for (int i = 0; i < gjenståendeKandidater.Count(); i++)
                                    {
                                        valgteKandidater.Add(gjenståendeKandidater[i]);
                                        hjelpValgteKandidater.Add(gjenståendeKandidater[i]);
                                        noenValgt = true;
                                        antallLedigeplasser--;
                                    }
                                    avsluttValg = true;

                                    cell.AddElement(new Paragraph("  "));
                                    Paragraph nyttParagraf = new Paragraph("Antall gjenstående kandidater er minde enn eller lik antall ledige plasser, valget avsluttes.");
                                    cell.AddElement(nyttParagraf);
                                    cell.PaddingBottom = 20;
                                    cell.BorderWidthTop = 0;
                                    cell.BorderWidthLeft = 0;
                                    cell.BorderWidthRight = 0;
                                    table.AddCell(cell);
                                    PdfPCell alleValgteCell = new PdfPCell();
                                    Paragraph allevalgteP = new Paragraph("Valgte kandidater: ", boldFont);
                                    alleValgteCell.AddElement(allevalgteP);
                                    for (int i = 0; i < valgteKandidater.Count(); i++)
                                    {
                                        Paragraph leggTilValgtP = new Paragraph(valgteKandidater[i].navn);
                                        alleValgteCell.AddElement(leggTilValgtP);
                                    }
                                    table.AddCell(alleValgteCell);
                                    //Legger til stemmesedler for valget
                                    string valgtKandidat = "";
                                    string valgtKandidatNavn = "";
                                    int sedlerTeller = sedlerPreferansevalg.Count();

                                    for (int j = 0; j < sedlerTeller; j++)
                                    {
                                        int stemmeID = sedlerPreferansevalg[j].stemmeseddelID;
                                        PdfPCell cell7 = new PdfPCell(new Phrase(" \n Stemmesedler med stemmeID \n " + stemmeID + "\n \n", tablefont3));
                                        cell7.Colspan = 12;
                                        cell7.HorizontalAlignment = 1;
                                        table.AddCell(cell7);

                                        valgtKandidat = "Kandidatvalg 1";
                                        var b1 = new PdfPCell(new Paragraph(valgtKandidat, tablefont4));
                                        b1.FixedHeight = 50f;
                                        b1.Colspan = 4;
                                        table.AddCell(b1);

                                        if (sedlerPreferansevalg[j].kandidatnrEn != null)
                                        {
                                            valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrEn;
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

                                        if (sedlerPreferansevalg[j].kandidatnrTo != null)
                                        {
                                            valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrTo;
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

                                        if (sedlerPreferansevalg[j].kandidatnrTre != null)
                                        {
                                            valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrTre;
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

                                        if (sedlerPreferansevalg[j].kandidatnrFire != null)
                                        {
                                            valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrFire;
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

                                        if (sedlerPreferansevalg[j].kandidatnrFem != null)
                                        {
                                            valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrFem;
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

                                        if (sedlerPreferansevalg[j].kandidatnrSeks != null)
                                        {
                                            valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrSeks;
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


                                        if (sedlerPreferansevalg[j].kandidatnrSju != null)
                                        {
                                            valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrSju;
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

                                        if (sedlerPreferansevalg[j].kandidatnrÅtte != null)
                                        {
                                            valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrÅtte;
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

                                        if (sedlerPreferansevalg[j].kandidatnrNi != null)
                                        {
                                            valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrNi;
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

                                        if (sedlerPreferansevalg[j].kandidatnrTi != null)
                                        {
                                            valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrTi;
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

                                        if (sedlerPreferansevalg[j].kandidatnrElleve != null)
                                        {
                                            valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrElleve;
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

                                        if (sedlerPreferansevalg[j].kandidatnrTolv != null)
                                        {
                                            valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrTolv;
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

                                    doc.Add(table);
                                    doc.Close();


                                    return ms;
                                }
                            }
                        }

                        //Kvotering -- Om en av klassene har fylt opp maksKvote, så ekskluder resten av den klassen fra gjenstående
                        if (kvoteringsvalg == true)
                        {
                            if (valgteKandidater.Count() > 0)
                            {
                                //Sjekker først for klasse 1
                                int tellerAntallKlasseEn = 0;
                                for (int i = 0; i < valgteKandidater.Count(); i++)
                                {
                                    if (valgteKandidater[i].klasse == klasse1)
                                    {
                                        tellerAntallKlasseEn++;
                                    }
                                }
                                if (tellerAntallKlasseEn >= maksKvoteKlasse1)
                                {
                                    Paragraph nyttParagraf = new Paragraph("Antallet av " + klasse1 + " har oppnådd sin maksimumskvote. Dermed ekskluderes alle gjenstående kandidater fra denne klassen.");
                                    cell.AddElement(nyttParagraf);
                                    for (int k = 0; k < gjenståendeKandidater.Count(); k++)
                                    {
                                        if (gjenståendeKandidater[k].klasse == klasse1)
                                        {
                                            ekskluderteKandidater.Add(gjenståendeKandidater[k]);
                                            ikkeEkskluderFler = false;
                                            fortsett = false;
                                        }
                                    }
                                    for (int i = 0; i < ekskluderteKandidater.Count(); i++)
                                    {
                                        for (int j = 0; j < gjenståendeKandidater.Count(); j++)
                                        {
                                            if (ekskluderteKandidater[i].klasse == gjenståendeKandidater[j].klasse)
                                            {
                                                gjenståendeKandidater.Remove(gjenståendeKandidater[j]);
                                            }
                                        }
                                    }
                                }

                                //Sjekker for klasse2
                                int tellerAntallKlasseTo = 0;
                                for (int i = 0; i < valgteKandidater.Count(); i++)
                                {
                                    if (valgteKandidater[i].klasse == klasse2)
                                    {
                                        tellerAntallKlasseTo++;
                                    }
                                }
                                if (tellerAntallKlasseTo >= maksKvoteKlasse2)
                                {
                                    Paragraph nyttParagraf = new Paragraph("Antallet av " + klasse1 + " har oppnådd sin maksimumskvote. Dermed ekskluderes alle gjenstående kandidater fra denne klassen.");
                                    cell.AddElement(nyttParagraf);
                                    for (int k = 0; k < gjenståendeKandidater.Count(); k++)
                                    {
                                        if (gjenståendeKandidater[k].klasse == klasse2)
                                        {
                                            ekskluderteKandidater.Add(gjenståendeKandidater[k]);
                                            ikkeEkskluderFler = true;
                                            fortsett = false;
                                        }
                                    }
                                    for (int i = 0; i < ekskluderteKandidater.Count(); i++)
                                    {
                                        for (int j = 0; j < gjenståendeKandidater.Count(); j++)
                                        {
                                            if (ekskluderteKandidater[i].klasse == gjenståendeKandidater[j].klasse)
                                            {
                                                gjenståendeKandidater.Remove(gjenståendeKandidater[j]);
                                            }
                                        }
                                    }
                                }
                            }
                        }


                        //Om antall erklærte valgte kandidater er fylt opp
                        if (valgteKandidater.Count() == antallRepresentanter)
                        {
                            //leggTilValgteKandidater(valgteKandidater);
                            //return valgteKandidater;
                            cell.AddElement(new Paragraph("  "));
                            Paragraph nyttParagraf = new Paragraph("Antall faste valgrepresentanter som skal velges er fylt, valget avsluttes.");
                            cell.AddElement(nyttParagraf);
                            cell.PaddingBottom = 20;
                            cell.BorderWidthTop = 0;
                            cell.BorderWidthLeft = 0;
                            cell.BorderWidthRight = 0;
                            table.AddCell(cell);
                            //Legger til stemmesedler for valget
                            string valgtKandidat = "";
                            string valgtKandidatNavn = "";
                            int sedlerTeller = sedlerPreferansevalg.Count();

                            for (int j = 0; j < sedlerTeller; j++)
                            {
                                int stemmeID = sedlerPreferansevalg[j].stemmeseddelID;
                                PdfPCell cell7 = new PdfPCell(new Phrase(" \n Stemmesedler med stemmeID \n " + stemmeID + "\n \n", tablefont3));
                                cell7.Colspan = 12;
                                cell7.HorizontalAlignment = 1;
                                table.AddCell(cell7);

                                valgtKandidat = "Kandidatvalg 1";
                                var b1 = new PdfPCell(new Paragraph(valgtKandidat, tablefont4));
                                b1.FixedHeight = 50f;
                                b1.Colspan = 4;
                                table.AddCell(b1);

                                if (sedlerPreferansevalg[j].kandidatnrEn != null)
                                {
                                    valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrEn;
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

                                if (sedlerPreferansevalg[j].kandidatnrTo != null)
                                {
                                    valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrTo;
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

                                if (sedlerPreferansevalg[j].kandidatnrTre != null)
                                {
                                    valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrTre;
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

                                if (sedlerPreferansevalg[j].kandidatnrFire != null)
                                {
                                    valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrFire;
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

                                if (sedlerPreferansevalg[j].kandidatnrFem != null)
                                {
                                    valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrFem;
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

                                if (sedlerPreferansevalg[j].kandidatnrSeks != null)
                                {
                                    valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrSeks;
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


                                if (sedlerPreferansevalg[j].kandidatnrSju != null)
                                {
                                    valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrSju;
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

                                if (sedlerPreferansevalg[j].kandidatnrÅtte != null)
                                {
                                    valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrÅtte;
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

                                if (sedlerPreferansevalg[j].kandidatnrNi != null)
                                {
                                    valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrNi;
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

                                if (sedlerPreferansevalg[j].kandidatnrTi != null)
                                {
                                    valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrTi;
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

                                if (sedlerPreferansevalg[j].kandidatnrElleve != null)
                                {
                                    valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrElleve;
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

                                if (sedlerPreferansevalg[j].kandidatnrTolv != null)
                                {
                                    valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrTolv;
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
                            doc.Add(table);
                            doc.Close();
                            return ms;
                        }


                        //--------------------------------UNDER FINNE UT HVEM SOM KAN EKSKLUDERES-----------------------------------
                        if (ikkeEkskluderFler == false)
                        {
                            //Finne ut om noen kan ekskluderes
                            totaltOverskudd = 0;
                            stemmetallOgOverskudd = 0;

                            //Finner laveste stemmetall
                            if (gjenståendeKandidater.Count() > 0)
                            {
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
                                    double tempStemmetall = gjenståendeKandidater[i].stemmetall;
                                    if (tempStemmetall < stemmetallEkskludert)
                                    {
                                        if (tempStemmetall != 0)
                                        {
                                            stemmetallEkskludert = tempStemmetall;
                                        }

                                    }
                                }
                            }//Funnet laveste stemmetall

                            //Om noen har overskudd legge til for å finne hvem som kan ekskluderes
                            if (valgteKandidater.Count() > 0)
                            {
                                for (int i = 0; i < valgteKandidater.Count(); i++)
                                {
                                    double overskuddKandidat = 0;
                                    double tempValgtall = valgtall;
                                    overskuddKandidat = valgteKandidater[i].stemmetall - tempValgtall;
                                    if (overskuddKandidat > 0)
                                    {
                                        totaltOverskudd += overskuddKandidat;
                                    }
                                }
                            }

                            //Viktige variabler og lister for Ekskludering
                            høyesteEkskluderes = gjenståendeKandidater.Count() - antallLedigeplasser;
                            List<VaraSTV> potensiellEkskluderte = new List<VaraSTV>();
                            stemmetallOgOverskudd = totaltOverskudd;

                            //Sjekker hvem som potensielt kan ekskluderes
                            if (kvoteringsvalg == true)
                            {
                                for (int i = 0; i < gjenståendeKandidater.Count(); i++)
                                {
                                    bool sjekkPlassEtterEks = false;
                                    if (gjenståendeKandidater[i].stemmetall == stemmetallEkskludert && høyesteEkskluderes > potensiellEkskluderte.Count())
                                    {
                                        if (gjenståendeKandidater[i].klasse == klasse1)
                                        {
                                            int tellerAntallKlasse1IGj = 0;
                                            for (int k = 0; k < gjenståendeKandidater.Count(); k++)
                                            {
                                                if (gjenståendeKandidater[k].klasse == klasse1)
                                                {
                                                    tellerAntallKlasse1IGj++;
                                                }
                                            }
                                            if (tellerAntallKlasse1IGj > antallTilMinsteFylt1)
                                            {
                                                sjekkPlassEtterEks = true;
                                            }
                                        }
                                        else if (gjenståendeKandidater[i].klasse == klasse2)
                                        {
                                            int tellerAntallKlasse2IGj = 0;
                                            for (int k = 0; k < gjenståendeKandidater.Count(); k++)
                                            {
                                                if (gjenståendeKandidater[k].klasse == klasse2)
                                                {
                                                    tellerAntallKlasse2IGj++;
                                                }
                                            }
                                            if (tellerAntallKlasse2IGj > antallTilMinsteFylt2)
                                            {
                                                sjekkPlassEtterEks = true;
                                            }
                                        }

                                    }
                                    if (sjekkPlassEtterEks == true)
                                    {
                                        potensiellEkskluderte.Add(gjenståendeKandidater[i]);
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < gjenståendeKandidater.Count(); i++)
                                {
                                    if (gjenståendeKandidater[i].stemmetall == stemmetallEkskludert && høyesteEkskluderes > potensiellEkskluderte.Count())
                                    {
                                        potensiellEkskluderte.Add(gjenståendeKandidater[i]);
                                    }
                                }
                            }


                            if (potensiellEkskluderte.Count() > 0)
                            {
                                for (int i = 0; i < potensiellEkskluderte.Count(); i++)
                                {
                                    bool hjelpEkskludering = true;
                                    VaraSTV potensiellEkskluderes = potensiellEkskluderte[i];
                                    stemmetallOgOverskudd += potensiellEkskluderes.stemmetall;
                                    for (int k = 0; k < gjenståendeKandidater.Count(); k++)
                                    {
                                        if (gjenståendeKandidater[k].stemmetall > stemmetallEkskludert && gjenståendeKandidater[k].stemmetall <= stemmetallOgOverskudd)
                                        {
                                            hjelpEkskludering = false;
                                        }
                                    }
                                    if (hjelpEkskludering == true)
                                    {
                                        ekskluderteKandidater.Add(potensiellEkskluderes);
                                        fortsett = false;
                                    }
                                }
                                //Fjerne ekskludert kandidat fra gjenstående liste
                                for (int i = 0; i < ekskluderteKandidater.Count(); i++)
                                {
                                    for (int k = 0; k < gjenståendeKandidater.Count(); k++)
                                    {
                                        if (ekskluderteKandidater[i].navn == gjenståendeKandidater[k].navn)
                                        {
                                            gjenståendeKandidater.Remove(ekskluderteKandidater[i]);
                                        }
                                    }
                                }

                            }
                        }

                        //---------------------------Eksludert sine stemmer overføres---------------------------------------
                        if (ekskluderteKandidater.Count() > 0)
                        {
                            //Inne her skal jeg overføre stemmeseddel for den som eventuelt har blitt ekskludert og sette fortsett til false
                            for (int i = 0; i < ekskluderteKandidater.Count(); i++)
                            {
                                VaraSTV kandidatEkskluderes = ekskluderteKandidater[i];
                                string navnEkskludert = ekskluderteKandidater[i].navn;
                                double tempStemmetallEksKandidat = ekskluderteKandidater[i].stemmetall - 1;

                                Paragraph ekskluderingP = new Paragraph("- Kandidat " + navnEkskludert + " ekskluderes fra valget.");
                                cell.AddElement(ekskluderingP);
                                List<Stemmeseddel_db> listeEkskludert = db.Stemmesedler.Where(b => b.KandidatnrEn == navnEkskludert && b.ValgtypeID == valgtypeid).ToList();

                                //Sjekke om ekskludert kandidat har fått overført overskudd tidligere som må videreføres
                                if (overførteOverskudd.Count() > 0)
                                {
                                    for (int ind = 0; ind < overførteOverskudd.Count(); ind++)
                                    {
                                        OverførtTilSeddel stemme = overførteOverskudd[ind];
                                        if (navnEkskludert == stemme.overførtTil)
                                        {
                                            string seddelNavnet = "";
                                            string navnToPåSeddel = stemme.kandidatnrTo;
                                            string navnTrePåSeddel = stemme.kandidatnrTre;
                                            string navnFirePåSeddel = stemme.kandidatnrFire;
                                            string navnFemPåSeddel = stemme.kandidatnrFem;
                                            string navnSeksPåSeddel = stemme.kandidatnrSeks;
                                            string navnSjuPåSeddel = stemme.kandidatnrSju;
                                            string navnÅttePåSeddel = stemme.kandidatnrÅtte;
                                            string navnNiPåSeddel = stemme.kandidatnrNi;
                                            string navnTiPåSeddel = stemme.kandidatnrTi;
                                            string navnEllevePåSeddel = stemme.kandidatnrElleve;
                                            string navnTolvPåSedde = stemme.kandidatnrTolv;
                                            bool funnet = false;

                                            if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnToPåSeddel) != null && stemme.overførtTil != navnToPåSeddel && !funnet)
                                            {
                                                seddelNavnet = navnToPåSeddel;
                                                funnet = true;
                                            }
                                            if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnTrePåSeddel) != null && stemme.overførtTil != navnTrePåSeddel && !funnet)
                                            {
                                                seddelNavnet = navnTrePåSeddel;
                                                funnet = true;
                                            }
                                            if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnFirePåSeddel) != null && stemme.overførtTil != navnFirePåSeddel && !funnet)
                                            {
                                                seddelNavnet = navnFirePåSeddel;
                                                funnet = true;
                                            }
                                            if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnFemPåSeddel) != null && stemme.overførtTil != navnFemPåSeddel && !funnet)
                                            {
                                                seddelNavnet = navnFemPåSeddel;
                                                funnet = true;
                                            }
                                            if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnSeksPåSeddel) != null && stemme.overførtTil != navnSeksPåSeddel && !funnet)
                                            {
                                                seddelNavnet = navnSeksPåSeddel;
                                                funnet = true;
                                            }
                                            if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnSjuPåSeddel) != null && stemme.overførtTil != navnSjuPåSeddel && !funnet)
                                            {
                                                seddelNavnet = navnSjuPåSeddel;
                                                funnet = true;
                                            }
                                            if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnÅttePåSeddel) != null && stemme.overførtTil != navnÅttePåSeddel && !funnet)
                                            {
                                                seddelNavnet = navnÅttePåSeddel;
                                                funnet = true;
                                            }
                                            if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnNiPåSeddel) != null && stemme.overførtTil != navnNiPåSeddel && !funnet)
                                            {
                                                seddelNavnet = navnNiPåSeddel;
                                                funnet = true;
                                            }
                                            if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnTiPåSeddel) != null && stemme.overførtTil != navnTiPåSeddel && !funnet)
                                            {
                                                seddelNavnet = navnTiPåSeddel;
                                                funnet = true;
                                            }
                                            if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnEllevePåSeddel) != null && stemme.overførtTil != navnEllevePåSeddel && !funnet)
                                            {
                                                seddelNavnet = navnEllevePåSeddel;
                                                funnet = true;
                                            }
                                            if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnTolvPåSedde) != null && stemme.overførtTil != navnTolvPåSedde && !funnet)
                                            {
                                                seddelNavnet = navnTolvPåSedde;
                                                funnet = true;
                                            }

                                            if (seddelNavnet != "")
                                            {
                                                double tempStemmetallValgtKandidat = kandidatEkskluderes.stemmetall - stemme.overførtSum;

                                                VaraSTV oppdaterKandidat = gjenståendeKandidater.FirstOrDefault(b => b.navn == seddelNavnet);
                                                double tempStemmetallet = oppdaterKandidat.stemmetall + stemme.overførtSum;
                                                oppdaterKandidat.stemmetall = tempStemmetallet;

                                                kandidatEkskluderes.stemmetall = tempStemmetallValgtKandidat;

                                                Paragraph overførtStemmerP = new Paragraph(" " + navnEkskludert + " sin stemme overføres til " + seddelNavnet + ".");
                                                cell.PaddingLeft = 10;
                                                cell.AddElement(overførtStemmerP);

                                                OverførtTilSeddel nySeddel = new OverførtTilSeddel()
                                                {
                                                    seddelID = stemme.seddelID,
                                                    kandidatnrEn = stemme.kandidatnrEn,
                                                    kandidatnrTo = stemme.kandidatnrTo,
                                                    kandidatnrTre = stemme.kandidatnrTre,
                                                    kandidatnrFire = stemme.kandidatnrFire,
                                                    kandidatnrFem = stemme.kandidatnrFem,
                                                    kandidatnrSeks = stemme.kandidatnrSeks,
                                                    kandidatnrSju = stemme.kandidatnrSju,
                                                    kandidatnrÅtte = stemme.kandidatnrÅtte,
                                                    kandidatnrNi = stemme.kandidatnrNi,
                                                    kandidatnrTi = stemme.kandidatnrTi,
                                                    kandidatnrElleve = stemme.kandidatnrElleve,
                                                    kandidatnrTolv = stemme.kandidatnrTolv,
                                                    overførtFra = navnEkskludert,
                                                    overførtTil = seddelNavnet,
                                                    overførtSum = stemme.overførtSum,
                                                    rundenr = runde,
                                                    bleValgt = false
                                                };
                                                overførteOverskudd.Add(nySeddel);

                                                if (oppdaterKandidat.stemmetall >= valgtall)
                                                {
                                                    for (int j = 0; j < overførteOverskudd.Count(); j++)
                                                    {
                                                        if (seddelNavnet == overførteOverskudd[j].overførtTil && overførteOverskudd[j].rundenr == runde)
                                                        {
                                                            overførteOverskudd[j].bleValgt = true;
                                                        }
                                                    }
                                                }
                                            }//if-seddelnavnet!=''
                                        }
                                    }
                                }//If ekskludert kandidat hadde fått overført overskudd tidligere slutt

                                for (int j = 0; j < listeEkskludert.Count(); j++)
                                {
                                    string seddelNavnet = "";
                                    string navnToPåSeddel = listeEkskludert[j].KandidatnrTo;
                                    string navnTrePåSeddel = listeEkskludert[j].KandidatnrTre;
                                    string navnFirePåSeddel = listeEkskludert[j].KandidatnrFire;
                                    string navnFemPåSeddel = listeEkskludert[j].KandidatnrFem;
                                    string navnSeksPåSeddel = listeEkskludert[j].KandidatnrSeks;
                                    string navnSjuPåSeddel = listeEkskludert[j].KandidatnrSju;
                                    string navnÅttePåSeddel = listeEkskludert[j].KandidatnrÅtte;
                                    string navnNiPåSeddel = listeEkskludert[j].KandidatnrNi;
                                    string navnTiPåSeddel = listeEkskludert[j].KandidatnrTi;
                                    string navnEllevePåSeddel = listeEkskludert[j].KandidatnrElleve;
                                    string navnTolvPåSedde = listeEkskludert[j].KandidatnrTolv;
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
                                    else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnSeksPåSeddel) != null && !funnet)
                                    {
                                        seddelNavnet = navnSeksPåSeddel;
                                        funnet = true;
                                    }
                                    else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnSjuPåSeddel) != null && !funnet)
                                    {
                                        seddelNavnet = navnSjuPåSeddel;
                                        funnet = true;
                                    }
                                    else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnÅttePåSeddel) != null && !funnet)
                                    {
                                        seddelNavnet = navnÅttePåSeddel;
                                        funnet = true;
                                    }
                                    else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnNiPåSeddel) != null && !funnet)
                                    {
                                        seddelNavnet = navnNiPåSeddel;
                                        funnet = true;
                                    }
                                    else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnTiPåSeddel) != null && !funnet)
                                    {
                                        seddelNavnet = navnTiPåSeddel;
                                        funnet = true;
                                    }
                                    else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnEllevePåSeddel) != null && !funnet)
                                    {
                                        seddelNavnet = navnEllevePåSeddel;
                                        funnet = true;
                                    }
                                    else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnTolvPåSedde) != null && !funnet)
                                    {
                                        seddelNavnet = navnTolvPåSedde;
                                        funnet = true;
                                    }

                                    if (seddelNavnet == "")
                                    {
                                    }
                                    else
                                    {
                                        tempStemmetallEksKandidat = ekskluderteKandidater[i].stemmetall - 1;

                                        VaraSTV oppdaterKandidat = gjenståendeKandidater.FirstOrDefault(b => b.navn == seddelNavnet);
                                        double tempStemmetallet = oppdaterKandidat.stemmetall + 1;
                                        oppdaterKandidat.stemmetall = tempStemmetallet;

                                        ekskluderteKandidater[i].stemmetall = tempStemmetallEksKandidat;

                                        Paragraph overførtStemmerP = new Paragraph(" " + navnEkskludert + " sin stemme overføres til " + seddelNavnet + ".");
                                        cell.PaddingLeft = 10;
                                        cell.AddElement(overførtStemmerP);

                                        for (int k = 0; k < ekskluderteKandidater.Count(); k++)
                                        {
                                            if (ekskluderteKandidater[k].navn == seddelNavnet)
                                            {
                                                ekskluderteKandidater.Remove(oppdaterKandidat);
                                            }
                                        }
                                    }
                                }
                            }
                        }


                        //---------------------------------EKLSUDERING OVERFØRING STEMMER FERDIG----------------------------------------

                        //Sjekke om noen ble valgt etter at stemmer ble eksludert og overført
                        if (kvoteringsvalg == true)
                        {
                            for (int i = 0; i < gjenståendeKandidater.Count(); i++)
                            {
                                if (gjenståendeKandidater[i].stemmetall >= valgtall)
                                {
                                    if (valgteKandidater.Count() < antallRepresentanter)
                                    {
                                        if (gjenståendeKandidater[i].klasse == klasse1)
                                        {
                                            if (antallLedigeKvote1 > 0)
                                            {
                                                valgteKandidater.Add(gjenståendeKandidater[i]);
                                                hjelpValgteKandidater.Add(gjenståendeKandidater[i]);
                                                noenValgt = true;
                                                antallLedigeKvote1--;
                                                antallLedigeplasser--;
                                            }
                                        }
                                        else if (gjenståendeKandidater[i].klasse == klasse2)
                                        {
                                            if (antallLedigeKvote2 > 0)
                                            {
                                                valgteKandidater.Add(gjenståendeKandidater[i]);
                                                hjelpValgteKandidater.Add(gjenståendeKandidater[i]);
                                                noenValgt = true;
                                                antallLedigeKvote2--;
                                                antallLedigeplasser--;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < gjenståendeKandidater.Count(); i++)
                            {
                                if (gjenståendeKandidater[i].stemmetall >= valgtall)
                                {
                                    if (valgteKandidater.Count() < antallRepresentanter)
                                    {
                                        valgteKandidater.Add(gjenståendeKandidater[i]);
                                        hjelpValgteKandidater.Add(gjenståendeKandidater[i]);
                                        noenValgt = true;
                                        antallLedigeplasser--;
                                    }
                                }
                            }
                        }

                        //Slette de eventuelle valgte kandidatene som ble valgt fra gjenstående
                        for (int i = 0; i < valgteKandidater.Count(); i++)
                        {
                            for (int k = 0; k < gjenståendeKandidater.Count(); k++)
                            {
                                if (valgteKandidater[i].navn == gjenståendeKandidater[k].navn)
                                {
                                    gjenståendeKandidater.Remove(gjenståendeKandidater[k]);
                                }
                            }
                        }

                        //-----------------------------------Overføre overskudd eller ekskludere en random--------------------------------

                        //Under her ta en if-fortsett == true.. Så skal det overføres ingen ble ekskludert denne runden
                        if (fortsett == true)
                        {
                            //Inne her overføre eventuelt overskudd eller tving en ekskludering

                            //Sjekke om en eller flere har overskudd
                            int antallHarOverskudd = 0;
                            for (int i = 0; i < valgteKandidater.Count(); i++)
                            {
                                double overskudd = 0;
                                VaraSTV valgtKandidat = valgteKandidater[i];
                                double tempValgtall = valgtall;
                                overskudd = valgtKandidat.stemmetall - tempValgtall;
                                if (overskudd > 0)
                                {
                                    antallHarOverskudd++;
                                }
                            }

                            //Om den fant noen valgte kandidater med overskudd
                            if (antallHarOverskudd > 0)
                            {
                                //Finn det største overskuddet
                                double tempOverskudd = 0;
                                VaraSTV kandidatOverføresFra = new VaraSTV();
                                for (int i = 0; i < valgteKandidater.Count(); i++)
                                {
                                    double overskudd = 0;
                                    VaraSTV valgtKandidat = valgteKandidater[i];
                                    double tempValgtall = valgtall;
                                    overskudd = valgtKandidat.stemmetall - tempValgtall;
                                    if (overskudd > tempOverskudd)
                                    {
                                        tempOverskudd = overskudd;
                                        kandidatOverføresFra = valgtKandidat; //Kandidat med største overskuddet
                                    }
                                }

                                int antallAvKandidatIListe = 0;
                                for (int i = 0; i < overførteOverskudd.Count(); i++)
                                {
                                    if (overførteOverskudd[i].overførtTil == kandidatOverføresFra.navn && overførteOverskudd[i].bleValgt == true)
                                    {
                                        antallAvKandidatIListe++;
                                    }
                                }

                                //Om kandidat det overføres fra sitt stemmetall ble større etter å ha fått overført stemmer i senere runder og ikke i første
                                if (antallAvKandidatIListe > 0)
                                {
                                    List<OverførtTilSeddel> overførteStemmer = new List<OverførtTilSeddel>();
                                    for (int i = 0; i < overførteOverskudd.Count(); i++)
                                    {
                                        if (overførteOverskudd[i].overførtTil == kandidatOverføresFra.navn && overførteOverskudd[i].bleValgt == true)
                                        {
                                            overførteStemmer.Add(overførteOverskudd[i]);
                                        }
                                    }

                                    double overføresOverskudd = tempOverskudd / kandidatOverføresFra.stemmetall;
                                    Paragraph overføringp = new Paragraph("- Overfører overskudd fra " + kandidatOverføresFra.navn + ".");
                                    cell.AddElement(overføringp);

                                    //Starter overføringen
                                    for (int i = 0; i < overførteStemmer.Count(); i++)
                                    {
                                        OverførtTilSeddel stemme = overførteStemmer[i];
                                        string seddelNavnet = "";
                                        string navnToPåSeddel = stemme.kandidatnrTo;
                                        string navnTrePåSeddel = stemme.kandidatnrTre;
                                        string navnFirePåSeddel = stemme.kandidatnrFire;
                                        string navnFemPåSeddel = stemme.kandidatnrFem;
                                        string navnSeksPåSeddel = stemme.kandidatnrSeks;
                                        string navnSjuPåSeddel = stemme.kandidatnrSju;
                                        string navnÅttePåSeddel = stemme.kandidatnrÅtte;
                                        string navnNiPåSeddel = stemme.kandidatnrNi;
                                        string navnTiPåSeddel = stemme.kandidatnrTi;
                                        string navnEllevePåSeddel = stemme.kandidatnrElleve;
                                        string navnTolvPåSeddel = stemme.kandidatnrTolv;
                                        bool funnet = false;

                                        if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnToPåSeddel) != null && stemme.overførtTil != navnToPåSeddel && !funnet)
                                        {
                                            seddelNavnet = navnToPåSeddel;
                                            funnet = true;
                                        }
                                        if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnTrePåSeddel) != null && stemme.overførtTil != navnTrePåSeddel && !funnet)
                                        {
                                            seddelNavnet = navnTrePåSeddel;
                                            funnet = true;
                                        }
                                        if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnFirePåSeddel) != null && stemme.overførtTil != navnFirePåSeddel && !funnet)
                                        {
                                            seddelNavnet = navnFirePåSeddel;
                                            funnet = true;
                                        }
                                        if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnFemPåSeddel) != null && stemme.overførtTil != navnFemPåSeddel && !funnet)
                                        {
                                            seddelNavnet = navnFemPåSeddel;
                                            funnet = true;
                                        }
                                        if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnSeksPåSeddel) != null && stemme.overførtTil != navnSeksPåSeddel && !funnet)
                                        {
                                            seddelNavnet = navnSeksPåSeddel;
                                            funnet = true;
                                        }
                                        if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnSjuPåSeddel) != null && stemme.overførtTil != navnSjuPåSeddel && !funnet)
                                        {
                                            seddelNavnet = navnSjuPåSeddel;
                                            funnet = true;
                                        }
                                        if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnÅttePåSeddel) != null && stemme.overførtTil != navnÅttePåSeddel && !funnet)
                                        {
                                            seddelNavnet = navnÅttePåSeddel;
                                            funnet = true;
                                        }
                                        if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnNiPåSeddel) != null && stemme.overførtTil != navnNiPåSeddel && !funnet)
                                        {
                                            seddelNavnet = navnNiPåSeddel;
                                            funnet = true;
                                        }
                                        if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnTiPåSeddel) != null && stemme.overførtTil != navnTiPåSeddel && !funnet)
                                        {
                                            seddelNavnet = navnTiPåSeddel;
                                            funnet = true;
                                        }
                                        if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnEllevePåSeddel) != null && stemme.overførtTil != navnEllevePåSeddel && !funnet)
                                        {
                                            seddelNavnet = navnEllevePåSeddel;
                                            funnet = true;
                                        }
                                        if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnTolvPåSeddel) != null && stemme.overførtTil != navnTolvPåSeddel && !funnet)
                                        {
                                            seddelNavnet = navnTolvPåSeddel;
                                            funnet = true;
                                        }

                                        if (seddelNavnet != "")
                                        {
                                            double tempStemmetallValgtKandidat = kandidatOverføresFra.stemmetall - overføresOverskudd;

                                            VaraSTV oppdaterKandidat = gjenståendeKandidater.FirstOrDefault(b => b.navn == seddelNavnet);
                                            double tempStemmetallet = oppdaterKandidat.stemmetall + overføresOverskudd;
                                            oppdaterKandidat.stemmetall = tempStemmetallet;

                                            kandidatOverføresFra.stemmetall = tempStemmetallValgtKandidat;

                                            noenOverførtOverskudd = true;
                                            OverførtTilSeddel nySeddel = new OverførtTilSeddel()
                                            {
                                                seddelID = stemme.seddelID,
                                                kandidatnrEn = stemme.kandidatnrEn,
                                                kandidatnrTo = stemme.kandidatnrTo,
                                                kandidatnrTre = stemme.kandidatnrTre,
                                                kandidatnrFire = stemme.kandidatnrFire,
                                                kandidatnrFem = stemme.kandidatnrFem,
                                                kandidatnrSeks = stemme.kandidatnrSeks,
                                                kandidatnrSju = stemme.kandidatnrSju,
                                                kandidatnrÅtte = stemme.kandidatnrÅtte,
                                                kandidatnrNi = stemme.kandidatnrNi,
                                                kandidatnrTi = stemme.kandidatnrTi,
                                                kandidatnrElleve = stemme.kandidatnrElleve,
                                                kandidatnrTolv = stemme.kandidatnrTolv,
                                                overførtFra = kandidatOverføresFra.navn,
                                                overførtTil = seddelNavnet,
                                                overførtSum = overføresOverskudd,
                                                rundenr = runde,
                                                bleValgt = false
                                            };
                                            overførteOverskudd.Add(nySeddel);

                                            if (oppdaterKandidat.stemmetall >= valgtall)
                                            {
                                                for (int j = 0; j < overførteOverskudd.Count(); j++)
                                                {
                                                    if (seddelNavnet == overførteOverskudd[j].overførtTil && overførteOverskudd[j].rundenr == runde)
                                                    {
                                                        overførteOverskudd[j].bleValgt = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    kandidatOverføresFra.stemmetall = valgtall;

                                }
                                else
                                {
                                    List<Stemmeseddel_db> overførStemmer = db.Stemmesedler.Where(s => s.KandidatnrEn == kandidatOverføresFra.navn && s.ValgtypeID == valgtypeid).ToList();
                                    //Finne overskuddtallet som skal overføres til alle stemmesedlene
                                    double overføresOverskudd = tempOverskudd / kandidatOverføresFra.stemmetall;
                                    Paragraph overføringp = new Paragraph("- Overfører overskudd fra " + kandidatOverføresFra.navn + ".");
                                    cell.AddElement(overføringp);

                                    for (int i = 0; i < overførStemmer.Count(); i++)
                                    {
                                        Stemmeseddel_db stemme = overførStemmer[i];
                                        string seddelNavnet = "";
                                        string navnToPåSeddel = stemme.KandidatnrTo;
                                        string navnTrePåSeddel = stemme.KandidatnrTre;
                                        string navnFirePåSeddel = stemme.KandidatnrFire;
                                        string navnFemPåSeddel = stemme.KandidatnrFem;
                                        string navnSeksPåSeddel = stemme.KandidatnrSeks;
                                        string navnSjuPåSeddel = stemme.KandidatnrSju;
                                        string navnÅttePåSeddel = stemme.KandidatnrÅtte;
                                        string navnNiPåSeddel = stemme.KandidatnrNi;
                                        string navnTiPåSeddel = stemme.KandidatnrTi;
                                        string navnEllevePåSeddel = stemme.KandidatnrElleve;
                                        string navnTolvPåSeddel = stemme.KandidatnrTolv;
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
                                        else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnSeksPåSeddel) != null && !funnet)
                                        {
                                            seddelNavnet = navnSeksPåSeddel;
                                            funnet = true;
                                        }
                                        else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnSjuPåSeddel) != null && !funnet)
                                        {
                                            seddelNavnet = navnSjuPåSeddel;
                                            funnet = true;
                                        }
                                        else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnÅttePåSeddel) != null && !funnet)
                                        {
                                            seddelNavnet = navnÅttePåSeddel;
                                            funnet = true;
                                        }
                                        else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnNiPåSeddel) != null && !funnet)
                                        {
                                            seddelNavnet = navnNiPåSeddel;
                                            funnet = true;
                                        }
                                        else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnTiPåSeddel) != null && !funnet)
                                        {
                                            seddelNavnet = navnTiPåSeddel;
                                            funnet = true;
                                        }
                                        else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnEllevePåSeddel) != null && !funnet)
                                        {
                                            seddelNavnet = navnEllevePåSeddel;
                                            funnet = true;
                                        }
                                        else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnTolvPåSeddel) != null && !funnet)
                                        {
                                            seddelNavnet = navnTolvPåSeddel;
                                            funnet = true;
                                        }

                                        if (seddelNavnet != "")
                                        {
                                            double tempStemmetallValgtKandidat = kandidatOverføresFra.stemmetall - overføresOverskudd;

                                            VaraSTV oppdaterKandidat = gjenståendeKandidater.FirstOrDefault(b => b.navn == seddelNavnet);
                                            double tempStemmetallet = oppdaterKandidat.stemmetall + overføresOverskudd;
                                            oppdaterKandidat.stemmetall = tempStemmetallet;

                                            kandidatOverføresFra.stemmetall = tempStemmetallValgtKandidat;
                                            noenOverførtOverskudd = true;

                                            OverførtTilSeddel nySeddel = new OverførtTilSeddel()
                                            {
                                                seddelID = stemme.StemmeseddelID,
                                                kandidatnrEn = stemme.KandidatnrEn,
                                                kandidatnrTo = stemme.KandidatnrTo,
                                                kandidatnrTre = stemme.KandidatnrTre,
                                                kandidatnrFire = stemme.KandidatnrFire,
                                                kandidatnrFem = stemme.KandidatnrFem,
                                                kandidatnrSeks = stemme.KandidatnrSeks,
                                                kandidatnrSju = stemme.KandidatnrSju,
                                                kandidatnrÅtte = stemme.KandidatnrÅtte,
                                                kandidatnrNi = stemme.KandidatnrNi,
                                                kandidatnrTi = stemme.KandidatnrTi,
                                                kandidatnrElleve = stemme.KandidatnrElleve,
                                                kandidatnrTolv = stemme.KandidatnrTolv,
                                                overførtFra = kandidatOverføresFra.navn,
                                                overførtTil = seddelNavnet,
                                                overførtSum = overføresOverskudd,
                                                rundenr = runde,
                                                bleValgt = false
                                            };
                                            overførteOverskudd.Add(nySeddel);

                                            if (oppdaterKandidat.stemmetall >= valgtall)
                                            {
                                                for (int j = 0; j < overførteOverskudd.Count(); j++)
                                                {
                                                    if (seddelNavnet == overførteOverskudd[j].overførtTil && overførteOverskudd[j].rundenr == runde)
                                                    {
                                                        overførteOverskudd[j].bleValgt = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    kandidatOverføresFra.stemmetall = valgtall;
                                }
                            }//If- Fant overskudd
                            else 
                            {
                                if (kvoteringsvalg == true)
                                {
                                    //Finner laveste stemmetall
                                    if (gjenståendeKandidater.Count() > 0)
                                    {
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
                                            double tempStemmetall = gjenståendeKandidater[i].stemmetall;
                                            if (tempStemmetall < stemmetallEkskludert)
                                            {
                                                if (tempStemmetall != 0)
                                                {
                                                    if (gjenståendeKandidater[i].klasse == klasse1)
                                                    {
                                                        int hjelpeteller = 0;
                                                        for (int k = 0; k < gjenståendeKandidater.Count(); k++)
                                                        {
                                                            if (gjenståendeKandidater[k].klasse == klasse1)
                                                            {
                                                                hjelpeteller++;
                                                            }
                                                        }
                                                        if (hjelpeteller > antallTilMinsteFylt1)
                                                        {
                                                            stemmetallEkskludert = tempStemmetall;
                                                        }
                                                    }
                                                    else if (gjenståendeKandidater[i].klasse == klasse2)
                                                    {
                                                        int hjelpeteller2 = 0;
                                                        for (int k = 0; k < gjenståendeKandidater.Count(); k++)
                                                        {
                                                            if (gjenståendeKandidater[k].klasse == klasse2)
                                                            {
                                                                hjelpeteller2++;
                                                            }
                                                        }
                                                        if (hjelpeteller2 > antallTilMinsteFylt2)
                                                        {
                                                            stemmetallEkskludert = tempStemmetall;
                                                        }
                                                    }
                                                }

                                            }
                                        }
                                    }//Funnet laveste stemmetall

                                    VaraSTV kandidatLavestStemmetall = gjenståendeKandidater.FirstOrDefault(k => k.stemmetall == stemmetallEkskludert);
                                    if (kandidatLavestStemmetall != null)
                                    {
                                        gjenståendeKandidater.Remove(kandidatLavestStemmetall);
                                    }
                                    string navnEkskludert = kandidatLavestStemmetall.navn;
                                    Paragraph ekskluderingP = new Paragraph("- Kandidat " + navnEkskludert + " ekskluderes fra valget.");
                                    cell.AddElement(ekskluderingP);

                                    double tempStemmetallEksKandidat = kandidatLavestStemmetall.stemmetall - 1;
                                    List<Stemmeseddel_db> listeEkskludert = db.Stemmesedler.Where(b => b.KandidatnrEn == navnEkskludert && b.ValgtypeID == valgtypeid).ToList();

                                    for (int j = 0; j < listeEkskludert.Count(); j++)
                                    {
                                        string seddelNavnet = "";
                                        string navnToPåSeddel = listeEkskludert[j].KandidatnrTo;
                                        string navnTrePåSeddel = listeEkskludert[j].KandidatnrTre;
                                        string navnFirePåSeddel = listeEkskludert[j].KandidatnrFire;
                                        string navnFemPåSeddel = listeEkskludert[j].KandidatnrFem;
                                        string navnSeksPåSeddel = listeEkskludert[j].KandidatnrSeks;
                                        string navnSjuPåSeddel = listeEkskludert[j].KandidatnrSju;
                                        string navnÅttePåSeddel = listeEkskludert[j].KandidatnrÅtte;
                                        string navnNiPåSeddel = listeEkskludert[j].KandidatnrNi;
                                        string navnTiPåSeddel = listeEkskludert[j].KandidatnrTi;
                                        string navnEllevePåSeddel = listeEkskludert[j].KandidatnrElleve;
                                        string navnTolvPåSedde = listeEkskludert[j].KandidatnrTolv;
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
                                        else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnSeksPåSeddel) != null && !funnet)
                                        {
                                            seddelNavnet = navnSeksPåSeddel;
                                            funnet = true;
                                        }
                                        else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnSjuPåSeddel) != null && !funnet)
                                        {
                                            seddelNavnet = navnSjuPåSeddel;
                                            funnet = true;
                                        }
                                        else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnÅttePåSeddel) != null && !funnet)
                                        {
                                            seddelNavnet = navnÅttePåSeddel;
                                            funnet = true;
                                        }
                                        else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnNiPåSeddel) != null && !funnet)
                                        {
                                            seddelNavnet = navnNiPåSeddel;
                                            funnet = true;
                                        }
                                        else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnTiPåSeddel) != null && !funnet)
                                        {
                                            seddelNavnet = navnTiPåSeddel;
                                            funnet = true;
                                        }
                                        else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnEllevePåSeddel) != null && !funnet)
                                        {
                                            seddelNavnet = navnEllevePåSeddel;
                                            funnet = true;
                                        }
                                        else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnTolvPåSedde) != null && !funnet)
                                        {
                                            seddelNavnet = navnTolvPåSedde;
                                            funnet = true;
                                        }

                                        if (seddelNavnet == "")
                                        {
                                        }
                                        else
                                        {
                                            tempStemmetallEksKandidat = kandidatLavestStemmetall.stemmetall - 1;

                                            VaraSTV oppdaterKandidat = gjenståendeKandidater.FirstOrDefault(b => b.navn == seddelNavnet);
                                            double tempStemmetallet = oppdaterKandidat.stemmetall + 1;
                                            oppdaterKandidat.stemmetall = tempStemmetallet;

                                            kandidatLavestStemmetall.stemmetall = tempStemmetallEksKandidat;

                                            Paragraph overførtStemmerP = new Paragraph(" " + navnEkskludert + " sin stemme overføres til " + seddelNavnet + ".");
                                            cell.AddElement(overførtStemmerP);
                                        }
                                    }
                                }
                                else
                                {
                                 //Finner laveste stemmetall
                                    if (gjenståendeKandidater.Count() > 0)
                                    {
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
                                            double tempStemmetall = gjenståendeKandidater[i].stemmetall;
                                            if (tempStemmetall < stemmetallEkskludert)
                                            {
                                                if (tempStemmetall != 0)
                                                {
                                                    stemmetallEkskludert = tempStemmetall;
                                                }

                                            }
                                        }
                                    }//Funnet laveste stemmetall
                                     //Finner en av de med laveste stemmetallet - Tilfeldig
                                    VaraSTV kandidatLavestStemmetall = gjenståendeKandidater.FirstOrDefault(k => k.stemmetall == stemmetallEkskludert);
                                    if (kandidatLavestStemmetall != null)
                                    {
                                        gjenståendeKandidater.Remove(kandidatLavestStemmetall);
                                    }
                                    string navnEkskludert = kandidatLavestStemmetall.navn;
                                    double tempStemmetallEksKandidat = kandidatLavestStemmetall.stemmetall - 1;
                                    List<Stemmeseddel_db> listeEkskludert = db.Stemmesedler.Where(b => b.KandidatnrEn == navnEkskludert && b.ValgtypeID == valgtypeid).ToList();

                                    for (int j = 0; j < listeEkskludert.Count(); j++)
                                    {
                                        string seddelNavnet = "";
                                        string navnToPåSeddel = listeEkskludert[j].KandidatnrTo;
                                        string navnTrePåSeddel = listeEkskludert[j].KandidatnrTre;
                                        string navnFirePåSeddel = listeEkskludert[j].KandidatnrFire;
                                        string navnFemPåSeddel = listeEkskludert[j].KandidatnrFem;
                                        string navnSeksPåSeddel = listeEkskludert[j].KandidatnrSeks;
                                        string navnSjuPåSeddel = listeEkskludert[j].KandidatnrSju;
                                        string navnÅttePåSeddel = listeEkskludert[j].KandidatnrÅtte;
                                        string navnNiPåSeddel = listeEkskludert[j].KandidatnrNi;
                                        string navnTiPåSeddel = listeEkskludert[j].KandidatnrTi;
                                        string navnEllevePåSeddel = listeEkskludert[j].KandidatnrElleve;
                                        string navnTolvPåSedde = listeEkskludert[j].KandidatnrTolv;
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
                                        else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnSeksPåSeddel) != null && !funnet)
                                        {
                                            seddelNavnet = navnSeksPåSeddel;
                                            funnet = true;
                                        }
                                        else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnSjuPåSeddel) != null && !funnet)
                                        {
                                            seddelNavnet = navnSjuPåSeddel;
                                            funnet = true;
                                        }
                                        else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnÅttePåSeddel) != null && !funnet)
                                        {
                                            seddelNavnet = navnÅttePåSeddel;
                                            funnet = true;
                                        }
                                        else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnNiPåSeddel) != null && !funnet)
                                        {
                                            seddelNavnet = navnNiPåSeddel;
                                            funnet = true;
                                        }
                                        else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnTiPåSeddel) != null && !funnet)
                                        {
                                            seddelNavnet = navnTiPåSeddel;
                                            funnet = true;
                                        }
                                        else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnEllevePåSeddel) != null && !funnet)
                                        {
                                            seddelNavnet = navnEllevePåSeddel;
                                            funnet = true;
                                        }
                                        else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnTolvPåSedde) != null && !funnet)
                                        {
                                            seddelNavnet = navnTolvPåSedde;
                                            funnet = true;
                                        }

                                        if (seddelNavnet == "")
                                        {
                                        }
                                        else
                                        {
                                            tempStemmetallEksKandidat = kandidatLavestStemmetall.stemmetall - 1;

                                            VaraSTV oppdaterKandidat = gjenståendeKandidater.FirstOrDefault(b => b.navn == seddelNavnet);
                                            double tempStemmetallet = oppdaterKandidat.stemmetall + 1;
                                            oppdaterKandidat.stemmetall = tempStemmetallet;

                                            kandidatLavestStemmetall.stemmetall = tempStemmetallEksKandidat;

                                            Paragraph overførtStemmerP = new Paragraph(" " + navnEkskludert + " sin stemme overføres til " + seddelNavnet + ".");
                                            cell.AddElement(overførtStemmerP);

                                        }
                                    }
                                }

                            }//Else-if fant ikke overskudd, eksluderte en av de med laveste stemmetall
                        }

                        //Sjekke om noen ble valgt etter at stemmer ble eksludert og overført
                        if (kvoteringsvalg == true)
                        {
                            for (int i = 0; i < gjenståendeKandidater.Count(); i++)
                            {
                                if (gjenståendeKandidater[i].stemmetall >= valgtall)
                                {
                                    if (valgteKandidater.Count() < antallRepresentanter)
                                    {
                                        if (gjenståendeKandidater[i].klasse == klasse1)
                                        {
                                            if (antallLedigeKvote1 > 0)
                                            {
                                                valgteKandidater.Add(gjenståendeKandidater[i]);
                                                hjelpValgteKandidater.Add(gjenståendeKandidater[i]);
                                                noenValgt = true;
                                                antallLedigeKvote1--;
                                                antallLedigeplasser--;
                                            }
                                        }
                                        else if (gjenståendeKandidater[i].klasse == klasse2)
                                        {
                                            if (antallLedigeKvote2 > 0)
                                            {
                                                valgteKandidater.Add(gjenståendeKandidater[i]);
                                                hjelpValgteKandidater.Add(gjenståendeKandidater[i]);
                                                noenValgt = true;
                                                antallLedigeKvote2--;
                                                antallLedigeplasser--;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < gjenståendeKandidater.Count(); i++)
                            {
                                if (gjenståendeKandidater[i].stemmetall >= valgtall)
                                {
                                    if (valgteKandidater.Count() < antallRepresentanter)
                                    {
                                        valgteKandidater.Add(gjenståendeKandidater[i]);
                                        hjelpValgteKandidater.Add(gjenståendeKandidater[i]);
                                        noenValgt = true;
                                        antallLedigeplasser--;
                                    }
                                }
                            }
                        }

                        //Slette de eventuelle valgte kandidatene som ble valgt fra gjenstående
                        for (int i = 0; i < valgteKandidater.Count(); i++)
                        {
                            for (int k = 0; k < gjenståendeKandidater.Count(); k++)
                            {
                                if (valgteKandidater[i].navn == gjenståendeKandidater[k].navn)
                                {
                                    gjenståendeKandidater.Remove(gjenståendeKandidater[k]);
                                }
                            }
                        }
                        //Sletter om det fortsatt er noen som har null stemmer 
                        for (int i = 0; i < gjenståendeKandidater.Count(); i++)
                        {
                            if (gjenståendeKandidater[i].stemmetall == 0)
                            {
                                gjenståendeKandidater.Remove(gjenståendeKandidater[i]);
                            }
                        }

                        //Clearer ekskluderte lista
                        for (int i = 0; i < ekskluderteKandidater.Count(); i++)
                        {
                            kandidaterEkskludert2.Add(ekskluderteKandidater[i]);
                        }

                        //Om noen ble valgt denne runden legg det inn her
                        if (noenValgt == true)
                        {
                            //Skal sette inn noe i cells her
                            cell.AddElement(new Paragraph(" "));
                            for (int i = 0; i < hjelpValgteKandidater.Count(); i++)
                            {
                                Paragraph valgtkandidat = new Paragraph("- Kandidat" + hjelpValgteKandidater[i].navn + " sitt stemmetall er " +
                                    hjelpValgteKandidater[i].stemmetall + " og blir dermed erklært valgt.");
                                cell.AddElement(valgtkandidat);
                            }
                        }
                        if (valgteKandidater.Count() == antallRepresentanter)
                        {
                            
                            cell.AddElement(new Paragraph("  "));
                            Paragraph nyttParagraf = new Paragraph("Antall faste valgrepresentanter som skal velges er fylt, valget avsluttes.");
                            cell.AddElement(nyttParagraf);
                            cell.PaddingBottom = 20;
                            cell.BorderWidthTop = 0;
                            cell.BorderWidthLeft = 0;
                            cell.BorderWidthRight = 0;
                            cell.AddElement(new Paragraph("  "));
                            table.AddCell(cell);
                            PdfPCell alleValgteCell = new PdfPCell();
                            Paragraph allevalgteP = new Paragraph("Valgte kandidater: ", boldFont);
                            alleValgteCell.AddElement(allevalgteP);
                            for (int i = 0; i < valgteKandidater.Count(); i++)
                            {
                                Paragraph leggTilValgtP = new Paragraph(valgteKandidater[i].navn);
                                alleValgteCell.AddElement(leggTilValgtP);
                            }
                            table.AddCell(alleValgteCell);
                            //Legger til stemmesedler for valget
                            string valgtKandidat = "";
                            string valgtKandidatNavn = "";
                            int sedlerTeller = sedlerPreferansevalg.Count();

                            for (int j = 0; j < sedlerTeller; j++)
                            {
                                int stemmeID = sedlerPreferansevalg[j].stemmeseddelID;
                                PdfPCell cell7 = new PdfPCell(new Phrase(" \n Stemmesedler med stemmeID \n " + stemmeID + "\n \n", tablefont3));
                                cell7.Colspan = 12;
                                cell7.HorizontalAlignment = 1;
                                table.AddCell(cell7);

                                valgtKandidat = "Kandidatvalg 1";
                                var b1 = new PdfPCell(new Paragraph(valgtKandidat, tablefont4));
                                b1.FixedHeight = 50f;
                                b1.Colspan = 4;
                                table.AddCell(b1);

                                if (sedlerPreferansevalg[j].kandidatnrEn != null)
                                {
                                    valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrEn;
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

                                if (sedlerPreferansevalg[j].kandidatnrTo != null)
                                {
                                    valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrTo;
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

                                if (sedlerPreferansevalg[j].kandidatnrTre != null)
                                {
                                    valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrTre;
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

                                if (sedlerPreferansevalg[j].kandidatnrFire != null)
                                {
                                    valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrFire;
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

                                if (sedlerPreferansevalg[j].kandidatnrFem != null)
                                {
                                    valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrFem;
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

                                if (sedlerPreferansevalg[j].kandidatnrSeks != null)
                                {
                                    valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrSeks;
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


                                if (sedlerPreferansevalg[j].kandidatnrSju != null)
                                {
                                    valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrSju;
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

                                if (sedlerPreferansevalg[j].kandidatnrÅtte != null)
                                {
                                    valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrÅtte;
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

                                if (sedlerPreferansevalg[j].kandidatnrNi != null)
                                {
                                    valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrNi;
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

                                if (sedlerPreferansevalg[j].kandidatnrTi != null)
                                {
                                    valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrTi;
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

                                if (sedlerPreferansevalg[j].kandidatnrElleve != null)
                                {
                                    valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrElleve;
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

                                if (sedlerPreferansevalg[j].kandidatnrTolv != null)
                                {
                                    valgtKandidatNavn = sedlerPreferansevalg[j].kandidatnrTolv;
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

                            doc.Add(table);
                            doc.Close();
                            return ms;
                        }
                        else if (valgteKandidater.Count() > 0 && hjelpValgteKandidater.Count() > 0)
                        {
                            cell.AddElement(new Paragraph("  "));
                            Paragraph nyttParagraf = new Paragraph("Det ble valgt " + hjelpValgteKandidater.Count() + " denne runden. Valget fortsetter.");
                            cell.AddElement(nyttParagraf);
                            cell.PaddingBottom = 20;
                            cell.BorderWidthTop = 0;
                            cell.BorderWidthLeft = 0;
                            cell.BorderWidthRight = 0;
                            table.AddCell(cell);
                        }
                        if (hjelpValgteKandidater.Count() == 0 && ekskluderteKandidater.Count() > 0)
                        {
                            cell.AddElement(new Paragraph("  "));
                            Paragraph nyttParagraf = new Paragraph("Det ble ekskludert " + ekskluderteKandidater.Count() + " denne runden, og ingen ble valgt så valget fortsetter.");
                            cell.AddElement(nyttParagraf);
                            cell.PaddingBottom = 20;
                            cell.BorderWidthTop = 0;
                            cell.BorderWidthLeft = 0;
                            cell.BorderWidthRight = 0;
                            table.AddCell(cell);
                        }
                        else if (hjelpValgteKandidater.Count() > 0 && ekskluderteKandidater.Count() > 0)
                        {
                            cell.AddElement(new Paragraph("  "));
                            Paragraph nyttParagraf = new Paragraph("Det ble valgt " + hjelpValgteKandidater.Count() + " denne runden, etter å ha ekskludert " + ekskluderteKandidater.Count() + " kandidater. Valget fortsetter.");
                            cell.AddElement(nyttParagraf);
                            cell.PaddingBottom = 20;
                            cell.BorderWidthTop = 0;
                            cell.BorderWidthLeft = 0;
                            cell.BorderWidthRight = 0;
                            table.AddCell(cell);
                        }
                        if (noenOverførtOverskudd == true && hjelpValgteKandidater.Count() > 0)
                        {
                            cell.AddElement(new Paragraph("  "));
                            Paragraph nyttParagraf = new Paragraph("Det ble valgt " + hjelpValgteKandidater.Count() + " denne runden, etter at det ble overført overskudd . Valget fortsetter.");
                            cell.AddElement(nyttParagraf);
                            cell.PaddingBottom = 20;
                            cell.BorderWidthTop = 0;
                            cell.BorderWidthLeft = 0;
                            cell.BorderWidthRight = 0;
                            table.AddCell(cell);
                        }
                        else if (noenOverførtOverskudd == true && hjelpValgteKandidater.Count() == 0)
                        {
                            cell.AddElement(new Paragraph("  "));
                            Paragraph nyttParagraf = new Paragraph("Det ble valgt ikke valgt noen representanter denne runden, etter at det ble overført overskudd. Valget fortsetter.");
                            cell.AddElement(nyttParagraf);
                            cell.PaddingBottom = 20;
                            cell.BorderWidthTop = 0;
                            cell.BorderWidthLeft = 0;
                            cell.BorderWidthRight = 0;
                            table.AddCell(cell);
                        }

                        ekskluderteKandidater.Clear();

                    }//While stopper her

                }

                return ms;
            }

        }
        public List<Stemmeseddel> preferansevalgsedler(int id)
        {
            using (var db = new BrukerContext())
            {
                var stemmeseddel = db.Stemmesedler.Select(k => new Stemmeseddel()
                {
                    valgtypeid = k.ValgtypeID,
                    kandidatnrEn = k.KandidatnrEn,
                    kandidatnrTo = k.KandidatnrTo,
                    kandidatnrTre = k.KandidatnrTre,
                    kandidatnrFire = k.KandidatnrFire,
                    kandidatnrFem = k.KandidatnrFem,
                    kandidatnrSeks = k.KandidatnrSeks,
                    kandidatnrSju = k.KandidatnrSju,
                    kandidatnrÅtte = k.KandidatnrÅtte,
                    kandidatnrNi = k.KandidatnrNi,
                    kandidatnrTi = k.KandidatnrTi,
                    kandidatnrElleve = k.KandidatnrElleve,
                    kandidatnrTolv = k.KandidatnrTolv,
                    stemmeseddelID = k.StemmeseddelID
                }).Where(k => k.valgtypeid == id).ToList();

                //For å få ut 'tom' istedenfor Velg kandidat
                List<Stemmeseddel> returStemmesedler = new List<Stemmeseddel>();
                for (int i = 0; i < stemmeseddel.Count(); i++)
                {
                    returStemmesedler.Add(stemmeseddel[i]);
                }
                for (int i = 0; i < returStemmesedler.Count(); i++)
                {
                    Stemmeseddel seddel = returStemmesedler[i];
                    if (seddel.kandidatnrEn == "Velg kandidat")
                    {
                        seddel.kandidatnrEn = "-";
                    }
                    if (seddel.kandidatnrTo == "Velg kandidat")
                    {
                        seddel.kandidatnrTo = "-";
                    }
                    if (seddel.kandidatnrTre == "Velg kandidat")
                    {
                        seddel.kandidatnrTre = "-";
                    }
                    if (seddel.kandidatnrFire == "Velg kandidat")
                    {
                        seddel.kandidatnrFire = "-";
                    }
                    if (seddel.kandidatnrFem == "Velg kandidat")
                    {
                        seddel.kandidatnrFem = "-";
                    }
                    if (seddel.kandidatnrSeks == "Velg kandidat")
                    {
                        seddel.kandidatnrSeks = "-";
                    }
                    if (seddel.kandidatnrSju == "Velg kandidat")
                    {
                        seddel.kandidatnrSju = "-";
                    }
                    if (seddel.kandidatnrÅtte == "Velg kandidat")
                    {
                        seddel.kandidatnrÅtte = "-";
                    }
                    if (seddel.kandidatnrNi == "Velg kandidat")
                    {
                        seddel.kandidatnrNi = "-";
                    }
                    if (seddel.kandidatnrTi == "Velg kandidat")
                    {
                        seddel.kandidatnrTi = "-";
                    }
                    if (seddel.kandidatnrElleve == "Velg kandidat")
                    {
                        seddel.kandidatnrElleve = "";
                    }
                    if (seddel.kandidatnrTolv == "Velg kandidat")
                    {
                        seddel.kandidatnrTolv = "-";
                    }
                }

                return returStemmesedler;
            }
        }
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
            List<KandidatSTV> valgteKandidater = ResultatPreferansevalg(valg.valgtypeid, preferansevalg.KvoteKlasseEn, preferansevalg.KvoteKlasseTo,
                                                 preferansevalg.KlasseEnProsent, preferansevalg.KlasseToProsent);

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
            double divider = (double)antallStemmer / (double)antallRepresentanter;
            //double divider = antallStemmer / antallRepresentanter;
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
        public List<KandidatSTV> ResultatPreferansevalg(int valgtypeid, string klasse1, string klasse2, int prosent1, int prosent2)
        {
            var db = new BrukerContext();
            BeregnStemmetallFørsteRunde(valgtypeid); //Beregner stemmetallene og sorterer de for første runde
            double valgtall = BeregnValgtall(valgtypeid);
            var preferansevalg = db.PreferanseValg.FirstOrDefault(p => p.ValgtypeID == valgtypeid);
            int antallRepresentanter = preferansevalg.AntallRepresentanter;
            int antallLedigeplasser = antallRepresentanter;
            int runde = 1;
            int høyesteEkskluderes = 0;
            double stemmetallEkskludert = 0;
            double totaltOverskudd = 0;
            double stemmetallOgOverskudd = 0;
            bool avsluttValg = false;
            bool fortsett = true;
            bool ikkeEkskluderFler = false;

            List<KandidatSTV> listeAvKandidater = hentKandidatlisteMedID(valgtypeid);
            List<KandidatSTV> valgteKandidater = new List<KandidatSTV>();
            List<KandidatSTV> gjenståendeKandidater = new List<KandidatSTV>();
            List<KandidatSTV> ekskluderteKandidater = new List<KandidatSTV>();
            List<KandidatSTV> kandidaterEkskludert2 = new List<KandidatSTV>();
            List<OverførtTilSeddel> overførteOverskudd = new List<OverførtTilSeddel>();

            //Kvotering
            bool kvoteringsvalg = false;
            double tempMinsteKvoteKlasse1 = (double)prosent1 * antallRepresentanter / 100;
            int minsteKvoteKlasse1 = (int)Math.Ceiling(tempMinsteKvoteKlasse1);
            int antallTilMinsteFylt1 = minsteKvoteKlasse1;

            double tempMinsteKvoteKlasse2 = (double)prosent2 * antallRepresentanter / 100;
            int minsteKvoteKlasse2 = (int)Math.Ceiling(tempMinsteKvoteKlasse2);
            int antallTilMinsteFylt2 = minsteKvoteKlasse2;

            int maksKvoteKlasse1 = antallRepresentanter - minsteKvoteKlasse2;
            int maksKvoteKlasse2 = antallRepresentanter - minsteKvoteKlasse1;
            int antallLedigeKvote1 = maksKvoteKlasse1;
            int antallLedigeKvote2 = maksKvoteKlasse2;

            if (klasse1 != "tom" && klasse2 != "tom" && klasse1 != "Klasse" && klasse2 != "Klasse" && klasse1 != "Choose candidate" && klasse2 != "Choose candidate")
            {
                kvoteringsvalg = true;
            }

            //Om noen erklært valgt legge til i Valgte kandidater liste ellers i gjenstående
            for (int i = 0; i < listeAvKandidater.Count(); i++)
            {
                if (listeAvKandidater[i].stemmetall >= valgtall)
                {
                    //Kvotering
                    if (kvoteringsvalg == true)
                    {
                        if (listeAvKandidater[i].klasse == klasse1 && antallLedigeKvote1 > 0)
                        {
                            valgteKandidater.Add(listeAvKandidater[i]);
                            antallTilMinsteFylt1--;
                            antallLedigeKvote1--;
                            antallLedigeplasser--;
                        }
                        else if (listeAvKandidater[i].klasse == klasse2 && antallLedigeKvote2 > 0)
                        {
                            valgteKandidater.Add(listeAvKandidater[i]);
                            antallTilMinsteFylt2--;
                            antallLedigeKvote2--;
                            antallLedigeplasser--;
                        }
                    }
                    else
                    {
                        //Om ikke kvotering
                        valgteKandidater.Add(listeAvKandidater[i]);
                        antallLedigeplasser--;
                    }
                }
                else
                {
                    gjenståendeKandidater.Add(listeAvKandidater[i]);
                }
            }

            //Om ingen ble valgt første runde, ekskludere alle med null
            if (valgteKandidater.Count() == 0)
            {
                //Kvotering - Funker 100%
                if (kvoteringsvalg == true)
                {
                    //Dette kjøres om det er kvotering
                    List<KandidatSTV> hjelpelistegjenståendeNull = new List<KandidatSTV>();
                    for (int i = 0; i < gjenståendeKandidater.Count(); i++)
                    {
                        if (gjenståendeKandidater[i].stemmetall > 0)
                        {
                            hjelpelistegjenståendeNull.Add(gjenståendeKandidater[i]);
                        }
                        else if (gjenståendeKandidater[i].stemmetall == 0)//Sjekker om jeg stryker en kandidat, vil det være mulig å fylle opp minstekvoten
                        {
                            if (gjenståendeKandidater[i].klasse == klasse1)
                            {
                                int tempTeller = 0;
                                int tempTellerSjekkOppMot = antallLedigeKvote1 - 1;
                                //Sjekker hvor mange av den klasse det er igjen
                                for (int k = 0; k < gjenståendeKandidater.Count(); k++)
                                {
                                    if (gjenståendeKandidater[k].klasse == klasse1)
                                    {
                                        tempTeller++;
                                    }
                                }
                                if (tempTeller <= antallTilMinsteFylt1)
                                {
                                    hjelpelistegjenståendeNull.Add(gjenståendeKandidater[i]);
                                }

                            }
                            else if (gjenståendeKandidater[i].klasse == klasse2)
                            {
                                int tempTeller = 0;
                                int tempTellerSjekkOppMot = antallLedigeKvote2 - 1;
                                //Sjekker hvor mange av den klasse det er igjen
                                for (int k = 0; k < gjenståendeKandidater.Count(); k++)
                                {
                                    if (gjenståendeKandidater[k].klasse == klasse2)
                                    {
                                        tempTeller++;
                                    }
                                }
                                if (tempTeller <= antallTilMinsteFylt2)
                                {
                                    hjelpelistegjenståendeNull.Add(gjenståendeKandidater[i]);
                                }
                            }
                        }
                    }
                    gjenståendeKandidater.Clear();
                    gjenståendeKandidater = hjelpelistegjenståendeNull;
                }
                else
                {
                    //Dette kjøres om det ikke er kvotering
                    List<KandidatSTV> hjelpelistegjenståendeNull = new List<KandidatSTV>();
                    for (int i = 0; i < gjenståendeKandidater.Count(); i++)
                    {
                        if (gjenståendeKandidater[i].stemmetall > 0)
                        {
                            hjelpelistegjenståendeNull.Add(gjenståendeKandidater[i]);
                        }
                    }
                    gjenståendeKandidater.Clear();
                    gjenståendeKandidater = hjelpelistegjenståendeNull;
                }
            }

            //Antall ledige plasser er fylt ut
            if (valgteKandidater.Count() == antallRepresentanter)
            {
                leggTilValgteKandidater(valgteKandidater);
                return valgteKandidater;
            }

            //Starter senere runder --KK
            while (valgteKandidater.Count() <= antallRepresentanter && avsluttValg == false)
            {
                //Øk rundenummer
                runde++;
                ikkeEkskluderFler = false;
                fortsett = true;

                //Om gjenstående kandidater er mindre eller lik antall ledige plasser
                if (kvoteringsvalg == true)
                {
                    if (gjenståendeKandidater.Count() <= antallLedigeplasser)
                    {
                        if (gjenståendeKandidater.Count() <= antallLedigeplasser)
                        {
                            //Alle gjenstående kandidater blir valgt
                            for (int i = 0; i < gjenståendeKandidater.Count(); i++)
                            {
                                if (gjenståendeKandidater[i].klasse == klasse1 && valgteKandidater.Count() < antallRepresentanter)
                                {
                                    if (antallLedigeKvote1 > 0)
                                    {
                                        valgteKandidater.Add(gjenståendeKandidater[i]);
                                        antallLedigeKvote1--;
                                        antallLedigeplasser--;
                                    }
                                }
                                else if (gjenståendeKandidater[i].klasse == klasse2)
                                {
                                    if (antallLedigeKvote2 > 0 && valgteKandidater.Count() < antallRepresentanter)
                                    {
                                        valgteKandidater.Add(gjenståendeKandidater[i]);
                                        antallLedigeKvote2--;
                                        antallLedigeplasser--;
                                    }
                                }
                            }
                            avsluttValg = true;
                            leggTilValgteKandidater(valgteKandidater);
                            return valgteKandidater;
                        }
                    }
                }
                else
                {
                    if (gjenståendeKandidater.Count() <= antallLedigeplasser)
                    {
                        if (gjenståendeKandidater.Count() <= antallLedigeplasser)
                        {
                            //Alle gjenstående kandidater blir valgt
                            for (int i = 0; i < gjenståendeKandidater.Count(); i++)
                            {
                                valgteKandidater.Add(gjenståendeKandidater[i]);
                                antallLedigeplasser--;
                            }
                            avsluttValg = true;
                            leggTilValgteKandidater(valgteKandidater);
                            return valgteKandidater;
                        }
                    }
                }

                //Kvotering -- Om en av klassene har fylt opp maksKvote, så ekskluder resten av den klassen fra gjenstående
                if (valgteKandidater.Count() > 0)
                {
                    //Sjekker først for klasse 1
                    int tellerAntallKlasseEn = 0;
                    for (int i = 0; i < valgteKandidater.Count(); i++)
                    {
                        if (valgteKandidater[i].klasse == klasse1)
                        {
                            tellerAntallKlasseEn++;
                        }
                    }
                    if (tellerAntallKlasseEn >= maksKvoteKlasse1)
                    {
                        for (int k = 0; k < gjenståendeKandidater.Count(); k++)
                        {
                            if (gjenståendeKandidater[k].klasse == klasse1)
                            {
                                ekskluderteKandidater.Add(gjenståendeKandidater[k]);
                                ikkeEkskluderFler = false;
                                fortsett = false;
                            }
                        }
                        for (int i = 0; i < ekskluderteKandidater.Count(); i++)
                        {
                            for (int j = 0; j < gjenståendeKandidater.Count(); j++)
                            {
                                if (ekskluderteKandidater[i].klasse == gjenståendeKandidater[j].klasse)
                                {
                                    gjenståendeKandidater.Remove(gjenståendeKandidater[j]);
                                }
                            }
                        }
                    }

                    //Sjekker for klasse2
                    int tellerAntallKlasseTo = 0;
                    for (int i = 0; i < valgteKandidater.Count(); i++)
                    {
                        if (valgteKandidater[i].klasse == klasse2)
                        {
                            tellerAntallKlasseTo++;
                        }
                    }
                    if (tellerAntallKlasseTo >= maksKvoteKlasse2)
                    {
                        for (int k = 0; k < gjenståendeKandidater.Count(); k++)
                        {
                            if (gjenståendeKandidater[k].klasse == klasse2)
                            {
                                ekskluderteKandidater.Add(gjenståendeKandidater[k]);
                                ikkeEkskluderFler = true;
                                fortsett = false;
                            }
                        }
                        for (int i = 0; i < ekskluderteKandidater.Count(); i++)
                        {
                            for (int j = 0; j < gjenståendeKandidater.Count(); j++)
                            {
                                if (ekskluderteKandidater[i].klasse == gjenståendeKandidater[j].klasse)
                                {
                                    gjenståendeKandidater.Remove(gjenståendeKandidater[j]);
                                }
                            }
                        }
                    }
                }

                //Om antall erklærte valgte kandidater er fylt opp
                if (valgteKandidater.Count() == antallRepresentanter)
                {
                    leggTilValgteKandidater(valgteKandidater);
                    return valgteKandidater;
                }


                //--------------------------------UNDER FINNE UT HVEM SOM KAN EKSKLUDERES-----------------------------------
                if (ikkeEkskluderFler == false)
                {
                    //Finne ut om noen kan ekskluderes
                    totaltOverskudd = 0;
                    stemmetallOgOverskudd = 0;

                    //Finner laveste stemmetall
                    if (gjenståendeKandidater.Count() > 0)
                    {
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
                            double tempStemmetall = gjenståendeKandidater[i].stemmetall;
                            if (tempStemmetall < stemmetallEkskludert)
                            {
                                if (tempStemmetall != 0)
                                {
                                    stemmetallEkskludert = tempStemmetall;
                                }

                            }
                        }
                    }//Funnet laveste stemmetall

                    //Om noen har overskudd legge til for å finne hvem som kan ekskluderes
                    if (valgteKandidater.Count() > 0)
                    {
                        for (int i = 0; i < valgteKandidater.Count(); i++)
                        {
                            double overskuddKandidat = 0;
                            double tempValgtall = valgtall;
                            overskuddKandidat = valgteKandidater[i].stemmetall - tempValgtall;
                            if (overskuddKandidat > 0)
                            {
                                totaltOverskudd += overskuddKandidat;
                            }
                        }
                    }

                    //Viktige variabler og lister for Ekskludering
                    høyesteEkskluderes = gjenståendeKandidater.Count() - antallLedigeplasser;
                    List<KandidatSTV> potensiellEkskluderte = new List<KandidatSTV>();
                    stemmetallOgOverskudd = totaltOverskudd;

                    //Sjekker hvem som potensielt kan ekskluderes
                    if (kvoteringsvalg == true)
                    {
                        for (int i = 0; i < gjenståendeKandidater.Count(); i++)
                        {
                            bool sjekkPlassEtterEks = false;
                            if (gjenståendeKandidater[i].stemmetall == stemmetallEkskludert && høyesteEkskluderes > potensiellEkskluderte.Count())
                            {
                                if (gjenståendeKandidater[i].klasse == klasse1)
                                {
                                    int tellerAntallKlasse1IGj = 0;
                                    for (int k = 0; k < gjenståendeKandidater.Count(); k++)
                                    {
                                        if (gjenståendeKandidater[k].klasse == klasse1)
                                        {
                                            tellerAntallKlasse1IGj++;
                                        }
                                    }
                                    if (tellerAntallKlasse1IGj > antallTilMinsteFylt1)
                                    {
                                        sjekkPlassEtterEks = true;
                                    }
                                }
                                else if (gjenståendeKandidater[i].klasse == klasse2)
                                {
                                    int tellerAntallKlasse2IGj = 0;
                                    for (int k = 0; k < gjenståendeKandidater.Count(); k++)
                                    {
                                        if (gjenståendeKandidater[k].klasse == klasse2)
                                        {
                                            tellerAntallKlasse2IGj++;
                                        }
                                    }
                                    if (tellerAntallKlasse2IGj > antallTilMinsteFylt2)
                                    {
                                        sjekkPlassEtterEks = true;
                                    }
                                }

                            }
                            if (sjekkPlassEtterEks == true)
                            {
                                potensiellEkskluderte.Add(gjenståendeKandidater[i]);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < gjenståendeKandidater.Count(); i++)
                        {
                            if (gjenståendeKandidater[i].stemmetall == stemmetallEkskludert && høyesteEkskluderes > potensiellEkskluderte.Count())
                            {
                                potensiellEkskluderte.Add(gjenståendeKandidater[i]);
                            }
                        }
                    }


                    if (potensiellEkskluderte.Count() > 0)
                    {
                        for (int i = 0; i < potensiellEkskluderte.Count(); i++)
                        {
                            bool hjelpEkskludering = true;
                            KandidatSTV potensiellEkskluderes = potensiellEkskluderte[i];
                            stemmetallOgOverskudd += potensiellEkskluderes.stemmetall;
                            for (int k = 0; k < gjenståendeKandidater.Count(); k++)
                            {
                                if (gjenståendeKandidater[k].stemmetall > stemmetallEkskludert && gjenståendeKandidater[k].stemmetall <= stemmetallOgOverskudd)
                                {
                                    hjelpEkskludering = false;
                                }
                            }
                            if (hjelpEkskludering == true)
                            {
                                ekskluderteKandidater.Add(potensiellEkskluderes);
                                fortsett = false;
                            }
                        }
                        //Fjerne ekskludert kandidat fra gjenstående liste
                        for (int i = 0; i < ekskluderteKandidater.Count(); i++)
                        {
                            for (int k = 0; k < gjenståendeKandidater.Count(); k++)
                            {
                                if (ekskluderteKandidater[i].navn == gjenståendeKandidater[k].navn)
                                {
                                    gjenståendeKandidater.Remove(ekskluderteKandidater[i]);
                                }
                            }
                        }


                        //Om antall gjenstående kandidater er mindre enn eller lik antall ledige plasser - avslutt valg
                        if (kvoteringsvalg == true)
                        {
                            if (gjenståendeKandidater.Count() <= antallLedigeplasser)
                            {
                                if (gjenståendeKandidater.Count() <= antallLedigeplasser)
                                {
                                    //Alle gjenstående kandidater blir valgt
                                    for (int i = 0; i < gjenståendeKandidater.Count(); i++)
                                    {
                                        if (gjenståendeKandidater[i].klasse == klasse1 && valgteKandidater.Count() < antallRepresentanter)
                                        {
                                            if (antallLedigeKvote1 > 0)
                                            {
                                                valgteKandidater.Add(gjenståendeKandidater[i]);
                                                antallLedigeKvote1--;
                                                antallLedigeplasser--;
                                            }
                                        }
                                        else if (gjenståendeKandidater[i].klasse == klasse2 && valgteKandidater.Count() < antallRepresentanter)
                                        {
                                            if (antallLedigeKvote2 > 0)
                                            {
                                                valgteKandidater.Add(gjenståendeKandidater[i]);
                                                antallLedigeKvote2--;
                                                antallLedigeplasser--;
                                            }
                                        }
                                    }
                                    avsluttValg = true;
                                    leggTilValgteKandidater(valgteKandidater);
                                    return valgteKandidater;
                                }
                            }
                        }
                        else
                        {
                            if (gjenståendeKandidater.Count() <= antallLedigeplasser)
                            {
                                if (gjenståendeKandidater.Count() <= antallLedigeplasser)
                                {
                                    //Alle gjenstående kandidater blir valgt
                                    for (int i = 0; i < gjenståendeKandidater.Count(); i++) //Må sjekke denne tror den blir feil
                                    {
                                        valgteKandidater.Add(gjenståendeKandidater[i]);
                                        antallLedigeplasser--;
                                    }
                                    avsluttValg = true;
                                    leggTilValgteKandidater(valgteKandidater);
                                    return valgteKandidater;
                                }
                            }
                        }
                    }
                }

                //---------------------------Eksludert sine stemmer overføres---------------------------------------
                if (ekskluderteKandidater.Count() > 0)
                {
                    //Inne her skal jeg overføre stemmeseddel for den som eventuelt har blitt ekskludert og sette fortsett til false
                    for (int i = 0; i < ekskluderteKandidater.Count(); i++)
                    {
                        KandidatSTV kandidatEkskluderes = ekskluderteKandidater[i];
                        string navnEkskludert = ekskluderteKandidater[i].navn;
                        double tempStemmetallEksKandidat = ekskluderteKandidater[i].stemmetall - 1;
                        List<Stemmeseddel_db> listeEkskludert = db.Stemmesedler.Where(b => b.KandidatnrEn == navnEkskludert && b.ValgtypeID == valgtypeid).ToList();

                        //Sjekke om ekskludert kandidat har fått overført overskudd tidligere som må videreføres
                        if (overførteOverskudd.Count() > 0)
                        {
                            for (int ind = 0; ind < overførteOverskudd.Count(); ind++)
                            {
                                OverførtTilSeddel stemme = overførteOverskudd[ind];
                                if (navnEkskludert == stemme.overførtTil)
                                {
                                    string seddelNavnet = "";
                                    string navnToPåSeddel = stemme.kandidatnrTo;
                                    string navnTrePåSeddel = stemme.kandidatnrTre;
                                    string navnFirePåSeddel = stemme.kandidatnrFire;
                                    string navnFemPåSeddel = stemme.kandidatnrFem;
                                    string navnSeksPåSeddel = stemme.kandidatnrSeks;
                                    string navnSjuPåSeddel = stemme.kandidatnrSju;
                                    string navnÅttePåSeddel = stemme.kandidatnrÅtte;
                                    string navnNiPåSeddel = stemme.kandidatnrNi;
                                    string navnTiPåSeddel = stemme.kandidatnrTi;
                                    string navnEllevePåSeddel = stemme.kandidatnrElleve;
                                    string navnTolvPåSedde = stemme.kandidatnrTolv;
                                    bool funnet = false;

                                    if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnToPåSeddel) != null && stemme.overførtTil != navnToPåSeddel && !funnet)
                                    {
                                        seddelNavnet = navnToPåSeddel;
                                        funnet = true;
                                    }
                                    if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnTrePåSeddel) != null && stemme.overførtTil != navnTrePåSeddel && !funnet)
                                    {
                                        seddelNavnet = navnTrePåSeddel;
                                        funnet = true;
                                    }
                                    if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnFirePåSeddel) != null && stemme.overførtTil != navnFirePåSeddel && !funnet)
                                    {
                                        seddelNavnet = navnFirePåSeddel;
                                        funnet = true;
                                    }
                                    if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnFemPåSeddel) != null && stemme.overførtTil != navnFemPåSeddel && !funnet)
                                    {
                                        seddelNavnet = navnFemPåSeddel;
                                        funnet = true;
                                    }
                                    if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnSeksPåSeddel) != null && stemme.overførtTil != navnSeksPåSeddel && !funnet)
                                    {
                                        seddelNavnet = navnSeksPåSeddel;
                                        funnet = true;
                                    }
                                    if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnSjuPåSeddel) != null && stemme.overførtTil != navnSjuPåSeddel && !funnet)
                                    {
                                        seddelNavnet = navnSjuPåSeddel;
                                        funnet = true;
                                    }
                                    if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnÅttePåSeddel) != null && stemme.overførtTil != navnÅttePåSeddel && !funnet)
                                    {
                                        seddelNavnet = navnÅttePåSeddel;
                                        funnet = true;
                                    }
                                    if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnNiPåSeddel) != null && stemme.overførtTil != navnNiPåSeddel && !funnet)
                                    {
                                        seddelNavnet = navnNiPåSeddel;
                                        funnet = true;
                                    }
                                    if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnTiPåSeddel) != null && stemme.overførtTil != navnTiPåSeddel && !funnet)
                                    {
                                        seddelNavnet = navnTiPåSeddel;
                                        funnet = true;
                                    }
                                    if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnEllevePåSeddel) != null && stemme.overførtTil != navnEllevePåSeddel && !funnet)
                                    {
                                        seddelNavnet = navnEllevePåSeddel;
                                        funnet = true;
                                    }
                                    if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnTolvPåSedde) != null && stemme.overførtTil != navnTolvPåSedde && !funnet)
                                    {
                                        seddelNavnet = navnTolvPåSedde;
                                        funnet = true;
                                    }

                                    if (seddelNavnet != "")
                                    {
                                        double tempStemmetallValgtKandidat = kandidatEkskluderes.stemmetall - stemme.overførtSum;

                                        KandidatSTV oppdaterKandidat = gjenståendeKandidater.FirstOrDefault(b => b.navn == seddelNavnet);
                                        double tempStemmetallet = oppdaterKandidat.stemmetall + stemme.overførtSum;
                                        oppdaterKandidat.stemmetall = tempStemmetallet;

                                        KandidatListeSTV endreKandidat = db.KandidaterSTV.FirstOrDefault(b => b.Navn == seddelNavnet && b.ValgtypeID == valgtypeid);
                                        endreKandidat.KandidatID = oppdaterKandidat.kandidatListeID;
                                        endreKandidat.Navn = oppdaterKandidat.navn;
                                        endreKandidat.Stemmetall = tempStemmetallet;

                                        KandidatListeSTV endreValgtKandidat = db.KandidaterSTV.FirstOrDefault(b => b.Navn == navnEkskludert && b.ValgtypeID == valgtypeid);
                                        endreValgtKandidat.KandidatID = kandidatEkskluderes.kandidatListeID;
                                        endreValgtKandidat.Navn = navnEkskludert;
                                        endreValgtKandidat.Stemmetall = tempStemmetallValgtKandidat;

                                        kandidatEkskluderes.stemmetall = tempStemmetallValgtKandidat;

                                        bool sjekk = false;

                                        OverførtTilSeddel nySeddel = new OverførtTilSeddel()
                                        {
                                            seddelID = stemme.seddelID,
                                            kandidatnrEn = stemme.kandidatnrEn,
                                            kandidatnrTo = stemme.kandidatnrTo,
                                            kandidatnrTre = stemme.kandidatnrTre,
                                            kandidatnrFire = stemme.kandidatnrFire,
                                            kandidatnrFem = stemme.kandidatnrFem,
                                            kandidatnrSeks = stemme.kandidatnrSeks,
                                            kandidatnrSju = stemme.kandidatnrSju,
                                            kandidatnrÅtte = stemme.kandidatnrÅtte,
                                            kandidatnrNi = stemme.kandidatnrNi,
                                            kandidatnrTi = stemme.kandidatnrTi,
                                            kandidatnrElleve = stemme.kandidatnrElleve,
                                            kandidatnrTolv = stemme.kandidatnrTolv,
                                            overførtFra = navnEkskludert,
                                            overførtTil = seddelNavnet,
                                            overførtSum = stemme.overførtSum,
                                            rundenr = runde,
                                            bleValgt = false
                                        };
                                        overførteOverskudd.Add(nySeddel);

                                        if (oppdaterKandidat.stemmetall >= valgtall)
                                        {
                                            for (int j = 0; j < overførteOverskudd.Count(); j++)
                                            {
                                                if (seddelNavnet == overførteOverskudd[j].overførtTil && overførteOverskudd[j].rundenr == runde)
                                                {
                                                    overførteOverskudd[j].bleValgt = true;
                                                }
                                            }
                                        }
                                        db.SaveChanges();
                                    }//if-seddelnavnet!=''
                                }
                            }
                        }//If ekskludert kandidat hadde fått overført overskudd tidligere slutt------------------------------------

                        //De stemmesedlene med vekt 1.0, ligger i stemmeseddel databasen, ekskludert stemme overføres
                        for (int j = 0; j < listeEkskludert.Count(); j++)
                        {
                            string seddelNavnet = "";
                            string navnToPåSeddel = listeEkskludert[j].KandidatnrTo;
                            string navnTrePåSeddel = listeEkskludert[j].KandidatnrTre;
                            string navnFirePåSeddel = listeEkskludert[j].KandidatnrFire;
                            string navnFemPåSeddel = listeEkskludert[j].KandidatnrFem;
                            string navnSeksPåSeddel = listeEkskludert[j].KandidatnrSeks;
                            string navnSjuPåSeddel = listeEkskludert[j].KandidatnrSju;
                            string navnÅttePåSeddel = listeEkskludert[j].KandidatnrÅtte;
                            string navnNiPåSeddel = listeEkskludert[j].KandidatnrNi;
                            string navnTiPåSeddel = listeEkskludert[j].KandidatnrTi;
                            string navnEllevePåSeddel = listeEkskludert[j].KandidatnrElleve;
                            string navnTolvPåSedde = listeEkskludert[j].KandidatnrTolv;
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
                            else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnSeksPåSeddel) != null && !funnet)
                            {
                                seddelNavnet = navnSeksPåSeddel;
                                funnet = true;
                            }
                            else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnSjuPåSeddel) != null && !funnet)
                            {
                                seddelNavnet = navnSjuPåSeddel;
                                funnet = true;
                            }
                            else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnÅttePåSeddel) != null && !funnet)
                            {
                                seddelNavnet = navnÅttePåSeddel;
                                funnet = true;
                            }
                            else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnNiPåSeddel) != null && !funnet)
                            {
                                seddelNavnet = navnNiPåSeddel;
                                funnet = true;
                            }
                            else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnTiPåSeddel) != null && !funnet)
                            {
                                seddelNavnet = navnTiPåSeddel;
                                funnet = true;
                            }
                            else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnEllevePåSeddel) != null && !funnet)
                            {
                                seddelNavnet = navnEllevePåSeddel;
                                funnet = true;
                            }
                            else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnTolvPåSedde) != null && !funnet)
                            {
                                seddelNavnet = navnTolvPåSedde;
                                funnet = true;
                            }

                            if (seddelNavnet == "")
                            {
                            }
                            else
                            {
                                tempStemmetallEksKandidat = ekskluderteKandidater[i].stemmetall - 1;

                                KandidatSTV oppdaterKandidat = gjenståendeKandidater.FirstOrDefault(b => b.navn == seddelNavnet);
                                double tempStemmetallet = oppdaterKandidat.stemmetall + 1;
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


                                for (int k = 0; k < ekskluderteKandidater.Count(); k++)
                                {
                                    if (ekskluderteKandidater[k].navn == seddelNavnet)
                                    {
                                        ekskluderteKandidater.Remove(oppdaterKandidat);
                                    }
                                }
                                db.SaveChanges();
                            }
                        }

                        KandidatListeSTV ekskludereKandidatTilslutt = db.KandidaterSTV.FirstOrDefault(b => b.Navn == navnEkskludert && b.ValgtypeID == valgtypeid);
                        ekskludereKandidatTilslutt.KandidatID = ekskluderteKandidater[i].kandidatListeID;
                        ekskludereKandidatTilslutt.Navn = ekskluderteKandidater[i].navn;
                        ekskludereKandidatTilslutt.Stemmetall = 0;

                        ekskluderteKandidater[i].stemmetall = 0;
                    }
                    db.SaveChanges();
                }


                //---------------------------------EKLSUDERING OVERFØRING STEMMER FERDIG----------------------------------------

                //Sjekke om noen ble valgt etter at stemmer ble eksludert og overført
                if (kvoteringsvalg == true)
                {
                    for (int i = 0; i < gjenståendeKandidater.Count(); i++)
                    {
                        if (gjenståendeKandidater[i].stemmetall >= valgtall)
                        {
                            if (valgteKandidater.Count() < antallRepresentanter)
                            {
                                if (gjenståendeKandidater[i].klasse == klasse1)
                                {
                                    if (antallLedigeKvote1 > 0)
                                    {
                                        valgteKandidater.Add(gjenståendeKandidater[i]);
                                        antallLedigeKvote1--;
                                        antallLedigeplasser--;
                                    }
                                }
                                else if (gjenståendeKandidater[i].klasse == klasse2)
                                {
                                    if (antallLedigeKvote2 > 0)
                                    {
                                        valgteKandidater.Add(gjenståendeKandidater[i]);
                                        antallLedigeKvote2--;
                                        antallLedigeplasser--;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < gjenståendeKandidater.Count(); i++)
                    {
                        if (gjenståendeKandidater[i].stemmetall >= valgtall)
                        {
                            if (valgteKandidater.Count() < antallRepresentanter)
                            {
                                valgteKandidater.Add(gjenståendeKandidater[i]);
                                antallLedigeplasser--;
                            }
                        }
                    }
                }

                //Slette de eventuelle valgte kandidatene som ble valgt fra gjenstående
                for (int i = 0; i < valgteKandidater.Count(); i++)
                {
                    for (int k = 0; k < gjenståendeKandidater.Count(); k++)
                    {
                        if (valgteKandidater[i].navn == gjenståendeKandidater[k].navn)
                        {
                            gjenståendeKandidater.Remove(gjenståendeKandidater[k]);
                        }
                    }
                }

                //-----------------------------------Overføre overskudd eller ekskludere en random--------------------------------

                //Under her ta en if-fortsett == true.. Så skal det overføres ingen ble ekskludert denne runden
                if (fortsett == true)
                {
                    //Inne her overføre eventuellt overskudd eller tving en ekskludering

                    //Sjekke om en eller flere har overskudd
                    int antallHarOverskudd = 0;
                    for (int i = 0; i < valgteKandidater.Count(); i++)
                    {
                        double overskudd = 0;
                        KandidatSTV valgtKandidat = valgteKandidater[i];
                        double tempValgtall = valgtall;
                        overskudd = valgtKandidat.stemmetall - tempValgtall;
                        if (overskudd > 0)
                        {
                            antallHarOverskudd++;
                        }
                    }

                    //Om den fant noen valgte kandidater med overskudd
                    if (antallHarOverskudd > 0)
                    {
                        //Finn det største overskuddet
                        double tempOverskudd = 0;
                        KandidatSTV kandidatOverføresFra = new KandidatSTV();
                        for (int i = 0; i < valgteKandidater.Count(); i++)
                        {
                            double overskudd = 0;
                            KandidatSTV valgtKandidat = valgteKandidater[i];
                            double tempValgtall = valgtall;
                            overskudd = valgtKandidat.stemmetall - tempValgtall;
                            if (overskudd > tempOverskudd)
                            {
                                tempOverskudd = overskudd;
                                kandidatOverføresFra = valgtKandidat; //Kandidat med største overskuddet
                            }
                        }

                        int antallAvKandidatIListe = 0;
                        for (int i = 0; i < overførteOverskudd.Count(); i++)
                        {
                            if (overførteOverskudd[i].overførtTil == kandidatOverføresFra.navn && overførteOverskudd[i].bleValgt == true)
                            {
                                antallAvKandidatIListe++;
                            }
                        }

                        //Om kandidat det overføres fra sitt stemmetall ble større etter å ha fått overført stemmer i senere runder og ikke i første
                        if (antallAvKandidatIListe > 0)
                        {
                            List<OverførtTilSeddel> overførteStemmer = new List<OverførtTilSeddel>();
                            for (int i = 0; i < overførteOverskudd.Count(); i++)
                            {
                                if (overførteOverskudd[i].overførtTil == kandidatOverføresFra.navn && overførteOverskudd[i].bleValgt == true)
                                {
                                    overførteStemmer.Add(overførteOverskudd[i]);
                                }
                            }

                            double overføresOverskudd = tempOverskudd / kandidatOverføresFra.stemmetall;
                            //Starter overføringen
                            for (int i = 0; i < overførteStemmer.Count(); i++)
                            {
                                OverførtTilSeddel stemme = overførteStemmer[i];
                                string seddelNavnet = "";
                                string navnToPåSeddel = stemme.kandidatnrTo;
                                string navnTrePåSeddel = stemme.kandidatnrTre;
                                string navnFirePåSeddel = stemme.kandidatnrFire;
                                string navnFemPåSeddel = stemme.kandidatnrFem;
                                string navnSeksPåSeddel = stemme.kandidatnrSeks;
                                string navnSjuPåSeddel = stemme.kandidatnrSju;
                                string navnÅttePåSeddel = stemme.kandidatnrÅtte;
                                string navnNiPåSeddel = stemme.kandidatnrNi;
                                string navnTiPåSeddel = stemme.kandidatnrTi;
                                string navnEllevePåSeddel = stemme.kandidatnrElleve;
                                string navnTolvPåSeddel = stemme.kandidatnrTolv;
                                bool funnet = false;

                                if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnToPåSeddel) != null && stemme.overførtTil != navnToPåSeddel && !funnet)
                                {
                                    seddelNavnet = navnToPåSeddel;
                                    funnet = true;
                                }
                                if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnTrePåSeddel) != null && stemme.overførtTil != navnTrePåSeddel && !funnet)
                                {
                                    seddelNavnet = navnTrePåSeddel;
                                    funnet = true;
                                }
                                if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnFirePåSeddel) != null && stemme.overførtTil != navnFirePåSeddel && !funnet)
                                {
                                    seddelNavnet = navnFirePåSeddel;
                                    funnet = true;
                                }
                                if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnFemPåSeddel) != null && stemme.overførtTil != navnFemPåSeddel && !funnet)
                                {
                                    seddelNavnet = navnFemPåSeddel;
                                    funnet = true;
                                }
                                if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnSeksPåSeddel) != null && stemme.overførtTil != navnSeksPåSeddel && !funnet)
                                {
                                    seddelNavnet = navnSeksPåSeddel;
                                    funnet = true;
                                }
                                if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnSjuPåSeddel) != null && stemme.overførtTil != navnSjuPåSeddel && !funnet)
                                {
                                    seddelNavnet = navnSjuPåSeddel;
                                    funnet = true;
                                }
                                if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnÅttePåSeddel) != null && stemme.overførtTil != navnÅttePåSeddel && !funnet)
                                {
                                    seddelNavnet = navnÅttePåSeddel;
                                    funnet = true;
                                }
                                if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnNiPåSeddel) != null && stemme.overførtTil != navnNiPåSeddel && !funnet)
                                {
                                    seddelNavnet = navnNiPåSeddel;
                                    funnet = true;
                                }
                                if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnTiPåSeddel) != null && stemme.overførtTil != navnTiPåSeddel && !funnet)
                                {
                                    seddelNavnet = navnTiPåSeddel;
                                    funnet = true;
                                }
                                if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnEllevePåSeddel) != null && stemme.overførtTil != navnEllevePåSeddel && !funnet)
                                {
                                    seddelNavnet = navnEllevePåSeddel;
                                    funnet = true;
                                }
                                if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnTolvPåSeddel) != null && stemme.overførtTil != navnTolvPåSeddel && !funnet)
                                {
                                    seddelNavnet = navnTolvPåSeddel;
                                    funnet = true;
                                }

                                if (seddelNavnet != "")
                                {
                                    double tempStemmetallValgtKandidat = kandidatOverføresFra.stemmetall - overføresOverskudd;

                                    KandidatSTV oppdaterKandidat = gjenståendeKandidater.FirstOrDefault(b => b.navn == seddelNavnet);
                                    double tempStemmetallet = oppdaterKandidat.stemmetall + overføresOverskudd;
                                    oppdaterKandidat.stemmetall = tempStemmetallet;

                                    KandidatListeSTV endreKandidat = db.KandidaterSTV.FirstOrDefault(b => b.Navn == seddelNavnet && b.ValgtypeID == valgtypeid);
                                    endreKandidat.KandidatID = oppdaterKandidat.kandidatListeID;
                                    endreKandidat.Navn = oppdaterKandidat.navn;
                                    endreKandidat.Stemmetall = tempStemmetallet;

                                    KandidatListeSTV endreValgtKandidat = db.KandidaterSTV.FirstOrDefault(b => b.Navn == kandidatOverføresFra.navn && b.ValgtypeID == valgtypeid);
                                    endreValgtKandidat.KandidatID = kandidatOverføresFra.kandidatListeID;
                                    endreValgtKandidat.Navn = kandidatOverføresFra.navn;
                                    endreValgtKandidat.Stemmetall = tempStemmetallValgtKandidat;

                                    kandidatOverføresFra.stemmetall = tempStemmetallValgtKandidat;

                                    bool sjekk = false;


                                    OverførtTilSeddel nySeddel = new OverførtTilSeddel()
                                    {
                                        seddelID = stemme.seddelID,
                                        kandidatnrEn = stemme.kandidatnrEn,
                                        kandidatnrTo = stemme.kandidatnrTo,
                                        kandidatnrTre = stemme.kandidatnrTre,
                                        kandidatnrFire = stemme.kandidatnrFire,
                                        kandidatnrFem = stemme.kandidatnrFem,
                                        kandidatnrSeks = stemme.kandidatnrSeks,
                                        kandidatnrSju = stemme.kandidatnrSju,
                                        kandidatnrÅtte = stemme.kandidatnrÅtte,
                                        kandidatnrNi = stemme.kandidatnrNi,
                                        kandidatnrTi = stemme.kandidatnrTi,
                                        kandidatnrElleve = stemme.kandidatnrElleve,
                                        kandidatnrTolv = stemme.kandidatnrTolv,
                                        overførtFra = kandidatOverføresFra.navn,
                                        overførtTil = seddelNavnet,
                                        overførtSum = overføresOverskudd,
                                        rundenr = runde,
                                        bleValgt = false
                                    };
                                    overførteOverskudd.Add(nySeddel);

                                    if (oppdaterKandidat.stemmetall >= valgtall)
                                    {
                                        for (int j = 0; j < overførteOverskudd.Count(); j++)
                                        {
                                            if (seddelNavnet == overførteOverskudd[j].overførtTil && overførteOverskudd[j].rundenr == runde)
                                            {
                                                overførteOverskudd[j].bleValgt = true;
                                            }
                                        }
                                    }
                                    db.SaveChanges();
                                }
                            }
                            kandidatOverføresFra.stemmetall = valgtall;

                        }
                        else
                        {
                            //DENNE DELEN ER HUNDRE PROSENT 
                            List<Stemmeseddel_db> overførStemmer = db.Stemmesedler.Where(s => s.KandidatnrEn == kandidatOverføresFra.navn && s.ValgtypeID == valgtypeid).ToList();
                            //Finne overskuddtallet som skal overføres til alle stemmesedlene
                            double overføresOverskudd = tempOverskudd / kandidatOverføresFra.stemmetall;

                            for (int i = 0; i < overførStemmer.Count(); i++)
                            {
                                Stemmeseddel_db stemme = overførStemmer[i];
                                string seddelNavnet = "";
                                string navnToPåSeddel = stemme.KandidatnrTo;
                                string navnTrePåSeddel = stemme.KandidatnrTre;
                                string navnFirePåSeddel = stemme.KandidatnrFire;
                                string navnFemPåSeddel = stemme.KandidatnrFem;
                                string navnSeksPåSeddel = stemme.KandidatnrSeks;
                                string navnSjuPåSeddel = stemme.KandidatnrSju;
                                string navnÅttePåSeddel = stemme.KandidatnrÅtte;
                                string navnNiPåSeddel = stemme.KandidatnrNi;
                                string navnTiPåSeddel = stemme.KandidatnrTi;
                                string navnEllevePåSeddel = stemme.KandidatnrElleve;
                                string navnTolvPåSeddel = stemme.KandidatnrTolv;
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
                                else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnSeksPåSeddel) != null && !funnet)
                                {
                                    seddelNavnet = navnSeksPåSeddel;
                                    funnet = true;
                                }
                                else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnSjuPåSeddel) != null && !funnet)
                                {
                                    seddelNavnet = navnSjuPåSeddel;
                                    funnet = true;
                                }
                                else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnÅttePåSeddel) != null && !funnet)
                                {
                                    seddelNavnet = navnÅttePåSeddel;
                                    funnet = true;
                                }
                                else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnNiPåSeddel) != null && !funnet)
                                {
                                    seddelNavnet = navnNiPåSeddel;
                                    funnet = true;
                                }
                                else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnTiPåSeddel) != null && !funnet)
                                {
                                    seddelNavnet = navnTiPåSeddel;
                                    funnet = true;
                                }
                                else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnEllevePåSeddel) != null && !funnet)
                                {
                                    seddelNavnet = navnEllevePåSeddel;
                                    funnet = true;
                                }
                                else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnTolvPåSeddel) != null && !funnet)
                                {
                                    seddelNavnet = navnTolvPåSeddel;
                                    funnet = true;
                                }

                                if (seddelNavnet != "")
                                {
                                    double tempStemmetallValgtKandidat = kandidatOverføresFra.stemmetall - overføresOverskudd;

                                    KandidatSTV oppdaterKandidat = gjenståendeKandidater.FirstOrDefault(b => b.navn == seddelNavnet);
                                    double tempStemmetallet = oppdaterKandidat.stemmetall + overføresOverskudd;
                                    oppdaterKandidat.stemmetall = tempStemmetallet;

                                    KandidatListeSTV endreKandidat = db.KandidaterSTV.FirstOrDefault(b => b.Navn == seddelNavnet && b.ValgtypeID == valgtypeid);
                                    endreKandidat.KandidatID = oppdaterKandidat.kandidatListeID;
                                    endreKandidat.Navn = oppdaterKandidat.navn;
                                    endreKandidat.Stemmetall = tempStemmetallet;

                                    KandidatListeSTV endreValgtKandidat = db.KandidaterSTV.FirstOrDefault(b => b.Navn == kandidatOverføresFra.navn && b.ValgtypeID == valgtypeid);
                                    endreValgtKandidat.KandidatID = kandidatOverføresFra.kandidatListeID;
                                    endreValgtKandidat.Navn = kandidatOverføresFra.navn;
                                    endreValgtKandidat.Stemmetall = tempStemmetallValgtKandidat;

                                    kandidatOverføresFra.stemmetall = tempStemmetallValgtKandidat;

                                    bool sjekk = false;


                                    OverførtTilSeddel nySeddel = new OverførtTilSeddel()
                                    {
                                        seddelID = stemme.StemmeseddelID,
                                        kandidatnrEn = stemme.KandidatnrEn,
                                        kandidatnrTo = stemme.KandidatnrTo,
                                        kandidatnrTre = stemme.KandidatnrTre,
                                        kandidatnrFire = stemme.KandidatnrFire,
                                        kandidatnrFem = stemme.KandidatnrFem,
                                        kandidatnrSeks = stemme.KandidatnrSeks,
                                        kandidatnrSju = stemme.KandidatnrSju,
                                        kandidatnrÅtte = stemme.KandidatnrÅtte,
                                        kandidatnrNi = stemme.KandidatnrNi,
                                        kandidatnrTi = stemme.KandidatnrTi,
                                        kandidatnrElleve = stemme.KandidatnrElleve,
                                        kandidatnrTolv = stemme.KandidatnrTolv,
                                        overførtFra = kandidatOverføresFra.navn,
                                        overførtTil = seddelNavnet,
                                        overførtSum = overføresOverskudd,
                                        rundenr = runde,
                                        bleValgt = false
                                    };
                                    overførteOverskudd.Add(nySeddel);

                                    if (oppdaterKandidat.stemmetall >= valgtall)
                                    {
                                        for (int j = 0; j < overførteOverskudd.Count(); j++)
                                        {
                                            if (seddelNavnet == overførteOverskudd[j].overførtTil && overførteOverskudd[j].rundenr == runde)
                                            {
                                                overførteOverskudd[j].bleValgt = true;
                                            }
                                        }
                                    }
                                    db.SaveChanges();
                                }
                            }
                            kandidatOverføresFra.stemmetall = valgtall;
                        }
                    }//If- Fant overskudd
                    else //Lage en av de med laveste stemmetallene ekskluderes - (Random)
                    {
                        if (kvoteringsvalg == true)
                        {
                            //Finner laveste stemmetall
                            if (gjenståendeKandidater.Count() > 0)
                            {
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
                                    double tempStemmetall = gjenståendeKandidater[i].stemmetall;
                                    if (tempStemmetall < stemmetallEkskludert)
                                    {
                                        if (tempStemmetall != 0)
                                        {
                                            if (gjenståendeKandidater[i].klasse == klasse1)
                                            {
                                                int hjelpeteller = 0;
                                                for (int k = 0; k < gjenståendeKandidater.Count(); k++)
                                                {
                                                    if (gjenståendeKandidater[k].klasse == klasse1)
                                                    {
                                                        hjelpeteller++;
                                                    }
                                                }
                                                if (hjelpeteller > antallTilMinsteFylt1)
                                                {
                                                    stemmetallEkskludert = tempStemmetall;
                                                }
                                            }
                                            else if (gjenståendeKandidater[i].klasse == klasse2)
                                            {
                                                int hjelpeteller2 = 0;
                                                for (int k = 0; k < gjenståendeKandidater.Count(); k++)
                                                {
                                                    if (gjenståendeKandidater[k].klasse == klasse2)
                                                    {
                                                        hjelpeteller2++;
                                                    }
                                                }
                                                if (hjelpeteller2 > antallTilMinsteFylt2)
                                                {
                                                    stemmetallEkskludert = tempStemmetall;
                                                }
                                            }
                                        }

                                    }
                                }
                            }//Funnet laveste stemmetall

                            KandidatSTV kandidatLavestStemmetall = gjenståendeKandidater.FirstOrDefault(k => k.stemmetall == stemmetallEkskludert);
                            if (kandidatLavestStemmetall != null)
                            {
                                gjenståendeKandidater.Remove(kandidatLavestStemmetall);
                            }
                            string navnEkskludert = kandidatLavestStemmetall.navn;
                            double tempStemmetallEksKandidat = kandidatLavestStemmetall.stemmetall - 1;
                            List<Stemmeseddel_db> listeEkskludert = db.Stemmesedler.Where(b => b.KandidatnrEn == navnEkskludert && b.ValgtypeID == valgtypeid).ToList();

                            for (int j = 0; j < listeEkskludert.Count(); j++)
                            {
                                string seddelNavnet = "";
                                string navnToPåSeddel = listeEkskludert[j].KandidatnrTo;
                                string navnTrePåSeddel = listeEkskludert[j].KandidatnrTre;
                                string navnFirePåSeddel = listeEkskludert[j].KandidatnrFire;
                                string navnFemPåSeddel = listeEkskludert[j].KandidatnrFem;
                                string navnSeksPåSeddel = listeEkskludert[j].KandidatnrSeks;
                                string navnSjuPåSeddel = listeEkskludert[j].KandidatnrSju;
                                string navnÅttePåSeddel = listeEkskludert[j].KandidatnrÅtte;
                                string navnNiPåSeddel = listeEkskludert[j].KandidatnrNi;
                                string navnTiPåSeddel = listeEkskludert[j].KandidatnrTi;
                                string navnEllevePåSeddel = listeEkskludert[j].KandidatnrElleve;
                                string navnTolvPåSedde = listeEkskludert[j].KandidatnrTolv;
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
                                else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnSeksPåSeddel) != null && !funnet)
                                {
                                    seddelNavnet = navnSeksPåSeddel;
                                    funnet = true;
                                }
                                else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnSjuPåSeddel) != null && !funnet)
                                {
                                    seddelNavnet = navnSjuPåSeddel;
                                    funnet = true;
                                }
                                else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnÅttePåSeddel) != null && !funnet)
                                {
                                    seddelNavnet = navnÅttePåSeddel;
                                    funnet = true;
                                }
                                else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnNiPåSeddel) != null && !funnet)
                                {
                                    seddelNavnet = navnNiPåSeddel;
                                    funnet = true;
                                }
                                else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnTiPåSeddel) != null && !funnet)
                                {
                                    seddelNavnet = navnTiPåSeddel;
                                    funnet = true;
                                }
                                else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnEllevePåSeddel) != null && !funnet)
                                {
                                    seddelNavnet = navnEllevePåSeddel;
                                    funnet = true;
                                }
                                else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnTolvPåSedde) != null && !funnet)
                                {
                                    seddelNavnet = navnTolvPåSedde;
                                    funnet = true;
                                }

                                if (seddelNavnet == "")
                                {
                                }
                                else
                                {
                                    tempStemmetallEksKandidat = kandidatLavestStemmetall.stemmetall - 1;

                                    KandidatSTV oppdaterKandidat = gjenståendeKandidater.FirstOrDefault(b => b.navn == seddelNavnet);
                                    double tempStemmetallet = oppdaterKandidat.stemmetall + 1;
                                    oppdaterKandidat.stemmetall = tempStemmetallet;

                                    KandidatListeSTV endreKandidat = db.KandidaterSTV.FirstOrDefault(b => b.Navn == seddelNavnet && b.ValgtypeID == valgtypeid);
                                    endreKandidat.KandidatID = oppdaterKandidat.kandidatListeID;
                                    endreKandidat.Navn = oppdaterKandidat.navn;
                                    endreKandidat.Stemmetall = tempStemmetallet;

                                    KandidatListeSTV ekskludereKandidat = db.KandidaterSTV.FirstOrDefault(b => b.Navn == navnEkskludert && b.ValgtypeID == valgtypeid);
                                    ekskludereKandidat.KandidatID = kandidatLavestStemmetall.kandidatListeID;
                                    ekskludereKandidat.Navn = kandidatLavestStemmetall.navn;
                                    ekskludereKandidat.Stemmetall = tempStemmetallEksKandidat;

                                    kandidatLavestStemmetall.stemmetall = tempStemmetallEksKandidat;


                                    db.SaveChanges();

                                }
                            }
                        }
                        else
                        {//Uten kvotering
                            //Finner laveste stemmetall
                            if (gjenståendeKandidater.Count() > 0)
                            {
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
                                    double tempStemmetall = gjenståendeKandidater[i].stemmetall;
                                    if (tempStemmetall < stemmetallEkskludert)
                                    {
                                        if (tempStemmetall != 0)
                                        {
                                            stemmetallEkskludert = tempStemmetall;
                                        }

                                    }
                                }
                            }//Funnet laveste stemmetall
                             //Finner en av de med laveste stemmetallet - Tilfeldig
                            KandidatSTV kandidatLavestStemmetall = gjenståendeKandidater.FirstOrDefault(k => k.stemmetall == stemmetallEkskludert);
                            if (kandidatLavestStemmetall != null)
                            {
                                gjenståendeKandidater.Remove(kandidatLavestStemmetall);
                            }
                            string navnEkskludert = kandidatLavestStemmetall.navn;
                            double tempStemmetallEksKandidat = kandidatLavestStemmetall.stemmetall - 1;
                            List<Stemmeseddel_db> listeEkskludert = db.Stemmesedler.Where(b => b.KandidatnrEn == navnEkskludert && b.ValgtypeID == valgtypeid).ToList();

                            for (int j = 0; j < listeEkskludert.Count(); j++)
                            {
                                string seddelNavnet = "";
                                string navnToPåSeddel = listeEkskludert[j].KandidatnrTo;
                                string navnTrePåSeddel = listeEkskludert[j].KandidatnrTre;
                                string navnFirePåSeddel = listeEkskludert[j].KandidatnrFire;
                                string navnFemPåSeddel = listeEkskludert[j].KandidatnrFem;
                                string navnSeksPåSeddel = listeEkskludert[j].KandidatnrSeks;
                                string navnSjuPåSeddel = listeEkskludert[j].KandidatnrSju;
                                string navnÅttePåSeddel = listeEkskludert[j].KandidatnrÅtte;
                                string navnNiPåSeddel = listeEkskludert[j].KandidatnrNi;
                                string navnTiPåSeddel = listeEkskludert[j].KandidatnrTi;
                                string navnEllevePåSeddel = listeEkskludert[j].KandidatnrElleve;
                                string navnTolvPåSedde = listeEkskludert[j].KandidatnrTolv;
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
                                else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnSeksPåSeddel) != null && !funnet)
                                {
                                    seddelNavnet = navnSeksPåSeddel;
                                    funnet = true;
                                }
                                else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnSjuPåSeddel) != null && !funnet)
                                {
                                    seddelNavnet = navnSjuPåSeddel;
                                    funnet = true;
                                }
                                else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnÅttePåSeddel) != null && !funnet)
                                {
                                    seddelNavnet = navnÅttePåSeddel;
                                    funnet = true;
                                }
                                else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnNiPåSeddel) != null && !funnet)
                                {
                                    seddelNavnet = navnNiPåSeddel;
                                    funnet = true;
                                }
                                else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnTiPåSeddel) != null && !funnet)
                                {
                                    seddelNavnet = navnTiPåSeddel;
                                    funnet = true;
                                }
                                else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnEllevePåSeddel) != null && !funnet)
                                {
                                    seddelNavnet = navnEllevePåSeddel;
                                    funnet = true;
                                }
                                else if (gjenståendeKandidater.FirstOrDefault(b => b.navn == navnTolvPåSedde) != null && !funnet)
                                {
                                    seddelNavnet = navnTolvPåSedde;
                                    funnet = true;
                                }

                                if (seddelNavnet == "")
                                {
                                }
                                else
                                {
                                    tempStemmetallEksKandidat = kandidatLavestStemmetall.stemmetall - 1;

                                    KandidatSTV oppdaterKandidat = gjenståendeKandidater.FirstOrDefault(b => b.navn == seddelNavnet);
                                    double tempStemmetallet = oppdaterKandidat.stemmetall + 1;
                                    oppdaterKandidat.stemmetall = tempStemmetallet;

                                    KandidatListeSTV endreKandidat = db.KandidaterSTV.FirstOrDefault(b => b.Navn == seddelNavnet && b.ValgtypeID == valgtypeid);
                                    endreKandidat.KandidatID = oppdaterKandidat.kandidatListeID;
                                    endreKandidat.Navn = oppdaterKandidat.navn;
                                    endreKandidat.Stemmetall = tempStemmetallet;

                                    KandidatListeSTV ekskludereKandidat = db.KandidaterSTV.FirstOrDefault(b => b.Navn == navnEkskludert && b.ValgtypeID == valgtypeid);
                                    ekskludereKandidat.KandidatID = kandidatLavestStemmetall.kandidatListeID;
                                    ekskludereKandidat.Navn = kandidatLavestStemmetall.navn;
                                    ekskludereKandidat.Stemmetall = tempStemmetallEksKandidat;

                                    kandidatLavestStemmetall.stemmetall = tempStemmetallEksKandidat;


                                    db.SaveChanges();

                                }
                            }
                        }

                    }//Else-if fant ikke overskudd, eksluderte en av de med laveste stemmetall
                }

                //Sjekke om noen ble valgt etter at stemmer ble eksludert og overført
                if (kvoteringsvalg == true)
                {
                    for (int i = 0; i < gjenståendeKandidater.Count(); i++)
                    {
                        if (gjenståendeKandidater[i].stemmetall >= valgtall)
                        {
                            if (valgteKandidater.Count() < antallRepresentanter)
                            {
                                if (gjenståendeKandidater[i].klasse == klasse1)
                                {
                                    if (antallLedigeKvote1 > 0)
                                    {
                                        valgteKandidater.Add(gjenståendeKandidater[i]);
                                        antallLedigeKvote1--;
                                        antallLedigeplasser--;
                                    }
                                }
                                else if (gjenståendeKandidater[i].klasse == klasse2)
                                {
                                    if (antallLedigeKvote2 > 0)
                                    {
                                        valgteKandidater.Add(gjenståendeKandidater[i]);
                                        antallLedigeKvote2--;
                                        antallLedigeplasser--;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < gjenståendeKandidater.Count(); i++)
                    {
                        if (gjenståendeKandidater[i].stemmetall >= valgtall)
                        {
                            if (valgteKandidater.Count() < antallRepresentanter)
                            {
                                valgteKandidater.Add(gjenståendeKandidater[i]);
                                antallLedigeplasser--;
                            }
                        }
                    }
                }

                //Slette de eventuelle valgte kandidatene som ble valgt fra gjenstående
                for (int i = 0; i < valgteKandidater.Count(); i++)
                {
                    for (int k = 0; k < gjenståendeKandidater.Count(); k++)
                    {
                        if (valgteKandidater[i].navn == gjenståendeKandidater[k].navn)
                        {
                            gjenståendeKandidater.Remove(gjenståendeKandidater[k]);
                        }
                    }
                }
                //Sletter om det fortsatt er noen som har null stemmer 
                for (int i = 0; i < gjenståendeKandidater.Count(); i++)
                {
                    if (gjenståendeKandidater[i].stemmetall == 0)
                    {
                        gjenståendeKandidater.Remove(gjenståendeKandidater[i]);
                    }
                }

                //Clearer ekskluderte lista
                for (int i = 0; i < ekskluderteKandidater.Count(); i++)
                {
                    kandidaterEkskludert2.Add(ekskluderteKandidater[i]);
                }

                ekskluderteKandidater.Clear();

            }//While stopper her

            leggTilValgteKandidater(valgteKandidater); //Foreløig
            return valgteKandidater; //Foreløpig
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
                    kvoteKlasseEn = enDbValg.KvoteKlasseEn,
                    kvoteKlasseTo = enDbValg.KvoteKlasseTo,
                    klasseEnProsent = enDbValg.KlasseEnProsent,
                    klasseToProsent = enDbValg.KlasseToProsent,
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
                    klasse = varaListe[i].Klasse,
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
                        double tempStemmetall = oppdaterKandidat.stemmetall + 1;

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
        public string lagreNyttPreferansevalg(string beskrivelse, int antallRepresentanter, string klasse1, string klasse2, int prosent1, int prosent2)
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
                KvoteKlasseEn = klasse1,
                KvoteKlasseTo = klasse2,
                KlasseEnProsent = prosent1,
                KlasseToProsent = prosent2,
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
            for (int i = 0; i < varaer.Count(); i++)
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
                    klasse = kandidatliste[i].Klasse,
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
                db.SaveChanges();
                List<PreferansevalgValgt_db> preferansevalgValgt = db.PreferansevalgValgt.Where(p => p.ValgtypeID == id).ToList();
                for (int i = 0; i < preferansevalgValgt.Count(); i++)
                {
                    db.PreferansevalgValgt.Remove(preferansevalgValgt[i]);
                }

                List<KandidatListeSTV> kandidatListe = db.KandidaterSTV.Where(v => v.ValgtypeID == id).ToList();
                for (int i = 0; i < kandidatListe.Count(); i++)
                {
                    if (kandidatListe[i].ValgtypeID == id)
                    {
                        db.KandidaterSTV.Remove(kandidatListe[i]);
                    }
                }

                List<VaraListeSTV> varaListe = db.PreferansevalgVaraer.Where(v => v.ValgtypeID == id).ToList();
                for (int i = 0; i < varaListe.Count(); i++)
                {
                    if (varaListe[i].ValgtypeID == id)
                    {
                        db.PreferansevalgVaraer.Remove(varaListe[i]);
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
        public bool slettForhåndslagredePreferanseValg(int id)
        {
            var db = new BrukerContext();
            try
            {
                Valgtyper_db valgtyper = db.Valgtyper.FirstOrDefault(b => b.ValgtypeID == id);
                db.Valgtyper.Remove(valgtyper);
                // slett også 
                Preferansevalg_db preferansevalg = db.PreferanseValg.FirstOrDefault(p => p.ValgtypeID == id);
                db.PreferanseValg.Remove(preferansevalg);

                List<PreferansevalgValgt_db> preferansevalgValgt = db.PreferansevalgValgt.Where(p => p.ValgtypeID == id).ToList();
                for (int i = 0; i < preferansevalgValgt.Count(); i++)
                {
                    db.PreferansevalgValgt.Remove(preferansevalgValgt[i]);
                }

                List<KandidatListeSTV> kandidatListe = db.KandidaterSTV.Where(v => v.ValgtypeID == id).ToList();
                for (int i = 0; i < kandidatListe.Count(); i++)
                {
                    if (kandidatListe[i].ValgtypeID == id)
                    {
                        db.KandidaterSTV.Remove(kandidatListe[i]);
                    }
                }

                List<VaraListeSTV> varaListe = db.PreferansevalgVaraer.Where(v => v.ValgtypeID == id).ToList();
                for (int i = 0; i < varaListe.Count(); i++)
                {
                    if (varaListe[i].ValgtypeID == id)
                    {
                        db.PreferansevalgVaraer.Remove(varaListe[i]);
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
        public string lagreKandidatSTV(string id, string klasse)
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
                    StemmetallSatt = false,
                    Klasse = klasse
                };
                var kandidatVaraliste = new VaraListeSTV()
                {
                    Navn = id,
                    StemmetallSatt = false,
                    Klasse = klasse
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
        public string startPreferansevalg(string beskrivelse, int antallRepresentanter,
                                          string klasse1, string klasse2, int prosent1, int prosent2)
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
                KvoteKlasseEn = klasse1,
                KvoteKlasseTo = klasse2,
                KlasseEnProsent = prosent1,
                KlasseToProsent = prosent2,
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


       
        
    }
}