using System;
using UnityEngine;



namespace ECS
{
    [Serializable]
    public class CharacterControllerComponent : IComponent, ICloneable
    {
        public CharacterController CharacterController;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }



    [AddComponentMenu("ECS/Components/CharacterController")]
    [RequireComponent(typeof(CharacterController))]
    [DisallowMultipleComponent]
    public class ECSCharacterController : ComponentWrapper<CharacterControllerComponent>
    {
        private void Awake()
        {
            TypedComponent.CharacterController = gameObject.GetComponent<CharacterController>();
        }
    }
}
