using System;
using UnityEngine;



namespace ECS
{
    [Serializable]
    public class CapsuleCollidersComponent : IComponent, ICloneable
    {
        public CapsuleCollider[] Colliders;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }



    [AddComponentMenu("ECS/Components/CapsuleColliders")]
    [RequireComponent(typeof(CapsuleCollider))]
    [DisallowMultipleComponent]
    public class ECSCapsuleColliders : ComponentWrapper<CapsuleCollidersComponent>
    {
        void Awake()
        {
            TypedComponent.Colliders = GetComponents<CapsuleCollider>();
        }
    }
}