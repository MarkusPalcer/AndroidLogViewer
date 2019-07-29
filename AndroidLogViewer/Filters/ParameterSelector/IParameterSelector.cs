using System.Collections.Generic;
using System.ComponentModel;
using AndroidLogViewer.Properties;

namespace AndroidLogViewer.Filters.ParameterSelector
{
    public interface IParameterSelector : INotifyPropertyChanged
    {
        bool IsParameterValid { get; }
    }

    public interface IParameterSelector<out TParameter> : IParameterSelector
    {
        TParameter Parameter { get; }
    }
}