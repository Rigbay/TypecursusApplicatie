using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;
using TypecursusApplicatie.Models;
using TypecursusApplicatie.BusinessLogicLayer;
using TypecursusApplicatie.Data_Access_Layer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;  // For RoutedEventArgs and MouseButtonEventArgs
using System.Windows.Input;  // For MouseButtonEventArgs

namespace TypecursusApplicatie
{
    public partial class Statistiekenpagina : UserControl
    {
        private MainWindow mainWindow;
        private GebruikerDAL gebruikerDAL;
        private List<ModulePogingen> pogingen;

        public SeriesCollection WpmSeries { get; set; }
        public List<string> TimeLabels { get; set; }

        public Statistiekenpagina()
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.DataContext = mainWindow;
            DataContext = this;
            gebruikerDAL = new GebruikerDAL();

            LoadStatistiekenData();
        }

        public Statistiekenpagina(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.DataContext = mainWindow;
            DataContext = this;
            gebruikerDAL = new GebruikerDAL();

            LoadStatistiekenData();
        }
        
        // Methode die alle statistieken van de gebruiker laadt
        private void LoadStatistiekenData()
        {
            if (!UserSession.IsLoggedIn())
            {
                return;
            }

            int userId = UserSession.CurrentUserID;
            var gebruiker = gebruikerDAL.GetGebruikerById(userId);
            if (gebruiker == null)
            {
                return;
            }

            VoornaamTextBlock.Text = gebruiker.Voornaam;
            AchternaamTextBlock.Text = gebruiker.Achternaam;
            EmailadresTextBlock.Text = gebruiker.Emailadres;

            var modulePogingen = gebruikerDAL.GetModulePogingenByUserId(userId);
            LoadWpmChartData(modulePogingen); 

            var completedModules = gebruikerDAL.GetCompletedModulesByUserId(userId);
            CompletedModulesCount.Text = completedModules.ToString();

            var earnedBadges = gebruikerDAL.GetEarnedBadgesCountByUserId(userId);
            CompletedLevelsCount.Text = earnedBadges.ToString();

            int completedModulesCount = gebruikerDAL.GetCompletedModulesByUserId(userId);
            CompletedModulesCount.Text = completedModulesCount.ToString();

            int earnedBadgesCount = gebruikerDAL.GetEarnedBadgesCountByUserId(userId);
            CompletedLevelsCount.Text = earnedBadgesCount.ToString();

            pogingen = gebruikerDAL.GetModulePogingenByUserId(userId).ToList();
            LoadWpmChartData(pogingen);


        }

        private int dataPointDisplayRange = 20; // Aantal datapunten dat tegelijkertijd wordt weergegeven
        private int currentDataPointStartIndex = 0;  // Index van het eerste datapunt dat wordt weergegeven

        private void LoadWpmChartData(IEnumerable<ModulePogingen> pogingen)
        {
            var sortedPogingen = pogingen.Where(p => p.GebruikersWPM > 0)
                                         .OrderByDescending(p => p.PogingDatum) 
                                         .ToList();
            this.pogingen = sortedPogingen;

            UpdateVisibleDataPoints();
        }

        // Methode die de grafiek van de WPM laadt
        private void UpdateVisibleDataPoints()
        {
            int endIndex = Math.Min(currentDataPointStartIndex + dataPointDisplayRange, pogingen.Count);
            var visiblePogingen = pogingen.GetRange(currentDataPointStartIndex, endIndex - currentDataPointStartIndex);

            var chartValues = new ChartValues<double>(visiblePogingen.Select(p => (double)p.GebruikersWPM));

            var combinedLabels = visiblePogingen.Select(p => p.PogingDatum.ToString("dd-MM-yyyy HH:mm")).ToList();

            WpmSeries = new SeriesCollection
        {
        new LineSeries
        {
            Values = chartValues,
            Title = "",
            DataLabels = false,
            LabelPoint = point => $"WPM {point.Y}",
            PointGeometry = DefaultGeometries.Circle,
            PointGeometrySize = 10
        }
        };

            WpmChart.Series = WpmSeries;
            WpmChart.AxisX.First().Labels = combinedLabels;
        }

        // Methodes voor de knoppen van de grafiek
        private void PreviousDataPointsButton_Click(object sender, RoutedEventArgs e)
        {
            currentDataPointStartIndex = Math.Max(0, currentDataPointStartIndex - dataPointDisplayRange);
            UpdateVisibleDataPoints();
        }

        private void NextDataPointsButton_Click(object sender, RoutedEventArgs e)
        {
            currentDataPointStartIndex = Math.Min(pogingen.Count - dataPointDisplayRange, currentDataPointStartIndex + dataPointDisplayRange);
            UpdateVisibleDataPoints();
        }


        // De event handlers voor de sidebar en logo

        private void Homepagina_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindow main)
            {
                mainWindow = main;
            }
        }

        private void BadgesButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(mainWindow.MainContent.Content is BadgeOverzichtspagina))
            {
                mainWindow.LoadBadgeOverzichtspagina();
            }
        }  

        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(mainWindow.MainContent.Content is Statistiekenpagina))
            {
                mainWindow.LoadAccountControl();
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