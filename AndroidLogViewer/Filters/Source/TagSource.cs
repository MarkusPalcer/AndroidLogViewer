using System.Collections.Generic;
using AndroidLogViewer.Filters.ParameterSelector;
using AndroidLogViewer.Filters.Predicate;
using PantherDI.Attributes;

namespace AndroidLogViewer.Filters.Source
{
    [Singleton]
    public class TagSource : FilterSource<string>
    {
        public TagSource()
        {
            AvailablePredicates = new IFilterPredicate[]
            {
                new IsPredicate<string>(new ListSelector<string>(this)), 
                new IsNotPredicate<string>(new ListSelector<string>(this)), 
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