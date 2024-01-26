using System.ComponentModel;
using System.Windows.Media;
using TypecursusApplicatie.Models;

namespace TypecursusApplicatie.ViewModels
{
    public class BadgeViewModel : INotifyPropertyChanged
    {
        private Badge _badge;

        public Badge Badge
        {
            get => _badge;
            set
            {
                _badge = value;
                OnPropertyChanged(nameof(Badge));
            }
        }

        public ImageSource ImageSource
        {
            get => _badge.ImageSource;
            set
            {
                _badge.ImageSource = value;
                OnPropertyChanged(nameof(ImageSource));
            }
        }

        public string TooltipText { get; set;}

        public string Tooltip => $"{_badge.BadgeNaam}: {_badge.BadgeBeschrijving}";

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public BadgeViewModel(Badge badge)
        {
            _badge = badge;
        }
    }
}
