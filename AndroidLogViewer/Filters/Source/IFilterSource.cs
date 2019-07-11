using System.Collections.Generic;
using System.ComponentModel;
using AndroidLogViewer.Filters.Predicate;

namespace AndroidLogViewer.Filters.Source
{
    public interface IFilterSource : INotifyPropertyChanged
    {
        string Name { get; }

        IEnumerable<IFilterPredicate> AvailablePredicates { get; }
    }
    public interface IFilterSource<TOut> : IFilterSource
    {
        TOut TransformLogEntry(LogEntry entry);
    }
}