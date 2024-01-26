// Importeer de benodigde namespaces
using System.Collections.Generic;
using System.Data.Entity;
using System.Runtime.Remoting.Contexts;
using TypecursusApplicatie.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Namespace voor de data toegangslaag van de Typecursus Applicatie
namespace TypecursusApplicatie.DataAccessLayer
{
    // DatabaseContext erft van DbContext, een Entity Framework klasse voor database interactie
    public class DatabaseContext : DbContext
    {
        // Constructor van DatabaseContext, stelt de database naam in
        public DatabaseContext() : base("name=Typecursusdatabase")
        {
        }

        // DbSet voor Gebruikers, beheert de Gebruiker entiteiten in de database
        public DbSet<Gebruiker> Gebruikers { get; set; }

        // DbSet voor Levels, beheert de Level entiteiten in de database
        public DbSet<Level> Levels { get; set; }

        // DbSet voor Modules, beheert de Module entiteiten in de database
        public DbSet<Module> Modules { get; set; }

        // DbSet voor GebruikersVoortgang, beheert de voortgang van gebruikers in de database
        public DbSet<GebruikersVoortgang> GebruikersVoortgang { get; set; }

        // DbSet voor ModulePogingen, beheert de pogingen van modules in de database
        public DbSet<ModulePogingen> ModulePogingen { get; set; }

        // DbSet voor Badges, beheert de badge entiteiten in de database
        public DbSet<Badge> Badges { get; set; }

        // DbSet voor GebruikersBadges, beheert de koppeling tussen gebruikers en badges in de database
        public DbSet<GebruikersBadges> GebruikersBadges { get; set; }
    }
}
