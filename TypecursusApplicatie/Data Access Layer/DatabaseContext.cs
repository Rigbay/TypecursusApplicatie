using System.Collections.Generic;
using System.Data.Entity;
using System.Runtime.Remoting.Contexts;
using TypecursusApplicatie.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TypecursusApplicatie.DataAccessLayer
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base("name=Typecursusdatabase")
        {
        }

        public DbSet<Gebruiker> Gebruikers { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<GebruikersVoortgang> GebruikersVoortgang { get; set; }
        public DbSet<ModulePogingen> ModulePogingen { get; set; }
        public DbSet<Badge> Badges { get; set; }
        public DbSet<GebruikersBadges> GebruikersBadges { get; set; }
    }
}
