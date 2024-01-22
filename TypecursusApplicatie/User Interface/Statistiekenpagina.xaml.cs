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

        private void LoadStatistiekenData()
        {
            if (!UserSession.IsLoggedIn())
            {
                // Handle the case when the user is not logged in
                return;
            }

            int userId = UserSession.CurrentUserID;
            var gebruiker = gebruikerDAL.GetGebruikerById(userId);
            if (gebruiker == null)
            {
                // Handle user not found scenario
                return;
            }

            // Populate user details in UI
            VoornaamTextBlock.Text = gebruiker.Voornaam;
            AchternaamTextBlock.Text = gebruiker.Achternaam;
            EmailadresTextBlock.Text = gebruiker.Emailadres;

            var modulePogingen = gebruikerDAL.GetModulePogingenByUserId(userId);
            LoadWpmChartData(modulePogingen); // Pass the list to the method

            // Load completed modules count
            var completedModules = gebruikerDAL.GetCompletedModulesByUserId(userId);
            CompletedModulesCount.Text = completedModules.ToString();

            // Load earned badges count
            var earnedBadges = gebruikerDAL.GetEarnedBadgesCountByUserId(userId);
            CompletedLevelsCount.Text = earnedBadges.ToString();

            // Load module completion data
            int completedModulesCount = gebruikerDAL.GetCompletedModulesByUserId(userId);
            CompletedModulesCount.Text = completedModulesCount.ToString();

            // Load badge data
            int earnedBadgesCount = gebruikerDAL.GetEarnedBadgesCountByUserId(userId);
            CompletedLevelsCount.Text = earnedBadgesCount.ToString();

            pogingen = gebruikerDAL.GetModulePogingenByUserId(userId).ToList();
            LoadWpmChartData(pogingen);


        }

        private int dataPointDisplayRange = 20; // Number of points to display at once
        private int currentDataPointStartIndex = 0; // Starting index for the displayed data points

        private void LoadWpmChartData(IEnumerable<ModulePogingen> pogingen)
        {
            var sortedPogingen = pogingen.Where(p => p.GebruikersWPM > 0)
                                         .OrderByDescending(p => p.PogingDatum) // Changed to OrderByDescending
                                         .ToList();
            this.pogingen = sortedPogingen;

            UpdateVisibleDataPoints();
        }


        private void UpdateVisibleDataPoints()
        {
            int endIndex = Math.Min(currentDataPointStartIndex + dataPointDisplayRange, pogingen.Count);
            var visiblePogingen = pogingen.GetRange(currentDataPointStartIndex, endIndex - currentDataPointStartIndex);

            var chartValues = new ChartValues<double>(visiblePogingen.Select(p => (double)p.GebruikersWPM));

            // Combine date and time for labels
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
            WpmChart.AxisX.First().Labels = combinedLabels; // Assigning combined date and time labels
        }


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




        // The rest of your event handlers and methods should go here

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
                // Reset the selection if the level is locked
                comboBox.SelectedIndex = -1;
            }
        }
    }
}