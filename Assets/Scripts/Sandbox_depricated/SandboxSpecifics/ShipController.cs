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

		List<Engine> leftEngines = new List<Engine>();
		List<Engine> rightEngines = new List<Engine>();
		List<Engine> allEngines = new List<Engine>();


		foreach(VoxelData e in voxel.GetVoxelData())
		{
			Voxel2D.IntVector2 pos = GetPosition();
			if(e is Engine)
			{
				Voxel2D.IntVector2 engPos = e.GetPosition();

				allEngines.Add((Engine)e);

				if( engPos.x < pos.x )
				{
					leftEngines.Add((Engine)e);
					continue;
				}
				if( engPos.x > pos.x )
				{
					rightEngines.Add((Engine)e);
					continue;
				}
			}
		}

		if(Input.GetKeyDown(KeyCode.W))
			foreach(Engine e in allEngines)
				e.OnActivate();

		if(Input.GetKeyUp(KeyCode.W))
			foreach(Engine e in allEngines)
				e.OnDeactivate();

		if(Input.GetKeyDown(KeyCode.A))
			foreach(Engine e in rightEngines)
				e.OnActivate();
		
		if(Input.GetKeyUp(KeyCode.A))
			foreach(Engine e in rightEngines)
				e.OnDeactivate();

		if(Input.GetKeyDown(KeyCode.D))
			foreach(Engine e in leftEngines)
				e.OnActivate();
		
		if(Input.GetKeyUp(KeyCode.D))
			foreach(Engine e in leftEngines)
				e.OnDeactivate();
	}
	
}
