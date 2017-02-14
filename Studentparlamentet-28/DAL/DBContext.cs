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

    }

    public class Bruker_db
    {
        [Key]
        public string Brukernavn { get; set; }

        public byte[] Passord { get; set; }
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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}