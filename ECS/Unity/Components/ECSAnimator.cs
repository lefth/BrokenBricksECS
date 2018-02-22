using System;
using UnityEngine;



namespace ECS
{
    [Serializable]
    public class AnimatorComponent : IComponent, ICloneable
    {
        public Animator Animator;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }



    [AddComponentMenu("ECS/Components/Animator")]
    [RequireComponent(typeof(Animator))]
    [DisallowMultipleComponent]
    public class ECSAnimator : ComponentWrapper<AnimatorComponent>
    {
        private void Awake()
        {
            TypedComponent.Animator = gameObject.GetComponent<Animator>();
        }
    }
}
