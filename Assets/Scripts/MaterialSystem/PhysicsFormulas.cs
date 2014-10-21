using UnityEngine;
using System.Collections;

namespace MaterialSystem{
	public static class PhysicsFormulas{
		
		public static float KineticEnergy(float mass, Vector2 velocity)
		{
			//real formula
			return 0.5f*Mathf.Pow(velocity.magnitude,2)*mass;

			//linear
			//return mass*velocity.magnitude;
		}
		
	}
}
