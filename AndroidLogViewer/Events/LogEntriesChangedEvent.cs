using System.Collections.Generic;

namespace AndroidLogViewer.Events
{
    public class LogEntriesChangedEvent : Event<IEnumerable<LogEntry>> { }
}