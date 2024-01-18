using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TypecursusApplicatie.Data_Access_Layer;
using TypecursusApplicatie.Models;

namespace TypecursusApplicatie
{
    public partial class Registratiepagina : UserControl
    {
        private MainWindow mainWindow;

        public Registratiepagina()
        {
            InitializeComponent();
            this.Loaded += Registratiepagina_Loaded;
        }

        public Registratiepagina(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.DataContext = mainWindow;
        }

        private void Registratiepagina_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is MainWindow main)
            {
                mainWindow = main;
            }
        }

        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.LoadAccountControl();
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.LoadLoginControl();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.LogoutUser();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string voornaam = txtVoornaam.Text;
            string achternaam = txtAchternaam.Text;
            string email = txtEmailReg.Text;
            string wachtwoord = txtPasswordReg.Password;

            if (string.IsNullOrWhiteSpace(voornaam) || string.IsNullOrWhiteSpace(achternaam) || !IsEmailValid(email) || !IsPasswordValid(wachtwoord))
            {
                MessageBox.Show("Controleer of alle velden correct zijn ingevuld.");
                return;
            }

            if (!IsEmailValid(email))
            {
                MessageBox.Show("Ongeldig e-mailadres.");
                return;
            }

            if (!IsPasswordValid(wachtwoord))
            {
                MessageBox.Show("Wachtwoord moet minstens 8 tekens lang zijn en zowel letters als cijfers bevatten.");
                return;
            }

            GebruikerDAL gebruikerDAL = new GebruikerDAL();
            Gebruiker nieuweGebruiker = new Gebruiker
            {
                Voornaam = voornaam,
                Achternaam = achternaam,
                Emailadres = email,
                Wachtwoord = wachtwoord,
            };

            gebruikerDAL.AddGebruiker(nieuweGebruiker);
            MessageBox.Show("Registratie succesvol!");
            mainWindow.LoadLoginControl();
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

        private void RegistratieButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(mainWindow.MainContent.Content is Registratiepagina))
            {
                mainWindow.LoadRegisterControl();
            }
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
    }
}
