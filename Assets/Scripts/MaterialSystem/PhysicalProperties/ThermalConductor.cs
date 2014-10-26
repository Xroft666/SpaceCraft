using UnityEngine;
using System.Collections;

namespace MaterialSystem{
	public class ThermalConductor : PhysicalProperty {
		
		public override void OnUpdate (Voxel2D.VoxelData vox)
		{
			base.OnUpdate (vox);
			
			if(vox.VDU!=null){
				vox.stats.temperature = PhysicsFormulas.ThermalConductivity(vox,vox.VDU);
				vox.VDU.stats.temperature = PhysicsFormulas.ThermalConductivity(vox.VDU,vox);
			}
			if(vox.VDR!=null){
				vox.stats.temperature = PhysicsFormulas.ThermalConductivity(vox,vox.VDR);
				vox.VDR.stats.temperature = PhysicsFormulas.ThermalConductivity(vox.VDR,vox);
			}
		}
		
	}
}
