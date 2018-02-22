using System;
using System.Collections.Generic;
using System.Linq;



namespace ECS.Injection
{
    public class IocContainer
    {
        public readonly Dictionary<Type, RegisteredObject> RegisteredObjects = new Dictionary<Type, RegisteredObject>();

        public void Register<TTypeToResolve, TConcrete>(LifeTime lifetime)
        {
            var resolveType = typeof(TTypeToResolve);
            RegisteredObjects.Add(resolveType, new RegisteredObject(resolveType, typeof(TConcrete), lifetime));
        }

        public void Register(Type typeToResolve, Type concreteType, LifeTime lifetime)
        {
            RegisteredObjects.Add(typeToResolve, new RegisteredObject(typeToResolve, concreteType, lifetime));
        }

        public bool IsRegistered(Type typeToResolve)
        {
            return RegisteredObjects.ContainsKey(typeToResolve);
        }

        public T Resolve<T>()
        {
            return (T)ResolveObject(typeof(T));
        }

        public object Resolve(Type type)
        {
            return ResolveObject(type);
        }

        object ResolveObject(Type typeToResolve)
        {
            RegisteredObject registeredObject;
            if (!RegisteredObjects.TryGetValue(typeToResolve, out registeredObject))
                throw new TypeNotRegisteredException(string.Format("The type {0} has not been registered", typeToResolve.Name));
            return GetInstance(registeredObject);
        }

        object GetInstance(RegisteredObject registeredObject)
        {
            if (registeredObject.Instance == null || registeredObject.Lifetime == LifeTime.PerInstance)
            {
                var parameters = ResolveConstructorParameters(registeredObject);
                registeredObject.CreateInstance(parameters.ToArray());
            }
            return registeredObject.Instance;
        }

        IEnumerable<object> ResolveConstructorParameters(RegisteredObject registeredObject)
        {
            var constructorInfo = registeredObject.ConcreteType.GetConstructors().First();
            foreach (var parameter in constructorInfo.GetParameters()) yield return ResolveObject(parameter.ParameterType);
            yield break;
        }
    }
}
