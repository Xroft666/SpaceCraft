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

		public Device device;

		IntVector2 position;

		int elementID;
		
		public VoxelData(int elementID, IntVector2 pos, Device device){
			this.elementID = elementID;
			position = pos;
			stats = new ElementStats(elementID);
			this.device = device;
		}
		
		public void SetElementID(int ID){
			this.elementID = ID;
		}
		
		public int GetElementID(){
			return elementID;
		}

		public IntVector2 GetPosition(){
			return position;
		}

		public void SetPosition(IntVector2 pos){
			position = pos;
		}

		public void OnDelete(){
			device.OnDelete();
		}
	}
}