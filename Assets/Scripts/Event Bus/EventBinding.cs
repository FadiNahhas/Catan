using System;
using Event_Bus;

namespace Space_Station_Tycoon.Scripts.Event_System
{
    internal interface IEventBinding<T>
    {
        public Action<T> OnEvent { get; set; }
        public Action OnEventNoArgs { get; set; }
    }
    
    public class EventBinding<T> : IEventBinding<T> where T : IEvent
    {
        private Action<T> onEvent = _ => { };
        private Action onEventNoArgs = () => { };
        
        Action<T> IEventBinding<T>.OnEvent
        {
            get => onEvent;
            set => onEvent = value;
        }
        
        Action IEventBinding<T>.OnEventNoArgs
        {
            get => onEventNoArgs;
            set => onEventNoArgs = value;
        }

        public EventBinding(Action<T> on_event) => this.onEvent = on_event;
        public EventBinding(Action on_event_no_args) => this.onEventNoArgs = on_event_no_args;
        
        public void Add(Action on_event) => onEventNoArgs += on_event;
        public void Add(Action<T> on_event) => this.onEvent += on_event;
        
        public void Remove(Action on_event) => onEventNoArgs -= on_event;
        public void Remove(Action<T> on_event) => this.onEvent -= on_event;
    }
}