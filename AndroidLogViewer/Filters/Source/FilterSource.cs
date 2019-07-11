using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AndroidLogViewer.Annotations;
using AndroidLogViewer.Filters.Predicate;

namespace AndroidLogViewer.Filters.Source
{
    public abstract class FilterSource<TOut> : IFilterSource<TOut>
    {
        private IFilterPredicate selectedPredicate;


        public abstract string Name { get; }

        public abstract TOut TransformLogEntry(LogEntry entry);

        public abstract IEnumerable<IFilterPredicate> AvailablePredicates { get; }

        public IFilterPredicate SelectedPredicate
        {
            get => selectedPredicate;
            set
            {
                if (Equals(value, selectedPredicate)) return;
                selectedPredicate = value;
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