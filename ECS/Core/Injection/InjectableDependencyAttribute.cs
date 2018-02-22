using System;



namespace ECS
{

    public enum LifeTime { Singleton, PerInstance }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
    public class InjectableDependencyAttribute : Attribute
    {
        public LifeTime Lifetime { get; private set; }

        public InjectableDependencyAttribute(LifeTime lifetime)
        {
            Lifetime = lifetime;
        }
    }
}