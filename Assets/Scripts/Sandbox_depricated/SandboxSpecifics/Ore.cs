using UnityEngine;
using System.Collections;
using SpaceSandbox;
using Voxel2D;

public class Ore :  VoxelData{

	public Ore(int elementID, Voxel2D.IntVector2 pos, int rotation, VoxelSystem voxel):base(elementID,pos,rotation, voxel){
		deviceName = "Ore";
	}


}
