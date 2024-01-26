// Gebruik de TypescursusApplicatie Models namespace
using TypecursusApplicatie.Models;

// De business logica laag van de Typecursus Applicatie
namespace TypecursusApplicatie.BusinessLogicLayer
{
    // De klasse UserSession beheert de gebruikerssessie
    public class UserSession
    {
        // Huidige gebruiker van de sessie, static zodat het overal in de app toegankelijk is
        public static Gebruiker CurrentUser { get; set; }

        // Het ID van de huidige gebruiker, ook static
        public static int CurrentUserID { get; set; }

        // Methode voor inloggen, stelt de huidige gebruiker en zijn/haar ID in
        public static void Login(Gebruiker user)
        {
            CurrentUser = user; // Zet de huidige gebruiker
            CurrentUserID = user.GebruikersID; // Zet het ID van de huidige gebruiker
        }

        // Methode voor uitloggen, zet de huidige gebruiker op null
        public static void Logout()
        {
            CurrentUser = null; // Verwijdert de huidige gebruiker uit de sessie
        }

        // Methode om te controleren of er een gebruiker is ingelogd
        public static bool IsLoggedIn()
        {
            return CurrentUser != null; // Geeft true terug als er een gebruiker is ingelogd, anders false
        }
    }
}
