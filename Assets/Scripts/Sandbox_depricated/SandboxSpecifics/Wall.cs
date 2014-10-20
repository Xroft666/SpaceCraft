using UnityEngine;
using System.Collections;
using Voxel2D;

public class Wall : VoxelData {

	public Wall(int elementID, Voxel2D.IntVector2 pos, int rotation, VoxelSystem voxel):base(elementID,pos,rotation, voxel){
		stats.sizeModifier = 1;
	}
}
