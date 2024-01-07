using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using TypecursusApplicatie.Data_Access_Layer;
using TypecursusApplicatie.Models;
using System.Windows.Input;

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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadModulesForLevel(currentLevelId);
        }

        private void LoadModulesForLevel(int levelId)
        {
            GebruikerDAL gebruikerDAL = new GebruikerDAL();
            var modulesForLevel = gebruikerDAL.GetModulesForLevel(levelId);
            Modules.Clear();
            foreach (var module in modulesForLevel)
            {
                Modules.Add(module);
            }
        }

        private void StartModuleButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Module selectedModule = btn.DataContext as Module;
            MessageBox.Show($"Module {selectedModule.ModuleNaam} gestart!");
            // Voeg hier logica toe om de module te starten
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
