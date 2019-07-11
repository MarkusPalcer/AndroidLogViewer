using System.Collections.Generic;
using System.ComponentModel;
using AndroidLogViewer.Filters.ParameterSelector;
using AndroidLogViewer.Filters.Predicate;
using AndroidLogViewer.Filters.Source;

namespace AndroidLogViewer.Filters
{
    public interface IFilterEditorViewModel : INotifyPropertyChanged
    {
        IEnumerable<IFilterSource> FilterSources { get; }

        IFilterSource SelectedFilterSource { get; set; }

        IEnumerable<IFilterPredicate> FilterPredicates { get; }

        IFilterPredicate SelectedFilterPredicate { get; set; }

        IParameterSelector ParameterSelector { get; }
    }
}