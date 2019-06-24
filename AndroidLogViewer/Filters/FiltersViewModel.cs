using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using AndroidLogViewer.Annotations;

namespace AndroidLogViewer.Filters
{
    public class FiltersViewModel : INotifyPropertyChanged
    {
        public FiltersViewModel()
        {
            newFilterViewModel = new NewFilterViewModel();
        }

        private NewFilterViewModel newFilterViewModel;

        public NewFilterViewModel NewFilterViewModel
        {
            get => newFilterViewModel;
            private set
            {
                if (Equals(value, newFilterViewModel)) return;
                newFilterViewModel = value;
                OnPropertyChanged();
            }
        }



        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}