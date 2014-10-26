using UnityEngine;
using System.Collections;

namespace MaterialSystem{
	public class ThermalRadiator : PhysicalProperty {
		
		public override void OnUpdate (Voxel2D.VoxelData vox)
		{
			base.OnUpdate (vox);

			if(vox.VDU==null){
				vox.stats.removeThermalEnergy(vox.stats.thermalRadiation);
			}
			if(vox.VDR==null){
				vox.stats.removeThermalEnergy(vox.stats.thermalRadiation);
			}
			
			if(vox.VDD==null){
				vox.stats.removeThermalEnergy(vox.stats.thermalRadiation);
			}
			if(vox.VDL==null){
				vox.stats.removeThermalEnergy(vox.stats.thermalRadiation);
			}
		}
	}
}