using UnityEngine;
using System.Collections;

namespace MaterialSystem{
	public class RadioActivity : PhysicalProperty {
		
		public override void OnUpdate (Voxel2D.VoxelData vox)
		{
			base.OnUpdate (vox);

			if(vox.VDU!=null){
				vox.VDU.stats.addThermalEnergy(vox.stats.e.fisionRate);
			}
			if(vox.VDR!=null){
				vox.VDR.stats.addThermalEnergy(vox.stats.e.fisionRate);
			}
			
			if(vox.VDD!=null){
				vox.VDD.stats.addThermalEnergy(vox.stats.e.fisionRate);
			}
			if(vox.VDL!=null){
				vox.VDL.stats.addThermalEnergy(vox.stats.e.fisionRate);
			}
		}
	}
}