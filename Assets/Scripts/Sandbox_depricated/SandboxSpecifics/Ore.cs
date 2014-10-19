using UnityEngine;
using System.Collections;
using SpaceSandbox;
using Voxel2D;

public class Ore :  VoxelData{

	public Ore(int elementID, Voxel2D.IntVector2 pos, int rotation):base(elementID,pos,rotation){
		deviceName = "Ore";
	}

	public override void OnStart(params object[] input){
		deviceName = "Ore";
	}
}
