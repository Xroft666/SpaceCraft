using UnityEngine;
using System.Collections;
using SpaceSandbox;
using Voxel2D;

public class Ore :  VoxelData{
	
	VoxelData VDU;
	VoxelData VDD;
	VoxelData VDL;
	VoxelData VDR;
	
	public Ore(int elementID, Voxel2D.IntVector2 pos, int rotation, VoxelSystem voxel):base(elementID,pos,rotation, voxel){
		stats.sizeModifier = 1;
		

	}
	
	public override void OnUpdate(){




		if(VDU!=null){
			stats.temperature = calculateNewTemp(this,VDU);
			VDU.stats.temperature = calculateNewTemp(VDU,this);
		}
		if(VDR!=null){
			stats.temperature = calculateNewTemp(this,VDR);
			VDR.stats.temperature = calculateNewTemp(VDR,this);
		}

		/*if(VDD!=null){
			VDD.stats.addThermalEnergy(stats.thermalTransfer);
			stats.removeThermalEnergy(stats.thermalTransfer);
		}
		if(VDL!=null){
			VDL.stats.addThermalEnergy(stats.thermalTransfer);
			stats.removeThermalEnergy(stats.thermalTransfer);
		}*/
	}

	public override void OnNeighbourChange(){
		if(VoxelUtility.IsPointInBounds(voxel.GetGridSize(),new Vector2(position.x,position.y+1)) && !voxel.IsVoxelEmpty(position.x,position.y+1)){
			VDU = voxel.GetVoxel(position.x,position.y+1);
		} 
		if(VoxelUtility.IsPointInBounds(voxel.GetGridSize(),new Vector2(position.x,position.y-1)) && !voxel.IsVoxelEmpty(position.x,position.y-1)){
			VDD = voxel.GetVoxel(position.x,position.y-1);
		} 
		if(VoxelUtility.IsPointInBounds(voxel.GetGridSize(),new Vector2(position.x+1,position.y)) && !voxel.IsVoxelEmpty(position.x+1,position.y)){
			VDR = voxel.GetVoxel(position.x+1,position.y);
		} 
		if(VoxelUtility.IsPointInBounds(voxel.GetGridSize(),new Vector2(position.x-1,position.y)) && !voxel.IsVoxelEmpty(position.x-1,position.y)){
			VDL = voxel.GetVoxel(position.x-1,position.y);
		}
	}

	/// <summary>
	/// Returns the new temperature of t1	/// </summary>
	/// <returns>The new temp.</returns>
	/// <param name="t1">T1.</param>
	/// <param name="t2">T2.</param>
	private float calculateNewTemp(VoxelData t1, VoxelData t2){
		float finalTemp = (t1.stats.temperature*t1.stats.totalHeatCapacity+t2.stats.temperature*t2.stats.totalHeatCapacity)/
			(t1.stats.totalHeatCapacity+t2.stats.totalHeatCapacity);
		float rate = Mathf.Min(t1.stats.e.thermalConductivity,t2.stats.e.thermalConductivity);
		float tempThis = ((1-rate)*t1.stats.temperature+rate*finalTemp);

		return tempThis;
	}
}
