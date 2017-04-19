using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Studentparlamentet_28.DAL
{
    
    public class Admin_db
    {
        [Key]
        public string Brukernavn { get; set; }

        public byte[] Passord { get; set; }

        public bool Innlogget { get; set; }

        public bool Administrator { get; set; }

    }

    public class Bruker_db
    {
        [Key]
        public int Id { get; set; }

        public string Brukernavn { get; set; }

        public string Passord { get; set; }

        public bool Innlogget { get; set; }

        public bool Administrator { get; set; }
    }
    public class Votering_db
    {
        [Key]
        public int VoteringID { get; set; }
        public string svarFor { get; set; }
        public string svarMot { get; set; }
        public string svarBlank { get; set; }
        public int ValgtypeID { get; set; }
    }
    public class Valgtyper_db
    {
        [Key]
        public int ValgtypeID { get; set; }
        public string Valgtype { get; set; }
        public bool Start { get; set; }

    }

    public class BrukereStemt_db
    {
        [Key]
        public int Id { get; set; }
        public string Brukernavn { get; set; }
        public int ValgtypeID { get; set; }
        public string Valgtype { get; set; }

    }
    public class Personvalg_db
    {
        [Key]
        public int ID { get; set; }
        public string Fornavn { get; set; }

        public string Etternavn { get; set; }

        public int ValgtypeID { get; set; }

        public bool Lagret { get; set; }


    }

    public class PersonvalgResultat_db
    {
        [Key]
        public int ID { get; set; }

        public int ValgtypeID { get; set; }

        public String Vinner { get; set; }

        public int Stemmer { get; set; }
        public int TotalantallStemmer { get; set;}

        public int AntallKandidater { get; set; }
    }
    public class PersonvalgKandidatResultat_db
    {
        [Key]
        public int ID { get; set; }

        public int ValgtypeID { get; set; }

        public String Fornavn { get; set; }
        public String Etternavn { get; set; }
        public int Stemmer { get; set; }

    }

    public class PersonvalgResultatStemmer_db
    {
        [Key]
        public int ID { get; set; }

        public int ValgtypeID { get; set; }

        public string Fornavn { get; set; }

        public string Etternavn { get; set; }

    }

    public class BrukerContext : DbContext
    {
        public BrukerContext()
            : base("name=DbStudentparlamentet")
        {
            Database.CreateIfNotExists();
        }
        
        public DbSet<Admin_db> AdminBrukere { get; set; }
        public DbSet<Bruker_db> Brukere { get; set; }
        public DbSet<Votering_db> Voteringer { get; set; }
        public DbSet<Valgtyper_db> Valgtyper { get; set; }
        public DbSet<BrukereStemt_db> BrukereStemt { get; set; }
        public DbSet<Personvalg_db> Personvalger { get; set; }
        public DbSet<PersonvalgResultat_db> PersonvalgResultat { get; set; }
        public DbSet<PersonvalgResultatStemmer_db> PersonvalgResultatStemmer { get; set; }
        public DbSet<PersonvalgKandidatResultat_db> PersonvalgKandidatResultat { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}