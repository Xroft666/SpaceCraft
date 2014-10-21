using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Voxel2D;

public class ShipController : VoxelData {

	private bool delete = false;

	private List<Engine> engineList = new List<Engine>();

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

		voxel.VoxelSystemUpdated += updated;
	}

	private void updated(VoxelSystem vox){
		if(voxel != null){
			findEngines();
		}
	}

	private void findEngines(){
		engineList.Clear();
		foreach(VoxelData e in voxel.GetVoxelData())
		{
			if(e is Engine)
			{
				engineList.Add((Engine)e);
			}
		}
	}

	public override void OnDelete()
	{
		if(voxel != null)
		{
			voxel.VoxelSystemUpdated -= updated;
		}
	}

	public override void OnUpdate()
	{
		if(delete)
		{
			voxel.RemoveVoxel(position.x,position.y);
		}


		if(Input.GetKey(KeyCode.W))
		{
			foreach(Engine e in engineList)
			{
				e.OnActivate();
			}			
		}
		else
		{
			foreach(Engine e in engineList)
			{
				e.OnDeactivate();
			}
			
		} 
	}
	
}
