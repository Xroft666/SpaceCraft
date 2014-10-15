using UnityEngine;
using System.Collections;
using MaterialSystem;

namespace Voxel2D{
	/// <summary>
	/// Holds data about the voxel
	/// </summary>
	public class VoxelData{

		public ElementStats stats;

		int ID;
		
		public VoxelData(int ID){
			this.ID = ID;
			stats = new ElementStats(ID);
		}
		
		public void SetID(int ID){
			this.ID = ID;
		}
		
		public int GetID(){
			return ID;
		}
	}
}