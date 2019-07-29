using System;
using System.Collections.Generic;
using AndroidLogViewer.Filters.ParameterSelector;
using AndroidLogViewer.Filters.Predicate;
using PantherDI.Attributes;

namespace AndroidLogViewer.Filters.Source
{
    [Singleton]
    public class ProcessSource : FilterSource<int>
    {
        public ProcessSource(Func<FilterSource<int>, ListSelector<int>> listSelectorFactory)
        {
            AvailablePredicates = new IFilterPredicate[]
            {
                new IsPredicate<int>(()=>listSelectorFactory(this)), 
                new IsNotPredicate<int>(()=>listSelectorFactory(this)), 
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