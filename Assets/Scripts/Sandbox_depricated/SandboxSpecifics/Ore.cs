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
			//Debug.Log("TEST");
			VDU.stats.addThermalEnergy(stats.thermalTransfer);
			stats.removeThermalEnergy(stats.thermalTransfer);
		}
		if(VDD!=null){
			VDD.stats.addThermalEnergy(stats.thermalTransfer);
			stats.removeThermalEnergy(stats.thermalTransfer);
		}
		if(VDR!=null){
			VDR.stats.addThermalEnergy(stats.thermalTransfer);
			stats.removeThermalEnergy(stats.thermalTransfer);
		}
		if(VDL!=null){
			VDL.stats.addThermalEnergy(stats.thermalTransfer);
			stats.removeThermalEnergy(stats.thermalTransfer);
		}
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
}
