using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using TypecursusApplicatie.Data_Access_Layer;
using TypecursusApplicatie.Models;
using System.Windows.Input;
using TypecursusApplicatie.BusinessLogicLayer;

namespace TypecursusApplicatie
{
    public partial class ModuleOverzichtspagina : UserControl
    {
        private MainWindow mainWindow;
        private int currentLevelId;
        public ObservableCollection<Module> Modules { get; private set; }

        public ModuleOverzichtspagina(MainWindow mainWindow, int levelId)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.currentLevelId = levelId;
            Modules = new ObservableCollection<Module>();
            this.DataContext = this;
        }

        // Methode die de modules laadt voor het level dat is meegegeven
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadModulesForLevel(currentLevelId);
        }

        // Methode die de modules laadt
        private void LoadModulesForLevel(int levelId)
        {
            GebruikerDAL gebruikerDAL = new GebruikerDAL();
            var modulesForLevel = gebruikerDAL.GetModulesForLevel(levelId);
            Modules.Clear();

            foreach (var module in modulesForLevel)
            {
                module.IsModuleCompleted = gebruikerDAL.IsModuleCompleted(UserSession.CurrentUserID, module.ModuleID);
                Modules.Add(module);
            }
        }

        // Methode voor de knop die de module start
        private void StartModuleButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Module selectedModule = btn.DataContext as Module;
            mainWindow.LoadModuleDetailpagina(selectedModule.ModuleID);
        }

        // De event handlers voor de sidebar en logo
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
                comboBox.SelectedIndex = -1; 
            }
        }
    }
}
