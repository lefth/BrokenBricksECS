using System;
using UnityEngine;
#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using ECS.VisualDebugging;
#endif



namespace ECS
{
    [InjectableDependency(LifeTime.PerInstance)]
    public class UnityStandardSystemRoot : UnitySystemRoot<UnityEntityManager> { }

    public partial class UnitySystemRoot<TEntityManager> : SystemRoot<TEntityManager> where TEntityManager : UnityEntityManager
    {
        protected override void OnError(Exception ex)
        {
			Debug.LogException(ex);
        }
    }



#if (UNITY_EDITOR && ECS_DEBUG)
    [InjectableDependency(LifeTime.PerInstance)]
    public partial class UnitySystemRoot<TEntityManager> : SystemRoot<TEntityManager> where TEntityManager : UnityEntityManager
    {
        readonly Dictionary<string, DebugSystems> _componentSystemList = new Dictionary<string, DebugSystems>();
        DebugSystems _rootDebugSystems;

        public UnitySystemRoot()
        {
            _rootDebugSystems = new DebugSystems("Systems");
        }

        public override void AddSystem<TComponentSystem>()
        {
            var componentSystem = InjectionManager.CreateObject<TComponentSystem>();
            HandleTupleInjection(componentSystem);
            AddToDebugSystem(componentSystem);
        }

        public override void AddSystem(ComponentSystem system)
        {
            InjectionManager.ResolveDependency(system);
            HandleTupleInjection(system);
            AddToDebugSystem(system);
        }

        void AddToDebugSystem(ComponentSystem system)
        {
            var groupName = GetGroupNameFromSystem(system.GetType());
            if (string.IsNullOrEmpty(groupName))
            {
                _rootDebugSystems.AddSystem(system);
                return;
            }

            DebugSystems debugSystems;
            if (!_componentSystemList.TryGetValue(groupName, out debugSystems))
            {
                debugSystems = new DebugSystems(groupName);
                _rootDebugSystems.AddSystem(debugSystems);
                _componentSystemList.Add(groupName, debugSystems);
            }
            debugSystems.AddSystem(system);
        }

        static string GetGroupNameFromSystem(Type systemType)
        {

            DebugSystemGroupAttribute debugSystemGroupAttribute = systemType
                .GetCustomAttributes(typeof(DebugSystemGroupAttribute), false)
                .Cast<DebugSystemGroupAttribute>().FirstOrDefault();

            if (debugSystemGroupAttribute == null)
            {
                return "";
            }
            return debugSystemGroupAttribute.Group;
        }

        public override void RemoveSystem(ComponentSystem system)
        {
            DebugSystems debugSystems;
            var groupName = GetGroupNameFromSystem(system.GetType());
            if (_componentSystemList.TryGetValue(groupName, out debugSystems))
            {
                _rootDebugSystems.RemoveSystem(system);
                debugSystems.RemoveSystem(system);
                IComponentSystemSetup systemSetup = system;
                systemSetup.RemoveAllGroups();
            }
        }

        public override void Start()
        {
            _rootDebugSystems.OnStart();
        }

        public override void Update()
        {
            _rootDebugSystems.OnUpdate();
        }

        public override void FixedUpdate()
        {
            _rootDebugSystems.OnFixedUpdate();
        }
    }
#endif
}
