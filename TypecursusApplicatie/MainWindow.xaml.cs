using System.Windows;
using System.ComponentModel;
using TypecursusApplicatie.BusinessLogicLayer;
using System.Collections.ObjectModel;
using TypecursusApplicatie.Data_Access_Layer;
using TypecursusApplicatie.Models;
using System.Windows.Controls;

namespace TypecursusApplicatie
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            this.WindowState = WindowState.Maximized;
            LoadLevels();
            LoadLoginControl();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsUserLoggedIn => UserSession.IsLoggedIn();

        public bool IsNotLoggedIn => !IsUserLoggedIn;

        public bool IsCurrentPageLogin()
        {
            return MainContent.Content is Inlogpagina;
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void LoadLoginControl()
        {
            MainContent.Content = new Inlogpagina(this);
        }

        public void LoadRegisterControl()
        {
            MainContent.Content = new Registratiepagina(this);
        }

        public void LoadLevelsControl()
        {
            MainContent.Content = new Levelspagina(this);
        }

        public void LoadHomeControl()
        {
            MainContent.Content = new Homepagina(this);
        }

        public void LoadAccountControl()
        {
            MainContent.Content = new Statistiekenpagina(this);
        }

        public void LogoutUser()
        {
            UserSession.Logout();
            OnPropertyChanged(nameof(IsUserLoggedIn));
            OnPropertyChanged(nameof(IsNotLoggedIn));
            LoadLoginControl();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            LoadHomeControl();
        }

        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            LoadAccountControl();
        }

        private void BadgesButton_Click(object sender, RoutedEventArgs e)
        {
            LoadBadgeOverzichtspagina();
        }

        private void Logo_Click(object sender, RoutedEventArgs e)
        {
            LoadHomeControl();
        }

        public void LoadModuleOverzichtspagina(int levelId)
        {
            MainContent.Content = new ModuleOverzichtspagina(this, levelId);
        }

        public void LoadModuleDetailpagina(int moduleId)
        {
            MainContent.Content = new ModuleDetailpagina(this, moduleId);
        }

        public void LoadBadgeOverzichtspagina()
        {
            MainContent.Content = new BadgeOverzichtspagina(this);
        }

        public void LoadStatistiekenpagina()
        {
            MainContent.Content = new Statistiekenpagina(this);
        }

        private ObservableCollection<Level> _levels;

        public ObservableCollection<Level> Levels
        {
            get { return _levels; }
            set
            {
                _levels = value;
                OnPropertyChanged(nameof(Levels));
            }
        }

        public void LoadLevels()
        {
            GebruikerDAL gebruikerDAL = new GebruikerDAL();
            var alleLevels = gebruikerDAL.GetAllLevels();
            Levels = new ObservableCollection<Level>();

            int userId = UserSession.CurrentUserID;
            bool previousLevelCompleted = true;

            foreach (var level in alleLevels)
            {
                int progress = gebruikerDAL.GetProgressForLevel(userId, level.LevelID);
                level.ProgressPercentage = progress;
                level.ProgressDisplay = $"{progress}% completed";
                level.IsUnlocked = previousLevelCompleted;
                Levels.Add(level);
                previousLevelCompleted = level.ProgressPercentage == 100;
            }

            // Notify pages that levels are loaded
            OnPropertyChanged(nameof(Levels));
        }

        public void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            var selectedLevel = comboBox.SelectedItem as Level;
            if (selectedLevel != null && selectedLevel.IsUnlocked)
            {
                LoadModuleOverzichtspagina(selectedLevel.LevelID);
                comboBox.SelectedIndex = -1; // Reset selection after navigation
            }
        }



    }
}
