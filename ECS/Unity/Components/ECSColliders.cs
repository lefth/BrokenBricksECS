using System;
using UnityEngine;



namespace ECS
{
    [Serializable]
    public class ColliderComponent : IComponent, ICloneable
    {
        public Collider[] Colliders;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }



    [AddComponentMenu("ECS/Components/Colliders")]
    [RequireComponent(typeof(Collider))]
    [DisallowMultipleComponent]
    public class ECSColliders : ComponentWrapper<ColliderComponent>
    {
        private void Awake()
        {
            if (TypedComponent.Colliders == null) TypedComponent.Colliders = GetComponents<Collider>();
        }
    }
}
