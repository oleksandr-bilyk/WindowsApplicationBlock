using System;
using Tampleworks.WindowsApplicationBlock.ViewModel;

namespace Tampleworks.WindowsApplicationBlock.Demo.AppLogic.MainPage
{
    public sealed class OrganisationDetailsViewModel : ViewModelBase
    {
        private bool isFavorite;

        public string Address => "Some Address";

        public bool LogoIsAvaialble => Logo != null;

        public byte[] Logo { get; set; }
        public bool IsFavorite
        {
            get { return isFavorite; }
            set
            {
                if (isFavorite == value) return;
                isFavorite = value;
                OnCallerPropertyChanged();
                OnIsFavoriteChanged();
            }
        }
        public event Action IsFavoriteChanged;
        private void OnIsFavoriteChanged() => IsFavoriteChanged?.Invoke();
    }
}
