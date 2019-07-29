using System;
using AndroidLogViewer.Filters.ParameterSelector;

namespace AndroidLogViewer.Filters.Predicate
{
    public class IsNotPredicate<T> : FilterPredicate<T,T>
    {
        public IsNotPredicate(Func<ParameterSelector<T>> parameterSelectorFactory) : base(parameterSelectorFactory)
        {
        }

        public override string Name => "is not";

        public override bool Evaluate(T data, T parameter)
        {
            return !Equals(data, parameter);
        }
    }
}