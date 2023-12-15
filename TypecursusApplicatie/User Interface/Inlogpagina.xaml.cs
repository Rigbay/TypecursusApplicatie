using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TypecursusApplicatie
{
    public partial class Inlogpagina : UserControl
    {
        private MainWindow mainWindow;

        public Inlogpagina()
        {
            InitializeComponent();
            this.Loaded += Inlogpagina_Loaded;
        }

        public Inlogpagina(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private void Inlogpagina_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is MainWindow main)
            {
                mainWindow = main;
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Hier komt de logica voor het inloggen
            // Bij succes, navigeer naar een andere pagina of update de UI
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.LoadRegisterControl();

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
