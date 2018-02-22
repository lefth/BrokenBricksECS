using UnityEngine;



namespace ECS
{
    public class ScriptBehaviour : MonoBehaviour
    {

        protected virtual void Awake()
        {
            InjectionManager.ResolveDependency(this);
        }
    }
}
