using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using AndroidLogViewer.Extensions;
using AndroidLogViewer.Properties;
using PantherDI.Attributes;

namespace AndroidLogViewer.Events
{
    [Singleton]
    public class EventAggregator : IEventAggregator
    {
        private readonly Dictionary<Type, List<Action<object>>> handlers = new Dictionary<Type, List<Action<object>>>();

        public IDisposable Subscribe<TEvent, TEventArgs>([NotNull]Action<TEventArgs> eventHandler) where TEvent : IEvent<TEventArgs>
        {
            if (eventHandler == null) throw new ArgumentNullException(nameof(eventHandler));
            Contract.EndContractBlock();

            List<Action<object>> eventHandlers;

            void Handler(object x) => eventHandler(x is TEventArgs args ? args : default);
            void Remover() => eventHandlers.Remove(Handler);

            if (!handlers.TryGetValue(typeof(TEvent), out eventHandlers))
            {
                eventHandlers = new List<Action<object>>();
                handlers.Add(typeof(TEvent), eventHandlers);
            }

            eventHandlers.Add(Handler);
            return ((Action) Remover).ToDisposable();
        }

        public IDisposable Subscribe<TEvent>(Action eventHandler) where TEvent : IEvent
        {
            if (eventHandler == null) throw new ArgumentNullException(nameof(eventHandler));
            Contract.EndContractBlock();

            List<Action<object>> eventHandlers;

            void Handler(object x) => eventHandler();
            void Remover() => eventHandlers.Remove(Handler);

            if (!handlers.TryGetValue(typeof(TEvent), out eventHandlers))
            {
                eventHandlers = new List<Action<object>>();
                handlers.Add(typeof(TEvent), eventHandlers);
            }

            eventHandlers.Add(Handler);
            return ((Action) Remover).ToDisposable();
        }

        public void Raise<TEvent, TEventArgs>(TEventArgs arguments) where TEvent : IEvent<TEventArgs>
        {
            if (handlers.TryGetValue(typeof(TEvent), out var eventHandlers))
            {
                eventHandlers.ForEach(x => x(arguments));
            }
        }

        public void Raise<TEvent>() where TEvent : IEvent
        {
            if (handlers.TryGetValue(typeof(TEvent), out var eventHandlers))
            {
                eventHandlers.ForEach(x => x(null));
            }
        }
    }
}