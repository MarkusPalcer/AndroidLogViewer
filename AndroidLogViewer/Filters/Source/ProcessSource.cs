using System.Collections.Generic;
using AndroidLogViewer.Filters.ParameterSelector;
using AndroidLogViewer.Filters.Predicate;
using PantherDI.Attributes;

namespace AndroidLogViewer.Filters.Source
{
    [Singleton]
    public class ProcessSource : FilterSource<int>
    {
        public ProcessSource()
        {
            AvailablePredicates = new IFilterPredicate[]
            {
                new IsPredicate<int>(new ListSelector<int>(this)), 
                new IsNotPredicate<int>(new ListSelector<int>(this)), 
            };
        }

        public override string Name => "Process";

        public override int TransformLogEntry(LogEntry entry)
        {
            return entry.Process;
        }

        public override IEnumerable<IFilterPredicate> AvailablePredicates { get; }
    }
}