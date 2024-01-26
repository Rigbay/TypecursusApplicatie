using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TypecursusApplicatie.Data_Access_Layer;
using TypecursusApplicatie.BusinessLogicLayer;
using TypecursusApplicatie.Models;
using System.Windows.Media.Animation;
using System.Linq;

namespace TypecursusApplicatie
{
    public partial class Inlogpagina : UserControl
    {
        private MainWindow mainWindow;

        public Inlogpagina()
        {
            InitializeComponent();
            this.Loaded += Inlogpagina_Loaded;
            this.mainWindow = mainWindow;
            this.DataContext = mainWindow;
            txtEmail.KeyDown += TxtEmail_KeyDown;
            txtPassword.KeyDown += TxtPassword_KeyDown;
        }

        // Methode die ervoor zorgt dat je kan inloggen met de enter toets
        private void TxtEmail_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                LoginButton_Click(this, new RoutedEventArgs());
        }

        // Methode die ervoor zorgt dat je kan inloggen met de enter toets
        private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                LoginButton_Click(this, new RoutedEventArgs());
        }

        // Methode die ervoor zorgt dat je kan inloggen met de enter toets
        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginButton_Click(this, new RoutedEventArgs());
            }
        }

        public Inlogpagina(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.DataContext = mainWindow;
        }

        private void Inlogpagina_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is MainWindow main)
            {
                mainWindow = main;
            }
        }

        // Wanneer je op de login knop klikt wordt er gecontroleerd of het e-mailadres en wachtwoord correct is ingevoerd. Als dit niet het geval is krijg je een melding te zien. Als dit wel het geval is wordt er gecontroleerd of het e-mailadres en wachtwoord overeenkomen met de gegevens in de database.
        // Als dit niet het geval is krijg je een melding te zien. Als dit wel het geval is wordt je ingelogd en wordt je doorgestuurd naar de homepagina.
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text;
            string ingevoerdeWachtwoord = txtPassword.Password;

            if (!IsEmailValid(email) || !IsPasswordValid(ingevoerdeWachtwoord))
            {
                var messageBox = new CustomMessageBox("Controleer of het e-mailadres en wachtwoord correct is ingevoerd.");
                messageBox.ShowDialog();
                return;
            }

            GebruikerDAL gebruikerDAL = new GebruikerDAL();
            Gebruiker gebruiker = gebruikerDAL.GetGebruikerByEmail(email);

            if (gebruiker != null && gebruiker.Wachtwoord == GebruikerDAL.HashWachtwoord(ingevoerdeWachtwoord, gebruiker.Salt))
            {
                UserSession.Login(gebruiker);
                mainWindow.LoadHomeControl();
            }
            else
            {
                var messageBox = new CustomMessageBox("Onjuiste inloggegevens.");
                messageBox.ShowDialog();
            }
        }

        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.LoadAccountControl();
        }

        private void InlogButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(mainWindow.MainContent.Content is Inlogpagina))
            {
                mainWindow.LoadLoginControl();
            }
        }


        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.LogoutUser();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.LoadRegisterControl();

        }

        private void SidebarToggle_Click(object sender, RoutedEventArgs e)
        {
            Sidebar.Width = Sidebar.Width == 0 ? 250 : 0;
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.LoadHomeControl();
        }

        private void Logo_Click(object sender, MouseButtonEventArgs e)
        {
            mainWindow.LoadHomeControl(); 
        }
        private bool IsEmailValid(string email)
        {
            var regex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return regex.IsMatch(email);
        }

        private bool IsPasswordValid(string password)
        {
            return password.Length >= 8 && password.Any(char.IsDigit) && password.Any(char.IsLetter);
        }

    }
}
