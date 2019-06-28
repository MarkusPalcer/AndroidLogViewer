using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AndroidLogViewer.Annotations;

namespace AndroidLogViewer.Filters.Predicate
{
    public class FilterPredicate<TIn> : INotifyPropertyChanged
    {
        public String Name { get; set; }

        public ParameterSelector.ParameterSelector<TIn> ParameterSelector { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}