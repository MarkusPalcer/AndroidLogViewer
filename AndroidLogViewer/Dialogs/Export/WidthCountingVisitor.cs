using System;
using System.Collections.Generic;
using AndroidLogViewer.Models;

namespace AndroidLogViewer.Dialogs.Export
{
    public class WidthCountingVisitor : ILogEntryVisitor
    {
        public int TimestampWidth { get; private set; }
        public int ProcessIdWidth { get; private set; }
        public int ThreadIdWidth { get; private set; }
        public int TagWidth { get; private set; }   
        public int MessageWidth { get; private set; }

        public void Visit(IEnumerable<LogEntry> entries)
        {
            foreach (var entry in entries)
            {
                entry.Accept(this);
            }
        }

        public void Visit(LogEntry entry)
        {
            TimestampWidth = Math.Max(TimestampWidth, entry.Time.Length);
            ProcessIdWidth = Math.Max(ProcessIdWidth, entry.Process.ToString().Length);
            ThreadIdWidth = Math.Max(ThreadIdWidth, entry.Thread.ToString().Length);
            TagWidth = Math.Max(TagWidth, entry.Tag.Length);
            MessageWidth = Math.Max(MessageWidth, entry.Message.Length);
        }

        public void Visit(StartOfBufferEntry entry)
        {
            MessageWidth = Math.Max(MessageWidth, entry.Message.Length);
        }
    }
}