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

            // Load module and level completion data
            var modulePogingen = gebruikerDAL.GetModulePogingenByUserId(userId);
            CompletedModulesCount.Text = modulePogingen.Count().ToString();
            CompletedLevelsCount.Text = gebruikerDAL.GetCompletedLevelsCount(userId).ToString();

            LoadWpmChartData(modulePogingen);
        }

        private void LoadWpmChartData(IEnumerable<ModulePogingen> pogingen)
        {
            var groupedPogingen = pogingen.GroupBy(p => p.PogingDatum.Date)
                                          .Select(group => new
                                          {
                                              Date = group.Key,
                                              AverageWpm = group.Average(p => p.GebruikersWPM)
                                          })
                                          .OrderBy(p => p.Date)
                                          .ToList();

            ChartValues<double> chartValues = new ChartValues<double>(groupedPogingen.Select(p => p.AverageWpm));
            TimeLabels = groupedPogingen.Select(p => p.Date.ToShortDateString()).ToList();

            WpmSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Values = chartValues,
                    Title = "Average WPM"
                }
            };

            WpmChart.Series = WpmSeries;
            WpmChart.AxisX.First().Labels = TimeLabels;
        }

        // The rest of your event handlers and methods should go here

        private void Homepagina_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindow main)
            {
                mainWindow = main;
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
