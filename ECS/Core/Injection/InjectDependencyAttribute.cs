using System;



namespace ECS
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter)]
    public class InjectDependencyAttribute : Attribute { }
}
