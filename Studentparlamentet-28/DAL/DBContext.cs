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
        public string Brukernavn { get; set; }
        public int ValgtypeID { get; set; }
        public string Valgtype { get; set; }

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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}