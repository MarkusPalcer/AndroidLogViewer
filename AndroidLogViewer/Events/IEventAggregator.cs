using System;
using PantherDI.Attributes;

namespace AndroidLogViewer.Events
{
    [Contract]
    public interface IEventAggregator
    {
        IDisposable Subscribe<TEvent, TEventArgs>(Action<TEventArgs> eventHandler) where TEvent : IEvent<TEventArgs>;
        IDisposable Subscribe<TEvent>(Action eventHandler) where TEvent : IEvent;

        void Raise<TEvent, TEventArgs>(TEventArgs arguments) where TEvent : IEvent<TEventArgs>;
        void Raise<TEvent>() where TEvent : IEvent;
    }
}