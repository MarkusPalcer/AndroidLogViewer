using System.Collections.Generic;
using System.ComponentModel;

namespace AndroidLogViewer.Filters.ParameterSelector
{
    public interface IParameterSelector : INotifyPropertyChanged
    {
        bool IsParameterValid { get; }

        void RefreshSources(IEnumerable<LogEntry> logEntries);
    }

    public interface IParameterSelector<out TParameter> : IParameterSelector
    {
        TParameter Parameter { get; }
    }
}