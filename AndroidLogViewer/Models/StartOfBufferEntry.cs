using System.Data;
using AndroidLogViewer.Models;

namespace AndroidLogViewer
{
    public class StartOfBufferEntry : LogEntry
    {
        public override void Accept(ILogEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}