using System.Collections.Generic;
using System.ComponentModel;
using AndroidLogViewer.Filters.Predicate;
using PantherDI.Attributes;

namespace AndroidLogViewer.Filters.Source
{
    [Contract]
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