using System.Collections.Generic;
using AndroidLogViewer.Models;

namespace AndroidLogViewer.Dialogs.Export
{
    public class SimpleLogExportVisitor :  ILogEntryVisitor
    {
        public List<string> LogLines { get; } = new List<string>();

        public void Visit(IEnumerable<LogEntry> entries)
        {
            foreach (var entry in entries)
            {
                entry.Accept(this);
            }
        }

        public void Visit(LogEntry entry)
        {
            LogLines.Add(FormatLogEntry(entry));
        }

        public void Visit(StartOfBufferEntry x)
        {
            LogLines.Add($"{x.Time}{x.Tag}{x.Message}");
        }

        public static string FormatLogEntry(LogEntry x)
        {
            return $"{x.Time} {x.Process} {x.Thread} {x.Level} {x.Tag}: {x.Message}";
        }
    }
}