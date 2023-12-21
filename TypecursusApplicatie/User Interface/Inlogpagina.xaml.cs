using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TypecursusApplicatie.Data_Access_Layer;
using TypecursusApplicatie.BusinessLogicLayer;
using TypecursusApplicatie.Models;
using System.Windows.Media.Animation;

namespace TypecursusApplicatie
{
    public partial class Inlogpagina : UserControl
    {
        private MainWindow mainWindow;

        public Inlogpagina()
        {
            InitializeComponent();
            this.Loaded += Inlogpagina_Loaded;
            this.PreviewKeyDown += Inlogpagina_PreviewKeyDown;
            this.mainWindow = mainWindow;
            this.DataContext = mainWindow;

            txtEmail.KeyDown += TxtEmail_KeyDown;
            txtPassword.KeyDown += TxtPassword_KeyDown;
        }
        private void Inlogpagina_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                LoginButton_Click(this, new RoutedEventArgs());
        }
        private void TxtEmail_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                LoginButton_Click(this, new RoutedEventArgs());
        }

        private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                LoginButton_Click(this, new RoutedEventArgs());
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

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            GebruikerDAL gebruikerDAL = new GebruikerDAL();
            string email = txtEmail.Text;
            string ingevoerdeWachtwoord = txtPassword.Password;

            Gebruiker gebruiker = gebruikerDAL.GetGebruikerByEmail(email);

            // Hash het ingevoerde wachtwoord en vergelijk het met het gehashte wachtwoord in de database
            if (gebruiker != null && GebruikerDAL.HashWachtwoord(ingevoerdeWachtwoord) == gebruiker.Wachtwoord)
            {
                // Stel de ingelogde gebruiker in op de gebruikerssessie
                UserSession.Login(gebruiker);

                MessageBox.Show("Succesvol ingelogd!");
                mainWindow.LoadHomeControl();

                // Update de IsUserLoggedIn property in MainWindow
                mainWindow.OnPropertyChanged(nameof(mainWindow.IsUserLoggedIn));
            }
            else
            {
                MessageBox.Show("Onjuiste inloggegevens.");
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
            // Controleer of de huidige pagina al de Inlogpagina is
            if (mainWindow.MainContent.Content is Inlogpagina)
            {
                return; // Als dat zo is, doe niets
            }

            // Zo niet, navigeer naar de inlogpagina
            mainWindow.LoadLoginControl();
        }


        private void Logo_Click(object sender, MouseButtonEventArgs e)
        {
            mainWindow.LoadHomeControl(); 
        }
    }
}
