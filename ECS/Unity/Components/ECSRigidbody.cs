using System;
using UnityEngine;



namespace ECS
{
    [Serializable]
    public class RigidbodyComponent : IComponent, ICloneable
    {
        public Rigidbody Rigidbody;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }



    [AddComponentMenu("ECS/Components/Rigidbody")]
    [RequireComponent(typeof(Rigidbody))]
    [DisallowMultipleComponent]
    public class ECSRigidbody : ComponentWrapper<RigidbodyComponent>
    {
        private void Awake()
        {
            TypedComponent.Rigidbody = gameObject.GetComponent<Rigidbody>();
        }
    }
}
