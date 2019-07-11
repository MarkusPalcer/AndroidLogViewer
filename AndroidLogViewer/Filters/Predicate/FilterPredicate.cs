using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AndroidLogViewer.Annotations;
using AndroidLogViewer.Filters.ParameterSelector;

namespace AndroidLogViewer.Filters.Predicate
{
    public abstract class FilterPredicate<TIn, TParameter> : IFilterPredicate<TIn, TParameter>
    {
        private readonly ParameterSelector<TParameter> parameterSelector;

        protected FilterPredicate(ParameterSelector<TParameter> parameterSelector)
        {
            this.parameterSelector = parameterSelector;
        }

        public abstract string Name { get; }

        public IParameterSelector ParameterSelector => parameterSelector;

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