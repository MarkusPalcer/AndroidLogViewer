using System;
using AndroidLogViewer.Filters.ParameterSelector;

namespace AndroidLogViewer.Filters.Predicate
{
    public class IsPredicate<T> : FilterPredicate<T, T>
    {
        public IsPredicate(Func<ParameterSelector<T>> parameterSelectorFactory) : base(parameterSelectorFactory)
        {
        }

        public override string Name => "is";

        public override bool Evaluate(T data, T parameter)
        {
            return Equals(data, parameter);
        }
    }
}