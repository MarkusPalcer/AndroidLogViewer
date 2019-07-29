using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using AndroidLogViewer.Filters.Source;
using AndroidLogViewer.LogEntries;

namespace AndroidLogViewer.Filters.ParameterSelector
{
    /// <summary>
    /// A <code>ParameterSelector</code> which lets the user select an entry from a list (interface for data templating in WPF)
    /// </summary>
    public interface IListSelector : INotifyPropertyChanged
    {
        IEnumerable AvailableValues { get; }
        object SelectedValue { get; set; }
    }

    /// <summary>
    /// A <code>ParameterSelector</code> which lets the user select an entry from a list
    /// </summary>
    public class ListSelector<T> : ParameterSelector<T>, IListSelector
    {
        private readonly IFilterSource<T> source;
        private readonly ILogEntryRepository logEntryRepository;
        private HashSet<T> availableValues;
        private object selectedValue;

        public ListSelector(IFilterSource<T> source, ILogEntryRepository logEntryRepository)
        {
            this.source = source;
            this.logEntryRepository = logEntryRepository;
            RefreshSources();
            WeakEventManager<ILogEntryRepository, EventArgs>.AddHandler(
                logEntryRepository,
                nameof(logEntryRepository.LogEntriesChanged), 
                (_, __) => RefreshSources());

            OnParameterValueChanged += (sender, args) =>
            {
                IsParameterValid = availableValues.Contains(Parameter);
                SelectedValue = Parameter;
            };
        }

        public IEnumerable AvailableValues => availableValues;

        public object SelectedValue
        {
            get => selectedValue;
            set
            {
                if (Equals(value, selectedValue)) return;
                selectedValue = value;
                OnPropertyChanged();
                Parameter = (T) selectedValue;
            }
        }

        public void SelectParameterFrom(LogEntry entry)
        {
            SelectedValue = source.TransformLogEntry(entry);
        }

        public void RefreshSources()
        {
            availableValues = new HashSet<T>(logEntryRepository.LogEntries.Select(source.TransformLogEntry));
            OnPropertyChanged(nameof(AvailableValues));
        }
    }
}