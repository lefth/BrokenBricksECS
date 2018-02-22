using System;
using UnityEngine;



namespace ECS
{
    [Serializable]
    public class TransformComponent : IComponent, ICloneable
    {
        public Transform Transform;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }



    [AddComponentMenu("ECS/Components/Transform")]
    [DisallowMultipleComponent]
    public class ECSTransform : ComponentWrapper<TransformComponent>
    {
        private void Awake()
        {
            TypedComponent.Transform = gameObject.transform;
        }
    }
}
