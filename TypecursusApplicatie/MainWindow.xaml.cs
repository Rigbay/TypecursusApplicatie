﻿using System.Windows;
using System.ComponentModel;
using TypecursusApplicatie.BusinessLogicLayer;

namespace TypecursusApplicatie
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            LoadLoginControl();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsUserLoggedIn => UserSession.IsLoggedIn();

        public bool IsNotLoggedIn => !IsUserLoggedIn;

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
            MainContent.Content = new Levelspagina();
        }

        public void LoadHomeControl()
        {
            MainContent.Content = new Homepagina(this);
        }

        public void LogoutUser()
        {
            UserSession.Logout();
            OnPropertyChanged(nameof(IsUserLoggedIn));
            OnPropertyChanged(nameof(IsNotLoggedIn));
            LoadLoginControl(); // Terug naar inlogscherm
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            LoadHomeControl();
        }

        private void Logo_Click(object sender, RoutedEventArgs e)
        {
            LoadHomeControl();
        }
    }
}
