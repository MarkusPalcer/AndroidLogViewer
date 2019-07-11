using AndroidLogViewer.Filters.ParameterSelector;

namespace AndroidLogViewer.Filters.Predicate
{
    public class IsNotPredicate<T> : FilterPredicate<T,T>
    {
        public IsNotPredicate(ParameterSelector<T> parameterSelector) : base(parameterSelector)
        {
        }

        public override string Name => "is not";

        public override bool Evaluate(T data, T parameter)
        {
            return !Equals(data, parameter);
        }
    }
}