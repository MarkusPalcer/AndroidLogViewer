using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AndroidLogViewer.Annotations;
using AndroidLogViewer.Filters.ParameterSelector;

namespace AndroidLogViewer.Filters.Predicate
{
    public abstract class FilterPredicate<TIn, TParameter> : IFilterPredicate<TIn, TParameter>
    {
        private readonly Func<ParameterSelector<TParameter>> parameterSelectorFactory;

        protected FilterPredicate(Func<ParameterSelector<TParameter>> parameterSelectorFactory)
        {
            this.parameterSelectorFactory = parameterSelectorFactory;
        }

        public abstract string Name { get; }

        public Func<IParameterSelector> ParameterSelector => parameterSelectorFactory;

        public abstract bool Evaluate(TIn data, TParameter parameter);

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion 
    }
}