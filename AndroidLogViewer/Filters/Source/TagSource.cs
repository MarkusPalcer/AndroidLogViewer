using System;
using System.Collections.Generic;
using AndroidLogViewer.Filters.ParameterSelector;
using AndroidLogViewer.Filters.Predicate;
using PantherDI.Attributes;

namespace AndroidLogViewer.Filters.Source
{
    [Singleton]
    public class TagSource : FilterSource<string>
    {
        public TagSource(Func<FilterSource<string>, ListSelector<string>> listSelectorFactory)
        {
            AvailablePredicates = new IFilterPredicate[]
            {
                new IsPredicate<string>(()=>listSelectorFactory(this)), 
                new IsNotPredicate<string>(()=>listSelectorFactory(this)), 
            };
        }

        public override string Name => "Tag";

        public override string TransformLogEntry(LogEntry entry)
        {
            return entry.Tag;
        }

        public override IEnumerable<IFilterPredicate> AvailablePredicates { get; }
    }
}