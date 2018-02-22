using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;



namespace ECS
{
    [AddComponentMenu("ECS/Entity")]
    public class GameObjectEntity : MonoBehaviour, IEntityRemovedEventListener, IComponentAddedToEntityEventListener, IComponentRemovedFromEntityEventListener
    {
#if !UNITY_EDITOR
        static readonly Type _editorOnlyAttributeType = typeof(EditorOnlyAttribute);
#endif
        public int Id;
        public bool AutoAddECSComponents = true;

        Entity _entity;
        EntityManager _entityManager;

        Dictionary<Type, Type> _cachedComponentWrapperType = new Dictionary<Type, Type>();
        Dictionary<Type, ComponentWrapper> _componentWrapperMap = new Dictionary<Type, ComponentWrapper>();

        public bool IsInitialized { get; private set; }
        public Entity Entity { get { return _entity; } }
        public EntityManager EntityManager { get { return _entityManager; } }

        public UnityEvent OnInitialized;

        Dictionary<Type, ComponentWrapper> _componentsToDelete = new Dictionary<Type, ComponentWrapper>();

        void AddECSComponents()
        {
            if (!AutoAddECSComponents || IsInitialized) return;

            if (!gameObject.GetComponent<ECSTransform>())
            {
                var ecsTransform = gameObject.AddComponent<ECSTransform>();
                ecsTransform.hideFlags = HideFlags.HideInInspector;
            }

            if (gameObject.GetComponent<Rigidbody>() && !gameObject.GetComponent<ECSRigidbody>())
            {
                var ecsRigidbody = gameObject.AddComponent<ECSRigidbody>();
                ecsRigidbody.hideFlags = HideFlags.HideInInspector;
            }

            if (gameObject.GetComponent<Animator>() && !gameObject.GetComponent<ECSAnimator>())
            {
                var ecsAnimator = gameObject.AddComponent<ECSAnimator>();
                ecsAnimator.hideFlags = HideFlags.HideInInspector;
            }

            if (gameObject.GetComponent<Collider>() && !gameObject.GetComponent<ECSColliders>())
            {
                var ecsColliders = gameObject.AddComponent<ECSColliders>();
                ecsColliders.hideFlags = HideFlags.HideInInspector;
            }

            if (gameObject.GetComponent<CapsuleCollider>() && !gameObject.GetComponent<ECSCapsuleColliders>())
            {
                var ecsCapsuleColliders = gameObject.AddComponent<ECSCapsuleColliders>();
                ecsCapsuleColliders.hideFlags = HideFlags.HideInInspector;
            }

            if (gameObject.GetComponent<CharacterController>() && !gameObject.GetComponent<ECSCharacterController>())
            {
                var ecsCharacterController = gameObject.AddComponent<ECSCharacterController>();
                ecsCharacterController.hideFlags = HideFlags.HideInInspector;
            }
        }

        public void SetEntity(Entity entity, EntityManager entityManager)
        {
            if (IsInitialized)
            {
                Debug.LogError(name + ": is already initialized by entity");
                return;
            }

            Id = entity.Id;
            _componentWrapperMap.Clear();
            _entity = entity;
            _entityManager = entityManager;

            entityManager.SubscribeOnEntityRemoved(_entity, this);
            entityManager.SubscribeOnComponentAddedToEntity(entity, this);
            entityManager.SubscribeOnComponentRemovedFromEntity(entity, this);

            AddECSComponents();

            ComponentWrapper[] componentWrapper = GetComponents();
            for (int i = 0; i < componentWrapper.Length; i++)
            {
                _componentWrapperMap.Add(componentWrapper[i].ComponentType, componentWrapper[i]);
                componentWrapper[i].Initialize(entity, entityManager);
            }

            IsInitialized = true;
            if (OnInitialized != null) OnInitialized.Invoke();
        }

        public void OnEntityRemoved(object sender, Entity entity)
        {
            Destroy(gameObject);
        }

        public void OnComponentAddedToEntity<TComponent>(object sender, Entity entity, TComponent component)
        {
            Type componentType = typeof(TComponent);
            if (_componentWrapperMap.ContainsKey(componentType))
            {
                if (_componentsToDelete.ContainsKey(componentType)) _componentsToDelete.Remove(componentType);
                return;
            }

            Type componentWrapperType = null;
            if (_cachedComponentWrapperType.ContainsKey(componentType))
            {
                componentWrapperType = _cachedComponentWrapperType[componentType];
            }
            else
            {
                componentWrapperType = componentType.Assembly.GetTypes()
                .Where(field => field.BaseType != null && field.BaseType.GetGenericArguments().Where(genericArgType => genericArgType == componentType).Any())
                .FirstOrDefault();
                _cachedComponentWrapperType.Add(componentType, componentWrapperType);
            }

            if (componentWrapperType != null)
            {

#if !UNITY_EDITOR
               if (componentWrapperType.GetCustomAttributes(_editorOnlyAttributeType, true).Any()) return;
#endif
                var componentWrapper = (ComponentWrapper)gameObject.AddComponent(componentWrapperType);
                _componentWrapperMap.Add(componentType, componentWrapper);
            }
        }

        public void OnComponentRemovedFromEntity(object sender, Entity entity, Type componentType)
        {
            ComponentWrapper componentWrapper;
            if (_componentWrapperMap.TryGetValue(componentType, out componentWrapper))
            {
                if (componentWrapper.enabled && !_componentsToDelete.ContainsKey(componentType))
                {
                    _componentsToDelete.Add(componentType, componentWrapper);
                }
            }
        }

        private void LateUpdate()
        {
            foreach (KeyValuePair<Type, ComponentWrapper> pair in _componentsToDelete)
            {
                Destroy(pair.Value);
                _componentWrapperMap.Remove(pair.Key);
            }
            _componentsToDelete.Clear();
        }

        private void OnDestroy()
        {
            if (_entityManager != null)
            {
                _entityManager.DestroyEntity(_entity);

                _entityManager.UnsubscribeOnEntityRemoved(_entity, (IEntityRemovedEventListener)this);
                _entityManager.UnsubscribeOnComponentAddedToEntity(_entity, this);
                _entityManager.UnsubscribeOnComponentRemovedFromEntity(_entity, this);
            }
        }



        public ComponentWrapper[] GetComponents()
        {
            ComponentWrapper[] componentWrapper = GetComponents<ComponentWrapper>();
            return componentWrapper;
        }


        public void SetComponentToEntityManager<TComponent>(TComponent component) where TComponent : struct, IComponent
        {
            if (_entityManager != null)
            {
                _entityManager.SetComponent(_entity, component);
            }
        }

        public TComponent GetComponentFromEntityManager<TComponent>() where TComponent : IComponent
        {
            return _entityManager.GetComponent<TComponent>(_entity);
        }
    }
}
