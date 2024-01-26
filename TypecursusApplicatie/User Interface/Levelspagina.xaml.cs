using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using TypecursusApplicatie.Data_Access_Layer;
using TypecursusApplicatie.BusinessLogicLayer;
using TypecursusApplicatie.Models;
using System.Windows.Input;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System;

namespace TypecursusApplicatie
{
    public partial class Levelspagina : UserControl
    {
        private MainWindow mainWindow;
        public ObservableCollection<TypecursusApplicatie.Models.Level> Levels { get; private set; }

        public Levelspagina(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            Levels = new ObservableCollection<Level>();
            this.DataContext = this;
        }

        // Methode die controleert of de gebruiker is ingelogd. Als de gebruiker is ingelogd worden de levels geladen, anders krijgt de gebruiker een melding dat hij/zij moet inloggen.
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

        // Methode die de levels laadt.
        private void LoadLevels()
        {
            GebruikerDAL gebruikerDAL = new GebruikerDAL();
            var alleLevels = gebruikerDAL.GetAllLevels();
            Levels.Clear();

            int userId = UserSession.CurrentUserID; // Get current user ID
            bool previousLevelCompleted = true; // Assuming level 1 is always unlocked

            foreach (var level in alleLevels)
            {
                int progress = gebruikerDAL.GetProgressForLevel(userId, level.LevelID);
                level.ProgressPercentage = progress;
                level.ProgressDisplay = GenerateProgressDisplay(progress);
                level.IsUnlocked = previousLevelCompleted; // Unlock the level if the previous level is completed

                Levels.Add(level);
                previousLevelCompleted = level.ProgressPercentage == 100; // Update for next iteration
            }
        }

        // Methode die de progressie van de gebruiker weergeeft.
        private string GenerateProgressDisplay(int progress)
        {
            return $"{progress}% completed";
        }

        // Methode voor de knoppen van de levels. Als je op een level klikt wordt je doorgestuurd naar de module overzichtspagina van dat level.
        private void LevelTile_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            Level selectedLevel = clickedButton.DataContext as Level;
            if (selectedLevel != null && selectedLevel.IsUnlocked)
            {
                mainWindow.LoadModuleOverzichtspagina(selectedLevel.LevelID);
            }
        }

        private void Homepagina_Loaded(object sender, RoutedEventArgs e)
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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            var selectedLevel = comboBox.SelectedItem as Level;
            if (selectedLevel != null && selectedLevel.IsUnlocked)
            {
                mainWindow.LoadModuleOverzichtspagina(selectedLevel.LevelID);
            }
            else
            {
                // Reset the selection if the level is locked
                comboBox.SelectedIndex = -1;
            }
        }

    }
}
