using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AndroidLogViewer.Filters.Source;

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
        private HashSet<T> availableValues;
        private object selectedValue;

        public ListSelector(IFilterSource<T> source)
        {
            this.source = source;
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

        public override void RefreshSources(IEnumerable<LogEntry> logEntries)
        {
            availableValues = new HashSet<T>(logEntries.Select(source.TransformLogEntry));
            OnPropertyChanged(nameof(AvailableValues));
        }
    }
}