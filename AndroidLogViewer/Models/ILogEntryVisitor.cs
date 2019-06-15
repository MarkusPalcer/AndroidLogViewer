namespace AndroidLogViewer.Models
{
    public interface ILogEntryVisitor
    {
        void Visit(LogEntry entry);
        void Visit(StartOfBufferEntry entry);
    }
}