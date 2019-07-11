using System.ComponentModel;
using System.Runtime.CompilerServices;
using AndroidLogViewer.Annotations;

namespace AndroidLogViewer.Filters
{
    public class FiltersViewModel : INotifyPropertyChanged
    {


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