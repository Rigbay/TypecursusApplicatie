using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using TypecursusApplicatie.Data_Access_Layer;
using TypecursusApplicatie.BusinessLogicLayer;
using TypecursusApplicatie.Models;
using System.Windows.Input;

namespace TypecursusApplicatie
{
    public partial class Levelspagina : UserControl
    {
        private MainWindow mainWindow;
        private ObservableCollection<LevelVoortgang> levelsMetVoortgang;

        public Levelspagina(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            levelsMetVoortgang = new ObservableCollection<LevelVoortgang>();
            this.DataContext = this;
        }

        private void Levelspagina_Loaded(object sender, RoutedEventArgs e)
        {
            if (UserSession.IsLoggedIn())
            {
                GebruikerDAL gebruikerDAL = new GebruikerDAL();
                var gebruikersVoortgang = gebruikerDAL.GetGebruikersVoortgangPerLevel(UserSession.CurrentUser.GebruikersID);

                foreach (var levelVoortgang in gebruikersVoortgang)
                {
                    levelsMetVoortgang.Add(levelVoortgang);
                }
            }
            else
            {
                MessageBox.Show("U moet ingelogd zijn om deze pagina te bekijken.");
                mainWindow.LoadLoginControl();
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
