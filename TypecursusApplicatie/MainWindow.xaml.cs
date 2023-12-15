using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;

namespace TypecursusApplicatie
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private bool _isUserLoggedIn = false;

        public bool IsUserLoggedIn
        {
            get { return _isUserLoggedIn; }
            set
            {
                _isUserLoggedIn = value;
                OnPropertyChanged(nameof(IsUserLoggedIn));
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            LoadLoginControl();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
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

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            this.LoadHomeControl();
        }

        private void Logo_Click(object sender, RoutedEventArgs e)
        {
            this.LoadHomeControl();
        }
    }
}