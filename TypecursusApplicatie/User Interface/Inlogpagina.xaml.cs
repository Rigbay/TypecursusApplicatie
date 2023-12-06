using System.Windows;

namespace TypecursusApplicatie
{
    public partial class Inlogpagina : Window
    {
        public Inlogpagina()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Implementeer inloglogica hier
        }

        private void SidebarToggle_Click(object sender, RoutedEventArgs e)
        {
            // Toggle de breedte van de sidebar
            Sidebar.Width = Sidebar.Width == 0 ? 200 : 0;
        }
    }
}
