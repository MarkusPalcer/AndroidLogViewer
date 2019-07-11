using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using AndroidLogViewer.Annotations;
using AndroidLogViewer.Filters.ParameterSelector;
using AndroidLogViewer.Filters.Predicate;
using AndroidLogViewer.Filters.Source;

namespace AndroidLogViewer.Filters
{
    public class FilterEditorViewModel : IFilterEditorViewModel
    {
        private IFilterSource selectedFilterSource;
        private IFilterPredicate selectedFilterPredicate;

        public FilterEditorViewModel()
        {
            FilterSources = new IFilterSource[]
            {
                new ProcessSource(), 
                new TagSource(), 
            };
        }

        public IEnumerable<IFilterSource> FilterSources { get; }

        public IFilterSource SelectedFilterSource
        {
            get => selectedFilterSource;
            set
            {
                if (Equals(value, selectedFilterSource)) return;
                selectedFilterSource = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FilterPredicates));
            }
        }

        public IEnumerable<IFilterPredicate> FilterPredicates => SelectedFilterSource?.AvailablePredicates ?? Enumerable.Empty<IFilterPredicate>();

        public IFilterPredicate SelectedFilterPredicate
        {
            get => selectedFilterPredicate;
            set
            {
                if (Equals(value, selectedFilterPredicate)) return;
                selectedFilterPredicate = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ParameterSelector));
            }
        }

        public IParameterSelector ParameterSelector => SelectedFilterPredicate?.ParameterSelector;

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