using UnityEngine;
using System.Collections;

namespace Voxel2D{
	/// <summary>
	/// Holds data about the voxel
	/// </summary>
	public class VoxelData{
		
		int ID;
		
		public VoxelData(int ID){
			this.ID = ID;
		}
		
		public void SetID(int ID){
			this.ID = ID;
		}
		
		public int GetID(){
			return ID;
		}
	}
}