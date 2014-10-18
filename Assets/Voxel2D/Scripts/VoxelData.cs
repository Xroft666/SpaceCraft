using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MaterialSystem;
using SpaceSandbox;

namespace Voxel2D{
	/// <summary>
	/// Holds data about the voxel
	/// </summary>
	public class VoxelData{

		public ElementStats stats;

		public List<Device> deviceList = new List<Device>();

		IntVector2 position;

		int ID;
		
		public VoxelData(int ID, IntVector2 pos){
			this.ID = ID;
			position = pos;
			stats = new ElementStats(ID);
		}
		
		public void SetID(int ID){
			this.ID = ID;
		}
		
		public int GetID(){
			return ID;
		}

		public IntVector2 GetPosition(){
			return position;
		}

		public void SetPosition(IntVector2 pos){
			position = pos;
		}

		public void OnDelete(){
			foreach(Device d in deviceList){
				d.OnDelete();
			}
		}
	}
}