using UnityEngine;
using System.Collections;
using Voxel2D;

namespace MaterialSystem{
	public static class PhysicsFormulas{
		
		public static float KineticEnergy(float mass, Vector2 velocity)
		{
			//real formula
			//return 0.5f*Mathf.Pow(velocity.magnitude,2)*mass;

			//linear
			return mass*velocity.magnitude;
		}

		/// <summary>
		/// Returns the new temperature of t1	/// </summary>
		/// <returns>The new temp.</returns>
		/// <param name="t1">T1.</param>
		/// <param name="t2">T2.</param>
		public static float ThermalConductivity(VoxelData t1, VoxelData t2){
			float finalTemp = (t1.stats.temperature*t1.stats.totalHeatCapacity+t2.stats.temperature*t2.stats.totalHeatCapacity)/
				(t1.stats.totalHeatCapacity+t2.stats.totalHeatCapacity);
			float rate = Mathf.Min(t1.stats.e.thermalConductivity,t2.stats.e.thermalConductivity);
			float tempThis = ((1-rate)*t1.stats.temperature+rate*finalTemp);
			
			return tempThis;
		}
		
	}
}
