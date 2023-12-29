using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using TypecursusApplicatie.Data_Access_Layer;
using TypecursusApplicatie.BusinessLogicLayer;
using TypecursusApplicatie.Models;
using System.Windows.Input;
using System.Diagnostics;

namespace TypecursusApplicatie
{
    public partial class Levelspagina : UserControl
    {
        private MainWindow mainWindow;
        public ObservableCollection<Level> Levels { get; private set; }

        public Levelspagina(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            Levels = new ObservableCollection<Level>();
            this.DataContext = this;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (UserSession.IsLoggedIn())
            {
                LoadLevels();
            }
            else
            {
                MessageBox.Show("U moet ingelogd zijn om deze pagina te bekijken.");
                mainWindow.LoadLoginControl();
            }
        }

        private void LoadLevels()
        {
            GebruikerDAL gebruikerDAL = new GebruikerDAL();
            var alleLevels = gebruikerDAL.GetAllLevels();
            Levels.Clear();

            foreach (var level in alleLevels)
            {
                Levels.Add(level);
            }
        }


        // Event handlers voor knoppen zoals SidebarToggle_Click, Logo_Click, etc.
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

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.LogoutUser();
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
    }
}
