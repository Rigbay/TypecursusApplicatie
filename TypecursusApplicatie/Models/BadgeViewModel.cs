using System.ComponentModel;
using System.Windows.Media;
using TypecursusApplicatie.Models;

namespace TypecursusApplicatie.ViewModels
{
    // BadgeViewModel klasse, bedoeld voor de weergave van badgegegevens in de gebruikersinterface
    public class BadgeViewModel : INotifyPropertyChanged
    {
        private Badge _badge; // Privéveld voor de Badge

        // Eigenschap Badge, krijgt en zet de Badge. Bij het zetten wordt OnPropertyChanged aangeroepen
        public Badge Badge
        {
            get => _badge;
            set
            {
                _badge = value;
                OnPropertyChanged(nameof(Badge));
            }
        }

        // Eigenschap ImageSource, krijgt en zet de afbeeldingsbron van de badge. Bij het zetten wordt OnPropertyChanged aangeroepen
        public ImageSource ImageSource
        {
            get => _badge.ImageSource;
            set
            {
                _badge.ImageSource = value;
                OnPropertyChanged(nameof(ImageSource));
            }
        }

        // TooltipText voor extra informatie, kan handmatig gezet worden
        public string TooltipText { get; set; }

        // Tooltip eigenschap, geeft een samengestelde string terug met de naam en beschrijving van de badge
        public string Tooltip => $"{_badge.BadgeNaam}: {_badge.BadgeBeschrijving}";

        // Event PropertyChanged, onderdeel van het INotifyPropertyChanged interface om de UI te updaten bij veranderingen
        public event PropertyChangedEventHandler PropertyChanged;

        // OnPropertyChanged methode, roept PropertyChanged event aan. Wordt aangeroepen als een eigenschap verandert
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Constructor van BadgeViewModel, initialiseert een nieuwe instantie van Badge
        public BadgeViewModel(Badge badge)
        {
            _badge = badge;
        }
    }
}
