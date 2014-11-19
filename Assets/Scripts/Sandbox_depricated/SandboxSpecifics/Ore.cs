using UnityEngine;
using System.Collections;
using SpaceSandbox;
using Voxel2D;
using MaterialSystem;

public class Ore :  VoxelData{
	
	public Ore(int elementID, Voxel2D.IntVector2 pos, int rotation, VoxelSystem voxel):base(elementID,pos,rotation, voxel){
		stats.sizeModifier = 1;
		propertyList.Add(new ThermalConductor());
		propertyList.Add(new ThermalRadiator());
		propertyList.Add(new RadioActivity());

	}
	
}
