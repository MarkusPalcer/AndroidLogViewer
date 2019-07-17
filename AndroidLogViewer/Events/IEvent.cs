namespace AndroidLogViewer.Events
{
    public interface IEvent { }

    public abstract class Event : IEvent { }

    public interface IEvent<TEventArgs> 
    {
    }

    public abstract class Event<TEventArgs> : IEvent<TEventArgs>
    {
    }
}