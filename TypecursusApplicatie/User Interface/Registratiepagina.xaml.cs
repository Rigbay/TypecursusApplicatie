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
            GebruikerDAL gebruikerDAL = new GebruikerDAL();

            // Hier moet je de waarden uit je registratieformulier halen
            // Bijvoorbeeld: string email = txtEmailReg.Text; 
            string email = txtEmailReg.Text; // Vervang door juiste TextBox naam
            string wachtwoord = txtPasswordReg.Password; // Vervang door juiste PasswordBox naam
            string voornaam = txtVoornaam.Text; // Vervang door juiste TextBox naam
            string achternaam = txtAchternaam.Text; // Vervang door juiste TextBox naam

            Gebruiker nieuweGebruiker = new Gebruiker
            {
                Voornaam = voornaam,
                Achternaam = achternaam,
                Emailadres = email,
                Wachtwoord = wachtwoord // Overweeg hashing voor wachtwoordbeveiliging
            };

            gebruikerDAL.AddGebruiker(nieuweGebruiker);
            MessageBox.Show("Registratie succesvol!");
            mainWindow.LoadLoginControl();
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
