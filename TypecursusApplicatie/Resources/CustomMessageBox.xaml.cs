using System.Windows;

namespace TypecursusApplicatie
{
    // Definitie van de klasse CustomMessageBox, die erft van Window
    public partial class CustomMessageBox : Window
    {
        // Constructor van CustomMessageBox met een parameter voor het bericht
        public CustomMessageBox(string message)
        {
            InitializeComponent(); // Initialiseert de XAML-componenten
            MessageText.Text = message; // Zet de tekst van het bericht in de TextBlock van de XAML
        }

        // Event handler voor de klikactie van de OK knop
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true; // Stelt het DialogResult in op true, wat het venster sluit
        }
    }
}
