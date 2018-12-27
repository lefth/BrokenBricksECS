﻿using UnityEngine;

namespace ECS {

	//use this tag to group systems for debugging purposes
	//[DebugSystemGroup("MyGroup")] 
	public class RotateObjectSystem : ComponentSystem {
		
		// this will inject all entities with components of type MyComponent 
		// if you define multiple ComponentArrays with the [InjectTuple] tag
		// you will only receive those entities with both Components
		[InjectTuple]
		ComponentArray<RotationSpeed> rotationSpeedArray = null;
        [InjectTuple]
        ComponentArray<TransformComponent> transformArray = null;


        // Use this for standard unity update function
        public override void OnFixedUpdate() {
            //for (int i = 1; i < transformArray.Length; i++) {
            //    Debug.Log((transformArray[i - 1] == transformArray[i]) + "  " +
            //    (transformArray[i - 1].transform == transformArray[i].transform));
            //}
            float dt = Time.fixedDeltaTime;
            for (int i = 0; i < transformArray.Length; i++) {
                transformArray[i].Transform.rotation *= Quaternion.AngleAxis(rotationSpeedArray[i].speed * dt, Vector3.up);
            }
        }
	}
}