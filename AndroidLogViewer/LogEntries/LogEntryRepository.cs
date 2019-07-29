using System;
using PantherDI.Attributes;

namespace AndroidLogViewer.LogEntries
{
    [Contract]
    public interface ILogEntryRepository
    {
        LogEntry[] LogEntries { get; set; }
        event EventHandler LogEntriesChanged;
    }

    /// <summary>
    /// This class holds the current log entries in memory so they can be accessed throughout the application
    /// </summary>
    [Singleton]
    public class LogEntryRepository : ILogEntryRepository
    {
        private LogEntry[] logEntries = new LogEntry[0];

        public LogEntry[] LogEntries
        {
            get => logEntries;
            set
            {
                if (value == logEntries) return;

                logEntries = value;
                OnLogEntriesChanged();
            }
        }

        public event EventHandler LogEntriesChanged;

        protected virtual void OnLogEntriesChanged()
        {
            LogEntriesChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}