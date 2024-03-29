﻿using System;
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

        // Methode om gebruikersinformatie te laden
        private void LoadUserInfo()
        {
            if (!UserSession.IsLoggedIn())
            {
                return;
            }
            // Haalt de gebruiker op uit de database
            int userId = UserSession.CurrentUserID;
            var gebruiker = gebruikerDAL.GetGebruikerById(userId);
            if (gebruiker == null)
            {
                return;
            }

            VoornaamTextBlock.Text = gebruiker.Voornaam;
            AchternaamTextBlock.Text = gebruiker.Achternaam;
            EmailadresTextBlock.Text = gebruiker.Emailadres;

            // Haalt de voltooide modules en verdiende badges op
            var completedModules = gebruikerDAL.GetCompletedModulesByUserId(userId);
            CompletedModulesCount.Text = completedModules.ToString();

            var earnedBadges = gebruikerDAL.GetEarnedBadgesCountByUserId(userId);
            CompletedLevelsCount.Text = earnedBadges.ToString();

            int completedModulesCount = gebruikerDAL.GetCompletedModulesByUserId(userId);
            CompletedModulesCount.Text = completedModulesCount.ToString();

            int earnedBadgesCount = gebruikerDAL.GetEarnedBadgesCountByUserId(userId);
            CompletedLevelsCount.Text = earnedBadgesCount.ToString();
        }

        // Methode om een BLOB om te zetten naar een BitmapImage
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
                    image.Freeze();  
                    return image;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error converting blob to image: " + ex.Message);
                return null;
            }
        }

        // Methode om de badges te laden
        private void LoadBadges()
        {
            int userId = UserSession.CurrentUserID;
            var allBadges = gebruikerDAL.GetAllBadges();
            var earnedBadgeIds = gebruikerDAL.GetBadgesByUserId(userId).Select(b => b.BadgeID).ToList();

            BadgeViewModels = new ObservableCollection<BadgeViewModel>(
                allBadges.Select(badge => new BadgeViewModel(badge)
                {
                    ImageSource = earnedBadgeIds.Contains(badge.BadgeID)
                        ? ConvertBlobToImage(badge.BadgeAfbeelding) 
                        : ConvertBlobToGrayscaleImage(badge.BadgeAfbeelding) 
                }));

            BadgesList.ItemsSource = BadgeViewModels;
        }


        // Methode om een BLOB om te zetten naar een BitmapImage in grijstinten
        private BitmapImage ConvertBlobToGrayscaleImage(byte[] blob)
        {
            var colorImage = ConvertBlobToImage(blob);
            if (colorImage != null)
            {
                return GrayscaleConverter.ConvertImageToGrayscale(colorImage);
            }
            else
            {
                return null;
            }
        }

        // Methode om de badges te updaten
        private void UpdateBadgesDisplay()
        {
            int userId = UserSession.CurrentUserID;
            var userPerformance = gebruikerDAL.GetLatestPerformanceForUser(userId); 
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
                    if (CheckCriteria(userPerformance, badge.Criteria)) 
                    {
                        badge.IsUnlocked = true;
                        gebruikerDAL.AddBadgeToUser(userId, badge.BadgeID);
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

        // Methode om een BLOB om te zetten naar een BitmapImage in grijstinten
        private BitmapImage ConvertStringToGrayscaleImage(byte[] blob)
        {
            var image = ConvertBlobToImage(blob);
            return GrayscaleConverter.ConvertImageToGrayscale(image);
        }

        // Methode om de criteria van modules te controleren
        private bool CheckCriteria(UserPerformance userPerformance, string criteria)
        {
            return true;
        }

        // Methode om een string om te zetten naar een BitmapImage
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
                Console.WriteLine("Invalid Base64 string: " + ex.Message);
                return new BitmapImage(new Uri("/Images/20WPM.png", UriKind.RelativeOrAbsolute));
            }
        }

        // Event handlers voor de zijbalk en logo
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
                comboBox.SelectedIndex = -1;
            }
        }
    }
}