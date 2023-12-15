using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TypecursusApplicatie.Data_Access_Layer;
using TypecursusApplicatie.Models;

namespace TypecursusApplicatie
{
    public partial class Homepagina : UserControl
    {
        private MainWindow mainWindow;

        public Homepagina()
        {
            InitializeComponent();
            this.Loaded += Homepagina_Loaded;
        }

        public Homepagina(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private void Homepagina_Loaded(object sender, RoutedEventArgs e)
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

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.LoadRegisterControl();

        }

        private void LevelsPagina_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.LoadLevelsControl();
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

        private void TestDatabase_Click(object sender, RoutedEventArgs e)
        {
            GebruikerDAL gebruikerDAL = new GebruikerDAL();
            Gebruiker gebruiker = gebruikerDAL.GetGebruiker(1); // Veronderstelt dat er een gebruiker is met ID 1

            if (gebruiker != null)
            {
                MessageBox.Show($"Gebruiker gevonden: {gebruiker.Voornaam} {gebruiker.Achternaam}");
            }
            else
            {
                MessageBox.Show("Gebruiker niet gevonden.");
            }
        }

    }
}
