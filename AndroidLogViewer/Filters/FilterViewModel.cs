using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AndroidLogViewer.Annotations;

namespace AndroidLogViewer.Filters
{
    public class FilterViewModel : INotifyPropertyChanged
    {

        private class FilterSource : INotifyPropertyChanged
        {
            public String Name { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;
        }




        public FilterSource 

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}