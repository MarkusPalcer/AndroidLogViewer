using System;
using System.Collections.Generic;
using AndroidLogViewer.Models;

namespace AndroidLogViewer.Dialogs.Export
{
    public class AlignedLogExportVisitor : ILogEntryVisitor
    {
        private readonly WidthCountingVisitor _widthCounter;
        public List<string> LogLines { get; } = new List<string>();

        public void Visit(IEnumerable<LogEntry> entries)
        {
            foreach (var entry in entries)
            {
                entry.Accept(this);
            }
        }

        public AlignedLogExportVisitor(WidthCountingVisitor widthCounter)
        {
            this._widthCounter = widthCounter;
        }

        public void Visit(LogEntry x)
        {
            LogLines.Add($"{x.Time.PadRight(_widthCounter.TimestampWidth)} {x.Process.ToString().PadRight(_widthCounter.ProcessIdWidth)} {x.Thread.ToString().PadRight(_widthCounter.ThreadIdWidth)} {x.Level} {$"{x.Tag}:".PadRight(_widthCounter.TagWidth)} {x.Message}");
        }

        public void Visit(StartOfBufferEntry entry)
        {
             LogLines.Add($"{new string(' ', _widthCounter.TimestampWidth + _widthCounter.ProcessIdWidth + _widthCounter.ThreadIdWidth + _widthCounter.TagWidth + 5)} {entry.Tag} {entry.Message}");
        }
    }
}