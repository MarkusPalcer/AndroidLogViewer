using System;
using System.ComponentModel;
using AndroidLogViewer.Filters.ParameterSelector;

namespace AndroidLogViewer.Filters.Predicate
{
    public interface IFilterPredicate : INotifyPropertyChanged
    {
        string Name { get; }

        Func<IParameterSelector> ParameterSelector { get; }
    }


    public interface IFilterPredicate<in TIn, in TParameter> : IFilterPredicate
    {
        bool Evaluate(TIn data, TParameter parameter);
    }
}