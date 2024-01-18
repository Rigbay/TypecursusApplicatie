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
    public partial class Homepagina : UserControl
    {
        private MainWindow mainWindow;

        public Homepagina()
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.DataContext = mainWindow;
        }

        public Homepagina(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.DataContext = mainWindow;
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
