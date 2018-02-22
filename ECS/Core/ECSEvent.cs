using System;
using System.Collections.Generic;



namespace ECS
{
    public class ECSEvent<T>
    {
        List<Action<T>> _eventListeners = new List<Action<T>>();

        public void Subscribe(Action<T> eventListener)
        {
            if (!_eventListeners.Contains(eventListener)) _eventListeners.Add(eventListener);
        }

        public void Unsubscribe(Action<T> eventListener)
        {
            _eventListeners.Remove(eventListener);
        }

        public void CallEvent(T eventArg)
        {
            for (var i = 0; i < _eventListeners.Count; i++) _eventListeners[i].Invoke(eventArg);
        }
    }



    class EntityAddedEvent
    {
        List<IEntityAddedEventListener> _eventListeners = new List<IEntityAddedEventListener>();

        public void Subscribe(IEntityAddedEventListener eventListener)
        {
            _eventListeners.Add(eventListener);
        }

        public void Unsubscribe(IEntityAddedEventListener eventListener)
        {
            _eventListeners.Remove(eventListener);
        }

        public void CallEvent(object sender, ref Entity entity)
        {
            for (var i = 0; i < _eventListeners.Count; i++) _eventListeners[i].OnEntityAdded(sender, entity);
        }
    }



    class EntityRemovingEvent
    {
        Dictionary<Entity, List<IEntityRemovingEventListener>> _eventListenerMap = new Dictionary<Entity, List<IEntityRemovingEventListener>>();
        List<IEntityRemovingEventListener> _eventListeners = new List<IEntityRemovingEventListener>();

        public void Subscribe(ref Entity entity, IEntityRemovingEventListener eventListener)
        {
            List<IEntityRemovingEventListener> eventListeners;
            if (!_eventListenerMap.TryGetValue(entity, out eventListeners))
            {
                eventListeners = new List<IEntityRemovingEventListener>();
                _eventListenerMap.Add(entity, eventListeners);
            }
            eventListeners.Add(eventListener);
        }

        public void Unsubscribe(ref Entity entity, IEntityRemovingEventListener eventListener)
        {
            List<IEntityRemovingEventListener> eventListeners;
            if (_eventListenerMap.TryGetValue(entity, out eventListeners))
            {
                eventListeners.Remove(eventListener);
                if (eventListeners.Count == 0) _eventListenerMap.Remove(entity);
            }
        }

        public void Subscribe(IEntityRemovingEventListener eventListener)
        {
            _eventListeners.Add(eventListener);
        }

        public void Unsubscribe(IEntityRemovingEventListener eventListener)
        {
            _eventListeners.Remove(eventListener);
        }

        public void CallEvent(object sender, ref Entity entity)
        {
            List<IEntityRemovingEventListener> mapEventListeners;
            if (_eventListenerMap.TryGetValue(entity, out mapEventListeners))
                for (int i = 0; i < mapEventListeners.Count; i++) mapEventListeners[i].OnEntityRemoving(sender, entity);

            for (int i = 0; i < _eventListeners.Count; i++) _eventListeners[i].OnEntityRemoving(sender, entity);
        }
    }



    class EntityRemovedEvent
    {
        Dictionary<Entity, List<IEntityRemovedEventListener>> _eventListenerMap = new Dictionary<Entity, List<IEntityRemovedEventListener>>();
        List<IEntityRemovedEventListener> _eventListeners = new List<IEntityRemovedEventListener>();

        public void Subscribe(ref Entity entity, IEntityRemovedEventListener eventListener)
        {
            List<IEntityRemovedEventListener> eventListeners;
            if (!_eventListenerMap.TryGetValue(entity, out eventListeners))
            {
                eventListeners = new List<IEntityRemovedEventListener>();
                _eventListenerMap.Add(entity, eventListeners);
            }
            eventListeners.Add(eventListener);
        }

        public void Unsubscribe(ref Entity entity, IEntityRemovedEventListener eventListener)
        {
            List<IEntityRemovedEventListener> eventListeners;
            if (_eventListenerMap.TryGetValue(entity, out eventListeners))
            {
                eventListeners.Remove(eventListener);
                if (eventListeners.Count == 0) _eventListenerMap.Remove(entity);
            }
        }

        public void Subscribe(IEntityRemovedEventListener eventListener)
        {
            _eventListeners.Add(eventListener);
        }

        public void Unsubscribe(IEntityRemovedEventListener eventListener)
        {
            _eventListeners.Remove(eventListener);
        }

        public void CallEvent(object sender, ref Entity entity)
        {
            List<IEntityRemovedEventListener> mapEventListeners;
            if (_eventListenerMap.TryGetValue(entity, out mapEventListeners))
                for (var i = 0; i < mapEventListeners.Count; i++) mapEventListeners[i].OnEntityRemoved(sender, entity);

            for (var i = 0; i < _eventListeners.Count; i++) _eventListeners[i].OnEntityRemoved(sender, entity);
        }

    }



    public abstract class ECSComponentEvent<TComponentEventListener>
    {
        protected Dictionary<Entity, List<TComponentEventListener>> EventListenerMap = new Dictionary<Entity, List<TComponentEventListener>>();
        protected List<TComponentEventListener> EventListeners = new List<TComponentEventListener>();

        public void Subscribe(ref Entity entity, TComponentEventListener eventListener)
        {
            List<TComponentEventListener> eventListeners;
            if (!EventListenerMap.TryGetValue(entity, out eventListeners))
            {
                eventListeners = new List<TComponentEventListener>();
                EventListenerMap.Add(entity, eventListeners);
            }
            eventListeners.Add(eventListener);
        }

        public void Unsubscribe(ref Entity entity, TComponentEventListener eventListener)
        {
            List<TComponentEventListener> eventListeners;
            if (EventListenerMap.TryGetValue(entity, out eventListeners))
            {
                eventListeners.Remove(eventListener);
                if (eventListeners.Count == 0) EventListenerMap.Remove(entity);
            }
        }

        public void Subscribe(TComponentEventListener eventListener)
        {
            EventListeners.Add(eventListener);
        }

        public void Unsubscribe(TComponentEventListener eventListener)
        {
            EventListeners.Remove(eventListener);
        }

        public void RemoveEntityFromEvent(ref Entity entity)
        {
            EventListenerMap.Remove(entity);
        }

    }



    public class ComponentAddedToEntityEvent : ECSComponentEvent<IComponentAddedToEntityEventListener>
    {
        public void CallEvent<TComponent>(object sender, ref Entity entity, ref TComponent component)
        {
            for (int i = 0; i < EventListeners.Count; i++) EventListeners[i].OnComponentAddedToEntity(sender, entity, component);

            List<IComponentAddedToEntityEventListener> entityEventListeners;
            if (EventListenerMap.TryGetValue(entity, out entityEventListeners))
                for (int i = 0; i < entityEventListeners.Count; i++) entityEventListeners[i].OnComponentAddedToEntity(sender, entity, component);
        }
    }



    public class ComponentRemovingFromEntityEvent : ECSComponentEvent<IComponentRemovingFromEntityEventListener>
    {
        public void CallEvent<TComponent>(object sender, ref Entity entity, ref TComponent component)
        {
            for (int i = 0; i < EventListeners.Count; i++) EventListeners[i].OnComponentRemovingFromEntity(sender, entity, component);

            List<IComponentRemovingFromEntityEventListener> entityEventListeners;
            if (EventListenerMap.TryGetValue(entity, out entityEventListeners))
                for (int i = 0; i < entityEventListeners.Count; i++) entityEventListeners[i].OnComponentRemovingFromEntity(sender, entity, component);
        }
    }



    public class ComponentRemovedFromEntityEvent : ECSComponentEvent<IComponentRemovedFromEntityEventListener>
    {
        public void CallEvent(object sender, ref Entity entity, Type componentType)
        {
            for (int i = 0; i < EventListeners.Count; i++) EventListeners[i].OnComponentRemovedFromEntity(sender, entity, componentType);

            List<IComponentRemovedFromEntityEventListener> entityEventListeners;
            if (EventListenerMap.TryGetValue(entity, out entityEventListeners))
                for (int i = 0; i < entityEventListeners.Count; i++) entityEventListeners[i].OnComponentRemovedFromEntity(sender, entity, componentType);
        }
    }



    public class ComponentChangedOfEntityEvent : ECSComponentEvent<IComponentChangedOfEntityEventListener>
    {
        public void CallEvent<TComponent>(object sender, ref Entity entity, ref TComponent component)
        {
            for (int i = 0; i < EventListeners.Count; i++) EventListeners[i].OnComponentChangedOfEntity(sender, entity, component);

            List<IComponentChangedOfEntityEventListener> entityEventListeners;
            if (EventListenerMap.TryGetValue(entity, out entityEventListeners))
                for (int i = 0; i < entityEventListeners.Count; i++) entityEventListeners[i].OnComponentChangedOfEntity(sender, entity, component);
        }
    }



    public class ComponentAddedToEntityEvent<TComponent> : ECSComponentEvent<IComponentAddedToEntityEventListener<TComponent>> where TComponent : IComponent
    {
        public void CallEvent(object sender, ref Entity entity, ref TComponent component)
        {
            for (int i = 0; i < EventListeners.Count; i++) EventListeners[i].OnEntityAdded(sender, entity, component);

            List<IComponentAddedToEntityEventListener<TComponent>> entityEventListeners;
            if (EventListenerMap.TryGetValue(entity, out entityEventListeners))
                for (int i = 0; i < entityEventListeners.Count; i++) entityEventListeners[i].OnEntityAdded(sender, entity, component);
        }
    }



    public class ComponentRemovingFromEntityEvent<TComponent> : ECSComponentEvent<IComponentRemovingFromEntityEventListener<TComponent>> where TComponent : IComponent
    {
        public void CallEvent(object sender, ref Entity entity, ref TComponent component)
        {
            for (int i = 0; i < EventListeners.Count; i++) EventListeners[i].OnEntityRemoved(sender, entity, component);

            List<IComponentRemovingFromEntityEventListener<TComponent>> entityEventListeners;
            if (EventListenerMap.TryGetValue(entity, out entityEventListeners))
                for (int i = 0; i < entityEventListeners.Count; i++) entityEventListeners[i].OnEntityRemoved(sender, entity, component);
        }
    }



    public class ComponentRemovedFromEntityEvent<TComponent> : ECSComponentEvent<IComponentRemovedFromEntityEventListener<TComponent>> where TComponent : IComponent
    {
        public void CallEvent(object sender, ref Entity entity)
        {
            for (int i = 0; i < EventListeners.Count; i++) EventListeners[i].OnEntityRemoved(sender, entity);

            List<IComponentRemovedFromEntityEventListener<TComponent>> entityEventListeners;
            if (EventListenerMap.TryGetValue(entity, out entityEventListeners))
                for (int i = 0; i < entityEventListeners.Count; i++) entityEventListeners[i].OnEntityRemoved(sender, entity);
        }
    }



    public class ComponentChangedOfEntityEvent<TComponent> : ECSComponentEvent<IComponentChangedOfEntityEventListener<TComponent>> where TComponent : IComponent
    {
        public void CallEvent(object sender, ref Entity entity, ref TComponent component)
        {
            for (int i = 0; i < EventListeners.Count; i++) EventListeners[i].OnComponentChanged(sender, entity, component);

            List<IComponentChangedOfEntityEventListener<TComponent>> entityEventListeners;
            if (EventListenerMap.TryGetValue(entity, out entityEventListeners))
                for (int i = 0; i < entityEventListeners.Count; i++) entityEventListeners[i].OnComponentChanged(sender, entity, component);
        }
    }
}
