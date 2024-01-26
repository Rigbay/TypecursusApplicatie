using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Media;

// Alle data modellen voor de database
namespace TypecursusApplicatie.Models
{
    public class Gebruiker
    {
        [Key]
        public int GebruikersID { get; set; }
        public string Voornaam { get; set; }
        public string Achternaam { get; set; }
        public string Emailadres { get; set; }
        public string Wachtwoord { get; set; } // Gehasht
        public string Salt { get; set; }

        // Navigatie-eigenschappen
        public virtual ICollection<GebruikersVoortgang> Voortgang { get; set; }
        public virtual ICollection<ModulePogingen> Pogingen { get; set; }
        public virtual ICollection<GebruikersBadges> Badges { get; set; }
    }

    public class Level
    {
        [Key]
        public int LevelID { get; set; }
        public string LevelNaam { get; set; }
        public string Moeilijkheid { get; set; }

        // Navigatie-eigenschap
        public virtual ICollection<Module> Modules { get; set; }

        [NotMapped]
        public int ProgressPercentage { get; set; }
        [NotMapped]
        public string ProgressDisplay { get; set; }
        public bool IsUnlocked { get; set; }
    }

    public class Module
    {
        [Key]
        public int ModuleID { get; set; }
        [ForeignKey("Level")]
        public int LevelID { get; set; }
        public string ModuleNaam { get; set; }
        public string ModuleBeschrijving { get; set; }
        public string ModuleContent { get; set; }
        public int MinWPM { get; set; }
        public int MinNauwkeurigheid { get; set; }
        public int TimeLeft { get; set; }

        // Navigatie-eigenschappen
        public virtual Level Level { get; set; }
        public virtual ICollection<GebruikersVoortgang> Voortgang { get; set; }
        public virtual ICollection<ModulePogingen> Pogingen { get; set; }

        public bool IsModuleCompleted { get; set; }
    }

    public class GebruikersVoortgang
    {
        [Key]
        public int VoortgangsID { get; set; }
        [ForeignKey("Gebruiker")]
        public int GebruikersID { get; set; }
        [ForeignKey("Module")]
        public int ModuleID { get; set; }
        public bool LevelVoltooid { get; set; }
        public bool ModuleVoltooid { get; set; }
        public DateTime VoltooiDatum { get; set; }

        // Navigatie-eigenschappen
        public virtual Gebruiker Gebruiker { get; set; }
        public virtual Module Module { get; set; }
    }

    public class ModulePogingen
    {
        [Key]
        public int PogingID { get; set; }
        [ForeignKey("Gebruiker")]
        public int GebruikersID { get; set; }
        [ForeignKey("Module")]
        public int ModuleID { get; set; }
        public int GebruikersWPM { get; set; }
        public int GebruikersNauwkeurigheid { get; set; }
        public DateTime PogingDatum { get; set; }

        // Navigatie-eigenschappen
        public virtual Gebruiker Gebruiker { get; set; }
        public virtual Module Module { get; set; }
    }

    public class Badge
    {
        public int BadgeID { get; set; }
        public string BadgeNaam { get; set; }
        public string Criteria { get; set; }
        public byte[] BadgeAfbeelding { get; set; }
        public string BadgeBeschrijving { get; set; } 
        public bool IsUnlocked { get; set; } 
        public DateTime? BadgeBehaalDatum { get; set; }
        public ImageSource ImageSource { get; set; } 
    }


    public class GebruikersBadges
    {
        [Key]
        public int GebruikersBadgeID { get; set; }
        [ForeignKey("Gebruiker")]
        public int GebruikersID { get; set; }
        [ForeignKey("Badge")]
        public int BadgeID { get; set; }
        public DateTime BadgeBehaalDatum { get; set; }

        // Navigatie-eigenschappen
        public virtual Gebruiker Gebruiker { get; set; }
        public virtual Badge Badge { get; set; }
    }

    public class LevelVoortgang
    {
        public int LevelID { get; set; }
        public string LevelNaam { get; set; }
        public int Voortgang { get; set; }
    }
}
