using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TypecursusApplicatie.BusinessLogicLayer;
using TypecursusApplicatie.Converters;
using TypecursusApplicatie.Data_Access_Layer;
using TypecursusApplicatie.Models;
using TypecursusApplicatie.ViewModels;

namespace TypecursusApplicatie
{
    public partial class BadgeOverzichtspagina : UserControl
    {
        private MainWindow mainWindow;
        private GebruikerDAL gebruikerDAL;

        public ObservableCollection<Badge> Badges { get; set; }
        public ObservableCollection<BadgeViewModel> BadgeViewModels { get; set; }

        public BadgeOverzichtspagina(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            DataContext = this;
            gebruikerDAL = new GebruikerDAL();

            LoadBadges();
            LoadUserInfo();
        }

        private void LoadUserInfo()
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
        }

        // Method to convert a byte array to a BitmapImage
        private BitmapImage ConvertBlobToImage(byte[] blob)
        {
            try
            {
                using (var ms = new MemoryStream(blob))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();
                    image.Freeze();  // Freezing the image for performance improvements
                    return image;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error converting blob to image: " + ex.Message);
                return null;
            }
        }

        // Method to load badges and process images
        private void LoadBadges()
        {
            int userId = UserSession.CurrentUserID;
            var allBadges = gebruikerDAL.GetAllBadges();
            var earnedBadgeIds = gebruikerDAL.GetBadgesByUserId(userId).Select(b => b.BadgeID).ToList();

            BadgeViewModels = new ObservableCollection<BadgeViewModel>(
                allBadges.Select(badge => new BadgeViewModel(badge)
                {
                    ImageSource = earnedBadgeIds.Contains(badge.BadgeID)
                        ? ConvertBlobToImage(badge.BadgeAfbeelding) // Directly use the BLOB data
                        : ConvertBlobToGrayscaleImage(badge.BadgeAfbeelding) // Directly use the BLOB data
                }));

            BadgesList.ItemsSource = BadgeViewModels;
        }


        // Method to apply grayscale effect
        private BitmapImage ConvertBlobToGrayscaleImage(byte[] blob)
        {
            var colorImage = ConvertBlobToImage(blob);
            if (colorImage != null)
            {
                return GrayscaleConverter.ConvertImageToGrayscale(colorImage); // Assuming GrayscaleConverter is implemented correctly
            }
            else
            {
                return null; // Handle null image case
            }
        }

        // Method to update the badges display
        private void UpdateBadgesDisplay()
        {
            int userId = UserSession.CurrentUserID;
            var userPerformance = gebruikerDAL.GetLatestPerformanceForUser(userId); // This is a method to be implemented
            var allBadges = gebruikerDAL.GetAllBadges();
            var userBadges = gebruikerDAL.GetBadgesForUser(userId);

            foreach (var badge in allBadges)
            {
                if (userBadges.Any(ub => ub.BadgeID == badge.BadgeID))
                {
                    badge.IsUnlocked = true;
                }
                else
                {
                    if (CheckCriteria(userPerformance, badge.Criteria)) // CheckCriteria is a method to be implemented
                    {
                        badge.IsUnlocked = true;
                        gebruikerDAL.AddBadgeToUser(userId, badge.BadgeID); // AddBadgeToUser is a method to be implemented
                    }
                    else
                    {
                        badge.IsUnlocked = false;
                    }
                }
            }

            Badges = new ObservableCollection<Badge>(allBadges);
            BadgesList.ItemsSource = Badges;
        }

        


        private BitmapImage ConvertStringToGrayscaleImage(byte[] blob)
        {
            var image = ConvertBlobToImage(blob);
            // Apply grayscale effect if needed
            // Assuming GrayscaleConverter is implemented correctly
            return GrayscaleConverter.ConvertImageToGrayscale(image);
        }


        private bool CheckCriteria(UserPerformance userPerformance, string criteria)
        {
            // Implement the logic to check if the user performance meets the criteria
            return true;
        }


        private BitmapImage ConvertStringToImage(string base64String)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(base64String);
                using (var ms = new MemoryStream(bytes))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();
                    return image;
                }
            }
            catch (FormatException ex)
            {
                // Log the error for debugging purposes
                Console.WriteLine("Invalid Base64 string: " + ex.Message);

                // Optionally return a default/fallback image
                return new BitmapImage(new Uri("/Images/20WPM.png", UriKind.RelativeOrAbsolute));
            }
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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.LoadStatistiekenpagina();
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