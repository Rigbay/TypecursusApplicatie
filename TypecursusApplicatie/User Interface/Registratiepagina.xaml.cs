﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TypecursusApplicatie
{
    public partial class Registratiepagina : UserControl
    {
        private MainWindow mainWindow;

        public Registratiepagina()
        {
            InitializeComponent();
            this.Loaded += Registratiepagina_Loaded;
        }

        public Registratiepagina(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private void Registratiepagina_Loaded(object sender, RoutedEventArgs e)
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

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Hier komt de logica voor het registreren
            // Bij succes, navigeer naar een andere pagina of update de UI
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
