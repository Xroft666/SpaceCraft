using UnityEngine;
using System.Collections;
using Voxel2D;
using MaterialSystem;

public class Floor : VoxelData {

	public Floor(int elementID, Voxel2D.IntVector2 pos, int rotation, VoxelSystem voxel):base(elementID,pos,rotation, voxel){
		stats.sizeModifier = 0.3f;
		propertyList.Add(new ThermalConductor());
		propertyList.Add(new ThermalRadiator());
		propertyList.Add(new RadioActivity());
	}

}
