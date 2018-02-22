using ECS.Extensions;
using ECS.Injection;
using System;
using System.Linq;
using System.Reflection;



namespace ECS
{
    public static class InjectionManager
    {
        static readonly IocContainer _iocContainer = new IocContainer();

        static InjectionManager()
        {
            var assemblyArray = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblyArray.Length; i++)
            {
                var typeArray = assemblyArray[i].GetTypes();
                for (int j = 0; j < typeArray.Length; j++)
                {
                    var injectableDependencyAttributes = 
                        typeArray[j].GetCustomAttributes(typeof(InjectableDependencyAttribute), true) as InjectableDependencyAttribute[];
                    if (injectableDependencyAttributes.Any()) _iocContainer.Register(typeArray[j], typeArray[j], injectableDependencyAttributes[0].Lifetime);
                }
            }
        }

        public static void ResolveDependency(object obj)
        {
            var type = obj.GetType();
            var dependencyFields = type.GetFieldsRecursive(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
                .Where(field => field.GetCustomAttributes(typeof(InjectDependencyAttribute), true).Any());
            foreach (var field in dependencyFields) if (field.GetValue(obj) == null) field.SetValue(obj, _iocContainer.Resolve(field.FieldType));
        }


        public static object CreateObject(Type type)
        {
            if (!_iocContainer.IsRegistered(type)) _iocContainer.Register(type, type, LifeTime.PerInstance);
            var instance = _iocContainer.Resolve(type);
            ResolveDependency(instance);
            return instance;
        }

        public static T CreateObject<T>()
        {
            if (!_iocContainer.IsRegistered(typeof(T))) _iocContainer.Register<T, T>(LifeTime.PerInstance);
            var instance = _iocContainer.Resolve<T>();
            ResolveDependency(instance);
            return instance;
        }
    }
}
