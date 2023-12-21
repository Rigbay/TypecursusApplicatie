using TypecursusApplicatie.Models;

namespace TypecursusApplicatie.BusinessLogicLayer
{
    public class UserSession
    {
        public static Gebruiker CurrentUser { get; set; }

        public static void Login(Gebruiker user)
        {
            CurrentUser = user;
        }

        public static void Logout()
        {
            CurrentUser = null;
        }

        public static bool IsLoggedIn()
        {
            return CurrentUser != null;
        }
    }
}