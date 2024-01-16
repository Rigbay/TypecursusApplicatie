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

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text;
            string ingevoerdeWachtwoord = txtPassword.Password;

            if (!IsEmailValid(email) || !IsPasswordValid(ingevoerdeWachtwoord))
            {
                MessageBox.Show("Controleer of het e-mailadres en wachtwoord correct zijn.");
                return;
            }

            GebruikerDAL gebruikerDAL = new GebruikerDAL();
            Gebruiker gebruiker = gebruikerDAL.GetGebruikerByEmail(email);

            if (gebruiker != null && gebruiker.Wachtwoord == GebruikerDAL.HashWachtwoord(ingevoerdeWachtwoord, gebruiker.Salt))
            {
                UserSession.Login(gebruiker);
                MessageBox.Show("Succesvol ingelogd!");
                mainWindow.LoadHomeControl();
            }
            else
            {
                MessageBox.Show("Onjuiste inloggegevens.");
            }
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
