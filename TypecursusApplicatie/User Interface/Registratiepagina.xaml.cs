using System.Windows;

namespace TypecursusApplicatie
{
    public partial class Registratiepagina : Window
    {
        public Registratiepagina()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Inlogpagina inlogPagina = new Inlogpagina();
            inlogPagina.Show();
            this.Close();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            Registratiepagina registratiepagina = new Registratiepagina();
            registratiepagina.Show();
            this.Close();
        }

        private void SidebarToggle_Click(object sender, RoutedEventArgs e)
        {
            Sidebar.Width = Sidebar.Width == 0 ? 250 : 0;
        }
    }
}
