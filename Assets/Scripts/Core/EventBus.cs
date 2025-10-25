using System;
using System.Collections.Generic;

namespace Core
{
    public static class EventBus
    {
        private static readonly Dictionary<Type, List<Delegate>> _events = new();

        // Subscribe with typed event handler
        public static void Subscribe<T>(Action<T> listener)
        {
            if (!_events.ContainsKey(typeof(T)))
                _events[typeof(T)] = new List<Delegate>();

            _events[typeof(T)].Add(listener);
        }

        // Subscribe with generic object handler
        public static void Subscribe<T>(Action<object> listener)
        {
            Action<T> wrapper = e => listener(e);
            Subscribe(wrapper);
        }

        // Unsubscribe (works for both forms)
        public static void Unsubscribe<T>(Action<T> listener)
        {
            if (_events.TryGetValue(typeof(T), out var list))
                list.Remove(listener);
        }

        public static void Unsubscribe<T>(Action<object> listener)
        {
            if (_events.ContainsKey(typeof(T)))
                _events.Remove(typeof(T));
        }

        // Publish event to all listeners
        public static void Publish<T>(T publishedEvent)
        {
            if (_events.TryGetValue(typeof(T), out var list))
            {
                foreach (var del in list)
                {
                    if (del is Action<T> action)
                        action.Invoke(publishedEvent);
                }
            }
        }
    }
}
