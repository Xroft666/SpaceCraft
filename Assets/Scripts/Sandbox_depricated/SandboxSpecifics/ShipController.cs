using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Voxel2D;

public class ShipController : VoxelData {

	private bool delete = false;

	public ShipController(int elementID, Voxel2D.IntVector2 pos, int rotation, VoxelSystem voxel):base(elementID,pos,rotation, voxel){
		stats.sizeModifier = 1;
		
		foreach(VoxelData sc in voxel.GetVoxelData()){
			if(sc is ShipController){
				
				if(sc != this){
					Debug.LogWarning("Ship controller allready exists in this system, deleting");
					delete = true;
				}
			}
		}
	}
	
	private void findEngines(ref List<Engine> engineList){
		foreach(VoxelData e in voxel.GetVoxelData()){
			if(e is Engine){
				engineList.Add((Engine)e);
			}
		}
	}
	
	public override void OnUpdate(){
		if(delete){
			voxel.RemoveVoxel(position.x,position.y);
		}

		
		List<Engine> engineList = new List<Engine>();
		findEngines(ref engineList);
		if(Input.GetKey(KeyCode.W)){
			foreach(Engine e in engineList){
				e.OnActivate();
			}			
		}else{
			foreach(Engine e in engineList){
				e.OnDeactivate();
			}
			
		} 
	}
	
}
